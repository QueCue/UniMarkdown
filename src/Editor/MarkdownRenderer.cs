using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// Markdown渲染器，负责在Unity Editor GUI中渲染MarkdownElement列表
    /// 内置解析功能，提供通用的Markdown渲染接口
    /// </summary>
    public sealed class MarkdownRenderer : IDisposable
    {
        // 元素渲染器工厂
        private ElementRendererFactory m_rendererFactory;

        // 内部管理解析后的元素列表
        private List<MarkdownElement> m_markdownElements;

        // 当前解析的Markdown内容，用于避免重复解析
        private string m_cachedMarkdownContent;

        // 缓存滚动位置
        private Vector2 m_scrollPosition;

        /// <summary>
        /// 私有构造函数
        /// </summary>
        public MarkdownRenderer()
        {
            m_rendererFactory = new ElementRendererFactory(this);
            m_markdownElements = new List<MarkdownElement>();
            m_cachedMarkdownContent = string.Empty;
        }

        public void Dispose()
        {
            Reset();

            // 清理元素到对象池
            if (m_markdownElements != null)
            {
                for (var i = 0; i < m_markdownElements.Count; i++)
                {
                    MarkdownElement.ReturnToPool(m_markdownElements[i]);
                }

                m_markdownElements.Clear();
                m_markdownElements = null;
            }
        }

        /// <summary>
        /// 设置重绘回调 - 传递给所有渲染器
        /// </summary>
        /// <param name="repaintCallback">重绘回调</param>
        public void SetRepaintCallback(Action repaintCallback)
        {
            m_rendererFactory?.SetRepaintCallback(repaintCallback);
        }

        public void Reset()
        {
            m_rendererFactory?.ClearCache();

            // 清理缓存的元素
            if (m_markdownElements != null)
            {
                for (var i = 0; i < m_markdownElements.Count; i++)
                {
                    MarkdownElement.ReturnToPool(m_markdownElements[i]);
                }

                m_markdownElements.Clear();
            }

            m_cachedMarkdownContent = string.Empty;
        }

        /// <summary>
        /// 渲染Markdown文本 - 通用接口，自动解析和渲染
        /// </summary>
        /// <param name="markdownText">Markdown文本内容</param>
        /// <param name="scrollPosition">滚动位置</param>
        /// <returns>更新后的滚动位置</returns>
        public Vector2 RenderMarkdown(string markdownText, Vector2 scrollPosition)
        {
            // 检查是否需要重新解析
            if (markdownText != m_cachedMarkdownContent)
            {
                ParseMarkdownText(markdownText);
                m_cachedMarkdownContent = markdownText;
            }

            return RenderMarkdown(m_markdownElements, scrollPosition);
        }

        /// <summary>
        /// 渲染Markdown元素列表 - 兼容接口
        /// </summary>
        /// <param name="elements">要渲染的元素列表</param>
        /// <param name="scrollPosition">滚动位置</param>
        /// <returns>更新后的滚动位置</returns>
        public Vector2 RenderMarkdown(List<MarkdownElement> elements, Vector2 scrollPosition)
        {
            // 绘制背景
            //EditorGUI.DrawRect(rect, MarkdownStyleManager.Inst.BackgroundColor);
            if (elements == null ||
                elements.Count == 0)
            {
                EditorGUILayout.LabelField("没有内容可显示", EditorStyles.centeredGreyMiniLabel);
                return Vector2.zero;
            }

            // 创建滚动视图，让其自动计算高度
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                MarkdownStyleManager.Inst.GetScrollViewBackgroundStyle());

            try
            {
                // 合并同一行的内联元素
                List<List<MarkdownElement>> groupedElements = GroupInlineElements(elements);
                for (var i = 0; i < groupedElements.Count; i++)
                {
                    List<MarkdownElement> group = groupedElements[i];
                    if (i > 0)
                    {
                        AddSpacingBetweenElements(groupedElements[i - 1], group);
                    }

                    if (group.Count == 1)
                    {
                        RenderElement(group[0]);
                    }
                    else
                    {
                        RenderInlineGroup(group);
                    }
                }
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }

            return scrollPosition;
        }

        /// <summary>
        /// 根据Markdown标准添加元素间距
        /// </summary>
        /// <param name="previousGroup">前一个元素组</param>
        /// <param name="currentGroup">当前元素组</param>
        private void AddSpacingBetweenElements(List<MarkdownElement> previousGroup,
            List<MarkdownElement> currentGroup)
        {
            MarkdownElement prevElement = previousGroup[0];
            MarkdownElement currElement = currentGroup[0];
            float spacing = CalculateSpacing(prevElement, currElement);
            if (spacing > 0)
            {
                GUILayout.Space(spacing);
            }
        }

        /// <summary>
        /// 计算两个元素之间的间距 - 严格遵循标准Markdown规则
        /// </summary>
        /// <param name="prevElement">前一个元素</param>
        /// <param name="currElement">当前元素</param>
        /// <returns>间距像素值</returns>
        private float CalculateSpacing(MarkdownElement prevElement, MarkdownElement currElement)
        {
            MarkdownStyleManager styleMgr = MarkdownStyleManager.Inst;
            if (prevElement?.elementType == MarkdownElementType.ListItem &&
                currElement?.elementType == MarkdownElementType.ListItem)
            {
                return styleMgr.Em(0.5f);
            }

            if (prevElement?.elementType != MarkdownElementType.ListItem &&
                currElement?.elementType == MarkdownElementType.ListItem)
            {
                return styleMgr.Em(1.2f);
            }

            return styleMgr.Em(1);
        }

        /// <summary>
        /// 将内联元素分组
        /// </summary>
        /// <param name="elements">元素列表</param>
        /// <returns>分组后的元素列表</returns>
        private List<List<MarkdownElement>> GroupInlineElements(List<MarkdownElement> elements)
        {
            var groups = new List<List<MarkdownElement>>();
            var currentGroup = new List<MarkdownElement>();

            for (var i = 0; i < elements.Count; i++)
            {
                MarkdownElement element = elements[i];

                // 如果是硬换行符，结束当前组（但不添加换行符到结果中）
                if (element.elementType == MarkdownElementType.LineBreak)
                {
                    if (currentGroup.Count > 0)
                    {
                        groups.Add(currentGroup);
                        currentGroup = new List<MarkdownElement>();
                    }

                    // 不添加硬换行符元素本身，它只用于分组
                    continue;
                }

                // 如果是软换行符，加入当前组并继续
                if (element.elementType == MarkdownElementType.SoftLineBreak)
                {
                    currentGroup.Add(element);
                    continue;
                }

                // 如果是块级元素，结束当前组
                if (IsBlockElement(element))
                {
                    if (currentGroup.Count > 0)
                    {
                        groups.Add(currentGroup);
                        currentGroup = new List<MarkdownElement>();
                    }

                    groups.Add(new List<MarkdownElement> { element });
                }
                else
                {
                    currentGroup.Add(element);
                }
            }

            if (currentGroup.Count > 0)
            {
                groups.Add(currentGroup);
            }

            return groups;
        }

        /// <summary>
        /// 判断是否为块级元素
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns>是否为块级元素</returns>
        private bool IsBlockElement(MarkdownElement element)
        {
            return element.elementType == MarkdownElementType.Header ||
                element.elementType == MarkdownElementType.CodeBlock ||
                element.elementType == MarkdownElementType.Image ||
                element.elementType == MarkdownElementType.ListItem ||
                element.elementType == MarkdownElementType.TaskList;
        }

        /// <summary>
        /// 渲染内联元素组
        /// </summary>
        /// <param name="group">内联元素组</param>
        private void RenderInlineGroup(List<MarkdownElement> group)
        {
            EditorGUILayout.BeginHorizontal();

            // 渲染组内所有元素
            for (var i = 0; i < group.Count; i++)
            {
                MarkdownElement element = group[i];
                RenderElement(element, true);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 渲染单个Markdown元素
        /// </summary>
        /// <param name="element">要渲染的元素</param>
        /// <param name="isInline">是否是行内元素</param>
        public void RenderElement(MarkdownElement element, bool isInline = false)
        {
            if (null == element)
            {
                return;
            }

            IElementRenderer renderer = m_rendererFactory.GetRenderer(element.elementType);
            renderer?.Render(element, isInline);
        }

        /// <summary>
        /// 解析Markdown文本为元素列表
        /// </summary>
        /// <param name="markdownText">要解析的Markdown文本</param>
        private void ParseMarkdownText(string markdownText)
        {
            if (string.IsNullOrEmpty(markdownText))
            {
                return;
            }

            try
            {
                // 复用现有的元素列表减少GC
                MarkdownParser.ParseMarkdown(markdownText, m_markdownElements);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MarkdownRenderer] 解析Markdown时发生错误: {ex.Message}");

                // 出错时清空元素列表
                for (var i = 0; i < m_markdownElements.Count; i++)
                {
                    MarkdownElement.ReturnToPool(m_markdownElements[i]);
                }

                m_markdownElements.Clear();
            }
        }
    }
}