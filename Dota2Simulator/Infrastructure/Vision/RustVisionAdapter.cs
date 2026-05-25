using System;
using System.Collections.Generic;
using System.Drawing;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Ports;

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

    /// <summary>
    /// 在当前帧中查找模板，返回首个命中（屏幕坐标）或 <see cref="FindResult.Miss"/>。
    /// 容差为 0 时走精确匹配路径，与旧代码 <c>ImageFinder.FindImage</c> 逐字节一致。
    /// </summary>
    public FindResult Find(Template needle, MatchRate rate, Tolerance tolerance)
    {
        ImageHandle needleHandle = LazyImageLoader.GetImage(needle.Name);
        ImageHandle frame = GlobalScreenCapture.GetCurrentHandle();
        if (!needleHandle.IsValid || !frame.IsValid)
            return FindResult.Miss;

        Point? hit = tolerance.Value == 0
            ? ImageFinder.FindImage(in needleHandle, in frame, rate.Value)
            : ImageFinder.FindImageWithTolerance(in needleHandle, in frame, tolerance.Value, rate.Value);

        if (hit is null)
            return FindResult.Miss;

        // ImageFinder 返回缓冲坐标；加坐标偏移换算为屏幕坐标（与 GlobalScreenCapture.FindImage 一致）。
        (int offsetX, int offsetY) = GlobalScreenCapture.CoordinateOffset;
        return FindResult.Hit(new ScreenPoint(hit.Value.X + offsetX, hit.Value.Y + offsetY));
    }

    /// <summary>
    /// 在当前帧中查找模板的所有命中位置（屏幕坐标）。
    /// 底层 <see cref="GlobalScreenCapture.FindAllImages"/> 不支持颜色容差，<paramref name="tolerance"/> 暂被忽略。
    /// </summary>
    public IReadOnlyList<ScreenPoint> FindAll(Template needle, MatchRate rate, Tolerance tolerance)
    {
        ImageHandle needleHandle = LazyImageLoader.GetImage(needle.Name);
        if (!needleHandle.IsValid)
            return Array.Empty<ScreenPoint>();

        // FindAllImages 内部已加坐标偏移，返回的是屏幕坐标。
        List<Point> hits = GlobalScreenCapture.FindAllImages(in needleHandle, rate.Value);
        var result = new List<ScreenPoint>(hits.Count);
        foreach (Point p in hits)
            result.Add(new ScreenPoint(p.X, p.Y));
        return result;
    }

    /// <summary>读取当前帧指定屏幕坐标的颜色（内部按坐标偏移换算到缓冲坐标）。</summary>
    public Color PixelAt(ScreenPoint point)
        => GlobalScreenCapture.GetColor(point.X, point.Y);

    /// <summary>区域内查找模板（屏幕坐标，1920×1080）。封装 ImageFinder.FindImageInRegionBool。</summary>
#pragma warning disable CS0618 // 实现仍允许调本接口已废弃方法
    public bool FindInRegion(Template needle, ScreenRegion region, MatchRate rate)
    {
        ImageHandle needleHandle = LazyImageLoader.GetImage(needle.Name);
        ImageHandle frame = GlobalScreenCapture.GetCurrentHandle();
        if (!needleHandle.IsValid || !frame.IsValid)
            return false;

        Rectangle rect = new(region.X, region.Y, region.Width, region.Height);
        return ImageFinder.FindImageInRegionBool(in needleHandle, in frame, rect, rate.Value);
    }
#pragma warning restore CS0618

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

    /// <summary>
    /// V3 临时妥协路径：业务直接传 ImageHandle，跳过 Template→ImageHandle 反查。V6 删。
    /// </summary>
#pragma warning disable CS0618 // 实现仍允许调本接口已废弃方法
    public FindResult Find(ImageHandle needle, ScreenRegion region, MatchRate rate, Tolerance tolerance)
    {
        ImageHandle frame = GlobalScreenCapture.GetCurrentHandle();
        if (!needle.IsValid || !frame.IsValid)
            return FindResult.Miss;

        Rectangle rect = new(region.X, region.Y, region.Width, region.Height);
        Point? hit = ImageFinder.FindImageInRegion(in needle, in frame, rect, rate.Value);
        if (hit is null)
            return FindResult.Miss;
        return FindResult.Hit(new ScreenPoint(hit.Value.X, hit.Value.Y));
    }
#pragma warning restore CS0618

    /// <summary>
    /// 获取三缓冲读缓冲区当前帧句柄，供 ConditionDelegateBitmap 委托链路使用。
    /// Phase 6 临时方法 — Phase 7+ 改委托签名后移除。
    /// </summary>
#pragma warning disable CS0618 // 实现仍允许调本接口已废弃方法
    public ImageHandle GetCurrentFrame() => GlobalScreenCapture.GetCurrentHandle();
#pragma warning restore CS0618
}
