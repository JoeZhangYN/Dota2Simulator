#if DOTA2
namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 智力英雄策略注册——Wave 4 智力组。
/// 每个英雄一行 Register；与 .Strength / .Agility partial 文件零合并冲突。
/// </summary>
public sealed partial class HeroStrategyRegistry
{
    partial void RegisterIntelligence()
    {
        Register(new Heroes.Intelligence.修补匠Strategy());
        // Register(new Heroes.Intelligence.光法Strategy());  // modifier 待修：依赖 e.Modifiers 组合键，暂走旧 switch fallback
        Register(new Heroes.Intelligence.天怒Strategy());
        // Register(new Heroes.Intelligence.墨客Strategy());  // modifier 待修：依赖 e.Modifiers 组合键，暂走旧 switch fallback
        Register(new Heroes.Intelligence.宙斯Strategy());
        Register(new Heroes.Intelligence.巫医Strategy());
        // Register(new Heroes.Intelligence.巫妖Strategy());  // modifier 待修：依赖 e.Modifiers 组合键，暂走旧 switch fallback
        // Register(new Heroes.Intelligence.帕克Strategy());  // modifier 待修：依赖 e.Modifiers 组合键，暂走旧 switch fallback
        Register(new Heroes.Intelligence.骨法Strategy());
        Register(new Heroes.Intelligence.干扰者Strategy());
        Register(new Heroes.Intelligence.黑鸟Strategy());
        Register(new Heroes.Intelligence.谜团Strategy());
        Register(new Heroes.Intelligence.冰女Strategy());
        Register(new Heroes.Intelligence.火女Strategy());
        Register(new Heroes.Intelligence.蓝猫Strategy());
        Register(new Heroes.Intelligence.卡尔Strategy());
        Register(new Heroes.Intelligence.拉席克Strategy());
        Register(new Heroes.Intelligence.术士Strategy());
        Register(new Heroes.Intelligence.暗影萨满Strategy());
        Register(new Heroes.Intelligence.小仙女Strategy());
        Register(new Heroes.Intelligence.炸弹人Strategy());
        Register(new Heroes.Intelligence.神域Strategy());
        Register(new Heroes.Intelligence.莱恩Strategy());
        Register(new Heroes.Intelligence.沉默Strategy());
        Register(new Heroes.Intelligence.戴泽Strategy());
        Register(new Heroes.Intelligence.双头龙Strategy());
        Register(new Heroes.Intelligence.奶绿Strategy());
        Register(new Heroes.Intelligence.女王Strategy());
        Register(new Heroes.Intelligence.蓝胖Strategy());
        Register(new Heroes.Intelligence.祸乱之源Strategy());
        Register(new Heroes.Intelligence.瘟疫法师Strategy());
    }
}
#endif
