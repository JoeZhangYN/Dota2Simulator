using System.Drawing;

namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>
/// 屏幕矩形区域（1920×1080 桌面坐标系），用于 IScreenVision.FindInRegion 端口边界。
/// 取代散落的 System.Drawing.Rectangle 跨越领域/基础设施层。
/// </summary>
public readonly record struct ScreenRegion(int X, int Y, int Width, int Height)
{
    /// <summary>
    /// 从 System.Drawing.Rectangle 隐式转换。
    /// Phase 18 V3 引入——业务侧 92 Strategy 大量持 Rectangle 常量（命石区域 / buff状态技能栏 / ItemEngine.获取物品范围 等），
    /// 直接传入 _vision.Find 减少 4 字段重写。
    /// </summary>
    public static implicit operator ScreenRegion(Rectangle rect)
        => new(rect.X, rect.Y, rect.Width, rect.Height);
}
