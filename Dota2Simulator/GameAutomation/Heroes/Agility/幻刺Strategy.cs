// Phase 15 C1: 幻刺 Strategy 迁 HeroPlan + Pre + Execute DSL — F1+HasAgh AdjustLegSwap, Q/W/E/D 4 clause, W Pre(_input.Press(A)), D2 Execute (Skills.SetMode + TTS).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("幻刺", HeroAttribute.Agility)]
public sealed partial class 幻刺Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.F1).WhenHasAghanim().AdjustLegSwap(Keys.D, true)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.W).Pre(() => _input.Press(VirtualKey.From(Keys.A))).CastSkill(Keys.W).AfterEnterCD()
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD(continueAttack: false)
        .OnKey(Keys.D).WhenHasAghanim().CastSkill(Keys.D).AfterEnterCD()
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.SetMode(SlotKey.W, 1);
            Dota2Simulator.TTS.TTS.Speak("闪烁分身晕锤一次");
        })
        .Done();
}
#endif
