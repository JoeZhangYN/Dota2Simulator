namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 条件槽标识。C1~C9 对应原 Main.cs 的 _条件1~_条件9，
/// Z/X/C/V/B/Space 对应原 _条件z~_条件space。
/// 枚举序 0~14 与原 条件配置项 数组索引一致。
/// </summary>
public enum ConditionSlotKey
{
    C1, C2, C3, C4, C5, C6, C7, C8, C9,
    Z, X, C, V, B, Space,
}
