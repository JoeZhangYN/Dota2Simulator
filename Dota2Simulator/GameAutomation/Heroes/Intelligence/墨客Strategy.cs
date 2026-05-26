// Phase 16 C2: 墨客 Strategy 迁 HeroPlan — 5 标准 clause (Q/W/E/R/D AfterCast / AfterEnterCD), E+Alt → Execute lambda 共享 C3 槽 Active (KeyModifier 形态, 复用 bare E 的 Probe).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("墨客", HeroAttribute.Intelligence)]
public sealed partial class 墨客Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public override void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD()
        .OnKey(Keys.E).CastSkill(Keys.E).AfterCast()
        .OnKey(Keys.E, KeyModifiers.Alt).Execute(() => _main._聚合.Conditions[ConditionSlotKey.C3].Active = true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.D).CastSkill(Keys.D).AfterEnterCD()
        .Done();
}
#endif
