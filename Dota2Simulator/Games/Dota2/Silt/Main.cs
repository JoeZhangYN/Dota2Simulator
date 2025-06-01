#if Silt

using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.TTS;
using ImageProcessingSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Dota2Simulator.Games.Dota2.Main;
using static Dota2Simulator.Games.Dota2.Item;
using Dota2Simulator.PictureProcessing.Ocr;

namespace Dota2Simulator.Games.Dota2.Silt
{
    internal class Main
    {


        #region 图片

        public static readonly ImageHandle Slit_选择天赋_handle = 缓存嵌入的图片("Silt.选择天赋");
        public static readonly ImageHandle Slit_金色天赋_handle = 缓存嵌入的图片("Silt.金色天赋");
        public static readonly ImageHandle Slit_普通天赋_handle = 缓存嵌入的图片("Silt.普通天赋");


        #region 钢背

        public static readonly ImageHandle Slit_鼻涕_handle = 缓存嵌入的图片("Silt.钢背.鼻涕");
        public static readonly ImageHandle Slit_刺针扫射_handle = 缓存嵌入的图片("Silt.钢背.刺针扫射");
        public static readonly ImageHandle Slit_钢毛后背_handle = 缓存嵌入的图片("Silt.钢背.钢毛后背");
        public static readonly ImageHandle Slit_毛团_handle = 缓存嵌入的图片("Silt.钢背.毛团");
        public static readonly ImageHandle Slit_战意_handle = 缓存嵌入的图片("Silt.钢背.战意");

        #endregion


        #region 附魔

        #region 1-4

        public static readonly ImageHandle Slit_神秘_handle = 缓存嵌入的图片("Silt.附魔.神秘");
        public static readonly ImageHandle Slit_壮实_handle = 缓存嵌入的图片("Silt.附魔.壮实");
        public static readonly ImageHandle Slit_警觉_handle = 缓存嵌入的图片("Silt.附魔.警觉");
        public static readonly ImageHandle Slit_坚强_handle = 缓存嵌入的图片("Silt.附魔.坚强");
        public static readonly ImageHandle Slit_迅速_handle = 缓存嵌入的图片("Silt.附魔.迅速");

        #endregion

        #region 2-3

        public static readonly ImageHandle Slit_犀利_handle = 缓存嵌入的图片("Silt.附魔.犀利");
        // 高远
        public static readonly ImageHandle Slit_贪婪_handle = 缓存嵌入的图片("Silt.附魔.贪婪");

        #endregion

        #region 2-4

        public static readonly ImageHandle Slit_吸血鬼_handle = 缓存嵌入的图片("Silt.附魔.吸血鬼");

        #endregion

        #region 4-5

        public static readonly ImageHandle Slit_永恒_handle = 缓存嵌入的图片("Silt.附魔.永恒");
        public static readonly ImageHandle Slit_巨神_handle = 缓存嵌入的图片("Silt.附魔.巨神");
        public static readonly ImageHandle Slit_粗暴_handle = 缓存嵌入的图片("Silt.附魔.粗暴");

        #endregion

        #region 5

        public static readonly ImageHandle Slit_狂热_handle = 缓存嵌入的图片("Silt.附魔.狂热");
        public static readonly ImageHandle Slit_捷足_handle = 缓存嵌入的图片("Silt.附魔.捷足");
        public static readonly ImageHandle Slit_冒险_handle = 缓存嵌入的图片("Silt.附魔.冒险");
        public static readonly ImageHandle Slit_进化_handle = 缓存嵌入的图片("Silt.附魔.进化");
        public static readonly ImageHandle Slit_无边_handle = 缓存嵌入的图片("Silt.附魔.无边");
        public static readonly ImageHandle Slit_睿智_handle = 缓存嵌入的图片("Silt.附魔.睿智");

        #endregion


        #endregion

        #endregion

        #region Silt
#if Silt
        private static bool 已吃书;

        public static async Task<bool> 有书吃书(ImageHandle 句柄)
        {
            if (已吃书 
                && 根据图片使用物品(物品_书_handle) == 0)
            {
                SimKeyBoard.KeyPress(Keys.D2);
                已吃书 = false;
            }
            else if (根据图片使用物品(物品_书_handle) == 1)
            {
                Delay(50);
                已吃书 = true;
            }
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        private readonly static Rectangle RPG选择技能范围 = new(379, 115, 1140, 153);
        private readonly static Rectangle RPG第一技能金 = new(631, 290, 60, 400);

        private static int 获取碎片循环计数;
        private static Point 之前位置;

        public static void 跳过循环获取金碎片(in ImageHandle 句柄)
        {
            if (获取碎片循环计数 == 0) 之前位置 = Control.MousePosition;

            if (获取碎片循环计数 > 12)
            {
                获取碎片循环计数 = 0;
                SimKeyBoard.MouseMove(之前位置);
                之前位置 = new();
                Tts.Speak("12次退出");
                var 当前英雄 = "";
                Dota2.Main.Form?.Invoke(() =>
                {
                    当前英雄 = Dota2.Main.Form.tb_name.Text.Trim();
                });
                if (当前英雄 == "飞机")
                {
                    // 需要循环需加速buff
                    SimKeyBoard.MouseMove(1332, 92);
                    Delay(50);
                    SimKeyBoard.MouseLeftClick();
                }
            }
            else
            {
                if (ImageFinder.FindImageInRegionBool(in Slit_选择天赋_handle, GlobalScreenCapture.GetCurrentHandle(), RPG选择技能范围)
                    && ImageFinder.FindImageInRegionBool(in Slit_普通天赋_handle, GlobalScreenCapture.GetCurrentHandle(), RPG第一技能金))
                {
                    SimKeyBoard.MouseMove(768, 674);
                    Delay(50);
                    SimKeyBoard.MouseLeftClick();
                    Delay(50);
                    SimKeyBoard.MouseLeftClick();
                    获取碎片循环计数++;
                    Task.Run(() =>
                    {
                        Delay(250);
                        // 自我调用
                        SimKeyBoard.KeyPress(Keys.NumPad1);
                    }).ConfigureAwait(true);
                }
            }
        }


        public static void 自动屏蔽3个选项(in ImageHandle 句柄)
        {
            if (ImageFinder.FindImageInRegionBool(in Slit_选择天赋_handle, GlobalScreenCapture.GetCurrentHandle(), RPG选择技能范围))
            {
                var p = Control.MousePosition;
                // 第三个技能右上角颜色金
                if (ColorExtensions.ColorAEqualColorB(GlobalScreenCapture.GetColor(1318, 296),
                    Color.FromArgb(188, 134, 1), 10))
                {
                    SimKeyBoard.MouseMove(690, 673);
                    Delay(50);
                    SimKeyBoard.MouseLeftClick();
                    Delay(50);
                    SimKeyBoard.MouseMove(953, 673);
                    Delay(50);
                    SimKeyBoard.MouseLeftClick();
                    Delay(50);
                    SimKeyBoard.MouseMove(1205, 673);
                    Delay(50);
                    SimKeyBoard.MouseLeftClick();
                    Delay(50);
                    SimKeyBoard.MouseMove(540, 146);
                    Delay(50);
                    SimKeyBoard.MouseLeftClick();
                    Delay(50);
                }
                SimKeyBoard.MouseMove(p);
            }
        }
        public static void 点击黑皇(in ImageHandle 句柄)
        {
            var p = Control.MousePosition;
            SimKeyBoard.MouseMove(40, 500);
            Delay(25);
            SimKeyBoard.MouseLeftClick();
            Delay(25);
            SimKeyBoard.MouseMove(p);
        }

        public static void 点击暴击(in ImageHandle 句柄)
        {
            var p = Control.MousePosition;
            SimKeyBoard.MouseMove(40, 580);
            Delay(25);
            SimKeyBoard.MouseLeftClick();
            Delay(25);
            SimKeyBoard.MouseMove(p);
        }

        public static void 测试识别(in ImageHandle 句柄)
        {
            var rustResults = GlobalScreenCapture.FindAllImagesOptimized(Slit_钢毛后背_handle, 0.9, 100, 10);
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
                Delay(500);
                SimKeyBoard.MouseMove(b_skip);
                Delay(500);
            }
            Dota2.Main.Form.Invoke(() =>
            {
                Dota2.Main.Form.tb_阵营.Text = string1;
            });
            Tts.Speak("完成");
            // Tts.Speak(PaddleOCR.获取图片文字(in 句柄, new Rectangle(620, 437, 120, 40)));
        }
#endif
        #endregion
    }
}

#endif
