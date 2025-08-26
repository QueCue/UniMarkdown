using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 代码块元素渲染器，专门负责渲染代码块元素
    /// 完全独立的渲染逻辑，包含自己的语法高亮、语言识别和样式处理
    /// </summary>
    public sealed class CodeBlockElementRenderer : BaseElementRenderer
    {
        private const float g_copyDisplayDuration = 3f; // 3秒显示时间

        // 每个代码块实例的复制状态管理
        private static Dictionary<string, CopyState> g_copyStates = new Dictionary<string, CopyState>();

        /// <summary>
        /// 支持的元素类型：代码块
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.CodeBlock;

        /// <summary>
        /// 代码块渲染器的优先级
        /// </summary>
        public override int Priority => 30;

        /// <summary>
        /// 渲染代码块元素 - 完全提取自MarkdownRenderer.RenderCodeBlock
        /// </summary>
        /// <param name="element">代码块元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            // 应用语法高亮
            string displayContent = SyntaxManager.Inst.HighlightSyntax(element.content, element.language);

            // 计算代码块内容的实际行数
            string[] lines = element.content.Split('\n');
            int lineCount = lines.Length;

            // 添加代码块整体缩进支持
            if (element.indentLevel > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(element.indentLevel * 4); // 每个缩进层级使用4个像素的空白
            }

            // 开始代码块容器
            GUILayout.BeginVertical(GetInlineCodeStyle());

            // 语言标识和复制按钮区域 - 在顶部，固定高度
            GUILayout.BeginHorizontal(GUILayout.Height(Em(1.5f)));

            // 显示语言标识（如果有）
            string displayLanguage = SyntaxManager.Inst.GetDisplayLanguageName(element.language);
            GUILayout.Label($"  [{displayLanguage}]", GUILayout.ExpandWidth(false));

            GUILayout.FlexibleSpace(); // 推到右边

            // 为当前代码块生成唯一标识
            var codeBlockId = $"{element.content.GetHashCode()}_{element.language}";

            // 获取或创建当前代码块的复制状态
            if (!g_copyStates.TryGetValue(codeBlockId, out CopyState copyState))
            {
                copyState = new CopyState
                {
                    IsCopied = false,
                    CopyTime = 0.0
                };

                g_copyStates[codeBlockId] = copyState;
            }

            // 检查复制状态是否过期
            if (copyState.IsCopied &&
                EditorApplication.timeSinceStartup - copyState.CopyTime > g_copyDisplayDuration)
            {
                copyState.IsCopied = false;
            }

            // 计算淡入淡出透明度
            var alpha = 1.0f;
            if (copyState.IsCopied)
            {
                double elapsedTime = EditorApplication.timeSinceStartup - copyState.CopyTime;
                if (elapsedTime < 0.3) // 淡入阶段
                {
                    alpha = (float) (elapsedTime / 0.3);
                }
                else if (elapsedTime > g_copyDisplayDuration - 0.3) // 淡出阶段
                {
                    alpha = (float) ((g_copyDisplayDuration - elapsedTime) / 0.3);
                }

                alpha = Mathf.Clamp01(alpha);
            }

            // 显示"Copied!"文字（仅在复制状态时显示）
            if (copyState.IsCopied)
            {
                Color originalColor = GUI.color;
                GUI.color = new Color(0, 1, 0, alpha); // 绿色，透明度根据状态变化
                GUILayout.Label("Copied!", GUILayout.Height(Em(1f)));
                GUI.color = originalColor;
            }

            // 复制按钮
            string buttonText = copyState.IsCopied ? " ✓" : "Copy";
            if (GUILayout.Button(buttonText,
                GetInlineCodeStyle(),
                GUILayout.Width(Em(2.5f)),
                GUILayout.Height(Em(1.25f))))
            {
                // 使用Unity的系统剪贴板API
                GUIUtility.systemCopyBuffer = element.content;
                Debug.Log("[MarkdownRenderer] 代码已复制到剪贴板");

                // 更新当前代码块的复制状态
                copyState.IsCopied = true;
                copyState.CopyTime = EditorApplication.timeSinceStartup;

                // 强制重绘以显示状态变化
                EditorApplication.delayCall += () =>
                {
                    if (EditorWindow.focusedWindow != null)
                    {
                        EditorWindow.focusedWindow.Repaint();
                    }
                };
            }

            // 在复制状态期间持续重绘以维持动画
            if (copyState.IsCopied)
            {
                Repaint();
            }

            GUILayout.EndHorizontal();

            // 代码内容区域 - 使用SelectableLabel以支持选择但不可编辑
            GUIStyle readOnlyCodeStyle = GetCodeBlockStyle();

            // 精确高度计算，完全贴合内容
            // 使用实际样式的行高，如果无法获取则使用计算值
            float actualLineHeight;
            if (readOnlyCodeStyle.lineHeight > 0)
            {
                actualLineHeight = readOnlyCodeStyle.lineHeight;
            }
            else
            {
                // 根据字体大小计算行高：使用更紧凑的行高
                float fontSize = readOnlyCodeStyle.fontSize > 0 ? readOnlyCodeStyle.fontSize : Em(1);

                actualLineHeight = fontSize * 1.1f; // 进一步减少行高倍数
            }

            // 精确计算：只包含实际需要的空间
            float paddingTop = readOnlyCodeStyle.padding.top;
            float paddingBottom = readOnlyCodeStyle.padding.bottom;

            // 核心内容高度：行数 × 行高
            float coreContentHeight = lineCount * actualLineHeight;

            // 总高度：内容 + 必要的内边距，不添加任何额外缓冲
            float contentHeight = coreContentHeight + paddingTop + paddingBottom;

            // 最小高度保护：确保至少能显示一行内容
            float minHeight = actualLineHeight + paddingTop + paddingBottom;
            contentHeight = Mathf.Max(contentHeight, minHeight);

            // 使用语法高亮后的内容
            // 使用SelectableLabel来正确显示多行代码，支持选择但不可编辑
            EditorGUILayout.SelectableLabel(displayContent,
                readOnlyCodeStyle,
                GUILayout.Height(contentHeight),
                GUILayout.ExpandWidth(true));

            GUILayout.EndVertical();

            // 结束缩进容器
            if (element.indentLevel > 0)
            {
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 复制状态数据结构
        /// </summary>
        private class CopyState
        {
            public bool IsCopied;
            public double CopyTime;
        }
    }
}