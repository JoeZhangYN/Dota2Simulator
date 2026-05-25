// Phase 13 C2: 瘟疫法师 Strategy 迁 HeroPlan + ToggleSlot DSL — Q/W/R 简单 mode 0 (no continueAttack), F+HasShard NoProbe, D3 toggle 循环死亡脉冲, F1+HasShard AdjustLegSwap, LegSwap E + RepeatThreshold(100).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("瘟疫法师", HeroAttribute.Intelligence)]
public sealed partial class 瘟疫法师Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD(continueAttack: false)
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD(continueAttack: false)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD()
        .OnKey(Keys.F).WhenHasShard().NoProbe()
        .OnKey(Keys.D3).ToggleSlot(skillKey: Keys.Q, speakOn: "循环脉冲", speakOff: "终止循环")
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.F, true)
        .LegSwap(Keys.E, alwaysSwap: false)
        .RepeatThreshold(100)
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
