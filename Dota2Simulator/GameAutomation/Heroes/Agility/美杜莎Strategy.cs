// Phase 12 Chunk 3b: 美杜莎 Strategy 迁 HeroPlan DSL — 3 helper 全 AfterCast(true), 100% fit Plan.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>美杜莎（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "美杜莎"。Phase 12 C3b: 链式 DSL.</summary>
[HeroStrategy("美杜莎", HeroAttribute.Agility)]
public sealed partial class 美杜莎Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast(continueAttack: true)
        .OnKey(Keys.E).CastSkill(Keys.E).AfterCast(continueAttack: true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast(continueAttack: true)
        .LegSwap(Keys.Q, alwaysSwap: false)
        .Done();

    public override void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
