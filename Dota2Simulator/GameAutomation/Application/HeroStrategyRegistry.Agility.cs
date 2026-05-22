#if DOTA2
namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 敏捷英雄策略注册——Wave 4 敏捷组填充。
/// 每个英雄一行 Register(...)，对应 Heroes/Agility/{英雄名}Strategy.cs。
/// </summary>
public sealed partial class HeroStrategyRegistry
{
    partial void RegisterAgility()
    {
        Register(new Heroes.Agility.小骷髅Strategy());
        Register(new Heroes.Agility.小黑Strategy());
        Register(new Heroes.Agility.巨魔Strategy());
        Register(new Heroes.Agility.幻刺Strategy());
        Register(new Heroes.Agility.猴子Strategy());
        Register(new Heroes.Agility.幽鬼Strategy());
        Register(new Heroes.Agility.影魔Strategy());
        Register(new Heroes.Agility.TBStrategy());
        Register(new Heroes.Agility.敌法Strategy());
        Register(new Heroes.Agility.小鱼人Strategy());
        Register(new Heroes.Agility.小松鼠Strategy());
        Register(new Heroes.Agility.火猫Strategy());
        Register(new Heroes.Agility.拍拍Strategy());
        Register(new Heroes.Agility.火枪Strategy());
        Register(new Heroes.Agility.飞机Strategy());
        Register(new Heroes.Agility.美杜莎Strategy());
        Register(new Heroes.Agility.虚空Strategy());
        Register(new Heroes.Agility.血魔Strategy());
        Register(new Heroes.Agility.赏金Strategy());
        Register(new Heroes.Agility.电棍Strategy());
        Register(new Heroes.Agility.露娜Strategy());
        Register(new Heroes.Agility.大圣Strategy());
        Register(new Heroes.Agility.剃刀Strategy());
    }
}
#endif
