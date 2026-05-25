// Phase 12 Chunk 1: Hf2Engine 业务化 —— 7 个 HF2_X helper 复制粘贴 → 声明式 Stratagem 表 + key→stratagem 查表 dispatch.
// - 7 helper method 全删 (业务定义迁 Stratagems.cs static readonly).
// - HandleKeyAsync 替原 switch (key → Task.Run(HF2_X)) 为 _bindings 字典查表 → Stratagem.ExecuteAsync(_input).
// - ctor 仅留 IInputExecutor (vision/ui 未用 stub 残留, 按「业务需要啥要啥」简化, 消 IDE0052 pragma).
// - SimEnigo 静态调全消 (走 _input.PressViaEnigo / MouseClickViaEnigo 端口, 后端不变).
#if HF2

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Games.HF2
{
    public sealed class Hf2Engine
    {
        private readonly IInputExecutor _input;

        public Hf2Engine(IInputExecutor input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }

        /// <summary>key → Stratagem 静态绑定; 业务扩展只动数据.</summary>
        private static readonly IReadOnlyDictionary<Keys, Stratagem> _bindings = new Dictionary<Keys, Stratagem>
        {
            [Keys.NumPad1] = Stratagems.补给,
            [Keys.NumPad2] = Stratagems.救援,
            [Keys.NumPad3] = Stratagems.飞鹰_空袭,
            [Keys.NumPad5] = Stratagems.飞鹰_110,
            [Keys.NumPad6] = Stratagems.飞鹰_重填装,
        };

        /// <summary>
        /// HF2 build dispatch 入口 —— 查表命中即执行 Stratagem, 否则 no-op.
        /// 名字保留 (Chunk 2 IGameEngine 统一 API 时再改 HandleKeyAsync).
        /// </summary>
        public Task 根据当前英雄增强(string name, KeyEventArgs e)
        {
            _ = name;
            return _bindings.TryGetValue(e.KeyCode, out Stratagem s)
                ? Task.Run(() => s.ExecuteAsync(_input))
                : Task.CompletedTask;
        }
    }
}

#endif
