using System.Drawing;

namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>
/// 截图模式：绑定截图区域与坐标偏移，使屏幕坐标 / 缓冲坐标的转换类型安全。
/// 取代 Main.cs 散落的 截图模式1/2 const int 组。
/// </summary>
public sealed record CaptureMode(string Name, Rectangle Region, Point CoordinateOffset)
{
    /// <summary>局部高画质截图（671,727,760,346）——主循环模式1，比全屏快约 3 倍。</summary>
    public static readonly CaptureMode HighQuality =
        new("HighQuality", new Rectangle(671, 727, 760, 346), new Point(671, 727));

    /// <summary>全屏截图（0,0,1920,1080）——主循环模式2。</summary>
    public static readonly CaptureMode FullScreen =
        new("FullScreen", new Rectangle(0, 0, 1920, 1080), new Point(0, 0));

    public BufferPoint ToBuffer(ScreenPoint p) =>
        new(p.X - CoordinateOffset.X, p.Y - CoordinateOffset.Y);

    public ScreenPoint ToScreen(BufferPoint p) =>
        new(p.X + CoordinateOffset.X, p.Y + CoordinateOffset.Y);
}
