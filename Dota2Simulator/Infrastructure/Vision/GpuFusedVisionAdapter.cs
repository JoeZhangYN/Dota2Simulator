// Infrastructure/Vision/GpuFusedVisionAdapter.cs
// epic Phase 24A C1: GpuFusedVisionAdapter 骨架 + 装配开关.
// 装配开关 = csproj DefineConstants `GpuVision` → AdapterFactory.CreateVision 走本 adapter; 否则 RustVisionAdapter.
// 类型本身 #if GpuVision 包裹: 关闭开关时类型不存在, 0 编译负担.
#if GpuVision
using System;
using System.Collections.Generic;
using System.Drawing;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Vision.Adapters;

/// <summary>
/// IScreenVision 的 GPU 端实现 (epic Phase 24 全段落地, C1 仅骨架).
///
/// epic 设计目标 (C2+ 逐步落地, 详 handoff §Phase 24+ 候选):
/// - DXGI Desktop Duplication 截屏直接产 GPU texture (C3)
/// - Compute shader 模板匹配 (C2, 复用 GpuTemplateMatchProbe.GpuProbeContext 派生)
/// - 候选区域裁剪 (C2, region 参数限制 Dispatch 范围)
/// - 异步 fence 回读 (C4, latency-1 帧 trade-off)
///
/// C1 状态: 4 IScreenVision 方法 throw NotImplementedException 占位, 仅证明装配开关闭环.
/// 用户 grill 三问对齐 (2026-05-26):
///   1. 与 RustVisionAdapter 关系: 并存 + 装配开关 (默认 Rust, csproj GpuVision define 切 Gpu)
///   2. DXGI 范围: 纳入本 epic in-scope (端到端 GPU 零回传)
///   3. 候选裁剪: 复用 IScreenVision.Find(region) 参数 (P0); PreloadHints 反向 cache 留 Phase 25+
/// </summary>
public sealed class GpuFusedVisionAdapter : IScreenVision
{
    public void Capture(CaptureMode mode)
        => throw new NotImplementedException(
            "GpuFusedVisionAdapter.Capture: DXGI Desktop Duplication 接入待 epic Phase 24 C3.");

    public Color PixelAt(ScreenPoint point)
        => throw new NotImplementedException(
            "GpuFusedVisionAdapter.PixelAt: GPU PixelAt 待评估策略 " +
            "(Map staging buffer 单点开销 vs GDI fallback 双路) 待 epic Phase 24 C4.");

    public FindResult Find(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance)
        => throw new NotImplementedException(
            "GpuFusedVisionAdapter.Find: GPU compute shader 模板匹配待 epic Phase 24 C2.");

    public IReadOnlyList<ScreenPoint> FindAll(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance)
        => throw new NotImplementedException(
            "GpuFusedVisionAdapter.FindAll: GPU FindAll (HLSL 返多命中) 待 epic Phase 24 C2.");
}
#endif
