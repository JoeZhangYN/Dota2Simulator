using System.Windows.Forms;

namespace Dota2Simulator.GameAutomation.Domain.Actuation;

/// <summary>
/// 虚拟按键码。统一取代散落于 Keys / char / ushort / short / uint 之间的裸转换。
/// ToNative() 仅供 Input adapter 在 interop 边界调用。
/// </summary>
public readonly record struct VirtualKey
{
    public ushort Code { get; }

    private VirtualKey(ushort code) => Code = code;

    public static VirtualKey From(Keys key) => new((ushort)key);

    public ushort ToNative() => Code;

    public static readonly VirtualKey Q = From(Keys.Q);
    public static readonly VirtualKey W = From(Keys.W);
    public static readonly VirtualKey E = From(Keys.E);
    public static readonly VirtualKey R = From(Keys.R);
    public static readonly VirtualKey D = From(Keys.D);
    public static readonly VirtualKey F = From(Keys.F);
    public static readonly VirtualKey Space = From(Keys.Space);
}
