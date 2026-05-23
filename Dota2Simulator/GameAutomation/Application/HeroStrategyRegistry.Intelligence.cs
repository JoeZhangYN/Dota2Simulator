#if DOTA2
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 智力英雄策略注册——Wave 4 智力组。
/// 每个英雄一行 Register；与 .Strength / .Agility partial 文件零合并冲突。
/// </summary>
public sealed partial class HeroStrategyRegistry
{
    partial void RegisterIntelligence(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item)
    {
        Register(new Heroes.Intelligence.修补匠Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.光法Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.天怒Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.墨客Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.宙斯Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.巫医Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.巫妖Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.帕克Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.骨法Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.干扰者Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.黑鸟Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.谜团Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.冰女Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.火女Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.蓝猫Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.卡尔Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.拉席克Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.术士Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.暗影萨满Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.小仙女Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.炸弹人Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.神域Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.莱恩Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.沉默Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.戴泽Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.双头龙Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.奶绿Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.女王Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.蓝胖Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.祸乱之源Strategy(input, vision, skill, item));
        Register(new Heroes.Intelligence.瘟疫法师Strategy(input, vision, skill, item));
    }
}
#endif
