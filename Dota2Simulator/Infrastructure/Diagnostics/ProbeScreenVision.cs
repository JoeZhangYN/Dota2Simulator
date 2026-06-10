using System;
using System.Collections.Generic;
using System.Drawing;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Vision;

namespace Dota2Simulator.Diagnostics;

/// <summary>
/// <see cref="IScreenVision"/> 探针装饰器：把截图 / 找图 / 取色调用连同其查询结果录入
/// <see cref="RecordReplayProbe"/>。找图结果是回归对比的关键信号——重构若改变视觉查询路径，
/// diff 两份日志的 Find 返回值即可发现。装饰器在 4.0b Vision adapter 化后接入。
/// </summary>
public sealed class ProbeScreenVision : IScreenVision
{
    private const string Port = "Vision";
    private readonly IScreenVision _inner;

    public ProbeScreenVision(IScreenVision inner) => _inner = inner;

    public void Capture(CaptureMode mode)
    {
        if (RecordReplayProbe.Enabled) RecordReplayProbe.Record(Port, nameof(Capture), mode.ToString());
        _inner.Capture(mode);
    }

    public Color PixelAt(ScreenPoint point)
    {
        Color result = _inner.PixelAt(point);
        if (RecordReplayProbe.Enabled)
            RecordReplayProbe.Record(Port, nameof(PixelAt), $"{point} => {result}");
        return result;
    }

    public FindResult Find(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance)
    {
        FindResult result = _inner.Find(needle, region, rate, tolerance);
        if (RecordReplayProbe.Enabled)
            RecordReplayProbe.Record(Port, nameof(Find), $"{needle}, {region}, {rate} => {result}");
        return result;
    }

    public IReadOnlyList<ScreenPoint> FindAll(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance)
    {
        IReadOnlyList<ScreenPoint> result = _inner.FindAll(needle, region, rate, tolerance);
        if (RecordReplayProbe.Enabled)
            RecordReplayProbe.Record(Port, nameof(FindAll), $"{needle}, {region}, {rate} => {result.Count} 命中");
        return result;
    }

    public IReadOnlyList<FindResult> FindMany(IReadOnlyList<Template> needles, ScreenRegion region, MatchRate rate, Tolerance tolerance)
    {
        IReadOnlyList<FindResult> result = _inner.FindMany(needles, region, rate, tolerance);
        if (RecordReplayProbe.Enabled)
        {
            int found = 0;
            for (int i = 0; i < result.Count; i++)
                if (result[i].Found) found++;
            RecordReplayProbe.Record(Port, nameof(FindMany), $"{needles.Count} 模板, {region}, {rate} => {found} 命中");
        }
        return result;
    }

    /// <summary>Phase 25A C3: WithFrame 装饰 — enabled 时递归装饰 frame 供内部 PixelAt 也录 (保 record/replay 完整性), 否则直透传 0 overhead.</summary>
    public T WithFrame<T>(Func<IScreenFrame, T> read)
    {
        return _inner.WithFrame(frame =>
        {
            IScreenFrame probed = RecordReplayProbe.Enabled ? new ProbeFrame(frame) : frame;
            T result = read(probed);
            if (RecordReplayProbe.Enabled)
                RecordReplayProbe.Record(Port, nameof(WithFrame), $"=> {result}");
            return result;
        });
    }

    private sealed class ProbeFrame : IScreenFrame
    {
        private const string FramePort = "Vision.Frame";
        private readonly IScreenFrame _inner;
        public ProbeFrame(IScreenFrame inner) => _inner = inner;
        public Color PixelAt(ScreenPoint point)
        {
            Color c = _inner.PixelAt(point);
            if (RecordReplayProbe.Enabled)
                RecordReplayProbe.Record(FramePort, nameof(PixelAt), $"{point} => {c}");
            return c;
        }
    }
}
