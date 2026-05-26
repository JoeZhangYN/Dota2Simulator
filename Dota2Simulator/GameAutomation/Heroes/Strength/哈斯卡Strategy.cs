// Phase 14 C1: 哈斯卡 Strategy 迁 HeroPlan + CustomProbe DSL — Q→W 特殊键位映射 mode 1, R 自定义 Probe (主动技能释放后续 lambda + DOTA2释放CD就绪技能).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("哈斯卡", HeroAttribute.Strength)]
public sealed partial class 哈斯卡Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.W).AfterCast()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCastDo(() =>
        {
            _input.MouseClick(MouseButton.Right);
            if (_skill.DOTA2释放CD就绪技能(Keys.Q))
            {
                return;
            }
            _input.Press(VirtualKey.From(Keys.A));
        })
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();
}
#endif
