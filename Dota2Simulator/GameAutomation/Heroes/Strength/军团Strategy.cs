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

public sealed class 军团Strategy : IHeroStrategy
{

    private readonly IInputExecutor _input;
#pragma warning disable IDE0052
    private readonly IScreenVision _vision;
#pragma warning restore IDE0052

    private readonly SkillEngine _skill;
    private readonly ItemEngine _item;
    public 军团Strategy(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item)
    {
        _input = input;
        _vision = vision;
        _skill = skill;
        _item = item;
    }
    public HeroId Hero => new("军团", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 压倒性优势去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 强攻去后摇;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= 决斗;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 决斗去后摇;
        Main._聚合.Skills.SetStep(SlotKey.Global, -1);
        Main._聚合.LegSwap.配置.修改配置(Keys.E, false);
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.W)
        {
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.R)
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }
        else if (key == VirtualKey.F)
        {
            if (Main._聚合.Skills.Step(SlotKey.Global) == -1)
            {
                Main._聚合.Skills.SetStep(SlotKey.Global, 0);
                Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.ToggleMode(SlotKey.Global);
            TTS.TTS.Speak(Main._聚合.Skills.Mode(SlotKey.Global) == 1 ? "跳刀决斗" : "直接决斗");
        }
    }

    private async Task<bool> 决斗(ImageHandle 句柄)
    {
        return await Task.Run(async () =>
        {
            int 步骤 = Main._聚合.Skills.Step(SlotKey.Global);

            switch (步骤)
            {
                case < 1:
                    {
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.臂章));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.魂戒));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.相位鞋));

                        if (_skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
                        {
                            _input.ComboAlt(VirtualKey.From(Keys.W));
                            return await Task.FromResult(true).ConfigureAwait(true);
                        }

                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.刃甲));

                        Main._聚合.Skills.SetStep(SlotKey.Global, 1);
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }
                case < 2 when 步骤 == 1:
                    {
                        Common.Delay(33 *
                              (
                                  _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀)
                                  + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀)
                                  + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀)
                                  + _item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀)
                              ));

                        Main._聚合.Skills.SetStep(SlotKey.Global, 2);

                        return await Task.FromResult(true).ConfigureAwait(true);
                    }
                case < 3:
                    {
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.紫苑));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.血棘));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.否决));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.散失));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.散魂));
                        Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.深渊之刃));

                        Main._聚合.Skills.SetStep(SlotKey.Global, 3);

                        return await Task.FromResult(true).ConfigureAwait(true);
                    }

                case < 4:
                    {
                        // 触发激怒，让周围的小兵都攻击你
                        _input.Press(VirtualKey.From(Keys.A));

                        if (_skill.DOTA2释放CD就绪技能(Keys.R, in 句柄))
                        {
                            Common.Delay(60);
                            return await Task.FromResult(true).ConfigureAwait(true);
                        }

                        Main._聚合.Skills.SetStep(SlotKey.Global, -1);
                        return await Task.FromResult(false).ConfigureAwait(true);
                    }
            }

            return await Task.FromResult(false).ConfigureAwait(true);
        }).ConfigureAwait(true);
    }

    private async Task<bool> 压倒性优势去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
    }

    private async Task<bool> 强攻去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
    }

    private async Task<bool> 决斗去后摇(ImageHandle 句柄)
    {
        return await _skill.技能通用判断(Keys.R, 1, 要接的按键: Keys.Q).ConfigureAwait(true);
    }
}
#endif
