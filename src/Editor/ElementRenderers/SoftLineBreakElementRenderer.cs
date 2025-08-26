using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 软换行元素渲染器，专门负责渲染由行末两个空格产生的软换行
    /// 渲染为一个小的换行，不像硬换行那样产生段落分隔
    /// </summary>
    public sealed class SoftLineBreakElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：软换行
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.SoftLineBreak;

        /// <summary>
        /// 软换行渲染器的优先级
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 渲染软换行元素
        /// </summary>
        /// <param name="element">软换行元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            if (isInline)
            {
                // 在内联渲染中，强制换行到下一行
                // 使用Unity的换行机制
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
            else
            {
                // 在块级渲染中，添加小的垂直空间
                GUILayout.Space(MarkdownStyleManager.Inst.Em(0.5f));
            }
        }
    }
}