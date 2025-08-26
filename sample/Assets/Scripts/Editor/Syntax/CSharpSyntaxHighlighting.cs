using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 高级C#语法高亮，支持完整的语法元素识别和配色
    /// </summary>
    public sealed class CSharpSyntaxHighlighting : ISyntaxHighlighting
    {
        // 正则表达式缓存
        private static readonly Regex g_numberRegex
            = new(@"\b\d+(\.\d+)?[fFdDlLuU]?\b", RegexOptions.Compiled);

        private static readonly Regex g_identifierRegex
            = new(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", RegexOptions.Compiled);

        private static readonly Regex g_preprocessorRegex
            = new(@"^\s*#\w+.*$", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex g_attributeRegex = new(@"\[@\w+.*?\]", RegexOptions.Compiled);

        private static readonly Regex g_xmlDocRegex
            = new(@"///.*$", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex g_genericTypeRegex
            = new(@"\b[A-Z][a-zA-Z0-9_]*<[^>]+>", RegexOptions.Compiled);

        // 缓存语法高亮颜色
        private Dictionary<string, string> m_syntaxColors;

        // 缓存C#关键字集合
        private HashSet<string> m_csharpKeywords = new()
        {
            "abstract",
            "as",
            "base",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "class",
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "virtual",
            "void",
            "volatile",
            "while",
            "add",
            "alias",
            "ascending",
            "async",
            "await",
            "descending",
            "dynamic",
            "from",
            "get",
            "global",
            "group",
            "into",
            "join",
            "let",
            "orderby",
            "partial",
            "remove",
            "select",
            "set",
            "value",
            "var",
            "where",
            "yield"
        };

        // 缓存C#基本类型
        private HashSet<string> m_csharpTypes = new()
        {
            "bool",
            "byte",
            "sbyte",
            "char",
            "decimal",
            "double",
            "float",
            "int",
            "uint",
            "long",
            "ulong",
            "object",
            "short",
            "ushort",
            "string",
            "void",
            "var",
            "dynamic"
        };

        // 缓存常用框架类型
        private HashSet<string> m_frameworkTypes = new()
        {
            "List",
            "Dictionary",
            "HashSet",
            "Array",
            "IEnumerable",
            "ICollection",
            "IList",
            "IDictionary",
            "StringBuilder",
            "Regex",
            "DateTime",
            "TimeSpan",
            "Guid",
            "Exception",
            "ArgumentException",
            "InvalidOperationException",
            "NotImplementedException",
            "NotSupportedException",
            "Task",
            "Action",
            "Func",
            "Predicate",
            "Comparison",
            "EventHandler",
            "PropertyChangedEventArgs",
            "Component",
            "MonoBehaviour",
            "ScriptableObject",
            "GameObject",
            "Transform",
            "Vector2",
            "Vector3",
            "Vector4",
            "Quaternion",
            "Color",
            "Texture2D",
            "Material",
            "Mesh",
            "Renderer"
        };

        public CSharpSyntaxHighlighting()
        {
            InitializeSyntaxColors();
        }

        public string Highlight(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return code;
            }

            var result = new StringBuilder(code.Length * 3);
            string[] lines = code.Split('\n');

            var inBlockComment = false;
            var inXmlDoc = false;

            for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                if (lineIndex > 0)
                {
                    result.Append('\n');
                }

                string line = lines[lineIndex];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                ProcessAdvancedCSharpLine(line, result, ref inBlockComment, ref inXmlDoc);
            }

            return result.ToString();
        }

        public void Dispose()
        {
            m_csharpKeywords?.Clear();
            m_csharpKeywords = null;
            m_csharpTypes?.Clear();
            m_csharpTypes = null;
            m_frameworkTypes?.Clear();
            m_frameworkTypes = null;
            m_syntaxColors?.Clear();
            m_syntaxColors = null;
        }

        /// <summary>
        /// 初始化完整的语法高亮颜色配置
        /// </summary>
        private void InitializeSyntaxColors()
        {
            m_syntaxColors = new Dictionary<string, string>();

            // 基于VS Code OneDark主题的完整配色方案
            bool isDarkTheme = EditorGUIUtility.isProSkin;
            if (isDarkTheme)
            {
                // OneDark深色主题配色
                m_syntaxColors["keyword"] = "#D55FDE"; // 关键字 - 紫色
                m_syntaxColors["identifier"] = "#BBBBBB"; // 标识符 - 红色
                m_syntaxColors["string"] = "#89CA78"; // 字符串 - 绿色
                m_syntaxColors["number"] = "#D19A66"; // 数字 - 橙色
                m_syntaxColors["operator"] = "#BBBBBB"; // 运算符 - 青色
                m_syntaxColors["overloaded_operator"] = "#61AFEF"; // 重载运算符 - 蓝色
                m_syntaxColors["punctuation"] = "#BBBBBB"; // 标点符号 - 灰白色
                m_syntaxColors["comment"] = "#49C8C3"; // 行注释 - 灰色
                m_syntaxColors["block_comment"] = "#49C8C3"; // 块注释 - 灰色
                m_syntaxColors["xml_doc"] = "#49C8C3"; // XML文档注释 - 深灰色
                m_syntaxColors["xml_tag"] = "#49C8C3"; // XML标签 - 红色
                m_syntaxColors["preprocessor"] = "#D55FDE"; // 预处理器 - 紫色
                m_syntaxColors["label"] = "#BBBBBB"; // 标签 - 橙色
                m_syntaxColors["constant"] = "#D19A66"; // 常量 - 橙色
                m_syntaxColors["global_variable"] = "#EF596F"; // 全局变量 - 黄色
                m_syntaxColors["function"] = "#61AFEF"; // 函数声明 - 蓝色
                m_syntaxColors["function_call"] = "#61AFEF"; // 函数调用 - 蓝色
                m_syntaxColors["event"] = "#E06C75"; // 事件标识符 - 红色
                m_syntaxColors["local_variable"] = "#BBBBBB"; // 局部变量 - 灰白色
                m_syntaxColors["reassigned_local"] = "#BBBBBB"; // 重新赋值的局部变量 - 灰白色
                m_syntaxColors["parameter"] = "#D19A66"; // 参数 - 红色
                m_syntaxColors["reassigned_parameter"] = "#D19A66"; // 重新赋值的参数 - 红色
                m_syntaxColors["namespace"] = "#BBBBBB"; // 命名空间 - 灰白色
                m_syntaxColors["interface"] = "#E5C07B"; // 接口名 - 淡绿色
                m_syntaxColors["class"] = "#E5C07B"; // 类名 - 黄色
                m_syntaxColors["delegate"] = "#E5C07B"; // 委托名 - 黄色
                m_syntaxColors["enum"] = "#E5C07B"; // 枚举名 - 黄色
                m_syntaxColors["struct"] = "#E5C07B"; // 结构体名 - 黄色
                m_syntaxColors["instance_method"] = "#61AFEF"; // 实例方法 - 蓝色
                m_syntaxColors["instance_field"] = "#EF596F"; // 实例字段 - 红色
                m_syntaxColors["instance_property"] = "#EF596F"; // 实例属性 - 红色
                m_syntaxColors["static_method"] = "#61AFEF"; // 静态方法 - 蓝色
                m_syntaxColors["static_field"] = "#EF596F"; // 静态字段 - 红色
                m_syntaxColors["generic_type"] = "#E5C07B"; // 泛型类型 - 黄色
                m_syntaxColors["generic_parameter"] = "#D19A66"; // 泛型参数 - 橙色
                m_syntaxColors["attribute"] = "#89CA78"; // 特性 - 黄色
                m_syntaxColors["template_entity"] = "#EF596F"; // 模板实体 - 绿色
                m_syntaxColors["template_language"] = "#E5C07B"; // 模板语言 - 紫色
            }
            else
            {
                // 浅色主题配色
                m_syntaxColors["keyword"] = "#0000FF"; // 关键字 - 蓝色
                m_syntaxColors["identifier"] = "#000000"; // 标识符 - 黑色
                m_syntaxColors["string"] = "#A31515"; // 字符串 - 深红色
                m_syntaxColors["number"] = "#098658"; // 数字 - 深绿色
                m_syntaxColors["operator"] = "#000000"; // 运算符 - 黑色
                m_syntaxColors["overloaded_operator"] = "#267F99"; // 重载运算符 - 深青色
                m_syntaxColors["punctuation"] = "#000000"; // 标点符号 - 黑色
                m_syntaxColors["comment"] = "#008000"; // 行注释 - 绿色
                m_syntaxColors["block_comment"] = "#008000"; // 块注释 - 绿色
                m_syntaxColors["xml_doc"] = "#808080"; // XML文档注释 - 灰色
                m_syntaxColors["xml_tag"] = "#800000"; // XML标签 - 深红色
                m_syntaxColors["preprocessor"] = "#9B9B9B"; // 预处理器 - 灰色
                m_syntaxColors["label"] = "#000000"; // 标签 - 黑色
                m_syntaxColors["constant"] = "#000000"; // 常量 - 黑色
                m_syntaxColors["global_variable"] = "#000000"; // 全局变量 - 黑色
                m_syntaxColors["function"] = "#795E26"; // 函数声明 - 棕色
                m_syntaxColors["function_call"] = "#795E26"; // 函数调用 - 棕色
                m_syntaxColors["event"] = "#000000"; // 事件标识符 - 黑色
                m_syntaxColors["local_variable"] = "#000000"; // 局部变量 - 黑色
                m_syntaxColors["reassigned_local"] = "#000000"; // 重新赋值的局部变量 - 黑色
                m_syntaxColors["parameter"] = "#000000"; // 参数 - 黑色
                m_syntaxColors["reassigned_parameter"] = "#000000"; // 重新赋值的参数 - 黑色
                m_syntaxColors["namespace"] = "#000000"; // 命名空间 - 黑色
                m_syntaxColors["interface"] = "#2B91AF"; // 接口名 - 蓝色
                m_syntaxColors["class"] = "#2B91AF"; // 类名 - 蓝色
                m_syntaxColors["delegate"] = "#2B91AF"; // 委托名 - 蓝色
                m_syntaxColors["enum"] = "#2B91AF"; // 枚举名 - 蓝色
                m_syntaxColors["struct"] = "#2B91AF"; // 结构体名 - 蓝色
                m_syntaxColors["instance_method"] = "#795E26"; // 实例方法 - 棕色
                m_syntaxColors["instance_field"] = "#000000"; // 实例字段 - 黑色
                m_syntaxColors["instance_property"] = "#000000"; // 实例属性 - 黑色
                m_syntaxColors["static_method"] = "#795E26"; // 静态方法 - 棕色
                m_syntaxColors["static_field"] = "#000000"; // 静态字段 - 黑色
                m_syntaxColors["generic_type"] = "#2B91AF"; // 泛型类型 - 蓝色
                m_syntaxColors["generic_parameter"] = "#2B91AF"; // 泛型参数 - 蓝色
                m_syntaxColors["attribute"] = "#2B91AF"; // 特性 - 蓝色
                m_syntaxColors["template_entity"] = "#A31515"; // 模板实体 - 深红色
                m_syntaxColors["template_language"] = "#0000FF"; // 模板语言 - 蓝色
            }
        }

        /// <summary>
        /// 处理高级C#语法高亮
        /// </summary>
        private void ProcessAdvancedCSharpLine(string line, StringBuilder result, ref bool inBlockComment,
            ref bool inXmlDoc)
        {
            var i = 0;
            while (i < line.Length)
            {
                char c = line[i];

                // 处理块注释
                if (inBlockComment)
                {
                    if (c == '*' &&
                        i + 1 < line.Length &&
                        line[i + 1] == '/')
                    {
                        result.Append($"<color={m_syntaxColors["block_comment"]}><i>*/</i></color>");
                        inBlockComment = false;
                        i += 2;
                        continue;
                    }

                    result.Append($"<color={m_syntaxColors["block_comment"]}><i>{c}</i></color>");
                    i++;
                    continue;
                }

                // 处理XML文档注释
                if (c == '/' &&
                    i + 2 < line.Length &&
                    line[i + 1] == '/' &&
                    line[i + 2] == '/')
                {
                    string xmlComment = line.Substring(i);
                    result.Append(ProcessXmlDocComment(xmlComment));
                    break;
                }

                // 处理单行注释
                if (c == '/' &&
                    i + 1 < line.Length &&
                    line[i + 1] == '/')
                {
                    string comment = line.Substring(i);
                    result.Append($"<color={m_syntaxColors["comment"]}><i>{comment}</i></color>");
                    break;
                }

                // 处理块注释开始
                if (c == '/' &&
                    i + 1 < line.Length &&
                    line[i + 1] == '*')
                {
                    inBlockComment = true;
                    result.Append($"<color={m_syntaxColors["block_comment"]}><i>/*</i></color>");
                    i += 2;
                    continue;
                }

                // 处理字符串字面量
                if (c == '"')
                {
                    int stringEnd = FindStringEnd(line, i);
                    string str = line.Substring(i, stringEnd - i + 1);
                    result.Append($"<color={m_syntaxColors["string"]}>{str}</color>");
                    i = stringEnd + 1;
                    continue;
                }

                // 处理字符字面量
                if (c == '\'' &&
                    i + 2 < line.Length &&
                    line[i + 2] == '\'')
                {
                    string charLiteral = line.Substring(i, 3);
                    result.Append($"<color={m_syntaxColors["string"]}>{charLiteral}</color>");
                    i += 3;
                    continue;
                }

                // 处理预处理器指令
                if (c == '#' &&
                    IsLineStart(line, i))
                {
                    int directiveEnd = FindDirectiveEnd(line, i);
                    string directive = line.Substring(i, directiveEnd - i);
                    result.Append($"<color={m_syntaxColors["preprocessor"]}>{directive}</color>");
                    i = directiveEnd;
                    continue;
                }

                // 处理特性
                if (c == '@' &&
                    i + 1 < line.Length &&
                    char.IsLetter(line[i + 1]))
                {
                    int attrEnd = FindAttributeEnd(line, i);
                    string attribute = line.Substring(i, attrEnd - i);
                    result.Append($"<color={m_syntaxColors["attribute"]}>{attribute}</color>");
                    i = attrEnd;
                    continue;
                }

                // 处理标识符、关键字、类型等
                if (char.IsLetter(c) ||
                    c == '_')
                {
                    int identifierEnd = FindIdentifierEnd(line, i);
                    string identifier = line.Substring(i, identifierEnd - i);

                    // 检查是否为标签（标识符后跟冒号）
                    if (IsLabel(line, identifierEnd))
                    {
                        result.Append($"<color={m_syntaxColors["label"]}>{identifier}</color>");
                        i = identifierEnd;
                        continue;
                    }

                    result.Append(ProcessIdentifier(identifier, line, i, identifierEnd));
                    i = identifierEnd;
                    continue;
                }

                // 处理数字
                if (char.IsDigit(c))
                {
                    int numberEnd = FindNumberEnd(line, i);
                    string number = line.Substring(i, numberEnd - i);
                    result.Append($"<color={m_syntaxColors["number"]}>{number}</color>");
                    i = numberEnd;
                    continue;
                }

                // 处理运算符
                if (IsOperator(c))
                {
                    string operatorColor
                        = IsOverloadableOperator(c, line, i) ? "overloaded_operator" : "operator";

                    result.Append($"<color={m_syntaxColors[operatorColor]}>{c}</color>");
                    i++;
                    continue;
                }

                // 处理标点符号
                if (IsPunctuation(c))
                {
                    result.Append($"<color={m_syntaxColors["punctuation"]}>{c}</color>");
                    i++;
                    continue;
                }

                // 其他字符直接添加
                result.Append(c);
                i++;
            }
        }

        /// <summary>
        /// 处理标识符并确定其类型
        /// </summary>
        private string ProcessIdentifier(string identifier, string line, int start, int end)
        {
            // 关键字检查
            if (m_csharpKeywords.Contains(identifier))
            {
                return $"<color={m_syntaxColors["keyword"]}>{identifier}</color>";
            }

            // 基本类型检查
            if (m_csharpTypes.Contains(identifier))
            {
                return $"<color={m_syntaxColors["class"]}>{identifier}</color>";
            }

            // 框架类型检查
            if (m_frameworkTypes.Contains(identifier))
            {
                return $"<color={m_syntaxColors["class"]}>{identifier}</color>";
            }

            // 函数调用检查
            if (IsFunctionCall(line, end))
            {
                // 检查是否为静态方法调用
                if (IsStaticMethodCall(line, start))
                {
                    return $"<color={m_syntaxColors["static_method"]}>{identifier}</color>";
                }

                return $"<color={m_syntaxColors["function_call"]}>{identifier}</color>";
            }

            // 命名空间检查
            if (IsNamespace(line, start, end))
            {
                return $"<color={m_syntaxColors["namespace"]}>{identifier}</color>";
            }

            // 接口名检查（以I开头的大写标识符）
            if (IsInterfaceName(identifier))
            {
                return $"<color={m_syntaxColors["interface"]}>{identifier}</color>";
            }

            // 枚举类型检查
            if (IsEnumType(identifier, line, start))
            {
                return $"<color={m_syntaxColors["enum"]}>{identifier}</color>";
            }

            // 结构体类型检查
            if (IsStructType(identifier, line, start))
            {
                return $"<color={m_syntaxColors["struct"]}>{identifier}</color>";
            }

            // 委托类型检查
            if (IsDelegateType(identifier, line, start))
            {
                return $"<color={m_syntaxColors["delegate"]}>{identifier}</color>";
            }

            // 类名检查（首字母大写）
            if (IsClassName(identifier))
            {
                return $"<color={m_syntaxColors["class"]}>{identifier}</color>";
            }

            // 常量检查（全大写）
            if (IsConstant(identifier))
            {
                return $"<color={m_syntaxColors["constant"]}>{identifier}</color>";
            }

            // 字段检查
            if (IsField(identifier, line, start))
            {
                if (IsStaticField(line, start))
                {
                    return $"<color={m_syntaxColors["static_field"]}>{identifier}</color>";
                }

                return $"<color={m_syntaxColors["instance_field"]}>{identifier}</color>";
            }

            // 属性检查
            if (IsProperty(identifier, line, start))
            {
                return $"<color={m_syntaxColors["instance_property"]}>{identifier}</color>";
            }

            // 事件检查
            if (IsEvent(identifier, line, start))
            {
                return $"<color={m_syntaxColors["event"]}>{identifier}</color>";
            }

            // 参数检查（基于上下文）
            if (IsParameter(identifier, line, start))
            {
                if (IsReassignedParameter(identifier, line))
                {
                    return $"<color={m_syntaxColors["reassigned_parameter"]}>{identifier}</color>";
                }

                return $"<color={m_syntaxColors["parameter"]}>{identifier}</color>";
            }

            // 全局变量检查
            if (IsGlobalVariable(identifier, line, start))
            {
                return $"<color={m_syntaxColors["global_variable"]}>{identifier}</color>";
            }

            // 局部变量检查
            if (IsReassignedLocal(identifier, line))
            {
                return $"<color={m_syntaxColors["reassigned_local"]}>{identifier}</color>";
            }

            // 默认为局部变量或标识符
            if (char.IsLower(identifier[0]))
            {
                return $"<color={m_syntaxColors["local_variable"]}>{identifier}</color>";
            }

            // 其他标识符
            return $"<color={m_syntaxColors["identifier"]}>{identifier}</color>";
        }

        /// <summary>
        /// 处理XML文档注释
        /// </summary>
        private string ProcessXmlDocComment(string xmlComment)
        {
            var result = new StringBuilder();
            var i = 0;
            var inTag = false;

            while (i < xmlComment.Length)
            {
                char c = xmlComment[i];

                if (c == '<')
                {
                    inTag = true;
                    result.Append($"<color={m_syntaxColors["xml_tag"]}><i><</i></color>");
                }
                else if (c == '>')
                {
                    inTag = false;
                    result.Append($"<color={m_syntaxColors["xml_tag"]}><i>></i></color>");
                }
                else if (inTag)
                {
                    result.Append($"<color={m_syntaxColors["xml_tag"]}><i>{c}</i></color>");
                }
                else
                {
                    result.Append($"<color={m_syntaxColors["xml_doc"]}><i>{c}</i></color>");
                }

                i++;
            }

            return result.ToString();
        }

        #region 辅助方法

        /// <summary>
        /// 查找字符串结尾位置
        /// </summary>
        private int FindStringEnd(string line, int start)
        {
            for (int i = start + 1; i < line.Length; i++)
            {
                if (line[i] == '"' &&
                    line[i - 1] != '\\')
                {
                    return i;
                }
            }

            return line.Length - 1;
        }

        /// <summary>
        /// 判断是否为行首
        /// </summary>
        private bool IsLineStart(string line, int pos)
        {
            for (var i = 0; i < pos; i++)
            {
                if (!char.IsWhiteSpace(line[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 查找预处理器指令结尾
        /// </summary>
        private int FindDirectiveEnd(string line, int start)
        {
            for (int i = start; i < line.Length; i++)
            {
                if (char.IsWhiteSpace(line[i]) &&
                    line[i] != ' ')
                {
                    return i;
                }
            }

            return line.Length;
        }

        /// <summary>
        /// 查找特性结尾
        /// </summary>
        private int FindAttributeEnd(string line, int start)
        {
            for (int i = start; i < line.Length; i++)
            {
                char c = line[i];
                if (!char.IsLetterOrDigit(c) &&
                    c != '_' &&
                    c != '@')
                {
                    return i;
                }
            }

            return line.Length;
        }

        /// <summary>
        /// 查找标识符结尾位置
        /// </summary>
        private int FindIdentifierEnd(string line, int start)
        {
            for (int i = start; i < line.Length; i++)
            {
                char c = line[i];
                if (!char.IsLetterOrDigit(c) &&
                    c != '_')
                {
                    return i;
                }
            }

            return line.Length;
        }

        /// <summary>
        /// 查找数字结尾位置
        /// </summary>
        private int FindNumberEnd(string line, int start)
        {
            for (int i = start; i < line.Length; i++)
            {
                char c = line[i];
                if (!char.IsDigit(c) &&
                    c != '.' &&
                    c != 'f' &&
                    c != 'F' &&
                    c != 'd' &&
                    c != 'D' &&
                    c != 'l' &&
                    c != 'L' &&
                    c != 'u' &&
                    c != 'U')
                {
                    return i;
                }
            }

            return line.Length;
        }

        /// <summary>
        /// 判断是否为运算符
        /// </summary>
        private bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '=' || c == '!' ||
                c == '<' || c == '>' || c == '&' || c == '|' || c == '^' || c == '~' || c == '?';
        }

        /// <summary>
        /// 判断是否为可重载运算符
        /// </summary>
        private bool IsOverloadableOperator(char c, string line, int pos)
        {
            // 简化的重载运算符检测逻辑
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '=' || c == '!' ||
                c == '<' || c == '>' || c == '&' || c == '|' || c == '^' || c == '~';
        }

        /// <summary>
        /// 判断是否为标点符号
        /// </summary>
        private bool IsPunctuation(char c)
        {
            return c == ':' || c == ';' || c == ',' || c == '.' || c == '(' || c == ')' || c == '[' ||
                c == ']' || c == '{' || c == '}';
        }

        /// <summary>
        /// 判断是否为函数调用
        /// </summary>
        private bool IsFunctionCall(string line, int identifierEnd)
        {
            for (int i = identifierEnd; i < line.Length; i++)
            {
                char c = line[i];
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                return c == '(';
            }

            return false;
        }

        /// <summary>
        /// 判断是否为接口名
        /// </summary>
        private bool IsInterfaceName(string identifier)
        {
            return identifier.Length > 1 && identifier[0] == 'I' && char.IsUpper(identifier[1]);
        }

        /// <summary>
        /// 判断是否为类名
        /// </summary>
        private bool IsClassName(string identifier)
        {
            if (string.IsNullOrEmpty(identifier) ||
                identifier.Length < 2)
            {
                return false;
            }

            return char.IsUpper(identifier[0]) && !m_csharpKeywords.Contains(identifier) &&
                !m_csharpTypes.Contains(identifier);
        }

        /// <summary>
        /// 判断是否为常量
        /// </summary>
        private bool IsConstant(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return false;
            }

            // 检查是否全大写（允许下划线）
            for (var i = 0; i < identifier.Length; i++)
            {
                char c = identifier[i];
                if (!char.IsUpper(c) &&
                    c != '_' &&
                    !char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断是否为参数（简化检测）
        /// </summary>
        private bool IsParameter(string identifier, string line, int start)
        {
            // 简化的参数检测逻辑：如果在函数声明中且是小写开头
            return char.IsLower(identifier[0]) && line.Contains("(") && line.Contains(")");
        }

        /// <summary>
        /// 判断是否为静态方法调用
        /// </summary>
        private bool IsStaticMethodCall(string line, int start)
        {
            // 查找前面是否有类名.方法名的模式
            for (int i = start - 1; i >= 0; i--)
            {
                if (line[i] == '.')
                {
                    // 找到点号，检查前面是否为大写字母开头的标识符
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (char.IsWhiteSpace(line[j]))
                        {
                            continue;
                        }

                        if (char.IsUpper(line[j]))
                        {
                            return true;
                        }

                        break;
                    }
                }
                else if (!char.IsLetterOrDigit(line[i]) &&
                    line[i] != '_')
                {
                    break;
                }
            }

            return false;
        }

        /// <summary>
        /// 判断是否为命名空间
        /// </summary>
        private bool IsNamespace(string line, int start, int end)
        {
            // 查找using语句或namespace关键字后的标识符
            string beforeIdentifier = start > 0 ? line.Substring(0, start).Trim() : "";
            return beforeIdentifier.EndsWith("using") || beforeIdentifier.EndsWith("namespace") ||
                beforeIdentifier.Contains("using") && beforeIdentifier.Contains(".");
        }

        /// <summary>
        /// 判断是否为枚举类型
        /// </summary>
        private bool IsEnumType(string identifier, string line, int start)
        {
            // 简化检测：enum关键字后的标识符或已知枚举模式
            return line.Contains("enum ") && char.IsUpper(identifier[0]);
        }

        /// <summary>
        /// 判断是否为结构体类型
        /// </summary>
        private bool IsStructType(string identifier, string line, int start)
        {
            // 简化检测：struct关键字后的标识符
            return line.Contains("struct ") && char.IsUpper(identifier[0]);
        }

        /// <summary>
        /// 判断是否为委托类型
        /// </summary>
        private bool IsDelegateType(string identifier, string line, int start)
        {
            // 简化检测：delegate关键字后的标识符或Delegate结尾
            return line.Contains("delegate ") || identifier.EndsWith("Delegate") ||
                identifier.EndsWith("Handler") || identifier.EndsWith("Callback");
        }

        /// <summary>
        /// 判断是否为字段
        /// </summary>
        private bool IsField(string identifier, string line, int start)
        {
            // 简化检测：成员变量模式（通常以m_开头或简单标识符）
            return identifier.StartsWith("m_") || identifier.StartsWith("_") ||
                char.IsLower(identifier[0]) && !line.Contains("(");
        }

        /// <summary>
        /// 判断是否为静态字段
        /// </summary>
        private bool IsStaticField(string line, int start)
        {
            // 检查前面是否有static关键字
            return line.Contains("static") && (line.Contains("readonly") || line.Contains("const"));
        }

        /// <summary>
        /// 判断是否为属性
        /// </summary>
        private bool IsProperty(string identifier, string line, int start)
        {
            // 简化检测：首字母大写且在属性上下文中
            return char.IsUpper(identifier[0]) &&
                (line.Contains("get") || line.Contains("set") || line.Contains(" => "));
        }

        /// <summary>
        /// 判断是否为事件
        /// </summary>
        private bool IsEvent(string identifier, string line, int start)
        {
            // 简化检测：event关键字后或Event结尾
            return line.Contains("event ") || identifier.EndsWith("Event") ||
                identifier.EndsWith("Changed") || identifier.EndsWith("Handler");
        }

        /// <summary>
        /// 判断是否为重新赋值的参数
        /// </summary>
        private bool IsReassignedParameter(string identifier, string line)
        {
            // 简化检测：参数在赋值操作中
            return line.Contains($"{identifier} =") || line.Contains($"{identifier}++") ||
                line.Contains($"{identifier}--") || line.Contains($"++{identifier}") ||
                line.Contains($"--{identifier}");
        }

        /// <summary>
        /// 判断是否为全局变量
        /// </summary>
        private bool IsGlobalVariable(string identifier, string line, int start)
        {
            // 简化检测：类级别的static字段
            return line.Contains("static") && char.IsLower(identifier[0]) && !line.Contains("(") &&
                !line.Contains("const");
        }

        /// <summary>
        /// 判断是否为重新赋值的局部变量
        /// </summary>
        private bool IsReassignedLocal(string identifier, string line)
        {
            // 简化检测：局部变量在赋值操作中
            return char.IsLower(identifier[0]) && (line.Contains($"{identifier} =") ||
                line.Contains($"{identifier}++") || line.Contains($"{identifier}--") ||
                line.Contains($"++{identifier}") || line.Contains($"--{identifier}"));
        }

        /// <summary>
        /// 判断是否为标签
        /// </summary>
        private bool IsLabel(string line, int identifierEnd)
        {
            // 检查标识符后是否直接跟着冒号（忽略空白字符）
            for (int i = identifierEnd; i < line.Length; i++)
            {
                char c = line[i];
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                return c == ':' && (i + 1 >= line.Length || line[i + 1] != ':'); // 排除 ::
            }

            return false;
        }

        #endregion
    }
}