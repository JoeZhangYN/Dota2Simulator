// GameAutomation/Application/GameSession.cs
// #if DOTA2：依赖 Games.Dota2.Main（仅 DOTA2 构建存在）。
// Form2 对 GameSession 的接入同样在 #if DOTA2 块内，一致。
#if DOTA2
#nullable enable

using System;
using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Games;
using Dota2Simulator.Vision;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 入站端口 <see cref="IGameSession"/> 的实现：单轨策略分发。
///
/// 策略路径：英雄已在 <see cref="HeroStrategyRegistry"/> 注册 → 走 IHeroStrategy。
/// 未注册路径：no-op（旧 Common.HeroLoopHost!.根据当前英雄增强 switch 已于 Chunk 4.24 删除；
/// 原 switch 对未知英雄名同样什么都不做，二者等价）。
/// </summary>
public sealed class GameSession : IGameSession
{
    private readonly HeroStrategyRegistry _registry;
    private readonly SessionState _sessionState;
    private readonly HeroLoopHost _host;

    /// <summary>当前激活的英雄上下文；未激活或被 CancelAll 清空时为 null。</summary>
    private HeroContext? _current;

    // Phase 11 P5: ctor 扩 HeroLoopHost (消 3 处 Common.HeroLoopHost! 直调).
    // AppContainer 装配序调整: GameSession 推迟到 BindUi 内 new (HeroLoopHost new 后).
    public GameSession(HeroStrategyRegistry registry, SessionState sessionState, HeroLoopHost host)
    {
        _registry = registry;
        _sessionState = sessionState;
        _host = host;
    }

    /// <inheritdoc />
    public bool IsPaused
    {
        get => _sessionState.IsPaused;
        set => _sessionState.IsPaused = value;
    }

    /// <inheritdoc />
    public async Task DispatchAsync(HeroId hero, KeyTrigger trigger)
    {
        if (_registry.TryGet(hero.Name, out IHeroStrategy strategy))
        {
            // 策略路径（Wave 4 注册英雄后才会进入）。
            if (_current is null || _current.Hero != strategy.Hero)
            {
                // 4.7 过渡：复用全局 _host._聚合，待后续 chunk 让每英雄独立聚合.
                _current = new HeroContext(strategy.Hero, _host._聚合);
                strategy.OnActivate(_current);

                // 启动 Main 主循环（一般程序循环）。状态初始化() 内为 while(_总循环条件)
                // 无限循环，不能 await（会阻塞 DispatchAsync 永不返回）——fire-and-forget。
                // OnActivate 须已置位 _总循环条件，与旧 case 块 _总循环条件=true 先于
                // 状态初始化() 调用的顺序一致。
                _ = _host.状态初始化();

                // Phase 10B B4: 按英雄预加载 SG hint 字典提示的图片资源, fire-and-forget 不阻塞主流程.
                // 异常仅 log + 不抛 (与 LazyImageLoader.LoadImageCore 容错风格一致).
                // 消 Phase 10A SOFT_FAIL #1 (PreloadHints 桥半建): SG 已 emit 字典但主项目 0 引用.
                // 重复 key 由 LazyImageLoader 内 ConcurrentDictionary<Lazy<ImageHandle>> 防御零代价.
                if (PictureHeroPreloadHints.Hints.TryGetValue(hero.Name, out var preloadKeys) && preloadKeys.Length > 0)
                {
                    _ = LazyImageLoader.PreloadImagesAsync(preloadKeys).ContinueWith(
                        t => Console.WriteLine($"[Preload] {hero.Name} 失败: {t.Exception?.GetBaseException().Message}"),
                        TaskContinuationOptions.OnlyOnFaulted);
                }
            }

            await strategy.OnKeyAsync(trigger, _current).ConfigureAwait(false);
        }
        else
        {
            // 未注册英雄：策略未实现，no-op。
            // 旧 Common.HeroLoopHost!.根据当前英雄增强 switch 已于 Chunk 4.24 删除——其 switch(name)
            // 对未命中的 name 本就走默认分支什么都不做，此 no-op 与之行为等价。
        }
    }

    /// <inheritdoc />
    public void CancelAll()
    {
        _current = null;
        _host.取消所有功能();
    }
}

#endif
