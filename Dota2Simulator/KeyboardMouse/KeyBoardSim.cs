using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Dota2Simulator.KeyboardMouse
{
    internal class SimKeyBoard
    {
        #region 模拟按键

        #region 旧API 要按需修改 但KeyPress

        ///// <summary>
        /////     单个耗时 0.8ms
        ///// </summary>
        //public static void RightClick()
        //{
        //    KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.RightDown);
        //    KeyboardMouseSimulateDriverAPI.MouseUp((uint)Dota2Simulator.MouseButtons.RightUp);
        //}

        //public static void LeftClick()
        //{
        //    KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.LeftDown);
        //    KeyboardMouseSimulateDriverAPI.MouseUp((uint)Dota2Simulator.MouseButtons.LeftUp);
        //}

        //public static void LeftDown()
        //{
        //    KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.LeftDown);
        //}

        //public static void LeftUp()
        //{
        //    KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.LeftUp);
        //}

        //public new static void KeyPress(uint key)
        //{
        //    KeyboardMouseSimulateDriverAPI.KeyDown(key);
        //    KeyboardMouseSimulateDriverAPI.KeyUp(key);
        //}

        //public new static void KeyDown(uint key)
        //{
        //    KeyboardMouseSimulateDriverAPI.KeyDown(key);
        //}
        //public new static void KeyUp(uint key)
        //{
        //    KeyboardMouseSimulateDriverAPI.KeyUp(key);
        //}

        //public new static void MouseMove(int x, int y, bool relative = false)
        //{
        //    KeyboardMouseSimulateDriverAPI.MouseMove(x, y, !relative);
        //} 

        #endregion

        public static void MouseLeftClick()
        {
            SimEnigo.MouseLeftClick();
        }

        public static void MouseRightClick()
        {
            SimEnigo.MouseRightClick();
        }

        public static void MouseLeftUp()
        {
            SimEnigo.MouseLeftUp();
        }

        public static void MouseLeftDown()
        {
            SimEnigo.MouseLeftDown();
        }

        public static void MouseRightUp()
        {
            SimEnigo.MouseRightUp();
        }

        public static void MouseRightDown()
        {
            SimEnigo.MouseRightDown();
        }

        /// <summary>
        ///     单个耗时 1-3ms
        /// </summary>
        /// <param name="key"></param>
        public static void KeyPress(Keys key)
        {
            SimEnigo.KeyPress(key);
        }

        public static void KeyUp(Keys key)
        {
            SimEnigo.KeyUp(key);
        }

        public static void KeyDown(Keys key)
        {
            SimEnigo.KeyDown(key);
        }

        /// <summary>
        ///     优化原先逻辑，原先默认key_click 需要等待30ms，现取消
        /// </summary>
        /// <param name="key"></param>
        /// <param name="key1">Keys.LShift</param>
        public static void KeyPressWhile(Keys key, Keys key1)
        {
            SimEnigo.KeyPressWhile(key, key1);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="key1">Keys.LShift</param>
        /// <param name="key1">Keys.Alt</param>
        public static void KeyPressWhileTwo(Keys key, Keys key1, Keys key2)
        {
            SimEnigo.KeyPressWhileTwo(key, key1, key2);
        }

        /// <summary>
        ///     优化原先逻辑，原先默认key_click 需要等待30ms，现取消
        /// </summary>
        /// <param name="key"></param>
        public static void KeyPressAlt(Keys key)
        {
            SimEnigo.KeyPressAlt(key);
        }

        public static void MouseMove(int x, int y, bool relative = false)
        {
            SimEnigo.MouseMove(x, y, relative ? 1 : 0);
        }

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