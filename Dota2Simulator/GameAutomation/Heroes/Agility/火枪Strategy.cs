// Phase 16 C1b: 火枪 Strategy 迁 HeroPlan — OnEveryKey + AdjustLegSwapDynamic(D, ctx => HasShard) 每键无条件动态切假腿; Q/D/R AfterCast; E CustomProbe (主动技能进入CD后续 + 疯狂面具物品 + 通用技能后续).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("火枪", HeroAttribute.Agility)]
public sealed partial class 火枪Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .LegSwap(Keys.W, alwaysSwap: false)
        .OnEveryKey().AdjustLegSwapDynamic(Keys.D, ctx => ctx.Aggregate.HasShard)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.E).CustomProbe(async () => await _skill.主动技能进入CD后续(Keys.E, () =>
        {
            _ = _item.根据图片使用物品(Dota2_Pictrue.物品.疯狂面具_Tpl);
            _skill.通用技能后续动作();
        }).ConfigureAwait(true))
        .OnKey(Keys.D).CastSkill(Keys.D).AfterCast()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .Done();
}
#endif
