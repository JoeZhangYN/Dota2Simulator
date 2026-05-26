using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dota2Simulator.Analyzers;

/// <summary>
/// DS0002: HeroStrategyBase 子类禁 override OnKeyAsync (除非 [SkipDslDispatch] escape-hatch).
/// Phase 19G-2 引入 base 默认 Plan.DispatchAsync, 92 hero 全 0 override 后 (Phase 25A C2 收尾);
/// 业务侧 override 走 BuildPlan DSL 表达 (.Pre / .OnStone / .WhenStoneChoiceEq 等) 即可消所有已知形态.
/// 真特殊形态 (DSL 不能表达) 标 [SkipDslDispatch] attribute 显式逃生口.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DS0002_HeroOverrideOnKeyAsyncAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DS0002";
    private const string BaseTypeName = "HeroStrategyBase";
    private const string MethodName = "OnKeyAsync";
    private const string EscapeHatchAttrFullName = "Dota2Simulator.GameAutomation.Domain.Heroes.SkipDslDispatchAttribute";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "HeroStrategyBase 子类禁 override OnKeyAsync",
        messageFormat: "HeroStrategyBase 子类禁 override OnKeyAsync (用 BuildPlan DSL 表达; 真特殊形态标 [SkipDslDispatch] escape-hatch)",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Phase 25A C5: 拆桥 lint — Phase 19G-2 + 25A C2 收尾后 92/92 hero 全 0 override, lint 防回退.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
    }

    private static void AnalyzeMethod(SymbolAnalysisContext context)
    {
        var method = (IMethodSymbol)context.Symbol;
        if (!method.IsOverride) return;
        if (method.Name != MethodName) return;
        if (method.OverriddenMethod is null) return;
        if (method.OverriddenMethod.ContainingType.Name != BaseTypeName) return;

        foreach (var attr in method.GetAttributes())
        {
            if (attr.AttributeClass?.ToDisplayString() == EscapeHatchAttrFullName) return;
        }

        foreach (var loc in method.Locations)
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, loc));
        }
    }
}
