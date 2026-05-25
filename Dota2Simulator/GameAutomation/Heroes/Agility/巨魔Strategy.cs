// Phase 12 Chunk 3b: 巨魔 Strategy 迁 HeroPlan DSL — 3 helper 全简单一行式, 100% fit Plan.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>巨魔（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "巨魔"。Phase 12 C3b: 链式 DSL.</summary>
[HeroStrategy("巨魔", HeroAttribute.Agility)]
public sealed partial class 巨魔Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast(continueAttack: true)
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD(continueAttack: true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD(continueAttack: true)
        .LegSwap(Keys.Q, alwaysSwap: false)
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
