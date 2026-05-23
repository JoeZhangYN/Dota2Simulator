#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>赏金（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "赏金"。</summary>
public sealed class 赏金Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 赏金Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("赏金", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 飞镖接平a;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 标记去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 循环标记;
        Item._切假腿配置.修改配置(Keys.W, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = !Main._聚合.Conditions[ConditionSlotKey.C3].Active;
            Dota2Simulator.TTS.TTS.Speak(Main._聚合.Conditions[ConditionSlotKey.C3].Active ? "循环标记" : "不循环标记");
        }
    }

    private static async Task<bool> 飞镖接平a(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 标记去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 循环标记(ImageHandle 句柄)
    {
        await Skill.技能通用判断(Keys.R, 2).ConfigureAwait(true);
        return await Task.FromResult(Main._聚合.Conditions[ConditionSlotKey.C3].Active).ConfigureAwait(true);
    }
}
#endif
