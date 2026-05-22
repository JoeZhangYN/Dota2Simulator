namespace Dota2Simulator.GameAutomation.Application;

public sealed partial class HeroStrategyRegistry
{
    partial void RegisterStrength()
    {
#if DOTA2
        Register(new Heroes.Strength.大牛Strategy());
        Register(new Heroes.Strength.发条Strategy());
        Register(new Heroes.Strength.尸王Strategy());
        Register(new Heroes.Strength.伐木机Strategy());
        Register(new Heroes.Strength.全能Strategy());
        Register(new Heroes.Strength.军团Strategy());
        Register(new Heroes.Strength.骷髅王Strategy());
        Register(new Heroes.Strength.人马Strategy());
        Register(new Heroes.Strength.哈斯卡Strategy());
        Register(new Heroes.Strength.小狗Strategy());
        Register(new Heroes.Strength.土猫Strategy());
        Register(new Heroes.Strength.孽主Strategy());
        Register(new Heroes.Strength.小小Strategy());
        Register(new Heroes.Strength.海民Strategy());
        Register(new Heroes.Strength.屠夫Strategy());
        Register(new Heroes.Strength.斧王Strategy());
        Register(new Heroes.Strength.大鱼人Strategy());
        Register(new Heroes.Strength.斯温Strategy());
        Register(new Heroes.Strength.船长Strategy());
        Register(new Heroes.Strength.夜魔Strategy());
        Register(new Heroes.Strength.树精Strategy());
        Register(new Heroes.Strength.混沌Strategy());
        Register(new Heroes.Strength.马尔斯Strategy());
        Register(new Heroes.Strength.破晓晨星Strategy());
        Register(new Heroes.Strength.钢背Strategy());
        Register(new Heroes.Strength.龙骑Strategy());
#endif
    }
}
