// Phase 19D: 神域 Strategy 迁 HeroPlan — W/E/R/D 4 CustomProbe (3 异步 CD 检查 + Press A/MouseClick.Right 模板 + D 同步). W→C1 / E→C2 / R→C3 / D→C5 (跳 C4! 用 NoProbe 占 C4 维持顺序映射).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("神域", HeroAttribute.Intelligence)]
public sealed partial class 神域Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.W).CustomProbe(命运敕令去后摇)
        .OnKey(Keys.E).CustomProbe(涤罪之焰去后摇)
        .OnKey(Keys.R).CustomProbe(虚妄之诺去后摇)
        .OnKey(Keys.None).NoProbe()  // 占 C4 (原 OnKeyAsync 无 C4 触发, 业务侧 D→C5 跳 C4)
        .OnKey(Keys.D).CustomProbe(天命之雨去后摇)
        .Done();

    private async Task<bool> 命运敕令去后摇()
    {
        async Task 命运敕令后() => await Task.Run(() => _input.MouseClick(MouseButton.Right)).ConfigureAwait(true);
        if (_skill.DOTA2判断技能是否CD(Keys.W))
            return await Task.FromResult(true).ConfigureAwait(true);
        命运敕令后().Start();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 涤罪之焰去后摇()
    {
        async Task 涤罪之焰后() => await Task.Run(() => _input.Press(VirtualKey.From(Keys.A))).ConfigureAwait(true);
        if (_skill.DOTA2判断技能是否CD(Keys.E))
            return await Task.FromResult(true).ConfigureAwait(true);
        涤罪之焰后().Start();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 虚妄之诺去后摇()
    {
        async Task 虚妄之诺后() => await Task.Run(() => _input.Press(VirtualKey.From(Keys.A))).ConfigureAwait(true);
        if (_skill.DOTA2判断技能是否CD(Keys.R))
            return await Task.FromResult(true).ConfigureAwait(true);
        虚妄之诺后().Start();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 天命之雨去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.D))
            return await Task.FromResult(true).ConfigureAwait(true);
        _input.MouseClick(MouseButton.Right);
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
