// Phase 11 P12: LOL MainClass static → LolEngine instance 化 (同 Silt P6 / HeroLoopHost 模板, 但 body 仍 stub).
// - class MainClass (static) → class LolEngine (sealed) + ctor 接 3 ports (IInputExecutor/IScreenVision/IUiInvoker).
// - public static 根据当前英雄增强 → public instance method (Form2 改 _lolEngine.X 调用).
// - body 仍为 Phase 11 P11 stub (return Task.CompletedTask + 注释保留原 8 method 名+case 骨架),
//   LOL 子系统未来真业务实现时按 SiltEngine 模板填 _input/_vision/_ui 调用 + 加 ItemEngine port (如需).
// - ports 暂未使用, 加 #pragma IDE0052 压.
// Form2 LOL build 下走无参 ctor — Phase 11 P12 同 commit Form2 加 #if LOL 字段 + new + dispatch 切.
#if LOL

using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Vision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.Games.LOL
{
    public sealed class LolEngine
    {
#pragma warning disable IDE0052 // P12 stub: ports 已就位待 LolEngine 真业务实现时填.
        private readonly IInputExecutor _input;
        private readonly IScreenVision _vision;
        private readonly IUiInvoker _ui;
#pragma warning restore IDE0052

        public LolEngine(IInputExecutor input, IScreenVision vision, IUiInvoker ui)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _vision = vision ?? throw new ArgumentNullException(nameof(vision));
            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        }

        /// LOL 最高画质 1920 * 1080
        private const int 截图模式1X = 647;
        private const int 截图模式1Y = 941;
        private const int 截图模式1W = 642;
        private const int 截图模式1H = 130;
        private const int 等待延迟 = 6;

        // 根据截图模式不同，用于判断技能 和 物品的具体x, y值随之变化
        private const int 坐标偏移x = 647;
        private const int 坐标偏移y = 941;

        public Task 根据当前英雄增强(string name, KeyEventArgs e)
        {
            // Phase 11 P12 stub: 英雄分发骨架 (原 case "魔腾" / "男枪") 保留为注释 + 整体 no-op,
            // 等真业务实现时按 SiltEngine 模板 ctor 注入 ItemEngine (如需) + 填充 _input/_vision 调用.
            _ = name; _ = e;
            return Task.CompletedTask;
        }

        #region LOL具体实现 (Phase 11 P11+P12 stub: body 全注释; 等 LolEngine 真业务实现时填)

        // 原方法 (Phase 11 P11 stub 化, P12 instance 化保留骨架):
        //   private async Task<bool> 梦魇之径接平A(ImageHandle 句柄)
        //   private async Task<bool> 无言恐惧接梦魇之径(ImageHandle 句柄)
        //   private async Task<bool> 鬼影重重接无言恐惧(ImageHandle 句柄)
        //   private async Task<bool> 重复释放无言恐惧(ImageHandle 句柄)
        //   private async Task<bool> 穷途末路接平A(ImageHandle 句柄)
        //   private async Task<bool> 烟雾弹接平A(ImageHandle 句柄)
        //   private async Task<bool> 快速拔枪接平A(ImageHandle 句柄)
        //   private async Task<bool> 终极爆弹接平A(ImageHandle 句柄)
        // (依赖 Common.SimKeyBoard / Common.ImageProcessing 等 facade — 已在 Phase 11.A 真删。
        //  真业务实现时经 _input/_vision/_ui 调用; 按 SiltEngine 模板填.)

        #endregion
    }
}

#endif
