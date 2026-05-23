// Games/Dota2/Main.cs
#if DOTA2

using Collections.Pooled;
using Dota2Simulator.Vision;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain;
using Dota2Simulator.GameAutomation.Domain.Combat;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.KeyboardMouse;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Core.Tokens;

namespace Dota2Simulator.Games.Dota2
{
    internal class Main
    {
        #region 全局变量

        /// dota 2 适配7.36 1920 * 1080 动态肖像 法线贴图 地面视差 主界面高画质 计算器渲染器 纹理、特效、阴影 中 渲染 70% 高级水面效果
        /// 最主要就是主界面高画质,其他没事
        private const int 截图模式1X = 671;
        private const int 截图模式1Y = 727;
        private const int 截图模式1W = 760;
        private const int 截图模式1H = 346;
        private static Rectangle 截图模式1Reg = new(截图模式1X, 截图模式1Y, 截图模式1W, 截图模式1H);

        private const int 截图模式2X = 0;
        private const int 截图模式2Y = 0;
        private const int 截图模式2W = 1920;
        private const int 截图模式2H = 1080;
        private static Rectangle 截图模式2Reg = new(截图模式2X, 截图模式2Y, 截图模式2W, 截图模式2H);

        private const int 等待延迟 = 33;



        // 根据截图模式不同，用于判断技能 和 物品的具体x, y值随之变化 涉及到 全局假腿，技能位置，使用物品位置，判断神杖、魔晶 中立道具
        public static int 坐标偏移x;
        public static int 坐标偏移y;



        public static readonly Dictionary<Keys, Action> 按键匹配条件更新 = new()
        {
            { Keys.Z, () => _聚合.Conditions[ConditionSlotKey.Z].Active = true },
            { Keys.X, () => _聚合.Conditions[ConditionSlotKey.X].Active = true },
            { Keys.C, () => _聚合.Conditions[ConditionSlotKey.C].Active = true },
            { Keys.V, () => _聚合.Conditions[ConditionSlotKey.V].Active = true },
            { Keys.B, () => _聚合.Conditions[ConditionSlotKey.B].Active = true },
            { Keys.Space, () => _聚合.Conditions[ConditionSlotKey.Space].Active = true }
        };

        #region 循环用到

        /// <summary>
        ///     循环条件
        /// </summary>
        private static bool _总循环条件;

        // 创建全局截图缓冲区
        private static byte[] _initialData = new byte[1920 * 1080 * 4];

        /// <summary>
        ///     1080p 增益状态栏 
        ///     <para>12个buff最多 单buff36像素 间隔9像素</para> 962, 857, 526, 80
        ///     <para>升级框会改变buff位置</para>
        /// </summary>
        private static Rectangle buff状态技能栏 = new(962, 826, 526, 80);

        /// <summary>
        ///     1080p 增益状态栏 
        ///     <para>12个buff最多 单buff36像素 间隔9像素</para> 962, 857, 526, 80
        ///     <para>升级框会改变buff位置</para>
        /// </summary>
        private static Rectangle debuff状态技能栏 = new(435, 826, 526, 80);

        /// <summary>
        ///     1080p 命石范围 6技能最左738 4技能最右807 单个25*25大小
        /// </summary>
        private static Rectangle 命石区域 = new(738, 945, 70, 26);


        /// <summary>
        ///     用于记录缓存全局图像句柄
        /// </summary>
        private static readonly Dictionary<string, ImageHandle> _缓存图像句柄 = new();

        /// <summary>
        ///     获取图片委托
        /// </summary>
        /// <returns></returns>
        private delegate bool 截图();

        /// <summary>
        ///     获取图片委托
        /// </summary>
        private static 截图 _循环内获取图片;

        /// <summary>
        ///     中断条件布尔
        /// </summary>
        public static bool _中断条件;

        /// <summary>当前英雄的运行态聚合（技能槽 + 条件槽 + 攻击计时）。
        /// public 因 Item.cs 跨类访问。</summary>
        public static readonly HeroAggregate _聚合 = new();

        #endregion

        #endregion

        #region 模式

        /// <summary>_全局模式e 的并发锁，沿用原 Main.cs 设计。</summary>
        private static readonly Lock _全局模式e_lock = new();

        #endregion


        /// <summary>
        ///     分辨率 3840*2160
        /// </summary>
        /// <param name="i"></param>
        private static void 切配装通用(int i)
        {
            Point p = Control.MousePosition;
            int x, y;
            // x295,483  678 + 174 * Math.Floor((n - 1) / 2)
            if (i % 2 == 0)
                x = 483;
            else
                x = 295;

            y = 678 + 174 * (int)Math.Floor((double)(i - 1) / 2);

            命运2按键(Keys.F1);
            Common.Delay(600);
            命运2按键(Keys.Left);
            SimKeyBoard.MouseMove(x, y);
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(25);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(25);
            SimKeyBoard.MouseLeftClick();
            命运2按键(Keys.F1);
            SimKeyBoard.MouseMove(p);
        }

        /// <summary>
        ///     耗时10ms左右 延迟 原先50ms
        ///     CTRL SHIFT 键无法模拟？
        /// </summary>
        /// <param name="Key"></param>
        private static void 命运2按键(Keys Key)
        {
            SimKeyBoard.KeyDown(Key);
            Common.Delay(10); // 命运2操作延迟不然不切
            SimKeyBoard.KeyUp(Key);
        }

        private static void 命运2急切()
        {
            Common.Delay(400);
            SimKeyBoard.MouseLeftDown();
            Common.Delay(10);
            SimKeyBoard.MouseLeftUp();
            Task.Run(() =>
            {
                Common.Delay(2700);
                TTS.TTS.Speak("好");
            }).ConfigureAwait(false);
            命运2按键(Keys.LShiftKey);
            命运2按键(Keys.D2);
            Common.Delay(200);
            命运2按键(Keys.Space);
        }

        private static void 命运2冰好耶()
        {
            SimKeyBoard.KeyDown(Keys.W);
            命运2按键(Keys.LShiftKey);
            Common.Delay(150);
            SimKeyBoard.KeyUp(Keys.W);
            命运2按键(Keys.LControlKey);
            Common.Delay(150);
            命运2按键(Keys.C);
            Common.Delay(100);
            SimKeyBoard.KeyDown(Keys.S);
            Common.Delay(500);
            SimKeyBoard.KeyUp(Keys.S);
        }

        private static void 命运2冰好耶1()
        {
            SimKeyBoard.KeyDown(Keys.W);
            命运2按键(Keys.LShiftKey);
            Common.Delay(150);
            SimKeyBoard.KeyUp(Keys.W);
            命运2按键(Keys.LControlKey);
            Common.Delay(150);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(100);
            SimKeyBoard.KeyDown(Keys.S);
            Common.Delay(500);
            SimKeyBoard.KeyUp(Keys.S);
        }

        private static void 删除角色()
        {
            // 移动 1341,658 位置 长按 f 10秒
            // AFD鼠标右键 长按回车 10秒
            // 1341,658 位置
            // 1344,349 位置 
            // 1189,873 位置
            // 1372,443 位置
            // 182,612 位置
            // 1475,860 位置
            // esc 3次
            // 724,559 位置 单

            // 删除角色

            SimKeyBoard.MouseMove(1341, 658);
            Common.Delay(1200);

            SimKeyBoard.KeyDown(Keys.F);
            Common.Delay(4500);
            SimKeyBoard.KeyUp(Keys.F);

            Common.Delay(500);
            确认删除();
        }

        public static void 确认删除()
        {
            SimKeyBoard.KeyDown(Keys.A);
            Common.Delay(100);
            SimKeyBoard.KeyUp(Keys.A);
            Common.Delay(100);
            SimKeyBoard.KeyDown(Keys.A);
            Common.Delay(100);
            SimKeyBoard.KeyUp(Keys.A);
            Common.Delay(100);
            SimKeyBoard.KeyDown(Keys.F);
            Common.Delay(100);
            SimKeyBoard.KeyUp(Keys.F);
            Common.Delay(100);
            SimKeyBoard.KeyDown(Keys.F);
            Common.Delay(100);
            SimKeyBoard.KeyUp(Keys.F);
            Common.Delay(100);
            SimKeyBoard.KeyDown(Keys.D);
            Common.Delay(100);
            SimKeyBoard.KeyUp(Keys.D);
            Common.Delay(100);
            SimKeyBoard.KeyDown(Keys.D);
            Common.Delay(100);
            SimKeyBoard.KeyUp(Keys.D);
            Common.Delay(100);
            SimKeyBoard.MouseRightClick();
            Common.Delay(100);
            SimKeyBoard.MouseRightClick();
            Common.Delay(100);

            SimKeyBoard.KeyDown(Keys.Enter);
            Common.Delay(4500);
            SimKeyBoard.KeyUp(Keys.Enter);
        }

        public static void 创建角色()
        {
            // 创建角色
            SimKeyBoard.MouseMove(1341, 658);
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();

            Common.Delay(500);

            SimKeyBoard.MouseMove(1344, 349);
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();

            Common.Delay(500);

            SimKeyBoard.MouseMove(1189, 873);
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();
        }

        public static void 领取()
        {
            // 进入老角色

            SimKeyBoard.MouseMove(1372, 443);
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();

            Common.Delay(3000);

            // 门户

            SimKeyBoard.MouseMove(1128, 823);
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();

            Common.Delay(1200);

            // 武器周

            SimKeyBoard.MouseMove(182, 612);
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();

            Common.Delay(800);

            // 领取

            SimKeyBoard.MouseMove(1475, 860);
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();
        }

        public static void 退出()
        {
            // 退出

            SimKeyBoard.KeyPress(Keys.Escape);
            Common.Delay(500);
            SimKeyBoard.KeyPress(Keys.Escape);
            Common.Delay(500);
            SimKeyBoard.KeyPress(Keys.Escape);

            Common.Delay(1000);

            // 更换角色
            SimKeyBoard.MouseMove(724, 589);
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();
            Common.Delay(300);
            SimKeyBoard.MouseLeftClick();

            Common.Delay(300);

            SimKeyBoard.KeyPress(Keys.Enter);
        }

        #region 获取指定位置颜色

        public static Task<Color> 获取指定位置颜色(int x, int y)
        {
            _ = 获取图片_2();

            return Task.FromResult(GlobalScreenCapture.GetColor(x, y));
        }

        public static Color 获取指定位置颜色(int x, int y, in ImageHandle 句柄)
        {
            return ImageManager.GetColor(in 句柄, x - 坐标偏移x, y - 坐标偏移y);
        }

        #endregion

        #region 续走A 走A去等待后摇（保留：Item.cs 引用）

        /// <summary>
        ///     根据攻速去后摇
        /// </summary>
        /// <param name="额外攻速"></param>
        /// <returns></returns>
        private static void 走A去等待后摇()
        {
            SpinWait spinWait = new();
            bool 等待平A = false;
            while (true)
            {
                if (!_聚合.Attack.开启走A)
                {
                    return;
                }

                if (_聚合.Attack.停止走A > 0)
                {
                    _聚合.Attack.停止走A = 0;
                    break;
                }

                long currentTime = Common.获取当前时间毫秒();

                long remainingTime;
                if (!等待平A)
                {
                    // A+等待前摇 = 实际出手时间， 实际出手时间+等待间隔 = 下一次攻击开始时间 转身速率0.7 转180°157ms延时 
                    remainingTime = _聚合.Attack.实际出手时间 - currentTime + 180;

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
                        if (remainingTime <= 0)
                        {
                            SimKeyBoard.KeyPress(Keys.M);
                            等待平A = true;
                            continue;
                        }

                        spinWait.SpinOnce(); // SpinWait for very short intervals
                    }
                }
                else
                {
                    // 攻击前摇算在攻击间隔里面
                    remainingTime = _聚合.Attack.实际出手时间 + _聚合.Attack.实际攻击间隔 - _聚合.Attack.实际攻击前摇 - currentTime;

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
                        if (remainingTime <= 0)
                        {
                            SimKeyBoard.KeyPress(Keys.A);
                            break;
                        }

                        spinWait.SpinOnce(); // SpinWait for very short intervals
                    }
                }
            }
        }

        #endregion

        #region 测试_捕捉颜色

        #region 延时测试

        private static void 测试其他功能()
        {
            _聚合.Skills.SetTime(SlotKey.Global, Common.获取当前时间毫秒());

            //using (var duplicator = new DesktopDuplicator())
            //{
            //    for (int i = 0; i < 3; i++)
            //    {
            //        duplicator.Capture(new Rectangle(截图模式1X, 截图模式1Y, 截图模式1W, 截图模式1H));
            //    }
            //}

            //var i1 = Convert.ToInt32(tb_攻速.Text.Trim());

            Item.保存当前物品();

            Common.UiInvoker?.Invoke(() => Common.UiInvoker.SetText(UiField.Y, (Common.获取当前时间毫秒() - _聚合.Skills.Time(SlotKey.Global)).ToString(CultureInfo.InvariantCulture)));

            TTS.TTS.Speak("完成");
        }

        #endregion

        #region 旧找色

        /***
         *  旧使用说明
         *  物品 4 1118 943 1309 1034
         *  物品 5 1134 943 1324 1034
         *  物品 6 1162 943 1353 1034
            代表	q左	q右	w左	w右	e左	e右	r左	r右
            宙斯	799	856	864	921	929	986	994	1051
            827.5		892.5		957.5		1022.5
            代表	q左	q右	w左	w右	e左	e右	d左	d右	r左	r右
            宙斯A杖	784	837	842	895	900	953	958	1011	1016	1069
            810.5		868.5		926.5		984.5		1042.5
            代表	q左	q右	w左	w右	e左	e右	d左	d右	f左	f右	r左	r右
            小松鼠魔晶A杖	754	807	812	865	870	923	928	981	986	1039	1044	1097
            780.5		838.5		896.5		954.5		1012.5		1070.5

            5、6 技能
            技能坐标 右上角 +3 边框长度

            沉默 恐惧 不能释放 颜色 吹风等其他控制依旧是原色
            25,29,32

            默认颜色 不释放 释放中 都不变色
            变回改色说明已可以释放，优先级高于白色刷新，且后续不变色
            45,52,59

            4技能
            技能坐标 右上角 +4 边框长度

            沉默 恐惧 不能释放 颜色 吹风等其他控制依旧是原色
            14, 18, 20

            默认颜色 不释放 释放中 都不变色
            改变颜色说明已经释放，只要释放出后颜色就会改变无关框大小
            变回改色说明已可以释放，优先级高于白色刷新，且后续不变色
            4 技能 65, 74, 81
            5 6 技能 45, 52, 59

            4技能 67, 76, 84 最右上角（性能优化）
            5 6 技能 55, 62, 70  54, 61, 69 最右上角（性能优化）

            状态颜色 11,17,8 未开启 0,129,0 已开启

            逻辑上是简单而且通用，但延时方面稍微逊色（优化后改善）
            新通过顶点颜色匹配，获取原来的颜色，然后判断是否释放颜色

            4释放颜色，为起始颜色
            new R = R * R * 0.0001 + 0.7667 * R 颜色容差2.5065 保险3
            new G = G * G * 0.0014 + 0.0251 * G 颜色容差2.0513 保险3
            new B = G * G * 0.0002 + 0.7586 * G 颜色容差2.8620 保险3

            比技能CD更加有用，判断更快，6技能通用

            技能图标右侧几个像素颜色，此颜色为CD就绪，反之进入CD
            数组找色基本不耗时间

            主要还是看实现，上面的已经很大的变化了
         ***/

        #endregion

        #endregion

        #region 通用循环

        #region 循环

        private static async Task 一般程序循环()
        {
            const int 主循环间隔 = 1;
            const int 中断检查间隔 = 1;

            while (_总循环条件)
            {
                try
                {
                    if (_中断条件)
                    {
                        await Task.Delay(中断检查间隔).ConfigureAwait(true);
                        continue;
                    }

                    if (_循环内获取图片 is null)
                    {
                        await Task.Delay(主循环间隔).ConfigureAwait(true);
                        continue;
                    }

                    // 获取图片数据
                    _循环内获取图片();

                    // 处理命石相关逻辑
                    if (_聚合.Conditions.StoneProbe is not null)
                    {
                        await _聚合.Conditions.StoneProbe(GlobalScreenCapture.GetCurrentHandle()).ConfigureAwait(true);
                    }

                    // 获取技能颜色信息
                    Skill.DOTA2获取所有释放技能前颜色(GlobalScreenCapture.GetCurrentHandle());

                    // 核心逻辑：处理条件更新，包括运行期外部修改
                    await _聚合.Conditions.TickAsync().ConfigureAwait(true);

                    // 处理假腿切换逻辑
                    await 处理假腿切换().ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    Common.Main_Logger.Error($"主循环异常: {ex.Message}");
                }

                await Task.Delay(主循环间隔).ConfigureAwait(true);
            }
        }

        private static async Task 处理假腿切换()
        {
            if (Main._聚合.LegSwap.条件保持假腿 && Main._聚合.LegSwap.条件开启切假腿 && Main._聚合.LegSwap.需要切假腿)
            {
                await 切假腿处理(Main._聚合.LegSwap.条件假腿敏捷 ? "敏捷" : "力量").ConfigureAwait(true);
            }
        }

        private static async Task 切假腿处理(string 假腿类型)
        {
            if (Main._聚合.LegSwap.切假腿中)
            {
                return;
            }

            await Task.Run(async () =>
            {
                ImageHandle 句柄 = 假腿类型 switch
                {
                    "敏捷" => Dota2_Pictrue.物品.假腿_敏捷腿,
                    "力量" => Dota2_Pictrue.物品.假腿_力量腿,
                    _ => throw new NotImplementedException()
                };

                if (ImageFinder.FindImageInRegionBool(句柄, GlobalScreenCapture.GetCurrentHandle(), Item.获取物品范围(Skill._技能数量)))
                {
                    return;
                }

                Main._聚合.LegSwap.切假腿中 = true;
                _ = await Item.切假腿类型(假腿类型).ConfigureAwait(true);
                await Task.Run(() =>
                {
                    Common.Delay(100);
                    Main._聚合.LegSwap.切假腿中 = false;
                    Main._聚合.LegSwap.需要切假腿 = false; // 切假腿完毕，无需再切
                }).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        // internal：GameSession 策略路径首次激活英雄后调此启动主循环（一般程序循环）。
        // _总循环条件 守卫由调用方负责（策略路径在 OnActivate 前已置位，与旧 case 块一致）。
        internal static async Task 状态初始化()
        {
#if Silt
            _聚合.Conditions[ConditionSlotKey.C8].Probe ??= Silt.Main.有书吃书;
            _聚合.Conditions[ConditionSlotKey.C8].Active = true;
#endif
            _循环内获取图片 ??= 获取图片_2;
            await 一般程序循环().ConfigureAwait(true);
        }

        #region 取消所有功能

        public static void 取消所有功能()
        {
            _总循环条件 = false;
            _循环内获取图片 = null;
            _中断条件 = false;

            // _丢装备条件 = false;

            _聚合.Conditions.Reset();

            _ = Item.重置耗蓝物品委托和条件();

            Main._聚合.LegSwap.条件开启切假腿 = false;
            Main._聚合.LegSwap.条件保持假腿 = false;
            Main._聚合.LegSwap.条件假腿敏捷 = false;
            Main._聚合.LegSwap.切假腿中 = false;
            Main._聚合.LegSwap.需要切假腿 = false;

            Item._是否魔晶 = false;
            Item._是否神杖 = false;

            _聚合.Skills.SetStep(SlotKey.Global, 0);
            _聚合.Skills.SetStep(SlotKey.Q, 0);
            _聚合.Skills.SetStep(SlotKey.W, 0);
            _聚合.Skills.SetStep(SlotKey.E, 0);
            _聚合.Skills.SetStep(SlotKey.R, 0);
            _聚合.Skills.SetStep(SlotKey.D, 0);
            _聚合.Skills.SetStep(SlotKey.F, 0);

            _聚合.Skills.SetMode(SlotKey.Global, 0);
            _聚合.Skills.SetMode(SlotKey.Q, 0);
            _聚合.Skills.SetMode(SlotKey.W, 0);
            _聚合.Skills.SetMode(SlotKey.E, 0);
            _聚合.Skills.SetMode(SlotKey.R, 0);
            _聚合.Skills.SetMode(SlotKey.D, 0);
            _聚合.Skills.SetMode(SlotKey.F, 0);

            _聚合.Skills.SetTime(SlotKey.Global, -1);
            _聚合.Skills.SetTime(SlotKey.Q, -1);
            _聚合.Skills.SetTime(SlotKey.W, -1);
            _聚合.Skills.SetTime(SlotKey.E, -1);
            _聚合.Skills.SetTime(SlotKey.R, -1);
            _聚合.Skills.SetTime(SlotKey.D, -1);
            _聚合.Skills.SetTime(SlotKey.F, -1);

            _聚合.Attack.Reset();

            _聚合.Skills.SetTarget(SlotKey.Global, new Point(0, 0));
            _聚合.Skills.SetTarget(SlotKey.Q, new Point(0, 0));
            _聚合.Skills.SetTarget(SlotKey.W, new Point(0, 0));
            _聚合.Skills.SetTarget(SlotKey.E, new Point(0, 0));
            _聚合.Skills.SetTarget(SlotKey.R, new Point(0, 0));
            _聚合.Skills.SetTarget(SlotKey.D, new Point(0, 0));
            _聚合.Skills.SetTarget(SlotKey.F, new Point(0, 0));

            Skill.重置所有技能判断();

            // 重置切假腿配置
            Main._聚合.LegSwap.配置 = new 技能切假腿配置();
            Main._聚合.LegSwap.假腿按键 = Keys.Escape;


        }

        #endregion

        #region 获取图片

        /// <summary>
        ///     初始化全屏捕捉
        ///     <para>图像信息赋值给GlobalScreenCapture.GetCurrentHandle()</para>
        /// </summary>
        /// <param name="rectangle">截图区域,自动调整偏移量</param>
        private static void 执行屏幕捕捉捕捉(in Rectangle rectangle)
        {
            GlobalScreenCapture.CaptureScreen(rectangle);
        }

        /// <summary>
        ///     获取时间为6.92ms，占程序的大头
        /// </summary>
        /// <returns></returns>
        private static bool 获取图片_1()
        {
            // 最新通过DesktopDuplicator 实现更快速的全屏捕获
            // 671 856 760 217 基本所有技能状态物品 .net9 6-7ms 延迟。
            // 671 727 760 346 所有技能状态物品加上施法状态 .net9 6-7ms fps142+
            // 初始化全局图像和数组
            //_全局图像 ??= new Bitmap(截图模式1W, 截图模式1H);

            /* 
            if (GlobalScreenCapture.GetCurrentHandle() == null || GlobalScreenCapture.GetCurrentHandle().图片尺寸.Width + GlobalScreenCapture.GetCurrentHandle().图片尺寸.Height == 0)
            {
                GlobalScreenCapture.GetCurrentHandle() = new ImageHandle(new byte[截图模式1W * 截图模式1H * 4], new Size(截图模式1W, 截图模式1H));
            }

            // 更新字节数组
            CaptureScreen_固定数组(GlobalScreenCapture.GetCurrentHandle(), 截图模式1X, 截图模式1Y);

            //// 捕获屏幕
            //CaptureScreen_固定大小(ref _全局图像, 截图模式1X, 截图模式1Y);

            //// 获取位图字节数组
            //GetBitmapByte_固定数组(in _全局图像, ref GlobalScreenCapture.GetCurrentHandle());

            // 直接返回已完成的任务，减少异步开销
            return await Task.FromResult(true).ConfigureAwait(true);
            */

            执行屏幕捕捉捕捉(截图模式1Reg);

            return true;
        }

        public static bool 获取图片_2()
        {
            执行屏幕捕捉捕捉(截图模式2Reg);

            return true;

            /*
            // 优化之后16-17ms fps 58+
            //_全局图像 ??= new Bitmap(1920, 1080);

            //if (GlobalScreenCapture.GetCurrentHandle() == null || GlobalScreenCapture.GetCurrentHandle().图片尺寸.Width == 截图模式1W || GlobalScreenCapture.GetCurrentHandle().图片尺寸.Height == 截图模式1H)
            //{
            //    GlobalScreenCapture.GetCurrentHandle() = new ImageHandle(new byte[1920 * 1080 * 4], new Size(1920, 1080));
            //}

            //// 更新字节数组
            //CaptureScreen_固定数组(GlobalScreenCapture.GetCurrentHandle(), 0, 0);

            ////// 捕获屏幕
            ////CaptureScreen_固定大小(ref _全局图像, 0, 0);

            ////// 获取位图字节数组
            ////GetBitmapByte_固定数组(in _全局图像, ref GlobalScreenCapture.GetCurrentHandle());

            //// 直接返回已完成的任务，减少异步开销
            //return await Task.FromResult(true).ConfigureAwait(true);
            */
        }

        // 清理资源
        private static void CleanupImageSystem()
        {
            // 释放所有缓存的图像
            foreach (var handle in _缓存图像句柄.Values)
            {
                ImageManager.ReleaseImage(handle);
            }
            _缓存图像句柄.Clear();

            // 释放全局图像
            if (GlobalScreenCapture.GetCurrentHandle().IsValid)
            {
                ImageManager.ReleaseImage(GlobalScreenCapture.GetCurrentHandle());
            }

            // 执行完整清理
            ImageSystemMonitor.PerformFullCleanup();
        }

        #endregion

#endregion

#endregion
    }
}

#endif