// Phase 12 Chunk 1: HF2 战备指令 (Stratagem) 业务概念抽象 + Builder typestate.
// - 替原 7 个 HF2_X helper method (Ctrl + 方向序列 + 可选 Click 复制粘贴) 为声明式 value object.
// - 不变量「起手 Ctrl 必须先按」内化到 Begin() 起点; 业务定义无 Ctrl 字面.
// - 「方向序列 → 终止动作」两阶段分离 (Builder → Stratagem), 中途无 ExecuteAsync, 终止后无 .Up().
// - 后端固定 Enigo (simengio.dll, 绝地潜兵 2 战备菜单输入路径); 走 IInputExecutor.PressViaEnigo / MouseClickViaEnigo.
#if HF2

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.Games.HF2;

/// <summary>
/// 绝地潜兵 2 战备指令 —— 起手 Ctrl + 方向键序列 + 可选 MouseLeftClick 确认。
/// 不可变值对象, 由 <see cref="Begin"/> 起 Builder 链构造.
/// </summary>
public readonly record struct Stratagem(ImmutableArray<Keys> Sequence, bool EndsWithClick)
{
    /// <summary>起 Builder; 内化「Ctrl 起手」, 业务层无须显式写 Ctrl.</summary>
    public static StratagemBuilder Begin() => new();

    /// <summary>顺序执行 Sequence (Enigo 后端单键 Press), 若 EndsWithClick 则末位补 MouseLeft.</summary>
    public Task ExecuteAsync(IInputExecutor input)
    {
        foreach (Keys key in Sequence)
        {
            input.PressViaEnigo(VirtualKey.From(key));
        }

        if (EndsWithClick)
        {
            input.MouseClickViaEnigo(MouseButton.Left);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Stratagem 构造中间态 typestate —— 方向键累加阶段.
/// 终止动作 (<see cref="Click"/> / <see cref="NoClick"/>) 返回 <see cref="Stratagem"/>, 编译期确保「序列 → 终止」单向流转.
/// </summary>
public sealed class StratagemBuilder
{
    private readonly List<Keys> _seq = new() { Keys.Control };

    public StratagemBuilder Up() { _seq.Add(Keys.Up); return this; }
    public StratagemBuilder Down() { _seq.Add(Keys.Down); return this; }
    public StratagemBuilder Left() { _seq.Add(Keys.Left); return this; }
    public StratagemBuilder Right() { _seq.Add(Keys.Right); return this; }

    /// <summary>终止 + 末位附 MouseLeftClick 确认; 返回不可变 Stratagem.</summary>
    public Stratagem Click() => new(_seq.ToImmutableArray(), EndsWithClick: true);

    /// <summary>终止 + 不附 Click; 返回不可变 Stratagem.</summary>
    public Stratagem NoClick() => new(_seq.ToImmutableArray(), EndsWithClick: false);
}

#endif
