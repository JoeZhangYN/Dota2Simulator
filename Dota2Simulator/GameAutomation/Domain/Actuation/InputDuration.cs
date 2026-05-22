using System;

namespace Dota2Simulator.GameAutomation.Domain.Actuation;

/// <summary>输入动作时长（毫秒），构造时拒绝负值。</summary>
public readonly record struct InputDuration
{
    public int Milliseconds { get; }

    public InputDuration(int milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds), milliseconds, "时长不可为负");
        Milliseconds = milliseconds;
    }

    /// <summary>基准帧延迟 33ms（≈30fps）。</summary>
    public static readonly InputDuration Frame = new(33);
}
