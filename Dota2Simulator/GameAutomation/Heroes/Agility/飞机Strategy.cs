#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Agility;

/// <summary>飞机（敏捷）策略——迁移自 Main.根据当前英雄增强 的 case "飞机"。</summary>
public sealed class 飞机Strategy : IHeroStrategy
{
    public HeroId Hero => new("飞机", HeroAttribute.Agility);

    public void OnActivate(HeroContext ctx)
    {
        //_聚合.Conditions[ConditionSlotKey.C1].Probe ??= 火箭弹幕敏捷;
        //_聚合.Conditions[ConditionSlotKey.C2].Probe ??= 追踪导弹敏捷;
        //_聚合.Conditions[ConditionSlotKey.C3].Probe ??= 高射火炮敏捷;
        //_聚合.Conditions[ConditionSlotKey.C4].Probe ??= 召唤飞弹敏捷;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 循环火箭弹幕;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        //if (key == VirtualKey.Q)
        //{
        //    Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        //}
        //else if (key == VirtualKey.W)
        //{
        //    Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        //}
        //else if (key == VirtualKey.E)
        //{
        //    Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        //}
        //else if (key == VirtualKey.R)
        //{
        //    Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        //}
        if (key == VirtualKey.From(Keys.D3))
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = !Main._聚合.Conditions[ConditionSlotKey.C5].Active;
            Dota2Simulator.TTS.TTS.Speak(Main._聚合.Conditions[ConditionSlotKey.C5].Active ? "循环弹幕" : "关闭弹幕");
        }
    }

    private static async Task<bool> 火箭弹幕敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 追踪导弹敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 高射火炮敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 召唤飞弹敏捷(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 循环火箭弹幕(ImageHandle 句柄)
    {
        if (Common.获取当前时间毫秒() - Main._聚合.Skills.Time(SlotKey.Q) > 400)
            await Skill.主动技能已就绪后续(Keys.Q, () =>
            {
                SimKeyBoard.KeyPress(Keys.Q);
                Main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());
            }).ConfigureAwait(true);

        return await Task.FromResult(Main._聚合.Conditions[ConditionSlotKey.C5].Active);
    }
}
#endif
