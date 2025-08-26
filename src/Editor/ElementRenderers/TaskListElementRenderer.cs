using System.Collections.Generic;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 任务列表元素渲染器，专门负责渲染待办事项（Task List）元素
    /// 支持已完成和未完成状态的勾选框显示
    /// </summary>
    public sealed class TaskListElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：任务列表
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.TaskList;

        /// <summary>
        /// 任务列表渲染器的优先级
        /// </summary>
        public override int Priority => 11;

        /// <summary>
        /// 渲染任务列表元素
        /// </summary>
        /// <param name="element">任务列表元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            // 根据完成状态生成勾选框符号
            string checkbox = element.isTaskCompleted ? "☑" : "☐";

            GUIStyle checkboxStyle = MarkdownStyleManager.Inst.GetTaskCheckboxStyle();
            using (new GUILayout.HorizontalScope())
            {
                // 缩进策略：与列表项保持一致
                const float baseIndentUnit = 2f;
                const float childIndentUnit = 2.5f;
                float totalIndent = element.nestingLevel * childIndentUnit + baseIndentUnit;
                float totalIndentEm = MarkdownStyleManager.Inst.Em(totalIndent);
                GUILayout.Space(totalIndentEm - checkboxStyle.fixedWidth);

                // 勾选框显示
                GUILayout.Label(checkbox, checkboxStyle, GUILayout.ExpandWidth(false));

                // 内容紧跟勾选框，增加小间距分隔
                GUILayout.Space(MarkdownStyleManager.Inst.Em(0.3f));

                // 解析任务项内容中的内联元素
                List<MarkdownElement> inlineElements
                    = MarkdownParser.ParseInlineElementsOnly(element.content);

                // 文本内容容器
                GUIStyle contentStyle = element.isTaskCompleted
                    ? MarkdownStyleManager.Inst.GetCompletedTaskContentStyle()
                    : MarkdownStyleManager.Inst.GetTaskContentStyle();

                for (var i = 0; i < inlineElements.Count; i++)
                {
                    // 已完成任务使用删除线样式
                    if (element.isTaskCompleted &&
                        inlineElements[i].elementType == MarkdownElementType.Text)
                    {
                        // 为已完成任务添加删除线效果
                        GUILayout.Label(inlineElements[i].content,
                            contentStyle,
                            GUILayout.ExpandWidth(false));
                    }
                    else
                    {
                        RenderElement(inlineElements[i], true);
                    }
                }

                GUILayout.FlexibleSpace();

                // 清理临时元素
                for (var i = 0; i < inlineElements.Count; i++)
                {
                    MarkdownElement.ReturnToPool(inlineElements[i]);
                }
            }
        }
    }
}