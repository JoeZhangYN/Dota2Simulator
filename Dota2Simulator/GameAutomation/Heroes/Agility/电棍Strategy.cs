// Phase 12 Chunk 3b: 电棍 Strategy 迁 HeroPlan DSL — 3 helper 全简单一行式, 100% fit Plan.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>电棍（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "电棍"。Phase 12 C3b: 链式 DSL.</summary>
[HeroStrategy("电棍", HeroAttribute.Agility)]
public sealed partial class 电棍Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD(continueAttack: true)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast(continueAttack: true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD(continueAttack: true)
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();
}
#endif
