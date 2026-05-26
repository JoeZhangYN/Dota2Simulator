#if DOTA2
using System;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>
/// Phase 26 D2 (2026-05-26): 控制状态 Observable — 检测 hero 是否被控 (stun/silence/吹风/无敌 等).
/// <para><b>当前实现 = Stub</b>: probe 永返 false (hero 永不被控). 具体 Debuff 状态栏 (435, 826, 526, 80) ROI 检测推迟 Phase 27+ —
/// 需 SME 提供具体 Debuff 图标颜色/像素特征. framework 完整, 业务 probe 实现为 0 行 stub.</para>
/// <para>消费路径: HeroLoopHost 主循环 Tick + 订阅 OnStateChanged → IsControlled=true→false 边缘 fire DeferredQueue.FlushAsync.</para>
/// </summary>
public sealed class ControlObservable : IStateChangeObservable
{
    private readonly IScreenVision _vision;
    private bool _lastState;

    public ControlObservable(IScreenVision vision)
    {
        _vision = vision ?? throw new ArgumentNullException(nameof(vision));
    }

    public event Action<bool, bool>? OnStateChanged;

    public bool CurrentState => _lastState;

    public void Tick()
    {
        bool current = ProbeIsControlled();
        if (current != _lastState)
        {
            bool before = _lastState;
            _lastState = current;
            OnStateChanged?.Invoke(before, current);
        }
    }

    /// <summary>
    /// Phase 26 D2 Stub: 具体 Debuff 检测推迟 Phase 27+. 需 SME 提供 Debuff 状态栏 (435, 826, 526, 80) 内 stun/silence/吹风/无敌 图标特征.
    /// 当前永返 false → OnStateChanged 永不触发 → DeferredQueue 入队后只能等 TTL 过期清理 (不会被 flush 出来执行).
    /// 升级路径: 此 method 内调 _vision.WithFrame(frame => /* 检测 debuff 图标 */) 即接入真实控制检测.
    /// </summary>
    private bool ProbeIsControlled()
    {
        // TODO Phase 27+: 接入真实 Debuff ROI 检测 — _vision.WithFrame + Debuff 状态栏像素特征匹配.
        _ = _vision;
        return false;
    }
}
#endif
