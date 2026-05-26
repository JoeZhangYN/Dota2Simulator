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

@rules/project.md
