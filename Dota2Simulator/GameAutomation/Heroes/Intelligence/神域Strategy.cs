#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("神域", HeroAttribute.Intelligence)]
public sealed partial class 神域Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 命运敕令去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 涤罪之焰去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 虚妄之诺去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 天命之雨去后摇;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            _main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private async Task<bool> 命运敕令去后摇()
    {
        async Task 命运敕令后()
        {
            await Task.Run(() => _input.MouseClick(MouseButton.Right)).ConfigureAwait(true);

            // _input.Press(VirtualKey.From(Keys.A));
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        命运敕令后().Start();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 涤罪之焰去后摇()
    {
        async Task 涤罪之焰后()
        {
            await Task.Run(() => { _input.Press(VirtualKey.From(Keys.A)); }).ConfigureAwait(true);
            // RightClick();
        }

        if (_skill.DOTA2判断技能是否CD(Keys.E))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        涤罪之焰后().Start();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 虚妄之诺去后摇()
    {
        async Task 虚妄之诺后()
        {
            await Task.Run(() => { _input.Press(VirtualKey.From(Keys.A)); }).ConfigureAwait(true);
            // _input.Press(VirtualKey.From(Keys.A));
        }

        if (_skill.DOTA2判断技能是否CD(Keys.R))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        虚妄之诺后().Start();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 天命之雨去后摇()
    {
        void 天命之雨后()
        {
            _input.MouseClick(MouseButton.Right);
            // _input.Press(VirtualKey.From(Keys.A));
        }

        if (_skill.DOTA2判断技能是否CD(Keys.D))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        天命之雨后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
