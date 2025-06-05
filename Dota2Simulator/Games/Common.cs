using ImageProcessingSystem;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.Games
{
    internal class Common
    {
        #region 其他通用变量
        /// <summary>
        ///     静态 Logger 实例
        /// </summary>
        public static readonly Logger Main_Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     调用UI页面元素
        /// </summary>
        public static Form2? Main_Form = Form2.Instance;
        #endregion

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

        #region 状态管理器
        /// <summary>
        /// 高性能状态管理器 - 使用位标志和连续内存布局
        /// </summary>
        public sealed class GameStateManager : IDisposable
        {
            // 使用结构体确保连续内存布局
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct GameState
            {
                public long GlobalTime;
                public Point TargetPoint;
                public int GlobalMode;
                public int GlobalStep;
                public ConditionFlags Conditions;

                public void Reset()
                {
                    GlobalTime = -1;
                    TargetPoint = Point.Empty;
                    GlobalMode = 0;
                    GlobalStep = 0;
                    Conditions = ConditionFlags.None;
                }
            }

            // 使用位标志提高内存效率
            [Flags]
            public enum ConditionFlags : ulong
            {
                None = 0,
                Condition1 = 1 << 0,
                Condition2 = 1 << 1,
                Condition3 = 1 << 2,
                Condition4 = 1 << 3,
                Condition5 = 1 << 4,
                Condition6 = 1 << 5,
                Condition7 = 1 << 6,
                Condition8 = 1 << 7,
                Condition9 = 1 << 8,

                // 技能快捷键条件
                ConditionQ = 1 << 9,
                ConditionW = 1 << 10,
                ConditionE = 1 << 11,
                ConditionR = 1 << 12,
                ConditionD = 1 << 13,
                ConditionF = 1 << 14,

                // 物品快捷键条件
                ConditionZ = 1 << 15,
                ConditionX = 1 << 16,
                ConditionC = 1 << 17,
                ConditionV = 1 << 18,
                ConditionB = 1 << 19,
                ConditionN = 1 << 20,
                ConditionSpace = 1 << 21,

                // 其他条件
                MaintainTreads = 1 << 22,
                TreadSwitchEnabled = 1 << 23,
                TreadsAgility = 1 << 24,
                Interrupted = 1 << 25,
                TreadSwitching = 1 << 26,
                NeedTreadSwitch = 1 << 27,
                AutoAttackEnabled = 1 << 28,
            }

            // 使用数组而不是字典，通过索引直接访问
            private readonly GameState[] _states = new GameState[32];
            private readonly ReaderWriterLockSlim _lock = new();

            // 使用FrozenDictionary提高查找性能
            private static readonly FrozenDictionary<Keys, ConditionFlags> KeyToConditionMap =
                new Dictionary<Keys, ConditionFlags>
                {
                { Keys.Q, ConditionFlags.ConditionQ },
                { Keys.W, ConditionFlags.ConditionW },
                { Keys.E, ConditionFlags.ConditionE },
                { Keys.R, ConditionFlags.ConditionR },
                { Keys.D, ConditionFlags.ConditionD },
                { Keys.F, ConditionFlags.ConditionF },
                { Keys.Z, ConditionFlags.ConditionZ },
                { Keys.X, ConditionFlags.ConditionX },
                { Keys.C, ConditionFlags.ConditionC },
                { Keys.V, ConditionFlags.ConditionV },
                { Keys.B, ConditionFlags.ConditionB },
                { Keys.N, ConditionFlags.ConditionN },
                { Keys.Space, ConditionFlags.ConditionSpace }
                }.ToFrozenDictionary();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool GetCondition(ConditionFlags flag)
            {
                _lock.EnterReadLock();
                try
                {
                    return (_states[0].Conditions & flag) != 0;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCondition(ConditionFlags flag, bool value)
            {
                _lock.EnterWriteLock();
                try
                {
                    if (value)
                        _states[0].Conditions |= flag;
                    else
                        _states[0].Conditions &= ~flag;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }

            public ref GameState GetState(int index)
            {
                if (index < 0 || index >= _states.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return ref _states[index];
            }

            public void Dispose()
            {
                _lock?.Dispose();
            }
        } 
        #endregion
    }
}
