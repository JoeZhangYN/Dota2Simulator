using Dota2Simulator.KeyboardMouse;
using ImageProcessingSystem;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.Games
{
    internal class Common
    {
        // 创建一个静态的 Logger 实例
        public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region 三缓冲设计

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

            public void Dispose()
            {
                // 代码错误,并未实现dispose方法
                //_readBuffer.Handle?.Dispose();
                //_writeBuffer.Handle?.Dispose();
                //_readyBuffer.Handle?.Dispose();
            }
        }
        #endregion

        /// <summary>
        ///     提高内存效率的标志
        /// <para>使用位标志提高内存效率 最高32个条件即0-31</para>
        /// </summary>
        [Flags]
        public enum ConditionFlags : uint
        {
            None = 0,
            条件1 = 1 << 0,
            条件2 = 1 << 1,
            条件3 = 1 << 2,
            条件4 = 1 << 3,
            条件5 = 1 << 4,
            条件6 = 1 << 5,
            条件7 = 1 << 6,
            条件8 = 1 << 7,
            条件9 = 1 << 8,

            // 技能快捷键 条件
            条件Q = 1 << 9,
            条件W = 1 << 10,
            条件E = 1 << 11,
            条件R = 1 << 12,
            条件D = 1 << 13,
            条件F = 1 << 14,

            // 物品快捷键 条件
            条件Z = 1 << 15,
            条件X = 1 << 16,
            条件C = 1 << 17,
            条件V = 1 << 18,
            条件B = 1 << 19,
            条件N = 1 << 20,
            条件Space = 1 << 21,

            // 其他条件
            条件保持假腿 = 1 << 22,
            条件开启切假腿 = 1 << 23,
            条件假腿敏捷 = 1 << 24,
            中断条件 = 1 << 25,
            切假腿中 = 1 << 26,
            需要切假腿 = 1 << 27,
            开启走A = 1 << 28,
        }

        // 条件委托定义
        public delegate Task<bool> ConditionDelegateBitmap(ImageHandle 句柄);

        // 截图委托定义
        public delegate bool ScreenCaptureDelegate();

        /// <summary>
        ///     sealed不可修改继承类,优化类
        /// </summary>
        public sealed class GameStateManager : IDisposable
        {
            // 连续内存布局
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct GameStateData
            {
                public long 全局时间;
                public Point 指定地点;
                public int 全局模式;
                public int 全局步骤;

                public void Reset()
                {
                    全局时间 = -1;
                    指定地点 = new Point(0, 0);
                    全局模式 = 0;
                    全局步骤 = 0;
                }
            }

            /// <summary>
            ///     状态信息组
            ///     <para>几个条件对应几组</para>
            /// </summary>
            private readonly GameStateData[] _gameStates = new GameStateData[22];

            /// <summary>
            ///     volatile多线程访问条件指针
            /// </summary>
            private volatile uint _conditionFlags;

            /// <summary>
            ///     读写线程锁
            /// </summary>
            private readonly ReaderWriterLockSlim _conditionLock = new();

            /// <summary>
            ///     委托组
            ///     <para>几个条件对应几个委托</para>
            /// </summary>
            private readonly ConditionDelegateBitmap[] _conditionDelegates = new ConditionDelegateBitmap[22];

            /// <summary>
            ///     三重缓存系统
            /// </summary>
            private readonly TripleBufferSystem _tripleBuffer;

            /// <summary>
            ///     双重缓存,用于跨线程下次调用
            /// </summary>
            private readonly ConcurrentBag<ConditionFlags> _nextCycleConditions = new();

            private bool _总循环条件 = false;
            private ScreenCaptureDelegate _循环内获取图片;

            /// <summary>
            ///     根据图片匹配不同的模式
            /// </summary>
            private ConditionDelegateBitmap _循环前委托;

            /// <summary>
            ///     条件对应数组序号
            /// </summary>
            private static readonly Dictionary<ConditionFlags, int> ConditionIndexMap = new()
    {
        { ConditionFlags.条件1, 0 }, { ConditionFlags.条件2, 1 }, { ConditionFlags.条件3, 2 },
        { ConditionFlags.条件4, 3 }, { ConditionFlags.条件5, 4 }, { ConditionFlags.条件6, 5 },
        { ConditionFlags.条件7, 6 }, { ConditionFlags.条件8, 7 }, { ConditionFlags.条件9, 8 },
        { ConditionFlags.条件Q, 9 }, { ConditionFlags.条件W, 10 }, { ConditionFlags.条件E, 11 },
        { ConditionFlags.条件R, 12 }, { ConditionFlags.条件D, 13 }, { ConditionFlags.条件F, 14 },
        { ConditionFlags.条件Z, 15 }, { ConditionFlags.条件X, 16 }, { ConditionFlags.条件C, 17 },
        { ConditionFlags.条件V, 18 }, { ConditionFlags.条件B, 19 }, { ConditionFlags.条件N, 20 },
        { ConditionFlags.条件Space, 21 }
    };

            /// <summary>
            ///     按键对应条件
            /// </summary>
            public static readonly Dictionary<Keys, ConditionFlags> KeyToConditionMap = new()
    {
        { Keys.D1, ConditionFlags.条件1 }, { Keys.D2, ConditionFlags.条件2 }, { Keys.D3, ConditionFlags.条件3 },
        { Keys.D4, ConditionFlags.条件4 }, { Keys.D5, ConditionFlags.条件5 }, { Keys.D6, ConditionFlags.条件6 },
        { Keys.D7, ConditionFlags.条件7 }, { Keys.D8, ConditionFlags.条件8 }, { Keys.D9, ConditionFlags.条件9 },
        { Keys.Q, ConditionFlags.条件Q }, { Keys.W, ConditionFlags.条件W }, { Keys.E, ConditionFlags.条件E },
        { Keys.R, ConditionFlags.条件R }, { Keys.D, ConditionFlags.条件D }, { Keys.F, ConditionFlags.条件F },
        { Keys.Z, ConditionFlags.条件Z }, { Keys.X, ConditionFlags.条件X }, { Keys.C, ConditionFlags.条件C },
        { Keys.V, ConditionFlags.条件V }, { Keys.B, ConditionFlags.条件B }, { Keys.N, ConditionFlags.条件N },
        { Keys.Space, ConditionFlags.条件Space }
    };

            /// <summary>
            ///     初始化游戏管理
            /// </summary>
            public GameStateManager()
            {
                // 新增一个三重缓冲截图 不传参 默认1920*1080
                _tripleBuffer = new TripleBufferSystem();

                // 清除所有的游戏状态
                for (int i = 0; i < _gameStates.Length; i++)
                    _gameStates[i].Reset();
            }

            /// <summary>
            ///     通用按键捕获设置条件为真
            ///     <para>数字1-9 QWERDF ZXCVBN Space</para>
            /// </summary>
            /// <param name="keyCode"></param>
            /// <returns></returns>
            public async Task HandleKeyPress(Keys keyCode)
            {
                if (!_总循环条件) _总循环条件 = true;

                if (KeyToConditionMap.TryGetValue(keyCode, out var condition))
                    SetCondition(condition, true);
            }

            /// <summary>
            ///     读取优化的全局变量
            /// </summary>
            /// <param name="flag">变量类型</param>
            /// <returns></returns>
            public bool GetCondition(ConditionFlags flag)
            {
                // 读锁
                _conditionLock.EnterReadLock();

                // 读取优化的全局变量
                try { return (_conditionFlags & (uint)flag) != 0; }
                finally { _conditionLock.ExitReadLock(); }
            }

            /// <summary>
            ///     设置优化的全局变量
            /// </summary>
            /// <param name="flag">变量类型</param>
            /// <param name="value">设置的值</param>
            public void SetCondition(ConditionFlags flag, bool value)
            {
                _conditionLock.EnterWriteLock();
                try
                {
                    var oldValue = (_conditionFlags & (uint)flag) != 0;
                    if (value) _conditionFlags |= (uint)flag;
                    else _conditionFlags &= ~(uint)flag;

                    if (!oldValue && value && ConditionIndexMap.ContainsKey(flag))
                        _nextCycleConditions.Add(flag);
                }
                finally { _conditionLock.ExitWriteLock(); }
            }

            /// <summary>
            ///     根据字典设置委托数组
            /// </summary>
            /// <param name="flag">变量类型</param>
            /// <param name="delegateFunc">具体委托</param>
            public void SetConditionDelegate(ConditionFlags flag, ConditionDelegateBitmap delegateFunc)
            {
                if (ConditionIndexMap.TryGetValue(flag, out var index))
                    _conditionDelegates[index] = delegateFunc;
            }

            /// <summary>
            ///     获取图像委托
            /// </summary>
            /// <param name="captureDelegate"></param>
            public void SetScreenCaptureDelegate(ScreenCaptureDelegate captureDelegate) => _循环内获取图片 = captureDelegate;

            /// <summary>
            ///     设置循环前委托,用于匹配不同的模式,修改对应按键触发逻辑
            /// </summary>
            /// <param name="gemstoneDelegate"></param>
            public void SetGemstoneDelegate(ConditionDelegateBitmap gemstoneDelegate) => _循环前委托 = gemstoneDelegate;

            /// <summary>
            ///     引用游戏状态信息,减少分配
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public ref GameStateData GetGameState(int index) => ref _gameStates[index];

            /// <summary>
            ///     获取全局时间引用
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public ref long GetGlobalTime(int index) => ref GetGameState(index).全局时间;

            /// <summary>
            ///     获取全局步骤引用
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public ref int GetGlobalStep(int index) => ref GetGameState(index).全局步骤;

            /// <summary>
            ///     主要循环方式
            /// </summary>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public async Task RunMainLoop(CancellationToken cancellationToken = default)
            {
                const int 主循环间隔 = 1;
                var captureTask = Task.CompletedTask;

                while (_总循环条件 && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (GetCondition(ConditionFlags.中断条件))
                        {
                            await Task.Delay(主循环间隔, cancellationToken);
                            continue;
                        }

                        if (_循环内获取图片 == null)
                        {
                            await Task.Delay(主循环间隔, cancellationToken);
                            continue;
                        }

                        // TODO:循环内获取图片需要修正,实际上和三重缓冲一个功能
                        if (captureTask.IsCompleted)
                        {
                            captureTask = Task.Run(() =>
                            {
                                _tripleBuffer.BeginCapture();
                                if (_循环内获取图片())
                                    _tripleBuffer.CommitCapture();
                            }, cancellationToken);
                        }

                        var (currentBuffer, isNewFrame) = _tripleBuffer.GetReadBuffer();

                        if (isNewFrame)
                        {
                            if (_循环前委托 != null)
                                await _循环前委托(currentBuffer);

                            await ProcessConditions(currentBuffer, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"主循环异常: {ex.Message}");
                    }

                    await Task.Delay(主循环间隔, cancellationToken);
                }

                await captureTask;
            }

            /// <summary>
            ///     循环处理各条件为真的条件委托
            /// </summary>
            /// <param name="currentBuffer"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            private async Task ProcessConditions(ImageHandle currentBuffer, CancellationToken cancellationToken)
            {
                var tasksToRun = new List<Task>();
                // 执行列表
                var conditionsToProcess = new List<ConditionFlags>();

                // 循环取出所有上一轮设置为真的条件
                while (_nextCycleConditions.TryTake(out var condition))
                {
                    if (GetCondition(condition))
                        conditionsToProcess.Add(condition);
                }

                // 获取当前条件
                var currentFlags = GetAllConditions();

                // 循环所有条件获取条件为真的类型添加如执行列表
                foreach (var kvp in ConditionIndexMap)
                {
                    // 当条件为真,且未被添加进执行列表
                    if ((currentFlags & (uint)kvp.Key) != 0 && !conditionsToProcess.Contains(kvp.Key))
                        conditionsToProcess.Add(kvp.Key);
                }

                // 根据执行列表已有的条件类型执行对应的委托并更新条件本身
                foreach (var flag in conditionsToProcess)
                {
                    if (ConditionIndexMap.TryGetValue(flag, out var index))
                    {
                        var delegateFunc = _conditionDelegates[index];
                        if (delegateFunc != null)
                        {
                            tasksToRun.Add(Task.Run(async () =>
                            {
                                try
                                {
                                    // 执行条件对应的委托
                                    var result = await delegateFunc(currentBuffer);
                                    // 设置条件为原值
                                    SetCondition(flag, result);
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error($"条件处理失败 [{flag}]: {ex.Message}");
                                }
                            }, cancellationToken));
                        }
                    }
                }

                if (tasksToRun.Count > 0)
                    await Task.WhenAll(tasksToRun);
            }

            public uint GetAllConditions()
            {
                _conditionLock.EnterReadLock();
                try { return _conditionFlags; }
                finally { _conditionLock.ExitReadLock(); }
            }

            public void Dispose()
            {
                _conditionLock?.Dispose();
                _tripleBuffer?.Dispose();
            }
        }
    }

}
