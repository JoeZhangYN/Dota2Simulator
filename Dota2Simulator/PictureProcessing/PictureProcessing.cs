using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Dota2Simulator;

/// <summary>
///     图片处理类
/// </summary>
public class PictureProcessing
{

    #region 图片处理

    #region 屏幕截图

    public struct Bgr8
    {
        public uint b;
        public uint g;
        public uint r;
    }

    [DllImport("rscaptrs.dll")]
    public static extern IEnumerable<Bgr8> GetColor(uint i);

    [DllImport("rscaptrs.dll")]
    public static extern IntTuple GetSize();

    [StructLayout(LayoutKind.Sequential)]
    public struct IntTuple
    {
        public uint x;
        public uint y;

        public static implicit operator Tuple<uint, uint>(IntTuple t)
        {
            return Tuple.Create(t.x, t.y);
        }

        public static implicit operator IntTuple(Tuple<uint, uint> t)
        {
            return new IntTuple { x = t.Item1, y = t.Item2 };
        }
    };

    /// <summary>
    ///     屏幕截图，单操作耗时7ms，能一次解决的不要并行（因为有锁，基本时间是翻倍的）
    /// </summary>
    /// <param name="x">图片左上角X坐标</param>
    /// <param name="y">图片左上角Y坐标</param>
    /// <param name="width">图片的宽度</param>
    /// <param name="height">图片的长度</param>
    /// <returns></returns>
    public static Bitmap CaptureScreen(int x, int y, int width, int height)
    {
        Bitmap bitmap = new(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(x, y, 0, 0, new Size(width, height));
        return bitmap;

        //SaveFileDialog dialog = new SaveFileDialog();
        //dialog.Filter = "Png Files|*.png";
        //if (dialog.ShowDialog() == DialogResult.OK) bitmap.Save(dialog.FileName, ImageFormat.Png);
    }

    /// <summary>
    ///     屏幕截图，单操作耗时7ms，能一次解决的不要并行（因为有锁，基本时间是翻倍的）
    /// </summary>
    /// <param name="x">图片左上角X坐标</param>
    /// <param name="y">图片左上角Y坐标</param>
    /// <param name="width">图片的宽度</param>
    /// <param name="height">图片的长度</param>
    /// <param name="bitmap">输出的bitmap</param>
    /// <returns></returns>
    public static void CaptureScreen(int x, int y, ref Bitmap bitmap)
    {
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(x, y, 0, 0, bitmap.Size);

        //SaveFileDialog dialog = new SaveFileDialog();
        //dialog.Filter = "Png Files|*.png";
        //if (dialog.ShowDialog() == DialogResult.OK) bitmap.Save(dialog.FileName, ImageFormat.Png);
    }

    #endregion

    #region 屏幕取色

    /// <summary>
    ///     屏幕截图，单操作耗时7ms（最好是一次性截图，然后多点提取）
    /// </summary>
    /// <param name="x">图片左上角X坐标</param>
    /// <param name="y">图片左上角Y坐标</param>
    /// <param name="width">图片的宽度</param>
    /// <param name="height">图片的长度</param>
    /// <returns></returns>
    public static Color CaptureColor(int x, int y)
    {
        Color c;
        using Bitmap bitmap = new(1, 1);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(x, y, 0, 0, new Size(1, 1));
        }

        c = bitmap.GetPixel(0, 0);

        return c;

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
    public static Point FindColor(Bitmap parBitmap, string searchColor, Rectangle searchRect = new(),
        byte errorRange = 10)
    {
        var colorX = ColorTranslator.FromHtml(searchColor);
        var parData = parBitmap.LockBits(new Rectangle(0, 0, parBitmap.Width, parBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var byteArraryPar = new byte[parData.Stride * parData.Height];
        Marshal.Copy(parData.Scan0, byteArraryPar, 0, parData.Stride * parData.Height);
        if (searchRect.IsEmpty) searchRect = new Rectangle(0, 0, parBitmap.Width, parBitmap.Height);
        var searchLeftTop = searchRect.Location;
        var searchSize = searchRect.Size;
        var iMax = searchLeftTop.Y + searchSize.Height; //行
        var jMax = searchLeftTop.X + searchSize.Width; //列
        var pointX = -1;
        var pointY = -1;
        for (var m = searchRect.Y; m < iMax; m++)
        for (var n = searchRect.X; n < jMax; n++)
        {
            var index = m * parBitmap.Width * 4 + n * 4;
            var color = Color.FromArgb(byteArraryPar[index + 3], byteArraryPar[index + 2],
                byteArraryPar[index + 1], byteArraryPar[index]);
            if (!ColorAEqualColorB(color, colorX, errorRange)) continue;
            pointX = n;
            pointY = m;
            goto END;
        }

        END:
        parBitmap.UnlockBits(parData);
        return new Point(pointX, pointY);
    }

    #endregion

    #region 找图

    /// <summary>
    ///     根据数组查找坐标
    /// </summary>
    /// <param name="byteArrarySub">小图像数组</param>
    /// <param name="byteArraryPar">大图像数组</param>
    /// <param name="subSize">小图像尺码</param>
    /// <param name="parSize">大图像尺码</param>
    /// <param name="errorRange">误差范围</param>
    /// <param name="matchRate">匹配率</param>
    /// <returns></returns>
    public static List<Point> FindBytesParallel(byte[] byteArrarySub,
        Size subSize, byte[] byteArraryPar, Size parSize, byte errorRange = 0,
        double matchRate = 0.9)
    {
        List<Point> ListPoint = new();
        var subWidth = subSize.Width;
        var subHeight = subSize.Height;
        var parWidth = parSize.Width;
        //int parHeight = parBitmap.Height;
        var searchRect = new Rectangle(0, 0, parSize.Width, parSize.Height);

        var searchLeftTop = searchRect.Location;
        var searchSize = searchRect.Size;
        // 第一个颜色
        var startPixelColor = Color.FromArgb(byteArrarySub[3], byteArrarySub[2],
            byteArrarySub[1], byteArrarySub[0]);

        var iMax = searchLeftTop.Y + searchSize.Height - subSize.Height; //行
        var jMax = searchLeftTop.X + searchSize.Width - subSize.Width; //列

        int smallOffsetX = 0, smallOffsetY = 0;
        int smallStartX, smallStartY;
        int pointX;
        int pointY;


        var balanceLock = new object();

        Parallel.For(searchLeftTop.X, jMax, () => new Point(), (j, loop, subPoint) =>
            {
                for (var i = searchLeftTop.Y; i < iMax; i++)
                {
                    // for (var j = searchLeftTop.X; j < jMax; j++)

                    //大图x，y坐标处的颜色值
                    int x = j, y = i;
                    var parIndex = i * parWidth * 4 + j * 4;
                    var colorBig = Color.FromArgb(byteArraryPar[parIndex + 3], byteArraryPar[parIndex + 2],
                        byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);
                    if (ColorAEqualColorB(colorBig, startPixelColor, errorRange))
                    {
                        smallStartX = x - smallOffsetX; //待找的图X坐标
                        smallStartY = y - smallOffsetY; //待找的图Y坐标
                        var sum = 0; //所有需要比对的有效点
                        var matchNum = 0; //成功匹配的点
                        for (var m = 0; m < subHeight; m++)
                        for (var n = 0; n < subWidth; n++)
                        {
                            int x1 = n, y1 = m;
                            var subIndex = m * subWidth * 4 + n * 4;
                            var color = Color.FromArgb(byteArrarySub[subIndex + 3], byteArrarySub[subIndex + 2],
                                byteArrarySub[subIndex + 1], byteArrarySub[subIndex]);

                            sum++;
                            int x2 = smallStartX + x1, y2 = smallStartY + y1;
                            var parReleativeIndex = y2 * parWidth * 4 + x2 * 4; //比对大图对应的像素点的颜色
                            var colorPixel = Color.FromArgb(byteArraryPar[parReleativeIndex + 3],
                                byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                                byteArraryPar[parReleativeIndex]);
                            if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
                        }

                        if (!((double) matchNum / sum >= matchRate)) continue;
                        // Console.WriteLine((double)matchNum / sum);
                        pointX = smallStartX;
                        pointY = smallStartY;
                        Point point = new(pointX, pointY);
                        if (!ListContainsPoint(ListPoint, point))
                            subPoint = point;
                        return subPoint;
                    }
                }

                return subPoint;

                //小图x1,y1坐标处的颜色值
            },
            x =>
            {
                lock (balanceLock)
                {
                    if (x.X != 0) ListPoint.Add(x);
                }
            }
        );

        return ListPoint;
    }

    /// <summary>
    ///     查找图片，不能镂空
    /// </summary>
    /// <param name="subBitmap">小图像</param>
    /// <param name="parBitmap">大图像</param>
    /// <param name="searchRect">如果为empty，则默认查找整个图像</param>
    /// <param name="errorRange">容错，单个色值范围内视为正确0~255</param>
    /// <param name="matchRate">图片匹配度，默认90%</param>
    /// <returns>返回查找到的图片的左上角坐标</returns>
    public static List<Point> FindPictureParallel(Bitmap subBitmap, Bitmap parBitmap,
        Rectangle searchRect = new(), byte errorRange = 0,
        double matchRate = 0.9)
    {
        List<Point> listPoint = new();
        var subWidth = subBitmap.Width;
        var subHeight = subBitmap.Height;
        var parWidth = parBitmap.Width;
        //int parHeight = parBitmap.Height;
        if (searchRect.IsEmpty) searchRect = new Rectangle(0, 0, parBitmap.Width, parBitmap.Height);

        var searchLeftTop = searchRect.Location;
        var searchSize = searchRect.Size;
        var startPixelColor = subBitmap.GetPixel(0, 0);
        var subData = subBitmap.LockBits(new Rectangle(0, 0, subBitmap.Width, subBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var parData = parBitmap.LockBits(new Rectangle(0, 0, parBitmap.Width, parBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var byteArrarySub = new byte[subData.Stride * subData.Height];
        var byteArraryPar = new byte[parData.Stride * parData.Height];
        Marshal.Copy(subData.Scan0, byteArrarySub, 0, subData.Stride * subData.Height);
        Marshal.Copy(parData.Scan0, byteArraryPar, 0, parData.Stride * parData.Height);

        var iMax = searchLeftTop.Y + searchSize.Height - subData.Height; //行
        var jMax = searchLeftTop.X + searchSize.Width - subData.Width; //列

        int smallOffsetX = 0, smallOffsetY = 0;
        int smallStartX, smallStartY;
        int pointX;
        int pointY;


        var balanceLock = new object();

        Parallel.For(searchLeftTop.X, jMax, () => new Point(), (j, loop, subPoint) =>
            {
                for (var i = searchLeftTop.Y; i < iMax; i++)
                {
                    // for (var j = searchLeftTop.X; j < jMax; j++)

                    //大图x，y坐标处的颜色值
                    int x = j, y = i;
                    var parIndex = i * parWidth * 4 + j * 4;
                    var colorBig = Color.FromArgb(byteArraryPar[parIndex + 3], byteArraryPar[parIndex + 2],
                        byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);
                    if (!ColorAEqualColorB(colorBig, startPixelColor, errorRange)) continue;
                    smallStartX = x - smallOffsetX; //待找的图X坐标
                    smallStartY = y - smallOffsetY; //待找的图Y坐标
                    var sum = 0; //所有需要比对的有效点
                    var matchNum = 0; //成功匹配的点
                    for (var m = 0; m < subHeight; m++)
                    for (var n = 0; n < subWidth; n++)
                    {
                        int x1 = n, y1 = m;
                        var subIndex = m * subWidth * 4 + n * 4;
                        var color = Color.FromArgb(byteArrarySub[subIndex + 3], byteArrarySub[subIndex + 2],
                            byteArrarySub[subIndex + 1], byteArrarySub[subIndex]);

                        sum++;
                        int x2 = smallStartX + x1, y2 = smallStartY + y1;
                        var parReleativeIndex = y2 * parWidth * 4 + x2 * 4; //比对大图对应的像素点的颜色
                        var colorPixel = Color.FromArgb(byteArraryPar[parReleativeIndex + 3],
                            byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                            byteArraryPar[parReleativeIndex]);
                        if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
                    }

                    if (!((double) matchNum / sum >= matchRate)) continue;
                    // Console.WriteLine((double)matchNum / sum);
                    pointX = smallStartX;
                    pointY = smallStartY;
                    Point point = new(pointX, pointY);
                    if (!ListContainsPoint(listPoint, point))
                        subPoint = point;
                    return subPoint;
                }

                return subPoint;

                //小图x1,y1坐标处的颜色值
            },
            x =>
            {
                lock (balanceLock)
                {
                    if (x.X != 0) listPoint.Add(x);
                }
            }
        );

        subBitmap.UnlockBits(subData);
        parBitmap.UnlockBits(parData);
        subBitmap.Dispose();
        parBitmap.Dispose();
        GC.Collect();
        return listPoint;
    }

    /// <summary>
    ///     查找图片，不能镂空
    /// </summary>
    /// <param name="subBitmap">小图像</param>
    /// <param name="parBitmap">大图像</param>
    /// <param name="searchRect">如果为empty，则默认查找整个图像</param>
    /// <param name="errorRange">容错，单个色值范围内视为正确0~255</param>
    /// <param name="matchRate">图片匹配度，默认90%</param>
    /// <returns>返回查找到的图片的左上角坐标</returns>
    public static List<Point> FindPicture(Bitmap subBitmap, Bitmap parBitmap,
        Rectangle searchRect = new(), byte errorRange = 0,
        double matchRate = 0.9, bool isFindAll = true)
    {
        List<Point> listPoint = new();
        var subWidth = subBitmap.Width;
        var subHeight = subBitmap.Height;
        var parWidth = parBitmap.Width;
        //int parHeight = parBitmap.Height;
        if (searchRect.IsEmpty) searchRect = new Rectangle(0, 0, parBitmap.Width, parBitmap.Height);

        var searchLeftTop = searchRect.Location;
        var searchSize = searchRect.Size;
        var startPixelColor = subBitmap.GetPixel(0, 0);
        var subData = subBitmap.LockBits(new Rectangle(0, 0, subBitmap.Width, subBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var parData = parBitmap.LockBits(new Rectangle(0, 0, parBitmap.Width, parBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var byteArrarySub = new byte[subData.Stride * subData.Height];
        var byteArraryPar = new byte[parData.Stride * parData.Height];
        Marshal.Copy(subData.Scan0, byteArrarySub, 0, subData.Stride * subData.Height);
        Marshal.Copy(parData.Scan0, byteArraryPar, 0, parData.Stride * parData.Height);

        var iMax = searchLeftTop.Y + searchSize.Height - subData.Height; //行
        var jMax = searchLeftTop.X + searchSize.Width - subData.Width; //列

        int smallOffsetX = 0, smallOffsetY = 0;
        int smallStartX, smallStartY;
        int pointX;
        int pointY;

        for (var i = searchLeftTop.Y; i < iMax; i++)
        for (var j = searchLeftTop.X; j < jMax; j++)
        {
            //大图x，y坐标处的颜色值
            int x = j, y = i;
            var parIndex = i * parWidth * 4 + j * 4;
            var colorBig = Color.FromArgb(byteArraryPar[parIndex + 3], byteArraryPar[parIndex + 2],
                byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);
            if (!ColorAEqualColorB(colorBig, startPixelColor, errorRange)) continue;
            smallStartX = x - smallOffsetX; //待找的图X坐标
            smallStartY = y - smallOffsetY; //待找的图Y坐标
            var sum = 0; //所有需要比对的有效点
            var matchNum = 0; //成功匹配的点
            for (var m = 0; m < subHeight; m++)
            for (var n = 0; n < subWidth; n++)
            {
                int x1 = n, y1 = m;
                var subIndex = m * subWidth * 4 + n * 4;
                var color = Color.FromArgb(byteArrarySub[subIndex + 3], byteArrarySub[subIndex + 2],
                    byteArrarySub[subIndex + 1], byteArrarySub[subIndex]);

                sum++;
                int x2 = smallStartX + x1, y2 = smallStartY + y1;
                var parReleativeIndex = y2 * parWidth * 4 + x2 * 4; //比对大图对应的像素点的颜色
                var colorPixel = Color.FromArgb(byteArraryPar[parReleativeIndex + 3],
                    byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                    byteArraryPar[parReleativeIndex]);
                if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
            }

            if (!((double) matchNum / sum >= matchRate)) continue;
            // Console.WriteLine((double)matchNum / sum);
            pointX = smallStartX;
            pointY = smallStartY;
            Point point = new(pointX, pointY);
            if (!ListContainsPoint(listPoint, point)) listPoint.Add(point);
            if (!isFindAll) goto FIND_END;
        }

        FIND_END:
        subBitmap.UnlockBits(subData);
        parBitmap.UnlockBits(parData);
        subBitmap.Dispose();
        parBitmap.Dispose();
        GC.Collect();
        return listPoint;
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
        return //Math.Abs(colorA.A - colorB.A) <= errorRange &&
            Math.Abs(colorA.R - colorB.R) <= errorRange &&
            Math.Abs(colorA.G - colorB.G) <= errorRange &&
            Math.Abs(colorA.B - colorB.B) <= errorRange;
    }

    public static bool ColorAEqualColorB(Color colorA, Color colorB, byte errorR, byte errorG, byte errorB)
    {
        return //Math.Abs(colorA.A - colorB.A) <= errorRange &&
            Math.Abs(colorA.R - colorB.R) <= errorR &&
            Math.Abs(colorA.G - colorB.G) <= errorG &&
            Math.Abs(colorA.B - colorB.B) <= errorB;
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
        var isExist = false;
        foreach (var item in listPoint.Where(item => item.X <= point.X + errorRange && item.X >= point.X - errorRange &&
                                                     item.Y <= point.Y + errorRange && item.Y >= point.Y - errorRange))
            isExist = true;
        return isExist;
    }

    #endregion

    #region 增加对比度

    public static Bitmap MethodBaseOnMemory(Bitmap bitmap, int degree = 15)
    {
        if (bitmap == null) return null;
        var Deg = (100.0 + degree) / 100.0;

        var width = bitmap.Width;
        var height = bitmap.Height;

        var length = height * 3 * width;
        var RGB = new byte[length];

        var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb);

        var Scan0 = data.Scan0;
        Marshal.Copy(Scan0, RGB, 0, length);

        double gray;
        for (var i = 0; i < RGB.Length; i += 3)
        for (var j = 0; j < 3; j++)
        {
            gray = ((RGB[i + j] / 255.0 - 0.5) * Deg + 0.5) * 255.0;
            if (gray > 255)
                gray = 255;

            if (gray < 0)
                gray = 0;
            RGB[i + j] = (byte) gray;
        }

        Marshal.Copy(RGB, 0, Scan0, length); // 此处Copy是之前Copy的逆操作
        bitmap.UnlockBits(data);
        return bitmap;
    }

    #endregion

    #region 转化为灰度图

    /// <summary>
    /// </summary>
    /// <param name="bmp">原彩色图片</param>
    /// <param name="mode">模式 0 加权平均 1 算数平均</param>
    /// <returns></returns>
    public static Bitmap ToGray(Bitmap bmp, int mode = 0)
    {
        if (bmp == null) return null;

        var w = bmp.Width;
        var h = bmp.Height;
        try
        {
            byte newColor = 0;
            var srcData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            unsafe
            {
                var p = (byte*) srcData.Scan0.ToPointer();
                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        if (mode == 0) // 加权平均
                            newColor = (byte) (p[0] * 0.114f + p[1] * 0.587f + p[2] * 0.299f);
                        else // 算数平均
                            newColor = (byte) ((p[0] + p[1] + p[2]) / 3.0f);
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

    public static string FormatColor(Color c)
    {
        return string.Concat(c.R.ToString(), ",", c.G.ToString(), ",", c.B.ToString());
    }

    #endregion

    #region 减少图片大小对比

    /// <summary>
    ///     需要图片完全相似，不能小图找大图
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="matchrate"></param>
    /// <returns></returns>
    public static bool RegPictrueSmall(Bitmap a, Bitmap b, double matchrate = 0.7)
    {
        return CalcSimilarDegree(GetHash(a), GetHash(b), matchrate);
    }

    private static string GetHash(Bitmap bitmap)
    {
        var grayValues = ReduceColor(ReduceSize(bitmap));
        var average = CalcAverage(grayValues);
        var reslut = ComputeBits(grayValues, average);
        return reslut;
    }

    // Step 1 : Reduce size to 8*8

    private static Bitmap ReduceSize(Bitmap bitMap, int width = 8, int height = 8)
    {
        Image img = bitMap;
        return (Bitmap) img.GetThumbnailImage(width, height, () => { return false; }, IntPtr.Zero);
    }

    // Step 2 : Reduce Color
    private static byte[] ReduceColor(Bitmap bitMap)
    {
        var grayValues = new byte[bitMap.Width * bitMap.Height];
        for (var x = 0; x < bitMap.Width; x++)
        for (var y = 0; y < bitMap.Height; y++)
        {
            var color = bitMap.GetPixel(x, y);
            var grayValue = (byte) ((color.R * 30 + color.G * 59 + color.B * 11) / 100);
            grayValues[x * bitMap.Width + y] = grayValue;
        }

        return grayValues;
    }

    // Step 3 : Average the colors
    private static byte CalcAverage(byte[] values)
    {
        var sum = 0;
        for (var i = 0; i < values.Length; i++)
            sum += values[i];
        return Convert.ToByte(sum / values.Length);
    }


    // Step 4 : Compute the bits

    private static string ComputeBits(byte[] values, byte averageValue)
    {
        var result = new char[values.Length];
        for (var i = 0; i < values.Length; i++)
            if (values[i] < averageValue)
                result[i] = '0';
            else
                result[i] = '1';
        return new string(result);
    }

    // Compare hash
    private static bool CalcSimilarDegree(string a, string b, double matchRate = 0.7)
    {
        if (a.Length != b.Length)
            return false;
        var count = a.Where((t, i) => t == b[i]).Count();
        return count / a.Length >= matchRate;
    }

    #endregion
}