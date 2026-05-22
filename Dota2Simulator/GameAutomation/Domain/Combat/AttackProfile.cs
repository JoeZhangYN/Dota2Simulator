namespace Dota2Simulator.GameAutomation.Domain.Combat;

/// <summary>
/// 攻击计时与走 A 状态的聚合——收编 Main.cs 原 9 个攻击相关 static 字段
/// （_基础攻击前摇/间隔、_攻击速度、_实际出手时间、_停止走A、_开启走A、
/// _实际攻击前摇/间隔、_状态抗性倍数）。
///
/// 注：基础攻击前摇/间隔以「秒」计，实际攻击前摇/间隔以「毫秒」计——
/// 沿用原字段语义，二者是不同概念变量、非同变量单位混用。
/// </summary>
public sealed class AttackProfile
{
    public double 基础攻击前摇 { get; set; } = 0.5;
    public double 基础攻击间隔 { get; set; } = 1.7;
    public double 攻击速度 { get; set; } = 100;
    public long 实际出手时间 { get; set; }
    public long 停止走A { get; set; }
    public bool 开启走A { get; set; }
    public long 实际攻击前摇 { get; set; } = 650 + 30;
    public long 实际攻击间隔 { get; set; } = 1700;
    public double 状态抗性倍数 { get; set; }

    /// <summary>
    /// 复位——沿用原 取消所有功能 的重置值（注意与字段初值不同：基础攻击前摇 0.3、
    /// 实际攻击前摇/间隔归 0）。原逻辑不重置 状态抗性倍数，此处保持一致。
    /// </summary>
    public void Reset()
    {
        攻击速度 = 100;
        基础攻击前摇 = 0.3;
        基础攻击间隔 = 1.7;
        实际出手时间 = 0;
        停止走A = 0;
        开启走A = false;
        实际攻击前摇 = 0;
        实际攻击间隔 = 0;
    }
}
