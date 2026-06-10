using System;
using System.Collections.Generic;
using System.Drawing;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Infrastructure.Vision;

namespace Dota2Simulator.Vision.Adapters;

/// <summary>
/// <see cref="IScreenVision"/> 的实现：把领域端口形态（Template / ScreenPoint / MatchRate）
/// 翻译为 Vision BC 现役基础设施调用——
/// 截图 / 取色经三缓冲 <see cref="GlobalScreenCapture"/>，找图经 Rust 的 findpoints.dll
/// （<see cref="ImageFinder"/>），模板名→图像句柄经懒加载 <see cref="LazyImageLoader"/>。
///
/// 业务层在 Wave 1-4 逐步从直连静态类切到本 adapter；旧静态类的多余公开面 Wave 6 收口清理。
/// </summary>
public sealed class RustVisionAdapter : IScreenVision
{
    /// <summary>按指定模式截屏到三缓冲写缓冲区。</summary>
    public void Capture(CaptureMode mode)
        => GlobalScreenCapture.CaptureScreen(mode.Region);

    /// <summary>读取当前帧指定屏幕坐标的颜色（内部按坐标偏移换算到缓冲坐标）。</summary>
    public Color PixelAt(ScreenPoint point)
        => GlobalScreenCapture.GetColor(point.X, point.Y);

    /// <summary>
    /// V1 主力路径：在指定 region 查找模板，返回首个命中（屏幕坐标）或 Miss。
    /// 容差为 0 时走精确路径；ignoreColor 走 ImageFinder 默认（255,20,147 magenta）。
    /// </summary>
    public FindResult Find(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance)
    {
        ImageHandle needleHandle = LazyImageLoader.GetImage(needle.Name);
        ImageHandle frame = GlobalScreenCapture.GetCurrentHandle();
        if (!needleHandle.IsValid || !frame.IsValid)
            return FindResult.Miss;

        Rectangle rect = new(region.X, region.Y, region.Width, region.Height);
        // FindImageInRegion 底层暂未提供 tolerance 参数；tolerance 字段保留以与全屏 Find 签名对齐，
        // 待 V5/V6 后 Vision BC 内底层 API 扩 tolerance 参数时打通。本期内 tolerance 被忽略。
        Point? hit = ImageFinder.FindImageInRegion(in needleHandle, in frame, rect, rate.Value);

        if (hit is null)
            return FindResult.Miss;

        // ImageFinder.FindImageInRegion 返回的是 region 内绝对坐标（已含偏移）。
        return FindResult.Hit(new ScreenPoint(hit.Value.X, hit.Value.Y));
    }

    /// <summary>
    /// 一帧内批量查找一组模板（同 region）。一次 <see cref="GlobalScreenCapture.GetCurrentHandle"/>
    /// 喂全组 → 修单模板循环跨帧不一致。结果与 needles 同序对齐。
    /// </summary>
    public IReadOnlyList<FindResult> FindMany(IReadOnlyList<Template> needles, ScreenRegion region, MatchRate rate, Tolerance tolerance)
    {
        if (needles is null || needles.Count == 0)
            return Array.Empty<FindResult>();

        var results = new FindResult[needles.Count];
        ImageHandle frame = GlobalScreenCapture.GetCurrentHandle();
        if (!frame.IsValid)
        {
            Array.Fill(results, FindResult.Miss);
            return results;
        }

        // 解析全部模板句柄 → 一次融合 FFI 调用（单趟 rayon 摊销 per-call 开销）。
        var handles = new ImageHandle[needles.Count];
        for (int i = 0; i < needles.Count; i++)
            handles[i] = LazyImageLoader.GetImage(needles[i].Name);

        Rectangle rect = new(region.X, region.Y, region.Width, region.Height);
        Point?[] hits = ImageFinder.FindManyInRegion(in frame, handles, rect, rate.Value);
        for (int i = 0; i < hits.Length; i++)
            results[i] = hits[i] is null
                ? FindResult.Miss
                : FindResult.Hit(new ScreenPoint(hits[i]!.Value.X, hits[i]!.Value.Y));
        return results;
    }

    /// <summary>V1 主力路径：在指定 region 查找模板的所有命中位置（屏幕坐标）。</summary>
    public IReadOnlyList<ScreenPoint> FindAll(Template needle, ScreenRegion region, MatchRate rate, Tolerance tolerance)
    {
        ImageHandle needleHandle = LazyImageLoader.GetImage(needle.Name);
        ImageHandle frame = GlobalScreenCapture.GetCurrentHandle();
        if (!needleHandle.IsValid || !frame.IsValid)
            return Array.Empty<ScreenPoint>();

        Rectangle rect = new(region.X, region.Y, region.Width, region.Height);
        List<Point> hits = ImageFinder.FindAllImagesInRegion(in needleHandle, in frame, rect, rate.Value);
        var result = new List<ScreenPoint>(hits.Count);
        foreach (Point p in hits)
            result.Add(new ScreenPoint(p.X, p.Y));
        return result;
    }

    /// <summary>Phase 25A C3: typestate frame scope — 走 GlobalScreenFrame singleton, 与 PixelAt 同一底层 (_tripleBuffer 单例同帧).</summary>
    public T WithFrame<T>(Func<IScreenFrame, T> read) => read(GlobalScreenFrame.Instance);
}
