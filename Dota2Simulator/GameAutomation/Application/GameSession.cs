// GameAutomation/Application/GameSession.cs
// #if DOTA2：依赖 Games.Dota2.Main（仅 DOTA2 构建存在）。
// Form2 对 GameSession 的接入同样在 #if DOTA2 块内，一致。
#if DOTA2
#nullable enable

using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Ports;
using Main = Dota2Simulator.Games.Dota2.Main;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 入站端口 <see cref="IGameSession"/> 的实现：双轨分发。
///
/// 策略路径：英雄已在 <see cref="HeroStrategyRegistry"/> 注册（Wave 4 后才有）→ 走 IHeroStrategy。
/// fallback 路径：未注册 → 回退旧 Main.根据当前英雄增强 switch。
///
/// 4.7 阶段 registry 为空，DispatchAsync 永远走 fallback，行为与
/// Form2 原来直调 根据当前英雄增强(name, e) 逐字节等价。
/// </summary>
public sealed class GameSession : IGameSession
{
    private readonly HeroStrategyRegistry _registry;

    /// <summary>当前激活的英雄上下文；未激活或被 CancelAll 清空时为 null。</summary>
    private HeroContext? _current;

    public GameSession(HeroStrategyRegistry registry) => _registry = registry;

    /// <inheritdoc />
    public async Task DispatchAsync(HeroId hero, KeyTrigger trigger)
    {
        if (_registry.TryGet(hero.Name, out IHeroStrategy strategy))
        {
            // 策略路径（Wave 4 注册英雄后才会进入）。
            if (_current is null || _current.Hero != strategy.Hero)
            {
                // 4.7 过渡：复用全局 Main._聚合，待后续 chunk 让每英雄独立聚合。
                _current = new HeroContext(strategy.Hero, Main._聚合);
                strategy.OnActivate(_current);

                // 启动 Main 主循环（一般程序循环）。状态初始化() 内为 while(_总循环条件)
                // 无限循环，不能 await（会阻塞 DispatchAsync 永不返回）——fire-and-forget。
                // OnActivate 须已置位 _总循环条件，与旧 case 块 _总循环条件=true 先于
                // 状态初始化() 调用的顺序一致。
                _ = Main.状态初始化();
            }

            await strategy.OnKeyAsync(trigger.Key, _current).ConfigureAwait(false);
        }
        else
        {
            // fallback：旧 switch。重建 KeyEventArgs(KeyCode) 与原 e 逐字节等价——
            // 全项目 根据当前英雄增强 及其调用链零使用 e.Handled / SuppressKeyPress，仅用 e.KeyCode。
            await Main.根据当前英雄增强(
                hero.Name,
                new System.Windows.Forms.KeyEventArgs(
                    (System.Windows.Forms.Keys)trigger.Key.ToNative()))
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void CancelAll()
    {
        _current = null;
        Main.取消所有功能();
    }
}

#endif
