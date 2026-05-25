// Phase 11 P6: Silt/Main.cs static god class → SiltEngine instance 化.
// - class Main (static) → class SiltEngine (sealed, ctor 接 4 ports: input/vision/ui/item).
// - 5 public static method → public instance method (有书吃书 / 跳过循环获取金碎片 / 自动屏蔽3个选项 / 点击黑皇 / 点击暴击 / 测试识别 / 沙王自动选择 / 初始化天赋选择).
// - private static field (已吃书 / 获取碎片循环计数 / 之前位置 / _循环时间) → instance field.
// - readonly static Rectangle 常量保持 static (纯只读, Phase 8 C6 模板).
// - 2 处 Common.ItemEngine! → _item; 3 处 Common.HeroLoopHost!.Ui → _ui.
// - 跳过重新选择 内部 static local function → instance local function.
// 调用方 (ItemEngine.cs:80-95 NumPad1-NumPad6 dispatch) 5 处 Dota2Simulator.Games.Dota2.Silt.Main.X → _silt!.X (P6 同 commit 改).
// Phase 19E: 删除内嵌死代码示例 ~200 行 (BasicImageFinding/ContinuousMonitoring/PerformanceTest/ClickAt/AdvancedExample), 同步清理无用 using (System / System.Diagnostics / System.Threading / Dota2Simulator.Vision.Rust).
#if DOTA2
#if Silt

using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Vision;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision.Ocr;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.Games;
using Color = System.Drawing.Color;
using ImageHandle = Dota2Simulator.Vision.ImageHandle;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Dota2Simulator.Games.Dota2.Silt
{
    public sealed class SiltEngine
    {
        private readonly IInputExecutor _input;
        private readonly IScreenVision _vision;
        private readonly IUiInvoker _ui;
        private readonly ItemEngine _item;

        public SiltEngine(IInputExecutor input, IScreenVision vision, IUiInvoker ui, ItemEngine item)
        {
            _input = input;
            _vision = vision;
            _ui = ui;
            _item = item;
        }

        private bool 已吃书;

        public async Task<bool> 有书吃书()
        {
            if (已吃书
                && _item.根据图片使用物品(Dota2_Pictrue.物品.书_Tpl) == 0)
            {
                SimKeyBoard.KeyPress(Keys.D2);
                已吃书 = false;
            }
            else if (_item.根据图片使用物品(Dota2_Pictrue.物品.书_Tpl) == 1)
            {
                Common.Delay(50);
                已吃书 = true;
            }
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        private readonly static Rectangle RPG选择技能范围 = new(379, 115, 1140, 153);
        private readonly static Rectangle RPG第一技能金 = new(631, 290, 60, 400);

        private int 获取碎片循环计数;
        private Point 之前位置;

        private long _循环时间;

        public void 跳过循环获取金碎片(in ImageHandle 句柄)
        {
            if (获取碎片循环计数 == 0) 之前位置 = Control.MousePosition;

            _循环时间 = Common.获取当前时间毫秒();

            if (获取碎片循环计数 > 12)
            {
                获取碎片循环计数 = 0;
                SimKeyBoard.MouseMove(之前位置);
                之前位置 = new();
                TTS.TTS.Speak("12次退出");
                var 当前英雄 = "";
                _ui.Invoke(() =>
                {
                    当前英雄 = _ui.GetText(UiField.Name).Trim();
                });
                if (当前英雄 == "飞机")
                {
                    // 需要循环需加速buff
                    SimKeyBoard.MouseMove(1332, 92);
                    Common.Delay(50);
                    SimKeyBoard.MouseLeftClick();
                }
            }
            else
            {
                var 描述 = PaddleOCR.获取图片文字(GlobalScreenCapture.GetCurrentHandle(), new Rectangle(730, 503, 71, 39)).Trim();
                // _ui.Invoke(() => _ui.SetText(UiField.阵营, 描述));
                if (_vision.Find(Dota2_Pictrue.Silt.选择天赋_Tpl, RPG选择技能范围, new MatchRate(0.9), Tolerance.Exact).Found
                    && _vision.Find(Dota2_Pictrue.Silt.普通天赋_Tpl, RPG第一技能金, new MatchRate(0.9), Tolerance.Exact).Found)
                {
                    跳过重新选择();
                }
                else if (描述 != "+45")
                {
                    跳过重新选择();
                }
                else
                {
                    获取碎片循环计数 = 0;
                }
            }

            void 跳过重新选择()
            {
                SimKeyBoard.MouseMove(768, 674);
                Common.Delay(10);
                SimKeyBoard.MouseLeftClick();
                Common.Delay(10);
                SimKeyBoard.MouseLeftClick();

                获取碎片循环计数++;
                Task.Run(() =>
                {
                    Common.Delay(325, _循环时间);
                    // 自我调用
                    SimKeyBoard.KeyPress(Keys.NumPad1);
                }).ConfigureAwait(true);
            }
        }


        public void 自动屏蔽3个选项(in ImageHandle 句柄)
        {
            if (_vision.Find(Dota2_Pictrue.Silt.选择天赋_Tpl, RPG选择技能范围, new MatchRate(0.9), Tolerance.Exact).Found)
            {
                var p = Control.MousePosition;
                // 第三个技能右上角颜色金
                if (ColorExtensions.ColorAEqualColorB(_vision.PixelAt(new ScreenPoint(1318, 296)),
                    Color.FromArgb(188, 134, 1), 10))
                {
                    SimKeyBoard.MouseMove(690, 673);
                    Common.Delay(25);
                    SimKeyBoard.MouseLeftClick();
                    Common.Delay(25);
                    SimKeyBoard.MouseMove(953, 673);
                    Common.Delay(25);
                    SimKeyBoard.MouseLeftClick();
                    Common.Delay(25);
                    SimKeyBoard.MouseMove(1205, 673);
                    Common.Delay(25);
                    SimKeyBoard.MouseLeftClick();
                    Common.Delay(25);
                    SimKeyBoard.MouseMove(540, 146);
                    Common.Delay(25);
                    SimKeyBoard.MouseLeftClick();
                    Common.Delay(25);
                }
                SimKeyBoard.MouseMove(p);
            }
        }
        public void 点击黑皇(in ImageHandle 句柄)
        {
            var p = Control.MousePosition;
            SimKeyBoard.MouseMove(40, 500);
            Common.Delay(25);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(25);
            SimKeyBoard.MouseMove(p);
        }

        public void 点击暴击(in ImageHandle 句柄)
        {
            var p = Control.MousePosition;
            SimKeyBoard.MouseMove(40, 580);
            Common.Delay(25);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(25);
            SimKeyBoard.MouseMove(p);
        }

        // Phase 19E: 删除内嵌死代码示例 ~200 行 (BasicImageFinding / ContinuousMonitoring / PerformanceTest / ClickAt / AdvancedExample)
        //  -- Vision.Rust 示例代码, 业务零调用, Phase 11 handoff_notes #2 标记 Phase 19E 候选清理.

        public void 测试识别(in ImageHandle 句柄)
        {
            // 用整屏 region 替代原全屏 FindAllImages（V5 强制裁剪：此处业务场景是 RPG 天赋扫描，整屏可接受）
            var rustResults = _vision.FindAll(Dota2_Pictrue.Silt.钢毛后背_Tpl, new ScreenRegion(0, 0, 1920, 1080), new MatchRate(0.9), Tolerance.Exact);
            var string1 = "";
            foreach (var point in rustResults)
            {
                var rege文字 = new Rectangle(point.X - 66, point.Y + 114, 212, 42);
                var rege效果 = new Rectangle(point.X - 66, point.Y + 183, 212, 42);
                var b_pick = new Point(point.X + 41, point.Y + 306);
                var b_skip = new Point(point.X + 41, point.Y + 306);
                var 描述 = PaddleOCR.获取图片文字(in 句柄, rege文字);
                var 效果 = PaddleOCR.获取图片文字(in 句柄, rege效果);
                string1 += 描述 + "\r\n" + 效果;
                SimKeyBoard.MouseMove(b_pick);
                Common.Delay(500);
                SimKeyBoard.MouseMove(b_skip);
                Common.Delay(500);
            }
            _ui.Invoke(() => _ui.SetText(UiField.阵营, string1));
            TTS.TTS.Speak("完成");
            // Tts.Speak(PaddleOCR.获取图片文字(in 句柄, new Rectangle(620, 437, 120, 40)));
        }

        public void 沙王自动选择(in ImageHandle gameHandle)
        {
            var p = Control.MousePosition;
            // Phase 11 P7: 传 _ui 穿透到 TalentSelectionExamples 消 Common.HeroLoopHost! service locator.
            // Phase 18 V4: 同样传 _vision 穿透，让 TalentSelector 内部走 IScreenVision 端口而非 GlobalScreenCapture 静态调.
            TalentSelectionExamples.ExecuteHeroSelection("沙王", gameHandle, _ui, _vision);
            SimKeyBoard.MouseMove(p);
        }

        public void 初始化天赋选择(in ImageHandle gameHandle)
        {
        }
    }
}




#endif
#endif
