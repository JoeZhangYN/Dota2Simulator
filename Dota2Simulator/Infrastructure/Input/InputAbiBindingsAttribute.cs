using System;

namespace Dota2Simulator.Infrastructure.Input;

/// <summary>
/// 标记一个 partial 绑定类，令 <c>InputAbiExternGenerator</c> 从 input_abi 契约 manifest
/// （Infrastructure/Input/input_abi.contract.txt）生成其 42 个 <see cref="System.Runtime.InteropServices.DllImportAttribute"/>
/// extern 声明。<paramref name="dllName"/> = 目标后端 DLL 文件名。
/// </summary>
/// <remarks>
/// 仅生成可机投影的标量签名符号；<c>LastError</c>（缓冲协议）与 <c>simengio_text</c>
/// （契约外）刻意留各绑定手写。
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
internal sealed class InputAbiBindingsAttribute : Attribute
{
    public InputAbiBindingsAttribute(string dllName) => DllName = dllName;

    public string DllName { get; }
}
