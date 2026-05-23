using Collections.Pooled;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Threading.Timer;
using Tuple = Dota2Simulator.Vision.ImageFinder.Tuple;

namespace Dota2Simulator.Vision
{
    /// <summary>
    /// 静态图像缓存 - 专门用于缓存特征图片
    /// </summary>
    public static unsafe class StaticImageCache
    {
        private static readonly ConcurrentDictionary<int, CachedImage> _cache = new();
        private static readonly Lock _cacheLock = new();
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 缓存的图像数据
        /// </summary>
        private sealed class CachedImage
        {
            public readonly int Width;
            public readonly int Height;
            public readonly byte[] Data;
            public readonly GCHandle Handle;
            public readonly uint* Pixels;
            public DateTime LastAccess;
            public int AccessCount;

            public CachedImage(int width, int height, byte[] data)
            {
                Width = width;
                Height = height;
                Data = data;
                Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                Pixels = (uint*)Handle.AddrOfPinnedObject();
                LastAccess = DateTime.Now;
                AccessCount = 0;
            }

            public void Dispose()
            {
                if (Handle.IsAllocated)
                    Handle.Free();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public uint GetPixel(int x, int y)
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return 0;

                Interlocked.Increment(ref AccessCount);
                LastAccess = DateTime.Now;
                return Pixels[y * Width + x];
            }
        }

        /// <summary>
        /// 添加静态图像到缓存
        /// </summary>
        public static void CacheImage(int imageId, in Size size, byte[] data)
        {
            if (data.Length != size.Width * size.Height * 4)
                throw new ArgumentException("数据大小与图像尺寸不匹配");

            lock (_cacheLock)
            {
                // 如果已存在，先释放旧的
                if (_cache.TryRemove(imageId, out var old))
                {
                    old.Dispose();
                    ImageProcessingSystem.Deallocate(old.Data.Length);
                }

                // 检查内存预算
                if (!ImageProcessingSystem.TryAllocate(data.Length))
                {
                    CleanupLeastUsed();
                    if (!ImageProcessingSystem.TryAllocate(data.Length))
                    {
                        throw new OutOfMemoryException("缓存内存不足");
                    }
                }

                var cached = new CachedImage(size.Width, size.Height, data);
                _cache[imageId] = cached;
                _logger.Info($"缓存静态图像 {imageId}: {size.Width}x{size.Height}");
            }
        }

        /// <summary>
        /// 获取缓存的像素 - 使用ref避免复制
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetPixel(int imageId, int x, int y)
        {
            if (_cache.TryGetValue(imageId, out var cached))
            {
                return cached.GetPixel(x, y);
            }
            return 0;
        }

        /// <summary>
        /// 获取缓存的图像数据指针
        /// </summary>
        public static (IntPtr ptr, int width, int height)? GetImagePointer(int imageId)
        {
            if (_cache.TryGetValue(imageId, out var cached))
            {
                cached.LastAccess = DateTime.Now;
                Interlocked.Increment(ref cached.AccessCount);
                return ((IntPtr)cached.Pixels, cached.Width, cached.Height);
            }
            return null;
        }

        /// <summary>
        /// 清理最少使用的缓存
        /// </summary>
        private static void CleanupLeastUsed()
        {
            var now = DateTime.Now;
            var toRemove = _cache
                .Where(kvp => (now - kvp.Value.LastAccess).TotalMinutes > 10)
                .OrderBy(kvp => kvp.Value.AccessCount)
                .ThenBy(kvp => kvp.Value.LastAccess)
                .Take(Math.Max(1, _cache.Count / 4))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var id in toRemove)
            {
                if (_cache.TryRemove(id, out var cached))
                {
                    cached.Dispose();
                    ImageProcessingSystem.Deallocate(cached.Data.Length);
                    _logger.Info($"清理静态图像缓存: {id}");
                }
            }
        }

        public static void Clear()
        {
            lock (_cacheLock)
            {
                foreach (var cached in _cache.Values)
                {
                    cached.Dispose();
                    ImageProcessingSystem.Deallocate(cached.Data.Length);
                }
                _cache.Clear();
            }
        }

        public static int Count => _cache.Count;
    }
}
