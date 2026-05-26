// Phase 14 C1: 巫医 Strategy 迁 HeroPlan + CustomProbe DSL — Q 简单 AfterCast(continueKey:E),
// E/R 自定义 Probe (主动技能释放后续 lambda 引用 _input/_item instance 字段) ⇒ _plan instance lazy-init.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("巫医", HeroAttribute.Intelligence)]
public sealed partial class 巫医Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(continueKey: Keys.E)
        .OnKey(Keys.E).CastSkill(Keys.E).AfterCastDo(() =>
        {
            _input.Press(VirtualKey.From(Keys.A));
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.魂之灵龛_Tpl);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.影之灵龛_Tpl);
        })
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCastDo(() =>
        {
            _ = _item.根据图片自我使用物品(Dota2_Pictrue.物品.微光披风_Tpl);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.隐刀_Tpl);
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.大隐刀_Tpl);
        })
        .Done();
}
#endif
