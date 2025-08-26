using System;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// Markdown元素渲染器基类，完全独立的渲染器基础
    /// 不依赖任何外部渲染器，每个子类自己管理样式和逻辑
    /// </summary>
    public abstract class BaseElementRenderer : IElementRenderer
    {
        private Action m_repaintCallback;
        private MarkdownRenderer m_rendererMgr;

        public void SetRendererManager(MarkdownRenderer rendererMgr)
        {
            m_rendererMgr = rendererMgr;
        }

        /// <summary>
        /// 设置重绘回调
        /// </summary>
        public void SetRepaintCallback(Action repaintCallback)
        {
            m_repaintCallback = repaintCallback;
        }

        /// <summary>
        /// 渲染元素的抽象方法，子类必须实现
        /// </summary>
        /// <param name="element">要渲染的元素</param>
        /// <param name="isInline">是否为行内元素</param>
        public void Render(MarkdownElement element, bool isInline)
        {
            if (null == element ||
                !CanRender(element.elementType))
            {
                return;
            }

            OnRender(element, isInline);
        }

        /// <summary>
        /// 检查是否能够渲染指定类型的元素
        /// 默认实现：检查元素类型是否匹配支持的类型
        /// </summary>
        /// <param name="elementType">元素类型</param>
        /// <returns>是否能够渲染</returns>
        public virtual bool CanRender(MarkdownElementType elementType)
        {
            return elementType == SupportedElementType;
        }

        /// <summary>
        /// 获取支持的元素类型，子类必须实现
        /// </summary>
        public abstract MarkdownElementType SupportedElementType { get; }

        /// <summary>
        /// 渲染器优先级，默认为0
        /// 子类可以重写以调整优先级
        /// </summary>
        public virtual int Priority => 0;

        public void Dispose()
        {
            m_repaintCallback = null;
            m_rendererMgr = null;
            OnDispose();
        }

        // 样式访问方法 - 统一从MarkdownStyleManager.Inst获取
        // 这些方法调用独立的样式管理器，保持与原始MarkdownRenderer完全一致的样式

        /// <summary>
        /// 获取文本样式
        /// </summary>
        protected GUIStyle GetTextStyle()
        {
            return MarkdownStyleManager.Inst.GetTextStyle();
        }

        /// <summary>
        /// 获取指定级别的标题样式
        /// </summary>
        /// <param name="level">标题级别 (1-6)</param>
        protected GUIStyle GetHeaderStyle(int level)
        {
            return MarkdownStyleManager.Inst.GetHeaderStyle(level);
        }

        /// <summary>
        /// 获取代码块样式
        /// </summary>
        protected GUIStyle GetCodeBlockStyle()
        {
            return MarkdownStyleManager.Inst.GetCodeBlockStyle();
        }

        /// <summary>
        /// 获取行内代码样式
        /// </summary>
        protected GUIStyle GetInlineCodeStyle()
        {
            return MarkdownStyleManager.Inst.GetInlineCodeStyle();
        }

        /// <summary>
        /// 获取链接样式
        /// </summary>
        protected GUIStyle GetLinkStyle()
        {
            return MarkdownStyleManager.Inst.GetLinkStyle();
        }

        /// <summary>
        /// 获取粗体样式
        /// </summary>
        protected GUIStyle GetBoldStyle()
        {
            return MarkdownStyleManager.Inst.GetBoldStyle();
        }

        /// <summary>
        /// 获取斜体样式
        /// </summary>
        protected GUIStyle GetItalicStyle()
        {
            return MarkdownStyleManager.Inst.GetItalicStyle();
        }

        /// <summary>
        /// 获取粗斜体样式
        /// </summary>
        protected GUIStyle GetBoldItalicStyle()
        {
            return MarkdownStyleManager.Inst.GetBoldItalicStyle();
        }

        /// <summary>
        /// 获取列表样式
        /// </summary>
        protected GUIStyle GetListStyle()
        {
            return MarkdownStyleManager.Inst.GetListStyle();
        }

        /// <summary>
        /// 获取有序列表项目符号样式
        /// </summary>
        protected GUIStyle GetOrderedListBulletStyle()
        {
            return MarkdownStyleManager.Inst.GetOrderedListBulletStyle();
        }

        /// <summary>
        /// 获取有序列表项目符号样式
        /// </summary>
        protected GUIStyle GetUnorderedListBulletStyle()
        {
            return MarkdownStyleManager.Inst.GetUnorderedListBulletStyle();
        }

        protected float Em(float value)
        {
            return MarkdownStyleManager.Inst.Em(value);
        }

        protected int EmInt(float value)
        {
            return MarkdownStyleManager.Inst.EmInt(value);
        }

        protected GUILayoutOption[] GetInlineOptions()
        {
            return MarkdownStyleManager.Inst.GetInlineOptions();
        }

        protected void RenderElement(MarkdownElement element, bool isInline)
        {
            m_rendererMgr.RenderElement(element, isInline);
        }

        protected void Repaint()
        {
            m_repaintCallback?.Invoke();
        }

        protected virtual void OnDispose() { }

        protected abstract void OnRender(MarkdownElement element, bool isInline);
    }
}