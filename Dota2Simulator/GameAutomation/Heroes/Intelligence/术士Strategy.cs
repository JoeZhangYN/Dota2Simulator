// Phase 19D: 术士 Strategy 迁 HeroPlan — Q Pre(_item 纷争) + CustomProbe + W CustomProbe + E Execute (SetTime only) + R Pre(SetTime) + CustomProbe. 3 helper 同质 (CD 检查 + 通用后续 false).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("术士", HeroAttribute.Intelligence)]
public sealed partial class 术士Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).Pre(() => _item.根据图片使用物品(Dota2_Pictrue.物品.纷争_Tpl)).CustomProbe(致命链接去后摇)
        .OnKey(Keys.W).CustomProbe(暗言术去后摇)
        .OnKey(Keys.E).Execute(() => _main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒()))
        .OnKey(Keys.R).Pre(() => _main._聚合.Skills.SetTime(SlotKey.R, Common.获取当前时间毫秒())).CustomProbe(混乱之祭去后摇)
        .Done();

    private async Task<bool> 致命链接去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.Q))
            return await Task.FromResult(true).ConfigureAwait(true);
        _skill.通用技能后续动作(false);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 暗言术去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.W))
            return await Task.FromResult(true).ConfigureAwait(true);
        _skill.通用技能后续动作(false);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 混乱之祭去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.R))
            return await Task.FromResult(true).ConfigureAwait(true);
        _skill.通用技能后续动作(false);
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
