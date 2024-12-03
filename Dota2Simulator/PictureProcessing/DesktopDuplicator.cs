using System;
using System.Drawing;
using System.Drawing.Imaging;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.Direct3D11.Resource;

namespace Dota2Simulator.PictureProcessing
{
    internal class DesktopDuplicator : IDisposable
    {
        private Device device;
        private OutputDuplication outputDuplication;
        private int screenHeight;
        private int screenWidth;

        public DesktopDuplicator()
        {
            Initialize();
        }

        public void Dispose()
        {
            outputDuplication?.Dispose();
            device?.Dispose();
        }

        private void Initialize()
        {
            // 创建 Direct3D 设备
            device = new Device(DriverType.Hardware, DeviceCreationFlags.None);

            // 获取输出
            using Factory1 dxgiFactory = new();
            Adapter1 dxgiAdapter = dxgiFactory.Adapters1[0];
            Output output = dxgiAdapter.Outputs[0];
            Output1 output1 = output.QueryInterface<Output1>();

            screenWidth = output.Description.DesktopBounds.Right - output.Description.DesktopBounds.Left;
            screenHeight = output.Description.DesktopBounds.Bottom - output.Description.DesktopBounds.Top;

            // 初始化输出复制
            outputDuplication = output1.DuplicateOutput(device);
        }

        public Bitmap Capture(Rectangle captureArea)
        {
            OutputDuplicateFrameInformation frameInfo;
            Resource desktopResource = null;
            SharpDX.DXGI.Resource tempResource = null;

            try
            {
                // 捕捉屏幕
                _ = outputDuplication.TryAcquireNextFrame(500, out frameInfo, out tempResource);
                desktopResource = tempResource.QueryInterface<Resource>();

                using Texture2D desktopTexture = desktopResource.QueryInterface<Texture2D>();
                Texture2DDescription textureDesc = new()
                {
                    CpuAccessFlags = CpuAccessFlags.Read,
                    BindFlags = BindFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = captureArea.Width,
                    Height = captureArea.Height,
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Staging
                };

                Texture2D captureTexture = new(device, textureDesc);

                // 将指定区域从屏幕纹理中复制到捕获纹理
                ResourceRegion region = new()
                {
                    Left = captureArea.X,
                    Top = captureArea.Y,
                    Right = captureArea.X + captureArea.Width,
                    Bottom = captureArea.Y + captureArea.Height,
                    Front = 0,
                    Back = 1
                };

                device.ImmediateContext.CopySubresourceRegion(desktopTexture, 0, region, captureTexture, 0);

                // 将纹理数据复制到位图
                Bitmap bitmap = new(captureArea.Width, captureArea.Height, PixelFormat.Format32bppArgb);
                DataBox mapSource =
                    device.ImmediateContext.MapSubresource(captureTexture, 0, MapMode.Read, MapFlags.None);

                Rectangle boundsRect = new(0, 0, captureArea.Width, captureArea.Height);
                BitmapData mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                nint sourcePtr = mapSource.DataPointer;
                nint destPtr = mapDest.Scan0;

                for (int y = 0; y < captureArea.Height; y++)
                {
                    Utilities.CopyMemory(destPtr, sourcePtr, captureArea.Width * 4);
                    sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                    destPtr = IntPtr.Add(destPtr, mapDest.Stride);
                }

                // 解锁位图
                bitmap.UnlockBits(mapDest);
                device.ImmediateContext.UnmapSubresource(captureTexture, 0);

                captureTexture.Dispose();

                return bitmap;
            }
            finally
            {
                if (desktopResource != null)
                {
                    desktopResource.Dispose();
                }

                if (tempResource != null)
                {
                    tempResource.Dispose();
                }

                outputDuplication.ReleaseFrame();
            }
        }
    }
}