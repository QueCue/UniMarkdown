using System;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 文本元素渲染器，专门负责渲染普通文本元素
    /// 这是第一个具体的元素渲染器实现，作为重构的起点
    /// </summary>
    public interface ISyntaxHighlighting : IDisposable
    {
        public string Highlight(string code);
    }
}