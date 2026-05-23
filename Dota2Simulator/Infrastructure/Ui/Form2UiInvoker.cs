using System;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Ui.Adapters;

/// <summary>
/// IUiInvoker adapter：封装 Form2 单例的线程切换 + 6 个 TextBox 字段读写。
/// 持 Form2 引用——同 Form2.Instance 单例等价（D5 删 Form2.Instance 后此处仍持构造期注入的引用）。
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

    public void SetText(UiField field, string value) => Resolve(field).Text = value;

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
