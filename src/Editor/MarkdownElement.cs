using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// Markdown元素类型枚举，定义支持的Markdown元素类型
    /// </summary>
    public enum MarkdownElementType
    {
        /// <summary>
        /// 普通文本
        /// </summary>
        Text,

        /// <summary>
        /// 标题1-6级
        /// </summary>
        Header,

        /// <summary>
        /// 代码块
        /// </summary>
        CodeBlock,

        /// <summary>
        /// 行内代码
        /// </summary>
        InlineCode,

        /// <summary>
        /// 超链接
        /// </summary>
        Link,

        /// <summary>
        /// 图片
        /// </summary>
        Image,

        /// <summary>
        /// 列表项
        /// </summary>
        ListItem,

        /// <summary>
        /// 粗体文本
        /// </summary>
        Bold,

        /// <summary>
        /// 斜体文本
        /// </summary>
        Italic,

        /// <summary>
        /// 粗斜体文本（***文本***）
        /// </summary>
        BoldItalic,

        /// <summary>
        /// 分割线
        /// </summary>
        Divide,

        /// <summary>
        /// 软换行（行末两个空格产生的换行）
        /// </summary>
        SoftLineBreak,

        /// <summary>
        /// 硬换行（段落分隔，空行产生）
        /// </summary>
        LineBreak,

        /// <summary>
        /// 待办事项（任务列表）
        /// </summary>
        TaskList,

        /// <summary>
        /// 表格
        /// </summary>
        Table
    }

    /// <summary>
    /// Markdown元素数据类，存储解析后的Markdown元素信息
    /// </summary>
    [Serializable]
    public sealed class MarkdownElement
    {
        // 复用对象减少GC压力
        private static readonly Queue<MarkdownElement> g_elementPool = new Queue<MarkdownElement>();

        // 使用字段而非属性减少开销
        public MarkdownElementType elementType;
        public string content;
        public string url;
        public string altText;
        public int headerLevel;
        public bool isOrdered;
        public int listIndex; // 有序列表的序号，从1开始
        public int nestingLevel; // 列表嵌套层级，0为顶级，1为一级嵌套，以此类推
        public string language; // 代码块语言标识（如 "csharp", "javascript"等）
        public int indentLevel; // 元素缩进层级，以空格数计算，用于代码块等需要保持缩进的元素
        public bool isTaskCompleted; // 待办事项是否已完成，true表示已勾选，false表示未勾选

        // 图片尺寸相关字段
        public float imageWidth; // 图片宽度，-1表示使用原始尺寸
        public float imageHeight; // 图片高度，-1表示使用原始尺寸
        public bool imageIsPercentage; // 是否为百分比尺寸

        // 表格相关字段
        public List<List<string>> tableRows; // 表格行数据，每行包含多个单元格
        public List<string> tableAlignment; // 表格列对齐方式："left", "center", "right"

        /// <summary>
        /// 私有构造函数，强制使用对象池
        /// </summary>
        private MarkdownElement() { }

        /// <summary>
        /// 从对象池获取MarkdownElement实例
        /// </summary>
        /// <returns>可复用的MarkdownElement实例</returns>
        public static MarkdownElement GetFromPool()
        {
            if (g_elementPool.Count > 0)
            {
                return g_elementPool.Dequeue();
            }

            return new MarkdownElement();
        }

        /// <summary>
        /// 归还元素到对象池
        /// </summary>
        /// <param name="element">要归还的元素</param>
        public static void ReturnToPool(MarkdownElement element)
        {
            if (element == null)
            {
                return;
            }

            // 重置数据
            element.Reset();
            g_elementPool.Enqueue(element);
        }

        /// <summary>
        /// 重置元素数据
        /// </summary>
        public void Reset()
        {
            elementType = MarkdownElementType.Text;
            content = string.Empty;
            url = string.Empty;
            altText = string.Empty;
            headerLevel = 1;
            isOrdered = false;
            listIndex = 0; // 重置列表序号
            nestingLevel = 0; // 重置嵌套层级
            language = string.Empty;
            indentLevel = 0; // 重置缩进层级
            isTaskCompleted = false; // 重置待办事项状态

            // 重置图片尺寸字段
            imageWidth = -1;
            imageHeight = -1;
            imageIsPercentage = false;

            // 重置表格字段
            tableRows?.Clear();
            tableAlignment?.Clear();
        }

        /// <summary>
        /// 设置文本元素
        /// </summary>
        /// <param name="text">文本内容</param>
        public void SetText(string text)
        {
            elementType = MarkdownElementType.Text;
            content = text;
        }

        /// <summary>
        /// 设置标题元素
        /// </summary>
        /// <param name="text">标题文本</param>
        /// <param name="level">标题级别1-6</param>
        public void SetHeader(string text, int level)
        {
            elementType = MarkdownElementType.Header;
            content = text;
            headerLevel = Mathf.Clamp(level, 1, 6);
        }

        /// <summary>
        /// 设置代码块元素
        /// </summary>
        /// <param name="code">代码内容</param>
        /// <param name="lang">编程语言标识（可选）</param>
        /// <param name="indent">缩进层级（以空格数计算，可选）</param>
        public void SetCodeBlock(string code, string lang = "", int indent = 0)
        {
            elementType = MarkdownElementType.CodeBlock;
            content = code;
            language = lang;
            indentLevel = indent; // 设置缩进层级
        }

        /// <summary>
        /// 设置行内代码元素
        /// </summary>
        /// <param name="code">代码内容</param>
        public void SetInlineCode(string code)
        {
            elementType = MarkdownElementType.InlineCode;
            content = code;
        }

        /// <summary>
        /// 设置链接元素
        /// </summary>
        /// <param name="text">链接显示文本</param>
        /// <param name="linkUrl">链接URL</param>
        public void SetLink(string text, string linkUrl)
        {
            elementType = MarkdownElementType.Link;
            content = text;
            url = linkUrl;
        }

        /// <summary>
        /// 设置图片元素
        /// </summary>
        /// <param name="alt">替代文本</param>
        /// <param name="imagePath">图片路径</param>
        public void SetImage(string alt, string imagePath)
        {
            SetImage(alt, imagePath, -1, -1, false);
        }

        /// <summary>
        /// 设置图片元素（包含尺寸）
        /// </summary>
        /// <param name="alt">替代文本</param>
        /// <param name="imagePath">图片路径</param>
        /// <param name="width">宽度，-1表示使用原始尺寸</param>
        /// <param name="height">高度，-1表示使用原始尺寸</param>
        /// <param name="isPercentage">是否为百分比尺寸</param>
        public void SetImage(string alt, string imagePath, float width, float height, bool isPercentage)
        {
            elementType = MarkdownElementType.Image;
            altText = alt;
            url = imagePath;
            imageWidth = width;
            imageHeight = height;
            imageIsPercentage = isPercentage;
        }

        /// <summary>
        /// 设置列表项元素
        /// </summary>
        /// <param name="text">列表项文本</param>
        /// <param name="ordered">是否为有序列表</param>
        /// <param name="index">列表项序号（有序列表使用，从1开始）</param>
        /// <param name="nesting">嵌套层级（0为顶级，1为一级嵌套，以此类推）</param>
        public void SetListItem(string text, bool ordered, int index = 0, int nesting = 0)
        {
            elementType = MarkdownElementType.ListItem;
            content = text;
            isOrdered = ordered;
            listIndex = index; // 设置列表序号
            nestingLevel = nesting; // 设置嵌套层级
        }

        /// <summary>
        /// 设置待办事项元素
        /// </summary>
        /// <param name="text">待办事项文本</param>
        /// <param name="completed">是否已完成</param>
        /// <param name="nesting">嵌套层级（0为顶级，1为一级嵌套，以此类推）</param>
        public void SetTaskList(string text, bool completed, int nesting = 0)
        {
            elementType = MarkdownElementType.TaskList;
            content = text;
            isTaskCompleted = completed; // 设置完成状态
            nestingLevel = nesting; // 设置嵌套层级
        }

        /// <summary>
        /// 设置分割线元素
        /// </summary>
        public void SetDivide()
        {
            elementType = MarkdownElementType.Divide;
            content = "---"; // 标准分割线表示
        }

        /// <summary>
        /// 设置表格元素
        /// </summary>
        /// <param name="rows">表格行数据</param>
        /// <param name="alignment">列对齐方式</param>
        public void SetTable(List<List<string>> rows, List<string> alignment = null)
        {
            elementType = MarkdownElementType.Table;
            
            // 初始化表格数据
            if (tableRows == null)
            {
                tableRows = new List<List<string>>();
            }
            else
            {
                tableRows.Clear();
            }

            if (tableAlignment == null)
            {
                tableAlignment = new List<string>();
            }
            else
            {
                tableAlignment.Clear();
            }

            // 复制表格数据
            if (rows != null)
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    var row = new List<string>();
                    if (rows[i] != null)
                    {
                        row.AddRange(rows[i]);
                    }
                    tableRows.Add(row);
                }
            }

            // 复制对齐方式
            if (alignment != null)
            {
                tableAlignment.AddRange(alignment);
            }
        }
    }
}