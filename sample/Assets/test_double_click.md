# 测试双击打开功能

这个文件用于测试双击Markdown文件使用MarkdownEditorWindow打开的功能。

## 使用方法

1. 在Unity编辑器的Project窗口中
2. 找到这个.md文件
3. 双击它
4. 应该会自动在MarkdownEditorWindow中打开

## 预期效果

- ✅ 双击.md文件会打开MarkdownEditorWindow
- ✅ 文件内容会自动加载到编辑器中
- ✅ 可以看到格式化的Markdown预览
- ✅ 窗口会获得焦点并显示在前台

## 技术实现

使用了Unity的`OnOpenAsset`回调来拦截文件双击事件：

```csharp
[OnOpenAsset(1)]
public static bool OnOpenAsset(int instanceID, int line)
{
    // 检查是否为.md文件
    // 如果是，使用MarkdownEditorWindow打开
    // 返回true阻止默认行为
}
```

## 支持的文件格式

- `.md`文件
- `.markdown`文件

如果您看到这个内容在MarkdownEditorWindow中显示，说明功能工作正常！🎉
