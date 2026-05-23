#if DOTA2
using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 单个英雄的自动化策略——取代 Main.根据当前英雄增强 里一个 case "英雄名" 分支：
/// <see cref="OnActivate"/> 对应 case 的 if(!_总循环条件) 注册块，
/// <see cref="OnKeyAsync"/> 对应 case 的 switch(e.KeyCode) 块。
/// </summary>
public interface IHeroStrategy
{
    /// <summary>该策略对应的英雄标识。</summary>
    HeroId Hero { get; }

    /// <summary>英雄首次激活时调用——注册条件委托、配置假腿等。</summary>
    void OnActivate(HeroContext ctx);

    /// <summary>每次按键时调用——把按键映射为条件激活、技能释放等。</summary>
    Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx);
}

#endif
