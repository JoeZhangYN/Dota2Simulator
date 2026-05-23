#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.GameAutomation.Ports;

using Dota2Simulator.GameAutomation.Domain.Perception;
namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>命运2策略——迁移自 _main.根据当前英雄增强 的 case "命运2"。</summary>
[HeroStrategy("命运2", HeroAttribute.Universal)]
public sealed partial class 命运2Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.From(Keys.V))
        {
            _ = Task.Run(命运2冰好耶);
        }
        else if (key == VirtualKey.From(Keys.B))
        {
            _ = Task.Run(命运2冰好耶1);
        }
        else if (key == VirtualKey.Q)
        {
            _input.MouseMoveTo(new ScreenPoint(1920, 1080));
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     耗时10ms左右 延迟 原先50ms
    ///     CTRL SHIFT 键无法模拟？
    /// </summary>
    /// <param name="Key"></param>
    private void 命运2按键(Keys Key)
    {
        _input.KeyDown(VirtualKey.From(Key));
        Common.Delay(10); // 命运2操作延迟不然不切
        _input.KeyUp(VirtualKey.From(Key));
    }

    private void 命运2冰好耶()
    {
        _input.KeyDown(VirtualKey.From(Keys.W));
        命运2按键(Keys.LShiftKey);
        Common.Delay(150);
        _input.KeyUp(VirtualKey.From(Keys.W));
        命运2按键(Keys.LControlKey);
        Common.Delay(150);
        命运2按键(Keys.C);
        Common.Delay(100);
        _input.KeyDown(VirtualKey.From(Keys.S));
        Common.Delay(500);
        _input.KeyUp(VirtualKey.From(Keys.S));
    }

    private void 命运2冰好耶1()
    {
        _input.KeyDown(VirtualKey.From(Keys.W));
        命运2按键(Keys.LShiftKey);
        Common.Delay(150);
        _input.KeyUp(VirtualKey.From(Keys.W));
        命运2按键(Keys.LControlKey);
        Common.Delay(150);
        _input.MouseClick(MouseButton.Left);
        Common.Delay(100);
        _input.KeyDown(VirtualKey.From(Keys.S));
        Common.Delay(500);
        _input.KeyUp(VirtualKey.From(Keys.S));
    }
}
#endif
