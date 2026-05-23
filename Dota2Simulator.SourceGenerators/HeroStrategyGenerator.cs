using Microsoft.CodeAnalysis;

namespace Dota2Simulator.SourceGenerators;

/// <summary>
/// Phase 10C S1 hello-world: 验证 SG 项目串联 (与 PictureManifestGenerator / PictureHeroPreloadGenerator 共存).
/// S3 后扩 EmitStrategyPartial (扫 [HeroStrategy] attribute emit OnActivate/OnKeyAsync partial).
/// S4 后扩 EmitRegistry (emit HeroStrategyRegistry 静态映射, 删 4 个手写 partial).
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class HeroStrategyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // S1 hello-world: emit 单一标记文件验证 SG 项目串联
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource("HeroStrategyGenerator.Hello.g.cs",
                "// Phase 10C S1 HeroStrategyGenerator hello-world\n" +
                "// SG 项目串联实证 OK; S3 后扩 EmitStrategyPartial / S4 后扩 EmitRegistry\n");
        });
    }
}
