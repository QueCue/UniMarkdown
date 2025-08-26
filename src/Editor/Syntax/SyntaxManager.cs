using System;
using System.Collections.Generic;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 语法高亮管理器
    /// </summary>
    public class SyntaxManager : IDisposable
    {
        private static SyntaxManager g_inst;

        private Dictionary<string, ISyntaxHighlighting> m_highlightingDict = new();

        public static SyntaxManager Inst
        {
            get { return g_inst ??= new SyntaxManager(); }
        }

        public void Dispose()
        {
            if (null != m_highlightingDict)
            {
                foreach (ISyntaxHighlighting highlighting in m_highlightingDict.Values)
                {
                    highlighting?.Dispose();
                }

                m_highlightingDict.Clear();
            }

            m_highlightingDict = null;
        }

        public string HighlightSyntax(string code, string language)
        {
            if (string.IsNullOrEmpty(code) ||
                string.IsNullOrEmpty(language))
            {
                return code;
            }

            if (m_highlightingDict.TryGetValue(language, out ISyntaxHighlighting syntaxHighlighting))
            {
                return syntaxHighlighting.Highlight(code);
            }

            syntaxHighlighting = GetHighlighting(language);
            if (null == syntaxHighlighting)
            {
                return code;
            }

            m_highlightingDict[language] = syntaxHighlighting;
            code = syntaxHighlighting.Highlight(code);
            return code;
        }

        private ISyntaxHighlighting GetHighlighting(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                return null;
            }

            // 支持多种语言的语法高亮
            string lowerLanguage = language.ToLower();

            if (lowerLanguage == "csharp" ||
                lowerLanguage == "cs" ||
                lowerLanguage == "c#")
            {
                return new CSharpSyntaxHighlighting();
            }

            if (lowerLanguage == "json")
            {
                return new JsonSyntaxHighlighting();
            }

            return null;
        }

        /// <summary>
        /// 获取语言显示名称
        /// </summary>
        /// <param name="language">语言标识</param>
        /// <returns>显示名称</returns>
        public string GetDisplayLanguageName(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                return "Code";
            }

            switch (language.ToLower())
            {
                case "csharp":
                case "cs":
                case "c#":
                    return "C#";
                case "javascript":
                case "js":
                    return "JavaScript";
                case "typescript":
                case "ts":
                    return "TypeScript";
                case "python":
                case "py":
                    return "Python";
                case "java":
                    return "Java";
                case "cpp":
                case "c++":
                    return "C++";
                case "c":
                    return "C";
                case "json":
                    return "JSON";
                default:
                    return language.ToUpper();
            }
        }
    }
}