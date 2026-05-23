#if DOTA2
using System.Drawing;
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

using Dota2Simulator.GameAutomation.Domain.Perception;
namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 海民Strategy : IHeroStrategy
{
    /// <summary>命石范围（沿用 Main.命石区域）。</summary>
    private static readonly Rectangle 命石区域 = new(738, 945, 70, 26);


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 海民Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("海民", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions.StoneProbe ??= 海民获取命石;
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 冰片去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 摔角行家去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 海象神拳接雪球;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 酒友去后摇;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            switch (Main._聚合.Conditions.StoneChoice)
            {
                case 1:
                    Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
                    break;
                case 2:
                    Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
                    break;
            }
        }
        else if (key == VirtualKey.F)
        {
            if (Main._聚合.Conditions.StoneChoice == 1)
            {
                Skill.DOTA2释放CD就绪技能(Keys.E, GlobalScreenCapture.GetCurrentHandle());
            }

            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.SetTarget(SlotKey.D, Control.MousePosition);
            TTS.TTS.Speak("确定指定地点");
        }
    }

    private async Task<bool> 海民获取命石(ImageHandle 句柄)
    {
        if (Main._聚合.Conditions.StoneChoice == 0)
        {
            Main._聚合.Conditions.StoneChoice = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.命石.海民_酒友, GlobalScreenCapture.GetCurrentHandle(), 命石区域) ? 2 : 1;
        }

        Main._聚合.Conditions.StoneProbe = null;
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 冰片去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 摔角行家去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private async Task<bool> 酒友去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
    }

    // 基本完美了。。。
    private async Task<bool> 海象神拳接雪球(ImageHandle 句柄)
    {
        return await Skill.法球技能进入CD后续(Keys.R, () =>
        {
            Point p = Control.MousePosition;
            for (int i = 0; i < 2; i++)
            {
                Common.Delay(33);
                _input.MouseMoveTo(new ScreenPoint(p.X, p.Y - 60 * i));
                _input.Press(VirtualKey.From(Keys.W));
            }

            _ = Task.Run(() =>
            {
                Common.Delay(100);
                _input.MouseMoveTo(new ScreenPoint(p.X, p.Y));
                Common.Delay(850);
                if (Main._中断条件)
                {
                    return;
                }

                _input.KeyDown(VirtualKey.From(Keys.D));
                Common.Delay(100);
                Point target = Main._聚合.Skills.Target(SlotKey.D);
                _input.MouseMoveTo(new ScreenPoint(target.X, target.Y));
                Common.Delay(100);
                _input.KeyUp(VirtualKey.From(Keys.D));
                Common.Delay(600);
                _input.Press(VirtualKey.From(Keys.W));
            });
        }).ConfigureAwait(true);
    }
}
#endif
