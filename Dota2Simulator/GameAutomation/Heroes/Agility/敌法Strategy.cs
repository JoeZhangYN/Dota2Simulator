// Phase 16 C1a: 敌法 Strategy 迁 HeroPlan — F1+HasShard AdjustLegSwap(D,true), W CustomProbe (主动技能释放后续 lambda + 分身物品组合 + 分身一齐攻击 helper), E AfterEnterCDLegOnly, R AfterCast, D+HasShard AfterEnterCDLegOnly, D2 Execute (SetMode+TTS 一次性).
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

[HeroStrategy("敌法", HeroAttribute.Agility)]
public sealed partial class 敌法Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .LegSwap(Keys.Q, alwaysSwap: false)
        .OnKey(Keys.F1).WhenHasShard().AdjustLegSwap(Keys.D, paramBool: true)
        .OnKey(Keys.W).CustomProbe(async () => await _skill.主动技能释放后续(Keys.W, () =>
        {
            if (_main._聚合.Skills.Mode(SlotKey.W) == 1)
            {
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.幻影斧);
                分身一齐攻击();
                _ = _item.根据图片使用物品(Dota2_Pictrue.物品.深渊之刃);
                _main._聚合.Skills.SetMode(SlotKey.W, 0);
            }
            _skill.通用技能后续动作();
        }).ConfigureAwait(true))
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCDLegOnly()
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()
        .OnKey(Keys.D).WhenHasShard().CastSkill(Keys.D).AfterEnterCDLegOnly()
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.SetMode(SlotKey.W, 1);
            Dota2Simulator.TTS.TTS.Speak("闪烁分身晕锤一次");
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
