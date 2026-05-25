// Phase 15 C1: 大牛 Strategy 迁 HeroPlan + Pre DSL — Q AfterCast(postDelayMs:1300), W Pre(_input.Press(A)) + CustomProbe (释放技能后替换图标技能后续), R AfterCast.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("大牛", HeroAttribute.Strength)]
public sealed partial class 大牛Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(postDelayMs: 1300)
        .OnKey(Keys.W).Pre(() => _input.Press(VirtualKey.From(Keys.A))).CustomProbe(async () =>
            await _skill.释放技能后替换图标技能后续(
                Keys.W,
                () => _main._聚合.Skills.Step(SlotKey.W),
                v => _main._聚合.Skills.SetStep(SlotKey.W, v)).ConfigureAwait(true))
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();
}
#endif
