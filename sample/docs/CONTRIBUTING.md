# 贡献指南

感谢你对 MarkdownForEditor 的兴趣！请在提交 PR 前阅读以下规范。

## 提交前检查

- 代码风格：遵循仓库的 C# 代码规范与 Unity 最佳实践
- 无不必要的分配：避免在 OnGUI/Update 中创建对象或捕获闭包
- API 稳定性：保持现有公共 API（类名/方法名/枚举）不破坏，如需变更请在 PR 描述中说明迁移策略
- 文档：为新增的公开能力补充 docs/

## 分支与提交

- 建议分支：feature/xxx、fix/xxx、docs/xxx
- Commit 信息：使用动宾短语，英文为主，必要时附中文补充
- PR 模板：清晰描述动机、变更点、验证方式（含截图/动图）与影响范围

## 测试

- 可在 `Tests/TestFiles` 中添加更多 Markdown 样例
- 建议补充 Unity Test Framework 用例（EditMode/PlayMode）

## 许可证与版权

- 请确保引入的第三方代码或资源具有兼容许可，并在 PR 中注明来源与许可证
