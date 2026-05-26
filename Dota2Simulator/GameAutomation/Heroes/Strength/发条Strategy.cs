// Phase 19B: 发条 业务死代码清理 — Probe 全注释 + W 键 PreAction (_input.Press(A) 后置 Active) ⇒ 迁 HeroPlan NoProbe + Pre 形态.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("发条", HeroAttribute.Strength)]
public sealed partial class 发条Strategy : IHeroStrategy
{
    private HeroPlan? _plan;
    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).NoProbe()
        .OnKey(Keys.W).Pre(() => _input.Press(VirtualKey.From(Keys.A))).NoProbe()  // 回收时按 W 先 Press A
        .OnKey(Keys.R).NoProbe()
        .Done();

    public override void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);
}
#endif
