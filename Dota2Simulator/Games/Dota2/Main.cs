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

        // Phase 9 B：截图模式 / 坐标偏移 / 等待延迟 const 迁 GameAutomation.Domain.GameLayout。
        // 各 Strategy 内若用 `等待延迟` 已 local 复刻。

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

        // Phase 9 B：buff/debuff/命石 Rectangle 在 Main.cs 内 0 使用（各 Strategy 已 local 复刻），整块砍。

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
        /// Phase 8 C3 过渡 service locator：AppContainer 装配后注入 SessionState 单例；
        /// 取代原 static `_中断条件` 字段——所有读写经此 ref 走 SessionState.IsPaused。
        /// C5/C6 ItemEngine/HeroLoopHost 实例化后改 ctor 注入，D1 删此 field。
        /// </summary>
        internal static GameAutomation.Application.SessionState? _session;

        /// <summary>当前英雄的运行态聚合（技能槽 + 条件槽 + 攻击计时）。
        /// public 因 Item.cs 跨类访问。
        /// C6 单阶段：不再类型加载期 default-ctor 实例化（HeroAggregate ctor 现接 vision），由 AppContainer ctor 内 new HeroAggregate(Vision) 赋值。</summary>
        public static HeroAggregate _聚合 = null!;

        #endregion

        #endregion

        #region 模式

        /// <summary>_全局模式e 的并发锁，沿用原 Main.cs 设计。</summary>
        private static readonly Lock _全局模式e_lock = new();

        #endregion

        #region 获取指定位置颜色

        public static Task<Color> 获取指定位置颜色(int x, int y)
        {
            _ = 获取图片_2();

            return Task.FromResult(GlobalScreenCapture.GetColor(x, y));
        }

        public static Color 获取指定位置颜色(int x, int y, in ImageHandle 句柄)
        {
            return ImageManager.GetColor(in 句柄, x - GameLayout.OffsetX, y - GameLayout.OffsetY);
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
                    if (_session!.IsPaused)
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

                if (ImageFinder.FindImageInRegionBool(句柄, GlobalScreenCapture.GetCurrentHandle(), Item.获取物品范围(Main._聚合.SkillCount)))
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
            _session!.IsPaused = false;

            // _丢装备条件 = false;

            _聚合.Conditions.Reset();

            _ = Item.重置耗蓝物品委托和条件();

            Main._聚合.LegSwap.条件开启切假腿 = false;
            Main._聚合.LegSwap.条件保持假腿 = false;
            Main._聚合.LegSwap.条件假腿敏捷 = false;
            Main._聚合.LegSwap.切假腿中 = false;
            Main._聚合.LegSwap.需要切假腿 = false;

            Main._聚合.HasAghanim = false;  // 原 Item._是否神杖
            Main._聚合.HasShard = false;    // 原 Item._是否魔晶

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

            执行屏幕捕捉捕捉(GameLayout.截图模式1Reg);

            return true;
        }

        public static bool 获取图片_2()
        {
            执行屏幕捕捉捕捉(GameLayout.截图模式2Reg);

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