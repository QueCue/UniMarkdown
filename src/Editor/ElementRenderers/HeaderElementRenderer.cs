using UnityEditor;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 标题元素渲染器，专门负责渲染各级标题元素
    /// 支持H1-H6所有级别，包括分割线和标准间距
    /// </summary>
    public sealed class HeaderElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：标题
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.Header;

        /// <summary>
        /// 标题渲染器的优先级
        /// </summary>
        public override int Priority => 20;

        /// <summary>
        /// 渲染标题元素
        /// </summary>
        /// <param name="element">标题元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            // 获取对应级别的标题样式
            GUIStyle style = GetHeaderStyle(element.headerLevel);
            GUILayout.Label(element.content, style);
            //EditorGUI.DrawRect(GUILayoutUtility.GetLastRect(), new Color(1f, 1f, 0.5f, 0.3f)); // 区域绘制-调试专用
            RenderHeaderDivider(element.headerLevel);
        }

        /// <summary>
        /// 渲染标题分割线（用于H1和H2）
        /// </summary>
        private void RenderHeaderDivider(int level)
        {
            if (level != 1 &&
                level != 2)
            {
                return; // 仅H1和H2需要分割线
            }

            Rect rect = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true)); // 增加分割线高度
            EditorGUI.DrawRect(rect, new Color32(61, 68, 77, 255)); // 淡灰色分割线
        }
    }
}