// Phase 15 C2: 混沌 Strategy 迁 HeroPlan — Q Pre (_item.物品使用) + CustomProbe (动态 continueKey: Skills.Mode(Q)==1 ? W : A), W/R 简单 mode 11/1, D2 Execute (ToggleMode + TTS), D3 Execute (切臂章同步化), LegSwap E.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("混沌", HeroAttribute.Strength)]
public sealed partial class 混沌Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).Pre(() =>
        {
            _item.根据图片使用物品(Dota2_Pictrue.物品.紫苑);
            _item.根据图片使用物品(Dota2_Pictrue.物品.血棘);
        }).CustomProbe(async () => await _skill.技能通用判断(
            Keys.Q,
            1,
            要接的按键: _main._聚合.Skills.Mode(SlotKey.Q) == 1 ? Keys.W : Keys.A).ConfigureAwait(true))
        .OnKey(Keys.W).CastSkill(Keys.W).AfterCastLegOnly()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.Q);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Q) == 1 ? "混乱之箭接拉" : "混乱之箭接A");
        })
        .OnKey(Keys.D3).Execute(() =>
        {
            Keys k = _item.根据图片获取物品按键(Dota2_Pictrue.物品.臂章_开启);
            if (k != Keys.Escape)
            {
                _input.Press(VirtualKey.From(k));
                Common.Delay(15);
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.魔棒);
                _ = _item.根据图片自我使用物品(Dota2_Pictrue.物品.吊坠);
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.仙草);
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.假腿_力量腿);
                Common.Delay(15);
                _input.Press(VirtualKey.From(k));
                _main._聚合.LegSwap.条件假腿敏捷 = false;
                _item.要求保持假腿();
            }
        })
        .LegSwap(Keys.E, alwaysSwap: false)
        .Done();
}
#endif
