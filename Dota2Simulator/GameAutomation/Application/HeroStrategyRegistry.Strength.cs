using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Application;

public sealed partial class HeroStrategyRegistry
{
    partial void RegisterStrength(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
#if DOTA2
        Register(new Heroes.Strength.大牛Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.发条Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.尸王Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.伐木机Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.全能Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.军团Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.骷髅王Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.人马Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.哈斯卡Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.小狗Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.土猫Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.孽主Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.小小Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.海民Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.屠夫Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.斧王Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.大鱼人Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.斯温Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.船长Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.夜魔Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.树精Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.混沌Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.马尔斯Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.破晓晨星Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.钢背Strategy(input, vision, skill, item, main));
        Register(new Heroes.Strength.龙骑Strategy(input, vision, skill, item, main));
#endif
    }
}
