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
        /// 渲染文本元素 - 使用上下文感知的智能换行控制
        /// </summary>
        /// <param name="element">文本元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            // 使用上下文感知的文本样式：
            // - 混合行内元素上下文：禁用自动换行，保持行内流式布局
            // - 纯文本上下文：启用自动换行，允许长文本自动折行
            GUIStyle textStyle = GetTextStyleForContext(element.isInMixedInlineContext);
            
            // 根据上下文调整布局选项
            GUILayoutOption[] options;
            if (isInline)
            {
                if (element.isInMixedInlineContext)
                {
                    // 混合上下文：使用紧凑布局
                    options = GetInlineOptions(); // ExpandWidth(false)
                }
                else
                {
                    // 纯文本上下文：允许扩展宽度以支持换行
                    options = new[] { GUILayout.ExpandWidth(true) };
                }
            }
            else
            {
                options = null;
            }
            
            GUILayout.Label(element.content, textStyle, options);
            //EditorGUI.DrawRect(GUILayoutUtility.GetLastRect(), new Color(1f, 1f, 0.5f, 0.3f)); // 区域绘制-调试专用
        }
    }
}