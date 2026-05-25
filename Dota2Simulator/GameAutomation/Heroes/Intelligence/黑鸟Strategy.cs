// Phase 15 C1: 黑鸟 Strategy 迁 HeroPlan + Pre/CustomProbe/Execute/NoProbe — D Pre(Press W) + CustomProbe 神智之蚀, R CustomProbe 关接跳, E NoProbe, W Execute (直接 _item.使用 纷争).
// OnKey 顺序按 Probe 注册重排 (D→C1, R→C2, E→C3, W→setup; 原 OnActivate 仅 C1/C2 Probe, W 直 _item 副作用, E 无 Probe 死代码占位).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("黑鸟", HeroAttribute.Intelligence)]
public sealed partial class 黑鸟Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.D).Pre(() => _input.Press(VirtualKey.From(Keys.W))).CustomProbe(async () =>
        {
            if (_skill.DOTA2判断技能是否CD(Keys.R))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            _input.Press(VirtualKey.From(Keys.A));
            return await Task.FromResult(false).ConfigureAwait(true);
        })
        .OnKey(Keys.R).CustomProbe(async () =>
            _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀) == 1
                ? await Task.FromResult(false).ConfigureAwait(true)
                : await Task.FromResult(true).ConfigureAwait(true))
        .OnKey(Keys.E).NoProbe()
        .OnKey(Keys.W).Execute(() => _item.根据图片使用物品(Dota2_Pictrue.物品.纷争))
        .Done();
}
#endif
