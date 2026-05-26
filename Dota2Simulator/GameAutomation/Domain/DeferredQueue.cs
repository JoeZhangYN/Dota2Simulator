#if DOTA2
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dota2Simulator.GameAutomation.Domain;

/// <summary>
/// Phase 26 D1 (2026-05-26): 延迟命令队列子聚合 — 异步控制语义底层 §D 段.
/// 命令入队条件满足才出队执行 — 防"窗口期错失" (吹风/无敌期间命令丢失, buff 结束瞬间无法秒接跳刀; Phase 26 grill 子问题 #3).
/// 状态变化驱动 flush: <see cref="GameAutomation.Application.HeroLoopHost"/> 订阅 BuffObservable / StunObservable / ChannelObservable 等 state-change event, 每次触发调 <see cref="FlushAsync"/> 扫队列出队执行 condition 满足的.
/// TTL 过期清理: 入队超 <see cref="DeferredCommand.TtlMs"/> ms 未被 flush 直接丢弃 (默认 3000ms), 防"古早命令" 长期占队列消费 stale 状态.
/// </summary>
public sealed class DeferredQueue
{
    private readonly ConcurrentQueue<DeferredCommand> _queue = new();

    /// <summary>入队命令. 调用方需保证 condition / action 跨线程安全 (无副作用 / 幂等).</summary>
    public void Enqueue(DeferredCommand cmd) => _queue.Enqueue(cmd);

    /// <summary>当前队列深度 (诊断 / 测试用).</summary>
    public int Count => _queue.Count;

    /// <summary>清空队列 (hero 切换 / 主循环 reset 用).</summary>
    public void Clear() { while (_queue.TryDequeue(out _)) { } }

    /// <summary>
    /// 扫队列尝试出队 — condition 满足 + 未过期: 出队执行; condition 不满足 + 未过期: 留在队列; 已过期: 丢弃.
    /// 由状态变化 event handler 调用 (BuffObservable.OnStateChanged 等).
    /// </summary>
    public async Task FlushAsync(Func<DeferredCommand, bool> conditionEval)
    {
        long now = Games.Common.获取当前时间毫秒();
        List<DeferredCommand> retained = new();
        while (_queue.TryDequeue(out DeferredCommand? cmd))
        {
            if (cmd is null) continue;
            if (now - cmd.EnqueueTimeMs > cmd.TtlMs)
            {
                continue; // 过期丢弃
            }
            if (conditionEval(cmd))
            {
                await cmd.Action().ConfigureAwait(true);
            }
            else
            {
                retained.Add(cmd);
            }
        }
        foreach (DeferredCommand r in retained)
        {
            _queue.Enqueue(r);
        }
    }
}

/// <summary>
/// 延迟命令记录 — 含 condition / action / 入队时间戳 / TTL.
/// condition: 评估"是否可执行" (例: <c>ctx => !ctx.Aggregate.Buff.IsControlled</c>);
/// action: 真正执行的副作用 (例: <c>() => _item.根据图片使用物品(跳刀_Tpl)</c>);
/// EnqueueTimeMs / TtlMs: TTL 过期清理.
/// </summary>
public sealed record DeferredCommand(
    string Name,
    Func<Task> Action,
    long EnqueueTimeMs,
    int TtlMs = 3000);
#endif
