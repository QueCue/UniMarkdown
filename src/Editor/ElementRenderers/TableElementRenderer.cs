using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 表格元素渲染器，专门负责渲染Markdown表格元素
    /// 支持列对齐、表头样式等功能
    /// </summary>
    public sealed class TableElementRenderer : BaseElementRenderer
    {
        // 静态字段用于存储表格的滚动位置，以保持滚动状态
        private static Vector2 g_tableScrollPosition = Vector2.zero;

        // 性能优化：缓存GUIContent对象减少GC分配
        private static readonly GUIContent g_tempContent = new();

        public override MarkdownElementType SupportedElementType => MarkdownElementType.Table;

        /// <summary>
        /// 检查是否能够渲染指定类型的元素
        /// </summary>
        /// <param name="elementType">元素类型</param>
        /// <returns>是否能够渲染</returns>
        public override bool CanRender(MarkdownElementType elementType)
        {
            return elementType == MarkdownElementType.Table;
        }

        /// <summary>
        /// 渲染表格元素
        /// </summary>
        /// <param name="element">表格元素</param>
        /// <param name="isInline">是否为行内元素（表格总是块级元素）</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            if (element.tableRows == null ||
                element.tableRows.Count == 0)
            {
                return;
            }

            List<List<string>> rows = element.tableRows;
            List<string> alignment = element.tableAlignment ?? new List<string>();

            // 计算列数 - 性能优化：避免LINQ
            var columnCount = 0;
            for (var i = 0; i < rows.Count; i++)
            {
                if (rows[i].Count > columnCount)
                {
                    columnCount = rows[i].Count;
                }
            }

            if (columnCount == 0)
            {
                return;
            }

            // 计算每列的宽度 - 根据内容自适应
            var columnWidths = new float[columnCount];
            float availableWidth = EditorGUIUtility.currentViewWidth - Em(2.5f);

            // 获取表格样式用于测量
            GUIStyle headerStyleForMeasure = GetTableHeaderStyle();
            GUIStyle cellStyleForMeasure = GetTableCellStyle();

            // 计算每列的最大内容宽度
            for (var colIndex = 0; colIndex < columnCount; colIndex++)
            {
                float maxWidth = Em(3.75f); // 最小列宽，使用Em单位

                for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                {
                    List<string> row = rows[rowIndex];
                    var cellContent = "";
                    if (colIndex < row.Count &&
                        row[colIndex] != null)
                    {
                        cellContent = row[colIndex];
                    }

                    // 根据行类型选择正确的样式进行测量
                    GUIStyle measureStyle = rowIndex == 0 ? headerStyleForMeasure : cellStyleForMeasure;

                    // 性能优化：重用GUIContent对象
                    g_tempContent.text = cellContent;
                    Vector2 contentSize = measureStyle.CalcSize(g_tempContent);
                    maxWidth = Mathf.Max(maxWidth, contentSize.x + Em(0.75f)); // 加上内边距，使用Em单位
                }

                columnWidths[colIndex] = maxWidth;
            }

            // 计算总宽度
            var totalWidth = 0f;
            for (var i = 0; i < columnCount; i++)
            {
                totalWidth += columnWidths[i];
            }

            // 不再进行自动缩放，保持表格的原始大小
            // 如果表格宽度超过可用宽度，将使用水平滚动视图
            bool useScrollView = totalWidth > availableWidth;

            // 开始渲染表格 - 使用MarkdownStyleManager的表格容器样式
            EditorGUILayout.BeginVertical(GetTableContainerStyle());

            // 如果表格宽度超过可用宽度，使用滚动视图
            if (useScrollView)
            {
                g_tableScrollPosition = EditorGUILayout.BeginScrollView(g_tableScrollPosition,
                    GUILayout.Width(availableWidth),
                    GUILayout.Height(Em(2f) * rows.Count + Em(1.25f))); // 使用Em单位计算表格高度
            }

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                List<string> row = rows[rowIndex];
                bool isHeaderRow = rowIndex == 0;

                // 渲染每一行
                EditorGUILayout.BeginHorizontal();

                for (var colIndex = 0; colIndex < columnCount; colIndex++)
                {
                    var cellContent = "";
                    if (colIndex < row.Count &&
                        row[colIndex] != null)
                    {
                        cellContent = row[colIndex];
                    }

                    // 获取基础样式并创建副本以避免修改原始缓存样式
                    GUIStyle baseStyle = isHeaderRow ? GetTableHeaderStyle() : GetTableCellStyle();
                    var cellStyle = new GUIStyle(baseStyle)
                    {
                        // 默认设置为居中对齐
                        alignment = TextAnchor.MiddleCenter
                    };

                    // 设置对齐方式 - 默认居中，只有明确指定时才改变
                    if (colIndex < alignment.Count &&
                        !string.IsNullOrEmpty(alignment[colIndex]))
                    {
                        string align = alignment[colIndex].ToLower();
                        if (align == "left")
                        {
                            cellStyle.alignment = TextAnchor.MiddleLeft;
                        }
                        else if (align == "right")
                        {
                            cellStyle.alignment = TextAnchor.MiddleRight;
                        }
                        else if (align == "center")
                        {
                            cellStyle.alignment = TextAnchor.MiddleCenter;
                        }
                        // 如果alignment数组中有值但不是标准的对齐方式，保持默认居中
                    }

                    // 渲染单元格 - 性能优化：重用GUIContent对象
                    g_tempContent.text = cellContent;
                    Rect cellRect = GUILayoutUtility.GetRect(g_tempContent,
                        cellStyle,
                        GUILayout.Width(columnWidths[colIndex]),
                        GUILayout.Height(Em(2f)));

                    // 绘制单元格背景和边框（在Repaint时）
                    if (Event.current.type == EventType.Repaint)
                    {
                        MarkdownStyleManager styleManager = MarkdownStyleManager.Inst;

                        // 绘制单元格背景
                        if (isHeaderRow)
                        {
                            EditorGUI.DrawRect(cellRect, styleManager.TableHeaderBackground);
                        }

                        // 绘制边框 - 使用MarkdownStyleManager的颜色
                        Color borderColor = styleManager.TableBorderColor;

                        // 顶部边框
                        if (rowIndex == 0)
                        {
                            EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, cellRect.width, 1f),
                                borderColor);
                        }

                        // 底部边框
                        EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.yMax - 1f, cellRect.width, 1f),
                            borderColor);

                        // 左边框
                        if (colIndex == 0)
                        {
                            EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.y, 1f, cellRect.height),
                                borderColor);
                        }

                        // 右边框
                        EditorGUI.DrawRect(new Rect(cellRect.xMax - 1f, cellRect.y, 1f, cellRect.height),
                            borderColor);

                        // 表头下方加粗线
                        if (isHeaderRow)
                        {
                            EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.yMax - 2f, cellRect.width, 2f),
                                styleManager.TableThickBorderColor);
                        }
                    }

                    // 渲染文本 - 使用GUI.Label来确保对齐生效
                    GUI.Label(cellRect, cellContent, cellStyle);
                }

                EditorGUILayout.EndHorizontal();
            }

            // 如果使用了滚动视图，需要结束它
            if (useScrollView)
            {
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
        }
    }
}