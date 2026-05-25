// Phase 16 C1a: 小松鼠 Strategy 迁 HeroPlan — F1+HasShard AdjustLegSwap(F,true), Q/W CustomProbe (主动技能释放后续 Mode 条件分支释放 Q/W/A), R NoProbe (原 一箭穿心 dummy false Probe), F+HasShard AfterCast, D2/D3 Execute ToggleMode+TTS.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("小松鼠", HeroAttribute.Agility)]
public sealed partial class 小松鼠Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.F, paramBool: true)
        .OnKey(Keys.Q).CustomProbe(async () => await _skill.主动技能释放后续(Keys.Q, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.W) == 1)
            {
                _input.Press(VirtualKey.From(Keys.W));
            }
            else
            {
                _skill.通用技能后续动作();
            }
        }).ConfigureAwait(true))
        .OnKey(Keys.W).CustomProbe(async () => await _skill.主动技能释放后续(Keys.W, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.E) == 1)
            {
                _input.Press(VirtualKey.From(Keys.Q));
            }
            else
            {
                _skill.通用技能后续动作();
            }
        }).ConfigureAwait(true))
        .OnKey(Keys.R).NoProbe()
        .OnKey(Keys.F).WhenHasShard().CastSkill(Keys.F).AfterCast()
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.W);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.W) == 0 ? "种树接平A" : "种树接捆");
        })
        .OnKey(Keys.D3).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.E);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.E) == 0 ? "捆接平A" : "捆接种树");
        })
        .Done();
}
#endif
