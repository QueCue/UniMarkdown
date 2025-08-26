using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 粗斜体元素渲染器，用于渲染 ***文本*** 格式的粗斜体内容
    /// 同时应用粗体和斜体的视觉效果
    /// </summary>
    public class BoldItalicElementRenderer : BaseElementRenderer
    {
        public override MarkdownElementType SupportedElementType => MarkdownElementType.BoldItalic;

        public override int Priority => 10;

        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            if (string.IsNullOrEmpty(element.content))
            {
                return;
            }

            GUIStyle italicStyle = GetBoldItalicStyle();
            GUILayout.Label(element.content, italicStyle, GetInlineOptions());
        }
    }
}