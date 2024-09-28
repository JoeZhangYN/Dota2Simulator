using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public unsafe class OptimizedGraphics : IDisposable
{
    // 缓存 ScreenDC
    private static readonly IntPtr s_screenDC = NativeMethods.GetDC(IntPtr.Zero);
    private bool _disposed;
    private IntPtr _hdc;

    public OptimizedGraphics(IntPtr hdc)
    {
        _hdc = hdc;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    //public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
    //{
    //    if (_disposed)
    //    {
    //        throw new ObjectDisposedException(nameof(OptimizedGraphics));
    //    }

    //    if (!Enum.IsDefined(typeof(CopyPixelOperation), copyPixelOperation))
    //    {
    //        throw new InvalidEnumArgumentException(nameof(copyPixelOperation), (int)copyPixelOperation, typeof(CopyPixelOperation));
    //    }

    //    RECT rect = new RECT
    //    {
    //        left = sourceX,
    //        top = sourceY,
    //        right = sourceX + blockRegionSize.Width,
    //        bottom = sourceY + blockRegionSize.Height
    //    };

    //    if (!NativeMethods.BitBlt(
    //        _hdc,
    //        destinationX,
    //        destinationY,
    //        rect.right - rect.left,
    //        rect.bottom - rect.top,
    //        s_screenDC,
    //        rect.left,
    //        rect.top,
    //        (uint)copyPixelOperation))
    //    {
    //        throw new Win32Exception(Marshal.GetLastWin32Error());
    //    }
    //}

    public void CaptureScreenToExistingBitmap(ref Bitmap bitmap, int sourceX, int sourceY)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedGraphics));

        ArgumentNullException.ThrowIfNull(bitmap);

        BitmapData bitmapData = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            IntPtr ptr = bitmapData.Scan0;
            int width = bitmap.Width;
            int height = bitmap.Height;

            if (!NativeMethods.BitBlt(
                    _hdc,
                    0, 0,
                    width, height,
                    s_screenDC,
                    sourceX, sourceY,
                    (uint)CopyPixelOperation.SourceCopy))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            // 设置 Alpha 通道为 255（完全不透明）
            int bytes = bitmapData.Stride * height;
            byte* ptrByte = (byte*)ptr.ToPointer();
            for (int i = 3; i < bytes; i += 4) ptrByte[i] = 255;
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
        }
    }

    public unsafe Bitmap CaptureScreenToBitmap(int sourceX, int sourceY, int width, int height)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedGraphics));

        Bitmap bitmap = new(width, height, PixelFormat.Format32bppArgb);
        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            IntPtr ptr = bitmapData.Scan0;
            if (!NativeMethods.BitBlt(_hdc, 0, 0, width, height, s_screenDC, sourceX, sourceY,
                    (uint)CopyPixelOperation.SourceCopy)) throw new Win32Exception(Marshal.GetLastWin32Error());

            int bytes = bitmapData.Stride * height;
            byte* ptrByte = (byte*)ptr.ToPointer();
            for (int i = 0; i < bytes; i += 4) ptrByte[i] = 255; // Alpha channel

            return bitmap;
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
        }
    }

    public unsafe byte[] CaptureScreenToBytes(int sourceX, int sourceY, int width, int height)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedGraphics));

        int bytesPerPixel = 4;
        int byteCount = width * height * bytesPerPixel;
        byte[] bytes = new byte[byteCount];

        fixed (byte* pBytes = bytes)
        {
            IntPtr ptr = (IntPtr)pBytes;
            using (Bitmap tempBitmap = new(width, height, width * bytesPerPixel, PixelFormat.Format32bppArgb, ptr))
            {
                using (Graphics g = Graphics.FromImage(tempBitmap))
                {
                    IntPtr hdc = g.GetHdc();
                    try
                    {
                        if (!NativeMethods.BitBlt(hdc, 0, 0, width, height, s_screenDC, sourceX, sourceY,
                                (uint)CopyPixelOperation.SourceCopy))
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                    finally
                    {
                        g.ReleaseHdc(hdc);
                    }
                }
            }

            // Set alpha channel to 255
            for (int i = 3; i < byteCount; i += 4) pBytes[i] = 255;
        }

        return bytes;
    }

    public static unsafe byte[] BitmapToByteArray(Bitmap bitmap)
    {
        if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));

        int width = bitmap.Width;
        int height = bitmap.Height;
        int stride = width * 4; // 假设是32位颜色（ARGB）
        byte[] result = new byte[stride * height];

        BitmapData bmpData = bitmap.LockBits(
            new Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            byte* ptr = (byte*)bmpData.Scan0;

            // 使用并行处理来加速大图像的处理
            if (height > 64)
                _ = Parallel.For(0, height, y =>
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < stride; x++) result[rowOffset + x] = ptr[y * bmpData.Stride + x];
                });
            else
                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < stride; x++) result[rowOffset + x] = ptr[y * bmpData.Stride + x];
                }

            return result;
        }
        finally
        {
            bitmap.UnlockBits(bmpData);
        }
    }

    // 可选：添加一个方法来处理不同的像素格式
    public static unsafe byte[] BitmapToByteArray(Bitmap bitmap, PixelFormat targetFormat)
    {
        if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));

        int width = bitmap.Width;
        int height = bitmap.Height;
        int bytesPerPixel = Image.GetPixelFormatSize(targetFormat) / 8;
        int stride = width * bytesPerPixel;
        byte[] result = new byte[stride * height];

        Bitmap targetBitmap = bitmap;
        if (bitmap.PixelFormat != targetFormat)
        {
            targetBitmap = new Bitmap(width, height, targetFormat);
            using (Graphics g = Graphics.FromImage(targetBitmap))
            {
                g.DrawImage(bitmap, new Rectangle(0, 0, width, height));
            }
        }

        BitmapData bmpData = targetBitmap.LockBits(
            new Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly,
            targetFormat);

        try
        {
            byte* ptr = (byte*)bmpData.Scan0;

            if (height > 64)
                _ = Parallel.For(0, height, y =>
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < stride; x++) result[rowOffset + x] = ptr[y * bmpData.Stride + x];
                });
            else
                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < stride; x++) result[rowOffset + x] = ptr[y * bmpData.Stride + x];
                }

            return result;
        }
        finally
        {
            targetBitmap.UnlockBits(bmpData);
            if (targetBitmap != bitmap) targetBitmap.Dispose();
        }
    }

    public IntPtr GetHdc()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedGraphics));

        return _hdc;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (_hdc != IntPtr.Zero)
            {
                _ = NativeMethods.DeleteDC(_hdc);
                _hdc = IntPtr.Zero;
            }

            _disposed = true;
        }
    }

    ~OptimizedGraphics()
    {
        Dispose(false);
    }

    public static OptimizedGraphics FromHdc(IntPtr hdc)
    {
        return new OptimizedGraphics(hdc);
    }

    public static OptimizedGraphics CreateGraphics()
    {
        IntPtr hdc = NativeMethods.CreateCompatibleDC(IntPtr.Zero);
        if (hdc == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
        return new OptimizedGraphics(hdc);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    private static class NativeMethods
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
            int nXSrc, int nYSrc, uint dwRop);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}