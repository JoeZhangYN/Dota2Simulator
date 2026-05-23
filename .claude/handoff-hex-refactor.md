# Handoff: Dota2Simulator 完整六边形架构重写

## Epic 概要
C# .NET 10 WinForms 游戏自动化框架，重写为六边形架构。
- Phase 4 plan SSOT: `C:\Users\JoeZhang\.claude\plans\noble-percolating-hejlsberg.md`
- Phase 6 plan SSOT: `C:\Users\JoeZhang\.claude\plans\shimmying-strolling-starfish.md`
- Phase 8 plan SSOT: `C:\Users\JoeZhang\.claude\plans\sequential-nibbling-lightning.md`
- Phase 9 plan SSOT: `C:\Users\JoeZhang\.claude\plans\idempotent-brewing-kurzweil.md`
- Phase 10A plan SSOT: `C:\Users\JoeZhang\.claude\plans\generated-glinting-knuth.md`
- Phase 10B plan SSOT: `C:\Users\JoeZhang\.claude\plans\sturdy-bridging-rabin.md`
- Phase 10C plan SSOT: `C:\Users\JoeZhang\.claude\plans\crystal-emitting-knuth.md`

## 进度
- [x] Phase 1-3（commit `11d51e8` → `3ea4129`）
- [x] **Phase 4**（= 原 Phase 4 + Phase 5 合并）—— 2026-05-22 完成（commit `744ed12`），用户已冒烟验证
- [x] **Phase 6 + 6.5**（hex 闭环 + 物理重组）—— 2026-05-23 完成（15 chunk，A1-A6 + B1-B5 + C1）
- [x] **Phase 7**（IUiInvoker port + 单例终结）—— 2026-05-23 完成（5 chunk，D1-D5）
- [x] **Phase 8 段落性收尾**（现代化全量解耦 8/10）—— 2026-05-23 完成（A + C1 + C3 + C4 + C5 + C6 + C7 + D1；HeroLoopHost 推迟 Phase 9）
- [x] **Phase 9 HeroLoopHost 实例化 + 真删收尾**—— 2026-05-23 完成（7 commit，A + B + C + D + D' + E + F；plan SSOT `~/.claude/plans/idempotent-brewing-kurzweil.md`）
- [x] **Phase 10A SG 图片资源改造**—— 2026-05-23 完成（4 chunk，净 +108 行；plan SSOT `~/.claude/plans/generated-glinting-knuth.md`）
- [x] **Phase 10B 6 SOFT_FAIL 消除**—— 2026-05-23 完成（5 chunk，B1-B5；plan SSOT `~/.claude/plans/sturdy-bridging-rabin.md`）
- [x] **Phase 10C Strategy SG 92 样板消除**—— 2026-05-23 完成（5 chunk，S1-S5；plan SSOT `~/.claude/plans/crystal-emitting-knuth.md`，净 -1479 LOC）

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

## Phase 9 完整收尾（2026-05-23）

**plan SSOT**：`C:\Users\JoeZhang\.claude\plans\idempotent-brewing-kurzweil.md`

**用户 grill 三问对齐**：
1. 完成形态：**激进删 Main.cs**（整文件删，非 thin facade）
2. in-scope：**Silt 同步处理**（5 处 Common.UiInvoker 切换）
3. 类型加载顺序：**HeroLoopHost 在 SkillEngine/ItemEngine 之后**（消除 ItemEngine→Main 循环依赖）

**7 commit 主干**：
- `b3087b6` **A** 砍 Main.cs 322 行死代码（命运2 4 + 删除角色族 5 + 测试其他功能 + 旧找色注释；grep 0 外调验证）
- `20918e0` **B** GameLayout 常量类（OffsetX/Y + 截图模式 1/2 Rect）+ 坐标偏移迁移 23 处 + 删 buff/debuff/命石 Rectangle 死代码
- `cd41124` **C** HeroLoopHost 新建（~360 行，7 ports ctor）+ 主循环 / 截屏 / 取消 / 走A / 获取指定位置颜色 全迁；Main.cs 降级 thin facade
- `0dfe24b` **D** 92 策略 ctor 扩参 +HeroLoopHost + Heroes/ 90 文件 1075 处 `Main.X` → `_main.X`（PowerShell .NET File API）
- `06c1b36` **D'** 上层 4 站点 20 处切 Common.HeroLoopHost（Form2 + GameSession + SkillEngine + ItemEngine）
- `6299a7d` **E** Silt 8 处 Common.UiInvoker → Common.HeroLoopHost!.Ui（轻量桥接；Silt instance 化推迟 Phase 11）
- `e854115` **F** 真删 Main.cs / Skill.cs / Item.cs 3 facade（271 行）+ enum 技能类型 上移 SkillEngine + ItemEngine ctor 扩 SkillEngine + AppContainer 重写 + 删 Common.UiInvoker/Common.SkillEngine 字段

## Phase 9 关键不变量

1. 每 chunk 单 commit + dotnet build -c Debug 0 错误 + 自动 backup push ✅
2. **类型加载顺序终态**：Vision → HeroAggregate → SessionState → SkillEngine → ItemEngine(+skill) → HeroLoopHost(+skill,+item) → Registry.RegisterAll
3. **HeroLoopHost 是 BC 业务入口聚合**：原 Main.cs static 全局状态全压实例字段；92 策略经 `_main.X / _main._聚合.X.Y / _main.Session.IsPaused` 访问
4. **Common 残留 2 service locator 字段**（Phase 11 处理）：
   - `Common.ItemEngine`：SkillEngine 反向依赖（SkillEngine 先 new）+ Silt 静态调用
   - `Common.HeroLoopHost`：Form2/GameSession 未 ctor 扩 + Silt 未 instance 化 + ItemEngine 4 处反向调
5. **完成不变量 grep verify 通过**：
   - `Common.UiInvoker` / `Common.SkillEngine` 0 活码引用 ✅
   - `Games/Dota2/{Main,Skill,Item}.cs` 已删 ✅
   - Heroes/ 0 `Main.X / Skill.X / Item.X` 活码 ✅
   - Main.cs 823 行 → 0 ✅
6. 默认 build（`DOTA2 + Silt`）0 错误 ✅

## 待用户冒烟（Phase 9 收尾）

所有 7 chunk build 0 错误，但**功能未经实战**。建议 5 项关键回归：

1. 启动程序（管理员）+ AppContainer 类型加载顺序无 NRE
2. 抽样英雄全技能键：大牛/屠夫/海民/斧王/马尔斯（Strength）/影魔/敌法/小黑/猴子/幽鬼（Agility）/光法/卡尔/暗影萨满/莱恩/双头龙（Intelligence）/猛犸/狼人/剧毒/进化岛/命运2（Universal）
3. **C 关键回归**：HeroLoopHost.状态初始化() / 取消所有功能() / 一般程序循环；猛犸震荡波后摇 / 切假腿 / Esc 暂停 / F1 重置耗蓝物品
4. **D/D' 关键回归**：92 策略 ctor 扩参 Registry 装配；任意策略英雄 OnActivate / OnKeyAsync 路径；Form2 取消所有功能 / 获取图片_2
5. **E 关键回归**：Silt 启用时 UI SetText / GetText 正常

回滚锚点：每 chunk 单 commit。完整撤回 Phase 9：`git revert e854115..b3087b6`（7 commit）。

## 后续（Phase 10+，本 epic 外）

**Phase 10 候选**（详 2026-05-23 audit 报告）：

- **10A 图片资源 SG**（H 优先级）：Dota2_Pictrue 127 手写属性 + csproj 250 行重复 + .resx → C# Source Generator 编译期扫 `Assets/**/*.png` 自动生成 partial class。调用方 API `Dota2_Pictrue.Buff.X` 不变，文件名增删自动重生成 + Roslyn analyzer 缺失检测 + SHA1 资源签名 + 按英雄预加载 hint
- **10B 92 Strategy SG**（H）：`[HeroStrategy("骷髅王", HeroAttribute.Strength)]` attribute + SG 自动生成 ctor + 字段 + Register + Hero——消 ~1500 行样板
- **10C 单元测试基础设施**（H，Phase 10 SG 重构网兜）：xUnit + FakeItEasy + Phase 6 ports mock + 92 策略 smoke test
- **Phase 11 Silt 子 BC 整顿**：Silt.Main + DynamicSkillAutoSelector instance 化 + Form2/GameSession ctor 扩 HeroLoopHost → 删 Common.ItemEngine + Common.HeroLoopHost 终态 0 service locator

**M 优先级现代化**：
- GameLayout 扩 buff/命石区域（6 处 inline 复刻 hoist）
- VirtualKey SG 全集（消 `.From(Keys.X)` 数百处）
- Microsoft.Extensions.DependencyInjection 替手工组合根
- 技能信息 record + named-arg

**P1 bug**：`SystemSpeechAdapter.Speak` 每次 `new SpeechSynthesizer` 不 Dispose（资源 leak，独立修）

**已知 Phase 8 遗留**：
- 实际删除死代码（_Legacy/ + Other/ + 4 个标 TODO-dead 文件）
- ConditionDelegateBitmap 签名改 ImageHandle → IScreenVision
- KeyboardMouse/ 4 基础驱动文件评估迁 Infrastructure/Native/
- Vision 子 namespace 评估
- LOL/HF2 切 GameSession（统一入站端口）
- **Vision 性能**：候选区域裁剪 + GpuFusedVisionAdapter
- **HeroIdentity epic**：F1 触发 HUD 英雄名提取 + 像素模板多帧投票 + 全 HUD gate

## Phase 10A 完整收尾 (2026-05-23)

**plan SSOT**：`C:\Users\JoeZhang\.claude\plans\generated-glinting-knuth.md` (478 行)

**Epic 目标**：Dota2_Pictrue 126 手写懒加载属性 → C# Incremental Source Generator 编译期扫 `Assets/**/*.bmp` 自动生成 partial class；调用方 API `Dota2_Pictrue.Buff.X` 1:1 不变；附带 SHA1 资源签名校验链路 + 按英雄预加载 hint 字典。

**4 commit 主干**：
- `b52cba7` **A** SG 骨架 + hello-world 注入（PictureManifestGenerator IIncrementalGenerator；netstandard2.0；csproj ProjectReference + EmitCompilerGeneratedFiles）
- `54a5747` **B** SG 全量复刻 126 属性 + Dota2_Pictrue.cs 降级 partial 占位（418 → 13 行；33 文件 / 202 处调用方零改动 grep 前后一致；`#if Silt` 语义保留；`_fileStemToPropertyName` 特例 dict + Silt flatten）
- `e98e8a3` **C** SG SHA1 manifest emit + LazyImageLoader RegisterSha1Manifest 校验链路（`_expectedSha1Map` + `Sha1MismatchCount` Interlocked 计数 + `LoadImageCore` SHA1 分叉 + `BitmapToHandle` local helper + `[ModuleInitializer]` 注册；mismatch 非阻断仅 log）
- `7286e17` **D** SG 英雄预加载 hint emit（PictureHeroPreloadGenerator regex `[\w一-鿿]+` 扫 Strategy + `ClassNameToManifestPrefix` 反向映射；emit `PictureHeroPreloadHints.Hints` 29 entry 字典 —— 集成桥半建主项目 0 引用，Phase 10B 候选）

**净行数**：7 files changed / +535 / -427 / 净 +108 行

**关键文件**：

| 文件 | 状态 | 说明 |
|------|------|------|
| `Dota2Simulator.SourceGenerators/Dota2Simulator.SourceGenerators.csproj` | NEW | netstandard2.0 / Microsoft.CodeAnalysis.CSharp 4.11.0 / Microsoft.CodeAnalysis.Analyzers 3.11.0 |
| `Dota2Simulator.SourceGenerators/PictureManifestGenerator.cs` | NEW | 主 SG，5 类映射 + `_fileStemToPropertyName` 特例 dict + Silt flatten + SHA1 emit + ModuleInitializer；含 `DirToClassName` / `FileStemToPropertyName` 两 dict + `#pragma warning disable RS1035`/`CA5350` |
| `Dota2Simulator.SourceGenerators/PictureHeroPreloadGenerator.cs` | NEW | 第二 SG，regex `[\w一-鿿]+` 扫 Strategy 文件 + `ClassNameToManifestPrefix` 反向映射 |
| `Dota2Simulator/Games/Dota2/Dota2_Pictrue.cs` | EDIT | 418 → 13 行降级 partial 占位 |
| `Dota2Simulator/Vision/Cache/LazyImageLoader.cs` | EDIT | +109 行：`_expectedSha1Map` 字段 + `RegisterSha1Manifest` API + `Sha1MismatchCount` 属性 + `LoadImageCore` SHA1 分叉 + `BitmapToHandle` local helper + XML doc R7 语义边界 |
| `Dota2Simulator/Dota2Simulator.csproj` | EDIT | +13 行 ProjectReference + `EmitCompilerGeneratedFiles` + 2 AdditionalFiles |
| `Dota2Simulator/Form2.cs` | EDIT | +3 行 R2 时序 log `[Form2.Load] {DateTime.Now.Ticks}` |

## Phase 10A 关键不变量 (architecture-sentinel 全 PASS)

1. 每 chunk 单 commit + dotnet build 0 错误 + auto backup push ✅
2. **126 ImageHandle 属性签名 1:1 复刻**（B diff verify = 0）✅
3. **33 文件 / 202 处调用方零改动**（B 前后 grep 一致）✅
4. **`#if Silt` 语义保留**：SG 按 define 条件 emit / 调用方 ifdef 边界不变 ✅
5. **SHA1 校验链路非阻塞**：mismatch 仅 log + Interlocked.Increment，不抛 ✅
6. **Phase 9 装配序零侵入**：HeroLoopHost / SkillEngine / ItemEngine / Common 不变；ModuleInitializer 早于 Form2 ctor 调用 ✅（实测仍 untested，待用户冒烟）
7. **0 新增编译警告**：220 = baseline，0 RS1xxx ✅

## Phase 10A architecture-sentinel 6 项 SOFT_FAIL handoff_notes (非阻断)

均作 **Phase 10B 候选**，收尾仅记录不处理：

1. **PreloadHints 集成桥半建**：`PictureHeroPreloadHints.Hints` 29 entry 已 emit 但**主项目 0 引用**。设计预留，Phase 10B 候选接入 `HeroLoopHost.OnHeroChanged` → `await LazyImageLoader.PreloadImagesAsync(PictureHeroPreloadHints.Hints[heroName])`
2. **API 可见性偏宽**：`LazyImageLoader.RegisterSha1Manifest` public，可考虑收紧 internal（SG emit 的 `[ModuleInitializer]` 同 assembly 调用，跨 assembly 无消费方）。当前 public 不影响功能
3. **Form2 ctor 注释 tag 错位**：log 在 ctor 非 Load handler，标签 `[Form2.Load]` 易误导。建议改 tag `[Form2.ctor]` 或把 log 移到 `Form2_Load` event
4. **pragma 豁免未同步 plan**：C chunk 主 SG 内 `#pragma warning disable RS1035`（SG 内 File.ReadAllBytes，bmp 二进制不可 AdditionalText.GetText）+ `#pragma warning disable CA5350`（SHA1 弱加密，完整性校验非密码学，与 plan R7 一致）—— 真豁免但 plan §4 mitigation 未记录
5. **SG 反向映射双 dict**：`PictureHeroPreloadGenerator.ClassNameToManifestPrefix`（Buff→BUFF / 英雄技能→技能 / 播报信息→播报）与 `PictureManifestGenerator.DirToClassName` 反向重复。未来加新顶层目录需同步改两 SG —— Phase 10B 候选 "抽公共常量类供两 SG 共享"
6. **SHA1 字典 Silt 关闭时含死 key**：SG 对 Sha1 字典未按 `#if Silt` 分割，全 126 SHA1 整体 `#if DOTA2` 包裹。Silt define 关闭时 dict 仍含 43 Silt key —— 仅诊断微噪不阻断

## 待用户冒烟（Phase 10A 收尾）

**R2 ModuleInitializer 时序实测**（架构师标 untested，等用户冒烟升级 experiential）：
- 跑 `Dota2Simulator.exe` (admin) 看控制台首屏：
  - `[LazyLoad] SHA1 manifest 已注册 (126 条)` 出现
  - `[ModuleInit] <ticks1> RegisterSha1Manifest 已调用`
  - `[Form2.Load] <ticks2>`
  - **必须** ticks1 < ticks2（ModuleInit 早于 Form2 实例化）
- 失序对策：改 `Dota2_PictrueSha1.RegisterOnModuleInit()` 从 ModuleInitializer 改 `Form2.Load` early 手动调

**功能回归冒烟**（建议同 Phase 9 抽样）：
1. 启动程序（admin）+ AppContainer 类型加载顺序无 NRE
2. 抽样 4 属性英雄全技能键（大牛/影魔/卡尔/猛犸 等，含图片识别路径）
3. 物品使用（假腿切换 / 神杖魔晶判断）
4. SHA1 mismatch 测试（可选）：临时改一张 .bmp，重 build，启动看 stdout `Sha1MismatchCount` 递增

## Phase 10A 回滚锚点

- 单 chunk revert：`git revert <hash>`（A/B/C/D 任一）
- 完整撤回 Phase 10A：`git revert 7286e17 e98e8a3 54a5747 b52cba7`（4 commit 逆序）

## 下次 session 起手指引（Phase 10B / Phase 11 任选）

- **Phase 10B 优先候选**（与 Phase 10A 自然衔接）：
  1. PreloadHints 接 HeroLoopHost.OnHeroChanged（消架构师 SOFT_FAIL #1）
  2. RegisterSha1Manifest 收 internal（消 #2）
  3. Form2 ctor log tag 改名 / 迁 Form2_Load（消 #3）
  4. plan §4 mitigation 补 pragma 豁免清单（消 #4）
  5. SG 反向映射抽公共常量类（消 #5）
  6. Strategy SG（消 ~1500 行 92 策略样板 ctor + 字段 + Register）
  7. 单测基础设施（xUnit + FakeItEasy + 92 策略 smoke test）
- **Phase 11**：Silt 子 BC 整顿（Silt.Main + DynamicSkillAutoSelector instance 化 + Form2/GameSession ctor 扩 HeroLoopHost → 删 Common.ItemEngine + Common.HeroLoopHost 终态 0 service locator）

## Phase 10B 完整收尾 (2026-05-23)

**plan SSOT**：`C:\Users\JoeZhang\.claude\plans\sturdy-bridging-rabin.md` (459 行 / 9 节)

**Epic 目标**：消除 Phase 10A architecture-sentinel 6 项 SOFT_FAIL handoff_notes（PreloadHints 桥半建 / API 可见性偏宽 / Form2 log tag 错位 / pragma 豁免 plan 未同步 / SG 反向映射双 dict / SHA1 Silt 死 key）—— 不引入新功能，只收口架构债务。

**5 commit 主干**（自底向上，6 SOFT_FAIL 一一对应）：

- `b0059c7` **B1** SG 公共常量类 `PictureCategoryMap` 抽取（消 #5 SG 双 dict 漂移）—— 净 +32 / -23
- `7373d29` **B2** SG SHA1 Silt 分割 + `ModuleInitializer` 合并单次注册（消 #6 SHA1 Silt 死 key）—— ~50 行
- `f2c60eb` **B3** `LazyImageLoader.RegisterSha1Manifest` + `Sha1MismatchCount` public → internal（消 #2 API 可见性）—— 净 +9 / -2
- `ca7bdce` **B4** `GameSession.PreloadHints` fire-and-forget 集成（消 #1 PreloadHints 桥半建）—— +9 行
- `99325d4` **B5** Form2 ctor log tag `[Form2.Load]` → `[Form2.ctor]` + 10A plan §4 跳转（消 #3 log tag + #4 pragma 豁免文档）—— +1 / +1 行

**关键文件**：

| 文件 | 状态 | chunk | 说明 |
|------|------|-------|------|
| `Dota2Simulator.SourceGenerators/PictureCategoryMap.cs` | NEW | B1 | internal static class 公共常量 dict + BuildReverse 反向派生 (~30 行) |
| `Dota2Simulator.SourceGenerators/PictureManifestGenerator.cs` | EDIT | B1+B2 | B1 引用 PictureCategoryMap 删本地 dict / B2 EmitSha1 拆 NonSiltMap + SiltMap + ModuleInitializer 合并注册 |
| `Dota2Simulator.SourceGenerators/PictureHeroPreloadGenerator.cs` | EDIT | B1 | 引用 PictureCategoryMap 删本地反向 dict |
| `Dota2Simulator/Vision/Cache/LazyImageLoader.cs` | EDIT | B3 | RegisterSha1Manifest + Sha1MismatchCount public → internal + XML doc 说明 |
| `Dota2Simulator/GameAutomation/Application/GameSession.cs` | EDIT | B4 | DispatchAsync 新英雄激活分支后追加 PreloadHints fire-and-forget 桥接 |
| `Dota2Simulator/Form2.cs` | EDIT | B5 | line 185 字面替 `[Form2.Load]` → `[Form2.ctor]` |
| `~/.claude/plans/generated-glinting-knuth.md` | EDIT | B5 | §4 末尾追加跳转单行（pragma 豁免 SSOT 单源化指引） |

## Phase 10B 关键不变量 (architecture-sentinel ACCEPT)

**继承 Phase 10A 7 不变量**（126 属性 1:1 / 33 文件 0 改 / `#if Silt` 语义 / SHA1 非阻断 / Phase 9 装配序零侵入 / 0 新增警告 / 每 chunk 单 commit 0 错），新增：

1. **6 SOFT_FAIL 100% 消除** ✅（architecture-sentinel 5 反模式 PASS + dogfood 双 build 0 错误）
2. **SG 单 dict SSOT**：`PictureCategoryMap.DirToClassName` 为唯一来源，反向映射 `BuildReverse` 派生 —— 加新顶层目录只改一处 ✅
3. **SHA1 Silt 分割语义正确**：`g.cs` 文本恒含 `#if Silt SiltMap #endif` 包裹段，csc 预处理器编译期剔除字段；Silt 关闭时 DLL 0 死 key ✅
4. **LazyImageLoader internal API 收口**：`RegisterSha1Manifest` / `Sha1MismatchCount` 仅 SG emit `[ModuleInitializer]` 同 assembly 调用，跨 assembly 0 消费方 ✅
5. **PreloadHints 桥接闭环**：`GameSession.DispatchAsync` 新英雄激活后 fire-and-forget `LazyImageLoader.PreloadImagesAsync(PictureHeroPreloadHints.Hints[hero])` ✅（实测仍 untested，待用户冒烟）
6. **接口契约破坏自检 PASS**：B3 收紧 internal 跨 assembly 0 引用；B4 GameSession 签名不变；B5 字面替换不动行为 ✅

## Phase 10B architecture-sentinel verdict

**ACCEPT** —— 5 反模式全 PASS / dogfood 双 build（DOTA2 单独 + DOTA2+Silt）0 错误 / 6 SOFT_FAIL 100% 消除 / 接口契约破坏自检 PASS / Phase 10A 7 不变量全保留。

## Phase 10B handoff_notes (5 项 Phase 10C+ 候选，不污染当前 epic 完成状态)

1. **PreloadHints hero key 一致性 lint / runtime warning**（Phase 10C+ 候选）：`hero.Name = Form2.tb_name.Text.Trim()` ↔ Strategy 文件名 stem 字符串约定耦合，miss 路径静默；建议 `GameSession.DispatchAsync` 内 TryGetValue miss 分支加 `Trace.WriteLine` / 单元测试 assert `PreloadHints.Hints.Keys ≡ Strategy 文件名 stem 集合`
2. **Sha1MismatchCount 消费方缺失**（Phase 10C+ 候选）：B3 internal 收紧后主项目 0 read，counter 永无 consumer；建议 admin 命令 / Form2 调试面板暴露 / 或删该 counter（YAGNI）
3. **`[Preload]` / `[LazyLoad]` log tag 统一**（Phase 10C+ 候选）：`GameSession.cs:69-71` 用 `[Preload]`，`LazyImageLoader.cs:208` 用 `[LazyLoad]`，建议后续 tag 统一
4. **plan SSOT 措辞勘误**（Phase 10C+ 候选）：
   - B2 反预测「g.cs 中 Silt 字段也消失」实际应为「DLL 中 SiltMap 字段消失」（g.cs 文本始终含 `#if Silt` 包裹段，csc 预处理器在编译期剔除）
   - B4 plan 写双 using `Dota2Simulator.Vision + Cache`，实际 namespace 平铺仅需单 using `Dota2Simulator.Vision`
5. **HeroIdentity epic / F1 HUD 英雄名提取**（Phase 10C+ 候选 / plan §1.2 显式排除）：真根因解 = 用 OCR / HUD 英雄名提取替代 tb_name 用户输入，从源头消 PreloadHints key 约定耦合

## Phase 10B 反预测与实测三处偏差备忘

- **B2**：g.cs 文本始终含 `#if Silt SiltMap #endif` 段，编译期剔除字段；语义不变量正确，plan 措辞偏严
- **B4**：plan 双 using → 实际单 using（namespace 平铺，agent 自纠 CS0234 后 PASS）
- **B5**：跑 Release build 时撞 B1 并发未 commit 中间状态 → B1 完成后已恢复

## 待用户冒烟（Phase 10B 收尾）

继承 Phase 10A 冒烟清单 + 新增 B4 PreloadHints 实测项：

1. **B4 PreloadHints 实测**（新增）：
   - 启动程序（admin）+ 输入英雄名切换 → stdout 出现 `[LazyLoad] 加载` 系列（B4 fire-and-forget 触发预加载）
   - 重复切同一英雄第二次起命中缓存（不重复 `[LazyLoad] 加载`，只见命中日志）
2. **B2 Silt 关闭 rebuild 自检**（B2 已实测，用户可二次确认）：
   - csproj `DefineConstants` 移除 `Silt` → `dotnet build` → 0 错误 + DLL 内 SiltMap 字段消失
3. **继承 Phase 10A 冒烟项**：
   - R2 ModuleInitializer 时序：`[ModuleInit] <ticks1>` 早于 `[Form2.ctor] <ticks2>`（B5 tag 已改为 ctor）
   - 抽样 4 属性英雄全技能键（大牛/影魔/卡尔/猛犸 等，含图片识别路径）
   - 物品使用（假腿切换 / 神杖魔晶判断）
   - SHA1 mismatch 测试（可选）：临时改一张 .bmp，重 build，启动看 stdout SHA1 mismatch log

## Phase 10B 回滚锚点

- 单 chunk revert：`git revert <hash>`（B1-B5 任一）
- 完整撤回 Phase 10B：`git revert ca7bdce f2c60eb 7373d29 b0059c7 99325d4`（5 commit 顺序无关，因 B5 与 B1-B4 文件不相交）

## 下次 session 起手指引（Phase 10C / Phase 11 任选）

- **Phase 10C 候选**（5 项 handoff_notes，详见上文 Phase 10B handoff_notes 段）：
  1. PreloadHints hero key 一致性 lint / runtime warning
  2. Sha1MismatchCount 消费方接入或删除（YAGNI）
  3. `[Preload]` / `[LazyLoad]` log tag 统一
  4. plan SSOT 措辞勘误回写（B2 + B4）
  5. HeroIdentity epic / F1 HUD 英雄名提取（真根因解）
- **Phase 10C 其他候选**（Phase 10A 后续段已写）：
  6. Strategy SG（消 ~1500 行 92 策略样板 ctor + 字段 + Register）
  7. 单测基础设施（xUnit + FakeItEasy + 92 策略 smoke test）
- **Phase 11**：Silt 子 BC 整顿（Silt.Main + DynamicSkillAutoSelector instance 化 + Form2/GameSession ctor 扩 HeroLoopHost → 删 Common.ItemEngine + Common.HeroLoopHost 终态 0 service locator）

## Phase 10C 完整收尾 (2026-05-23)

**plan SSOT**：`C:\Users\JoeZhang\.claude\plans\crystal-emitting-knuth.md` (847 行)

**Epic 目标**：92 策略类 ctor + `_input`/`_vision`/`_skill`/`_item` 4 字段 + `Hero` enum 属性 + 4 手写 partial Registry 8 样板点 → `[HeroStrategy("name", HeroAttribute.X, RequiresUi=false)]` attribute SSOT + `HeroStrategyGenerator` (`ForAttributeWithMetadataName`) 全量 emit；调用方 API 全保（92 策略类实例化路径不变，AppContainer 装配链字节零改），消 ~1500 行重复样板。

**5 chunk 主干**（自底向上，S1-S5；S3/S4 为「同 commit 双部分整改」即 SG emit 与业务真删一次性配套提交以保 0 build 错）：

- `9f75a9d` **S1** HeroStrategyAttribute SSOT + HeroStrategyGenerator hello-world（SG 项目串联实证；netstandard2.0 / Microsoft.CodeAnalysis.CSharp 4.11.0；attribute 三 metadata: Name / HeroAttribute / RequiresUi；与 HeroAttribute enum 同 namespace 免双 using）
- `554c864` **S2** 92 *Strategy.cs 加 `[HeroStrategy]` attribute（双源期保编译, 0 删除；distinct hero name = 92 验证；全 Edit 工具路径绕开 PowerShell UTF-8 损毁）
- `7a2be5b` **S3** **同 commit 双部分整改**：SG emit Strategy partial（`ForAttributeWithMetadataName` 首次实证，92 命中）+ 92 业务真删 ctor/field/Hero（CS0102/CS0111/CS1729/CS0103 free；净 -1407 LOC）
- `95e2f3a` **S4** **同 commit 双部分整改**：SG emit `HeroStrategyRegistry.Generated`（4 partial method body + 92 Register 调用 + 测试Strategy `_ui!` 6 ports）+ 删 4 手写 partial Registry（CS0759/CS8795 free；净 -72 LOC；主 `HeroStrategyRegistry.cs`/`AppContainer.cs` 字节零改）
- **S5** verify-only 无 commit（14 项 verify 全 PASS / distinct hero name = 92 核心 gate）

**净行数**：约 -1479 LOC（S2 +92 / S3 净 -1407 / S4 净 -72 / S5 0），消除 92 策略重复样板（ctor + field + Hero + 4 partial Registry register 调用），样板压缩率约 83%。

**关键文件**：

| 文件 | 状态 | chunk | 说明 |
|------|------|-------|------|
| `Dota2Simulator/GameAutomation/Domain/Heroes/HeroStrategyAttribute.cs` | NEW | S1 | attribute SSOT (Name / HeroAttribute / RequiresUi 三 metadata)；与 HeroAttribute enum 同 namespace 免双 using |
| `Dota2Simulator.SourceGenerators/HeroStrategyGenerator.cs` | NEW | S1+S3+S4 | hello-world (S1) → EmitStrategyPartial 92 命中 (S3) → EmitRegistry (S4) 4 partial method body + 92 Register 调用 + 测试Strategy `_ui!` 6 ports；S4 用 `global::` fully qualified namespace 避免 4 sub namespace using 冲突 |
| `Dota2Simulator/GameAutomation/Heroes/{Strength,Agility,Intelligence,Universal}/*Strategy.cs` (92 文件) | EDIT | S2+S3 | S2 +`[HeroStrategy]` / S3 `sealed` → `sealed partial` + 删 ctor / 4 field / Hero |
| `Dota2Simulator/GameAutomation/Application/HeroStrategyRegistry.Strength.cs` | DELETE | S4 | 手写 partial Registry 删除 |
| `Dota2Simulator/GameAutomation/Application/HeroStrategyRegistry.Agility.cs` | DELETE | S4 | 手写 partial Registry 删除 |
| `Dota2Simulator/GameAutomation/Application/HeroStrategyRegistry.Intelligence.cs` | DELETE | S4 | 手写 partial Registry 删除 |
| `Dota2Simulator/GameAutomation/Application/HeroStrategyRegistry.Universal.cs` | DELETE | S4 | 手写 partial Registry 删除 |
| `Dota2Simulator/GameAutomation/Application/HeroStrategyRegistry.cs` | 字节零改 | - | 主 partial 保留：`_ui` 字段 (line 25) + 4 partial method declaration (line 55-58) |
| `Dota2Simulator/CompositionRoot/AppContainer.cs` | 字节零改 | - | 装配链全程不变 |
| `Dota2Simulator/GameAutomation/Application/IHeroStrategy.cs` | 字节零改 | - | 接口契约不变 |

## Phase 10C 关键不变量 (architecture-sentinel PASS)

**继承 Phase 10A 7 + Phase 10B 6 不变量**（126 属性 1:1 / 33 文件 0 改 / `#if Silt` 语义 / SHA1 非阻断 / Phase 9 装配序零侵入 / 0 新增警告 / 每 chunk 单 commit 0 错 / 6 SOFT_FAIL 消除 / SG 单 dict SSOT / SHA1 Silt 分割正确 / LazyImageLoader internal API 收口 / PreloadHints 桥接闭环 / 接口契约自检），新增 5 项：

1. **`[HeroStrategy]` attribute 是 92 策略身份 SSOT**：Name / HeroAttribute / RequiresUi 三 metadata 一次声明，SG emit ctor + 4 field + Hero + Register 全派生 ✅
2. **`ForAttributeWithMetadataName` 实证命中 = 92**（distinct hero name 核心 gate）：S5 verify gate 通过；net new SG 增量 cache 命中点 ✅
3. **AppContainer.cs / IHeroStrategy.cs / HeroStrategyRegistry.cs 主 partial 字节零改**：装配链 + 接口契约 + 共享字段声明 0 侵入；92 策略调用方实例化路径不变 ✅
4. **4 手写 partial Registry 真删 + SG emit `Generated` 替代**：Register 调用 SSOT 由 SG `[HeroStrategy].Name` 驱动，加新英雄只改 1 处（attribute）✅
5. **同 commit 双部分整改不变量**（S3 / S4 模板）：SG emit + 业务真删一次性配套提交，保中间态不出 0 build 错；revert 必须按倒序整 commit 撤回（不可单部分撤）✅

## Phase 10C architecture-sentinel verdict

**PASS** —— 5 反模式全 PASS / dogfood 14 项 13 verified + 1 falsified-by-noise 实质 PASS / 接口契约 8 项全保留字节级 diff = 0 / Phase 10A 7 + Phase 10B 6 不变量全保留 / 反预测 4 verified + 1 falsified-by-noise。

## Phase 10C handoff_notes (7 项 Phase 10D+ 候选，不污染当前 epic 完成状态)

1. **SG enum 反查改用 `ITypeSymbol.GetMembers`**（中优先 / Phase 10D+ 候选）：当前 SG 用数值 switch 反查 HeroAttribute enum (0=Strength, 1=Agility, 2=Intelligence, 3=Universal)，未来 enum 插值/加值时静默落 Universal bucket；建议改 `ITypeSymbol.GetMembers` 取真 field name 解 metadata
2. **SG `HeroStrategyGenerator.Hello.g.cs` 决断**（低 / Phase 10D+ 候选）：plan §8 预估 S3 起删除，实测 HEAD = S4 仍保留 1 行注释 g.cs；0 功能影响但 plan 实施不一致 —— 删 hello-world 或更新 plan §8
3. **plan §S5 v15 AppContainer 路径笔误**（低 / Phase 10D+ 候选）：plan 写 `Dota2Simulator/Application/AppContainer.cs`，实际 `Dota2Simulator/CompositionRoot/AppContainer.cs` —— plan SSOT 勘误回写
4. **plan §S5 v13 `_ui!` verify 改精确匹配**（低 / Phase 10D+ 候选）：当前 grep `_ui!` 实测 = 2（SG 注释 line 3 + 真调用 line 113），实质 PASS；建议改 `\s+_ui!\s*,` 精确匹配剔除注释 false-positive
5. **Linux/macOS CI 启用时 reverify 中文 class name SG hint name 跨平台稳定性**（低 / Phase 10D+ 候选 / 当前 Windows-only 部署无暴露面）
6. **VS hot reload 启用时实证 `ForAttributeWithMetadataName` 增量 cache 命中**（低 / Phase 10D+ 候选 / 当前 dotnet CLI 工作流无暴露面）
7. **plan §3 S3 实战 2 意外累积到 fact 知识库**（低 / Phase 10D+ 候选）：
   - `大牛Strategy.cs` 行尾注释 `// A4 阶段：_vision 暂未使用` 致 regex 漏命中 → Edit 单文件补刀
   - subagent 用 `record struct` 触发 CS0518 (netstandard2.0 缺 `IsExternalInit` polyfill) → 改普通 struct 修复
   - 建议两条经验入 `.claude/dream/knowledge/facts/` 或本仓 lessons-learned

## Phase 10C 反预测与实测三处偏差备忘

- **S3** subagent 实施期遇 2 意外（regex 漏命中 + `record struct` CS0518），均通过 Edit 补刀 + struct 改造解决，非反预测漂移
- **S4** SG 用 `global::` fully qualified namespace 避免 4 sub namespace using 冲突，plan 描述未提，实际优化
- **S5** v13 `_ui!` 命中 = 2 注释 false-positive；plan v15 AppContainer 路径笔误（CompositionRoot/ 非 Application/）

## 待用户冒烟（Phase 10C 收尾）

继承 Phase 10A / Phase 10B 冒烟清单 + 新增 Phase 10C 专项实测：

1. **S3 SG emit ctor / field 实测**（新增）：4 属性抽样英雄按 Q/W/E/R 验证 SkillEngine/ItemEngine 调用不中断（条件委托链路无 NRE / `_skill` / `_item` / `_vision` / `_input` 字段经 SG emit 后注入正确）
   - Strength: 军团 / 屠夫
   - Agility: 小黑 / 影魔
   - Intelligence: 卡尔 / 暗影萨满
   - Universal: 测试 / 猛犸
2. **S4 测试Strategy 6 ports + `_ui!` 注入实测**（新增）：按 Q 触发 UI 注入路径，验证 `_ui!` (RequiresUi=true) 经 SG emit 后真注入 UiInvoker 不 NPE
3. **S3/S4 Registry 装配实测**（新增）：启动程序后切英雄触发 GameSession 内 Strategy 实例化路径全程无类型加载顺序异常
4. **Phase 10B B4 PreloadHints 联动**（继承）：切英雄触发 `GameSession.DispatchAsync` PreloadHints fire-and-forget（Phase 10B B4），`[LazyLoad] 加载` log 正常
5. **继承 Phase 10A / 10B 冒烟项**：
   - R2 ModuleInitializer 时序：`[ModuleInit] <ticks1>` 早于 `[Form2.ctor] <ticks2>`
   - 物品使用（假腿切换 / 神杖魔晶判断）
   - SHA1 mismatch 测试（可选）

## Phase 10C 回滚锚点

- **完整撤回 Phase 10C**：`git revert 95e2f3a 7a2be5b 554c864 9f75a9d`（4 commit 顺序倒序）
- **注意**：S3 / S4 是「同 commit 双部分整改」（SG emit + 业务真删一次性配套），revert 单个 commit 会触发编译双源错（field 真删但 SG emit 撤 → CS0103 / Hero 真删但 ctor 撤 → CS0102）；**建议按倒序全 revert** 才能回到稳定态
- S5 无 commit，无需 revert

## 下次 session 起手指引（Phase 10D / Phase 11 任选）

- **Phase 10D 候选**（7 项 Phase 10C handoff_notes + 5 项 Phase 10B 剩余 handoff_notes，详见各段）：
  - 10C #1 SG enum 反查改 `ITypeSymbol.GetMembers`（中优先）
  - 10C #2-#7 plan 勘误 / hello-world g.cs 决断 / `_ui!` 精确匹配 / CI 跨平台 / VS hot reload / fact 知识库累积
  - 10B #1-#5 PreloadHints hero key lint / Sha1MismatchCount 消费方 / log tag 统一 / plan 措辞勘误 / HeroIdentity epic
- **Phase 11**：Silt 子 BC 整顿（Silt.Main + DynamicSkillAutoSelector instance 化 + Form2/GameSession ctor 扩 HeroLoopHost → 删 Common.ItemEngine + Common.HeroLoopHost 终态 0 service locator）—— 继承 Phase 10A / 10B / 10C 后续段
