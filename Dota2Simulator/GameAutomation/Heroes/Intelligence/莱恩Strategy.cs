// Phase 17: 莱恩 Strategy 迁 HeroPlan — W CustomProbe (莱恩羊接技能 + 局部函数 莱恩羊后 读 C6 接 E/A), R PreAsync (大招前纷争 物品序列) + CustomProbe (死亡一指去后摇), D2/D3 CustomProbe (推推破林肯秒羊 / 羊刺刷新秒人 Step 状态机), D4/D5 ToggleConditionSlot(C5/C6), S Execute (Session.IsPaused=true 暂停). E 键空 case 删除.
#if DOTA2
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("莱恩", HeroAttribute.Intelligence)]
public sealed partial class 莱恩Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.W).CustomProbe(async () => await 莱恩羊接技能().ConfigureAwait(true))
        .OnKey(Keys.R).PreAsync(async () => await 大招前纷争().ConfigureAwait(true))
            .CustomProbe(async () => await 死亡一指去后摇().ConfigureAwait(true))
        .OnKey(Keys.D2).CustomProbe(async () => await 推推破林肯秒羊().ConfigureAwait(true))
        .OnKey(Keys.D3).CustomProbe(async () => await 羊刺刷新秒人().ConfigureAwait(true))
        .OnKey(Keys.D4).ToggleConditionSlot(ConditionSlotKey.C5, "开启刷新秒人", "关闭刷新秒人")
        .OnKey(Keys.D5).ToggleConditionSlot(ConditionSlotKey.C6, "开启羊接吸", "开启羊接A")
        .OnKey(Keys.S).Execute(() => _main.Session!.IsPaused = true)
        .Done();

    private async Task<bool> 莱恩羊接技能()
    {
        void 莱恩羊后()
        {
            if (_main._聚合.Conditions[ConditionSlotKey.C6].Active)
            {
                Press(Keys.E);
            }
            else
            {
                走A();
            }
        }

        if (_skill.DOTA2判断技能是否CD(Keys.W))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        莱恩羊后();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 死亡一指去后摇()
    {
        if (_skill.DOTA2判断技能是否CD(Keys.R))
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }
        走A();
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 大招前纷争()
    {
        _item.批量使用物品并行(物品连招.大招前纷争);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 推推破林肯秒羊()
    {
        if (_item.根据图片使用物品(Dota2_Pictrue.物品.推推棒_Tpl) == 1)
        {
            Common.Delay(等待延迟);
            return await Task.FromResult(true).ConfigureAwait(true);
        }
        Press(Keys.W);
        return await Task.FromResult(false).ConfigureAwait(true);
    }

    private async Task<bool> 羊刺刷新秒人()
    {
        int 步骤 = _main._聚合.Skills.Step(SlotKey.Global);

        if (步骤 == 1)
        {
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.奥术鞋_Tpl) == 1) return await Task.FromResult(true).ConfigureAwait(true);
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.魂戒_Tpl) == 1) return await Task.FromResult(true).ConfigureAwait(true);
            if (_skill.DOTA2判断技能是否CD(Keys.Q))
            {
                Press(Keys.Q);
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            if (_skill.DOTA2判断技能是否CD(Keys.R))
            {
                Press(Keys.R);
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
        }
        else if (步骤 == 0)
        {
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_Tpl) == 1) return await Task.FromResult(true).ConfigureAwait(true);
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀_Tpl) == 1) return await Task.FromResult(true).ConfigureAwait(true);
            if (_skill.DOTA2判断技能是否CD(Keys.W))
            {
                Press(Keys.W);
                Common.Delay(等待延迟);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            if (_skill.DOTA2判断技能是否CD(Keys.Q))
            {
                Press(Keys.Q);
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.奥术鞋_Tpl) == 1) return await Task.FromResult(true).ConfigureAwait(true);
            if (_item.根据图片使用物品(Dota2_Pictrue.物品.魂戒_Tpl) == 1) return await Task.FromResult(true).ConfigureAwait(true);
            if (_skill.DOTA2判断技能是否CD(Keys.R))
            {
                Press(Keys.R);
                Common.Delay(60);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
            if (_main._聚合.Conditions[ConditionSlotKey.C5].Active && _item.根据图片使用物品(Dota2_Pictrue.物品.刷新球_Tpl) == 1)
            {
                _main._聚合.Skills.SetStep(SlotKey.Global, 1);
                Common.Delay(120);
                return await Task.FromResult(true).ConfigureAwait(true);
            }
        }

        return await Task.FromResult(false).ConfigureAwait(true);
    }
}
#endif
