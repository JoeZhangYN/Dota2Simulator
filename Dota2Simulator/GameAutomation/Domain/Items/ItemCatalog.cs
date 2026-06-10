#if DOTA2
using System.Collections.Generic;

namespace Dota2Simulator.GameAutomation.Domain.Items;

/// <summary>
/// Phase 26 E1 (2026-05-26): 全游戏物品元数据 SSOT — 异步控制语义底层 §E 段.
/// <para>初版守报 IsInsertable=false (即所有未知物品默认走 DeferredQueue, 保守); 用户冒烟通过后逐批 enable Insertable=true.</para>
/// <para>已 enable 清单 (Insertable=true, 即按下立即生效不受 stun/silence 阻断):</para>
/// <list type="bullet">
/// <item>BKB (黑皇杖) — 物理无敌大招, 引擎自 queue, stun 中可立即按入</item>
/// <item>林肯法球 — 被动魔免, 主动驱散, 引擎自 queue</item>
/// </list>
/// <para>已 enable 清单 (Insertable=false, 即必须 hero 能行动):</para>
/// <list type="bullet">
/// <item>跳刀 (闪烁) — 受 silence/stun 阻断, 在 buff 期间发会丢失</item>
/// </list>
/// <para>查询 API: <see cref="TryGet"/> 按物品名 lookup, miss 时返 null 由调用方走默认 (E2 默认走 DeferredQueue, 保守).</para>
/// </summary>
public static class ItemCatalog
{
    private static readonly Dictionary<string, ItemDescriptor> _byName = new()
    {
        // Insertable (引擎自 queue, 按下立即生效):
        ["黑皇杖"] = new("黑皇杖", IsInsertable: true),
        ["林肯法球"] = new("林肯法球", IsInsertable: true),
        // Blocked (必须 hero 能行动, 受 stun/silence 阻断):
        ["跳刀"] = new("跳刀", IsInsertable: false),
        // 消费品 (按一次永久消失) — 键用完整 Template 名 ("物品.X") 以匹配 TryGet(template.Name);
        // IsInsertable: true 保证不触发 DeferredQueue (与未注册的直发默认一致, 零行为改变),
        // IsConsumedOnUse: true 供 ConsumeOnceTracker 消费后跳过. 红杖 = Aghanim 5 视觉变体.
        ["物品.红杖"] = new("物品.红杖", IsInsertable: true, IsConsumedOnUse: true),
        ["物品.红杖2"] = new("物品.红杖2", IsInsertable: true, IsConsumedOnUse: true),
        ["物品.红杖3"] = new("物品.红杖3", IsInsertable: true, IsConsumedOnUse: true),
        ["物品.红杖4"] = new("物品.红杖4", IsInsertable: true, IsConsumedOnUse: true),
        ["物品.红杖5"] = new("物品.红杖5", IsInsertable: true, IsConsumedOnUse: true),
    };

    /// <summary>该物品名是否为消费品（消费一次后不再匹配）。miss → false（保守）。</summary>
    public static bool IsConsumedOnUse(string name)
        => _byName.TryGetValue(name, out ItemDescriptor? desc) && desc.IsConsumedOnUse;

    /// <summary>按物品名 lookup; miss 返 null (调用方走默认: E2 保守走 DeferredQueue).</summary>
    public static ItemDescriptor? TryGet(string name)
        => _byName.TryGetValue(name, out ItemDescriptor? desc) ? desc : null;

    /// <summary>诊断 / 测试用: 已注册物品数.</summary>
    public static int Count => _byName.Count;
}
#endif
