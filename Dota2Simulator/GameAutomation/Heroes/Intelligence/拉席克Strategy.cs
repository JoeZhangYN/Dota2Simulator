// Phase 13 C1: 拉席克 Strategy 迁 HeroPlan — 5 helper 全 mode 0 (AfterEnterCD), 100% fit Plan.
// OnKey 顺序按 Probe 注册 C1..C5 (Q/W/E/R/D) 写入 (原 OnKeyAsync 中 D 在 R 之前是代码顺序, 槽映射 D→C5/R→C4 与 Probe 注册一致).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("拉席克", HeroAttribute.Intelligence)]
public sealed partial class 拉席克Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD()
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD()
        .OnKey(Keys.D).CastSkill(Keys.D).AfterEnterCD()
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
