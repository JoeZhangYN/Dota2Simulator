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

    private OptimizedGraphics()
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [DllImport("gdi32.dll")]
    static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines, [Out] byte[] lpvBits, ref BITMAPINFO lpbi, uint uUsage);

    [DllImport("gdi32.dll")]
    static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

    [DllImport("gdi32.dll")]
    static extern bool DeleteObject(IntPtr hObject);

    [DllImport("gdi32.dll")]
    static extern bool DeleteDC(IntPtr hdc);

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFO
    {
        public Int32 biSize;
        public Int32 biWidth;
        public Int32 biHeight;
        public Int16 biPlanes;
        public Int16 biBitCount;
        public Int32 biCompression;
        public Int32 biSizeImage;
        public Int32 biXPelsPerMeter;
        public Int32 biYPelsPerMeter;
        public Int32 biClrUsed;
        public Int32 biClrImportant;
        public Int32 colors1;
        public Int32 colors2;
        public Int32 colors3;
        public Int32 colors4;
    }

    public void CaptureScreenToExistingBitmap(ref Bitmap bitmap, int sourceX, int sourceY)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedGraphics));

        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            IntPtr hScreenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr hMemDC = CreateCompatibleDC(hScreenDC);
            IntPtr hBitmap = CreateCompatibleBitmap(hScreenDC, bitmap.Width, bitmap.Height);
            IntPtr hOldBitmap = SelectObject(hMemDC, hBitmap);

            try
            {
                // 复制屏幕内容到内存DC
                if (!NativeMethods.BitBlt(hMemDC, 0, 0, bitmap.Width, bitmap.Height, hScreenDC, sourceX, sourceY,
                    (uint)CopyPixelOperation.SourceCopy))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // 准备 BITMAPINFO 结构
                BITMAPINFO bmi = new()
                {
                    biSize = Marshal.SizeOf(typeof(BITMAPINFO)),
                    biWidth = bitmap.Width,
                    biHeight = -bitmap.Height,  // 负值表示自顶向下的位图
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = 0,  // BI_RGB
                    biSizeImage = bitmap.Width * bitmap.Height * 4
                };

                // 创建一个字节数组来存储位图数据
                int bytes = bitmap.Width * bitmap.Height * 4;
                byte[] pixelData = new byte[bytes];

                // 使用 GetDIBits 将位图数据复制到 pixelData 数组
                if (GetDIBits(hMemDC, hBitmap, 0, (uint)bitmap.Height, pixelData, ref bmi, 0) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // 将处理后的数据复制回 bitmapData
                Marshal.Copy(pixelData, 0, bitmapData.Scan0, bytes);
            }
            finally
            {
                SelectObject(hMemDC, hOldBitmap);
                DeleteObject(hBitmap);
                DeleteDC(hMemDC);
                NativeMethods.ReleaseDC(IntPtr.Zero, hScreenDC);
            }
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
            IntPtr hScreenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr hMemDC = CreateCompatibleDC(hScreenDC);
            IntPtr hBitmap = CreateCompatibleBitmap(hScreenDC, width, height);
            IntPtr hOldBitmap = SelectObject(hMemDC, hBitmap);

            try
            {
                // 复制屏幕内容到内存DC
                if (!NativeMethods.BitBlt(hMemDC, 0, 0, width, height, hScreenDC, sourceX, sourceY,
                    (uint)CopyPixelOperation.SourceCopy))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // 准备 BITMAPINFO 结构
                BITMAPINFO bmi = new()
                {
                    biSize = Marshal.SizeOf(typeof(BITMAPINFO)),
                    biWidth = width,
                    biHeight = -height,  // 负值表示自顶向下的位图
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = 0,  // BI_RGB
                    biSizeImage = width * height * 4
                };

                // 创建一个字节数组来存储位图数据
                int bytes = bitmapData.Stride * height;
                byte[] pixelData = new byte[bytes];

                // 使用 GetDIBits 将位图数据复制到 pixelData 数组
                if (GetDIBits(hMemDC, hBitmap, 0, (uint)height, pixelData, ref bmi, 0) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // 将处理后的数据复制回 bitmapData
                Marshal.Copy(pixelData, 0, bitmapData.Scan0, bytes);
            }
            finally
            {
                SelectObject(hMemDC, hOldBitmap);
                DeleteObject(hBitmap);
                DeleteDC(hMemDC);
                NativeMethods.ReleaseDC(IntPtr.Zero, hScreenDC);
            }

            return bitmap;
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
        }
    }

    public unsafe bool CaptureScreenToBytes(int sourceX, int sourceY, int width, int height, ref byte[] bts)
    {
        if (bts.Length != width * height * 4) return false;
        if (_disposed) throw new ObjectDisposedException(nameof(OptimizedGraphics));

        IntPtr hScreenDC = NativeMethods.GetDC(IntPtr.Zero);
        IntPtr hMemDC = CreateCompatibleDC(hScreenDC);
        IntPtr hBitmap = CreateCompatibleBitmap(hScreenDC, width, height);
        IntPtr hOldBitmap = SelectObject(hMemDC, hBitmap);

        try
        {
            // 复制屏幕内容到内存DC
            if (!NativeMethods.BitBlt(hMemDC, 0, 0, width, height, hScreenDC, sourceX, sourceY,
                    (uint)CopyPixelOperation.SourceCopy))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            // 准备 BITMAPINFO 结构
            BITMAPINFO bmi = new()
            {
                biSize = Marshal.SizeOf(typeof(BITMAPINFO)),
                biWidth = width,
                biHeight = -height, // 负值表示自顶向下的位图
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0, // BI_RGB
                biSizeImage = width * height * 4
            };

            // 使用 GetDIBits 将位图数据复制到 bts 数组
            if (GetDIBits(hMemDC, hBitmap, 0, (uint)height, bts, ref bmi, 0) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        finally
        {
            SelectObject(hMemDC, hOldBitmap);
            DeleteObject(hBitmap);
            DeleteDC(hMemDC);
            NativeMethods.ReleaseDC(IntPtr.Zero, hScreenDC);
        }

        return true;
    }

    public static unsafe byte[] BitmapToByteArray(Bitmap bitmap)
    {
        if (bitmap == null)
        {
            throw new ArgumentNullException(nameof(bitmap));
        }

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
            {
                _ = Parallel.For(0, height, y =>
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < stride; x++)
                    {
                        result[rowOffset + x] = ptr[(y * bmpData.Stride) + x];
                    }
                });
            }
            else
            {
                for (int y = 0; y < height; y++)
                {
                    int rowOffset = y * stride;
                    for (int x = 0; x < stride; x++)
                    {
                        result[rowOffset + x] = ptr[(y * bmpData.Stride) + x];
                    }
                }
            }

            return result;
        }
        finally
        {
            bitmap.UnlockBits(bmpData);
        }
    }

    public IntPtr GetHdc()
    {
        return _disposed ? throw new ObjectDisposedException(nameof(OptimizedGraphics)) : s_screenDC;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            //if (s_screenDC != IntPtr.Zero)
            //{
            //    _ = NativeMethods.DeleteDC(_hdc);
            //    _hdc = IntPtr.Zero;
            //}

            _disposed = true;
        }
    }

    ~OptimizedGraphics()
    {
        Dispose(false);
    }

    public static OptimizedGraphics CreateGraphics()
    {
        return new OptimizedGraphics();
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