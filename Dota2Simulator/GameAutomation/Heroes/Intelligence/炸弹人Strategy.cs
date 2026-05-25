// Phase 19D: 炸弹人 Strategy 迁 HeroPlan — Q/W CustomProbe (CD + Press A 同质) + E Pre(_item 纷争) + CustomProbe (爆破后 case 1 跨 clause Conditions[C4].Active 副作用) + D2 Execute (ToggleMode E + TTS) + RegisterProbe(C4, 爆破后接3雷粘性炸弹).
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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("炸弹人", HeroAttribute.Intelligence)]
public sealed partial class 炸弹人Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(粘性炸弹去后摇)
        .OnKey(Keys.W).CustomProbe(活性电击去后摇)
        .OnKey(Keys.E).Pre(() => _item.根据图片使用物品(Dota2_Pictrue.物品.纷争_Tpl)).CustomProbe(爆破起飞去后摇)
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.E);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.E) == 0 ? "起飞后不接3连炸弹" : "起飞后接3连炸弹");
        })
        .RegisterProbe(ConditionSlotKey.C4, 爆破后接3雷粘性炸弹)
        .Done();

    private async Task<bool> 粘性炸弹去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.Q))
            return await Task.FromResult(true).ConfigureAwait(true);
        _input.Press(VirtualKey.From(Keys.A));
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 活性电击去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.W))
            return await Task.FromResult(true).ConfigureAwait(true);
        _input.Press(VirtualKey.From(Keys.A));
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 爆破起飞去后摇()
    {
        void 爆破起飞后()
        {
            _input.Press(VirtualKey.From(Keys.A));
            Common.Delay(750);
            switch (_main._聚合.Skills.Mode(SlotKey.E))
            {
                case 1:
                    _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
                    _main._聚合.Skills.SetTarget(SlotKey.R, Control.MousePosition);
                    _main._聚合.Skills.SetTime(SlotKey.R, Common.获取当前时间毫秒());
                    break;
                case 0:
                    break;
            }
        }

        if (_skill.DOTA2判断技能是否CD(Keys.E))
            return await Task.FromResult(true).ConfigureAwait(true);
        爆破起飞后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 爆破后接3雷粘性炸弹()
    {
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.R) >= 3000)
        {
            _main._聚合.Skills.SetTime(SlotKey.R, -1);
            return await Task.FromResult(false).ConfigureAwait(true);
        }
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
