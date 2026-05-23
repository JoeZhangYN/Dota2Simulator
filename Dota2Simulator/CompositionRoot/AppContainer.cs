// CompositionRoot/AppContainer.cs
// #if DOTA2：Phase 6 正式组合根，取代过渡形态 AppComposition。
// 实例化 + 持有 HeroStrategyRegistry / GameSession，由 Program.cs 构造后传入 Form2。
#if DOTA2

using Dota2Simulator.Diagnostics;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Games;
using Dota2Simulator.Input.Adapters;
using Dota2Simulator.Ui.Adapters;
using Dota2Simulator.Vision.Adapters;

namespace Dota2Simulator.CompositionRoot;

/// <summary>
/// 应用组合根（Composition Root）：在 Program.cs 单点装配所有依赖，
/// 把 GameSession 实例从静态全局变为构造器注入到 Form2。
///
/// A2 阶段：仅承载 Registry + GameSession（前身静态 AppComposition 已删）。
/// A3 后：将持有 IInputExecutor / IScreenVision ports，并把它们注入 HeroStrategyRegistry。
/// A6 后：ports 经 ProbeInputExecutor / ProbeScreenVision 装饰。
/// D1 后：新增 IUiInvoker 通过 BindUi(Form2) 在 Form2 构造完成后注入；
///        同时 set Common.UiInvoker 给 BC 内 static class（service locator 临时入口，D5 评估去留）。
/// </summary>
internal sealed class AppContainer
{
    public IInputExecutor Input { get; }

    public IScreenVision Vision { get; }

    public IUiInvoker? Ui { get; private set; }

    public HeroStrategyRegistry Registry { get; }

    public GameSession GameSession { get; }

    public SessionState SessionState { get; }

    public AppContainer()
    {
        // ports 装饰链：HybridInputAdapter / RustVisionAdapter → ProbeXxx 录像装饰 → 注入下游
        // RecordReplayProbe.Enabled 默认 false，装饰器零开销（仅一次 volatile bool 读)
        Input = new ProbeInputExecutor(new HybridInputAdapter());
        Vision = new ProbeScreenVision(new RustVisionAdapter());

        // C6 单阶段：HeroAggregate ctor 接 vision；取代原 A5 双阶段 Main._聚合.Init(Vision)。
        Games.Dota2.Main._聚合 = new HeroAggregate(Vision);

        Registry = new HeroStrategyRegistry(Input, Vision);
        SessionState = new SessionState();
        // D2 双阶段：测试Strategy 需 IUiInvoker，但 Ui 要等 Form2 构造后 BindUi 才到位。
        // 故 RegisterAll 推迟到 BindUi 内调；ctor 期 Registry.Count == 0，
        // GameSession 在此窗口期收到按键也只是 no-op 查不到策略——Form2 ctor 立刻接 BindUi。
        GameSession = new GameSession(Registry, SessionState);

        // C3 过渡 service locator：Main / Item / Skill 仍是 static class，
        // 无法 ctor 注入 SessionState；先经 static field 桥接，D1 删（届时 Main/Item/Skill 已实例化走 ctor 注入）。
        Games.Dota2.Main._session = SessionState;
    }

    /// <summary>
    /// Form2 构造完成后回调——此时 InitializeComponent 已跑过，tb_* 字段已可用。
    /// 同步 set Common.UiInvoker / Common.SkillEngine（BC 内 thin facade 走的 service locator 入口）
    /// + Registry.RegisterAll。
    /// </summary>
    public void BindUi(Form2 form)
    {
        Ui = new Form2UiInvoker(form);
        Common.UiInvoker = Ui;
        // C4 / C5: SkillEngine / ItemEngine 实例化（ctor 接 4 ports），桥接给 BC 内 facade。
        Common.SkillEngine = new SkillEngine(Input, Vision, Ui, Games.Dota2.Main._聚合);
        Common.ItemEngine = new ItemEngine(Input, Vision, Ui, Games.Dota2.Main._聚合);
        // Phase 9 C: HeroLoopHost 实例化（ctor 接 7 ports，含 SkillEngine/ItemEngine 消除 Main↔Item 循环依赖）。
        // 装配顺序：SkillEngine/ItemEngine 之后（HeroLoopHost 内自调 _skill.X / _item.X，反向依赖闭合）。
        Common.HeroLoopHost = new HeroLoopHost(Input, Vision, Ui, Games.Dota2.Main._聚合, SessionState, Common.SkillEngine, Common.ItemEngine);
        // C7: Registry.RegisterAll 扩参，透传 SkillEngine / ItemEngine 给 92 策略 ctor。
        Registry.RegisterAll(Ui, Common.SkillEngine, Common.ItemEngine);
    }
}

#endif
