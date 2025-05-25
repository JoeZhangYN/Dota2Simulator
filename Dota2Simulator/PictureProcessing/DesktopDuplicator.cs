// Desktop Duplication API 实现 (Windows 8+)
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using Buffer = System.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

public class DesktopDuplicationProvider : IDisposable
{
    private SharpDX.Direct3D11.Device _device;
    private Texture2D _stagingTexture;
    private OutputDuplication _duplication;
    private Output1 _output;
    private Texture2D _desktopImageTexture;
    private bool _disposed;
    private readonly Lock _lock = new();
    private DateTime _lastFrameTime = DateTime.MinValue;

    public bool Initialize()
    {
        try
        {
            // 创建 D3D11 设备
            _device = new Device(
                SharpDX.Direct3D.DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                new[] {
                    SharpDX.Direct3D.FeatureLevel.Level_11_1,
                    SharpDX.Direct3D.FeatureLevel.Level_11_0,
                    SharpDX.Direct3D.FeatureLevel.Level_10_1,
                    SharpDX.Direct3D.FeatureLevel.Level_10_0
                });

            // 获取 DXGI 设备和适配器
            using (var dxgiDevice = _device.QueryInterface<SharpDX.DXGI.Device>())
            using (var adapter = dxgiDevice.GetParent<Adapter>())
            {
                // 获取第一个输出（主显示器）
                _output = adapter.GetOutput(0).QueryInterface<Output1>();

                // 创建输出复制
                _duplication = _output.DuplicateOutput(_device);
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Desktop Duplication initialization failed: {ex.Message}");
            Dispose();
            return false;
        }
    }

    public Bitmap CaptureScreen(int x, int y, int width, int height)
    {
        if (_disposed || _duplication == null)
            throw new InvalidOperationException("Desktop duplication not initialized");

        lock (_lock)
        {
            SharpDX.DXGI.Resource? screenResource = null;
            OutputDuplicateFrameInformation frameInfo;

            try
            {
                // 尝试获取下一帧
                var result = _duplication.TryAcquireNextFrame(0, out frameInfo, out screenResource);

                if (result.Failure)
                {
                    // 如果没有新帧，使用缓存的桌面图像
                    if (_desktopImageTexture != null &&
                        (DateTime.Now - _lastFrameTime).TotalMilliseconds < 100)
                    {
                        return ExtractBitmap(_desktopImageTexture, x, y, width, height);
                    }

                    // 重新创建复制
                    RecreateDuplication();
                    result = _duplication.TryAcquireNextFrame(100, out frameInfo, out screenResource);

                    if (result.Failure || screenResource == null)
                        throw new InvalidOperationException("Failed to acquire frame");
                }

                using (var screenTexture = screenResource.QueryInterface<Texture2D>())
                {
                    // 缓存桌面图像
                    CacheDesktopImage(screenTexture);
                    _lastFrameTime = DateTime.Now;

                    return ExtractBitmap(screenTexture, x, y, width, height);
                }
            }
            finally
            {
                screenResource?.Dispose();

                try
                {
                    _duplication?.ReleaseFrame();
                }
                catch { }
            }
        }
    }

    public bool CaptureScreenToPtr(int x, int y, int width, int height, IntPtr targetPtr)
    {
        if (_disposed || _duplication == null)
            throw new InvalidOperationException("Desktop duplication not initialized");

        lock (_lock)
        {
            SharpDX.DXGI.Resource? screenResource = null;
            OutputDuplicateFrameInformation frameInfo;

            try
            {
                // 尝试获取下一帧
                var result = _duplication.TryAcquireNextFrame(0, out frameInfo, out screenResource);

                if (result.Failure)
                {
                    // 如果没有新帧，使用缓存的桌面图像
                    if (_desktopImageTexture != null &&
                        (DateTime.Now - _lastFrameTime).TotalMilliseconds < 100)
                    {
                        return ExtractToPtr(_desktopImageTexture, x, y, width, height, targetPtr);
                    }

                    // 重新创建复制
                    RecreateDuplication();
                    result = _duplication.TryAcquireNextFrame(100, out frameInfo, out screenResource);

                    if (result.Failure || screenResource == null)
                        return false;
                }

                using (var screenTexture = screenResource.QueryInterface<Texture2D>())
                {
                    // 缓存桌面图像
                    CacheDesktopImage(screenTexture);
                    _lastFrameTime = DateTime.Now;

                    return ExtractToPtr(screenTexture, x, y, width, height, targetPtr);
                }
            }
            finally
            {
                screenResource?.Dispose();

                try
                {
                    _duplication?.ReleaseFrame();
                }
                catch { }
            }
        }
    }

    // 新增：直接提取到内存指针的方法
    private unsafe bool ExtractToPtr(Texture2D source, int x, int y, int width, int height, IntPtr targetPtr)
    {
        var desc = source.Description;

        // 创建一个可以被CPU访问的暂存纹理
        var stagingDesc = new Texture2DDescription
        {
            Width = desc.Width,
            Height = desc.Height,
            MipLevels = 1,
            ArraySize = 1,
            Format = desc.Format,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Staging,
            BindFlags = BindFlags.None,
            CpuAccessFlags = CpuAccessFlags.Read,
            OptionFlags = ResourceOptionFlags.None
        };

        using (var stagingTexture = new Texture2D(_device, stagingDesc))
        {
            // 复制纹理到暂存纹理
            _device.ImmediateContext.CopyResource(source, stagingTexture);

            // 映射暂存纹理以读取数据
            var dataBox = _device.ImmediateContext.MapSubresource(
                stagingTexture, 0, MapMode.Read, MapFlags.None);

            try
            {
                byte* srcPtr = (byte*)dataBox.DataPointer;
                byte* dstPtr = (byte*)targetPtr;

                int srcPitch = dataBox.RowPitch;
                int dstPitch = width * 4; // BGRA格式，每像素4字节

                // 验证边界
                if (x < 0 || y < 0 || x + width > desc.Width || y + height > desc.Height)
                    return false;

                // 逐行复制数据
                for (int row = 0; row < height; row++)
                {
                    byte* srcRow = srcPtr + ((y + row) * srcPitch) + (x * 4);
                    byte* dstRow = dstPtr + (row * dstPitch);

                    Buffer.MemoryCopy(srcRow, dstRow, dstPitch, dstPitch);
                }

                return true;
            }
            finally
            {
                _device.ImmediateContext.UnmapSubresource(stagingTexture, 0);
            }
        }
    }

    // 优化版本：如果需要捕获整个屏幕，可以使用更高效的方法
    public unsafe bool CaptureFullScreenToPtr(IntPtr targetPtr)
    {
        if (_disposed || _duplication == null)
            throw new InvalidOperationException("Desktop duplication not initialized");

        lock (_lock)
        {
            SharpDX.DXGI.Resource? screenResource = null;
            OutputDuplicateFrameInformation frameInfo;

            try
            {
                var result = _duplication.TryAcquireNextFrame(0, out frameInfo, out screenResource);

                if (result.Failure)
                {
                    if (_desktopImageTexture != null &&
                        (DateTime.Now - _lastFrameTime).TotalMilliseconds < 100)
                    {
                        return CopyFullTextureToPtr(_desktopImageTexture, targetPtr);
                    }

                    RecreateDuplication();
                    result = _duplication.TryAcquireNextFrame(100, out frameInfo, out screenResource);

                    if (result.Failure || screenResource == null)
                        return false;
                }

                using (var screenTexture = screenResource.QueryInterface<Texture2D>())
                {
                    CacheDesktopImage(screenTexture);
                    _lastFrameTime = DateTime.Now;

                    return CopyFullTextureToPtr(screenTexture, targetPtr);
                }
            }
            finally
            {
                screenResource?.Dispose();

                try
                {
                    _duplication?.ReleaseFrame();
                }
                catch { }
            }
        }
    }

    // 辅助方法：复制整个纹理到内存
    private unsafe bool CopyFullTextureToPtr(Texture2D source, IntPtr targetPtr)
    {
        var desc = source.Description;

        // 如果源纹理已经是可读的，直接映射
        if (desc.Usage == ResourceUsage.Staging &&
            desc.CpuAccessFlags.HasFlag(CpuAccessFlags.Read))
        {
            var dataBox = _device.ImmediateContext.MapSubresource(
                source, 0, MapMode.Read, MapFlags.None);

            try
            {
                byte* srcPtr = (byte*)dataBox.DataPointer;
                byte* dstPtr = (byte*)targetPtr;

                int srcPitch = dataBox.RowPitch;
                int dstPitch = desc.Width * 4;
                int copySize = dstPitch;

                // 如果行间距相同，可以一次性复制
                if (srcPitch == dstPitch)
                {
                    Buffer.MemoryCopy(srcPtr, dstPtr,
                        (long)dstPitch * desc.Height,
                        (long)dstPitch * desc.Height);
                }
                else
                {
                    // 逐行复制
                    for (int y = 0; y < desc.Height; y++)
                    {
                        Buffer.MemoryCopy(
                            srcPtr + y * srcPitch,
                            dstPtr + y * dstPitch,
                            copySize,
                            copySize);
                    }
                }

                return true;
            }
            finally
            {
                _device.ImmediateContext.UnmapSubresource(source, 0);
            }
        }
        else
        {
            // 需要创建暂存纹理
            return ExtractToPtr(source, 0, 0, desc.Width, desc.Height, targetPtr);
        }
    }

    private void CacheDesktopImage(Texture2D sourceTexture)
    {
        var desc = sourceTexture.Description;

        if (_desktopImageTexture == null ||
            _desktopImageTexture.Description.Width != desc.Width ||
            _desktopImageTexture.Description.Height != desc.Height)
        {
            _desktopImageTexture?.Dispose();

            var cacheDesc = new Texture2DDescription
            {
                Width = desc.Width,
                Height = desc.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = desc.Format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            _desktopImageTexture = new Texture2D(_device, cacheDesc);
        }

        _device.ImmediateContext.CopyResource(sourceTexture, _desktopImageTexture);
    }

    private unsafe Bitmap ExtractBitmap(Texture2D sourceTexture, int x, int y, int width, int height)
    {
        var desc = sourceTexture.Description;

        // 创建或重用暂存纹理
        if (_stagingTexture == null ||
            _stagingTexture.Description.Width < width ||
            _stagingTexture.Description.Height < height)
        {
            _stagingTexture?.Dispose();

            var stagingDesc = new Texture2DDescription
            {
                Width = Math.Max(width, desc.Width),
                Height = Math.Max(height, desc.Height),
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.None
            };

            _stagingTexture = new Texture2D(_device, stagingDesc);
        }

        // 复制指定区域
        var sourceRegion = new ResourceRegion
        {
            Left = Math.Max(0, x),
            Top = Math.Max(0, y),
            Right = Math.Min(desc.Width, x + width),
            Bottom = Math.Min(desc.Height, y + height),
            Front = 0,
            Back = 1
        };

        _device.ImmediateContext.CopySubresourceRegion(
            sourceTexture, 0, sourceRegion,
            _stagingTexture, 0, 0, 0, 0);

        // 创建位图
        var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        var bitmapData = bitmap.LockBits(
            new Rectangle(0, 0, width, height),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);

        try
        {
            var dataBox = _device.ImmediateContext.MapSubresource(
                _stagingTexture, 0, MapMode.Read, MapFlags.None);

            try
            {
                int actualWidth = sourceRegion.Right - sourceRegion.Left;
                int actualHeight = sourceRegion.Bottom - sourceRegion.Top;

                byte* src = (byte*)dataBox.DataPointer;
                byte* dst = (byte*)bitmapData.Scan0;

                // 复制数据
                for (int row = 0; row < actualHeight; row++)
                {
                    byte* srcRow = src + (row * dataBox.RowPitch);
                    byte* dstRow = dst + (row * bitmapData.Stride);

                    Buffer.MemoryCopy(srcRow, dstRow, bitmapData.Stride, actualWidth * 4);
                }

                // 填充剩余区域（如果截图区域超出屏幕）
                if (actualHeight < height)
                {
                    for (int row = actualHeight; row < height; row++)
                    {
                        byte* dstRow = dst + (row * bitmapData.Stride);
                        new Span<byte>(dstRow, width * 4).Clear();
                    }
                }
            }
            finally
            {
                _device.ImmediateContext.UnmapSubresource(_stagingTexture, 0);
            }
        }
        finally
        {
            bitmap.UnlockBits(bitmapData);
        }

        return bitmap;
    }

    private void RecreateDuplication()
    {
        try
        {
            _duplication?.Dispose();
            _duplication = _output.DuplicateOutput(_device);
        }
        catch
        {
            // 如果重创建失败，可能需要重新初始化整个设备
            var tempDevice = _device;
            var tempOutput = _output;
            var tempStagingTexture = _stagingTexture;
            var tempDesktopTexture = _desktopImageTexture;

            _device = null;
            _output = null;
            _stagingTexture = null;
            _desktopImageTexture = null;

            tempStagingTexture?.Dispose();
            tempDesktopTexture?.Dispose();
            tempOutput?.Dispose();
            tempDevice?.Dispose();

            Initialize();
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _disposed = true;

            _desktopImageTexture?.Dispose();
            _stagingTexture?.Dispose();
            _duplication?.Dispose();
            _output?.Dispose();
            _device?.Dispose();
        }
    }
}