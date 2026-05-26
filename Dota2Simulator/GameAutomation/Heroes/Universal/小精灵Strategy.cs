// Phase 19C: 小精灵 Strategy 重构 — 用 WhenNotHasAghanim 反向 Guard DSL 把 Phase 17 Execute lambda 内 if (!HasAghanim) 上移到 setup level; RegisterProbe DSL 替代 OnActivate 手动 ??= 注册.
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
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("小精灵", HeroAttribute.Universal)]
public sealed partial class 小精灵Strategy : IHeroStrategy
{
    private static readonly Rectangle buff状态技能栏 = new(962, 826, 526, 80);

    private HeroPlan? _plan;

    public override void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.W).WhenNotHasAghanim().Execute(() => _main._聚合.Conditions[ConditionSlotKey.C2].Active = true)
        .OnKey(Keys.D3).ToggleConditionSlot(ConditionSlotKey.C3, "开启续过载", "关闭续过载")
        .RegisterProbe(ConditionSlotKey.C2, 幽魂检测)
        .RegisterProbe(ConditionSlotKey.C3, 循环续过载)
        .RepeatThreshold(150)
        .Done();

    private async Task<bool> 幽魂检测()
    {
        return _vision.Find(Dota2_Pictrue.Buff.小精灵_幽魂_Tpl, buff状态技能栏, new MatchRate(0.9), Tolerance.Exact).Found
            ? await Task.FromResult(true).ConfigureAwait(true)
            : await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 循环续过载()
    {
        bool guozai = _vision.Find(Dota2_Pictrue.Buff.小精灵_过载_Tpl, buff状态技能栏, new MatchRate(0.9), Tolerance.Exact).Found;
        if (guozai)
        {
            _main._聚合.Skills.SetStep(SlotKey.E, 3);
            return await Task.FromResult(_main._聚合.Conditions[ConditionSlotKey.C3].Active).ConfigureAwait(true);
        }
        await _skill.技能通用判断(Keys.E, 2).ConfigureAwait(true);
        return await Task.FromResult(_main._聚合.Conditions[ConditionSlotKey.C3].Active).ConfigureAwait(true);
    }
}
#endif
