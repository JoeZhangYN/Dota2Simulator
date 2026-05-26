using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Dota2Simulator.Analyzers;

/// <summary>
/// DS0003: Application/+Heroes/ 业务侧禁直调 `GlobalScreenCapture.GetCurrentHandle()`.
/// 业务侧拿 ImageHandle 后大概率走 ImageManager.GetColor 多次同帧取色 — 违反 L3 铁律 1d 「复杂度下沉到元工具」.
/// 修法: 单次取色用 _vision.PixelAt; 多次同帧取色用 _vision.WithFrame typestate scope (adapter 内部自管帧).
/// 白名单: HeroLoopHost.cs (IsValid/ReleaseImage 生命周期 2 处), ItemEngine.cs (Silt 透传 5 处 + SaveImage 1 处, Phase 11 Silt 拆出后收紧).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DS0003_BusinessGetCurrentHandleAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DS0003";
    private const string TargetTypeFullName = "Dota2Simulator.Vision.GlobalScreenCapture";
    private const string TargetMethodName = "GetCurrentHandle";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "业务侧禁直调 GlobalScreenCapture.GetCurrentHandle",
        messageFormat: "Application/+Heroes/ 业务侧禁直调 GlobalScreenCapture.GetCurrentHandle() (改 _vision.PixelAt / _vision.WithFrame 端口; 白名单: HeroLoopHost / ItemEngine)",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Phase 25A C5: 拆桥 lint — IScreenVision.WithFrame typestate (Phase 25A C3 引入) 让 adapter 内部自管帧; 业务侧 0 wire.");

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
        var method = invocation.TargetMethod;
        if (method.Name != TargetMethodName) return;
        if (method.ContainingType.ToDisplayString() != TargetTypeFullName) return;

        var path = invocation.Syntax.SyntaxTree.FilePath;
        if (!PathHelper.IsBusinessPath(path)) return;
        if (PathHelper.IsWhitelistedForGetCurrentHandle(path)) return;

        context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.Syntax.GetLocation()));
    }
}
