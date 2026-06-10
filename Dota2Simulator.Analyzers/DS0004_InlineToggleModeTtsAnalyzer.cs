using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Dota2Simulator.Analyzers;

/// <summary>
/// DS0004: Heroes/ 内禁 inline <c>.Execute(() => { Skills.ToggleMode(slot); TTS.Speak(Mode(slot)==N ? a : b); })</c>.
/// Phase 28 C5 收口 17 处为 <c>.ToggleModeTts(slot, on, off)</c> DSL 算子后, 拆桥 lint 防回退.
/// 精确匹配 (零误报): Execute lambda block 内含 ToggleMode 调用 + Speak(纯三元) 调用 + 无 if 控制流.
/// 排除异形: 龙骑 (Speak arg 字符串拼接非纯三元) / 小黑 (含 if/else 分支).
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DS0004_InlineToggleModeTtsAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "DS0004";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Heroes/ 内禁 inline ToggleMode + TTS, 用 .ToggleModeTts DSL 算子",
        messageFormat: "用 .ToggleModeTts(slot, on, off) 替代 inline .Execute(() => {{ Skills.ToggleMode + TTS.Speak 三元 }})",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Phase 28 C5 拆桥 lint — 17 处 ToggleMode+TTS 已收口 .ToggleModeTts, lint 防回退到 inline Execute lambda. 真异形 (字符串拼接 / if 分支) 不命中.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // 1. 必须是 .Execute(...) 调用.
        if (invocation.Expression is not MemberAccessExpressionSyntax ma || ma.Name.Identifier.Text != "Execute")
        {
            return;
        }

        // 2. 仅 Heroes/ 业务策略文件 (DSL/Engine 内部不在范围).
        if (!PathHelper.IsHeroesPath(invocation.SyntaxTree.FilePath))
        {
            return;
        }

        // 3. 取 lambda block body.
        if (invocation.ArgumentList.Arguments.Count != 1)
        {
            return;
        }
        var lambdaArg = invocation.ArgumentList.Arguments[0].Expression;
        BlockSyntax? body = lambdaArg switch
        {
            ParenthesizedLambdaExpressionSyntax p => p.Body as BlockSyntax,
            SimpleLambdaExpressionSyntax s => s.Body as BlockSyntax,
            _ => null,
        };
        if (body is null)
        {
            return;
        }

        // 4. 排除含 if 控制流的异形 (小黑 形态: ToggleMode + if/else 条件播报).
        if (body.DescendantNodes().OfType<IfStatementSyntax>().Any())
        {
            return;
        }

        var invs = body.DescendantNodes().OfType<InvocationExpressionSyntax>().ToList();

        // 5a. 含 *.ToggleMode(...) 调用.
        bool hasToggleMode = invs.Any(i =>
            i.Expression is MemberAccessExpressionSyntax m && m.Name.Identifier.Text == "ToggleMode");
        if (!hasToggleMode)
        {
            return;
        }

        // 5b. 含 *.Speak(三元) 调用 (排除龙骑: Speak arg 是字符串拼接 BinaryExpression 而非纯三元).
        bool hasSpeakTernary = invs.Any(i =>
            i.Expression is MemberAccessExpressionSyntax m
            && m.Name.Identifier.Text == "Speak"
            && i.ArgumentList.Arguments.Count == 1
            && i.ArgumentList.Arguments[0].Expression is ConditionalExpressionSyntax);
        if (!hasSpeakTernary)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, ma.Name.GetLocation()));
    }
}
