// Games/Dota2/Main.cs
// Phase 9 C 降级 thin facade: 主循环 / 截屏 / 取消所有功能 / 走A / 获取指定位置颜色 全迁 GameAutomation/Application/HeroLoopHost.cs。
// 本文件保留：
//   - _聚合 / _session 共享状态字段（Heroes/ 92 策略 + ItemEngine/SkillEngine 仍引用，D 阶段切 _main.X 后 F 删）。
//   - 按键匹配条件更新 Dict（ItemEngine.cs:103 仍引用，D 阶段切 _main.按键匹配条件更新 后 F 删）。
//   - 5 facade 方法转发 Common.HeroLoopHost.X（Form2 + GameSession + SkillEngine + ItemEngine 内部 6 处仍引用）。
#if DOTA2

using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain;
using Dota2Simulator.GameAutomation.Domain.Combat;
using Dota2Simulator.Vision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.Games.Dota2
{
    internal class Main
    {
        #region 共享状态字段（C 阶段保留 facade，D 阶段切调用方，F 删）

        /// <summary>
        /// 按键 → 条件激活映射。ItemEngine.cs:103 仍引用，D 阶段切 _main.按键匹配条件更新。
        /// 闭包捕获 _聚合，与 HeroLoopHost 实例 Dict 行为一致。
        /// </summary>
        public static readonly Dictionary<Keys, Action> 按键匹配条件更新 = new()
        {
            { Keys.Z, () => _聚合.Conditions[ConditionSlotKey.Z].Active = true },
            { Keys.X, () => _聚合.Conditions[ConditionSlotKey.X].Active = true },
            { Keys.C, () => _聚合.Conditions[ConditionSlotKey.C].Active = true },
            { Keys.V, () => _聚合.Conditions[ConditionSlotKey.V].Active = true },
            { Keys.B, () => _聚合.Conditions[ConditionSlotKey.B].Active = true },
            { Keys.Space, () => _聚合.Conditions[ConditionSlotKey.Space].Active = true }
        };

        /// <summary>
        /// Phase 8 C3 引入 SessionState 桥；Phase 9 C 后 HeroLoopHost 已 ctor 注入 SessionState，
        /// 本字段仅供 ItemEngine.cs:61/62 + Heroes/ 残留站点用，D 阶段切 _main / Common.HeroLoopHost.Session 后 F 删。
        /// </summary>
        internal static SessionState? _session;

        /// <summary>
        /// 当前英雄的运行态聚合（技能槽 + 条件槽 + 攻击计时 + LegSwap + HasAghanim / HasShard / SkillCount）。
        /// AppContainer ctor 内 `new HeroAggregate(Vision)` 单阶段构造（Phase 8 C6）。
        /// Heroes/ 92 策略 + Form2 + GameSession + SkillEngine + ItemEngine + HeroLoopHost 共享，D 阶段切 _main.Aggregate / _aggregate 后 F 删。
        /// </summary>
        public static HeroAggregate _聚合 = null!;

        #endregion

        #region facade 转发 Common.HeroLoopHost（Phase 9 C 过渡，F 删）

        public static Task<Color> 获取指定位置颜色(int x, int y)
            => Common.HeroLoopHost!.获取指定位置颜色(x, y);

        public static Color 获取指定位置颜色(int x, int y, in ImageHandle 句柄)
            => Common.HeroLoopHost!.获取指定位置颜色(x, y, in 句柄);

        public static bool 获取图片_2()
            => Common.HeroLoopHost!.获取图片_2();

        public static void 取消所有功能()
            => Common.HeroLoopHost!.取消所有功能();

        public static Task 状态初始化()
            => Common.HeroLoopHost!.状态初始化();

        #endregion
    }
}

#endif
