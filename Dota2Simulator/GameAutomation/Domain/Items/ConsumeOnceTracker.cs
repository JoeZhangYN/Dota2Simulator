#if DOTA2
using System.Collections.Generic;

namespace Dota2Simulator.GameAutomation.Domain.Items;

/// <summary>
/// 跨帧「消费一次」状态机：跟踪消费品（如红杖/Aghanim：按一次永久消失）的
/// 可使用 → 消失 转移，消费后标记 Consumed，调用方据此跳过、不再匹配。
///
/// 关键判据（区别于 CD 物品）：物品**曾以可使用形态出现**，之后变为 <see cref="ItemDiscreteState.Absent"/>
/// **且不在 CD**（用末槽 CD 信号排除「只是进 CD 的灰图标」）→ 判为已消费。
/// CD 物品永远 OnCooldown 或仍 present，不会被误标。
///
/// 纯逻辑、无 IO；状态归属调用方（ItemEngine 单例 = 每局），<see cref="Reset"/> 在 F1 重置时调。
/// </summary>
public sealed class ConsumeOnceTracker
{
    private readonly HashSet<string> _seenPresent = new();
    private readonly HashSet<string> _consumed = new();

    /// <summary>该物品是否已消费（调用方据此跳过搜索/使用）。</summary>
    public bool IsConsumed(string itemName) => _consumed.Contains(itemName);

    /// <summary>
    /// 喂入本帧解析出的物品状态，驱动消费一次转移。
    /// 已消费 → 幂等忽略；在位（任意离散态）→ 记忆「曾在位」；曾在位且现不在且**非 CD** → 标记消费。
    /// CD 物品按下后变 OnCooldown（仍 present 或灰图标 + CD 信号），被 `!OnCooldown` 守住不误标。
    /// </summary>
    public void Observe(string itemName, ItemSlotState state)
    {
        if (_consumed.Contains(itemName))
            return;

        if (state.IsPresent)
        {
            _seenPresent.Add(itemName);
            return;
        }

        // 此处 Discrete == Absent。曾在位 + 现不在 + 非 CD → 真消费（区别于只是进 CD 的灰图标）。
        if (_seenPresent.Contains(itemName) && !state.Cooldown.OnCooldown)
        {
            _consumed.Add(itemName);
        }
    }

    /// <summary>新一局重置：清空已见/已消费（F1 重置 / 换局调用）。</summary>
    public void Reset()
    {
        _seenPresent.Clear();
        _consumed.Clear();
    }
}
#endif
