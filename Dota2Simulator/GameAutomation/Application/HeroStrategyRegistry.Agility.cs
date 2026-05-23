#if DOTA2
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 敏捷英雄策略注册。每个英雄一行 Register(...)，对应 Heroes/Agility/{英雄名}Strategy.cs。
/// A4b 切 ctor 注入：所有 Strategy 接 (input, vision) ports。
/// </summary>
public sealed partial class HeroStrategyRegistry
{
    partial void RegisterAgility(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item)
    {
        Register(new Heroes.Agility.小骷髅Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.小黑Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.巨魔Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.幻刺Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.猴子Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.幽鬼Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.影魔Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.TBStrategy(input, vision, skill, item));
        Register(new Heroes.Agility.敌法Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.小鱼人Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.小松鼠Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.火猫Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.拍拍Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.火枪Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.飞机Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.美杜莎Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.虚空Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.血魔Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.赏金Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.电棍Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.露娜Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.大圣Strategy(input, vision, skill, item));
        Register(new Heroes.Agility.剃刀Strategy(input, vision, skill, item));
    }
}
#endif
