using System.Drawing;

namespace Dota2Simulator.GameAutomation.Domain.Loop;

/// <summary>
/// 七个技能槽（Global + Q/W/E/R/D/F）的可变容器——per-hero 聚合的组成部分。
/// 取代 Main.cs 中 _全局时间 / _全局模式 / _全局步骤 / _指定地点 四组共 28 个散落 static 字段。
///
/// 对外只暴露按 <see cref="SlotKey"/> 参数化的语义方法，调用方以「方法 + 槽位参数」组合表达，
/// 不再有 28 个具名字段那样的同构重复。
/// </summary>
public sealed class SkillSlotSet
{
    private readonly SkillSlot[] _slots = new SkillSlot[7];

    public SkillSlotSet() => Reset();

    /// <summary>
    /// 复位所有槽。Global 槽 <see cref="SkillSlot.TimeMs"/> 初值 -1
    /// （沿用原 _全局时间 的未初始化哨兵），其余字段全 0。
    /// </summary>
    public void Reset()
    {
        for (int i = 0; i < _slots.Length; i++)
            _slots[i] = default;
        _slots[(int)SlotKey.Global] = default(SkillSlot) with { TimeMs = -1 };
    }

    /// <summary>按槽位读写整槽状态。</summary>
    public SkillSlot this[SlotKey key]
    {
        get => _slots[(int)key];
        set => _slots[(int)key] = value;
    }

    // --- 时间（原 _全局时间X）---
    public long Time(SlotKey key) => _slots[(int)key].TimeMs;
    public void SetTime(SlotKey key, long ms) => _slots[(int)key] = _slots[(int)key] with { TimeMs = ms };

    // --- 模式（原 _全局模式X）---
    public int Mode(SlotKey key) => _slots[(int)key].Mode;
    public void SetMode(SlotKey key, int value) => _slots[(int)key] = _slots[(int)key] with { Mode = value };

    /// <summary>模式 0/1 自反翻转——取代散落 21 处的 _全局模式X = 1 - _全局模式X。</summary>
    public void ToggleMode(SlotKey key) => _slots[(int)key] = _slots[(int)key] with { Mode = 1 - _slots[(int)key].Mode };

    // --- 步骤（原 _全局步骤X）---
    public int Step(SlotKey key) => _slots[(int)key].Step;
    public void SetStep(SlotKey key, int value) => _slots[(int)key] = _slots[(int)key] with { Step = value };

    // --- 指定地点（原 _指定地点X）---
    public Point Target(SlotKey key) => _slots[(int)key].TargetPoint;
    public void SetTarget(SlotKey key, Point value) => _slots[(int)key] = _slots[(int)key] with { TargetPoint = value };
}
