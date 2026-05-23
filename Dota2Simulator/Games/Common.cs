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
        /// BC 内 static class（Games.Dota2 / Silt）的 IUiInvoker 入口。
        /// 由 AppContainer.BindUi 在 Form2 构造完成后赋值。LOL/HF2 路径不创建 AppContainer，
        /// 此字段保持 null——但 BC 业务在 #if DOTA2 / #if Silt 编译开关下也不会被调用。
        /// 后续 BC 整顿完成后此 service locator 可替换为 ctor 注入。
        /// </summary>
        public static IUiInvoker? UiInvoker;

        /// <summary>
        /// Phase 8 C4 过渡 service locator：Skill facade thin 转发壳调本字段。
        /// AppContainer.BindUi 装配。C7 92 策略 ctor 扩参 SkillEngine 后，D1 删本字段 + 删 Skill facade。
        /// </summary>
        public static SkillEngine? SkillEngine;

        /// <summary>
        /// Phase 8 C5 过渡 service locator：Item facade thin 转发壳调本字段。同 SkillEngine 模式。
        /// </summary>
        public static ItemEngine? ItemEngine;
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
