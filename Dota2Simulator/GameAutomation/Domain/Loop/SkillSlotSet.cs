namespace Dota2Simulator.GameAutomation.Domain.Loop;

/// <summary>
/// 七个技能槽（Global + Q/W/E/R/D/F）的可变容器——per-hero 聚合的组成部分。
/// 取代 Main.cs 中 _全局时间/_全局模式/_全局步骤/_指定地点 四组共 28 个散落 static 字段。
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
}
