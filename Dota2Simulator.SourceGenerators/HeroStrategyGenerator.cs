#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Dota2Simulator.SourceGenerators;

/// <summary>
/// Phase 10C S1 hello-world: 验证 SG 项目串联 (Phase 10D #2 已删除 hello-world emit, plan §8 一致).
/// Phase 10C S3 扩 EmitStrategyPartial: 扫 [HeroStrategy] attribute (via ForAttributeWithMetadataName)
/// emit Strategy partial 文件 — 含 ports 字段 + ctor + HeroId Hero property,
/// 业务 *Strategy.cs 同 commit 真删 ctor/field/Hero (S3 双部分整改 CS0102/CS0111 free).
/// Phase 10C S4 扩 EmitRegistry: 聚合 92 StrategyTarget,
/// emit HeroStrategyRegistry.Generated.g.cs 内 4 partial method body (Register* 调用,
/// attribute 分流 + RequiresUi 测试Strategy 走 _ui! 第 5 参数),
/// 同 commit 删 4 手写 partial 文件 (CS0759/CS8795 双向 free).
/// Phase 10D #2: hello-world emit 已删 (S3 起 SG emit 92 Strategy.g.cs + 1 Registry.Generated.g.cs 充分实证 SG 串联, hello 冗余).
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class HeroStrategyGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = "Dota2Simulator.GameAutomation.Domain.Heroes.HeroStrategyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Phase 10D #2: 删 hello-world emit (S3 起 92 Strategy.g.cs + 1 Registry.Generated.g.cs 充分实证 SG 串联).

        // S3: ForAttributeWithMetadataName 扫 [HeroStrategy] 命中
        var strategyClasses = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeFullName,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) =>
                {
                    var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
                    var attr = ctx.Attributes[0];

                    // attribute ctor: (string name, HeroAttribute attribute)
                    string heroName = attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value is string n
                        ? n
                        : symbol.Name; // fallback (理论不命中, defensive)

                    // Phase 10D #1: HeroAttribute enum field name 反查 — 改用 ITypeSymbol.GetMembers 取真 field name.
                    // 替代旧数值 switch (0=Strength/1=Agility/2=Intelligence/3=Universal) 的健壮性提升:
                    // (a) enum 插值 (在中间加项) 不再静默落 Universal bucket;
                    // (b) enum 加新 bucket (如 Sentinel) 自动 emit 正确 name, 无需改 SG;
                    // (c) 失败兜底 "Universal" 仅当 attribute 元数据极端损坏 (TypedConstant.Type 为 null), 不静默吞 enum 漂移.
                    string heroAttribute = "Universal";
                    if (attr.ConstructorArguments.Length > 1)
                    {
                        var enumArg = attr.ConstructorArguments[1];
                        if (enumArg.Type is INamedTypeSymbol enumType
                            && enumType.TypeKind == TypeKind.Enum
                            && enumArg.Value is not null)
                        {
                            // 在 enum type 内反查与数值匹配的 field name.
                            foreach (var member in enumType.GetMembers())
                            {
                                if (member is IFieldSymbol field
                                    && field.HasConstantValue
                                    && field.ConstantValue is not null
                                    && field.ConstantValue.Equals(enumArg.Value))
                                {
                                    heroAttribute = field.Name;
                                    break;
                                }
                            }
                        }
                    }

                    // RequiresUi 命名参数
                    bool requiresUi = false;
                    foreach (var na in attr.NamedArguments)
                    {
                        if (na.Key == "RequiresUi" && na.Value.Value is bool b)
                        {
                            requiresUi = b;
                            break;
                        }
                    }

                    return new StrategyTarget(
                        className: symbol.Name,
                        @namespace: symbol.ContainingNamespace.ToDisplayString(),
                        heroName: heroName,
                        heroAttribute: heroAttribute,
                        requiresUi: requiresUi);
                });

        context.RegisterSourceOutput(strategyClasses, static (spc, target) =>
        {
            var source = EmitPartial(target);
            // 文件名: <ClassName>.g.cs (中文文件名 SG host 支持; PictureManifestGenerator 中文 key 已实证)
            spc.AddSource($"{target.ClassName}.g.cs", SourceText.From(source, Encoding.UTF8));
        });

        // S4: 聚合 92 targets -> 单一 HeroStrategyRegistry.Generated.g.cs
        // Collect() 把 IncrementalValuesProvider<T> -> IncrementalValueProvider<ImmutableArray<T>>
        var allTargets = strategyClasses.Collect();
        context.RegisterSourceOutput(allTargets, static (spc, targets) =>
        {
            if (targets.IsDefaultOrEmpty)
            {
                return;
            }
            var source = EmitRegistry(targets);
            spc.AddSource("HeroStrategyRegistry.Generated.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }

    private readonly struct StrategyTarget
    {
        public string ClassName { get; }
        public string Namespace { get; }
        public string HeroName { get; }
        public string HeroAttribute { get; }
        public bool RequiresUi { get; }

        public StrategyTarget(string className, string @namespace, string heroName, string heroAttribute, bool requiresUi)
        {
            ClassName = className;
            Namespace = @namespace;
            HeroName = heroName;
            HeroAttribute = heroAttribute;
            RequiresUi = requiresUi;
        }
    }

    private static string EmitPartial(StrategyTarget t)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("// SSOT: [HeroStrategy] attribute on " + t.ClassName + "; SG = HeroStrategyGenerator (Phase 10C S3).");
        sb.AppendLine("// emit partial 提供: ports 字段 + ctor + HeroId Hero property (业务侧已真删, SG 单源).");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("#if DOTA2");
        sb.AppendLine();
        sb.AppendLine("using Dota2Simulator.GameAutomation.Application;");
        sb.AppendLine("using Dota2Simulator.GameAutomation.Domain.Actuation;");
        sb.AppendLine("using Dota2Simulator.GameAutomation.Domain.Heroes;");
        sb.AppendLine("using Dota2Simulator.GameAutomation.Domain.Loop;");
        sb.AppendLine("using Dota2Simulator.GameAutomation.Ports;");
        sb.AppendLine("using Dota2Simulator.Vision;");
        sb.AppendLine();
        sb.AppendLine($"namespace {t.Namespace}");
        sb.AppendLine("{");
        sb.AppendLine($"    public sealed partial class {t.ClassName}");
        sb.AppendLine("    {");

        // 字段顺序 (与现状对齐): _input, _vision (pragma 包裹), _skill, _item, (_ui 仅 RequiresUi), _main
        sb.AppendLine("        private readonly IInputExecutor _input;");
        sb.AppendLine("#pragma warning disable IDE0052");
        sb.AppendLine("        private readonly IScreenVision _vision;");
        sb.AppendLine("#pragma warning restore IDE0052");
        sb.AppendLine("        private readonly SkillEngine _skill;");
        sb.AppendLine("        private readonly ItemEngine _item;");
        if (t.RequiresUi)
        {
            sb.AppendLine("        private readonly IUiInvoker _ui;");
        }
        sb.AppendLine("        private readonly HeroLoopHost _main;");
        sb.AppendLine();

        // ctor 签名 (与现状对齐): 普通 (input, vision, skill, item, main); 测试 (input, vision, skill, item, ui, main)
        if (t.RequiresUi)
        {
            sb.AppendLine($"        public {t.ClassName}(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, IUiInvoker ui, HeroLoopHost main)");
        }
        else
        {
            sb.AppendLine($"        public {t.ClassName}(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)");
        }
        sb.AppendLine("        {");
        sb.AppendLine("            _input = input;");
        sb.AppendLine("            _vision = vision;");
        sb.AppendLine("            _skill = skill;");
        sb.AppendLine("            _item = item;");
        if (t.RequiresUi)
        {
            sb.AppendLine("            _ui = ui;");
        }
        sb.AppendLine("            _main = main;");
        sb.AppendLine("        }");
        sb.AppendLine();

        // HeroId Hero property
        sb.Append("        public HeroId Hero => new(\"")
          .Append(EscapeStringLiteral(t.HeroName))
          .Append("\", HeroAttribute.")
          .Append(t.HeroAttribute)
          .AppendLine(");");

        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("#endif");

        return sb.ToString();
    }

    private static string EmitRegistry(ImmutableArray<StrategyTarget> targets)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("// SSOT: 92 [HeroStrategy] attribute on *Strategy classes; SG = HeroStrategyGenerator (Phase 10C S4).");
        sb.AppendLine("// emit HeroStrategyRegistry partial 提供: 4 partial method body (Register* 按 HeroAttribute 分流, RequiresUi 测试Strategy 走 _ui!).");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("#if DOTA2");
        sb.AppendLine();
        sb.AppendLine("using Dota2Simulator.GameAutomation.Ports;");
        sb.AppendLine();
        sb.AppendLine("namespace Dota2Simulator.GameAutomation.Application");
        sb.AppendLine("{");
        sb.AppendLine("    public sealed partial class HeroStrategyRegistry");
        sb.AppendLine("    {");

        // 4 buckets: Strength / Agility / Intelligence / Universal
        EmitRegisterMethod(sb, "RegisterStrength", "Strength", targets);
        sb.AppendLine();
        EmitRegisterMethod(sb, "RegisterAgility", "Agility", targets);
        sb.AppendLine();
        EmitRegisterMethod(sb, "RegisterIntelligence", "Intelligence", targets);
        sb.AppendLine();
        EmitRegisterMethod(sb, "RegisterUniversal", "Universal", targets);

        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();
        sb.AppendLine("#endif");

        return sb.ToString();
    }

    private static void EmitRegisterMethod(StringBuilder sb, string methodName, string bucket, ImmutableArray<StrategyTarget> targets)
    {
        sb.Append("        partial void ")
          .Append(methodName)
          .AppendLine("(IInputExecutor input, IScreenVision vision, SkillEngine skill, ItemEngine item, HeroLoopHost main)");
        sb.AppendLine("        {");

        // 稳定输出顺序: 按 ClassName 排序 (避免 incremental SG 抖动)
        foreach (var t in targets.Where(x => x.HeroAttribute == bucket).OrderBy(x => x.ClassName, StringComparer.Ordinal))
        {
            // Strategy fully qualified name: <namespace>.<className>
            // RequiresUi: 第 5 参数插 _ui!
            sb.Append("            Register(new global::")
              .Append(t.Namespace)
              .Append('.')
              .Append(t.ClassName)
              .Append("(input, vision, skill, item, ");
            if (t.RequiresUi)
            {
                sb.Append("_ui!, ");
            }
            sb.AppendLine("main));");
        }

        sb.AppendLine("        }");
    }

    private static string EscapeStringLiteral(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}
