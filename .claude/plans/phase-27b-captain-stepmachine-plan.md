---
epic: Phase 27B SkillEngine helper async polling adapter (船长 .StepMachine 迁移)
plan_step: 3 (plan agent)
prev_epic: Phase 27A retry 2 (HEAD 5863da8)
landing_shape: selective_ports
generated_on: 2026-05-26
revised_on: 2026-05-26 (plan-reviewer R1-A REJECT 后 10 项必修 + 3 项建议补落实)
sot_slice: .claude/sot/phase-27b-captain-stepmachine.md
references:
  - .claude/handoff-hex-refactor.md §3091-3107 (5 deferred + grill 三问)
  - .claude/CLAUDE.md §Phase 27A retry 2 显式破坏点
  - .claude/rules/lessons-learned.md (L1 行尾注释致 regex 漏命中)
  - Dota2Simulator/GameAutomation/Application/ConditionSlot.cs:11 (revision verify ground truth: `public delegate Task<bool> ConditionDelegateBitmap()` async 无参)
revision_notes:
  - 用户重决断 D2 (作废原 方案 H 扩 RegisterProbeAsync): C3 / C4 都保留 `.RegisterProbe` 原 DSL, lambda 内嵌 async 包 AwaitSkillHelper. 落点 1 行 × 2.
  - 元层教训: step 2 sot-annotator + step 3 plan agent 串行环节均未 Read ConditionSlot.cs:11 verify, 仅靠 plan-reviewer 1 个 Grep 检出. 触违铁律 5(b) Zoom-out + grill-me §动手前 fact 必查. 修订纪律: 凡引用现存 type/delegate/method signature 必先 Read/Grep verify, 不凭训练印象 / 不凭 SOT 切片描述 (上游 SOT 也可能错).
  - 措辞精确化 (2026-05-26 lite 修订, handoff §3318-3319): (a) "sync invoke afterAction" → "fire-and-forget `_ = Task.Run(afterAction)`" — S2 实施期 SkillEngine.cs:1382/1402/1433/1453/1473 五个 X后续 helper 实证均为 Task.Run 包装 (非 sync invoke); 落点正确性不影响 ("不经 Runner 调度" 含义保留). (b) "build verify 4 档 DOTA2/Silt/LOL/HF2" → "build verify (Debug/Release 2 档, DOTA2+Silt 激活)" — csproj 实际仅 Debug/Release 两档, 两档都 `DefineConstants=DOTA2;TRACE;Silt`; LOL/HF2 不在编译矩阵里, 留 27A retry 2 attempt 1 沿用措辞遗留.
---

# Phase 27B 船长 .StepMachine 迁移 — chunk 实施 plan

> plan agent (step 3) 产出. 严格不偏离 SOT 切片; 偏离点 §6 显式声明.
> scope: 仅船长 1 hero (Q/E/D2 + C3/C4 RegisterProbe). 不动 SkillEngine 内部 / Runner ctor / sub-builder 8 维 / 4 hero 已稳定代码.

## §0 BC 落地形态自检 (W2 引入)

承接 Phase 27A retry 2 同 BC (DSL fluent + StepMachine 子领域).

- Q1 业务规则频繁演化? yes — 4 hero 已迁 + 船长 + 27B+ 横向耦合迁延续, DSL 表达力持续扩 (但本 epic 不加新维度)
- Q2 多入口? no — DSL fluent 仅 HeroPlanBuilder 单入口; AwaitSkillHelper 原语单入口 (Runner switch case)
- Q3 IO 后端会换? no — SkillEngine helper 是既有实现, 不动内部; Runner Probe 自包含不调外部 backend
- 决断: landing_shape = selective_ports — DSL fluent + StepMachine 子域是 selective port (业务零感知 backend), 未到 full_hexagon (无 ports/adapters 多 implementor)

---

## §1 落点 1NF (file × line range × old × new × 涉及原语)

### Row 1: StepCommand.cs 加 AwaitSkillHelper record

- 文件: `Dota2Simulator/GameAutomation/Domain/StepMachine/StepCommand.cs`
- line range: 当前 L54 后 (Delay record 之后, #endif 之前) 插入. sot-annotator L56-63 注释段在代码落地后可删
- new 关键 (插入 L55):
  ```csharp
  // 20. SkillEngine helper async polling adapter (Phase 27B 船长加入)
  public sealed record AwaitSkillHelper(Func<Task<bool>> Probe, int? TimeoutMs = null) : StepCommand;
  ```
- 涉及原语: +AwaitSkillHelper (第 20 个)
- 验证: grep AwaitSkillHelper × 1; 字段命名 PascalCase (lesson 8c5fe86 Conditional positional 字段)

### Row 2: StepMachineRunner.cs InterpretAsync switch +1 case + private method

- 文件: `Dota2Simulator/GameAutomation/Application/StepMachine/StepMachineRunner.cs`
- line range A (switch case): L109 之后, L113 `_ => throw` 之前. L110-112 sot-annotator 注释段代码落地后可删
- new switch fragment:
  ```csharp
  Delay d => await InterpretDelay(d).ConfigureAwait(false),
  AwaitSkillHelper a => await InterpretAwaitSkillHelper(a).ConfigureAwait(false),
  _ => throw new InvalidOperationException($"Unknown StepCommand: {cmd.GetType().Name}")
  ```
- line range B (private method): L295 (InterpretDelay 之后)
- new method:
  ```csharp
  private static async Task<bool> InterpretAwaitSkillHelper(AwaitSkillHelper a)
  {
      if (a.TimeoutMs is int t && t > 0)
      {
          Task<bool> probeTask = a.Probe();
          Task winner = await Task.WhenAny(probeTask, Task.Delay(t)).ConfigureAwait(false);
          return winner == probeTask && await probeTask.ConfigureAwait(false);
      }
      return await a.Probe().ConfigureAwait(false);
  }
  ```
- 涉及原语: AwaitSkillHelper case + private static async method
- 验证: switch case 列序保留 20 case + 1 throw; method static 不调 _skillEngine / _hero / _itemEngine (verify INV15)

### Row 3: 船长Strategy.cs 删 _全局模式e_lock static field (INV17 落地)

- 文件: `Dota2Simulator/GameAutomation/Heroes/Strength/船长Strategy.cs`
- line range: L19-21 (含 sot-annotator 注释 + `private static readonly Lock _全局模式e_lock = new();`)
- new: (整段删)
- 方案 verify: 方案 B Runner 单 machine 内序列化 OK; 但跨 machine 触发 (captain-Q + captain-E) 由 HeroPlan.DispatchAsync L466 Task.Run fire-and-forget 包裹, 跨 machine 理论上可能并发. ⚠️ 详 §3 R8-B + 方案 B 实证段.

### Row 4: 船长Strategy.cs BuildPlan Q clause 迁 .StepMachine

- 文件: 船长Strategy.cs
- line range: L23-32 现 `.OnKey(Keys.Q).CustomProbe(洪流接x回)`
- new:
  ```csharp
  .OnKey(Keys.Q).NoProbe().StepMachine("captain-Q", sub => sub
      .Initial(0)
      .Step(0, step => step.Do(
          new AwaitSkillHelper(() => _skill.主动技能释放后续(Keys.Q, () =>
          {
              _main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
              _main._聚合.Skills.SetTime(SlotKey.Q, Common.获取当前时间毫秒());
              if (!_main.Session!.IsPaused && _main._聚合.Skills.Step(SlotKey.E) == 1)
              {
                  Common.Delay(1350, _main._聚合.Skills.Time(SlotKey.Q));
                  _main._聚合.Conditions[ConditionSlotKey.C4].Active = false;
                  Press(Keys.E);
              }
          }))
      ))
  )
  ```
- 涉及原语: NoProbe + StepMachine + AwaitSkillHelper (内嵌单原语)
- ⚠️ inner callback 真相 (R3-A blocker 必明示): 上述 inner action callback 内 `Press(Keys.E)` 是 `HeroStrategyBase` instance method (走 `_input.Press`), **不是** Press StepCommand record. `Common.Delay(1350, ...)` 是静态调用, **不是** Delay 原语 record. 该 callback 由 `SkillEngine.主动技能释放后续(Keys, Action afterAction)` 内部 `_ = Task.Run(afterAction).ConfigureAwait(true)` fire-and-forget 包装 (SkillEngine.cs:1382, afterAction 签名是 `Action`, 非 `Task`), **不经 Runner 调度**.
- ⚠️ 关键决断: `Delay(1350)` hardcode 不迁 Delay 原语 — 该 Delay 在 inner action callback 内, 不是 Runner 调度的 StepCommand 序列. INV3 hardcode Delay 零回归范围 = 业务侧 Strategy.cs **顶层 method** (不含 SkillEngine helper inner action callback closure). 同理 `Common.Delay(1350)` + `Press(E)` + `Conditions[C4].Active = false` + `Skills.SetTime` 都保留 inline. plan §2 INV3 措辞精确化.
- 验证: grep `_skill\.主动技能释放后续` 在 船长Strategy.cs **顶层 method** × 0 (helper 调用迁到 AwaitSkillHelper.Probe lambda 内); grep `private async Task<bool> 洪流接x回` × 0 (method 删)

### Row 5: 船长Strategy.cs BuildPlan E clause 迁 .StepMachine

- 文件: 船长Strategy.cs
- line range: L24-33 现 `.OnKey(Keys.E).WhenStepNotEq(SlotKey.E, 1).CustomProbe(x释放后相关逻辑)`
- new:
  ```csharp
  .OnKey(Keys.E).WhenStepNotEq(SlotKey.E, 1).NoProbe().StepMachine("captain-E", sub => sub
      .Initial(0)
      .Step(0, step => step.Do(
          new AwaitSkillHelper(() => _skill.主动技能释放后续(Keys.E, () =>
          {
              int 步骤e = _main._聚合.Skills.Step(SlotKey.E);
              if (步骤e == 1) return;
              _main._聚合.Skills.SetTime(SlotKey.E, Common.获取当前时间毫秒());
              if (_main._聚合.Skills.Step(SlotKey.R) == 1)
              {
                  Press(Keys.R);
                  _main._聚合.Skills.SetStep(SlotKey.R, 0);
              }
              _main._聚合.Skills.SetStep(SlotKey.E, 1);
              _main._聚合.Conditions[ConditionSlotKey.C3].Active = true;
              int 等待时间 = (int)Math.Floor(3000 * _main._聚合.Attack.状态抗性倍数) - 1670;
              Common.Delay(等待时间, _main._聚合.Skills.Time(SlotKey.E));
              _main._聚合.Conditions[ConditionSlotKey.C4].Active = true;
          }))
      ))
  )
  ```
- 涉及原语: NoProbe + StepMachine + AwaitSkillHelper (内嵌单原语, 同 Row 4 对称)
- ⚠️ inner callback 真相 (R3-A blocker 必明示): 上述 inner action callback 内 `Press(Keys.R)` 是 `HeroStrategyBase` instance method (走 `_input.Press`), **不是** Press StepCommand record. `Common.Delay(等待时间, ...)` 是静态调用, **不是** Delay 原语 record. 该 callback 由 `SkillEngine.主动技能释放后续(Keys, Action afterAction)` 内部 sync invoke afterAction (afterAction 签名是 `Action`, 非 `Task`), **不经 Runner 调度**. 因此 callback 内不存在"原语对应"映射, 整段保留 inline.
- ⚠️ 关键决断 A (偏离 SOT): SOT 切片 §落点 1NF 第 5 行建议拆 6 原语 (Conditional + Press + SetStep + ActivateClause + DynamicDelay) = 过设计 + 偏离. 拆 6 原语需要 Conditional.Cond 签名扩访问 _hero.Aggregate.Skills (现 Func<ImageHandle, bool> 无法访问 Skills) — 破 INV4 + 19+1 原语稳定. 倾向单 AwaitSkillHelper + 整 callback inline.
- ⚠️ 关键决断 B: lock 删除依据 = inner callback 同步执行天然无并发. R8-B 跨 machine 并发风险独立存在 (§3 R8-B). C3 / C4 ConditionSlot 副作用由 Row 7-8 `.RegisterProbe` async lambda 处理 cycle 内 reset (D2 重决断后路径), Row 5 inner callback 仅 set Active = true (D1 callback 内例外延续).
- 验证: grep `private async Task<bool> x释放后相关逻辑` × 0; grep `lock \(_全局模式e_lock\)` × 0

### Row 6: 船长Strategy.cs BuildPlan D2 Execute 保留不动

- 文件: 船长Strategy.cs
- line range: L25-29 (`.OnKey(Keys.D2).Execute(...)`)
- 改动: 无 — Execute 与 .StepMachine 路径正交, scope 仅 4 技能键
- 验证: D2 lambda 不变: `Skills.SetStep(R,1) + Press(E)`

### Row 7: 船长Strategy.cs RegisterProbe C3 — 保留 DSL + 内嵌 async lambda (D2 重决断落实)

- 文件: 船长Strategy.cs
- line range: L30 `.RegisterProbe(ConditionSlotKey.C3, x2次释放后)` + L84-94 method `x2次释放后`
- **Ground truth verify** (修订必查): `ConditionSlot.cs:11` 真实签名 `public delegate Task<bool> ConditionDelegateBitmap()` — async 无参. `RegisterProbe(slot, ConditionDelegateBitmap)` 直接接 async delegate. 船长 4 helper `private async Task<bool> X()` 完美匹配, **无须扩任何 DSL 维度**.
- D2 决断 (用户 confirm): 保留 `.RegisterProbe` 原 DSL, lambda 内嵌 async 包. 落点 1 行.
- old:
  ```csharp
  .RegisterProbe(ConditionSlotKey.C3, x2次释放后)
  ```
  + L84-94 method `x2次释放后` body
- new:
  ```csharp
  .RegisterProbe(ConditionSlotKey.C3, async () => await _skill.主动技能进入CD后续(Keys.E, () =>
  {
      Common.Delay(2000);
      _main._聚合.Skills.SetStep(SlotKey.E, 0);
  }).ConfigureAwait(true))
  ```
- 涉及原语: **无新原语** (DSL 0 变, AwaitSkillHelper StepCommand 在此**未使用** — 仅 Row 4-5 BuildPlan .StepMachine 内使用)
- ⚠️ INV3 hardcode Delay 决断同 Row 4: inner action callback 内 `Common.Delay(2000)` 在 helper 内部 closure, 不计 INV3 顶层零回归 (D1 inner-callback 例外延续)
- BuildPlan 链改动: 仅 lambda 改写 (1 行 vs 1 行); method `x2次释放后` 整 method 删 (Row 9)
- 验证: grep `private async Task<bool> x2次释放后` × 0; grep `RegisterProbe\(ConditionSlotKey\.C3` × 1 (DSL 保留); grep `_skill\.主动技能进入CD后续` 在 船长Strategy.cs **顶层 method** × 0 / 在 .RegisterProbe lambda 内 × 1 (Probe 自包含的正确形态, 同 INV14 callback 内不计原则)

### Row 8: 船长Strategy.cs RegisterProbe C4 — 保留 DSL + 内嵌 async lambda (D2 重决断落实)

- 文件: 船长Strategy.cs
- line range: L31 `.RegisterProbe(ConditionSlotKey.C4, 立即释放洪流)` + L97-100 method `立即释放洪流`
- **Ground truth verify** (同 Row 7): `ConditionDelegateBitmap` 真实签名 = async `delegate Task<bool>()`, `RegisterProbe` 直接接 async lambda. 原 plan "设计阻塞 6 方案各有缺陷" 论断**整段失效** — 实际 0 阻塞, 1 行直改.
- D2 决断 (用户 confirm): 保留 `.RegisterProbe` 原 DSL, lambda 内嵌 async 包. 落点 1 行.
- old:
  ```csharp
  .RegisterProbe(ConditionSlotKey.C4, 立即释放洪流)
  ```
  + L97-100 method `立即释放洪流` body
- new:
  ```csharp
  .RegisterProbe(ConditionSlotKey.C4, async () => await _skill.主动技能已就绪后续(Keys.Q, () => Press(Keys.Q)).ConfigureAwait(true))
  ```
- 涉及原语: **无新原语** (DSL 0 变, AwaitSkillHelper StepCommand 在此**未使用**)
- BuildPlan 链改动: 仅 lambda 改写 (1 行 vs 1 行); method `立即释放洪流` 整 method 删 (Row 9)
- 验证: grep `private async Task<bool> 立即释放洪流` × 0; grep `RegisterProbe\(ConditionSlotKey\.C4` × 1 (DSL 保留); grep `_skill\.主动技能已就绪后续` 在 船长Strategy.cs **顶层 method** × 0 / 在 .RegisterProbe lambda 内 × 1 (callback 内不计, 同 INV14 原则)

### Row 9: 4 method 删 (含全部 sot-annotator 注释段)

- 文件: 船长Strategy.cs
- line range: L43-100 (4 method: 洪流接x回 / x释放后相关逻辑 / x2次释放后 / 立即释放洪流)
- new: (全数删)
- 验证: 船长Strategy.cs 总行数 ≈ L1-42 (header + using + class 开头) + BuildPlan .StepMachine 闭包 captain-Q + captain-E (~30 行, 各 1 个 Step 含 inline callback) + RegisterProbe(C3/C4) 内嵌 async lambda (~6 行) + #endif ≈ 60-80 行 (vs 现 102 行净减 ~30 行). Row 7-8 D2 重决断后 epic 落点缩减 (无 captain-E Step(1) / 无新 DSL 维度), 比原 plan 估计省 ~10 行

### Row 10: .editorconfig 加 VSTHRD110 = error 过滤路径

- 文件: .editorconfig
- line range: L1 头部 comment 改 + 加 1 新 section
- old (L1): `# Phase 27A retry 2 S5 (2026-05-26): VSTHRD110 Task.Run fire-and-forget 防回归 — 仅作用于 4 已迁 hero (S3 巫妖/骨法 + S4 军团 + S5 天怒).`
- new (L1): `# Phase 27A retry 2 S5 + Phase 27B (2026-05-26): VSTHRD110 Task.Run fire-and-forget 防回归 — 5 hero 迁 (4 hero 27A + 船长 27B).`
- 加 section:
  ```ini
  [GameAutomation/Heroes/Strength/船长Strategy.cs]
  dotnet_diagnostic.VSTHRD110.severity = error
  ```
- 涉及不变量: INV7
- 验证: S3 chunk build + grep `_ = Task\.Run` 在 船长Strategy.cs × 0 + VSTHRD110 命中船长 .cs

---

## §2 18 不变量 + 验证策略 (27A 13 + 27B 5)

| # | 内容 | 验证手段 |
|---|---|---|
| INV2 | 业务侧 Task.Run 零回归 (5 hero 完结) | grep `Task\.Run` 在 Heroes/Strength/船长Strategy.cs × 0 (业务侧). wiring 内 L466 Task.Run 不计 (Phase 27A 既成事实) |
| INV3 | hardcode `Common.Delay(\d{3,})` 在**业务侧顶层 method 内**零回归 | grep `Common\.Delay\(\d{3,}` 在 船长Strategy.cs **顶层 method 内 (BuildPlan 顶层链外)** × 0. inner action callback closure 内 Delay 保留 (helper 内部 closure 段, 同 INV2 wiring 内部不计原则). plan §1 Row 4-5 决断 + §10 escape hatch 表第 5 行 |
| INV4 | StepCommand switch exhaustive | build verify CS8509 = 0 (S1 chunk). switch 20 case + 1 throw |
| INV5 | sub-builder 8 维不破 | grep `StepMachineSubBuilder` method count = 8 (0 method 增删). 27B epic 0 新增 DSL 维度 (D2 重决断后无方案 H) |
| INV6 | SG 0 改 | Get-FileHash HeroStrategyGenerator.cs 自 5863da8 不变 |
| INV7 | lint VSTHRD110=error 命中船长 | .editorconfig 加船长路径 (Row 10) + build verify |
| INV8 | SlotKey.Global 直写 0 | grep `SlotKey\.Global` 在 船长Strategy.cs × 0 |
| INV10 | Conditions[name].Active 直写 0 | grep `Conditions\[ConditionSlotKey\.(C3\|C4)\]\.Active\s*=` 在 船长Strategy.cs **BuildPlan 顶层链 / 顶层 method 内 (BuildPlan 链外)** × 0; inner action callback closure 内保留 (D1 决断: callback 由 SkillEngine `_ = Task.Run(afterAction)` fire-and-forget 包装, 不经 Runner) |
| INV11 | 业务零感知 Runner | grep `new StepMachineRunner` 在 Heroes/ × 0 |
| INV12 | 业务侧只读不写状态机 | grep `_main\._聚合\.StepMachines\.SetStep` 在 船长Strategy.cs × 0 |
| INV13 | wiring 路径活 | build verify (Debug/Release 2 档, DOTA2+Silt 激活) 0 error |
| INV14 | SkillEngine helper 直调零回归 (船长) | grep `_skill\.(主动技能释放后续\|主动技能进入CD后续\|主动技能已就绪后续)` 在 船长Strategy.cs **BuildPlan 顶层链 / 顶层 method 内 (RegisterProbe/AwaitSkillHelper lambda 外)** × 0. AwaitSkillHelper.Probe lambda 内调用 + RegisterProbe(C3/C4) async lambda 内调用 都不计 (Probe 自包含 + RegisterProbe 内嵌 async, 均为 D2 决断正确路径) |
| INV15 | AwaitSkillHelper Probe 自包含 | grep `_skillEngine` 在 InterpretAwaitSkillHelper × 0 (verify static method 无 instance 依赖) |
| INV16 | 跨 clause 副作用走原语 (BuildPlan 顶层链不直写) | 船长**BuildPlan 顶层链 / 顶层 method 内 (lambda 外)** `Conditions[X].Active =` × 0. inner action callback closure 内保留 (Row 4-5 决断). 命名精确化为 "BuildPlan 顶层链不直写; callback 内例外" |
| INV17 | _全局模式e_lock 删除 | grep `_全局模式e_lock` 在 船长Strategy.cs × 0 (HeroLoopHost.cs L44 同名 field 是 Phase 27A 既成事实, 不动) |
| INV18 | escape hatch documented | plan §10 文档化 4 + 1 行 (含 D1 callback 内例外 + D2 RegisterProbe async lambda 例外) |

INV9 删 (Phase 27A 已 documented); INV1 SOT 切片未列.

---

## §3 R6-R10 风险预案

### R6 Runner ctor 5 参数稳定

- 触发条件: subagent 在 S1 chunk 改 StepMachineRunner ctor 加第 6 参数
- 早期检测: build verify S1 chunk + grep `public StepMachineRunner\(` 应仅 1 出现 + 5 参数
- 兜底回退: BLOCKED file `.claude/BLOCKED_S1_runner_ctor_drift.md` + chunk 0 改动 + 主 lead grill subagent

### R7 SkillEngine 内部不动

- 触发条件: subagent 在 S2 chunk 改 SkillEngine.cs L1375/1395/1415 三 helper method 实现
- 早期检测: Get-FileHash SkillEngine.cs 自 S2 chunk 起点不变
- 兜底回退: BLOCKED file + chunk 0 改动 + grill 复述 (scope 仅业务侧 BuildPlan 迁, handoff §3106)

### R8 tick 节奏 / Task.Run 并发

- 触发条件 A: AwaitSkillHelper 在主 tick 33ms 等待期阻塞 → 主循环卡顿
- 早期检测 A: 用户冒烟 5 分钟剧本 (S3 chunk 验证) 报无卡顿
- 触发条件 B (新发现): HeroPlan.DispatchAsync L466 `_ = Task.Run(async () => await runner.ExecuteAsync(def))` fire-and-forget. 同 hero 多 .StepMachine clause 各自 Task.Run → 用户连按 Q + E → 两 Task.Run 并发跑两 machine → 跨 machine 共享 _main._聚合.Conditions[C3/C4] / Skills state → race condition
- 早期检测 B: 用户冒烟剧本特意制造 Q + E 高频交替按键 (5 分钟 ≥ 50 次切换) → 测 Skills.Step / Conditions.Active 跨 machine 写入有无错乱 (TTS / 假腿切换 状态紊乱征兆)
- 兜底回退 B: 若 R8-B 实证, 退回保留 _全局模式e_lock instance field (HeroLoopHost.cs L44 模式), 业务侧 inner callback 内 lock 包 — 但这超 27B scope, 应推 27C 独立 epic

### R9 (修订) S2 BLOCKED file 触发条件 — 三类

修订: R9 不再特定指 Row 8 C4 设计阻塞 (D2 重决断后该 sub-risk 消解). 改为 S2 chunk subagent 通用 BLOCKED file 触发协议三类:

- **触发条件 9a (Row 5 inner callback 拆 6 原语决断回退)**: subagent 强行按 SOT 切片 §落点 1NF 第 5 行原义拆 6 原语 (Conditional + Press + SetStep + ActivateClause + DynamicDelay), 遇 Conditional.Cond 签名 `Func<ImageHandle, bool>` 无法访问 _hero.Aggregate.Skills 而 cargo build 破
- **触发条件 9b (R8-B race condition 冒烟实证)**: S3 chunk 用户冒烟 Q+E 50 次切换时 Skills/Conditions state 错乱 (TTS 重复 / 假腿不切 / 状态紊乱征兆)
- **触发条件 9c (Runner ctor 漂移)**: subagent 在 S1 chunk 改 StepMachineRunner ctor 加第 6 参数 (R6 触发)
- 早期检测: subagent 自检 plan + 创建 `.claude/BLOCKED_<chunk>_<symptom>.md` 0 改动退出
- 兜底回退: 主 lead grill 决断 — 9a 接 inline callback 决断 (plan §6 偏离 1 措辞) / 9b 退保留 lock instance field (推 27C 独立 epic) / 9c BLOCKED file + scope 复述

### R10 (修订) Row 4-5 inner callback 决断 plan-reviewer 必审

- 触发条件: plan-reviewer R3 不接受 inner action callback 内保留 Common.Delay / Conditions.Active 直写决断, 要求拆 6 原语
- 早期检测: plan-reviewer 报告内 INV3/INV10/INV16 措辞 inner-callback 例外不可接受
- 兜底回退: 主 lead 二次 grill 用户决断: (a) 接 inner callback 例外, 不变量措辞精确化; (b) 全数拆原语, 接受 .StepMachine 范式重写 callback 复杂度 (需先解 Conditional.Cond 签名扩问题)

### R11 (info-grade) inner action callback 内异常路径

- 触发条件: callback 内 NRE (e.g. `_main._聚合.Skills.SetTime` 时 `_main._聚合` 为 null / `_main.Session!` null force unwrap / `Press(Keys.E)` 在 _input 未初始化时) — Row 4-5 callback 直接抛
- 早期检测: 运行时 `Common.Main_Logger.Error` 报 NRE (HeroPlan.DispatchAsync L466 catch wrap 兜底, 但 inner callback 直跑在 SkillEngine helper async path 内, 异常路径不同). S3 冒烟剧本中实测可发现
- 兜底回退: S2 chunk 实施时 inner callback 内**首行加 defensive null check** (`if (_main?._聚合 == null) return;`), 或 S3 冒烟实证后 try-catch wrap. Phase 27A 4 hero callback 路径无此防护 (历史既成), 27B 沿用同形态; info-grade 仅记录, 不做硬阻断
- R11 vs R8-A 关系 (plan-reviewer 二审 INFO-3 明示): inner callback NRE 通过 probeTask 异常路径返 `Task.WhenAny` winner == probeTask 即 false (Runner InterpretAwaitSkillHelper L67-68 `winner == probeTask && await probeTask` 表达式 short-circuit, NRE 抛出导致第 2 段 await 抛, 但 winner == probeTask 已 true). R8-A 超时 (winner != probeTask, winner == Task.Delay) 和 R11 NRE (winner == probeTask 但 await 抛) 在 winner 比较上汇合, 不会双重失败堆叠

---

## §4 lint / deny 改动 + dogfood

### lint 改动

仅 Row 10 (.editorconfig 加船长 VSTHRD110=error 过滤路径). 不引入新 analyzer / 新 rule.

### dogfood 验证 (S3 chunk 执行, plan agent 不真改业务代码不真 build)

- build verify (Debug/Release 2 档, DOTA2+Silt 激活) 全 0 error 0 warning (LOL/HF2 不在 csproj 编译矩阵, 切 DefineConstants build 留 S4 用户冒烟)
- grep verify (主 lead S3 chunk 内执行):
  - INV2 `Task\.Run` 在 船长Strategy.cs × 0
  - INV3 `Common\.Delay\(\d{3,}` 在 船长Strategy.cs **顶层 method 内 (BuildPlan 顶层链外, RegisterProbe/AwaitSkillHelper lambda 外)** × 0 (plan-reviewer 二审 INFO-2 措辞精确化: lambda 内 inner callback 合法命中不计为 false alarm)
  - INV10 `Conditions\[ConditionSlotKey\.(C3\|C4)\]\.Active\s*=\s*(true\|false)` 在 船长Strategy.cs **顶层 method 内 (BuildPlan 顶层链外, RegisterProbe/AwaitSkillHelper lambda 外)** × 0
  - INV14 `_skill\.(主动技能释放后续\|主动技能进入CD后续\|主动技能已就绪后续)` 在 船长Strategy.cs **顶层 method 内 (BuildPlan 顶层链外, RegisterProbe/AwaitSkillHelper lambda 外)** × 0
  - INV17 `_全局模式e_lock` 在 船长Strategy.cs × 0

⚠️ plan agent **本次不自跑 dotnet build** (plan agent 不真改业务代码, build verify 是 S1-S4 chunk 主 lead 职责).

### 用户冒烟 (S3 + S4 chunk)

- S3 chunk 5 分钟实剧本: 船长 Q (洪流) + E (X标记) + EQ/QE 组合 + 假腿切换 + 命石. 特意制造 R8-B race condition 测试场景 (Q + E 高频切换 ≥ 50 次)
- S4 chunk 27A 4 hero 全形态冒烟 + Phase 26 抽样 4 属性英雄回归

---

## §5 chunk 拓扑 (4 chunk 全串行 + 主 lead 显式 wave schedule)

### chunk 详表

```toml
[[chunk]]
id = "S1"
description = "Domain 层 + Runner 层基建"
files = [
  "Dota2Simulator/GameAutomation/Domain/StepMachine/StepCommand.cs",
  "Dota2Simulator/GameAutomation/Application/StepMachine/StepMachineRunner.cs",
]
expected_loc = "+15 / -10"
反预测 = "build verify (Debug/Release 2 档, DOTA2+Silt 激活) 全 PASS; 4 hero 行为 0 变化"
反预测_failure_signal = "S1 commit build 任一档 error / CS8509 复现 / 4 hero 任一 strategy 编译破坏"
blocks = []
blocked_by = []
abstraction_layer = 0
commit_subject = "Phase27B-S1 StepCommand +AwaitSkillHelper + Runner +InterpretAwaitSkillHelper (第 20 原语)"

[[chunk]]
id = "S2"
description = "船长 BuildPlan 迁: Q/E .StepMachine + C3/C4 RegisterProbe 内嵌 async lambda + 删 4 method + 删 lock"
files = [
  "Dota2Simulator/GameAutomation/Heroes/Strength/船长Strategy.cs",
]
expected_loc = "+25 / -65 (净减 ~40 行; D2 重决断后落点缩减: C3/C4 仅 lambda 改写 1 行 × 2, 无 captain-E Step(1) 整合)"
反预测 = "build verify (Debug/Release 2 档, DOTA2+Silt 激活) 全 PASS; grep INV2/INV3/INV10/INV14/INV17 全 0 (顶层 / lambda 外); RegisterProbe 仍命中 C3/C4 × 各 1"
反预测_failure_signal = "R9a Row 5 inner callback 拆 6 原语决断回退 / R9c Runner ctor 漂移 / build error / VSTHRD002 命中 RegisterProbe lambda (sync-over-async 反例) — 均创建 BLOCKED file"
blocks = []
blocked_by = ["S1"]
abstraction_layer = 1
commit_subject = "Phase27B-S2 船长Strategy 迁 .StepMachine (captain-Q + captain-E) + C3/C4 RegisterProbe 内嵌 async lambda + 4 method 删 + lock 删"

[[chunk]]
id = "S3"
description = "lint 加 + grep verify + 用户冒烟"
files = [
  ".editorconfig",
]
expected_loc = "+2 / -1"
反预测 = "lint 命中船长 VSTHRD110 不报错 (S2 已迁 0 Task.Run); 用户冒烟 5 分钟剧本 PASS (含 R8-B race 测试)"
反预测_failure_signal = "VSTHRD110 报错船长 .cs 残留 Task.Run (S2 漏迁); 或冒烟 NRE / 卡死 / Q+E 切换状态错乱"
blocks = []
blocked_by = ["S2"]
abstraction_layer = 2
commit_subject = "Phase27B-S3 .editorconfig 船长 VSTHRD110=error + 冒烟剧本 PASS"

[[chunk]]
id = "S4"
description = "27A 4 hero + Phase 26 抽样回归冒烟"
files = []
expected_loc = "0 / 0"
反预测 = "27A 4 hero 全形态冒烟 PASS; Phase 26 抽样 4 属性英雄 + 物品 / ReplaceIcon / KeepLeg 全 PASS"
反预测_failure_signal = "任一 hero 行为退化 → BLOCKED file + 主 lead 决断 revert"
blocks = []
blocked_by = ["S3"]
abstraction_layer = 3
commit_subject = "Phase27B-S4 整轮回归 PASS (27A 4 hero + Phase 26 抽样 0 退化)"
```

### 构建顺序自检

- S1 abstraction_layer = 0 (Domain + Application interpreter 最底层)
- S2 abstraction_layer = 1 blocked_by S1
- S3 abstraction_layer = 2 blocked_by S2
- S4 abstraction_layer = 3 blocked_by S3
- **无反向层依赖** PASS

### 主 lead spawn 指令 (强制串行 schedule, 单 chunk 单 wave)

**Wave 1 (单 chunk S1)**:
- S1: spawn general-purpose model=opus[1m] prompt=<self-contained brief 含 §1 Row 1-2 落点 + §2 INV4/INV5/INV15 + §3 R6 风险 + 反预测>

**Wave 1 收口条件**: S1 PASS (build verify 2 档 0 error + grep verify INV4 switch 20+1 case) → S2

**Wave 2 (单 chunk S2)** [blocked_by=[S1]]:
- S2: spawn general-purpose model=opus[1m] prompt=<self-contained brief 含: (a) §1 Row 3-9 落点 — Row 7-8 D2 重决断后**仅 lambda 改写 1 行 × 2**, 保留 RegisterProbe DSL 0 改动 (ConditionDelegateBitmap = async delegate Task<bool>() 已 verify, 详 frontmatter `references` + §6 偏离 2); (b) §2 INV2/INV3/INV10/INV14/INV16/INV17 措辞精确化 (BuildPlan 顶层链 / 顶层 method 内 lambda 外零回归; inner action callback closure + RegisterProbe async lambda 内例外); (c) §3 R7/R9/R10/R11 风险; (d) **明示禁项**: subagent 不许自扩 HeroPlanBuilder DSL 维度 / 不许把 C3 C4 整合到 captain-E machine Step(1) / 不许拆 Row 5 inner callback 为 6 原语 — 任一念头触发立即创建 BLOCKED file 0 改动退出; (e) 反预测>

**Wave 2 收口条件**: S2 PASS (build verify 2 档 + grep verify 5 INV) → S3

**Wave 3 (单 chunk S3)** [blocked_by=[S2]]:
- S3: spawn general-purpose model=opus[1m] prompt=<self-contained brief 含 §1 Row 10 落点 + §2 INV7 + S3 用户冒烟剧本 5 分钟 + R8-B race 测试场景 + 反预测>

**Wave 3 收口条件**: S3 PASS (lint 命中船长 + 用户冒烟 PASS) → S4

**Wave 4 (单 chunk S4)** [blocked_by=[S3]]:
- S4: spawn general-purpose model=opus[1m] prompt=<self-contained brief: 27A 4 hero 全形态冒烟脚本 + Phase 26 抽样 4 属性 + 反预测>

**Wave 4 收口条件**: 用户回报 PASS → step 5 architecture-sentinel spawn

**全 epic 完成态**: 所有 wave 收口 + handoff 更新 Phase 27B 完结 + 5 hero StepMachine 完结状态记录

### 全串行决断依据

- S1 → S2 真实依赖: S2 BuildPlan 引用 AwaitSkillHelper symbol, S1 不落 record 编译破坏
- S2 → S3 真实依赖: S3 lint VSTHRD110 命中船长需 S2 已迁完
- S3 → S4 真实依赖: S4 回归冒烟需 S3 lint gate 通过 + 用户冒烟基础 PASS
- **无可并行点** — attempt 1 wave 3 教训反向证实串行抗副作用

---

## §6 SOT 切片偏离声明

### 用户决断 (2026-05-26 plan-reviewer R1-A REJECT 后重决断 + confirm)

- **D1 偏离 1** (保留): ✅ 接 plan 推荐 — 单 AwaitSkillHelper + inline callback (INV3/INV10/INV16 措辞加 inner-callback 例外, §10 escape hatch documented 段落实)
- **D2 偏离 2 / Row 7-8** (作废原 + 重决断): ~~原决断 方案 H 扩 `HeroPlanBuilder.RegisterProbeAsync(ConditionSlotKey, Func<Task<bool>>)` 兄弟 method~~ **作废**. 实测 `ConditionSlot.cs:11` 签名 `public delegate Task<bool> ConditionDelegateBitmap()` 本就是 async, RegisterProbe 直接接 async lambda — 方案 H 不需要存在. **新决断**: C3 / C4 都保留 `.RegisterProbe` 原 DSL, lambda 内嵌 async 包 AwaitSkillHelper helper 调用. 落点 1 文件 1 行 × 2 (Row 7 + Row 8). epic 0 新增 DSL 维度
- **D3 偏离 3 / R8-B** (保留): ✅ 接 plan 推荐 — S3 冒烟 5 分钟 ≥ 50 次 Q+E 高频切换实证 race 后决断; INV17 (_全局模式e_lock 删除) 本 epic 落地; 若实证 race 退回 lock instance field 推 27C 独立 epic
- 三项决断 **冻结**, plan-reviewer R3 在此基础上审 plan 主体内一致性 + 落点完备

### 偏离 1: Row 5 E clause 拆 6 原语 → 单 AwaitSkillHelper + inline callback (D1 决断)

- SOT 切片原文 (§落点 1NF 第 5 行): `AwaitSkillHelper + Conditional + Press + SetStep + ActivateClause + DynamicDelay + ActivateClause`
- plan §1 Row 5 决断: 单 `AwaitSkillHelper(() => _skill.主动技能释放后续(E, () => { ... 全数 inline ... }))` 不拆原语
- 偏离理由: inner action callback 由 SkillEngine helper `_ = Task.Run(afterAction)` fire-and-forget 包装 (SkillEngine.cs:1382/1402/1433/1453/1473 五个 X后续 helper 同模式, afterAction 签名 `Action` 非 `Task`), **不经 Runner 调度**. callback 内 `Press(...)` 是 HeroStrategyBase instance method 不是 Press StepCommand record; `Common.Delay(...)` 是静态调用不是 Delay 原语 record. 拆 6 原语需扩 Conditional.Cond 签名访问 _hero.Aggregate.Skills (现 Func<ImageHandle, bool>) — 破 INV4 + 19+1 原语稳定
- 影响: INV3 / INV10 / INV16 措辞精确化为 "BuildPlan 顶层链 / 顶层 method 内 (lambda 外) 零回归; inner action callback closure 内保留"
- **plan-reviewer R3 必审** (R10 触发条件)

### 偏离 2 (整段重写): SOT 切片 + 原 plan 均未 verify ConditionSlot.cs:11 真实签名

- **撤回原 plan 误述**: 原 plan §6 偏离 2 论断 "RegisterProbe 签名 `ConditionDelegateBitmap = Func<ImageHandle, bool>` sync, 无法接 AwaitSkillHelper" — **事实错误**, 整段作废.
- **真实情况** (plan-reviewer R1-A blocker 实测 + 修订 Read verify):
  - `Dota2Simulator/GameAutomation/Application/ConditionSlot.cs:11` — `public delegate Task<bool> ConditionDelegateBitmap();` (async, 无参)
  - `HeroPlanBuilder.cs:787` — `RegisterProbe(ConditionSlotKey slot, ConditionDelegateBitmap probe)` 直接接 async delegate
  - 现有业务调用 `.RegisterProbe(C3, x2次释放后)` 中 `x2次释放后` 是 `private async Task<bool>` 方法, 完美匹配 async delegate
- **真实偏离根因**: sot-annotator 与 plan agent 均未 Read `ConditionSlot.cs:11` verify ConditionDelegateBitmap 真实签名, 仅凭 SOT 切片描述 + 训练印象推断为 sync. 触违铁律 5(b) Zoom-out + grill-me §动手前 fact 必查. 修订纪律: 凡引用现存 type/delegate/method signature 必先 Read/Grep verify, 不凭训练印象 / 不凭 SOT 切片描述 (上游 SOT 也可能错; SOT step 2 agent 同样跳过 verify)
- **D2 重决断落点** (Row 7-8 实施):
  ```csharp
  .RegisterProbe(C3, async () => await _skill.主动技能进入CD后续(E, () => { Common.Delay(2000); _main._聚合.Skills.SetStep(SlotKey.E, 0); }).ConfigureAwait(true))
  .RegisterProbe(C4, async () => await _skill.主动技能已就绪后续(Q, () => Press(Q)).ConfigureAwait(true))
  ```
- epic surface delta: 0 新增 DSL 维度 (vs 原 plan 倾向方案 H); 0 整合到 captain-E Step(1) (vs 原 plan 倾向方案 C2). 落点缩减 ~10 行
- **plan-reviewer R3 必审** (验证 D2 落点精确度)

### 偏离 3: R8-B Task.Run 跨 machine 并发是新发现风险

- SOT 切片原文 (§R6 R7 R8 风险预案): R8 = "tick 节奏" (单一 AwaitSkillHelper 阻塞 step), 未提跨 machine Task.Run 并发
- plan §3 R8-B 决断: HeroPlan.DispatchAsync L466 `Task.Run` 是 fire-and-forget. 同 hero 多 .StepMachine clause 各自 Task.Run → 跨 machine 共享 Aggregate state → race condition 实证缺口
- 偏离理由: SOT 切片 §INV17 "StepMachine 顺序语义天然替代 Lock" 仅对单 machine 内成立, 跨 machine 并发是真 race 缺口
- 影响: R8-B 用户冒烟特殊场景 50 次 Q+E 切换测试, 实证 race 即退回保留 lock instance field (推 27C 独立 epic)
- **plan-reviewer R3 必审**

---

## §7 SSOT 漂移点 (Phase 27A 同源延续)

> 27B SSOT 漂移面 = **0 项新增** (D2 重决断后 epic 0 新增 DSL 维度, 主 builder / sub-builder 均无 method 增删).

| 漂移点 | 位置 | 说明 |
|---|---|---|
| HeroPlan.DispatchAsync wiring hook 内 fire-and-forget Task.Run | HeroPlan.cs L466 | 27A 既成事实, 27B 不改; INV2 措辞精确化为业务侧零回归 |
| inner action callback closure 经 `_ = Task.Run(afterAction)` fire-and-forget 包装 vs Runner 异步调度 | SkillEngine.主动技能释放后续 内部 (SkillEngine.cs:1382 等 5 helper 同模式) | 两套范式真实存在 (均 fire-and-forget, 但执行入口不同); plan §6 偏离 1 重要 documented |
| RegisterProbe 既有 async delegate 签名 vs 原 plan 推断 sync 误述 | ConditionSlot.cs:11 (真) vs 原 plan §1 Row 7-8 (误) | sot-annotator + plan agent 未 verify 触违铁律 5(b), plan-reviewer R1-A 检出修订; epic 0 新增 DSL |
| _全局模式e_lock instance vs static | HeroLoopHost.cs L44 (instance) vs 船长Strategy.cs L21 (static) | 同名巧合, 不同实例; 27B 仅删船长 static 不动 HeroLoopHost |

---

## §8 DSL 维度增删表 (Phase 27A 同源延续 + 27B 微调)

| 维度 | 27A 状态 | 27B 改动 |
|---|---|---|
| HeroPlanBuilder.StepMachine(name, configure) | 加 (27A S2) | 不动 |
| HeroPlanBuilder.RegisterProbe(slot, probe) | 历史 (实测签名 `ConditionDelegateBitmap = async delegate Task<bool>()`, 直接接 async lambda) | 不动 — D2 重决断后 C3/C4 复用原 DSL, lambda 内嵌 async |
| StepMachineSubBuilder 8 维 | 加 (27A S2) | 不动 |
| StepCommand 原语集 | 19 (27A) | +1 AwaitSkillHelper = 20 |
| _全局模式e_lock 跨 clause Lock | static (历史) | 删 (船长) |

**delta 总结**: 27B epic surface = +1 StepCommand 原语 (AwaitSkillHelper) + 1 Runner case + 1 Runner private method + 6 lambda 改写 (船长 4 method → 2 .StepMachine inline + 2 RegisterProbe inline) + 1 lint 配置. **0 新增 DSL 维度** (撤回原 plan +1 HeroPlanBuilder.RegisterProbeAsync 行).

---

## §9 alternative 否决记录补充 (SOT 切片 + 新增)

| 方案 | 否决理由 |
|---|---|
| (SOT 6 项) Runner inline polling / 改 ctor / 加 sub-builder 第 9 维 / StepMachineState 跨锁 / 并发跨 hero / 重做 27A | SOT 切片 §设计 alternative 否决记录 6 项, 全数承接 |
| (新增) 方案 B2 lambda 内 Task.Run wrap + 立即 return false | 违 INV2 + INV14, 业务侧仍有 Task.Run + helper 直调 |
| (新增) 方案 D 新建 captain-Q-ready machine attach 到 `.OnKey(Keys.None)` placeholder | placeholder hack Phase 19C 已弃 (HeroPlanBuilder.cs L783 — 注: 是 placeholder DSL 路径弃用, 非 `Keys.None` enum 本身废弃) |
| (新增) 方案 E RegisterProbe lambda 内 GetAwaiter().GetResult() sync-over-async | **不需要** — `ConditionDelegateBitmap` 实测为 async `delegate Task<bool>()`, RegisterProbe 直接接 async lambda 无须 sync→async 适配. 原 plan 否决理由 (VSTHRD002 + block UI) 仍成立但前提失效 |
| (新增) 方案 F C4 RegisterProbe 全删 | 业务退化 (自动连按洪流功能丢失) |
| (新增) 方案 G 复刻 SkillEngine 内部到 lambda | 违 DRY |
| ~~(原 plan 新增) 方案 H 扩 HeroPlanBuilder.RegisterProbeAsync 兄弟 method~~ | **撤回** — `ConditionDelegateBitmap` 实测本就 async, 方案 H 不需要存在. D2 重决断后 0 新增 DSL 维度 |
| (新增) E clause 拆 6 原语 (SOT Row 5 原义) | 需扩 Conditional.Cond 签名访问 _hero.Aggregate, 破 INV4 + 19+1 原语稳定 |

---

## §10 escape hatch documented (INV18 完整版)

承接 SOT 切片 §escape hatch documented 扩 4 行表 + 加 1 行船长本 epic 特化:

| 位置 | 是否合法 | 理由 |
|---|---|---|
| .StepMachine(name, sub => ...) 闭包内 Do() 序列 | 合法 (setup 段) | builder 阶段定义, 非 runtime predicate |
| BuildPlan .OnKey().PreAsync(...) 桥接 (船长当前无, 军团 + 天怒 有) | 合法 (Pre 段) | Pre 段在 clause Active 设置前 await, 桥接业务侧旧路径 |
| .StepMachine Step(n).Do(...) 序列内 SetStep / ActivateClause 原语 | 合法 (原语接受) | 业务侧通过原语 (而非直接 _state.SetStep) 修改状态, 经 Runner 受控调度 |
| 业务 method body 内 _main._聚合.StepMachines.SetStep(...) 直接调用 (predicate 段) | 违规 (INV12) | predicate 内不允许写状态机, 应通过 ActivateClause / SetStep 原语在 .StepMachine 闭包内定义 |
| (新增 D1) AwaitSkillHelper.Probe lambda 内 inner action callback closure 内副作用 (Conditions[X].Active = / SetStep / SetTime / Common.Delay / Press) | 合法 (helper 内部 closure 段) | SkillEngine.主动技能释放后续 await 完成后 `_ = Task.Run(afterAction).ConfigureAwait(true)` fire-and-forget 包装 callback (SkillEngine.cs:1382), 不走 Runner 调度 → 不破 INV12 业务侧只读 (业务侧定义 callback, 但运行时执行入口在 SkillEngine 内通过 Task.Run 异步触发). plan §6 偏离 1 documented |
| (新增 D2) `.RegisterProbe(slot, async () => await _skill.XX(K, () => {...}))` 内嵌 async lambda + inner action callback | 合法 (RegisterProbe async lambda 段 + 内嵌 helper closure 段) | `ConditionDelegateBitmap` 真实签名 `delegate Task<bool>()` 直接接 async lambda; lambda 内嵌 `_skill.主动技能X后续` helper 调用 + inner action callback 同 D1 例外. INV14 (SkillEngine helper 直调零回归) 措辞精确化为 "BuildPlan 顶层链 / 顶层 method 内 (lambda 外) 零回归; RegisterProbe async lambda 内 + AwaitSkillHelper.Probe lambda 内例外". plan §6 偏离 2 documented |
