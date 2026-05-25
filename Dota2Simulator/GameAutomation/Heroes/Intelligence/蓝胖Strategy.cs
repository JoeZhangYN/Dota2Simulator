#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("蓝胖", HeroAttribute.Intelligence)]
public sealed partial class 蓝胖Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 火焰轰爆去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 引燃去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 嗜血术去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 烈火护盾去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 未精通火焰轰爆去后摇;
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
        else if (key == VirtualKey.F)
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.ToggleMode(SlotKey.W);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.W) == 0 ? "引燃接轰爆" : "引燃不接轰爆");
        }

        return Task.CompletedTask;
    }

    private async Task<bool> 火焰轰爆去后摇()
    {
        void 火焰轰爆后()
        {
            _input.MouseClick(MouseButton.Right);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.Q))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        火焰轰爆后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 引燃去后摇()
    {
        void 引燃后()
        {
            switch (_main._聚合.Skills.Mode(SlotKey.W))
            {
                case 1:
                    _input.Press(VirtualKey.From(Keys.Q));
                    break;
                default:
                    _input.MouseClick(MouseButton.Right);
                    break;
            }
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        引燃后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 嗜血术去后摇()
    {
        void 嗜血术后()
        {
            _input.MouseClick(MouseButton.Right);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.E))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        嗜血术后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 未精通火焰轰爆去后摇()
    {
        void 未精通火焰轰爆后()
        {
            _input.MouseClick(MouseButton.Right);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.D))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        未精通火焰轰爆后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 烈火护盾去后摇()
    {
        void 烈火护盾后()
        {
            _input.MouseClick(MouseButton.Right);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.F))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        烈火护盾后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
