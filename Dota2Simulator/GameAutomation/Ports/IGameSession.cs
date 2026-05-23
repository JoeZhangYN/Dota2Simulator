using System.Threading.Tasks;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 入站端口：UI 驱动领域的入口。取代 Form2 直接调用 Main.根据当前英雄增强。
/// </summary>
public interface IGameSession
{
    /// <summary>把一次按键触发分发给指定英雄的逻辑。</summary>
    Task DispatchAsync(HeroId hero, KeyTrigger trigger);

    /// <summary>取消所有进行中的自动化逻辑。</summary>
    void CancelAll();

    /// <summary>Phase 8 C3: 会话级暂停标志。
    /// 转发到内部 SessionState，所有读写经此 port——取代 Main._中断条件 static。</summary>
    bool IsPaused { get; set; }
}
