using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// JSON语法高亮，支持完整的JSON语法元素识别和配色
    /// </summary>
    public sealed class JsonSyntaxHighlighting : ISyntaxHighlighting
    {
        // 正则表达式缓存
        private static readonly Regex g_stringRegex = new(@"""(\\.|[^""\\])*""", RegexOptions.Compiled);

        private static readonly Regex g_numberRegex
            = new(@"-?\d+(\.\d+)?([eE][+-]?\d+)?", RegexOptions.Compiled);

        private static readonly Regex g_booleanRegex = new(@"\b(true|false)\b", RegexOptions.Compiled);
        private static readonly Regex g_nullRegex = new(@"\bnull\b", RegexOptions.Compiled);

        private static readonly Regex g_propertyKeyRegex
            = new(@"""(\\.|[^""\\])*""\s*:", RegexOptions.Compiled);

        // 缓存语法高亮颜色
        private Dictionary<string, string> m_syntaxColors;

        public JsonSyntaxHighlighting()
        {
            InitializeSyntaxColors();
        }

        public string Highlight(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return code;
            }

            var result = new StringBuilder(code.Length * 2);
            string[] lines = code.Split('\n');

            for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                if (lineIndex > 0)
                {
                    result.Append('\n');
                }

                string line = lines[lineIndex];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                ProcessJsonLine(line, result);
            }

            return result.ToString();
        }

        public void Dispose()
        {
            m_syntaxColors?.Clear();
            m_syntaxColors = null;
        }

        /// <summary>
        /// 初始化JSON语法高亮颜色配置
        /// </summary>
        private void InitializeSyntaxColors()
        {
            m_syntaxColors = new Dictionary<string, string>();

            // 基于VS Code OneDark主题的JSON配色方案
            bool isDarkTheme = EditorGUIUtility.isProSkin;
            if (isDarkTheme)
            {
                // OneDark深色主题配色
                m_syntaxColors["property_key"] = "#EF596F"; // 属性键 - 红色
                m_syntaxColors["string_value"] = "#89CA78"; // 字符串值 - 绿色
                m_syntaxColors["number"] = "#D19A66"; // 数字 - 橙色
                m_syntaxColors["boolean"] = "#D55FDE"; // 布尔值 - 紫色
                m_syntaxColors["null"] = "#D55FDE"; // null值 - 紫色
                m_syntaxColors["punctuation"] = "#BBBBBB"; // 标点符号 - 灰白色
                m_syntaxColors["brace"] = "#61AFEF"; // 大括号 - 蓝色
                m_syntaxColors["bracket"] = "#E5C07B"; // 方括号 - 黄色
                m_syntaxColors["comma"] = "#BBBBBB"; // 逗号 - 灰白色
                m_syntaxColors["colon"] = "#BBBBBB"; // 冒号 - 灰白色
            }
            else
            {
                // 浅色主题配色
                m_syntaxColors["property_key"] = "#0451A5"; // 属性键 - 蓝色
                m_syntaxColors["string_value"] = "#A31515"; // 字符串值 - 深红色
                m_syntaxColors["number"] = "#098658"; // 数字 - 深绿色
                m_syntaxColors["boolean"] = "#0000FF"; // 布尔值 - 蓝色
                m_syntaxColors["null"] = "#0000FF"; // null值 - 蓝色
                m_syntaxColors["punctuation"] = "#000000"; // 标点符号 - 黑色
                m_syntaxColors["brace"] = "#000000"; // 大括号 - 黑色
                m_syntaxColors["bracket"] = "#000000"; // 方括号 - 黑色
                m_syntaxColors["comma"] = "#000000"; // 逗号 - 黑色
                m_syntaxColors["colon"] = "#000000"; // 冒号 - 黑色
            }
        }

        /// <summary>
        /// 处理JSON语法高亮的单行内容
        /// </summary>
        private void ProcessJsonLine(string line, StringBuilder result)
        {
            var i = 0;
            while (i < line.Length)
            {
                char c = line[i];

                // 处理字符串（属性键或字符串值）
                if (c == '"')
                {
                    int stringEnd = FindStringEnd(line, i);
                    if (stringEnd == -1)
                    {
                        // 未闭合的字符串，直接添加剩余内容
                        result.Append(line.Substring(i));
                        break;
                    }

                    string str = line.Substring(i, stringEnd - i + 1);

                    // 检查是否为属性键（字符串后跟冒号）
                    bool isPropertyKey = IsPropertyKey(line, stringEnd + 1);
                    string colorKey = isPropertyKey ? "property_key" : "string_value";

                    result.Append($"<color={m_syntaxColors[colorKey]}>{str}</color>");
                    i = stringEnd + 1;
                    continue;
                }

                // 处理数字
                if (char.IsDigit(c) ||
                    c == '-' && i + 1 < line.Length && char.IsDigit(line[i + 1]))
                {
                    int numberEnd = FindNumberEnd(line, i);
                    string number = line.Substring(i, numberEnd - i);
                    result.Append($"<color={m_syntaxColors["number"]}>{number}</color>");
                    i = numberEnd;
                    continue;
                }

                // 处理布尔值
                if (c == 't' &&
                    IsWordMatch(line, i, "true"))
                {
                    result.Append($"<color={m_syntaxColors["boolean"]}>true</color>");
                    i += 4;
                    continue;
                }

                if (c == 'f' &&
                    IsWordMatch(line, i, "false"))
                {
                    result.Append($"<color={m_syntaxColors["boolean"]}>false</color>");
                    i += 5;
                    continue;
                }

                // 处理null值
                if (c == 'n' &&
                    IsWordMatch(line, i, "null"))
                {
                    result.Append($"<color={m_syntaxColors["null"]}>null</color>");
                    i += 4;
                    continue;
                }

                // 处理标点符号
                switch (c)
                {
                    case '{':
                    case '}':
                        result.Append($"<color={m_syntaxColors["brace"]}>{c}</color>");
                        break;
                    case '[':
                    case ']':
                        result.Append($"<color={m_syntaxColors["bracket"]}>{c}</color>");
                        break;
                    case ':':
                        result.Append($"<color={m_syntaxColors["colon"]}>{c}</color>");
                        break;
                    case ',':
                        result.Append($"<color={m_syntaxColors["comma"]}>{c}</color>");
                        break;
                    default:
                        result.Append(c);
                        break;
                }

                i++;
            }
        }

        /// <summary>
        /// 查找字符串结尾位置
        /// </summary>
        private int FindStringEnd(string line, int start)
        {
            var escaped = false;
            for (int i = start + 1; i < line.Length; i++)
            {
                char c = line[i];
                if (escaped)
                {
                    escaped = false;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    continue;
                }

                if (c == '"')
                {
                    return i;
                }
            }

            return -1; // 未找到结尾
        }

        /// <summary>
        /// 查找数字结尾位置
        /// </summary>
        private int FindNumberEnd(string line, int start)
        {
            int i = start;

            // 处理负号
            if (line[i] == '-')
            {
                i++;
            }

            // 处理整数部分
            while (i < line.Length &&
                char.IsDigit(line[i]))
            {
                i++;
            }

            // 处理小数部分
            if (i < line.Length &&
                line[i] == '.')
            {
                i++;
                while (i < line.Length &&
                    char.IsDigit(line[i]))
                {
                    i++;
                }
            }

            // 处理指数部分
            if (i < line.Length &&
                (line[i] == 'e' || line[i] == 'E'))
            {
                i++;
                if (i < line.Length &&
                    (line[i] == '+' || line[i] == '-'))
                {
                    i++;
                }

                while (i < line.Length &&
                    char.IsDigit(line[i]))
                {
                    i++;
                }
            }

            return i;
        }

        /// <summary>
        /// 检查是否为属性键（字符串后跟冒号）
        /// </summary>
        private bool IsPropertyKey(string line, int afterStringPos)
        {
            for (int i = afterStringPos; i < line.Length; i++)
            {
                char c = line[i];
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                return c == ':';
            }

            return false;
        }

        /// <summary>
        /// 检查单词匹配
        /// </summary>
        private bool IsWordMatch(string line, int start, string word)
        {
            if (start + word.Length > line.Length)
            {
                return false;
            }

            // 检查单词内容
            for (var i = 0; i < word.Length; i++)
            {
                if (line[start + i] != word[i])
                {
                    return false;
                }
            }

            // 检查单词边界（确保不是更长单词的一部分）
            int nextPos = start + word.Length;
            if (nextPos < line.Length)
            {
                char nextChar = line[nextPos];
                if (char.IsLetterOrDigit(nextChar) ||
                    nextChar == '_')
                {
                    return false;
                }
            }

            // 检查前面的字符
            if (start > 0)
            {
                char prevChar = line[start - 1];
                if (char.IsLetterOrDigit(prevChar) ||
                    prevChar == '_')
                {
                    return false;
                }
            }

            return true;
        }
    }
}