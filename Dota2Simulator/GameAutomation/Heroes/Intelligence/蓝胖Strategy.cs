// Phase 19D: 蓝胖 Strategy 迁 HeroPlan — Q/W/E/F/D 5 CustomProbe (CD 检查 + 副作用同质模板) + D2 Execute (ToggleMode W + TTS). E 键 OnKeyAsync 不触发但 OnActivate 注册 C3 嗜血术 (dead Probe), 用 NoProbe 占 C3 维持顺序映射.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("蓝胖", HeroAttribute.Intelligence)]
public sealed partial class 蓝胖Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(火焰轰爆去后摇)
        .OnKey(Keys.W).CustomProbe(引燃去后摇)
        .OnKey(Keys.E).NoProbe()  // 占 C3 (原 OnActivate 注册嗜血术 dead Probe, OnKeyAsync 不触发 E)
        .OnKey(Keys.F).CustomProbe(烈火护盾去后摇)
        .OnKey(Keys.D).CustomProbe(未精通火焰轰爆去后摇)
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.W);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.W) == 0 ? "引燃接轰爆" : "引燃不接轰爆");
        })
        .Done();

    private async Task<bool> 火焰轰爆去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.Q))
            return await Task.FromResult(true).ConfigureAwait(true);
        _input.MouseClick(MouseButton.Right);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 引燃去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.W))
            return await Task.FromResult(true).ConfigureAwait(true);
        switch (_main._聚合.Skills.Mode(SlotKey.W))
        {
            case 1: _input.Press(VirtualKey.From(Keys.Q)); break;
            default: _input.MouseClick(MouseButton.Right); break;
        }
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 烈火护盾去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.F))
            return await Task.FromResult(true).ConfigureAwait(true);
        _input.MouseClick(MouseButton.Right);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 未精通火焰轰爆去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.D))
            return await Task.FromResult(true).ConfigureAwait(true);
        _input.MouseClick(MouseButton.Right);
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
