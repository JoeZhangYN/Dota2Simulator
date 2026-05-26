// Phase 12 Chunk 3c: TB Strategy 迁 HeroPlan + WhenHasAghanim + WhenHasShard + AdjustLegSwap setup.
// 关键迁移点:
//   - F1 + HasAghanim → LegSwap.修改配置(F, true): .OnKey(F1).WhenHasAghanim().AdjustLegSwap(F, true).
//   - D + HasShard → Conditions[C4] (恶魔狂热): .OnKey(D).WhenHasShard().CastSkill(D).AfterCast.
//   - F + HasAghanim → Conditions[C5] (恐怖心潮): .OnKey(F).WhenHasAghanim().CastSkill(F).AfterEnterCD.
//   - Q/W/E/R: 无 guard 简单 clause.
// 6 helper (倒影/幻惑/魔化/恶魔狂热/恐怖心潮/断魂) 全简单一行式 → Plan 内化.
//
// Plan OnKey 顺序映射 C1..C6: Q→C1, W→C2, E→C3, D→C4, F→C5, R→C6 (与原 OnActivate 注册顺序 1:1).
#if DOTA2
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>TB（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "TB"。Phase 12 C3c: HeroPlan + Guard + Setup.</summary>
[HeroStrategy("TB", HeroAttribute.Agility)]
public sealed partial class TBStrategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.F1).WhenHasAghanim().AdjustLegSwap(Keys.F, true)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(continueAttack: true)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast(continueAttack: true)
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD(continueAttack: true)
        .OnKey(Keys.D).WhenHasShard().CastSkill(Keys.D).AfterCast(continueAttack: true)
        .OnKey(Keys.F).WhenHasAghanim().CastSkill(Keys.F).AfterEnterCD(continueAttack: true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast(continueAttack: true)
        .LegSwap(Keys.W, alwaysSwap: false)
        .Done();
}
#endif
