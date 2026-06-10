#if DOTA2
namespace Dota2Simulator.GameAutomation.Domain.Items;

/// <summary>
/// 物品槽离散态（某一时刻互斥，由视觉/逻辑信号解析）。
/// 与连续态 <see cref="Cooldown"/> 叠加构成 <see cref="ItemSlotState"/>。
/// </summary>
public enum ItemDiscreteState
{
    /// <summary>不在（消费掉 / 从未拥有）。</summary>
    Absent,

    /// <summary>可使用（正常图标，可立即按）。</summary>
    Usable,

    /// <summary>鼠标悬停高亮（瞬态视觉）。检测信号待补，当前解析器不产出。</summary>
    Highlighted,

    /// <summary>可使用但被锁闭。</summary>
    Locked,

    /// <summary>可使用但英雄被硬控（present 但不可发）。</summary>
    HeroDisabled,

    /// <summary>可使用但蓝量不够。检测信号待补，当前解析器不产出。</summary>
    LowMana,
}

/// <summary>
/// 物品连续态：CD。当前只承载「在/不在 CD」的离散信号（来自槽位灰线色检测）；
/// 剩余时间为后续扩展（需更细的 CD 数值读取信号）。
/// </summary>
public readonly record struct Cooldown(bool OnCooldown)
{
    public static readonly Cooldown Ready = new(false);
    public static readonly Cooldown Active = new(true);
}

/// <summary>
/// 物品槽叠加态 = (离散态, 连续态)。把「可使用 / 锁闭 / 硬控 / CD / 不在」等业务硬约束
/// 内化进类型，供消费一次判定与物品逻辑统一查询（替散落的 bool 检查）。
///
/// 解析走纯函数 <see cref="Resolve"/>：ItemEngine 采集原始信号（模板是否命中 / 槽 CD /
/// 锁闭 / 硬控）后组合成本类型，模型层无 IO、可独立推理。
/// </summary>
public readonly record struct ItemSlotState(ItemDiscreteState Discrete, Cooldown Cooldown)
{
    /// <summary>物品当前在槽内（无论是否可立即发）。</summary>
    public bool IsPresent => Discrete != ItemDiscreteState.Absent;

    /// <summary>此刻可立即使用：离散态为可使用且不在 CD。</summary>
    public bool CanUseNow => Discrete == ItemDiscreteState.Usable && !Cooldown.OnCooldown;

    /// <summary>
    /// 由原始信号纯组合出状态。<paramref name="found"/> = 可使用形态模板是否命中。
    /// 离散态优先级：硬控 &gt; 锁闭 &gt; 可使用；未命中 → 不在（连续态仍可携带末槽 CD 信号，
    /// 用于把「只是进 CD 的灰图标」与「真消费掉」区分开）。
    /// LowMana / Highlighted 需额外信号，本工厂暂不产出。
    /// </summary>
    public static ItemSlotState Resolve(bool found, bool onCooldown, bool locked, bool heroDisabled)
    {
        Cooldown cd = onCooldown ? Cooldown.Active : Cooldown.Ready;
        if (!found)
            return new ItemSlotState(ItemDiscreteState.Absent, cd);

        ItemDiscreteState discrete =
            heroDisabled ? ItemDiscreteState.HeroDisabled :
            locked ? ItemDiscreteState.Locked :
            ItemDiscreteState.Usable;
        return new ItemSlotState(discrete, cd);
    }
}
#endif
