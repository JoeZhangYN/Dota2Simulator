// Phase 12 Chunk 1: Hf2Engine 业务化 —— 7 个 HF2_X helper 复制粘贴 → 声明式 Stratagem 表 + key→stratagem 查表 dispatch.
// Phase 19G-1: ctor 扩 3 ports (input/vision/ui) 与 GameSession / LolEngine 装配对称, 通过 AdapterFactory SSOT 共享.
// - 7 helper method 全删 (业务定义迁 Stratagems.cs static readonly).
// - HandleKeyAsync 替原 switch (key → Task.Run(HF2_X)) 为 _bindings 字典查表 → Stratagem.ExecuteAsync(_input).
// - SimEnigo 静态调全消 (走 _input.PressViaEnigo / MouseClickViaEnigo 端口, 后端不变).
#if HF2

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Games.HF2
{
    public sealed class Hf2Engine : IGameEngine
    {
        private readonly IInputExecutor _input;
        private readonly IScreenVision _vision;
        private readonly IUiInvoker _ui;

        public Hf2Engine(IInputExecutor input, IScreenVision vision, IUiInvoker ui)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _vision = vision ?? throw new ArgumentNullException(nameof(vision));
            _ui = ui ?? throw new ArgumentNullException(nameof(ui));
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

        /// <summary>HF2 build dispatch 入口 —— 查表命中即执行 Stratagem, 否则 no-op.</summary>
        public Task HandleKeyAsync(string heroName, KeyEventArgs e)
        {
            _ = heroName; _ = _vision; _ = _ui;
            return _bindings.TryGetValue(e.KeyCode, out Stratagem s)
                ? Task.Run(() => s.ExecuteAsync(_input))
                : Task.CompletedTask;
        }

        public void CancelAll()
        {
            // HF2 stub: Stratagem 执行 fire-and-forget 短任务, 无长 loop 可取消.
        }
    }
}

#endif
