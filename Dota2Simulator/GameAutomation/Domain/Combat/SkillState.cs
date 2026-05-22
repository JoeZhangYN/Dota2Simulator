namespace Dota2Simulator.GameAutomation.Domain.Combat;

/// <summary>
/// 技能状态。显式状态对象（discriminated union），取代裸 int 步骤 / 模式当状态机。
/// 刻意不用位标志 / 连续内存——那是已否决的 GameStateManager 方向。
/// </summary>
public abstract record SkillState
{
    private SkillState() { }

    /// <summary>空闲，可释放。</summary>
    public sealed record Idle : SkillState;

    /// <summary>施法中，处于第 Step 步，已耗时 Elapsed。</summary>
    public sealed record Casting(int Step, AttackTiming Elapsed) : SkillState;

    /// <summary>冷却中，ReadyAtMs 时刻就绪。</summary>
    public sealed record OnCooldown(long ReadyAtMs) : SkillState;
}
