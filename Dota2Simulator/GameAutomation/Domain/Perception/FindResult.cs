namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>
/// 找图结果。用显式 Found 标志表达「未找到」，取代魔法哨兵坐标 (245760,143640)。
/// </summary>
public readonly record struct FindResult
{
    public bool Found { get; }
    public ScreenPoint Point { get; }

    private FindResult(bool found, ScreenPoint point)
    {
        Found = found;
        Point = point;
    }

    public static FindResult Hit(ScreenPoint point) => new(true, point);

    /// <summary>未找到。</summary>
    public static readonly FindResult Miss = new(false, default);

    public bool TryGet(out ScreenPoint point)
    {
        point = Point;
        return Found;
    }
}
