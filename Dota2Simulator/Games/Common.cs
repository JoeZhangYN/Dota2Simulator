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
        public static readonly Logger Main_Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     调用UI页面元素
        /// </summary>
        public static Form2? Main_Form = Form2.Instance;

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
            ///     双重缓存,用于跨线程下次调用
            /// </summary>
            private readonly ConcurrentBag<ConditionFlags> _nextCycleConditions = new();

            private bool _总循环条件;
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
                //_tripleBuffer = new TripleBufferSystem();

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
            public void HandleKeyPress(Keys keyCode)
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
                                //_tripleBuffer.BeginCapture();
                                if (_循环内获取图片()) {
                                    //_tripleBuffer.CommitCapture();
                                }
                            }, cancellationToken);
                        }

                        //var (currentBuffer, isNewFrame) = _tripleBuffer.GetReadBuffer();

                        //if (isNewFrame)
                        //{
                        //    if (_循环前委托 != null)
                        //        await _循环前委托(currentBuffer);

                        //    await ProcessConditions(currentBuffer, cancellationToken);
                        //}
                    }
                    catch (Exception ex)
                    {
                        Main_Logger.Error($"主循环异常: {ex.Message}");
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
                                    Main_Logger.Error($"条件处理失败 [{flag}]: {ex.Message}");
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
                //_tripleBuffer?.Dispose();
            }
        }



        #region 延时

        /// <summary>
        ///     精准延迟，并减少性能消耗
        /// </summary>
        /// <param name="delay">需要延迟的时间</param>
        /// <param name="time"></param>
        public static void Delay(int delay, long time = -1)
        {
            if (delay <= 0)
            {
                return;
            }

            time = time == -1 ? 获取当前时间毫秒() : time;
            long endTime = time + delay;
            SpinWait spinWait = new();

            while (true)
            {
                long currentTime = 获取当前时间毫秒();
                if (currentTime >= endTime)
                {
                    break;
                }

                long remainingTime = endTime - currentTime;

                if (remainingTime > 10)
                {
                    Thread.Sleep((int)(remainingTime / 2)); // 睡眠2分之一
                }
                else if (remainingTime > 2)
                {
                    Thread.Sleep(1); // 如果剩余时间小于10毫秒，但大于2毫秒，则睡眠1毫秒
                }
                else
                {
                    spinWait.SpinOnce(); // SpinWait for very short intervals
                }
            }
        }

        public static void Delay(long delay, long time = -1)
        {
            if (delay <= 0)
            {
                return;
            }

            time = time == -1 ? 获取当前时间毫秒() : time;
            long endTime = time + delay;
            SpinWait spinWait = new();

            while (true)
            {
                long currentTime = 获取当前时间毫秒();
                if (currentTime >= endTime)
                {
                    break;
                }

                long remainingTime = endTime - currentTime;

                if (remainingTime > 10)
                {
                    Thread.Sleep((int)(remainingTime / 2)); // 睡眠2分之一
                }
                else if (remainingTime > 2)
                {
                    Thread.Sleep(1); // 如果剩余时间小于10毫秒，但大于2毫秒，则睡眠1毫秒
                }
                else
                {
                    spinWait.SpinOnce(); // SpinWait for very short intervals
                }
            }
        }

        #endregion

        #region 获取当前时间毫秒

        public static long 获取当前时间毫秒()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
        public static void 初始化全局时间(ref long time)
        {
            time = 获取当前时间毫秒();
        }

        #endregion
    }
}
