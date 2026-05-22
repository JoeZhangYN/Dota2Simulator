#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

public sealed class 神域Strategy : IHeroStrategy
{
    public HeroId Hero => new("神域", HeroAttribute.Intelligence);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 命运敕令去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 涤罪之焰去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 虚妄之诺去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C5].Probe ??= 天命之雨去后摇;
    }

    public async Task OnKeyAsync(VirtualKey key, HeroContext ctx)
    {
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.D)
        {
            Main._聚合.Conditions[ConditionSlotKey.C5].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
    }

    private static async Task<bool> 命运敕令去后摇(ImageHandle 句柄)
    {
        static async Task 命运敕令后()
        {
            await Task.Run(SimKeyBoard.MouseRightClick).ConfigureAwait(true);

            // SimKeyBoard.KeyPress(Keys.A);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        命运敕令后().Start();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 涤罪之焰去后摇(ImageHandle 句柄)
    {
        static async Task 涤罪之焰后()
        {
            await Task.Run(() => { SimKeyBoard.KeyPress(Keys.A); }).ConfigureAwait(true);
            // RightClick();
        }

        if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        涤罪之焰后().Start();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 虚妄之诺去后摇(ImageHandle 句柄)
    {
        static async Task 虚妄之诺后()
        {
            await Task.Run(() => { SimKeyBoard.KeyPress(Keys.A); }).ConfigureAwait(true);
            // SimKeyBoard.KeyPress(Keys.A);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        虚妄之诺后().Start();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 天命之雨去后摇(ImageHandle 句柄)
    {
        static void 天命之雨后()
        {
            SimKeyBoard.MouseRightClick();
            // SimKeyBoard.KeyPress(Keys.A);
        }

        if (Skill.DOTA2判断技能是否CD(Keys.D, in 句柄))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        天命之雨后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
