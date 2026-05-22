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
}
