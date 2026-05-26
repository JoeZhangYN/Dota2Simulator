// Phase 27A retry2 S3 (2026-05-26): 骨法 R 状态机迁 .StepMachine — 删 fire-and-forget Task.Run + Common.Delay(200), 改 new Delay(200) StepCommand 显式.
// Pre 物品 burst (希瓦+纷争) 保留 .OnKey(R).Pre(...) (是 setup 不是 step machine); .CustomProbe(生命吸取去后摇) 替为 .NoProbe() 保 Pre clause 终结合法 + lastIdx 锚点 (.StepMachine 紧贴 R clause 后).
// 横向耦合保留: Q/E 仍读 _main._聚合.Skills.Step(SlotKey.R); 因此 StepMachine 内 Conditional/SetStepIf wet Cond 同步写回 Skills.SetStep 桥接 (Phase 27B/C/D 评估读侧迁移).
// D2 Execute (ToggleMode R + TTS) 走 _setups 路径不影响 _clauses[lastIdx] = R clause, .StepMachine 仍正确绑 R clause.
// Phase 19G-4 base: 4 CustomProbe (Q/E 动态 mode + W 物品组合 + R Step 状态机) + D2 Execute (ToggleMode R + TTS).
#if DOTA2
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.GameAutomation.Domain.StepMachine;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("骨法", HeroAttribute.Intelligence)]
public sealed partial class 骨法Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(幽冥轰爆去后摇)  // C1: 动态 mode (Step(R)>0 ? 10 : 0)
        .OnKey(Keys.W).CustomProbe(衰老去后摇)  // C2: 主动技能进入CD后续 + 5 红杖
        .OnKey(Keys.E).CustomProbe(幽冥守卫去后摇)  // C3: 动态 mode
        .OnKey(Keys.R).Pre(() =>
        {
            _item.根据图片使用物品(Dota2_Pictrue.物品.希瓦_Tpl);
            _item.根据图片使用物品(Dota2_Pictrue.物品.纷争_Tpl);
        }).NoProbe()  // Pre burst 保留, Step 状态机迁 .StepMachine("R_Mode", ...) 下方 chain (紧贴 R clause 后, 保 lastIdx 锚 R).
        .StepMachine("R_Mode", sub => sub
            .Initial(0)
            .Step(0).Do(
                new Conditional(
                    _ =>
                    {
                        if (_skill.DOTA2判断技能是否CD(Keys.R))
                        {
                            _main._聚合.Skills.SetStep(SlotKey.R, 0);  // 横向耦合读侧 bridge: Q/E 仍读 Skills.Step(SlotKey.R).
                            return true;
                        }
                        if (_main._聚合.Skills.Mode(SlotKey.R) == 1)
                            Press(Keys.W);
                        _main._聚合.Skills.SetStep(SlotKey.R, 1);
                        return false;
                    },
                    IfSteps: new StepCommand[] { new SetStep(0) },
                    ElseSteps: new StepCommand[] { new SetStep(1) })
            ).End()
            .Step(1).Do(
                new Delay(200),                                                                                        // 替代原 _=Task.Run(()=>Common.Delay(200)) fire-and-forget.
                new SetStepIf(_ => { _main._聚合.Skills.SetStep(SlotKey.R, 2); return true; }, 2)                       // 同步写回 Skills.Step(SlotKey.R) 桥接 + StepMachineState SetStep(2).
            ).End()
            .Step(2).Do(
                new Conditional(
                    _ =>
                    {
                        if (_skill.DOTA2判断是否持续施法()) return true;
                        _main._聚合.Skills.SetStep(SlotKey.R, 0);  // 横向耦合读侧 bridge.
                        走A();
                        return false;
                    },
                    IfSteps: Array.Empty<StepCommand>(),
                    ElseSteps: new StepCommand[] { new SetStep(0) })
            ).End()
        )
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.R);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.R) == 1 ? "吸取接衰老" : "吸取不接衰老");
        })
        .Done();

    private async Task<bool> 幽冥轰爆去后摇()
        => await _skill.技能通用判断(Keys.Q, _main._聚合.Skills.Step(SlotKey.R) > 0 ? 10 : 0).ConfigureAwait(true);

    private async Task<bool> 衰老去后摇()
    {
        return await _skill.主动技能进入CD后续(Keys.W, () =>
        {
            _item.批量使用物品并行(
                Dota2_Pictrue.物品.红杖_Tpl,
                Dota2_Pictrue.物品.红杖2_Tpl,
                Dota2_Pictrue.物品.红杖3_Tpl,
                Dota2_Pictrue.物品.红杖4_Tpl,
                Dota2_Pictrue.物品.红杖5_Tpl);
        }).ConfigureAwait(true);
    }

    private async Task<bool> 幽冥守卫去后摇()
        => await _skill.技能通用判断(Keys.E, _main._聚合.Skills.Step(SlotKey.R) > 0 ? 10 : 0).ConfigureAwait(true);
}
#endif
