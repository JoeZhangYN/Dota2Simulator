// Phase 19G-3: VS Strategy 迁 HeroPlan — Q AfterCast + R AfterCast (移形换位 C2) + W Execute (hard-code C2 共享 R, 业务侧 W/R 同 C3 实际语义即 W→移形换位). C3 恐怖波动 dead Probe (业务 OnKeyAsync 不触发) 不迁.
#if DOTA2
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("VS", HeroAttribute.Universal)]
public sealed partial class VSStrategy : IHeroStrategy
{
    // Phase 21A: 删 override OnKeyAsync — W 键 hard-code C2 Active 改用 Execute setup DSL (匹配 trigger + lambda 设 Active).
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.W).SetActive(ConditionSlotKey.C2)  // 业务原 W/R 都触发 C3 移形换位; 现 R 走 C2 (clause 顺序), W 走 setup hard-code C2 等价
        .Done();
}
#endif
