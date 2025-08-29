using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// Markdown编辑器窗口，提供Markdown文件的加载、预览和编辑功能
    /// </summary>
    public sealed class MarkdownEditorWindow : EditorWindow
    {
        private const string g_windowTitle = "Markdown Editor";
        private const string g_defaultFilePath = "Assets/TestFiles/sample_en.md";

        [SerializeField]
        private string m_currentFilePath;

        [SerializeField]
        private Vector2 m_viewScrollPosition;

        // GUI相关
        private bool m_isEditMode;

        // Markdown渲染器 - 通用渲染组件
        private MarkdownRenderer m_markdownRenderer;
        private string m_markdownContent;
        private Vector2 m_editScrollPosition;

        /// <summary>
        /// 窗口初始化
        /// </summary>
        private void OnEnable()
        {
            titleContent = new GUIContent(g_windowTitle);
            m_markdownRenderer = new MarkdownRenderer();

            // 设置渲染器的重绘回调
            m_markdownRenderer.SetRepaintCallback(Repaint);

            // 自动加载示例文件
            if (!File.Exists(m_currentFilePath))
            {
                m_currentFilePath = g_defaultFilePath;
            }

            if (File.Exists(m_currentFilePath))
            {
                LoadMarkdownFile(m_currentFilePath);
            }

            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        /// <summary>
        /// 窗口销毁时清理资源
        /// </summary>
        private void OnDisable()
        {
            // 渲染器内部会处理元素清理
            m_markdownRenderer?.Dispose();
            m_markdownRenderer = null;
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        private void OnGUI()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height),
                MarkdownStyleManager.Inst.BackgroundColor);

            DrawToolbar();

            EditorGUILayout.Space();

            if (m_isEditMode)
            {
                DrawEditMode();
            }
            else
            {
                DrawPreviewMode();
            }
        }

        /// <summary>
        /// 当窗口获得焦点时的处理
        /// </summary>
        private void OnFocus()
        {
            // 检查文件是否被外部修改
            if (!string.IsNullOrEmpty(m_currentFilePath) &&
                File.Exists(m_currentFilePath))
            {
                string fileContent = File.ReadAllText(m_currentFilePath);
                if (fileContent != m_markdownContent)
                {
                    if (EditorUtility.DisplayDialog("File Modified", "File was modified externally. Do you want to reload it?", "Reload", "Keep Current"))
                    {
                        m_markdownContent = fileContent;
                        // MarkdownRenderer会自动检测内容变化并重新解析
                    }
                }
            }
        }

        /// <summary>
        /// 显示窗口的菜单项
        /// </summary>
        [MenuItem("UniMarkdown/Markdown Editor/Open Markdown Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<MarkdownEditorWindow>(g_windowTitle);
            //window.minSize = new Vector2(600, 400);
            window.Show();
        }

        /// <summary>
        /// 打开指定的Markdown文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void OpenMarkdownFile(string filePath)
        {
            var window = GetWindow<MarkdownEditorWindow>(g_windowTitle);
            window.LoadMarkdownFile(filePath);
            window.Show();
            window.Focus();
        }

        /// <summary>
        /// 加载示例文件的菜单项
        /// </summary>
        [MenuItem("UniMarkdown/Markdown Editor/Load Sample File")]
        public static void LoadSampleFile()
        {
            var window = GetWindow<MarkdownEditorWindow>(g_windowTitle);
            window.LoadMarkdownFile(g_defaultFilePath);
            window.Show();
        }

        /// <summary>
        /// 绘制工具栏
        /// </summary>
        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            // 文件操作按钮
            if (GUILayout.Button("Open File", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                OpenFileDialog();
            }

            if (GUILayout.Button("Save File", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                SaveCurrentFile();
            }

            if (GUILayout.Button("Save As", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                SaveAsDialog();
            }

            GUILayout.FlexibleSpace();

            // 显示当前文件路径
            if (!string.IsNullOrEmpty(m_currentFilePath))
            {
                GUILayout.Label($"Current File: {m_currentFilePath}", EditorStyles.toolbarButton);
            }

            GUILayout.FlexibleSpace();

            // 模式切换按钮
            m_isEditMode = GUILayout.Toggle(m_isEditMode,
                "Edit Mode",
                EditorStyles.toolbarButton,
                GUILayout.Width(80));

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制编辑模式
        /// </summary>
        private void DrawEditMode()
        {
            EditorGUILayout.LabelField("Markdown Source Editing:", EditorStyles.boldLabel);

            m_editScrollPosition = EditorGUILayout.BeginScrollView(m_editScrollPosition);

            // 使用TextArea而非TextField提高大文本性能
            string newContent = EditorGUILayout.TextArea(m_markdownContent ?? string.Empty,
                GUILayout.ExpandHeight(true));

            if (newContent != null &&
                newContent != m_markdownContent)
            {
                m_markdownContent = newContent;
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("Preview Markdown", GUILayout.Height(30)))
            {
                m_isEditMode = false;
                // MarkdownRenderer会在下次渲染时自动解析新内容
            }
        }

        /// <summary>
        /// 绘制预览模式
        /// </summary>
        private void DrawPreviewMode()
        {
            EditorGUILayout.LabelField("Markdown Preview:", EditorStyles.boldLabel);

            if (string.IsNullOrEmpty(m_markdownContent))
            {
                EditorGUILayout.HelpBox("No content to display. Please load a Markdown file or switch to Edit Mode to create content.", MessageType.Info);

                if (GUILayout.Button("Load Sample File", GUILayout.Height(30)))
                {
                    LoadMarkdownFile(g_defaultFilePath);
                }
            }
            else
            {
                // 使用渲染器直接渲染Markdown文本
                m_viewScrollPosition = m_markdownRenderer.RenderMarkdown(m_markdownContent,
                    m_viewScrollPosition);
            }
        }

        /// <summary>
        /// 打开文件对话框
        /// </summary>
        private void OpenFileDialog()
        {
            string filePath = EditorUtility.OpenFilePanel("Select Markdown File", Application.dataPath, "md");

            if (!string.IsNullOrEmpty(filePath))
            {
                // 转换为相对路径
                if (filePath.StartsWith(Application.dataPath))
                {
                    filePath = "Assets" + filePath.Substring(Application.dataPath.Length);
                }

                LoadMarkdownFile(filePath);
            }
        }

        /// <summary>
        /// 保存当前文件
        /// </summary>
        private void SaveCurrentFile()
        {
            if (string.IsNullOrEmpty(m_currentFilePath))
            {
                SaveAsDialog();
                return;
            }

            try
            {
                File.WriteAllText(m_currentFilePath, m_markdownContent ?? string.Empty);
                Debug.Log($"[MarkdownEditor] File saved: {m_currentFilePath}");

                // 刷新AssetDatabase
                if (m_currentFilePath.StartsWith("Assets/"))
                {
                    AssetDatabase.ImportAsset(m_currentFilePath);
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Save Failed", $"Error saving file:\n{ex.Message}", "OK");
            }
        }

        /// <summary>
        /// 另存为对话框
        /// </summary>
        private void SaveAsDialog()
        {
            string filePath = EditorUtility.SaveFilePanel(
                "Save Markdown File",
                Application.dataPath,
                "new_markdown",
                "md");

            if (!string.IsNullOrEmpty(filePath))
            {
                // 转换为相对路径
                if (filePath.StartsWith(Application.dataPath))
                {
                    filePath = "Assets" + filePath.Substring(Application.dataPath.Length);
                }

                m_currentFilePath = filePath;
                SaveCurrentFile();
            }
        }

        /// <summary>
        /// 加载Markdown文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <summary>
        /// 加载Markdown文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void LoadMarkdownFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) ||
                !File.Exists(filePath))
            {
                EditorUtility.DisplayDialog("File Not Found", $"Invalid path or file does not exist:\n{filePath}", "OK");
                return;
            }

            try
            {
                // 清理现有内容（让MarkdownRenderer处理）
                m_markdownRenderer?.Reset();

                m_currentFilePath = filePath;
                m_markdownContent = File.ReadAllText(filePath);
                // MarkdownRenderer会在下次渲染时自动解析内容

                Debug.Log($"[MarkdownEditor] File loaded: {filePath}");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Load Failed", $"Error loading file:\n{ex.Message}", "OK");
            }
        }

        private void PlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    m_markdownRenderer.Reset();
                    Repaint();
                    break;
            }
        }
    }
}