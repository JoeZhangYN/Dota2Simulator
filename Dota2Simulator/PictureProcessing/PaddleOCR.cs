using ImageProcessingSystem;
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models.Local;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Dota2Simulator.PictureProcessing
{
    /*
     *
     * 安装对应的Nuget包
     Sdcb.PaddleInference
     Sdcb.PaddleOCR
     Sdcb.PaddleOCR.Models.Local
     Sdcb.PaddleInference.runtime.win64.mkl
     Sdcb.PaddleInference.runtime.win64.cu20-sm86-89 选一个，Gpu用下面的，实际上完全没法用。
     OpenCvSharp4.runtime.win
     */
    internal abstract class PaddleOcr
    {
        private static PaddleOcrAll _PaddleOcrAll;

        private static bool _isStart;

        public static bool 初始化PaddleOcr()
        {
            _PaddleOcrAll = new PaddleOcrAll(LocalFullModels.ChineseV4, PaddleDevice.Mkldnn())
            {
                AllowRotateDetection = false, /* 允许识别有角度的文字 */
                Enable180Classification = false /* 允许识别旋转角度大于90度的文字 */
            };

            _isStart = true;
            return true;
        }

        public static bool 释放PaddleOcr()
        {
            _PaddleOcrAll?.Dispose();
            _PaddleOcrAll = null;
            return true;
        }

        #region 获取图片文字

        /// <summary>
        /// 从 ImageHandle 获取图片文字
        /// </summary>
        public static string 获取图片文字(in ImageHandle handle)
        {
            if (!_isStart)
            {
                _ = 初始化PaddleOcr();
            }

            try
            {
                var (ptr, length, size) = ImageManager.GetImageData(in handle);
                int width = (int)size.x;
                int height = (int)size.y;

                unsafe
                {
                    // 直接从指针创建 Mat，避免额外的内存复制
                    using Mat argbMat = Mat.FromPixelData(height, width, MatType.CV_8UC4, ptr);

                    // 转换为 BGR 格式（PaddleOCR 需要）
                    using Mat bgrMat = new();
                    Cv2.CvtColor(argbMat, bgrMat, ColorConversionCodes.BGRA2BGR);

                    // 运行 OCR
                    PaddleOcrResult result = _PaddleOcrAll.Run(bgrMat);
                    return result.Text;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 从 ImageHandle 的指定区域获取图片文字
        /// </summary>
        public static string 获取图片文字(in ImageHandle handle, Rectangle region)
        {
            if (!_isStart)
            {
                _ = 初始化PaddleOcr();
            }

            try
            {
                var (ptr, length, size) = ImageManager.GetImageData(in handle);

                // 确保区域在图像范围内
                var imageRect = new Rectangle(0, 0, (int)size.x, (int)size.y);
                region.Intersect(imageRect);

                if (region.IsEmpty)
                    return string.Empty;

                unsafe
                {
                    // 创建区域的临时缓冲区
                    int regionSize = region.Width * region.Height * 4;
                    byte[] regionData = new byte[regionSize];
                    fixed (byte* dstPtr = regionData)
                    {
                        uint* dst = (uint*)dstPtr;
                        int dstIndex = 0;

                        // 复制区域数据
                        for (int y = region.Y; y < region.Bottom; y++)
                        {
                            for (int x = region.X; x < region.Right; x++)
                            {
                                dst[dstIndex++] = ImageManager.GetPixel(in handle, x, y);
                            }
                        }

                        // 在 fixed 块内调用方法
                        return 获取图片文字((IntPtr)dstPtr, region.Width, region.Height);
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 优化的从指针直接创建 Mat 的方法
        /// </summary>
        public static unsafe string 获取图片文字(IntPtr ptr, int width, int height)
        {
            if (!_isStart)
            {
                _ = 初始化PaddleOcr();
            }

            if (ptr == IntPtr.Zero)
            {
                throw new ArgumentException("Input pointer is null.");
            }

            try
            {
                // 直接从指针创建 Mat
                using Mat argbMat = Mat.FromPixelData(height, width, MatType.CV_8UC4, ptr);

                // 转换为 BGR 格式
                using Mat bgrMat = new();
                Cv2.CvtColor(argbMat, bgrMat, ColorConversionCodes.BGRA2BGR);

                // 运行 OCR
                PaddleOcrResult result = _PaddleOcrAll.Run(bgrMat);
                return result.Text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 从屏幕坐标获取图片文字（适配后）
        /// </summary>
        public static string 获取图片文字(int x, int y, int width, int height)
        {
            try
            {
                var _currentScreenHandle = Games.Dota2.MainClass._全局图像句柄;
                // 方案1: 如果有全屏截图的 ImageHandle
                if (_currentScreenHandle.IsValid)
                {
                    var region = new Rectangle(x, y, width, height);
                    return 获取图片文字(in _currentScreenHandle, region);
                }

                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 批量 OCR 识别 - 用于同时识别多个区域
        /// </summary>
        public static Dictionary<Rectangle, string> 批量获取图片文字(ImageHandle handle, List<Rectangle> regions)
        {
            var results = new Dictionary<Rectangle, string>();

            if (!_isStart)
            {
                _ = 初始化PaddleOcr();
            }

            // 并行处理多个区域
            Parallel.ForEach(regions, region =>
            {
                var text = 获取图片文字(handle, region);
                lock (results)
                {
                    results[region] = text;
                }
            });

            return results;
        }

        /// <summary>
        /// 优化的 BitmapToMat 方法 - 直接使用指针
        /// </summary>
        private static unsafe Mat BitmapToMat(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                // 直接从 BitmapData 创建 Mat，避免额外的内存复制
                Mat mat = Mat.FromPixelData(bitmap.Height, bitmap.Width, MatType.CV_8UC4, bmpData.Scan0);

                // 如果需要拥有数据的副本，使用 Clone
                return mat.Clone();
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }
        }

        /// <summary>
        /// 从 ImageHandle 创建 Bitmap（如果需要与旧代码兼容）
        /// </summary>
        public static unsafe Bitmap CreateBitmapFromHandle(in ImageHandle handle)
        {
            var (ptr, length, size) = ImageManager.GetImageData(in handle);
            int width = (int)size.x;
            int height = (int)size.y;

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var bmpData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                // 直接复制数据
                Buffer.MemoryCopy((void*)ptr, (void*)bmpData.Scan0, length, length);
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }

            return bitmap;
        }
    }
    #endregion
}