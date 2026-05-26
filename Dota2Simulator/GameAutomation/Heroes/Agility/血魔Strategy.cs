// Phase 16 C2: 血魔 Strategy 迁 HeroPlan — W CustomProbe (主动技能释放后续 lambda 血祭物品组合 + MousePosition 保留), R AfterCast, Q AfterEnterCD, Q+Alt Execute 共享 C3 (血怒).
#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("血魔", HeroAttribute.Agility)]
public sealed partial class 血魔Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public override void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.W).CustomProbe(async () => await _skill.主动技能释放后续(Keys.W, () =>
        {
            _input.MouseClick(MouseButton.Right);
            _input.Press(VirtualKey.From(Keys.A));
            _item.要求保持假腿();
            Common.Delay(2400);
            Point p = Control.MousePosition;
            _input.MouseMoveTo(new ScreenPoint(601, 988));
            if (_skill.DOTA2释放CD就绪技能(Keys.Q))
            {
                _input.MouseMoveTo(new ScreenPoint(p.X, p.Y));
            }
        }).ConfigureAwait(true))
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()
        .OnKey(Keys.Q, KeyModifiers.Alt).Execute(() => _main._聚合.Conditions[ConditionSlotKey.C3].Active = true)
        .Done();
}
#endif
