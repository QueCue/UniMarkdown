using UnityEditor;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// Markdown样式管理器，从MarkdownRenderer中提取的完整样式逻辑
    /// 提供所有渲染器统一的样式访问接口，保持原始样式设计
    /// 使用单例模式，按需初始化样式
    /// </summary>
    public sealed class MarkdownStyleManager
    {
        // 标准Markdown行距规则配置
        private const int g_baseFontSize = 16; // 基准字体大小 fontSize = 16px

        private static MarkdownStyleManager g_inst;
        private static readonly object g_lock = new();
        public Color BackgroundColor { get; private set; }
        public Color CodeBackgroundColor { get; private set; }
        public Color CodeTextColor { get; private set; }
        public Color HeaderColor { get; private set; }
        public Color HeaderH6Color { get; private set; }
        public Color InlineCodeBackgroundColor { get; private set; }
        public Color InlineCodeTextColor { get; private set; }
        public Color LinkColor { get; private set; }
        public Color LinkHoverColor { get; private set; }
        public Color SecondaryTextColor; // 次要文本颜色（用于已完成任务 { get; private set; }
        public Color TextColor { get; private set; }
        
        // 表格相关颜色
        public Color TableHeaderBackground { get; private set; }
        public Color TableBorderColor { get; private set; }
        public Color TableThickBorderColor { get; private set; }

        // 基础配置缓存
        private Font m_monoFont;
        private GUILayoutOption[] m_inlineOptions;
        private GUIStyle m_scrollViewBackgroundStyle;
        private GUIStyle m_boldItalicStyle;
        private GUIStyle m_boldStyle;
        private GUIStyle m_codeBlockStyle;
        private GUIStyle m_completedTaskContentStyle; // 已完成任务内容样式
        private GUIStyle m_headerStyle1;
        private GUIStyle m_headerStyle2;
        private GUIStyle m_headerStyle3;
        private GUIStyle m_headerStyle4;
        private GUIStyle m_headerStyle5;
        private GUIStyle m_headerStyle6;
        private GUIStyle m_inlineCodeStyle;
        private GUIStyle m_italicStyle;
        private GUIStyle m_linkStyle;
        private GUIStyle m_listStyle;
        private GUIStyle m_orderedListBulletStyle;
        private GUIStyle m_taskCheckboxStyle; // 任务勾选框样式
        private GUIStyle m_taskContentStyle; // 任务内容样式
        private GUIStyle m_textStyle;
        private GUIStyle m_inlineTextStyle; // 行内文本样式
        private GUIStyle m_unorderedListBulletStyle;
        
        // 表格相关样式
        private GUIStyle m_tableContainerStyle; // 表格容器样式
        private GUIStyle m_tableHeaderStyle; // 表格表头样式
        private GUIStyle m_tableCellStyle; // 表格单元格样式

        /// <summary>
        /// 单例实例
        /// </summary>
        public static MarkdownStyleManager Inst
        {
            get
            {
                if (g_inst != null)
                {
                    return g_inst;
                }

                lock (g_lock)
                {
                    g_inst ??= new MarkdownStyleManager();
                }

                return g_inst;
            }
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private MarkdownStyleManager()
        {
            InitializeBaseConfig();
        }

        /// <summary>
        /// 初始化基础配置 - 字体和颜色设置
        /// </summary>
        private void InitializeBaseConfig()
        {
            // 字体和颜色设置 - 原始实现
            m_monoFont = EditorGUIUtility.Load("Fonts/RobotoMono/RobotoMono-Regular.ttf") as Font;

            // 主题适配逻辑 - 原始实现
            if (EditorGUIUtility.isProSkin)
            {
                // Dark Theme
                HeaderColor = new Color(0.941f, 0.965f, 0.988f, 1f); // GitHub标题白色 (#f0f6fc)
                HeaderH6Color = new Color32(145, 152, 161, 255);
                TextColor = new Color(0.941f, 0.965f, 0.988f, 1f); // GitHub文本白色 (#f0f6fc)
                CodeTextColor = new Color32(11, 200, 195, 255); // GitHub代码白色 (#f0f6fc)
                BackgroundColor = new Color(0.051f, 0.067f, 0.090f, 1f); // #0d1117
                CodeBackgroundColor = new Color(0.082f, 0.106f, 0.137f, 1f); // GitHub代码块背景 (#151b23)
                InlineCodeTextColor = new Color(0.941f, 0.965f, 0.988f, 1f); // GitHub行内代码白色 (#f0f6fc)
                InlineCodeBackgroundColor = new Color(0.118f, 0.141f, 0.165f, 1f); // GitHub行内代码背景 (#1e242a)
                LinkColor = new Color(0.267f, 0.576f, 0.973f, 1f); // GitHub链接蓝色 (#4493f8)
                LinkHoverColor = new Color(0.267f, 0.576f, 0.973f, 1f); // GitHub悬停蓝色 (#4493f8)
                SecondaryTextColor = new Color(0.7f, 0.7f, 0.7f, 1f); // 已完成任务的次要文本颜色
                
                // 表格颜色 - Dark Theme
                TableHeaderBackground = new Color(0.25f, 0.25f, 0.25f, 0.8f);
                TableBorderColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);
                TableThickBorderColor = new Color(0.5f, 0.5f, 0.5f, 0.9f);
            }
            else
            {
                // Light Theme
                HeaderColor = Color.black; // 深蓝色标题
                HeaderH6Color = new Color32(89, 99, 110, 255);
                TextColor = new Color(0.1f, 0.1f, 0.1f, 1f); // 深灰色文本
                CodeTextColor = new Color32(11, 200, 195, 255); // 深绿色代码
                BackgroundColor = Color.white; // #0d1117
                CodeBackgroundColor = new Color(0.95f, 0.97f, 0.98f, 1f); // 浅蓝灰背景
                InlineCodeTextColor = new Color(0.6f, 0.1f, 0.3f, 1f); // 深红色行内代码
                InlineCodeBackgroundColor = new Color(0.98f, 0.94f, 0.96f, 1f); // 浅粉背景
                LinkColor = new Color32(9, 105, 218, 255); // GitHub链接蓝色 (#4493f8)
                LinkHoverColor = new Color32(9, 105, 218, 255); // GitHub链接蓝色 (#4493f8)
                SecondaryTextColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 已完成任务的次要文本颜色
                
                // 表格颜色 - Light Theme
                TableHeaderBackground = new Color(0.95f, 0.95f, 0.95f, 1f);
                TableBorderColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);
                TableThickBorderColor = new Color(0.6f, 0.6f, 0.6f, 1f);
            }
        }

        /// <summary>
        /// 创建纯色纹理 - 从MarkdownRenderer提取
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="color">颜色</param>
        /// <returns>纹理对象</returns>
        private Texture2D MakeTexture(int width, int height, Color color)
        {
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        // 公共样式访问接口 - 按需初始化

        /// <summary>
        /// 获取文本样式（已弃用，请使用GetTextStyleForContext）
        /// </summary>
        [System.Obsolete("请使用GetTextStyleForContext(bool isInMixedContext)以获得更好的换行控制")]
        public GUIStyle GetTextStyle()
        {
            return GetTextStyleForContext(false);
        }

        /// <summary>
        /// 根据上下文获取合适的文本样式
        /// </summary>
        /// <param name="isInMixedContext">是否在混合行内元素上下文中</param>
        /// <returns>适合当前上下文的文本样式</returns>
        public GUIStyle GetTextStyleForContext(bool isInMixedContext)
        {
            if (isInMixedContext)
            {
                return GetInlineTextStyle();
            }
            else
            {
                return GetBlockTextStyle();
            }
        }

        /// <summary>
        /// 获取块级文本样式（用于纯文本行，支持自动换行）
        /// </summary>
        private GUIStyle GetBlockTextStyle()
        {
            if (m_textStyle == null)
            {
                m_textStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    font = m_monoFont,
                    fontSize = EmInt(1),
                    normal = { textColor = TextColor },
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    wordWrap = true
                };
            }

            return m_textStyle;
        }

        /// <summary>
        /// 获取行内文本样式（用于混合元素行，禁用自动换行）
        /// </summary>
        private GUIStyle GetInlineTextStyle()
        {
            if (m_inlineTextStyle == null)
            {
                m_inlineTextStyle = new GUIStyle(EditorStyles.label)
                {
                    font = m_monoFont,
                    fontSize = EmInt(1),
                    normal = { textColor = TextColor },
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    wordWrap = false,
                    alignment = TextAnchor.MiddleLeft
                };
            }

            return m_inlineTextStyle;
        }

        /// <summary>
        /// 获取指定级别的标题样式
        /// </summary>
        /// <param name="level">标题级别 (1-6)</param>
        public GUIStyle GetHeaderStyle(int level)
        {
            switch (level)
            {
                case 1:
                {
                    if (m_headerStyle1 == null)
                    {
                        int fontSize = EmInt(2); // H1: 32px (2em)
                        m_headerStyle1 = new GUIStyle(EditorStyles.boldLabel)
                        {
                            fontSize = fontSize,
                            font = m_monoFont,
                            normal = { textColor = HeaderColor },
                            hover = { textColor = HeaderColor },
                            padding = new RectOffset(0, 0, 0, 0),
                            margin = new RectOffset(0, 0, 0, EmInt(0.3f, fontSize)),
                            alignment = TextAnchor.LowerLeft,
                            stretchHeight = false,
                            wordWrap = true
                        };
                    }

                    return m_headerStyle1;
                }
                case 2:
                    if (m_headerStyle2 == null)
                    {
                        int fontSize = EmInt(1.5f); // H2: 24px (1.5em)
                        m_headerStyle2 = new GUIStyle(EditorStyles.boldLabel)
                        {
                            fontSize = fontSize,
                            font = m_monoFont,
                            normal = { textColor = HeaderColor },
                            hover = { textColor = HeaderColor },
                            padding = new RectOffset(0, 0, 0, 0),
                            margin = new RectOffset(0, 0, EmInt(0.8f), EmInt(0.3f, fontSize)),
                            alignment = TextAnchor.LowerLeft,
                            wordWrap = true,
                            stretchHeight = false
                        };
                    }

                    return m_headerStyle2;

                case 3:
                    if (m_headerStyle3 == null)
                    {
                        int fontSize = EmInt(1.25f); // H3: 1.25em
                        m_headerStyle3 = new GUIStyle(EditorStyles.boldLabel)
                        {
                            fontSize = fontSize,
                            font = m_monoFont,
                            normal = { textColor = HeaderColor },
                            hover = { textColor = HeaderColor },
                            padding = new RectOffset(0, 0, 0, 0),
                            margin = new RectOffset(0, 0, EmInt(0.8f), 0),
                            alignment = TextAnchor.LowerLeft,
                            wordWrap = true,
                            stretchHeight = false
                        };
                    }

                    return m_headerStyle3;

                case 4:
                    if (m_headerStyle4 == null)
                    {
                        int fontSize = EmInt(1); // H4: 16px (1em)
                        m_headerStyle4 = new GUIStyle(EditorStyles.boldLabel)
                        {
                            fontSize = fontSize,
                            font = m_monoFont,
                            normal = { textColor = HeaderColor },
                            hover = { textColor = HeaderColor },
                            padding = new RectOffset(0, 0, 0, 0),
                            margin = new RectOffset(0, 0, 0, 0),
                            alignment = TextAnchor.LowerLeft,
                            wordWrap = true,
                            stretchHeight = false
                        };
                    }

                    return m_headerStyle4;

                case 5:
                    if (m_headerStyle5 == null)
                    {
                        int fontSize = EmInt(0.875f); // H5: 0.875em
                        m_headerStyle5 = new GUIStyle(EditorStyles.boldLabel)
                        {
                            fontSize = fontSize,
                            font = m_monoFont,
                            normal = { textColor = HeaderColor },
                            hover = { textColor = HeaderColor },
                            padding = new RectOffset(0, 0, 0, 0),
                            margin = new RectOffset(0, 0, 0, 0),
                            alignment = TextAnchor.LowerLeft,
                            wordWrap = true,
                            stretchHeight = false
                        };
                    }

                    return m_headerStyle5;

                case 6:
                    if (m_headerStyle6 == null)
                    {
                        int fontSize = EmInt(0.85f); // H6: 0.85em
                        m_headerStyle6 = new GUIStyle(EditorStyles.boldLabel)
                        {
                            fontSize = fontSize,
                            font = m_monoFont,
                            normal = { textColor = HeaderH6Color },
                            hover = { textColor = HeaderH6Color },
                            padding = new RectOffset(0, 0, 0, 0),
                            margin = new RectOffset(0, 0, 0, 0),
                            alignment = TextAnchor.LowerLeft,
                            wordWrap = true,
                            stretchHeight = false
                        };
                    }

                    return m_headerStyle6;

                default:
                    return GetHeaderStyle(1);
            }
        }

        /// <summary>
        /// 获取代码块样式
        /// </summary>
        public GUIStyle GetCodeBlockStyle()
        {
            if (m_codeBlockStyle == null)
            {
                m_codeBlockStyle = new GUIStyle(GetTextStyle())
                {
                    fontSize = EmInt(0.875f),
                    font = m_monoFont,
                    fontStyle = FontStyle.Normal,
                    richText = true,
                    normal =
                    {
                        textColor = CodeTextColor,
                        background = MakeTexture(2, 2, CodeBackgroundColor)
                    },
                    padding = new RectOffset(EmInt(0.5f), EmInt(0.5f), EmInt(0.5f), EmInt(0.5f)),
                    margin = new RectOffset(0, 0, 0, 0),
                    wordWrap = false,
                    stretchHeight = true
                };
            }

            return m_codeBlockStyle;
        }

        /// <summary>
        /// 获取行内代码样式
        /// </summary>
        public GUIStyle GetInlineCodeStyle()
        {
            if (m_inlineCodeStyle == null)
            {
                m_inlineCodeStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = EmInt(0.875f),
                    font = m_monoFont,
                    normal =
                    {
                        textColor = InlineCodeTextColor,
                        background = MakeTexture(2, 2, InlineCodeBackgroundColor)
                    },
                    padding = new RectOffset(EmInt(0.25f), EmInt(0.25f), EmInt(0.125f), EmInt(0.125f)),
                    margin = new RectOffset(0, 0, 0, 0),
                    alignment = TextAnchor.MiddleLeft
                };
            }

            return m_inlineCodeStyle;
        }

        /// <summary>
        /// 获取链接样式
        /// </summary>
        public GUIStyle GetLinkStyle()
        {
            if (m_linkStyle == null)
            {
                m_linkStyle = new GUIStyle(EditorStyles.label)
                {
                    font = m_monoFont,
                    fontSize = EmInt(1),
                    normal = { textColor = LinkColor },
                    hover = { textColor = LinkHoverColor },
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    richText = true,
                    alignment = TextAnchor.MiddleLeft
                };
            }

            return m_linkStyle;
        }

        /// <summary>
        /// 获取粗体样式
        /// </summary>
        public GUIStyle GetBoldStyle()
        {
            if (m_boldStyle == null)
            {
                m_boldStyle = new GUIStyle(EditorStyles.label)
                {
                    font = m_monoFont,
                    fontSize = EmInt(1),
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = TextColor },
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }

            return m_boldStyle;
        }

        /// <summary>
        /// 获取斜体样式
        /// </summary>
        public GUIStyle GetItalicStyle()
        {
            if (m_italicStyle == null)
            {
                m_italicStyle = new GUIStyle(EditorStyles.label)
                {
                    font = m_monoFont,
                    fontSize = EmInt(1),
                    fontStyle = FontStyle.Italic,
                    normal = { textColor = TextColor },
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }

            return m_italicStyle;
        }

        /// <summary>
        /// 获取粗斜体样式
        /// </summary>
        public GUIStyle GetBoldItalicStyle()
        {
            if (m_boldItalicStyle == null)
            {
                m_boldItalicStyle = new GUIStyle(EditorStyles.label)
                {
                    font = m_monoFont,
                    fontSize = EmInt(1),
                    fontStyle = FontStyle.BoldAndItalic,
                    normal = { textColor = TextColor },
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }

            return m_boldItalicStyle;
        }

        /// <summary>
        /// 获取列表样式
        /// </summary>
        public GUIStyle GetListStyle()
        {
            if (m_listStyle == null)
            {
                m_listStyle = new GUIStyle(EditorStyles.label)
                {
                    font = m_monoFont,
                    fontSize = EmInt(1),
                    normal = { textColor = TextColor },
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    wordWrap = true
                };
            }

            return m_listStyle;
        }

        /// <summary>
        /// 获取有序列表项目符号样式
        /// </summary>
        public GUIStyle GetOrderedListBulletStyle()
        {
            if (m_orderedListBulletStyle == null)
            {
                // 项目符号样式 - 确保与文本基线对齐
                // 为有序列表使用更宽的固定宽度以适应两位数字和罗马数字
                GUIStyle listStyle = GetListStyle();
                m_orderedListBulletStyle = new GUIStyle(listStyle)
                {
                    alignment = TextAnchor.MiddleRight,
                    fixedWidth = 40f, // 增加宽度以适应罗马数字
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    stretchHeight = false,
                    wordWrap = false // 禁用文字换行
                };
            }

            return m_orderedListBulletStyle;
        }

        /// <summary>
        /// 获取无序列表项目符号样式
        /// </summary>
        public GUIStyle GetUnorderedListBulletStyle()
        {
            if (m_unorderedListBulletStyle == null)
            {
                // 项目符号样式 - 确保与文本基线对齐
                // 无序列表符号相对较短，保持原有宽度
                GUIStyle listStyle = GetListStyle();
                m_unorderedListBulletStyle = new GUIStyle(listStyle)
                {
                    alignment = TextAnchor.MiddleRight,
                    fixedWidth = 20f,
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    stretchHeight = false,
                    wordWrap = false // 禁用文字换行
                };
            }

            return m_unorderedListBulletStyle;
        }

        public GUILayoutOption[] GetInlineOptions()
        {
            if (null == m_inlineOptions)
            {
                m_inlineOptions = new[] { GUILayout.ExpandWidth(false) };
            }

            return m_inlineOptions;
        }

        public int EmInt(float value, float baseFontSize = g_baseFontSize)
        {
            return (int) Em(value, baseFontSize);
        }

        public float Em(float value, float baseFontSize = g_baseFontSize)
        {
            return value * baseFontSize;
        }

        /// <summary>
        /// 获取任务勾选框样式
        /// </summary>
        /// <returns>任务勾选框的GUIStyle</returns>
        public GUIStyle GetTaskCheckboxStyle()
        {
            if (m_taskCheckboxStyle == null)
            {
                GUIStyle textStyle = GetTextStyle();
                m_taskCheckboxStyle = new GUIStyle(textStyle)
                {
                    fontSize = textStyle.fontSize,
                    fontStyle = FontStyle.Normal,
                    normal = { textColor = textStyle.normal.textColor },
                    fixedWidth = Em(1.2f), // 勾选框固定宽度
                    alignment = TextAnchor.MiddleLeft
                };
            }

            return m_taskCheckboxStyle;
        }

        /// <summary>
        /// 获取任务内容样式
        /// </summary>
        /// <returns>任务内容的GUIStyle</returns>
        public GUIStyle GetTaskContentStyle()
        {
            if (m_taskContentStyle == null)
            {
                GUIStyle textStyle = GetTextStyle();
                m_taskContentStyle = new GUIStyle(textStyle)
                {
                    fontSize = textStyle.fontSize,
                    fontStyle = FontStyle.Normal,
                    normal = { textColor = textStyle.normal.textColor },
                    wordWrap = true,
                    alignment = TextAnchor.MiddleLeft
                };
            }

            return m_taskContentStyle;
        }

        /// <summary>
        /// 获取已完成任务内容样式
        /// </summary>
        /// <returns>已完成任务内容的GUIStyle</returns>
        public GUIStyle GetCompletedTaskContentStyle()
        {
            if (m_completedTaskContentStyle == null)
            {
                GUIStyle textStyle = GetTextStyle();
                m_completedTaskContentStyle = new GUIStyle(textStyle)
                {
                    fontSize = textStyle.fontSize,
                    fontStyle = FontStyle.Normal,
                    normal = { textColor = SecondaryTextColor }, // 使用次要文本颜色
                    wordWrap = true,
                    alignment = TextAnchor.MiddleLeft
                };
            }

            return m_completedTaskContentStyle;
        }

        public GUIStyle GetScrollViewBackgroundStyle()
        {
            if (m_scrollViewBackgroundStyle == null)
            {
                m_scrollViewBackgroundStyle = new GUIStyle
                {
                    padding = new RectOffset(10, 10, 10, 10), //这里因为是容器，就不用Em换算了
                    normal = { background = MakeTexture(2, 2, BackgroundColor) }
                };
            }

            return m_scrollViewBackgroundStyle;
        }

        /// <summary>
        /// 获取表格容器样式
        /// </summary>
        /// <returns>表格容器样式</returns>
        public GUIStyle GetTableContainerStyle()
        {
            if (m_tableContainerStyle == null)
            {
                m_tableContainerStyle = new GUIStyle
                {
                    padding = new RectOffset(0, 0, EmInt(0.3f), EmInt(0.3f)), // 使用合理的容器内边距
                    margin = new RectOffset(0, 0, EmInt(0.3f), EmInt(0.3f))   // 使用合理的容器外边距
                    // 移除背景色，避免影响布局
                };
            }

            return m_tableContainerStyle;
        }

        /// <summary>
        /// 获取表格表头样式
        /// </summary>
        /// <returns>表格表头样式</returns>
        public GUIStyle GetTableHeaderStyle()
        {
            if (m_tableHeaderStyle == null)
            {
                m_tableHeaderStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = EmInt(0.875f), // 表头使用0.875em = 14px
                    fontStyle = FontStyle.Bold,
                    font = EditorStyles.label.font, // 确保使用正确的字体
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(EmInt(0.75f), EmInt(0.75f), EmInt(0.5f), EmInt(0.5f)), // 修复padding大小
                    margin = new RectOffset(0, 0, 0, 0),
                    wordWrap = false,
                    normal = { textColor = HeaderColor },
                    richText = true
                };
            }

            return m_tableHeaderStyle;
        }

        /// <summary>
        /// 获取表格单元格样式
        /// </summary>
        /// <returns>表格单元格样式</returns>
        public GUIStyle GetTableCellStyle()
        {
            if (m_tableCellStyle == null)
            {
                m_tableCellStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = EmInt(0.875f), // 单元格使用0.875em = 14px
                    font = EditorStyles.label.font, // 确保使用正确的字体
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(EmInt(0.75f), EmInt(0.75f), EmInt(0.5f), EmInt(0.5f)), // 修复padding大小
                    margin = new RectOffset(0, 0, 0, 0),
                    wordWrap = false,
                    normal = { textColor = TextColor },
                    richText = true
                };
            }

            return m_tableCellStyle;
        }
    }
}