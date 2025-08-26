# JSON语法高亮测试文档

## 基础JSON示例

```json
{
  "name": "John Doe",
  "age": 30,
  "isActive": true,
  "spouse": null,
  "height": 175.5,
  "skills": ["JavaScript", "C#", "Unity"]
}
```

## 复杂嵌套JSON

```json
{
  "company": {
    "name": "Tech Corp",
    "employees": [
      {
        "id": 1001,
        "name": "Alice",
        "position": "Developer",
        "salary": 75000.00,
        "remote": true,
        "projects": ["WebApp", "MobileApp"],
        "contact": {
          "email": "alice@techcorp.com",
          "phone": "+1-555-0123"
        }
      },
      {
        "id": 1002,
        "name": "Bob",
        "position": "Designer",
        "salary": 65000.50,
        "remote": false,
        "projects": ["UI/UX", "Branding"],
        "contact": {
          "email": "bob@techcorp.com",
          "phone": null
        }
      }
    ],
    "founded": 2015,
    "headquarters": "San Francisco",
    "publiclyTraded": false
  }
}
```

## 数组和数值格式

```json
{
  "numbers": {
    "integer": 42,
    "negative": -123,
    "decimal": 3.14159,
    "scientific": 1.23e-4,
    "largeNumber": 1.5E+10
  },
  "arrays": {
    "strings": ["hello", "world", "json"],
    "numbers": [1, 2, 3, 4.5, -6.7],
    "mixed": ["text", 123, true, null, {"nested": "object"}],
    "empty": []
  },
  "booleans": {
    "isTrue": true,
    "isFalse": false
  },
  "nullValue": null
}
```

## 配置文件示例

```json
{
  "app": {
    "name": "MarkdownEditor",
    "version": "1.0.0",
    "environment": "development"
  },
  "database": {
    "host": "localhost",
    "port": 5432,
    "name": "markdown_db",
    "ssl": true,
    "credentials": {
      "username": "admin",
      "password": "secretPassword123"
    }
  },
  "logging": {
    "level": "debug",
    "outputs": ["console", "file"],
    "rotation": {
      "enabled": true,
      "maxSize": "10MB",
      "maxFiles": 5
    }
  },
  "features": {
    "syntaxHighlighting": true,
    "autoSave": true,
    "livePreview": false,
    "themes": ["dark", "light", "auto"]
  }
}
```

## 多语言字符串

```json
{
  "messages": {
    "en": {
      "welcome": "Welcome to our application!",
      "goodbye": "Thank you for using our service."
    },
    "zh": {
      "welcome": "欢迎使用我们的应用程序！",
      "goodbye": "感谢您使用我们的服务。"
    },
    "emoji": {
      "celebration": "🎉 Congratulations! 🎊",
      "warning": "⚠️ Please be careful ⚠️"
    }
  },
  "unicode": {
    "symbols": "☆★♥♦♣♠",
    "math": "∑∏∫√∞≠≤≥",
    "arrows": "→←↑↓↔↕"
  }
}
```

这个测试文件展示了JSON语法高亮器能够正确识别和着色的各种JSON语法元素：

- **属性键**：以红色高亮显示
- **字符串值**：以绿色高亮显示  
- **数字**：以橙色高亮显示（支持整数、小数、科学计数法）
- **布尔值**：以紫色高亮显示（true/false）
- **null值**：以紫色高亮显示
- **大括号**：以蓝色高亮显示
- **方括号**：以黄色高亮显示
- **标点符号**：冒号和逗号以适当颜色显示
