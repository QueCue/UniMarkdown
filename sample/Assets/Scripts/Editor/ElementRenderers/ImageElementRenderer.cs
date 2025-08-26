using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniMarkdown.Editor
{
    /// <summary>
    /// 图片元素渲染器，专门负责渲染图片元素
    /// 完全提取自MarkdownRenderer的原始图片加载和渲染逻辑
    /// </summary>
    public sealed class ImageElementRenderer : BaseElementRenderer
    {
        // 下载进度信息
        private struct DownloadProgress
        {
            public long BytesReceived;
            public long TotalBytes;
            public float Percentage;
            public string SizeText;

            public DownloadProgress(long received, long total)
            {
                BytesReceived = received;
                TotalBytes = total;
                Percentage = total > 0 ? (float) received / total : 0f;
                SizeText = FormatFileSize(received, total);
            }

            private static string FormatFileSize(long received, long total)
            {
                string receivedStr = FormatBytes(received);
                if (total > 0)
                {
                    string totalStr = FormatBytes(total);
                    return $"{receivedStr} / {totalStr}";
                }

                return receivedStr;
            }
        }

        private static readonly Dictionary<string, Texture2D> g_imageCache = new();

        // 网络图片下载状态缓存
        private static readonly Dictionary<string, NetworkImageStatus> g_networkImageStatus = new();

        // 网络图片下载进度缓存
        private static readonly Dictionary<string, DownloadProgress> g_downloadProgress = new();

        private GUIContent m_failLoadIcon;

        /// <summary>
        /// 支持的元素类型：图片
        /// </summary>
        public override MarkdownElementType SupportedElementType => MarkdownElementType.Image;

        private GUIContent FailLoadIcon
        {
            get
            {
                if (null != m_failLoadIcon)
                {
                    return m_failLoadIcon;
                }

                m_failLoadIcon = new GUIContent("图片加载失败",
                    EditorGUIUtility.IconContent("RawImage Icon").image);

                return m_failLoadIcon;
            }
        }

        /// <summary>
        /// 图片渲染器的优先级
        /// </summary>
        public override int Priority => 10;

        /// <summary>
        /// 渲染图片元素 - 完全提取自MarkdownRenderer.RenderImage
        /// </summary>
        /// <param name="element">图片元素</param>
        /// <param name="isInline">是否为行内元素</param>
        protected override void OnRender(MarkdownElement element, bool isInline)
        {
            // 检测是否为badge并特殊处理
            if (IsBadgeUrl(element.url))
            {
                RenderBadge(element, isInline);
                return;
            }

            // 处理网络图片的异步加载
            if (IsNetworkUrl(element.url))
            {
                RenderNetworkImage(element);
            }
            else
            {
                // 处理本地图片的同步加载
                RenderLocalImage(element);
            }
        }

        /// <summary>
        /// 渲染网络图片（异步加载）
        /// </summary>
        /// <param name="element">图片元素</param>
        private void RenderNetworkImage(MarkdownElement element)
        {
            // 检查缓存中是否已有图片
            if (g_imageCache.TryGetValue(element.url, out Texture2D cachedTexture) &&
                cachedTexture != null)
            {
                RenderImageTexture(element, cachedTexture);
                return;
            }

            // 检查下载状态
            if (!g_networkImageStatus.TryGetValue(element.url, out NetworkImageStatus status))
            {
                status = NetworkImageStatus.NotStarted;
                g_networkImageStatus[element.url] = status;
            }

            switch (status)
            {
                case NetworkImageStatus.NotStarted:
                    // 开始异步下载
                    StartNetworkImageDownload(element.url);
                    GUILayout.Label($"[正在加载图片...] {element.url}", EditorStyles.miniLabel);
                    break;

                case NetworkImageStatus.Downloading:
                    // 显示下载进度
                    if (g_downloadProgress.TryGetValue(element.url, out DownloadProgress progress))
                    {
                        string progressText;
                        if (progress.TotalBytes > 0)
                        {
                            progressText = $"[正在下载图片... {progress.Percentage:P1}] {progress.SizeText}";
                        }
                        else
                        {
                            progressText = $"[正在下载图片...] {FormatBytes(progress.BytesReceived)}";
                        }

                        GUILayout.Label(progressText, EditorStyles.miniLabel);

                        // 显示进度条（如果有总大小信息）
                        if (progress.TotalBytes > 0)
                        {
                            Rect rect = GUILayoutUtility.GetRect(200, 16);
                            EditorGUI.ProgressBar(rect, progress.Percentage, $"{progress.Percentage:P1}");
                        }
                    }
                    else
                    {
                        GUILayout.Label($"[正在下载图片...] {element.url}", EditorStyles.miniLabel);
                    }

                    break;

                case NetworkImageStatus.Failed:
                    // 显示失败状态
                    GUILayout.Label($"[图片下载失败] {element.url}", EditorStyles.miniLabel);
                    if (!string.IsNullOrEmpty(element.altText))
                    {
                        GUILayout.Label($"Alt文本: {element.altText}", EditorStyles.miniLabel);
                    }

                    break;

                case NetworkImageStatus.Completed:
                    // 下载完成，从缓存获取图片
                    if (g_imageCache.TryGetValue(element.url, out Texture2D completedTexture) &&
                        completedTexture != null)
                    {
                        RenderImageTexture(element, completedTexture);
                    }
                    else
                    {
                        GUILayout.Label($"[图片缓存丢失] {element.url}", EditorStyles.miniLabel);
                        status = NetworkImageStatus.NotStarted;
                        g_networkImageStatus[element.url] = status;
                    }

                    break;
            }
        }

        /// <summary>
        /// 渲染本地图片（同步加载）
        /// </summary>
        /// <param name="element">图片元素</param>
        private void RenderLocalImage(MarkdownElement element)
        {
            Texture2D texture = LoadAndCacheLocalImage(element.url);
            if (texture != null)
            {
                RenderImageTexture(element, texture);
            }
            else
            {
                Color oriColor = GUI.color;
                GUI.color = Color.gray;
                // 图片加载失败时，显示更清晰的错误信息
                string label = !string.IsNullOrEmpty(element.altText)
                    ? $"[{element.altText}]{element.url}"
                    : $"{element.url}";

                GUILayout.Label(FailLoadIcon, EditorStyles.miniLabel);
                GUILayout.Label(label, EditorStyles.miniLabel);
                GUI.color = oriColor;
            }
        }

        /// <summary>
        /// 渲染图片纹理（通用方法）
        /// </summary>
        /// <param name="element">图片元素</param>
        /// <param name="texture">图片纹理</param>
        private void RenderImageTexture(MarkdownElement element, Texture2D texture)
        {
            // 计算最终显示尺寸
            CalculateDisplaySize(texture, element, out float displayWidth, out float displayHeight);

            // 使用GUI.DrawTexture实现多种缩放模式
            Rect imageRect = GUILayoutUtility.GetRect(displayWidth,
                displayHeight,
                GUILayout.Width(displayWidth),
                GUILayout.Height(displayHeight));

            // 根据是否为自定义尺寸决定缩放模式
            ScaleMode scaleMode;
            if (element.imageWidth <= 0 &&
                element.imageHeight <= 0)
            {
                // 没有自定义尺寸时保持宽高比
                scaleMode = ScaleMode.ScaleToFit;
            }
            else
            {
                // 有自定义尺寸时强制拉伸到指定尺寸
                scaleMode = ScaleMode.StretchToFill;
            }

            GUI.DrawTexture(imageRect, texture, scaleMode);
        }

        /// <summary>
        /// 计算图片显示尺寸
        /// </summary>
        /// <param name="texture">原始纹理</param>
        /// <param name="element">图片元素</param>
        /// <param name="displayWidth">输出显示宽度</param>
        /// <param name="displayHeight">输出显示高度</param>
        private void CalculateDisplaySize(Texture2D texture, MarkdownElement element, out float displayWidth,
            out float displayHeight)
        {
            float originalWidth = texture.width;
            float originalHeight = texture.height;

            // 默认最大尺寸限制
            var maxWidth = 400f;
            var maxHeight = 300f;

            // 检查是否有自定义尺寸
            if (element.imageWidth > 0 ||
                element.imageHeight > 0)
            {
                if (element.imageIsPercentage)
                {
                    // 百分比尺寸 - 基于默认最大尺寸计算
                    displayWidth = element.imageWidth > 0
                        ? maxWidth * element.imageWidth / 100f
                        : originalWidth;

                    displayHeight = element.imageHeight > 0
                        ? maxHeight * element.imageHeight / 100f
                        : originalHeight;
                }
                else
                {
                    // 像素尺寸
                    displayWidth = element.imageWidth > 0 ? element.imageWidth : originalWidth;
                    displayHeight = element.imageHeight > 0 ? element.imageHeight : originalHeight;
                }

                // 如果只指定了宽度或高度，按比例计算另一个维度
                if (element.imageWidth > 0 &&
                    element.imageHeight <= 0)
                {
                    displayHeight = displayWidth * (originalHeight / originalWidth);
                }
                else if (element.imageHeight > 0 &&
                    element.imageWidth <= 0)
                {
                    displayWidth = displayHeight * (originalWidth / originalHeight);
                }
            }
            else
            {
                // 使用默认的尺寸限制逻辑
                displayWidth = originalWidth;
                displayHeight = originalHeight;

                if (displayWidth > maxWidth)
                {
                    displayHeight *= maxWidth / displayWidth;
                    displayWidth = maxWidth;
                }

                if (displayHeight > maxHeight)
                {
                    displayWidth *= maxHeight / displayHeight;
                    displayHeight = maxHeight;
                }
            }
        }

        /// <summary>
        /// 开始异步下载网络图片
        /// </summary>
        /// <param name="url">图片URL</param>
        private void StartNetworkImageDownload(string url)
        {
            g_networkImageStatus[url] = NetworkImageStatus.Downloading;

            // 立即初始化进度信息
            g_downloadProgress[url] = new DownloadProgress(0, -1);

            // 使用Unity的协程系统进行异步下载
            var downloadCoroutine
                = new NetworkImageDownloader(url, OnImageDownloadComplete, OnImageDownloadProgressChanged);

            downloadCoroutine.Start();
        }

        /// <summary>
        /// 图片下载完成回调
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <param name="texture">下载的纹理（null表示失败）</param>
        private void OnImageDownloadComplete(string url, Texture2D texture)
        {
            if (texture != null)
            {
                g_imageCache[url] = texture;
                g_networkImageStatus[url] = NetworkImageStatus.Completed;
            }
            else
            {
                g_networkImageStatus[url] = NetworkImageStatus.Failed;
                Debug.LogError($"[IMAGE] 异步下载失败: {url}");
            }

            // 刷新GUI显示
            EditorApplication.QueuePlayerLoopUpdate();
        }

        private void OnImageDownloadProgressChanged()
        {
            Repaint();
        }

        /// <summary>
        /// 专门用于Badge的下载方法（使用PNG URL下载，但关联到原始URL）
        /// </summary>
        /// <param name="originalUrl">原始badge URL（用作缓存键）</param>
        /// <param name="downloadUrl">实际下载的PNG格式URL</param>
        private void StartNetworkImageDownloadForBadge(string originalUrl, string downloadUrl)
        {
            g_networkImageStatus[originalUrl] = NetworkImageStatus.Downloading;

            // 立即初始化进度信息
            g_downloadProgress[originalUrl] = new DownloadProgress(0, -1);

            // 使用Unity的协程系统进行异步下载
            var downloadCoroutine = new NetworkImageDownloader(downloadUrl,
                (url, texture) => OnBadgeDownloadComplete(originalUrl, url, texture),
                OnImageDownloadProgressChanged);

            downloadCoroutine.Start();
        }

        /// <summary>
        /// Badge下载完成回调
        /// </summary>
        /// <param name="originalUrl">原始badge URL</param>
        /// <param name="downloadUrl">实际下载的URL</param>
        /// <param name="texture">下载的纹理（null表示失败）</param>
        private void OnBadgeDownloadComplete(string originalUrl, string downloadUrl, Texture2D texture)
        {
            if (texture != null)
            {
                // 使用原始URL作为缓存键
                g_imageCache[originalUrl] = texture;
                g_networkImageStatus[originalUrl] = NetworkImageStatus.Completed;
            }
            else
            {
                g_networkImageStatus[originalUrl] = NetworkImageStatus.Failed;
                Debug.LogError($"[BADGE] Badge下载失败: {originalUrl} (PNG URL: {downloadUrl})");
            }

            // 刷新GUI显示
            EditorApplication.QueuePlayerLoopUpdate();
        }

        /// <summary>
        /// 从字节数据创建纹理（主线程调用）
        /// </summary>
        /// <param name="imageData">图片字节数据</param>
        /// <returns>创建的纹理</returns>
        private static Texture2D CreateTextureFromData(byte[] imageData)
        {
            try
            {
                var texture = new Texture2D(2, 2);
                if (texture.LoadImage(imageData))
                {
                    return texture;
                }

                Object.DestroyImmediate(texture);
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[IMAGE] 创建纹理失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 格式化字节数为可读的文件大小
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化后的文件大小字符串</returns>
        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} B";
            }

            if (bytes < 1024 * 1024)
            {
                return $"{bytes / 1024f:F1} KB";
            }

            if (bytes < 1024 * 1024 * 1024)
            {
                return $"{bytes / (1024f * 1024f):F1} MB";
            }

            return $"{bytes / (1024f * 1024f * 1024f):F1} GB";
        }

        /// <summary>
        /// 加载本地图片 - 支持本地路径（相对、绝对）
        /// </summary>
        /// <param name="imagePath">图片路径</param>
        /// <returns>加载的纹理，失败时返回null</returns>
        private Texture2D LoadAndCacheLocalImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }

            // 使用缓存避免重复加载
            if (g_imageCache.TryGetValue(imagePath, out Texture2D cachedTexture))
            {
                return cachedTexture;
            }

            Texture2D texture = null;

            // 智能路径识别和处理（仅本地图片）
            try
            {
                // 检查是否为本地绝对路径
                if (Path.IsPathRooted(imagePath))
                {
                    texture = LoadImageFromAbsolutePath(imagePath);
                }
                // 尝试从Assets文件夹加载
                else if (imagePath.StartsWith("Assets/"))
                {
                    texture = LoadImageFromAssets(imagePath);
                }
                // 尝试从StreamingAssets加载
                else
                {
                    texture = LoadImageFromStreamingAssets(imagePath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[IMAGE] 图片加载异常: {imagePath}, 错误: {ex.Message}");
                if (texture != null)
                {
                    Object.DestroyImmediate(texture);
                    texture = null;
                }
            }

            // 缓存结果（包括null结果，避免重复尝试）
            g_imageCache[imagePath] = texture;

            return texture;
        }

        /// <summary>
        /// 检查文件扩展名是否为支持的图片格式 - 提取自MarkdownRenderer.IsValidImageFormat
        /// </summary>
        /// <param name="extension">文件扩展名（带点）</param>
        /// <returns>是否为支持的图片格式</returns>
        private bool IsValidImageFormat(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            // Unity支持的图片格式
            string[] supportedFormats =
            {
                ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".exr", ".hdr", ".tif", ".tiff"
            };

            return Array.Exists(supportedFormats,
                format => string.Equals(format, extension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 检查路径是否为网络URL
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>是否为网络URL</returns>
        private bool IsNetworkUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            return path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 从本地绝对路径加载图片
        /// </summary>
        /// <param name="absolutePath">绝对路径</param>
        /// <returns>加载的纹理，失败时返回null</returns>
        private Texture2D LoadImageFromAbsolutePath(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                Debug.LogError($"[IMAGE] 绝对路径文件不存在: {absolutePath}");
                return null;
            }

            return LoadImageFromFile(absolutePath);
        }

        /// <summary>
        /// 从Assets文件夹加载图片
        /// </summary>
        /// <param name="assetPath">Assets相对路径</param>
        /// <returns>加载的纹理，失败时返回null</returns>
        private Texture2D LoadImageFromAssets(string assetPath)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture != null)
            {
                return texture;
            }

            Debug.LogWarning($"[IMAGE] AssetDatabase加载失败: {assetPath}");
            return null;
        }

        /// <summary>
        /// 从StreamingAssets文件夹加载图片
        /// </summary>
        /// <param name="relativePath">StreamingAssets相对路径</param>
        /// <returns>加载的纹理，失败时返回null</returns>
        private Texture2D LoadImageFromStreamingAssets(string relativePath)
        {
            // 首先尝试通过AssetDatabase加载StreamingAssets中的文件
            var assetPath = $"Assets/StreamingAssets/{relativePath}";
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (texture != null)
            {
                return texture;
            }

            // 如果AssetDatabase加载失败，尝试文件读取方式
            string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, relativePath);
            streamingAssetsPath = streamingAssetsPath.Replace("\\", "/"); // 确保路径格式正确

            if (File.Exists(streamingAssetsPath))
            {
                return LoadImageFromFile(streamingAssetsPath);
            }

            Debug.LogError($"[IMAGE] StreamingAssets图片文件不存在: {streamingAssetsPath}");
            return null;
        }

        /// <summary>
        /// 从文件系统加载图片的通用方法
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>加载的纹理，失败时返回null</returns>
        private Texture2D LoadImageFromFile(string filePath)
        {
            try
            {
                // 添加文件信息诊断
                var fileInfo = new FileInfo(filePath);

                // 检查文件是否为有效的图片格式
                string extension = fileInfo.Extension.ToLower();
                if (!IsValidImageFormat(extension))
                {
                    Debug.LogError($"[IMAGE] 不支持的图片格式: {extension}, 支持的格式: .png, .jpg, .jpeg, .bmp, .tga");
                    return null;
                }

                // 检查文件大小是否合理
                if (fileInfo.Length == 0)
                {
                    Debug.LogError($"[IMAGE] 图片文件为空: {filePath}");
                    return null;
                }

                if (fileInfo.Length > 50 * 1024 * 1024) // 50MB限制
                {
                    Debug.LogError($"[IMAGE] 图片文件过大 ({fileInfo.Length / (1024 * 1024)}MB): {filePath}");
                    return null;
                }

                // 读取并加载图片数据
                byte[] imageData = File.ReadAllBytes(filePath);
                var texture = new Texture2D(2, 2);

                if (texture.LoadImage(imageData))
                {
                    return texture;
                }

                Debug.LogError($"[IMAGE] 图片数据格式不支持或损坏: {filePath}");
                Debug.LogError($"[IMAGE] 文件大小: {imageData.Length} bytes");
                Debug.LogError($"[IMAGE] 文件扩展名: {extension}");

                // 检查文件头部字节以诊断格式问题
                if (imageData.Length >= 4)
                {
                    var hexHeader = BitConverter.ToString(imageData, 0, Math.Min(8, imageData.Length));
                    Debug.LogError($"[IMAGE] 文件头部字节: {hexHeader}");
                }

                Object.DestroyImmediate(texture);
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[IMAGE] 读取文件时发生异常: {filePath}, 错误: {ex.Message}");
                return null;
            }
        }

        protected override void OnDispose()
        {
            g_imageCache?.Clear();
            g_networkImageStatus?.Clear();
            m_failLoadIcon = null;
        }

        // 网络图片下载状态枚举
        private enum NetworkImageStatus
        {
            NotStarted, // 未开始下载
            Downloading, // 正在下载
            Failed, // 下载失败
            Completed // 下载完成
        }

        /// <summary>
        /// 网络图片下载器（支持进度跟踪）
        /// </summary>
        private class NetworkImageDownloader
        {
            private readonly Action m_onProgressChanged;
            private readonly Action<string, Texture2D> m_onComplete;
            private readonly string m_url;

            public NetworkImageDownloader(string url, Action<string, Texture2D> onComplete,
                Action onProgressChanged)
            {
                m_url = url;
                m_onComplete = onComplete;
                m_onProgressChanged = onProgressChanged;
            }

            public void Start()
            {
                // 使用Unity的延迟调用在下一帧开始下载
                EditorApplication.CallbackFunction downloadCallback = null;
                downloadCallback = () =>
                {
                    EditorApplication.update -= downloadCallback;
                    StartDownload();
                };

                EditorApplication.update += downloadCallback;
            }

            private void StartDownload()
            {
                try
                {
                    // 创建WebClient进行下载
                    var webClient = new WebClient();
                    webClient.Headers.Add("User-Agent", "Unity-MarkdownEditor/1.0");

                    // 注册进度回调
                    webClient.DownloadProgressChanged += (sender, e) =>
                    {
                        // 直接更新进度（这个事件在主线程触发）
                        var progress = new DownloadProgress(e.BytesReceived, e.TotalBytesToReceive);
                        g_downloadProgress[m_url] = progress;
                        m_onProgressChanged?.Invoke();
                    };

                    // 异步下载完成事件
                    webClient.DownloadDataCompleted += (sender, e) =>
                    {
                        EditorApplication.delayCall += () =>
                        {
                            ProcessDownloadResult(e);
                            m_onProgressChanged?.Invoke();
                            webClient.Dispose();
                        };
                    };

                    // 开始异步下载
                    webClient.DownloadDataAsync(new Uri(m_url));
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[IMAGE] 启动异步下载失败: {m_url}, 错误: {ex.Message}");
                    g_networkImageStatus[m_url] = NetworkImageStatus.Failed;
                    m_onComplete?.Invoke(m_url, null);
                }
            }

            private void ProcessDownloadResult(DownloadDataCompletedEventArgs e)
            {
                try
                {
                    if (e.Error != null)
                    {
                        Debug.LogError($"[IMAGE] 下载错误: {m_url}, 错误: {e.Error.Message}");
                        g_networkImageStatus[m_url] = NetworkImageStatus.Failed;
                        g_downloadProgress.Remove(m_url); // 清理进度信息
                        m_onComplete?.Invoke(m_url, null);
                        return;
                    }

                    if (e.Cancelled)
                    {
                        Debug.LogWarning($"[IMAGE] 下载被取消: {m_url}");
                        g_networkImageStatus[m_url] = NetworkImageStatus.Failed;
                        g_downloadProgress.Remove(m_url); // 清理进度信息
                        m_onComplete?.Invoke(m_url, null);
                        return;
                    }

                    byte[] imageData = e.Result;
                    if (imageData == null ||
                        imageData.Length == 0)
                    {
                        Debug.LogError($"[IMAGE] 网络图片数据为空: {m_url}");
                        g_networkImageStatus[m_url] = NetworkImageStatus.Failed;
                        g_downloadProgress.Remove(m_url); // 清理进度信息
                        m_onComplete?.Invoke(m_url, null);
                        return;
                    }

                    if (imageData.Length > 50 * 1024 * 1024) // 50MB限制
                    {
                        Debug.LogError($"[IMAGE] 网络图片文件过大 ({imageData.Length / (1024 * 1024)}MB): {m_url}");
                        g_networkImageStatus[m_url] = NetworkImageStatus.Failed;
                        g_downloadProgress.Remove(m_url); // 清理进度信息
                        m_onComplete?.Invoke(m_url, null);
                        return;
                    }

                    // 在主线程中创建纹理
                    Texture2D texture = CreateTextureFromData(imageData);

                    // 更新下载状态
                    if (texture != null)
                    {
                        g_networkImageStatus[m_url] = NetworkImageStatus.Completed;
                    }
                    else
                    {
                        g_networkImageStatus[m_url] = NetworkImageStatus.Failed;
                    }

                    // 清理进度信息
                    g_downloadProgress.Remove(m_url);

                    m_onComplete?.Invoke(m_url, texture);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[IMAGE] 处理下载结果失败: {m_url}, 错误: {ex.Message}");
                    g_networkImageStatus[m_url] = NetworkImageStatus.Failed;
                    g_downloadProgress.Remove(m_url); // 清理进度信息
                    m_onComplete?.Invoke(m_url, null);
                }
            }
        }

        #region Badge Support

        /// <summary>
        /// 检测URL是否为badge（徽章）
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <returns>是否为badge</returns>
        private static bool IsBadgeUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            // 常见的badge服务域名
            string[] badgeHosts =
            {
                "shields.io", "img.shields.io", "badge.fury.io", "travis-ci.org", "travis-ci.com",
                "circleci.com", "codecov.io", "coveralls.io", "badgen.net", "flat.badgen.net"
            };

            try
            {
                var uri = new Uri(url);
                string host = uri.Host.ToLower();

                foreach (string badgeHost in badgeHosts)
                {
                    if (host.Contains(badgeHost))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // URL解析失败，不是badge
                return false;
            }

            return false;
        }

        /// <summary>
        /// 将Badge URL转换为PNG格式（Unity支持的格式）
        /// </summary>
        /// <param name="url">原始badge URL</param>
        /// <returns>PNG格式的badge URL</returns>
        private static string ConvertBadgeUrlToPng(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            try
            {
                var uri = new Uri(url);
                string host = uri.Host.ToLower();

                // shields.io系列服务：添加.png后缀
                if (host.Contains("shields.io"))
                {
                    // 如果URL已经以图片格式结尾，直接返回
                    string path = uri.AbsolutePath.ToLower();
                    if (path.EndsWith(".png") ||
                        path.EndsWith(".jpg") ||
                        path.EndsWith(".jpeg"))
                    {
                        return url;
                    }

                    // 添加.png后缀
                    var builder = new UriBuilder(uri);
                    builder.Path = uri.AbsolutePath + ".png";
                    return builder.ToString();
                }

                // badge.fury.io: 添加.png后缀
                if (host.Contains("badge.fury.io"))
                {
                    if (!uri.AbsolutePath.ToLower().EndsWith(".png"))
                    {
                        var builder = new UriBuilder(uri);
                        builder.Path = uri.AbsolutePath + ".png";
                        return builder.ToString();
                    }
                }

                // badgen.net: 支持.png后缀
                if (host.Contains("badgen.net"))
                {
                    if (!uri.AbsolutePath.ToLower().EndsWith(".png"))
                    {
                        var builder = new UriBuilder(uri);
                        builder.Path = uri.AbsolutePath + ".png";
                        return builder.ToString();
                    }
                }

                // 对于codecov, travis等不支持简单PNG转换的服务，直接使用原URL
                // 这些服务通常返回SVG，我们会在后续处理中显示占位符
                if (host.Contains("codecov.io") ||
                    host.Contains("travis-ci.org") ||
                    host.Contains("travis-ci.com") ||
                    host.Contains("circleci.com") ||
                    host.Contains("coveralls.io"))
                {
                    Debug.LogWarning($"[BADGE] 该Badge服务可能不支持PNG格式，将显示占位符: {host}");
                    return url; // 返回原URL，但后续会处理为占位符
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[BADGE] 转换Badge URL到PNG格式失败: {url}, 错误: {ex.Message}");
            }

            return url; // 转换失败时返回原URL
        }

        /// <summary>
        /// 渲染badge（徽章）
        /// </summary>
        /// <param name="element">图片元素</param>
        /// <param name="isInline">是否为行内元素</param>
        private void RenderBadge(MarkdownElement element, bool isInline)
        {
            // Badge应该行内显示，有特殊的尺寸处理
            if (IsNetworkUrl(element.url))
            {
                RenderNetworkBadge(element);
            }
            else
            {
                // 本地badge（不太常见，但也支持）
                RenderLocalBadge(element);
            }
        }

        /// <summary>
        /// 渲染网络badge
        /// </summary>
        /// <param name="element">图片元素</param>
        private void RenderNetworkBadge(MarkdownElement element)
        {
            // 检查是否为不支持PNG转换的服务
            if (IsUnsupportedBadgeService(element.url))
            {
                RenderBadgeAsText(element);
                return;
            }

            // 检查缓存（使用原始URL作为键）
            if (g_imageCache.TryGetValue(element.url, out Texture2D cachedTexture) &&
                cachedTexture != null)
            {
                RenderBadgeTexture(element, cachedTexture);
                return;
            }

            // 转换为PNG格式的URL用于下载
            string downloadUrl = ConvertBadgeUrlToPng(element.url);

            // 检查下载状态（使用原始URL作为状态键）
            if (!g_networkImageStatus.TryGetValue(element.url, out NetworkImageStatus status))
            {
                status = NetworkImageStatus.NotStarted;
                g_networkImageStatus[element.url] = status;
            }

            switch (status)
            {
                case NetworkImageStatus.NotStarted:
                    // 显示占位符并开始下载（使用PNG URL下载，但状态用原始URL跟踪）
                    RenderBadgePlaceholder(element);
                    StartNetworkImageDownloadForBadge(element.url, downloadUrl);
                    g_networkImageStatus[element.url] = NetworkImageStatus.Downloading;
                    break;

                case NetworkImageStatus.Downloading:
                    // 显示下载进度或占位符
                    RenderBadgePlaceholder(element);
                    break;

                case NetworkImageStatus.Failed:
                    // 显示失败状态，fallback到文本显示
                    RenderBadgeAsText(element);
                    break;

                case NetworkImageStatus.Completed:
                    // 从缓存获取并渲染
                    if (g_imageCache.TryGetValue(element.url, out Texture2D completedTexture) &&
                        completedTexture != null)
                    {
                        RenderBadgeTexture(element, completedTexture);
                    }
                    else
                    {
                        RenderBadgeAsText(element);
                        status = NetworkImageStatus.NotStarted;
                        g_networkImageStatus[element.url] = status;
                    }

                    break;
            }
        }

        /// <summary>
        /// 检查是否为不支持PNG转换的Badge服务
        /// </summary>
        /// <param name="url">Badge URL</param>
        /// <returns>是否不支持PNG转换</returns>
        private static bool IsUnsupportedBadgeService(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            try
            {
                var uri = new Uri(url);
                string host = uri.Host.ToLower();

                // 这些服务已知不支持简单的PNG转换
                return host.Contains("codecov.io") || host.Contains("travis-ci.org") ||
                    host.Contains("travis-ci.com") || host.Contains("circleci.com") ||
                    host.Contains("coveralls.io");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将Badge渲染为文本（fallback方案）
        /// </summary>
        /// <param name="element">图片元素</param>
        private void RenderBadgeAsText(MarkdownElement element)
        {
            // 提取Badge名称（从URL或alt text）
            string badgeName = GetBadgeName(element);

            using (new GUILayout.HorizontalScope())
            {
                // 使用小字体和背景色模拟badge外观
                var style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = Mathf.RoundToInt(GUI.skin.label.fontSize * 0.8f),
                    padding = new RectOffset(4, 4, 2, 2),
                    normal =
                    {
                        background = CreateSolidTexture(new Color(0.5f, 0.5f, 0.5f, 0.8f)),
                        textColor = Color.white
                    }
                };

                GUILayout.Label($"[{badgeName}]",
                    style,
                    GUILayout.Height(EditorGUIUtility.singleLineHeight * 0.8f));
            }
        }

        /// <summary>
        /// 从Badge URL或element中提取Badge名称
        /// </summary>
        /// <param name="element">图片元素</param>
        /// <returns>Badge名称</returns>
        private static string GetBadgeName(MarkdownElement element)
        {
            // 优先使用alt text
            if (!string.IsNullOrEmpty(element.altText))
            {
                return element.altText;
            }

            // 从URL中提取badge信息
            try
            {
                var uri = new Uri(element.url);
                string host = uri.Host.ToLower();

                if (host.Contains("shields.io"))
                {
                    // shields.io的URL通常包含badge信息
                    string path = uri.AbsolutePath;
                    if (path.StartsWith("/badge/"))
                    {
                        return path.Substring(7).Replace("-", " ");
                    }
                }

                // 其他服务使用域名
                return host.Replace("www.", "").Split('.')[0];
            }
            catch
            {
                return "Badge";
            }
        }

        /// <summary>
        /// 创建纯色纹理（用于Badge背景）
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns>纹理</returns>
        private static Texture2D CreateSolidTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// 渲染本地badge
        /// </summary>
        /// <param name="element">图片元素</param>
        private void RenderLocalBadge(MarkdownElement element)
        {
            Texture2D texture = LoadAndCacheLocalImage(element.url);
            if (texture != null)
            {
                RenderBadgeTexture(element, texture);
            }
            else
            {
                RenderBadgeFailure(element);
            }
        }

        /// <summary>
        /// 渲染badge纹理
        /// </summary>
        /// <param name="element">图片元素</param>
        /// <param name="texture">纹理</param>
        private void RenderBadgeTexture(MarkdownElement element, Texture2D texture)
        {
            // Badge的特殊尺寸处理
            float badgeHeight = EditorGUIUtility.singleLineHeight; // 与文本行高一致
            float aspectRatio = (float) texture.width / texture.height;
            float badgeWidth = badgeHeight * aspectRatio;

            // 确保badge不会太宽
            var maxWidth = 200f;
            if (badgeWidth > maxWidth)
            {
                badgeWidth = maxWidth;
                badgeHeight = badgeWidth / aspectRatio;
            }

            // 行内渲染，与文本对齐
            Rect badgeRect = GUILayoutUtility.GetRect(badgeWidth,
                badgeHeight,
                GUILayout.Width(badgeWidth),
                GUILayout.Height(badgeHeight));

            GUI.DrawTexture(badgeRect, texture, ScaleMode.ScaleToFit);
        }

        /// <summary>
        /// 渲染badge占位符
        /// </summary>
        /// <param name="element">图片元素</param>
        private void RenderBadgePlaceholder(MarkdownElement element)
        {
            float placeholderHeight = EditorGUIUtility.singleLineHeight;
            var placeholderWidth = 80f; // 预估宽度

            var placeholderStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { background = Texture2D.grayTexture }
            };

            Rect placeholderRect = GUILayoutUtility.GetRect(placeholderWidth,
                placeholderHeight,
                GUILayout.Width(placeholderWidth),
                GUILayout.Height(placeholderHeight));

            GUI.Label(placeholderRect, "Loading...", placeholderStyle);
        }

        /// <summary>
        /// 渲染badge失败状态
        /// </summary>
        /// <param name="element">图片元素</param>
        private void RenderBadgeFailure(MarkdownElement element)
        {
            // 显示alt文本或简化的失败提示
            string displayText = !string.IsNullOrEmpty(element.altText) ? element.altText : "[Badge]";

            var failureStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.red } };

            GUILayout.Label(displayText, failureStyle, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }

        #endregion
    }
}