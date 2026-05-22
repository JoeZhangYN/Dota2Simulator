using Collections.Pooled;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Threading.Timer;
using Tuple = Dota2Simulator.Vision.ImageFinder.Tuple;

namespace Dota2Simulator.Vision
{
    /// <summary>
    /// 动态图像缓冲区 - 用于截图等会变化的图像
    /// </summary>
    public static unsafe class DynamicImageBuffer
    {
        private static byte* _buffer;
        private static long _bufferSize;
        private static long _currentOffset;
        private static GCHandle _handle;
        private static bool _initialized;
        private static readonly Lock _allocLock = new();
        private static ILogger _logger = new ConsoleLogger();

        // 使用分段内存池减少碎片
        private const int SEGMENT_SIZE = 64 * 1024 * 1024; // 64MB per segment
        private static readonly List<(long offset, long size, bool inUse)> _segments = new();

        public static bool IsInitialized => _initialized;

        public static void Initialize(long initialSize = 64 * 1024 * 1024)
        {
            if (_initialized)
                Cleanup();

            _bufferSize = initialSize;
            _currentOffset = 0;

            if (!ImageProcessingSystem.TryAllocate(_bufferSize))
            {
                throw new OutOfMemoryException($"无法分配动态缓冲区: {_bufferSize / (1024 * 1024)}MB");
            }

            var managedArray = new byte[_bufferSize];
            _handle = GCHandle.Alloc(managedArray, GCHandleType.Pinned);
            _buffer = (byte*)_handle.AddrOfPinnedObject();
            _initialized = true;

            _logger.LogInfo($"动态缓冲区初始化: {_bufferSize / (1024 * 1024)}MB");
        }

        /// <summary>
        /// 分配空间 - 使用内存池管理
        /// </summary>
        public static long AllocateSpace(long size)
        {
            lock (_allocLock)
            {
                // 查找可重用的段
                for (int i = 0; i < _segments.Count; i++)
                {
                    var segment = _segments[i];
                    if (!segment.inUse && segment.size >= size)
                    {
                        _segments[i] = (segment.offset, segment.size, true);
                        return segment.offset;
                    }
                }

                // 分配新空间
                if (_currentOffset + size > _bufferSize)
                {
                    ExpandBuffer(size);
                }

                long offset = _currentOffset;
                _segments.Add((offset, size, true));
                _currentOffset += size;
                return offset;
            }
        }

        /// <summary>
        /// 释放空间
        /// </summary>
        public static void ReleaseSpace(long offset)
        {
            lock (_allocLock)
            {
                for (int i = 0; i < _segments.Count; i++)
                {
                    var segment = _segments[i];
                    if (segment.offset == offset)
                    {
                        _segments[i] = (segment.offset, segment.size, false);
                        break;
                    }
                }
            }
        }

        private static void ExpandBuffer(long additionalSize)
        {
            long newSize = _bufferSize + Math.Max(additionalSize, SEGMENT_SIZE);
            _logger.LogInfo($"扩容动态缓冲区: {_bufferSize / (1024 * 1024)}MB -> {newSize / (1024 * 1024)}MB");

            var newArray = new byte[newSize];
            var newHandle = GCHandle.Alloc(newArray, GCHandleType.Pinned);
            var newBuffer = (byte*)newHandle.AddrOfPinnedObject();

            Buffer.MemoryCopy(_buffer, newBuffer, newSize, _currentOffset);

            _handle.Free();
            ImageProcessingSystem.Deallocate(_bufferSize);

            _handle = newHandle;
            _buffer = newBuffer;
            _bufferSize = newSize;

            ImageProcessingSystem.TryAllocate(newSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateData(long offset, byte[] data)
        {
            fixed (byte* src = data)
            {
                Buffer.MemoryCopy(src, _buffer + offset, data.Length, data.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetPixel(long offset, int x, int y, int width)
        {
            long pixelOffset = offset + ((y * width + x) * 4);
            return *(uint*)(_buffer + pixelOffset);
        }

        public static IntPtr GetPointer(long offset) => (IntPtr)(_buffer + offset);

        public static void Cleanup()
        {
            if (_initialized)
            {
                _handle.Free();
                ImageProcessingSystem.Deallocate(_bufferSize);
                _segments.Clear();
                _currentOffset = 0;
                _initialized = false;
            }
        }
    }
}
