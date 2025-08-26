using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 行内代码元素渲染器，专门负责渲染行内代码元素
    /// 支持行内代码的特殊样式（背景色、字体等）
    /// </summary>
    public sealed class InlineCodeElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：行内代码
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.InlineCode;

        /// <summary>
        /// 行内代码渲染器的优先级
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 渲染行内代码元素 - 完全提取自MarkdownRenderer.RenderInlineCode
        /// </summary>
        /// <param name="element">行内代码元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            GUILayout.Label(element.content, GetInlineCodeStyle(), GetInlineOptions());
        }
    }
}