using System;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// Markdown元素渲染器接口，用于实现不同类型元素的渲染策略
    /// 遵循策略模式，支持元素渲染的可扩展性和职责分离
    /// </summary>
    public interface IElementRenderer : IDisposable
    {
        /// <summary>
        /// 获取渲染器支持的元素类型
        /// </summary>
        MarkdownElementType SupportedElementType { get; }

        /// <summary>
        /// 渲染器的优先级（用于处理同一元素类型的多个渲染器）
        /// 数值越高优先级越高
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 渲染指定的Markdown元素
        /// </summary>
        /// <param name="element">要渲染的元素</param>
        /// <param name="isInline">是否为行内元素</param>
        void Render(MarkdownElement element, bool isInline);

        void SetRendererManager(MarkdownRenderer rendererMgr);

        void SetRepaintCallback(Action repaintCallback);

        /// <summary>
        /// 检查当前渲染器是否能够处理指定类型的元素
        /// </summary>
        /// <param name="elementType">元素类型</param>
        /// <returns>如果能够处理返回true，否则返回false</returns>
        bool CanRender(MarkdownElementType elementType);
    }
}