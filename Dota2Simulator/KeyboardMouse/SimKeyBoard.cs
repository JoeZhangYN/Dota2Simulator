using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Dota2Simulator.Diagnostics;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Input.Adapters;

namespace Dota2Simulator.KeyboardMouse
{
    /// <summary>
    /// 键鼠操作门面。Strangler Fig 过渡形态：保留原静态 API 供 ~120 处业务调用，
    /// 内部转发到 IInputExecutor 端口。Phase 4 业务层直连端口后，本门面于 Phase 6 删除。
    /// </summary>
    internal class SimKeyBoard
    {
        // 经探针装饰器包裹——探针默认关闭、零开销；冒烟测试时 RecordReplayProbe.BeginSession 开录。
        private static readonly IInputExecutor _executor =
            RecordReplayProbe.Wrap(new HybridInputAdapter());

        #region 模拟按键

        /// <summary>
        ///     用于预热,基本没用.
        /// </summary>
        public static void InitEnigoThreadlocal()
        {
            SimEnigo.init_enigo_threadlocal();
        }

        public static void MouseLeftClick() => _executor.MouseClick(MouseButton.Left);

        public static void MouseRightClick() => _executor.MouseClick(MouseButton.Right);

        public static void MouseLeftUp() => _executor.MouseUp(MouseButton.Left);

        public static void MouseLeftDown() => _executor.MouseDown(MouseButton.Left);

        public static void MouseRightUp() => _executor.MouseUp(MouseButton.Right);

        public static void MouseRightDown() => _executor.MouseDown(MouseButton.Right);

        /// <summary>
        ///     单个耗时 1-3ms
        /// </summary>
        public static void KeyPress(Keys key) => _executor.Press(VirtualKey.From(key));

        public static void KeyUp(Keys key) => _executor.KeyUp(VirtualKey.From(key));

        public static void KeyDown(Keys key) => _executor.KeyDown(VirtualKey.From(key));

        /// <summary>
        ///     优化原先逻辑，原先默认key_click 需要等待30ms，现取消
        /// </summary>
        /// <param name="key1">Keys.LShift</param>
        public static void KeyPressWhile(Keys key, Keys key1)
            => _executor.ComboWhile(VirtualKey.From(key), VirtualKey.From(key1));

        /// <param name="key1">Keys.LShift</param>
        /// <param name="key2">Keys.Alt</param>
        public static void KeyPressWhileTwo(Keys key, Keys key1, Keys key2)
            => _executor.ComboWhile(VirtualKey.From(key), VirtualKey.From(key1), VirtualKey.From(key2));

        /// <summary>
        ///     优化原先逻辑，原先默认key_click 需要等待30ms，现取消
        /// </summary>
        public static void KeyPressAlt(Keys key) => _executor.ComboAlt(VirtualKey.From(key));

        public static void MouseMoveTo(int x, int y) => _executor.MouseMoveTo(new ScreenPoint(x, y));

        public static void MouseMove(int x, int y, bool relative = false)
            => _executor.MouseMove(x, y, relative);

        public static void MouseMoveSim(int X, int Y)
        {
            Point p = Control.MousePosition;

            int def_x = (X - p.X) / 15;
            int def_y = (Y - p.Y) / 15;

            for (int i = 1; i < 15; i++)
            {
                Delay(1);
                MouseMove(p.X + (def_x * i), p.Y + (def_y * i));
            }

            MouseMove(X, Y);
        }

        public static void MouseMove(Point p, bool relative = false)
        {
            int X = p.X;
            int Y = p.Y;

            if (relative)
            {
                Point p1 = Control.MousePosition;
                X += p1.X;
                Y += p1.Y;
            }

            MouseMove(X, Y);
        }

        #endregion

        #region 延时

        /// <summary>
        ///     精准延迟，并减少性能消耗
        /// </summary>
        /// <param name="delay">需要延迟的时间</param>
        /// <param name="time"></param>
        private static void Delay(int delay, long time = -1)
        {
            time = time == -1 ? 获取当前时间毫秒() : time;
            long endTime = time + delay;
            SpinWait spinWait = new();

            while (获取当前时间毫秒() < endTime)
            {
                int remainingTime = (int)(endTime - 获取当前时间毫秒());
                switch (remainingTime)
                {
                    case > 0 and > 10:
                        Thread.Sleep(remainingTime / 2); // Sleep for half of the remaining time to avoid oversleeping
                        break;
                    case > 0:
                        spinWait.SpinOnce(); // SpinWait for very short intervals
                        break;
                }
            }
        }

        private static long 获取当前时间毫秒()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        #endregion
    }
}
