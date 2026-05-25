// Phase 12 Chunk 3 pilot: 影魔 Strategy 迁 HeroPlan DSL — 4 个 helper (z炮/x炮/c炮/灵魂盛宴) closure 化进 Plan; OnActivate/OnKeyAsync 两行委托.
// 注: 原 R 键映射 Conditions[C5].Active (跳过 C4 因 C4 是 D 键的灵魂盛宴), Plan 顺序 = Q→W→E→D→R 累加 C1→C5 保 case 行为不变.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>影魔（敏捷）策略——迁移自 _main.根据当前英雄增强 的 case "影魔"。Phase 12 C3: 体内化 HeroPlan DSL.</summary>
[HeroStrategy("影魔", HeroAttribute.Agility)]
public sealed partial class 影魔Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(continueAttack: false)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast(continueAttack: false)
        .OnKey(Keys.E).CastSkill(Keys.E).AfterCast(continueAttack: false)
        .OnKey(Keys.D).CastSkill(Keys.D).AfterCast(continueAttack: false)
        // R 键仅占 C5 槽 (旧 OnKeyAsync R→C5.Active=true), Probe 在旧 OnActivate 已注释 (如影随形 helper 不存在) — 用 NoProbe 保 case 行为不变.
        .OnKey(Keys.R).NoProbe()
        .LegSwap(Keys.F, alwaysSwap: false)
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
