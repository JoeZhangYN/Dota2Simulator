#if DOTA2
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 英雄策略执行的上下文：当前英雄标识 + 其运行态聚合。
/// 策略类经此访问状态，不直接触碰 Main.cs 的全局 static。
/// </summary>
public sealed class HeroContext
{
    public HeroId Hero { get; }

    /// <summary>当前英雄的运行态聚合（技能槽 / 条件槽 / 攻击计时）。</summary>
    public HeroAggregate Aggregate { get; }

    public HeroContext(HeroId hero, HeroAggregate aggregate)
    {
        Hero = hero;
        Aggregate = aggregate;
    }
}

#endif
