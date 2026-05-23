using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Tuple = Dota2Simulator.Vision.ImageFinder.Tuple;

namespace Dota2Simulator.Vision
{
    /// <summary>
    /// ImageManager 拆分 partial：像素 / 颜色 / 数据指针读取。
    /// </summary>
    public static partial class ImageManager
    {
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
    }
}
