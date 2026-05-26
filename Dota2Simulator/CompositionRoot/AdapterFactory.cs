// CompositionRoot/AdapterFactory.cs
// Phase 19G-1: LOL/HF2 装配链统一前置 — 抽公共 adapter 装饰链 SSOT.
// DOTA2 / LOL / HF2 三 build 共享 Input/Vision adapter 创建逻辑 (ProbeXxx 装饰链).
// AppContainer (DOTA2) ctor + Form2 (LOL/HF2) ctor 都消费此工厂, 避免装配链分叉漂移.
using Dota2Simulator.Diagnostics;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Input.Adapters;
using Dota2Simulator.Ui.Adapters;
using Dota2Simulator.Vision.Adapters;

namespace Dota2Simulator.CompositionRoot;

/// <summary>
/// adapter 装饰链工厂 (无 #if game-define 包裹, 三 build 共享).
/// 装饰顺序: HybridInputAdapter / RustVisionAdapter → ProbeXxx 录像装饰 (回归对比信号).
/// </summary>
internal static class AdapterFactory
{
    public static IInputExecutor CreateInput() => new ProbeInputExecutor(new HybridInputAdapter());

    public static IScreenVision CreateVision() => new ProbeScreenVision(new RustVisionAdapter());

    public static IUiInvoker CreateUi(Form2 form) => new Form2UiInvoker(form);
}
