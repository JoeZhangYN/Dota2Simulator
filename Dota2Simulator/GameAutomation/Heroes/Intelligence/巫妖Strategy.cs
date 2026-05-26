// Phase 27A retry2 S3 (2026-05-26): 巫妖 E 状态机迁 .StepMachine — 删 fire-and-forget Task.Run + Common.Delay(200), 改 new Delay(200) StepCommand 显式.
// Phase 16 C2 base: 5 全 CustomProbe (GetStep("E_Mode")>0 ? 11/10 : 1/0 动态 mode, Q/W/R Step 状态机 11/1 切换), W+Alt Execute 共享 C2.
// Phase 27C-A (2026-05-26): 横向耦合读侧迁完 — Q/W/R/D 改读 StepMachines.GetStep("E_Mode"); 4 处 wet bridge Skills.SetStep(SlotKey.E, X) 全数删. StepMachine 子聚合成 SSOT (handoff §3091 #2 闭环).
// S3 设计折衷: 走A() + 羊刀 是任意 side-effect, 19 原语无 InvokeAction 类直接表达, 走 escape hatch (predicate 内嵌副作用 + 返 bool) 与 S5 一致 pattern.
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

[HeroStrategy("巫妖", HeroAttribute.Intelligence)]
public sealed partial class 巫妖Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(async () => await _skill.技能通用判断(Keys.Q, _main._聚合.StepMachines.GetStep("E_Mode") > 0 ? 11 : 1).ConfigureAwait(true))
        .OnKey(Keys.W).CustomProbe(async () => await _skill.技能通用判断(Keys.W, _main._聚合.StepMachines.GetStep("E_Mode") > 0 ? 11 : 1, false).ConfigureAwait(true))
        .OnKey(Keys.W, KeyModifiers.Alt).SetActive(ConditionSlotKey.C2)
        .OnKey(Keys.E).NoProbe()  // Step 状态机迁 .StepMachine("E_Mode", ...) 下方 chain; NoProbe 保 clause 终结合法 + lastIdx 锚点.
        .StepMachine("E_Mode", sub => sub
            .Initial(0)
            .Step(0).Do(
                new Conditional(
                    _ => _skill.DOTA2判断技能是否CD(Keys.E),
                    IfSteps: new StepCommand[] { new SetStep(0) },
                    ElseSteps: new StepCommand[] { new SetStep(1) })
            ).End()
            .Step(1).Do(
                new Delay(200),                                                                                        // 替代原 _=Task.Run(()=>Common.Delay(200)) fire-and-forget.
                new SetStep(2)                                                                                         // Phase 27C-A: StepMachineState SetStep(2), wet bridge 已删.
            ).End()
            .Step(2).Do(
                new Conditional(
                    _ =>
                    {
                        if (_skill.DOTA2判断是否持续施法()) return true;
                        走A();
                        _item.根据图片使用物品(Dota2_Pictrue.物品.羊刀_Tpl);
                        return false;
                    },
                    IfSteps: Array.Empty<StepCommand>(),
                    ElseSteps: new StepCommand[] { new SetStep(0) })
            ).End()
        )
        .OnKey(Keys.R).CustomProbe(async () => await _skill.技能通用判断(Keys.R, _main._聚合.StepMachines.GetStep("E_Mode") > 0 ? 11 : 1).ConfigureAwait(true))
        .OnKey(Keys.D).CustomProbe(async () => await _skill.技能通用判断(Keys.D, _main._聚合.StepMachines.GetStep("E_Mode") > 0 ? 10 : 0).ConfigureAwait(true))
        .Done();
}
#endif
