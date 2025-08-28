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
  <a href="#install--quick-start-" style="text-decoration: none;">
    <img src="https://img.shields.io/badge/Quick%20Start-Install%20via%20UPM-00d9ff?style=for-the-badge&logo=unity&logoColor=white&labelColor=1a1a2e">
  </a>
</div>

<p/>
<p/>

[中文说明](./README.zh.md) | English

> **The ultimate native Markdown editor for Unity. Fast, beautiful, and deeply integrated.**

</div>

<!-- Dynamic badges (uncomment and replace OWNER/REPO after publishing)
[![Stars](https://img.shields.io/github/stars/OWNER/REPO?style=social)](https://github.com/OWNER/REPO/stargazers)
[![Issues](https://img.shields.io/github/issues/OWNER/REPO)](https://github.com/OWNER/REPO/issues)
[![Last Commit](https://img.shields.io/github/last-commit/OWNER/REPO)](https://github.com/OWNER/REPO/commits)
[![Release](https://img.shields.io/github/v/release/OWNER/REPO)](https://github.com/OWNER/REPO/releases)
-->

## Solving Unity Documentation "Absurd Moments" 😤

Have you ever endured these "absurd moments" with Unity documentation?  
- Writing a Shader note feels like playing a "window-switching game"—flicking between Unity, VS Code, and Typora, missing the right file three times in a row. (I'm here to develop games, not work as a full-time window switcher!)  
- The reference images you insert are either so big they take over the screen or so small they turn into pixel blobs. Tweaking their size makes you want to slam your mouse. (Does this image have a mind of its own? Is it that hard to fit properly?)  
- Collaborating on docs feels like a "file relay race"—you send Version 2, someone edits Version 3, and soon your folder is stuffed with "Note V1 to V8." No one knows which is the latest. (This isn't documentation—it's a "document family pack"!)  
- You try adding a table to record parameters, but the columns are as lopsided as a wind-tousled fence. Fixing alignment takes 10 minutes—harder than teaching a newbie to tweak a Shader. (Is a neat table really that much to ask for?)  
- And the most frustrating one: You stay up late writing *Character Skill Logic.md*, but when the game designer opens it, they're staring at a screen full of `# * - |` like it's Morse code. Finally, they hold up their phone and ask: "Am I supposed to decode this?" (I wrote a document, not an encrypted telegram!)  

UniMarkdown is built specifically to solve these documentation headaches for Unity developers. As a **Unity-native Markdown toolkit**, it doesn't just let you double-click to open .md files directly in the Editor—it also renders them in real time into a "symbol-free, clean version" (headings auto-bolded, tables neatly aligned, images clearly displayed). Most importantly:  
Even non-technical teammates (who know nothing about Markdown) can understand the well-formatted docs just by opening Unity. No extra software to install, no syntax to learn—goodbye to the communication waste where "you write it for nothing, they read it for nothing."  

From "you write comfortably but others can't understand" to "the whole team reads easily," UniMarkdown turns documentation into a collaboration booster—instead of a barrier between departments.

## Demo & Screenshots 🖼️

<div align="center">

### 🎥 Live Demo
<img src="image/preview.gif" alt="UniMarkdown Demo" width="600"/>

### 🌙 Dark Theme
<img src="image/preview-dark.png" alt="Dark Theme Preview" width="500"/>

### ☀️ Light Theme  
<img src="image/preview-light.png" alt="Light Theme Preview" width="500"/>

</div>

## Why UniMarkdown? ❓

-   🔗 **Native Integration**: More than just an editor, it seamlessly blends with your Unity workflow (double-click to open, Inspector previews).
-   ⚡ **High Performance**: Built with compiled Regex and object pooling to handle large documents smoothly without editor lag.
-   🧩 **Highly Extensible**: Easily add custom syntax highlighters and element renderers to meet your team's specific needs.
-   💫 **Familiar Feel**: A perfect replica of the GitHub style you know and love, with consistent theming.


## Features ✨

-   📱 **Editor & Preview**: A standalone editor window and live previews directly within the Inspector.
-   🔄 **Workflow Integration**: Supports opening `.md` files directly with a double-click.
-   🎨 **GitHub Style**: Automatically adapts to the Unity editor's dark/light themes.
-   📝 **Full Syntax Support**: Headers, lists, task lists, code blocks, quotes, images, links, and more.
-   🎯 **Extended Syntax**:
    -   Image size control (`=300x200`, `{width=50%}`, etc.).
    -   Advanced formatting and custom extensions.
-   💻 **Enhanced Code Blocks**:
    -   Syntax highlighting for multiple languages (C#/JSON built-in, extensible).
    -   One-click copy button with an animated confirmation.
-   🚀 **High Performance**: Core parsing logic is optimized to minimize GC Alloc.
-   🔧 **Easy to Extend**: Modular renderer and syntax highlighting systems.

## Performance & Compatibility 📊

<div align="center">

| Metric | Status | Details |
|--------|--------|---------|
| **Unity Version** | ✅ | 2021.3+ LTS |
| **Editor Performance** | ✅ | < 1ms parsing, GC-optimized |
| **File Size Support** | ✅ | Tested up to 10MB+ documents |
| **Theme Compatibility** | ✅ | Auto-adapts to Dark/Light |
| **Platform Support** | ✅ | Editor-only, all Unity platforms |

</div>

## Install & Quick Start 🚀

### 📦 Installation Methods

#### 🌟 Option A: Unity Package Manager (Recommended)

1. Open Unity → **Window** → **Package Manager**
2. Click the **+** button → **"Add package from git URL..."**
3. Enter: `https://github.com/QueCue/UniMarkdown.git?path=src`
4. Click **Add** and wait for Unity to import

**Benefits**: Easy updates, clean project structure, automatic dependency management  
**To Update**: Select UniMarkdown in Package Manager → Click **Update**

#### Option B: Direct Copy (For custom modifications)

1. Download or clone this repository
2. Copy `src/Editor` folder to `Assets/UniMarkdown/` in your Unity project
3. Unity will automatically detect and compile the package

**Use case**: When you need to modify the source code directly

### 🎯 Quick Usage

1. **Inspector Preview**: Select any `.md` file in Project window to see live preview
2. **Editor Window**: *Window → UniMarkdown* (if available) for dedicated editing
3. **Double-click**: Open `.md` files directly (when configured)

### Extend (2-min how-to) ⚡

-   Renderers: add a class under `ElementRenderers/` inheriting `BaseElementRenderer`, then register it in `ElementRendererFactory` (element type → renderer).
-   Syntax highlighting: implement `ISyntaxHighlighting` and register it in `SyntaxManager` (see built-ins for C#/JSON).

Tips: keep GUI paths allocation-free and reuse styles via `MarkdownStyleManager`.

## Renderer Support Status 🎯

### Currently Supported

#### Core Elements
- **Text Formatting**: Text, Bold, Italic, BoldItalic
- **Headers**: H1-H6 with styling
- **Line Breaks**: Soft line break / Hard line break
- **Dividers**: Horizontal rules

#### Lists & Navigation
- **Lists**: Unordered/ordered lists with nesting support
- **Task Lists**: Interactive checkboxes with nesting
- **Links**: Internal and external linking

#### Rich Content
- **Images**: Full support with size parameters (`=300x200`, `{width=50%}`)
- **Code Blocks**: Syntax highlighting (C#/JSON built-in, extensible)
- **Inline Code**: Styled code snippets
- **Tables**: Complete table rendering with column alignment, header styling, borders, and responsive layout

### 🚧 Roadmap (Planned Features)
- **Enhanced Text**: Emoji support, Strikethrough
- **Code Improvements**: Line numbers for code blocks
- **Rich Blocks**: Blockquote, Footnotes
- **Advanced**: Callout/Admonition blocks (tip/note/warning)
- **Diagrams**: Mermaid support (via external rendering)


## Contributing 🤝

We welcome contributions from the community! Here's how you can help:

- 🐛 **Bug Reports**: Found an issue? [Open an issue](https://github.com/QueCue/UniMarkdown/issues/new)
- 💡 **Feature Requests**: Have an idea? We'd love to hear it!
- 🔧 **Pull Requests**: Code contributions are welcome (discuss major changes first)
- ⭐ **Star the Project**: If UniMarkdown helps you, please give us a star!

## Documentation 📚

- **Changelog**: [`CHANGELOG.md`](./CHANGELOG.md) | [`CHANGELOG.zh.md`](./CHANGELOG.zh.md)
- **License**: [MIT License](./LICENSE)
- **Version**: `0.2.0-Preview` (Unity 2021.3+)

---

<div align="center">

*UniMarkdown - The Markdown Companion for Unity Developers*

</div>
