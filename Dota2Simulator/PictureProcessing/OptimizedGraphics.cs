using ImageProcessingSystem;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

public static class OptimizedGraphics
{
    private static readonly Lock _initLock = new();
    private static CaptureMethod _preferredMethod = CaptureMethod.Unknown;

    private static DesktopDuplicationProvider _desktopDuplicationProvider;
    private static readonly Lock _providerLock = new();

    // 缓存的设备上下文
    private static readonly ThreadLocal<DeviceContextCache> _dcCache =
        new ThreadLocal<DeviceContextCache>(() => new DeviceContextCache(), true);

    // 截图专用的图像句柄缓存
    private static readonly ConcurrentDictionary<string, ImageHandle> _screenCaptureHandles = new();

    public enum CaptureMethod
    {
        Unknown,
        WindowsGraphicsCapture,  // Windows 10 1903+ (最快) 
        // 资源上 1080p 时和Desktop Duplication差不多 延时也接近
        // 4K 时 延时是 Desktop Duplication的一倍 资源稍微少消耗一点
        DesktopDuplication,      // Windows 8+ (次快)
        BitBlt,                  // Windows Vista+ (较快)
        GdiPlus                  // 所有Windows版本 (最慢但最兼容)
    }

    #region 公共API

    /// <summary>
    /// 初始化捕获屏幕区域到新的ImageHandle,并缓存到动态图像
    /// </summary>
    public static ImageHandle CaptureScreen(int x, int y, int width, int height, string? name = null)
    {
        InitializePreferredMethod();

        // 创建图像句柄
        var imageData = new byte[width * height * 4];
        var handle = ImageManager.CreateDynamicImage(imageData, new Size(width, height), name ?? $"Screen_{DateTime.Now.Ticks}");

        // 捕获屏幕到句柄
        if (!CaptureScreenToHandle(handle, x, y))
        {
            ImageManager.ReleaseImage(handle);
            return ImageHandle.Invalid;
        }

        return handle;
    }

    /// <summary>
    /// 根据已存在的ImageHandle捕获屏幕
    /// </summary>
    public static bool CaptureScreenToHandle(in ImageHandle targetHandle, int sourceX, int sourceY)
    {
        if (!targetHandle.IsValid)
            throw new ArgumentException("无效的图像句柄", nameof(targetHandle));

        InitializePreferredMethod();

        // 获取目标图像的数据指针
        var (ptr, length, size) = ImageManager.GetImageData(targetHandle);

        switch (_preferredMethod)
        {
            case CaptureMethod.WindowsGraphicsCapture:
                return CaptureUsingWindowsGraphicsToPtr(sourceX, sourceY, (int)size.x, (int)size.y, ptr);

            case CaptureMethod.DesktopDuplication:
                return CaptureUsingDesktopDuplicationToPtr(sourceX, sourceY, (int)size.x, (int)size.y, ptr);

            case CaptureMethod.BitBlt:
                return CaptureUsingBitBltToPtr(sourceX, sourceY, (int)size.x, (int)size.y, ptr);

            default:
                // 对于其他方法，先捕获到Bitmap再转换
                using (var bitmap = CaptureUsingGdiPlus(sourceX, sourceY, (int)size.x, (int)size.y))
                {
                    CopyBitmapToPtr(bitmap, ptr);
                }
                return true;
        }
    }

    /// <summary>
    /// 异步捕获屏幕
    /// </summary>
    public static Task<ImageHandle> CaptureScreenAsync(int x, int y, int width, int height, string? name = null)
    {
        return Task.Run(() => CaptureScreen(x, y, width, height, name));
    }

    /// <summary>
    /// 获取或创建屏幕捕获句柄（用于连续捕获）
    /// </summary>
    public static ImageHandle GetOrCreateScreenCaptureHandle(string name, int width, int height)
    {
        return _screenCaptureHandles.GetOrAdd(name, _ =>
        {
            var data = new byte[width * height * 4];
            var handle = ImageManager.CreateDynamicImage(data, new Size(width, height), name);
            return handle;
        });
    }

    public static void SetPreferredMethod(CaptureMethod method)
    {
        _preferredMethod = method;
    }

    /// <summary>
    /// 释放屏幕捕获句柄
    /// </summary>
    public static void ReleaseScreenCaptureHandle(string name)
    {
        if (_screenCaptureHandles.TryRemove(name, out var handle))
        {
            ImageManager.ReleaseImage(handle);
        }
    }

    #endregion

    #region 性能优化的连续捕获API

    /// <summary>
    /// 高性能连续屏幕捕获
    /// </summary>
    public class ContinuousScreenCapture : IDisposable
    {
        private readonly ImageHandle _handle;
        private readonly int _x, _y;
        private readonly CancellationTokenSource _cts = new();
        private Task _captureTask;

        public ImageHandle Handle => _handle;
        public event Action<ImageHandle> OnFrameCaptured;

        public ContinuousScreenCapture(int x, int y, int width, int height, string name = null)
        {
            _x = x;
            _y = y;
            _handle = GetOrCreateScreenCaptureHandle(name ?? $"Continuous_{x}_{y}_{width}x{height}", width, height);
        }

        public void Start(int intervalMs = 4) // 默认240fps
        {
            _captureTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (CaptureScreenToHandle(_handle, _x, _y))
                    {
                        OnFrameCaptured?.Invoke(_handle);
                    }

                    await Task.Delay(intervalMs, _cts.Token);
                }
            }, _cts.Token);
        }

        public void Stop()
        {
            _cts.Cancel();
            _captureTask?.Wait(1000);
        }

        public void Dispose()
        {
            Stop();
            _cts.Dispose();
        }
    }

    #endregion

    #region 初始化和配置

    private static void InitializePreferredMethod()
    {
        if (_preferredMethod != CaptureMethod.Unknown)
            return;

        lock (_initLock)
        {
            if (_preferredMethod != CaptureMethod.Unknown)
                return;

            // 检测可用的最佳方法
            if (IsWindowsGraphicsCaptureAvailable())
            {
                _preferredMethod = CaptureMethod.WindowsGraphicsCapture;
            }
            else if (IsDesktopDuplicationAvailable())
            {
                _preferredMethod = CaptureMethod.DesktopDuplication;
            }
            else if (IsBitBltAvailable())
            {
                _preferredMethod = CaptureMethod.BitBlt;
            }
            else
            {
                _preferredMethod = CaptureMethod.GdiPlus;
            }
        }
    }

    private static bool IsWindowsGraphicsCaptureAvailable()
    {
        try
        {
            // Windows 10 版本 1903 (10.0.18362) 或更高
            var version = Environment.OSVersion.Version;
            return version.Major > 10 || (version.Major == 10 && version.Build >= 18362);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsDesktopDuplicationAvailable()
    {
        try
        {
            // Windows 8 或更高
            var version = Environment.OSVersion.Version;
            return version.Major > 6 || (version.Major == 6 && version.Minor >= 2);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsBitBltAvailable()
    {
        try
        {
            IntPtr hdc = NativeMethods.GetDC(IntPtr.Zero);
            if (hdc != IntPtr.Zero)
            {
                _ = NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Windows Graphics Capture 实现（直接到内存）

    private static bool CaptureUsingWindowsGraphicsToPtr(int x, int y, int width, int height, IntPtr targetPtr)
    {
        // Windows Graphics Capture 的简化实现
        // 由于 Windows Graphics Capture API 需要复杂的 WinRT 互操作，
        // UWP有可以直接实现的API，但这不是UWP 直接降级了
        return CaptureUsingDesktopDuplicationToPtr(x, y, width, height, targetPtr);
    }

    #endregion

    #region Desktop Duplication 实现（直接到内存）

    private static bool CaptureUsingDesktopDuplicationToPtr(int x, int y, int width, int height, IntPtr targetPtr)
    {
        try
        {
            if (_desktopDuplicationProvider == null)
            {
                lock (_providerLock)
                {
                    if (_desktopDuplicationProvider == null)
                    {
                        var provider = new DesktopDuplicationProvider();
                        if (provider.Initialize())
                        {
                            _desktopDuplicationProvider = provider;
                        }
                        else
                        {
                            provider.Dispose();
                            _preferredMethod = CaptureMethod.BitBlt;
                            return CaptureUsingBitBltToPtr(x, y, width, height, targetPtr);
                        }
                    }
                }
            }

            return _desktopDuplicationProvider.CaptureScreenToPtr(x, y, width, height, targetPtr);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Desktop Duplication capture failed: {ex.Message}");

            lock (_providerLock)
            {
                _desktopDuplicationProvider?.Dispose();
                _desktopDuplicationProvider = null;
                _preferredMethod = CaptureMethod.BitBlt;
            }

            return CaptureUsingBitBltToPtr(x, y, width, height, targetPtr);
        }
    }

    #endregion

    #region BitBlt 实现（直接到内存）

    private static unsafe bool CaptureUsingBitBltToPtr(int x, int y, int width, int height, IntPtr targetPtr)
    {
        var cache = _dcCache.Value;

        IntPtr hScreenDC = cache.GetScreenDC();
        IntPtr hMemDC = cache.GetMemoryDC();
        IntPtr hBitmap = cache.GetCompatibleBitmap(hScreenDC, width, height);
        IntPtr hOldBitmap = NativeMethods.SelectObject(hMemDC, hBitmap);

        try
        {
            if (!NativeMethods.BitBlt(hMemDC, 0, 0, width, height,
                hScreenDC, x, y, NativeMethods.SRCCOPY))
            {
                return false;
            }

            var bmi = new BITMAPINFO
            {
                biSize = Marshal.SizeOf<BITMAPINFO>(),
                biWidth = width,
                biHeight = -height,  // 保持负高度，表示自上而下存储
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0,
                biSizeImage = width * height * 4
            };

            // 获取位图数据
            if (NativeMethods.GetDIBits(hMemDC, hBitmap, 0, (uint)height,
                targetPtr, ref bmi, 0) == 0)
            {
                return false;
            }

            return true;
        }
        finally
        {
            NativeMethods.SelectObject(hMemDC, hOldBitmap);
        }
    }

    #region 辅助方法

    private static unsafe void CopyBitmapToPtr(Bitmap bitmap, IntPtr targetPtr)
    {
        BitmapData bmpData = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            byte* src = (byte*)bmpData.Scan0;
            byte* dst = (byte*)targetPtr;
            int rowBytes = bitmap.Width * 4;

            if (bmpData.Stride == rowBytes)
            {
                // 连续内存，一次性复制
                Buffer.MemoryCopy(src, dst, rowBytes * bitmap.Height, rowBytes * bitmap.Height);
            }
            else
            {
                // 逐行复制
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Buffer.MemoryCopy(
                        src + y * bmpData.Stride,
                        dst + y * rowBytes,
                        rowBytes,
                        rowBytes
                    );
                }
            }
        }
        finally
        {
            bitmap.UnlockBits(bmpData);
        }
    }

    #endregion

    #endregion

    #region 辅助方法

    public static unsafe void BitmapToByteArray(Bitmap bitmap, byte[] buffer)
    {
        BitmapData bmpData = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            int bytes = bitmap.Width * bitmap.Height * 4;
            if (bmpData.Stride == bitmap.Width * 4)
            {
                // 连续内存，一次性复制
                Marshal.Copy(bmpData.Scan0, buffer, 0, bytes);
            }
            else
            {
                // 需要逐行复制
                byte* ptr = (byte*)bmpData.Scan0;
                int rowBytes = bitmap.Width * 4;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    Marshal.Copy(new IntPtr(ptr + y * bmpData.Stride),
                        buffer, y * rowBytes, rowBytes);
                }
            }
        }
        finally
        {
            bitmap.UnlockBits(bmpData);
        }
    }

    public static unsafe byte[] BitmapToByteArray(Bitmap bitmap)
    {
        var buffer = new byte[bitmap.Width * bitmap.Height * 4];

        BitmapData bmpData = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            int bytes = bitmap.Width * bitmap.Height * 4;
            if (bmpData.Stride == bitmap.Width * 4)
            {
                // 连续内存，一次性复制
                Marshal.Copy(bmpData.Scan0, buffer, 0, bytes);
            }
            else
            {
                // 需要逐行复制
                byte* ptr = (byte*)bmpData.Scan0;
                int rowBytes = bitmap.Width * 4;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    Marshal.Copy(new IntPtr(ptr + y * bmpData.Stride),
                        buffer, y * rowBytes, rowBytes);
                }
            }
        }
        finally
        {
            bitmap.UnlockBits(bmpData);
        }

        return buffer;
    }

    #endregion

    #region 设备上下文缓存

    private class DeviceContextCache : IDisposable
    {
        private IntPtr _screenDC;
        private IntPtr _memoryDC;
        private readonly ConcurrentDictionary<long, IntPtr> _bitmapCache = new();
        private readonly Lock _lock = new();

        public IntPtr GetScreenDC()
        {
            if (_screenDC == IntPtr.Zero)
            {
                lock (_lock)
                {
                    if (_screenDC == IntPtr.Zero)
                    {
                        _screenDC = NativeMethods.GetDC(IntPtr.Zero);
                    }
                }
            }
            return _screenDC;
        }

        public IntPtr GetMemoryDC()
        {
            if (_memoryDC == IntPtr.Zero)
            {
                lock (_lock)
                {
                    if (_memoryDC == IntPtr.Zero)
                    {
                        _memoryDC = NativeMethods.CreateCompatibleDC(GetScreenDC());
                    }
                }
            }
            return _memoryDC;
        }

        public IntPtr GetCompatibleBitmap(IntPtr hdc, int width, int height)
        {
            // 缓存常用尺寸的位图
            long key = ((long)width << 32) | (uint)height;

            if (_bitmapCache.TryGetValue(key, out IntPtr cached))
            {
                return cached;
            }

            IntPtr hBitmap = NativeMethods.CreateCompatibleBitmap(hdc, width, height);

            // 只缓存小于 10 个位图，避免内存占用过大
            if (_bitmapCache.Count < 10)
            {
                _bitmapCache.TryAdd(key, hBitmap);
            }

            return hBitmap;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var bitmap in _bitmapCache.Values)
                {
                    NativeMethods.DeleteObject(bitmap);
                }
                _bitmapCache.Clear();

                if (_memoryDC != IntPtr.Zero)
                {
                    NativeMethods.DeleteDC(_memoryDC);
                    _memoryDC = IntPtr.Zero;
                }

                if (_screenDC != IntPtr.Zero)
                {
                    _ = NativeMethods.ReleaseDC(IntPtr.Zero, _screenDC);
                    _screenDC = IntPtr.Zero;
                }
            }
        }

        ~DeviceContextCache()
        {
            Dispose();
        }
    }

    #endregion

    #region Native Methods

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFO
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public uint[] colors;
    }

    private static class NativeMethods
    {
        public const uint SRCCOPY = 0x00CC0020;

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll")]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan,
            uint cScanLines, IntPtr lpvBits, ref BITMAPINFO lpbi, uint uUsage);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }

    #endregion

    #region 清理资源

    static OptimizedGraphics()
    {
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
    }

    private static void OnProcessExit(object sender, EventArgs e)
    {
        lock (_providerLock)
        {
            _desktopDuplicationProvider?.Dispose();
        }

        foreach (var cache in _dcCache.Values)
        {
            cache?.Dispose();
        }
    }

    #endregion

    #region 兼容性方法（保留部分原有API）

    /// <summary>
    /// 捕获屏幕到Bitmap（兼容旧代码）
    /// </summary>
    [Obsolete("请使用返回ImageHandle的新方法")]
    public static Bitmap CaptureScreenToBitmap(int x, int y, int width, int height)
    {
        var handle = CaptureScreen(x, y, width, height);
        try
        {
            // 从句柄导出为Bitmap
            var tempFile = System.IO.Path.GetTempFileName();
            ImageManager.SaveImage(in handle, tempFile);
            var bitmap = new Bitmap(tempFile);
            System.IO.File.Delete(tempFile);
            return bitmap;
        }
        finally
        {
            ImageManager.ReleaseImage(handle);
        }
    }

    #region Windows Graphics Capture 实现

    private static Bitmap CaptureUsingWindowsGraphicsCapture(int x, int y, int width, int height)
    {
        // Windows Graphics Capture 的简化实现
        // 由于 Windows Graphics Capture API 需要复杂的 WinRT 互操作，
        // UWP有可以直接实现的API，但这不是UWP 直接降级了
        return CaptureUsingDesktopDuplication(x, y, width, height);
    }

    #endregion

    #region Desktop Duplication 实现

    private static Bitmap CaptureUsingDesktopDuplication(int x, int y, int width, int height)
    {
        try
        {
            if (_desktopDuplicationProvider == null)
            {
                lock (_providerLock)
                {
                    if (_desktopDuplicationProvider == null)
                    {
                        var provider = new DesktopDuplicationProvider();
                        if (provider.Initialize())
                        {
                            _desktopDuplicationProvider = provider;
                        }
                        else
                        {
                            provider.Dispose();
                            _preferredMethod = CaptureMethod.BitBlt;
                            return CaptureUsingBitBlt(x, y, width, height);
                        }
                    }
                }
            }

            return _desktopDuplicationProvider.CaptureScreen(x, y, width, height);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Desktop Duplication capture failed: {ex.Message}");

            lock (_providerLock)
            {
                _desktopDuplicationProvider?.Dispose();
                _desktopDuplicationProvider = null;
                _preferredMethod = CaptureMethod.BitBlt;
            }

            return CaptureUsingBitBlt(x, y, width, height);
        }
    }

    #endregion

    #region BitBlt 实现（优化版）

    private static Bitmap CaptureUsingBitBlt(int x, int y, int width, int height)
    {
        var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        CaptureUsingBitBltToExisting(bitmap, x, y);
        return bitmap;
    }

    private static unsafe void CaptureUsingBitBltToExisting(Bitmap bitmap, int sourceX, int sourceY)
    {
        var cache = _dcCache.Value;
        BitmapData bitmapData = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            IntPtr hScreenDC = cache.GetScreenDC();
            IntPtr hMemDC = cache.GetMemoryDC();
            IntPtr hBitmap = cache.GetCompatibleBitmap(hScreenDC, bitmap.Width, bitmap.Height);
            IntPtr hOldBitmap = NativeMethods.SelectObject(hMemDC, hBitmap);

            try
            {
                if (!NativeMethods.BitBlt(hMemDC, 0, 0, bitmap.Width, bitmap.Height,
                    hScreenDC, sourceX, sourceY, NativeMethods.SRCCOPY))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                var bmi = new BITMAPINFO
                {
                    biSize = Marshal.SizeOf<BITMAPINFO>(),
                    biWidth = bitmap.Width,
                    biHeight = -bitmap.Height,
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = 0,
                    biSizeImage = bitmap.Width * bitmap.Height * 4
                };

                // 直接写入目标内存
                if (NativeMethods.GetDIBits(hMemDC, hBitmap, 0, (uint)bitmap.Height,
                    bitmapData.Scan0, ref bmi, 0) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                NativeMethods.SelectObject(hMemDC, hOldBitmap);
            }
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
        }
    }

    private static unsafe bool CaptureUsingBitBltToBytes(int x, int y, int width, int height, byte[] buffer)
    {
        var cache = _dcCache.Value;

        fixed (byte* pBuffer = buffer)
        {
            IntPtr hScreenDC = cache.GetScreenDC();
            IntPtr hMemDC = cache.GetMemoryDC();
            IntPtr hBitmap = cache.GetCompatibleBitmap(hScreenDC, width, height);
            IntPtr hOldBitmap = NativeMethods.SelectObject(hMemDC, hBitmap);

            try
            {
                if (!NativeMethods.BitBlt(hMemDC, 0, 0, width, height,
                    hScreenDC, x, y, NativeMethods.SRCCOPY))
                {
                    return false;
                }

                var bmi = new BITMAPINFO
                {
                    biSize = Marshal.SizeOf<BITMAPINFO>(),
                    biWidth = width,
                    biHeight = -height,
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = 0,
                    biSizeImage = width * height * 4
                };

                return NativeMethods.GetDIBits(hMemDC, hBitmap, 0, (uint)height,
                    new IntPtr(pBuffer), ref bmi, 0) != 0;
            }
            finally
            {
                NativeMethods.SelectObject(hMemDC, hOldBitmap);
            }
        }
    }

    #endregion

    #region GDI+ 实现（备用）

    private static Bitmap CaptureUsingGdiPlus(int x, int y, int width, int height)
    {
        var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
        }
        return bitmap;
    }

    #endregion

    #endregion
}