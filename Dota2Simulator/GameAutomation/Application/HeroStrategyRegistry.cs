using System.Collections.Generic;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 英雄策略注册表：英雄中文名 → <see cref="IHeroStrategy"/>。
/// GameSession 据此分发；未注册的英雄 fallback 回旧 Main.根据当前英雄增强 switch（双轨过渡）。
///
/// 89 个英雄策略按属性分批注册——partial class 拆 .Strength / .Agility / .Intelligence
/// 三个文件，Wave 4 三组并行填充 RegisterStrength/Agility/Intelligence 时零合并冲突。
/// </summary>
public sealed partial class HeroStrategyRegistry
{
    private readonly Dictionary<string, IHeroStrategy> _byName = new();

    /// <summary>注册一个英雄策略（以英雄中文名为键）。</summary>
    public void Register(IHeroStrategy strategy) => _byName[strategy.Hero.Name] = strategy;

    /// <summary>按英雄名查找策略；未注册返回 false（调用方 fallback 旧 switch）。</summary>
    public bool TryGet(string name, out IHeroStrategy strategy)
        => _byName.TryGetValue(name, out strategy!);

    /// <summary>已注册的英雄数。</summary>
    public int Count => _byName.Count;

    /// <summary>注册所有英雄策略。各属性分组在 partial 文件实现（Wave 4 填充）。</summary>
    public void RegisterAll()
    {
        RegisterStrength();
        RegisterAgility();
        RegisterIntelligence();
    }

    // Wave 4 三个 partial 文件各实现一个；未实现的 partial void 调用编译期消除。
    partial void RegisterStrength();
    partial void RegisterAgility();
    partial void RegisterIntelligence();
}
