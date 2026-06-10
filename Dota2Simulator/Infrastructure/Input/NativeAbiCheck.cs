using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Dota2Simulator.KeyboardMouse;

namespace Dota2Simulator.Input;

/// <summary>
/// 启动期输入后端 ABI 对账：用反射收集绑定类的全部 [DllImport] 入口名，
/// 逐一 <see cref="NativeLibrary.TryGetExport"/> 核对部署的 DLL。
/// 把「部署了旧版 DLL / DLL 与绑定声明漂移」从运行时静默失败（EntryPointNotFoundException
/// 延迟爆雷或错误被吞）变成启动即报错——本仓实锤踩过部署 simengio.dll 与源码漂移的坑。
/// 名单 SSOT = 绑定类的 DllImport 声明本身（exe 实际需要的符号集）。
/// </summary>
public static class NativeAbiCheck
{
    /// <summary>核对两个输入后端 DLL 的导出面与绑定声明一致。不一致抛带缺失清单的异常。</summary>
    public static void EnsureContractSurface()
    {
        Verify("interception_input.dll", typeof(InterceptionInput));
        Verify("simengio.dll", typeof(SimEnigo));
    }

    private static void Verify(string dllName, Type binding)
    {
        var entryPoints = binding
            .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => (Method: m, Attr: m.GetCustomAttribute<DllImportAttribute>()))
            .Where(t => t.Attr is not null)
            .Select(t => t.Attr!.EntryPoint ?? t.Method.Name)
            .Distinct()
            .ToList();

        IntPtr handle;
        try
        {
            handle = NativeLibrary.Load(dllName, binding.Assembly, DllImportSearchPath.ApplicationDirectory);
        }
        catch (DllNotFoundException e)
        {
            throw new InvalidOperationException($"输入后端 {dllName} 未找到（确认已随程序部署）", e);
        }

        List<string> missing = entryPoints.Where(n => !NativeLibrary.TryGetExport(handle, n, out _)).ToList();
        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                $"{dllName} 缺失导出符号: {string.Join(", ", missing)}。" +
                "部署的 DLL 与绑定声明不一致——检查是否为旧版 DLL（需与本 exe 同批构建部署）。");
        }
    }
}
