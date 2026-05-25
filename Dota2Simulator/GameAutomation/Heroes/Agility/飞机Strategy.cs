// Phase 17: 飞机 Strategy 迁 HeroPlan — D3 ToggleConditionSlot(C5, 循环弹幕/关闭弹幕). 其他 Q/W/E/R 注释死代码, OnActivate 保留 C5 Probe (循环火箭弹幕) ??= 注册.
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
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("飞机", HeroAttribute.Agility)]
public sealed partial class 飞机Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx)
    {
        GetPlan().Apply(ctx, _skill);
        // C5 Probe 单独注册 (DSL plan 只承载 D3 ToggleConditionSlot setup, 不占 clause 槽)
        _main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 循环火箭弹幕;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.D3).ToggleConditionSlot(ConditionSlotKey.C5, "循环弹幕", "关闭弹幕")
        .Done();

    private async Task<bool> 循环火箭弹幕(ImageHandle 句柄)
    {
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Q) > 400)
            await _skill.主动技能已就绪后续(Keys.Q, () =>
            {
                _input.Press(VirtualKey.From(Keys.Q));
                _main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());
            }).ConfigureAwait(true);

        return await Task.FromResult(_main._聚合.Conditions[ConditionSlotKey.C5].Active);
    }
}
#endif
