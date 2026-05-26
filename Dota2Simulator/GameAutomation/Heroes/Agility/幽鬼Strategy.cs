// Phase 16 C1a: 幽鬼 Strategy 迁 HeroPlan — F1+HasShard AdjustLegSwap(E,true), Q AfterCast(false), R/D CustomProbe (Mode 检查物品组合), E AfterEnterCD, D2 Execute (ToggleMode F + TTS).
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

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

[HeroStrategy("幽鬼", HeroAttribute.Agility)]
public sealed partial class 幽鬼Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public override void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .LegSwap(Keys.W, alwaysSwap: false)
        .LegSwap(Keys.E, alwaysSwap: false)
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.E, paramBool: true)
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast(continueAttack: false)
        .OnKey(Keys.R).CustomProbe(async () => await _skill.主动技能释放后续(Keys.R, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                _input.Press(VirtualKey.From(Keys.D));
            }
        }).ConfigureAwait(true))
        .OnKey(Keys.D).CustomProbe(async () => await _skill.主动技能进入CD后续(Keys.D, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.F) == 1)
            {
                if (_item.根据图片使用物品(Dota2_Pictrue.物品.幻影斧_Tpl) == 1)
                {
                    分身一齐攻击();
                }
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.否决_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.紫苑_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.血棘_Tpl));
            }
            _item.要求保持假腿();
            _input.Press(VirtualKey.From(Keys.A));
        }).ConfigureAwait(true))
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD()
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.F);
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.F) == 1 ? "如影随形分身" : "关闭随形分身");
        })
        .Done();

    /// <summary>因为有0.1秒的分裂时间，所以必须等待——复制自 _main.分身一齐攻击。</summary>
    private void 分身一齐攻击()
    {
        Common.Delay(140);
        _input.KeyDown(VirtualKey.From(Keys.Control));
        _input.Press(VirtualKey.From(Keys.A));
        _input.KeyUp(VirtualKey.From(Keys.Control));
    }
}
#endif
