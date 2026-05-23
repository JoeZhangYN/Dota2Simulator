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

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 龙骑Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 龙骑Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("龙骑", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 喷火去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 神龙摆尾去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 变龙去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 火球去后摇;
        Main._聚合.LegSwap.配置.修改配置(Keys.E, false);
        Main._聚合.Attack.基础攻击前摇 = 0.5;
        Main._聚合.Attack.基础攻击间隔 = 1.6;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.From(Keys.F1))
        {
            if (Main._聚合.HasShard)
            {
                Main._聚合.LegSwap.配置.修改配置(Keys.D, true);
            }
        }
        else if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            if (Main._聚合.HasShard)
            {
                Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
            }
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.W);
            TTS.TTS.Speak("W接" + (Main._聚合.Skills.Mode(SlotKey.W) == 1 ? "火球" : "喷火"));
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.D);
            TTS.TTS.Speak("火球" + (Main._聚合.Skills.Mode(SlotKey.D) == 1 ? "接" : "不接") + "喷火");
        }
    }

    private async Task<bool> 喷火去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private async Task<bool> 神龙摆尾去后摇(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.W, () =>
        {
            _input.Press(VirtualKey.From(Keys.A));
            _ = Main._聚合.Skills.Mode(SlotKey.W) == 1 && Main._聚合.HasShard ? Skill.DOTA2释放CD就绪技能(Keys.D, in 句柄) : Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄);

            Item.要求保持假腿();
        }).ConfigureAwait(true);
    }

    private async Task<bool> 变龙去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
    }

    private async Task<bool> 火球去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 0, 要接的按键: Main._聚合.Skills.Mode(SlotKey.D) == 1 && Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄) ? Keys.Q : Keys.A)
            .ConfigureAwait(true);
    }
}
#endif
