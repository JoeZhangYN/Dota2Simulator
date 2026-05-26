// Phase 19G-3: 军团 Strategy 迁 HeroPlan — Q/W/R 3 simple AfterX + F 决斗 Step 状态机 + D2 Execute (ToggleMode Global + TTS) + LegSwap E false.
// Phase 27A retry 2 S4 (2026-05-26): F 决斗 inline switch (4 段 state machine) 迁 .StepMachine("Duel", ...) DSL —
//   拆 SlotKey.Global (Skills 子聚合) → StepMachineState 子聚合 ("Duel" machine, Initial(-1)); 删 InitSkillStep + 决斗() inline switch.
//   step0 = 一阶 burst (臂章+魂戒+相位鞋 UseItem 序列) + W CD 重路由 (Conditional + PressAlt Alt 自我施法 + AbortIf reset 本 tick) + 刃甲 + SetStep(1).
//   step1 = 4 跳刀变体 ParallelBatch (跳刀/力量跳刀/智力跳刀/敏捷跳刀 UseItem; ParallelBatch 等价原 _item.批量使用物品并行 Burst) + SetStep(2).
//   step2 = 二阶 burst (紫苑/血棘/否决/散失/散魂/深渊之刃 UseItem 序列) + SetStep(3).
//   step3 = Press(A) 走A + Conditional (R 就绪释放成功 → Delay 60 等后摇; R 失败 fallback SetStep(-1) 全 reset).
// D2 Execute ToggleMode/Mode(SlotKey.Global) 保留 — mode 是 Skills 子聚合独立维度 (与 Step 子维不同), 不在 INV8 拆桥范围.
// Pre lambda 桥接 StepMachineState -1 → 0 (Initial 由 Apply 写入; Pre 仅启动一次新决斗序列, 与 step3 fallback SetStep(-1) 形成 -1 ↔ 0..3 循环).
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

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("军团", HeroAttribute.Strength)]
public sealed partial class 军团Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()  // C1: 压倒性优势
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast()  // C2: 强攻
        // C3: F 决斗 — .StepMachine "Duel" 4 step 状态机 (Phase 27A retry 2 S4 迁); Pre 桥接 -1 → 0; .NoProbe() 终结合法 + lastIdx 锚 F clause.
        .OnKey(Keys.F).Pre(() =>
        {
            if (_main._聚合.StepMachines.GetStep("Duel") == -1)
                _main._聚合.StepMachines.SetStep("Duel", 0);
        }).NoProbe()
        .StepMachine("Duel", sub => sub
            .Initial(-1)
            // Step 0: 一阶 burst (臂章/魂戒/相位鞋) + W CD 重路由 (PressAlt 自我施法 + AbortIf 中断本 tick) + 刃甲 + SetStep(1).
            .Step(0).Do(
                new UseItem(Dota2_Pictrue.物品.臂章_Tpl),
                new UseItem(Dota2_Pictrue.物品.魂戒_Tpl),
                new UseItem(Dota2_Pictrue.物品.相位鞋_Tpl),
                new Conditional(
                    Cond: _ => _skill.DOTA2判断技能是否CD(Keys.W),
                    IfSteps: new StepCommand[] { new PressAlt(Keys.W, Alt: true), new AbortIf(_ => true) },
                    ElseSteps: Array.Empty<StepCommand>()),
                new UseItem(Dota2_Pictrue.物品.刃甲_Tpl),
                new SetStep(1)
            ).End()
            // Step 1: 4 跳刀变体 ParallelBatch Burst (等价原 _item.批量使用物品并行 行为) + SetStep(2).
            .Step(1).Do(
                new ParallelBatch(new StepCommand[]
                {
                    new UseItem(Dota2_Pictrue.物品.跳刀_Tpl),
                    new UseItem(Dota2_Pictrue.物品.跳刀_力量跳刀_Tpl),
                    new UseItem(Dota2_Pictrue.物品.跳刀_智力跳刀_Tpl),
                    new UseItem(Dota2_Pictrue.物品.跳刀_敏捷跳刀_Tpl),
                }),
                new SetStep(2)
            ).End()
            // Step 2: 二阶 burst (紫苑/血棘/否决/散失/散魂/深渊之刃) + SetStep(3).
            .Step(2).Do(
                new UseItem(Dota2_Pictrue.物品.紫苑_Tpl),
                new UseItem(Dota2_Pictrue.物品.血棘_Tpl),
                new UseItem(Dota2_Pictrue.物品.否决_Tpl),
                new UseItem(Dota2_Pictrue.物品.散失_Tpl),
                new UseItem(Dota2_Pictrue.物品.散魂_Tpl),
                new UseItem(Dota2_Pictrue.物品.深渊之刃_Tpl),
                new SetStep(3)
            ).End()
            // Step 3: 走A + R 释放尝试; R 就绪释放成功 → Delay 60 等后摇 (后续 D2 决斗 mode toggle 节奏); R 失败 fallback SetStep(-1) 全 reset.
            .Step(3).Do(
                new Press(Keys.A),
                new Conditional(
                    Cond: _ => _skill.DOTA2释放CD就绪技能(Keys.R),
                    IfSteps: new StepCommand[] { new Delay(60) },
                    ElseSteps: new StepCommand[] { new SetStep(-1) })
            ).End()
        )
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()  // C4: 决斗去后摇 (mode 1)
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.Global);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Global) == 1 ? "跳刀决斗" : "直接决斗");
        })
        .Done();
}
#endif
