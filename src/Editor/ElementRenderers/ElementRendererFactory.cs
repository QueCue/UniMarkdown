using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 元素渲染器工厂，负责创建和管理所有类型的元素渲染器
    /// 采用工厂模式 + 注册表模式，支持动态添加新的渲染器类型
    /// 完全独立设计，不依赖任何外部渲染器
    /// </summary>
    public sealed class ElementRendererFactory
    {
        // 渲染器缓存，避免重复创建
        private readonly Dictionary<MarkdownElementType, IElementRenderer> m_rendererCache;

        // 渲染器注册表，支持优先级排序
        private readonly Dictionary<MarkdownElementType, List<Type>> m_rendererRegistry;

        private readonly MarkdownRenderer m_rendererMgr;

        // 重绘回调，传递给所有渲染器
        private Action m_repaintCallback;

        /// <summary>
        /// 构造函数 - 不再需要外部依赖
        /// </summary>
        public ElementRendererFactory(MarkdownRenderer rendererMgr)
        {
            m_rendererMgr = rendererMgr;
            m_rendererCache = new Dictionary<MarkdownElementType, IElementRenderer>();
            m_rendererRegistry = new Dictionary<MarkdownElementType, List<Type>>();

            // 注册默认的渲染器
            RegisterDefaultRenderers();
        }

        /// <summary>
        /// 注册默认的元素渲染器
        /// </summary>
        private void RegisterDefaultRenderers()
        {
            RegisterRenderer<TextElementRenderer>(MarkdownElementType.Text);
            //RegisterRenderer<EmojiElementRenderer>(MarkdownElementType.Text); // Emoji渲染器，优先级更高
            RegisterRenderer<HeaderElementRenderer>(MarkdownElementType.Header);
            RegisterRenderer<CodeBlockElementRenderer>(MarkdownElementType.CodeBlock);
            RegisterRenderer<InlineCodeElementRenderer>(MarkdownElementType.InlineCode);
            RegisterRenderer<LinkElementRenderer>(MarkdownElementType.Link);
            RegisterRenderer<ImageElementRenderer>(MarkdownElementType.Image);
            RegisterRenderer<ListItemElementRenderer>(MarkdownElementType.ListItem);
            RegisterRenderer<TaskListElementRenderer>(MarkdownElementType.TaskList);
            RegisterRenderer<BoldElementRenderer>(MarkdownElementType.Bold);
            RegisterRenderer<ItalicElementRenderer>(MarkdownElementType.Italic);
            RegisterRenderer<BoldItalicElementRenderer>(MarkdownElementType.BoldItalic);
            RegisterRenderer<DivideElementRenderer>(MarkdownElementType.Divide);
            RegisterRenderer<SoftLineBreakElementRenderer>(MarkdownElementType.SoftLineBreak);
            RegisterRenderer<LineBreakElementRenderer>(MarkdownElementType.LineBreak);
            RegisterRenderer<TableElementRenderer>(MarkdownElementType.Table);
        }

        /// <summary>
        /// 设置重绘回调 - 会应用到所有渲染器
        /// </summary>
        /// <param name="repaintCallback">重绘回调</param>
        public void SetRepaintCallback(Action repaintCallback)
        {
            m_repaintCallback = repaintCallback;

            // 为已创建的渲染器设置回调
            foreach (IElementRenderer renderer in m_rendererCache.Values)
            {
                renderer.SetRepaintCallback(repaintCallback);
            }
        }

        /// <summary>
        /// 注册渲染器类型
        /// </summary>
        /// <typeparam name="T">渲染器类型</typeparam>
        /// <param name="elementType">支持的元素类型</param>
        public void RegisterRenderer<T>(MarkdownElementType elementType) where T : IElementRenderer
        {
            if (!m_rendererRegistry.TryGetValue(elementType, out List<Type> rendererList))
            {
                rendererList = new List<Type>();
                m_rendererRegistry[elementType] = rendererList;
            }

            Type rendererType = typeof(T);
            if (!rendererList.Contains(rendererType))
            {
                rendererList.Add(rendererType);

                // 按优先级排序（需要实例化来获取优先级，这里先简单处理）
                // TODO: 优化为延迟排序
            }
        }

        /// <summary>
        /// 获取指定元素类型的渲染器
        /// </summary>
        /// <param name="elementType">元素类型</param>
        /// <returns>对应的渲染器实例，如果没有找到返回null</returns>
        public IElementRenderer GetRenderer(MarkdownElementType elementType)
        {
            // 先从缓存中查找
            if (m_rendererCache.TryGetValue(elementType, out IElementRenderer cachedRenderer))
            {
                return cachedRenderer;
            }

            // 从注册表中创建新的渲染器
            if (m_rendererRegistry.TryGetValue(elementType, out List<Type> rendererTypes) &&
                rendererTypes.Count > 0)
            {
                // 取第一个注册的渲染器类型（后续可以根据优先级选择）
                Type rendererType = rendererTypes[0];
                IElementRenderer renderer = CreateRenderer(rendererType);

                if (renderer != null)
                {
                    // 缓存渲染器
                    m_rendererCache[elementType] = renderer;
                    renderer.SetRendererManager(m_rendererMgr);
                    renderer.SetRepaintCallback(m_repaintCallback);
                    return renderer;
                }
            }

            return null;
        }

        /// <summary>
        /// 创建渲染器实例
        /// </summary>
        /// <param name="rendererType">渲染器类型</param>
        /// <returns>渲染器实例</returns>
        private IElementRenderer CreateRenderer(Type rendererType)
        {
            try
            {
                // 创建独立的渲染器实例，不需要外部依赖
                return (IElementRenderer) Activator.CreateInstance(rendererType);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ElementRendererFactory] Failed to create renderer: {rendererType.Name}, error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public void ClearCache()
        {
            if (null != m_rendererCache)
            {
                foreach (IElementRenderer elementRenderer in m_rendererCache.Values)
                {
                    elementRenderer?.Dispose();
                }

                m_rendererCache.Clear();
            }
        }

        /// <summary>
        /// 获取所有已注册的元素类型
        /// </summary>
        /// <returns>元素类型列表</returns>
        public IEnumerable<MarkdownElementType> GetRegisteredElementTypes()
        {
            return m_rendererRegistry.Keys;
        }
    }
}