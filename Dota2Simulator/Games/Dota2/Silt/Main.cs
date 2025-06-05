#if DOTA2
#if Silt

using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.PictureProcessing.OCR;
using ImageProcessingSystem;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Dota2Simulator.Games.Dota2.Item;

namespace Dota2Simulator.Games.Dota2.Silt
{
    internal class Main
    {
        private static bool 已吃书;

        public static async Task<bool> 有书吃书(ImageHandle 句柄)
        {
            if (已吃书
                && 根据图片使用物品(Dota2_Pictrue.物品.书) == 0)
            {
                SimKeyBoard.KeyPress(Keys.D2);
                已吃书 = false;
            }
            else if (根据图片使用物品(Dota2_Pictrue.物品.书) == 1)
            {
                Common.Delay(50);
                已吃书 = true;
            }
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        private readonly static Rectangle RPG选择技能范围 = new(379, 115, 1140, 153);
        private readonly static Rectangle RPG第一技能金 = new(631, 290, 60, 400);

        private static int 获取碎片循环计数;
        private static Point 之前位置;

        private static long _循环时间;

        public static void 跳过循环获取金碎片(in ImageHandle 句柄)
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
                Common.Main_Form?.Invoke(() =>
                {
                    当前英雄 = Common.Main_Form.tb_name.Text.Trim();
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
                // Common.Main_Form.Invoke(() => { Common.Main_Form.tb_阵营.Text = 描述; });
                if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Silt.选择天赋, GlobalScreenCapture.GetCurrentHandle(), RPG选择技能范围)
                    && ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Silt.普通天赋, GlobalScreenCapture.GetCurrentHandle(), RPG第一技能金))
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

            static void 跳过重新选择()
            {
                SimKeyBoard.MouseMove(768, 674);
                Common.Delay(10);
                SimKeyBoard.MouseLeftClick();
                Common.Delay(10);
                SimKeyBoard.MouseLeftClick();

                获取碎片循环计数++;
                Task.Run(() =>
                {
                    Common.Delay(350, _循环时间);
                    // 自我调用
                    SimKeyBoard.KeyPress(Keys.NumPad1);
                }).ConfigureAwait(true);
            }
        }


        public static void 自动屏蔽3个选项(in ImageHandle 句柄)
        {
            if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Silt.选择天赋, GlobalScreenCapture.GetCurrentHandle(), RPG选择技能范围))
            {
                var p = Control.MousePosition;
                // 第三个技能右上角颜色金
                if (ColorExtensions.ColorAEqualColorB(GlobalScreenCapture.GetColor(1318, 296),
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
        public static void 点击黑皇(in ImageHandle 句柄)
        {
            var p = Control.MousePosition;
            SimKeyBoard.MouseMove(40, 500);
            Common.Delay(25);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(25);
            SimKeyBoard.MouseMove(p);
        }

        public static void 点击暴击(in ImageHandle 句柄)
        {
            var p = Control.MousePosition;
            SimKeyBoard.MouseMove(40, 580);
            Common.Delay(25);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(25);
            SimKeyBoard.MouseMove(p);
        }

        public static void 测试识别(in ImageHandle 句柄)
        {
            var rustResults = GlobalScreenCapture.FindAllImages(Dota2_Pictrue.Silt.钢毛后背, 0.9, 100, 10);
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
            Common.Main_Form.Invoke(() =>
            {
                Common.Main_Form.tb_阵营.Text = string1;
            });
            TTS.TTS.Speak("完成");
            // Tts.Speak(PaddleOCR.获取图片文字(in 句柄, new Rectangle(620, 437, 120, 40)));
        }

        public static void 沙王自动选择(in ImageHandle gameHandle)
        {
            var p = Control.MousePosition;
            TalentSelectionExamples.ExecuteHeroSelection("夜魔", gameHandle);
            SimKeyBoard.MouseMove(p);
        }

        public static void 初始化天赋选择(in ImageHandle gameHandle)
        {
        }
    }
}

#endif
#endif