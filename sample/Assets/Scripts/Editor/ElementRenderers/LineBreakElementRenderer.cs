namespace UniMarkdown.Editor
{
    /// <summary>
    /// 换行元素渲染器，专门负责渲染换行元素
    /// 提供标准化的垂直间距控制
    /// </summary>
    public sealed class LineBreakElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：换行
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.LineBreak;

        /// <summary>
        /// 换行渲染器的优先级
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 渲染换行元素
        /// </summary>
        /// <param name="element">换行元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline) { }
    }
}