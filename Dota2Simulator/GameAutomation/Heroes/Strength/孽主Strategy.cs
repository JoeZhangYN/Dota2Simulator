// Phase 14 C2: 孽主 Strategy 迁 HeroPlan + CustomProbe DSL — Q 简单 AfterCast, W 自定义 (主动技能释放后续 lambda 内 MouseClick + DOTA2释放CD就绪技能 + Press(A)).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("孽主", HeroAttribute.Strength)]
public sealed partial class 孽主Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()
        .OnKey(Keys.W).CustomProbe(async 句柄 => await _skill.主动技能释放后续(Keys.W, () =>
        {
            _input.MouseClick(MouseButton.Right);
            if (_skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄))
            {
                return;
            }
            _input.Press(VirtualKey.From(Keys.A));
        }).ConfigureAwait(true))
        .Done();
}
#endif
