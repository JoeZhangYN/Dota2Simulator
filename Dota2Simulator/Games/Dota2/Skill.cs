// Games/Dota2/Skill.cs
// Phase 8 C4 facade：原 1809 行 static class Skill 主体迁 GameAutomation/Application/SkillEngine.cs。
// 本类降级为 thin static facade：
// - 保留 `技能类型` enum（外部 86 文件引用契约，C7 + D1 评估上移 Domain 后删本壳）。
// - 所有 public 方法一行转发到 Common.SkillEngine（C3 已建立的 service locator 模式；D1 删 Common.SkillEngine 字段时本壳一并删）。
// - `重复按键执行间隔阈值` static → SkillEngine 实例字段，本壳暴露 static property 转发以保 Heroes/ ctor 期 `Skill.X = 100` 调用不改（C7 替换为 _skill.X = 100）。
#if DOTA2

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.Games;
using Dota2Simulator.Vision;

namespace Dota2Simulator.Games.Dota2
{
    internal static class Skill
    {
        /// <summary>技能类型枚举（外部 86 文件引用契约，保留在 facade 内）。</summary>
        public enum 技能类型
        {
            图标CD,
            法球,
            状态,
            释放变色,
            QWERDF图标,
            被动技能存在,
            破坏被动技能,
            未学主动技能,
            未学法球技能,
            推荐学习技能
        }

        /// <summary>循环使用技能等待时间——转发到 SkillEngine 实例字段。</summary>
        public static int 重复按键执行间隔阈值
        {
            get => Common.SkillEngine?.重复按键执行间隔阈值 ?? 100;
            set { if (Common.SkillEngine is not null) Common.SkillEngine.重复按键执行间隔阈值 = value; }
        }

        public static Task<bool> 设置当前技能数量()
            => Common.SkillEngine!.设置当前技能数量();

        public static int 获取当前技能数量(in ImageHandle 句柄)
            => Common.SkillEngine!.获取当前技能数量(in 句柄);

        public static bool 判断技能状态(Keys 技能位置, in ImageHandle 句柄, 技能类型 类型 = 技能类型.图标CD)
            => Common.SkillEngine!.判断技能状态(技能位置, in 句柄, 类型);

        public static Color 获取技能释放判断颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
            => SkillEngine.获取技能释放判断颜色(技能位置, in 句柄, 技能数量);

        public static Color 获取技能进入CD判断颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
            => SkillEngine.获取技能进入CD判断颜色(技能位置, in 句柄, 技能数量);

        public static Color 获取QWERDF颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
            => SkillEngine.获取QWERDF颜色(技能位置, in 句柄, 技能数量);

        public static Color 获取法球颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
            => SkillEngine.获取法球颜色(技能位置, in 句柄, 技能数量);

        public static Color 获取状态颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
            => SkillEngine.获取状态颜色(技能位置, in 句柄, 技能数量);

        public static Color 获取被动颜色(Keys 技能位置, in ImageHandle 句柄, int 技能数量 = 4)
            => SkillEngine.获取被动颜色(技能位置, in 句柄, 技能数量);

        public static bool DOTA2判断技能是否CD(Keys 技能位置, in ImageHandle 句柄)
            => Common.SkillEngine!.DOTA2判断技能是否CD(技能位置, in 句柄);

        public static bool DOTA2释放CD就绪技能(Keys 技能位置, in ImageHandle 句柄)
            => Common.SkillEngine!.DOTA2释放CD就绪技能(技能位置, in 句柄);

        public static bool DOTA2判断状态技能是否启动(Keys 技能位置, in ImageHandle 句柄)
            => Common.SkillEngine!.DOTA2判断状态技能是否启动(技能位置, in 句柄);

        public static bool DOTA2判断是否持续施法(in ImageHandle 句柄)
            => SkillEngine.DOTA2判断是否持续施法(in 句柄);

        public static bool DOTA2获取单个释放技能前颜色(Keys 技能位置, in ImageHandle 句柄)
            => Common.SkillEngine!.DOTA2获取单个释放技能前颜色(技能位置, in 句柄);

        public static bool DOTA2获取所有释放技能前颜色(in ImageHandle 句柄)
            => Common.SkillEngine!.DOTA2获取所有释放技能前颜色(in 句柄);

        public static void 重置所有技能判断()
            => Common.SkillEngine!.重置所有技能判断();

        public static Task<bool> 主动技能释放后续(Keys skill, Action afterAction)
            => Common.SkillEngine!.主动技能释放后续(skill, afterAction);

        public static Task<bool> 主动技能进入CD后续(Keys skill, Action afterAction)
            => Common.SkillEngine!.主动技能进入CD后续(skill, afterAction);

        public static Task<bool> 主动技能已就绪后续(Keys skill, Action afterAction)
            => Common.SkillEngine!.主动技能已就绪后续(skill, afterAction);

        public static Task<bool> 法球技能进入CD后续(Keys skill, Action afterAction)
            => Common.SkillEngine!.法球技能进入CD后续(skill, afterAction);

        public static Task<bool> 状态技能启动后续(Keys skill, Action afterAction)
            => Common.SkillEngine!.状态技能启动后续(skill, afterAction);

        public static void 清除技能时间记录(Keys skill)
            => Common.SkillEngine!.清除技能时间记录(skill);

        public static void 清除所有时间记录()
            => Common.SkillEngine!.清除所有时间记录();

        public static Task<bool> 技能通用判断(Keys 技能键, int 判断模式, bool 是否接按键 = true, Keys 要接的按键 = Keys.A,
            int 判断成功后延时 = 0)
            => Common.SkillEngine!.技能通用判断(技能键, 判断模式, 是否接按键, 要接的按键, 判断成功后延时);

        public static Task<bool> 释放技能后替换图标技能后续(Keys key, Func<int> 获取全局步骤, Action<int> 设置全局步骤)
            => Common.SkillEngine!.释放技能后替换图标技能后续(key, 获取全局步骤, 设置全局步骤);

        public static void 通用技能后续动作(bool 是否接按键 = true, Keys 要接的按键 = Keys.A, int 等待的延迟 = 0, bool 是否保持假腿 = true)
            => Common.SkillEngine!.通用技能后续动作(是否接按键, 要接的按键, 等待的延迟, 是否保持假腿);

        public static void 后续切回假腿(bool 是否接按键 = true, Keys 要接的按键 = Keys.A, int 等待的延迟 = 0)
            => Common.SkillEngine!.后续切回假腿(是否接按键, 要接的按键, 等待的延迟);

        public static Task 测试方法(int x, int y)
            => Common.SkillEngine!.测试方法(x, y);

        public static Task 捕捉颜色()
            => Common.SkillEngine!.捕捉颜色();
    }
}

#endif
