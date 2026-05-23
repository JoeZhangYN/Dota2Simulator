using System;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Ui.Adapters;

/// <summary>
/// IUiInvoker adapter：封装 Form2 实例的线程切换 + 6 个 TextBox 字段读写。
/// 由 AppContainer.BindUi 在 Form2(container) 构造完成后实例化，持构造期注入的引用。
/// </summary>
internal sealed class Form2UiInvoker : IUiInvoker
{
    private readonly Form2 _form;

    public Form2UiInvoker(Form2 form)
    {
        _form = form ?? throw new ArgumentNullException(nameof(form));
    }

    public void Invoke(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (_form.IsDisposed) return;
        _form.Invoke(action);
    }

    public string GetText(UiField field) => Resolve(field).Text;

    /// <summary>
    /// Phase 8 A: 写场景异步化——内部走 BeginInvoke（Post 消息，不阻塞业务线程）。
    /// 与 Invoke (Send 阻塞) 的差异: 调用返回时 UI 控件未必已更新（消息排队中）。
    /// 用户判定（2026-05-23）TTS.Speak 抢跑 UI 渲染可接受——音频与 UI 渲染独立。
    /// 25 调用方 0 改动，外层 Common.UiInvoker.Invoke 包裹现冗余但保留（不破坏行为）。
    /// </summary>
    public void SetText(UiField field, string value)
    {
        if (_form.IsDisposed) return;
        _form.BeginInvoke(() => Resolve(field).Text = value);
    }

    private TextBox Resolve(UiField field) => field switch
    {
        UiField.Name => _form.tb_name,
        UiField.阵营 => _form.tb_阵营,
        UiField.X => _form.tb_x,
        UiField.Y => _form.tb_y,
        UiField.等待延迟 => _form.tb_等待延迟,
        UiField.状态抗性 => _form.tb_状态抗性,
        _ => throw new ArgumentOutOfRangeException(nameof(field), field, null),
    };
}
