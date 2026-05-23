# Other/ — TODO-dead-Phase7-remove

## 状态

本目录所有 13 个驱动二进制 (`.sys / .inf / .cat / .dll / .exe`) **不被任何活代码引用**，仅 `_Legacy/KeyboardMouse/DriverStageHelper/*.cs` 历史使用，已在 `Dota2Simulator.csproj` 第 34-36 行 Compile Remove。

## 清单

```
Depends_x64.exe         — 依赖分析工具
kmclass.cat             — kmclass 驱动证书
kmclass.inf             — kmclass 驱动安装信息
kmclass.sys             — kmclass 驱动二进制
kmclassdll.dll          — kmclass 驱动 DLL
WinIo32.dll / .sys      — WinIO 32 位驱动 (_Legacy/WinIO32.cs 用)
WinIo64.dll / .sys      — WinIO 64 位驱动 (_Legacy/WinIO64.cs 用)
WinRing0.dll / .sys     — WinRing0 32 位驱动
WinRing0x64.dll / .sys  — WinRing0 64 位驱动
```

## Phase 7 计划

待 _Legacy/ 整目录删除时，`git rm -r Other/` 一并清理。
本目录无对应 csproj `<None Update>` 行（驱动二进制未被编译期 copy），删除零副作用。
