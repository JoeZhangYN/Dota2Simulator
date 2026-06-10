// Phase 19D: 猛犸 Strategy 迁 HeroPlan — Q/W/E/R/D 5 CustomProbe (全部 wrap _skill.通用技能后续动作) + F/D2 Execute lambda (跳拱/指定地点 多步骤宏). Probe/Execute lambda 引用 _skill/_input/_main ⇒ instance _plan lazy-init.
#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("猛犸", HeroAttribute.Universal)]
public sealed partial class 猛犸Strategy : IHeroStrategy
{

    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(通用后续Probe)
        .OnKey(Keys.W).CustomProbe(通用后续Probe)
        .OnKey(Keys.E).CustomProbe(通用后续Probe)
        .OnKey(Keys.R).CustomProbe(通用后续Probe)
        .OnKey(Keys.D).CustomProbe(通用后续Probe)
        .OnKey(Keys.F).Execute(() => Task.Run(跳拱指定地点))
        .OnKey(Keys.D2).Execute(() => Task.Run(指定地点))
        .Done();

    /// <summary>原 5 个 helper (震荡波/授予力量/巨角冲撞/两级反转/长角抛物去后摇) 全调 _skill.通用技能后续动作() 后返 false. 合并单 Probe 复用.</summary>
    private async Task<bool> 通用后续Probe()
    {
        _skill.通用技能后续动作();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    // todo 逻辑优化 有鱼叉
    private void 跳拱指定地点()
    {
        Press(Keys.Space);
        Common.Delay(等待延迟);
        Press(Keys.D9);
        Point target = _main._聚合.Skills.Target(SlotKey.Global);
        _input.MouseMoveTo(new ScreenPoint(target.X, target.Y));
        Common.Delay(等待延迟);
        Press(Keys.E);
        Common.Delay(等待延迟);
        Press(Keys.D9);
    }

    private void 指定地点()
    {
        _main._聚合.Skills.SetTarget(SlotKey.Global, Control.MousePosition);

        Common.Delay(等待延迟);
        _input.KeyDown(VirtualKey.From(Keys.Control));
        Common.Delay(等待延迟);
        Press(Keys.D9);
        Common.Delay(等待延迟);
        _input.KeyUp(VirtualKey.From(Keys.Control));
    }
}
#endif
