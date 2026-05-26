// Phase 19D: 沉默 Strategy 迁 HeroPlan — Q/R 2 CustomProbe (奥数诅咒 Mode 切换复杂 + 全领域沉默 1200ms 超时同质模板). 大招前纷争 helper 保留作 Q Probe 内部调用.
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

[HeroStrategy("沉默", HeroAttribute.Intelligence)]
public sealed partial class 沉默Strategy : IHeroStrategy
{
    private HeroPlan? _plan;

    public override void OnActivate(HeroContext ctx) => GetPlan().Apply(ctx, _skill);

    public override Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx) => GetPlan().DispatchAsync(trigger, ctx, _item);

    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(奥数诅咒去后摇)
        .OnKey(Keys.R).CustomProbe(全领域沉默去后摇)
        .Done();

    private async Task<bool> 大招前纷争()
    {
        Common.Delay(33 * (
            _item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃_Tpl)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.纷争_Tpl)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖_Tpl)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖2_Tpl)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖3_Tpl)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖4_Tpl)
            + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖5_Tpl)
        ));
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 奥数诅咒去后摇()
    {
        async void 奥数诅咒后()
        {
            _main._聚合.Skills.SetTime(SlotKey.Q, -1);
            switch (_main._聚合.Skills.Mode(SlotKey.Q))
            {
                case < 1:
                    _ = await 大招前纷争().ConfigureAwait(true);
                    _input.Press(VirtualKey.From(Keys.E));
                    break;
                case 1:
                    _ = await 大招前纷争().ConfigureAwait(true);
                    Common.Delay(1300);
                    _input.Press(VirtualKey.From(Keys.E));
                    break;
                case 2:
                    _input.Press(VirtualKey.From(Keys.A));
                    break;
            }
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Q) > 1200 && _main._聚合.Skills.Time(SlotKey.Q) != -1)
        {
            奥数诅咒后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }
        if (_skill.DOTA2判断技能是否CD(Keys.Q))
            return await Task.FromResult(true).ConfigureAwait(true);
        奥数诅咒后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 全领域沉默去后摇()
    {
        void 全领域沉默后()
        {
            _main._聚合.Skills.SetTime(SlotKey.R, -1);
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.R) > 1200 && _main._聚合.Skills.Time(SlotKey.R) != -1)
        {
            全领域沉默后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }
        if (_skill.DOTA2判断技能是否CD(Keys.R))
            return await Task.FromResult(true).ConfigureAwait(true);
        全领域沉默后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
