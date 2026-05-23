#nullable enable
using System;
using System.Collections.Generic;

namespace Dota2Simulator.SourceGenerators;

internal static class PictureCategoryMap
{
    public static readonly IReadOnlyDictionary<string, string> DirToClassName =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["BUFF"] = "Buff",
            ["命石"] = "命石",
            ["技能"] = "英雄技能",
            ["播报"] = "播报信息",
            ["物品"] = "物品",
            ["Silt"] = "Silt",
        };

    public static readonly IReadOnlyDictionary<string, string> ClassNameToManifestPrefix = BuildReverse();

    private static IReadOnlyDictionary<string, string> BuildReverse()
    {
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var kv in DirToClassName) dict[kv.Value] = kv.Key;
        return dict;
    }
}
