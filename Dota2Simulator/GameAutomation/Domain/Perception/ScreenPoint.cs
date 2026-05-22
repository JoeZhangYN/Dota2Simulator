namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>屏幕绝对坐标（1920×1080 桌面坐标系）。与 BufferPoint 只能经 CaptureMode 转换。</summary>
public readonly record struct ScreenPoint(int X, int Y);
