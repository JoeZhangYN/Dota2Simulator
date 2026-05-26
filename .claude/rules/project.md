# 项目规则

## 构建与运行

```bash
# 用 Visual Studio 2022 或 dotnet CLI
dotnet build -c Release

# 启动 (需要管理员权限，用于驱动级键鼠模拟)
dotnet run -c Release
```

**注意**: 必须以管理员身份运行，驱动级键鼠需要提权。

## 条件编译

| 符号 | 启用内容 |
|------|---------|
| `DOTA2` | Dota2 全部逻辑 (Main/Item/Skill/Dota2_Pictrue) |
| `Silt` | Underlords 地下城模式 (Silt/Main, Silt/DynamicSkillAutoSelector) |
| `LOL` | League of Legends 模式 |
| `HF2` | Helldivers 2 模式 |

在 `.csproj` 的 `<DefineConstants>` 中配置。

## 新增英雄步骤

1. 在 `Games/Dota2/Main.cs` 的 `根据当前英雄增强` switch 中添加 `case "英雄名":` 块
2. 在 `#region Dota2具体实现` 下添加对应的条件委托方法
3. 如需新图片资源，在 `Dota2_Pictrue.cs` 的对应分类中添加懒加载属性
4. 图片资源添加为嵌入式资源 (Properties → Resources)

## 条件委托规范

```csharp
// 标准模式: 等待技能进CD后执行后续
private static async Task<bool> XxxSkill去后摇(ImageHandle 句柄)
    => await Skill.技能通用判断(Keys.Q, 1);
//                               ^     ^ 0=接平A, 1=不接平A

// 命石模式: 检测后置null
private static async Task<bool> XxxHero获取命石(ImageHandle 句柄)
{
    if (_命石选择 == 0) { /* 匹配图片设置_命石选择 */ }
    _命石根据图片委托 = null;
    return await Task.FromResult(false);
}
```

## 物品坐标规范

所有物品坐标基于 1920×1080，使用 `Main.坐标偏移x/y` 修正:

```csharp
// 正确: 使用坐标偏移
ImageManager.GetColor(in 句柄, x - Main.坐标偏移x, y - Main.坐标偏移y)

// 错误: 硬编码绝对坐标
ImageManager.GetColor(in 句柄, 1136, 943)
```

## 延迟规范

```csharp
const int 等待延迟 = 33;  // 基准帧延迟 (≈30fps)

// 物品使用间隔
Common.Delay(33 * Item.根据图片使用物品(xxx));  // 使用到了就延迟33ms
```

## 性能注意

- 主循环截图模式1 (局部) 比模式2 (全屏) 快约3倍，优先用模式1
- `ImageFinder.FindImageInRegion` 指定 `region` 范围可显著提速
- 条件委托返回 `false` 表示条件未满足继续等待，`true` 等待下一次触发前可做清理

## 已知坐标参考

| 区域 | 坐标 |
|------|------|
| Buff状态栏 | (962, 826, 526, 80) |
| Debuff状态栏 | (435, 826, 526, 80) |
| 命石区域 | (738, 945, 70, 26) |
| 技能4物品起始x | 1136 |
| 技能5物品起始x | 1150 |
| 技能6物品起始x | 1180 |
| 物品行y | 943 |
| 物品间隔x | 67 (62+5) |
| 物品间隔y | 48 (45+3) |
| 神杖/魔晶x (技能4) | 1096 |
| 神杖y | 959 |
| 魔晶y | 994 |

## 调试技巧

```csharp
// 获取指定坐标颜色 (调试用)
var color = await Main.获取指定位置颜色(x, y);
Common.Main_Form.Invoke(() => Common.Main_Form.tb_等待延迟.Text = color.ToString());

// 保存当前物品截图
Item.保存当前物品();  // 保存到 J:\Desktop\物品_1~6.bmp
```
