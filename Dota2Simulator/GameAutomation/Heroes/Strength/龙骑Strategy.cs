// Phase 16 C1a: 龙骑 Strategy 迁 HeroPlan — F1+HasShard AdjustLegSwap(D,true), W CustomProbe (主动技能进入CD后续 + Mode/HasShard 条件释放 D/Q), D+HasShard CustomProbe (动态 continueKey: Mode==1 && Q-CD ? Q : A), D2/D3 Execute ToggleMode+TTS.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("龙骑", HeroAttribute.Strength)]
public sealed partial class 龙骑Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .AttackTiming(preDelay: 0.5, interval: 1.6)
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.D, paramBool: true)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCDDo(() =>
        {
            _input.Press(VirtualKey.From(Keys.A));
            _ = _main._聚合.Skills.Mode(SlotKey.W) == 1 && _main._聚合.HasShard ? _skill.DOTA2释放CD就绪技能(Keys.D) : _skill.DOTA2释放CD就绪技能(Keys.Q);
            _item.要求保持假腿();
        })
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD()
        .OnKey(Keys.D).WhenHasShard().CustomProbe(async () => await _skill.技能通用判断(
            Keys.D,
            0,
            要接的按键: _main._聚合.Skills.Mode(SlotKey.D) == 1 && _skill.DOTA2判断技能是否CD(Keys.Q) ? Keys.Q : Keys.A).ConfigureAwait(true))
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.W);
            Dota2Simulator.TTS.TTS.Speak("W接" + (_main._聚合.Skills.Mode(SlotKey.W) == 1 ? "火球" : "喷火"));
        })
        .OnKey(Keys.D3).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.D);
            Dota2Simulator.TTS.TTS.Speak("火球" + (_main._聚合.Skills.Mode(SlotKey.D) == 1 ? "接" : "不接") + "喷火");
        })
        .Done();
}
#endif
