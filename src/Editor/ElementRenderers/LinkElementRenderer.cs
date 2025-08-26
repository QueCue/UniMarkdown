using UnityEditor;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 链接元素渲染器，专门负责渲染超链接元素
    /// 支持可点击的链接样式，点击时在浏览器中打开链接
    /// </summary>
    public sealed class LinkElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：链接
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.Link;

        /// <summary>
        /// 链接渲染器的优先级
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 渲染链接元素 - 支持行内显示
        /// </summary>
        /// <param name="element">链接元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            GUIStyle linkStyle = GetLinkStyle();
            // 行内渲染 - 不使用布局组，保持在同一行
            if (GUILayout.Button(element.content, linkStyle, GetInlineOptions()))
            {
                Application.OpenURL(element.url);
            }

            // 获取按钮矩形区域用于绘制下划线和光标
            Rect buttonRect = GUILayoutUtility.GetLastRect();

            // 添加鼠标悬停时的手指光标
            EditorGUIUtility.AddCursorRect(buttonRect, MouseCursor.Link);

            // 绘制下划线
            var underlineRect = new Rect(buttonRect.x,
                buttonRect.y + buttonRect.height - 1,
                buttonRect.width,
                1);

            EditorGUI.DrawRect(underlineRect, linkStyle.normal.textColor);
        }
    }
}