#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Dota2Simulator.SourceGenerators;

/// <summary>
/// Phase 10A Chunk B: 扫描 Picture_Dota2/**/*.bmp + emit Dota2_Pictrue partial class.
/// SSOT = 磁盘 bmp 文件; SG 在编译期派生 partial static class (5 类 + Silt).
/// 后续 chunk C 会在此基础上加 SHA1 emit, chunk D 加 PreloadHints 第二个 SG.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class PictureManifestGenerator : IIncrementalGenerator
{
    // 磁盘顶层目录 → .cs 嵌套类名 (历史不对称: 技能→英雄技能, 播报→播报信息, BUFF→Buff)
    private static readonly Dictionary<string, string> DirToClassName = new(StringComparer.Ordinal)
    {
        ["BUFF"] = "Buff",
        ["命石"] = "命石",
        ["技能"] = "英雄技能",
        ["播报"] = "播报信息",
        ["物品"] = "物品",
        ["Silt"] = "Silt",
    };

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

            var source = Emit(paths, hasSilt);
            spc.AddSource("Dota2_Pictrue.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }

    private record struct Entry(string DirTop, string SubDir, string FileStem, string ClassName, string PropertyName, string ManifestKey);

    private static string Emit(ImmutableArray<string> paths, bool hasSilt)
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
            if (!DirToClassName.TryGetValue(dirTop, out var className)) continue; // 未登记目录跳过 (扩展可加 dict)

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

            entries.Add(new Entry(dirTop, subDir, fileStem, className, propertyName, manifestKey));
        }

        // 按 (className, propertyName) 排序保证 emit 顺序稳定 (增量 SG cache 命中)
        entries.Sort((a, b) =>
        {
            var c = string.Compare(a.ClassName, b.ClassName, StringComparison.Ordinal);
            if (c != 0) return c;
            return string.Compare(a.PropertyName, b.PropertyName, StringComparison.Ordinal);
        });

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
            sb.Append(pad2)
              .Append("public static ImageHandle ")
              .Append(e.PropertyName)
              .Append(" => LazyImageLoader.GetImage(\"")
              .Append(e.ManifestKey)
              .AppendLine("\");");
        }
        sb.Append(pad).AppendLine("}");
    }
}
