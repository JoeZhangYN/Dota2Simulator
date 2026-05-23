#if DOTA2
using System;
using System.Collections.Generic;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 英雄策略注册表：英雄中文名 → <see cref="IHeroStrategy"/>。
/// GameSession 据此分发；未注册的英雄 no-op（旧 Main switch 已于 Chunk 4.24 删除）。
///
/// 92 个英雄策略按属性分批注册——partial class 拆 .Strength / .Agility / .Intelligence / .Universal
/// 四个文件，并行填充 RegisterStrength/Agility/Intelligence/Universal 时零合并冲突。
///
/// Phase 6 A4：构造期接收 IInputExecutor / IScreenVision，由 4 个 partial method 透传给各 Strategy ctor，
/// 让 Heroes/ 内层只依赖端口接口，不再 using Dota2Simulator.KeyboardMouse。
/// </summary>
public sealed partial class HeroStrategyRegistry
{
    private readonly Dictionary<string, IHeroStrategy> _byName = new();
    private readonly IInputExecutor _input;
    private readonly IScreenVision _vision;
    // D2: 测试Strategy 是 92 策略中唯一需要 IUiInvoker 的（反向抓 UI tb_阵营 / tb_y）。
    // 字段而非 ctor 参数——Registry 在 AppContainer.ctor 期 new，Ui 要等 Form2 构造后 BindUi 才到位。
    // 走双阶段：ctor 接 input+vision，RegisterAll(ui) 时补 _ui 再分发到 Universal partial。
    private IUiInvoker? _ui;

    public HeroStrategyRegistry(IInputExecutor input, IScreenVision vision)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _vision = vision ?? throw new ArgumentNullException(nameof(vision));
    }

    /// <summary>注册一个英雄策略（以英雄中文名为键）。</summary>
    public void Register(IHeroStrategy strategy) => _byName[strategy.Hero.Name] = strategy;

    /// <summary>按英雄名查找策略；未注册返回 false（GameSession 走 no-op）。</summary>
    public bool TryGet(string name, out IHeroStrategy strategy)
        => _byName.TryGetValue(name, out strategy!);

    /// <summary>已注册的英雄数。</summary>
    public int Count => _byName.Count;

    /// <summary>注册所有英雄策略——AppContainer.BindUi 时调用（此刻 ui / skill / item / main 已到位）。
    /// Phase 9 D: 扩 +HeroLoopHost 透传给 92 策略 ctor，消灭 Main.X service locator。</summary>
    public void RegisterAll(IUiInvoker ui, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        RegisterStrength(_input, _vision, skill, item, main);
        RegisterAgility(_input, _vision, skill, item, main);
        RegisterIntelligence(_input, _vision, skill, item, main);
        RegisterUniversal(_input, _vision, skill, item, main);
    }

    // 各 partial 文件实现一个；未实现的 partial void 调用编译期消除。
    partial void RegisterStrength(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main);
    partial void RegisterAgility(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main);
    partial void RegisterIntelligence(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main);
    partial void RegisterUniversal(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main);
}

#endif
