# JSON高级语法测试

## 边界情况测试

### 转义字符处理

```json
{
  "escapes": {
    "quotes": "He said \"Hello World\"",
    "backslash": "Path: C:\\Windows\\System32",
    "newline": "Line 1\nLine 2",
    "tab": "Column1\tColumn2",
    "unicode": "\u4F60\u597D\u4E16\u754C"
  }
}
```

### 空值和空结构

```json
{
  "empty": {},
  "emptyArray": [],
  "emptyString": "",
  "nullValue": null,
  "nested": {
    "empty": {},
    "array": [null, "", {}]
  }
}
```

### 数字格式验证

```json
{
  "numbers": {
    "zero": 0,
    "negative": -42,
    "decimal": 123.456,
    "scientific1": 1.23e10,
    "scientific2": 1.23E-10,
    "scientific3": -1.23e+5,
    "large": 999999999999999
  }
}
```

### 深度嵌套

```json
{
  "level1": {
    "level2": {
      "level3": {
        "level4": {
          "level5": {
            "data": "deep nesting test",
            "numbers": [1, 2, 3],
            "success": true
          }
        }
      }
    }
  }
}
```

### 布尔值测试

```json
{
  "flags": {
    "isEnabled": true,
    "isDisabled": false,
    "conditions": [true, false, true],
    "mixed": [true, null, false, "not boolean"]
  }
}
```

### API响应示例

```json
{
  "status": "success",
  "code": 200,
  "data": {
    "users": [
      {
        "id": "user_001",
        "username": "developer",
        "email": "dev@example.com",
        "isActive": true,
        "lastLogin": "2025-08-19T10:30:00Z",
        "permissions": ["read", "write", "admin"],
        "profile": {
          "firstName": "John",
          "lastName": "Developer",
          "avatar": null,
          "preferences": {
            "theme": "dark",
            "language": "en",
            "notifications": true
          }
        }
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 1,
      "hasNext": false
    }
  },
  "timestamp": 1692444600000,
  "version": "1.0"
}
```
