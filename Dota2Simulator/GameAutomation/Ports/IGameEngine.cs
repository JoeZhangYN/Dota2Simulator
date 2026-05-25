// Phase 12 Chunk 2: 三引擎统一入站端口.
// DOTA2 (GameSession) / LOL (LolEngine) / HF2 (Hf2Engine) 实现同一接口, Form2 dispatch 一行无 #if 分支.
// 跨 build 编译 (无 #if guard) —— 三 build 都需此接口存在.

using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.GameAutomation.Ports;

/// <summary>
/// 游戏引擎入站端口 —— Form2 → 三家具体引擎统一调用面.
///
/// 内化「(heroName, KeyEventArgs) → 业务分发」业务概念; 调用站点无 game 分支适配.
/// 各实现内部按自身业务需求转换 WinForms KeyEventArgs (e.g. GameSession 内转 KeyTrigger 领域类型).
/// </summary>
public interface IGameEngine
{
    /// <summary>处理一次按键事件; 各 game 按自身业务分发 (DOTA2 走 Strategy / HF2 走 Stratagem 查表 / LOL 当前 stub).</summary>
    Task HandleKeyAsync(string heroName, KeyEventArgs e);

    /// <summary>取消所有进行中的自动化逻辑.</summary>
    void CancelAll();
}
