// Phase 19G-4: 测试 Strategy 迁 HeroPlan — RequiresUi=true (唯一 UI 注入 Strategy). D1/D2/D3/D4 4 Execute (测试其他功能 / 捕捉颜色 / UI text 解析键映射 / 测试方法 多步骤宏).
#if DOTA2
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Application.HeroPlans;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

[HeroStrategy("测试", HeroAttribute.Universal, RequiresUi = true)]
public sealed partial class 测试Strategy : IHeroStrategy
{
    protected override HeroPlan BuildPlan() => HeroPlanBuilder.New()
        .OnKey(Keys.D1).Execute(() => Task.Run(测试其他功能))
        .OnKey(Keys.D2).Execute(() => Task.Run(() => _skill.捕捉颜色().Start()))
        .OnKey(Keys.D3).Execute(D3键映射执行)
        .OnKey(Keys.D4).Execute(() => Task.Run(() => _skill.测试方法(802, 946)))
        .Done();

    private void D3键映射执行()
    {
        _ = Task.Run(() => _skill.捕捉颜色().Start());
        Common.Delay(100);
        Dictionary<char, Keys> keyMapping = new()
        {
            { 'q', Keys.Q }, { 'w', Keys.W }, { 'e', Keys.E },
            { 'r', Keys.R }, { 'd', Keys.D }, { 'f', Keys.F }
        };
        string text = "";
        _ui!.Invoke(() => text = _ui.GetText(UiField.阵营).ToLower(CultureInfo.CurrentCulture));
        foreach (KeyValuePair<char, Keys> kvp in keyMapping)
        {
            if (text.Contains(kvp.Key))
            {
                Press(kvp.Value);
                break;
            }
        }
    }

    private void 测试其他功能()
    {
        _main._聚合.Skills.SetTime(SlotKey.Global, Common.获取当前时间毫秒());
        _item.保存当前物品();
        _ui!.Invoke(() => _ui.SetText(UiField.Y, (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Global)).ToString(CultureInfo.InvariantCulture)));
        Dota2Simulator.TTS.TTS.Speak("完成");
    }
}
#endif
