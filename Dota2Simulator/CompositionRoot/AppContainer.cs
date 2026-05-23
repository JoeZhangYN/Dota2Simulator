// CompositionRoot/AppContainer.cs
// Phase 9 F：组合根装配链终态——Main/Skill/Item facade 已删，直接持有所有实例。
// 装配顺序：Input/Vision → HeroAggregate → SessionState → SkillEngine → ItemEngine → HeroLoopHost → Registry。
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
/// 应用组合根（Composition Root）：在 Program.cs 单点装配所有依赖，由 Program 注入 Form2。
/// </summary>
internal sealed class AppContainer
{
    public IInputExecutor Input { get; }
    public IScreenVision Vision { get; }
    public IUiInvoker? Ui { get; private set; }
    public HeroStrategyRegistry Registry { get; }
    public GameSession GameSession { get; }
    public SessionState SessionState { get; }
    public HeroAggregate Aggregate { get; }

    /// <summary>HeroLoopHost 在 BindUi 装配（依赖 Ui port）；Form2 经 AppContainer.HeroLoopHost 访问。</summary>
    public HeroLoopHost? HeroLoopHost { get; private set; }

    public AppContainer()
    {
        // ports 装饰链：HybridInputAdapter / RustVisionAdapter → ProbeXxx 录像装饰
        Input = new ProbeInputExecutor(new HybridInputAdapter());
        Vision = new ProbeScreenVision(new RustVisionAdapter());

        // C6 单阶段 HeroAggregate ctor 接 vision。
        Aggregate = new HeroAggregate(Vision);

        Registry = new HeroStrategyRegistry(Input, Vision);
        SessionState = new SessionState();
        // D2 双阶段：测试Strategy 需 IUiInvoker，Ui 要等 Form2 构造后 BindUi 才到位。
        // 故 RegisterAll 推迟到 BindUi 内调；ctor 期 Registry.Count == 0。
        GameSession = new GameSession(Registry, SessionState);
    }

    /// <summary>
    /// Form2 构造完成后回调——此时 InitializeComponent 已跑过，tb_* 字段已可用。
    /// 依次装 Ui / SkillEngine / ItemEngine / HeroLoopHost / Registry.RegisterAll。
    /// </summary>
    public void BindUi(Form2 form)
    {
        Ui = new Form2UiInvoker(form);
        // SkillEngine 先 new（ItemEngine ctor 接 SkillEngine；HeroLoopHost ctor 接两者）。
        var skill = new SkillEngine(Input, Vision, Ui, Aggregate);
        // Phase 11 P3: ItemEngine ctor 扩 SessionState (Esc 暂停经 _session.IsPaused 直调).
        var item = new ItemEngine(Input, Vision, Ui, Aggregate, skill, SessionState);
        // Phase 11 P2: setter 注入消反向 service locator——SkillEngine 内 2 处 Common.ItemEngine!.要求保持假腿() 改 _item!.要求保持假腿().
        skill.BindItem(item);
        HeroLoopHost = new HeroLoopHost(Input, Vision, Ui, Aggregate, SessionState, skill, item);
        // Phase 11 P3: ItemEngine 4 处反向 HeroLoopHost 经 BindHost setter 注入 (取消所有功能 / 按键匹配条件更新 / 获取图片_2).
        item.BindHost(HeroLoopHost);
        // Phase 11 P4: SkillEngine 2 处反向 HeroLoopHost 经 BindHost setter 注入 (测试方法 / 捕捉颜色 的获取图片_2).
        skill.BindHost(HeroLoopHost);
        // Common.ItemEngine 保留 (P9 真删): Silt/Main.cs:29/34 仍 2 处反向 (Silt instance 化 P6 处理).
        Common.ItemEngine = item;
        // Common.HeroLoopHost 保留：Silt 子 BC 经 Common.HeroLoopHost.Ui 访问 UI
        // （Phase 11 Silt instance 化时本桥可删）。
        Common.HeroLoopHost = HeroLoopHost;
        Registry.RegisterAll(Ui, skill, item, HeroLoopHost);
    }
}

#endif
