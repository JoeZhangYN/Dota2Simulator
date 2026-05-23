// GameAutomation/Application/HeroLoopHost.cs
// Phase 9 C: 原 static class Games.Dota2.Main (主循环 + 截屏 + 取消所有功能 + 走A 等) → 实例化 HeroLoopHost。
// - 可变状态字段（_总循环条件 / _initialData / _缓存图像句柄 / _循环内获取图片 / _全局模式e_lock）改实例字段。
// - 7 端口 ctor 注入: (IInputExecutor, IScreenVision, IUiInvoker, HeroAggregate, SessionState, SkillEngine, ItemEngine)。
// - 替换: SimKeyBoard.X → _input.X; Skill.X → _skill.X; Item.X → _item.X。
// - GlobalScreenCapture / ImageFinder / ImageManager / ImageSystemMonitor 保留 static helper (不在本 epic 切走)。
// C 阶段 Main.cs 降级为 thin facade（_聚合 / _session / 按键匹配条件更新 + 5 方法转发 Common.HeroLoopHost），
// D 阶段 92 策略 + 上层 5 站点改 _main.X / Common.HeroLoopHost.X，F 阶段删 Main.cs facade。
#if DOTA2

using Dota2Simulator.GameAutomation.Domain;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Combat;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.GameAutomation.Application
{
    public sealed class HeroLoopHost
    {
        private readonly IInputExecutor _input;
        private readonly IScreenVision _vision;
        private readonly IUiInvoker _ui;
        private readonly HeroAggregate _aggregate;
        private readonly SessionState _session;
        private readonly SkillEngine _skill;
        private readonly ItemEngine _item;

        // 主循环可变状态（迁自 Main static field）
        private bool _总循环条件;
        private byte[] _initialData = new byte[1920 * 1080 * 4];
        private readonly Dictionary<string, ImageHandle> _缓存图像句柄 = new();
        private readonly Lock _全局模式e_lock = new();

        private delegate bool 截图();
        private 截图? _循环内获取图片;

        /// <summary>
        /// 按键 → 条件激活映射。闭包捕获 _aggregate，每个 ctor 一份独立 Dict。
        /// </summary>
        public IReadOnlyDictionary<Keys, Action> 按键匹配条件更新 { get; }

        /// <summary>
        /// Heroes/ 92 策略调 `_main._聚合.X.Y` 通过本属性访问聚合（同名匹配 Main._聚合，方便 sed 批替换）。
        /// </summary>
        public HeroAggregate _聚合 => _aggregate;

        /// <summary>会话级共享状态——ItemEngine.cs:61/62 等切 `_main.Session.IsPaused` 用。</summary>
        public SessionState Session => _session;

        /// <summary>Phase 9 E：Silt 5 处 Common.UiInvoker → Common.HeroLoopHost!.Ui（service locator 桥，
        /// Phase 11 Silt 子 BC instance 化时 Silt ctor 接 IUiInvoker，本属性可保留作 HeroLoopHost 内部 ui 出口）。</summary>
        public IUiInvoker Ui => _ui;

        public HeroLoopHost(
            IInputExecutor input,
            IScreenVision vision,
            IUiInvoker ui,
            HeroAggregate aggregate,
            SessionState session,
            SkillEngine skill,
            ItemEngine item)
        {
            _input = input;
            _vision = vision;
            _ui = ui;
            _aggregate = aggregate;
            _session = session;
            _skill = skill;
            _item = item;

            按键匹配条件更新 = new Dictionary<Keys, Action>
            {
                { Keys.Z, () => _aggregate.Conditions[ConditionSlotKey.Z].Active = true },
                { Keys.X, () => _aggregate.Conditions[ConditionSlotKey.X].Active = true },
                { Keys.C, () => _aggregate.Conditions[ConditionSlotKey.C].Active = true },
                { Keys.V, () => _aggregate.Conditions[ConditionSlotKey.V].Active = true },
                { Keys.B, () => _aggregate.Conditions[ConditionSlotKey.B].Active = true },
                { Keys.Space, () => _aggregate.Conditions[ConditionSlotKey.Space].Active = true }
            };
        }

        #region 获取指定位置颜色

        public Task<Color> 获取指定位置颜色(int x, int y)
        {
            _ = 获取图片_2();
            return Task.FromResult(GlobalScreenCapture.GetColor(x, y));
        }

        public Color 获取指定位置颜色(int x, int y, in ImageHandle 句柄)
        {
            return ImageManager.GetColor(in 句柄, x - GameLayout.OffsetX, y - GameLayout.OffsetY);
        }

        #endregion

        #region 续走A 走A去等待后摇

        /// <summary>
        /// 根据攻速去后摇。ItemEngine.cs:123 `Run(走A去等待后摇)` 引用。
        /// </summary>
        public void 走A去等待后摇()
        {
            SpinWait spinWait = new();
            bool 等待平A = false;
            while (true)
            {
                if (!_aggregate.Attack.开启走A)
                {
                    return;
                }

                if (_aggregate.Attack.停止走A > 0)
                {
                    _aggregate.Attack.停止走A = 0;
                    break;
                }

                long currentTime = Common.获取当前时间毫秒();

                long remainingTime;
                if (!等待平A)
                {
                    // A+等待前摇 = 实际出手时间， 实际出手时间+等待间隔 = 下一次攻击开始时间 转身速率0.7 转180°157ms延时
                    remainingTime = _aggregate.Attack.实际出手时间 - currentTime + 180;

                    if (remainingTime > 10)
                    {
                        Thread.Sleep((int)(remainingTime / 2));
                    }
                    else if (remainingTime > 2)
                    {
                        Thread.Sleep(1);
                    }
                    else
                    {
                        if (remainingTime <= 0)
                        {
                            SimKeyBoard.KeyPress(Keys.M);
                            等待平A = true;
                            continue;
                        }

                        spinWait.SpinOnce();
                    }
                }
                else
                {
                    // 攻击前摇算在攻击间隔里面
                    remainingTime = _aggregate.Attack.实际出手时间 + _aggregate.Attack.实际攻击间隔 - _aggregate.Attack.实际攻击前摇 - currentTime;

                    if (remainingTime > 10)
                    {
                        Thread.Sleep((int)(remainingTime / 2));
                    }
                    else if (remainingTime > 2)
                    {
                        Thread.Sleep(1);
                    }
                    else
                    {
                        if (remainingTime <= 0)
                        {
                            SimKeyBoard.KeyPress(Keys.A);
                            break;
                        }

                        spinWait.SpinOnce();
                    }
                }
            }
        }

        #endregion

        #region 通用循环

        private async Task 一般程序循环()
        {
            const int 主循环间隔 = 1;
            const int 中断检查间隔 = 1;

            while (_总循环条件)
            {
                try
                {
                    if (_session.IsPaused)
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
                    if (_aggregate.Conditions.StoneProbe is not null)
                    {
                        await _aggregate.Conditions.StoneProbe(GlobalScreenCapture.GetCurrentHandle()).ConfigureAwait(true);
                    }

                    // 获取技能颜色信息
                    _skill.DOTA2获取所有释放技能前颜色(GlobalScreenCapture.GetCurrentHandle());

                    // 核心逻辑：处理条件更新，包括运行期外部修改
                    await _aggregate.Conditions.TickAsync().ConfigureAwait(true);

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

        private async Task 处理假腿切换()
        {
            if (_aggregate.LegSwap.条件保持假腿 && _aggregate.LegSwap.条件开启切假腿 && _aggregate.LegSwap.需要切假腿)
            {
                await 切假腿处理(_aggregate.LegSwap.条件假腿敏捷 ? "敏捷" : "力量").ConfigureAwait(true);
            }
        }

        private async Task 切假腿处理(string 假腿类型)
        {
            if (_aggregate.LegSwap.切假腿中)
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

                if (ImageFinder.FindImageInRegionBool(句柄, GlobalScreenCapture.GetCurrentHandle(), ItemEngine.获取物品范围(_aggregate.SkillCount)))
                {
                    return;
                }

                _aggregate.LegSwap.切假腿中 = true;
                _ = await _item.切假腿类型(假腿类型).ConfigureAwait(true);
                await Task.Run(() =>
                {
                    Common.Delay(100);
                    _aggregate.LegSwap.切假腿中 = false;
                    _aggregate.LegSwap.需要切假腿 = false;
                }).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// GameSession 策略路径首次激活英雄后调此启动主循环。
        /// _总循环条件 守卫由调用方负责（与旧 case 块一致——Dota2 路径下 OnActivate 不置位，主循环不进入；
        /// 这是 Phase 8 重构遗留现状，Phase 9 不在 in-scope 修复）。
        /// </summary>
        public async Task 状态初始化()
        {
#if Silt
            _aggregate.Conditions[ConditionSlotKey.C8].Probe ??= Dota2Simulator.Games.Dota2.Silt.Main.有书吃书;
            _aggregate.Conditions[ConditionSlotKey.C8].Active = true;
#endif
            _循环内获取图片 ??= 获取图片_2;
            await 一般程序循环().ConfigureAwait(true);
        }

        public void 取消所有功能()
        {
            _总循环条件 = false;
            _循环内获取图片 = null;
            _session.IsPaused = false;

            _aggregate.Conditions.Reset();

            _ = _item.重置耗蓝物品委托和条件();

            _aggregate.LegSwap.条件开启切假腿 = false;
            _aggregate.LegSwap.条件保持假腿 = false;
            _aggregate.LegSwap.条件假腿敏捷 = false;
            _aggregate.LegSwap.切假腿中 = false;
            _aggregate.LegSwap.需要切假腿 = false;

            _aggregate.HasAghanim = false;
            _aggregate.HasShard = false;

            _aggregate.Skills.SetStep(SlotKey.Global, 0);
            _aggregate.Skills.SetStep(SlotKey.Q, 0);
            _aggregate.Skills.SetStep(SlotKey.W, 0);
            _aggregate.Skills.SetStep(SlotKey.E, 0);
            _aggregate.Skills.SetStep(SlotKey.R, 0);
            _aggregate.Skills.SetStep(SlotKey.D, 0);
            _aggregate.Skills.SetStep(SlotKey.F, 0);

            _aggregate.Skills.SetMode(SlotKey.Global, 0);
            _aggregate.Skills.SetMode(SlotKey.Q, 0);
            _aggregate.Skills.SetMode(SlotKey.W, 0);
            _aggregate.Skills.SetMode(SlotKey.E, 0);
            _aggregate.Skills.SetMode(SlotKey.R, 0);
            _aggregate.Skills.SetMode(SlotKey.D, 0);
            _aggregate.Skills.SetMode(SlotKey.F, 0);

            _aggregate.Skills.SetTime(SlotKey.Global, -1);
            _aggregate.Skills.SetTime(SlotKey.Q, -1);
            _aggregate.Skills.SetTime(SlotKey.W, -1);
            _aggregate.Skills.SetTime(SlotKey.E, -1);
            _aggregate.Skills.SetTime(SlotKey.R, -1);
            _aggregate.Skills.SetTime(SlotKey.D, -1);
            _aggregate.Skills.SetTime(SlotKey.F, -1);

            _aggregate.Attack.Reset();

            _aggregate.Skills.SetTarget(SlotKey.Global, new Point(0, 0));
            _aggregate.Skills.SetTarget(SlotKey.Q, new Point(0, 0));
            _aggregate.Skills.SetTarget(SlotKey.W, new Point(0, 0));
            _aggregate.Skills.SetTarget(SlotKey.E, new Point(0, 0));
            _aggregate.Skills.SetTarget(SlotKey.R, new Point(0, 0));
            _aggregate.Skills.SetTarget(SlotKey.D, new Point(0, 0));
            _aggregate.Skills.SetTarget(SlotKey.F, new Point(0, 0));

            _skill.重置所有技能判断();

            _aggregate.LegSwap.配置 = new 技能切假腿配置();
            _aggregate.LegSwap.假腿按键 = Keys.Escape;
        }

        #endregion

        #region 获取图片

        private static void 执行屏幕捕捉捕捉(in Rectangle rectangle)
        {
            GlobalScreenCapture.CaptureScreen(rectangle);
        }

        public bool 获取图片_1()
        {
            执行屏幕捕捉捕捉(GameLayout.截图模式1Reg);
            return true;
        }

        public bool 获取图片_2()
        {
            执行屏幕捕捉捕捉(GameLayout.截图模式2Reg);
            return true;
        }

        public void CleanupImageSystem()
        {
            foreach (var handle in _缓存图像句柄.Values)
            {
                ImageManager.ReleaseImage(handle);
            }
            _缓存图像句柄.Clear();

            if (GlobalScreenCapture.GetCurrentHandle().IsValid)
            {
                ImageManager.ReleaseImage(GlobalScreenCapture.GetCurrentHandle());
            }

            ImageSystemMonitor.PerformFullCleanup();
        }

        #endregion
    }
}

#endif
