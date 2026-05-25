// Phase 13 C1: 尸王 Strategy 迁 HeroPlan — 4 helper 全 _skill.技能通用判断. E 键映 Keys.R 释放是特殊键位 (墓碑技能槽), DSL 支持 TriggerKey != SkillKey.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("尸王", HeroAttribute.Strength)]
public sealed partial class 尸王Strategy : IHeroStrategy
{
    private static readonly HeroPlan _plan = HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCast()
        .OnKey(Keys.E).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterEnterCD()
        .Done();

    public void OnActivate(HeroContext ctx) => _plan.Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => _plan.DispatchAsync(trigger, ctx, _item);
}
#endif
