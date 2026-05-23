// Phase 11 P14: HF2 MainClass static → Hf2Engine instance 化 (同 LolEngine P12 模板).
// - class MainClass (static) → class Hf2Engine (sealed) + ctor 接 3 ports (IInputExecutor/IScreenVision/IUiInvoker).
// - public static 根据当前英雄增强 → public instance Task method.
// - 7 private static helper (HF2_补给 / HF2_救援 / HF2_背包_激光 / HF2_SOS / HF2_飞鹰_110 / HF2_飞鹰_空袭 / HF2_飞鹰_重填装) → instance method.
// - SimEnigo.X static 调用保留 (game-agnostic 键鼠驱动 facade, 未受 Phase 11.A Common 删除影响; 未来若切 IInputExecutor 由 P15 handoff 标 Phase 12 候选).
// - async Task helper 无 await → 加 #pragma 1998 压 (保 instance 模板一致).
// Form2 HF2 build 下走无参 ctor — Phase 11 P14 同 commit Form2 加 #if HF2 字段 + new + dispatch 切.
#if HF2

using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.KeyboardMouse;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.Games.HF2
{
    public sealed class Hf2Engine
    {
#pragma warning disable IDE0052 // P14 stub: ports 已就位待 Hf2Engine 真业务/迁 IInputExecutor 时填.
        private readonly IInputExecutor _input;
        private readonly IScreenVision _vision;
        private readonly IUiInvoker _ui;
#pragma warning restore IDE0052

        public Hf2Engine(IInputExecutor input, IScreenVision vision, IUiInvoker ui)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _vision = vision ?? throw new ArgumentNullException(nameof(vision));
            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        }

        /// <summary>
        /// if (e.KeyValue == (int)Keys.A && (int)e.ModifierKeys == (int)Keys.Alt) && (int)e.ModifierKeys ==
        ///     (int)Keys.Control)
        ///     ctrl + alt + A 捕获
        /// </summary>
        public Task 根据当前英雄增强(string name, KeyEventArgs e)
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
            return Task.CompletedTask;
        }

        #region 绝地潜兵2具体实现（虽然没用）
#pragma warning disable CS1998 // P14 instance 化保留原 async 签名 (与 Task.Run 调用站点签名一致); SimEnigo.KeyPress 同步阻塞, 未来若切 _input.X async 可去 pragma.

        private async Task HF2_补给()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.MouseLeftClick();
        }

        private async Task HF2_救援()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.KeyPress(Keys.Left);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.MouseLeftClick();
        }
        private async Task HF2_背包_激光()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Left);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.MouseLeftClick();
        }

        private async Task HF2_SOS()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Down);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.MouseLeftClick();
        }

        private async Task HF2_飞鹰_110()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Down);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Left);
        }

        private async Task HF2_飞鹰_空袭()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
            SimEnigo.KeyPress(Keys.Down);
            SimEnigo.KeyPress(Keys.Right);
        }
        private async Task HF2_飞鹰_重填装()
        {
            SimEnigo.KeyPress(Keys.Control);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Left);
            SimEnigo.KeyPress(Keys.Up);
            SimEnigo.KeyPress(Keys.Right);
        }
#pragma warning restore CS1998

        #endregion
    }
}

#endif
