// Phase 19D: 女王 Strategy 迁 HeroPlan — Q/W/E/R 4 CustomProbe (CD 检查 + SetTime(-1) + Press A / MouseClick.Right 副作用同质) + S Execute (Session.IsPaused).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("女王", HeroAttribute.Intelligence)]
public sealed partial class 女王Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(暗影突袭去后摇)
        .OnKey(Keys.W).CustomProbe(闪烁去后摇)
        .OnKey(Keys.E).CustomProbe(痛苦尖叫去后摇)
        .OnKey(Keys.R).CustomProbe(冲击波去后摇)
        .OnKey(Keys.S).Execute(() => _main.Session!.IsPaused = true)
        .Done();

    private async Task<bool> 暗影突袭去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.Q))
            return await Task.FromResult(true).ConfigureAwait(true);
        _main._聚合.Skills.SetTime(SlotKey.Q, -1);
        _input.Press(VirtualKey.From(Keys.A));
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 闪烁去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.W))
            return await Task.FromResult(true).ConfigureAwait(true);
        _main._聚合.Skills.SetTime(SlotKey.W, -1);
        _input.MouseClick(MouseButton.Right);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 痛苦尖叫去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.E))
            return await Task.FromResult(true).ConfigureAwait(true);
        _main._聚合.Skills.SetTime(SlotKey.E, -1);
        _input.Press(VirtualKey.From(Keys.A));
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 冲击波去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.R))
            return await Task.FromResult(true).ConfigureAwait(true);
        _main._聚合.Skills.SetTime(SlotKey.R, -1);
        _input.Press(VirtualKey.From(Keys.A));
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
