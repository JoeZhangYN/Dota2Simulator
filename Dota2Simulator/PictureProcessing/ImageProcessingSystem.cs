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

namespace ImageProcessingSystem
{
    #region 三缓冲设计

    /* 优点         
     * 读取缓冲、写入缓冲、就绪缓冲三个独立缓冲区
     * 读写真正解耦,写入永不阻塞,可立即开始下一帧捕获
     * 使用SpinLock实现超低延迟的缓冲区交换
     * 缺点
     * 获取的是2帧前的数据
     * 额外8MB内存（对现代系统微不足道）
     * 略微增加的复杂度
     */
    /// <summary>
    ///     三缓冲设计   
    /// </summary>
    public sealed class TripleBufferSystem : IDisposable
    {
        private struct BufferInfo
        {
            public ImageHandle Handle { get; set; }
            public long Timestamp { get; set; }
            public int FrameId { get; set; }
        }

        /// <summary>
        ///     读buff
        /// </summary>
        private BufferInfo _readBuffer;
        /// <summary>
        ///     写buff
        /// </summary>
        private BufferInfo _writeBuffer;
        /// <summary>
        ///     三层缓冲buff
        /// </summary>
        private BufferInfo _readyBuffer;
        /// <summary>
        ///     锁
        /// </summary>
        private SpinLock _spinLock;
        /// <summary>
        ///     帧数计数
        ///     <para>理论上有上限,每帧增加会爆</para>
        ///     <para>按照1秒240hz,达到上限2147483647,需要103天</para>
        /// </summary>
        private int _frameCounter;

        /// <summary>
        ///     初始化三重缓冲
        /// </summary>
        /// <param name="width">宽度 默认1920</param>
        /// <param name="height">高度 默认1080</param>
        public TripleBufferSystem(int width = 1920, int height = 1080)
        {
            var size = new Size(width, height);
            var initialData = new byte[width * height * 4];

            _readBuffer = new BufferInfo { Handle = ImageManager.CreateDynamicImage(initialData, size, "ReadBuffer"), FrameId = -1 };
            _writeBuffer = new BufferInfo { Handle = ImageManager.CreateDynamicImage(initialData, size, "WriteBuffer"), FrameId = -1 };
            _readyBuffer = new BufferInfo { Handle = ImageManager.CreateDynamicImage(initialData, size, "ReadyBuffer"), FrameId = -1 };
        }

        public ImageHandle GetWriteBuffer() => _writeBuffer.Handle;

        /// <summary>
        ///     获取当前时间,暂无其他功能
        /// </summary>
        public void BeginCapture() => _writeBuffer.Timestamp = Stopwatch.GetTimestamp();

        /// <summary>
        ///     修改确认,
        /// </summary>
        public void CommitCapture()
        {
            _writeBuffer.FrameId = Interlocked.Increment(ref _frameCounter);
            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                (_writeBuffer, _readyBuffer) = (_readyBuffer, _writeBuffer);
            }
            finally
            {
                if (lockTaken) _spinLock.Exit();
            }
        }

        /// <summary>
        /// 获取读缓冲区句柄和新帧标志
        /// </summary>
        public (ImageHandle handle, bool isNewFrame) GetReadBuffer()
        {
            bool lockTaken = false;
            bool hasNewFrame = false;

            try
            {
                _spinLock.Enter(ref lockTaken);
                if (_readyBuffer.FrameId > _readBuffer.FrameId)
                {
                    (_readBuffer, _readyBuffer) = (_readyBuffer, _readBuffer);
                    hasNewFrame = true;
                }
            }
            finally
            {
                if (lockTaken) _spinLock.Exit();
            }

            return (_readBuffer.Handle, hasNewFrame);
        }

        /// <summary>
        /// 仅获取读缓冲区句柄（高性能版本）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImageHandle GetReadHandle()
        {
            bool lockTaken = false;

            try
            {
                _spinLock.Enter(ref lockTaken);
                // 如果有新帧，交换缓冲区
                if (_readyBuffer.FrameId > _readBuffer.FrameId)
                {
                    (_readBuffer, _readyBuffer) = (_readyBuffer, _readBuffer);
                }
                return _readBuffer.Handle;
            }
            finally
            {
                if (lockTaken) _spinLock.Exit();
            }
        }

        /// <summary>
        /// 仅获取当前读缓冲区句柄，不进行缓冲区交换（最高性能）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImageHandle GetCurrentReadHandle()
        {
            // 直接返回当前读缓冲区，不检查是否有新帧
            return _readBuffer.Handle;
        }

        /// <summary>
        /// 检查是否有新帧可用
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasNewFrame()
        {
            return _readyBuffer.FrameId > _readBuffer.FrameId;
        }

        public void Dispose()
        {
            // 代码错误,并未实现dispose方法
            //_readBuffer.Handle?.Dispose();
            //_writeBuffer.Handle?.Dispose();
            //_readyBuffer.Handle?.Dispose();
        }
    }
    #endregion

    #region 日志接口

    // 日志接口，用于依赖注入
    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }

    // 默认控制台日志实现
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message) => Console.WriteLine($"[INFO] {message}");
        public void LogWarning(string message) => Console.WriteLine($"[WARN] {message}");
        public void LogError(string message) => Console.WriteLine($"[ERROR] {message}");
    }

    #endregion

    #region 核心数据结构

    /// <summary>
    /// 图像类型枚举
    /// </summary>
    public enum ImageType : byte
    {
        Dynamic = 0,    // 动态图像（截图等会变化的）
        Static = 1      // 静态图像（特征图片等不变的）
    }

    /// <summary>
    /// 图像句柄
    /// </summary>
    public readonly struct ImageHandle : IEquatable<ImageHandle>
    {
        public int Id { get; }
        public Size Size { get; }
        public long Offset { get; }
        public ImageType Type { get; }

        internal ImageHandle(int id, in Size size, long offset, ImageType type)
        {
            Id = id;
            Size = size;
            Offset = offset;
            Type = type;
        }

        public bool IsValid => Id > 0;
        public bool IsStatic => Type == ImageType.Static;
        public bool IsDynamic => Type == ImageType.Dynamic;

        public static ImageHandle Invalid => new ImageHandle(0, Size.Empty, 0, ImageType.Dynamic);

        public bool Equals(ImageHandle other) => Id == other.Id;
        public override bool Equals(object obj) => obj is ImageHandle other && Equals(other);
        public override int GetHashCode() => Id;
    }

    ///// <summary>
    ///// 内联数组缓冲区 - 用于减少内存碎片
    ///// </summary>
    //[InlineArray(1024 * 1024)]  // 1MB 内联数组
    //public struct InlineBuffer
    //{
    //    private byte _element0;
    //}

    // Rust 交互结构
    [StructLayout(LayoutKind.Sequential)]
    public struct Tuple
    {
        public uint x;
        public uint y;
    }

    #endregion

    #region 内存管理

    // 内存预算管理器
    public static class ImageProcessingSystem
    {
        private static long _totalBudget = 512 * 1024 * 1024; // 512mb 默认预算 1920*1080*4 = 6.4mb
        private static long _currentUsage;
        private static ILogger _logger = new ConsoleLogger();

        public static long TotalBudget
        {
            get => _totalBudget;
            set => Interlocked.Exchange(ref _totalBudget, value);
        }

        public static long CurrentUsage => Interlocked.Read(ref _currentUsage);
        public static long AvailableMemory => _totalBudget - CurrentUsage;
        public static double UsagePercentage => (double)CurrentUsage / _totalBudget * 100;

        public static void SetLogger(ILogger logger)
        {
            _logger = logger ?? new ConsoleLogger();
        }

        public static bool TryAllocate(long size)
        {
            long currentUsage, newUsage;
            do
            {
                currentUsage = _currentUsage;
                newUsage = currentUsage + size;
                if (newUsage > _totalBudget)
                    return false;
            } while (Interlocked.CompareExchange(ref _currentUsage, newUsage, currentUsage) != currentUsage);

            return true;
        }

        public static void Deallocate(long size)
        {
            Interlocked.Add(ref _currentUsage, -size);
        }

        public static void ReportUsage()
        {
            _logger.LogInfo($"内存使用: {CurrentUsage / (1024 * 1024):F2}MB / {TotalBudget / (1024 * 1024):F2}MB ({UsagePercentage:F1}%)");
        }
    }

    #endregion

    #region 缓存系统

    #region 静态图像缓存系统

    /// <summary>
    /// 静态图像缓存 - 专门用于缓存特征图片
    /// </summary>
    public static unsafe class StaticImageCache
    {
        private static readonly ConcurrentDictionary<int, CachedImage> _cache = new();
        private static readonly Lock _cacheLock = new();
        private static ILogger _logger = new ConsoleLogger();

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

        public static void SetLogger(ILogger logger)
        {
            _logger = logger ?? new ConsoleLogger();
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
                _logger.LogInfo($"缓存静态图像 {imageId}: {size.Width}x{size.Height}");
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
                    _logger.LogInfo($"清理静态图像缓存: {id}");
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

    #endregion

    #region 动态图像缓冲区

    /// <summary>
    /// 动态图像缓冲区 - 用于截图等会变化的图像
    /// </summary>
    public static unsafe class DynamicImageBuffer
    {
        private static byte* _buffer;
        private static long _bufferSize;
        private static long _currentOffset;
        private static GCHandle _handle;
        private static bool _initialized;
        private static readonly Lock _allocLock = new();
        private static ILogger _logger = new ConsoleLogger();

        // 使用分段内存池减少碎片
        private const int SEGMENT_SIZE = 64 * 1024 * 1024; // 64MB per segment
        private static readonly List<(long offset, long size, bool inUse)> _segments = new();

        public static bool IsInitialized => _initialized;

        public static void Initialize(long initialSize = 64 * 1024 * 1024)
        {
            if (_initialized)
                Cleanup();

            _bufferSize = initialSize;
            _currentOffset = 0;

            if (!ImageProcessingSystem.TryAllocate(_bufferSize))
            {
                throw new OutOfMemoryException($"无法分配动态缓冲区: {_bufferSize / (1024 * 1024)}MB");
            }

            var managedArray = new byte[_bufferSize];
            _handle = GCHandle.Alloc(managedArray, GCHandleType.Pinned);
            _buffer = (byte*)_handle.AddrOfPinnedObject();
            _initialized = true;

            _logger.LogInfo($"动态缓冲区初始化: {_bufferSize / (1024 * 1024)}MB");
        }

        /// <summary>
        /// 分配空间 - 使用内存池管理
        /// </summary>
        public static long AllocateSpace(long size)
        {
            lock (_allocLock)
            {
                // 查找可重用的段
                for (int i = 0; i < _segments.Count; i++)
                {
                    var segment = _segments[i];
                    if (!segment.inUse && segment.size >= size)
                    {
                        _segments[i] = (segment.offset, segment.size, true);
                        return segment.offset;
                    }
                }

                // 分配新空间
                if (_currentOffset + size > _bufferSize)
                {
                    ExpandBuffer(size);
                }

                long offset = _currentOffset;
                _segments.Add((offset, size, true));
                _currentOffset += size;
                return offset;
            }
        }

        /// <summary>
        /// 释放空间
        /// </summary>
        public static void ReleaseSpace(long offset)
        {
            lock (_allocLock)
            {
                for (int i = 0; i < _segments.Count; i++)
                {
                    var segment = _segments[i];
                    if (segment.offset == offset)
                    {
                        _segments[i] = (segment.offset, segment.size, false);
                        break;
                    }
                }
            }
        }

        private static void ExpandBuffer(long additionalSize)
        {
            long newSize = _bufferSize + Math.Max(additionalSize, SEGMENT_SIZE);
            _logger.LogInfo($"扩容动态缓冲区: {_bufferSize / (1024 * 1024)}MB -> {newSize / (1024 * 1024)}MB");

            var newArray = new byte[newSize];
            var newHandle = GCHandle.Alloc(newArray, GCHandleType.Pinned);
            var newBuffer = (byte*)newHandle.AddrOfPinnedObject();

            Buffer.MemoryCopy(_buffer, newBuffer, newSize, _currentOffset);

            _handle.Free();
            ImageProcessingSystem.Deallocate(_bufferSize);

            _handle = newHandle;
            _buffer = newBuffer;
            _bufferSize = newSize;

            ImageProcessingSystem.TryAllocate(newSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateData(long offset, byte[] data)
        {
            fixed (byte* src = data)
            {
                Buffer.MemoryCopy(src, _buffer + offset, data.Length, data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetPixel(long offset, int x, int y, int width)
        {
            long pixelOffset = offset + ((y * width + x) * 4);
            return *(uint*)(_buffer + pixelOffset);
        }

        public static IntPtr GetPointer(long offset) => (IntPtr)(_buffer + offset);

        public static void Cleanup()
        {
            if (_initialized)
            {
                _handle.Free();
                ImageProcessingSystem.Deallocate(_bufferSize);
                _segments.Clear();
                _currentOffset = 0;
                _initialized = false;
            }
        }
    }

    #endregion

    #region 像素缓存,已知关键点像素缓存
    /// <summary>
    /// 像素代码缓存 - 用于缓存特定位置的颜色信息
    /// </summary>
    public static class PixelCodeCache
    {
        private static readonly ConcurrentDictionary<string, PixelInfo> _pixelCache = new();
        private const int MAX_CACHE_SIZE = 100000;

        public struct PixelInfo
        {
            public Point Position { get; set; }
            public uint ColorCode { get; set; }
            public string Description { get; set; }
            public DateTime CachedTime { get; set; }
        }

        public static int CacheSize => _pixelCache.Count;

        /// <summary>
        /// 缓存特定位置的颜色信息
        /// </summary>
        public static void CachePixel(string key, Point position, uint colorCode, string description = "")
        {
            if (_pixelCache.Count > MAX_CACHE_SIZE)
            {
                ClearOldEntries();
            }

            _pixelCache[key] = new PixelInfo
            {
                Position = position,
                ColorCode = colorCode,
                Description = description,
                CachedTime = DateTime.Now
            };
        }

        /// <summary>
        /// 获取缓存的像素信息
        /// </summary>
        public static PixelInfo? GetPixelInfo(string key)
        {
            return _pixelCache.TryGetValue(key, out var info) ? info : null;
        }

        /// <summary>
        /// 验证缓存的像素是否仍然匹配
        /// </summary>
        public static bool ValidatePixel(string key, in ImageHandle handle, byte tolerance = 0)
        {
            if (!_pixelCache.TryGetValue(key, out var info))
                return false;

            uint currentColor = ImageManager.GetPixel(handle, info.Position.X, info.Position.Y);
            return PixelCodeCache.ComparePixelCodes(currentColor, info.ColorCode, tolerance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ColorToCode(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color CodeToColor(uint code)
        {
            return Color.FromArgb(
                (int)((code >> 24) & 0xFF),
                (int)((code >> 16) & 0xFF),
                (int)((code >> 8) & 0xFF),
                (int)(code & 0xFF)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ComparePixelCodes(uint code1, uint code2, byte tolerance)
        {
            if (tolerance == 0)
                return code1 == code2;

            byte r1 = (byte)((code1 >> 16) & 0xFF);
            byte g1 = (byte)((code1 >> 8) & 0xFF);
            byte b1 = (byte)(code1 & 0xFF);

            byte r2 = (byte)((code2 >> 16) & 0xFF);
            byte g2 = (byte)((code2 >> 8) & 0xFF);
            byte b2 = (byte)(code2 & 0xFF);

            return Math.Abs(r1 - r2) <= tolerance &&
                   Math.Abs(g1 - g2) <= tolerance &&
                   Math.Abs(b1 - b2) <= tolerance;
        }

        public static void ClearOldEntries()
        {
            var now = DateTime.Now;
            var toRemove = _pixelCache
                .Where(kvp => (now - kvp.Value.CachedTime).TotalMinutes > 30)
                .Select(kvp => kvp.Key)
                .Take(_pixelCache.Count / 3)
                .ToList();

            foreach (var key in toRemove)
            {
                _pixelCache.TryRemove(key, out _);
            }
        }

        public static void Clear()
        {
            _pixelCache.Clear();
        }
    }
    #endregion

    #endregion

    #region 图像管理器

    /// <summary>
    /// 统一的图像管理器
    /// </summary>
    public static class ImageManager
    {
        #region 检测位置是否有效

        private static readonly Point 无效坐标 = new(245760, 143640);

        public static bool 是否无效位置(Point? 位置)
        {
            return 位置 == null ||
           位置 == 无效坐标 ||
           位置.Value.X <= 0 ||
           位置.Value.Y <= 0;
        }

        #endregion

        private static readonly ConcurrentDictionary<int, ImageMetadata> _images = new();
        private static int _nextId = 1;
        private static ILogger _logger = new ConsoleLogger();

        private struct ImageMetadata
        {
            public Size Size;
            public long Offset;
            public ImageType Type;
            public DateTime CreatedTime;
            public DateTime LastAccessTime;
            public string Name;
            public bool IsPersistent;
            public bool UseTripleBuffer;
            public TripleBufferSystem TripleBuffer;
        }


        /// <summary>
        ///     创建动态图像（截图等）
        /// </summary>
        /// <param name="data">具体的数据</param>
        /// <param name="size">数据对应图片的大小</param>
        /// <param name="name">主要就是看看..</param>
        /// <returns>实例化结构</returns>
        /// <exception cref="ArgumentException"></exception>
        public static ImageHandle CreateDynamicImage(byte[] data, in Size size, string? name = null)
        {
            if (data.Length != size.Width * size.Height * 4)
                throw new ArgumentException("数据大小与图像尺寸不匹配");

            // 单例递增
            int id = Interlocked.Increment(ref _nextId);
            // 初始化检测
            if (!DynamicImageBuffer.IsInitialized) DynamicImageBuffer.Initialize();
            long offset = DynamicImageBuffer.AllocateSpace(data.Length);

            DynamicImageBuffer.UpdateData(offset, data);

            var metadata = new ImageMetadata
            {
                Size = size,
                Offset = offset,
                Type = ImageType.Dynamic,
                CreatedTime = DateTime.Now,
                LastAccessTime = DateTime.Now,
                Name = name ?? $"Dynamic_{id}",
                IsPersistent = false
            };

            _images[id] = metadata;
            return new ImageHandle(id, in size, offset, ImageType.Dynamic);
        }

        /// <summary>
        ///     创建静态图像（特征图片）
        /// </summary>
        /// <param name="data">具体的数据</param>
        /// <param name="size">数据对应图片的大小</param>
        /// <param name="name">主要就是看看..</param>
        /// <returns>实例化结构</returns>
        /// <exception cref="ArgumentException"></exception>
        public static ImageHandle CreateStaticImage(byte[] data, in Size size, string? name = null)
        {
            if (data.Length != size.Width * size.Height * 4)
                throw new ArgumentException("数据大小与图像尺寸不匹配");

            int id = Interlocked.Increment(ref _nextId);
            // 静态图像直接缓存
            StaticImageCache.CacheImage(id, in size, data);

            var metadata = new ImageMetadata
            {
                Size = size,
                Offset = 0, // 静态图像不使用offset
                Type = ImageType.Static,
                CreatedTime = DateTime.Now,
                LastAccessTime = DateTime.Now,
                Name = name ?? $"Static_{id}",
                IsPersistent = true
            };

            _images[id] = metadata;
            return new ImageHandle(id, in size, 0, ImageType.Static);
        }

        /// <summary>
        /// 从文件创建静态图像
        /// </summary>
        public static ImageHandle CreateStaticImageFromFile(string filePath, string? name = null)
        {
            using (var bitmap = new Bitmap(filePath))
            {
                var data = GetBitmapData(bitmap);
                return CreateStaticImage(data, bitmap.Size, name ?? Path.GetFileNameWithoutExtension(filePath));
            }
        }

        /// <summary>
        /// 获取像素 - 根据图像类型自动选择源
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetPixel(in ImageHandle handle, int x, int y)
        {
            if (!_images.TryGetValue(handle.Id, out var metadata))
                return 0;

            if (x < 0 || x >= metadata.Size.Width || y < 0 || y >= metadata.Size.Height)
                return 0;

            metadata.LastAccessTime = DateTime.Now;
            _images[handle.Id] = metadata;

            // 如果使用三重缓冲
            if (metadata.UseTripleBuffer && metadata.TripleBuffer != null)
            {
                var (readHandle, _) = metadata.TripleBuffer.GetReadBuffer();
                if (readHandle.IsValid)
                {
                    // 从三重缓冲的读缓冲区获取像素
                    var (ptr, _, size) = GetImageData(readHandle);
                    unsafe
                    {
                        uint* pixels = (uint*)ptr;
                        return pixels[y * (int)size.x + x];
                    }
                }
                return 0;
            }

            // 原有逻辑
            return handle.Type switch
            {
                ImageType.Static => StaticImageCache.GetPixel(handle.Id, x, y),
                ImageType.Dynamic => DynamicImageBuffer.GetPixel(metadata.Offset, x, y, metadata.Size.Width),
                _ => 0
            };
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetColor(in ImageHandle handle, int x, int y)
        {
            uint pixel = GetPixel(in handle, x, y);
            return Color.FromArgb(
                (int)((pixel >> 24) & 0xFF),
                (int)((pixel >> 16) & 0xFF),
                (int)((pixel >> 8) & 0xFF),
                (int)(pixel & 0xFF)
            );
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetColor(in ImageHandle handle, in Point p)
        {
            uint pixel = GetPixel(in handle, p.X, p.Y);
            return Color.FromArgb(
                (int)((pixel >> 24) & 0xFF),
                (int)((pixel >> 16) & 0xFF),
                (int)((pixel >> 8) & 0xFF),
                (int)(pixel & 0xFF)
            );
        }

        /// <summary>
        /// 获取图像数据指针
        /// </summary>
        /// <summary>
        /// 获取图像数据指针（用于与Rust交互）- 增强版
        /// </summary>
        public static unsafe (IntPtr ptr, int length, Tuple size) GetImageData(in ImageHandle handle)
        {
            if (!_images.TryGetValue(handle.Id, out var metadata))
                throw new ArgumentException("无效的图像句柄");

            IntPtr ptr;
            int length = metadata.Size.Width * metadata.Size.Height * 4;
            var size = new Tuple { x = (uint)metadata.Size.Width, y = (uint)metadata.Size.Height };

            // 如果使用三重缓冲
            if (metadata.UseTripleBuffer && metadata.TripleBuffer != null)
            {
                var (readHandle, _) = metadata.TripleBuffer.GetReadBuffer();
                if (readHandle.IsValid)
                {
                    // 递归调用获取实际的缓冲区数据
                    return GetImageData(readHandle);
                }
                throw new InvalidOperationException("三重缓冲读取失败");
            }

            // 原有逻辑
            if (handle.Type == ImageType.Static)
            {
                var cached = StaticImageCache.GetImagePointer(handle.Id);
                if (!cached.HasValue)
                    throw new InvalidOperationException("静态图像缓存丢失");

                ptr = cached.Value.ptr;
            }
            else
            {
                ptr = DynamicImageBuffer.GetPointer(metadata.Offset);
            }

            // 验证指针有效性
            if (ptr == IntPtr.Zero)
                throw new InvalidOperationException($"{handle.Type}图像指针无效");

            metadata.LastAccessTime = DateTime.Now;
            _images[handle.Id] = metadata;

            return (ptr, length, size);
        }

        /// <summary>
        /// 释放图像
        /// </summary>
        public static void ReleaseImage(in ImageHandle handle)
        {
            if (_images.TryRemove(handle.Id, out var metadata))
            {
                if (metadata.UseTripleBuffer)
                {
                    // 三重缓冲由外部管理，这里只移除引用
                    _logger.LogInfo($"释放三重缓冲图像引用: {metadata.Name}");
                }
                else if (handle.Type == ImageType.Dynamic)
                {
                    DynamicImageBuffer.ReleaseSpace(metadata.Offset);
                }

                _logger.LogInfo($"释放图像: {metadata.Name}");
            }
        }

        /// <summary>
        /// 批量获取像素 - 使用 Span 避免数组分配
        /// </summary>
        public static void GetPixelRegion(in ImageHandle handle, Rectangle region, Span<uint> pixels)
        {
            if (!_images.TryGetValue(handle.Id, out var metadata))
                return;

            int index = 0;
            for (int y = region.Y; y < region.Bottom && y < metadata.Size.Height; y++)
            {
                for (int x = region.X; x < region.Right && x < metadata.Size.Width; x++)
                {
                    if (index < pixels.Length)
                    {
                        pixels[index++] = GetPixel(in handle, x, y);
                    }
                }
            }
        }

        /// <summary>
        /// 从Bitmap获取数据
        /// </summary>
        public static unsafe byte[] GetBitmapData(Bitmap bitmap)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                var data = new byte[bitmap.Width * bitmap.Height * 4];
                var scan0 = (byte*)bitmapData.Scan0;

                // 使用 SIMD 优化复制
                if (bitmapData.Stride == bitmap.Width * 4)
                {
                    // 连续内存，直接复制
                    Buffer.MemoryCopy(scan0, (byte*)Unsafe.AsPointer(ref data[0]),
                        data.Length, data.Length);
                }
                else
                {
                    // 有 padding，逐行复制
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        Buffer.MemoryCopy(scan0 + y * bitmapData.Stride,
                            (byte*)Unsafe.AsPointer(ref data[y * bitmap.Width * 4]),
                            bitmap.Width * 4, bitmap.Width * 4);
                    }
                }

                return data;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        /// <summary>
        /// 优化的保存图像方法
        /// </summary>
        public static unsafe bool SaveImage(in ImageHandle handle, string filePath, Rectangle? region = null)
        {
            if (!_images.TryGetValue(handle.Id, out var metadata))
                return false;

            try
            {
                // 直接使用 GetImageData 方法获取所有需要的信息
                var (ptr, length, size) = GetImageData(in handle);

                // 确定保存区域
                Rectangle saveRegion;
                if (region.HasValue)
                {
                    // 验证并调整区域边界
                    var rect = region.Value;
                    int maxWidth = (int)size.x;
                    int maxHeight = (int)size.y;

                    saveRegion = new Rectangle(
                        Math.Max(0, Math.Min(rect.X, maxWidth)),
                        Math.Max(0, Math.Min(rect.Y, maxHeight)),
                        Math.Max(1, Math.Min(rect.Width, maxWidth - Math.Max(0, rect.X))),
                        Math.Max(1, Math.Min(rect.Height, maxHeight - Math.Max(0, rect.Y)))
                    );

                    // 如果调整后的区域无效，返回false
                    if (saveRegion.Width <= 0 || saveRegion.Height <= 0)
                    {
                        _logger.LogError($"无效的保存区域: {rect}，图像尺寸: {size.x}x{size.y}");
                        return false;
                    }
                }
                else
                {
                    // 保存整个图像
                    saveRegion = new Rectangle(0, 0, (int)size.x, (int)size.y);
                }

                using (var bitmap = new Bitmap(saveRegion.Width, saveRegion.Height, PixelFormat.Format32bppArgb))
                {
                    var rect = new Rectangle(0, 0, saveRegion.Width, saveRegion.Height);
                    var bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    try
                    {
                        byte* src = (byte*)ptr;
                        byte* dst = (byte*)bitmapData.Scan0;
                        int srcWidth = (int)size.x;
                        int dstWidth = saveRegion.Width;
                        int dstHeight = saveRegion.Height;
                        int bytesPerPixel = 4; // ARGB

                        // 计算源图像中区域起始位置
                        int srcStartX = saveRegion.X;
                        int srcStartY = saveRegion.Y;

                        // 使用并行复制提高大图像区域的保存速度
                        if (dstHeight > 100)
                        {
                            // 大区域使用并行复制
                            Parallel.For(0, dstHeight, y =>
                            {
                                byte* srcRow = src + ((srcStartY + y) * srcWidth + srcStartX) * bytesPerPixel;
                                byte* dstRow = dst + y * bitmapData.Stride;
                                Buffer.MemoryCopy(srcRow, dstRow, dstWidth * bytesPerPixel, dstWidth * bytesPerPixel);
                            });
                        }
                        else
                        {
                            // 小区域使用顺序复制
                            for (int y = 0; y < dstHeight; y++)
                            {
                                byte* srcRow = src + ((srcStartY + y) * srcWidth + srcStartX) * bytesPerPixel;
                                byte* dstRow = dst + y * bitmapData.Stride;
                                Buffer.MemoryCopy(srcRow, dstRow, dstWidth * bytesPerPixel, dstWidth * bytesPerPixel);
                            }
                        }
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    // 根据扩展名保存
                    var extension = Path.GetExtension(filePath).ToLower(System.Globalization.CultureInfo.CurrentCulture);
                    var format = extension switch
                    {
                        ".bmp" => ImageFormat.Bmp,
                        ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                        ".png" => ImageFormat.Png,
                        ".gif" => ImageFormat.Gif,
                        ".tiff" or ".tif" => ImageFormat.Tiff,
                        ".ico" => ImageFormat.Icon,
                        _ => ImageFormat.Bmp
                    };

                    // 对于JPEG格式，可以设置质量
                    if (format == ImageFormat.Jpeg)
                    {
                        var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                        if (jpegEncoder == null)
                        {
                            bitmap.Save(filePath, format);
                        }
                        else
                        {
                            var encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 90L); // 90% 质量
                            bitmap.Save(filePath, jpegEncoder, encoderParams);
                        }
                    }
                    else
                    {
                        bitmap.Save(filePath, format);
                    }
                }

                // 更新访问时间
                metadata.LastAccessTime = DateTime.Now;
                _images[handle.Id] = metadata;

                string regionInfo = region.HasValue ? $"区域: {saveRegion}" : "完整图像";
                _logger.LogInfo($"图像已保存: {filePath} (类型: {handle.Type}, {regionInfo}, 保存尺寸: {saveRegion.Width}x{saveRegion.Height})");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"保存图像失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取图像编码器
        /// </summary>
        private static ImageCodecInfo? GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        ///     高性能版本 - 使用不安全代码直接访问内存
        /// </summary>
        public static unsafe PooledList<Point> FindColors(
            PooledList<Color> colors,
            PooledList<Point> points,
            in ImageHandle handle,
            byte tolerance = 1)
        {
            if (colors.Count != points.Count)
                throw new ArgumentException("颜色数量与坐标点数量不匹配");

            var results = new PooledList<Point>();

            if (colors.Count == 0)
                return results;

            // 获取图像数据
            var (ptr, length, size) = ImageManager.GetImageData(in handle);
            int imageWidth = (int)size.x;
            int imageHeight = (int)size.y;
            uint* pixels = (uint*)ptr;

            // 计算搜索范围
            int minX = points.Min(p => p.X);
            int maxX = points.Max(p => p.X);
            int minY = points.Min(p => p.Y);
            int maxY = points.Max(p => p.Y);

            // 预计算颜色代码
            var colorCodes = stackalloc uint[colors.Count];
            for (int i = 0; i < colors.Count; i++)
            {
                colorCodes[i] = PixelCodeCache.ColorToCode(colors[i]);
            }

            // 复制点数组到栈上以提高访问速度
            var pointsArray = stackalloc Point[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                pointsArray[i] = points[i];
            }

            // 搜索
            for (int baseY = -minY; baseY < imageHeight - maxY; baseY++)
            {
                for (int baseX = -minX; baseX < imageWidth - maxX; baseX++)
                {
                    bool match = true;

                    // 使用指针算术直接访问像素
                    for (int i = 0; i < points.Count; i++)
                    {
                        int checkX = baseX + pointsArray[i].X;
                        int checkY = baseY + pointsArray[i].Y;

                        if (checkX < 0 || checkX >= imageWidth || checkY < 0 || checkY >= imageHeight)
                        {
                            match = false;
                            break;
                        }

                        uint pixelCode = pixels[checkY * imageWidth + checkX];

                        if (!PixelCodeCache.ComparePixelCodes(pixelCode, colorCodes[i], tolerance))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        results.Add(new Point(baseX, baseY));
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// 创建使用三重缓冲的动态图像
        /// </summary>
        public static ImageHandle CreateDynamicImageWithTripleBuffer(
            TripleBufferSystem tripleBuffer,
            in Size size,
            string? name = null)
        {
            int id = Interlocked.Increment(ref _nextId);

            var metadata = new ImageMetadata
            {
                Size = size,
                Offset = 0,  // 三重缓冲不使用offset
                Type = ImageType.Dynamic,
                CreatedTime = DateTime.Now,
                LastAccessTime = DateTime.Now,
                Name = name ?? $"TripleBuffer_{id}",
                IsPersistent = false,
                UseTripleBuffer = true,
                TripleBuffer = tripleBuffer
            };

            _images[id] = metadata;
            return new ImageHandle(id, in size, 0, ImageType.Dynamic);
        }
    }

    #endregion

    #region 全局截图管理器

    /// <summary>
    /// 全局截图管理器 - 直接使用三重缓冲系统
    /// </summary>
    public static class GlobalScreenCapture
    {
        private static TripleBufferSystem _tripleBuffer;
        private static int _coordinateOffsetX;
        private static int _coordinateOffsetY;
        private static readonly Lock _initLock = new();
        private static ILogger _logger = new ConsoleLogger();
        private static Size _captureSize;

        /// <summary>
        /// 获取坐标偏移
        /// </summary>
        public static (int x, int y) CoordinateOffset => (_coordinateOffsetX, _coordinateOffsetY);

        /// <summary>
        /// 获取捕获区域大小
        /// </summary>
        public static Size CaptureSize => _captureSize;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public static bool IsInitialized => _tripleBuffer != null;

        /// <summary>
        /// 初始化全局截图缓冲区
        /// </summary>
        public static void Initialize(int width = 1920, int height = 1080, int x = 0, int y = 0)
        {
            lock (_initLock)
            {
                if (_tripleBuffer != null)
                {
                    _logger.LogWarning("全局截图缓冲区已初始化");
                    return;
                }

                // 创建三重缓冲系统
                _tripleBuffer = new TripleBufferSystem(width, height);
                _captureSize = new Size(width, height);

                // 设置坐标偏移
                _coordinateOffsetX = x;
                _coordinateOffsetY = y;

                _logger.LogInfo($"全局截图缓冲区初始化完成: {width}x{height}, 偏移({x}, {y})");
            }
        }

        /// <summary>
        /// 执行屏幕捕捉
        /// </summary>
        public static void CaptureScreen(in Rectangle rectangle)
        {
            // 如果未初始化，自动初始化
            if (_tripleBuffer == null)
            {
                Initialize(rectangle.Width, rectangle.Height, rectangle.X, rectangle.Y);
            }

            // 检查尺寸是否匹配
            if (_captureSize.Width != rectangle.Width || _captureSize.Height != rectangle.Height)
            {
                _logger.LogWarning($"截图区域尺寸变化，重新初始化缓冲区");
                Cleanup();
                Initialize(rectangle.Width, rectangle.Height, rectangle.X, rectangle.Y);
            }

            // 开始捕获
            _tripleBuffer.BeginCapture();

            // 获取写缓冲区句柄并捕获屏幕
            var writeHandle = _tripleBuffer.GetWriteBuffer();
            bool success = ModifyGraphics.CaptureScreenToHandle(writeHandle, rectangle.X, rectangle.Y);

            if (success)
            {
                // 提交捕获，交换缓冲区
                _tripleBuffer.CommitCapture();
            }
            else
            {
                _logger.LogError("屏幕捕获失败");
            }
        }

        /// <summary>
        /// 异步捕获屏幕
        /// </summary>
        public static async Task CaptureScreenAsync(Rectangle rectangle)
        {
            await Task.Run(() => CaptureScreen(rectangle));
        }

        /// <summary>
        /// 获取当前可读的图像句柄（包含是否新帧信息）
        /// </summary>
        public static (ImageHandle handle, bool isNewFrame) GetCurrentFrame()
        {
            if (_tripleBuffer == null)
            {
                return (ImageHandle.Invalid, false);
            }
            return _tripleBuffer.GetReadBuffer();
        }

        /// <summary>
        /// 获取当前可读的图像句柄（高性能版本）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImageHandle GetCurrentHandle()
        {
            return _tripleBuffer?.GetReadHandle() ?? ImageHandle.Invalid;
        }

        /// <summary>
        /// 获取当前读缓冲区句柄，不更新缓冲区（最高性能）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImageHandle GetCurrentHandleNoSwap()
        {
            return _tripleBuffer?.GetCurrentReadHandle() ?? ImageHandle.Invalid;
        }

        /// <summary>
        /// 检查是否有新帧
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasNewFrame()
        {
            return _tripleBuffer?.HasNewFrame() ?? false;
        }

        /// <summary>
        /// 直接获取像素（包含坐标偏移处理）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetPixel(int screenX, int screenY)
        {
            if (_tripleBuffer == null)
                return 0;

            // 转换为缓冲区坐标
            int bufferX = screenX - _coordinateOffsetX;
            int bufferY = screenY - _coordinateOffsetY;

            // 边界检查
            if (bufferX < 0 || bufferX >= _captureSize.Width ||
                bufferY < 0 || bufferY >= _captureSize.Height)
                return 0;

            var (handle, _) = _tripleBuffer.GetReadBuffer();
            if (!handle.IsValid)
                return 0;

            return ImageManager.GetPixel(handle, bufferX, bufferY);
        }

        /// <summary>
        /// 获取颜色（包含坐标偏移处理）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetColor(int screenX, int screenY)
        {
            uint pixel = GetPixel(screenX, screenY);
            return Color.FromArgb(
                (int)((pixel >> 24) & 0xFF),
                (int)((pixel >> 16) & 0xFF),
                (int)((pixel >> 8) & 0xFF),
                (int)(pixel & 0xFF)
            );
        }

        /// <summary>
        /// 在全局截图中查找图像
        /// </summary>
        public static Point? FindImage(in ImageHandle targetImage, double matchRate = 0.9)
        {
            var (currentFrame, _) = GetCurrentFrame();
            if (!currentFrame.IsValid)
                return null;

            var result = ImageFinder.FindImage(targetImage, currentFrame, matchRate);

            // 如果找到，添加坐标偏移
            if (result.HasValue)
            {
                return new Point(
                    result.Value.X + _coordinateOffsetX,
                    result.Value.Y + _coordinateOffsetY
                );
            }

            return null;
        }

        /// <summary>
        /// 保存当前帧
        /// </summary>
        public static bool SaveCurrentFrame(string filePath)
        {
            var (handle, _) = GetCurrentFrame();
            if (!handle.IsValid)
                return false;

            return ImageManager.SaveImage(handle, filePath);
        }

        /// <summary>
        /// 在全局截图中查找所有匹配的图像（使用Rust高性能实现）
        /// </summary>
        public static List<Point> FindAllImages(
            in ImageHandle targetImage,
            double matchRate = 0.9,
            int maxResults = 100,
            int minDistance = 1,
            bool earlyExit = true)
        {
            var handle = GlobalScreenCapture.GetCurrentHandle();
            if (!handle.IsValid)
                return new List<Point>();

            var results = ImageFinder.FindAllImages(
                targetImage,
                handle,
                matchRate,
                maxResults,
                minDistance,
                earlyExit
            );

            // 添加坐标偏移
            var (offsetX, offsetY) = GlobalScreenCapture.CoordinateOffset;
            return results.Select(p => new Point(p.X + offsetX, p.Y + offsetY)).ToList();
        }

        /// <summary>
        /// 实时追踪多个目标（高性能版本）
        /// </summary>
        public static async IAsyncEnumerable<List<Point>> TrackMultipleTargets(
            ImageHandle targetImage,
            double matchRate = 0.9,
            int maxTargets = 10,
            int minDistance = 10,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var lastPositions = new List<Point>();
            var frameSkip = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                // 智能帧跳过：如果目标稳定，降低检测频率
                if (frameSkip > 0)
                {
                    frameSkip--;
                    await Task.Delay(5, cancellationToken);
                    continue;
                }

                var currentPositions = FindAllImages(
                    targetImage,
                    matchRate,
                    maxTargets,
                    minDistance,
                    true // 早期退出以提高性能
                );

                // 计算位置变化
                if (!PositionsChanged(lastPositions, currentPositions))
                {
                    // 目标稳定，增加帧跳过
                    frameSkip = 5;
                }
                else
                {
                    // 目标移动，重置帧跳过
                    frameSkip = 0;
                    lastPositions = currentPositions;
                    yield return currentPositions;
                }

                await Task.Delay(1, cancellationToken);
            }
        }

        private static bool PositionsChanged(List<Point> list1, List<Point> list2, int threshold = 2)
        {
            if (list1.Count != list2.Count)
                return true;

            // 按坐标排序以进行比较
            var sorted1 = list1.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            var sorted2 = list2.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();

            for (int i = 0; i < sorted1.Count; i++)
            {
                var dx = Math.Abs(sorted1[i].X - sorted2[i].X);
                var dy = Math.Abs(sorted1[i].Y - sorted2[i].Y);
                if (dx > threshold || dy > threshold)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public static void Cleanup()
        {
            _tripleBuffer?.Dispose();
            _tripleBuffer = null;
            _captureSize = Size.Empty;
            _logger.LogInfo("全局截图缓冲区已清理");
        }
    }

    #endregion

    #region 高级用法示例

    public static class AdvancedGlobalScreenUsage
    {
        /// <summary>
        /// 等待屏幕上出现指定图像
        /// </summary>
        public static async Task<Point?> WaitForImage(
            ImageHandle targetImage,
            int timeoutMs = 5000,
            double matchRate = 0.9)
        {
            var sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                var position = GlobalScreenCapture.FindImage(targetImage, matchRate);
                if (position.HasValue)
                    return position;

                await Task.Delay(50); // 每50ms检查一次
            }

            return null;
        }

        /// <summary>
        /// 监控屏幕区域颜色变化
        /// </summary>
        public static async Task MonitorColorChange(
            Point screenPoint,
            Action<Color, Color> onColorChanged,
            CancellationToken cancellationToken)
        {
            Color lastColor = Color.Empty;

            while (!cancellationToken.IsCancellationRequested)
            {
                var currentColor = GlobalScreenCapture.GetColor(screenPoint.X, screenPoint.Y);

                if (lastColor != Color.Empty && currentColor != lastColor)
                {
                    onColorChanged(lastColor, currentColor);
                }

                lastColor = currentColor;
                await Task.Delay(10);
            }
        }

        /// <summary>
        /// 实时追踪目标图像位置
        /// </summary>
        public static async IAsyncEnumerable<Point> TrackImage(
            ImageHandle targetImage,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Point? lastPosition = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                var position = GlobalScreenCapture.FindImage(targetImage, 0.9);

                if (position.HasValue && position != lastPosition)
                {
                    lastPosition = position;
                    yield return position.Value;
                }

                await Task.Delay(1);
            }
        }
    }

    #endregion

    #region 图像处理基类

    /// <summary>
    /// 图像处理器基类
    /// </summary>
    /// <summary>
    /// 图像处理器基类 - 使用 ref struct 避免复制
    /// </summary>
    public abstract class ImageProcessor
    {
        protected ImageHandle CurrentImage { get; private set; }
        protected ILogger Logger { get; set; }

        protected ImageProcessor(ILogger logger = null)
        {
            Logger = logger ?? new ConsoleLogger();
        }

        public virtual async Task<bool> ProcessAsync(ImageHandle image)
        {
            CurrentImage = image;

            try
            {
                Logger.LogInfo($"开始处理图像: {image.Id} ({(image.IsStatic ? "静态" : "动态")})");
                var result = await ProcessCore();
                Logger.LogInfo($"图像处理完成: {image.Id}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError($"图像处理失败: {ex.Message}");
                return false;
            }
        }

        protected abstract Task<bool> ProcessCore();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected uint GetPixel(int x, int y) => ImageManager.GetPixel(CurrentImage, x, y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Color GetColor(int x, int y) => ImageManager.GetColor(CurrentImage, x, y);

        /// <summary>
        /// 并行处理图像区域
        /// </summary>
        protected async Task ProcessParallel(Action<int, int> pixelProcessor)
        {
            var (_, _, size) = ImageManager.GetImageData(CurrentImage);
            int width = (int)size.x;
            int height = (int)size.y;

            await Task.Run(() =>
            {
                Parallel.For(0, height, y =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixelProcessor(x, y);
                    }
                });
            });
        }
    }

    #endregion

    #region 颜色对比
    /// <summary>
    ///     颜色对比
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        ///     比较颜色
        /// </summary>
        /// <param name="colorA"></param>
        /// <param name="colorB"></param>
        /// <param name="errorRange">默认不传的话容差为10</param>
        /// <returns></returns>
        public static bool ColorAEqualColorB(in Color colorA, in Color colorB, byte errorRange = 10)
        {
            return colorA.EqualsWithError(colorB, errorRange);
        }

        /// <summary>
        ///     比较颜色
        /// </summary>
        /// <param name="colorA"></param>
        /// <param name="colorB"></param>
        /// <param name="errorR">R容差</param>
        /// <param name="errorG">G容差</param>
        /// <param name="errorB">B容差</param>
        /// <returns></returns>
        public static bool ColorAEqualColorB(in Color colorA, in Color colorB, byte errorR, byte errorG, byte errorB)
        {
            return colorA.EqualsRGBWithError(in colorB, errorR, errorG, errorB);
        }

        /// <summary>
        /// 带误差范围的颜色相等比较（所有RGBA通道使用相同误差值）
        /// </summary>
        /// <param name="color">要比较的颜色</param>
        /// <param name="other">目标颜色</param>
        /// <param name="errorRange">允许的误差范围（默认5）</param>
        /// <returns>如果颜色在误差范围内相等则返回true</returns>
        private static bool EqualsWithError(this in Color color, in Color other, byte errorRange = 5)
        {
            // 使用SIMD指令实现RGBA值快速比较
            if (Sse2.IsSupported)
            {
                Vector128<int> vecA = Vector128.Create(
                    color.B,
                    color.G,
                    color.R,
                    color.A
                );
                Vector128<int> vecB = Vector128.Create(
                    other.B,
                    other.G,
                    other.R,
                    other.A
                );
                Vector128<int> errorVec = Vector128.Create((int)errorRange);

                // 计算差值
                Vector128<int> diff = Sse2.Subtract(vecA, vecB);
                Vector128<int> signMask = Sse2.ShiftRightArithmetic(diff, 31);  // 获取符号位
                Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);  // 绝对值计算

                // 比较差值是否超过误差范围
                Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

                // 检查所有分量是否都在误差范围内
                return Sse2.MoveMask(cmp.AsSingle()) == 0;
            }

            // 不支持SIMD时的回退方案
            return Math.Abs(color.R - other.R) <= errorRange &&
                   Math.Abs(color.G - other.G) <= errorRange &&
                   Math.Abs(color.B - other.B) <= errorRange &&
                   Math.Abs(color.A - other.A) <= errorRange;
        }

        /// <summary>
        /// 带误差范围的RGB颜色相等比较（各通道可以指定不同误差值）
        /// </summary>
        /// <param name="color">要比较的颜色</param>
        /// <param name="other">目标颜色</param>
        /// <param name="errorR">R通道允许的误差范围</param>
        /// <param name="errorG">G通道允许的误差范围</param>
        /// <param name="errorB">B通道允许的误差范围</param>
        /// <returns>如果颜色在误差范围内相等则返回true</returns>
        private static bool EqualsRGBWithError(this in Color color, in Color other,
            byte errorR = 5, byte errorG = 5, byte errorB = 5)
        {
            // 使用SIMD指令实现RGB值快速比较
            if (Sse2.IsSupported)
            {
                Vector128<int> vecA = Vector128.Create(
                    color.B,
                    color.G,
                    color.R,
                    0
                );
                Vector128<int> vecB = Vector128.Create(
                    other.B,
                    other.G,
                    other.R,
                    0
                );
                Vector128<int> errorVec = Vector128.Create(
                    errorB,
                    errorG,
                    errorR,
                    0
                );

                // 计算差值
                Vector128<int> diff = Sse2.Subtract(vecA, vecB);
                Vector128<int> signMask = Sse2.ShiftRightArithmetic(diff, 31);  // 获取符号位
                Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);  // 绝对值计算

                // 比较差值是否超过误差范围
                Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

                // 只检查RGB三个分量（忽略Alpha）
                return (Sse2.MoveMask(cmp.AsSingle()) & 0x7) == 0;
            }

            // 不支持SIMD时的回退方案
            return Math.Abs(color.R - other.R) <= errorR &&
                   Math.Abs(color.G - other.G) <= errorG &&
                   Math.Abs(color.B - other.B) <= errorB;
        }

        /// <summary>
        ///     基于uint指针的颜色比较（ARGB格式，统一误差范围）
        /// </summary>
        /// <param name="pixelA">指向第一个像素的指针</param>
        /// <param name="pixelB">指向第二个像素的指针</param>
        /// <param name="errorRange">误差范围（默认10）</param>
        /// <returns>如果颜色在误差范围内相等则返回true</returns>
        public static unsafe bool ComparePixels(uint* pixelA, uint* pixelB, byte errorRange = 10)
        {
            return ComparePixelValues(*pixelA, *pixelB, errorRange);
        }

        /// <summary>
        ///     基于uint值的颜色比较（ARGB格式，统一误差范围）
        /// </summary>
        /// <param name="pixelA">第一个像素值（ARGB格式）</param>
        /// <param name="pixelB">第二个像素值（ARGB格式）</param>
        /// <param name="errorRange">误差范围（默认10）</param>
        /// <returns>如果颜色在误差范围内相等则返回true</returns>
        public static bool ComparePixelValues(uint pixelA, uint pixelB, byte errorRange = 10)
        {
            // 使用SIMD指令实现ARGB值快速比较
            if (Sse2.IsSupported)
            {
                // 解析ARGB格式 (0xAARRGGBB)
                Vector128<int> vecA = Vector128.Create(
                    (int)(pixelA & 0xFF),        // B
                    (int)((pixelA >> 8) & 0xFF), // G
                    (int)((pixelA >> 16) & 0xFF),// R
                    (int)((pixelA >> 24) & 0xFF) // A
                );
                Vector128<int> vecB = Vector128.Create(
                    (int)(pixelB & 0xFF),        // B
                    (int)((pixelB >> 8) & 0xFF), // G
                    (int)((pixelB >> 16) & 0xFF),// R
                    (int)((pixelB >> 24) & 0xFF) // A
                );
                Vector128<int> errorVec = Vector128.Create((int)errorRange);

                // 计算差值
                Vector128<int> diff = Sse2.Subtract(vecA, vecB);
                Vector128<int> signMask = Sse2.ShiftRightArithmetic(diff, 31);
                Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);

                // 比较差值是否超过误差范围
                Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

                // 检查所有分量是否都在误差范围内
                return Sse2.MoveMask(cmp.AsSingle()) == 0;
            }

            // 不支持SIMD时的回退方案
            byte aR = (byte)((pixelA >> 16) & 0xFF);
            byte aG = (byte)((pixelA >> 8) & 0xFF);
            byte aB = (byte)(pixelA & 0xFF);
            byte aA = (byte)((pixelA >> 24) & 0xFF);

            byte bR = (byte)((pixelB >> 16) & 0xFF);
            byte bG = (byte)((pixelB >> 8) & 0xFF);
            byte bB = (byte)(pixelB & 0xFF);
            byte bA = (byte)((pixelB >> 24) & 0xFF);

            return Math.Abs(aR - bR) <= errorRange &&
                   Math.Abs(aG - bG) <= errorRange &&
                   Math.Abs(aB - bB) <= errorRange &&
                   Math.Abs(aA - bA) <= errorRange;
        }

        /// <summary>
        ///     基于uint指针的RGB颜色比较（各通道独立误差范围）
        /// </summary>
        /// <param name="pixelA">指向第一个像素的指针</param>
        /// <param name="pixelB">指向第二个像素的指针</param>
        /// <param name="errorR">R通道误差范围</param>
        /// <param name="errorG">G通道误差范围</param>
        /// <param name="errorB">B通道误差范围</param>
        /// <returns>如果RGB颜色在误差范围内相等则返回true</returns>
        public static unsafe bool ComparePixelsRGB(uint* pixelA, uint* pixelB,
            byte errorR, byte errorG, byte errorB)
        {
            return ComparePixelValuesRGB(*pixelA, *pixelB, errorR, errorG, errorB);
        }

        /// <summary>
        ///     基于uint值的RGB颜色比较（各通道独立误差范围）
        /// </summary>
        /// <param name="pixelA">第一个像素值（ARGB格式）</param>
        /// <param name="pixelB">第二个像素值（ARGB格式）</param>
        /// <param name="errorR">R通道误差范围</param>
        /// <param name="errorG">G通道误差范围</param>
        /// <param name="errorB">B通道误差范围</param>
        /// <returns>如果RGB颜色在误差范围内相等则返回true</returns>
        public static bool ComparePixelValuesRGB(uint pixelA, uint pixelB,
            byte errorR, byte errorG, byte errorB)
        {
            // 使用SIMD指令实现RGB值快速比较
            if (Sse2.IsSupported)
            {
                // 解析ARGB格式，忽略Alpha通道
                Vector128<int> vecA = Vector128.Create(
                    (int)(pixelA & 0xFF),        // B
                    (int)((pixelA >> 8) & 0xFF), // G
                    (int)((pixelA >> 16) & 0xFF),// R
                    0                            // 忽略A
                );
                Vector128<int> vecB = Vector128.Create(
                    (int)(pixelB & 0xFF),        // B
                    (int)((pixelB >> 8) & 0xFF), // G
                    (int)((pixelB >> 16) & 0xFF),// R
                    0                            // 忽略A
                );
                Vector128<int> errorVec = Vector128.Create(
                    errorB,
                    errorG,
                    errorR,
                    0
                );

                // 计算差值
                Vector128<int> diff = Sse2.Subtract(vecA, vecB);
                Vector128<int> signMask = Sse2.ShiftRightArithmetic(diff, 31);
                Vector128<int> absDiff = Sse2.Xor(Sse2.Add(diff, signMask), signMask);

                // 比较差值是否超过误差范围
                Vector128<int> cmp = Sse2.CompareGreaterThan(absDiff, errorVec);

                // 只检查RGB三个分量（忽略Alpha）
                return (Sse2.MoveMask(cmp.AsSingle()) & 0x7) == 0;
            }

            // 不支持SIMD时的回退方案
            byte aR = (byte)((pixelA >> 16) & 0xFF);
            byte aG = (byte)((pixelA >> 8) & 0xFF);
            byte aB = (byte)(pixelA & 0xFF);

            byte bR = (byte)((pixelB >> 16) & 0xFF);
            byte bG = (byte)((pixelB >> 8) & 0xFF);
            byte bB = (byte)(pixelB & 0xFF);

            return Math.Abs(aR - bR) <= errorR &&
                   Math.Abs(aG - bG) <= errorG &&
                   Math.Abs(aB - bB) <= errorB;
        }

        /// <summary>
        ///     批量像素比较（优化性能）
        /// </summary>
        /// <param name="pixelsA">第一组像素数据指针</param>
        /// <param name="pixelsB">第二组像素数据指针</param>
        /// <param name="count">要比较的像素数量</param>
        /// <param name="errorRange">误差范围</param>
        /// <returns>返回相等的像素数量</returns>
        public static unsafe int CompareBatchPixels(uint* pixelsA, uint* pixelsB,
            int count, byte errorRange = 10)
        {
            int equalCount = 0;

            for (int i = 0; i < count; i++)
            {
                if (ComparePixelValues(pixelsA[i], pixelsB[i], errorRange))
                {
                    equalCount++;
                }
            }

            return equalCount;
        }

        /// <summary>
        ///     从uint值获取各颜色分量
        /// </summary>
        /// <param name="pixel">像素值（ARGB格式）</param>
        /// <returns>返回(A, R, G, B)元组</returns>
        public static (byte A, byte R, byte G, byte B) GetColorComponents(uint pixel)
        {
            return (
                (byte)((pixel >> 24) & 0xFF), // A
                (byte)((pixel >> 16) & 0xFF), // R
                (byte)((pixel >> 8) & 0xFF),  // G
                (byte)(pixel & 0xFF)          // B
            );
        }

        /// <summary>
        ///     从各颜色分量构建uint像素值
        /// </summary>
        /// <param name="a">Alpha通道</param>
        /// <param name="r">Red通道</param>
        /// <param name="g">Green通道</param>
        /// <param name="b">Blue通道</param>
        /// <returns>ARGB格式的像素值</returns>
        public static uint CreatePixelValue(byte a, byte r, byte g, byte b)
        {
            return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
        }
    }

    #endregion

    #region 图像查找系统

    #region Rust交互识别图像
    // 图像查找器（与Rust交互）
    public static class ImageFinder
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // Rust DLL 导入
        [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Tuple FindBytesRust(
       IntPtr n1,
       UIntPtr len1,
       Tuple t1,
       IntPtr n2,
       UIntPtr len2,
       Tuple t2,
       double matchRate,
       byte ignore_r,
       byte ignore_g,
       byte ignore_b
   );

        [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Tuple FindBytesTolerance(
            IntPtr n1,
            UIntPtr len1,
            Tuple t1,
            IntPtr n2,
            UIntPtr len2,
            Tuple t2,
            double matchRate,
            byte ignore_r,
            byte ignore_g,
            byte ignore_b,
            byte tolerance
        );

        // 查找失败时的返回值
        private static readonly Tuple NotFound = new() { x = 245760, y = 143640 };

        [StructLayout(LayoutKind.Sequential)]
        public struct Region
        {
            public uint x;
            public uint y;
            public uint width;
            public uint height;
        }

        [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Tuple FindBytesInRegion(
            IntPtr n1, UIntPtr len1, Tuple t1,
            IntPtr n2, UIntPtr len2, Tuple t2,
            Region region,
            double matchRate,
            byte ignore_r, byte ignore_g, byte ignore_b
        );

        /// <summary>
        ///     指定区域大图小图对比
        /// </summary>
        /// <param name="subImage"></param>
        /// <param name="mainImage"></param>
        /// <param name="region"></param>
        /// <param name="matchRate"></param>
        /// <param name="ignoreColor"></param>
        /// <returns></returns>
        public static Point? FindImageInRegion(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            Rectangle region,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            // 调用新的 Rust 函数
            var regionStruct = new Region
            {
                x = (uint)region.X,
                y = (uint)region.Y,
                width = (uint)region.Width,
                height = (uint)region.Height
            };


            // 获取图像数据，增加异常处理
            IntPtr mainPtr, subPtr;
            int mainLen, subLen;
            Tuple mainSize, subSize;

            try
            {
                var mainData = ImageManager.GetImageData(in mainImage);
                mainPtr = mainData.ptr;
                mainLen = mainData.length;
                mainSize = mainData.size;

                var subData = ImageManager.GetImageData(in subImage);
                subPtr = subData.ptr;
                subLen = subData.length;
                subSize = subData.size;
            }
            catch (Exception ex)
            {
                _logger?.Error($"获取图像数据失败: {ex.Message}");
                return null;
            }

            // 处理忽略色
            byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
            if (ignoreColor.HasValue)
            {
                ignoreR = ignoreColor.Value.R;
                ignoreG = ignoreColor.Value.G;
                ignoreB = ignoreColor.Value.B;
            }

            var result = FindBytesInRegion(
                mainPtr, (UIntPtr)mainLen, mainSize,
                subPtr, (UIntPtr)subLen, subSize,
                regionStruct,
                matchRate,
                ignoreR, ignoreG, ignoreB
            );

            // 结果已经是相对于原始大图的坐标
            return result.x == NotFound.x ? null : new Point((int)result.x, (int)result.y);
        }

        /// <summary>
        ///     指定区域大图存在小图
        /// </summary>
        /// <param name="subImage"></param>
        /// <param name="mainImage"></param>
        /// <param name="region"></param>
        /// <param name="matchRate"></param>
        /// <param name="ignoreColor"></param>
        /// <returns></returns>
        public static bool FindImageInRegionBool(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            Rectangle region,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            // 调用新的 Rust 函数
            var regionStruct = new Region
            {
                x = (uint)region.X,
                y = (uint)region.Y,
                width = (uint)region.Width,
                height = (uint)region.Height
            };


            // 获取图像数据，增加异常处理
            IntPtr mainPtr, subPtr;
            int mainLen, subLen;
            Tuple mainSize, subSize;

            try
            {
                var mainData = ImageManager.GetImageData(in mainImage);
                mainPtr = mainData.ptr;
                mainLen = mainData.length;
                mainSize = mainData.size;

                var subData = ImageManager.GetImageData(in subImage);
                subPtr = subData.ptr;
                subLen = subData.length;
                subSize = subData.size;
            }
            catch (Exception ex)
            {
                _logger?.Error($"获取图像数据失败: {ex.Message}");
                return false;
            }

            // 处理忽略色
            byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
            if (ignoreColor.HasValue)
            {
                ignoreR = ignoreColor.Value.R;
                ignoreG = ignoreColor.Value.G;
                ignoreB = ignoreColor.Value.B;
            }

            var result = FindBytesInRegion(
                mainPtr, (UIntPtr)mainLen, mainSize,
                subPtr, (UIntPtr)subLen, subSize,
                regionStruct,
                matchRate,
                ignoreR, ignoreG, ignoreB
            );

            // 结果已经是相对于原始大图的坐标
            return result.x == NotFound.x ? false : true;
        }

        // 在大图中查找小图（精确匹配）
        /// <summary>
        /// 查找图像 - 增加安全检查
        /// </summary>
        public static unsafe Point? FindImage(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            try
            {
                // 验证句柄有效性
                if (!mainImage.IsValid || !subImage.IsValid)
                {
                    _logger?.Error("无效的图像句柄");
                    return null;
                }

                // 获取图像数据，增加异常处理
                IntPtr mainPtr, subPtr;
                int mainLen, subLen;
                Tuple mainSize, subSize;

                try
                {
                    var mainData = ImageManager.GetImageData(in mainImage);
                    mainPtr = mainData.ptr;
                    mainLen = mainData.length;
                    mainSize = mainData.size;

                    var subData = ImageManager.GetImageData(in subImage);
                    subPtr = subData.ptr;
                    subLen = subData.length;
                    subSize = subData.size;
                }
                catch (Exception ex)
                {
                    _logger? .Error($"获取图像数据失败: {ex.Message}");
                    return null;
                }

                // 验证指针和长度
                if (mainPtr == IntPtr.Zero || subPtr == IntPtr.Zero)
                {
                    _logger?.Error("图像数据指针为空");
                    return null;
                }

                if (mainLen <= 0 || subLen <= 0)
                {
                    _logger?.Error($"无效的图像数据长度: main={mainLen}, sub={subLen}");
                    return null;
                }

                // 验证图像尺寸
                if (mainSize.x == 0 || mainSize.y == 0 || subSize.x == 0 || subSize.y == 0)
                {
                    _logger?.Error("无效的图像尺寸");
                    return null;
                }

                // 确保子图像不大于主图像
                if (subSize.x > mainSize.x || subSize.y > mainSize.y)
                {
                    _logger?.Error($"子图像尺寸大于主图像\n\n子图像{subSize.x},{subSize.y}\r\n母图像{mainSize.x},{mainSize.y}");
                    return null;
                }

                // 验证数据长度与尺寸匹配
                int expectedMainLen = (int)(mainSize.x * mainSize.y * 4);
                int expectedSubLen = (int)(subSize.x * subSize.y * 4);

                if (mainLen != expectedMainLen || subLen != expectedSubLen)
                {
                    _logger?.Error($"数据长度与图像尺寸不匹配: " +
                        $"main expected={expectedMainLen}, actual={mainLen}, " +
                        $"sub expected={expectedSubLen}, actual={subLen}");
                    return null;
                }

                // 处理忽略色
                byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
                if (ignoreColor.HasValue)
                {
                    ignoreR = ignoreColor.Value.R;
                    ignoreG = ignoreColor.Value.G;
                    ignoreB = ignoreColor.Value.B;
                }

                // 确保内存对齐和有效性
                GC.KeepAlive(mainImage);
                GC.KeepAlive(subImage);

                // 调用 Rust 函数
                var result = FindBytesRust(
                    mainPtr,
                    (UIntPtr)mainLen,
                    mainSize,
                    subPtr,
                    (UIntPtr)subLen,
                    subSize,
                    matchRate,
                    ignoreR, ignoreG, ignoreB
                );

                // 检查是否找到
                if (result.x == NotFound.x && result.y == NotFound.y)
                    return null;

                // 验证结果在合理范围内
                if (result.x >= mainSize.x || result.y >= mainSize.y)
                {
                    _logger?.Error($"查找结果超出图像范围: ({result.x}, {result.y})");
                    return null;
                }

                return new Point((int)result.x, (int)result.y);
            }
            catch (Exception ex)
            {
                _logger?.Error($"图像查找失败: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        public static unsafe bool FindImageBool(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            try
            {
                // 验证句柄有效性
                if (!mainImage.IsValid || !subImage.IsValid)
                {
                    _logger?.Error("无效的图像句柄");
                    return false;
                }

                // 获取图像数据，增加异常处理
                IntPtr mainPtr, subPtr;
                int mainLen, subLen;
                Tuple mainSize, subSize;

                try
                {
                    var mainData = ImageManager.GetImageData(in mainImage);
                    mainPtr = mainData.ptr;
                    mainLen = mainData.length;
                    mainSize = mainData.size;

                    var subData = ImageManager.GetImageData(in subImage);
                    subPtr = subData.ptr;
                    subLen = subData.length;
                    subSize = subData.size;
                }
                catch (Exception ex)
                {
                    _logger?.Error($"获取图像数据失败: {ex.Message}");
                    return false;
                }

                // 验证指针和长度
                if (mainPtr == IntPtr.Zero || subPtr == IntPtr.Zero)
                {
                    _logger?.Error("图像数据指针为空");
                    return false;
                }

                if (mainLen <= 0 || subLen <= 0)
                {
                    _logger?.Error($"无效的图像数据长度: main={mainLen}, sub={subLen}");
                    return false;
                }

                // 验证图像尺寸
                if (mainSize.x == 0 || mainSize.y == 0 || subSize.x == 0 || subSize.y == 0)
                {
                    _logger?.Error("无效的图像尺寸");
                    return false;
                }

                // 确保子图像不大于主图像
                if (subSize.x > mainSize.x || subSize.y > mainSize.y)
                {
                    _logger?.Error($"子图像尺寸大于主图像\n\n 子图像{subSize.x},{subSize.y}\r\n母图像{mainSize.x},{mainSize.y}");
                    return false;
                }

                // 验证数据长度与尺寸匹配
                int expectedMainLen = (int)(mainSize.x * mainSize.y * 4);
                int expectedSubLen = (int)(subSize.x * subSize.y * 4);

                if (mainLen != expectedMainLen || subLen != expectedSubLen)
                {
                    _logger?.Error($"数据长度与图像尺寸不匹配: " +
                        $"main expected={expectedMainLen}, actual={mainLen}, " +
                        $"sub expected={expectedSubLen}, actual={subLen}");
                    return false;
                }

                // 处理忽略色
                byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
                if (ignoreColor.HasValue)
                {
                    ignoreR = ignoreColor.Value.R;
                    ignoreG = ignoreColor.Value.G;
                    ignoreB = ignoreColor.Value.B;
                }

                // 确保内存对齐和有效性
                GC.KeepAlive(mainImage);
                GC.KeepAlive(subImage);

                // 调用 Rust 函数
                var result = FindBytesRust(
                    mainPtr,
                    (UIntPtr)mainLen,
                    mainSize,
                    subPtr,
                    (UIntPtr)subLen,
                    subSize,
                    matchRate,
                    ignoreR, ignoreG, ignoreB
                );

                // 检查是否找到
                if (result.x == NotFound.x && result.y == NotFound.y)
                    return false;

                // 验证结果在合理范围内
                if (result.x >= mainSize.x || result.y >= mainSize.y)
                {
                    _logger?.Error($"查找结果超出图像范围: ({result.x}, {result.y})");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error($"图像查找失败: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        // 带容差的查找
        /// <summary>
        /// 带容差的查找 - 增加安全检查
        /// </summary>
        public static unsafe Point? FindImageWithTolerance(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            byte tolerance,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            try
            {
                // 验证句柄有效性
                if (!mainImage.IsValid || !subImage.IsValid)
                {
                    _logger?.Error("无效的图像句柄");
                    return null;
                }

                // 获取图像数据
                var mainData = ImageManager.GetImageData(in mainImage);
                var subData = ImageManager.GetImageData(in subImage);

                // 验证数据
                if (mainData.ptr == IntPtr.Zero || subData.ptr == IntPtr.Zero ||
                    mainData.length <= 0 || subData.length <= 0)
                {
                    _logger?.Error("无效的图像数据");
                    return null;
                }

                byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
                if (ignoreColor.HasValue)
                {
                    ignoreR = ignoreColor.Value.R;
                    ignoreG = ignoreColor.Value.G;
                    ignoreB = ignoreColor.Value.B;
                }

                // 确保内存有效
                GC.KeepAlive(mainImage);
                GC.KeepAlive(subImage);

                var result = FindBytesTolerance(
                    mainData.ptr,
                    (UIntPtr)mainData.length,
                    mainData.size,
                    subData.ptr,
                    (UIntPtr)subData.length,
                    subData.size,
                    matchRate,
                    ignoreR, ignoreG, ignoreB,
                    tolerance
                );

                if (result.x == NotFound.x && result.y == NotFound.y)
                    return null;

                return new Point((int)result.x, (int)result.y);
            }
            catch (Exception ex)
            {
                _logger?.Error($"带容差的图像查找失败: {ex.Message}");
                return null;
            }
        }

        // 新增：批量查找配置
        [StructLayout(LayoutKind.Sequential)]
        public struct FindAllConfig
        {
            public int max_results;
            public int min_distance;
            [MarshalAs(UnmanagedType.I1)]
            public bool early_exit;
        }

        // 新增：批量查找结果
        [StructLayout(LayoutKind.Sequential)]
        public struct MultipleResults
        {
            public IntPtr points;
            public int count;
            public int capacity;
        }

        [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern MultipleResults FindAllBytesInRegion(
            IntPtr n1, UIntPtr len1, Tuple t1,
            IntPtr n2, UIntPtr len2, Tuple t2,
            Region region,
            double matchRate,
            byte ignore_r, byte ignore_g, byte ignore_b,
            FindAllConfig config
        );

        [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void FreeMultipleResults(MultipleResults results);

        /// <summary>
        /// 查找所有匹配的图像（高性能Rust实现）
        /// </summary>
        public static unsafe List<Point> FindAllImages(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            double matchRate = 0.9,
            int maxResults = 100,
            int minDistance = 1,
            bool earlyExit = true,
            Color? ignoreColor = null)
        {
            var results = new List<Point>();

            try
            {
                // 验证句柄
                if (!mainImage.IsValid || !subImage.IsValid)
                {
                    _logger?.Error("无效的图像句柄");
                    return results;
                }

                // 获取图像数据
                var mainData = ImageManager.GetImageData(mainImage);
                var subData = ImageManager.GetImageData(subImage);

                // 设置搜索区域为整个图像
                var region = new Region
                {
                    x = 0,
                    y = 0,
                    width = (uint)mainImage.Size.Width,
                    height = (uint)mainImage.Size.Height
                };

                // 配置批量查找参数
                var config = new FindAllConfig
                {
                    max_results = maxResults,
                    min_distance = minDistance,
                    early_exit = earlyExit
                };

                // 处理忽略色
                byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
                if (ignoreColor.HasValue)
                {
                    ignoreR = ignoreColor.Value.R;
                    ignoreG = ignoreColor.Value.G;
                    ignoreB = ignoreColor.Value.B;
                }

                // 确保内存有效
                GC.KeepAlive(mainImage);
                GC.KeepAlive(subImage);

                // 调用 Rust 函数
                var multiResults = FindAllBytesInRegion(
                    mainData.ptr, (UIntPtr)mainData.length, mainData.size,
                    subData.ptr, (UIntPtr)subData.length, subData.size,
                    region,
                    matchRate,
                    ignoreR, ignoreG, ignoreB,
                    config
                );

                // 转换结果
                if (multiResults.count > 0 && multiResults.points != IntPtr.Zero)
                {
                    Tuple* points = (Tuple*)multiResults.points;
                    for (int i = 0; i < multiResults.count; i++)
                    {
                        if (points[i].x != NotFound.x)
                        {
                            results.Add(new Point((int)points[i].x, (int)points[i].y));
                        }
                    }
                }

                // 释放 Rust 分配的内存
                FreeMultipleResults(multiResults);
            }
            catch (Exception ex)
            {
                _logger?.Error($"批量查找图像失败: {ex.Message}");
            }

            return results;
        }

        /// <summary>
        /// 在指定区域查找所有匹配的图像
        /// </summary>
        public static unsafe List<Point> FindAllImagesInRegion(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            Rectangle searchRegion,
            double matchRate = 0.9,
            int maxResults = 100,
            int minDistance = 1,
            Color? ignoreColor = null)
        {
            var results = new List<Point>();

            try
            {
                var mainData = ImageManager.GetImageData(mainImage);
                var subData = ImageManager.GetImageData(subImage);

                // 设置搜索区域
                var region = new Region
                {
                    x = (uint)searchRegion.X,
                    y = (uint)searchRegion.Y,
                    width = (uint)searchRegion.Width,
                    height = (uint)searchRegion.Height
                };

                var config = new FindAllConfig
                {
                    max_results = maxResults,
                    min_distance = minDistance,
                    early_exit = true
                };

                byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
                if (ignoreColor.HasValue)
                {
                    ignoreR = ignoreColor.Value.R;
                    ignoreG = ignoreColor.Value.G;
                    ignoreB = ignoreColor.Value.B;
                }

                GC.KeepAlive(mainImage);
                GC.KeepAlive(subImage);

                var multiResults = FindAllBytesInRegion(
                    mainData.ptr, (UIntPtr)mainData.length, mainData.size,
                    subData.ptr, (UIntPtr)subData.length, subData.size,
                    region,
                    matchRate,
                    ignoreR, ignoreG, ignoreB,
                    config
                );

                if (multiResults.count > 0 && multiResults.points != IntPtr.Zero)
                {
                    Tuple* points = (Tuple*)multiResults.points;
                    for (int i = 0; i < multiResults.count; i++)
                    {
                        results.Add(new Point((int)points[i].x, (int)points[i].y));
                    }
                }

                FreeMultipleResults(multiResults);
            }
            catch (Exception ex)
            {
                _logger?.Error($"区域批量查找失败: {ex.Message}");
            }

            return results;
        }
    } 
    #endregion

    #region 其他查找功能
    // 高级图像查找功能
    public static class AdvancedImageFinder
    {
        // 多模板匹配
        public static Dictionary<string, Point?> FindMultipleTemplates(
            ImageHandle mainImage,
            Dictionary<string, ImageHandle> templates,
            double matchRate = 0.95)
        {
            var results = new ConcurrentDictionary<string, Point?>();

            // 并行查找所有模板
            Parallel.ForEach(templates, template =>
            {
                var position = ImageFinder.FindImage(mainImage, template.Value, matchRate);
                results.TryAdd(template.Key, position);
            });

            return results.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        // 查找最佳匹配
        public static (Point? position, double confidence) FindBestMatch(
            in ImageHandle mainImage,
            in ImageHandle subImage,
            double minMatchRate = 0.7)
        {
            // 从高到低尝试不同的匹配率
            for (double rate = 1.0; rate >= minMatchRate; rate -= 0.05)
            {
                var position = ImageFinder.FindImage(mainImage, subImage, rate);
                if (position.HasValue)
                {
                    return (position, rate);
                }
            }

            return (null, 0.0);
        }

        // 带缩放的查找
        public static Point? FindImageWithScale(
            in ImageHandle mainImage,
            in ImageHandle subImage,
            float minScale = 0.8f,
            float maxScale = 1.2f,
            float scaleStep = 0.1f,
            double matchRate = 0.95)
        {
            // TODO: 实现缩放查找逻辑
            // 需要创建不同缩放比例的子图像并逐一匹配
            throw new NotImplementedException("缩放查找功能尚未实现");
        }

        // 模糊查找（使用容差）
        public static List<(Point position, double similarity)> FuzzyFind(
            in ImageHandle mainImage,
            in ImageHandle subImage,
            byte maxTolerance = 30,
            double minMatchRate = 0.7)
        {
            var results = new List<(Point, double)>();

            // 尝试不同的容差值
            for (byte tolerance = 0; tolerance <= maxTolerance; tolerance += 5)
            {
                var position = ImageFinder.FindImageWithTolerance(
                    mainImage, subImage, tolerance, minMatchRate);

                if (position.HasValue)
                {
                    // 计算相似度（容差越小，相似度越高）
                    double similarity = 1.0 - (tolerance / (double)maxTolerance) * 0.3;
                    results.Add((position.Value, similarity));
                    break; // 找到第一个匹配就停止
                }
            }

            return results;
        }
    } 
    #endregion

    #endregion

    #region 系统监控和维护

    // 系统监控维护
    public static class ImageSystemMonitor
    {
        private static readonly Timer _maintenanceTimer;
        private static bool _autoMaintenanceEnabled = true;
        private static ILogger _logger = new ConsoleLogger();

        public static void SetLogger(ILogger logger)
        {
            _logger = logger ?? new ConsoleLogger();
        }

        public static void PrintStatus()
        {
            _logger.LogInfo("=== 图像系统状态 ===");
            ImageProcessingSystem.ReportUsage();
            _logger.LogInfo($"静态缓存数:{StaticImageCache.Count}");
            _logger.LogInfo($"像素缓存数: {PixelCodeCache.CacheSize}");
            _logger.LogInfo("==================");
        }

        public static bool AutoMaintenanceEnabled
        {
            get => _autoMaintenanceEnabled;
            set
            {
                _autoMaintenanceEnabled = value;
                if (value)
                {
                    _maintenanceTimer.Change(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
                }
                else
                {
                    _maintenanceTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
        }

        // 手动触发完整清理
        public static void PerformFullCleanup()
        {
            _logger.LogInfo("执行完整清理...");

            // 清理缓存
            StaticImageCache.Clear();
            DynamicImageBuffer.Cleanup();
            PixelCodeCache.Clear();

            // 强制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            _logger.LogInfo("清理完成");
            PrintStatus();
        }
    }

    #endregion
}