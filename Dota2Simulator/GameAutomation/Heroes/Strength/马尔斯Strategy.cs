// Phase 15 C2: 马尔斯 Strategy 迁 HeroPlan — Q/R CustomProbe (主动技能释放后续 lambda 内 Mode 检查 + Press), W 简单 AfterCast, D2 Execute (ToggleMode + TTS, 复用 Execute lambda 不专 DSL).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("马尔斯", HeroAttribute.Strength)]
public sealed partial class 马尔斯Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCastDo(() =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.Q) == 1)
            {
                Press(Keys.R);
            }
            else
            {
                _skill.通用技能后续动作();
            }
        })
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCastDo(() =>
        {
            if (_skill.判断技能状态(Keys.E, SkillEngine.技能类型.状态))
            {
                Press(Keys.E);
            }
            _skill.通用技能后续动作();
        })
        .OnKey(Keys.D2).ToggleModeTts(SlotKey.Q, "矛接大招", "矛不接大招")
        .Done();
}
#endif
