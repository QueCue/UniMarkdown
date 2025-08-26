# 使用指南

本页介绍在 Unity Editor 中使用 Markdown 编辑器窗口的完整流程与技巧。

## 主要功能

1.  **编辑器窗口**：提供一个独立的、功能丰富的 Markdown 编辑与预览环境。
2.  **Inspector 预览**：在 Unity 的 Inspector 面板中直接预览 Markdown 文件的渲染效果。
3.  **双击直接打开**：在 Project 窗口中双击 `.md` 文件，可直接在编辑器窗口中打开。

---

## 1. 编辑器窗口

### 打开与示例

- **菜单打开**：Tools/Markdown编辑器/打开Markdown编辑器
- **双击打开**：直接在 Project 窗口双击任意 `.md` 或 `.markdown` 文件。
- **加载示例**：
  - Tools/Markdown编辑器/加载示例文件（Assets/StreamingAssets/Examples/sample.md）
  - Tools/Markdown编辑器/测试C#语法高亮、测试C#语法高亮2

### 编辑模式

- 顶部工具栏：
  - **打开文件**：从项目中选择 .md 文件（会转换为相对路径以便 Asset 导入）
  - **保存文件 / 另存为**：保存后会自动 Import 刷新
  - **编辑模式开关**：勾选为“编辑模式”，使用大文本框编辑 Markdown 源
- 编辑完成后点击“预览 Markdown”，会触发重新解析并切至预览
- 窗口获得焦点时如检测到外部修改，会询问是否重新加载

### 预览模式

- 渲染风格遵循 GitHub 暗色主题（随 Unity 主题自动调整）
- 滚动区域内展示解析后的元素：
  - 标题 H1–H6（H1/H2 可能带下划线分割）
  - 列表：有序/无序、任务清单，四个空格为一层嵌套
  - 链接与图片（支持尺寸扩展语法）
  - **Emoji**：支持常见的 GitHub Emoji, 如 `:tada:`, `:rocket:` 等。
  - 行内样式：行内代码、加粗/斜体/粗斜体
  - 代码块：语法高亮 + 复制按钮，点击后右上角出现“Copied!” 动画
- 渲染间距遵循 Markdown 规范，列表组之间与段落间距已细化

---

## 2. Inspector 预览

当你在 Project 窗口中选中一个 `.md` 或 `.markdown` 文件时，Inspector 面板会自动渲染其内容。

- **功能开关**：
  - **显示 Markdown 预览**：控制是否在 Inspector 中显示渲染效果。
  - **显示原始 Inspector**：显示 Unity 默认的 TextAsset Inspector。
- **快捷操作**：
  - **在编辑器中打开**：等同于双击文件，使用我们的编辑器窗口打开。
  - **刷新预览**：手动重新渲染当前预览。

> 这些设置会保存在 `EditorPrefs` 中，在不同文件间保持一致。

---

## 3. 语法与扩展

### 图片尺寸扩展

- 固定尺寸：`![alt](url =300x200)`
- 固定宽度：`![alt](url =300x)`
- 固定高度：`![alt](url =x200)`
- 属性写法：`![alt](url){width=300 height=200}` 或百分比（`width=50%`）

### 换行规则

- 行末两个空格 → 软换行（SoftLineBreak），仍在同段落
- 空行 → 段落分隔（硬换行 LineBreak），用于分段

### 代码块语法

- 使用 ``` 开始与结束，可在开始行指定语言：`csharp`、`json` 等
- 内置语法高亮：C#/JSON；其他语言可自行扩展
- 高亮由 `Syntax/SyntaxManager.cs` 路由（可查看 GetDisplayLanguageName 名称映射）

## 常见问题

- **打开外部路径的文件**？当前建议选择项目内路径（会转为 Assets/ 相对路径），以便 Unity Import
- **如何重置渲染器缓存**？切换 PlayMode 时会自动 Reset；或重新打开窗口
- **为什么有的 Markdown 语法没生效**？请对照 README 功能一览与文中的扩展语法说明
- **双击没反应**？请确认文件名后缀为 `.md` 或 `.markdown`，并检查控制台有无报错。
