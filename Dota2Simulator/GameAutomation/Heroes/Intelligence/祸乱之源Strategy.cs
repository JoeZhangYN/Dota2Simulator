#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("祸乱之源", HeroAttribute.Intelligence)]
public sealed partial class 祸乱之源Strategy : IHeroStrategy
{


    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 虚弱去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 噬脑去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 噩梦接平A锤;
    }

    public Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.Q)
        {
          _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
          _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.E)
        {
          Color 技能点颜色 = Color.FromArgb(203, 183, 124);
          _main._聚合.Skills.SetTime(SlotKey.Global, 0);
          if (ColorExtensions.ColorAEqualColorB(_main.获取指定位置颜色(971, 1008, GlobalScreenCapture.GetCurrentHandle()), 技能点颜色, 0))
            {
              _main._聚合.Skills.SetTime(SlotKey.Global, 7000);
            }
          else if (ColorExtensions.ColorAEqualColorB(_main.获取指定位置颜色(964, 1008, GlobalScreenCapture.GetCurrentHandle()), 技能点颜色, 0))
            {
              _main._聚合.Skills.SetTime(SlotKey.Global, 6000);
            }
          else if (ColorExtensions.ColorAEqualColorB(_main.获取指定位置颜色(947, 1008, GlobalScreenCapture.GetCurrentHandle()), 技能点颜色, 0))
            {
              _main._聚合.Skills.SetTime(SlotKey.Global, 5000);
            }
          else if (ColorExtensions.ColorAEqualColorB(_main.获取指定位置颜色(935, 1008, GlobalScreenCapture.GetCurrentHandle()), 技能点颜色, 0))
            {
              _main._聚合.Skills.SetTime(SlotKey.Global, 4000);
            }

          _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
          _main._聚合.Skills.ToggleMode(SlotKey.E);
          TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.E) == 0 ? "睡不接陨星锤" : "睡接陨星锤");
        }

        return Task.CompletedTask;
    }

    private async Task<bool> 虚弱去后摇(ImageHandle 句柄)
    {
        void 虚弱后()
        {
          _skill.通用技能后续动作(false);
        }

        if (_skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
        {
          return await Task.FromResult(true).ConfigureAwait(true);
        }

        虚弱后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 噬脑去后摇(ImageHandle 句柄)
    {
        void 噬脑后()
        {
          _skill.通用技能后续动作();
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
        {
          return await Task.FromResult(true).ConfigureAwait(true);
        }

        噬脑后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private static async Task<bool> 噩梦接平A锤(ImageHandle 句柄)
    {
        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
