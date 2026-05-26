// Phase 19G-3: 命运2 Strategy 迁 HeroPlan — Q Execute (鼠标移到右下角) + V Execute (命运2冰好耶 多步骤宏) + B Execute (命运2冰好耶1 多步骤宏 含 MouseClick.Left).
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("命运2", HeroAttribute.Universal)]
public sealed partial class 命运2Strategy : IHeroStrategy
{
    private HeroPlan? _plan;
    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.V).Execute(() => Task.Run(命运2冰好耶))
        .OnKey(Keys.B).Execute(() => Task.Run(命运2冰好耶1))
        .OnKey(Keys.Q).Execute(() => _input.MouseMoveTo(new ScreenPoint(1920, 1080)))
        .Done();

    protected override HeroPlan BuildPlan() => GetPlan();

    private void 命运2按键(Keys Key)
    {
        _input.KeyDown(VirtualKey.From(Key));
        Common.Delay(10);
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
