#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 伐木机Strategy : IHeroStrategy
{
    /// <summary>命石范围 6技能最左738 4技能最右807 单个25*25大小（沿用 Main.命石区域）。</summary>
    private static readonly Rectangle 命石区域 = new(738, 945, 70, 26);


    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    public 伐木机Strategy(IInputExecutor input, IScreenVision vision)
    {
        _input = input;
        _vision = vision;
    }
    public HeroId Hero => new("伐木机", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions.StoneProbe ??= 伐木机获取命石;
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 死亡旋风去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 伐木聚链去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 锯齿轮旋去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 喷火装置去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 锯齿飞轮去后摇;
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
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            if (Main._聚合.Conditions.StoneChoice == 2)
            {
                Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
        }
        else if (key == VirtualKey.F)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
    }

    private static async Task<bool> 伐木机获取命石(ImageHandle 句柄)
    {
        if (Main._聚合.Conditions.StoneChoice == 0)
        {
            if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.命石.伐木机_碎木击, GlobalScreenCapture.GetCurrentHandle(), 命石区域))
            {
                Main._聚合.Conditions.StoneChoice = 1;
            }
            else if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.命石.伐木机_锯齿轮旋, GlobalScreenCapture.GetCurrentHandle(), 命石区域))
            {
                Main._聚合.Conditions.StoneChoice = 2;
                Main._聚合.LegSwap.配置.修改配置(Keys.D, true);
            }
        }

        Main._聚合.Conditions.StoneProbe = null;
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 死亡旋风去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private static async Task<bool> 伐木聚链去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 锯齿轮旋去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
    }

    private static async Task<bool> 锯齿飞轮去后摇(ImageHandle 句柄)
    {
        return await Skill.释放技能后替换图标技能后续(Keys.R, () => Main._聚合.Skills.Step(Domain.Loop.SlotKey.R), v => Main._聚合.Skills.SetStep(Domain.Loop.SlotKey.R, v)).ConfigureAwait(true);
    }

    private static async Task<bool> 喷火装置去后摇(ImageHandle 句柄)
    {
        return await Skill.技能通用判断(Keys.F, 0).ConfigureAwait(true);
    }
}
#endif
