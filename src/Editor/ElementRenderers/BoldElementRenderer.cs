using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 粗体元素渲染器，专门负责渲染粗体文本元素
    /// 提供粗体文本的专用样式渲染
    /// </summary>
    public sealed class BoldElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：粗体
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.Bold;

        /// <summary>
        /// 粗体渲染器的优先级
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 渲染粗体元素
        /// </summary>
        /// <param name="element">粗体元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            if (string.IsNullOrEmpty(element.content))
            {
                return;
            }

            GUIStyle boldStyle = GetBoldStyle();
            GUILayoutOption[] options = isInline ? GetInlineOptions() : null;
            GUILayout.Label(element.content, boldStyle, options);
        }
    }
}