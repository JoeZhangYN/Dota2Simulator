#if DOTA2
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>全才属性英雄 + 特殊 case 的策略注册。</summary>
public sealed partial class HeroStrategyRegistry
{
    partial void RegisterUniversal(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)
    {
        Register(new Heroes.Universal.剧毒Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.猛犸Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.狼人Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.紫猫Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.VSStrategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.小精灵Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.马西Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.小强Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.沙王Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.进化岛Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.命运2Strategy(input, vision, skill, item, main));
        Register(new Heroes.Universal.测试Strategy(input, vision, skill, item, _ui!, main));
    }
}
#endif
