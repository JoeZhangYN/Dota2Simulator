// Phase 19G-4: 猴子 Strategy 迁 HeroPlan — Q AfterCast + W CustomProbe (主动技能释放后续 lambda) + LegSwap 3 配置. override OnKeyAsync 处理 Q/W/R 前置 Press(E) conditional PreAction (条件 PreAction with Guard).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("猴子", HeroAttribute.Agility)]
public sealed partial class 猴子Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.E, alwaysSwap: false)
        .LegSwap(Keys.R, alwaysSwap: false)
        .LegSwap(Keys.W, alwaysSwap: true, "力量")  // Phase 20C: DSL 三参 LegSwap, 替原 override OnActivate 内 修改配置 第三参
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()  // C1: 灵魂之矛敏捷
        .OnKey(Keys.W).CustomProbe(神行百变选择幻象)  // C2: 神行百变
        .Done();

    public override async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        // Q/W/R 前置: 若 E 状态技能未启动则 Press(E) (条件 PreAction with Guard, DSL .Pre 不支持条件)
        if (key == VirtualKey.Q || key == VirtualKey.W || key == VirtualKey.R)
        {
            if (!_skill.DOTA2判断状态技能是否启动(Keys.E))
                Press(Keys.E);
        }

        if (key == VirtualKey.Q)
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        else if (key == VirtualKey.W)
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        // R 键无 Active (业务原也无, 仅 PreAction)
    }

    private async Task<bool> 神行百变选择幻象()
    {
        return await _skill.主动技能释放后续(Keys.W, () =>
        {
            Common.Delay(1000);
            Press(Keys.D1);
            Common.Delay(33);
            _input.MouseClick(MouseButton.Right);
            Press(Keys.F1);
            _item.要求保持假腿();
        }).ConfigureAwait(true);
    }
}
#endif
