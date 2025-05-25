#if LOL

using ImageProcessingSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.Games.LOL
{
    // TODO:后续更改
    internal class MainClass
    {

        /// LOL 最高画质 1920 * 1080
        private const int 截图模式1X = 647;
        private const int 截图模式1Y = 941;
        private const int 截图模式1W = 642;
        private const int 截图模式1H = 130;
        private const int 等待延迟 = 6;

        // 根据截图模式不同，用于判断技能 和 物品的具体x, y值随之变化
        private const int 坐标偏移x = 647;
        private const int 坐标偏移y = 941;

        public static async Task 根据当前英雄增强(string name, in KeyEventArgs e)
        {
            switch (name)
            {
                    #region 魔腾

                    case "魔腾":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 梦魇之径接平A;
                            _条件根据图片委托2 ??= 无言恐惧接梦魇之径;
                            _条件根据图片委托3 ??= 鬼影重重接无言恐惧;
                            _条件根据图片委托4 ??= 重复释放无言恐惧;
                            await 无物品状态初始化().ConfigureAwait(false);
                        }


                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.S:
                                _中断条件 = true;
                                _条件1 = false;
                                _条件2 = false;
                                _条件3 = false;
                                break;
                        }
                        break;
                    }

                #endregion

                #region 男枪

                case "男枪":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 穷途末路接平A;
                            _条件根据图片委托2 ??= 烟雾弹接平A;
                            _条件根据图片委托3 ??= 快速拔枪接平A;
                            _条件根据图片委托4 ??= 终极爆弹接平A;
                            await 无物品状态初始化().ConfigureAwait(false);
                        }


                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }
                        break;
                    }

                    #endregion
            }
        }

        #region LOL具体实现

        #region 魔腾

        private static async Task<bool> 梦魇之径接平A(ImageHandle 句柄)
        {
            // X 754 820 886 952 Y 1005 用于一次性技能释放CD
            var q4 = 获取指定位置颜色(751, 1005, in 句柄);

            static void 梦魇之径后()
            {
                RightClick();
                Delay(25);
                KeyPress(Keys.A);
            }

            if (ColorExtensions.ColorAEqualColorB(q4, 技能CD颜色, 0)) return await FromResult(true);

            梦魇之径后();
            return await FromResult(false);
        }

        private static async Task<bool> 无言恐惧接梦魇之径(ImageHandle 句柄)
        {
            // X 754 820 886 952 Y 1005 用于一次性技能释放CD
            var e4 = 获取指定位置颜色(886, 1005, in 句柄);

            static void 无言恐惧后()
            {
                RightClick();
                Delay(25);
                KeyPress(Keys.Q);
            }

            if (ColorExtensions.ColorAEqualColorB(e4, 技能CD颜色, 0)) return await FromResult(true);

            无言恐惧后();
            return await FromResult(false);
        }

        private static async Task<bool> 鬼影重重接无言恐惧(ImageHandle 句柄)
        {
            // X 950 Y 950 用于冲刺后检测
            var r4 = 获取指定位置颜色(950, 950, in 句柄);

            static void 鬼影重重后()
            {
                _条件4 = true;
            }

            if (!ColorAEqualColorB(r4, Color.FromArgb(1, 61, 97), 0)) return await FromResult(true);

            鬼影重重后();
            return await FromResult(false);
        }

        private static async Task<bool> 重复释放无言恐惧(ImageHandle 句柄)
        {
            // X 754 820 886 952 Y 1005 用于一次性技能释放CD
            var e4 = 获取指定位置颜色(886, 1005, in 句柄);

            Delay(125);
            KeyPress(Keys.E);

            if (ColorExtensions.ColorAEqualColorB(e4, 技能CD颜色, 0)) return await FromResult(true);

            return await FromResult(false);
        }

        #endregion

        #region 男枪
        private static async Task<bool> 穷途末路接平A(ImageHandle 句柄)
        {
            // X 754 820 886 952 Y 1005 用于一次性技能释放CD
            var q4 = 获取指定位置颜色(754, 1005, in 句柄);

            static void 穷途末路后()
            {
                RightClick();
                Delay(25);
                KeyPress(Keys.A);
            }

            if (ColorExtensions.ColorAEqualColorB(q4, 技能CD颜色, 0)) return await FromResult(true);

            穷途末路后();
            return await FromResult(false);
        }

        private static async Task<bool> 烟雾弹接平A(ImageHandle 句柄)
        {
            // X 754 820 886 952 Y 1005 用于一次性技能释放CD
            var w4 = 获取指定位置颜色(820, 1005, in 句柄);

            static void 烟雾弹后()
            {
                RightClick();
                Delay(25);
                KeyPress(Keys.A);
            }

            if (ColorExtensions.ColorAEqualColorB(w4, 技能CD颜色, 0)) return await FromResult(true);

            烟雾弹后();
            return await FromResult(false);
        }

        private static async Task<bool> 快速拔枪接平A(ImageHandle 句柄)
        {
            // X 754 820 886 952 Y 1005 用于一次性技能释放CD
            var e4 = 获取指定位置颜色(886, 1005, in 句柄);

            static void 快速拔枪后()
            {
                RightClick();
                Delay(25);
                KeyPress(Keys.A);
            }

            if (ColorExtensions.ColorAEqualColorB(e4, 技能CD颜色, 0)) return await FromResult(true);

            快速拔枪后();
            return await FromResult(false);
        }

        private static async Task<bool> 终极爆弹接平A(ImageHandle 句柄)
        {
            // X 754 820 886 952 Y 1005 用于一次性技能释放CD
            var r4 = 获取指定位置颜色(952, 1005, in 句柄);

            static void 终极爆弹后()
            {
                RightClick();
                Delay(25);
                KeyPress(Keys.A);
            }

            if (ColorExtensions.ColorAEqualColorB(r4, 技能CD颜色, 0)) return await FromResult(true);

            终极爆弹后();
            return await FromResult(false);
        }

        #endregion



        #endregion
    }
}

#endif
