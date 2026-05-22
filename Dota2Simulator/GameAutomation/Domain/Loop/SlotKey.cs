namespace Dota2Simulator.GameAutomation.Domain.Loop;

/// <summary>
/// 技能槽位标识。<see cref="Global"/> 对应原 Main.cs 无后缀字段
/// （_全局时间 / _全局模式 / _全局步骤）与 _指定地点p；Q~F 对应六个技能键槽。
/// </summary>
public enum SlotKey
{
    Global = 0,
    Q = 1,
    W = 2,
    E = 3,
    R = 4,
    D = 5,
    F = 6,
}
