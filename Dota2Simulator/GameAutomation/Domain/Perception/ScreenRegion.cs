namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>
/// 屏幕矩形区域（1920×1080 桌面坐标系），用于 IScreenVision.FindInRegion 端口边界。
/// 取代散落的 System.Drawing.Rectangle 跨越领域/基础设施层。
/// </summary>
public readonly record struct ScreenRegion(int X, int Y, int Width, int Height);
