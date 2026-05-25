// Phase 12 Chunk 3 pilot: 虚空 Strategy 迁 HeroPlan DSL — OnActivate/OnKeyAsync 体内化为 _plan.Apply / _plan.DispatchAsync 两行委托; helper method (时间漫游/膨胀/结界) 全删 (closure 由 Plan 内 SkillEngine.技能通用判断 替).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>虚空（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "虚空"。Phase 12 C3: 体内化 HeroPlan DSL.</summary>
[HeroStrategy("虚空", HeroAttribute.Agility)]
public sealed partial class 虚空Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(continueAttack: true)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD(continueAttack: true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast(continueAttack: true)
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
