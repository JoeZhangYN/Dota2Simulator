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
    // 图像查找器（与Rust交互）
    public static class ImageFinder
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // Rust DLL 导入 - 新的统一接口
        [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Tuple FindBytes(
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
        private static extern Tuple FindBytesWithTolerance(
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

        [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Tuple FindBytesInRegion(
            IntPtr n1, UIntPtr len1, Tuple t1,
            IntPtr n2, UIntPtr len2, Tuple t2,
            Region region,
            double matchRate,
            byte ignore_r, byte ignore_g, byte ignore_b
        );

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

        // 批量融合：一次调用查 N 个模板（单趟 rayon 派发，摊销 per-call 开销）。
        // subs 长度 = subCount；outResults 由调用方分配 subCount 个 Tuple，逐槽写命中或 NotFound。
        [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void FindManyBytesInRegion(
            IntPtr n1, UIntPtr len1, Tuple t1,
            [In] SubImageRef[] subs, UIntPtr subCount,
            Region region,
            double matchRate,
            byte ignore_r, byte ignore_g, byte ignore_b,
            [Out] Tuple[] outResults);

        // 查找失败时的返回值
        private static readonly Tuple NotFound = new() { x = 245760, y = 143640 };

        [StructLayout(LayoutKind.Sequential)]
        public struct Tuple
        {
            public uint x;
            public uint y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Region
        {
            public uint x;
            public uint y;
            public uint width;
            public uint height;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FindAllConfig
        {
            public int max_results;
            public int min_distance;
            [MarshalAs(UnmanagedType.I1)]
            public bool early_exit;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MultipleResults
        {
            public IntPtr points;
            public int count;
            public int capacity;
        }

        // 批量融合的单个子图引用（与 Rust SubImageRef #[repr(C)] 二进制一致）。
        [StructLayout(LayoutKind.Sequential)]
        public struct SubImageRef
        {
            public IntPtr ptr;
            public UIntPtr len;
            public uint width;
            public uint height;
        }

        /// <summary>
        /// 在大图中查找小图（精确匹配）
        /// </summary>
        public static unsafe Point? FindImage(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            try
            {
                // 获取图像数据
                var mainData = ImageManager.GetImageData(in mainImage);
                var subData = ImageManager.GetImageData(in subImage);

                // 处理忽略色
                byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
                if (ignoreColor.HasValue)
                {
                    ignoreR = ignoreColor.Value.R;
                    ignoreG = ignoreColor.Value.G;
                    ignoreB = ignoreColor.Value.B;
                }

                // 确保内存有效性
                GC.KeepAlive(mainImage);
                GC.KeepAlive(subImage);

                // 调用 Rust 函数（所有验证逻辑已在Rust端处理）
                var result = FindBytes(
                    mainData.ptr,
                    (UIntPtr)mainData.length,
                    mainData.size,
                    subData.ptr,
                    (UIntPtr)subData.length,
                    subData.size,
                    matchRate,
                    ignoreR, ignoreG, ignoreB
                );

                // 检查是否找到
                if (result.x == NotFound.x && result.y == NotFound.y)
                    return null;

                return new Point((int)result.x, (int)result.y);
            }
            catch (Exception ex)
            {
                _logger?.Error($"图像查找失败: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// 在大图中查找小图（返回bool）
        /// </summary>
        public static bool FindImageBool(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            return FindImage(in subImage, in mainImage, matchRate, ignoreColor) != null;
        }

        /// <summary>
        /// 带容差的查找
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
                // 获取图像数据
                var mainData = ImageManager.GetImageData(in mainImage);
                var subData = ImageManager.GetImageData(in subImage);

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

                var result = FindBytesWithTolerance(
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

        /// <summary>
        /// 指定区域大图小图对比
        /// </summary>
        public static Point? FindImageInRegion(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            Rectangle region,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            try
            {
                var regionStruct = new Region
                {
                    x = (uint)region.X,
                    y = (uint)region.Y,
                    width = (uint)region.Width,
                    height = (uint)region.Height
                };

                // 获取图像数据
                var mainData = ImageManager.GetImageData(in mainImage);
                var subData = ImageManager.GetImageData(in subImage);

                // 处理忽略色
                byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
                if (ignoreColor.HasValue)
                {
                    ignoreR = ignoreColor.Value.R;
                    ignoreG = ignoreColor.Value.G;
                    ignoreB = ignoreColor.Value.B;
                }

                GC.KeepAlive(mainImage);
                GC.KeepAlive(subImage);

                var result = FindBytesInRegion(
                    mainData.ptr, (UIntPtr)mainData.length, mainData.size,
                    subData.ptr, (UIntPtr)subData.length, subData.size,
                    regionStruct,
                    matchRate,
                    ignoreR, ignoreG, ignoreB
                );

                return result.x == NotFound.x ? null : new Point((int)result.x, (int)result.y);
            }
            catch (Exception ex)
            {
                _logger?.Error($"区域图像查找失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 指定区域大图存在小图
        /// </summary>
        public static bool FindImageInRegionBool(
            in ImageHandle subImage,
            in ImageHandle mainImage,
            Rectangle region,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            return FindImageInRegion(in subImage, in mainImage, region, matchRate, ignoreColor) != null;
        }

        /// <summary>
        /// 批量区域查找：一次融合调用在 <paramref name="region"/> 内查 <paramref name="subImages"/>
        /// 全部模板（单趟 rayon 派发，摊销 per-call 开销）。返回与入参同序对齐的逐模板首命中
        /// （未命中槽为 null）。无效子图句柄 → 该槽 null。
        /// </summary>
        public static unsafe Point?[] FindManyInRegion(
            in ImageHandle mainImage,
            IReadOnlyList<ImageHandle> subImages,
            Rectangle region,
            double matchRate = 0.9,
            Color? ignoreColor = null)
        {
            int n = subImages.Count;
            var results = new Point?[n];
            if (n == 0) return results;

            try
            {
                var mainData = ImageManager.GetImageData(in mainImage);
                var regionStruct = new Region
                {
                    x = (uint)region.X,
                    y = (uint)region.Y,
                    width = (uint)region.Width,
                    height = (uint)region.Height
                };

                byte ignoreR = 255, ignoreG = 20, ignoreB = 147;
                if (ignoreColor.HasValue)
                {
                    ignoreR = ignoreColor.Value.R;
                    ignoreG = ignoreColor.Value.G;
                    ignoreB = ignoreColor.Value.B;
                }

                var subRefs = new SubImageRef[n];
                for (int i = 0; i < n; i++)
                {
                    ImageHandle h = subImages[i];
                    if (!h.IsValid) { subRefs[i] = default; continue; }
                    var sd = ImageManager.GetImageData(in h);
                    subRefs[i] = new SubImageRef
                    {
                        ptr = sd.ptr,
                        len = (UIntPtr)sd.length,
                        width = sd.size.x,
                        height = sd.size.y
                    };
                }

                GC.KeepAlive(mainImage);

                var outTuples = new Tuple[n];
                FindManyBytesInRegion(
                    mainData.ptr, (UIntPtr)mainData.length, mainData.size,
                    subRefs, (UIntPtr)n,
                    regionStruct, matchRate, ignoreR, ignoreG, ignoreB,
                    outTuples);

                for (int i = 0; i < n; i++)
                {
                    results[i] = (outTuples[i].x == NotFound.x && outTuples[i].y == NotFound.y)
                        ? (Point?)null
                        : new Point((int)outTuples[i].x, (int)outTuples[i].y);
                }

                for (int i = 0; i < n; i++) GC.KeepAlive(subImages[i]);
            }
            catch (Exception ex)
            {
                _logger?.Error($"批量区域查找失败: {ex.Message}");
            }

            return results;
        }

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
}
