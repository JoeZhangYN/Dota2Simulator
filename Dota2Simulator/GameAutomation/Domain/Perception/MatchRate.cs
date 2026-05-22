using System;

namespace Dota2Simulator.GameAutomation.Domain.Perception;

/// <summary>图片匹配率，构造时强制落在 0~1 区间。取代散落的裸 double 0.9/0.95。</summary>
public readonly record struct MatchRate
{
    public double Value { get; }

    public MatchRate(double value)
    {
        if (value is < 0.0 or > 1.0)
            throw new ArgumentOutOfRangeException(nameof(value), value, "匹配率须在 0~1 之间");
        Value = value;
    }

    /// <summary>默认匹配率 0.95。</summary>
    public static readonly MatchRate Default = new(0.95);
}
