#if DOTA2
using System;

namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>
/// Phase 26 D2 (2026-05-26): 状态变化 Observable 抽象 — 异步控制语义底层 §D 段.
/// 主循环每帧调 <see cref="Tick"/>: 内部 probe 当前状态 vs 上一帧 — 变化即 fire <see cref="OnStateChanged"/> event.
/// 消费者 (HeroLoopHost) 订阅 event → 触发 DeferredQueue.FlushAsync (吹风秒接跳刀 等延迟命令释放).
/// </summary>
public interface IStateChangeObservable
{
    /// <summary>状态变化事件 — 参数: (before, after) 两个布尔状态值 (true=激活).</summary>
    event Action<bool, bool> OnStateChanged;

    /// <summary>主循环每帧调用 — 内部 probe + 与上一帧比对 + 变化时 fire event.</summary>
    void Tick();

    /// <summary>当前 cached 状态 (上次 Tick 后的); 业务 lambda 可用作 DeferredCommand condition.</summary>
    bool CurrentState { get; }
}
#endif
