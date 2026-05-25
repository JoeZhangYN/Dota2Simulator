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

[HeroStrategy("炸弹人", HeroAttribute.Intelligence)]
public sealed partial class 炸弹人Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 粘性炸弹去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 活性电击去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 爆破起飞去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 爆破后接3雷粘性炸弹;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            _item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.ToggleMode(SlotKey.E);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.E) == 0 ? "起飞后不接3连炸弹" : "起飞后接3连炸弹");
        }

        return Task.CompletedTask;
    }

    private async Task<bool> 粘性炸弹去后摇()
    {
        void 粘性炸弹后()
        {
            //RightClick();
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (_skill.DOTA2判断技能是否CD(Keys.Q))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        粘性炸弹后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 活性电击去后摇()
    {
        void 活性电击后()
        {
            //RightClick();
            _input.Press(VirtualKey.From(Keys.A));
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        活性电击后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 爆破起飞去后摇()
    {
        void 爆破起飞后()
        {
            //RightClick();
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
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        爆破起飞后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    // todo 逻辑修改
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
