#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Dota2Simulator.SourceGenerators;

/// <summary>
/// Phase 10A Chunk B: 扫描 Picture_Dota2/**/*.bmp + emit Dota2_Pictrue partial class.
/// Phase 10A Chunk C: 扩展 emit Dota2_PictrueSha1 字典 + [ModuleInitializer] 注册到 LazyImageLoader.
/// SSOT = 磁盘 bmp 文件; SG 在编译期派生 partial static class (5 类 + Silt) + SHA1 常量映射.
/// SHA1 语义 = build artifact 完整性校验 (编译期一次性嵌入), 不覆盖 runtime hot-reload 篡改.
/// 后续 chunk D 加 PreloadHints 第二个 SG.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class PictureManifestGenerator : IIncrementalGenerator
{
    // 磁盘顶层目录 → .cs 嵌套类名映射已移至 PictureCategoryMap.DirToClassName (B1: 消双 dict 漂移)

    // file stem ≠ 属性名特例 dict (R1 修订: 唯一不对称是 BUFF/物品_暗影护符 → 属性名 暗影护符)
    private static readonly Dictionary<(string Dir, string FileStem), string> FileStemToPropertyName = new()
    {
        [("BUFF", "物品_暗影护符")] = "暗影护符",
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 收集所有 Picture_Dota2 下的 .bmp 路径
        var bmpFiles = context.AdditionalTextsProvider
            .Where(static at =>
            {
                var p = at.Path.Replace('\\', '/');
                return p.IndexOf("/Picture_Dota2/", StringComparison.OrdinalIgnoreCase) >= 0
                    && p.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase);
            })
            .Select(static (at, _) => at.Path)
            .Collect();

        // 拿 preprocessor symbols 判断 Silt define 是否启用
        var defines = context.ParseOptionsProvider
            .Select(static (po, _) =>
            {
                if (po is Microsoft.CodeAnalysis.CSharp.CSharpParseOptions cs)
                    return cs.PreprocessorSymbolNames.ToImmutableArray();
                return ImmutableArray<string>.Empty;
            });

        var combined = bmpFiles.Combine(defines);

        context.RegisterSourceOutput(combined, static (spc, tuple) =>
        {
            var paths = tuple.Left;
            var symbols = tuple.Right;
            var hasSilt = symbols.Contains("Silt", StringComparer.Ordinal);

            var entries = BuildEntries(paths);
            var source = Emit(entries, hasSilt);
            spc.AddSource("Dota2_Pictrue.g.cs", SourceText.From(source, Encoding.UTF8));

            // Chunk C: SHA1 manifest emit + [ModuleInitializer] 注册到 LazyImageLoader
            var sha1Source = EmitSha1(entries, paths, hasSilt);
            spc.AddSource("Dota2_PictrueSha1.g.cs", SourceText.From(sha1Source, Encoding.UTF8));
        });
    }

    private record struct Entry(string DirTop, string SubDir, string FileStem, string ClassName, string PropertyName, string ManifestKey, string SourcePath);

    private static List<Entry> BuildEntries(ImmutableArray<string> paths)
    {
        var entries = new List<Entry>(paths.Length);

        foreach (var path in paths)
        {
            var norm = path.Replace('\\', '/');
            var idx = norm.IndexOf("/Picture_Dota2/", StringComparison.OrdinalIgnoreCase);
            if (idx < 0) continue;

            var rel = norm.Substring(idx + "/Picture_Dota2/".Length); // 如 "BUFF/大牛_回魂.bmp" 或 "Silt/钢背/鼻涕.bmp"
            var segments = rel.Split('/');
            if (segments.Length < 2) continue; // 平铺顶层文件 (现状无, 跳过)

            var dirTop = segments[0];
            if (!PictureCategoryMap.DirToClassName.TryGetValue(dirTop, out var className)) continue; // 未登记目录跳过 (扩展可加 dict)

            string subDir;
            string fileWithExt;
            if (segments.Length == 2)
            {
                subDir = string.Empty;
                fileWithExt = segments[1];
            }
            else
            {
                subDir = string.Join("/", segments, 1, segments.Length - 2);
                fileWithExt = segments[segments.Length - 1];
            }

            if (!fileWithExt.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)) continue;
            var fileStem = fileWithExt.Substring(0, fileWithExt.Length - ".bmp".Length);

            // 属性名: 先查特例 dict, fallback file stem
            string propertyName = FileStemToPropertyName.TryGetValue((dirTop, fileStem), out var overrideName)
                ? overrideName
                : fileStem;

            // manifest 键: 目录 + 子目录 + file stem (用 `.` 连接, 与 LazyImageLoader 现有约定一致)
            // 例: "BUFF.大牛_回魂", "Silt.钢背.鼻涕", "Silt.先天"
            string manifestKey = string.IsNullOrEmpty(subDir)
                ? $"{dirTop}.{fileStem}"
                : $"{dirTop}.{subDir.Replace('/', '.')}.{fileStem}";

            entries.Add(new Entry(dirTop, subDir, fileStem, className, propertyName, manifestKey, path));
        }

        // 按 (className, propertyName) 排序保证 emit 顺序稳定 (增量 SG cache 命中)
        entries.Sort((a, b) =>
        {
            var c = string.Compare(a.ClassName, b.ClassName, StringComparison.Ordinal);
            if (c != 0) return c;
            return string.Compare(a.PropertyName, b.PropertyName, StringComparison.Ordinal);
        });

        return entries;
    }

    private static string Emit(List<Entry> entries, bool hasSilt)
    {
        // 按 className 分组 emit
        var byClass = new Dictionary<string, List<Entry>>(StringComparer.Ordinal);
        foreach (var e in entries)
        {
            if (!byClass.TryGetValue(e.ClassName, out var list))
            {
                list = new List<Entry>();
                byClass[e.ClassName] = list;
            }
            list.Add(e);
        }

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("// SSOT: 磁盘 Picture_Dota2/**/*.bmp; SG = PictureManifestGenerator.");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("#if DOTA2");
        sb.AppendLine();
        sb.AppendLine("using Dota2Simulator.Vision;");
        sb.AppendLine("using Dota2Simulator.GameAutomation.Domain.Perception;");
        sb.AppendLine();
        sb.AppendLine("namespace Dota2Simulator.Games.Dota2");
        sb.AppendLine("{");
        sb.AppendLine("    public static partial class Dota2_Pictrue");
        sb.AppendLine("    {");

        // 非 Silt 类按字典序输出
        var nonSiltOrder = new[] { "Buff", "命石", "英雄技能", "播报信息", "物品" };
        foreach (var cls in nonSiltOrder)
        {
            if (!byClass.TryGetValue(cls, out var list)) continue;
            EmitClass(sb, cls, list, indent: 8);
        }

        // Silt 类: 仅在 Silt define 启用 + 磁盘有 Silt 文件时 emit, 用 #if Silt 包裹
        if (byClass.TryGetValue("Silt", out var siltList))
        {
            sb.AppendLine("#if Silt");
            EmitClass(sb, "Silt", siltList, indent: 8);
            sb.AppendLine("#endif");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("#endif");

        return sb.ToString();
    }

    private static void EmitClass(StringBuilder sb, string className, List<Entry> entries, int indent)
    {
        var pad = new string(' ', indent);
        var pad2 = new string(' ', indent + 4);
        sb.Append(pad).Append("public static partial class ").AppendLine(className);
        sb.Append(pad).AppendLine("{");
        foreach (var e in entries)
        {
            // Phase 19A: emit pair —— 旧 ImageHandle 属性 (调用方原路径) + 新 _Tpl Template 属性 (IScreenVision 主力路径).
            //  Template 仅持 manifestKey 字符串, 与 ImageHandle 平行存在; IScreenVision adapter 实现内反查 ImageHandle.
            sb.Append(pad2)
              .Append("public static ImageHandle ")
              .Append(e.PropertyName)
              .Append(" => LazyImageLoader.GetImage(\"")
              .Append(e.ManifestKey)
              .AppendLine("\");");
            sb.Append(pad2)
              .Append("public static Template ")
              .Append(e.PropertyName)
              .Append("_Tpl => new Template(\"")
              .Append(e.ManifestKey)
              .AppendLine("\");");
        }
        sb.Append(pad).AppendLine("}");
    }

    /// <summary>
    /// Phase 10A Chunk C — emit Dota2_PictrueSha1.g.cs.
    /// Phase 10B B2 — 按 dirTop=="Silt" 拆 NonSiltMap (始终 emit) / SiltMap (#if Silt 包裹),
    /// 消 Phase 10A SOFT_FAIL #6 关 Silt 时 SHA1 dict 含 42 死 key 的诊断微噪.
    /// 含 IReadOnlyDictionary&lt;manifestKey, sha1Hex&gt; + 单 [ModuleInitializer] 合并注册到 LazyImageLoader.
    /// SHA1 在 SG 编译期一次性计算 (File.ReadAllBytes + SHA1ComputeHash + BitConverter ToString lower hex 40 char).
    /// 失败 (路径不存在 / IO 异常) 静默 emit 空字符串, runtime 命中时自动 skip 该条 (避免 SG 崩 build).
    /// </summary>
    private static string EmitSha1(List<Entry> entries, ImmutableArray<string> paths, bool hasSilt)
    {
        // B2: 按 dirTop 拆 Silt / NonSilt 两组 entries
        var nonSiltEntries = new List<Entry>(entries.Count);
        var siltEntries = new List<Entry>();
        foreach (var e in entries)
        {
            if (string.Equals(e.DirTop, "Silt", StringComparison.Ordinal))
                siltEntries.Add(e);
            else
                nonSiltEntries.Add(e);
        }

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("// SSOT: 磁盘 Picture_Dota2/**/*.bmp 字节 SHA1; SG 编译期一次性计算.");
        sb.AppendLine("// 语义边界 = build artifact 完整性校验, 不覆盖 runtime hot-reload.");
        sb.AppendLine("// Phase 10B B2: NonSiltMap / SiltMap 按 Silt define 分割 emit, 关 Silt 时 SiltMap 不入 combined.");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("#if DOTA2");
        sb.AppendLine();
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine();
        sb.AppendLine("namespace Dota2Simulator.Games.Dota2");
        sb.AppendLine("{");
        sb.AppendLine("    internal static class Dota2_PictrueSha1");
        sb.AppendLine("    {");

        // NonSiltMap: 始终 emit (在 #if DOTA2 内)
        sb.AppendLine($"        internal static readonly IReadOnlyDictionary<string, string> NonSiltMap = new Dictionary<string, string>({nonSiltEntries.Count})");
        sb.AppendLine("        {");
        EmitSha1Entries(sb, nonSiltEntries);
        sb.AppendLine("        };");
        sb.AppendLine();

        // SiltMap: emit 在 #if Silt 包裹内, 关 Silt 时整段不存在
        sb.AppendLine("#if Silt");
        sb.AppendLine($"        internal static readonly IReadOnlyDictionary<string, string> SiltMap = new Dictionary<string, string>({siltEntries.Count})");
        sb.AppendLine("        {");
        EmitSha1Entries(sb, siltEntries);
        sb.AppendLine("        };");
        sb.AppendLine("#endif");
        sb.AppendLine();

        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// CLR module init 时合并 NonSiltMap (+ SiltMap if Silt) 单次注册到 LazyImageLoader.");
        sb.AppendLine("        /// 触发顺序: 早于 Program.Main 早于 Form2 ctor (R2 实测验证).");
        sb.AppendLine("        /// B2 设计: 全 epic SHA1 manifest 注册入口唯一 — 合并后单次调 RegisterSha1Manifest,");
        sb.AppendLine("        /// 不依赖 LazyImageLoader 多注册去重语义 (后者首注册胜出 + warning).");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        [ModuleInitializer]");
        sb.AppendLine("        internal static void RegisterOnModuleInit()");
        sb.AppendLine("        {");
        sb.AppendLine("            var combined = new Dictionary<string, string>(NonSiltMap);");
        sb.AppendLine("#if Silt");
        sb.AppendLine("            foreach (var kv in SiltMap) combined[kv.Key] = kv.Value;");
        sb.AppendLine("#endif");
        sb.AppendLine("            Dota2Simulator.Vision.LazyImageLoader.RegisterSha1Manifest(combined);");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("#endif");

        return sb.ToString();
    }

    /// <summary>
    /// emit SHA1 dict entries (供 NonSiltMap / SiltMap 复用).
    /// 编译期读 .bmp 字节算 SHA1, 失败 emit 空字符串 runtime skip.
    /// </summary>
    private static void EmitSha1Entries(StringBuilder sb, List<Entry> entries)
    {
        foreach (var e in entries)
        {
            string sha1Hex;
            try
            {
                // RS1035 抑制理由: SG 编译期读 .bmp 字节算 SHA1 是 epic 核心需求,
                // 与 AdditionalText.GetText() 不兼容 (二进制不可解码文本). 路径来自
                // AdditionalText.Path (受 MSBuild AdditionalFiles ItemGroup 约束, 沙箱内).
#pragma warning disable RS1035
                var bytes = File.ReadAllBytes(e.SourcePath);
#pragma warning restore RS1035
                using var sha = SHA1.Create();
                var hash = sha.ComputeHash(bytes);
                sha1Hex = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
            catch
            {
                // 路径读取失败 (权限 / 不存在 / 中文路径异常等), emit 空字符串 - runtime skip
                sha1Hex = string.Empty;
            }

            // EscapeKey 防 manifest key 含特殊字符 (现状无, 防御编程)
            sb.Append("            [\"").Append(EscapeStringLiteral(e.ManifestKey)).Append("\"] = \"").Append(sha1Hex).AppendLine("\",");
        }
    }

    private static string EscapeStringLiteral(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
