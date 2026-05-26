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
    /// <summary>Phase 11 P5: GameSession 推迟到 BindUi (ctor 接 HeroLoopHost). Form2 经 _app.GameSession! 访问.</summary>
    public GameSession? GameSession { get; private set; }

    /// <summary>Phase 12 Chunk 2: IGameEngine 别名 —— Form2 统一字段 _engine 由此装配 (DOTA2 build 走 GameSession).</summary>
    public IGameEngine? GameEngine => GameSession;
    public SessionState SessionState { get; }
    public HeroAggregate Aggregate { get; }

    /// <summary>HeroLoopHost 在 BindUi 装配（依赖 Ui port）；Form2 经 AppContainer.HeroLoopHost 访问。</summary>
    public HeroLoopHost? HeroLoopHost { get; private set; }

    public AppContainer()
    {
        // Phase 19G-1: adapter 装饰链通过 AdapterFactory 共享 (DOTA2 / LOL / HF2 三 build 装配链 SSOT)
        Input = AdapterFactory.CreateInput();
        Vision = AdapterFactory.CreateVision();

        // Phase 18 V6a: 委托签名 () 无参后 HeroAggregate 不再依赖 vision，回到纯领域聚合形态。
        Aggregate = new HeroAggregate();

        Registry = new HeroStrategyRegistry(Input, Vision);
        SessionState = new SessionState();
        // Phase 11 P5: GameSession 推迟到 BindUi (ctor 接 HeroLoopHost, 后者依 Ui port).
    }

    /// <summary>
    /// Form2 构造完成后回调——此时 InitializeComponent 已跑过，tb_* 字段已可用。
    /// 依次装 Ui / SkillEngine / ItemEngine / HeroLoopHost / Registry.RegisterAll。
    /// </summary>
    public void BindUi(Form2 form)
    {
        // Phase 19G-1: Ui adapter 经 AdapterFactory 装配 (与 LOL/HF2 build 共享)
        Ui = AdapterFactory.CreateUi(form);
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
        // Phase 11 P5: GameSession ctor 接 HeroLoopHost (消 3 处 Common.HeroLoopHost! 直调).
        GameSession = new GameSession(Registry, SessionState, HeroLoopHost);
#if Silt
        // Phase 11 P6: SiltEngine 装配——ctor 接 (input/vision/ui/item); ItemEngine NumPad1-6 dispatch + HeroLoopHost 状态初始化 经 BindSilt setter 注入.
        var silt = new Dota2Simulator.Games.Dota2.Silt.SiltEngine(Input, Vision, Ui, item);
        item.BindSilt(silt);
        HeroLoopHost.BindSilt(silt);
#endif
        // Phase 11 P9: Common.ItemEngine + Common.HeroLoopHost 两 service locator 字段真删 (0 service locator 终态).
        Registry.RegisterAll(Ui, skill, item, HeroLoopHost);
    }
}

#endif
