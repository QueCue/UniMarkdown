using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 斜体元素渲染器，专门负责渲染斜体文本元素
    /// 提供斜体文本的专用样式渲染
    /// </summary>
    public sealed class ItalicElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：斜体
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.Italic;

        /// <summary>
        /// 斜体渲染器的优先级
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 渲染斜体元素
        /// </summary>
        /// <param name="element">斜体元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            if (string.IsNullOrEmpty(element.content))
            {
                return;
            }

            GUIStyle italicStyle = GetItalicStyle();
            GUILayoutOption[] options
                = isInline ? GetInlineOptions() : new[] { GUILayout.ExpandWidth(false) };

            GUILayout.Label(element.content, italicStyle, options);
        }
    }
}