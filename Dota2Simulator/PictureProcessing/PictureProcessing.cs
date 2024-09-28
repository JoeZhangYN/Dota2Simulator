using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Collections.Pooled;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Dota2Simulator.PictureProcessing;

/// <summary>
///     图片处理类
/// </summary>
internal class PictureProcessing
{
    #region 屏幕取色

    public static Color CaptureColor(int x, int y)
    {
        using OptimizedGraphics graphics = OptimizedGraphics.CreateGraphics();
        byte[] bytes = graphics.CaptureScreenToBytes(x, y, 1, 1);
        return Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]);
    }

    #endregion

    #region 增加对比度

    public static Bitmap MethodBaseOnMemory(Bitmap bitmap, int degree = 15)
    {
        if (bitmap == null) return null;

        double deg = (100.0 + degree) / 100.0;

        int width = bitmap.Width;
        int height = bitmap.Height;

        int length = height * 3 * width;
        byte[] rgb = new byte[length];

        BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
            PixelFormat.Format32bppArgb);

        nint scan0 = data.Scan0;
        Marshal.Copy(scan0, rgb, 0, length);

        double gray;
        for (int i = 0; i < rgb.Length; i += 3)
        for (int j = 0; j < 3; j++)
        {
            gray = ((rgb[i + j] / 255.0 - 0.5) * deg + 0.5) * 255.0;
            if (gray > 255) gray = 255;

            if (gray < 0) gray = 0;

            rgb[i + j] = (byte)gray;
        }

        Marshal.Copy(rgb, 0, scan0, length);
        bitmap.UnlockBits(data);
        return bitmap;
    }

    #endregion

    #region 转化为灰度图

    public static Bitmap ToGray(Bitmap bmp, int mode = 0)
    {
        if (bmp == null) return null;
        int w = bmp.Width;
        int h = bmp.Height;
        try
        {
            BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* p = (byte*)srcData.Scan0.ToPointer();
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        byte newColor = mode == 0
                            ? (byte)(p[0] * 0.114f + p[1] * 0.587f + p[2] * 0.299f)
                            : (byte)((p[0] + p[1] + p[2]) / 3.0f);
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

    #region 多图片合并相同部分

    public static void PictureCombine(Bitmap[] images, string outputname)
    {
        if (images == null || images.Length < 2)
        {
            Console.WriteLine(@"需要至少两张图片进行比较。");
            return;
        }

        for (int i = 1; i < images.Length; i++)
            if (images[0].Size != images[i].Size)
            {
                Console.WriteLine(@"图像大小不同，无法进行合并。");
                return;
            }

        Bitmap resultImage = new(images[0].Width, images[0].Height);

        for (int x = 0; x < images[0].Width; x++)
        for (int y = 0; y < images[0].Height; y++)
        {
            bool identical = true;
            Color firstPixelColor = images[0].GetPixel(x, y);

            for (int n = 1; n < images.Length; n++)
                if (firstPixelColor.ToArgb() != images[n].GetPixel(x, y).ToArgb())
                {
                    identical = false;
                    break;
                }

            resultImage.SetPixel(x, y, identical ? firstPixelColor : Color.FromArgb(255, 20, 147));
        }

        resultImage.Save(outputname + ".bmp", ImageFormat.Bmp);
    }

    #endregion

    #region 屏幕截图

    internal struct Bgr8
    {
        public uint B;
        public uint G;
        public uint R;
    }

    [DllImport("rscaptrs.dll")]
    public static extern IEnumerable<Bgr8> GetColor(uint i);

    public static Bitmap CaptureScreen(int x, int y, int width, int height)
    {
        if (width + height == 0) return new Bitmap(0, 0);
        using OptimizedGraphics graphics = OptimizedGraphics.CreateGraphics();
        return graphics.CaptureScreenToBitmap(x, y, width, height);
    }

    public static byte[] CaptureScreenAllByte(int x, int y, int width, int height)
    {
        if (width + height == 0) return [];

        using OptimizedGraphics graphics = OptimizedGraphics.CreateGraphics();
        return graphics.CaptureScreenToBytes(x, y, width, height);
    }

    public static async Task<Bitmap> CaptureScreenAsync(int x, int y, Size size)
    {
        if (size.Height + size.Width == 0) return await Task.FromResult(new Bitmap(0, 0)).ConfigureAwait(true);

        using OptimizedGraphics graphics = OptimizedGraphics.CreateGraphics();
        return await Task.FromResult(graphics.CaptureScreenToBitmap(x, y, size.Width, size.Height))
            .ConfigureAwait(true);
    }

    public static void CaptureScreen_固定数组(ref 字节数组包含长宽 数组, int x, int y)
    {
        using OptimizedGraphics graphics = OptimizedGraphics.CreateGraphics();
        数组.字节数组 = graphics.CaptureScreenToBytes(x, y, 数组.图片尺寸.Width, 数组.图片尺寸.Height);
    }

    public static 字节数组包含长宽 CaptureScreen_固定大小(int x, int y, int width, int height)
    {
        using OptimizedGraphics graphics = OptimizedGraphics.CreateGraphics();
        return new 字节数组包含长宽
        {
            图片尺寸 = new Size(width, height),
            字节数组 = graphics.CaptureScreenToBytes(x, y, width, height)
        };
    }

    public static void CaptureScreen_固定大小(ref Bitmap bitmap, int x, int y)
    {
        using OptimizedGraphics graphics = OptimizedGraphics.CreateGraphics();
        graphics.CaptureScreenToExistingBitmap(ref bitmap, x, y);
    }

    #endregion

    #region 找色

    public static PooledList<Point> FindColor(Color color, byte[] byteArraryPar, Size parSize,
        byte errorRange = 0)
    {
        PooledList<Point> listPoint = [];
        int parWidth = parSize.Width;
        Rectangle searchRect = new(0, 0, parSize.Width, parSize.Height);

        Point searchLeftTop = searchRect.Location;
        Size searchSize = searchRect.Size;

        int iMax = searchLeftTop.Y + searchSize.Height;
        int jMax = searchLeftTop.X + searchSize.Width;

        object balanceLock = new();

        _ = Parallel.For(searchLeftTop.X, jMax, () => new Point(), (j, _, _) =>
            {
                for (int i = searchLeftTop.Y; i < iMax; i++)
                {
                    int parIndex = i * parWidth * 3 + j * 3;
                    Color colorBig = Color.FromArgb(byteArraryPar[parIndex + 2],
                        byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);

                    if (!ColorAEqualColorB(colorBig, color, errorRange)) continue;

                    return new Point(j, i);
                }

                return default;
            },
            x =>
            {
                lock (balanceLock)
                {
                    if (x.X != 0) listPoint.Add(x);
                }
            }
        );

        return listPoint;
    }

    public static PooledList<Point> FindColors(PooledList<Color> colors, PooledList<Point> points,
        byte[] byteArraryPar, Size parSize, byte errorRange = 0, double matchRate = 0.9)
    {
        PooledList<Point> listPoint = [];
        int subWidth = points.Max(p => p.X);
        int subHeight = points.Max(p => p.Y);

        int parWidth = parSize.Width;
        Rectangle searchRect = new(0, 0, parSize.Width, parSize.Height);

        Point searchLeftTop = searchRect.Location;
        Size searchSize = searchRect.Size;
        Color startPixelColor = colors[0];

        int iMax = searchLeftTop.Y + searchSize.Height - subHeight;
        int jMax = searchLeftTop.X + searchSize.Width - subWidth;

        for (int j = searchLeftTop.X; j < jMax; j++)
        for (int i = searchLeftTop.Y; i < iMax; i++)
        {
            int parIndex = (i + points[0].Y) * parWidth * 3 + (j + points[0].X) * 3;
            Color colorBig = Color.FromArgb(byteArraryPar[parIndex + 2],
                byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);
            if (!ColorAEqualColorB(colorBig, startPixelColor, errorRange)) continue;

            int smallStartX = j;
            int smallStartY = i;
            int sum = 0;
            int matchNum = 0;
            for (int m = 1; m < points.Count; m++)
            {
                int x2 = smallStartX + points[m].X;
                int y2 = smallStartY + points[m].Y;
                int parReleativeIndex = y2 * parWidth * 3 + x2 * 3;
                Color colorPixel = Color.FromArgb(
                    byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                    byteArraryPar[parReleativeIndex]);
                if (ColorAEqualColorB(colorPixel, colors[m], errorRange)) matchNum++;
                sum++;
            }

            if ((double)matchNum / sum >= matchRate)
            {
                Point point = new(smallStartX, smallStartY);
                if (!listPoint.Contains(point)) listPoint.Add(point);
            }
        }

        return listPoint;
    }

    #endregion

    #region 找图

    [StructLayout(LayoutKind.Sequential)]
    public struct Tuple
    {
        public uint x;
        public uint y;
    }

    [DllImport("findpoints.dll")]
    private static extern Tuple FindBytesRust(
        [In] byte[] n1,
        UIntPtr len1,
        Tuple t1,
        [In] byte[] n2,
        UIntPtr len2,
        Tuple t2,
        double matchRate,
        byte ignore_r,
        byte ignore_g,
        byte ignore_b
    );

    [DllImport("findpoints.dll")]
    private static extern Tuple FindBytesTolerance(
        [In] byte[] n1,
        UIntPtr len1,
        Tuple t1,
        [In] byte[] n2,
        UIntPtr len2,
        Tuple t2,
        double matchRate,
        byte ignore_r,
        byte ignore_g,
        byte ignore_b,
        byte tolerance
    );

    public static Tuple FindBytesR(byte[] n1, UIntPtr len1, Tuple t1, byte[] n2,
        UIntPtr len2, Tuple t2, double matchRate, byte ignore_r, byte ignore_g, byte ignore_b)
    {
        return FindBytesRust(n1, len1, t1, n2, len2, t2, matchRate, ignore_r, ignore_g, ignore_b);
    }

    public static Tuple FindBytesRTolerance(byte[] n1, UIntPtr len1, Tuple t1, byte[] n2,
        UIntPtr len2, Tuple t2, double matchRate, byte ignore_r, byte ignore_g, byte ignore_b, byte tolerance)
    {
        return FindBytesTolerance(n1, len1, t1, n2, len2, t2, matchRate, ignore_r, ignore_g, ignore_b, tolerance);
    }

    public static Point FindBitmaPoint(in Bitmap bp1, in Bitmap bp2)
    {
        Mat image = BitmapConverter.ToMat(bp1);
        Mat template = BitmapConverter.ToMat(bp2);

        Mat imageGray = new();
        Mat templateGray = new();
        Cv2.CvtColor(image, imageGray, ColorConversionCodes.BGR2GRAY);
        Cv2.CvtColor(template, templateGray, ColorConversionCodes.BGR2GRAY);

        Mat result = new();
        Cv2.MatchTemplate(imageGray, templateGray, result, TemplateMatchModes.CCoeffNormed);

        Cv2.MinMaxLoc(result, out _, out _, out _, out OpenCvSharp.Point maxLoc);

        return new Point(maxLoc.X, maxLoc.Y);
    }

    public static PooledList<Point> FindBytesParallel(byte[] byteArraySub,
        Size subSize, byte[] byteArrayPar, Size parSize, byte errorRange = 0,
        double matchRate = 0.9)
    {
        PooledList<Point> listPoint = [];
        int subWidth = subSize.Width;
        int subHeight = subSize.Height;
        int parWidth = parSize.Width;
        Rectangle searchRect = new(0, 0, parSize.Width, parSize.Height);

        Point searchLeftTop = searchRect.Location;
        Size searchSize = searchRect.Size;
        Color startPixelColor = Color.FromArgb(byteArraySub[2],
            byteArraySub[1], byteArraySub[0]);

        int iMax = searchLeftTop.Y + searchSize.Height - subSize.Height;
        int jMax = searchLeftTop.X + searchSize.Width - subSize.Width;

        object balanceLock = new();

        _ = Parallel.For(searchLeftTop.X, jMax, () => new Point(), (j, _, subPoint) =>
            {
                for (int i = searchLeftTop.Y; i < iMax; i++)
                {
                    int x = j, y = i;
                    int parIndex = i * parWidth * 4 + j * 4;
                    Color colorBig = Color.FromArgb(byteArrayPar[parIndex + 2],
                        byteArrayPar[parIndex + 1], byteArrayPar[parIndex]);
                    if (!ColorAEqualColorB(colorBig, startPixelColor, errorRange)) continue;

                    int smallStartX = x;
                    int smallStartY = y;
                    int sum = 0;
                    int matchNum = 0;
                    for (int m = 0; m < subHeight; m++)
                    for (int n = 0; n < subWidth; n++)
                    {
                        int subIndex = m * subWidth * 4 + n * 4;
                        Color color = Color.FromArgb(byteArraySub[subIndex + 2],
                            byteArraySub[subIndex + 1], byteArraySub[subIndex]);

                        sum++;
                        int x2 = smallStartX + n, y2 = smallStartY + m;
                        int parReleativeIndex = y2 * parWidth * 4 + x2 * 4;
                        Color colorPixel = Color.FromArgb(byteArrayPar[parReleativeIndex + 2],
                            byteArrayPar[parReleativeIndex + 1],
                            byteArrayPar[parReleativeIndex]);
                        if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
                    }

                    if ((double)matchNum / sum >= matchRate) return new Point(smallStartX, smallStartY);
                }

                return subPoint;
            },
            x =>
            {
                lock (balanceLock)
                {
                    if (x.X != 0) listPoint.Add(x);
                }
            }
        );

        return listPoint;
    }

    public static PooledList<Point> FindPictureParallel(Bitmap subBitmap, Bitmap parBitmap,
        Rectangle searchRect = new(), byte errorRange = 0,
        double matchRate = 0.9)
    {
        PooledList<Point> listPoint = [];
        int subWidth = subBitmap.Width;
        int subHeight = subBitmap.Height;
        int parWidth = parBitmap.Width;
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
        int iMax = searchLeftTop.Y + searchSize.Height - subData.Height;
        int jMax = searchLeftTop.X + searchSize.Width - subData.Width;

        object balanceLock = new();

        _ = Parallel.For(searchLeftTop.X, jMax, () => new Point(), (j, _, subPoint) =>
            {
                for (int i = searchLeftTop.Y; i < iMax; i++)
                {
                    int x = j, y = i;
                    int parIndex = i * parWidth * 4 + j * 4;
                    Color colorBig = Color.FromArgb(byteArraryPar[parIndex + 2],
                        byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);
                    if (!ColorAEqualColorB(colorBig, startPixelColor, errorRange)) continue;

                    int smallStartX = x;
                    int smallStartY = y;
                    int sum = 0;
                    int matchNum = 0;
                    for (int m = 0; m < subHeight; m++)
                    for (int n = 0; n < subWidth; n++)
                    {
                        int subIndex = m * subWidth * 4 + n * 4;
                        Color color = Color.FromArgb(byteArrarySub[subIndex + 2],
                            byteArrarySub[subIndex + 1], byteArrarySub[subIndex]);

                        sum++;
                        int x2 = smallStartX + n, y2 = smallStartY + m;
                        int parReleativeIndex = y2 * parWidth * 4 + x2 * 4;
                        Color colorPixel = Color.FromArgb(
                            byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                            byteArraryPar[parReleativeIndex]);
                        if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
                    }

                    if ((double)matchNum / sum >= matchRate) return new Point(smallStartX, smallStartY);
                }

                return subPoint;
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
        return listPoint;
    }

    public static PooledList<Point> FindPicture(Bitmap subBitmap, Bitmap parBitmap,
        Rectangle searchRect = new(), byte errorRange = 0,
        double matchRate = 0.9, bool isFindAll = true)
    {
        PooledList<Point> listPoint = [];
        int subWidth = subBitmap.Width;
        int subHeight = subBitmap.Height;
        int parWidth = parBitmap.Width;
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

        int iMax = searchLeftTop.Y + searchSize.Height - subData.Height;
        int jMax = searchLeftTop.X + searchSize.Width - subData.Width;

        for (int i = searchLeftTop.Y; i < iMax; i++)
        for (int j = searchLeftTop.X; j < jMax; j++)
        {
            int parIndex = i * parWidth * 4 + j * 4;
            Color colorBig = Color.FromArgb(byteArraryPar[parIndex + 2],
                byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);
            if (!ColorAEqualColorB(colorBig, startPixelColor, errorRange)) continue;

            int smallStartX = j;
            int smallStartY = i;
            int sum = 0;
            int matchNum = 0;
            for (int m = 0; m < subHeight; m++)
            for (int n = 0; n < subWidth; n++)
            {
                int subIndex = m * subWidth * 4 + n * 4;
                Color color = Color.FromArgb(byteArrarySub[subIndex + 2],
                    byteArrarySub[subIndex + 1], byteArrarySub[subIndex]);

                sum++;
                int x2 = smallStartX + n, y2 = smallStartY + m;
                int parReleativeIndex = y2 * parWidth * 4 + x2 * 4;
                Color colorPixel = Color.FromArgb(
                    byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                    byteArraryPar[parReleativeIndex]);
                if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
            }

            if ((double)matchNum / sum >= matchRate)
            {
                Point point = new(smallStartX, smallStartY);
                if (!listPoint.Contains(point)) listPoint.Add(point);

                if (!isFindAll) goto FIND_END;
            }
        }

        FIND_END:
        subBitmap.UnlockBits(subData);
        parBitmap.UnlockBits(parData);
        return listPoint;
    }

    #endregion

    #region 返回图片颜色数组

    public static async ValueTask<byte[]> GetBitmapByteAsync(Bitmap subBitmap)
    {
        if (subBitmap == null) return await ValueTask.FromResult(Array.Empty<byte>()).ConfigureAwait(true);

        byte[] bytes = OptimizedGraphics.BitmapToByteArray(subBitmap);

        return await ValueTask.FromResult(bytes).ConfigureAwait(true);
    }

    public static unsafe void GetBitmapByte_固定数组(in Bitmap subBitmap, ref byte[] bts)
    {
        if (subBitmap == null || bts == null) return;

        Rectangle rect = new(0, 0, subBitmap.Width, subBitmap.Height);
        BitmapData bitmapData = subBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            int bytesCount = Math.Min(bts.Length, bitmapData.Stride * bitmapData.Height);
            fixed (byte* destPtr = bts)
            {
                Buffer.MemoryCopy((void*)bitmapData.Scan0, destPtr, bytesCount, bytesCount);
            }
        }
        finally
        {
            subBitmap.UnlockBits(bitmapData);
        }
    }

    public static unsafe void GetBitmapByte_固定数组(in Bitmap subBitmap, ref 字节数组包含长宽 bts)
    {
        if (subBitmap == null || bts?.字节数组 == null) return;

        bts.图片尺寸 = subBitmap.Size;
        Rectangle rect = new(0, 0, subBitmap.Width, subBitmap.Height);
        BitmapData bitmapData = subBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        try
        {
            int bytesCount = Math.Min(bts.字节数组.Length, bitmapData.Stride * bitmapData.Height);
            fixed (byte* destPtr = bts.字节数组)
            {
                Buffer.MemoryCopy((void*)bitmapData.Scan0, destPtr, bytesCount, bytesCount);
            }
        }
        finally
        {
            subBitmap.UnlockBits(bitmapData);
        }
    }

    internal class 字节数组包含长宽
    {
        public 字节数组包含长宽(byte[] bpByts, Size bpSize)
        {
            字节数组 = bpByts;
            图片尺寸 = bpSize;
        }

        public 字节数组包含长宽()
        {
            字节数组 = [0];
            图片尺寸 = new Size();
        }

        public byte[] 字节数组 { get; set; }
        public Size 图片尺寸 { get; set; }

        public bool 数组保存为图片(string 文件路径, ImageFormat 图片格式 = null)
        {
            if (this == null || 字节数组 == null || 字节数组.Length == 0) throw new ArgumentException("数组不能为空");

            if (string.IsNullOrWhiteSpace(文件路径)) throw new ArgumentException("文件路径不能为空");

            图片格式 ??= ImageFormat.Jpeg; // 默认使用PNG格式

            int 宽度 = 图片尺寸.Width;
            int 高度 = 图片尺寸.Height;

            using Bitmap 位图 = new Bitmap(宽度, 高度, PixelFormat.Format32bppArgb);
            BitmapData 位图数据 = 位图.LockBits(new Rectangle(0, 0, 宽度, 高度),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                // 复制字节数组到位图
                Marshal.Copy(字节数组, 0, 位图数据.Scan0, 字节数组.Length);
            }
            finally
            {
                位图.UnlockBits(位图数据);
            }

            // 保存位图到文件
            位图.Save(文件路径, 图片格式);

            return true;
        }
    }

    public static byte[] GetBitmapByte(Bitmap subBitmap)
    {
        BitmapData subData = subBitmap.LockBits(new Rectangle(0, 0, subBitmap.Width, subBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        byte[] bts = new byte[subData.Stride * subData.Height];
        Marshal.Copy(subData.Scan0, bts, 0, subData.Stride * subData.Height);
        subBitmap.UnlockBits(subData);
        return bts;
    }

    #endregion

    #region 颜色对比

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rgba
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }

    [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool color_a_equal_color_b(
        in Rgba colorA,
        in Rgba colorB,
        byte errorRange
    );

    [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool color_a_equal_color_b_rgb(
        in Rgba colorA,
        in Rgba colorB,
        byte error_r,
        byte error_g,
        byte error_b
    );

    [DllImport("findpoints.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern Rgba rgba_new(byte r, byte g, byte b, byte a);

    public static bool ColorAEqualColorB(in Color colorA, in Color colorB, byte errorRange = 10)
    {
        Rgba a = rgba_new(colorA.R, colorA.G, colorA.B, colorA.A);
        Rgba b = rgba_new(colorB.R, colorB.G, colorB.B, colorB.A);
        return color_a_equal_color_b(a, b, errorRange);
    }

    public static bool ColorAEqualColorB(in Color colorA, in Color colorB, byte errorR, byte errorG, byte errorB)
    {
        Rgba a = rgba_new(colorA.R, colorA.G, colorA.B, colorA.A);
        Rgba b = rgba_new(colorB.R, colorB.G, colorB.B, colorB.A);
        return color_a_equal_color_b_rgb(a, b, errorR, errorG, errorB);
    }

    #endregion

    #region 减少图片大小对比

    public static bool RegPictrueSmall(Bitmap a, Bitmap b, double matchrate = 0.7)
    {
        return CalcSimilarDegree(GetHash(a), GetHash(b), matchrate);
    }

    private static string GetHash(Bitmap bitmap)
    {
        byte[] grayValues = ReduceColor(ReduceSize(bitmap));
        byte average = CalcAverage(grayValues);
        string reslut = ComputeBits(grayValues, average);
        return reslut;
    }

    private static Bitmap ReduceSize(Bitmap bitMap, int width = 8, int height = 8)
    {
        Image img = bitMap;
        return (Bitmap)img.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
    }

    private static byte[] ReduceColor(Bitmap bitMap)
    {
        byte[] grayValues = new byte[bitMap.Width * bitMap.Height];
        for (int x = 0; x < bitMap.Width; x++)
        for (int y = 0; y < bitMap.Height; y++)
        {
            Color color = bitMap.GetPixel(x, y);
            byte grayValue = (byte)((color.R * 30 + color.G * 59 + color.B * 11) / 100);
            grayValues[x * bitMap.Width + y] = grayValue;
        }

        return grayValues;
    }

    private static byte CalcAverage(byte[] values)
    {
        int sum = 0;
        for (int i = 0; i < values.Length; i++) sum += values[i];

        return Convert.ToByte(sum / values.Length);
    }

    private static string ComputeBits(byte[] values, byte averageValue)
    {
        char[] result = new char[values.Length];
        for (int i = 0; i < values.Length; i++) result[i] = values[i] < averageValue ? '0' : '1';

        return new string(result);
    }

    private static bool CalcSimilarDegree(string a, string b, double matchRate = 0.7)
    {
        if (a.Length != b.Length) return false;

        int count = a.Where((t, i) => t == b[i]).Count();
        return count / (double)a.Length >= matchRate;
    }

    #endregion
}