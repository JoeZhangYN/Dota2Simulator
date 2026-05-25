// Phase 17: 小精灵 Strategy 迁 HeroPlan — D3 ToggleConditionSlot(C3, 开启续过载/关闭续过载). W (无 HasAghanim Guard) → Execute lambda hard-code C2 (反向 Guard 不在现有 DSL). OnActivate 保留 C2/C3 Probe ??= 注册.
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

    public void OnActivate(HeroContext ctx)
    {
        GetPlan().Apply(ctx, _skill);
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 幽魂检测;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 循环续过载;
        _skill.重复按键执行间隔阈值 = 150;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.W).Execute(() =>
        {
            // HasAghanim 守 (反向 Guard 现 DSL 不支持, hard-code Execute lambda 替代)
            if (!_main._聚合.HasAghanim)
            {
                _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
            }
        })
        .OnKey(Keys.D3).ToggleConditionSlot(ConditionSlotKey.C3, "开启续过载", "关闭续过载")
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
