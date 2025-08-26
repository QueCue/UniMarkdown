using UnityEditor;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 分割线元素渲染器
    /// </summary>
    public sealed class DivideElementRenderer : BaseElementRenderer
    {
        public override MarkdownElementType SupportedElementType => MarkdownElementType.Divide;

        public override int Priority => 10;

        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            // 分割线不支持内联模式
            if (isInline)
            {
                return;
            }

            GUILayout.Space(Em(0.5f));
            Rect rect = GUILayoutUtility.GetRect(0, Em(0.25f), GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
            GUILayout.Space(Em(0.5f));
        }
    }
}