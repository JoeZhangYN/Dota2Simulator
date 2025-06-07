using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Dota2Simulator.PictureProcessing.RustImageProcessingSystem
{
    // Structures matching Rust types
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Size
    {
        public uint Width;
        public uint Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int X;
        public int Y;
        public uint Width;
        public uint Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageHandle
    {
        public IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MultipleResults
    {
        public IntPtr points;
        public int count;
        public int capacity;
    }

    // Main wrapper class
    public class ImageProcessor : IDisposable
    {
        private bool _disposed = false;
        private bool _initialized = false;

        // P/Invoke declarations
        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ips_initialize(uint width, uint height);

        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ips_cleanup();

        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern ImageHandle ips_create_image(IntPtr data, uint width, uint height);

        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ips_release_image(ImageHandle handle);

        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ips_capture_screen(int x, int y, uint width, uint height);

        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ips_find_image(
            ImageHandle needle,
            double matchRate,
            byte tolerance,
            out int outX,
            out int outY);

        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern MultipleResults ips_find_all_images(
            ImageHandle needle,
            double matchRate,
            byte tolerance,
            int maxResults);

        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ips_free_results(MultipleResults results);

        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint ips_get_pixel(int x, int y);

        [DllImport("image_processing_system.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ips_get_memory_stats(
            out ulong totalSize,
            out ulong allocated,
            out ulong free);

        // Public methods
        public bool Initialize(uint width, uint height)
        {
            if (_initialized)
                return true;

            _initialized = ips_initialize(width, height);
            return _initialized;
        }

        public bool CaptureScreen(Rectangle region)
        {
            if (!_initialized)
                throw new InvalidOperationException("System not initialized");

            return ips_capture_screen(region.X, region.Y, region.Width, region.Height);
        }

        public bool CaptureScreen(int x, int y, uint width, uint height)
        {
            return CaptureScreen(new Rectangle { X = x, Y = y, Width = width, Height = height });
        }

        public ImageHandle CreateImage(Bitmap bitmap)
        {
            if (!_initialized)
                throw new InvalidOperationException("System not initialized");

            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                // Convert ARGB to BGRA
                int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
                byte[] rgbValues = new byte[bytes];
                Marshal.Copy(bitmapData.Scan0, rgbValues, 0, bytes);

                // Swap R and B channels
                for (int i = 0; i < rgbValues.Length; i += 4)
                {
                    byte temp = rgbValues[i];
                    rgbValues[i] = rgbValues[i + 2];
                    rgbValues[i + 2] = temp;
                }

                var handle = GCHandle.Alloc(rgbValues, GCHandleType.Pinned);
                try
                {
                    return ips_create_image(
                        handle.AddrOfPinnedObject(),
                        (uint)bitmap.Width,
                        (uint)bitmap.Height);
                }
                finally
                {
                    handle.Free();
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        public ImageHandle CreateImage(string imagePath)
        {
            using (var bitmap = new Bitmap(imagePath))
            {
                return CreateImage(bitmap);
            }
        }

        public void ReleaseImage(ImageHandle handle)
        {
            ips_release_image(handle);
        }

        public Point? FindImage(ImageHandle needle, double matchRate = 0.95, byte tolerance = 0)
        {
            if (!_initialized)
                throw new InvalidOperationException("System not initialized");

            if (ips_find_image(needle, matchRate, tolerance, out int x, out int y))
            {
                return new Point { X = x, Y = y };
            }
            return null;
        }

        public List<Point> FindAllImages(ImageHandle needle, double matchRate = 0.95, byte tolerance = 0, int maxResults = 100)
        {
            if (!_initialized)
                throw new InvalidOperationException("System not initialized");

            var results = ips_find_all_images(needle, matchRate, tolerance, maxResults);
            var points = new List<Point>(results.count);

            if (results.count > 0 && results.points != IntPtr.Zero)
            {
                int pointSize = Marshal.SizeOf<Point>();
                for (int i = 0; i < results.count; i++)
                {
                    var point = Marshal.PtrToStructure<Point>(
                        IntPtr.Add(results.points, i * pointSize));
                    points.Add(point);
                }

                ips_free_results(results);
            }

            return points;
        }

        public Color GetPixel(int x, int y)
        {
            if (!_initialized)
                throw new InvalidOperationException("System not initialized");

            uint pixel = ips_get_pixel(x, y);
            return new Color
            {
                B = (byte)(pixel & 0xFF),
                G = (byte)((pixel >> 8) & 0xFF),
                R = (byte)((pixel >> 16) & 0xFF),
                A = (byte)((pixel >> 24) & 0xFF)
            };
        }

        public (ulong totalSize, ulong allocated, ulong free) GetMemoryStats()
        {
            ips_get_memory_stats(out ulong total, out ulong allocated, out ulong free);
            return (total, allocated, free);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_initialized)
                {
                    ips_cleanup();
                    _initialized = false;
                }
                _disposed = true;
            }
        }

        ~ImageProcessor()
        {
            Dispose(false);
        }
    }

    // Helper class for automatic image handle management
    public class ManagedImage : IDisposable
    {
        private ImageHandle _handle;
        private ImageProcessor _processor;
        private bool _disposed = false;

        public ImageHandle Handle => _handle;

        internal ManagedImage(ImageProcessor processor, ImageHandle handle)
        {
            _processor = processor;
            _handle = handle;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_handle.ptr != IntPtr.Zero)
                {
                    _processor.ReleaseImage(_handle);
                    _handle.ptr = IntPtr.Zero;
                }
                _disposed = true;
            }
        }

        ~ManagedImage()
        {
            Dispose(false);
        }
    }
}
