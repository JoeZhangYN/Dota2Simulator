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
}
