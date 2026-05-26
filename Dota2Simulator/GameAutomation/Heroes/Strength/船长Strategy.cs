// Phase 27B S2 (2026-05-26): 船长 Strategy 4 method 迁 .StepMachine + RegisterProbe 内嵌 async lambda — 0 fire-and-forget Task.Run (业务侧) + 0 Common.Delay hardcode (顶层 method 内, callback closure 内保留) + 0 SkillEngine helper 顶层直调.
// Phase 19G-3 base: 船长 Q/E 2 CustomProbe + D2 Execute (Step R=1 + Press E 跳船宏); C3/C4 由 Probe 内跨 clause 副作用驱动 (Phase 16 #6 持续).
// 27B 落点: (1) Q/E .StepMachine(captain-Q/E) 单 Step(0) Do(AwaitSkillHelper) inline callback (D1 决断: callback 由 SkillEngine helper sync invoke afterAction, 不经 Runner 调度, 不拆原语); (2) C3/C4 RegisterProbe 保留原 DSL, lambda 内嵌 async (D2 决断: ConditionDelegateBitmap 真实签名 delegate Task<bool>() 直接接 async, 无须扩 DSL); (3) _全局模式e_lock 删 (StepMachine 单 machine 顺序语义天然替代; HeroLoopHost.cs:44 同名 instance field 不动).
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

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("船长", HeroAttribute.Strength)]
public sealed partial class 船长Strategy : IHeroStrategy
{
    // Phase 21A: E 键 short-circuit (Step(E)==1 跳过) 改用 WhenStepNotEq DSL Guard, 删 override OnKeyAsync.
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).NoProbe().StepMachine("captain-Q", sub => sub
            .Initial(0)
            .Step(0).Do(
                new AwaitSkillHelper(() => _skill.主动技能释放后续(Keys.Q, () =>
                {
                    _main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
                    _main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());
                    if (!_main.Session!.IsPaused && _main._聚合.Skills.Step(SlotKey.E) == 1)
                    {
                        Common.Delay(1350, _main._聚合.Skills.Time(SlotKey.Q));
                        _main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
                        Press(Keys.E);
                    }
                }))
            ).End()
        )
        .OnKey(Keys.E).WhenStepNotEq(SlotKey.E, 1).NoProbe().StepMachine("captain-E", sub => sub
            .Initial(0)
            .Step(0).Do(
                new AwaitSkillHelper(() => _skill.主动技能释放后续(Keys.E, () =>
                {
                    int 步骤e = _main._聚合.Skills.Step(SlotKey.E);
                    if (步骤e == 1) return;
                    _main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒());
                    if (_main._聚合.Skills.Step(SlotKey.R) == 1)
                    {
                        Press(Keys.R);
                        _main._聚合.Skills.SetStep(SlotKey.R, 0);
                    }
                    _main._聚合.Skills.SetStep(SlotKey.E, 1);
                    _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
                    int 等待时间 = (int)Math.Floor(3000 * _main._聚合.Attack.状态抗性倍数) - 1670;
                    Common.Delay(等待时间, _main._聚合.Skills.Time(SlotKey.E));
                    _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
                }))
            ).End()
        )
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.SetStep(SlotKey.R, 1);
            Press(Keys.E);
        })
        .RegisterProbe(ConditionSlotKey.C3, async () => await _skill.主动技能进入CD后续(Keys.E, () =>
        {
            Common.Delay(2000);
            _main._聚合.Skills.SetStep(SlotKey.E, 0);
        }).ConfigureAwait(true))
        .RegisterProbe(ConditionSlotKey.C4, async () => await _skill.主动技能已就绪后续(Keys.Q, () => Press(Keys.Q)).ConfigureAwait(true))
        .Done();
}
#endif
