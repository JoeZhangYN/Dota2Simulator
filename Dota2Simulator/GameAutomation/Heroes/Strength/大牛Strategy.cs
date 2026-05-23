#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 大牛Strategy : IHeroStrategy
{
    private readonly IInputExecutor _input;
#pragma warning disable IDE0052 // A4 阶段：_vision 暂未使用，A5 ConditionSlotSet 切 port 后用到
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 大牛Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }

    public HeroId Hero => new("大牛", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 回音践踏去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 灵体游魂去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 裂地沟壑去后摇;
        Main._聚合.LegSwap.配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            // 用于回收时按W
            _input.Press(VirtualKey.From(Keys.A));
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private static async Task<bool> 回音践踏去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 1, 判断成功后延时: 1300).ConfigureAwait(true);
    }

    private static async Task<bool> 灵体游魂去后摇(ImageHandle 句柄)
    {
        return await Skill.释放技能后替换图标技能后续(Keys.W, () => Main._聚合.Skills.Step(SlotKey.W), v => Main._聚合.Skills.SetStep(SlotKey.W, v)).ConfigureAwait(true);
    }

    private static async Task<bool> 裂地沟壑去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
    }
}
#endif
