using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dota2Simulator.Games;

namespace Dota2Simulator.GameAutomation.Application;

/// <summary>
/// 15 个条件槽（C1~C9 + Z/X/C/V/B/Space）的容器，外加命石委托——
/// 收编 Main.cs 原「条件委托系统」：15 个 _条件N + 15 个 _条件根据图片委托N
/// + 条件配置项[] + 处理条件更新_带外部变化检测 + 更新条件数组 + 检查条件并执行委托。
///
/// 槽位按 <see cref="ConditionSlotKey"/> 索引，取代原 15 行同构的 条件配置项 构造。
/// </summary>
public sealed class ConditionSlotSet
{
    private const int 槽数 = 15;
    private readonly ConditionSlot[] _slots;

    /// <summary>
    /// Phase 18 V6a：委托签名 () 无参后, 此聚合内不再调 _vision.GetCurrentFrame()。
    /// 委托方法本身走 this._vision (在 Strategy 内) 取当前帧，ConditionSlotSet 退回纯状态容器形态。
    /// </summary>
    public ConditionSlotSet()
    {
        _slots = new ConditionSlot[槽数];
        for (int i = 0; i < 槽数; i++)
            _slots[i] = new ConditionSlot();
    }

    /// <summary>按槽位访问条件槽（ConditionSlot 是引用类型，读写其 Active/Probe 即原地生效）。</summary>
    public ConditionSlot this[ConditionSlotKey key] => _slots[(int)key];

    /// <summary>把指定条件槽置为激活（原 _条件X = true）。</summary>
    public void Activate(ConditionSlotKey key) => _slots[(int)key].Active = true;

    /// <summary>复位所有条件槽（原 取消所有功能 中的条件清零段）. Phase 20D 后命石复位迁 <see cref="StoneState.Reset"/>, 由 HeroAggregate / 切英雄路径调用.</summary>
    public void Reset()
    {
        foreach (ConditionSlot slot in _slots)
            slot.Clear();
    }

    /// <summary>
    /// 主循环每轮的条件更新——逐行移植自 Main.cs 处理条件更新_带外部变化检测。
    /// 保留「运行期外部变化检测」：在初始委托执行期间持续轮询被外部新置 true 的条件，
    /// 为其补跑委托。不可简化为单纯 Task.WhenAll——否则伐木机命石、Item 耗蓝物品的
    /// 运行期延迟委托注入会失效。
    /// </summary>
    public async Task TickAsync()
    {
        // 1. 快照初始条件状态
        bool[] 原始 = new bool[槽数];
        for (int i = 0; i < 槽数; i++)
            原始[i] = _slots[i].Active;
        bool[] 工作 = (bool[])原始.Clone();

        // 2. 启动初始的并行委托执行
        Task 更新任务 = 更新条件数组Async(工作);

        // 3. 委托执行期间，持续检测被外部新置 true 的条件并补跑委托
        var 已处理 = new HashSet<int>();
        while (!更新任务.IsCompleted)
        {
            for (int i = 0; i < 槽数; i++)
            {
                bool 当前 = _slots[i].Active;
                if (当前 && !原始[i] && !已处理.Contains(i))
                {
                    已处理.Add(i);
                    int idx = i; // 用局部变量捕获——修正原代码 for 变量被闭包捕获的陷阱
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            bool 结果 = await 检查并执行(true, _slots[idx].Probe).ConfigureAwait(false);
                            if (_slots[idx].Active)
                                _slots[idx].Active = 结果;
                        }
                        catch (Exception ex)
                        {
                            Common.Main_Logger.Error($"新条件委托执行失败 [{(ConditionSlotKey)idx}]: {ex.Message}");
                        }
                    });
                }
            }

            await Task.Delay(1).ConfigureAwait(true); // 避免 CPU 占用过高
        }

        // 4. 等待初始任务完成
        await 更新任务.ConfigureAwait(true);

        // 5. 应用初始 true 条件的检测结果（只有原始为 true 的槽才回写）
        for (int i = 0; i < 槽数; i++)
            if (原始[i])
                _slots[i].Active = 工作[i];
    }

    /// <summary>并行执行初始条件的委托检测——移植自 Main.cs 更新条件数组。</summary>
    private async Task 更新条件数组Async(bool[] 工作)
    {
        IEnumerable<Task> 检测任务 = _slots.Select(async (slot, index) =>
        {
            try
            {
                工作[index] = await 检查并执行(工作[index], slot.Probe).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Common.Main_Logger.Error($"条件检测失败 [{(ConditionSlotKey)index}]: {ex.Message}");
            }
        });

        await Task.WhenAll(检测任务).ConfigureAwait(false);
    }

    /// <summary>条件成立且委托非空才执行委托——移植自 Main.cs 检查条件并执行委托。
    /// Phase 18 V6a：委托签名 () 无参后，删 GetCurrentFrame() 调用；委托方法走 this._vision 直接取。</summary>
    private async Task<bool> 检查并执行(bool 条件, ConditionDelegateBitmap? 委托)
    {
        if (!条件 || 委托 is null) return 条件;
        return await 委托().ConfigureAwait(true);
    }
}
