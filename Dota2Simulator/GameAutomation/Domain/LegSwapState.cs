#if DOTA2
using System.Collections.Generic;
using System.Windows.Forms;

namespace Dota2Simulator.GameAutomation.Domain;

/// <summary>
/// 技能切假腿配置——按 key (Q/W/E/R/D/F/Z/X/C/V/B/Space) 存 (是否激活, 假腿类型)。
/// Phase 8 C1 从 Games.Dota2.Item.技能切假腿配置 搬入 Domain，去 BC 内嵌。
/// </summary>
public sealed class 技能切假腿配置
{
    public 技能切假腿配置()
    {
        切假腿配置 = new Dictionary<Keys, (bool, string)>
        {
            { Keys.Q, (true, "智力") },
            { Keys.W, (true, "智力") },
            { Keys.E, (true, "智力") },
            { Keys.R, (true, "智力") },
            { Keys.D, (false, "智力") },
            { Keys.F, (false, "智力") },
            { Keys.Z, (false, "智力") },
            { Keys.X, (false, "智力") },
            { Keys.C, (false, "智力") },
            { Keys.V, (false, "智力") },
            { Keys.B, (false, "智力") },
            { Keys.Space, (false, "智力") }
        };
    }

    public Dictionary<Keys, (bool 是否激活, string 假腿类型)> 切假腿配置 { get; }

    public void 修改配置(Keys key, bool 是否激活, string 假腿类型 = "智力")
    {
        if (切假腿配置.ContainsKey(key))
            切假腿配置[key] = (是否激活, 假腿类型);
        else
            切假腿配置.Add(key, (是否激活, 假腿类型));
    }
}

/// <summary>
/// 切假腿子聚合——收编 Item.cs 原 8 个 static 字段（配置 + 假腿按键 + 6 bool flag）。
/// 由 HeroAggregate.LegSwap 持有，跨 Item / Skill / Main / 92 策略 共享。
/// </summary>
public sealed class LegSwapState
{
    public 技能切假腿配置 配置 { get; set; } = new();
    public Keys 假腿按键 { get; set; } = Keys.Escape;
    public bool 存在假腿 { get; set; }
    public bool 条件开启切假腿 { get; set; }
    public bool 条件保持假腿 { get; set; }
    public bool 需要切假腿 { get; set; }
    public bool 条件假腿敏捷 { get; set; }
    public bool 切假腿中 { get; set; }
}
#endif
