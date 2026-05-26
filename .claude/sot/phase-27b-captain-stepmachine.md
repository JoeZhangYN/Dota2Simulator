---
epic: Phase 27B SkillEngine helper async polling adapter (船长 .StepMachine 迁移)
sot_step: 2 (sot-annotator mode A)
prev_epic: Phase 27A retry 2 (commit 5863da8, 7 commit on main)
generated_by: sot-annotator
generated_on: 2026-05-26
references:
  - .claude/handoff-hex-refactor.md §2962-3107 (Phase 27A retry 2 完整记录 + 27B grill 三问 SSOT)
  - .claude/CLAUDE.md §"Phase 27A retry 2 显式破坏点" 段 (INV5 sub-builder 破坏文档化)
  - Dota2Simulator/GameAutomation/Heroes/Strength/船长Strategy.cs (本 epic 主迁对象, 90 行)
  - Dota2Simulator/GameAutomation/Application/StepMachine/StepMachineRunner.cs (Phase 27A 落成, ctor 5 参数固定, 19 case)
  - Dota2Simulator/GameAutomation/Domain/StepMachine/StepCommand.cs (19 原语 v2, 本 epic +1)
  - Dota2Simulator/GameAutomation/Application/HeroPlan/StepMachineSubBuilder.cs (8 维 sub-builder, 不破)
  - Dota2Simulator/GameAutomation/Application/HeroPlan/HeroPlan.cs (DispatchAsync L335-483 wiring hook, 不破)
---

# Phase 27B 船长 .StepMachine 迁移 SOT 切片

> sot-annotator mode A: 正向标 + epic SOT 切片. 真实代码迁移 = step 4 impl 阶段.
> 严格不破 Phase 27A 4 hero (巫妖 / 骨法 / 军团 / 天怒) 已稳定代码 + 不改 Runner ctor signature + 不改 sub-builder 8 维 + 不改 HeroPlanClause record. 仅扩 1 个 StepCommand 子类型 + 1 个 InterpretAsync case.

## § 不变量 (基于 27A 13 不变量延伸 + 27B 新增 5)

继承 27A 不变量 (不改):
- INV2 Task.Run 零回归 (4 hero 不变, 船长**加入**该清单 → 27B 完结后 5 hero 0 fire-and-forget)
- INV3 hardcode Common.Delay(\d{3,}) 零回归 (船长当前 `Common.Delay(1350, ...)` × 2 + `Common.Delay(2000)` × 1 共 3 处需迁 Delay / DynamicDelay 原语)
- INV4 StepCommand switch exhaustive (本 epic 加 1 case AwaitSkillHelper, throw 兜底不动)
- INV5 sub-builder 8 维不破 (本 epic 不加新维度, 8 维已足够表达船长所有形态)
- INV6 SG 0 改 (HeroStrategyGenerator.cs sha256 自 27A 起点不变, 本 epic 延续)
- INV7 lint VSTHRD110=error (船长加入 .editorconfig 过滤路径, 命中 5 处)
- INV8 SlotKey.Global 直写 0 (船长当前无 Global 写, 本 epic 不引入)
- INV10 Conditions[name].Active 直写 0 (船长当前 4 处直写 → 全数迁 ActivateClause / DeactivateClause 原语)
- INV11 业务零感知 Runner (Heroes/ `new StepMachineRunner` = 0; 本 epic 延续, Runner 仅 wiring hook 内构造)
- INV12 业务侧只读不写状态机 (船长 PreActionAsync escape hatch — Pre/setup 内桥接 OK, predicate 内写 SetStep 禁)
- INV13 wiring 路径活 (build verify 4 阶段 PASS)

**Phase 27B 新增不变量** (5 项):

- **INV14 SkillEngine helper 直调零回归 (船长)** (v2 措辞精确化, plan-reviewer 二审 WARN-1 同步): 船长 .cs grep `_skill\.(主动技能释放后续|主动技能进入CD后续|主动技能已就绪后续)` 在 **BuildPlan 顶层链 / 顶层 method 内 (RegisterProbe/AwaitSkillHelper lambda 外)** = **目标 0**. RegisterProbe async lambda 内 (Row 7-8 D2 决断) + AwaitSkillHelper.Probe lambda 内 (Row 4-5 D1 决断) 调用都不计 (Probe 自包含 + RegisterProbe 内嵌 async 均为正确路径).
- **INV15 AwaitSkillHelper Probe 自包含** (v2 措辞精确化, plan-reviewer 二审 WARN-1 同步): AwaitSkillHelper.Probe : Func<Task<bool>> 业务侧直接传 `() => _skill.主动技能X后续(key, () => { ... })` lambda, Runner 内仅 `await a.Probe()` 不经 _skillEngine 字段调用. **仅 Row 4-5 BuildPlan .StepMachine 内使用**; **Row 7-8 RegisterProbe 路径不经 AwaitSkillHelper** (D2 重决断: C3/C4 保留 `.RegisterProbe` + 内嵌 async lambda 直调 helper, 0 新增 DSL 维度). Runner ctor 5 参数不动 (R6 mitigation: skillEngine 已注入但 AwaitSkillHelper case 不用 — 留 ctor 兼容 27A 4 hero).
- **INV16 跨 clause 副作用迁原语**: 船长 4 处 `_main._聚合.Conditions[Cn].Active = true/false` 直写 → 全数迁 ActivateClause("Cn") / DeactivateClause("Cn") 原语 (天怒 D3 已用 ActivateClause).
- **INV17 _全局模式e_lock 删除**: 单 hero 跨 clause 共享 Lock 实例迁 .StepMachine 顺序语义天然替代 (StepMachine OnEntry/OnExit/Commands 序列化执行无并发, 不需 Lock 约束). 27A INV9 删除合理 (不在 27A scope), 27B 落地删 Lock. 倾向方案 B (不引入 Local lock 注册表).
- **INV18 escape hatch documented 扩**: handoff §3097 真推迟项 — plan §7.3 escape hatch 应明示 "Pre/setup 内桥接 OK, predicate 内写 SetStep 禁". 船长无 Pre 段桥接需求 (与军团 / 天怒 不同), 但 BuildPlan 内 .StepMachine 闭包 Step(0).Do(...) sequence 是 setup 段不是 predicate 段, 属合法.

## § AwaitSkillHelper 原语签名规约

```csharp
// Dota2Simulator/GameAutomation/Domain/StepMachine/StepCommand.cs (S1 后扩, 第 20 个原语)
public sealed record AwaitSkillHelper(Func<Task<bool>> Probe, int? TimeoutMs = null) : StepCommand;
```

**签名设计要点**:
1. **Probe : Func<Task<bool>>** — 直接对齐 _skill.主动技能X后续 三 helper 返 `Task<bool>` 签名, 业务侧调用零适配 (.cs:223-225 已有 ConditionDelegateBitmap wrapper 反例参考).
2. **TimeoutMs : int?** — 可选, null = 无限等待 (helper 自身有 polling 完成条件); 数值 = 超时 abort 后续 cmd, 返 false 同 WaitForColor.
3. **不带 _skillEngine 字段访问** — Probe 自包含, Runner 仅 `await Probe()`, 不调 _skillEngine. 这与 InterpretUseItem 走 _itemEngine.根据图片使用物品 形态对称: 复杂度下沉到既有 helper, Runner 仅调度.

**Runner 落点** (`StepMachineRunner.cs` InterpretAsync switch, Delay 之后 throw 之前):

```csharp
AwaitSkillHelper a => await InterpretAwaitSkillHelper(a).ConfigureAwait(false),
// ...
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

## § 落点 1NF (文件 × 改动 × 原语映射)

| # | 文件 | 改动 | 涉及原语 |
|---|---|---|---|
| 1 | Domain/StepMachine/StepCommand.cs | + `record AwaitSkillHelper(Func<Task<bool>>, int?)` | 新 1 原语 |
| 2 | Application/StepMachine/StepMachineRunner.cs | InterpretAsync switch + 1 case + private `InterpretAwaitSkillHelper` method | AwaitSkillHelper case |
| 3 | Heroes/Strength/船长Strategy.cs L19 | 删 `_全局模式e_lock` static field | Lock 删 (INV17) |
| 4 | 船长Strategy.cs L23 `.OnKey(Q).CustomProbe(洪流接x回)` | → `.OnKey(Q).NoProbe().StepMachine("captain-Q", sub => sub.Initial(0).Step(0).Do(AwaitSkillHelper(...), DeactivateClause("C4"), ...).End())` | AwaitSkillHelper + DeactivateClause + (内 lambda 含 Conditional + Delay + Press) |
| 5 | 船长Strategy.cs L24 `.OnKey(E).WhenStepNotEq(SlotKey.E,1).CustomProbe(x释放后相关逻辑)` | → `.StepMachine("captain-E", sub => sub.Step(0).Do(AwaitSkillHelper(...) , Conditional(...,[Press(R),SetStep(R,0)],[]), SetStep(E,1), ActivateClause("C3"), DynamicDelay(_ => TimeSpan.FromMilliseconds(状态抗性倍数*3000-1670)), ActivateClause("C4")).End())` | AwaitSkillHelper + Conditional + Press + SetStep + ActivateClause + DynamicDelay |
| 6 | 船长Strategy.cs L25-29 `.OnKey(D2).Execute(...)` Step R=1 + Press E 跳船宏 | 保留 Execute (Execute 不挂 ConditionSlot, 与 .StepMachine 路径正交; scope 仅 4 技能键) | 不改 |
| 7 | 船长Strategy.cs L30 `.RegisterProbe(C3, x2次释放后)` | → **修订 (plan-reviewer R1-A REJECT + D2 重决断 2026-05-26)**: `ConditionDelegateBitmap = delegate Task<bool>()` 本就 async (ConditionSlot.cs:11 verify), RegisterProbe 直接接 async lambda. 落点 1 行: `.RegisterProbe(C3, async () => await _skill.主动技能进入CD后续(E, () => { Common.Delay(2000); _main._聚合.Skills.SetStep(SlotKey.E, 0); }).ConfigureAwait(true))`. **AwaitSkillHelper StepCommand 在此未使用** (仅 Row 4-5 BuildPlan .StepMachine 内使用) | RegisterProbe (DSL 不破) + 内嵌 async lambda 直调 helper |
| 8 | 船长Strategy.cs L31 `.RegisterProbe(C4, 立即释放洪流)` | → **修订 (同 Row 7)**: 落点 1 行: `.RegisterProbe(C4, async () => await _skill.主动技能已就绪后续(Q, () => Press(Q)).ConfigureAwait(true))`. **AwaitSkillHelper StepCommand 在此未使用** | RegisterProbe + 内嵌 async lambda 直调 helper |
| 9 | 船长Strategy.cs 4 method (洪流接x回 / x释放后相关逻辑 / x2次释放后 / 立即释放洪流) | 整体删 (lambda 内联到 .StepMachine Do() 序列 / RegisterProbe 包) | — |
| 10 | .editorconfig | 加 `Heroes/Strength/船长Strategy.cs` 到 VSTHRD110=error 过滤路径 | INV7 |

## § 与 27A 桥接 (严格不破)

**不破点 (5)**:
1. **Runner ctor signature** = `(state, hero, itemEngine, skillEngine, handle)` 5 参数, 27B 严禁改 (W3 attempt 1 S4 改 ctor 破 S5 build 教训反预测)
2. **StepMachineSubBuilder 8 维** = Initial / Step / OnEntry / OnExit / Local / Probe / WithLockMode / WithTickAlignment, 27B 不加第 9 维 (8 维已足够)
3. **HeroPlanClause record** = 26 字段含 StepMachineRefId, 27B 不加新字段 (Phase 26+27A 已稳定)
4. **HeroPlan.DispatchAsync wiring hook** = L454-478 含 `_clauses[i].StepMachineRefId is { } refId && skill is not null && _stepMachineDefinitions.TryGetValue(refId, out def)` 三件套, 27B 不改 (船长 captain-Q + captain-E 走同一 wiring 路径)
5. **InterpretAsync 19 原语 case 列序** = 27A v2 一次性整全, 27B 仅末尾插 1 case (Delay 后 throw 前), 不重排已有 case

**新增点 (3)**:
- StepCommand: +1 record `AwaitSkillHelper(Func<Task<bool>> Probe, int? TimeoutMs = null)`
- StepMachineRunner.InterpretAsync: +1 case (Delay 之后 throw 之前)
- StepMachineRunner: +1 private static async method `InterpretAwaitSkillHelper`

## § escape hatch documented 扩 (INV12 真推迟项)

handoff §3097 真推迟项: "plan §7.3 escape hatch documented 扩 (后续 plan 修订): Pre/setup 内桥接 OK 应明示 (军团 + 天怒 实证), 防 Phase 27B+ 误读 INV12 严格 '业务侧只读'".

**Phase 27B 落实**:

| 位置 | 是否合法 | 理由 |
|---|---|---|
| .StepMachine(name, sub => ...) 闭包内 Do() 序列 | ✅ 合法 (setup 段) | 在 builder 阶段定义 step 序列, 非 runtime predicate; 业务侧 `Skills.SetStep` / `Conditions[X].Active = ` 写入是配置时定义, Runner 解释时由 InterpretSetStep / InterpretActivateClause 原语统一驱动 (业务侧不直接写状态机) |
| BuildPlan `.OnKey().PreAsync(async () => 桥接复位)` (船长当前无, 军团 + 天怒 有) | ✅ 合法 (Pre 段) | Pre 段在 clause Active 设置前 await, 桥接业务侧旧路径 (e.g. 复位 Skills.SetStep) 是 escape hatch |
| .StepMachine Step(n).Do(...) 序列内出现的 SetStep / ActivateClause 原语 | ✅ 合法 (原语接受) | 业务侧通过原语 (而非直接 _state.SetStep) 修改状态, 经 Runner 受控调度 |
| 业务 method body 内 `_main._聚合.StepMachines.SetStep(...)` 直接调用 (predicate 段) | ❌ 违规 (INV12) | predicate 内不允许写状态机, 应通过 ActivateClause / SetStep 原语在 .StepMachine 闭包内定义 |

**船长 captain-Q + captain-E 全在 builder 闭包内, 不触违规 escape**.

## § R6 R7 R8 风险预案

- **R6 Runner ctor 5 参数稳定**: AwaitSkillHelper case 内 Probe 自包含, 不调 _skillEngine 字段 (虽已注入). Runner ctor 不动, S2-S5 严禁改铁律延续.
- **R7 SkillEngine.cs helper 不动**: 仅扩 Runner 原语, 不改 _skill.主动技能X后续 三 method 内部实现 (handoff §3106 in-scope 边界明示).
- **R8 tick 节奏**: AwaitSkillHelper 等待 helper 完成期间 Runner 阻塞当前 step (与 WaitForProbeTrue 形态对称). 不引入 BackgroundTask polling, 不破 33ms tick.

## § 设计 alternative 否决记录

| 方案 | 否决理由 |
|---|---|
| Runner 内 inline polling 实现 helper 等价逻辑 | handoff §3107(c) 明示否决: "不在 Runner 内 inline polling 实现, 复杂度下沉到 SkillEngine 已有 helper, Runner 仅调度" |
| 改 Runner ctor 加 _skill 直访 field 给 InterpretAwaitSkillHelper 用 | W3 attempt 1 S4 改 ctor 破 S5 build 教训反预测; AwaitSkillHelper.Probe 自包含已足够 |
| StepMachineSubBuilder 加第 9 维 `.AwaitSkillHelper(helper)` 直接生成 AwaitSkillHelper 原语 | 8 维已足够 (业务侧 `Do(new AwaitSkillHelper(() => _skill.X(...)))` 直接用 record ctor, 不需新维度); 加维度违 INV5 sub-builder 稳定 (W3 attempt 1 教训反预测) |
| StepMachineState 加跨 machine 锁注册表替代 _全局模式e_lock | 过设计, StepMachine 顺序语义天然替代 (方案 B 实证); 不引入新跨 hero 抽象 |
| 并发跨 hero 验证 (5 hero 同 epic) | handoff §3107(b) 明示否决: "不并发跨 hero 验证 (attempt 1 wave 3 全失败教训)"; 本 epic scope 仅船长 1 hero |
| 重做 Phase 27A 4 hero | handoff §3107(a) 明示否决: 4 hero 已稳定不动 |

## § Phase 27B chunk 拓扑预案 (step 3 plan agent 消费)

**全串行 4 chunk** (同 27A 全串行模式, 抗 attempt 1 跨 chunk 隐性副作用教训):

```
S1 Domain 层基建 (1 chunk)
   - StepCommand.cs +1 record AwaitSkillHelper
   - StepMachineRunner.cs +1 case + private method InterpretAwaitSkillHelper
   - 主 lead build verify 4 档 (DOTA2/Silt/LOL/HF2) 0 error, INV4 switch exhaustive 0 CS8509

S2 船长 BuildPlan 迁 (1 chunk)
   - 船长Strategy.cs BuildPlan 链全数迁 .StepMachine + RegisterProbe 包 AwaitSkillHelper
   - 删 4 method (洪流接x回 / x释放后相关逻辑 / x2次释放后 / 立即释放洪流) + 删 _全局模式e_lock field
   - 主 lead build verify 4 档 PASS + grep verify (INV2/INV3/INV10/INV14/INV17)

S3 lint 加 + 冒烟剧本 (1 chunk)
   - .editorconfig 船长 VSTHRD110=error 过滤路径加
   - 用户冒烟实剧本 5 分钟: 船长 Q (洪流) + E (X标记 + 跳船宏 D2 + 大招组合) + 假腿切换 + 命石 全形态
   - INV7 命中船长 .cs 验证

S4 整轮回归 (1 chunk)
   - 27A 4 hero (巫妖 / 骨法 / 军团 / 天怒) 全形态冒烟 (回归不破)
   - 整轮 Phase 26 冒烟 (抽样 4 属性英雄全技能键 + 物品 / ReplaceIcon / KeepLeg)
```

## § step 3 plan agent 应注意的关键约束

1. **scope 严格 1 hero** (船长), 不扩 5 hero (handoff §3106 明示 in-scope)
2. **不改 Runner ctor** (W3 attempt 1 教训反预测, R6 风险锁死)
3. **AwaitSkillHelper.Probe 自包含**, 不经 _skillEngine 字段调用 (Probe lambda 内可自由调 _skill, 但 Runner 不直访)
4. **scope 仅 .StepMachine 路径**, Combo Scheduler J 段独立 epic (handoff §3106 明示)
5. **不动 SkillEngine 内部实现** (R7), 仅扩 Runner 原语 + 业务 BuildPlan 调用形态
6. **删 _全局模式e_lock 是 INV17 落地** (StepMachine 顺序语义天然替代, 方案 B; 方案 A/C 已否决)
7. **D2 跳船宏 .Execute 保留** (与 .StepMachine 路径正交, scope 仅 4 技能键)
8. **plan §7.3 escape hatch 扩** (handoff §3097 推迟项落地, 本 epic 内 BuildPlan 闭包 + Pre 段合法, predicate 段禁)
9. **chunk 拓扑全串行 4 chunk** (S1 → S2 → S3 → S4), 每 chunk 主 lead 单次 build verify
10. **BLOCKED file hard signal 协议延续** (subagent 发现 plan 假设破裂创建 `.claude/BLOCKED_<chunk>_<symptom>.md` 0 改动退出, 防自扩)

## § Phase 27A delta / 偏离声明

**无偏离**. 本 SOT 切片严格遵循 handoff §3091-3107 5 deferred 项 + grill 三问 + 推迟原因:

| handoff §位置 | SOT 切片承接 |
|---|---|
| §3091#1 船长迁 .StepMachine (Phase 27B+ 独立 epic) | 本 SOT = Phase 27B 主体 |
| §3091#2 横向耦合迁 (Phase 27B/C/D 评估) | 不在本 epic scope (后续评估) |
| §3091#3 walk SkillEngine/ItemEngine 框架内 Task.Run | 不在本 epic scope (handoff §3106 不动 SkillEngine 内部实现) |
| §3091#4 HeroPlanBuilder ≥1000 行预警 | 本 epic 不加 fluent 维度 (8 维不破), R5 风险预案监控 |
| §3091#5 plan §7.3 escape hatch documented 扩 | 本 SOT §escape hatch 段已扩 (Pre/setup 内桥接 OK, predicate 内写 SetStep 禁) |
| §3105 完成形态 | INV14-INV18 5 新增不变量对齐 |
| §3106 in-scope 边界 | 落点 1NF 严格仅船长 1 hero / 仅 4 技能键 + EQ/QE/QW/QR 组合 / 不动 SkillEngine 内部 / Runner 扩 1 case |
| §3107 已否决方向 | 设计 alternative 否决记录段 6 项全数对齐 |
