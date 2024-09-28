using System;
using System.Drawing;
using System.Drawing.Imaging;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.DXGI.Resource;
using ResultCode = SharpDX.DXGI.ResultCode;


namespace Dota2Simulator.PictureProcessing;

internal class ScreenCaptureDX : IDisposable
{
    private Adapter1 adapter;
    private Device device;
    private OutputDuplication duplicatedOutput;
    private Factory1 factory;
    private Output output;
    private Output1 output1;
    private Texture2D stagingTexture;

    public ScreenCaptureDX()
    {
        InitializeDevice();
    }

    public void Dispose()
    {
        stagingTexture?.Dispose();
        duplicatedOutput?.Dispose();
        output1?.Dispose();
        output?.Dispose();
        device?.Dispose();
        adapter?.Dispose();
        factory?.Dispose();
    }

    private void InitializeDevice()
    {
        try
        {
            factory = new Factory1();
            adapter = factory.GetAdapter1(0);
            device = new Device(adapter);
            output = adapter.GetOutput(0);
            output1 = output.QueryInterface<Output1>();

            // Attempt to duplicate the output, if it fails, catch the exception
            try
            {
                duplicatedOutput = output1.DuplicateOutput(device);
            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode.Code == ResultCode.NotCurrentlyAvailable.Code)
                    throw new NotSupportedException("Desktop duplication is not supported on this device.");
                else
                    throw;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing device: {ex.Message}");
            Dispose();
            throw;
        }
    }

    public Bitmap CaptureScreen(Rectangle captureRegion)
    {
        OutputDuplicateFrameInformation duplicateFrameInformation;

        Resource screenResource;
        try
        {
            Result result =
                duplicatedOutput.TryAcquireNextFrame(1000, out duplicateFrameInformation, out screenResource);
            if (result.Failure)
            {
                Console.WriteLine($"Failed to acquire next frame: {result}");
                return null;
            }
        }
        catch (SharpDXException ex) when (ex.ResultCode.Code == ResultCode.WaitTimeout.Code)
        {
            Console.WriteLine("Timeout while acquiring next frame");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error acquiring next frame: {ex.Message}");
            return null;
        }

        using Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>();
        Texture2DDescription desc = screenTexture2D.Description;

        if (captureRegion.X < 0 || captureRegion.Y < 0 || captureRegion.Right > desc.Width ||
            captureRegion.Bottom > desc.Height)
        {
            Console.WriteLine("Capture region is out of bounds");
            screenResource.Dispose();
            duplicatedOutput.ReleaseFrame();
            return null;
        }

        if (stagingTexture == null || stagingTexture.Description.Width != desc.Width ||
            stagingTexture.Description.Height != desc.Height)
        {
            stagingTexture?.Dispose();
            Texture2DDescription stagingDesc = new()
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = desc.Format,
                Width = desc.Width,
                Height = desc.Height,
                OptionFlags = desc.OptionFlags,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Staging
            };
            stagingTexture = new Texture2D(device, stagingDesc);
        }

        device.ImmediateContext.CopyResource(screenTexture2D, stagingTexture);

        try
        {
            DataBox mapSource = device.ImmediateContext.MapSubresource(stagingTexture, 0, MapMode.Read, MapFlags.None);

            Bitmap bitmap = new(captureRegion.Width, captureRegion.Height, PixelFormat.Format32bppArgb);
            Rectangle boundsRect = new(0, 0, captureRegion.Width, captureRegion.Height);

            BitmapData mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            nint sourcePtr = mapSource.DataPointer + captureRegion.Y * mapSource.RowPitch + captureRegion.X * 4;
            nint destPtr = mapDest.Scan0;
            int sourcePitch = mapSource.RowPitch;
            int destPitch = mapDest.Stride;

            for (int y = 0; y < captureRegion.Height; y++)
            {
                Utilities.CopyMemory(destPtr, sourcePtr, captureRegion.Width * 4);
                sourcePtr = IntPtr.Add(sourcePtr, sourcePitch);
                destPtr = IntPtr.Add(destPtr, destPitch);
            }

            bitmap.UnlockBits(mapDest);
            device.ImmediateContext.UnmapSubresource(stagingTexture, 0);

            return bitmap;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mapping subresource: {ex}");
            return null;
        }
        finally
        {
            screenResource.Dispose();
            duplicatedOutput.ReleaseFrame();
        }
    }
}