# Handoff: Dota2Simulator 完整六边形架构重写

## Epic 概要
C# .NET 10 WinForms 游戏自动化框架，重写为六边形架构。
- Phase 4 plan SSOT: `C:\Users\JoeZhang\.claude\plans\noble-percolating-hejlsberg.md`
- Phase 6 plan SSOT: `C:\Users\JoeZhang\.claude\plans\shimmying-strolling-starfish.md`

## 进度
- [x] Phase 1-3（commit `11d51e8` → `3ea4129`）
- [x] **Phase 4**（= 原 Phase 4 + Phase 5 合并）—— 2026-05-22 完成（commit `744ed12`），用户已冒烟验证
- [x] **Phase 6 + 6.5**（hex 闭环 + 物理重组）—— 2026-05-23 完成（15 chunk，A1-A6 + B1-B5 + C1）

## Phase 6 + 6.5 已完成（main 分支连续 commit，每 chunk 0 build 错误）

### 链 A — Hex 闭环（92 策略类切 port + CompositionRoot 实例化）
- `990c61f` A1 AppContainer 雏形 + Form2 构造器注入
- `de52f6c` A2 删 AppComposition.cs
- `819e2c0` A3 Port 扩 + ScreenRegion 值对象（IInputExecutor +PressViaEnigo / IScreenVision +FindInRegion +GetCurrentFrame）
- `be5f99d` A4a Strength 26 策略切 ctor 注入（11 文件替换 SimKeyBoard 调用）
- `7c36d68` A4b Agility 23 策略切 ctor 注入
- `401d660` A4c Intelligence 31 策略切 ctor 注入
- `050d567` A4d Universal 12 策略切 ctor 注入（含进化岛 3 处 SimEnigo → PressViaEnigo）
- `5fae140` A5 ConditionSlotSet 切 vision port + HeroAggregate.Init 双阶段
- `321c47b` A6 AppContainer 装饰 Probe ports

### 链 B — 物理重组（Vision 拆分 + Infrastructure 统一 + Logger 合并）
- `dc04afb` B1 Vision/ 子目录搬移 + 2 文件改名（13 文件 → Capture/Cache/Processing/Matching）
- `7a81921` B2 BenchMark→Benchmark 改名（**简化：跳过子 namespace 调整**，30+ 文件 using 改造成本过高）
- `db04ef7` B3 ImageManager 601 行 god class 拆 6 partial
- `1ddc6a6` B4 Logger 合 NLog 删 ILogger/ConsoleLogger
- `71524ec` B5 Infrastructure 骨架 + Adapters 迁入 + Native（21 文件 + 8 dll 迁入 / 4 空目录 rmdir）
- `9dd7319` C1 死代码标 TODO-dead-Phase7-remove（20 .cs + Other/README.md）

## 架构现状（Phase 6 完成后）

### 内层（GameAutomation/）
- **92 策略类**（Heroes/{Strength,Agility,Intelligence,Universal}/）全部 ctor 接 (IInputExecutor, IScreenVision)，0 个 `using KeyboardMouse` 残留，0 个 `SimKeyBoard.X` 直调。
- **Application 层**：HeroContext（HeroId + HeroAggregate）/ HeroStrategyRegistry（构造接 ports）/ GameSession（入站端口）/ ConditionSlotSet（Init(vision) 双阶段）。
- **Ports**：IInputExecutor（14 方法）/ IScreenVision（6 方法）/ IInputMonitor / ISpeaker / ITemplateRepository / IClock / IGameSession / IKeyHold。

### CompositionRoot/
- **AppContainer**（替代 AppComposition）：实例化 HybridInputAdapter + RustVisionAdapter，套 ProbeInputExecutor + ProbeScreenVision 装饰，注入 HeroStrategyRegistry + 调 Main._聚合.Init(Vision)。
- Program.cs `#if DOTA2` 走 AppContainer + Form2(container)；LOL/HF2 走原无参 Form2()。

### Infrastructure/（新建）
- Input/ + InputMonitor/ + Vision/ + Audio/ + Diagnostics/ + Native/{Win32, Libs} = 21 .cs + 8 dll。
- namespace 保持原样（如 `Dota2Simulator.Input.Adapters`）—— 简化决策，物理位置已表达分组。
- 唯一 namespace 改动：SetWindowTop → `Dota2Simulator.Infrastructure.Native.Win32`（using static 路径耦合必须改）。

### Vision/（子目录化 + god class 拆分）
- 根：ImageType / ImageHandle / ColorExtensions / ImageProcessor（标 TODO-dead）
- ImageManager.{Coordinates, Registry, Factory, PixelReader, IO, Matching}.cs（6 partial）
- Capture/（DesktopDuplicator + GlobalScreenCapture + ModifyGraphics）
- Cache/（DynamicImageBuffer + StaticImageCache + TripleBufferSystem + PixelCodeCache + LazyImageLoader）
- Processing/（ImageProcessingSystem + ImageSystemMonitor + RustImageProcessingSystem）
- Matching/（ImageFinder + AdvancedImageFinder + AdvancedGlobalScreenUsage）
- Benchmark/ + Ocr/

### 根目录现状
- .cs: Form2.cs / Form2.Designer.cs / Program.cs / RandomGenerator.cs(死代码标)
- 配置: app.manifest / nlog.config / dota2.xlsx
- 0 .dll（全部迁入 Infrastructure/Native/Libs/）

## 不变量（Phase 6 后追加）
1. 每 chunk `dotnet build -c Debug` 0 错误，独立 commit。
2. Domain 零基础设施依赖；Application 可依赖基础设施。
3. per-hero 聚合 + 不可变值对象（禁位标志状态机）。
4. 组合优于重复（禁 N 段同构复制）。
5. **Heroes/ 内层 0 `using KeyboardMouse`**；策略类只依赖 IInputExecutor / IScreenVision。
6. **Heroes/ 仍保留 `using Dota2Simulator.Vision`**（ImageHandle 类型在 Probe 委托签名里用，等 Phase 7+ 改委托签名后才能彻底禁）。

## 已知不做项（Phase 6 边界）
- `Common.Main_Form?.Invoke(...)` 13 站点保留（IUiInvoker port 留给 Phase 7）
- `测试Strategy.cs:51-53,77` 反向抓 UI 保留
- LOL/HF2 老 `根据当前英雄增强` 静态函数保留
- Form2 静态 `Instance` 单例保留
- `ConditionDelegateBitmap(ImageHandle)` 签名保留（_vision.GetCurrentFrame() [Obsolete] 临时妥协）
- `KeyboardMouse/` 4 文件保留原位（56 文件 using ns，改造成本最高）
- `Skill.cs / Item.cs / Main.cs / Silt/*` 内 SimKeyBoard/Vision 直调保留（Games.Dota2 BC）
- 死代码本次只标不删（Phase 7 真删 _Legacy/ + Other/ + RandomGenerator + AdvancedXxx + ImageProcessor）
- Vision 子 namespace（Vision.Capture / Vision.Cache 等）未做（30+ 调用方 using 改造成本过高）
- Infrastructure 子 namespace 未做（同上）

## 待用户冒烟（Phase 6 + 6.5）
所有 chunk build 0 错误，但**功能未经实战**。建议：
1. 启动程序（管理员）
2. 抽样英雄按全部技能键：大牛/屠夫/海民/斧王/马尔斯（Strength）/ 影魔/敌法/小黑/猴子/幽鬼（Agility）/ 光法/卡尔/暗影萨满/莱恩/双头龙（Intelligence）/ 猛犸/狼人/剧毒/进化岛/命运2（Universal）
3. **A5 关键回归**：猛犸震荡波后摇（Probe 链路触发 _vision.GetCurrentFrame()）
4. **A4d 关键回归**：进化岛技能（SimEnigo → PressViaEnigo 后端切换）
5. **B5 关键回归**：截图功能 + 找图功能（8 dll 迁入 Infrastructure/Native/Libs/ 后 P/Invoke 加载）

回滚锚点：每 chunk 单 commit，`git revert <hash>` 单步回滚。完整撤回 Phase 6：`git revert 9dd7319..990c61f`（15 commit）。

## Phase 7（2026-05-23 完成）

IUiInvoker port + 25 站点切走 Common.Main_Form 单例 + Form2.Instance 删除。

- `9919cef` D1 IUiInvoker 骨架（port + UiField enum + Form2UiInvoker adapter + AppContainer.BindUi + Common.UiInvoker service locator）
- `8a27111` D2 GameAutomation 内层 (测试Strategy 4 站点) ctor 注入；Registry 双阶段 RegisterAll(ui)
- `3c28398` D3 Games.Dota2 BC 14 站点 (Skill 11 / Item 2 / Main 1) 切 Common.UiInvoker
- `14816bb` D4 Silt BC 7 站点切
- `65470d1` D5 删 Common.Main_Form / Form2.Instance / TODO

Backup 基础设施同步装好：`D:\backup\C\temp\JoezhangYN\C#\Dota2Plus\Dota2Simulator.git` bare + post-commit hook 自动 push。

## Phase 8（进行中，2026-05-23 起）

**Epic 目标**：现代化全量解耦 = 0 service locator + 0 可变状态 static God class + 0 单例。

**plan SSOT**：`C:\Users\JoeZhang\.claude\plans\sequential-nibbling-lightning.md`

**已完成（4/10 chunk）**：
- `5232092` A 拆 Invoke 写场景异步化（SetText → BeginInvoke；用户判定 TTS 抢跑无碍）
- `d82a2ee` C1 抽 LegSwapState 子聚合（Item.cs 8 字段 + 内嵌 `技能切假腿配置` class 迁 GameAutomation/Domain/；HeroAggregate 加 LegSwap 属性；47 文件批替换跨 BC 共享状态）
- `3477f25` C3 抽 SessionState 单例 + 4 字段迁 HeroAggregate（74 处跨 BC 替换 / 34 文件）：
  - SessionState 单例（含 Phase 9 `IHeroIdentity` 占位接口供 HeroIdentity epic 挂接），AppContainer 装配
  - IGameSession.IsPaused 转发；GameSession ctor 接 SessionState
  - Main._session static field 桥接（C3 过渡 service locator，D1 删）
  - Skill._技能数量 → HeroAggregate.SkillCount (19 处)
  - Item._是否魔晶/神杖 → HeroAggregate.HasShard/HasAghanim (44 处，含 Heroes/ 18 文件)
  - Main._中断条件 → SessionState.IsPaused (11 处)
- **(待 commit)** C4 SkillEngine 实例化（Skill 1809 行 static class → SkillEngine 实例 + Skill thin facade）：
  - 新建 `GameAutomation/Application/SkillEngine.cs`（1820 行）= 可变状态字典/锁/时间/缓存/重复按键阈值 instance 化；ctor 接 4 ports (input/vision/ui/aggregate)
  - 内部 5 处 SimKeyBoard → _input；13 处 Common.UiInvoker → _ui；8 处 GlobalScreenCapture.GetCurrentHandle() → _vision.GetCurrentFrame()；Main._聚合 → _aggregate
  - 纯只读查找表保留 static：技能4/5/6、StringBuilderPool*、ColorMatcher 嵌套类、ColorConfig、纯 helper 函数（获取技能信息 / 获取技能位置信息 / 6 个 color getter / ShouldEndRelease 等）
  - Skill.cs 降级 thin facade（150 行）：保留 `技能类型` enum（马尔斯Strategy.cs:85 引用契约）+ `重复按键执行间隔阈值` static property 转发 + 27 个方法一行转发壳
  - Common.cs 加 `SkillEngine?` service locator 字段（D1 删）；AppContainer.BindUi 装配 `Common.SkillEngine = new SkillEngine(Input, Vision, Ui, Main._聚合)`
  - SkillEngine.cs `using 技能类型 = Skill.技能类型` type alias 避免内部 57 处 enum 引用改动
  - BC 内 3 处旧调用（Main:586/712, Item:30）通过 facade 透明转发 0 改动；Heroes/ 86 文件 331 处 `Skill.X` 调用通过 facade 透明转发 0 改动（C7 替换为 _skill.X）

**剩余 chunk（下次 session 接手）**：
- **B 子线撤出 epic**（用户决策 2026-05-23）：ConditionDelegateBitmap 签名改非完成形态硬指标，且 C 子线 BC 实例化后 SkillEngine 持 IScreenVision 自然消解
- **C5** ItemEngine 实例化（Item.cs 813 行；同 C4 模式：可变状态实例化 + ctor 接 4 ports + Common.ItemEngine 桥；物品4/5/6 常量保 static）
- **C6** HeroLoopHost (Games.Dota2.Main) 实例化 + HeroAggregate 单阶段 ctor 注入 vision（删 HeroAggregate.Init 双阶段）
- **C7** 92 策略 ctor 扩参 +SkillEngine +ItemEngine +HeroLoopHost；Registry 4 partial 扩参；策略类内 Skill./Item./Main. → \_skill./\_item./\_main.（1588 处替换，可拆 C7a-d 按属性并行）
- **D1** 删 Common.UiInvoker + Common.SkillEngine + Common.ItemEngine + Skill/Item/Main 三 facade + IScreenVision.GetCurrentFrame[Obsolete] + 收尾 grep 验证

**Phase 8 关键不变量**：
1. Phase 8 每 chunk 单 commit + dotnet build -c Debug 0 错误 + 自动 backup push（验证 `.git/_unbacked_commits.log` empty）
2. **HeroAggregate.LegSwap / SkillCount / HasAghanim / HasShard 是 BC 跨边界共享状态的 SSOT**（C1+C3 起）—— 后续 chunk 禁再在 Item/Skill 加 static flag
3. **SessionState 单例是会话级共享状态的 SSOT**（C3 起）—— Phase 9 HeroIdentity epic 在此挂接
4. **0 可变状态 static God class** 解读：纯只读查找表（ColorMatcher / RSquared / 物品4/5/6 / Rectangle 常量）保留 static；可变状态全压 HeroAggregate / SessionState
5. SetText 已异步（BeginInvoke）；调用方紧跟 TTS 用户接受抢跑
6. plan dep graph: C3 依 C1 ✅；C4/C5/C6 依 C3 ✅；C7 依 C4+C5+C6；D1 依 C7
7. **C3 引入 Main._session 过渡 service locator**（commit `3477f25` 内）—— 等 C5/C6 ItemEngine/HeroLoopHost 实例化后改 ctor 注入，D1 删

**Phase 9 epic 占位（C3 引入）**：
- `GameAutomation/Application/SessionState.cs` 内 `IHeroIdentity` 接口：F1 触发提取 HUD 英雄名区域 → 像素模板多帧一致性投票 → 稳态固化作"自己英雄"绑定；所有 HUD 读取经身份 gate，看别人状态时返回上一稳态快照避免误触（用户决策 2026-05-23 完整形态：识别 + 全 HUD gate + 上一稳态快照回退）

**下次 session 起手指引**：
1. Read `C:\Users\JoeZhang\.claude\plans\sequential-nibbling-lightning.md` 全文
2. Read 本 handoff 文件 Phase 8 段
3. 从 C5 开始（ItemEngine 实例化，Item.cs 813 行 → internal sealed class ItemEngine；可复用 C4 模板：facade 转发 + Common.ItemEngine 桥接 + AppContainer.BindUi 装配）

## 后续（Phase 9+，本 epic 外）
- 实际删除死代码（_Legacy/ + Other/ + 4 个标 TODO-dead 文件）
- ConditionDelegateBitmap 签名改 ImageHandle → IScreenVision（C 完后下游 API 改造便利时再做）
- Games.Dota2 BC SimKeyBoard 直调切 port（C 后 SkillEngine 持 IInputExecutor 自然消解大部分）
- KeyboardMouse/ 4 基础驱动文件评估是否迁 Infrastructure/Native/
- Vision 子 namespace 评估
- LOL/HF2 切 GameSession（统一入站端口）
- **Vision 性能**：候选区域裁剪 + GpuFusedVisionAdapter（GPU PoC 已验证免回传优势）
