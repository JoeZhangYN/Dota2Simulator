using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dota2Simulator.Analyzers;

/// <summary>
/// DS0001: Heroes/ 内禁直调 `_item.根据按键判断技能释放前通用逻辑`.
/// 该 method 由 HeroPlan.DispatchAsync 自动调用 (90 hero 已自动); 业务侧重复调 → 双调 / 通用前置漏勺.
/// 修法: 删该行, BuildPlan DSL dispatch 自动处理.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DS0001_PrePassDirectCallAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DS0001";
    private const string PrePassMethodName = "根据按键判断技能释放前通用逻辑";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Heroes/ 内禁直调通用前置",
        messageFormat: "Heroes/ 内禁直调 `_item.{0}` (HeroPlan.DispatchAsync 已自动调用; 删该行)",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Phase 25A C5: 拆桥 lint — 业务侧重复手补通用前置 = 双调 / 漏勺. 走 DSL dispatch.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        var invocation = (IInvocationOperation)context.Operation;
        if (invocation.TargetMethod.Name != PrePassMethodName) return;

        var path = invocation.Syntax.SyntaxTree.FilePath;
        if (!PathHelper.IsHeroesPath(path)) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.Syntax.GetLocation(), PrePassMethodName));
    }
}
