namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 会话级共享状态——跨英雄、跨切换不重置（直到 CancelAll 手动 Reset）。
/// Phase 8 C3 引入：取代 Main._中断条件 static，并为 Phase 9 HeroIdentity 预留挂点。
/// </summary>
public sealed class SessionState
{
    /// <summary>暂停标志——Item.cs Esc 键 toggle；Main 主循环 + 6 个策略读判分支。
    /// 取代原 Main.cs static `_中断条件` 字段。</summary>
    public bool IsPaused { get; set; }

    /// <summary>当局英雄身份模板（Phase 9 epic 实现）。
    /// 设计意图：F1 提取 HUD 英雄名区域 → 多帧像素一致性投票 → 稳态固化作"自己英雄"绑定；
    /// 所有 HUD 读取经身份 gate，看别人状态时返回上一稳态快照避免误触。
    /// C3 阶段仅占位，识别逻辑零实现。</summary>
    public IHeroIdentity? Identity { get; internal set; }
}

/// <summary>Phase 9 占位接口——HeroIdentity 模板提取/稳态固化/gate 决策方法在新 epic 内补全。</summary>
public interface IHeroIdentity
{
    /// <summary>当局已固化的英雄名称（稳态前可为 null）。</summary>
    string? CurrentHeroName { get; }
}
