using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 列表项元素渲染器，专门负责渲染列表项元素
    /// 支持有序和无序列表，处理项目符号和内联元素
    /// </summary>
    public sealed class ListItemElementRenderer : BaseElementRenderer
    {
        /// <summary>
        /// 支持的元素类型：列表项
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.ListItem;

        /// <summary>
        /// 列表项渲染器的优先级
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 渲染列表项元素
        /// </summary>
        /// <param name="element">列表项元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            // 根据是否为有序列表和嵌套层级生成正确的bullet符号
            string bullet = element.isOrdered
                ? GetOrderedBullet(element.listIndex, element.nestingLevel)
                : GetUnorderedBullet(element.nestingLevel);

            GUIStyle bulletStyle
                = element.isOrdered ? GetOrderedListBulletStyle() : GetUnorderedListBulletStyle();

            using (new GUILayout.HorizontalScope())
            {
                // 新的缩进策略：纯缩进 + 前缀分离
                // 基础缩进A = 2em
                const float baseIndentUnit = 2f;
                // 每级嵌套增加B的倍数
                const float childIndentUnit = 2.5f;
                float totalIndent = baseIndentUnit + element.nestingLevel * childIndentUnit;
                float totalIndentEm = Em(totalIndent);
                GUILayout.Space(totalIndentEm - bulletStyle.fixedWidth);

                // 前缀独立处理 - 不占用固定宽度，直接内联显示
                GUILayout.Label(bullet, bulletStyle, GUILayout.ExpandWidth(false));

                // 内容紧跟前缀，增加小间距分隔
                GUILayout.Space(Em(0.3f)); // 前缀和内容间的小间距

                // 解析列表项内容中的内联元素
                List<MarkdownElement> inlineElements
                    = MarkdownParser.ParseInlineElementsOnly(element.content);

                // 文本内容容器 - 正常流式布局
                for (var i = 0; i < inlineElements.Count; i++)
                {
                    RenderElement(inlineElements[i], true);
                }

                GUILayout.FlexibleSpace(); // 填充剩余空间

                // 清理临时元素
                for (var i = 0; i < inlineElements.Count; i++)
                {
                    MarkdownElement.ReturnToPool(inlineElements[i]);
                }
            }
        }

        /// <summary>
        /// 根据嵌套层级和序号获取有序列表项目符号
        /// </summary>
        /// <param name="index">序号</param>
        /// <param name="nestingLevel">嵌套层级</param>
        /// <returns>项目符号字符串</returns>
        private string GetOrderedBullet(int index, int nestingLevel)
        {
            // 不同层级使用不同的编号样式，且紧贴文本
            switch (nestingLevel)
            {
                case 0:
                    return $"{index}."; // 一级：1.、2.、3.
                case 1:
                    return $"{ToRomanNumeral(index)}."; // 二级：i.、ii.、iii.
                default:
                    return $"{ToLowerLetter(index)}."; // 三级及以上：a.、b.、c.
            }
        }

        /// <summary>
        /// 根据嵌套层级获取无序列表项目符号
        /// </summary>
        /// <param name="nestingLevel">嵌套层级</param>
        /// <returns>项目符号字符串</returns>
        private string GetUnorderedBullet(int nestingLevel)
        {
            // 不同层级使用不同的项目符号
            switch (nestingLevel)
            {
                case 0:
                    return "● "; // 实心圆
                case 1:
                    return "○ "; // 空心圆
                default:
                    return "■ "; // 实心方块
            }
        }

        /// <summary>
        /// 将数字转换为小写罗马数字
        /// </summary>
        /// <param name="number">数字（1-based）</param>
        /// <returns>小写罗马数字</returns>
        private string ToRomanNumeral(int number)
        {
            if (number <= 0)
            {
                return "i";
            }

            var romanNumerals = new[]
            {
                new
                {
                    Value = 1000,
                    Symbol = "m"
                },
                new
                {
                    Value = 900,
                    Symbol = "cm"
                },
                new
                {
                    Value = 500,
                    Symbol = "d"
                },
                new
                {
                    Value = 400,
                    Symbol = "cd"
                },
                new
                {
                    Value = 100,
                    Symbol = "c"
                },
                new
                {
                    Value = 90,
                    Symbol = "xc"
                },
                new
                {
                    Value = 50,
                    Symbol = "l"
                },
                new
                {
                    Value = 40,
                    Symbol = "xl"
                },
                new
                {
                    Value = 10,
                    Symbol = "x"
                },
                new
                {
                    Value = 9,
                    Symbol = "ix"
                },
                new
                {
                    Value = 5,
                    Symbol = "v"
                },
                new
                {
                    Value = 4,
                    Symbol = "iv"
                },
                new
                {
                    Value = 1,
                    Symbol = "i"
                }
            };

            var result = new StringBuilder();
            foreach (var item in romanNumerals)
            {
                while (number >= item.Value)
                {
                    result.Append(item.Symbol);
                    number -= item.Value;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 将数字转换为小写字母
        /// </summary>
        /// <param name="number">数字（1-based）</param>
        /// <returns>小写字母</returns>
        private string ToLowerLetter(int number)
        {
            if (number <= 0)
            {
                return "a";
            }

            // 处理1-26的范围，超出范围循环
            int index = (number - 1) % 26;
            return ((char) ('a' + index)).ToString();
        }
    }
}