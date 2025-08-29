# Unity Markdown 编辑器示例文档

欢迎使用 Unity Editor 环境下的 Markdown 展示工具！这个文档演示了各种 Markdown 语法的支持。

## 文本格式

这是一段普通的文本内容，支持中文和英文混合显示。

### 强调文本

你可以使用 **粗体文本** 来强调重要内容。

你也可以使用 *斜体文本* 来表示特殊含义。

你还可以使用 ***粗斜体文本*** 来表示极其重要的内容。

## 代码展示

### 行内代码

在文本中可以插入 `行内代码`，比如变量名 `playerHealth` 或函数名 `GetComponent<Transform>()`。

### 代码块

下面是一个 C# Unity 脚本的示例：

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed = 5f;
    
    private Rigidbody m_rigidbody;
    
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        m_rigidbody.velocity = movement * m_moveSpeed;
    }
}
```

## 链接

你可以创建指向外部资源的链接：

- [Unity 官方文档](https://docs.unity3d.com/)
- [Unity Asset Store](https://assetstore.unity.com/)
- [Unity Learn](https://learn.unity.com/)

## 图片

下面展示如何在 Markdown 中插入图片：

![123](Examples/123.png){width=300}
![Unity](https://emoji.aranja.com/static/emoji-data/img-apple-160/1f600.png){width=30}

![Unity](https://img.shields.io/badge/Unity-2021.3%2B-FCC624?logo=unity&logoColor=white)
*注意：图片路径相对于 StreamingAssets 文件夹*

## 分割线

你可以使用分割线来分隔不同的内容段落：

---

上面是一条分割线，下面是另一条：

***

还可以用这种方式：

___

## 列表

### 无序列表

Unity 游戏开发的核心概念：

- GameObjects 和 Components
- Scenes 和 Prefabs
- Scripts 和 MonoBehaviour
- Physics 和 Colliders
- Animation 和 Timeline
- UI 系统 (uGUI)

### 有序列表

Unity 项目开发流程：

1. 项目规划和设计
1. 搭建基础场景
1. 创建核心脚本
1. 实现游戏逻辑
1. 添加美术资源
1. 优化和测试
7. 构建和发布

### 任务列表

项目待办事项：

- [x] 完成 Markdown 渲染器基础功能
- [x] 实现代码高亮
- [ ] 添加表格支持
- [ ] 优化图片加载性能
- [x] 完善文档和示例
- [ ] 发布到 Asset Store

## 更多示例

### Unity 性能优化技巧

在 Unity 开发中，性能优化是非常重要的。以下是一些常用的优化技巧：

**对象池模式**
```csharp
// 避免频繁的 Instantiate 和 Destroy
public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> m_pool = new Queue<GameObject>();
    
    public GameObject GetObject()
    {
        if (m_pool.Count > 0)
        {
            return m_pool.Dequeue();
        }
        return Instantiate(prefab);
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        m_pool.Enqueue(obj);
    }
}
```

**避免在 Update 中分配内存**
```csharp
// 错误做法
void Update()
{
    string message = "Player health: " + health; // 每帧分配字符串
}

// 正确做法
private StringBuilder m_stringBuilder = new StringBuilder();

void Update()
{
    m_stringBuilder.Clear();
    m_stringBuilder.Append("Player health: ");
    m_stringBuilder.Append(health);
    string message = m_stringBuilder.ToString();
}
```

### Unity 编码规范

根据项目编码规范，我们应该：

- 使用 **UniMarkdown** 命名空间
- 私有字段使用 `m_` 前缀
- 静态私有字段使用 `g_` 前缀
- 公共成员使用帕斯卡命名法
- 为类和公共接口添加完整的 XML 注释

### 结语

这个 Markdown 编辑器工具可以帮助 Unity 开发者：

1. **文档编写**：项目设计文档、API 文档
2. **代码展示**：代码片段演示和说明
3. **教程制作**：游戏开发教程和指南
4. **团队协作**：技术分享和知识管理

希望这个工具能够提升你的 Unity 开发效率！

## 渲染器功能完整展示

### 文本样式组合

普通文本、**粗体**、*斜体*、***粗斜体***、`行内代码`、~~删除线~~

### 标题层级

# 一级标题
## 二级标题  
### 三级标题
#### 四级标题
##### 五级标题
###### 六级标题

### 代码展示

行内代码：`console.log("Hello World")`

多种语言的代码块：

**C# 示例：**
```csharp
public class Example : MonoBehaviour { }
```

**JSON 示例：**
```json
{ "key": "value", "number": 42 }
```

**JavaScript 示例：**
```javascript
function hello() { console.log("Hello!"); }
```

### 链接和图片

- 链接：[Unity Documentation](https://docs.unity3d.com/)
- 图片：![Sample](Examples/123.png){width=50}
- 带尺寸的图片：![Unity Logo](https://emoji.aranja.com/static/emoji-data/img-apple-160/1f600.png){width=30}

### 列表类型

**无序列表：**
- 项目 1
- 项目 2
  - 子项目 2.1
  - 子项目 2.2
- 项目 3

**有序列表：**
1. 第一步
2. 第二步
3. 第三步

**任务列表：**
- [x] 已完成任务
- [ ] 待完成任务
- [x] 另一个已完成任务

### 其他元素

**分割线：**

---

**换行处理：**
这是第一行  
这是第二行（硬换行）

这是第三行（软换行）
这是第四行

---

*最后更新：2025-08-14*
*工具版本：Unity Markdown Editor v0.1.0*
