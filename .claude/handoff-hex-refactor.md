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

## 后续（Phase 7+）
- 实际删除死代码（_Legacy/ + Other/ + 4 个标 TODO-dead 文件）
- IUiInvoker port 抽出（去 Common.Main_Form 单例）
- ConditionDelegateBitmap 签名改不带 ImageHandle（移除 _vision.GetCurrentFrame() Obsolete 临时妥协）
- Games.Dota2 BC（Main/Item/Skill/Silt）SimKeyBoard 直调切 port
- KeyboardMouse/ 4 基础驱动文件评估是否迁 Infrastructure/Native/
- Vision 子 namespace 评估
- LOL/HF2 切 GameSession（统一入站端口）
- **Vision 性能**：候选区域裁剪 + GpuFusedVisionAdapter（GPU PoC 已验证免回传优势）
