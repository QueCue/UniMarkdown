using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 文本元素渲染器，专门负责渲染普通文本元素
    /// 这是第一个具体的元素渲染器实现，作为重构的起点
    /// </summary>
    public sealed class TextElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：文本
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.Text;

        /// <summary>
        /// 文本渲染器的优先级
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 渲染文本元素 - 完全提取自MarkdownRenderer.RenderText
        /// </summary>
        /// <param name="element">文本元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            GUIStyle textStyle = GetTextStyle();
            GUILayoutOption[] options = isInline ? GetInlineOptions() : null;
            GUILayout.Label(element.content, textStyle, options);
            //EditorGUI.DrawRect(GUILayoutUtility.GetLastRect(), new Color(1f, 1f, 0.5f, 0.3f)); // 区域绘制-调试专用
        }
    }
}