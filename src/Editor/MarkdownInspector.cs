using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// Markdown文件的自定义Inspector，在Inspector面板中直接渲染Markdown内容
    /// </summary>
    [CustomEditor(typeof(TextAsset))]
    public class MarkdownInspector : UnityEditor.Editor
    {
        [SerializeField]
        private Vector2 m_scrollPosition;

        private bool m_isMarkdownFile;

        // 渲染设置
        private bool m_showMarkdownPreview = true;
        private bool m_showOriginalInspector;
        private UnityEditor.Editor m_defaultEditor;
        private MarkdownRenderer m_markdownRenderer;
        private string m_assetPath;

        // 性能优化缓存
        private string m_cachedMarkdownContent;
        private int m_lastRepaintFrame;

        private void Awake()
        {
            // 从EditorPrefs读取设置
            m_showMarkdownPreview = EditorPrefs.GetBool("MarkdownInspector.ShowPreview", true);
            m_showOriginalInspector = EditorPrefs.GetBool("MarkdownInspector.ShowOriginal", false);
        }

        private void OnEnable()
        {
            // 获取资源路径
            m_assetPath = AssetDatabase.GetAssetPath(target);

            // 检查是否为Markdown文件
            string extension = Path.GetExtension(m_assetPath).ToLower();
            m_isMarkdownFile = extension == ".md" || extension == ".markdown";

            if (m_isMarkdownFile)
            {
                // 初始化Markdown渲染器
                m_markdownRenderer = new MarkdownRenderer();
                m_markdownRenderer.SetRepaintCallback(Repaint);
            }

            // 创建默认的TextAsset Inspector作为备用
            Assembly assembly = typeof(UnityEditor.Editor).Assembly;
            Type defaultType = assembly.GetType("UnityEditor.TextAssetInspector");
            CreateCachedEditor(target, defaultType, ref m_defaultEditor);
        }

        private void OnDisable()
        {
            // 清理资源
            m_markdownRenderer?.Dispose();
            m_markdownRenderer = null;

            if (m_defaultEditor != null)
            {
                DestroyImmediate(m_defaultEditor);
                m_defaultEditor = null;
            }
        }

        public override void OnInspectorGUI()
        {
            if (!m_isMarkdownFile)
            {
                // 不是Markdown文件，显示默认Inspector
                if (m_defaultEditor != null)
                {
                    m_defaultEditor.OnInspectorGUI();
                }

                return;
            }

            bool oriEnabled = GUI.enabled;
            GUI.enabled = true;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;

            // 绘制标题和控制选项
            DrawHeaderAndControls();

            if (m_showMarkdownPreview)
            {
                //DrawMarkdownPreview();
                var textAsset = target as TextAsset;
                if (!textAsset ||
                    string.IsNullOrEmpty(textAsset.text))
                {
                    EditorGUILayout.HelpBox("Markdown file is empty or cannot be read", MessageType.Info);
                }
                else
                {
                    RenderMarkdownOptimized(textAsset.text);
                }
            }

            GUI.enabled = oriEnabled;
            if (m_showOriginalInspector)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Original Inspector", EditorStyles.boldLabel);
                if (m_defaultEditor != null)
                {
                    m_defaultEditor.OnInspectorGUI();
                }
            }
        }

        private void DrawHeaderAndControls()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // 控制选项
            EditorGUILayout.BeginHorizontal();

            bool newShowPreview
                = EditorGUILayout.ToggleLeft("Show Markdown Preview", m_showMarkdownPreview, GUILayout.Width(150));

            if (newShowPreview != m_showMarkdownPreview)
            {
                m_showMarkdownPreview = newShowPreview;
                EditorPrefs.SetBool("MarkdownInspector.ShowPreview", m_showMarkdownPreview);
            }

            bool newShowOriginal
                = EditorGUILayout.ToggleLeft("Show Original Inspector", m_showOriginalInspector, GUILayout.Width(150));

            if (newShowOriginal != m_showOriginalInspector)
            {
                m_showOriginalInspector = newShowOriginal;
                EditorPrefs.SetBool("MarkdownInspector.ShowOriginal", m_showOriginalInspector);
            }

            EditorGUILayout.EndHorizontal();

            // 快捷操作按钮
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Open in Editor", GUILayout.Width(120)))
            {
                AssetDatabase.OpenAsset(target);
            }

            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                m_markdownRenderer?.Reset();
                Repaint();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 优化的 Markdown 渲染方法，减少不必要的重绘
        /// </summary>
        /// <param name="markdownText">Markdown 文本内容</param>
        private void RenderMarkdownOptimized(string markdownText)
        {
            // 检查内容是否发生变化
            bool contentChanged = m_cachedMarkdownContent != markdownText;
            
            // 检查是否需要重新渲染（内容变化或强制刷新）
            bool shouldRender = contentChanged || ShouldForceRepaint();
            
            if (contentChanged)
            {
                m_cachedMarkdownContent = markdownText;
            }
            
            if (shouldRender)
            {
                m_lastRepaintFrame = Time.frameCount;
            }
            
            // 执行渲染
            m_scrollPosition = m_markdownRenderer.RenderMarkdown(markdownText, m_scrollPosition);
        }

        /// <summary>
        /// 判断是否需要强制重绘
        /// </summary>
        /// <returns>是否需要强制重绘</returns>
        private bool ShouldForceRepaint()
        {
            // 避免连续帧的重复渲染，限制最大重绘频率
            int currentFrame = Time.frameCount;
            int framesSinceLastRepaint = currentFrame - m_lastRepaintFrame;
            
            // 最多每 3 帧渲染一次，除非内容有变化
            return framesSinceLastRepaint >= 3;
        }
    }
}