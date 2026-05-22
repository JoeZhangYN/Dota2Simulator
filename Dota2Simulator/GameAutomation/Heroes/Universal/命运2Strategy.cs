#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games;
using Dota2Simulator.KeyboardMouse;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>命运2策略——迁移自 Main.根据当前英雄增强 的 case "命运2"。</summary>
public sealed class 命运2Strategy : IHeroStrategy
{
    public HeroId Hero => new("命运2", HeroAttribute.Universal);

    public void OnActivate(HeroContext ctx)
    {
    }

    public Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
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
            SimKeyBoard.MouseMoveTo(1920, 1080);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     耗时10ms左右 延迟 原先50ms
    ///     CTRL SHIFT 键无法模拟？
    /// </summary>
    /// <param name="Key"></param>
    private static void 命运2按键(Keys Key)
    {
        SimKeyBoard.KeyDown(Key);
        Common.Delay(10); // 命运2操作延迟不然不切
        SimKeyBoard.KeyUp(Key);
    }

    private static void 命运2冰好耶()
    {
        SimKeyBoard.KeyDown(Keys.W);
        命运2按键(Keys.LShiftKey);
        Common.Delay(150);
        SimKeyBoard.KeyUp(Keys.W);
        命运2按键(Keys.LControlKey);
        Common.Delay(150);
        命运2按键(Keys.C);
        Common.Delay(100);
        SimKeyBoard.KeyDown(Keys.S);
        Common.Delay(500);
        SimKeyBoard.KeyUp(Keys.S);
    }

    private static void 命运2冰好耶1()
    {
        SimKeyBoard.KeyDown(Keys.W);
        命运2按键(Keys.LShiftKey);
        Common.Delay(150);
        SimKeyBoard.KeyUp(Keys.W);
        命运2按键(Keys.LControlKey);
        Common.Delay(150);
        SimKeyBoard.MouseLeftClick();
        Common.Delay(100);
        SimKeyBoard.KeyDown(Keys.S);
        Common.Delay(500);
        SimKeyBoard.KeyUp(Keys.S);
    }
}
#endif
