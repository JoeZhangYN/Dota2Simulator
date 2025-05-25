#if HF2

using Dota2Simulator.KeyboardMouse;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.Games.HF2
{
    // TODO:后续更改
    internal class MainClass
    {
        /// <summary>
        /// if (e.KeyValue == (int)Keys.A && (int)e.ModifierKeys == (int)Keys.Alt) && (int)e.ModifierKeys ==
        ///     (int)Keys.Control)
        ///     ctrl + alt + A 捕获
        /// </summary>
        /// <param name="name"></param>
        /// <param name="e"></param>
        public static async Task 根据当前英雄增强(string name, in KeyEventArgs e)
        {
            switch (name)
            {
                case "hf2":
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.NumPad1:
                                _ = Task.Run(HF2_补给);
                                break;
                            case Keys.NumPad2:
                                _ = Task.Run(HF2_救援);
                                break;
                            case Keys.NumPad3:
                                _ = Task.Run(HF2_飞鹰_空袭);
                                break;
                            case Keys.NumPad5:
                                _ = Task.Run(HF2_飞鹰_110);
                                break;
                            case Keys.NumPad6:
                                _ = Task.Run(HF2_飞鹰_重填装);
                                break;
                        }

                        break;
                    }
            }
        }

        #region 绝地潜兵2具体实现（虽然没用）

        private static async Task HF2_补给()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.MouseLeftClick();
        }

        private static async Task HF2_救援()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.KeyPress(Keys.Left);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.MouseLeftClick();
        }
        private static async Task HF2_背包_激光()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Left);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.MouseLeftClick();
        }

        private static async Task HF2_SOS()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Down);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.MouseLeftClick();
        }

        private static async Task HF2_飞鹰_110()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Down);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Left);
        }

        private static async Task HF2_飞鹰_空袭()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.KeyPress(Keys.Down);
            SimEnigo.KeyPress(Keys.Right);
        }
        private static async Task HF2_飞鹰_重填装()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Left);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
        }

        #endregion
    }
}

#endif