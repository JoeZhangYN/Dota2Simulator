// CompositionRoot/AppContainer.cs
// #if DOTA2：Phase 6 正式组合根，取代过渡形态 AppComposition。
// 实例化 + 持有 HeroStrategyRegistry / GameSession，由 Program.cs 构造后传入 Form2。
#if DOTA2

using Dota2Simulator.GameAutomation.Application;

namespace Dota2Simulator.CompositionRoot;

/// <summary>
/// 应用组合根（Composition Root）：在 Program.cs 单点装配所有依赖，
/// 把 GameSession 实例从静态全局变为构造器注入到 Form2。
///
/// A2 阶段：仅承载 Registry + GameSession（前身静态 AppComposition 已删）。
/// A3 后：将持有 IInputExecutor / IScreenVision ports，并把它们注入 HeroStrategyRegistry。
/// A6 后：ports 经 ProbeInputExecutor / ProbeScreenVision 装饰。
/// </summary>
internal sealed class AppContainer
{
    public HeroStrategyRegistry Registry { get; }

    public GameSession GameSession { get; }

    public AppContainer()
    {
        Registry = new HeroStrategyRegistry();
        Registry.RegisterAll();
        GameSession = new GameSession(Registry);
    }
}

#endif
