#if DOTA2
using System.Drawing;

namespace Dota2Simulator.GameAutomation.Domain;

/// <summary>
/// Dota2 BC 1920x1080 屏幕坐标常量 SSOT。
/// Phase 9 B：从 Games.Dota2.Main 静态字段抽出，去可变状态 static God class 化。
/// 坐标偏移 = 截图模式相关偏移（历史可变 int，全仓 0 处赋值，本质常量 0 — 详 plan idempotent-brewing-kurzweil §关键摸底）。
/// 截图模式 1 = 671×727 局部 760×346（技能 + 状态 + 物品）；模式 2 = 全屏 1920×1080。
/// </summary>
internal static class GameLayout
{
    public const int OffsetX = 0;
    public const int OffsetY = 0;

    public const int 截图模式1X = 671;
    public const int 截图模式1Y = 727;
    public const int 截图模式1W = 760;
    public const int 截图模式1H = 346;
    public static readonly Rectangle 截图模式1Reg = new(截图模式1X, 截图模式1Y, 截图模式1W, 截图模式1H);

    public const int 截图模式2X = 0;
    public const int 截图模式2Y = 0;
    public const int 截图模式2W = 1920;
    public const int 截图模式2H = 1080;
    public static readonly Rectangle 截图模式2Reg = new(截图模式2X, 截图模式2Y, 截图模式2W, 截图模式2H);
}
#endif
