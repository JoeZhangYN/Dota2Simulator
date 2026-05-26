// Phase 19C: 飞机 Strategy 重构 — RegisterProbe DSL 替代 Phase 17 OnActivate 手动 ??= 注册 C5 Probe.
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
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.D3).ToggleConditionSlot(ConditionSlotKey.C5, "循环弹幕", "关闭弹幕")
        .RegisterProbe(ConditionSlotKey.C5, 循环火箭弹幕)
        .Done();

    private async Task<bool> 循环火箭弹幕()
    {
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Q) > 400)
            await _skill.主动技能已就绪后续(Keys.Q, () =>
            {
                Press(Keys.Q);
                _main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());
            }).ConfigureAwait(true);

        return await Task.FromResult(_main._聚合.Conditions[ConditionSlotKey.C5].Active);
    }
}
#endif
