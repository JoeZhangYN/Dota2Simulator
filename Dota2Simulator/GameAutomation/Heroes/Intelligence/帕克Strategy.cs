// Phase 16 C2: 帕克 Strategy 迁 HeroPlan — Q CustomProbe (Step+Delay+Press D), W AfterEnterCD, W+Ctrl Execute 共享 C2 槽 (新月之痕), R AfterEnterCD → C3 (重排), D CustomProbe (Press F1×2) → C4 (重排), D2 Execute ToggleMode + TTS.
// 注: 原 OnActivate C3 槽不挂 Probe (空 C3), 迁后 Probe 全按 clause index 紧凑顺序 (C1..C4 全有), C3 重新指 R 梦境缠绕 (原 C4) 不破坏 Probe 行为, 仅 ConditionSlot 索引重排.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("帕克", HeroAttribute.Intelligence)]
public sealed partial class 帕克Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(async () => await _skill.主动技能进入CD后续(Keys.Q, () =>
        {
            _main._聚合.Skills.SetStep(SlotKey.Q, 1);
            Common.Delay(3400);
            if (_main._聚合.Skills.Mode(SlotKey.D) == 1)
            {
                _input.Press(VirtualKey.From(Keys.D));
            }
            _main._聚合.Skills.SetStep(SlotKey.Q, 0);
        }).ConfigureAwait(true))
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD()
        .OnKey(Keys.W, KeyModifiers.Control).Execute(() => _main._聚合.Conditions[ConditionSlotKey.C2].Active = true)
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD()
        .OnKey(Keys.D).CustomProbe(async () => await _skill.主动技能进入CD后续(Keys.D, () =>
        {
            _input.Press(VirtualKey.From(Keys.F1));
            _input.Press(VirtualKey.From(Keys.F1));
        }).ConfigureAwait(true))
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.D);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.D) == 1 ? "传" : "不传");
        })
        .Done();
}
#endif
