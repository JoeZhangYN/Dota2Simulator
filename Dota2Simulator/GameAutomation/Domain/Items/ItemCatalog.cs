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
    };

    /// <summary>按物品名 lookup; miss 返 null (调用方走默认: E2 保守走 DeferredQueue).</summary>
    public static ItemDescriptor? TryGet(string name)
        => _byName.TryGetValue(name, out ItemDescriptor? desc) ? desc : null;

    /// <summary>诊断 / 测试用: 已注册物品数.</summary>
    public static int Count => _byName.Count;
}
#endif
