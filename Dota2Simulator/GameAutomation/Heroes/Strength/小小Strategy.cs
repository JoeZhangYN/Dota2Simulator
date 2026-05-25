// Phase 14 C2: 小小 Strategy 迁 HeroPlan + CustomProbe DSL — Q 简单 AfterEnterCD, W 自定义 (Task.Run 循环 _skill.通用技能后续动作 3 次) + 2 LegSwap.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("小小", HeroAttribute.Strength)]
public sealed partial class 小小Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()
        .OnKey(Keys.W).CustomProbe(async () => await Task.Run(() =>
        {
            for (int i = 0; i < 3; i++)
            {
                Common.Delay(20);
                _skill.通用技能后续动作();
            }
            return false;
        }).ConfigureAwait(true))
        .LegSwap(Keys.E, alwaysSwap: false)
        .LegSwap(Keys.R, alwaysSwap: false)
        .Done();
}
#endif
