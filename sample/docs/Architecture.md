# 架构说明

本页概述 MarkdownForEditor 的核心模块、调用关系与扩展点。

## 核心功能模块

1.  **编辑器窗口 (`MarkdownEditorWindow`)**
    -   负责窗口生命周期、菜单、文件 I/O、模式切换与重绘。
    -   调用解析器 `MarkdownParser` 生成 `List<MarkdownElement>`。
    -   持有 `MarkdownRenderer` 完成 GUI 渲染。

2.  **Inspector 预览 (`MarkdownInspector`)**
    -   使用 `[CustomEditor(typeof(TextAsset))]` 为 Markdown 文件提供自定义检视面板。
    -   在 Inspector 内直接渲染 `.md` 和 `.markdown` 文件的预览。
    -   提供预览开关与“在编辑器中打开”的快捷方式。

3.  **双击打开处理器 (`MarkdownAssetHandler`)**
    -   使用 `[OnOpenAsset(1)]` 捕获资源打开事件。
    -   判断文件扩展名，若是 Markdown 文件，则调用 `MarkdownEditorWindow.OpenMarkdownFile()`。
    -   返回 `true` 以阻止 Unity 的默认文本编辑器打开文件。

## 解析与渲染流程

-   **解析器 (`MarkdownParser`)**
    -   **性能优化**：使用 `RegexOptions.Compiled`、复用 `StringBuilder`、以及 `MarkdownElement` 对象池来减少 GC 压力。
    -   **语法支持**：标题、列表、任务清单、分隔线、图片（含尺寸扩展）、链接、Emoji、行内代码、加粗/斜体/粗斜体、换行、代码块（含语言与缩进）。
    -   **冲突消解**：行内元素采用优先级策略（如粗斜体 > 粗体 > 斜体）解决重叠匹配问题。

-   **渲染器 (`MarkdownRenderer`)**
    -   将元素列表按“块级”和“行内组”进行分组，以实现符合 Markdown 规范的间距。
    -   通过 `ElementRendererFactory` 委托给具体的元素渲染器完成绘制。

-   **渲染器体系**
    -   `IElementRenderer`：所有元素渲染器的统一接口。
    -   `BaseElementRenderer`：提供 Style 访问、Repaint 回调、Render 模板和 `Em` 尺寸换算等通用功能。
    -   `ElementRendererFactory`：负责注册、缓存和按类型获取渲染器实例。
    -   `MarkdownStyleManager`：主题感知的样式单例，提供所有 `GUIStyle`。

-   **语法高亮 (`Syntax/SyntaxManager`)**
    -   负责语言路由和显示名映射。内置 C# 和 JSON，可通过 `ISyntaxHighlighting` 接口扩展新语言。

## 数据流

1.  **用户操作**：双击文件、修改文本或选中资源。
2.  **事件触发**：`OnOpenAsset`、`OnGUI` 或 `OnInspectorGUI`。
3.  **内容解析**：`MarkdownParser.ParseMarkdown` 将文本转换为 `List<MarkdownElement>`。
4.  **渲染执行**：`MarkdownRenderer.RenderMarkdown` 遍历元素列表，通过 `ElementRendererFactory` 获取对应的渲染器进行绘制。
5.  **样式应用**：各渲染器从 `MarkdownStyleManager` 获取 `GUIStyle` 并完成最终渲染。

## 扩展点

-   **新增元素渲染器**
    1.  新建类实现 `IElementRenderer` 或继承 `BaseElementRenderer`。
    2.  在 `ElementRendererFactory.RegisterRenderer<T>(MarkdownElementType)` 中注册。
-   **新增语法高亮语言**
    1.  实现 `ISyntaxHighlighting` 接口。
    2.  在 `SyntaxManager.GetHighlighting` 中返回其实例。
-   **样式定制**
    -   扩展 `MarkdownStyleManager` 的样式或引入自定义主题切换。

## 测试与样例

-   `Assets/Scripts/Editor/MarkdownForEditor/Tests/TestFiles/` 内含大量手工样例，可用于对照渲染。
-   未来可补充基于 Unity Test Framework 的 EditMode/PlayMode 测试。
