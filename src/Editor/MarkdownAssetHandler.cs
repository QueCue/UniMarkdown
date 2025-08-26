using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 处理Markdown文件的双击打开事件，自动使用MarkdownEditorWindow打开
    /// </summary>
    public static class MarkdownAssetHandler
    {
        /// <summary>
        /// 处理资源双击事件
        /// </summary>
        /// <param name="instanceID">资源实例ID</param>
        /// <param name="line">行号（用于代码文件）</param>
        /// <returns>如果处理了该资源返回true，否则返回false</returns>
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // 获取资源对象
            Object target = EditorUtility.InstanceIDToObject(instanceID);
            
            if (target == null)
                return false;

            // 检查是否为TextAsset
            TextAsset textAsset = target as TextAsset;
            if (textAsset == null)
                return false;

            // 获取资源路径
            string assetPath = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrEmpty(assetPath))
                return false;

            // 检查文件扩展名是否为Markdown
            string extension = Path.GetExtension(assetPath).ToLower();
            if (extension != ".md" && extension != ".markdown")
                return false;

            // 获取文件的完整路径
            string fullPath = Path.GetFullPath(assetPath);
            
            // 使用MarkdownEditorWindow打开文件
            MarkdownEditorWindow.OpenMarkdownFile(fullPath);
            
            // 返回true表示我们已经处理了这个资源的打开事件
            return true;
        }
    }
}
