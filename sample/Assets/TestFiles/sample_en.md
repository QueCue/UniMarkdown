# Unity Markdown Editor Sample Document

Welcome to the Markdown display tool for Unity Editor environment! This document demonstrates support for various Markdown syntax features.

## Text Formatting

This is a paragraph of regular text content, supporting mixed display of Chinese and English.

### Emphasis Text

You can use **bold text** to emphasize important content.

You can also use *italic text* to indicate special meaning.

You can also use ***bold italic text*** to indicate extremely important content.

## Code Display

### Inline Code

You can insert `inline code` in text, such as variable names `playerHealth` or function names `GetComponent<Transform>()`.

### Code Blocks

Here's an example of a C# Unity script:

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

## Links

You can create links to external resources:

- [Unity Official Documentation](https://docs.unity3d.com/)
- [Unity Asset Store](https://assetstore.unity.com/)
- [Unity Learn](https://learn.unity.com/)

## Images

Here's how to insert images in Markdown:

![123](Examples/123.png){width=300}
![Unity](https://emoji.aranja.com/static/emoji-data/img-apple-160/1f600.png){width=30}

![Unity](https://img.shields.io/badge/Unity-2021.3%2B-FCC624?logo=unity&logoColor=white)
*Note: Image paths are relative to the StreamingAssets folder*

## Dividers

You can use dividers to separate different content sections:

---

Above is a divider, below is another one:

***

You can also use this style:

___

## Lists

### Unordered Lists

Core concepts of Unity game development:

- GameObjects and Components
- Scenes and Prefabs
- Scripts and MonoBehaviour
- Physics and Colliders
- Animation and Timeline
- UI System (uGUI)

### Ordered Lists

Unity project development workflow:

1. Project planning and design
2. Build basic scenes
3. Create core scripts
4. Implement game logic
5. Add art assets
6. Optimization and testing
7. Build and publish

### Task Lists

Project to-do items:

- [x] Complete Markdown renderer basic functionality
- [x] Implement code highlighting
- [ ] Add table support
- [ ] Optimize image loading performance
- [x] Improve documentation and examples
- [ ] Publish to Asset Store

## More Examples

### Unity Performance Optimization Tips

Performance optimization is very important in Unity development. Here are some commonly used optimization techniques:

**Object Pool Pattern**
```csharp
// Avoid frequent Instantiate and Destroy
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

**Avoid Memory Allocation in Update**
```csharp
// Wrong approach
void Update()
{
    string message = "Player health: " + health; // Allocates string every frame
}

// Correct approach
private StringBuilder m_stringBuilder = new StringBuilder();

void Update()
{
    m_stringBuilder.Clear();
    m_stringBuilder.Append("Player health: ");
    m_stringBuilder.Append(health);
    string message = m_stringBuilder.ToString();
}
```

### Unity Coding Standards

According to project coding standards, we should:

- Use **UniMarkdown** namespace
- Use `m_` prefix for private fields
- Use `g_` prefix for static private fields
- Use Pascal case for public members
- Add complete XML comments for classes and public interfaces

### Conclusion

This Markdown editor tool can help Unity developers with:

1. **Documentation Writing**: Project design documents, API documentation
2. **Code Display**: Code snippet demonstrations and explanations
3. **Tutorial Creation**: Game development tutorials and guides
4. **Team Collaboration**: Technical sharing and knowledge management

Hope this tool can improve your Unity development efficiency!

## Complete Renderer Feature Demo

### Text Style Combinations

Regular text, **bold**, *italic*, ***bold italic***, `inline code`, ~~strikethrough~~

### Heading Levels

# Heading 1
## Heading 2  
### Heading 3
#### Heading 4
##### Heading 5
###### Heading 6

### Code Display

Inline code: `console.log("Hello World")`

Code blocks in multiple languages:

**C# Example:**
```csharp
public class Example : MonoBehaviour { }
```

**JSON Example:**
```json
{ "key": "value", "number": 42 }
```

**JavaScript Example:**
```javascript
function hello() { console.log("Hello!"); }
```

### Links and Images

- Link: [Unity Documentation](https://docs.unity3d.com/)
- Image: ![Sample](Examples/123.png){width=50}
- Image with size: ![Unity Logo](https://emoji.aranja.com/static/emoji-data/img-apple-160/1f600.png){width=30}

### List Types

**Unordered List:**
- Item 1
- Item 2
  - Sub-item 2.1
  - Sub-item 2.2
- Item 3

**Ordered List:**
1. First step
2. Second step
3. Third step

**Task List:**
- [x] Completed task
- [ ] Pending task
- [x] Another completed task

### Other Elements

**Dividers:**

---

**Line Break Handling:**
This is the first line  
This is the second line (hard break)

This is the third line (soft break)
This is the fourth line

---

*Last updated: 2025-08-27*
*Tool version: Unity Markdown Editor v0.1.0*
