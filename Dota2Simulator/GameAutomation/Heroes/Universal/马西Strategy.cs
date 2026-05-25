// Phase 12 Chunk 3c (DSL 扩展验证): 马西 Strategy 迁 HeroPlan + WhenHasAghanim guard.
// 原: W 键 + 若 HasAghanim 直接 return (不激活 C2) → Plan 等价: OnKey(W) (无 Guard, 激活 C2)? 否!
// 实际原逻辑: HasAghanim → 不激活; !HasAghanim → 激活. 即 WhenNotHasAghanim. 当前 builder 只支持 Has 不支持 Not.
// 折中: 此英雄不迁 (W 键的 guard 是反向 !HasAghanim, 扩 WhenNotHasAghanim 仅为 1 个英雄不值得).
// **保留原写法** — 仅为示例错误 fit 模式 (Has guard 不等于 Not guard).
#if DOTA2
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.Vision;

using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>马西（全才）策略——迁移自 _main.根据当前英雄增强 的 case "马西"。Phase 12 C3c 评估为不 fit Plan (反向 !HasAghanim guard), 保留原写法.</summary>
[HeroStrategy("马西", HeroAttribute.Universal)]
public sealed partial class 马西Strategy : IHeroStrategy
{
    private static readonly Rectangle buff状态技能栏 = new(962, 826, 526, 80);



    public void OnActivate(HeroContext ctx)
    {
        _main._聚合.Conditions[ConditionSlotKey.C2].Probe ??= 幽魂检测;
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
    }

#pragma warning disable CS0618 // V3 临时妥协调用 Find(ImageHandle, ...) 重载，V6 改 SG 生成 Template 同步删
    private async Task<bool> 幽魂检测()
    {
        return _vision.Find(Dota2_Pictrue.Buff.小精灵_幽魂, buff状态技能栏, new MatchRate(0.9), Tolerance.Exact).Found
            ? await Task.FromResult(true).ConfigureAwait(true)
            : await Task.FromResult(false).ConfigureAwait(true);
    }
#pragma warning restore CS0618
}
#endif
