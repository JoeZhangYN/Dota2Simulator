#nullable enable
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Dota2Simulator.SourceGenerators;

/// <summary>
/// 从 input_abi 契约签名 manifest 生成两后端绑定类的 <c>[DllImport]</c> extern 声明。
///
/// SSOT = input_abi::with_input_abi_surface! → print_manifest → 提交的
/// <c>Infrastructure/Input/input_abi.contract.txt</c>（行格式 <c>export|csret|name:cstype,...</c>）。
/// 标 <c>[InputAbiBindings("&lt;dll&gt;")]</c> 的 partial 类各自获得 42 符号中可机投影的
/// extern（DllName 取自 attribute）。<c>LastError</c>（缓冲协议）与 <c>simengio_text</c>
/// （契约外）刻意跳过、留手写。
///
/// 杜绝 C# 侧 extern 签名手抄漂移——C ABI 导出表无签名信息，只能从此 SSOT 投影。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class InputAbiExternGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = "Dota2Simulator.Infrastructure.Input.InputAbiBindingsAttribute";
    private const string ManifestFileName = "input_abi.contract.txt";

    /// <summary>缓冲协议 / 契约外，留手写，不生成。</summary>
    private static readonly string[] HandWritten = { "LastError" };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 标记的 partial 绑定类 → (类名, 命名空间, 可见性, static, DllName)
        var bindings = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeFullName,
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) =>
            {
                var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
                var dll = ctx.Attributes[0].ConstructorArguments.Length > 0
                    && ctx.Attributes[0].ConstructorArguments[0].Value is string s
                        ? s
                        : string.Empty;
                return new BindingTarget(
                    className: symbol.Name,
                    @namespace: symbol.ContainingNamespace.ToDisplayString(),
                    accessibility: AccessibilityKeyword(symbol.DeclaredAccessibility),
                    isStatic: symbol.IsStatic,
                    dllName: dll);
            });

        // 契约 manifest（提交在主工程，经 <AdditionalFiles> 喂入）。
        var manifest = context.AdditionalTextsProvider
            .Where(static at => at.Path.Replace('\\', '/')
                .EndsWith("/" + ManifestFileName, StringComparison.OrdinalIgnoreCase))
            .Select(static (at, ct) => at.GetText(ct)?.ToString() ?? string.Empty)
            .Collect();

        var combined = bindings.Combine(manifest);

        context.RegisterSourceOutput(combined, static (spc, pair) =>
        {
            var target = pair.Left;
            var manifests = pair.Right;
            if (string.IsNullOrEmpty(target.DllName) || manifests.IsDefaultOrEmpty)
            {
                return;
            }
            var rows = ParseManifest(manifests);
            if (rows.Count == 0)
            {
                return;
            }
            var source = Emit(target, rows);
            spc.AddSource($"{target.ClassName}.Externs.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }

    private readonly struct BindingTarget
    {
        public string ClassName { get; }
        public string Namespace { get; }
        public string Accessibility { get; }
        public bool IsStatic { get; }
        public string DllName { get; }

        public BindingTarget(string className, string @namespace, string accessibility, bool isStatic, string dllName)
        {
            ClassName = className;
            Namespace = @namespace;
            Accessibility = accessibility;
            IsStatic = isStatic;
            DllName = dllName;
        }
    }

    private readonly struct ExternRow
    {
        public string Export { get; }
        public string CsRet { get; }
        /// <summary>已渲染为 C# 形参串（"ushort code, ulong holdMs"），无参为空。</summary>
        public string CsParams { get; }

        public ExternRow(string export, string csRet, string csParams)
        {
            Export = export;
            CsRet = csRet;
            CsParams = csParams;
        }
    }

    private static System.Collections.Generic.List<ExternRow> ParseManifest(ImmutableArray<string> manifests)
    {
        var rows = new System.Collections.Generic.List<ExternRow>();
        foreach (var content in manifests)
        {
            foreach (var raw in content.Split('\n'))
            {
                var line = raw.Trim();
                if (line.Length == 0)
                {
                    continue;
                }
                var seg = line.Split('|');
                if (seg.Length != 3)
                {
                    continue; // 容错：跳过畸形行
                }
                var export = seg[0].Trim();
                if (HandWritten.Contains(export))
                {
                    continue; // 留手写
                }
                rows.Add(new ExternRow(export, seg[1].Trim(), RenderParams(seg[2].Trim())));
            }
        }
        return rows;
    }

    /// <summary>"code:ushort,hold_ms:ulong" → "ushort code, ulong hold_ms"。</summary>
    private static string RenderParams(string paramsSeg)
    {
        if (paramsSeg.Length == 0)
        {
            return string.Empty;
        }
        var parts = paramsSeg.Split(',')
            .Select(static p =>
            {
                var nt = p.Split(':');
                return nt.Length == 2 ? $"{nt[1].Trim()} {nt[0].Trim()}" : p.Trim();
            });
        return string.Join(", ", parts);
    }

    private static string Emit(BindingTarget target, System.Collections.Generic.List<ExternRow> rows)
    {
        var modifiers = target.IsStatic
            ? $"{target.Accessibility} static partial class"
            : $"{target.Accessibility} partial class";

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("// SSOT: Infrastructure/Input/" + ManifestFileName
            + "（源自 input_abi::with_input_abi_surface!）; SG = InputAbiExternGenerator.");
        sb.AppendLine("// 生成 [DllImport] extern；LastError(缓冲协议)/simengio_text(契约外) 留手写。");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Runtime.InteropServices;");
        sb.AppendLine();
        sb.AppendLine($"namespace {target.Namespace}");
        sb.AppendLine("{");
        sb.AppendLine($"    {modifiers} {target.ClassName}");
        sb.AppendLine("    {");

        // 稳定输出顺序（按 export 名排序，避免 incremental SG 抖动）
        foreach (var r in rows.OrderBy(static x => x.Export, StringComparer.Ordinal))
        {
            sb.AppendLine($"        [DllImport(\"{target.DllName}\", CallingConvention = CallingConvention.Cdecl)]");
            sb.AppendLine($"        public static extern {r.CsRet} {r.Export}({r.CsParams});");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private static string AccessibilityKeyword(Accessibility a) => a switch
    {
        Accessibility.Public => "public",
        Accessibility.Internal => "internal",
        Accessibility.ProtectedOrInternal => "protected internal",
        Accessibility.Protected => "protected",
        Accessibility.Private => "private",
        Accessibility.ProtectedAndInternal => "private protected",
        _ => "internal",
    };
}
