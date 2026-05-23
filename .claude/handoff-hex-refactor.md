# Handoff: Dota2Simulator 完整六边形架构重写

## Epic 概要
C# .NET 10 WinForms 游戏自动化框架，重写为六边形架构。
- Phase 4 plan SSOT: `C:\Users\JoeZhang\.claude\plans\noble-percolating-hejlsberg.md`
- Phase 6 plan SSOT: `C:\Users\JoeZhang\.claude\plans\shimmying-strolling-starfish.md`
- Phase 8 plan SSOT: `C:\Users\JoeZhang\.claude\plans\sequential-nibbling-lightning.md`

## 进度
- [x] Phase 1-3（commit `11d51e8` → `3ea4129`）
- [x] **Phase 4**（= 原 Phase 4 + Phase 5 合并）—— 2026-05-22 完成（commit `744ed12`），用户已冒烟验证
- [x] **Phase 6 + 6.5**（hex 闭环 + 物理重组）—— 2026-05-23 完成（15 chunk，A1-A6 + B1-B5 + C1）
- [x] **Phase 7**（IUiInvoker port + 单例终结）—— 2026-05-23 完成（5 chunk，D1-D5）
- [x] **Phase 8 段落性收尾**（现代化全量解耦 8/10）—— 2026-05-23 完成（A + C1 + C3 + C4 + C5 + C6 + C7 + D1；HeroLoopHost 推迟 Phase 9）

## Phase 6 + 6.5 已完成（main 分支连续 commit，每 chunk 0 build 错误）

### 链 A — Hex 闭环（92 策略类切 port + CompositionRoot 实例化）
- `990c61f` A1 AppContainer 雏形 + Form2 构造器注入
- `de52f6c` A2 删 AppComposition.cs
- `819e2c0` A3 Port 扩 + ScreenRegion 值对象
- `be5f99d` / `7c36d68` / `401d660` / `050d567` A4a-d 92 策略 ctor 注入
- `5fae140` A5 ConditionSlotSet 切 vision port + HeroAggregate.Init 双阶段（C6 后单阶段）
- `321c47b` A6 AppContainer 装饰 Probe ports

### 链 B — 物理重组
- `dc04afb` B1 Vision/ 子目录搬移；`7a81921` B2；`db04ef7` B3 god class 拆分；`1ddc6a6` B4 Logger；`71524ec` B5 Infrastructure 骨架；`9dd7319` C1 死代码标 TODO

## Phase 7 已完成
IUiInvoker port + 25 站点切走 Common.Main_Form 单例 + Form2.Instance 删除。
- `9919cef` D1 / `8a27111` D2 / `3c28398` D3 / `14816bb` D4 / `65470d1` D5

Backup 基础设施：`D:\backup\C\temp\JoezhangYN\C#\Dota2Plus\Dota2Simulator.git` bare + post-commit hook 自动 push。

## Phase 8 段落性收尾（2026-05-23）

**Epic 目标**：现代化全量解耦 = 0 service locator + 0 可变状态 static God class + 0 单例。

**plan SSOT**：`C:\Users\JoeZhang\.claude\plans\sequential-nibbling-lightning.md`

**已完成 chunk（8/10，HeroLoopHost + 真删 facade 推迟 Phase 9）**：

- `5232092` **A** 拆 Invoke 写场景异步化（SetText → BeginInvoke；用户判定 TTS 抢跑无碍）
- `d82a2ee` **C1** 抽 LegSwapState 子聚合（Item.cs 8 字段 + 内嵌 `技能切假腿配置` 迁 Domain；HeroAggregate 加 LegSwap 属性；47 文件批替换）
- `3477f25` **C3** 抽 SessionState 单例 + 4 字段迁 HeroAggregate（74 处跨 BC 替换 / 34 文件）：
  - SessionState 单例（含 Phase 9 IHeroIdentity 占位接口），AppContainer 装配
  - IGameSession.IsPaused 转发；GameSession ctor 接 SessionState
  - Main._session static field 桥接（C3 过渡 service locator，HeroLoopHost 实例化后删）
  - Skill._技能数量 → HeroAggregate.SkillCount (19 处) / Item._是否魔晶/神杖 → HasShard/HasAghanim (44 处) / Main._中断条件 → SessionState.IsPaused (11 处)
- `e812dc6` **C4** SkillEngine 实例化（Skill 1809 行 → SkillEngine + Skill thin facade）：
  - 新建 `GameAutomation/Application/SkillEngine.cs`（1820 行）= 可变状态字典/锁/时间/缓存/重复按键阈值 instance 化；ctor 接 4 ports (input/vision/ui/aggregate)
  - 内部 5 处 SimKeyBoard → _input；13 处 Common.UiInvoker → _ui；8 处 GlobalScreenCapture → _vision；Main._聚合 → _aggregate
  - 纯只读查找表保留 static：技能4/5/6、StringBuilderPool*、ColorMatcher 嵌套类、ColorConfig、纯 helper
  - Skill.cs 降级 thin facade（150 行）：保留 `技能类型` enum + 重复按键执行间隔阈值 property + 27 转发壳
  - using 技能类型 = Skill.技能类型 type alias 避免 57 处 enum 引用改动
- `43058e4` **C5** ItemEngine 实例化（Item.cs 774 行 → ItemEngine + Item thin facade，同 C4 模板）：
  - 4 ports ctor；4 处 SimKeyBoard (KeyPress→Press/KeyPressAlt→ComboAlt/KeyPressWhile→ComboWhile) → _input；2 处 Common.UiInvoker → _ui；16 处 GlobalScreenCapture → _vision
  - Item.cs 60 行 facade（15 public 方法转发；物品信息/物品4-6 嵌套不暴露 = 外部 0 引用）
  - Silt namespace 跨边界用 fully qualified `Dota2Simulator.Games.Dota2.Silt.Main` 引用
- `ff6a8c1` **C6** HeroAggregate / ConditionSlotSet 单阶段 ctor 注入 vision（删 Init 双阶段）：
  - HeroAggregate ctor 改 `(IScreenVision vision)`；内部 `Conditions = new ConditionSlotSet(vision)`
  - ConditionSlotSet ctor 改 `(IScreenVision vision)` 单阶段，删 Init / GetCurrentFrame fallback
  - Main._聚合 `static readonly = new()` → `static = null!;`（不再类型加载期实例化）
  - AppContainer ctor 内 Vision 装好立即 `Main._聚合 = new HeroAggregate(Vision)`
  - **HeroLoopHost (Games.Dota2.Main 823 行) 实例化推迟到 Phase 9**：涉及 _聚合/_session/坐标偏移/按键匹配条件更新/取消所有功能/获取图片_2/状态初始化/获取指定位置颜色 等大量外部依赖（Form2 + GameSession + Strategy 内引用），且类型加载顺序风险高
- `8ef57a1` **C7** 92 策略 ctor 扩 +SkillEngine +ItemEngine + Heroes/ 1391 处替换：
  - 92 Strategy ctor 扩参；普通 (input,vision,skill,item)；测试 (input,vision,skill,item,ui)
  - Heroes/ 86 文件批替换：Skill.X → _skill.X (331 处，保留 Skill.技能类型 enum 契约)；Item.X → _item.X (60 处)
  - 40 文件内 `private static 条件委托方法` → instance；3 文件内 static 本地函数去 static
  - 静态成员调用类型名限定：SkillEngine.DOTA2判断是否持续施法、ItemEngine.获取物品范围/获取中立TP范围
  - HeroStrategyRegistry ctor + 4 partial 扩参；AppContainer.BindUi 装配
  - 可见性调整：SkillEngine/ItemEngine internal sealed → public sealed；Skill facade internal static → public static (CS0051 修复)
- **D1**（本 chunk，handoff 文档化）段落性收尾：
  - grep verify Heroes/ 内 0 `Skill.X / Item.X` 直调（除 Skill.技能类型 enum）✅
  - handoff 文件更新记录 Phase 8 段落性收尾 + Phase 9 起手指引

## Phase 8 未达成完成形态（推迟 Phase 9+）

- ❌ **0 service locator**：Common.UiInvoker / Common.SkillEngine / Common.ItemEngine 仍存
  - BC 内 Main.cs / Silt / Skill facade / Item facade 仍调用
- ❌ **0 单例**：Main._聚合 仍是 static field；HeroLoopHost (Games.Dota2.Main) 未实例化
- ❌ **0 可变状态 static God class**：Main.cs 内 _总循环条件 / _缓存图像句柄 / _initialData 等仍 static
- ❌ Skill / Item facade 仍保留（BC 内 Main.cs 3 处 + Form2 + GameSession 仍用）
- ❌ IScreenVision.GetCurrentFrame() [Obsolete] 仍保留（SkillEngine / ItemEngine instance 内仍调用，待 ConditionDelegateBitmap 签名改）

## Phase 8 关键不变量

1. 每 chunk 单 commit + dotnet build -c Debug 0 错误 + 自动 backup push（验证 `.git/_unbacked_commits.log` empty）✅
2. **HeroAggregate.LegSwap / SkillCount / HasAghanim / HasShard 是 BC 跨边界共享状态的 SSOT**（C1+C3 起）
3. **SessionState 单例是会话级共享状态的 SSOT**（C3 起）—— Phase 9 HeroIdentity epic 在此挂接
4. **0 可变状态 static God class** 解读：纯只读查找表（ColorMatcher / RSquared / 物品4/5/6 / 技能4/5/6 / Rectangle 常量）保留 static；可变状态全压 HeroAggregate / SessionState ✅ in C4/C5
5. SetText 已异步（BeginInvoke）；调用方紧跟 TTS 用户接受抢跑

## Phase 9 epic 占位（C3 引入）

- `GameAutomation/Application/SessionState.cs` 内 `IHeroIdentity` 接口：F1 触发提取 HUD 英雄名区域 → 像素模板多帧一致性投票 → 稳态固化作"自己英雄"绑定；所有 HUD 读取经身份 gate，看别人状态时返回上一稳态快照避免误触

## 待用户冒烟（Phase 8 段落性收尾）

所有 chunk build 0 错误，但**功能未经实战**。建议同 Phase 6 冒烟列表：
1. 启动程序（管理员）
2. 抽样英雄按全部技能键：大牛/屠夫/海民/斧王/马尔斯（Strength）/ 影魔/敌法/小黑/猴子/幽鬼（Agility）/ 光法/卡尔/暗影萨满/莱恩/双头龙（Intelligence）/ 猛犸/狼人/剧毒/进化岛/命运2（Universal）
3. **C4 关键回归**：所有英雄技能 CD 判断 / 状态判断 / 释放变色判断（SkillEngine 实例化）
4. **C5 关键回归**：物品使用 + 切假腿 + Esc 暂停 + F1 重置耗蓝物品
5. **C6 关键回归**：AppContainer 类型加载顺序（程序启动时 _聚合 = new HeroAggregate(Vision)）；条件委托链路（猛犸震荡波后摇）
6. **C7 关键回归**：92 策略 ctor 扩参后 Registry 装配；任意策略英雄触发 OnActivate / OnKeyAsync 路径

回滚锚点：每 chunk 单 commit，`git revert <hash>` 单步回滚。完整撤回 Phase 8 段落：`git revert 8ef57a1..5232092`（8 commit）。

## 下次 session 起手指引（Phase 9）

1. Read `C:\Users\JoeZhang\.claude\plans\sequential-nibbling-lightning.md` 全文
2. Read 本 handoff 文件 Phase 8 段
3. **起手做 HeroLoopHost 实例化**（Games.Dota2.Main 823 行 → HeroLoopHost.cs，同 C4/C5 模板 + Common.HeroLoopHost service locator 桥）
4. HeroLoopHost 完成后做 **真删收尾**：删 Common.UiInvoker / Common.SkillEngine / Common.ItemEngine / Skill+Item+Main facade + ConditionDelegateBitmap 签名改 + 删 IScreenVision.GetCurrentFrame [Obsolete]

## 后续（Phase 9+，本 epic 外）

- 实际删除死代码（_Legacy/ + Other/ + 4 个标 TODO-dead 文件）
- ConditionDelegateBitmap 签名改 ImageHandle → IScreenVision
- Games.Dota2 BC SimKeyBoard 直调切 port（HeroLoopHost 实例化后自然消解）
- KeyboardMouse/ 4 基础驱动文件评估是否迁 Infrastructure/Native/
- Vision 子 namespace 评估
- LOL/HF2 切 GameSession（统一入站端口）
- **Vision 性能**：候选区域裁剪 + GpuFusedVisionAdapter（GPU PoC 已验证免回传优势）
- **HeroIdentity epic**（用户决策 2026-05-23）：F1 触发 HUD 英雄名提取 + 像素模板多帧投票 + 全 HUD gate
