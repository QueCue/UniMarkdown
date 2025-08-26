# 扩展指南（草案）

## 自定义元素渲染器

1. 新建类：继承 `BaseElementRenderer`（推荐）或实现 `IElementRenderer`
2. 指定 `SupportedElementType` 与（可选）`Priority`
3. 实现 `OnRender(MarkdownElement element, bool isInline)` 完成绘制
4. 在 `ElementRendererFactory` 中注册：`RegisterRenderer<YourRenderer>(MarkdownElementType.XXX)`

注意：`BaseElementRenderer` 已提供样式访问（`MarkdownStyleManager.Inst`）、`RenderElement` 递归渲染、`Repaint()` 等辅助。

## 自定义语法高亮

1. 实现接口 `ISyntaxHighlighting`：`string Highlight(string code)` 与 `Dispose()`
2. 在 `SyntaxManager.GetHighlighting(string language)` 中返回对应实例（匹配 `language.ToLower()`）
3. 如需显示语言名，更新 `GetDisplayLanguageName`

## 样式主题

- 若需扩展主题或参数化字体/颜色，可在 `MarkdownStyleManager` 中添加接口与缓存字段
- 统一通过 `Inst` 单例访问，避免在渲染器中重复创造 Style

## 解析规则扩展

- 当前解析由 `MarkdownParser` 管理，可新增 Regex 或解析步骤
- 注意匹配优先级与“去重叠”逻辑，以免行内样式相互覆盖
