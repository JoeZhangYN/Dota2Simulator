namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>截图缓冲区坐标（相对截图区域左上角）。与 ScreenPoint 只能经 CaptureMode 转换。</summary>
public readonly record struct BufferPoint(int X, int Y);
