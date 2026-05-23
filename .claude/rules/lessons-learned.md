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
