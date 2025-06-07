using Dota2Simulator.ImageProcessingSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Dota2Simulator.PictureProcessing
{
    /// <summary>
    /// 懒加载图片管理器 - 只在真正需要时才加载图片资源
    /// </summary>
    public class LazyImageLoader
    {
        // 使用 ConcurrentDictionary 确保线程安全
        private static readonly ConcurrentDictionary<string, Lazy<ImageHandle>> _imageCache = new();

        // 加载统计
        private static int _totalRequested = 0;
        private static int _totalLoaded = 0;
        private static long _totalMemoryUsed = 0;

        // 加载回调
        public static event Action<string, bool> OnImageLoadAttempt;
        public static event Action<string, long> OnImageLoaded;

        #region 核心加载方法

        /// <summary>
        /// 获取图片句柄（懒加载）
        /// </summary>
        public static ImageHandle GetImage(string imageName)
        {
            Interlocked.Increment(ref _totalRequested);

            var lazy = _imageCache.GetOrAdd(imageName, key =>
                new Lazy<ImageHandle>(
                    () => LoadImageCore(key),
                    LazyThreadSafetyMode.ExecutionAndPublication
                )
            );

            // 访问 Value 属性时才真正加载
            return lazy.Value;
        }

        /// <summary>
        /// 尝试获取图片句柄（不触发加载）
        /// </summary>
        public static bool TryGetLoadedImage(string imageName, out ImageHandle handle)
        {
            if (_imageCache.TryGetValue(imageName, out var lazy) && lazy.IsValueCreated)
            {
                handle = lazy.Value;
                return true;
            }

            handle = ImageHandle.Invalid;
            return false;
        }

        /// <summary>
        /// 核心加载逻辑
        /// </summary>
        private static ImageHandle LoadImageCore(string imageName)
        {
            var startTime = DateTime.Now;
            OnImageLoadAttempt?.Invoke(imageName, true);

            try
            {
                Console.WriteLine($"[LazyLoad] 开始加载图片: {imageName}");

                // 从嵌入资源加载
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = $"Dota2Simulator.Picture_Dota2.{imageName}.bmp";

                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    TTS.TTS.Speak($"[LazyLoad] 图片不存在: {imageName}");
                    Thread.Sleep(3000);
                    OnImageLoadAttempt?.Invoke(imageName, false);
                    return ImageHandle.Invalid;
                }

                using Bitmap bitmap = new(stream);
                byte[] imageData = ImageManager.GetBitmapData(bitmap);

                // 估算内存使用
                long memoryUsage = imageData.Length;
                Interlocked.Add(ref _totalMemoryUsed, memoryUsage);
                Interlocked.Increment(ref _totalLoaded);

                // 创建图片句柄
                ImageHandle handle = ImageManager.CreateStaticImage(imageData, bitmap.Size, imageName);

                var loadTime = (DateTime.Now - startTime).TotalMilliseconds;
                Console.WriteLine($"[LazyLoad] 成功加载: {imageName} (耗时: {loadTime:F2}ms, 大小: {memoryUsage / 1024.0:F2}KB)");

                OnImageLoaded?.Invoke(imageName, memoryUsage);
                return handle;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LazyLoad] 加载失败: {imageName}, 错误: {ex.Message}");
                OnImageLoadAttempt?.Invoke(imageName, false);
                return ImageHandle.Invalid;
            }
        }

        #endregion

        #region 批量操作

        /// <summary>
        /// 预加载多个图片
        /// </summary>
        public static void PreloadImages(params string[] imageNames)
        {
            Console.WriteLine($"[LazyLoad] 预加载 {imageNames.Length} 个图片");

            foreach (var name in imageNames)
            {
                _ = GetImage(name);
            }
        }

        /// <summary>
        /// 异步预加载
        /// </summary>
        public static async Task PreloadImagesAsync(params string[] imageNames)
        {
            var tasks = imageNames.Select(name =>
                Task.Run(() => GetImage(name))
            ).ToArray();

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 按优先级预加载
        /// </summary>
        public static void PreloadByPriority(Dictionary<string, int> imagePriorities, int maxConcurrent = 3)
        {
            var sortedImages = imagePriorities
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key);

            var semaphore = new SemaphoreSlim(maxConcurrent);
            var tasks = new List<Task>();

            foreach (var imageName in sortedImages)
            {
                var task = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        GetImage(imageName);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        #endregion

        #region 缓存管理

        /// <summary>
        /// 检查图片是否已加载
        /// </summary>
        public static bool IsImageLoaded(string imageName)
        {
            return _imageCache.TryGetValue(imageName, out var lazy) && lazy.IsValueCreated;
        }

        /// <summary>
        /// 获取所有已加载的图片名称
        /// </summary>
        public static List<string> GetLoadedImageNames()
        {
            return _imageCache
                .Where(kvp => kvp.Value.IsValueCreated)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        /// <summary>
        /// 清除指定图片缓存
        /// </summary>
        public static bool RemoveImage(string imageName)
        {
            if (_imageCache.TryRemove(imageName, out var lazy))
            {
                if (lazy.IsValueCreated)
                {
                    Interlocked.Decrement(ref _totalLoaded);
                    // 注意：这里无法准确追踪内存释放，因为GC是异步的
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void ClearAllCache()
        {
            var count = _imageCache.Count(kvp => kvp.Value.IsValueCreated);
            _imageCache.Clear();

            _totalRequested = 0;
            _totalLoaded = 0;
            _totalMemoryUsed = 0;

            Console.WriteLine($"[LazyLoad] 清除了 {count} 个已加载的图片");

            // 建议垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <summary>
        /// 清除未使用的缓存（保留最近使用的N个）
        /// </summary>
        public static void TrimCache(int keepCount)
        {
            var loadedImages = _imageCache
                .Where(kvp => kvp.Value.IsValueCreated)
                .Select(kvp => kvp.Key)
                .Skip(keepCount)
                .ToList();

            foreach (var imageName in loadedImages)
            {
                RemoveImage(imageName);
            }

            Console.WriteLine($"[LazyLoad] 清理缓存，保留 {keepCount} 个图片");
        }

        #endregion

        #region 统计信息

        /// <summary>
        /// 获取加载统计信息
        /// </summary>
        public static LoadStatistics GetStatistics()
        {
            var stats = new LoadStatistics
            {
                TotalRequested = _totalRequested,
                TotalLoaded = _totalLoaded,
                CurrentCached = _imageCache.Count(kvp => kvp.Value.IsValueCreated),
                TotalCacheEntries = _imageCache.Count,
                EstimatedMemoryUsageMB = _totalMemoryUsed / (1024.0 * 1024.0),
                LoadedImages = GetLoadedImageNames()
            };

            return stats;
        }

        /// <summary>
        /// 获取详细报告
        /// </summary>
        public static string GetDetailedReport()
        {
            var stats = GetStatistics();
            var report = new System.Text.StringBuilder();

            report.AppendLine("=== LazyLoad 图片加载报告 ===");
            report.AppendLine($"总请求次数: {stats.TotalRequested}");
            report.AppendLine($"实际加载数: {stats.TotalLoaded}");
            report.AppendLine($"当前缓存数: {stats.CurrentCached}/{stats.TotalCacheEntries}");
            report.AppendLine($"预计内存占用: {stats.EstimatedMemoryUsageMB:F2} MB");
            report.AppendLine($"缓存命中率: {stats.CacheHitRate:P2}");
            report.AppendLine();

            if (stats.LoadedImages.Any())
            {
                report.AppendLine("已加载图片列表:");
                foreach (var image in stats.LoadedImages.OrderBy(n => n))
                {
                    report.AppendLine($"  - {image}");
                }
            }
            else
            {
                report.AppendLine("暂无已加载的图片");
            }

            return report.ToString();
        }

        #endregion

        #region 便捷访问器

#if false
        /// <summary>
        /// 提供类似原始代码的访问方式，但实现懒加载
        /// </summary>
        public static class Images
        {
            // 使用属性而不是字段，实现懒加载
            public static ImageHandle Buff_小精灵_幽魂 => GetImage("BUFF.小精灵_幽魂");
            public static ImageHandle Buff_钢毛后背 => GetImage("BUFF.钢毛后背");
            public static ImageHandle Skill_反击螺旋 => GetImage("Skill.反击螺旋");
            public static ImageHandle Skill_狂战士之吼 => GetImage("Skill.狂战士之吼");

            // 批量获取某类图片
            public static class Buffs
            {
                public static ImageHandle 小精灵幽魂 => GetImage("BUFF.小精灵_幽魂");
                public static ImageHandle 钢毛后背 => GetImage("BUFF.钢毛后背");
                public static ImageHandle 狂战士之血 => GetImage("BUFF.狂战士之血");
            }

            public static class Skills
            {
                public static ImageHandle 反击螺旋 => GetImage("Skill.反击螺旋");
                public static ImageHandle 狂战士之吼 => GetImage("Skill.狂战士之吼");
                public static ImageHandle 海妖外壳 => GetImage("Skill.海妖外壳");
            }
        } 
#endif

        #endregion
    }

    /// <summary>
    /// 加载统计信息
    /// </summary>
    public class LoadStatistics
    {
        public int TotalRequested { get; set; }
        public int TotalLoaded { get; set; }
        public int CurrentCached { get; set; }
        public int TotalCacheEntries { get; set; }
        public double EstimatedMemoryUsageMB { get; set; }
        public List<string> LoadedImages { get; set; }

        public double CacheHitRate => TotalRequested > 0
            ? (TotalRequested - TotalLoaded) / (double)TotalRequested
            : 0;
    }

#if false
    /// <summary>
    /// 使用示例
    /// </summary>
    public static class LazyLoadExample
    {
        public static void DemoUsage()
        {
            // 设置加载事件监听
            LazyImageLoader.OnImageLoadAttempt += (name, success) =>
            {
                Console.WriteLine($"[Event] 尝试加载 {name}: {(success ? "开始" : "失败")}");
            };

            LazyImageLoader.OnImageLoaded += (name, size) =>
            {
                Console.WriteLine($"[Event] 已加载 {name}, 大小: {size / 1024.0:F2}KB");
            };

            // 1. 基本使用
            Console.WriteLine("=== 基本使用示例 ===");
            var handle1 = LazyImageLoader.Images.Buff_小精灵_幽魂; // 第一次访问，触发加载
            var handle2 = LazyImageLoader.Images.Buff_小精灵_幽魂; // 第二次访问，使用缓存

            // 2. 批量预加载
            Console.WriteLine("\n=== 批量预加载 ===");
            LazyImageLoader.PreloadImages(
                "BUFF.钢毛后背",
                "Skill.反击螺旋",
                "Skill.狂战士之吼"
            );

            // 3. 按优先级加载
            Console.WriteLine("\n=== 优先级加载 ===");
            var priorities = new Dictionary<string, int>
            {
                { "Skill.海妖外壳", 100 },  // 最高优先级
                { "Skill.锚击", 50 },
                { "BUFF.狂战士之血", 10 }   // 最低优先级
            };
            LazyImageLoader.PreloadByPriority(priorities);

            // 4. 查看统计信息
            Console.WriteLine("\n=== 统计信息 ===");
            Console.WriteLine(LazyImageLoader.GetDetailedReport());

            // 5. 内存管理
            Console.WriteLine("\n=== 内存管理 ===");
            LazyImageLoader.TrimCache(3); // 只保留3个最近使用的图片
            Console.WriteLine(LazyImageLoader.GetDetailedReport());

            // 6. 条件加载
            Console.WriteLine("\n=== 条件加载 ===");
            string imageName = "BUFF.测试";
            if (!LazyImageLoader.IsImageLoaded(imageName))
            {
                Console.WriteLine($"{imageName} 未加载，现在加载...");
                var handle = LazyImageLoader.GetImage(imageName);
            }
        }

        // 在技能选择器中使用
        public static void UseInSkillSelector()
        {
            var selector = new FlexibleSkillSelector();

            // 注册懒加载图片
            selector.RegisterImageHandle("钢毛后背", LazyImageLoader.Images.Skills.钢毛后背);
            selector.RegisterImageHandle("反击螺旋", LazyImageLoader.Images.Skills.反击螺旋);

            // 图片只在 selector 真正查找时才会加载
        }
    } 
#endif
}