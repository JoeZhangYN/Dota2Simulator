// Phase 19G-3: 天怒 Strategy 迁 HeroPlan — Q/W/E/R 4 simple AfterEnterCD + D2 ToggleSlot(Q 循环鹰隼 mode 2) + D3 Execute (SetStep + Active 天怒秒人连招 C2 Step 状态机).
// 业务侧槽位映射: Q→C3 / W→C6 / E→C4 / R→C5 (Probe 顺序 C1=循环 C2=秒人 C3=Q C4=E C5=R C6=W); D2 toggle C1 + D3 设 C2 Active.
// HeroPlan clause 顺序累加 C1..C6 必须严格匹配业务 OnActivate Probe slot. 用 ToggleSlot 占 C1 (循环奥数鹰隼) + CustomProbe 占 C2 (天怒秒人连招).
// Phase 27A retry 2 S5 (2026-05-26): D3 秒人连招 inline switch → .StepMachine("D3_Combo") chain 迁:
//   - INV8 拆 SlotKey.Global 直写 (Pre 原 SetStep Global=1 改 .Initial(1); 新 Pre 桥接 StepMachineState step==0 → 1 解决 ResetMachine 后再次按 D3 的 re-arm).
//   - INV10 拆 Conditions[C1].Active = true 直写 (替为 ActivateClause("C1") 原语).
//   - 顺序就绪门控 step 1: WaitForProbeTrue 内嵌副作用 (W/E/Q DOTA2释放CD就绪技能 释放; 三者全释放完毕→预进 step 2) — escape hatch (predicate 内**读**+副作用, 禁内写 SetStep INV12).
//   - 多物品组合 step 1 末: UseItem 序列 + ParallelBatch 5 红杖 (等价原 _item.批量使用物品并行) + 羊刀 + Delay(33) + SetStep(2).
//   - R 释放门控 step 2: Conditional Cond=R 就绪 (!_skill.DOTA2判断技能是否CD); IfSteps Press(R)+SetStep(3) (R 就绪释放); ElseSteps ActivateClause("C1")+SetStep(3)+AbortIf 中断 (R 在 CD 启动循环鹰隼).
//   - sentinel step 3: ResetMachine → state=0; Pre 桥接 0 → 1 复位 (与 军团 F clause Pre -1 → 0 同 pattern).
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

[HeroStrategy("天怒", HeroAttribute.Intelligence)]
public sealed partial class 天怒Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.D2).ToggleSlot(Keys.Q, "循环鹰隼", "不循环鹰隼")  // C1: toggle 循环奥数鹰隼 (Probe 自循环 mode 2 of Q)
        // C2: D3 秒人连招 — Phase 27A retry 2 S5 .StepMachine "D3_Combo" 迁; Pre 桥接 ResetMachine 后 step==0 → 1 复位; .NoProbe() 终结合法 + lastIdx 锚 D3 clause.
        .OnKey(Keys.D3).Pre(() =>
        {
            if (_main._聚合.StepMachines.GetStep("D3_Combo") == 0)
                _main._聚合.StepMachines.SetStep("D3_Combo", 1);
        }).NoProbe()
        .StepMachine("D3_Combo", sub => sub
            .Initial(1)
            // Step 1: 顺序就绪门控 W/E/Q (DOTA2释放CD就绪技能 内 if CD-ready → Press 副作用; 三者全释放完毕一次性 burst items + 羊刀 Delay 后摇 + SetStep(2)).
            .Step(1).Do(
                new WaitForProbeTrue(handle =>
                {
                    bool wReady = _skill.DOTA2释放CD就绪技能(Keys.W);
                    bool eReady = _skill.DOTA2释放CD就绪技能(Keys.E);
                    bool qReady = _skill.DOTA2释放CD就绪技能(Keys.Q);
                    return wReady && eReady && qReady;
                }, IntervalMs: 33),
                new UseItem(Dota2_Pictrue.物品.阿托斯之棍_Tpl),  // 原 Common.Delay(0 * ...) 0 延迟 → UseItem 无 Delay 等价
                new UseItem(Dota2_Pictrue.物品.缚灵锁_Tpl),
                new UseItem(Dota2_Pictrue.物品.虚灵之刃_Tpl),
                new ParallelBatch(new StepCommand[]  // 5 红杖 — 等价原 _item.批量使用物品并行 Burst
                {
                    new UseItem(Dota2_Pictrue.物品.红杖_Tpl),
                    new UseItem(Dota2_Pictrue.物品.红杖2_Tpl),
                    new UseItem(Dota2_Pictrue.物品.红杖3_Tpl),
                    new UseItem(Dota2_Pictrue.物品.红杖4_Tpl),
                    new UseItem(Dota2_Pictrue.物品.红杖5_Tpl),
                }),
                new UseItem(Dota2_Pictrue.物品.羊刀_Tpl),
                new Delay(33),  // 原 Common.Delay(33 * _item.根据图片使用物品(羊刀)) — 羊刀 后摇延迟; 简化为固定 33ms.
                new SetStep(2)
            ).End()
            // Step 2: R 释放门控 (Cond = !_skill.DOTA2判断技能是否CD(R), 即 R 就绪). IfSteps Press(R)+SetStep(3); ElseSteps ActivateClause("C1") 循环鹰隼 + SetStep(3) + AbortIf 终止本 tick.
            .Step(2).Do(
                new Conditional(
                    Cond: handle => !_skill.DOTA2判断技能是否CD(Keys.R),
                    IfSteps: new StepCommand[]
                    {
                        new Press(Keys.R),
                        new SetStep(3),
                    },
                    ElseSteps: new StepCommand[]
                    {
                        new ActivateClause("C1"),
                        new SetStep(3),
                        new AbortIf(handle => true),
                    })
            ).End()
            // Step 3: sentinel — ResetMachine 设 state=0; Pre 桥接下次 D3 时 0 → 1 复位 (与 军团 F clause Pre -1 → 0 同 pattern).
            .Step(3).Do(
                new ResetMachine()
            ).End()
        )
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()  // C3: 奥数鹰隼
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD()  // C4: 上古封印
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()  // C5: 神秘之耀
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD()  // C6: 震荡光弹
        .RepeatThreshold(100)
        .Done();
}
#endif
