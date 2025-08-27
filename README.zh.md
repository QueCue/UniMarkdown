<div align="center">

![UniMarkdown Banner](https://capsule-render.vercel.app/api?type=waving&color=gradient&height=120&text=UniMarkdown&fontAlign=50&fontAlignY=35&fontSize=40&animation=fadeIn)

# UniMarkdown
<p>
  <img src="https://img.shields.io/badge/Unity-2021.3%2B-4ecdc4?style=for-the-badge&logo=unity&logoColor=white&labelColor=1a1a2e"/>
  <img src="https://img.shields.io/badge/Editor-Extension-ff6b6b?style=for-the-badge&logo=unity&logoColor=white&labelColor=1a1a2e"/>
    <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge&logoColor=white&labelColor=1a1a2e"/></a>
</p>
<p>
  <img src="https://img.shields.io/badge/C%23-Editor%20Tools-7289da?style=for-the-badge&logo=.Net&logoColor=white&labelColor=1a1a2e"/>
  <a href="https://unity.com"><img src="https://img.shields.io/badge/Made%20with-Unity-07c160?style=for-the-badge&logo=unity&logoColor=white&labelColor=1a1a2e"/></a>
</p>

<div align="center">
  <div style="width: 100%; height: 2px; margin: 20px 0; background: linear-gradient(90deg, transparent, #00d9ff, transparent);"></div>
</div>

<div align="center">
  <a href="#-quick-start" style="text-decoration: none;">
    <img src="https://img.shields.io/badge/Quick%20Start-Get%20Started%20Now-00d9ff?style=for-the-badge&logo=rocket&logoColor=white&labelColor=1a1a2e">
  </a>
</div>

<p/>
<p/>

<!-- Language Switch -->
中文说明 | [English](./README.md)

> **Unity 原生 Markdown 终极解决方案。高速、美观、深度集成。**


</div>

<!-- 可在仓库创建后启用的动态徽章示例：请将 OWNER/REPO 替换为实际值
[![Stars](https://img.shields.io/github/stars/OWNER/REPO?style=social)](https://github.com/OWNER/REPO/stargazers)
[![Issues](https://img.shields.io/github/issues/OWNER/REPO)](https://github.com/OWNER/REPO/issues)
[![Last Commit](https://img.shields.io/github/last-commit/OWNER/REPO)](https://github.com/OWNER/REPO/commits)
[![Release](https://img.shields.io/github/v/release/OWNER/REPO)](https://github.com/OWNER/REPO/releases)
-->

## 在线演示与截图 🖼️

![Demo placeholder](docs/images/demo.svg =800x)

![Preview dark placeholder](docs/images/preview-dark.svg){width=45%}
![Preview light placeholder](docs/images/preview-light.svg){width=45%}

> 当前为占位图。请将实际录制的 `docs/images/demo.gif` 和截图 `preview-dark.png` / `preview-light.png` 覆盖到相同文件名（或更新链接）。

## 为什么选择 UniMarkdown ❓

-   ✅ **原生集成 (Native Integration)**: 不只是个编辑器，更是与 Unity 工作流（双击打开、Inspector 预览）的无缝融合。
-   ✅ **性能卓越 (High Performance)**: 基于编译后 Regex 与对象池，处理大型文档依然流畅，无惧卡顿。
-   ✅ **高度可扩展 (Extensible)**: 轻松添加自定义语法高亮和元素渲染器，满足团队特殊需求。
-   ✅ **熟悉的感觉 (Familiar Feel)**: 完美复刻 GitHub 风格，无需学习成本，支持 Emoji 🎉。


## 特性亮点 ✨

-   ✅ **编辑器与预览**：独立的编辑器窗口与 Inspector 内实时预览。
-   ✅ **工作流集成**：支持双击 `.md` 文件直接打开。
-   ✅ **GitHub 风格**：自动适配 Unity 编辑器的深色/浅色主题。
-   ✅ **完整语法支持**：标题、列表、任务清单、代码块、引用、图片、链接等。
-   ✅ **扩展语法**：
    -   图片尺寸控制 (`=300x200`, `{width=50%}` 等)。
    -   Emoji 支持 (`:tada:`, `:rocket:` 等)。
-   ✅ **代码块增强**：
    -   多种语言语法高亮 (内置 C#/JSON，可扩展)。
    -   一键复制代码并伴有动画提示。
-   ✅ **高性能**：核心解析逻辑经过优化，避免不必要的 GC Alloc。
-   ✅ **易于扩展**：模块化的渲染器和语法高亮系统。

## 安装与快速开始 🚀

-   方式 A — 拷贝：将 `src/Editor` 复制到你的工程 `Assets/UniMarkdown`（或任意 Editor 目录）。
-   方式 B — 通过 Unity Package Manager (UPM)：
    1. 打开 Unity → Window → Package Manager
    2. 点击 + → "Add package from git URL..."
    3. 输入：`https://github.com/QueCue/UniMarkdown.git?path=src`（跟随 main 分支）

    升级：在 Package Manager 选中该包并点击“Update”即可获取最新提交。

-   打开 Markdown：
    - 在 Project 视图选中任意 `.md` 资源，可在 Inspector 中通过 `MarkdownInspector` 渲染。
    - 或通过 Unity 菜单打开 Markdown 预览窗口（如已提供）。

### 扩展（2 分钟上手）

-   渲染器：在 `ElementRenderers/` 下新建继承 `BaseElementRenderer` 的类，并在 `ElementRendererFactory` 注册（元素类型 → 渲染器）。
-   语法高亮：实现 `ISyntaxHighlighting` 并在 `SyntaxManager` 注册（参考内置 C#/JSON）。

提示：在 GUI 循环中避免分配，样式建议统一由 `MarkdownStyleManager` 复用管理。

## 渲染器支持情况 ⚡

- 当前支持
  - 标题（Header）
  - 粗体（Bold）、斜体（Italic）、粗斜体（BoldItalic）
  - 文本（Text）
  - 软换行（SoftLineBreak）/ 硬换行（LineBreak）
  - 分割线（Divide / Horizontal Rule）
  - 列表（ListItem：无序/有序）
  - 嵌套列表（Nested List）
  - 任务清单（TaskList）
  - 嵌套任务清单（Nested TaskList）
  - 链接（Link）
  - 图片（Image，支持尺寸参数）
  - 代码块（CodeBlock：内置 C#/JSON 语法高亮）
  - 行内代码（InlineCode）

- 即将支持（规划）
  - 表格（Table）
  - Emoji
  - 代码块行号
  - 引用（Blockquote）
  - 删除线（Strikethrough）
  - 脚注（Footnote）
  - Callout / Admonition（提示/注意/警告块）
  - Mermaid 图（可选，通过外部渲染或缓存静态图）

## 贡献与开发 🤝

欢迎提交 Issue 与 PR。较大改动建议先开 Issue 讨论。若本项目对你有帮助，欢迎点亮 Star ⭐️！

## 更新日志 📝

- 中文：[`CHANGELOG.zh.md`](./CHANGELOG.zh.md)
- 英文：[`CHANGELOG.md`](./CHANGELOG.md)

## 许可证 📄

本项目的许可证见仓库根目录的 [LICENSE](LICENSE)。

---

<div align="center">

*UniMarkdown - 为 Unity 开发者打造的 Markdown 伴侣*

</div>
