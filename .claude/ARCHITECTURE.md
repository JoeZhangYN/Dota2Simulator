# Dota2Simulator 架构详情

## 项目配置

- **目标框架**: .NET 10.0-windows7.0 (WinExe, x64)
- **条件编译符**: `DOTA2`、`TRACE`、`Silt`
- **核心依赖**: OpenCvSharp4、Sdcb.PaddleOCR、SharpDX、NAudio、NLog、NeoSmart.AsyncLock

---

## 模块详解

### Form2.cs — UI 入口

**职责**: 全局键盘监听 + 游戏路由

```csharp
private async void Hook_KeyDown(object sender, KeyEventArgs e)
{
#if DOTA2
    await Games.Dota2.Main.根据当前英雄增强(tb_name.Text.Trim(), e);
#endif
}
```

**初始化流程** (`StartListen`):
1. 进程优先级设为 High
2. `PaddleOCR.初始化PaddleOcr()`
3. `GlobalScreenCapture.初始化(TripleBufferSystem)`
4. 注册全局键盘 Hook (`HookUserActivity`)

**UI 控件**:
- `tb_name` — 英雄名称
- `tb_阵营` — 技能数量 (4/5/6)
- `tb_状态抗性` — 状态抗性倍数
- `tb_攻速` — 攻击速度
- `tb_等待延迟` — 延迟/颜色调试显示

---

### Games/Dota2/Main.cs — 核心逻辑 (9612 行)

#### 主入口

```csharp
public static async Task 根据当前英雄增强(string name, KeyEventArgs e)
```

**执行流程**:
1. `switch(name)` 分发到对应英雄
2. 首次调用: 初始化条件委托 (`_条件根据图片委托1~9`) + `await 状态初始化()`
3. `await Item.根据按键判断技能释放前通用逻辑(e)` — 处理物品/假腿
4. 按键路由: 设置对应条件布尔 (`_条件1~9`, `_条件z~space`)

#### 主循环系统

```
状态初始化()
  └→ 一般程序循环()   (后台 Task 循环)
       ├→ _循环内获取图片()  (截图: 模式1=局部/模式2=全屏)
       ├→ _条件根据图片委托1~9  (各自独立的条件检查)
       ├→ _命石根据图片委托     (命石选择)
       └→ _条件根据图片委托z/x/c/v/b/space  (物品CD检测)
```

**截图区域**:
- 模式1 (局部): `(671, 727, 760, 346)` — Buff/技能/物品区域
- 模式2 (全屏): `(0, 0, 1920, 1080)` — 完整界面

**坐标偏移**: `坐标偏移x / 坐标偏移y` — 根据截图模式动态调整所有像素坐标

#### 全局状态字段

```csharp
// 条件委托 (图片检测回调)
ConditionDelegateBitmap _条件根据图片委托1~9
ConditionDelegateBitmap _条件根据图片委托z/x/c/v/b/space
ConditionDelegateBitmap _命石根据图片委托

// 条件布尔 (由按键触发)
bool _条件1~9, _条件z/x/c/v/b/space, _中断条件, _总循环条件

// 全局时间戳 (去后摇计时)
long _全局时间, _全局时间q/w/e/r/d/f

// 全局步骤 (多步骤技能状态机)
int _全局步骤, _全局步骤e/r
Point _指定地点d/e

// 战斗参数
double _基础攻击前摇=0.5, _基础攻击间隔=1.7, _攻击速度=100
double _状态抗性倍数

// Buff/区域定义
Rectangle buff状态技能栏 = (962, 826, 526, 80)
Rectangle debuff状态技能栏 = (435, 826, 526, 80)
Rectangle 命石区域 = (738, 945, 70, 26)
```

#### 支持的英雄 (88个)

**力量系**: 大牛、发条、尸王、伐木机、全能、军团、骷髅王、人马、哈斯卡、小狗、土猫、孽主、小小、海民、屠夫、斧王、大鱼人、斯温、船长、夜魔、树精、混沌、马尔斯、破晓晨星、钢背、龙骑、小骷髅

**敏捷系**: 小黑、巨魔、幻刺、猴子、幽鬼、影魔、TB、敌法、小鱼人、小松鼠、火猫、拍拍、火枪、飞机、美杜莎、虚空、血魔、赏金、电棍、露娜、大圣、剃刀、修补匠

**智力系**: 光法、天怒、墨客、宙斯、巫医、巫妖、帕克、骨法、干扰者、黑鸟、谜团、冰女、火女、蓝猫、卡尔、拉席克、术士、暗影萨满、小仙女、炸弹人、神域、莱恩、沉默、戴泽、双头龙、奶绿、女王、蓝胖、祸乱之源、瘟疫法师、剧毒、猛犸、狼人、紫猫、VS、小精灵、马西、小强

#### 条件委托模式 (去后摇核心机制)

每个条件委托对应一个技能的"按键后→等待条件满足→执行后续"逻辑:

```csharp
// 典型委托: 技能释放后等待CD变化 → 平A后摇
private static async Task<bool> 回音践踏去后摇(ImageHandle 句柄)
    => await Skill.技能通用判断(Keys.Q, 1, 判断成功后延时: 1300);

// 复杂委托: 多步骤状态机
private static async Task<bool> 决斗(ImageHandle 句柄)
{
    switch (获取全局步骤()) {
        case < 1: 使用物品(); 设置全局步骤(1); return true;
        case < 2: 跳刀(); 设置全局步骤(2); return true;
        case < 3: 物品连招(); 设置全局步骤(3); return true;
        case < 4: 释放R技能(); 重置步骤(); return false;
    }
}
```

---

### Games/Dota2/Item.cs — 物品管理

#### 物品槽布局 (1080p, 技能4/5/6 三套)

```
技能4: 起始x=1136  技能5: 起始x=1150  技能6: 起始x=1180
物品间隔x=67  物品间隔y=48  共2行×3列=6格  按键: Z X C V B Space
```

#### 核心方法

| 方法 | 功能 |
|------|------|
| `根据按键判断技能释放前通用逻辑(e)` | 主入口: 检测假腿/神杖/魔晶 + 路由F1/Esc/NumPad |
| `根据图片使用物品(handle)` | 图片匹配→找到物品槽→KeyPress |
| `根据图片自我使用物品(handle)` | 同上但用 Alt+Key |
| `根据图片多次使用物品(handle, n, delay)` | 多次使用 |
| `根据图片获取物品按键(handle)` | 只查找不按键，返回 Keys |
| `切假腿类型(type)` | 智力/敏捷/力量假腿切换 |
| `DOTA2判断序号物品是否CD(i, handle)` | 检查第i格物品右上角颜色 (104,104,104) |
| `DOTA2判断任意物品是否锁闭(handle)` | 6格锁闭状态全检 |
| `阿哈利姆神杖(handle)` | 检测神杖颜色 (30,187,250) |
| `阿哈利姆魔晶(handle)` | 检测魔晶颜色 (30,187,254) |

#### 假腿切换状态机

```
_条件开启切假腿  — F1初始化后设置
_条件保持假腿   — 技能释放前置为false, 使用物品后置为true
_需要切假腿     — 标记需要恢复假腿
_条件假腿敏捷   — true=敏捷腿, false=力量腿
```

#### 物品CD检测委托

F1 键扫描物品栏，为 Z/X/C/V/B/Space 位的耗蓝物品设置 `_条件根据图片委托z~space`，检测到物品进入 CD 后触发 `要求保持假腿()`。

---

### Games/Dota2/Skill.cs — 技能检测

#### 技能信息参数 (4/5/6技能三套)

```
技能4: 左下角(820, 998)  间隔65px  按键[Q,W,E,R]
技能5: 左下角(802, 995)  间隔58px  按键[Q,W,E,D,R]
技能6: 左下角(772, 995)  间隔58px  按键[Q,W,E,D,F,R]
```

#### 技能类型枚举

| 类型 | 检测内容 | 典型颜色 |
|------|---------|---------|
| 图标CD | 技能CD灰色 | (194,198,202) |
| 法球 | 法球状态 | (34,40,39) |
| 状态 | 状态技能激活 | (0,129,0) 绿 |
| 释放变色 | 技能释放瞬间变色 | 动态 |
| QWERDF图标 | 框颜色 | (54,62,70) |
| 被动技能存在 | 被动底色 | (49,51,47) |

#### 核心方法

| 方法 | 功能 |
|------|------|
| `DOTA2判断技能是否CD(key, handle)` | 技能是否在CD |
| `DOTA2释放CD就绪技能(key, handle)` | CD就绪则释放 |
| `技能通用判断(key, mode, 延时)` | 最常用: 等技能变色/进CD后平A |
| `释放技能后替换图标技能后续(key, get, set)` | 替换图标类技能 (如W召唤) |
| `主动技能释放后续(key, action)` | 释放后执行自定义动作 |
| `设置当前技能数量()` | 检测当前英雄有4/5/6个技能 |

---

### PictureProcessing/ — 图像处理

#### GlobalScreenCapture (全局截图)

- 三重缓冲 (`TripleBufferSystem`) 实现零拷贝帧交付
- `GetCurrentHandle()` → `ImageHandle` (当前帧引用)
- `GetColor(x, y)` → 直接读像素

#### ImageManager

- `GetColor(handle, x, y)` → 读指定坐标颜色
- `SaveImage(handle, path, rect)` → 保存区域截图

#### ImageFinder

- `FindImageInRegion(template, frame, region)` → 模板匹配，返回 `Point?`
- `FindImageInRegionBool(...)` → 返回 bool
- 匹配率阈值默认 0.8

#### ColorExtensions

- `ColorAEqualColorB(c1, c2, tolerance)` → 颜色容差比较

---

### KeyboardMouse/SimKeyBoard.cs — 键鼠模拟

**后端**: InterceptionInput (驱动级) / SimEnigo (用户层)

| 方法 | 说明 |
|------|------|
| `KeyPress(key)` | 按下+抬起, ~1-3ms |
| `KeyDown(key)` / `KeyUp(key)` | 分离控制 |
| `KeyPressAlt(key)` | Alt+key |
| `KeyPressWhile(key, modifier)` | 修饰键+key |
| `MouseMove(x, y)` | 绝对坐标移动 |
| `MouseMoveSim(x, y)` | 15帧插值平滑移动 |
| `MouseLeftClick()` | 左键单击 |
| `MouseRightClick()` | 右键单击 |

---

### Games/Dota2/Dota2_Pictrue.cs — 图片资源

懒加载模式: `LazyImageLoader.GetImage()` 按需从嵌入资源加载

**分类**:
- `Buff` (8): 大牛回魂、光法大招、小精灵幽魂/过载、火猫无影拳
- `命石` (4): 伐木机碎木击/锯齿轮旋、海民酒友、骷髅王白骨守卫
- `英雄技能` (4): 卡尔系
- `物品` (69+): 假腿3种、跳刀4种、神杖/魔晶、黑皇杖、刷新球…
- `Silt` (91, `#if Silt`): 地下城英雄技能图标

---

### Games/Common.cs — 公共工具

```csharp
// 高精度延迟 (< 2ms 用 SpinWait, >= 2ms 用 Thread.Sleep)
public static void Delay(int delay, long time = -1)

// Unix 毫秒时间戳
public static long 获取当前时间毫秒()

// 全局窗口引用
public static Form2 Main_Form;

// NLog 日志
public static Logger Logger;
```

---

## 数据流

```
键盘事件 (全局Hook)
  → Hook_KeyDown
  → 根据当前英雄增强(name, e)
  → Item.根据按键判断技能释放前通用逻辑(e)
       ├ 检测假腿/神杖/魔晶
       ├ F1: 扫描物品栏，设置CD委托
       └ 其他键: 根据配置切假腿
  → 英雄switch: 设置条件委托 + 触发条件布尔

后台主循环 (一般程序循环)
  → 截图 (_循环内获取图片)
  → 检查条件委托1~9 (技能去后摇逻辑)
  → 检查条件委托z~space (物品CD监控)
  → 检查命石委托 (命石选择)
```

---

## 坐标系说明

全部坐标基于 **1920×1080** 分辨率，Dota2 UI 设置:
- 主界面高画质
- 动态肖像

`坐标偏移x/y` 根据截图模式调整:
- 模式1 (局部截图): `偏移x=671, 偏移y=727`
- 模式2 (全屏截图): `偏移x=0, 偏移y=0`
