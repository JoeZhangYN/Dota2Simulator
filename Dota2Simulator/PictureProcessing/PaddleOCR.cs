using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models.Local;

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

        public static string 获取图片文字(string path)
        {
            if (!_isStart)
            {
                _ = 初始化PaddleOcr();
            }

            using Mat src = Cv2.ImRead(path);
            PaddleOcrResult result = _PaddleOcrAll.Run(src);
            return result.Text;
        }

        public static string 获取图片文字(Bitmap bp)
        {
            if (!_isStart)
            {
                _ = 初始化PaddleOcr();
            }

            using Mat src = BitmapToMat(bp);
            using Mat bgrMat = new();
            Cv2.CvtColor(src, bgrMat, ColorConversionCodes.RGBA2BGR);

            PaddleOcrResult result = _PaddleOcrAll.Run(bgrMat);
            return result.Text;
        }

        private static Mat BitmapToMat(Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                Mat mat = new(bitmap.Height, bitmap.Width, MatType.CV_8UC4);

                byte[] bytes = new byte[bmpData.Stride * bitmap.Height];
                Marshal.Copy(bmpData.Scan0, bytes, 0, bytes.Length);
                Marshal.Copy(bytes, 0, mat.DataStart, bytes.Length);

                return mat;
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }
        }

        public static string 获取图片文字(byte[] bts, int width, int height)
        {
            if (!_isStart)
            {
                _ = 初始化PaddleOcr();
            }

            if (bts == null || bts.Length == 0)
            {
                throw new ArgumentException("Input byte array is null or empty.");
            }

            if (bts.Length != width * height * 4)
            {
                throw new ArgumentException(
                    $"Input byte array length ({bts.Length}) does not match the expected size ({width * height * 4}).");
            }

            // 直接从字节数组创建 Mat，指定正确的大小和类型
            using Mat argbMat = new(height, width, MatType.CV_8UC4);

            // 使用 Marshal.Copy 来填充 Mat 数据
            Marshal.Copy(bts, 0, argbMat.DataStart, bts.Length);

            // 转换为 BGR 格式
            using Mat bgrMat = new();
            Cv2.CvtColor(argbMat, bgrMat, ColorConversionCodes.BGRA2BGR);

            // 运行 OCR
            PaddleOcrResult result = _PaddleOcrAll.Run(bgrMat);
            return result.Text;
        }

        public static string 获取图片文字(int x, int y, int width, int height)
        {
            return 获取图片文字(PictureProcessing.CaptureScreenAllByte(x, y, width, height), width, height);
        }

        #endregion
    }
}