// Phase 19G-3: VS Strategy 迁 HeroPlan — Q AfterCast + R AfterCast (移形换位 C2) + W Execute (hard-code C2 共享 R, 业务侧 W/R 同 C3 实际语义即 W→移形换位). C3 恐怖波动 dead Probe (业务 OnKeyAsync 不触发) 不迁.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("VS", HeroAttribute.Universal)]
public sealed partial class VSStrategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .Done();

    protected override HeroPlan BuildPlan() => _plan;

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        // W 共享 C2 移形换位 (业务原 W/R 都触发 C3 移形换位 Probe, 现 R 走 BuildPlan 占 C2; W 走 Execute hard-code C2.Active=true 等价行为).
        if (trigger.Key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
            return Task.CompletedTask;
        }
        return base.OnKeyAsync(trigger, ctx);
    }
}
#endif
