using System;
using System.Text;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// Emoji元素渲染器，将emoji转换为图片URL并委托给ImageElementRenderer渲染
    /// 支持Unicode emoji转换为Apple风格的emoji图片
    /// </summary>
    public sealed class EmojiElementRenderer : BaseElementRenderer
    {
        // Emoji图片服务基础URL
        private const string g_emojiBaseURL = "https://emoji.aranja.com/static/emoji-data/img-apple-160/";

        // 复用ImageElementRenderer来实际渲染emoji图片
        private readonly ImageElementRenderer m_imageRenderer = new();

        /// <summary>
        /// 支持的元素类型：文本（用于检测其中的emoji）
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.Text;

        /// <summary>
        /// Emoji渲染器的优先级（高于普通文本渲染器）
        /// </summary>
        public override int Priority => 15;

        /// <summary>
        /// 渲染包含emoji的文本元素
        /// </summary>
        /// <param name="element">文本元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            string text = element.content; // 使用content属性而不是text
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // 检查文本中是否包含emoji
            if (!ContainsEmoji(text))
            {
                // 没有emoji，使用普通文本渲染（这里应该委托给TextElementRenderer，但为了简化先显示普通文本）
                GUILayout.Label(text);
                return;
            }

            // 处理包含emoji的文本，将emoji替换为图片
            RenderTextWithEmoji(text, isInline);
        }

        /// <summary>
        /// 检查文本是否包含emoji
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns>是否包含emoji</returns>
        private bool ContainsEmoji(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (IsEmojiAtPosition(text, i))
                {
                    return true;
                }

                // 如果是高位代理，跳过下一个字符（低位代理）
                if (char.IsHighSurrogate(text[i]))
                {
                    i++;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查指定位置是否为emoji（正确处理代理对）
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="index">位置索引</param>
        /// <returns>是否为emoji</returns>
        private bool IsEmojiAtPosition(string text, int index)
        {
            try
            {
                char c = text[index];
                int codePoint;

                // 处理代理对
                if (char.IsHighSurrogate(c) &&
                    index + 1 < text.Length &&
                    char.IsLowSurrogate(text[index + 1]))
                {
                    codePoint = char.ConvertToUtf32(c, text[index + 1]);
                }
                else if (char.IsLowSurrogate(c))
                {
                    // 低位代理单独出现，不是有效emoji
                    return false;
                }
                else
                {
                    codePoint = c;
                }

                // 常见的emoji Unicode范围（精确控制，避免误判标点符号）
                return codePoint >= 0x1F600 && codePoint <= 0x1F64F || // 表情符号
                    codePoint >= 0x1F300 && codePoint <= 0x1F5FF || // 杂项符号和图标
                    codePoint >= 0x1F680 && codePoint <= 0x1F6FF || // 交通和地图符号
                    codePoint >= 0x1F1E0 && codePoint <= 0x1F1FF || // 国旗
                    codePoint >= 0x2600 && codePoint <= 0x26FF || // 杂项符号
                    codePoint >= 0x2700 && codePoint <= 0x27BF || // 装饰符号
                    codePoint >= 0xFE00 && codePoint <= 0xFE0F || // 变体选择器
                    codePoint >= 0x1F900 && codePoint <= 0x1F9FF || // 补充符号和图标
                    codePoint >= 0x2320 && codePoint <= 0x233F || // 杂项技术符号（精确范围，包含⌨️ U+2328）
                    codePoint >= 0x2B00 && codePoint <= 0x2BFF || // 杂项符号和箭头
                    codePoint >= 0x1F000 && codePoint <= 0x1F02F || // 麻将牌
                    codePoint >= 0x1F0A0 && codePoint <= 0x1F0FF; // 扑克牌
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 渲染包含emoji的文本（将emoji转换为图片）
        /// </summary>
        /// <param name="text">包含emoji的文本</param>
        /// <param name="isInline">是否行内显示</param>
        private void RenderTextWithEmoji(string text, bool isInline)
        {
            using (new GUILayout.HorizontalScope())
            {
                var index = 0;
                while (index < text.Length)
                {
                    if (IsEmojiAtPosition(text, index))
                    {
                        // 处理emoji（可能是多字符组合）
                        string emojiSequence = ExtractEmojiSequence(text, ref index);
                        RenderEmoji(emojiSequence);
                    }
                    else
                    {
                        // 处理普通文本字符
                        string normalText = ExtractNormalText(text, ref index);
                        if (!string.IsNullOrEmpty(normalText))
                        {
                            GUILayout.Label(normalText, GUILayout.ExpandWidth(false));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 提取emoji序列（处理复合emoji）
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="index">当前索引（会被更新）</param>
        /// <returns>emoji序列</returns>
        private string ExtractEmojiSequence(string text, ref int index)
        {
            var emojiBuilder = new StringBuilder();

            while (index < text.Length)
            {
                // 如果是emoji相关字符，添加到序列中
                if (IsEmojiAtPosition(text, index) ||
                    IsEmojiModifierAtPosition(text, index))
                {
                    char currentChar = text[index];
                    emojiBuilder.Append(currentChar);
                    index++;

                    // 如果是高位代理，也要添加低位代理
                    if (char.IsHighSurrogate(currentChar) &&
                        index < text.Length &&
                        char.IsLowSurrogate(text[index]))
                    {
                        emojiBuilder.Append(text[index]);
                        index++;
                    }
                }
                else
                {
                    break;
                }
            }

            return emojiBuilder.ToString();
        }

        /// <summary>
        /// 检查指定位置是否为emoji修饰符（代理对安全版本）
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="index">位置索引</param>
        /// <returns>是否为emoji修饰符</returns>
        private bool IsEmojiModifierAtPosition(string text, int index)
        {
            try
            {
                char c = text[index];
                int codePoint;

                // 处理代理对
                if (char.IsHighSurrogate(c) &&
                    index + 1 < text.Length &&
                    char.IsLowSurrogate(text[index + 1]))
                {
                    codePoint = char.ConvertToUtf32(c, text[index + 1]);
                }
                else if (char.IsLowSurrogate(c))
                {
                    return false;
                }
                else
                {
                    codePoint = c;
                }

                return codePoint >= 0x1F3FB && codePoint <= 0x1F3FF || // 肤色修饰符
                    codePoint >= 0xFE00 && codePoint <= 0xFE0F || // 变体选择器
                    codePoint == 0x200D; // 零宽连接符
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 提取普通文本
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="index">当前索引（会被更新）</param>
        /// <returns>普通文本</returns>
        private string ExtractNormalText(string text, ref int index)
        {
            var textBuilder = new StringBuilder();

            while (index < text.Length)
            {
                if (!IsEmojiAtPosition(text, index) &&
                    !IsEmojiModifierAtPosition(text, index))
                {
                    char currentChar = text[index];
                    textBuilder.Append(currentChar);
                    index++;

                    // 如果是高位代理，也要添加低位代理（但这种情况下通常不是普通文本）
                    if (char.IsHighSurrogate(currentChar) &&
                        index < text.Length &&
                        char.IsLowSurrogate(text[index]))
                    {
                        textBuilder.Append(text[index]);
                        index++;
                    }
                }
                else
                {
                    break;
                }
            }

            return textBuilder.ToString();
        }

        /// <summary>
        /// 渲染单个emoji（转换为图片URL并使用ImageElementRenderer渲染）
        /// </summary>
        /// <param name="emoji">emoji字符</param>
        private void RenderEmoji(string emoji)
        {
            try
            {
                string emojiUrl = ConvertEmojiToImageUrl(emoji);

                // 创建虚拟的图片元素 - 使用对象池获取实例
                MarkdownElement imageElement = MarkdownElement.GetFromPool();
                imageElement.elementType = MarkdownElementType.Image;
                imageElement.url = emojiUrl;
                imageElement.altText = emoji; // 使用emoji本身作为alt文本
                imageElement.imageWidth = 16;

                // 使用ImageElementRenderer渲染emoji图片
                m_imageRenderer.Render(imageElement, true); // 行内显示

                // 归还到对象池
                MarkdownElement.ReturnToPool(imageElement);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[EMOJI] 渲染emoji失败: {emoji}, 错误: {ex.Message}");
                // 失败时回退到显示原始emoji文本
                GUILayout.Label(emoji, GUILayout.ExpandWidth(false));
            }
        }

        /// <summary>
        /// 将emoji转换为图片URL
        /// </summary>
        /// <param name="emoji">emoji字符</param>
        /// <returns>emoji图片URL</returns>
        private string ConvertEmojiToImageUrl(string emoji)
        {
            // 将emoji转换为UTF-32编码点
            var codePoints = new int[emoji.Length];
            var codePointCount = 0;

            for (var i = 0; i < emoji.Length; i++)
            {
                if (char.IsHighSurrogate(emoji[i]) &&
                    i + 1 < emoji.Length &&
                    char.IsLowSurrogate(emoji[i + 1]))
                {
                    // 处理代理对（高位+低位）
                    codePoints[codePointCount++] = char.ConvertToUtf32(emoji[i], emoji[i + 1]);
                    i++; // 跳过低位代理
                }
                else
                {
                    codePoints[codePointCount++] = emoji[i];
                }
            }

            // 转换为十六进制字符串
            var hexBuilder = new StringBuilder();
            for (var i = 0; i < codePointCount; i++)
            {
                if (i > 0)
                {
                    hexBuilder.Append("-");
                }

                hexBuilder.Append(codePoints[i].ToString("x").ToLower());
            }

            var hexCode = hexBuilder.ToString();

            // 构建完整的emoji图片URL
            return $"{g_emojiBaseURL}{hexCode}.png";
        }

        protected override void OnDispose()
        {
            m_imageRenderer?.Dispose();
        }
    }
}