#if DOTA2
using System.Collections.Generic;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Domain;

/// <summary>
/// Phase 26 A1 (2026-05-26): 不应期子聚合 — 异步控制语义底层 §A 段.
/// 命名不应期 (Refractory Period): 按命令名设截止时间, 业务侧 check <see cref="IsRefractory"/> 跳过决策, 防"按键 atomic 段 + grace period" 期间其他 probe 误触 (中间态循环根因).
/// 通用 string key 形态 — 未来 SkillCD / BuffWindow / ChannelLock 等都可共享同模型.
/// 否决记录: LegSwap.Intent/Observed 双轨 — 实测 LegSwap 无 Observed 反向同步路径, 双轨语义不成立; 改用 Refractory 单向不应期.
/// </summary>
public sealed class RefractoryState
{
    private readonly Dictionary<string, long> _deadlines = new();

    /// <summary>设置命名不应期 — 自 now 起 <paramref name="durationMs"/> 内 <see cref="IsRefractory"/> 返 true.</summary>
    public void SetRefractory(string name, int durationMs)
        => _deadlines[name] = Common.获取当前时间毫秒() + durationMs;

    /// <summary>查询是否在不应期内. now &lt; deadline → true.</summary>
    public bool IsRefractory(string name)
        => _deadlines.TryGetValue(name, out long deadline) && Common.获取当前时间毫秒() < deadline;

    /// <summary>显式清除某不应期 (测试 / 手动 override 用).</summary>
    public void Clear(string name) => _deadlines.Remove(name);

    /// <summary>清空所有不应期 (hero 切换 / 主循环 reset 用).</summary>
    public void Reset() => _deadlines.Clear();
}

#endif
