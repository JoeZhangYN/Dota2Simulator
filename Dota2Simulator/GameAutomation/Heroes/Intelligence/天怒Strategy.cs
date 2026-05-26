// Phase 19G-3: 天怒 Strategy 迁 HeroPlan — Q/W/E/R 4 simple AfterEnterCD + D2 ToggleSlot(Q 循环鹰隼 mode 2) + D3 Execute (SetStep + Active 天怒秒人连招 C2 Step 状态机).
// 业务侧槽位映射: Q→C3 / W→C6 / E→C4 / R→C5 (Probe 顺序 C1=循环 C2=秒人 C3=Q C4=E C5=R C6=W); D2 toggle C1 + D3 设 C2 Active.
// HeroPlan clause 顺序累加 C1..C6 必须严格匹配业务 OnActivate Probe slot. 用 ToggleSlot 占 C1 (循环奥数鹰隼) + CustomProbe 占 C2 (天怒秒人连招).
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

namespace Dota2Simulator.GameAutomation.Heroes.Intelligence;

[HeroStrategy("天怒", HeroAttribute.Intelligence)]
public sealed partial class 天怒Strategy : IHeroStrategy
{
    private HeroPlan? _plan;
    private HeroPlan GetPlan() => _plan ??= HeroPlanBuilder.New()
        .OnKey(Keys.D2).ToggleSlot(Keys.Q, "循环鹰隼", "不循环鹰隼")  // C1: toggle 循环奥数鹰隼 (Probe 自循环 mode 2 of Q)
        .OnKey(Keys.D3).Pre(() => _main._聚合.Skills.SetStep(SlotKey.Global, 1)).CustomProbe(天怒秒人连招)  // C2: 秒人连招 Step 状态机
        .OnKey(Keys.Q).CastSkill(Keys.Q).AfterEnterCD()  // C3: 奥数鹰隼
        .OnKey(Keys.E).CastSkill(Keys.E).AfterEnterCD()  // C4: 上古封印
        .OnKey(Keys.R).CastSkill(Keys.R).AfterCast()  // C5: 神秘之耀
        .OnKey(Keys.W).CastSkill(Keys.W).AfterEnterCD()  // C6: 震荡光弹
        .RepeatThreshold(100)
        .Done();

    protected override HeroPlan BuildPlan() => GetPlan();

    private async Task<bool> 天怒秒人连招()
    {
        int 步骤 = _main._聚合.Skills.Step(SlotKey.Global);
        switch (步骤)
        {
            case < 2:
                if (_skill.DOTA2释放CD就绪技能(Keys.W)) return await Task.FromResult(true).ConfigureAwait(true);
                if (_skill.DOTA2释放CD就绪技能(Keys.E)) return await Task.FromResult(true).ConfigureAwait(true);
                if (_skill.DOTA2释放CD就绪技能(Keys.Q)) return await Task.FromResult(true).ConfigureAwait(true);
                Common.Delay(0 * _item.根据图片使用物品(Dota2_Pictrue.物品.阿托斯之棍_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.缚灵锁_Tpl));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃_Tpl));
                Common.Delay(33 * (
                    _item.根据图片使用物品(Dota2_Pictrue.物品.红杖_Tpl)
                    + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖2_Tpl)
                    + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖3_Tpl)
                    + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖4_Tpl)
                    + _item.根据图片使用物品(Dota2_Pictrue.物品.红杖5_Tpl)));
                Common.Delay(33 * _item.根据图片使用物品(Dota2_Pictrue.物品.羊刀_Tpl));
                _main._聚合.Skills.SetStep(SlotKey.Global, 2);
                return await Task.FromResult(true).ConfigureAwait(true);
            case < 3:
                if (_skill.DOTA2释放CD就绪技能(Keys.R)) return await Task.FromResult(true).ConfigureAwait(true);
                _main._聚合.Conditions[ConditionSlotKey.C1].Active = true;  // 跨 clause 副作用: 启动循环 C1
                _main._聚合.Skills.SetStep(SlotKey.Global, 3);
                return await Task.FromResult(false).ConfigureAwait(true);
        }
        return await Task.FromResult(true).ConfigureAwait(true);
    }
}
#endif
