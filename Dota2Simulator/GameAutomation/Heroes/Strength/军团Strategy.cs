#if DOTA2
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

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("军团", HeroAttribute.Strength)]
public sealed partial class 军团Strategy : IHeroStrategy
{


    public override void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 压倒性优势去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 强攻去后摇;
        _main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 决斗;
        _main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 决斗去后摇;
        _main._聚合.Skills.SetStep(SlotKey.Global, -1);
        _main._聚合.LegSwap.配置.修改配置(Keys.E, false);
    }

    public override async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            _main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.F)
        {
            if (_main._聚合.Skills.Step(SlotKey.Global) == -1)
            {
                _main._聚合.Skills.SetStep(SlotKey.Global, 0);
                _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _main._聚合.Skills.ToggleMode(SlotKey.Global);
            TTS.TTS.Speak(_main._聚合.Skills.Mode(SlotKey.Global) == 1 ? "跳刀决斗" : "直接决斗");
        }
    }

    private async Task<bool> 决斗()
    {
        return await Task.Run(async () =>
        {
            int 步骤 = _main._聚合.Skills.Step(SlotKey.Global);

            switch (步骤)
            {
                case < 1:
                    {
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.臂章_Tpl));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.魂戒_Tpl));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.相位鞋_Tpl));

                        if (_skill.DOTA2判断技能是否CD(Keys.W))
                        {
                            _input.ComboAlt(VirtualKey.From(Keys.W));
                            return await Task.FromResult(true).ConfigureAwait(true);
                        }

                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.刃甲_Tpl));

                        _main._聚合.Skills.SetStep(SlotKey.Global, 1);
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }
                case < 2 when 步骤 == 1:
                    {
                        Common.Delay(33 *
                              (
                                  _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_Tpl)
                                  + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀_Tpl)
                                  + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀_Tpl)
                                  + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀_Tpl)
                              ));

                        _main._聚合.Skills.SetStep(SlotKey.Global, 2);

                        return await Task.FromResult(true).ConfigureAwait(true);
                    }
                case < 3:
                    {
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.紫苑_Tpl));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.血棘_Tpl));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.否决_Tpl));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.散失_Tpl));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.散魂_Tpl));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.深渊之刃_Tpl));

                        _main._聚合.Skills.SetStep(SlotKey.Global, 3);

                        return await Task.FromResult(true).ConfigureAwait(true);
                    }

                case < 4:
                    {
                        // 触发激怒，让周围的小兵都攻击你
                        _input.Press(VirtualKey.From(Keys.A));

                        if (_skill.DOTA2释放CD就绪技能(Keys.R))
                        {
                            Common.Delay(60);
                            return await Task.FromResult(true).ConfigureAwait(true);
                        }

                        _main._聚合.Skills.SetStep(SlotKey.Global, -1);
                        return await Task.FromResult(false).ConfigureAwait(true);
                    }
            }

            return await Task.FromResult(false).ConfigureAwait(true);
        }).ConfigureAwait(true);
    }

    private async Task<bool> 压倒性优势去后摇()
    {
        return await _skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private async Task<bool> 强攻去后摇()
    {
        return await _skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private async Task<bool> 决斗去后摇()
    {
        return await _skill.技能通用判断(Keys.R, 1, 要接的按键: Keys.Q).ConfigureAwait(true);
    }
}
#endif
