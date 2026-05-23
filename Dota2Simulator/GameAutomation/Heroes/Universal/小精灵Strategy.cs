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

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>小精灵（全才）策略——迁移自 _main.根据当前英雄增强 的 case "小精灵"。</summary>
[HeroStrategy("小精灵", HeroAttribute.Universal)]
public sealed partial class 小精灵Strategy : IHeroStrategy
{
    private static readonly Rectangle buff状态技能栏 = new(962, 826, 526, 80);



    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 幽魂检测;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 循环续过载;
        _skill.重复按键执行间隔阈值 = 150;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.W)
        {
            if (_main._聚合.HasAghanim)
            {
                return;
            }

            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = !_main._聚合.Conditions[ConditionSlotKey.C3].Active;
            Dota2Simulator.TTS.TTS.Speak(_main._聚合.Conditions[ConditionSlotKey.C3].Active ? "开启续过载" : "关闭续过载");
        }
    }

    private static async Task<bool> 幽魂检测(ImageHandle 句柄)
    {
        return ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.小精灵_幽魂, in 句柄, buff状态技能栏)
            ? await Task.FromResult(true).ConfigureAwait(true)
            : await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 循环续过载(ImageHandle 句柄)
    {
        bool guozai = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.小精灵_过载, in 句柄, buff状态技能栏);
        if (guozai)
        {
            _main._聚合.Skills.SetStep(SlotKey.E, 3);
            return await Task.FromResult(_main._聚合.Conditions[ConditionSlotKey.C3].Active).ConfigureAwait(true);
        }
        else
        {
            await _skill.技能通用判断(Keys.E, 2).ConfigureAwait(true);
            return await Task.FromResult(_main._聚合.Conditions[ConditionSlotKey.C3].Active).ConfigureAwait(true);
        }
    }
}
#endif
