using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Dota2Simulator
{
    /// <summary>
    ///     图片处理类
    /// </summary>
    public class PictureProcessing
    {
        #region 图片处理

        #region 屏幕截图

        /// <summary>
        ///     屏幕截图
        /// </summary>
        /// <param name="x">图片左上角X坐标</param>
        /// <param name="y">图片左上角Y坐标</param>
        /// <param name="width">图片的宽度</param>
        /// <param name="height">图片的长度</param>
        /// <returns></returns>
        public static Bitmap CaptureScreen(double x, double y, double width, double height)
        {
            int ix = Convert.ToInt32(x);
            int iy = Convert.ToInt32(y);
            int iw = Convert.ToInt32(width);
            int ih = Convert.ToInt32(height);

            Bitmap bitmap = new(iw, ih);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(ix, iy, 0, 0, new Size(iw, ih));
            return bitmap;

            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "Png Files|*.png";
            //if (dialog.ShowDialog() == DialogResult.OK) bitmap.Save(dialog.FileName, ImageFormat.Png);
        }

        #endregion

        #region 屏幕取色

        /// <summary>
        ///     屏幕截图
        /// </summary>
        /// <param name="x">图片左上角X坐标</param>
        /// <param name="y">图片左上角Y坐标</param>
        /// <param name="width">图片的宽度</param>
        /// <param name="height">图片的长度</param>
        /// <returns></returns>
        public static Color CaptureColor(double x, double y)
        {
            int ix = Convert.ToInt32(x);
            int iy = Convert.ToInt32(y);
            int iw = 1;
            int ih = 1;

            Bitmap bitmap = new(iw, ih);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(ix, iy, 0, 0, new Size(iw, ih));
            return bitmap.GetPixel(0, 0);

            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "Png Files|*.png";
            //if (dialog.ShowDialog() == DialogResult.OK) bitmap.Save(dialog.FileName, ImageFormat.Png);
        }

        #endregion

        #region 找色

        /// <summary>
        ///     找颜色
        /// </summary>
        /// <param name="parPic">查找的图片的绝对路径</param>
        /// <param name="searchColor">查找的16进制颜色值，如#0C5FAB</param>
        /// <param name="searchRect">查找的矩形区域范围内</param>
        /// <param name="errorRange">容错</param>
        /// <returns></returns>
        public static Point FindColor(Bitmap parBitmap, string searchColor, Rectangle searchRect = new Rectangle(),
            byte errorRange = 10)
        {
            Color colorX = ColorTranslator.FromHtml(searchColor);
            BitmapData parData = parBitmap.LockBits(new Rectangle(0, 0, parBitmap.Width, parBitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] byteArraryPar = new byte[parData.Stride * parData.Height];
            Marshal.Copy(parData.Scan0, byteArraryPar, 0, parData.Stride * parData.Height);
            if (searchRect.IsEmpty) searchRect = new Rectangle(0, 0, parBitmap.Width, parBitmap.Height);
            Point searchLeftTop = searchRect.Location;
            Size searchSize = searchRect.Size;
            int iMax = searchLeftTop.Y + searchSize.Height; //行
            int jMax = searchLeftTop.X + searchSize.Width; //列
            int pointX = -1;
            int pointY = -1;
            for (int m = searchRect.Y; m < iMax; m++)
                for (int n = searchRect.X; n < jMax; n++)
                {
                    int index = m * parBitmap.Width * 4 + n * 4;
                    Color color = Color.FromArgb(byteArraryPar[index + 3], byteArraryPar[index + 2],
                        byteArraryPar[index + 1], byteArraryPar[index]);
                    if (ColorAEqualColorB(color, colorX, errorRange))
                    {
                        pointX = n;
                        pointY = m;
                        goto END;
                    }
                }

            END:
            parBitmap.UnlockBits(parData);
            return new Point(pointX, pointY);
        }

        #endregion

        #region 找图

        /// <summary>
        ///     查找图片，不能镂空
        /// </summary>
        /// <param name="subBitmap">原始图像</param>
        /// <param name="parBitmap">对比图像</param>
        /// <param name="searchRect">如果为empty，则默认查找整个图像</param>
        /// <param name="errorRange">容错，单个色值范围内视为正确0~255</param>
        /// <param name="matchRate">图片匹配度，默认90%</param>
        /// <param name="isFindAll">是否查找所有相似的图片</param>
        /// <returns>返回查找到的图片的左上角坐标</returns>
        public static List<Point> FindPicture(Bitmap subBitmap, Bitmap parBitmap,
            Rectangle searchRect = new Rectangle(), byte errorRange = 0,
            double matchRate = 0.9, bool isFindAll = true)
        {
            List<Point> ListPoint = new();
            int subWidth = subBitmap.Width;
            int subHeight = subBitmap.Height;
            int parWidth = parBitmap.Width;
            //int parHeight = parBitmap.Height;
            if (searchRect.IsEmpty) searchRect = new Rectangle(0, 0, parBitmap.Width, parBitmap.Height);

            Point searchLeftTop = searchRect.Location;
            Size searchSize = searchRect.Size;
            Color startPixelColor = subBitmap.GetPixel(0, 0);
            BitmapData subData = subBitmap.LockBits(new Rectangle(0, 0, subBitmap.Width, subBitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData parData = parBitmap.LockBits(new Rectangle(0, 0, parBitmap.Width, parBitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] byteArrarySub = new byte[subData.Stride * subData.Height];
            byte[] byteArraryPar = new byte[parData.Stride * parData.Height];
            Marshal.Copy(subData.Scan0, byteArrarySub, 0, subData.Stride * subData.Height);
            Marshal.Copy(parData.Scan0, byteArraryPar, 0, parData.Stride * parData.Height);

            int iMax = searchLeftTop.Y + searchSize.Height - subData.Height; //行
            int jMax = searchLeftTop.X + searchSize.Width - subData.Width; //列

            int smallOffsetX = 0, smallOffsetY = 0;
            int smallStartX, smallStartY;
            int pointX;
            int pointY;
            for (int i = searchLeftTop.Y; i < iMax; i++)
                for (int j = searchLeftTop.X; j < jMax; j++)
                {
                    //大图x，y坐标处的颜色值
                    int x = j, y = i;
                    int parIndex = i * parWidth * 4 + j * 4;
                    Color colorBig = Color.FromArgb(byteArraryPar[parIndex + 3], byteArraryPar[parIndex + 2],
                        byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);
                    ;
                    if (ColorAEqualColorB(colorBig, startPixelColor, errorRange))
                    {
                        smallStartX = x - smallOffsetX; //待找的图X坐标
                        smallStartY = y - smallOffsetY; //待找的图Y坐标
                        int sum = 0; //所有需要比对的有效点
                        int matchNum = 0; //成功匹配的点
                        for (int m = 0; m < subHeight; m++)
                            for (int n = 0; n < subWidth; n++)
                            {
                                int x1 = n, y1 = m;
                                int subIndex = m * subWidth * 4 + n * 4;
                                Color color = Color.FromArgb(byteArrarySub[subIndex + 3], byteArrarySub[subIndex + 2],
                                    byteArrarySub[subIndex + 1], byteArrarySub[subIndex]);

                                sum++;
                                int x2 = smallStartX + x1, y2 = smallStartY + y1;
                                int parReleativeIndex = y2 * parWidth * 4 + x2 * 4; //比对大图对应的像素点的颜色
                                Color colorPixel = Color.FromArgb(byteArraryPar[parReleativeIndex + 3],
                                    byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                                    byteArraryPar[parReleativeIndex]);
                                if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
                            }

                        if ((double)matchNum / sum >= matchRate)
                        {
                            // Console.WriteLine((double)matchNum / sum);
                            pointX = smallStartX;
                            pointY = smallStartY;
                            Point point = new(pointX, pointY);
                            if (!ListContainsPoint(ListPoint, point)) ListPoint.Add(point);
                            if (!isFindAll) goto FIND_END;
                        }
                    }

                    //小图x1,y1坐标处的颜色值
                }

            FIND_END:
            subBitmap.UnlockBits(subData);
            parBitmap.UnlockBits(parData);
            subBitmap.Dispose();
            parBitmap.Dispose();
            GC.Collect();
            return ListPoint;
        }

        #endregion

        #region 颜色对比

        /// <summary>
        ///     颜色对比
        /// </summary>
        /// <param name="colorA">颜色A</param>
        /// <param name="colorB">颜色B</param>
        /// <param name="errorRange">色值误差，默认值10</param>
        /// <returns></returns>
        public static bool ColorAEqualColorB(Color colorA, Color colorB, byte errorRange = 10)
        {
            return colorA.A <= colorB.A + errorRange && colorA.A >= colorB.A - errorRange &&
                   colorA.R <= colorB.R + errorRange && colorA.R >= colorB.R - errorRange &&
                   colorA.G <= colorB.G + errorRange && colorA.G >= colorB.G - errorRange &&
                   colorA.B <= colorB.B + errorRange && colorA.B >= colorB.B - errorRange;
        }

        #endregion

        #region 坐标对比

        /// <summary>
        ///     坐标比对
        /// </summary>
        /// <param name="listPoint">坐标列表</param>
        /// <param name="point">坐标</param>
        /// <param name="errorRange">误差范围</param>
        /// <returns></returns>
        private static bool ListContainsPoint(List<Point> listPoint, Point point, double errorRange = 10)
        {
            bool isExist = false;
            foreach (Point item in listPoint)
                if (item.X <= point.X + errorRange && item.X >= point.X - errorRange &&
                    item.Y <= point.Y + errorRange && item.Y >= point.Y - errorRange)
                    isExist = true;
            return isExist;
        }

        #endregion

        #region 增加对比度

        public unsafe static Bitmap MethodBaseOnMemory(Bitmap bitmap, int degree = 15)
        {
            if (bitmap == null)
            {
                return null;
            }
            double Deg = (100.0 + degree) / 100.0;

            int width = bitmap.Width;
            int height = bitmap.Height;

            int length = height * 3 * width;
            byte[] RGB = new byte[length];

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            System.IntPtr Scan0 = data.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(Scan0, RGB, 0, length);

            double gray;
            for (int i = 0; i < RGB.Length; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    gray = (((RGB[i + j] / 255.0 - 0.5) * Deg + 0.5)) * 255.0;
                    if (gray > 255)
                        gray = 255;

                    if (gray < 0)
                        gray = 0;
                    RGB[i + j] = (byte)gray;
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(RGB, 0, Scan0, length);// 此处Copy是之前Copy的逆操作
            bitmap.UnlockBits(data);
            return bitmap;
        }

        #endregion

        #region 转化为灰度图

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp">原彩色图片</param>
        /// <param name="mode">模式 0 加权平均 1 算数平均</param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp, int mode = 0)
        {
            if (bmp == null)
            {
                return null;
            }

            int w = bmp.Width;
            int h = bmp.Height;
            try
            {
                byte newColor = 0;
                BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* p = (byte*)srcData.Scan0.ToPointer();
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {

                            if (mode == 0)　// 加权平均
                            {
                                newColor = (byte)((float)p[0] * 0.114f + (float)p[1] * 0.587f + (float)p[2] * 0.299f);
                            }
                            else　　　　// 算数平均
                            {
                                newColor = (byte)((float)(p[0] + p[1] + p[2]) / 3.0f);
                            }
                            p[0] = newColor;
                            p[1] = newColor;
                            p[2] = newColor;

                            p += 3;
                        }
                        p += srcData.Stride - w * 3;
                    }
                    bmp.UnlockBits(srcData);
                    return bmp;
                }
            }
            catch
            {
                return null;
            }

        }

        #endregion

        #endregion
    }
}