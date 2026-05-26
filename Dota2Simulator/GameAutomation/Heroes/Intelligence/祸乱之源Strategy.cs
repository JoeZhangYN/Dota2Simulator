// Phase 19D: 祸乱之源 Strategy 迁 HeroPlan — Q/W CustomProbe (虚弱/噬脑 同质 CD 检查 + 通用后续动作) + E Execute (噩梦颜色检测 4 档 SetTime + Active C3) + D2 Execute (ToggleMode E + TTS) + RegisterProbe(C3 dead noProbe-like).
#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("祸乱之源", HeroAttribute.Intelligence)]
public sealed partial class 祸乱之源Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CustomProbe(虚弱去后摇)
        .OnKey(Keys.W).CustomProbe(噬脑去后摇)
        .OnKey(Keys.E).Execute(() =>
        {
            // 噩梦键: 颜色检测 4 档 → SetTime(Global) → Active C3
            ImageHandle 句柄 = GlobalScreenCapture.GetCurrentHandle();
            Color 技能点颜色 = Color.FromArgb(203, 183, 124);
            _main._聚合.Skills.SetTime(SlotKey.Global, 0);
            if (ColorExtensions.ColorAEqualColorB(ImageManager.GetColor(in 句柄, 971, 1008), 技能点颜色, 0))
                _main._聚合.Skills.SetTime(SlotKey.Global, 7000);
            else if (ColorExtensions.ColorAEqualColorB(ImageManager.GetColor(in 句柄, 964, 1008), 技能点颜色, 0))
                _main._聚合.Skills.SetTime(SlotKey.Global, 6000);
            else if (ColorExtensions.ColorAEqualColorB(ImageManager.GetColor(in 句柄, 947, 1008), 技能点颜色, 0))
                _main._聚合.Skills.SetTime(SlotKey.Global, 5000);
            else if (ColorExtensions.ColorAEqualColorB(ImageManager.GetColor(in 句柄, 935, 1008), 技能点颜色, 0))
                _main._聚合.Skills.SetTime(SlotKey.Global, 4000);
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        })
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.ToggleMode(SlotKey.E);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.E) == 0 ? "睡不接陨星锤" : "睡接陨星锤");
        })
        .RegisterProbe(ConditionSlotKey.C3, 噩梦接平A锤)
        .Done();

    private async Task<bool> 虚弱去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.Q))
            return await Task.FromResult(true).ConfigureAwait(true);
        _skill.通用技能后续动作(false);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 噬脑去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.W))
            return await Task.FromResult(true).ConfigureAwait(true);
        _skill.通用技能后续动作();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 噩梦接平A锤()
    {
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
