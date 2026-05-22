namespace Dota2Simulator.GameAutomation.Domain.Combat;

/// <summary>关键物品状态。取代散落的裸 bool 神杖 / 魔晶 / 法球 + string 假腿。</summary>
public readonly record struct ItemState(
    bool HasScepter,
    bool HasShard,
    bool HasOrb,
    LegType ActiveLeg);
