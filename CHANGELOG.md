# Changelog

This project adheres to Keep a Changelog and uses Semantic Versioning.

## [0.2.0-Preview] - 2025-08-28
### Added
- **Table Renderer**: Complete table rendering system with column alignment, header styling, borders, and responsive layout
- **Table Features**: Support for left/center/right column alignment via Markdown syntax
- **Table Styling**: Automatic header background, border styling, and theme adaptation
- **Table Performance**: Optimized rendering with GC-friendly content measurement and style caching

### Technical Details
- `TableElementRenderer.cs`: Full implementation with responsive design and scrolling support
- `MarkdownStyleManager.cs`: Centralized table styling with Em-based responsive dimensions
- **Architecture**: Follows established renderer patterns with centralized style management

## [0.1.0] - 2025-08-26
### Added
- Initial public release
- Unity Package Manager (UPM) support via Git URL (`?path=src`)

### Changed
- README updated with Quick Start and UPM install option

### Notes
- Package name: `com.quecue.unimarkdown`
