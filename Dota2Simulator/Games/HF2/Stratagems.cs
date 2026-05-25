// Phase 12 Chunk 1: HF2 战备指令声明表 —— 7 个原 HF2_X helper 退化为声明式 Stratagem.
// 业务定义即注册, 0 复制粘贴; 起手 Ctrl 由 Stratagem.Begin() 内化, 业务层不见 Ctrl 字面.
#if HF2

namespace Dota2Simulator.Games.HF2;

/// <summary>绝地潜兵 2 战备指令静态表 —— 与 Hf2Engine._bindings 配对完成 (key → Stratagem) dispatch.</summary>
public static class Stratagems
{
    public static readonly Stratagem 补给         = Stratagem.Begin().Up().Right().Click();
    public static readonly Stratagem 救援         = Stratagem.Begin().Up().Right().Left().Up().Click();
    public static readonly Stratagem 背包_激光    = Stratagem.Begin().Up().Left().Up().Right().Click();
    public static readonly Stratagem SOS          = Stratagem.Begin().Up().Down().Right().Up().Click();
    public static readonly Stratagem 飞鹰_110     = Stratagem.Begin().Up().Down().Up().Left().NoClick();
    public static readonly Stratagem 飞鹰_空袭    = Stratagem.Begin().Up().Right().Down().Right().NoClick();
    public static readonly Stratagem 飞鹰_重填装  = Stratagem.Begin().Up().Up().Left().Up().Right().NoClick();
}

#endif
