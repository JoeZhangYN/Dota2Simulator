#if DOTA2

using ImageProcessingSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Dota2Simulator.Games.Dota2.Main;
using static Dota2Simulator.KeyboardMouse.SimKeyBoard;

namespace Dota2Simulator.Games.Dota2
{
    internal class Item
    {
        #region 全局变量
        internal class 技能切假腿配置
        {
            public 技能切假腿配置()
            {
                切假腿配置 = new Dictionary<Keys, (bool, string)>
                {
                    { Keys.Q, (true, "智力") },
                    { Keys.W, (true, "智力") },
                    { Keys.E, (true, "智力") },
                    { Keys.R, (true, "智力") },
                    { Keys.D, (false, "智力") },
                    { Keys.F, (false, "智力") },
                    { Keys.Z, (false, "智力") },
                    { Keys.X, (false, "智力") },
                    { Keys.C, (false, "智力") },
                    { Keys.V, (false, "智力") },
                    { Keys.B, (false, "智力") },
                    { Keys.Space, (false, "智力") }
                };
            }

            public Dictionary<Keys, (bool 是否激活, string 假腿类型)> 切假腿配置 { get; }

            public void 修改配置(Keys key, bool 是否激活, string 假腿类型 = "智力")
            {
                if (切假腿配置.ContainsKey(key))
                {
                    切假腿配置[key] = (是否激活, 假腿类型);
                }
                else
                {
                    切假腿配置.Add(key, (是否激活, 假腿类型));
                }
            }
        }

        public static 技能切假腿配置 _切假腿配置 = new();

        public static Keys _假腿按键 = Keys.Escape; 
        #endregion

        #region 主要逻辑

        public static async Task 根据按键判断技能释放前通用逻辑(KeyEventArgs e)
        {
            _ = await 设置当前技能数量().ConfigureAwait(true);
            _存在假腿 = 获取当前假腿按键();
            _是否神杖 = 阿哈利姆神杖(GlobalScreenCapture.GetCurrentHandle());
            _是否魔晶 = 阿哈利姆魔晶(GlobalScreenCapture.GetCurrentHandle());

            switch (e.KeyCode)
            {
                case Keys.F1:
                    _ = 重置耗蓝物品委托和条件();
                    _ = 获取当前耗蓝物品并设置切假腿();

                    break;
                case Keys.Escape:
                    _中断条件 = !_中断条件;
                    TTS.TTS.Speak($"{(_中断条件 ? "中断" : "继续")}运行");
                    break;
                case Keys.NumPad7 when _存在假腿:
                    切换假腿状态();
                    break;
                case Keys.NumPad8 when _存在假腿:
                    切换保持假腿状态();
                    break;
                case Keys.NumPad9 when _存在假腿:
                    取消所有功能();
                    break;
                case var _ when e.KeyCode == _假腿按键:
                    return;
                #region Silt
#if Silt
                case Keys.NumPad1:
                    Silt.Main.跳过循环获取金碎片(GlobalScreenCapture.GetCurrentHandle());
                    break;
                case Keys.NumPad2:
                    Silt.Main.自动屏蔽3个选项(GlobalScreenCapture.GetCurrentHandle());
                    break;
                case Keys.NumPad3:
                    break;
                case Keys.NumPad4:
                    Silt.Main.点击暴击(GlobalScreenCapture.GetCurrentHandle());
                    break;
                case Keys.NumPad5:
                    Silt.Main.点击黑皇(GlobalScreenCapture.GetCurrentHandle());
                    break;
                case Keys.NumPad6:
                    Silt.Main.沙王自动选择(GlobalScreenCapture.GetCurrentHandle());
                    break;
#endif
                #endregion
                default:
                    if (!_存在假腿)
                    {
                        return;
                    }

                    await 技能释放前切假腿(e, _切假腿配置).ConfigureAwait(true);
                    if (按键匹配条件更新.TryGetValue(e.KeyCode, out Action value))
                    {
                        value.Invoke();
                    }

                    break;
            }
            // 走A实际不行
#if false
                    else if (e.KeyCode == Keys.NumPad6)
            {
                _开启走A = !_开启走A;
                Tts.Speak(_开启走A ? "开启走A" : "关闭走A");
            }
            else if (e.KeyCode == Keys.A)
            {
                var 现在的时间 = 获取当前时间毫秒();
                if (现在的时间 - _实际出手时间 >= _实际攻击间隔 - _实际攻击前摇)
                {
                    _实际出手时间 = 现在的时间 + _实际攻击前摇;
                    Run(走A去等待后摇);
                }
            }
            else if (e.KeyCode == Keys.S)
            {
                _停止走A = 1;
            } 
#endif
        }

        private static async Task 技能释放前切假腿(KeyEventArgs e, 技能切假腿配置 配置)
        {
            if (配置.切假腿配置.TryGetValue(e.KeyCode, out (bool 是否激活, string 假腿类型) 配置值) && 配置值.是否激活)
            {
                await 技能释放前切假腿(配置值.假腿类型).ConfigureAwait(true);
            }
        }

        private static async Task 技能释放前切假腿(string 类型)
        {
            if (_条件开启切假腿 && _条件保持假腿 && _存在假腿)
            {
                _条件保持假腿 = false;
                _需要切假腿 = false;
                _ = await 切假腿类型(类型).ConfigureAwait(true);
            }
        }

        public static void 要求保持假腿()
        {
            _条件保持假腿 = _条件开启切假腿;
            _需要切假腿 = true;
        }

        private static void 切换假腿状态()
        {
            _条件假腿敏捷 = !_条件假腿敏捷;
            要求保持假腿();
            TTS.TTS.Speak(_条件假腿敏捷 ? "切敏捷" : "切力量");
        }

        private static void 切换保持假腿状态()
        {
            if (!_条件保持假腿 && _条件开启切假腿)
            {
                要求保持假腿();
            }
            else
            {
                _条件开启切假腿 = !_条件开启切假腿;
                要求保持假腿();
            }

            TTS.TTS.Speak(_条件保持假腿 ? "保持假腿" : "不保持假腿");
        }

        #endregion

        #region 使用物品

        #region Resource改版前

#if false
        /// <summary>
        ///     visual studio 改版资源浏览器，直接Bitmap没了，变成byte[]
        /// </summary>
        /// <param name="bp"></param>
        /// <param name="bts"></param>
        /// <param name="size"></param>
        /// <param name="mode"></param>
        /// <param name="matchRate"></param>
        /// <returns></returns>
        private static bool 根据图片以及类别使用物品(Bitmap bp, in ImageHandle 句柄, int mode = 4, double matchRate = 0.8)
        {
            var p = ImageFinder.FindImageBool(bp, in 句柄, matchRate);
            if ((p.X + p.Y <= 0) || (p.X == 245760) || (p.X == 143640)) return false;
            根据物品位置按键(p, mode, KeyBoardSim.KeyPress);
            return true;
        }

        /// <summary>
        ///     用时4-5ms左右
        /// </summary>
        /// <param name="bp"></param>
        /// <param name="bts"></param>
        /// <param name="size"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static bool 根据图片以及类别自我使用物品(Bitmap bp, in ImageHandle 句柄, int mode = 4)
        {
            var p = ImageFinder.FindImageBool(bp, in 句柄);
            if ((p.X + p.Y <= 0) || (p.X == 245760) || (p.X == 143640)) return false;
            根据物品位置按键(p, mode, KeyBoardSim.KeyPressAlt);
            return true;
        }

        private static bool 根据图片以及类别队列使用物品(Bitmap bp, in ImageHandle 句柄, int mode = 4)
        {
            var p = ImageFinder.FindImageBool(bp, in 句柄);
            if ((p.X + p.Y <= 0) || (p.X == 245760) || (p.X == 143640)) return false;
            根据物品位置按键(p, mode, key => KeyBoardSim.KeyPressWhile(key, Keys.Shift));
            return true;
        }

        private static bool 根据图片以及类别使用物品多次(Bitmap bp, ImageHandle 句柄, int times, int Common.Delay, int mode = 4)
        {
            var p = ImageFinder.FindImageBool(bp, in 句柄);
            if ((p.X + p.Y <= 0) || (p.X == 245760) || (p.X == 143640)) return false;

            for (var i = 0; i < times; i++)
            {
                根据物品位置按键(p, mode, KeyBoardSim.KeyPress);
                if (i == times - 1) break;

                Common.Delay(Common.Delay);
            }

            return true;
        } 
#endif

        #endregion

        #region 去掉Resource 模块化物品

        // 4技能 最上侧 943 最右侧 1195 CD颜色 104 104 104 最下侧 986 最左侧 1136 长度 44 宽度 60
        // 1136 1202 67 7 943 991 48 5 986 991 5
        // 最上侧 最右侧 用于判断物品是否可用
        // 1195 943

        public static readonly 物品信息 物品4 = new(1136,
            [
                Color.FromArgb(35, 36, 37), Color.FromArgb(34, 35, 35), Color.FromArgb(32, 32, 32),
                Color.FromArgb(35, 35, 35), Color.FromArgb(34, 33, 33), Color.FromArgb(31, 31, 32)
            ]
        );

        public static readonly 物品信息 物品5 = new(1150,
            [
                Color.FromArgb(35, 36, 37), Color.FromArgb(34, 35, 35), Color.FromArgb(32, 32, 32),
                Color.FromArgb(35, 35, 35), Color.FromArgb(34, 33, 33), Color.FromArgb(31, 31, 32)
            ]
        );

        public static readonly 物品信息 物品6 = new(1180,
            [
                Color.FromArgb(35, 36, 37), Color.FromArgb(34, 35, 35), Color.FromArgb(32, 32, 32),
                Color.FromArgb(35, 35, 35), Color.FromArgb(34, 33, 33), Color.FromArgb(31, 31, 32)
            ]
        );

        public class 物品信息(int 最左侧x, Color[] 物品锁闭颜色)
        {
            public int 物品最左侧x { get; } = 最左侧x;
            public int 物品最上侧y { get; } = 943; // 为灰色那条线的高度
            private int 物品长度 { get; } = 60;
            private int 物品宽度 { get; } = 44;
            private int 物品左右间隔 { get; } = 5;
            private int 物品上下间隔 { get; } = 4;
            public int 物品间隔x { get; } = 62 + 5; // 图片宽度 + 中间间隔
            public int 物品间隔y { get; } = 45 + 3; // 图片长度 + 中间间隔
            public int 物品CD右上角x { get; } = 最左侧x + 60 - 1; // CD好的时候物品框的右上方
            public int 物品CD右上角y { get; } = 943; // 为灰色那条线的高度
            public Color 物品CD颜色 { get; } = Color.FromArgb(104, 104, 104);
            public byte 物品CD颜色容差 { get; } = 1; // 0
            public int 物品锁闭x { get; } = 最左侧x + 61; // 锁闭框最右方
            public int 物品锁闭y { get; } = 989; // 锁闭框下方
            public Color[] 物品锁闭颜色 { get; } = 物品锁闭颜色; // 因为无物品时，无锁闭情况，但其他物品锁闭
            public byte 物品锁闭颜色容差 { get; }
            public Keys[] 物品位置 { get; } = [Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.Space];
            public Rectangle 物品范围 { get; } = new Rectangle(最左侧x, 943, 67 * 3, 48 * 2);
            public Rectangle 中立TP范围 { get; }= new Rectangle(最左侧x + 197, 968, 47, 101); // 6技能 1377,968,47,101 1180  197
        }

        private static bool 判断物品状态(物品信息 物品, int 序号, in ImageHandle 句柄, Point 初始位置, Color 目标颜色, byte 颜色容差)
        {
            Point 位置 = new(初始位置.X - 坐标偏移x, 初始位置.Y - 坐标偏移y);

            int 内部序号 = 序号;
            if (序号 >= 3)
            {
                内部序号 -= 3;
                位置.Y += 物品.物品间隔y;
            }

            位置.X += 内部序号 * 物品.物品间隔x;

            return ColorExtensions.ColorAEqualColorB(ImageManager.GetColor(in 句柄, 位置), 目标颜色, 颜色容差);
        }

        private static bool 判断物品状态(物品信息 物品, int 序号, in ImageHandle 句柄, Point 初始位置, Color[] 目标颜色, byte 颜色容差)
        {
            Point 位置 = new(初始位置.X - 坐标偏移x, 初始位置.Y - 坐标偏移y);

            int 内部序号 = 序号;
            if (序号 >= 3)
            {
                内部序号 -= 3;
                位置.Y += 物品.物品间隔y;
            }

            位置.X += 内部序号 * 物品.物品间隔x;

            Color 获取的颜色 = ImageManager.GetColor(in 句柄, 位置);

            bool b1 = ColorExtensions.ColorAEqualColorB(获取的颜色, 目标颜色[序号], 颜色容差);
            // if (!b1) Logger.Info($"获取到物品锁闭,当前物品{序号}，目标{目标颜色[序号]} 获取{获取的颜色}"); 
            return b1;
        }

        private static bool DOTA2判断序号物品是否CD(int 序号, in ImageHandle 句柄)
        {
            物品信息 物品 = 根据技能数量获取物品信息(_技能数量);
            Point 初始位置 = new(物品.物品CD右上角x, 物品.物品CD右上角y);
            Color 目标颜色 = 物品.物品CD颜色;
            byte 颜色容差 = 物品.物品CD颜色容差;

            return 判断物品状态(物品, 序号, in 句柄, 初始位置, 目标颜色, 颜色容差);
        }

        private static bool DOTA2判断任意物品是否锁闭(in ImageHandle 句柄)
        {
            物品信息 物品 = 根据技能数量获取物品信息(_技能数量);
            Point 初始位置 = new(物品.物品锁闭x, 物品.物品锁闭y);
            Color[] 目标颜色 = 物品.物品锁闭颜色;
            byte 颜色容差 = 物品.物品锁闭颜色容差;

            for (int i = 0; i < 6; i++)
            {
                if (!判断物品状态(物品, i, in 句柄, 初始位置, 目标颜色, 颜色容差))
                {
                    return true;
                }
            }

            return false;
        }

        public static 物品信息 根据技能数量获取物品信息(int mode = 4)
        {
            return mode switch
            {
                4 => 物品4,
                5 => 物品5,
                6 => 物品6,
                _ => throw new ArgumentException("Invalid Item number")
            };
        }

        public static Rectangle 获取物品范围(int mode = 4)
        {
            return mode switch
            {
                4 => 物品4.物品范围,
                5 => 物品5.物品范围,
                6 => 物品6.物品范围,
                _ => throw new ArgumentException("Invalid Item number")
            };
        }

        public static Rectangle 获取中立TP范围(int mode = 4)
        {
            return mode switch
            {
                4 => 物品4.中立TP范围,
                5 => 物品5.中立TP范围,
                6 => 物品6.中立TP范围,
                _ => throw new ArgumentException("Invalid Item number")
            };
        }

        public static bool 重置耗蓝物品委托和条件()
        {
            _条件z = false;
            _条件x = false;
            _条件c = false;
            _条件v = false;
            _条件b = false;
            _条件space = false;
            _条件根据图片委托z = null;
            _条件根据图片委托x = null;
            _条件根据图片委托c = null;
            _条件根据图片委托v = null;
            _条件根据图片委托b = null;
            _条件根据图片委托space = null;
            return true;
        }

        private static bool 获取当前耗蓝物品并设置切假腿()
        {
            ImageHandle[] 需切假腿物品句柄 =
            [
                Dota2_Pictrue.物品.黑皇杖,
                Dota2_Pictrue.物品.疯狂面具,
                Dota2_Pictrue.物品.虚空至宝_疯狂面具,
                Dota2_Pictrue.物品.紫苑,
                Dota2_Pictrue.物品.血棘,
                Dota2_Pictrue.物品.深渊之刃,
                Dota2_Pictrue.物品.雷神之锤,
                Dota2_Pictrue.物品.虚空至宝_雷神之锤,
                Dota2_Pictrue.物品.魂戒,
                Dota2_Pictrue.物品.鱼叉,
                Dota2_Pictrue.物品.散失,
                Dota2_Pictrue.物品.散魂,
                Dota2_Pictrue.物品.希瓦,
                Dota2_Pictrue.物品.青莲宝珠,
                Dota2_Pictrue.物品.飓风长戟,
                Dota2_Pictrue.物品.红杖,
                Dota2_Pictrue.物品.红杖2,
                Dota2_Pictrue.物品.红杖3,
                Dota2_Pictrue.物品.红杖4,
                Dota2_Pictrue.物品.红杖5,
                Dota2_Pictrue.物品.刷新球,
                Dota2_Pictrue.物品.虚灵之刃
            ];

            Dictionary<Keys, Action> 物品进入CD委托 = new()
            {
                { Keys.Z, () => _条件根据图片委托z ??= 物品z进入CD },
                { Keys.X, () => _条件根据图片委托x ??= 物品x进入CD },
                { Keys.C, () => _条件根据图片委托c ??= 物品c进入CD },
                { Keys.V, () => _条件根据图片委托v ??= 物品v进入CD },
                { Keys.B, () => _条件根据图片委托b ??= 物品b进入CD },
                { Keys.Space, () => _条件根据图片委托space ??= 物品space进入CD }
            };

            foreach (ImageHandle 匹配句柄 in 需切假腿物品句柄)
            {
                Keys key = 根据图片获取物品按键(匹配句柄);
                if (key != Keys.Escape && 物品进入CD委托.TryGetValue(key, out Action value))
                {
                    if (匹配句柄.Id == Dota2_Pictrue.物品.魂戒.Id)
                    {
                        _切假腿配置.修改配置(key, true, "力量");
                    }
                    else
                    {
                        _切假腿配置.修改配置(key, true);
                    }

                    value.Invoke();
                }
            }

            return true;
        }

        /*
        //// 定义一个结构体来表示颜色检查项
        //private struct ColorCheck
        //{
        //    public int Num;
        //    public Rectangle OcrArea; // OCR 的区域
        //}

        //private static double 获取当前攻击速度()
        //{
        //    // 定义颜色检查项的列表
        //    List<ColorCheck> colorChecks =
        //    [
        //        new()
        //        {
        //            Num = 4,
        //            OcrArea = new Rectangle(663, 959, 28, 30)
        //        },
        //        new()
        //        {
        //            Num = 5,
        //            OcrArea = new Rectangle(647, 959, 28, 30)
        //        },
        //        new()
        //        {
        //            Num = 6,
        //            OcrArea = new Rectangle(619, 959, 28, 30)
        //        }
        //    ];

        //    foreach (ColorCheck check in colorChecks)
        //    {
        //        if (check.Num != _技能数量)
        //        {
        //            continue;
        //        }

        //        string ocrResult = PaddleOcr.获取图片文字(check.OcrArea.X, check.OcrArea.Y, check.OcrArea.Width,
        //            check.OcrArea.Height);
        //        if (double.TryParse(ocrResult, out double result))
        //        {
        //            return result;
        //        }
        //        else
        //        {
        //            return 100.0;
        //            // throw new InvalidOperationException("OCR 结果无法转换为双精度浮点数。");
        //        }
        //    }

        //    return 100.0;
        //    // 如果没有匹配的颜色，抛出异常或返回默认值
        //    // throw new InvalidOperationException("未找到匹配的颜色。");
        //}
        */

        private static bool 获取当前假腿按键()
        {
            ImageHandle[] 假腿句柄集合 =
            [
                Dota2_Pictrue.物品.假腿_力量腿,
                Dota2_Pictrue.物品.假腿_敏捷腿,
                Dota2_Pictrue.物品.假腿_智力腿
            ];

            Dictionary<Keys, (Action 清空委托, Action 重置条件)> 清空物品进入CD委托和条件映射 = new()
            {
                { Keys.Z, (() => _条件根据图片委托z = null, () => _条件z = false) },
                { Keys.X, (() => _条件根据图片委托x = null, () => _条件x = false) },
                { Keys.C, (() => _条件根据图片委托c = null, () => _条件c = false) },
                { Keys.V, (() => _条件根据图片委托v = null, () => _条件v = false) },
                { Keys.B, (() => _条件根据图片委托b = null, () => _条件b = false) },
                { Keys.Space, (() => _条件根据图片委托space = null, () => _条件space = false) }
            };

            foreach (ImageHandle 假腿句柄 in 假腿句柄集合)
            {
                Keys key = 根据图片获取物品按键(假腿句柄);
                if (key != Keys.Escape)
                {
                    _假腿按键 = key;
                    if (清空物品进入CD委托和条件映射.TryGetValue(_假腿按键, out (Action 清空委托, Action 重置条件) actions))
                    {
                        actions.清空委托.Invoke();
                        actions.重置条件.Invoke();
                    }
                    break;
                }
            }

            return _假腿按键 != Keys.Escape;
        }

        public static Keys 根据图片获取物品按键(in ImageHandle 句柄)
        {
            var 位置 = ImageFinder.FindImageInRegion(in 句柄, GlobalScreenCapture.GetCurrentHandle(), 获取物品范围(_技能数量));
            return 根据位置获取按键(位置);
        }

        public static int 根据图片使用物品(in ImageHandle 句柄)
        {
            return 执行物品操作(句柄, (k) => KeyPress(k));
        }

        public static int 根据图片自我使用物品(in ImageHandle 句柄)
        {
            return 执行物品操作(句柄, (k) => KeyPressAlt(k));
        }

        public static bool 根据图片队列使用物品(in ImageHandle 句柄)
        {
            return 执行物品操作(句柄, (k) => KeyPressWhile(k, Keys.Shift)) > 0;
        }

        public static int 根据图片多次使用物品(in ImageHandle 句柄, int times, int 延迟)
        {
            return 执行物品操作(句柄, (k) =>
            {
                for (int i = 0; i < times; i++)
                {
                    KeyPress(k);
                    if (i < times - 1)
                    {
                        Common.Delay(延迟);
                    }
                }
            });
        }

        private static int 执行物品操作(in ImageHandle 句柄, Action<Keys> 按键操作)
        {
            Point? 位置 = ImageFinder.FindImageInRegion(in 句柄, GlobalScreenCapture.GetCurrentHandle(), 获取物品范围(_技能数量));
            if (ImageManager.是否无效位置(位置))
            {
                return 0;
            }

            Keys k = 根据位置获取按键(位置);
            if (k != Keys.Escape)
            {
                按键操作(k);
                return 1;
            }
            return 0;
        }

        private static Keys 根据位置获取按键(Point? 位置)
        {
            if (!位置.HasValue || ImageManager.是否无效位置(位置))
            {
                return Keys.Escape;
            }

            物品信息 物品 = 根据技能数量获取物品信息(_技能数量);
            int x = 位置.Value.X + 坐标偏移x;
            int y = 位置.Value.Y + 坐标偏移y;

            // 计算物品在物品栏中的索引
            int index = (int)Math.Floor((float)(x - 物品.物品最左侧x) / 物品.物品间隔x);

            // 如果在第二行，索引加3
            if (y - 物品.物品最上侧y >= 物品.物品间隔y)
            {
                index += 3;
            }

            // 验证索引范围并返回对应按键
            if (index >= 0 && index < 物品.物品位置.Length)
            {
                return 物品.物品位置[index];
            }

            return Keys.Escape;
        }

        #endregion

        #endregion

        #region buff或者装备

        private const int 技能4魔晶A杖x = 1096;
        private const int 技能5魔晶A杖x = 1110;
        private const int 技能6魔晶A杖x = 1140;
        private const int 技能A杖y = 959;
        private const int 技能魔晶y = 994;

        private static bool 阿哈利姆神杖(in ImageHandle 句柄)
        {
            return 检查技能颜色(in 句柄, [技能4魔晶A杖x, 技能5魔晶A杖x, 技能6魔晶A杖x], 技能A杖y, Color.FromArgb(30, 187, 250));
        }

        private static bool 阿哈利姆魔晶(in ImageHandle 句柄)
        {
            return 检查技能颜色(in 句柄, [技能4魔晶A杖x, 技能5魔晶A杖x, 技能6魔晶A杖x], 技能魔晶y, Color.FromArgb(30, 187, 254));
        }

        /// <summary>
        ///     检查技能颜色是否匹配
        /// </summary>
        /// <param name="bts">数组</param>
        /// <param name="size">大小</param>
        /// <param name="xCoords">x坐标数组</param>
        /// <param name="yCoord">y坐标</param>
        /// <param name="技能点颜色">技能点颜色</param>
        /// <returns>是否匹配</returns>
        private static bool 检查技能颜色(in ImageHandle 句柄, int[] xCoords, int yCoord, in Color 技能点颜色)
        {
            foreach (int xCoord in xCoords)
            {
                var color = ImageManager.GetColor(in 句柄, xCoord - 坐标偏移x, yCoord - 坐标偏移y);
                if (ColorExtensions.ColorAEqualColorB(color, 技能点颜色, 1))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 物品进入CD

        private static async Task<bool> 处理物品进入CD(int 序号, ImageHandle 句柄)
        {
            if (DOTA2判断序号物品是否CD(序号, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            _ = Task.Run(() =>
            {
                Common.Delay(60);
                要求保持假腿();
            }).ConfigureAwait(false);

            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 物品z进入CD(ImageHandle 句柄)
        {
            return await 处理物品进入CD(0, 句柄).ConfigureAwait(true);
        }

        private static async Task<bool> 物品x进入CD(ImageHandle 句柄)
        {
            return await 处理物品进入CD(1, 句柄).ConfigureAwait(true);
        }

        private static async Task<bool> 物品c进入CD(ImageHandle 句柄)
        {
            return await 处理物品进入CD(2, 句柄).ConfigureAwait(true);
        }

        private static async Task<bool> 物品v进入CD(ImageHandle 句柄)
        {
            return await 处理物品进入CD(3, 句柄).ConfigureAwait(true);
        }

        private static async Task<bool> 物品b进入CD(ImageHandle 句柄)
        {
            return await 处理物品进入CD(4, 句柄).ConfigureAwait(true);
        }

        private static async Task<bool> 物品space进入CD(ImageHandle 句柄)
        {
            return await 处理物品进入CD(5, 句柄).ConfigureAwait(true);
        }

        #endregion

        #region 所有物品可使用后续
        /// <summary>
        ///     所有物品可使用后续
        ///     <para>字节数组为副本</para>
        ///     <para>为物品可释放那一刻的图片</para>
        /// </summary>
        /// <param name="序号"></param>
        /// <param name="数组">字节数组为副本，为物品解除锁闭那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>
        ///     物品被锁闭后赋值状态后返回真
        ///     <para>(如未被锁闭一直返回真)</para>
        ///     <para>解除锁闭处理逻辑后返回假</para>
        /// </returns>
        public static async Task<bool> 所有物品可用后续(ImageHandle 句柄, Action afterAction)
        {
            if (DOTA2判断任意物品是否锁闭(in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            _ = Task.Run(afterAction).ConfigureAwait(false);
            return await Task.FromResult(false).ConfigureAwait(true);
        } 
        #endregion
    }
}

#endif