#if DOTA2
namespace Dota2Simulator.GameAutomation.Application;

/// <summary>全才属性英雄 + 特殊 case 的策略注册（Wave 4 全才组）。</summary>
public sealed partial class HeroStrategyRegistry
{
    partial void RegisterUniversal()
    {
        Register(new Heroes.Universal.剧毒Strategy());
        Register(new Heroes.Universal.猛犸Strategy());
        Register(new Heroes.Universal.狼人Strategy());
        Register(new Heroes.Universal.紫猫Strategy());
        Register(new Heroes.Universal.VSStrategy());
        Register(new Heroes.Universal.小精灵Strategy());
        Register(new Heroes.Universal.马西Strategy());
        Register(new Heroes.Universal.小强Strategy());
        Register(new Heroes.Universal.沙王Strategy());
        Register(new Heroes.Universal.进化岛Strategy());
        Register(new Heroes.Universal.命运2Strategy());
        Register(new Heroes.Universal.测试Strategy());
    }
}
#endif
