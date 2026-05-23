using Collections.Pooled;
using System;
using System.Drawing;
using System.Linq;

namespace Dota2Simulator.Vision
{
    /// <summary>
    /// ImageManager 拆分 partial：多颜色点匹配（FindColors）。
    /// Note: 与 ImageFinder（位于 Matching/）职责重叠，本方法是 stackalloc 优化版本，
    /// Phase 7+ 评估是否合并到 ImageFinder。
    /// </summary>
    public static partial class ImageManager
    {
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
    }
}
