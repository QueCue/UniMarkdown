using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// Markdown解析器，负责将Markdown文本解析为MarkdownElement列表
    /// </summary>
    public static class MarkdownParser
    {
        // 预编译正则表达式避免重复编译
        private static readonly Regex g_headerRegex
            = new Regex(@"^(#{1,6})\s+(.+)$", RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex
            g_codeBlockRegex = new Regex(@"```([\s\S]*?)```", RegexOptions.Compiled);

        private static readonly Regex g_inlineCodeRegex = new Regex(@"`([^`]+)`", RegexOptions.Compiled);

        private static readonly Regex g_linkRegex
            = new Regex(@"\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);

        private static readonly Regex g_imageRegex
            = new Regex(@"!\[([^\]]*)\]\(([^)]+)\)", RegexOptions.Compiled);

        // 支持图片尺寸的扩展语法正则表达式
        private static readonly Regex g_imageSizeRegex
            = new Regex(@"!\[([^\]]*)\]\(([^)]+)\s*=(\d+)x(\d+)\)",
                RegexOptions.Compiled); // ![alt](url =300x200)

        private static readonly Regex g_imageWidthRegex
            = new Regex(@"!\[([^\]]*)\]\(([^)]+)\s*=(\d+)x\)", RegexOptions.Compiled); // ![alt](url =300x)

        private static readonly Regex g_imageHeightRegex
            = new Regex(@"!\[([^\]]*)\]\(([^)]+)\s*=x(\d+)\)", RegexOptions.Compiled); // ![alt](url =x200)

        private static readonly Regex g_imageAttributeRegex
            = new Regex(@"!\[([^\]]*)\]\(([^)]+)\)\{([^}]+)\}",
                RegexOptions.Compiled); // ![alt](url){width=300 height=200}

        // 粗体正则：严格的空格规则 - 开头结尾不能是空格，避免 ** 文本 ** 被解析
        private static readonly Regex g_boldRegex
            = new Regex(@"\*\*([^\s\*](?:[^\*]*[^\s\*])?)\*\*", RegexOptions.Compiled);

        // 斜体正则：简化版本，匹配单个*包围的内容，依靠重叠检测避免冲突
        private static readonly Regex g_italicRegex
            = new Regex(@"\*([^\s\*][^\*]*[^\s\*]|\w)\*", RegexOptions.Compiled);

        // 粗斜体正则：匹配 ***文本*** 格式，支持单字符，严格空格规则
        private static readonly Regex g_boldItalicRegex
            = new Regex(@"\*\*\*((?:[^\s\*](?:[^\*]*[^\s\*])?)|(?:\w))\*\*\*", RegexOptions.Compiled);

        private static readonly Regex g_unorderedListRegex = new Regex(@"^(\s*)[-\*\+]\s+(.+)$",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex g_orderedListRegex
            = new Regex(@"^(\s*)\d+\.\s+(.+)$", RegexOptions.Multiline | RegexOptions.Compiled);

        // 任务列表正则：匹配 "- [ ]" 或 "- [x]" 格式的待办事项
        private static readonly Regex g_taskListRegex = new Regex(@"^(\s*)[-\*\+]\s+\[([ xX])\]\s+(.+)$",
            RegexOptions.Multiline | RegexOptions.Compiled);

        // 分割线正则：匹配3个或更多的 -、* 或 _ 字符
        private static readonly Regex g_divideRegex = new Regex(@"^[\s]*(-{3,}|\*{3,}|_{3,})[\s]*$",
            RegexOptions.Multiline | RegexOptions.Compiled);

        // 表格行正则：匹配以|分隔的表格行
        private static readonly Regex g_tableRowRegex = new Regex(@"^\s*\|(.+)\|\s*$",
            RegexOptions.Compiled);

        // 表格分隔行正则：匹配表格标题与内容的分隔行
        private static readonly Regex g_tableSeparatorRegex = new Regex(@"^\s*\|(\s*:?-+:?\s*\|)+\s*$",
            RegexOptions.Compiled);

        // 复用StringBuilder减少内存分配
        private static readonly StringBuilder g_stringBuilder = new StringBuilder(1024);

        /// <summary>
        /// 解析Markdown文本为元素列表
        /// </summary>
        /// <param name="markdownText">要解析的Markdown文本</param>
        /// <param name="result">用于接收结果的列表，如果为null则创建新列表</param>
        /// <returns>解析后的MarkdownElement列表</returns>
        public static void ParseMarkdown(string markdownText, List<MarkdownElement> result = null)
        {
            if (string.IsNullOrEmpty(markdownText))
            {
                return;
            }

            if (result == null)
            {
                result = new List<MarkdownElement>();
            }
            else
            {
                // 清理之前的元素到对象池
                for (var i = 0; i < result.Count; i++)
                {
                    MarkdownElement.ReturnToPool(result[i]);
                }

                result.Clear();
            }

            try
            {
                ParseMarkdownInternal(markdownText, result);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MarkdownParser] Error parsing Markdown: {ex.Message}");
            }
        }

        /// <summary>
        /// 解析内联元素（链接、图片、代码、粗体、斜体等）
        /// </summary>
        /// <param name="text">要解析的文本</param>
        /// <param name="result">用于接收结果的列表，如果为null则创建新列表</param>
        /// <returns>解析后的MarkdownElement列表</returns>
        public static List<MarkdownElement> ParseInlineElementsOnly(string text,
            List<MarkdownElement> result = null)
        {
            if (result == null)
            {
                result = new List<MarkdownElement>();
            }
            else
            {
                // 清理之前的元素到对象池
                for (var i = 0; i < result.Count; i++)
                {
                    MarkdownElement.ReturnToPool(result[i]);
                }

                result.Clear();
            }

            if (!string.IsNullOrEmpty(text))
            {
                ParseInlineElements(text, result);
            }

            return result;
        }

        /// <summary>
        /// 内部解析实现
        /// </summary>
        /// <param name="markdownText">Markdown文本</param>
        /// <param name="result">结果列表</param>
        private static void ParseMarkdownInternal(string markdownText, List<MarkdownElement> result)
        {
            // 按行分割文本 - 使用RemoveEmptyEntries避免空行导致的问题
            string[] lines = markdownText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var inCodeBlock = false;
            var codeBlockLanguage = ""; // 存储代码块语言标识符
            var orderedListCounters = new Dictionary<int, int>(); // 多级有序列表计数器
            var codeBlockIndent = 0; // 代码块缩进层级
            g_stringBuilder.Clear();

            for (var i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // 处理代码块（支持缩进的代码块）
                string trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("```"))
                {
                    if (inCodeBlock)
                    {
                        // 代码块结束 
                        MarkdownElement codeElement = MarkdownElement.GetFromPool();
                        codeElement.SetCodeBlock(g_stringBuilder.ToString(),
                            codeBlockLanguage,
                            codeBlockIndent);

                        result.Add(codeElement);
                        g_stringBuilder.Clear();
                        codeBlockLanguage = ""; // 重置语言标识符
                        codeBlockIndent = 0; // 重置缩进层级
                        inCodeBlock = false;
                    }
                    else
                    {
                        // 代码块开始，提取语言标识符和缩进层级
                        inCodeBlock = true;
                        // 提取语言标识符（```后面的文本）
                        codeBlockLanguage = trimmedLine.Length > 3 ? trimmedLine.Substring(3).Trim() : "";
                        // 计算缩进层级（开始行的前导空格数）
                        codeBlockIndent = line.Length - line.TrimStart().Length;
                    }

                    continue;
                }

                if (inCodeBlock)
                {
                    // 正确处理代码块中的换行和空行
                    if (g_stringBuilder.Length > 0)
                    {
                        g_stringBuilder.Append('\n');
                    }

                    // 去掉与代码块开始行相同的缩进，保持相对缩进
                    string codeBlockLine = line;
                    if (codeBlockIndent > 0 &&
                        line.Length > codeBlockIndent)
                    {
                        // 检查前导空格是否与代码块开始行的缩进一致
                        var canTrimIndent = true;
                        for (var k = 0; k < codeBlockIndent && k < line.Length; k++)
                        {
                            if (line[k] != ' ')
                            {
                                canTrimIndent = false;
                                break;
                            }
                        }

                        if (canTrimIndent)
                        {
                            codeBlockLine = line.Substring(codeBlockIndent);
                        }
                    }

                    g_stringBuilder.Append(codeBlockLine);
                    continue;
                }

                // 处理标题
                if (TryParseHeader(line, out string headerText, out int level))
                {
                    // 重置有序列表序号（遇到标题时）
                    orderedListCounters.Clear();
                    MarkdownElement headerElement = MarkdownElement.GetFromPool();
                    headerElement.SetHeader(headerText, level);
                    result.Add(headerElement);
                    continue;
                }

                // 处理分割线
                if (TryParseDivide(line))
                {
                    // 重置有序列表序号（遇到分割线时）
                    orderedListCounters.Clear();
                    MarkdownElement divideElement = MarkdownElement.GetFromPool();
                    divideElement.SetDivide();
                    result.Add(divideElement);
                    continue;
                }

                // 处理任务列表（待办事项）- 必须在无序列表之前解析
                if (TryParseTaskList(line,
                    out string taskText,
                    out bool isCompleted,
                    out int taskNestingLevel))
                {
                    // 重置更深层级的有序列表序号
                    ResetDeeperOrderedCounters(orderedListCounters, taskNestingLevel);

                    MarkdownElement taskElement = MarkdownElement.GetFromPool();
                    taskElement.SetTaskList(taskText, isCompleted, taskNestingLevel);
                    result.Add(taskElement);
                    continue;
                }

                // 处理无序列表
                if (TryParseUnorderedList(line, out string listText, out int nestingLevel))
                {
                    // 重置更深层级的有序列表序号
                    ResetDeeperOrderedCounters(orderedListCounters, nestingLevel);

                    MarkdownElement listElement = MarkdownElement.GetFromPool();
                    listElement.SetListItem(listText, false, 0, nestingLevel);
                    result.Add(listElement);
                    continue;
                }

                // 处理有序列表
                if (TryParseOrderedList(line, out string orderedListText, out int orderedNestingLevel))
                {
                    // 获取或初始化该层级的计数器
                    orderedListCounters.TryAdd(orderedNestingLevel, 0);
                    orderedListCounters[orderedNestingLevel]++;

                    // 重置更深层级的有序列表序号
                    ResetDeeperOrderedCounters(orderedListCounters, orderedNestingLevel);

                    MarkdownElement listElement = MarkdownElement.GetFromPool();
                    listElement.SetListItem(orderedListText,
                        true,
                        orderedListCounters[orderedNestingLevel],
                        orderedNestingLevel);

                    result.Add(listElement);
                    continue;
                }

                // 处理表格 - 检查当前行是否为表格行
                if (TryParseTable(lines, i, out var table, out int consumedLines))
                {
                    // 重置有序列表序号（遇到表格时）
                    orderedListCounters.Clear();
                    result.Add(table);
                    i += consumedLines - 1; // 跳过已处理的行（-1是因为for循环会自动+1）
                    continue;
                }

                // 处理空行 - 创建段落分隔
                if (string.IsNullOrWhiteSpace(line))
                {
                    // 空行不应该重置有序列表序号，只有遇到非列表内容才重置
                    // 这样可以保持列表项目在空行分隔后仍然正确编号

                    // 空行产生段落分隔（硬换行）
                    if (result.Count > 0 &&
                        result[^1].elementType != MarkdownElementType.LineBreak &&
                        result[^1].elementType != MarkdownElementType.SoftLineBreak)
                    {
                        MarkdownElement lineBreakElement = MarkdownElement.GetFromPool();
                        lineBreakElement.elementType = MarkdownElementType.LineBreak;
                        lineBreakElement.content = "";
                        result.Add(lineBreakElement);
                    }

                    continue;
                }

                // 处理普通文本行（包含内联元素）
                // 遇到普通文本行时重置有序列表序号
                orderedListCounters.Clear();

                // 记录解析前的元素数量
                int elementCountBefore = result.Count;

                // 检查行末是否有两个空格（软换行）
                bool hasSoftLineBreak = line.EndsWith("  ");
                string processedLine = hasSoftLineBreak ? line.Substring(0, line.Length - 2) : line;

                ParseInlineElements(processedLine, result);

                // 根据Markdown规则处理换行
                if (result.Count > elementCountBefore &&
                    i < lines.Length - 1)
                {
                    if (hasSoftLineBreak)
                    {
                        // 软换行：行末两个空格
                        MarkdownElement softBreakElement = MarkdownElement.GetFromPool();
                        softBreakElement.elementType = MarkdownElementType.SoftLineBreak;
                        softBreakElement.content = "";
                        result.Add(softBreakElement);
                    }
                    else
                    {
                        // 检查下一行是否为空行，决定是否需要段落分隔
                        bool nextLineIsEmpty
                            = i + 1 < lines.Length && string.IsNullOrWhiteSpace(lines[i + 1]);

                        if (nextLineIsEmpty)
                        {
                            // 下一行是空行，这将由空行处理逻辑处理段落分隔
                            // 这里不添加换行元素，让内容连接
                        }
                        // 下一行不是空行，内容应该连接（单个换行符在Markdown中不产生换行）
                        // 不添加换行元素
                    }
                }
            }

            // 处理未结束的代码块
            if (inCodeBlock && g_stringBuilder.Length > 0)
            {
                MarkdownElement codeElement = MarkdownElement.GetFromPool();
                codeElement.SetCodeBlock(g_stringBuilder.ToString(), codeBlockLanguage);
                result.Add(codeElement);
            }
        }

        /// <summary>
        /// 尝试解析标题
        /// </summary>
        /// <param name="line">文本行</param>
        /// <param name="headerText">输出标题文本</param>
        /// <param name="level">输出标题级别</param>
        /// <returns>是否成功解析</returns>
        private static bool TryParseHeader(string line, out string headerText, out int level)
        {
            Match match = g_headerRegex.Match(line);
            if (match.Success)
            {
                level = match.Groups[1].Value.Length;
                headerText = match.Groups[2].Value.Trim();
                return true;
            }

            headerText = string.Empty;
            level = 0;
            return false;
        }

        /// <summary>
        /// 尝试解析无序列表
        /// </summary>
        /// <param name="line">文本行</param>
        /// <param name="listText">输出列表文本</param>
        /// <param name="nestingLevel">输出嵌套层级</param>
        /// <returns>是否成功解析</returns>
        private static bool TryParseUnorderedList(string line, out string listText, out int nestingLevel)
        {
            Match match = g_unorderedListRegex.Match(line);
            if (match.Success)
            {
                // 计算嵌套层级（每4个空格为一级）
                string indentation = match.Groups[1].Value;
                nestingLevel = CalculateNestingLevel(indentation);
                listText = match.Groups[2].Value.Trim();
                return true;
            }

            listText = string.Empty;
            nestingLevel = 0;
            return false;
        }

        /// <summary>
        /// 尝试解析有序列表
        /// </summary>
        /// <param name="line">文本行</param>
        /// <param name="listText">输出列表文本</param>
        /// <param name="nestingLevel">输出嵌套层级</param>
        /// <returns>是否成功解析</returns>
        private static bool TryParseOrderedList(string line, out string listText, out int nestingLevel)
        {
            Match match = g_orderedListRegex.Match(line);
            if (match.Success)
            {
                // 计算嵌套层级（每4个空格为一级）
                string indentation = match.Groups[1].Value;
                nestingLevel = CalculateNestingLevel(indentation);
                listText = match.Groups[2].Value.Trim();
                return true;
            }

            listText = string.Empty;
            nestingLevel = 0;
            return false;
        }

        /// <summary>
        /// 尝试解析任务列表（待办事项）
        /// </summary>
        /// <param name="line">文本行</param>
        /// <param name="taskText">输出任务文本</param>
        /// <param name="isCompleted">输出是否已完成</param>
        /// <param name="nestingLevel">输出嵌套层级</param>
        /// <returns>是否成功解析</returns>
        private static bool TryParseTaskList(string line, out string taskText, out bool isCompleted,
            out int nestingLevel)
        {
            Match match = g_taskListRegex.Match(line);
            if (match.Success)
            {
                // 计算嵌套层级（每4个空格为一级）
                string indentation = match.Groups[1].Value;
                nestingLevel = CalculateNestingLevel(indentation);

                // 解析完成状态：空格表示未完成，x或X表示已完成
                string checkboxContent = match.Groups[2].Value;
                isCompleted = !string.IsNullOrWhiteSpace(checkboxContent) && checkboxContent.ToLower() == "x";

                taskText = match.Groups[3].Value.Trim();
                return true;
            }

            taskText = string.Empty;
            isCompleted = false;
            nestingLevel = 0;
            return false;
        }

        /// <summary>
        /// 尝试解析分割线
        /// </summary>
        /// <param name="line">文本行</param>
        /// <returns>是否成功解析</returns>
        private static bool TryParseDivide(string line)
        {
            // 检查是否匹配分割线模式：3个或更多的 -、* 或 _ 字符
            Match match = g_divideRegex.Match(line);
            return match.Success;
        }

        /// <summary>
        /// 计算列表项的嵌套层级
        /// </summary>
        /// <param name="indentation">前导空白字符</param>
        /// <returns>嵌套层级（0为顶级）</returns>
        private static int CalculateNestingLevel(string indentation)
        {
            // 将Tab转换为4个空格，然后计算层级
            string normalized = indentation.Replace("\t", "    ");
            return normalized.Length / 4;
        }

        /// <summary>
        /// 重置更深层级的有序列表计数器
        /// </summary>
        /// <param name="counters">计数器字典</param>
        /// <param name="currentLevel">当前层级</param>
        private static void ResetDeeperOrderedCounters(Dictionary<int, int> counters, int currentLevel)
        {
            // 重置所有比当前层级更深的计数器
            var keysToRemove = new List<int>();
            foreach (int key in counters.Keys)
            {
                if (key > currentLevel)
                {
                    keysToRemove.Add(key);
                }
            }

            for (var i = 0; i < keysToRemove.Count; i++)
            {
                counters.Remove(keysToRemove[i]);
            }
        }

        /// <summary>
        /// 解析行内元素（链接、图片、代码、粗体、斜体等）
        /// </summary>
        /// <param name="line">文本行</param>
        /// <param name="result">结果列表</param>
        private static void ParseInlineElements(string line, List<MarkdownElement> result)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            var currentIndex = 0;
            var matches = new List<(int start, int length, MarkdownElement element)>();

            // 优先收集带尺寸的图片语法，避免被普通图片语法覆盖
            CollectImageMatches(line, matches);
            CollectMatches(line, g_linkRegex, MarkdownElementType.Link, matches);
            CollectMatches(line, g_inlineCodeRegex, MarkdownElementType.InlineCode, matches);
            // 粗斜体优先级最高，避免被拆分成粗体和斜体
            CollectMatches(line, g_boldItalicRegex, MarkdownElementType.BoldItalic, matches);
            CollectMatches(line, g_boldRegex, MarkdownElementType.Bold, matches);
            CollectMatches(line, g_italicRegex, MarkdownElementType.Italic, matches);

            // 按位置排序
            matches.Sort((a, b) => a.start.CompareTo(b.start));

            // 移除重叠匹配，优先保留起始位置更早的匹配
            var filteredMatches = new List<(int start, int length, MarkdownElement element)>();
            for (var i = 0; i < matches.Count; i++)
            {
                (int start, int length, MarkdownElement element) current = matches[i];
                var hasOverlap = false;

                // 检查与已添加的匹配是否重叠
                for (var j = 0; j < filteredMatches.Count; j++)
                {
                    (int start, int length, MarkdownElement element) existing = filteredMatches[j];
                    // 检查重叠：当前匹配的起始位置在已有匹配的范围内
                    if (current.start < existing.start + existing.length &&
                        current.start + current.length > existing.start)
                    {
                        hasOverlap = true;
                        break;
                    }
                }

                if (!hasOverlap)
                {
                    filteredMatches.Add(current);
                }
                else
                {
                    // 回收重叠的元素到对象池
                    MarkdownElement.ReturnToPool(current.element);
                }
            }

            // 处理文本和内联元素
            for (var i = 0; i < filteredMatches.Count; i++)
            {
                (int start, int length, MarkdownElement element) match = filteredMatches[i];

                // 添加匹配前的普通文本
                if (match.start > currentIndex)
                {
                    string textBefore = line.Substring(currentIndex, match.start - currentIndex);
                    if (!string.IsNullOrEmpty(textBefore))
                    {
                        MarkdownElement textElement = MarkdownElement.GetFromPool();
                        textElement.SetText(textBefore);
                        result.Add(textElement);
                    }
                }

                // 添加匹配的元素
                result.Add(match.element);
                currentIndex = match.start + match.length;
            }

            // 添加剩余的普通文本
            if (currentIndex < line.Length)
            {
                string remainingText = line.Substring(currentIndex);
                if (!string.IsNullOrEmpty(remainingText))
                {
                    MarkdownElement textElement = MarkdownElement.GetFromPool();
                    textElement.SetText(remainingText);
                    result.Add(textElement);
                }
            }
            // 如果没有任何匹配且没有处理任何文本，整行作为普通文本
            else if (filteredMatches.Count == 0 &&
                !string.IsNullOrWhiteSpace(line))
            {
                MarkdownElement textElement = MarkdownElement.GetFromPool();
                textElement.SetText(line);
                result.Add(textElement);
            }
        }

        /// <summary>
        /// 收集图片匹配，支持多种尺寸语法
        /// </summary>
        /// <param name="line">文本行</param>
        /// <param name="matches">匹配结果列表</param>
        private static void CollectImageMatches(string line,
            List<(int start, int length, MarkdownElement element)> matches)
        {
            // 优先匹配带尺寸的语法，按复杂度降序

            // 1. 属性语法: ![alt](url){width=300 height=200}
            MatchCollection attributeMatches = g_imageAttributeRegex.Matches(line);
            for (var i = 0; i < attributeMatches.Count; i++)
            {
                Match match = attributeMatches[i];
                MarkdownElement element = MarkdownElement.GetFromPool();
                string alt = match.Groups[1].Value;
                string url = match.Groups[2].Value;
                string attributes = match.Groups[3].Value;
                (float width, float height, bool isPercentage) = ParseImageAttributes(attributes);
                element.SetImage(alt, url, width, height, isPercentage);
                matches.Add((match.Index, match.Length, element));
            }

            // 2. 尺寸语法: ![alt](url =300x200)
            MatchCollection sizeMatches = g_imageSizeRegex.Matches(line);
            for (var i = 0; i < sizeMatches.Count; i++)
            {
                Match match = sizeMatches[i];
                MarkdownElement element = MarkdownElement.GetFromPool();

                string alt = match.Groups[1].Value;
                string url = match.Groups[2].Value;
                float width = float.Parse(match.Groups[3].Value);
                float height = float.Parse(match.Groups[4].Value);

                element.SetImage(alt, url, width, height, false);
                matches.Add((match.Index, match.Length, element));
            }

            // 3. 宽度语法: ![alt](url =300x)
            MatchCollection widthMatches = g_imageWidthRegex.Matches(line);
            for (var i = 0; i < widthMatches.Count; i++)
            {
                Match match = widthMatches[i];
                MarkdownElement element = MarkdownElement.GetFromPool();

                string alt = match.Groups[1].Value;
                string url = match.Groups[2].Value;
                float width = float.Parse(match.Groups[3].Value);

                element.SetImage(alt, url, width, -1, false);
                matches.Add((match.Index, match.Length, element));
            }

            // 4. 高度语法: ![alt](url =x200)
            MatchCollection heightMatches = g_imageHeightRegex.Matches(line);
            for (var i = 0; i < heightMatches.Count; i++)
            {
                Match match = heightMatches[i];
                MarkdownElement element = MarkdownElement.GetFromPool();

                string alt = match.Groups[1].Value;
                string url = match.Groups[2].Value;
                float height = float.Parse(match.Groups[3].Value);

                element.SetImage(alt, url, -1, height, false);
                matches.Add((match.Index, match.Length, element));
            }

            // 5. 普通语法: ![alt](url) - 只匹配未被上述语法覆盖的部分
            MatchCollection normalMatches = g_imageRegex.Matches(line);
            for (var i = 0; i < normalMatches.Count; i++)
            {
                Match match = normalMatches[i];

                // 检查是否与已有匹配重叠
                var isOverlapping = false;
                for (var j = 0; j < matches.Count; j++)
                {
                    (int start, int length, MarkdownElement element) existingMatch = matches[j];
                    if (match.Index < existingMatch.start + existingMatch.length &&
                        match.Index + match.Length > existingMatch.start)
                    {
                        isOverlapping = true;
                        break;
                    }
                }

                if (!isOverlapping)
                {
                    MarkdownElement element = MarkdownElement.GetFromPool();
                    element.SetImage(match.Groups[1].Value, match.Groups[2].Value);
                    matches.Add((match.Index, match.Length, element));
                }
            }
        }

        /// <summary>
        /// 解析图片属性字符串
        /// </summary>
        /// <param name="attributes">属性字符串，如 "width=300 height=200" 或 "width=50%"</param>
        /// <returns>宽度、高度和是否为百分比</returns>
        private static (float width, float height, bool isPercentage) ParseImageAttributes(string attributes)
        {
            float width = -1;
            float height = -1;
            var isPercentage = false;
            if (string.IsNullOrEmpty(attributes))
            {
                return (width, height, false);
            }

            // 分割属性
            string[] parts = attributes.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (part.StartsWith("width="))
                {
                    string value = part.Substring(6);
                    if (value.EndsWith("%"))
                    {
                        isPercentage = true;
                        value = value.Substring(0, value.Length - 1);
                    }

                    if (float.TryParse(value, out float w))
                    {
                        width = w;
                    }
                    else
                    {
                        Debug.LogError($"[IMAGE] Failed to parse width: '{value}'");
                    }
                }
                else if (part.StartsWith("height="))
                {
                    string value = part.Substring(7);
                    if (value.EndsWith("%"))
                    {
                        isPercentage = true;
                        value = value.Substring(0, value.Length - 1);
                    }

                    if (float.TryParse(value, out float h))
                    {
                        height = h;
                    }
                    else
                    {
                        Debug.LogError($"[IMAGE] Failed to parse height: '{value}'");
                    }
                }
            }

            return (width, height, isPercentage);
        }

        /// <summary>
        /// 收集正则匹配并创建对应元素
        /// </summary>
        /// <param name="line">文本行</param>
        /// <param name="regex">正则表达式</param>
        /// <param name="elementType">元素类型</param>
        /// <param name="matches">匹配结果列表</param>
        private static void CollectMatches(string line, Regex regex, MarkdownElementType elementType,
            List<(int start, int length, MarkdownElement element)> matches)
        {
            MatchCollection regexMatches = regex.Matches(line);
            for (var i = 0; i < regexMatches.Count; i++)
            {
                Match match = regexMatches[i];
                MarkdownElement element = MarkdownElement.GetFromPool();
                switch (elementType)
                {
                    case MarkdownElementType.Image:
                        element.SetImage(match.Groups[1].Value, match.Groups[2].Value);
                        break;
                    case MarkdownElementType.Link:
                        element.SetLink(match.Groups[1].Value, match.Groups[2].Value);
                        break;
                    case MarkdownElementType.InlineCode:
                        element.SetInlineCode(match.Groups[1].Value);
                        break;
                    case MarkdownElementType.Bold:
                        element.elementType = MarkdownElementType.Bold;
                        element.content = match.Groups[1].Value;
                        break;
                    case MarkdownElementType.Italic:
                        element.elementType = MarkdownElementType.Italic;
                        element.content = match.Groups[1].Value;
                        break;
                    case MarkdownElementType.BoldItalic:
                        element.elementType = MarkdownElementType.BoldItalic;
                        element.content = match.Groups[1].Value;
                        break;
                }

                matches.Add((match.Index, match.Length, element));
            }
        }

        /// <summary>
        /// 尝试解析表格
        /// </summary>
        /// <param name="lines">所有行</param>
        /// <param name="startIndex">起始行索引</param>
        /// <param name="table">解析出的表格元素</param>
        /// <param name="consumedLines">消耗的行数</param>
        /// <returns>是否成功解析为表格</returns>
        private static bool TryParseTable(string[] lines, int startIndex, out MarkdownElement table, out int consumedLines)
        {
            table = null;
            consumedLines = 0;
            if (startIndex >= lines.Length)
            {
                return false;
            }

            var currentLine = lines[startIndex].Trim();
            // 检查第一行是否为表格行 - 简化检查
            if (!currentLine.StartsWith("|") || !currentLine.EndsWith("|"))
            {
                return false;
            }

            // 检查第二行是否为表格分隔行
            if (startIndex + 1 >= lines.Length)
            {
                return false;
            }

            var separatorLine = lines[startIndex + 1].Trim();
            if (!separatorLine.StartsWith("|") || !separatorLine.EndsWith("|") || !separatorLine.Contains("-"))
            {
                return false;
            }

            var tableRows = new List<List<string>>();
            var tableAlignment = new List<string>();

            // 解析表头行
            var headerCells = ParseTableRowSimple(lines[startIndex]);
            if (headerCells.Count == 0)
            {
                return false;
            }
            tableRows.Add(headerCells);

            // 解析分隔行获取对齐信息
            ParseTableAlignmentSimple(lines[startIndex + 1], tableAlignment);
            consumedLines = 2;

            // 解析数据行
            for (var i = startIndex + 2; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    break; // 遇到空行结束表格
                }

                var line = lines[i].Trim();
                if (!line.StartsWith("|") || !line.EndsWith("|"))
                {
                    break; // 不再是表格行，结束解析
                }

                var dataCells = ParseTableRowSimple(lines[i]);
                if (dataCells.Count == 0)
                {
                    break;
                }

                tableRows.Add(dataCells);
                consumedLines++;
            }

            // 创建表格元素
            table = MarkdownElement.GetFromPool();
            table.SetTable(tableRows, tableAlignment);
            return true;
        }

        /// <summary>
        /// 解析表格行，提取单元格内容 - 简化版本
        /// </summary>
        /// <param name="line">表格行</param>
        /// <returns>单元格内容列表</returns>
        private static List<string> ParseTableRowSimple(string line)
        {
            var cells = new List<string>();
            
            // 去掉首尾的 | 字符
            var trimmed = line.Trim();
            if (trimmed.StartsWith("|"))
            {
                trimmed = trimmed.Substring(1);
            }
            if (trimmed.EndsWith("|"))
            {
                trimmed = trimmed.Substring(0, trimmed.Length - 1);
            }

            // 按 | 分割并去除空白
            var cellArray = trimmed.Split('|');
            foreach (var cell in cellArray)
            {
                cells.Add(cell.Trim());
            }

            return cells;
        }

        /// <summary>
        /// 解析表格对齐信息 - 简化版本
        /// </summary>
        /// <param name="separatorLine">分隔行</param>
        /// <param name="alignment">对齐信息列表</param>
        private static void ParseTableAlignmentSimple(string separatorLine, List<string> alignment)
        {
            var cells = ParseTableRowSimple(separatorLine);
            
            foreach (var cell in cells)
            {
                var trimmed = cell.Trim();
                if (trimmed.StartsWith(":") && trimmed.EndsWith(":"))
                {
                    alignment.Add("center");
                }
                else if (trimmed.EndsWith(":"))
                {
                    alignment.Add("right");
                }
                else if (trimmed.StartsWith(":"))
                {
                    alignment.Add("left");
                }
                else
                {
                    // 没有明确指定对齐方式，使用空字符串，这样渲染器会使用默认的居中对齐
                    alignment.Add("");
                }
            }
        }
    }
}