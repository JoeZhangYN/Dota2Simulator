// Phase 12 Chunk 3e: 沙王 Strategy 迁 HeroPlan + RepeatThreshold builder API.
// 原 OnActivate 仅 1 行 _skill.重复按键执行间隔阈值 = 150 (无 Probe / 无 LegSwap), OnKeyAsync 全死代码注释 — Plan 0 clause + RepeatThreshold(150) 即等价.
#if DOTA2
using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>沙王（全才）策略——迁移自 _main.根据当前英雄增强 的 case "沙王"。Phase 12 C3e: RepeatThreshold 内化.</summary>
[HeroStrategy("沙王", HeroAttribute.Universal)]
public sealed partial class 沙王Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .RepeatThreshold(150)
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
