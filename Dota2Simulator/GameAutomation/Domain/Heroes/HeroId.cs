namespace Dota2Simulator.GameAutomation.Domain.Heroes;

/// <summary>
/// 英雄标识。中文名为稳定主键（贯穿图片资源 / UI 文本框），取代裸 string 分发。
/// </summary>
public readonly record struct HeroId(string Name, HeroAttribute Attribute);
