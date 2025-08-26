# Markdown文件双击打开功能说明

## 功能概述

现在您可以通过双击Project窗口中的Markdown文件来直接在MarkdownEditorWindow中打开它们，无需手动选择菜单或拖拽文件。

## 实现的功能

### 1. **自动识别Markdown文件**
- 支持`.md`文件扩展名
- 支持`.markdown`文件扩展名
- 自动过滤非Markdown的TextAsset文件

### 2. **无缝集成**
- 双击任何Markdown文件即可打开
- 自动获得窗口焦点
- 内容立即加载并显示
- 不影响其他文件类型的默认行为

### 3. **智能处理**
- 使用文件的完整路径确保准确性
- 优雅的错误处理
- 不会干扰Unity的默认资源处理

## 技术实现

### 核心组件

#### 1. `MarkdownAssetHandler.cs`
```csharp
[OnOpenAsset(1)]
public static bool OnOpenAsset(int instanceID, int line)
{
    // 处理Markdown文件的双击事件
    // 返回true表示已处理，阻止默认行为
}
```

#### 2. `MarkdownEditorWindow.OpenMarkdownFile()`
```csharp
public static void OpenMarkdownFile(string filePath)
{
    // 创建或获取MarkdownEditorWindow实例
    // 加载指定文件并显示窗口
}
```

### 优先级设置

使用`[OnOpenAsset(1)]`设置了较高的优先级，确保在其他处理器之前执行。

## 使用方法

1. **在Project窗口中找到任何.md文件**
2. **双击该文件**
3. **MarkdownEditorWindow会自动打开并加载文件内容**

## 测试文件

项目中包含了以下测试文件：
- `test_double_click.md` - 专门用于测试双击功能
- `test_markdown_inspector.md` - 用于测试Inspector预览
- `test_interactivity.md` - 用于测试交互功能

## 兼容性

- ✅ 兼容Unity 2021.3 LTS及以上版本
- ✅ 不影响其他文件类型的正常打开
- ✅ 与现有的Inspector预览功能并存
- ✅ 支持所有Unity支持的平台

## 故障排除

如果双击功能不工作，请检查：
1. 文件确实是`.md`或`.markdown`扩展名
2. Unity编辑器已完成脚本编译
3. 控制台中是否有错误信息

## 扩展可能

未来可以考虑添加：
- 支持更多Markdown相关的文件扩展名
- 右键菜单"使用Markdown编辑器打开"选项
- 设置选项来禁用/启用此功能
- 支持从外部拖拽Markdown文件到Unity

---

*此功能已集成到项目中，立即可用！*
