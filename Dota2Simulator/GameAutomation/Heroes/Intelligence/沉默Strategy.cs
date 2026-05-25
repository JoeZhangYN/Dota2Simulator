#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("沉默", HeroAttribute.Intelligence)]
public sealed partial class 沉默Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 奥数诅咒去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 全领域沉默去后摇;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }

        return Task.CompletedTask;
    }

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
            // RightClick();
            // _input.Press(VirtualKey.From(Keys.A));
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

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Q) > 1200 && _main._聚合.Skills.Time(SlotKey.Q) != -1)
        {
            奥数诅咒后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.Q))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        奥数诅咒后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 全领域沉默去后摇()
    {
        void 全领域沉默后()
        {
            _main._聚合.Skills.SetTime(SlotKey.R, -1);
            // RightClick();
            _input.Press(VirtualKey.From(Keys.A));
        }

        // 超时则切回 总体释放时间
        if (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.R) > 1200 && _main._聚合.Skills.Time(SlotKey.R) != -1)
        {
            全领域沉默后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.R))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        全领域沉默后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
