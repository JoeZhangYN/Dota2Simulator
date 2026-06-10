// Phase 28 C2 (2026-06-09): 命名物品连招 SSOT — 收口跨英雄重复的固定物品"序列字面量".
// 执行层 (批量使用物品 / 批量使用物品并行 / ParallelBatch) Phase 22B/26F1 已抽; 本类只去重序列字面量本身 (改一处全英雄生效).
#if DOTA2
using System.Linq;
using Dota2Simulator.GameAutomation.Domain.Perception;
using Dota2Simulator.GameAutomation.Domain.StepMachine;

namespace Dota2Simulator.Games.Dota2;

/// <summary>
/// 命名物品连招 — 把跨英雄重复的固定物品"序列字面量"收口为单一 SSOT.
/// 用法: <c>_item.批量使用物品并行(物品连招.红杖五连)</c>; StepMachine 内 <c>物品连招.红杖五连.AsParallelBatch()</c>.
/// </summary>
public static class 物品连招
{
    /// <summary>红杖 1-5 槽位 burst — 莱恩/沉默(经 大招前纷争)/骨法/屠夫/天怒 共用.</summary>
    public static Template[] 红杖五连 => new[]
    {
        Dota2_Pictrue.物品.红杖_Tpl,
        Dota2_Pictrue.物品.红杖2_Tpl,
        Dota2_Pictrue.物品.红杖3_Tpl,
        Dota2_Pictrue.物品.红杖4_Tpl,
        Dota2_Pictrue.物品.红杖5_Tpl,
    };

    /// <summary>大招前纷争 burst: 虚灵之刃 + 纷争 + 红杖五连 — 莱恩/沉默 共用.</summary>
    public static Template[] 大招前纷争 => new[]
    {
        Dota2_Pictrue.物品.虚灵之刃_Tpl,
        Dota2_Pictrue.物品.纷争_Tpl,
        Dota2_Pictrue.物品.红杖_Tpl,
        Dota2_Pictrue.物品.红杖2_Tpl,
        Dota2_Pictrue.物品.红杖3_Tpl,
        Dota2_Pictrue.物品.红杖4_Tpl,
        Dota2_Pictrue.物品.红杖5_Tpl,
    };

    /// <summary>跳刀全 4 属性变体 — 斧王/大鱼人/军团 共用 (找到任一即用; sum==1 / ParallelBatch 全按 顺序无关).
    /// 注: 拍拍仅 3 变体(无智力跳刀, 敏捷英雄不出)且 sum 驱动后续 走A+释放, 形态不同, 未并入.</summary>
    public static Template[] 跳刀全变体 => new[]
    {
        Dota2_Pictrue.物品.跳刀_Tpl,
        Dota2_Pictrue.物品.跳刀_力量跳刀_Tpl,
        Dota2_Pictrue.物品.跳刀_智力跳刀_Tpl,
        Dota2_Pictrue.物品.跳刀_敏捷跳刀_Tpl,
    };

    /// <summary>把物品序列转 ParallelBatch (StepMachine UseItem burst) — 天怒/军团 .StepMachine 内复用同一序列常量.</summary>
    public static ParallelBatch AsParallelBatch(this Template[] templates)
        => new(templates.Select(t => (StepCommand)new UseItem(t)).ToArray());
}
#endif
