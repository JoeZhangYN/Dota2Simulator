// Phase 12 Chunk 2: LolEngine 实现 IGameEngine (与 GameSession / Hf2Engine 同入站端口).
// - Body 仍为 stub (Phase 11 P11+P12 决策保留, 真业务实现走 Phase 12+ 单独 chunk).
// - ctor 简化只接 IInputExecutor (vision/ui 未用 stub 残留, 按「业务需要啥要啥」消 IDE0052 pragma; 未来真业务需要时再扩 ctor).
// - 方法名 根据当前英雄增强 → HandleKeyAsync (实现 IGameEngine 接口).
#if LOL

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Games.LOL
{
    public sealed class LolEngine : IGameEngine
    {
        private readonly IInputExecutor _input;

        public LolEngine(IInputExecutor input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }

        /// <summary>
        /// LOL build dispatch 入口 stub. 真业务实现按 SiltEngine 模板填 _input/_vision/_ui 调用 + 加 ItemEngine port (如需).
        /// 原 8 method (梦魇之径接平A / 无言恐惧 / 鬼影重重 / 穷途末路 / 烟雾弹 / 快速拔枪 / 终极爆弹 / 重复释放) Phase 11 P11 已 stub 化, 留 chunk 11.B 后续阶段填.
        /// </summary>
        public Task HandleKeyAsync(string heroName, KeyEventArgs e)
        {
            _ = heroName; _ = e; _ = _input;
            return Task.CompletedTask;
        }

        public void CancelAll()
        {
            // LOL stub: 无聚合状态可清.
        }
    }
}

#endif
