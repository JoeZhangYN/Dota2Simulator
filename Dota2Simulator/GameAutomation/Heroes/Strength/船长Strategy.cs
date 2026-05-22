#if DOTA2
using System;
using System.Threading;
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

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

public sealed class 船长Strategy : IHeroStrategy
{
    /// <summary>E 技能并发锁（沿用 Main._全局模式e_lock）。</summary>
    private static readonly Lock _全局模式e_lock = new();

    public HeroId Hero => new("船长", HeroAttribute.Strength);

    public void OnActivate(HeroContext ctx)
    {
        Main._聚合.Conditions[ConditionSlotKey.C1].Probe ??= 洪流接x回;
        Main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= x释放后相关逻辑;
        Main._聚合.Conditions[ConditionSlotKey.C3].Probe ??= x2次释放后;
        Main._聚合.Conditions[ConditionSlotKey.C4].Probe ??= 立即释放洪流;
    }

    public async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await Item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.Q)
        {
            Main._聚合.Conditions[ConditionSlotKey.C1].Active = true;
        }
        else if (key == VirtualKey.E)
        {
            if (Main._聚合.Skills.Step(SlotKey.E) == 1)
            {
                return;
            }
            Main._聚合.Conditions[ConditionSlotKey.C2].Active = true;
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            Main._聚合.Skills.SetStep(SlotKey.R, 1);
            SimKeyBoard.KeyPress(Keys.E);
        }
    }

    private static async Task<bool> 洪流接x回(ImageHandle 句柄)
    {
        return await Skill.主动技能释放后续(Keys.Q, () =>
        {
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
            Main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());

            // 如果E已经释放
            if (!Main._中断条件 && Main._聚合.Skills.Step(SlotKey.E) == 1)
            {
                // 1600 延迟 返回200施法时间
                Common.Delay(1350, Main._聚合.Skills.Time(SlotKey.Q));
                Main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
                SimKeyBoard.KeyPress(Keys.E);
            }
        }).ConfigureAwait(true);
    }

    private static async Task<bool> x释放后相关逻辑(ImageHandle 句柄)
    {
        // 释放x后放船，x的时间3秒，船0.3秒，3.1秒延迟，控制还是得靠水起来
        return await Skill.主动技能释放后续(Keys.E, () =>
        {
            int 步骤e = Main._聚合.Skills.Step(SlotKey.E);

            if (步骤e == 1) return;

            Main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒());

            if (Main._聚合.Skills.Step(SlotKey.R) == 1)
            {
                SimKeyBoard.KeyPress(Keys.R);
                Main._聚合.Skills.SetStep(SlotKey.R, 0);
            }

            lock (_全局模式e_lock)
            {
                Main._聚合.Skills.SetStep(SlotKey.E, 1);
                Main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
            }

            int 等待时间 = (int)Math.Floor(3000 * Main._聚合.Attack.状态抗性倍数) - 1670;
            Common.Delay(等待时间, Main._聚合.Skills.Time(SlotKey.E));
            Main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
        }).ConfigureAwait(true);
    }

    private static async Task<bool> x2次释放后(ImageHandle 句柄)
    {
        return await Skill.主动技能进入CD后续(Keys.E, () =>
        {
            lock (_全局模式e_lock)
            {
                // 玲珑心，释放完后至少再等6秒，等2秒基本完事
                // 因为释放q后，会再释放一次E
                // 等待说明E已经释放过一次，还在有效范围内
                Common.Delay(2000);
                Main._聚合.Skills.SetStep(SlotKey.E, 0);
            }
        }).ConfigureAwait(true);
    }

    private static async Task<bool> 立即释放洪流(ImageHandle 句柄)
    {
        return await Skill.主动技能已就绪后续(Keys.Q, () => { SimKeyBoard.KeyPress(Keys.Q); }).ConfigureAwait(true);
    }
}
#endif
