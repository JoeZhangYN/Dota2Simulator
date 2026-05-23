// Games/Common.cs

using NLog;
using System;
using System.Threading;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Ports;

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
        /// Phase 9 F 残留 service locator：仅 2 处反向依赖用：
        /// (1) SkillEngine.cs:1573/1593 内 ItemEngine.要求保持假腿()——SkillEngine 先于 ItemEngine 构造，不能 ctor 注；
        /// (2) Silt/Main.cs:29/34 内 ItemEngine.根据图片使用物品()——Silt 还是 static class，未 instance 化 (Phase 11 处理)。
        /// </summary>
        public static ItemEngine? ItemEngine;

        /// <summary>
        /// Phase 9 F 残留 service locator：Form2/GameSession/ItemEngine/SkillEngine/Silt 调 HeroLoopHost 都仍走桥。
        /// Phase 11 改 Form2/GameSession ctor 注入 + Silt instance 化后可删本字段。
        /// </summary>
        public static HeroLoopHost? HeroLoopHost;
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
