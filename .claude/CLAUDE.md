# Dota2Simulator

Windows Forms 游戏自动化框架 | C# .NET 10 | Dota2 / LOL / HF2

## 架构图

```
┌─ UI 层 ──────────────────────────────────────────────────────┐
│  Form2 (WinForms)  →  全局键盘 Hook  →  Hook_KeyDown         │
└──────────────────────────────────────────────────────────────┘
              ↓ 路由到对应游戏
┌─ 游戏逻辑层 ─────────────────────────────────────────────────┐
│  Games/Dota2/Main.cs   →  根据当前英雄增强(name, e)           │
│  Games/LOL/MainClass   →  根据当前英雄增强(name, e)           │
│  Games/HF2/MainClass   →  根据当前英雄增强(name, e)           │
└──────────────────────────────────────────────────────────────┘
              ↓
┌─ Dota2 子系统 ───────────────────────────────────────────────┐
│  Main      →  英雄分发 + 主循环 + 条件委托系统                │
│  Item      →  物品管理 (图片识别 + 假腿切换 + CD检测)         │
│  Skill     →  技能检测 (颜色分析 + CD/状态/法球)              │
│  Dota2_Pictrue →  图片资源懒加载 (物品/技能/Buff/命石)        │
└──────────────────────────────────────────────────────────────┘
              ↓
┌─ 基础设施层 ─────────────────────────────────────────────────┐
│  PictureProcessing/   →  截图 + 图片匹配 + OCR               │
│  KeyboardMouse/       →  键鼠模拟 (多后端驱动)                │
│  Audio/               →  音频攻击检测                         │
│  TTS/                 →  语音播报                             │
└──────────────────────────────────────────────────────────────┘
```

## 快速定位

| 任务 | 文件 |
|------|------|
| 新增/修改英雄逻辑 | `Games/Dota2/Main.cs` → `根据当前英雄增强` |
| 物品使用/假腿切换 | `Games/Dota2/Item.cs` |
| 技能CD/状态检测 | `Games/Dota2/Skill.cs` |
| 图片资源定义 | `Games/Dota2/Dota2_Pictrue.cs` |
| 截图/图片匹配 | `PictureProcessing/ImageProcessingSystem.cs` |
| 键鼠模拟 | `KeyboardMouse/SimKeyBoard.cs` |
| UI窗口/Hook | `Form2.cs` |

## Phase 27A retry 2 显式破坏点 (2026-05-26)

- **单 fluent type 不变量被破坏 (INV5)**: `HeroPlanBuilder.StepMachine(name, sub => ...)` 返 sub-builder `StepMachineSubBuilder` (8 维 fluent API: Initial / Step / OnEntry / OnExit / Local / Probe / WithLockMode / WithTickAlignment), 业务侧调链入 sub 域, 通过闭包结束自动返主 builder. Phase 26 前主 builder 始终保持单 fluent type, 本 Phase 显式破坏.
- **设计理由**: 4 hero 异型表达力刚需 (巫妖 + 骨法 fire-and-forget + 军团经典 switch + 天怒顺序就绪门控), 主 builder 无法保持单 fluent type 同时表达跨 step 共享状态.
- **关联 plan SSOT**: Phase 27A retry 2 `~/.claude/plans/<dream-name>` §7 SSOT 漂移点 + §7.2 DSL 维度增删表 (拆桥矩阵).
- **落点**:
  - `Application/HeroPlan/StepMachineSubBuilder.cs` (新, S2): 8 维 sub-builder + StepDefinitionBuilder.
  - `Application/HeroPlan/HeroPlanBuilder.cs`: 加 `.StepMachine(name, configure)` chain (lastIdx-with 模式同 Phase 26 .WithRefractory).
  - `Application/HeroPlan/HeroPlan.cs`: HeroPlanClause +StepMachineRefId 字段; HeroPlan +StepMachineDefinitions; Apply 注册 InitialStep; DispatchAsync +skill +handle 参数 + 命中后 Task.Run wrap fire-and-forget 调 Runner.
  - `Application/HeroStrategyBase.cs`: OnKeyAsync 透传 _skill + _main.CurrentHandle 给 Plan.DispatchAsync.
  - `Application/HeroLoopHost.cs`: +internal CurrentHandle 出口 (DS0003 文件级白名单内).
- **船长推 Phase 27B+**: SkillEngine helper async polling 范式与 .StepMachine sync RegisterProbe 不兼容, 留 27B+ 独立 epic 处理.

@rules/project.md
