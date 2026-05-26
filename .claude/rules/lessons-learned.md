# Lessons Learned (Dota2Simulator 项目级经验累积)

> 仓库内 `.claude/rules/` 目录下的项目级 lessons-learned. 跨 epic / chunk 实战中遇到的非显然反例 + 设计陷阱在此累积, 避免 subagent / 主 lead 反复踩同坑.

---

## 2026-05-23 (Phase 10C S3 实战 2 意外)

### L1: 行尾注释致 regex 漏命中

**场景**: Phase 10C S3 批量改 92 *Strategy.cs 删除 `private readonly IScreenVision _vision;` 行 (含 IDE0052 pragma 包裹).

**意外**: `大牛Strategy.cs` 该字段行尾含注释 `// A4 阶段：_vision 暂未使用`, subagent 用的简单 regex 漏命中, 留下半截改动.

**为何**: 批量 regex 假设字段行末是 `;` 直接结束, 未考虑行尾可能有注释; 92 文件 91 个干净, 第 92 个有边缘形态.

**如何避免**:
- 批量替换 regex 必须显式覆盖 "行尾可能含注释" 情况 (`;.*$` 而非 `;$`).
- 用 Edit 工具单文件单点改 (确定性 string match), 比 PowerShell `Get-Content` + regex 链处理更安全.
- 改动后 grep verify "改动后应为 0 命中" 的 invariant 同时跑全文件 (不仅看 build 0 错, 边缘形态可能留下未触发编译路径的残留).

### L2: SG 项目 netstandard2.0 不支持 `record struct`

**场景**: Phase 10C S3 subagent 在 SG 内用 `private readonly record struct StrategyTarget(string ClassName, ...)` 表 5 元组.

**意外**: 编译报 CS0518 "Predefined type 'System.Runtime.CompilerServices.IsExternalInit' is not defined or imported".

**为何**: `record struct` 是 C# 10 特性, 依赖 `IsExternalInit` polyfill. SG 项目 csproj 目标框架 = netstandard2.0 (因 Roslyn analyzer / SG 协议要求), 该框架未内置 `IsExternalInit` 类型. C# 9+ 的 init-only setter / record / record struct 都依赖此类型.

**如何避免**:
- SG 项目内禁用 C# 9+ 仅依赖 netstandard2.0 没有的语法:
  - `record class` / `record struct` (依赖 IsExternalInit)
  - `init` setter (依赖 IsExternalInit)
  - 默认接口方法 (依赖更高 runtime)
- 替代: 普通 `readonly struct` + 显式 ctor + readonly auto-property (无 init setter).
- 若必须用 `init`/`record`: 在 SG 项目内**手动定义** `IsExternalInit` polyfill 类 (常见社区做法), 或 multi-target `netstandard2.0;net6.0` 让分析器协议层用 netstandard2.0 编译产物.

**SSOT 参考**: `Dota2Simulator.SourceGenerators/HeroStrategyGenerator.cs` 内 `StrategyTarget` 即普通 readonly struct + 显式 ctor (规避此坑).

---

## 2026-05-26 (Phase 27B plan-reviewer 二审协议范例)

### L3: plan / SOT 引用现存 signature 必先 verify 真实代码

**场景**: Phase 27B 船长 .StepMachine 迁移 epic, step 2 sot-annotator + step 3 plan agent 串行环节均推 "方案 H 扩 `HeroPlanBuilder.RegisterProbeAsync(ConditionSlotKey, Func<Task<bool>>)` 兄弟 method" (跨 4 文件 ~30 行 LOC), 因假定 `ConditionDelegateBitmap` 是 sync `delegate bool()` 不能接 async lambda.

**意外**: plan-reviewer R1-A 二审 1 个 Grep 实测 `Application/ConditionSlot.cs:11` 签名 `public delegate Task<bool> ConditionDelegateBitmap()` — **本就是 async**, RegisterProbe 直接接 async lambda. 方案 H 不需要存在.

**为何**: step 2 sot-annotator 描述上游 SOT 不验证现存 type/delegate 真实签名; step 3 plan agent 信任 SOT 切片描述继续推方案 H; 串行环节均未 Read / Grep ConditionSlot.cs:11 verify, 仅靠 plan-reviewer 1 个 Grep 检出. 触违全局铁律 5(b) Zoom-out 先于局部修改 + grill-me §动手前 fact 必查.

**如何避免**:
- **修订纪律永久化** (跨 epic 复用价值最大): 凡 plan / SOT 引用现存 type / delegate / method signature, **必先 Read / Grep verify 真实代码**; 不凭训练印象 / 不凭 `.claude/CLAUDE.md` 顶层 doc 描述 / 不凭上游 SOT 切片描述 (上游 SOT 也可能错).
- **plan-reviewer 二审协议价值实证**: 1 个 Grep 检出 plan + SOT 上游事实错误, 避免 S2 subagent 实施时撞 BLOCKED + retry budget 浪费. 二审 REJECT 10 项必修 + 3 项建议补 (plan agent 修订) → 二审 PASS WITH CAVEATS (主 lead 3 处补丁) → S2 一次性 PASS 0 retry. 协议本身价值远超 plan-reviewer subagent spawn 开销.
- **epic surface 初判易过设计**: 27B plan v1 推扩兄弟 method (跨 4 文件 ~30 行) → v2 撤回方案 H 后落点缩到船长Strategy.cs 1 文件 1 行 × 2 (-30 行 LOC). 反例: "看似优雅但冗余的兄弟方法" → upstream verify 1 个 Grep 即可作废.
- **反馈到 step 2 sot-annotator agent + step 3 plan agent prompt 规约层**: 引用现存 signature 必先 grep 真实代码, 不凭描述.

**SSOT 参考**: `.claude/handoff-hex-refactor.md` §"Phase 27B 元层 retrospective" + `.claude/plans/phase-27b-captain-stepmachine-plan.md` §6 偏离 2 (整段重写) + `.claude/sot/phase-27b-captain-stepmachine.md` frontmatter `references` 加 `ConditionSlot.cs:11` verify ground truth.

---

## 2026-05-26 (Phase 27C lite mode 协议)

### L4: lite 模式判据 — 规模驱动 spawn 决策, 非默认全 5 步骤

**场景**: Phase 27C 横向耦合读侧迁 (2 hero 6 处迁, +12/-27 净减 15 行) 与 Phase 27B 船长 .StepMachine 迁 (1 hero 全聚合迁, 5 步骤含 plan-reviewer R1-A 检出方案 H 错误) 对照.

**意外**: 27B 完整 5 步骤含 plan-reviewer 二审 REJECT 10 项必修 (避免 S2 撞 BLOCKED + retry budget 浪费, 协议价值远超 spawn 开销); 27C 同走 5 步骤会触 spawn 开销摊销失衡 (2 hero × 3 行调用站迁不值多 4 个 agent spawn).

**为何**: epic 规模 < 3 hero / < 20 行 LOC + 0 新增原语 + 0 新增 INV + 无 plan-required-gate 触发条件 (无 pub API 变更 / 无跨边界 / 无类型不变量调整) → lite 模式直 grep verify + Edit + commit + 主 lead 手写 handoff 累加段 (而非 spawn handoff-archiver 走完整 step 5). 后置 handoff-archiver 二审一次性补 lessons + 验证未验证锚点 (本条 L4 即此路径产出).

**如何避免** (lite vs 完整判据):

- **规模 ≥ 3 hero 或 ≥ 1 新原语 或 ≥ 1 新 INV 或触 plan-required-gate** → 走**完整 5 步骤** (sot-annotator → plan → plan-reviewer → 实施 → handoff-archiver), 不省 plan-reviewer 二审 (27B 实证 1 个 Grep 检 plan 错值远超开销).
- **规模 < 3 hero / < 20 行 LOC / 0 新原语 / 0 新 INV / 不触 gate** → **lite 模式**, 主 lead 直跑 grep verify + Edit + commit + 手写 handoff 累加段, 后置 handoff-archiver 二审 (规模门槛过低时 spawn 开销摊不平).
- **handoff-archiver 二审无论 lite / 完整都跑**: handoff 累加段需 agent 验证未验证锚点 + 派生 lessons (主 lead 手写易漏); 不省 archiver, 只省其前 4 步.
- **多 epic 串行连发场景** (本次 session 27B + 27B+ item3 + 27C-A + 27C-B 累计 8 commit) lite 模式特别适用: 单次主对话内多个小 epic, 用完整 5 步骤 spawn 每个会 token / context 摊销失衡, lite 直跑可保持单一主对话 context 流.

**SSOT 参考**: `.claude/handoff-hex-refactor.md` §"Phase 27B+ deferred 项闭环 + Phase 27C 横向耦合读侧迁" (主 lead 手写累加段) + 后置 handoff-archiver 二审报告 (pass_with_caveats 1 软补丁即本条 L4).

---

## 通用 SG 注意 (横切多 epic)

- SG `AdditionalText` 仅返 `SourceText` (源代码文本); 处理二进制 (bmp/png) 必须 `File.ReadAllBytes` (触 RS1035, 但本场景必须豁免).
- SG `ForAttributeWithMetadataName` 比 `SyntaxProvider.CreateSyntaxProvider` 增量友好, Roslyn 4.0+ 首选.
- SG 内反查 enum field name **必须用 `ITypeSymbol.GetMembers()` 取真 IFieldSymbol.Name** (依赖 ConstantValue.Equals), **禁数值 switch 反查** (enum 插值 / 加值会静默漂移). 参考 `HeroStrategyGenerator.cs` Phase 10D #1 实施.
- SG emit `[ModuleInitializer]` 注册路径要求宿主 assembly + 静态 ctor 同 assembly 协议; 跨 assembly 静态初始化需测试覆盖.
- SG 增量编译 cache 在 hot reload / IDE 内 attribute 加删可能 stale; 用户工作流走 `dotnet build` 命令行而非 IDE hot reload 避免.

---

## 文档迁移说明

本文件 SSOT = 仓库内 `.claude/rules/lessons-learned.md`. 与全局 `~/.claude/dream/knowledge/facts/` 关系:
- 本文件存项目特化 (Dota2Simulator C# .NET 10 + Roslyn SG 等领域).
- 全局 fact 库存跨项目通用 (PowerShell UTF-8 损毁 / Claude harness 行为 等).
- 同主题两库都有时, 项目级条目应**指向**全局 fact (避免双源漂移).
