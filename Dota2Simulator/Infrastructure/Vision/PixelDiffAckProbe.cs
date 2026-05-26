#if DOTA2
using System;
using System.Drawing;
using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Infrastructure.Vision;

/// <summary>
/// Phase 26 B1 (2026-05-26): 像素差实现 — 命令前后 ROI 5 点采样, 任一点 RGB 通道 diff &gt; <see cref="DiffThreshold"/> 即 Acked.
/// 设计: 不抓全 ROI 像素 (开销大), 仅 4 corner + 1 center 5 点; 任一显著变化 (例: 技能图标进 CD 变灰边框 / 物品 CD 蒙层 / 按下后 hero 受 stun 动画) 即可识别.
/// 失败语义: budget 内 0 显著变化 → 命令未生效 (灰图标 / 被控 / 命令在途未完成); 上游 Engine 据此触发 backoff / queue / refractory 等策略.
/// </summary>
public sealed class PixelDiffAckProbe : ICommandAckProbe
{
    private readonly IScreenVision _vision;

    /// <summary>RGB 单通道 diff 阈值 — 默认 30 (256 阶 ~12% 显著变化, 抗轻微 alpha blending 抖动).</summary>
    public int DiffThreshold { get; set; } = 30;

    /// <summary>budget 内每次重抓的轮询间隔 (ms). 默认 8ms ≈ 120 Hz 上限.</summary>
    public int PollIntervalMs { get; set; } = 8;

    public PixelDiffAckProbe(IScreenVision vision)
    {
        _vision = vision ?? throw new ArgumentNullException(nameof(vision));
    }

    public AckSignature Capture(ScreenRegion roi)
    {
        return _vision.WithFrame(frame =>
        {
            (ScreenPoint tl, ScreenPoint tr, ScreenPoint bl, ScreenPoint br, ScreenPoint c) = SamplePoints(roi);
            return new AckSignature(
                roi,
                frame.PixelAt(tl),
                frame.PixelAt(tr),
                frame.PixelAt(bl),
                frame.PixelAt(br),
                frame.PixelAt(c));
        });
    }

    public async Task<AckResult> WaitForAckAsync(AckSignature before, int budgetMs)
    {
        long deadline = Games.Common.获取当前时间毫秒() + budgetMs;
        while (Games.Common.获取当前时间毫秒() < deadline)
        {
            _vision.Capture(CaptureMode.FullScreen);
            AckSignature now = Capture(before.Roi);
            if (Diff(before, now))
            {
                return AckResult.Acked;
            }
            await Task.Delay(PollIntervalMs).ConfigureAwait(true);
        }
        return AckResult.Failed;
    }

    private static (ScreenPoint tl, ScreenPoint tr, ScreenPoint bl, ScreenPoint br, ScreenPoint c) SamplePoints(ScreenRegion roi)
    {
        // 内缩 1px 避免边缘抗锯齿噪声.
        int x1 = roi.X + 1, y1 = roi.Y + 1;
        int x2 = roi.X + roi.Width - 2, y2 = roi.Y + roi.Height - 2;
        int cx = roi.X + roi.Width / 2, cy = roi.Y + roi.Height / 2;
        return (
            new ScreenPoint(x1, y1),
            new ScreenPoint(x2, y1),
            new ScreenPoint(x1, y2),
            new ScreenPoint(x2, y2),
            new ScreenPoint(cx, cy));
    }

    private bool Diff(AckSignature a, AckSignature b)
        => ChannelDiff(a.TopLeft, b.TopLeft) > DiffThreshold
        || ChannelDiff(a.TopRight, b.TopRight) > DiffThreshold
        || ChannelDiff(a.BottomLeft, b.BottomLeft) > DiffThreshold
        || ChannelDiff(a.BottomRight, b.BottomRight) > DiffThreshold
        || ChannelDiff(a.Center, b.Center) > DiffThreshold;

    private static int ChannelDiff(Color a, Color b)
        => Math.Max(Math.Max(Math.Abs(a.R - b.R), Math.Abs(a.G - b.G)), Math.Abs(a.B - b.B));
}
#endif
