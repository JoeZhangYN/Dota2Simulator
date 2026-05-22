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
}
