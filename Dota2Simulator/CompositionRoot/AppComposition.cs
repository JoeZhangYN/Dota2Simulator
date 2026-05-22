// CompositionRoot/AppComposition.cs
// #if DOTA2：持有 GameSession（仅 DOTA2 构建存在）。
#if DOTA2

using Dota2Simulator.GameAutomation.Application;

namespace Dota2Simulator.CompositionRoot;

/// <summary>
/// 临时静态组合根——把 HeroStrategyRegistry 与 GameSession 接线到一起，供 Form2 取用。
///
/// 过渡形态：Phase 6 将由正式的 AppContainer（实例化、可注入、生命周期受控）取代。
/// 当前用 static 是为让 Form2 在不改构造签名的前提下经 GameSession 分发按键。
///
/// 4.7 阶段 RegisterAll() 注册结果为空（英雄策略 Wave 4 才填充），
/// 故 GameSession.DispatchAsync 永远走 fallback 回退旧 Main switch。
/// </summary>
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
