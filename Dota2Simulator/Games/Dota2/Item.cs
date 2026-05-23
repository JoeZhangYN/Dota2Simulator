// Games/Dota2/Item.cs
// Phase 8 C5 facade：原 774 行 static class Item 主体迁 GameAutomation/Application/ItemEngine.cs。
// 本类降级为 thin static facade —— 15 个 public 方法一行转发到 Common.ItemEngine。
// 外部无显式引用 `Item.物品信息` / `Item.物品4/5/6`（已 grep 0 命中），故 facade 不暴露嵌套类型。
// D1 删 Common.ItemEngine + 本 facade 时一并清。
#if DOTA2

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.Games;
using Dota2Simulator.Vision;

namespace Dota2Simulator.Games.Dota2
{
    internal static class Item
    {
        public static Task 根据按键判断技能释放前通用逻辑(KeyEventArgs e)
            => Common.ItemEngine!.根据按键判断技能释放前通用逻辑(e);

        public static Task 技能释放前切假腿(string 类型)
            => Common.ItemEngine!.技能释放前切假腿(类型);

        public static void 要求保持假腿()
            => Common.ItemEngine!.要求保持假腿();

        public static Task<bool> 切假腿类型(string type)
            => Common.ItemEngine!.切假腿类型(type);

        public static Rectangle 获取物品范围(int mode = 4)
            => GameAutomation.Application.ItemEngine.获取物品范围(mode);

        public static Rectangle 获取中立TP范围(int mode = 4)
            => GameAutomation.Application.ItemEngine.获取中立TP范围(mode);

        public static bool 重置耗蓝物品委托和条件()
            => Common.ItemEngine!.重置耗蓝物品委托和条件();

        public static Keys 根据图片获取物品按键(in ImageHandle 句柄)
            => Common.ItemEngine!.根据图片获取物品按键(in 句柄);

        public static int 根据图片使用物品(in ImageHandle 句柄)
            => Common.ItemEngine!.根据图片使用物品(in 句柄);

        public static int 根据图片自我使用物品(in ImageHandle 句柄)
            => Common.ItemEngine!.根据图片自我使用物品(in 句柄);

        public static bool 根据图片队列使用物品(in ImageHandle 句柄)
            => Common.ItemEngine!.根据图片队列使用物品(in 句柄);

        public static int 根据图片多次使用物品(in ImageHandle 句柄, int times, int 延迟)
            => Common.ItemEngine!.根据图片多次使用物品(in 句柄, times, 延迟);

        public static Task<bool> 所有物品可用后续(ImageHandle 句柄, Action afterAction)
            => Common.ItemEngine!.所有物品可用后续(句柄, afterAction);

        public static void 保存当前物品()
            => Common.ItemEngine!.保存当前物品();
    }
}

#endif
