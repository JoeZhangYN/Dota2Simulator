#if DOTA2
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.GameAutomation.Application;
using Dota2Simulator.GameAutomation.Domain.Actuation;
using Dota2Simulator.GameAutomation.Domain.Heroes;
using Dota2Simulator.GameAutomation.Domain.Loop;
using Dota2Simulator.Games;
using Dota2Simulator.Games.Dota2;
using Dota2Simulator.GameAutomation.Ports;

namespace Dota2Simulator.GameAutomation.Heroes.Universal;

/// <summary>测试策略——迁移自 _main.根据当前英雄增强 的 case "测试"。</summary>
[HeroStrategy("测试", HeroAttribute.Universal, RequiresUi = true)]
public sealed partial class 测试Strategy : IHeroStrategy
{


    public override void OnActivate(HeroContext ctx)
    {
    }

    public override async Task OnKeyAsync(KeyTrigger trigger, HeroContext ctx)
    {
        VirtualKey key = trigger.Key;
        if (key == VirtualKey.From(Keys.D1))
        {
            _ = Task.Run(测试其他功能).ConfigureAwait(true);
        }
        else if (key == VirtualKey.From(Keys.D2))
        {
            _ = Task.Run(() => { _skill.捕捉颜色().Start(); }).ConfigureAwait(false);
        }
        else if (key == VirtualKey.From(Keys.D3))
        {
            _ = Task.Run(() => { _skill.捕捉颜色().Start(); }).ConfigureAwait(false);
            Common.Delay(100);
            Dictionary<char, Keys> keyMapping = new()
            {
                { 'q', Keys.Q },
                { 'w', Keys.W },
                { 'e', Keys.E },
                { 'r', Keys.R },
                { 'd', Keys.D },
                { 'f', Keys.F }
            };

            string text = "";
            _ui.Invoke(() =>
            {
                text = _ui.GetText(UiField.阵营).ToLower(CultureInfo.CurrentCulture); // 转换为小写，确保匹配时忽略大小写
            });

            foreach (KeyValuePair<char, Keys> kvp in keyMapping)
            {
                if (text.Contains(kvp.Key))
                {
                    _input.Press(VirtualKey.From(kvp.Value));
                    break; // 如果找到匹配项，退出循环
                }
            }
        }
        else if (key == VirtualKey.From(Keys.D4))
        {
            await Task.Run(() => _skill.测试方法(802, 946)).ConfigureAwait(false);
        }
    }

    private void 测试其他功能()
    {
        _main._聚合.Skills.SetTime(SlotKey.Global, Common.获取当前时间毫秒());

        _item.保存当前物品();

        _ui.Invoke(() => _ui.SetText(UiField.Y, (Common.获取当前时间毫秒() - _main._聚合.Skills.Time(SlotKey.Global)).ToString(CultureInfo.InvariantCulture)));

        Dota2Simulator.TTS.TTS.Speak("完成");
    }
}
#endif
