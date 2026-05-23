using System;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>WinForms TextBox 字段标识——隔离业务侧对具体控件类型的依赖。</summary>
public enum UiField
{
    Name,
    阵营,
    X,
    Y,
    等待延迟,
    状态抗性,
}

/// <summary>
/// 出站端口：领域对「UI 线程切换 + 状态字段读写」的需求。
/// 由 Form2UiInvoker adapter 实现，封装 Common.Main_Form 单例的 Invoke / TextBox.Text 访问。
/// 业务侧拿 raw string：数值解析（int.Parse）归业务，port 只搬运。
/// </summary>
public interface IUiInvoker
{
    /// <summary>在 UI 线程上同步执行 action（封装 Form2.Invoke）。</summary>
    void Invoke(Action action);

    /// <summary>读取指定 UI 字段的 raw 文本。返回值未 Trim——调用方按需 Trim。</summary>
    string GetText(UiField field);

    /// <summary>设置指定 UI 字段的文本。</summary>
    void SetText(UiField field, string value);
}
