using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Dota2Simulator.Vision
{
    /// <summary>
    /// ImageManager 拆分 partial：图像创建工厂方法
    /// （动态 / 静态 / 文件 / 三缓冲变体）。
    /// </summary>
    public static partial class ImageManager
    {
        /// <summary>
        ///     创建动态图像（截图等）
        /// </summary>
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
            using (var bitmap = new System.Drawing.Bitmap(filePath))
            {
                var data = GetBitmapData(bitmap);
                return CreateStaticImage(data, bitmap.Size, name ?? Path.GetFileNameWithoutExtension(filePath));
            }
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
