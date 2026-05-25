// Phase 17: 大圣 Strategy 迁 HeroPlan — Q/E/R 标准 AfterCast/AfterEnterCD, D3 ToggleConditionSlot(C4, 开启/关闭无限跳跃), C4 大圣无限跳跃 CustomProbe (释放 W 技能 mode 2 + 返 Active 自循环), RepeatThreshold(100), LegSwap Q/W false.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("大圣", HeroAttribute.Agility)]
public sealed partial class 大圣Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .RepeatThreshold(100)
        .LegSwap(Keys.Q, alwaysSwap: false)
        .LegSwap(Keys.W, alwaysSwap: false)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.None).CustomProbe(async () =>
        {
            await _skill.技能通用判断(Keys.W, 2).ConfigureAwait(true);
            return _main._聚合.Conditions[ConditionSlotKey.C4].Active;
        })
        .OnKey(Keys.D3).ToggleConditionSlot(ConditionSlotKey.C4, "开启无限跳跃", "关闭无限跳跃")
        .Done();
}
#endif
