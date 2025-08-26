# UniMarkdown
[![Unity](https://img.shields.io/badge/Unity-2021.3%2B-555555?logo=unity&logoColor=white)](ProjectSettings/ProjectVersion.txt)
[![Platform](https://img.shields.io/badge/Editor-Extension-2e7d32)](#)
[![Lang](https://img.shields.io/badge/C%23-Editor%20Tools-239120?logo=.net&logoColor=white)](#)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Made with Unity](https://img.shields.io/badge/Made%20with-Unity-555555.svg?logo=unity)](https://unity.com)

[中文说明](./README.zh.md) | English

> **The ultimate native Markdown editor for Unity. Fast, beautiful, and deeply integrated.**

---

<!-- Dynamic badges (uncomment and replace OWNER/REPO after publishing)
[![Stars](https://img.shields.io/github/stars/OWNER/REPO?style=social)](https://github.com/OWNER/REPO/stargazers)
[![Issues](https://img.shields.io/github/issues/OWNER/REPO)](https://github.com/OWNER/REPO/issues)
[![Last Commit](https://img.shields.io/github/last-commit/OWNER/REPO)](https://github.com/OWNER/REPO/commits)
[![Release](https://img.shields.io/github/v/release/OWNER/REPO)](https://github.com/OWNER/REPO/releases)
-->

## Demo & Screenshots 🖼️

![Demo placeholder](docs/images/demo.svg =800x)

![Preview dark placeholder](docs/images/preview-dark.svg){width=45%}
![Preview light placeholder](docs/images/preview-light.svg){width=45%}

> These are placeholders. Replace with your real `docs/images/demo.gif` and `preview-dark.png` / `preview-light.png` after recording.

---

## Why UniMarkdown? ❓

-   ✅ **Native Integration**: More than just an editor, it seamlessly blends with your Unity workflow (double-click to open, Inspector previews).
-   ✅ **High Performance**: Built with compiled Regex and object pooling to handle large documents smoothly without editor lag.
-   ✅ **Highly Extensible**: Easily add custom syntax highlighters and element renderers to meet your team's specific needs.
-   ✅ **Familiar Feel**: A perfect replica of the GitHub style you know and love, including Emoji support 🎉.

---

## Features ✨

-   ✅ **Editor & Preview**: A standalone editor window and live previews directly within the Inspector.
-   ✅ **Workflow Integration**: Supports opening `.md` files directly with a double-click.
-   ✅ **GitHub Style**: Automatically adapts to the Unity editor's dark/light themes.
-   ✅ **Full Syntax Support**: Headers, lists, task lists, code blocks, quotes, images, links, and more.
-   ✅ **Extended Syntax**:
    -   Image size control (`=300x200`, `{width=50%}`, etc.).
    -   Emoji support (`:tada:`, `:rocket:`, etc.).
-   ✅ **Enhanced Code Blocks**:
    -   Syntax highlighting for multiple languages (C#/JSON built-in, extensible).
    -   One-click copy button with an animated confirmation.
-   ✅ **High Performance**: Core parsing logic is optimized to minimize GC Alloc.
-   ✅ **Easy to Extend**: Modular renderer and syntax highlighting systems.

---

## Install & Quick Start 🚀

-   Option A — Copy: copy `src/Editor` into your Unity project at `Assets/UniMarkdown`.
-   Option B — Unity Package Manager (UPM):
    1. Open Unity → Window → Package Manager
    2. Click the + button → "Add package from git URL..."
    3. Use: `https://github.com/QueCue/UniMarkdown.git?path=src` (follow the main branch)
    Upgrade: select the package in Package Manager and click "Update" to get the latest commit
-   Open Markdown:
    -   Select a `.md` asset to render in the Inspector via `MarkdownInspector`.
    -   Or open the Markdown viewer window (if provided) from the Unity menu.

### Extend (2-min how-to)

-   Renderers: add a class under `ElementRenderers/` inheriting `BaseElementRenderer`, then register it in `ElementRendererFactory` (element type → renderer).
-   Syntax highlighting: implement `ISyntaxHighlighting` and register it in `SyntaxManager` (see built-ins for C#/JSON).

Tips: keep GUI paths allocation-free and reuse styles via `MarkdownStyleManager`.

---

## Contributing 🤝

Issues and PRs are welcome. For larger changes, open an issue to discuss first. If this project helps you, a ⭐️ is greatly appreciated!

---

## Changelog 🗒️

- English: [`CHANGELOG.md`](./CHANGELOG.md)
- 中文: [`CHANGELOG.zh.md`](./CHANGELOG.zh.md)

---

## License 📄

This project is licensed under the terms described in the root [LICENSE](LICENSE).

---

<div align="center">

*UniMarkdown - The Markdown Companion for Unity Developers*

</div>
