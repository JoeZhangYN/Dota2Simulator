// CompositionRoot/AppComposition.cs
// #if DOTA2：持有 GameSession（仅 DOTA2 构建存在）。
#if DOTA2

using Dota2Simulator.GameAutomation.Application;

namespace Dota2Simulator.CompositionRoot;

/// <summary>
/// 临时静态组合根——已被 <see cref="AppContainer"/> 取代。
///
/// A1 chunk：Program.cs 不再引用本类，Form2 经构造器注入 AppContainer 拿 GameSession。
/// A2 chunk：本文件将被删除。当前保留是为保证 A1 commit 单步可回滚。
/// </summary>
[System.Obsolete("已被 AppContainer 取代，A2 chunk 删除")]
internal static class AppComposition
{
    private static readonly HeroStrategyRegistry _registry = CreateRegistry();

    /// <summary>全局唯一的 GameSession 实例，Form2 经此分发按键。</summary>
    public static GameSession GameSession { get; } = new(_registry);

    private static HeroStrategyRegistry CreateRegistry()
    {
        HeroStrategyRegistry registry = new();
        registry.RegisterAll(); // 4.7 阶段为空；Wave 4 后填充英雄策略。
        return registry;
    }
}

#endif
