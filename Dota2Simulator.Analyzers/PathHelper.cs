using System;

namespace Dota2Simulator.Analyzers;

internal static class PathHelper
{
    public static bool IsHeroesPath(string path)
        => path.Contains("\\Heroes\\") || path.Contains("/Heroes/");

    public static bool IsBusinessPath(string path)
    {
        return path.Contains("\\GameAutomation\\Application\\")
            || path.Contains("/GameAutomation/Application/")
            || IsHeroesPath(path);
    }

    /// <summary>
    /// DS0003 GetCurrentHandle 白名单 — 当前文件级粒度:
    ///   - HeroLoopHost.cs: 生命周期 (IsValid / ReleaseImage) 2 处
    ///   - ItemEngine.cs: Silt 子系统 ImageHandle 透传 5 处 (Phase 11 Silt instance 化后收紧) + SaveImage 调试 1 处
    /// 后续 Phase 11 Silt 拆出独立文件后白名单收紧到 Silt*.cs.
    /// </summary>
    public static bool IsWhitelistedForGetCurrentHandle(string path)
    {
        return path.EndsWith("HeroLoopHost.cs", StringComparison.Ordinal)
            || path.EndsWith("ItemEngine.cs", StringComparison.Ordinal);
    }
}
