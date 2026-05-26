// Phase 19G-4: 海民 Strategy 迁 HeroPlan — RegisterStoneProbe DSL + 4 CustomProbe + override OnKeyAsync 处理 E 键双路由 (StoneChoice 1→C2 / 2→C4) + F 键 PreAction.
#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Strength;

[HeroStrategy("海民", HeroAttribute.Strength)]
public sealed partial class 海民Strategy : IHeroStrategy
{
    private static readonly Rectangle 命石区域 = new(738, 945, 70, 26);

    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterCast()  // C1: 冰片 (mode 1)
        .RegisterProbe(ConditionSlotKey.C2, 摔角行家去后摇)  // C2: 摔角行家 (E StoneChoice==1 触发)
        .RegisterProbe(ConditionSlotKey.C3, 海象神拳接雪球)  // C3: 海象神拳接雪球 (F 触发)
        .RegisterProbe(ConditionSlotKey.C4, 酒友去后摇)  // C4: 酒友 (E StoneChoice==2 触发)
        .RegisterStoneProbe(海民获取命石)
        .OnKey(Keys.D2).Execute(() =>
        {
            _main._聚合.Skills.SetTarget(SlotKey.D, Control.MousePosition);
            TTS.TTS.Speak("确定指定地点");
        })
        .Done();

    public override async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        await _item.根据按键判断技能释放前通用逻辑(new KeyEventArgs((Keys)key.ToNative())).ConfigureAwait(true);

        if (key == VirtualKey.E)
        {
            // E 键双路由: StoneChoice==1 设 C2 Active / StoneChoice==2 设 C4 Active.
            switch (_main._聚合.Stone.Choice)
            {
                case 1: _main._聚合.Conditions[ConditionSlotKey.C2].Active = true; break;
                case 2: _main._聚合.Conditions[ConditionSlotKey.C4].Active = true; break;
            }
        }
        else if (key == VirtualKey.F)
        {
            // F 键 PreAction: StoneChoice==1 时触发 E 技能 + Active C3.
            if (_main._聚合.Stone.Choice == 1)
                _skill.DOTA2释放CD就绪技能(Keys.E);
            _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
        }
        else
        {
            // 其他键走 base BuildPlan dispatch (Q→C1 / D2→Execute).
            await base.OnKeyAsync(trigger, ctx).ConfigureAwait(true);
        }
    }

    private async Task<bool> 海民获取命石()
    {
        if (_main._聚合.Stone.Choice == 0)
        {
            _main._聚合.Stone.Choice = _vision.Find(Dota2_Pictrue.命石.海民_酒友_Tpl, 命石区域, new MatchRate(0.9), Tolerance.Exact).Found ? 2 : 1;
        }
        _main._聚合.Stone.Probe = null;
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 摔角行家去后摇() => await _skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
    private async Task<bool> 酒友去后摇() => await _skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);

    private async Task<bool> 海象神拳接雪球()
    {
        return await _skill.法球技能进入CD后续(Keys.R, () =>
        {
            Point p = Control.MousePosition;
            for (int i = 0; i < 2; i++)
            {
                Common.Delay(33);
                _input.MouseMoveTo(new ScreenPoint(p.X, p.Y - 60 * i));
                _input.Press(VirtualKey.From(Keys.W));
            }
            _ = Task.Run(() =>
            {
                Common.Delay(100);
                _input.MouseMoveTo(new ScreenPoint(p.X, p.Y));
                Common.Delay(850);
                if (_main.Session!.IsPaused) return;
                _input.KeyDown(VirtualKey.From(Keys.D));
                Common.Delay(100);
                Point target = _main._聚合.Skills.Target(SlotKey.D);
                _input.MouseMoveTo(new ScreenPoint(target.X, target.Y));
                Common.Delay(100);
                _input.KeyUp(VirtualKey.From(Keys.D));
                Common.Delay(600);
                _input.Press(VirtualKey.From(Keys.W));
            });
        }).ConfigureAwait(true);
    }
}
#endif
