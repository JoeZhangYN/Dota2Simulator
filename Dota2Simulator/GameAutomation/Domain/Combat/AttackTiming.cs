namespace Dota2Simulator.GameAutomation.Domain.Combat;

/// <summary>
/// 攻击节奏时长。内部统一毫秒，取代 Main.cs 中 double 秒 / long 毫秒混用。
/// </summary>
public readonly record struct AttackTiming
{
    private readonly long _milliseconds;

    private AttackTiming(long milliseconds) => _milliseconds = milliseconds;

    public static AttackTiming FromSeconds(double seconds) => new((long)(seconds * 1000));

    public static AttackTiming FromMilliseconds(long milliseconds) => new(milliseconds);

    public long Milliseconds => _milliseconds;
}
