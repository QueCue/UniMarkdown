# 更新日志

本项目遵循 Keep a Changelog，并使用语义化版本（SemVer）。

## [0.2.0-Preview] - 2025-08-28
### 新增
- **表格渲染器**：完整的表格渲染系统，支持列对齐、表头样式、边框和响应式布局
- **表格功能**：通过 Markdown 语法支持左对齐/居中/右对齐列对齐
- **表格样式**：自动表头背景、边框样式和主题适配
- **表格性能**：优化渲染，采用 GC 友好的内容测量和样式缓存

### 技术细节
- `TableElementRenderer.cs`：完整实现，包含响应式设计和滚动支持
- `MarkdownStyleManager.cs`：集中化表格样式，采用 Em 单位的响应式尺寸
- **架构**：遵循既定的渲染器模式，采用集中式样式管理

## [0.1.0] - 2025-08-26
### 新增
- 首个公开版本
- 支持通过 Unity Package Manager (UPM) 使用 Git URL（`?path=src`）安装

### 变更
- README 更新：加入 Quick Start 与 UPM 安装方式

### 备注
- 包名：`com.quecue.unimarkdown`

> 说明：若中英文日志存在差异，以英文版 `CHANGELOG.md` 为准。
