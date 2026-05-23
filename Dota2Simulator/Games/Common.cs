// Games/Common.cs
// Phase 11 P9: 真删 ItemEngine / HeroLoopHost 两 service locator 字段 (0 service locator 终态).
// 历史: Phase 9 F 引入这两字段作为「最后过渡」桥, P11 P1-P7 全 8 处调用方分别切 ctor / setter / 形参注入完成消除.

using NLog;
using System;
using System.Threading;

namespace Dota2Simulator.Games
{
    internal class Common
    {
        #region 其他通用变量
        /// <summary>
        ///     静态 Logger 实例
        /// </summary>
        public static readonly Logger Main_Logger = LogManager.GetCurrentClassLogger();
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
    }
}
