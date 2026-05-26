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
    /* 优点
     * 读取缓冲、写入缓冲、就绪缓冲三个独立缓冲区
     * 读写真正解耦,写入永不阻塞,可立即开始下一帧捕获
     * 使用SpinLock实现超低延迟的缓冲区交换
     * 缺点
     * 获取的是2帧前的数据
     * 额外8MB内存（对现代系统微不足道）
     * 略微增加的复杂度
     */
    /// <summary>
    ///     三缓冲设计
    /// </summary>
    public sealed class TripleBufferSystem : IDisposable
    {
        private struct BufferInfo
        {
            public ImageHandle Handle { get; set; }
            public long Timestamp { get; set; }
            public int FrameId { get; set; }
        }

        /// <summary>
        ///     读buff
        /// </summary>
        private BufferInfo _readBuffer;
        /// <summary>
        ///     写buff
        /// </summary>
        private BufferInfo _writeBuffer;
        /// <summary>
        ///     三层缓冲buff
        /// </summary>
        private BufferInfo _readyBuffer;
        /// <summary>
        ///     锁
        /// </summary>
        private SpinLock _spinLock;
        /// <summary>
        ///     帧数计数
        ///     <para>理论上有上限,每帧增加会爆</para>
        ///     <para>按照1秒240hz,达到上限2147483647,需要103天</para>
        /// </summary>
        private int _frameCounter;

        /// <summary>
        ///     初始化三重缓冲
        /// </summary>
        /// <param name="width">宽度 默认1920</param>
        /// <param name="height">高度 默认1080</param>
        public TripleBufferSystem(int width = 1920, int height = 1080)
        {
            var size = new Size(width, height);
            var initialData = new byte[width * height * 4];

            _readBuffer = new BufferInfo { Handle = ImageManager.CreateDynamicImage(initialData, size, "ReadBuffer"), FrameId = -1 };
            _writeBuffer = new BufferInfo { Handle = ImageManager.CreateDynamicImage(initialData, size, "WriteBuffer"), FrameId = -1 };
            _readyBuffer = new BufferInfo { Handle = ImageManager.CreateDynamicImage(initialData, size, "ReadyBuffer"), FrameId = -1 };
        }

        public ImageHandle GetWriteBuffer() => _writeBuffer.Handle;

        /// <summary>
        ///     获取当前时间,暂无其他功能
        /// </summary>
        public void BeginCapture() => _writeBuffer.Timestamp = Stopwatch.GetTimestamp();

        /// <summary>
        ///     修改确认,
        /// </summary>
        public void CommitCapture()
        {
            _writeBuffer.FrameId = Interlocked.Increment(ref _frameCounter);
            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                (_writeBuffer, _readyBuffer) = (_readyBuffer, _writeBuffer);
            }
            finally
            {
                if (lockTaken) _spinLock.Exit();
            }
        }

        /// <summary>
        /// 获取读缓冲区句柄和新帧标志
        /// </summary>
        public (ImageHandle handle, bool isNewFrame) GetReadBuffer()
        {
            bool lockTaken = false;
            bool hasNewFrame = false;

            try
            {
                _spinLock.Enter(ref lockTaken);
                if (_readyBuffer.FrameId > _readBuffer.FrameId)
                {
                    (_readBuffer, _readyBuffer) = (_readyBuffer, _readBuffer);
                    hasNewFrame = true;
                }
            }
            finally
            {
                if (lockTaken) _spinLock.Exit();
            }

            return (_readBuffer.Handle, hasNewFrame);
        }

        /// <summary>
        /// 仅获取读缓冲区句柄（高性能版本）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImageHandle GetReadHandle()
        {
            bool lockTaken = false;

            try
            {
                _spinLock.Enter(ref lockTaken);
                // 如果有新帧，交换缓冲区
                if (_readyBuffer.FrameId > _readBuffer.FrameId)
                {
                    (_readBuffer, _readyBuffer) = (_readyBuffer, _readBuffer);
                }
                return _readBuffer.Handle;
            }
            finally
            {
                if (lockTaken) _spinLock.Exit();
            }
        }

        /// <summary>
        /// 仅获取当前读缓冲区句柄，不进行缓冲区交换（最高性能）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImageHandle GetCurrentReadHandle()
        {
            // 直接返回当前读缓冲区，不检查是否有新帧
            return _readBuffer.Handle;
        }

        /// <summary>
        /// 检查是否有新帧可用
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasNewFrame()
        {
            return _readyBuffer.FrameId > _readBuffer.FrameId;
        }

        public void Dispose()
        {
            // ImageHandle 是 readonly struct 不实现 IDisposable, 通过 ImageManager 注册表释放 backing buffer.
            // 调用方约定: Dispose 时无并发 reader/writer (与通用 IDisposable 模式一致).
            // BufferInfo.Handle 是 property (rvalue) 不能直传 in 参数, 用局部变量中转.
            ImageHandle r = _readBuffer.Handle;
            ImageHandle w = _writeBuffer.Handle;
            ImageHandle y = _readyBuffer.Handle;
            ImageManager.ReleaseImage(in r);
            ImageManager.ReleaseImage(in w);
            ImageManager.ReleaseImage(in y);
        }
    }
}
