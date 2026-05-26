// Phase 19G-3: 船长 Strategy 迁 HeroPlan — Q/E 2 CustomProbe + D2 Execute (Step R=1 + Press E 跳船宏). C3/C4 由 Probe 内跨 clause 副作用驱动 (Phase 16 #6 持续).
#if DOTA2
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("船长", HeroAttribute.Strength)]
public sealed partial class 船长Strategy : IHeroStrategy
{
    private static readonly Lock _全局模式e_lock = new();

    private HeroPlan? _plan;
    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(洪流接x回)
        .OnKey(Keys.E).CustomProbe(x释放后相关逻辑)
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.SetStep(SlotKey.R, 1);
            _input.Press(VirtualKey.From(Keys.E));
        })
        .RegisterProbe(ConditionSlotKey.C3, x2次释放后)
        .RegisterProbe(ConditionSlotKey.C4, 立即释放洪流)
        .Done();

    protected override HeroPlan BuildPlan() => GetPlan();

    public override async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        // E 键步骤 1 时跳过 (业务特有 short-circuit, 未走 BuildPlan dispatch).
        if (trigger.Key == VirtualKey.E && _main._聚合.Skills.Step(SlotKey.E) == 1)
            return;
        await base.OnKeyAsync(trigger, ctx).ConfigureAwait(true);
    }

    private async Task<bool> 洪流接x回()
    {
        return await _skill.主动技能释放后续(Keys.Q, () =>
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
            _main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());
            if (!_main.Session!.IsPaused && _main._聚合.Skills.Step(SlotKey.E) == 1)
            {
                Common.Delay(1350, _main._聚合.Skills.Time(SlotKey.Q));
                _main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
                _input.Press(VirtualKey.From(Keys.E));
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> x释放后相关逻辑()
    {
        return await _skill.主动技能释放后续(Keys.E, () =>
        {
            int 步骤e = _main._聚合.Skills.Step(SlotKey.E);
            if (步骤e == 1) return;
            _main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒());
            if (_main._聚合.Skills.Step(SlotKey.R) == 1)
            {
                _input.Press(VirtualKey.From(Keys.R));
                _main._聚合.Skills.SetStep(SlotKey.R, 0);
            }
            lock (_全局模式e_lock)
            {
                _main._聚合.Skills.SetStep(SlotKey.E, 1);
                _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
            int 等待时间 = (int)Math.Floor(3000 * _main._聚合.Attack.状态抗性倍数) - 1670;
            Common.Delay(等待时间, _main._聚合.Skills.Time(SlotKey.E));
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }).ConfigureAwait(true);
    }

    private async Task<bool> x2次释放后()
    {
        return await _skill.主动技能进入CD后续(Keys.E, () =>
        {
            lock (_全局模式e_lock)
            {
                Common.Delay(2000);
                _main._聚合.Skills.SetStep(SlotKey.E, 0);
            }
        }).ConfigureAwait(true);
    }

    private async Task<bool> 立即释放洪流()
    {
        return await _skill.主动技能已就绪后续(Keys.Q, () => _input.Press(VirtualKey.From(Keys.Q))).ConfigureAwait(true);
    }
}
#endif
