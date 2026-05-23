#nullable enable
using System;

namespace Dota2Simulator.GameAutomation.Domain.Heroes;

/// <summary>
/// Phase 10C S1: 标注 IHeroStrategy 实现类的 SSOT attribute.
/// SG (S3+) 扫此 attribute emit Strategy partial + Registry 映射.
/// </summary>
/// <remarks>
/// <para><see cref="Name"/>: 英雄名 (匹配 Form2 截图识别 string), 对应原 Main.根据当前英雄增强 switch case.</para>
/// <para><see cref="Attribute"/>: 主属性分类 (Strength/Agility/Intelligence/Universal), 取代 Main.cs region 隐式分类.</para>
/// <para><see cref="RequiresUi"/>: Phase 10C S3 测试 Strategy 6 ports 分流标识 (默认 false; true 表该 Strategy 需 UI 上下文).</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class HeroStrategyAttribute : Attribute
{
    public string Name { get; }
    public HeroAttribute Attribute { get; }
    public bool RequiresUi { get; init; }

    public HeroStrategyAttribute(string name, HeroAttribute attribute)
    {
        Name = name;
        Attribute = attribute;
    }
}
