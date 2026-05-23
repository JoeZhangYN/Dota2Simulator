using System;
using System.Collections.Concurrent;
using System.Drawing;

namespace Dota2Simulator.Vision
{
    /// <summary>
    /// ImageManager 拆分 partial：句柄登记表 + 元数据 struct + 日志字段 + ReleaseImage。
    /// 所有静态字段（_images / _nextId / _logger）统一在本 partial 声明，
    /// 避免跨 partial 静态字段初始化时序问题。
    /// </summary>
    public static partial class ImageManager
    {
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
    }
}
