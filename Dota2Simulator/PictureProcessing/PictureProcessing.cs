using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Collections.Pooled;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dota2Simulator;

[StructLayout(LayoutKind.Sequential)]
struct IntTupleSize
{
    public uint x;
    public uint y;

    public static implicit operator Tuple<uint, uint>(IntTupleSize t)
    {
        return Tuple.Create(t.x, t.y);
    }

    public static implicit operator IntTupleSize(Tuple<uint, uint> t)
    {
        return new IntTupleSize { x = t.Item1, y = t.Item2 };
    }
}

/// <summary>
///     图片处理类
/// </summary>
///
///

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
        private uint x;
        private uint y;

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
        if (width + height == 0) return new Bitmap(0, 0);
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
    ///     全屏截图大约要60ms
    /// </summary>
    /// <param name="x">图片左上角X坐标</param>
    /// <param name="y">图片左上角Y坐标</param>
    /// <param name="size">图片大小</param>
    /// <returns></returns>
    public static async Task<Bitmap> CaptureScreenAsync(int x, int y, Size size)
    {
        if (size.Height + size.Width == 0) return await Task.FromResult(new Bitmap(0, 0));
        Bitmap bitmap = new(size.Width, size.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(x, y, 0, 0, bitmap.Size);
        return await Task.FromResult(bitmap);

        //SaveFileDialog dialog = new SaveFileDialog();
        //dialog.Filter = "Png Files|*.png";
        //if (dialog.ShowDialog() == DialogResult.OK) bitmap.Save(dialog.FileName, ImageFormat.Png);
    }

    #endregion

    #region 屏幕取色

    /// <summary>
    ///     屏幕截图，单操作耗时7ms（最好是一次性截图，然后多点提取） 性能过低 排除
    /// </summary>
    /// <param name="x">图片左上角X坐标</param>
    /// <param name="y">图片左上角Y坐标</param>
    /// <param name="width">图片的宽度</param>
    /// <param name="height">图片的长度</param>
    /// <returns></returns>
    public static SimpleColor CaptureColor(int x, int y)
    {
        Color c;
        using Bitmap bitmap = new(1, 1);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(x, y, 0, 0, new Size(1, 1));
        }

        c = bitmap.GetPixel(0, 0);

        return new SimpleColor(c.R, c.G, c.B);

        //SaveFileDialog dialog = new SaveFileDialog();
        //dialog.Filter = "Png Files|*.png";
        //if (dialog.ShowDialog() == DialogResult.OK) bitmap.Save(dialog.FileName, ImageFormat.Png);
    }

    #endregion

    #region 找色

    /// <summary>
    ///     单点找色 找出符合条件的颜色坐标
    /// </summary>
    /// <param name="color">要找的颜色</param>
    /// <param name="byteArraryPar">找图数组</param>
    /// <param name="parSize">图片尺寸</param>
    /// <param name="errorRange">容错范围 一般为0</param>
    /// <returns></returns>
    public static PooledList<Point> FindColor(SimpleColor color, byte[] byteArraryPar, Size parSize,
        byte errorRange = 0)
    {
        PooledList<Point> listPoint = new();
        var parWidth = parSize.Width;
        var searchRect = new Rectangle(0, 0, parSize.Width, parSize.Height);

        var searchLeftTop = searchRect.Location;
        var searchSize = searchRect.Size;

        var iMax = searchLeftTop.Y + searchSize.Height; //行
        var jMax = searchLeftTop.X + searchSize.Width; //列

        var balanceLock = new object();

        Parallel.For(searchLeftTop.X, jMax, () => new Point(), (j, loop, subPoint) =>
            {
                for (var i = searchLeftTop.Y; i < iMax; i++)
                {
                    int x = j, y = i;
                    var parIndex = i * parWidth * 3 + j * 3;
                    var colorBig = SimpleColor.FromRgb(byteArraryPar[parIndex + 2],
                        byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);

                    if (!ColorAEqualColorB(colorBig, color, errorRange)) continue;
                    Point point = new(j, i);
                    return point;
                }

                //小图x1,y1坐标处的颜色值
                return default;
            },
            x =>
            {
                lock (balanceLock)
                {
                    if (x.X != 0)
                        listPoint.Add(x);
                }
            }
        );

        return listPoint;
    }

    /// <summary>
    ///     多线程耗时11ms(第二次优化后) 同步40ms 左右，相差的不是很大，
    ///     但准确度差很多，图片因为数量多所以掉几个没问题，找色就几个，掉几个就完全不一样了
    /// </summary>
    /// <param name="colors">颜色数组</param>
    /// <param name="points">坐标数组相，对于左上角的坐标</param>
    /// <param name="byteArraryPar">找图数组</param>
    /// <param name="parSize">图片尺寸</param>
    /// <param name="errorRange">容错范围 一般为0</param>
    /// <param name="matchRate">匹配率，大概有多少颜色是匹配的</param>
    /// <returns></returns>
    public static PooledList<Point> FindColors(PooledList<SimpleColor> colors, PooledList<Point> points,
        byte[] byteArraryPar, Size parSize, byte errorRange = 0, double matchRate = 0.9)
    {
        PooledList<Point> ListPoint = new();
        var subWidth = 0;
        var subHeight = 0;
        foreach (var p in points)
        {
            if (p.X > subWidth) subWidth = p.X;
            if (p.Y > subHeight) subHeight = p.Y;
        }

        var parWidth = parSize.Width;
        //int parHeight = parBitmap.Height;
        var searchRect = new Rectangle(0, 0, parSize.Width, parSize.Height);

        var searchLeftTop = searchRect.Location;
        var searchSize = searchRect.Size;
        // 第一个颜色
        var startPixelColor = colors[0];

        var iMax = searchLeftTop.Y + searchSize.Height - subHeight; //行
        var jMax = searchLeftTop.X + searchSize.Width - subWidth; //列

        int smallOffsetX = 0, smallOffsetY = 0;
        int smallStartX, smallStartY;
        int pointX;
        int pointY;

        for (var j = searchLeftTop.X; j < jMax; j++)
        {
            for (var i = searchLeftTop.Y; i < iMax; i++)
            {
                int x = j, y = i;
                var parIndex = (i + points[0].X) * parWidth * 3 + (j + points[0].Y) * 3; // 第一个颜色坐标的颜色
                var colorBig = SimpleColor.FromRgb(byteArraryPar[parIndex + 2],
                    byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);
                if (!ColorAEqualColorB(colorBig, startPixelColor, errorRange)) continue;
                smallStartX = x - smallOffsetX; //待找的点X坐标
                smallStartY = y - smallOffsetY; //待找的点Y坐标
                var sum = 0; //所有需要比对的有效点
                var matchNum = 0; //成功匹配的点
                for (var m = 1; m < points.Count; m++)
                {
                    int x1 = points[m].X, y1 = points[m].Y;
                    var color = colors[m];
                    sum++;
                    int x2 = smallStartX + x1, y2 = smallStartY + y1;
                    var parReleativeIndex = y2 * parWidth * 3 + x2 * 3; //比对大图对应的像素点的颜色
                    var colorPixel = SimpleColor.FromRgb(
                        byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                        byteArraryPar[parReleativeIndex]);
                    if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
                }

                if (((double)matchNum / sum < matchRate)) continue;
                // Console.WriteLine((double)matchNum / sum);
                pointX = smallStartX;
                pointY = smallStartY;
                // 获取找到匹配的左上角坐标
                Point point = new(pointX, pointY);
                if (!ListPoint.Contains(point))
                    ListPoint.Add(point);
            }
        }

        return ListPoint;
    }


    #endregion

    #region 找图

    [DllImport("findpoints.dll")]
    private static extern IntTupleSize FindBytesRust(byte[] n1, UIntPtr len1, IntTupleSize t1, byte[] n2,
        UIntPtr len2, IntTupleSize t2, double matchRate);

    public static Tuple<uint, uint> FindBytesR(byte[] n1, UIntPtr len1, Tuple<uint, uint> t1, byte[] n2,
        UIntPtr len2, Tuple<uint, uint> t2, double matchRate)
    {
        return FindBytesRust(n1, len1, t1, n2, len2, t2, matchRate);
    }

    // todo: 逻辑上有问题，匹配率本身无意义
    /// <summary>
    ///     根据数组查找坐标
    /// </summary>
    /// <param name="byteArraySub">小图像数组</param>
    /// <param name="byteArrayPar">大图像数组</param>
    /// <param name="subSize">小图像尺码</param>
    /// <param name="parSize">大图像尺码</param>
    /// <param name="errorRange">误差范围</param>
    /// <param name="matchRate">匹配率</param>
    /// <returns></returns>
    public static PooledList<Point> FindBytesParallel(byte[] byteArraySub,
        Size subSize, byte[] byteArrayPar, Size parSize, byte errorRange = 0,
        double matchRate = 0.9)
    {
        PooledList<Point> listPoint = new();
        var subWidth = subSize.Width;
        var subHeight = subSize.Height;
        var parWidth = parSize.Width;
        //int parHeight = parBitmap.Height;
        var searchRect = new Rectangle(0, 0, parSize.Width, parSize.Height);

        var searchLeftTop = searchRect.Location;
        var searchSize = searchRect.Size;
        // 第一个颜色
        var startPixelColor = SimpleColor.FromRgb(byteArraySub[2],
            byteArraySub[1], byteArraySub[0]);

        var iMax = searchLeftTop.Y + searchSize.Height - subSize.Height; //行
        var jMax = searchLeftTop.X + searchSize.Width - subSize.Width; //列

        int smallOffsetX = 0, smallOffsetY = 0;
        int smallStartX, smallStartY;
        // int pointX;
        // int pointY;


        var balanceLock = new object();

        Parallel.For(searchLeftTop.X, jMax, () => new Point(), (j, loop, subPoint) =>
            {
                for (var i = searchLeftTop.Y; i < iMax; i++)
                {
                    // for (var j = searchLeftTop.X; j < jMax; j++)
                    //大图x，y坐标处的颜色值
                    int x = j, y = i;
                    var parIndex = i * parWidth * 3 + j * 3;
                    var colorBig = SimpleColor.FromRgb(byteArrayPar[parIndex + 2],
                        byteArrayPar[parIndex + 1], byteArrayPar[parIndex]);
                    if (!ColorAEqualColorB(colorBig, startPixelColor, errorRange)) continue;
                    smallStartX = x - smallOffsetX; //待找的图X坐标
                    smallStartY = y - smallOffsetY; //待找的图Y坐标
                    var sum = 0; //所有需要比对的有效点
                    var matchNum = 0; //成功匹配的点
                    for (var m = 0; m < subHeight; m++)
                    for (var n = 0; n < subWidth; n++)
                    {
                        int x1 = n, y1 = m;
                        var subIndex = m * subWidth * 3 + n * 3;
                        var color = SimpleColor.FromRgb(byteArraySub[subIndex + 2],
                            byteArraySub[subIndex + 1], byteArraySub[subIndex]);

                        sum++;
                        int x2 = smallStartX + x1, y2 = smallStartY + y1;
                        var parReleativeIndex = y2 * parWidth * 3 + x2 * 3; //比对大图对应的像素点的颜色
                        var colorPixel = SimpleColor.FromRgb(byteArrayPar[parReleativeIndex + 2],
                            byteArrayPar[parReleativeIndex + 1],
                            byteArrayPar[parReleativeIndex]);
                        if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
                    }

                    if (!((double)matchNum / sum >= matchRate)) continue;
                    // Console.WriteLine((double)matchNum / sum);
                    return new Point(smallStartX, smallStartY);
                }

                return subPoint;

                //小图x1,y1坐标处的颜色值
            },
            x =>
            {
                lock (balanceLock)
                {
                    if (x.X != 0)
                        listPoint.Add(x);
                }
            }
        );

        return listPoint;
    }

    // todo: 逻辑上有问题，匹配率本身无意义
    /// <summary>
    ///     查找图片，不能镂空
    /// </summary>
    /// <param name="subBitmap">小图像</param>
    /// <param name="parBitmap">大图像</param>
    /// <param name="searchRect">如果为empty，则默认查找整个图像</param>
    /// <param name="errorRange">容错，单个色值范围内视为正确0~255</param>
    /// <param name="matchRate">图片匹配度，默认90%</param>
    /// <returns>返回查找到的图片的左上角坐标</returns>
    public static PooledList<Point> FindPictureParallel(Bitmap subBitmap, Bitmap parBitmap,
        Rectangle searchRect = new(), byte errorRange = 0,
        double matchRate = 0.9)
    {
        PooledList<Point> listPoint = new();
        var subWidth = subBitmap.Width;
        var subHeight = subBitmap.Height;
        var parWidth = parBitmap.Width;
        //int parHeight = parBitmap.Height;
        if (searchRect.IsEmpty) searchRect = new Rectangle(0, 0, parBitmap.Width, parBitmap.Height);

        var searchLeftTop = searchRect.Location;
        var searchSize = searchRect.Size;
        var startPixelColor = SimpleColor.FromColor(subBitmap.GetPixel(0, 0));
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
        // int pointX;
        // int pointY;


        var balanceLock = new object();

        Parallel.For(searchLeftTop.X, jMax, () => new Point(), (j, loop, subPoint) =>
            {
                for (var i = searchLeftTop.Y; i < iMax; i++)
                {
                    // for (var j = searchLeftTop.X; j < jMax; j++)

                    //大图x，y坐标处的颜色值
                    int x = j, y = i;
                    var parIndex = i * parWidth * 3 + j * 3;
                    var colorBig = SimpleColor.FromRgb(byteArraryPar[parIndex + 2],
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
                        var subIndex = m * subWidth * 3 + n * 3;
                        var color = SimpleColor.FromRgb(byteArrarySub[subIndex + 2],
                            byteArrarySub[subIndex + 1], byteArrarySub[subIndex]);

                        sum++;
                        int x2 = smallStartX + x1, y2 = smallStartY + y1;
                        var parReleativeIndex = y2 * parWidth * 3 + x2 * 3; //比对大图对应的像素点的颜色
                        var colorPixel = SimpleColor.FromRgb(
                            byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                            byteArraryPar[parReleativeIndex]);
                        if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
                    }

                    if (!((double)matchNum / sum >= matchRate)) continue;
                    // Console.WriteLine((double)matchNum / sum);
                    subPoint = new Point(smallStartX, smallStartY);
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
    /// <param name="isFindAll">是否全找到</param>
    /// <returns>返回查找到的图片的左上角坐标</returns>
    public static PooledList<Point> FindPicture(Bitmap subBitmap, Bitmap parBitmap,
        Rectangle searchRect = new(), byte errorRange = 0,
        double matchRate = 0.9, bool isFindAll = true)
    {
        PooledList<Point> listPoint = new();
        var subWidth = subBitmap.Width;
        var subHeight = subBitmap.Height;
        var parWidth = parBitmap.Width;
        //int parHeight = parBitmap.Height;
        if (searchRect.IsEmpty) searchRect = new Rectangle(0, 0, parBitmap.Width, parBitmap.Height);

        var searchLeftTop = searchRect.Location;
        var searchSize = searchRect.Size;
        var startPixelColor = SimpleColor.FromColor(subBitmap.GetPixel(0, 0));
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
            var parIndex = i * parWidth * 3 + j * 3;
            var colorBig = SimpleColor.FromRgb(byteArraryPar[parIndex + 2],
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
                var subIndex = m * subWidth * 3 + n * 3;
                var color = SimpleColor.FromRgb(byteArrarySub[subIndex + 2],
                    byteArrarySub[subIndex + 1], byteArrarySub[subIndex]);

                sum++;
                int x2 = smallStartX + x1, y2 = smallStartY + y1;
                var parReleativeIndex = y2 * parWidth * 3 + x2 * 3; //比对大图对应的像素点的颜色
                var colorPixel = SimpleColor.FromRgb(
                    byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1],
                    byteArraryPar[parReleativeIndex]);
                if (ColorAEqualColorB(colorPixel, color, errorRange)) matchNum++;
            }

            if (!((double)matchNum / sum >= matchRate)) continue;
            // Console.WriteLine((double)matchNum / sum);
            pointX = smallStartX;
            pointY = smallStartY;
            Point point = new(pointX, pointY);
            if (!listPoint.Contains(point)) listPoint.Add(point);
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

    #region 返回图片颜色数组

    /// <summary>
    ///     将图片数据写入对应的内存
    /// </summary>
    /// <param name="subBitmap">图片</param>
    /// <param name="bts">数组</param>
    /// <returns></returns>
    public static async Task<byte[]> GetBitmapByteAsync(Bitmap subBitmap)
    {
        if (subBitmap == null) return await Task.FromResult(Array.Empty<byte>());

        var subData = subBitmap.LockBits(new Rectangle(0, 0, subBitmap.Width, subBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var bts = new byte[subData.Stride * subData.Height * 4];

        Marshal.Copy(subData.Scan0, bts, 0, subData.Stride * subData.Height);
        subBitmap.UnlockBits(subData);
        var bts1 = new byte[subData.Stride * subData.Height * 3];
        for (var i = 0; i < subData.Stride; i++)
        {
            for (var j = 0; j < subData.Height; j++)
            {
                var index1 = j * subData.Stride * 4 + i * 4;
                var index2 = j * subData.Stride * 3 + i * 3;
                bts1[index2 + 2] = bts[index1 + 2];
                bts1[index2 + 1] = bts[index1 + 1];
                bts1[index2] = bts[index1];
            }
        }

        return await Task.FromResult(bts1);
    }

    /// <summary>
    ///     将图片数据写入对应的内存
    /// </summary>
    /// <param name="subBitmap">图片</param>
    /// <param name="bts">数组</param>
    /// <returns></returns>
    public static byte[] GetBitmapByte(Bitmap subBitmap)
    {
        var subData = subBitmap.LockBits(new Rectangle(0, 0, subBitmap.Width, subBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var bts = new byte[subData.Stride * subData.Height * 4];
        Marshal.Copy(subData.Scan0, bts, 0, subData.Stride * subData.Height);
        subBitmap.UnlockBits(subData);
        var bts1 = new byte[subData.Stride * subData.Height * 3];
        for (var i = 0; i < subData.Stride; i++)
        {
            for (var j = 0; j < subData.Height; j++)
            {
                var index1 = j * subData.Stride * 4 + i * 4;
                var index2 = j * subData.Stride * 3 + i * 3;
                bts1[index2 + 2] = bts[index1 + 2];
                bts1[index2 + 1] = bts[index1 + 1];
                bts1[index2] = bts[index1];
            }
        }

        bts = Array.Empty<byte>();
        return bts1;
    }

    #endregion

    #region 颜色对比


    public struct SimpleColor
    {
        public byte R;
        public byte G;
        public byte B;

        public SimpleColor(byte red, byte green, byte blue)
        {
            this.R = red;
            this.G = green;
            this.B = blue;
        }

        public static SimpleColor FromRgb(byte red, byte green, byte blue)
        {
            return new SimpleColor(red, green, blue);
        }

        public static SimpleColor FromColor(Color c)
        {
            return new SimpleColor(c.R, c.G, c.B);
        }
    }


    /// <summary>
    ///     颜色对比
    /// </summary>
    /// <param name="colorA">颜色A</param>
    /// <param name="colorB">颜色B</param>
    /// <param name="errorRange">色值误差，默认值10</param>
    /// <returns></returns>
    public static bool ColorAEqualColorB(in Color colorA, in Color colorB, byte errorRange = 10)
    {
        return //Math.Abs(colorA.A - colorB.A) <= errorRange &&
            Math.Abs(colorA.R - colorB.R) <= errorRange &&
            Math.Abs(colorA.G - colorB.G) <= errorRange &&
            Math.Abs(colorA.B - colorB.B) <= errorRange;
    }

    public static bool ColorAEqualColorB(in Color colorA, in Color colorB, byte errorR, byte errorG, byte errorB)
    {
        return //Math.Abs(colorA.A - colorB.A) <= errorRange &&
            Math.Abs(colorA.R - colorB.R) <= errorR &&
            Math.Abs(colorA.G - colorB.G) <= errorG &&
            Math.Abs(colorA.B - colorB.B) <= errorB;
    }

    public static bool ColorAEqualColorB(in SimpleColor colorA, in SimpleColor colorB, byte errorRange = 10)
    {
        return //Math.Abs(colorA.A - colorB.A) <= errorRange &&
            Math.Abs(colorA.R - colorB.R) <= errorRange &&
            Math.Abs(colorA.G - colorB.G) <= errorRange &&
            Math.Abs(colorA.B - colorB.B) <= errorRange;
    }

    public static bool ColorAEqualColorB(in SimpleColor colorA, in SimpleColor colorB, byte errorR, byte errorG,
        byte errorB)
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
    private static bool ListContainsPoint(PooledList<Point> listPoint, Point point, double errorRange = 10)
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
            PixelFormat.Format32bppArgb);

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
            RGB[i + j] = (byte)gray;
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
            var srcData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                var p = (byte*)srcData.Scan0.ToPointer();
                for (var y = 0; y < h; y++)
                {
                    for (var x = 0; x < w; x++)
                    {
                        if (mode == 0) // 加权平均
                            newColor = (byte)(p[0] * 0.114f + p[1] * 0.587f + p[2] * 0.299f);
                        else // 算数平均
                            newColor = (byte)((p[0] + p[1] + p[2]) / 3.0f);
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
        return (Bitmap)img.GetThumbnailImage(width, height, () => { return false; }, IntPtr.Zero);
    }

    // Step 2 : Reduce Color
    private static byte[] ReduceColor(Bitmap bitMap)
    {
        var grayValues = new byte[bitMap.Width * bitMap.Height];
        for (var x = 0; x < bitMap.Width; x++)
        for (var y = 0; y < bitMap.Height; y++)
        {
            var color = bitMap.GetPixel(x, y);
            var grayValue = (byte)((color.R * 30 + color.G * 59 + color.B * 11) / 100);
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


#region DX捕捉并不行

///// <summary>
/////     DirectX截图
///// </summary>
//DirectXScreenCapturer _capturer = new DirectXScreenCapturer();

//// 注销捕捉
//_capturer.Dispose();

//var bitmap = new Bitmap(19220,1080);
//Image image = bitmap;
//(_, _, image) = _capturer.GetFrameImage();
//pictureBox1.BackgroundImage = image;

//using var memoryStream = tempmanager.GetStream("全局图像bts");
//image.Save(memoryStream, image.RawFormat);
//public class DirectXScreenCapturer : IDisposable
//{
//    private Factory1 factory;
//    private Adapter1 adapter;
//    private SharpDX.Direct3D11.Device device;
//    private Output output;
//    private Output1 output1;
//    private Texture2DDescription textureDesc;
//    //2D 纹理，存储截屏数据
//    private Texture2D screenTexture;

//    public DirectXScreenCapturer()
//    {
//        // 获取输出设备（显卡、显示器），这里是主显卡和主显示器
//        factory = new Factory1();
//        adapter = factory.GetAdapter1(0);
//        device = new SharpDX.Direct3D11.Device(adapter);
//        output = adapter.GetOutput(0);
//        output1 = output.QueryInterface<Output1>();

//        //设置纹理信息，供后续使用（截图大小和质量）
//        textureDesc = new Texture2DDescription
//        {
//            CpuAccessFlags = CpuAccessFlags.Read,
//            BindFlags = BindFlags.None,
//            Format = Format.R8G8B8A8_UNorm,
//            Width = output.Description.DesktopBounds.Right,
//            Height = output.Description.DesktopBounds.Bottom,
//            OptionFlags = ResourceOptionFlags.None,
//            MipLevels = 1,
//            ArraySize = 1,
//            SampleDescription = { Count = 1, Quality = 0 },
//            Usage = ResourceUsage.Staging
//        };

//        screenTexture = new Texture2D(device, textureDesc);
//    }

//    public Result ProcessFrame(Action<DataBox, Texture2DDescription> processAction, int timeoutInMilliseconds = 5)
//    {
//        //截屏，可能失败
//        using OutputDuplication duplicatedOutput = output1.DuplicateOutput(device);
//        var result = duplicatedOutput.TryAcquireNextFrame(timeoutInMilliseconds, out OutputDuplicateFrameInformation duplicateFrameInformation, out SharpDX.DXGI.Resource screenResource);

//        if (!result.Success) return result;

//        using Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>();

//        //复制数据
//        device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);
//        DataBox mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

//        processAction?.Invoke(mapSource, textureDesc);

//        //释放资源
//        device.ImmediateContext.UnmapSubresource(screenTexture, 0);
//        screenResource.Dispose();
//        duplicatedOutput.ReleaseFrame();

//        return result;
//    }

//    public (Result result, bool isBlackFrame, Image image) GetFrameImage(int timeoutInMilliseconds = 5)
//    {
//        //生成 C# 用图像
//        Bitmap image = new Bitmap(textureDesc.Width, textureDesc.Height, PixelFormat.Format32bppArgb);
//        bool isBlack = true;
//        var result = ProcessFrame(ProcessImage);

//        if (!result.Success) image.Dispose();

//        return (result, isBlack, result.Success ? image : null);

//        void ProcessImage(DataBox dataBox, Texture2DDescription texture)
//        {
//            BitmapData data = image.LockBits(new Rectangle(0, 0, texture.Width, texture.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

//            unsafe
//            {
//                byte* dataHead = (byte*)dataBox.DataPointer.ToPointer();

//                for (int x = 0; x < texture.Width; x++)
//                {
//                    for (int y = 0; y < texture.Height; y++)
//                    {
//                        byte* pixPtr = (byte*)(data.Scan0 + y * data.Stride + x * 3);

//                        int pos = x + y * texture.Width;
//                        pos *= 4;

//                        byte r = dataHead[pos + 2];
//                        byte g = dataHead[pos + 1];
//                        byte b = dataHead[pos + 0];

//                        if (isBlack && (r != 0 || g != 0 || b != 0)) isBlack = false;

//                        pixPtr[0] = b;
//                        pixPtr[1] = g;
//                        pixPtr[2] = r;
//                    }
//                }
//            }

//            image.UnlockBits(data);
//        }
//    }

//    public (Result result, bool isBlackFrame, byte[] bts) GetFrameBytes(int timeoutInMilliseconds = 5)
//    {
//        //生成 C# 用图像
//        Bitmap image = new Bitmap(textureDesc.Width, textureDesc.Height, PixelFormat.Format32bppArgb);
//        bool isBlack = true;
//        var result = ProcessFrame(ProcessImage);

//        if (!result.Success) image.Dispose();

//        return (result, isBlack, result.Success ? new byte[0] : new byte[0]);

//        void ProcessImage(DataBox dataBox, Texture2DDescription texture)
//        {
//            BitmapData data = image.LockBits(new Rectangle(0, 0, texture.Width, texture.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

//            unsafe
//            {
//                byte* dataHead = (byte*)dataBox.DataPointer.ToPointer();

//                for (int x = 0; x < texture.Width; x++)
//                {
//                    for (int y = 0; y < texture.Height; y++)
//                    {
//                        byte* pixPtr = (byte*)(data.Scan0 + y * data.Stride + x * 3);

//                        int pos = x + y * texture.Width;
//                        pos *= 4;

//                        byte r = dataHead[pos + 2];
//                        byte g = dataHead[pos + 1];
//                        byte b = dataHead[pos + 0];

//                        if (isBlack && (r != 0 || g != 0 || b != 0)) isBlack = false;

//                        pixPtr[0] = b;
//                        pixPtr[1] = g;
//                        pixPtr[2] = r;
//                    }
//                }
//            }

//            image.UnlockBits(data);
//        }
//    }

//    #region IDisposable Support
//    private bool disposedValue = false; // 要检测冗余调用

//    protected virtual void Dispose(bool disposing)
//    {
//        if (!disposedValue)
//        {
//            if (disposing)
//            {
//                // TODO: 释放托管状态(托管对象)。
//                factory.Dispose();
//                adapter.Dispose();
//                device.Dispose();
//                output.Dispose();
//                output1.Dispose();
//                screenTexture.Dispose();
//            }

//            // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
//            // TODO: 将大型字段设置为 null。
//            factory = null;
//            adapter = null;
//            device = null;
//            output = null;
//            output1 = null;
//            screenTexture = null;

//            disposedValue = true;
//        }
//    }

//    // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
//    // ~DirectXScreenCapturer()
//    // {
//    //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
//    //   Dispose(false);
//    // }

//    // 添加此代码以正确实现可处置模式。
//    public void Dispose()
//    {
//        // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
//        Dispose(true);
//        // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
//        // GC.SuppressFinalize(this);
//    }
//    #endregion
//} 
#endregion