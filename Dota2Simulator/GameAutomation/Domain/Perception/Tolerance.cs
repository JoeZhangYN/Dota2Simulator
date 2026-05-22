namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>颜色匹配容差（每通道允许偏差）。命名值对象，消除「容差 / 误差范围」同义混淆。</summary>
public readonly record struct Tolerance(byte Value)
{
    /// <summary>精确匹配，零容差。</summary>
    public static readonly Tolerance Exact = new(0);
}
