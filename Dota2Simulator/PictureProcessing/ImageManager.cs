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
using Tuple = Dota2Simulator.ImageProcessingSystem.ImageFinder.Tuple;

namespace Dota2Simulator.ImageProcessingSystem
{
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
}
