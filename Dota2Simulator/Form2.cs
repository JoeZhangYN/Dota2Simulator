// #define 检测延时
// #define DEBUG

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Collections.Pooled;
using Dota2Simulator.KeyboardMouse;
using PaddleOCRSharp;
using WindowsHook;
using static Dota2Simulator.Picture_Dota2.Resource_Picture;
using static Dota2Simulator.PictureProcessing;
using static Dota2Simulator.SetWindowTop;
using static System.Threading.Tasks.Task;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;
using Keys = System.Windows.Forms.Keys;
using System.Reflection.Metadata.Ecma335;
// WindowsHook.KeyEventArgs
// WindowsHook.KeyEventHandler
// WindowsHook.Keys

namespace Dota2Simulator;

public partial class Form2 : Form
{
    /// 动态肖像 法线贴图 地面视差 主界面高画质 计算器渲染器 纹理、特效、阴影 中 渲染 70% 高级水面效果
    private const int 截图模式1X = 750;

    private const int 截图模式1Y = 856;
    private const int 截图模式1W = 657;
    private const int 截图模式1H = 217;
    private const int 等待延迟 = 30;

    #region 触发重载

    /// <summary>
    ///     按键触发，名称指定操作
    ///     如直接写在方法内则在按键触发前生效
    ///     例如可以切假腿放技能
    ///     if (e.KeyValue == (int)Keys.A && (int)Control.ModifierKeys == (int)Keys.Alt) && (int)Control.ModifierKeys ==
    ///     (int)Keys.Control)
    ///     ctrl + alt + A 捕获
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Hook_KeyDown(object sender, KeyEventArgs e)
    {
        #region 总开关

        switch (e.KeyCode)
        {
            case Keys.Home:
                _总开关条件 = !_总开关条件;
                TTS.Speak(_总开关条件 ? "开启功能" : "关闭功能");
                KeyPress((uint)Keys.End);
                Delay(等待延迟);
                break;
            case Keys.Insert:
                取消所有功能();
                TTS.Speak("已重置功能");
                break;
        }

        #endregion

        #region 打字时屏蔽功能 (未使用)

        //if (false
        //    // && CaptureColor(572, 771).Equals(SimpleColor.FromRgb(237, 222, 190))
        //   )
        //{
        //}

        #endregion

        #region 记录时间 (未使用)

        #endregion

        switch (tb_name.Text.Trim())
        {
            #region 力量

            #region 船长

            case "船长" when e.KeyCode == Keys.D2:
                label1.Text = "D2";

                KeyPress((uint)Keys.Q);

                await Run(洪流接x回);
                break;
            case "船长" when e.KeyCode == Keys.D3:
                label1.Text = "D3";

                await Run(最大化x伤害控制);
                break;
            case "船长":
                {
                    if (e.KeyCode == Keys.D4)
                    {
                        label1.Text = "D4";

                        KeyPress((uint)Keys.Q);

                        await Run(洪流接船);
                    }

                    break;
                }

            #endregion

            #region 军团

            case "军团":
                {
                    if (!_总循环条件)
                    {
                        _条件根据图片委托1 ??= 决斗;
                        _总循环条件 = true;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.E:
                            _全局时间 = -1;
                            _全局步骤 = 0;
                            _中断条件 = false;
                            _条件1 = true;
                            break;
                        case Keys.D2 when _全局模式 == 0:
                            TTS.Speak("切换无视野模式");
                            _全局模式 = 1;
                            break;
                        case Keys.D2:
                            TTS.Speak("切换有视野模式");
                            _全局模式 = 0;
                            break;
                        case Keys.H:
                            _中断条件 = true;
                            _条件1 = false;
                            break;
                    }

                    break;
                }

            #endregion

            #region 斧王

            case "斧王":
                {
                    if (!_总循环条件)
                    {
                        _条件根据图片委托1 ??= 吼去后摇;
                        _条件根据图片委托2 ??= 战斗饥渴去后摇;
                        _条件根据图片委托2 ??= 淘汰之刃去后摇;
                        _条件根据图片委托4 ??= 跳吼;
                        _总循环条件 = true;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否a杖)
                    {
                        _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            根据图片以及类别使用物品(物品_魂戒CD, _全局bts, _全局size, _技能数量);
                            _条件1 = true;
                            break;
                        case Keys.W:
                            根据图片以及类别使用物品(物品_魂戒CD, _全局bts, _全局size, _技能数量);
                            _条件2 = true;
                            break;
                        case Keys.E:
                            _条件4 = true;
                            break;
                        case Keys.R:
                            根据图片以及类别使用物品(物品_魂戒CD, _全局bts, _全局size, _技能数量);
                            _条件3 = true;
                            break;
                        case Keys.D2:
                            switch (_全局模式q)
                            {
                                case 1:
                                    _全局模式q = 0;
                                    TTS.Speak("吼不接刃甲");
                                    break;
                                case 0:
                                    _全局模式q = 1;
                                    TTS.Speak("吼接刃甲");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 孽主

            case "孽主":
                {
                    if (e.KeyCode == Keys.E)
                    {
                        label1.Text = "E";

                        await Run(深渊火雨阿托斯);
                    }

                    break;
                }

            #endregion

            #region 哈斯卡

            case "哈斯卡" when e.KeyCode == Keys.D2:
                label1.Text = "D2";

                await Run(切臂章);
                break;
            case "哈斯卡" when e.KeyCode == Keys.Q:
                label1.Text = "Q";

                await Run(心炎平a);
                break;
            case "哈斯卡":
                {
                    if (e.KeyCode == Keys.R)
                    {
                        label1.Text = "R";

                        //if (RegPicture(物品_臂章, "Z"))
                        //{
                        //    KeyPress((uint) Keys.Z);
                        //    Delay(等待延迟);
                        //}

                        await Run(牺牲平a刃甲);
                    }

                    break;
                }

            #endregion

            #region 海民

            case "海民":
                {
                    if (!_总循环条件)
                    {
                        _条件根据图片委托1 ??= 跳接勋章接摔角行家;
                        _条件根据图片委托2 ??= 摔角行家去后摇;
                        _条件根据图片委托3 ??= 飞踢接雪球;
                        _总循环条件 = true;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否a杖)
                    {
                        _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                        if (_是否a杖) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.G:
                            _条件1 = true;
                            break;
                        case Keys.E:
                            根据图片以及类别使用物品(物品_勇气勋章, _全局bts, _全局size, _技能数量);
                            根据图片以及类别使用物品(物品_炎阳勋章, _全局bts, _全局size, _技能数量);
                            _条件2 = true;
                            break;
                        case Keys.D:
                            _条件3 = true;
                            break;
                        case Keys.D2:
                            _指定地点d = MousePosition;
                            TTS.Speak("已指定地点");
                            break;
                        case Keys.D3:
                            if (_是否a杖)
                            {
                                var p = MousePosition;
                                KeyDown((uint)Keys.D);
                                Delay(等待延迟);
                                MouseMove(_指定地点d);
                                Delay(等待延迟);
                                KeyUp((uint)Keys.D);
                                Delay(等待延迟);
                                MouseMove(p);
                                _条件3 = true;
                            }

                            break;

                        case Keys.D4:
                            if (_是否a杖)
                            {
                                KeyDown((uint)Keys.Space);
                                Delay(等待延迟);
                                KeyDown((uint)Keys.W);
                                Delay(等待延迟);
                                var p = MousePosition;
                                KeyDown((uint)Keys.D);
                                Delay(等待延迟);
                                MouseMove(_指定地点d);
                                Delay(等待延迟);
                                KeyUp((uint)Keys.D);
                                Delay(等待延迟);
                                MouseMove(p);
                                _条件3 = true;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 钢背

            case "钢背":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 鼻涕针刺循环;
                        _条件根据图片委托2 ??= 毛团去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }

                    if (!_是否a杖) _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);

                    switch (e.KeyCode)
                    {
                        case Keys.D:
                            {
                                _条件开启切假腿 = false;
                                初始化全局时间(ref _全局时间d);
                                切智力腿(_技能数量);
                                _条件2 = true;
                                break;
                            }
                        case Keys.D2:
                            {
                                if (!_条件1)
                                    _条件1 = true;
                                _循环条件1 = !_循环条件1;
                                // 基本上魂戒可以放4下，只浪费10点蓝
                                // 配合一次鼻涕就一次也不浪费
                                if (_循环条件1)
                                    if (RegPicture(物品_魂戒CD, _全局bts, _全局size))
                                    {
                                        切力量腿(_全局bts, _全局size, _技能数量);
                                        根据图片以及类别使用物品(物品_魂戒CD, _全局bts, _全局size, _技能数量);
                                    }

                                break;
                            }
                        case Keys.D3:
                            {
                                if (!_条件1)
                                    _条件1 = true;
                                _循环条件2 = !_循环条件2;
                                if (_循环条件2)
                                    if (RegPicture(物品_魂戒CD, _全局bts, _全局size))
                                    {
                                        切力量腿(_全局bts, _全局size, _技能数量);
                                        根据图片以及类别使用物品(物品_魂戒CD, _全局bts, _全局size, _技能数量);
                                    }

                                break;
                            }
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                        case Keys.D5 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D5:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                    }

                    break;
                }

            #endregion

            #region 猛犸

            case "猛犸":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 = 切回假腿;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间q);
                            切智力腿(_技能数量);
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间q);
                            切智力腿(_技能数量);
                            _条件1 = true;
                            break;
                        case Keys.E:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间q);
                            切智力腿(_技能数量);
                            _条件1 = true;
                            break;
                        case Keys.R:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间r);
                            切智力腿(_技能数量);
                            _条件1 = true;
                            break;
                        case Keys.F:
                            await Run(跳拱指定地点);
                            break;
                        case Keys.D2:
                            await Run(指定地点);
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 屠夫

            case "屠夫":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 阿托斯接钩子;
                        _条件根据图片委托2 ??= 钩子去僵直;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.D1:
                            _条件1 = true;
                            break;
                        case Keys.Q:
                            _条件2 = true;
                            break;
                        case Keys.D2:
                            switch (_全局模式q)
                            {
                                case 0:
                                    _全局模式q = 1;
                                    TTS.Speak("勾接咬");
                                    break;
                                case 1:
                                    _全局模式q = 0;
                                    TTS.Speak("勾不接咬");
                                    break;
                            }

                            break;
                        case Keys.D3:
                            KeyPress((uint)Keys.S);
                            var w4 = 获取w4开关颜色(_全局bts, _全局size);
                            var w5 = 获取w5开关颜色(_全局bts, _全局size);
                            switch (_是否魔晶)
                            {
                                case true when !ColorAEqualColorB(w5, SimpleColor.FromRgb(0, 129, 0), 0):
                                case false when !ColorAEqualColorB(w4, SimpleColor.FromRgb(0, 129, 0), 0):
                                    KeyPressWhile((uint)Keys.W, (uint)Keys.LShiftKey);
                                    break;
                            }

                            KeyPressWhile((uint)Keys.Space, (uint)Keys.LShiftKey);
                            根据图片以及类别队列使用物品(物品_纷争, _全局bts, _全局size, _技能数量);
                            根据图片以及类别队列使用物品(物品_虚灵之刃, _全局bts, _全局size, _技能数量);
                            KeyPressWhile((uint)Keys.R, (uint)Keys.LShiftKey);
                            break;
                    }

                    break;
                }

            #endregion

            #region 破晓晨星

            case "破晓晨星":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 石破天惊使用物品;
                        _条件根据图片委托2 ??= 上界重锤去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否魔晶) _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            {
                                根据图片以及类别使用物品(物品_勇气勋章, _全局bts, _全局size);
                                根据图片以及类别使用物品(物品_炎阳勋章, _全局bts, _全局size);
                                if (_是否魔晶) _条件1 = true;

                                break;
                            }
                        case Keys.W:
                            {
                                根据图片以及类别使用物品(物品_魂戒CD, _全局bts, _全局size);
                                _条件2 = true;
                                break;
                            }
                        case Keys.R:
                            {
                                根据图片以及类别使用物品(物品_魂戒CD, _全局bts, _全局size);
                                break;
                            }
                    }

                    break;
                }

            #endregion

            #region 大鱼人

            case "大鱼人":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 踩去后摇;
                        _条件根据图片委托2 ??= 跳刀接踩;
                        _条件根据图片委托3 ??= 雾霭去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否魔晶) _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);


                    switch (e.KeyCode)
                    {
                        case Keys.W:
                            {
                                _条件保持假腿 = false;
                                根据图片以及类别使用物品(物品_勇气勋章, _全局bts, _全局size);
                                根据图片以及类别使用物品(物品_炎阳勋章, _全局bts, _全局size);
                                切智力腿(_技能数量);
                                _全局模式w = _是否魔晶 ? 1 : 0;
                                _条件1 = true;
                                break;
                            }
                        case Keys.E:
                            {
                                _条件保持假腿 = false;
                                _条件2 = true;
                                break;
                            }
                        case Keys.R:
                            {
                                _条件保持假腿 = false;
                                切智力腿(_技能数量);
                                _条件3 = true;
                                break;
                            }
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 小小

            case "小小":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 山崩去后摇;
                        _条件根据图片委托2 ??= 投掷去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否a杖)
                    {
                        _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                        if (_是否a杖) _技能数量 = "5";
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            {
                                切智力腿(_技能数量);
                                _条件保持假腿 = false;
                                _条件1 = true;
                                初始化全局时间(ref _全局时间q);
                                break;
                            }
                        case Keys.W:
                            {
                                切智力腿(_技能数量);
                                _条件保持假腿 = false;
                                _条件2 = true;
                                初始化全局时间(ref _全局时间w);
                                break;
                            }
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 小精灵

            case "小精灵":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 循环续勋章;
                        _条件根据图片委托2 ??= 幽魂检测;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否a杖)
                    {
                        _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                        if (_是否a杖) _技能数量 = "6";
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.W:
                            {
                                if (_是否a杖) break;
                                _条件2 = true;
                                break;
                            }
                        case Keys.D2:
                            {
                                if (_循环条件1)
                                {
                                    _条件1 = false;
                                    _循环条件1 = false;
                                    TTS.Speak("关闭续勋章");
                                }
                                else
                                {
                                    _条件1 = true;
                                    _循环条件1 = true;
                                    TTS.Speak("开启续勋章");
                                }

                                break;
                            }
                        case Keys.D3:
                            {
                                if (_选择队友头像 < 9)
                                    _选择队友头像 += 1;
                                else
                                    _选择队友头像 = 0;

                                TTS.Speak(string.Concat("选择第", _选择队友头像 + 1, "个人"));
                                break;
                            }
                    }

                    break;
                }

            #endregion

            #endregion

            #region 敏捷

            #region 露娜

            case "露娜":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 月光后敏捷平a;
                        _条件根据图片委托2 ??= 月蚀后敏捷平a;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间q);
                            切智力腿(_技能数量);
                            _条件1 = true;
                            break;
                        case Keys.R:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间r);
                            切智力腿(_技能数量);
                            _条件2 = true;
                            break;
                        case Keys.C:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            await Run(() =>
                            {
                                Delay(等待延迟);
                                _条件保持假腿 = true;
                            });
                            break;
                        case Keys.X:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            await Run(() =>
                            {
                                Delay(等待延迟);
                                _条件保持假腿 = true;
                            });
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 影魔

            case "影魔":
                //if (e.KeyValue == (uint)Keys.D)
                //{
                //    label1.Text = "Space";

                //    _tasks.Append() = new Task(吹风摇大);
                //    _taskDelay = new Task(() => delay(1000));

                //    _taskDelay.Start();
                //    _taskTrigger.Start();

                //    Task task = new Task(() => { Task.WaitAny(_taskTrigger, _taskDelay); });
                //    task.Start();
                //}
                break;

            #endregion

            #region 巨魔

            case "巨魔":
                {
                    // todo:巨魔逻辑适配 （但这英雄实在太弟弟了）
                    //if (!_总循环条件)
                    //{
                    //    _总循环条件 = true;
                    //    await 无物品状态初始化().ConfigureAwait(false);
                    //    _技能数量 = "5";
                    //}

                    //_条件根据图片委托1 ??= 巨魔远程飞斧接平a后切回;

                    //if (!_是否魔晶)
                    //{
                    //    _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                    //    _技能数量 = "6";
                    //}

                    //switch (e.KeyCode)
                    //{
                    //    case Keys.W:
                    //        {
                    //            if (_全局步骤q == 0)
                    //            {
                    //                var q5 = 获取q5颜色(_全局bts, _全局size);
                    //                var q6 = 获取q6颜色(_全局bts, _全局size);
                    //                var color = SimpleColor.FromRgb(56, 80, 80); // 远程形态 颜色
                    //                if (_是否魔晶)
                    //                {
                    //                    if (!ColorAEqualColorB(color, q6, 0))
                    //                    {
                    //                        KeyPress((uint)Keys.Q);
                    //                    }
                    //                    else
                    //                    {
                    //                        _全局步骤q = 3;
                    //                        _全局时间 = 获取当前时间毫秒();
                    //                    }

                    //                }
                    //                else
                    //                {
                    //                    if (!ColorAEqualColorB(color, q5, 0))
                    //                    {
                    //                        KeyPress((uint)Keys.Q);
                    //                    }
                    //                    else
                    //                    {
                    //                        _全局步骤q = 3;
                    //                        _全局时间 = 获取当前时间毫秒();
                    //                    }
                    //                }

                    //                _条件1 = true;
                    //            }

                    //            break;
                    //        }
                    //    case Keys.E:
                    //        {
                    //            var q5 = 获取q5颜色(_全局bts, _全局size);
                    //            var q6 = 获取q6颜色(_全局bts, _全局size);
                    //            var color = SimpleColor.FromRgb(128, 51, 12); // 近战形态 颜色
                    //            if (_是否魔晶)
                    //            {
                    //                if (!ColorAEqualColorB(color, q6, 0))
                    //                {
                    //                    KeyPress((uint)Keys.Q);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                if (!ColorAEqualColorB(color, q5, 0))
                    //                {
                    //                    KeyPress((uint)Keys.Q);
                    //                }
                    //            }

                    //            break;
                    //        }
                    //    case Keys.R:
                    //        {
                    //            根据图片以及类别使用物品(物品_相位, _全局bts, _全局size);
                    //            根据图片以及类别使用物品(物品_否决, _全局bts, _全局size);
                    //            根据图片以及类别使用物品(物品_散失, _全局bts, _全局size);
                    //            根据图片以及类别使用物品(物品_羊刀, _全局bts, _全局size);
                    //            根据图片以及类别使用物品(物品_紫苑, _全局bts, _全局size);
                    //            根据图片以及类别使用物品(物品_血棘, _全局bts, _全局size);
                    //            根据图片以及类别使用物品(物品_深渊之刃, _全局bts, _全局size);
                    //            根据图片以及类别使用物品(物品_勇气勋章, _全局bts, _全局size);
                    //            根据图片以及类别使用物品(物品_炎阳勋章, _全局bts, _全局size);
                    //            break;
                    //        }
                    //    case Keys.D2 when _全局模式 != 1:
                    //        _全局模式 = 1;
                    //        TTS.Speak("开启切假腿");
                    //        break;
                    //    case Keys.D2:
                    //        _全局模式 = 0;
                    //        TTS.Speak("关闭切假腿");
                    //        break;
                    //}

                    break;
                }

            #endregion

            #region 小骷髅

            case "小骷髅":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否a杖)
                    {
                        _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                        if (_是否a杖) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            切敏捷腿(_技能数量);
                            break;
                        case Keys.E:
                        case Keys.R:
                            切智力腿(_技能数量);
                            break;
                        case Keys.Z:
                            if (RegPicture(物品_魂戒CD, _全局bts, _全局size)) 切力量腿(_技能数量);
                            break;
                    }

                    break;
                }

            #endregion

            #region 小松鼠

            case "小松鼠" when e.KeyCode == Keys.D2:
                label1.Text = "D2";

                await Run(捆接种树);
                break;
            case "小松鼠":
                {
                    if (e.KeyCode == Keys.D3)
                    {
                        label1.Text = "D3";

                        await Run(飞镖接捆接种树);
                    }

                    break;
                }

            #endregion

            #region 拍拍

            case "拍拍":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 超强力量平a;
                        _条件根据图片委托2 ??= 震撼大地接平a;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.W:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间w);
                            切智力腿(_技能数量);
                            _条件1 = true;
                            break;
                        case Keys.Q:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间q);
                            切智力腿(_技能数量);
                            _条件2 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 小鱼人

            case "小鱼人":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 黑暗契约平a;
                        _条件根据图片委托2 ??= 跳水a;
                        _条件根据图片委托3 ??= 深海护罩a;
                        _条件根据图片委托4 ??= 跳水a;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间q);
                            切智力腿(_技能数量);
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间w);
                            切智力腿(_技能数量);
                            _条件2 = true;
                            break;
                        case Keys.R:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间r);
                            切智力腿(_技能数量);
                            _条件4 = true;
                            break;
                        case Keys.D:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间d);
                            切智力腿(_技能数量);
                            _条件3 = true;
                            break;
                        case Keys.Z:
                            if (RegPicture(物品_魂戒CD, _全局bts, _全局size)) 切力量腿(_技能数量);
                            break;
                        case Keys.D2:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);

                            // 径直移动键位
                            KeyDown((uint)Keys.L);
                            // 径直移动
                            RightClick();
                            // 基本上180°310  90°170 75°135 转身定点，配合A杖效果极佳
                            Delay(150);
                            KeyUp((uint)Keys.L);
                            KeyPress((uint)Keys.W);

                            _条件保持假腿 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 敌法

            case "敌法":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托2 ??= 闪烁敏捷;
                        _条件根据图片委托3 ??= 法力虚空取消后摇;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否a杖)
                    {
                        _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                        if (_是否a杖) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.W:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间w);
                            _条件2 = true;
                            break;
                        case Keys.E:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            await Run(() =>
                            {
                                Delay(等待延迟);
                                _条件保持假腿 = true;
                            }).ConfigureAwait(false);
                            break;
                        case Keys.R:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间r);
                            _条件3 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 猴子

            case "猴子":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 灵魂之矛敏捷;
                        _条件根据图片委托2 ??= 神行百变敏捷;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间q);
                            切智力腿(_技能数量);
                            _条件1 = true;
                            break;
                        case Keys.D:
                            _条件保持假腿 = false;
                            初始化全局时间(ref _全局时间w);
                            切智力腿(_技能数量);
                            await Run(() => { KeyPress((uint)Keys.W); });
                            _条件2 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 幻刺

            case "幻刺":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 窒息短匕敏捷;
                        _条件根据图片委托2 ??= 幻影突袭敏捷;
                        _条件根据图片委托3 ??= 魅影无形敏捷;
                        _条件根据图片委托4 ??= 刀阵旋风敏捷;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间w);
                            _条件2 = true;
                            break;
                        case Keys.E:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间e);
                            _条件3 = true;
                            break;
                        case Keys.D:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间d);
                            _条件4 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 虚空

            case "虚空":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 时间漫游敏捷;
                        _条件根据图片委托2 ??= 时间膨胀敏捷;
                        _条件根据图片委托3 ??= 时间结界敏捷;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间w);
                            _条件2 = true;
                            break;
                        case Keys.R:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间r);
                            _条件3 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region TB

            case "TB":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 倒影敏捷;
                        _条件根据图片委托2 ??= 幻惑敏捷;
                        _条件根据图片委托3 ??= 魔化敏捷;
                        _条件根据图片委托4 ??= 恶魔狂热去后摇;
                        _条件根据图片委托5 ??= 恐怖心潮敏捷;
                        _全局模式 = 0;
                        _条件根据图片委托6 ??= 断魂敏捷;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否a杖 || !_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                        _技能数量 = _是否魔晶 switch
                        {
                            true when _是否a杖 => "6",
                            false when !_是否a杖 => "4",
                            _ => "5"
                        };
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间w);
                            _条件2 = true;
                            break;
                        case Keys.E:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间e);
                            _条件3 = true;
                            break;
                        case Keys.D:
                            初始化全局时间(ref _全局时间d);
                            _条件4 = true;
                            break;
                        case Keys.F:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间f);
                            _条件5 = true;
                            break;
                        case Keys.R:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间r);
                            _条件6 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 赏金

            case "赏金":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 飞镖接平a;
                        _条件根据图片委托2 ??= 标记去后摇;
                        _条件根据图片委托3 ??= 循环标记;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件1 = true;
                            break;
                        case Keys.R:
                            _条件2 = true;
                            break;
                        case Keys.D3:
                            if (!_条件3)
                            {
                                _循环条件1 = true;
                                _条件3 = true;
                            }
                            else
                            {
                                _循环条件1 = false;
                                _条件3 = false;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 剧毒

            case "剧毒":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 瘴气去后摇;
                        _条件根据图片委托2 ??= 蛇棒去后摇;
                        _条件根据图片委托3 ??= 剧毒新星去后摇;
                        _条件根据图片委托4 ??= 循环蛇棒;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _中断条件 = false;
                            _条件1 = true;
                            break;
                        case Keys.E:
                            _中断条件 = false;
                            _条件2 = true;
                            break;
                        case Keys.R:
                            _中断条件 = false;
                            _条件3 = true;
                            break;
                        case Keys.D3:
                            {
                                _中断条件 = false;
                                if (!_条件4)
                                {
                                    _循环条件1 = true;
                                    _条件4 = true;
                                }
                                else
                                {
                                    _循环条件1 = false;
                                    _条件4 = false;
                                }

                                break;
                            }
                        case Keys.S:
                            _中断条件 = true;
                            _条件1 = false;
                            _条件2 = false;
                            _条件3 = false;
                            _条件4 = false;
                            break;
                    }

                    break;
                }

            #endregion

            #region 美杜莎

            case "美杜莎":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 秘术异蛇去后摇;
                        _条件根据图片委托2 ??= 石化凝视去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.W:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间w);
                            _条件1 = true;
                            break;
                        case Keys.R:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间r);
                            _条件2 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                        case Keys.D5:
                            switch (_循环条件1)
                            {
                                case true:
                                    _循环条件1 = false;
                                    TTS.Speak("关闭切分裂箭");
                                    break;
                                default:
                                    _循环条件1 = true;
                                    TTS.Speak("开启切分裂箭");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 幽鬼

            case "幽鬼":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 幽鬼之刃去后摇;
                        _条件根据图片委托2 ??= 如影随形去后摇;
                        _条件根据图片委托3 ??= 鬼影重重去后摇;
                        _全局模式 = 0;
                        _技能数量 = "5";
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否a杖)
                    {
                        _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                        if (_是否a杖) _技能数量 = "6";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.F:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间f);
                            _条件2 = true;
                            break;
                        case Keys.D:
                            await Run(() =>
                            {
                                // RightClick();
                                KeyPress((uint)Keys.A);
                                for (var i = 0; i < 4; i++)
                                {
                                    Delay(60);
                                    //RightClick();
                                    KeyPress((uint)Keys.A);
                                }
                            });
                            break;
                        case Keys.R:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间r);
                            _条件3 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 火枪

            case "火枪":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 流霰弹去后摇;
                        _条件根据图片委托2 ??= 瞄准去后摇;
                        _条件根据图片委托3 ??= 震荡手雷去后摇;
                        _条件根据图片委托4 ??= 暗杀去后摇;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.E:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间e);
                            _条件2 = true;
                            break;
                        case Keys.D:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间d);
                            _条件3 = true;
                            break;
                        case Keys.R:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间r);
                            _条件4 = true;
                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 小黑

            case "小黑":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 狂风去后摇;
                        _条件根据图片委托2 ??= 数箭齐发去后摇;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.D:
                            switch (_全局模式)
                            {
                                case 1:
                                    _条件开启切假腿 = true;
                                    _全局模式 = 0;
                                    break;
                                default:
                                    _条件开启切假腿 = false;
                                    切智力腿(_技能数量);
                                    _全局模式 = 1;
                                    break;
                            }

                            break;
                        case Keys.W:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间w);
                            _条件1 = true;
                            break;
                        case Keys.E:
                            _条件保持假腿 = false;
                            切智力腿(_技能数量);
                            初始化全局时间(ref _全局时间e);
                            _条件2 = true;
                            break;
                        case Keys.D2:
                            if (RegPicture(物品_疯狂面具, _全局bts, _全局size))
                            {
                                TTS.Speak("发现疯脸");
                                _条件保持假腿 = false;
                                切智力腿(_技能数量);
                            }

                            if (根据图片以及类别使用物品(物品_疯狂面具, _全局bts, _全局size))
                            {
                                _条件假腿敏捷 = true;
                                _条件保持假腿 = true;
                                _条件开启切假腿 = true;
                                KeyPress((uint)Keys.A);
                            }

                            break;
                        case Keys.D3 when _条件假腿敏捷:
                            _条件假腿敏捷 = false;
                            _条件保持假腿 = true;
                            TTS.Speak("切力量");
                            break;
                        case Keys.D3:
                            _条件假腿敏捷 = true;
                            TTS.Speak("切敏捷");
                            break;
                        case Keys.D4:
                            switch (_条件开启切假腿)
                            {
                                case true:
                                    _条件开启切假腿 = false;
                                    _条件保持假腿 = false;
                                    TTS.Speak("不保持假腿");
                                    break;
                                default:
                                    _条件开启切假腿 = true;
                                    _条件保持假腿 = true;
                                    TTS.Speak("保持假腿");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #endregion

            #region 智力

            #region 黑鸟

            case "黑鸟":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 关接陨星锤;
                        _条件根据图片委托2 ??= 神智之蚀去后摇;
                        _条件根据图片委托3 ??= 关接跳;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.D:
                            _中断条件 = false;
                            KeyPress((uint)Keys.W);
                            _条件1 = true;
                            break;
                        case Keys.H:
                            _中断条件 = true;
                            break;
                        case Keys.E:
                            _条件3 = true;
                            break;
                        case Keys.R:
                            _中断条件 = true;
                            根据图片以及类别使用物品(物品_纷争, _全局bts, _全局size, _技能数量);
                            _条件2 = true;
                            break;
                    }

                    break;
                }

            #endregion

            #region 谜团

            case "谜团" when e.KeyCode == Keys.D:
                label1.Text = "D";
                await Run(跳秒接午夜凋零黑洞);
                break;
            case "谜团":
                {
                    if (e.KeyCode == Keys.F)
                    {
                        label1.Text = "F";
                        await Run(刷新接凋零黑洞);
                    }

                    break;
                }

            #endregion

            #region 冰女

            case "冰女":
                {
                    break;
                }

            #endregion

            #region 火女

            case "火女":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 龙破斩去后摇;
                        _条件根据图片委托2 ??= 光击阵去后摇;
                        _条件根据图片委托3 ??= 神灭斩去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _中断条件 = false;
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _中断条件 = false;
                            _条件2 = true;
                            break;
                        case Keys.R:
                            _中断条件 = false;
                            _条件3 = true;
                            break;
                        case Keys.S:
                            _中断条件 = true;
                            _条件1 = false;
                            _条件2 = false;
                            _条件3 = false;
                            break;
                    }

                    break;
                }

            #endregion

            #region 蓝猫

            case "蓝猫":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 拉接平A;
                        _条件根据图片委托2 ??= 滚接平A;
                        _条件根据图片委托3 ??= 快速回城;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            await Run(残影接平A);
                            break;
                        case Keys.W:
                            _条件1 = true;
                            break;
                        case Keys.R:
                            _条件2 = true;
                            break;
                        case Keys.D4:
                            _条件3 = true;
                            break;
                        //else if (e.KeyCode == Keys.F)
                        //{
                        //    label1.Text = "F";
                        //    Task.await Run(原地滚A);
                        //}
                        case Keys.F when !_丢装备条件:
                            await Run(批量扔装备);
                            _丢装备条件 = !_丢装备条件;
                            break;
                        case Keys.F:
                            await Run(捡装备);
                            _丢装备条件 = !_丢装备条件;
                            break;
                    }

                    break;
                }

            #endregion

            #region 宙斯

            case "宙斯":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 弧形闪电去后摇;
                        _条件根据图片委托2 ??= 雷击去后摇;
                        _条件根据图片委托3 ??= 弧形闪电不能释放;
                        _条件根据图片委托4 ??= 神圣一跳去后摇;
                        _全局模式 = 0;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否魔晶) _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                    if (!_是否a杖) _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);


                    switch (e.KeyCode)
                    {
                        // 弧形闪电和雷击都是不朽
                        case Keys.Q when await 弧形闪电不能释放(_全局bts, _全局size):
                            _全局模式q = 1;
                            _条件3 = true;
                            break;
                        case Keys.Q:
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _条件2 = true;
                            break;
                        case Keys.E:
                            _条件4 = true;
                            break;
                        case Keys.D2:
                            switch (_全局模式)
                            {
                                case < 1:
                                    _全局模式 = 1;
                                    TTS.Speak("去后摇移动");
                                    break;
                                case 1:
                                    _全局模式 = 0;
                                    TTS.Speak("去后摇接平A");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 卡尔

            case "卡尔" when e.KeyCode == Keys.D2:
                label1.Text = "D2";

                await Run(三冰对线);
                break;
            case "卡尔" when e.KeyCode == Keys.D3:
                label1.Text = "D2";

                await Run(三火平A);
                break;
            case "卡尔" when e.KeyCode == Keys.D1:
                label1.Text = "D2";

                await Run(三雷幽灵);
                break;
            case "卡尔":
                {
                    if (e.KeyCode == Keys.D4)
                    {
                        label1.Text = "D2";

                        await Run(吹风天火);
                    }

                    break;
                }

            #endregion

            #region 拉西克

            case "拉席克" when e.KeyCode == Keys.F:
                label1.Text = "F";

                _中断条件 = false;

                await Run(吹风接撕裂大地);
                break;
            case "拉席克":
                {
                    if (e.KeyCode == Keys.S) _中断条件 = true;

                    break;
                }

            #endregion

            #region 暗影萨满

            case "暗影萨满":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 苍穹振击取消后摇;
                        _条件根据图片委托2 ??= 变羊取消后摇;
                        _条件根据图片委托3 ??= 释放群蛇守卫取消后摇;
                        _条件根据图片委托4 ??= 推推破林肯秒羊;
                        _条件根据图片委托5 ??= 枷锁持续施法隐身;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _全局步骤q = 0;
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.W:
                            var i = Convert.ToDouble(tb_状态抗性.Text.Trim());
                            _状态抗性倍数 = (100 - (i > 100 ? 0 : i)) / 100;
                            if (RegPicture(物品_祭礼长袍, _全局bts, _全局size)) _状态抗性倍数 *= 1.1;
                            if (RegPicture(物品_永恒遗物, _全局bts, _全局size)) _状态抗性倍数 *= 1.2;
                            if (await 智力跳刀buff(_全局bts, _全局size)) _状态抗性倍数 *= 1.2;
                            _条件2 = true;
                            break;
                        case Keys.E:
                            初始化全局时间(ref _全局时间e);
                            if (!RegPicture(物品_暗影护符buff, _全局bts, _全局size)) 根据图片以及类别自我使用物品(物品_暗影护符, _全局bts, _全局size, _技能数量);
                            _条件5 = true;
                            break;
                        case Keys.R:
                            初始化全局时间(ref _全局时间r);
                            _条件3 = true;
                            break;
                        case Keys.D1:
                            switch (_全局模式w)
                            {
                                case 0:
                                    _全局模式w = 1;
                                    TTS.Speak("羊拉");
                                    break;
                                case 1:
                                    _全局模式w = 2;
                                    TTS.Speak("羊电");
                                    break;
                                case 2:
                                    _全局模式w = 3;
                                    TTS.Speak("羊电拉");
                                    break;
                                case 3:
                                    _全局模式w = 4;
                                    TTS.Speak("羊电大拉");
                                    break;
                                case 4:
                                    _全局模式w = 0;
                                    TTS.Speak("羊接平A");
                                    break;
                            }

                            break;
                        case Keys.D2:
                            _条件4 = true;
                            break;
                        case Keys.D3:
                            await Run(async () =>
                            {
                                await Run(() => { 渐隐期间放技能((uint)Keys.E, 800); });
                                if (_全局模式 != 1) return;
                                Delay(650);
                                var p = MousePosition;
                                MouseMove(_指定地点p);
                                KeyPress((uint)Keys.Space);
                                Delay(等待延迟);
                                MouseMove(p);
                                _全局模式 = 0;
                            });
                            break;
                        case Keys.D4:
                            await Run(() =>
                            {
                                _指定地点p = MousePosition;
                                _全局模式 = 1;
                            });
                            break;
                        case Keys.D5:
                            _条件6 = true;
                            break;
                    }

                    break;
                }

            #endregion

            #region 小仙女

            case "小仙女":
                {
                    switch (e.KeyCode)
                    {
                        case Keys.D2:
                            label1.Text = "D2";

                            _循环条件2 = true;

                            await Run(诅咒皇冠吹风);
                            break;
                        case Keys.D9:
                            label1.Text = "D3";

                            _循环条件2 = true;

                            await Run(作祟暗影之境最大化伤害);
                            break;
                        case Keys.S:
                            _循环条件2 = false;
                            break;
                        case Keys.E:
                            await Run(皇冠延时计时);
                            break;
                    }

                    break;
                }

            #endregion

            #region 天怒

            case "天怒":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 循环奥数鹰隼;
                        _条件根据图片委托2 ??= 天怒秒人连招;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.D3:
                            _全局步骤 = 0;
                            _中断条件 = false;
                            _条件2 = true;
                            break;
                        case Keys.D2:
                            switch (_循环条件1)
                            {
                                case true:
                                    _中断条件 = true;
                                    _条件1 = false;
                                    _循环条件1 = false;
                                    break;
                                default:
                                    _中断条件 = false;
                                    _条件1 = true;
                                    _循环条件1 = true;
                                    break;
                            }

                            break;
                        case Keys.S:
                            {
                                for (var i = 0; i < 2; i++)
                                {
                                    _条件根据图片委托2 = null;
                                    _中断条件 = true;
                                    _条件3 = false;
                                    _条件2 = false;
                                    Delay(60); // 等待程序内延迟结束
                                }

                                break;
                            }
                    }

                    break;
                }

            #endregion

            #region 炸弹人

            case "炸弹人":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 粘性炸弹去后摇;
                        _条件根据图片委托2 ??= 活性电击去后摇;
                        _条件根据图片委托3 ??= 爆破起飞去后摇;
                        _条件根据图片委托4 ??= 爆破后接3雷粘性炸弹;
                        _技能数量 = "5";
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _条件2 = true;
                            break;
                        case Keys.E:
                            根据图片以及类别使用物品(物品_纷争, _全局bts, _全局size, _技能数量);
                            _条件3 = true;
                            break;
                        case Keys.D2:
                            {
                                switch (_全局模式e)
                                {
                                    case 0:
                                        _全局模式e = 1;
                                        TTS.Speak("起飞后接3连炸弹");
                                        break;
                                    case 1:
                                        _全局模式e = 0;
                                        TTS.Speak("起飞后不接3连炸弹");
                                        break;
                                }

                                break;
                            }
                        case Keys.D when !_丢装备条件:
                            await Run(批量扔装备);
                            _丢装备条件 = !_丢装备条件;
                            break;
                        case Keys.D:
                            await Run(捡装备);
                            _丢装备条件 = !_丢装备条件;
                            break;
                    }

                    break;
                }

            #endregion

            #region 神域

            case "神域":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 命运敕令去后摇;
                        _条件根据图片委托2 ??= 涤罪之焰去后摇;
                        _条件根据图片委托3 ??= 虚妄之诺去后摇;
                        _条件根据图片委托4 ??= 涤罪之焰不可释放;
                        _条件根据图片委托5 ??= 天命之雨去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    if (!_是否a杖)
                    {
                        _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                        if (_是否a杖) _技能数量 = "5";
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.W:
                            _条件1 = true;
                            break;
                        case Keys.E when await 涤罪之焰不可释放(_全局bts, _全局size):
                            _全局模式e = 1;
                            _条件4 = true;
                            break;
                        case Keys.E:
                            _条件2 = true;
                            break;
                        case Keys.D:
                            _条件5 = true;
                            break;
                        case Keys.R:
                            _条件3 = true;
                            break;
                    }

                    break;
                }

            #endregion

            #region 修补匠

            case "修补匠" when e.KeyCode == Keys.R:
                KeyPress((uint)Keys.C);
                KeyPress((uint)Keys.V);
                await Run(刷新完跳);
                break;

            case "修补匠" when e.KeyCode == Keys.D1:
                {
                    _条件1 = !_条件1;
                    TTS.Speak(_条件1 ? "开启刷导弹" : "关闭刷导弹");
                    break;
                }
            case "修补匠" when e.KeyCode == Keys.D2:
                {
                    _条件2 = !_条件2;
                    TTS.Speak(_条件2 ? "开启刷跳" : "关闭刷跳");
                    break;
                }
            case "修补匠" when e.KeyCode == Keys.D3:
                {
                    _条件3 = !_条件3;
                    TTS.Speak(_条件3 ? "开启希瓦" : "关闭希瓦");
                    break;
                }
            case "修补匠" when e.KeyCode == Keys.X:
                await Run(推推接刷新);
                break;
            case "修补匠":
                {
                    if (e.KeyCode == Keys.D1) await Run(检测敌方英雄自动导弹);

                    break;
                }

            #endregion

            #region 莱恩

            case "莱恩":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 莱恩羊接技能;
                        _条件根据图片委托2 ??= 死亡一指去后摇;
                        _条件根据图片委托3 ??= 推推破林肯秒羊;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.W:
                            _条件1 = true;
                            break;
                        case Keys.R:
                            await 大招前纷争(_全局bts, _全局size);
                            _条件2 = true;
                            break;
                        case Keys.D2:
                            _条件3 = true;
                            break;
                        case Keys.D3 when !_条件4:
                            _条件4 = true;
                            TTS.Speak("开启羊接吸");
                            break;
                        case Keys.D3:
                            _条件4 = false;
                            TTS.Speak("开启羊接A");
                            break;
                    }

                    break;
                }

            #endregion

            #region 沉默

            case "沉默":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 奥数诅咒去后摇;
                        _条件根据图片委托2 ??= 遗言去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.E:
                            初始化全局时间(ref _全局时间q);
                            _条件2 = true;
                            break;
                        case Keys.D2:
                            switch (_全局模式q)
                            {
                                case < 1:
                                    _全局模式q = 1;
                                    TTS.Speak("奥数诅咒最大化接遗言");
                                    break;
                                case 1:
                                    _全局模式q = 2;
                                    TTS.Speak("奥数诅咒接平A");
                                    break;
                                case 2:
                                    _全局模式q = 0;
                                    TTS.Speak("奥数诅咒快速接遗言");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 戴泽

            case "戴泽":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 剧毒之触去后摇;
                        _条件根据图片委托2 ??= 薄葬去后摇;
                        _条件根据图片委托3 ??= 暗影波去后摇;
                        _条件根据图片委托4 ??= 善咒去后摇;
                        _条件根据图片委托5 ??= 邪能去后摇;
                        _技能数量 = "5";
                        _基础攻击前摇 = 0.3;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.W:
                            初始化全局时间(ref _全局时间w);
                            _条件2 = true;
                            break;
                        case Keys.E:
                            初始化全局时间(ref _全局时间e);
                            _条件3 = true;
                            break;
                        case Keys.D:
                            初始化全局时间(ref _全局时间d);
                            _条件4 = true;
                            break;
                        case Keys.R:
                            初始化全局时间(ref _全局时间r);
                            _条件5 = true;
                            break;
                        case Keys.D1:
                            tb_攻速.Text = 获取图片文字(537, 510, 27, 16).Trim();
                            _攻击速度 = Convert.ToDouble(tb_攻速.Text);
                            break;
                        case Keys.D2:
                            await Run(续走A);
                            break;
                    }

                    break;
                }

            #endregion

            #region 双头龙

            case "双头龙":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 冰火交加去后摇;
                        _条件根据图片委托2 ??= 冰封路径去后摇;
                        _条件根据图片委托3 ??= 烈焰焚身去后摇;
                        _条件根据图片委托4 ??= 吹风接冰封路径;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.W:
                            初始化全局时间(ref _全局时间w);
                            _条件2 = true;
                            break;
                        case Keys.R:
                            初始化全局时间(ref _全局时间r);
                            _条件3 = true;
                            break;
                        case Keys.D2:
                            var d5 = 获取d5颜色(_全局bts, _全局size);
                            var e5 = 获取e5颜色(_全局bts, _全局size);
                            var e4 = 获取e4颜色(_全局bts, _全局size);
                            switch (_是否魔晶)
                            {
                                // RightClick();
                                case true:
                                    if (ColorAEqualColorB(d5, SimpleColor.FromRgb(9, 38, 81), 0))
                                        KeyPress((uint)Keys.D);
                                    else if (ColorAEqualColorB(e5, SimpleColor.FromRgb(79, 36, 7), 0))
                                        KeyPress((uint)Keys.E);
                                    else
                                        KeyPress((uint)Keys.A);
                                    break;
                                default:
                                    if (ColorAEqualColorB(e4, SimpleColor.FromRgb(70, 32, 8), 0))
                                        KeyPress((uint)Keys.E);
                                    else
                                        KeyPress((uint)Keys.A);
                                    break;
                            }

                            break;
                        case Keys.D3:
                            _条件4 = true;
                            break;
                    }

                    break;
                }

            #endregion

            #region 巫医

            case "巫医":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 麻痹药剂去后摇;
                        _条件根据图片委托2 ??= 巫蛊咒术去后摇;
                        _条件根据图片委托3 ??= 死亡守卫隐身;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }

                    if (!_是否魔晶)
                    {
                        _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                        if (_是否魔晶) _技能数量 = "5";
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            初始化全局时间(ref _全局时间q);
                            _条件1 = true;
                            break;
                        case Keys.E:
                            初始化全局时间(ref _全局时间e);
                            _条件2 = true;
                            break;
                        case Keys.D:
                            if (_是否魔晶) 根据图片以及类别自我使用物品(物品_暗影护符, _全局bts, _全局size, _技能数量);
                            break;
                        case Keys.R:
                            switch (_全局模式r)
                            {
                                case 1:
                                    根据图片以及类别使用物品(物品_黑黄杖, _全局bts, _全局size, _技能数量);
                                    break;
                            }

                            _条件3 = true;
                            break;
                        case Keys.D2:
                            await Run(() => { 渐隐期间放技能((uint)Keys.R, 800); });
                            break;
                        case Keys.D3:
                            switch (_全局模式q)
                            {
                                case 1:
                                    _全局模式q = 0;
                                    TTS.Speak("药剂平A");
                                    break;
                                case 0:
                                    _全局模式q = 1;
                                    TTS.Speak("药剂巫术死亡守卫");
                                    break;
                            }

                            break;
                        case Keys.D4:
                            switch (_全局模式r)
                            {
                                case 1:
                                    _全局模式r = 0;
                                    TTS.Speak("不开BKB");
                                    break;
                                case 0:
                                    _全局模式r = 1;
                                    TTS.Speak("开BKB");
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #region 女王

            case "女王":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 暗影突袭去后摇;
                        _条件根据图片委托2 ??= 闪烁去后摇;
                        _条件根据图片委托3 ??= 痛苦尖叫去后摇;
                        _条件根据图片委托4 ??= 冲击波去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _中断条件 = false;
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _中断条件 = false;
                            _条件2 = true;
                            break;
                        case Keys.E:
                            _中断条件 = false;
                            _条件3 = true;
                            break;
                        case Keys.R:
                            _中断条件 = false;
                            _条件4 = true;
                            break;
                        case Keys.S:
                            _中断条件 = true;
                            break;
                    }

                    break;
                }

            #endregion

            #region 干扰者

            case "干扰者":
                {
                    if (!_总循环条件)
                    {
                        _总循环条件 = true;
                        _条件根据图片委托1 ??= 风雷之击去后摇;
                        _条件根据图片委托2 ??= 恶念瞥视去后摇;
                        _条件根据图片委托3 ??= 动能力场去后摇;
                        _条件根据图片委托4 ??= 静态风暴去后摇;
                        await 无物品状态初始化().ConfigureAwait(false);
                    }


                    switch (e.KeyCode)
                    {
                        case Keys.Q:
                            _中断条件 = false;
                            _条件1 = true;
                            break;
                        case Keys.W:
                            _中断条件 = false;
                            _条件2 = true;
                            break;
                        case Keys.E:
                            _中断条件 = false;
                            _条件3 = true;
                            break;
                        case Keys.R:
                            _中断条件 = false;
                            _条件4 = true;
                            break;
                        case Keys.S:
                            _中断条件 = true;
                            break;
                        case Keys.D2:
                            switch (_全局模式q)
                            {
                                case 0:
                                    TTS.Speak("电接大接框");
                                    _全局模式q = 1;
                                    break;
                                case 1:
                                    TTS.Speak("电接A");
                                    _全局模式q = 0;
                                    break;
                            }

                            break;
                    }

                    break;
                }

            #endregion

            #endregion

            #region 其他

            case "切假腿":
                {
                    if (e.KeyCode is Keys.Q or Keys.W or Keys.E or Keys.D or Keys.F or Keys.R)
                        切智力腿();
                    break;
                }
            case "测试":
                {
                    switch (e.KeyCode)
                    {
                        case Keys.D2:
                            await Run(捕捉颜色);
                            break;
                        case Keys.D3:
                            await Run(测试方法_寻找大勋章);
                            break;
                        case Keys.D1:
                            await Run(() =>
                            {
                                KeyPress((uint)Keys.Space);
                                快速选择敌方英雄(type: 1, type1: 1);
                                KeyPress((uint)Keys.W);
                                KeyPress((uint)Keys.Q);
                            });
                            break;
                    }

                    break;
                }

                #endregion
        }
    }

    #endregion

    #region 延时

    /// <summary>
    ///     精准延迟
    /// </summary>
    /// <param name="delay">需要延迟的时间</param>
    /// <param name="time"></param>
    private static void Delay(int delay, long time = -1)
    {
        time = time switch
        {
            -1 => 获取当前时间毫秒(),
            _ => time
        };
        while (获取当前时间毫秒() - time <= delay)
        {
        }
    }

    #endregion

    #region 更改名字取消功能

    private void Tb_name_TextChanged(object sender, EventArgs e)
    {
        取消所有功能();
    }

    #endregion

    #region 按钮捕捉颜色

    private async void button1_Click(object sender, EventArgs e)
    {
        await Run(捕捉颜色);
    }

    #endregion

    #region 测试显示颜色

    /// <summary>
    ///     显示颜色
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tb_delay_TextChanged(object sender, EventArgs e)
    {
        if (tb_name.Text.Trim() != "测试") return;

        try
        {
            using var list = new PooledList<string>(tb_delay.Text.Split(','));
            pictureBox1.BackColor = Color.FromArgb(255, int.Parse(list[0]), int.Parse(list[1]), int.Parse(list[2]));
        }
        catch
        {
            // ignored
        }
    }

    #endregion

    #region 局部全局变量

    #region 循环用到

    /// <summary>
    ///     循环条件
    /// </summary>
    private static bool _总循环条件;

    /// <summary>
    ///     循环开关条件
    /// </summary>
    private static bool _总开关条件;

    /// <summary>
    ///     循环计数1
    /// </summary>
    private static bool _循环条件1;

    /// <summary>
    ///     循环计数2
    /// </summary>
    private static bool _循环条件2;

    /// <summary>
    ///     全局图像
    /// </summary>
    private static Bitmap _全局图像;

    /// <summary>
    ///     全局bytes
    /// </summary>
    private static byte[] _全局bts = new byte[3 * 截图模式1W * 截图模式1H];

    /// <summary>
    ///     全局假腿bytes
    /// </summary>
    private static byte[] _全局假腿bts = new byte[3 * 截图模式1W * 截图模式1H];

    /// <summary>
    ///     全局size
    /// </summary>
    private static Size _全局size;

    /// <summary>
    ///     获取图片委托
    /// </summary>
    /// <returns></returns>
    private delegate void GetBitmap();

    /// <summary>
    ///     获取图片委托
    /// </summary>
    private static GetBitmap _循环内获取图片;

    /// <summary>
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private delegate Task<bool> ConditionDelegateBitmap(byte[] bytes, Size size);

    /// <summary>
    ///     条件1委托
    /// </summary>
    private static ConditionDelegateBitmap _条件根据图片委托1;

    /// <summary>
    ///     条件2委托
    /// </summary>
    private static ConditionDelegateBitmap _条件根据图片委托2;

    /// <summary>
    ///     条件3委托
    /// </summary>
    private static ConditionDelegateBitmap _条件根据图片委托3;

    /// <summary>
    ///     条件4委托
    /// </summary>
    private static ConditionDelegateBitmap _条件根据图片委托4;

    /// <summary>
    ///     条件5委托
    /// </summary>
    private static ConditionDelegateBitmap _条件根据图片委托5;

    /// <summary>
    ///     条件6委托
    /// </summary>
    private static ConditionDelegateBitmap _条件根据图片委托6;

    /// <summary>
    ///     条件7委托
    /// </summary>
    private static ConditionDelegateBitmap _条件根据图片委托7;

    /// <summary>
    ///     条件8委托
    /// </summary>
    private static ConditionDelegateBitmap _条件根据图片委托8;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件1;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件2;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件3;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件4;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件5;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件6;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件7;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件8;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件保持假腿;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件开启切假腿;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件假腿敏捷;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _切假腿中;

    /// <summary>
    ///     条件走A
    /// </summary>
    private static bool _条件走a;

    /// <summary>
    ///     技能数量
    /// </summary>
    private static string _技能数量 = "4";

    /// <summary>
    ///     中断条件布尔
    /// </summary>
    private static bool _中断条件;

    /// <summary>
    ///     攻击前摇
    /// </summary>
    private static double _基础攻击前摇 = 0.3;

    /// <summary>
    ///     攻击速度
    /// </summary>
    private static double _攻击速度 = 100;

    /// <summary>
    ///     状态抗性
    /// </summary>
    private static double _状态抗性倍数;

    /// <summary>
    ///     用于判断是否延迟
    /// </summary>
    private static bool _循环最终是否延迟;

    /// <summary>
    ///     用于选择队友头像
    /// </summary>
    private static int _选择队友头像;

    #endregion

    #region 其他

    /// <summary>
    ///     用于捕获按键
    /// </summary>
    private readonly KeyboardHook _kHook = new();

    /// <summary>
    ///     按键钩子，用于捕获按下的键
    /// </summary>
    private KeyEventHandler _myKeyEventHandeler; //按键钩子

    /// <summary>
    ///     用于捕捉案件
    /// </summary>
    private IKeyboardMouseEvents _mGlobalHook = Hook.GlobalEvents();

    /// <summary>
    /// </summary>
    private readonly HookUserActivity _hookUser = new();

    /// <summary>
    ///     全局OCR
    /// </summary>
    private static PaddleOCREngine _PaddleOcrEngine;

    /// <summary>
    ///     丢装备条件布尔
    /// </summary>
    private static bool _丢装备条件;

    /// <summary>
    ///     全局是否魔晶
    /// </summary>
    private static bool _是否魔晶;

    /// <summary>
    ///     全局是否A杖
    /// </summary>
    private static bool _是否a杖;


    ///// <summary>
    /////     模拟按键
    ///// </summary>
    //private readonly IPressKey mPressKey;


    ///// <summary>
    /////     用于生成随机数
    ///// </summary>
    //private RandomGenerator randomGenerator = new(); 

    #endregion

    #region 模式

    /// <summary>
    ///     全局时间
    /// </summary>
    private static long _全局时间 = -1;

    /// <summary>
    ///     全局时间
    /// </summary>
    private static long _全局时间q;

    /// <summary>
    ///     全局时间
    /// </summary>
    private static long _全局时间w;

    /// <summary>
    ///     全局时间
    /// </summary>
    private static long _全局时间e;

    /// <summary>
    ///     全局时间
    /// </summary>
    private static long _全局时间r;

    /// <summary>
    ///     全局时间
    /// </summary>
    private static long _全局时间d;

    /// <summary>
    ///     全局时间
    /// </summary>
    private static long _全局时间f;

    /// <summary>
    ///     用于跳拱地点
    /// </summary>
    private static Point _指定地点p = new(0, 0);

    /// <summary>
    ///     用于跳拱地点
    /// </summary>
    private static Point _指定地点q;

    /// <summary>
    ///     用于跳拱地点
    /// </summary>
    private static Point _指定地点w;

    /// <summary>
    ///     用于跳拱地点
    /// </summary>
    private static Point _指定地点e;

    /// <summary>
    ///     用于跳拱地点
    /// </summary>
    private static Point _指定地点r;

    /// <summary>
    ///     用于跳拱地点
    /// </summary>
    private static Point _指定地点d;

    /// <summary>
    ///     用于跳拱地点
    /// </summary>
    private static Point _指定地点f;

    /// <summary>
    ///     用于不同设定
    /// </summary>
    private static int _全局模式;

    /// <summary>
    ///     用于不同设定
    /// </summary>
    private static int _全局模式q;

    /// <summary>
    ///     用于不同设定
    /// </summary>
    private static int _全局模式w;

    /// <summary>
    ///     用于不同设定
    /// </summary>
    private static int _全局模式e;

    /// <summary>
    ///     用于不同设定
    /// </summary>
    private static int _全局模式r;

    /// <summary>
    ///     用于不同设定
    /// </summary>
    private static int _全局模式d;

    /// <summary>
    ///     用于不同设定
    /// </summary>
    private static int _全局模式f;

    /// <summary>
    ///     用于阶段性
    /// </summary>
    private static int _全局步骤;

    /// <summary>
    ///     用于阶段性
    /// </summary>
    private static int _全局步骤q;

    /// <summary>
    ///     用于阶段性
    /// </summary>
    private static int _全局步骤w;

    /// <summary>
    ///     用于阶段性
    /// </summary>
    private static int _全局步骤e;

    /// <summary>
    ///     用于阶段性
    /// </summary>
    private static int _全局步骤r;

    /// <summary>
    ///     用于阶段性
    /// </summary>
    private static int _全局步骤d;

    /// <summary>
    ///     用于阶段性
    /// </summary>
    private static int _全局步骤f;

    #endregion

    #endregion

    #region Dota2具体实现

    #region 力量

    #region 猛犸

    private static async Task<bool> 切回假腿(byte[] bts, Size size)
    {
        _条件保持假腿 = true;
        return await FromResult(false);
    }

    private static void 跳拱指定地点()
    {
        KeyPress((uint)Keys.Space);
        Delay(等待延迟);
        KeyPress((uint)Keys.D9);
        MouseMove(_指定地点p);
        Delay(等待延迟);
        KeyPress((uint)Keys.E);
        Delay(等待延迟);
        KeyPress((uint)Keys.D9);
    }

    #endregion

    #region 船长

    private static void 洪流接x回()
    {
    }

    private static void 洪流接船()
    {
    }

    private void 最大化x伤害控制()
    {
    }

    #endregion

    #region 斧王

    private static async Task<bool> 吼去后摇(byte[] bts, Size size)
    {
        static async Task 吼后Async(byte[] bts, Size size)
        {
            await Run(() =>
            {
                if (_全局模式q == 1) 根据图片以及类别使用物品(物品_刃甲, bts, size, _技能数量);

                switch (_是否a杖)
                {
                    case true:
                        KeyPress((uint)Keys.A);
                        break;
                    default:
                        KeyPress((uint)Keys.W);
                        break;
                }
            });
        }

        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        await 吼后Async(bts, size);
        return await FromResult(false);
    }

    private static async Task<bool> 战斗饥渴去后摇(byte[] bts, Size size)
    {
        static async Task 战斗饥渴后Async()
        {
            await Run(RightClick);
        }

        var w4 = 获取w4左下角颜色(bts, size);

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        await 战斗饥渴后Async();
        return await FromResult(false);
    }

    private static async Task<bool> 淘汰之刃去后摇(byte[] bts, Size size)
    {
        static async Task 淘汰之刃后()
        {
            await Run(RightClick);
        }

        var r4 = 获取r4左下角颜色(bts, size);

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        await 淘汰之刃后();
        return await FromResult(false);
    }

    private static async Task<bool> 跳吼(byte[] bts, Size size)
    {
        if (根据图片以及类别使用物品(物品_跳刀, bts, size, _技能数量)
            || 根据图片以及类别使用物品(物品_跳刀_力量跳刀, bts, size, _技能数量)
            || 根据图片以及类别使用物品(物品_跳刀_智力跳刀, bts, size, _技能数量))
        {
            Delay(等待延迟);
            return await FromResult(true);
        }

        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) KeyPress((uint)Keys.Q);

        return await FromResult(false);
    }

    #endregion

    #region 军团

    private static async Task<bool> 决斗(byte[] bts, Size size)
    {
        switch (_全局步骤)
        {
            case < 1:
                {
                    _全局步骤 = 根据图片以及类别使用物品(物品_臂章, bts, size) ? 1 : 0;

                    根据图片以及类别使用物品(物品_魂戒CD, bts, size);

                    //if (RegPicture(军团_强攻CD, bts, size))
                    //{
                    //    KeyPressAlt((uint)Keys.W);
                    //    Delay(260); // 去后摇
                    //    RightClick();
                    //    Delay(等待延迟);
                    //}

                    break;
                }
            case < 2 when 根据图片以及类别使用物品(物品_刃甲, bts, size):
                return await FromResult(true);
            case < 2:
                {
                    if (根据图片以及类别使用物品(物品_跳刀, bts, size)) _全局步骤 = 2;

                    else if (根据图片以及类别使用物品(物品_跳刀_力量跳刀, bts, size)) _全局步骤 = 2;

                    else if (根据图片以及类别使用物品(物品_跳刀_力量跳刀, bts, size)) _全局步骤 = 2;

                    else if (根据图片以及类别使用物品(物品_跳刀_智力跳刀, bts, size)) _全局步骤 = 2;

                    return await FromResult(true);
                }
            case < 3:
                {
                    if (_全局模式 == 1) 快速选择敌方英雄();

                    _全局步骤 = 3;
                    break;
                }
        }

        if (_全局步骤 < 4)
        {
            if (_全局时间 < 0)
                _全局时间 = 获取当前时间毫秒();

            if (获取当前时间毫秒() - _全局时间 < 100)
            {
                根据图片以及类别使用物品(物品_勇气勋章, bts, size);
            }
            else
            {
                _全局步骤 = 4;
                _全局时间 = -1;
            }

            return await FromResult(true);
        }

        if (_全局步骤 < 5)
        {
            if (_全局时间 < 0)
                _全局时间 = 获取当前时间毫秒();

            //if (RegPicture(军团_决斗CD, bts, size))
            //{
            //    KeyPress((uint)Keys.R);
            //    Delay(等待延迟);
            //    return 获取当前时间毫秒() - _全局时间 <= 450;
            //}

            _全局步骤 = 5;
            return await FromResult(false);
        }

        return await FromResult(false);
    }

    #endregion

    #region 孽主

    private static void 深渊火雨阿托斯()
    {
    }

    #endregion

    #region 哈斯卡

    private void 心炎平a()
    {
    }

    private void 牺牲平a刃甲()
    {
    }

    #endregion

    #region 海民

    private static async Task<bool> 摔角行家去后摇(byte[] bts, Size size)
    {
        static async Task 摔角行家后()
        {
            await Run(() => { KeyPress((uint)Keys.A); });
        }

        var e4 = 获取e4左下角颜色(bts, size);
        var e5 = 获取e5左下角颜色(bts, size);

        if (_是否a杖)
        {
            if (ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
        }
        else
        {
            if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        }

        摔角行家后().Start();
        return await FromResult(false);
    }

    private static async Task<bool> 飞踢接雪球(byte[] bts, Size size)
    {
        static async Task 飞踢后()
        {
            await Run(() => { KeyPress((uint)Keys.W); });
        }

        var d5 = 获取d5颜色(bts, size);

        if (!ColorAEqualColorB(d5, SimpleColor.FromRgb(72, 73, 73), 0, 1, 1)) return await FromResult(true);

        飞踢后().Start();
        return await FromResult(false);
    }

    private static async Task<bool> 跳接勋章接摔角行家(byte[] bts, Size size)
    {
#if DEBUG
        var p = 正面跳刀_无转身(bts, size);

        var point = MousePosition;
        MouseMove(p.X, p.Y);
#endif

#if !DEBUG
        if (根据图片以及类别使用物品(物品_臂章, bts, size, _技能数量)) Delay(300);

        根据图片以及类别使用物品(物品_勇气勋章, _全局bts, _全局size, _技能数量);
        根据图片以及类别使用物品(物品_炎阳勋章, _全局bts, _全局size, _技能数量);

        var p = 正面跳刀_无转身(bts, size);

        var point = MousePosition;

        MouseMove(p.X, p.Y);

        // 跳刀空格
        KeyPress((uint)Keys.Space);

        Delay(等待延迟);

        MouseMove(point.X, point.Y);

        KeyPress((uint)Keys.E);
#endif
        return await FromResult(false);
    }

    #endregion

    #region 钢背

    private static async Task<bool> 鼻涕针刺循环(byte[] bts, Size size)
    {
        var q4 = 获取q4左下角颜色(bts, size);
        var w4 = 获取w4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);


        static void 针刺(in byte[] bts, Size size)
        {
            if (_条件开启切假腿 && !_切假腿中 && 获取当前时间毫秒() - _全局时间w > 200)
            {
                _条件开启切假腿 = false;
                切智力腿(bts, size, _技能数量);
                KeyPress((uint)Keys.W);
                Run(() =>
                {
                    Delay(_条件假腿敏捷 ? 250 : 60);
                    _条件开启切假腿 = true;
                });
                _全局时间w = 获取当前时间毫秒();
            }
            else
                KeyPress((uint)Keys.W);

            _循环最终是否延迟 = true;
        }

        static void 鼻涕(in byte[] bts, Size size)
        {
            if (_条件开启切假腿 && !_切假腿中 && 获取当前时间毫秒() - _全局时间q > 200)
            {
                _条件开启切假腿 = false;
                切智力腿(bts, size, _技能数量);
                KeyPress((uint)Keys.Q);
                Run(() =>
                {
                    Delay(_条件假腿敏捷 ? 250 : 60);
                    _条件开启切假腿 = true;
                });
                _全局时间q = 获取当前时间毫秒();
            }
            else
                KeyPress((uint)Keys.Q);

            _循环最终是否延迟 = true;
        }

        static void 循环末尾()
        {
            if (!_循环最终是否延迟) return;
            Delay(30);
        }


        switch (_循环条件1)
        {
            case true when _是否魔晶:
                {
                    if (
                        ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0)
                        &
                        !ColorAEqualColorB(w5, SimpleColor.FromRgb(25, 29, 32), 0) // 沉默 恐惧 不能释放
                    )
                        针刺(bts, size);

                    break;
                }
            case true:
                {
                    if (
                        ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)
                        &
                        !ColorAEqualColorB(w4, SimpleColor.FromRgb(14, 18, 20), 0) // 沉默 恐惧 不能释放
                    )
                        针刺(bts, size);

                    break;
                }
        }

        switch (_是否a杖)
        {
            case true when _循环条件2:
                {
                    if (_是否魔晶)
                    {
                        if (
                            ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)
                            &
                            !ColorAEqualColorB(q5, SimpleColor.FromRgb(25, 29, 32), 0))
                            鼻涕(bts, size);
                        else
                            return await FromResult(true);
                    }
                    else
                    {
                        if (
                            ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)
                            &
                            !ColorAEqualColorB(q4, SimpleColor.FromRgb(14, 18, 20), 0))
                            鼻涕(bts, size);
                        else
                            return await FromResult(true);
                    }

                    break;
                }
        }

        循环末尾();
        return await FromResult(true);
    }

    private static async Task<bool> 毛团去后摇(byte[] bts, Size size)
    {
        static void 毛团后()
        {
            _全局时间d = -1;
            //RightClick();
            KeyPress((uint)Keys.A);
            _条件开启切假腿 = true;
        }

        if (获取当前时间毫秒() - _全局时间d > 600 && _全局时间d != -1)
        {
            毛团后();
            return await FromResult(false);
        }

        var d5 = 获取d5左下角颜色(bts, size);

        if (ColorAEqualColorB(d5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        毛团后();
        return await FromResult(false);
    }

    #endregion

    #region 屠夫

    private static async Task<bool> 阿托斯接钩子(byte[] bts, Size size)
    {
        var time = 获取当前时间毫秒();
        根据图片以及类别使用物品(物品_阿托斯之棍_4, bts, size, _技能数量);
        var 是否以太 = RegPicture(物品_以太, bts, size);

        while (获取当前时间毫秒() - time < (是否以太 ? 420 : 300))
        {
        }

        KeyPress((uint)Keys.Q);
        return await FromResult(false);
    }

    private static async Task<bool> 钩子去僵直(byte[] bts, Size size)
    {
        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);
        var w4 = 获取w4开关颜色(bts, size);
        var w5 = 获取w5开关颜色(bts, size);

        static void 钩子后(SimpleColor w4, SimpleColor w5)
        {
            _全局时间q = -1;
            //RightClick();
            KeyPress((uint)Keys.S);
            switch (_是否魔晶)
            {
                case true when !ColorAEqualColorB(w5, SimpleColor.FromRgb(0, 129, 0), 0):
                case false when !ColorAEqualColorB(w4, SimpleColor.FromRgb(0, 129, 0), 0):
                    KeyPressWhile((uint)Keys.W, (uint)Keys.LShiftKey);
                    break;
            }

            if (_全局模式q != 1) return;
            根据图片以及类别队列使用物品(物品_纷争_被控, _全局bts, _全局size, _技能数量);
            根据图片以及类别队列使用物品(物品_虚灵_被控, _全局bts, _全局size, _技能数量);
            KeyPressWhile((uint)Keys.R, (uint)Keys.LShiftKey);
        }

        switch (_是否魔晶)
        {
            case true when ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0):
                return await FromResult(true);
            case true:
                钩子后(w4, w5);
                return await FromResult(false);

            case false when ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0):
                return await FromResult(true);
            case false:
                钩子后(w4, w5);
                return await FromResult(false);
        }
    }

    #endregion

    #region 破晓晨星

    private static async Task<bool> 石破天惊使用物品(byte[] bts, Size size)
    {
        static void 石破天惊后(in byte[] bts, Size size)
        {
            var bts1 = bts;
            根据图片以及类别使用物品(物品_相位鞋, bts1, size);
        }

        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        石破天惊后(bts, size);
        return await FromResult(false);
    }

    private static async Task<bool> 上界重锤去后摇(byte[] bts, Size size)
    {
        static void 上界重锤后()
        {
            RightClick();
        }

        var w4 = 获取w4左下角颜色(bts, size);

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        上界重锤后();
        return await FromResult(false);
    }

    #endregion

    #region 大鱼人

    private static async Task<bool> 踩去后摇(byte[] bts, Size size)
    {
        static void 鱼人碎击后()
        {
            switch (_全局模式w)
            {
                case 0:
                    KeyPress((uint)Keys.R);
                    break;
                case 1:
                    _条件保持假腿 = true;
                    KeyPress((uint)Keys.A);
                    break;
            }
        }

        var w4 = 获取w4左下角颜色(bts, size);

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        鱼人碎击后();
        return await FromResult(false);
    }

    private static async Task<bool> 跳刀接踩(byte[] bts, Size size)
    {
        if (
            根据图片以及类别使用物品(物品_魂戒CD, bts, size))
        {
            Delay(等待延迟);
            return await FromResult(true);

        }

        if (根据图片以及类别使用物品(物品_跳刀, bts, size) || 根据图片以及类别使用物品(物品_跳刀_力量跳刀, bts, size))

        {
            Delay(等待延迟);
            return await FromResult(true);

        }

        var w4 = 获取w4左下角颜色(bts, size);

        if (!ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);


        KeyPress((uint)Keys.W);
        return await FromResult(false);
    }

    private static async Task<bool> 雾霭去后摇(byte[] bts, Size size)
    {
        static void 雾霭后()
        {
            _条件保持假腿 = true;
            KeyPress((uint)Keys.A);
        }

        var r4 = 获取r4左下角颜色(bts, size);

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        雾霭后();
        return await FromResult(false);
    }

    #endregion

    #region 小小

    private static async Task<bool> 山崩去后摇(byte[] bts, Size size)
    {
        static void 山崩后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 500 && _全局时间q != -1)
        {
            山崩后();
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);

        switch (_是否a杖)
        {
            case true:
                if (ColorAEqualColorB(q5, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
                break;
            default:
                if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
                break;
        }

        山崩后();
        return await FromResult(false);
    }

    private static async Task<bool> 投掷去后摇(byte[] bts, Size size)
    {
        static void 投掷后()
        {
            _全局时间w = -1;
            _条件保持假腿 = true;
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 500 && _全局时间w != -1)
        {
            投掷后();
            return await FromResult(false);
        }

        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);

        switch (_是否a杖)
        {
            case true:
                if (ColorAEqualColorB(w5, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
                break;
            default:
                if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
                break;
        }

        投掷后();
        return await FromResult(false);
    }

    #endregion

    #region 小精灵

    private static async Task<bool> 循环续勋章(byte[] bts, Size size)
    {
        if (!RegPicture(物品_勇气勋章, bts, size) && !RegPicture(物品_炎阳勋章, bts, size))
            //TTS.Speak("未找到图片");
            if (_循环条件1) return await FromResult(true);
            else return await FromResult(false);

        var p = MousePosition;
        MouseMove(574 + _选择队友头像 * 61 + (_选择队友头像 >= 5 ? 216 : 0), 23);
        Delay(15);
        if (根据图片以及类别使用物品(物品_勇气勋章, bts, size, _技能数量) || 根据图片以及类别使用物品(物品_炎阳勋章, bts, size, _技能数量)) Delay(15);

        MouseMove(p);
        Delay(15);
        RightClick();
        if (_循环条件1) return await FromResult(true);
        else return await FromResult(false);
    }

    private static async Task<bool> 幽魂检测(byte[] bts, Size size)
    {
        if (RegPicture(小精灵_幽魂buff, bts, size))
        {
            _技能数量 = "6";
            return await FromResult(true);
        }

        _技能数量 = "4";
        return await FromResult(false);
    }

    #endregion

    #endregion

    #region 敏捷

    #region 露娜

    private static async Task<bool> 月光后敏捷平a(byte[] bts, Size size)
    {
        static void 月光后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            //RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1000 && _全局时间q != -1)
        {
            月光后();
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        月光后();
        return await FromResult(false);
    }

    private static async Task<bool> 月蚀后敏捷平a(byte[] bts, Size size)
    {
        static void 月蚀后()
        {
            _全局时间r = -1;
            _条件保持假腿 = true;
            //RightClick(); 
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 1000 && _全局时间r != -1)
        {
            月蚀后();
            return await FromResult(false);
        }

        var r4 = 获取r4左下角颜色(bts, size);

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        月蚀后();
        return await FromResult(false);
    }

    #endregion

    #region 影魔

    //private static void 吹风摇大()
    //{
    //    var w_down = 0;

    //    KeyPress((uint)Keys.Space);

    //    while (w_down == 0)
    //        if (RegPicture(Resource_Picture.吹风CD, 1291, 991, 60, 45))
    //        {
    //            w_down = 1;
    //            KeyPress((uint)Keys.M);
    //            delay(830);
    //            KeyPress((uint)Keys.R);
    //        }
    //}

    #endregion

    #region 小骷髅

    //private void 扫射接勋章()
    //{
    //    var time = 获取当前时间毫秒();

    //    // 勋章放在c位置
    //    while (RegPicture(Resource_Picture.物品_勇气, "C") || RegPicture(Resource_Picture.物品_炎阳, "C") ||
    //           RegPicture(Resource_Picture.物品_紫苑, "C") || RegPicture(Resource_Picture.物品_血棘, "C") ||
    //           RegPicture(Resource_Picture.物品_羊刀, "C"))
    //    {
    //        KeyPress((uint)Keys.C);
    //        Delay(等待延迟);
    //        if (获取当前时间毫秒() - time > 100) break;
    //    }

    //    time = 获取当前时间毫秒();

    //    while (RegPicture(Resource_Picture.物品_勇气, "Z") || RegPicture(Resource_Picture.物品_炎阳, "Z") ||
    //           RegPicture(Resource_Picture.物品_紫苑, "Z") || RegPicture(Resource_Picture.物品_血棘, "Z") ||
    //           RegPicture(Resource_Picture.物品_羊刀, "Z"))
    //    {
    //        KeyPress((uint)Keys.Z);
    //        Delay(等待延迟);
    //        if (获取当前时间毫秒() - time > 100) break;
    //    }

    //    time = 获取当前时间毫秒();

    //    while (RegPicture(Resource_Picture.物品_勇气, "SPACE") || RegPicture(Resource_Picture.物品_炎阳, "SPACE") ||
    //           RegPicture(Resource_Picture.物品_紫苑, "SPACE") || RegPicture(Resource_Picture.物品_血棘, "SPACE") ||
    //           RegPicture(Resource_Picture.物品_羊刀, "SPACE"))
    //    {
    //        KeyPress((uint)Keys.Space);
    //        Delay(等待延迟);
    //        if (获取当前时间毫秒() - time > 100) break;
    //    }

    //    time = 获取当前时间毫秒();

    //    // 否决放在x
    //    while (RegPicture(Resource_Picture.物品_否决, "X"))
    //    {
    //        KeyPress((uint)Keys.X);
    //        Delay(等待延迟);
    //        if (获取当前时间毫秒() - time > 100) break;
    //    }

    //    KeyPress((uint)Keys.A);

    //    Delay(100);

    //    切敏捷腿();
    //}

    //private static void 魂戒魔棒智力(byte[] bts, Size size)
    //{
    //    Delay(100);
    //    切智力腿(bts, size);
    //}

    #endregion

    #region 小松鼠

    private static void 捆接种树()
    {
    }

    private static void 飞镖接捆接种树()
    {
    }

    #endregion

    #region 拍拍

    private static async Task<bool> 超强力量平a(byte[] bts, Size size)
    {
        static void 超强力量后()
        {
            _全局时间w = -1;
            _条件保持假腿 = true;
            //RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 600 && _全局时间w != -1)
        {
            超强力量后();
            return await FromResult(false);
        }

        var w4 = 获取w4左下角颜色(bts, size);

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        超强力量后();
        return await FromResult(false);
    }

    private static async Task<bool> 震撼大地接平a(byte[] bts, Size size)
    {
        static void 震撼大地后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            //RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 600 && _全局时间q != -1)
        {
            震撼大地后();
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        震撼大地后();
        return await FromResult(false);
    }

    #endregion

    #region 巨魔

    //private static async Task<bool> 巨魔远程飞斧接平a后切回(byte[] bts, Size size)
    //{
    //    var q5 = 获取q5颜色(bts, size);
    //    var q6 = 获取q6颜色(bts, size);
    //    var w5 = 获取w5颜色(bts, size);
    //    var w6 = 获取w6颜色(bts, size);

    //    var 远程q颜色 = SimpleColor.FromRgb(56, 80, 80); // 远程形态 颜色
    //    var 远程飞斧可以释放颜色 = SimpleColor.FromRgb(87, 105, 97); // 技能可以释放颜色
    //    var 释放二阶段颜色 = SimpleColor.FromRgb(68, 165, 76); // 释放颜色
    //    var 技能进入CD颜色 = SimpleColor.FromRgb(255, 255, 255); // 释放完颜色
    //    var A杖技能进入CD颜色 = SimpleColor.FromRgb(1, 1, 1); // A杖释放完颜色

    //    switch (_全局步骤q)
    //    {
    //        case < 1:
    //        {
    //            if (_是否魔晶)
    //            {
    //                if (ColorAEqualColorB(远程q颜色, q6, 0))
    //                {
    //                    _全局步骤q = 1;
    //                }
    //            }
    //            else if (ColorAEqualColorB(远程q颜色, q5, 0))
    //            {
    //                _全局步骤q = 1;
    //            }

    //            return await FromResult(true);
    //        }
    //        case < 2:
    //        {
    //            if (_是否魔晶)
    //            {
    //                if (ColorAEqualColorB(远程飞斧可以释放颜色, w6, 0))
    //                {
    //                    _全局步骤q = 2;
    //                    _全局时间 = 获取当前时间毫秒();
    //                }
    //            }
    //            else if (ColorAEqualColorB(远程飞斧可以释放颜色, w5, 0))
    //            {
    //                _全局步骤q = 2;
    //                _全局时间 = 获取当前时间毫秒();
    //            }

    //            return await FromResult(true);
    //        }
    //        case < 4:
    //        {
    //            if (_是否魔晶)
    //            {
    //                if (ColorAEqualColorB(释放二阶段颜色,  w6, 0))
    //                {
    //                    _全局步骤q = 4;
    //                }

    //                KeyPress((uint)Keys.W);
    //                Delay(等待延迟);
    //            }
    //            else
    //            {
    //                if (ColorAEqualColorB(释放二阶段颜色, w5, 0))
    //                {
    //                    _全局步骤q = 4;
    //                }

    //                KeyPress((uint)Keys.W);
    //                Delay(等待延迟);
    //            }

    //            if (_全局时间 != -1 && 获取当前时间毫秒() - _全局时间 <= 500) return await FromResult(true);

    //            _全局时间 = -1;
    //            _全局步骤q = 0;
    //            return await FromResult(false);
    //        }
    //        case < 5:
    //        {
    //            if (_是否魔晶)
    //            {
    //                if (ColorAEqualColorB(A杖技能进入CD颜色,  w6, 0)
    //                    || ColorAEqualColorB(技能进入CD颜色, w6, 0))
    //                {
    //                    await Run(() =>
    //                    {
    //                        KeyPress((uint)Keys.A);
    //                        Delay(230);
    //                        KeyPress((uint)Keys.Q);
    //                        KeyPress((uint)Keys.M);
    //                        Delay(150);
    //                        KeyPress((uint)Keys.A);
    //                    });
    //                    _全局时间 = -1;
    //                    _全局步骤q = 0;
    //                    return await FromResult(false);
    //                }
    //            }
    //            else
    //            {
    //                if (ColorAEqualColorB(A杖技能进入CD颜色,  w5, 0)
    //                    || ColorAEqualColorB(技能进入CD颜色, w5, 0))
    //                {
    //                    await Run(() =>
    //                    {
    //                        KeyPress((uint)Keys.A);
    //                        Delay(230);
    //                        KeyPress((uint)Keys.Q);
    //                        KeyPress((uint)Keys.M);
    //                        Delay(150);
    //                        KeyPress((uint)Keys.A);
    //                    });
    //                    _全局时间 = -1;
    //                    _全局步骤q = 0;
    //                    return await FromResult(false);
    //                }
    //            }

    //            if (_全局时间 != -1 && 获取当前时间毫秒() - _全局时间 <= 500) return await FromResult(true);

    //            _全局时间 = -1;
    //            _全局步骤q = 0;
    //            return await FromResult(false);
    //        }
    //        default:
    //            return await FromResult(true);
    //    }
    //}

    #endregion

    #region 小鱼人

    private static async Task<bool> 黑暗契约平a(byte[] bts, Size size)
    {
        static void 黑暗契约后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            RightClick();
            // KeyPress((uint) Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 300 && _全局时间q != -1)
        {
            黑暗契约后();
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true when ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0):
                return await FromResult(true);
            case true:
                黑暗契约后();
                return await FromResult(false);

            case false when ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0):
                return await FromResult(true);
            case false:
                黑暗契约后();
                return await FromResult(false);
        }
    }

    private static async Task<bool> 跳水a(byte[] bts, Size size)
    {
        KeyPress((uint)Keys.A);
        _条件保持假腿 = true;
        return await FromResult(false);
    }

    private static async Task<bool> 深海护罩a(byte[] bts, Size size)
    {
        static void 深海护罩后()
        {
            _全局时间d = -1;
            _条件保持假腿 = true;
            //RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间d > 500 && _全局时间d != -1)
        {
            深海护罩后();
            return await FromResult(false);
        }

        var d5 = 获取d5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true when ColorAEqualColorB(d5, SimpleColor.FromRgb(45, 52, 59), 0): // 一般技能原色颜色
                return await FromResult(true);
            case true:
                深海护罩后();
                return await FromResult(false);
        }

        return await FromResult(true);
    }

    #endregion

    #region 敌法

    private static async Task<bool> 闪烁敏捷(byte[] bts, Size size)
    {
        static void 闪烁后()
        {
            _全局时间w = -1;
            _条件保持假腿 = true;
            //RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 600 && _全局时间w != -1)
        {
            闪烁后();
            return await FromResult(false);
        }

        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);

        switch (_是否a杖)
        {
            case true when ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0):
                return await FromResult(true);
            case true:
                闪烁后();
                return await FromResult(false);

            case false when ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0):
                return await FromResult(true);
            case false:
                闪烁后();
                return await FromResult(false);
        }
    }

    private static async Task<bool> 法力虚空取消后摇(byte[] bts, Size size)
    {
        static void 法力虚空后()
        {
            _全局时间r = -1;
            _条件保持假腿 = true;
            //RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 600 && _全局时间r != -1)
        {
            法力虚空后();
            return await FromResult(false);
        }

        var r4 = 获取r4左下角颜色(bts, size);
        var r5 = 获取r5左下角颜色(bts, size);

        switch (_是否a杖)
        {
            case true when ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0):
                return await FromResult(true);
            case true:
                {
                    法力虚空后();
                    return await FromResult(false);
                }
            case false when ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0):
                return await FromResult(true);
            case false:
                {
                    法力虚空后();
                    return await FromResult(false);
                }
        }
    }

    #endregion

    #region 猴子

    private static async Task<bool> 灵魂之矛敏捷(byte[] bts, Size size)
    {
        static void 灵魂之矛后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            RightClick();
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1000 && _全局时间q != -1)
        {
            灵魂之矛后();
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        灵魂之矛后();
        return await FromResult(false);
    }

    private static async Task<bool> 神行百变敏捷(byte[] bts, Size size)
    {
        static void 神行百变后()
        {
            _全局时间w = -1;
            Delay(1000);
            _条件保持假腿 = true;
            RightClick();
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 1500 && _全局时间w != -1)
        {
            神行百变后();
            return await FromResult(false);
        }

        var w4 = 获取w4左下角颜色(bts, size);

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        神行百变后();
        return await FromResult(false);
    }

    #endregion

    #region 幻刺

    private static async Task<bool> 窒息短匕敏捷(byte[] bts, Size size)
    {
        static void 匕首后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            RightClick();
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            匕首后();
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (!ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0))
                    {
                        匕首后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (!ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0))
                    {
                        匕首后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 幻影突袭敏捷(byte[] bts, Size size)
    {
        static void 幻影突袭后()
        {
            _全局时间w = -1;
            _条件保持假腿 = true;
        }

        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
        {
            幻影突袭后();
            return await FromResult(false);
        }

        switch (_是否魔晶)
        {
            case true:
                {
                    if (!ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0))
                    {
                        幻影突袭后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (!ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0))
                    {
                        幻影突袭后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 魅影无形敏捷(byte[] bts, Size size)
    {
        static void 魅影无形后()
        {
            _全局时间e = -1;
            _条件保持假腿 = true;
            RightClick();
        }

        var e4 = 获取e4左下角颜色(bts, size);
        var e5 = 获取e5左下角颜色(bts, size);

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间e > 1200 && _全局时间e != -1)
        {
            魅影无形后();
            return await FromResult(false);
        }

        switch (_是否魔晶)
        {
            case true:
                {
                    if (!ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0))
                    {
                        魅影无形后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (!ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0))
                    {
                        魅影无形后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 刀阵旋风敏捷(byte[] bts, Size size)
    {
        static void 刀阵旋风后()
        {
            _全局时间e = -1;
            _条件保持假腿 = true;
            KeyPress((uint)Keys.A);
        }

        var d5 = 获取d5左下角颜色(bts, size);

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间e > 1200 && _全局时间e != -1)
        {
            刀阵旋风后();
            return await FromResult(false);
        }

        switch (_是否魔晶)
        {
            case true:
                {
                    if (!ColorAEqualColorB(d5, SimpleColor.FromRgb(45, 52, 59), 0))
                    {
                        刀阵旋风后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    #endregion

    #region 虚空

    private static async Task<bool> 时间漫游敏捷(byte[] bts, Size size)
    {
        static void 时间漫游后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            时间漫游后();
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        时间漫游后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        时间漫游后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 时间膨胀敏捷(byte[] bts, Size size)
    {
        static void 时间膨胀后()
        {
            _全局时间w = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
        {
            时间膨胀后();
            return await FromResult(false);
        }

        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        时间膨胀后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        时间膨胀后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 时间结界敏捷(byte[] bts, Size size)
    {
        static void 时间结界后()
        {
            _全局时间r = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
        {
            时间结界后();
            return await FromResult(false);
        }

        var r4 = 获取r4左下角颜色(bts, size);
        var r5 = 获取r5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        时间结界后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        时间结界后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    #endregion

    #region TB

    private static async Task<bool> 倒影敏捷(byte[] bts, Size size)
    {
        static void 倒影后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            倒影后();
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);
        var q6 = 获取q6左下角颜色(bts, size);

        switch (_是否魔晶 || _是否a杖)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)
                        && !ColorAEqualColorB(q6, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        倒影后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        倒影后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 幻惑敏捷(byte[] bts, Size size)
    {
        static void 幻惑后()
        {
            _全局时间w = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
        {
            幻惑后();
            return await FromResult(false);
        }

        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);
        var w6 = 获取w6左下角颜色(bts, size);

        switch (_是否魔晶 || _是否a杖)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0)
                        && !ColorAEqualColorB(w6, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        幻惑后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        幻惑后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 魔化敏捷(byte[] bts, Size size)
    {
        static void 魔化后()
        {
            _全局时间e = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间e > 1200 && _全局时间e != -1)
        {
            魔化后();
            return await FromResult(false);
        }

        var e4 = 获取e4左下角颜色(bts, size);
        var e5 = 获取e5左下角颜色(bts, size);
        var e6 = 获取e6左下角颜色(bts, size);

        switch (_是否魔晶 || _是否a杖)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0)
                        && !ColorAEqualColorB(e6, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        魔化后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        魔化后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 恶魔狂热去后摇(byte[] bts, Size size)
    {
        static void 恶魔狂热后()
        {
            _全局时间d = -1;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间d > 1200 && _全局时间d != -1)
        {
            恶魔狂热后();
            return await FromResult(false);
        }

        var d5 = 获取d5左下角颜色(bts, size);
        var d6 = 获取d6左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(d5, SimpleColor.FromRgb(45, 52, 59), 0)
                        && !ColorAEqualColorB(d6, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        恶魔狂热后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 恐怖心潮敏捷(byte[] bts, Size size)
    {
        static void 恐怖心潮后()
        {
            _全局时间f = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间f > 1200 && _全局时间f != -1)
        {
            恐怖心潮后();
            return await FromResult(false);
        }

        var d5 = 获取d5左下角颜色(bts, size);
        var f6 = 获取f6左下角颜色(bts, size);

        switch (_是否a杖)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(d5, SimpleColor.FromRgb(45, 52, 59), 0)
                        && !ColorAEqualColorB(f6, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        恐怖心潮后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 断魂敏捷(byte[] bts, Size size)
    {
        static void 魂断后()
        {
            _全局时间r = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
        {
            魂断后();
            return await FromResult(false);
        }

        var r4 = 获取r4左下角颜色(bts, size);
        var r5 = 获取r5左下角颜色(bts, size);
        var r6 = 获取r6左下角颜色(bts, size);

        switch (_是否魔晶 || _是否a杖)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0)
                        && !ColorAEqualColorB(r6, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        魂断后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        魂断后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    #endregion

    #region 赏金

    private static async Task<bool> 飞镖接平a(byte[] bts, Size size)
    {
        static void 飞镖后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            飞镖后();
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        飞镖后();
        return await FromResult(false);
    }

    private static async Task<bool> 标记去后摇(byte[] bts, Size size)
    {
        static void 标记后()
        {
            _全局时间r = -1;
            _条件保持假腿 = true;
            RightClick();
            // KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
        {
            标记后();
            return await FromResult(false);
        }

        var r4 = 获取r4左下角颜色(bts, size);

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        标记后();
        return await FromResult(false);
    }

    private static async Task<bool> 循环标记(byte[] bts, Size size)
    {
        var r4 = 获取r4左下角颜色(bts, size);
        if (
            !ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)
        ) if (_循环条件1) return await FromResult(true);else return await FromResult(false);

        KeyPress((uint)Keys.R);
        Delay(100);
        if (_循环条件1) return await FromResult(true); else return await FromResult(false);
    }

    #endregion

    #region 剧毒

    private static async Task<bool> 循环蛇棒(byte[] bts, Size size)
    {
        var e4 = 获取e4左下角颜色(bts, size);
        if (
            !ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)
        ) if (_循环条件1) return await FromResult(true); else return await FromResult(false);

        KeyPress((uint)Keys.E);
        Delay(等待延迟);
        if (_循环条件1) return await FromResult(true); else return await FromResult(false);
    }

    private static async Task<bool> 蛇棒去后摇(byte[] bts, Size size)
    {
        var e4 = 获取e4左下角颜色(bts, size);

        if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        RightClick();

        return await FromResult(false);
    }

    private static async Task<bool> 瘴气去后摇(byte[] bts, Size size)
    {
        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        KeyPress((uint)Keys.A);
        return await FromResult(false);
    }

    private static async Task<bool> 剧毒新星去后摇(byte[] bts, Size size)
    {
        var r4 = 获取r4左下角颜色(bts, size);

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        RightClick();
        return await FromResult(false);
    }

    #endregion

    #region 美杜莎

    private static async Task<bool> 秘术异蛇去后摇(byte[] bts, Size size)
    {
        static void 秘术银蛇后()
        {
            _全局时间w = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);

            if (!_循环条件1) return;
            KeyPress((uint)Keys.Q);
            Delay(等待延迟);
            KeyPress((uint)Keys.Q);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
        {
            秘术银蛇后();
            return await FromResult(false);
        }

        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        秘术银蛇后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        秘术银蛇后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 石化凝视去后摇(byte[] bts, Size size)
    {
        static void 石化凝视后()
        {
            _全局时间r = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);

            if (!_循环条件1) return;
            KeyPress((uint)Keys.Q);
            Delay(等待延迟);
            KeyPress((uint)Keys.Q);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
        {
            石化凝视后();
            return await FromResult(false);
        }

        var r4 = 获取r4左下角颜色(bts, size);
        var r5 = 获取r5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        石化凝视后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        石化凝视后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    #endregion

    #region 幽鬼

    private static async Task<bool> 幽鬼之刃去后摇(byte[] bts, Size size)
    {
        static void 幽鬼之刃后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            RightClick();
            // KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            幽鬼之刃后();
            return await FromResult(false);
        }

        var q5 = 获取q5左下角颜色(bts, size);
        var q6 = 获取q6左下角颜色(bts, size);

        if (ColorAEqualColorB(q6, SimpleColor.FromRgb(45, 52, 59), 0) ||
            ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        幽鬼之刃后();
        return await FromResult(false);
    }

    private static async Task<bool> 如影随形去后摇(byte[] bts, Size size)
    {
        static void 如影随形后()
        {
            _全局时间f = -1;
            _条件保持假腿 = true;
            //RightClick();
            KeyPress((uint)Keys.D);
            Delay(等待延迟);
            KeyPress((uint)Keys.X);
            // KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间f > 1200 && _全局时间f != -1)
        {
            如影随形后();
            return await FromResult(false);
        }

        var f6 = 获取f6左下角颜色(bts, size);

        if (ColorAEqualColorB(f6, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        如影随形后();
        return await FromResult(false);
    }

    private static async Task<bool> 鬼影重重去后摇(byte[] bts, Size size)
    {
        static void 鬼影重重后()
        {
            _全局时间r = -1;
            _条件保持假腿 = true;
            //RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
        {
            鬼影重重后();
            return await FromResult(false);
        }

        var r5 = 获取r5左下角颜色(bts, size);
        var r6 = 获取r6左下角颜色(bts, size);

        if (ColorAEqualColorB(r6, SimpleColor.FromRgb(45, 52, 59), 0) ||
            ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        鬼影重重后();
        return await FromResult(false);
    }

    #endregion

    #region 火枪

    private static async Task<bool> 流霰弹去后摇(byte[] bts, Size size)
    {
        static void 流霰弹后()
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            流霰弹后();
            return await FromResult(false);
        }

        var q4 = 获取q4颜色(bts, size);
        var q5 = 获取q5颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        ColorAEqualColorB(q5, SimpleColor.FromRgb(94, 154, 25), 0)
                    )
                    {
                        流霰弹后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        ColorAEqualColorB(q4, SimpleColor.FromRgb(72, 150, 11), 0)
                    )
                    {
                        流霰弹后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 瞄准去后摇(byte[] bts, Size size)
    {
        static void 瞄准后(in byte[] bts1, Size size)
        {
            _全局时间e = -1;
            根据图片以及类别使用物品(物品_疯狂面具, bts1, size, _技能数量);
            KeyPress((uint)Keys.A);
            _条件保持假腿 = true;
            // RightClick();
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间e > 1200 && _全局时间e != -1)
        {
            瞄准后(bts, size);
            return await FromResult(false);
        }

        var e4 = 获取e4左下角颜色(bts, size);
        var e5 = 获取e5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        瞄准后(bts, size);
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        瞄准后(bts, size);
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 暗杀去后摇(byte[] bts, Size size)
    {
        static void 暗杀后()
        {
            _全局时间r = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 2500 && _全局时间r != -1)
        {
            暗杀后();
            return await FromResult(false);
        }

        var r4 = 获取r4左下角颜色(bts, size);
        var r5 = 获取r5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        暗杀后();
                        return await FromResult(false);
                    }

                    break;
                }
            default:
                {
                    if (
                        !ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)
                    )
                    {
                        暗杀后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 震荡手雷去后摇(byte[] bts, Size size)
    {
        static void 震荡手雷后()
        {
            _全局时间d = -1;
            _条件保持假腿 = true;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间d > 1200 && _全局时间d != -1)
        {
            震荡手雷后();
            return await FromResult(false);
        }

        var d5 = 获取d5左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true:
                {
                    if (
                        !ColorAEqualColorB(d5, SimpleColor.FromRgb(45, 52, 59), 0)
                    )
                    {
                        震荡手雷后();
                        return await FromResult(false);
                    }

                    break;
                }
        }

        return await FromResult(true);
    }

    #endregion

    #region 小黑

    private static async Task<bool> 狂风去后摇(byte[] bts, Size size)
    {
        static void 狂风后()
        {
            _全局时间q = -1;
            switch (_全局模式)
            {
                case 1:
                    _条件开启切假腿 = false;
                    切智力腿(_技能数量);
                    break;
                default:
                    _条件开启切假腿 = true;
                    _条件保持假腿 = true;
                    break;
            }

            // RightClick();
            KeyPress((uint)Keys.A);
        }

        var w4 = 获取w4左下角颜色(bts, size);

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        狂风后();
        return await FromResult(false);
    }

    private static async Task<bool> 数箭齐发去后摇(byte[] bts, Size size)
    {
        static void 数箭齐发后(in byte[] bts1, Size size)
        {
            _全局时间q = -1;
            var is25 = ColorAEqualColorB(GetSPixelBytes(bts1, size, 754 - 截图模式1X, 957 - 截图模式1Y),
                SimpleColor.FromRgb(246, 178, 60), 0);
            Delay(is25 ? 2600 : 1300);
            switch (_全局模式)
            {
                case 1:
                    _条件开启切假腿 = false;
                    切智力腿(_技能数量);
                    break;
                default:
                    _条件开启切假腿 = true;
                    _条件保持假腿 = true;
                    break;
            }

            // RightClick();
            KeyPress((uint)Keys.S);
            KeyPress((uint)Keys.A);
        }

        var e4 = 获取e4左下角颜色(bts, size);

        if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        数箭齐发后(bts, size);
        return await FromResult(false);
    }

    #endregion

    #endregion

    #region 智力

    #region 黑鸟

    private static async Task<bool> 关接陨星锤(byte[] bts, Size size)
    {
        var w4 = 获取w4颜色(bts, size);

        var time = 0;

        var 技能点颜色 = SimpleColor.FromRgb(203, 183, 124);

        if (ColorAEqualColorB(GetSPixelBytes(bts, size, 909 - 截图模式1X, 1008 - 截图模式1Y), 技能点颜色, 0))
            time = 4000;
        else if (ColorAEqualColorB(GetSPixelBytes(bts, size, 897 - 截图模式1X, 1008 - 截图模式1Y), 技能点颜色, 0))
            time = 3250;

        static async Task 关后(int time, byte[] bts, Size size)
        {
            await Run(() =>
            {
                Delay(110);
                初始化全局时间(ref _全局时间w);
                RightClick();
                Delay(150);
                KeyPress((uint)Keys.S);
                Delay(time - 3000, _全局时间w);
                if (!_中断条件) 根据图片以及类别使用物品(物品_陨星锤, bts, size);
            });
        }

        if (!ColorAEqualColorB(w4, SimpleColor.FromRgb(183, 242, 203), 0)) return await FromResult(true);

        关后(time, bts, size).Start();
        return await FromResult(false);
    }

    private static async Task<bool> 神智之蚀去后摇(byte[] bts, Size size)
    {
        static void 神智之蚀后()
        {
            KeyPress((uint)Keys.A);
        }

        var r4 = 获取r4左下角颜色(bts, size);

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        神智之蚀后();
        return await FromResult(false);
    }

    private static async Task<bool> 关接跳(byte[] bts, Size size)
    {
        return 根据图片以及类别使用物品(物品_跳刀, bts, size, _技能数量) ? await FromResult(false) : await FromResult(true);
        ;
    }

    #endregion

    #region 谜团

    private void 跳秒接午夜凋零黑洞()
    {
        //if (RegPicture(物品_黑黄杖, "Z")) KeyPress((uint) Keys.Z);

        //if (RegPicture(物品_纷争, "C")) KeyPress((uint) Keys.C);

        //var time = 获取当前时间毫秒();

        //while (RegPicture(物品_跳刀, "SPACE") || RegPicture(物品_跳刀_智力跳刀, "SPACE"))
        //{
        //    Delay(15);
        //    KeyPress((uint) Keys.Space);

        //    if (获取当前时间毫秒() - time > 300) break;
        //}

        Delay(等待延迟);

        //KeyDown((uint)Keys.LControlKey);

        //KeyPress((uint)Keys.A);

        //KeyUp((uint)Keys.LControlKey);
    }

    private void 刷新接凋零黑洞()
    { 
        KeyPress((uint)Keys.X);

        for (var i = 0; i < 2; i++)
        {
            Delay(等待延迟);
            KeyPress((uint)Keys.Z);
            KeyPress((uint)Keys.V);
            KeyPress((uint)Keys.R);
        }
    }

    #endregion

    #region 冰女

    #endregion

    #region 火女

    private static async Task<bool> 龙破斩去后摇(byte[] bts, Size size)
    {
        return await FromResult(false);
    }

    private static async Task<bool> 光击阵去后摇(byte[] bts, Size size)
    {
        return await FromResult(false);
    }

    private static async Task<bool> 神灭斩去后摇(byte[] bts, Size size)
    {
        return await FromResult(false);
    }

    #endregion

    #region 蓝猫

    private static async Task<bool> 拉接平A(byte[] bts, Size size)
    {
        return await FromResult(true);
    }

    private void 残影接平A()
    {
        Delay(等待延迟);
        KeyPress((uint)Keys.A);
    }

    private static async Task<bool> 滚接平A(byte[] bts, Size size)
    {
        return await FromResult(true);
    }

    #endregion

    #region 宙斯

    private static async Task<bool> 弧形闪电去后摇(byte[] bts, Size size)
    {
        static async Task 弧形闪电后()
        {
            await Run(() =>
            {
                switch (_全局模式)
                {
                    case < 1:
                        KeyPress((uint)Keys.A);
                        break;
                    case 1:
                        RightClick();
                        break;
                }
            });
        }

        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);
        var q6 = 获取q6左下角颜色(bts, size);

        switch (_是否魔晶)
        {
            case true when _是否a杖:
                {
                    if (ColorAEqualColorB(q6, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
                    弧形闪电后().Start();
                    return await FromResult(false);
                }
            default:
                {
                    if (_是否魔晶 || _是否a杖)
                    {
                        if (ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
                        弧形闪电后().Start();
                        return await FromResult(false);
                    }

                    if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
                    弧形闪电后().Start();
                    return await FromResult(false);
                }
        }
    }

    private static async Task<bool> 弧形闪电不能释放(byte[] bts, Size size)
    {
        /// 逻辑 
        /// 先检测是否可以释放
        /// 如果可以则返回false
        /// 外部触发原本逻辑
        /// 否则改全局模式为1，并重复触发本逻辑
        /// 直到可以释放，改回全局模式0，再延迟释放释放
        /// 用不同的循环条件，避免不可预知的时序错误

        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);
        var q6 = 获取q6左下角颜色(bts, size);

        if (_是否魔晶 && _是否a杖)
        {
            if (!ColorAEqualColorB(q6, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

            switch (_全局模式q)
            {
                case 0:
                    return await FromResult(false);
                case 1:
                    _全局模式q = 0;
                    await Run(() => { KeyPress((uint)Keys.Q); });
                    return await FromResult(false);
            }
        }
        else if (_是否魔晶 || _是否a杖)
        {
            if (!ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

            switch (_全局模式q)
            {
                case 0:
                    return await FromResult(false);
                case 1:
                    _全局模式q = 0;
                    await Run(() => { KeyPress((uint)Keys.Q); });
                    return await FromResult(false);
            }
        }
        else
        {
            if (!ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

            switch (_全局模式q)
            {
                case 0:
                    return await FromResult(false);
                case 1:
                    _全局模式q = 0;
                    Run(() => { KeyPress((uint)Keys.Q); });
                    return await FromResult(false);
            }
        }

        return await FromResult(true);
    }

    private static async Task<bool> 雷击去后摇(byte[] bts, Size size)
    {
        static async Task 雷击后()
        {
            await Run(() =>
            {
                switch (_全局模式)
                {
                    case < 1:
                        KeyPress((uint)Keys.A);
                        break;
                    case 1:
                        RightClick();
                        break;
                }
            });
        }

        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);
        var w6 = 获取w6左下角颜色(bts, size);

        if (_是否魔晶 && _是否a杖)
        {
            if (ColorAEqualColorB(w6, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

            雷击后().Start();
            return await FromResult(false);
        }

        if (_是否魔晶 || _是否a杖)
        {
            if (ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

            雷击后().Start();
            return await FromResult(false);
        }


        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        雷击后().Start();
        return await FromResult(false);
    }

    private static async Task<bool> 神圣一跳去后摇(byte[] bts, Size size)
    {
        static async Task 神圣一跳后()
        {
            await Run(() =>
            {
                switch (_全局模式)
                {
                    case < 1:
                        KeyPress((uint)Keys.A);
                        break;
                    case 1:
                        RightClick();
                        break;
                }
            });
        }

        var e4 = 获取e4左下角颜色(bts, size);
        var e5 = 获取e5左下角颜色(bts, size);
        var e6 = 获取e6左下角颜色(bts, size);

        if (_是否魔晶 && _是否a杖)
        {
            if (ColorAEqualColorB(e6, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

            神圣一跳后().Start();
            return await FromResult(false);
        }

        if (_是否魔晶 || _是否a杖)
        {
            if (ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

            神圣一跳后().Start();
            return await FromResult(false);
        }


        if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        神圣一跳后().Start();
        return await FromResult(false);
    }

    #endregion

    #region 卡尔

    private void 三冰对线()
    {
        KeyPress((uint)Keys.Q);
        Delay(等待延迟);
        KeyPress((uint)Keys.Q);
        Delay(等待延迟);
        KeyPress((uint)Keys.Q);
        Delay(等待延迟);
    }

    private void 三火平A()
    {
        KeyPress((uint)Keys.E);
        Delay(等待延迟);
        KeyPress((uint)Keys.E);
        Delay(等待延迟);
        KeyPress((uint)Keys.E);
        Delay(等待延迟);
    }

    private void 三雷幽灵()
    {
        KeyPress((uint)Keys.Q);
        Delay(等待延迟);
        KeyPress((uint)Keys.Q);
        Delay(等待延迟);
        KeyPress((uint)Keys.W);
        Delay(等待延迟);
        KeyPress((uint)Keys.R);
        Delay(等待延迟);
        KeyPress((uint)Keys.W);
        Delay(等待延迟);
        KeyPress((uint)Keys.W);
        Delay(等待延迟);
        KeyPress((uint)Keys.D);
    }

    private void 吹风天火()
    {
        KeyPress((uint)Keys.W);
        Delay(等待延迟);
        KeyPress((uint)Keys.W);
        Delay(等待延迟);
        KeyPress((uint)Keys.Q);
        Delay(等待延迟);
        KeyPress((uint)Keys.R);
        Delay(等待延迟);
        KeyPress((uint)Keys.D);
        Delay(等待延迟);
        KeyPress((uint)Keys.E);
        Delay(等待延迟);
        KeyPress((uint)Keys.E);
        Delay(等待延迟);
        KeyPress((uint)Keys.E);
        Delay(等待延迟);
        KeyPress((uint)Keys.R);
        Delay(600);
        KeyPress((uint)Keys.D);
    }

    #endregion

    #region 拉席克

    private void 吹风接撕裂大地()
    {
        // var all_down = 0;
        //if (RegPicture(物品_吹风CD完, "SPACE"))
        //{
        //    label1.Text = "FF";
        //    KeyPress((uint) Keys.Space);
        //    Delay(等待延迟);

        //    var time = 获取当前时间毫秒();

        //    while (all_down == 0)
        //    {
        //        if (获取当前时间毫秒() - time > 4000) break;

        //        //if (RegPicture(物品_吹风CD, "SPACE"))
        //        //{
        //        //    label1.Text = "FFF";
        //        //    if (RegPicture(物品_纷争, "C")) KeyPress((uint) Keys.C);
        //        //    Delay(80);
        //        //    KeyPress((uint) Keys.H);
        //        //    Delay(1280);
        //        //    if (_中断条件) break;
        //        //    KeyPress((uint) Keys.Q);
        //        //    Delay(760);
        //        //    KeyPress((uint) Keys.R);
        //        //    all_down = 1;
        //        //}

        //        Delay(50);
        //    }
        //}
    }

    #endregion

    #region 暗影萨满

    /// <summary>
    ///     前摇时间基本在
    /// </summary>
    /// <param name="bts"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private static async Task<bool> 苍穹振击取消后摇(byte[] bts, Size size)
    {
        static async Task 苍穹振击后()
        {
            await Run(() =>
            {
#if 检测延时
                检测时间播报(_全局时间q, 2000);
#endif
                KeyPress((uint)Keys.A);
            });
        }

        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        苍穹振击后().Start();
        return await FromResult(false);
    }

    /// <summary>
    ///     前摇时间基本再380-450 之间
    /// </summary>
    /// <param name="bts"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private static async Task<bool> 枷锁持续施法隐身(byte[] bts, Size size)
    {
        static async Task 枷锁后Async(byte[] bts, Size size)
        {
            await Run(() =>
            {
#if 检测延时
                检测时间播报(_全局时间e, 4000);
#endif
                if (!RegPicture(物品_暗影护符buff, bts, size)) 根据图片以及类别自我使用物品(物品_暗影护符, bts, size, _技能数量);
            });
        }

        var e4 = 获取e4左下角颜色(bts, size);

        if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        枷锁后Async(bts, size).Start();
        return await FromResult(false);
    }

    private static async Task<bool> 释放群蛇守卫取消后摇(byte[] bts, Size size)
    {
        static async Task 群蛇守卫后()
        {
            await Run(() =>
            {
#if 检测延时
                检测时间播报(_全局时间r, 3000);
#endif
                KeyPress((uint)Keys.A);
            });
        }

        var r4 = 获取r4左下角颜色(bts, size);

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        群蛇守卫后().Start();
        return await FromResult(false);
    }

    private static async Task<bool> 变羊取消后摇(byte[] bts, Size size)
    {
        static async Task 萨满变羊后(byte[] bts, Size size)
        {
            初始化全局时间(ref _全局时间w);

            await Run(async () =>
            {
                var time = 1250;

                var 技能点颜色 = SimpleColor.FromRgb(203, 183, 124);

                if (ColorAEqualColorB(GetSPixelBytes(bts, size, 909 - 截图模式1X, 1008 - 截图模式1Y), 技能点颜色, 0))
                    time = 3500;
                else if (ColorAEqualColorB(GetSPixelBytes(bts, size, 897 - 截图模式1X, 1008 - 截图模式1Y), 技能点颜色, 0))
                    time = 2750;
                else if (ColorAEqualColorB(GetSPixelBytes(bts, size, 885 - 截图模式1X, 1008 - 截图模式1Y), 技能点颜色, 0))
                    time = 2000;
                else if (ColorAEqualColorB(GetSPixelBytes(bts, size, 875 - 截图模式1X, 1008 - 截图模式1Y), 技能点颜色, 0))
                    time = 1300;

                var 智力跳刀buff = await Form2.智力跳刀buff(bts, size);
                time = Convert.ToInt32(_状态抗性倍数 * time);
#if 检测延时
                TTS.Speak(string.Concat("延时", time.ToString()));
#endif

                KeyPress((uint)Keys.A);

                switch (_全局模式w)
                {
                    case 1:
                        Delay(time - (智力跳刀buff ? 217 : 435), _全局时间w);
                        KeyPress((uint)Keys.E);
                        break;
                    case 2:
                        KeyPress((uint)Keys.Q);
                        break;
                    case 3:
                        KeyPress((uint)Keys.Q);
                        Delay(time - (智力跳刀buff ? 217 : 435), _全局时间w);
                        KeyPress((uint)Keys.E);
                        break;
                    case 4:
                        KeyPress((uint)Keys.R);
                        Delay(智力跳刀buff ? 200 : 400);
                        KeyPress((uint)Keys.Q);
                        Delay(time - (智力跳刀buff ? 217 : 435), _全局时间w);
                        KeyPress((uint)Keys.E);
                        break;
                }
            });
        }

        var w4 = 获取w4左下角颜色(bts, size);

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        萨满变羊后(bts, size).Start();
        return await FromResult(false);
    }

    #endregion

    #region 小仙女

    private void 皇冠延时计时()
    {
        var 总开始时间 = 获取当前时间毫秒();

        var w_down = 0;

        while (w_down == 0)
            if (获取当前时间毫秒() - 总开始时间 > 2000)
                return;
    }

    private void 诅咒皇冠吹风()
    {
        //var 总开始时间 = 获取当前时间毫秒();

        //KeyPress((uint)Keys.E);

        //var w_down = 0;

        //while (w_down == 0)
        //{
        //    if (获取当前时间毫秒() - 总开始时间 > 2000) return;

        //    if (RegPicture(小仙女_释放诅咒皇冠_不朽, "E", 7))
        //    {
        //        // Delay(阿哈利姆魔晶() ? 410 : 1410);  // 大部分技能抬手都是0.2-0.3之间
        //        if (!_循环条件2) return;

        //        if (RegPicture(物品_吹风, "SPACE", 7))
        //        {
        //            KeyPress((uint)Keys.Space);
        //            KeyPress((uint)Keys.M);

        //            Delay(2500);
        //            if (!_循环条件2) return;
        //            作祟暗影之境最大化伤害();
        //        }

        //        w_down = 1;
        //    }
        //}
    }

    private static void 作祟暗影之境最大化伤害()
    {
        // 释放纷争，增加大量伤害
        //if (RegPicture(物品_纷争, "C", 7)) KeyPress((uint) Keys.C);

        KeyPress((uint)Keys.M);
        Delay(等待延迟);
        KeyPress((uint)Keys.D);
        Delay(等待延迟);
        KeyPress((uint)Keys.W);
        Delay(等待延迟);
        KeyPress((uint)Keys.W);

        //var 暗影之境_开始时间 = 获取当前时间毫秒();

        //if (阿哈利姆神杖())
        //{
        //    Delay(400);
        //    KeyPress((uint)Keys.A);
        //}
        //else
        //{
        //    while (获取当前时间毫秒() - 暗影之境_开始时间 < 4500 || !loop_bool_2) { }
        //    if (!loop_bool_2) return;
        //    KeyPress((uint)Keys.A);
        //}
    }

    #endregion

    #region 天怒

    private static async Task<bool> 循环奥数鹰隼(byte[] bts, Size size)
    {
        static void 释放奥数鹰隼()
        {
            KeyPress((uint)Keys.Q);
            Delay(等待延迟);
        }

        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);

        if (_是否魔晶)
        {
            if (!ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)) return _循环条件1 ? await FromResult(true) : await FromResult(false);
            释放奥数鹰隼();
        }

        if (!ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return _循环条件1 ? await FromResult(true) : await FromResult(false);
        释放奥数鹰隼();

        return _循环条件1 ? await FromResult(true) : await FromResult(false);
    }

    private static async Task<bool> 天怒秒人连招(byte[] bts, Size size)
    {
        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);
        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);
        var e4 = 获取e4左下角颜色(bts, size);
        var e5 = 获取e5左下角颜色(bts, size);
        var r4 = 获取r4左下角颜色(bts, size);
        var r5 = 获取r5左下角颜色(bts, size);

        switch (_全局步骤)
        {
            case < 1:
                {
                    switch (_是否魔晶)
                    {
                        case true:
                            {
                                if (ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0))
                                {
                                    KeyPress((uint)Keys.W);
                                    Delay(等待延迟);
                                }
                                else
                                {
                                    _全局步骤 = 1;
                                }

                                break;
                            }
                        default:
                            {
                                if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0))
                                {
                                    KeyPress((uint)Keys.W);
                                    Delay(等待延迟);
                                }
                                else
                                {
                                    _全局步骤 = 1;
                                }

                                break;
                            }
                    }

                    return await FromResult(true);
                }
            case < 2 when 根据图片以及类别使用物品(物品_血精石, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_血精石, bts, size, _技能数量))
                        _全局步骤 = 2;
                    return await FromResult(true);
                }

            case < 3 when 根据图片以及类别使用物品(物品_虚灵之刃, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_虚灵之刃, bts, size, _技能数量))
                        _全局步骤 = 3;
                    return await FromResult(true);
                }

            case < 4 when 根据图片以及类别使用物品(物品_红杖, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_红杖, bts, size, _技能数量))
                        _全局步骤 = 4;
                    return await FromResult(true);
                }

            case < 4 when 根据图片以及类别使用物品(物品_红杖2, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_红杖2, bts, size, _技能数量))
                        _全局步骤 = 4;
                    return await FromResult(true);
                }

            case < 4 when 根据图片以及类别使用物品(物品_红杖3, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_红杖3, bts, size, _技能数量))
                        _全局步骤 = 4;
                    return await FromResult(true);
                }

            case < 4 when 根据图片以及类别使用物品(物品_红杖4, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_红杖4, bts, size, _技能数量))
                        _全局步骤 = 4;
                    return await FromResult(true);
                }

            case < 4 when 根据图片以及类别使用物品(物品_红杖5, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_红杖5, bts, size, _技能数量))
                        _全局步骤 = 4;
                    return await FromResult(true);
                }

            case < 5 when 根据图片以及类别使用物品(物品_羊刀, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_羊刀, bts, size, _技能数量))
                        _全局步骤 = 5;
                    return await FromResult(true);
                }


            case < 6 when 根据图片以及类别使用物品(物品_纷争, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_纷争, bts, size, _技能数量))
                        _全局步骤 = 6;
                    return await FromResult(true);
                }

            case < 7 when 根据图片以及类别使用物品(物品_阿托斯之棍_4, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_阿托斯之棍_4, bts, size, _技能数量))
                        _全局步骤 = 7;
                    return await FromResult(true);
                }

            case < 8 when 根据图片以及类别使用物品(物品_缚灵锁_4, bts, size, _技能数量):
                {
                    Delay(等待延迟);
                    RightClick();
                    if (!根据图片以及类别使用物品(物品_缚灵锁_4, bts, size, _技能数量))
                        _全局步骤 = 8;
                    return await FromResult(true);
                }

            case < 9:
                {
                    switch (_是否魔晶)
                    {
                        case true:
                            {
                                if (ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0))
                                {
                                    KeyPress((uint)Keys.Q);
                                    Delay(等待延迟);
                                }
                                else
                                {
                                    _全局步骤 = 9;
                                }

                                break;
                            }
                        default:
                            {
                                if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0))
                                {
                                    KeyPress((uint)Keys.Q);
                                    Delay(等待延迟);
                                }
                                else
                                {
                                    _全局步骤 = 9;
                                }

                                break;
                            }
                    }

                    return await FromResult(true);
                }

            case < 10:
                {
                    switch (_是否魔晶)
                    {
                        case true:
                            {
                                if (ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0))
                                {
                                    KeyPress((uint)Keys.E);
                                    Delay(等待延迟);
                                }
                                else
                                {
                                    _全局步骤 = 10;
                                }

                                break;
                            }
                        default:
                            {
                                if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0))
                                {
                                    KeyPress((uint)Keys.E);
                                    Delay(等待延迟);
                                }
                                else
                                {
                                    _全局步骤 = 10;
                                }

                                break;
                            }
                    }

                    return await FromResult(true);
                }


            case < 11:
                {
                    switch (_是否魔晶)
                    {
                        case true:
                            {
                                if (ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0))
                                {
                                    KeyPress((uint)Keys.R);
                                    Delay(等待延迟);
                                }
                                else
                                {
                                    _全局步骤 = 11;
                                }

                                break;
                            }
                        default:
                            {
                                if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0))
                                {
                                    KeyPress((uint)Keys.R);
                                    Delay(等待延迟);
                                }
                                else
                                {
                                    _全局步骤 = 11;
                                }

                                break;
                            }
                    }

                    return await FromResult(true);
                }

            case 11:
                {
                    return await FromResult(false);
                }
        }

        return await FromResult(true);
    }

    #endregion

    #region 炸弹人

    private static async Task<bool> 粘性炸弹去后摇(byte[] bts, Size size)
    {
        static void 粘性炸弹后()
        {
            //RightClick();
            KeyPress((uint)Keys.A);
        }

        var q5 = 获取q5左下角颜色(bts, size);

        if (ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        粘性炸弹后();
        return await FromResult(false);
    }

    private static async Task<bool> 活性电击去后摇(byte[] bts, Size size)
    {
        static void 活性电击后()
        {
            //RightClick();
            KeyPress((uint)Keys.A);
        }

        var w5 = 获取w5左下角颜色(bts, size);

        if (ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        活性电击后();
        return await FromResult(false);
    }

    private static async Task<bool> 爆破起飞去后摇(byte[] bts, Size size)
    {
        static void 爆破起飞后()
        {
            //RightClick();
            KeyPress((uint)Keys.A);
            Delay(750);

            switch (_全局模式e)
            {
                case 1:
                    _条件4 = true;
                    _指定地点r = MousePosition;
                    初始化全局时间(ref _全局时间r);
                    break;
                case 0:
                    break;
            }
        }

        var e5 = 获取e5左下角颜色(bts, size);

        if (ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        爆破起飞后();
        return await FromResult(false);
    }

    private static async Task<bool> 爆破后接3雷粘性炸弹(byte[] bts, Size size)
    {
        if (获取当前时间毫秒() - _全局时间r >= 3000)
        {
            _全局时间r = -1;
            return await FromResult(false);
        }

        if (RegPicture(炸弹人_数字3, bts, size))
        {
            MouseMove(_指定地点r.X - 34, _指定地点r.Y - 130);
            KeyPress((uint)Keys.R);
            Delay(等待延迟);
            return await FromResult(true);
        }

        if (RegPicture(炸弹人_数字2, bts, size))
        {
            MouseMove(_指定地点r.X - 139 - _全局步骤r, _指定地点r.Y + 96 + _全局步骤r);
            KeyPress((uint)Keys.R);
            Delay(等待延迟);
            _全局步骤r += 3;
            return await FromResult(true);
        }

        if (RegPicture(炸弹人_数字1, bts, size))
        {
            MouseMove(_指定地点r.X + 158 + _全局步骤r, _指定地点r.Y + 31 + _全局步骤r);
            KeyPress((uint)Keys.R);
            Delay(等待延迟);
            _全局步骤r += 3;
            return await FromResult(true);
        }

        if (RegPicture(炸弹人_数字0, bts, size))
        {
            _全局步骤r = 0;
            MouseMove(_指定地点r);

            if (根据图片以及类别使用物品(物品_虚灵之刃, bts, size, _技能数量))
            {
                Delay(等待延迟);
                return await FromResult(true);
            }

            if (根据图片以及类别使用物品(物品_红杖, bts, size, _技能数量))
            {
                Delay(等待延迟);
                return await FromResult(true);
            }

            if (根据图片以及类别使用物品(物品_红杖2, bts, size, _技能数量))
            {
                Delay(等待延迟);
                return await FromResult(true);
            }

            if (根据图片以及类别使用物品(物品_红杖3, bts, size, _技能数量))
            {
                Delay(等待延迟);
                return await FromResult(true);
            }

            if (根据图片以及类别使用物品(物品_红杖4, bts, size, _技能数量))
            {
                Delay(等待延迟);
                return await FromResult(true);
            }

            if (根据图片以及类别使用物品(物品_红杖5, bts, size, _技能数量))
            {
                Delay(等待延迟);
                return await FromResult(true);
            }

            return await FromResult(false);
        }

        return await FromResult(true);
    }

    #endregion

    #region 神域

    private static async Task<bool> 命运敕令去后摇(byte[] bts, Size size)
    {
        static void 命运敕令后()
        {
            RightClick();
            // KeyPress((uint)Keys.A);
        }

        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);

        if (_是否a杖)
        {
            if (ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

            命运敕令后();
            return await FromResult(false);
        }

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        命运敕令后();
        return await FromResult(false);
    }

    private static async Task<bool> 涤罪之焰去后摇(byte[] bts, Size size)
    {
        static void 涤罪之焰后()
        {
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        var e4 = 获取e4左下角颜色(bts, size);
        var e5 = 获取e5左下角颜色(bts, size);

        if (_是否a杖)
        {
            if (ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

            涤罪之焰后();
            return await FromResult(false);
        }

        if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        涤罪之焰后();
        return await FromResult(false);
    }

    private static async Task<bool> 涤罪之焰不可释放(byte[] bts, Size size)
    {
        static async Task 涤罪之焰释放()
        {
            switch (_全局模式e)
            {
                case 0:
                    break;
                case 1:
                    _全局模式e = 0;
                    await Run(() => { KeyPress((uint)Keys.E); });
                    break;
            }
        }

        var e4 = 获取e4左下角颜色(bts, size);
        var e5 = 获取e5左下角颜色(bts, size);

        if (_是否a杖)
        {
            if (!ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
            涤罪之焰释放().Start();

            return await FromResult(false);
        }

        if (!ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        涤罪之焰释放().Start();
        return await FromResult(false);
    }

    private static async Task<bool> 虚妄之诺去后摇(byte[] bts, Size size)
    {
        static void 虚妄之诺后()
        {
            RightClick();
            // KeyPress((uint)Keys.A);
        }

        var r4 = 获取r4左下角颜色(bts, size);
        var r5 = 获取r5左下角颜色(bts, size);

        if (_是否a杖)
        {
            if (ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

            虚妄之诺后();
            return await FromResult(false);
        }

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        虚妄之诺后();
        return await FromResult(false);
    }

    private static async Task<bool> 天命之雨去后摇(byte[] bts, Size size)
    {
        static void 天命之雨后()
        {
            RightClick();
            // KeyPress((uint)Keys.A);
        }

        var d5 = 获取d5左下角颜色(bts, size);

        if (!_是否a杖) return await FromResult(false);

        if (ColorAEqualColorB(d5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        天命之雨后();
        return await FromResult(false);
    }

    #endregion

    #region 修补匠

    private static void 检测敌方英雄自动导弹()
    {
        //Task t = new(() =>
        //{
        //    if (RegPicture(血量_敌人血量, 0, 0, 1920, 1080, 0.6))
        //    {
        //        KeyPress((uint) Keys.W);
        //        Delay(40);
        //    }
        //});
        //t.Start();
        //await t;
    }

    private static void 检测希瓦()
    {
    }

    private static void 推推接刷新()
    {
        var time = 获取当前时间毫秒();
        var x_down = 0;
        while (x_down == 0)
            //if (RegPicture(物品_推推BUFF, 400, 865, 1000, 60))
            //{
            //    KeyPress((uint) Keys.R);
            //    x_down = 1;
            //}
            if (获取当前时间毫秒() - time > 500)
                break;
    }


    private static void 刷新完跳()
    {
        var all_down = 0;
        var time = 获取当前时间毫秒();
        while (all_down == 0)
            //var r_down = 0;
            //if (RegPicture(修补匠_再填装施法, 773, 727, 75, 77, 0.7))
            //{
            //    if (_条件3)
            //        await 检测希瓦();
            //    while (r_down == 0)
            //        if (!RegPicture(修补匠_再填装施法, 773, 727, 75, 77, 0.7))
            //        {
            //            r_down = 1;
            //            all_down = 1;
            //            if (_条件1)
            //                await 检测敌方英雄自动导弹();
            //            if (_条件2)
            //            {
            //                Delay(60);
            //                KeyPress((uint)Keys.Space);
            //            }
            //        }
            //}

            if (获取当前时间毫秒() - time > 700)
                break;
    }

    #endregion

    #region 莱恩

    private static async Task<bool> 莱恩羊接技能(byte[] bts, Size size)
    {
        static void 莱恩羊后()
        {
            if (_条件4)
                KeyPress((uint)Keys.E);
            else
                KeyPress((uint)Keys.A);
        }

        var w4 = 获取w4左下角颜色(bts, size);

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        莱恩羊后();
        return await FromResult(false);
    }

    private static async Task<bool> 死亡一指去后摇(byte[] bts, Size size)
    {
        static void 死亡一指后()
        {
            RightClick();
            // KeyPress((uint)Keys.A);
        }

        var r4 = 获取r4左下角颜色(bts, size);

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        死亡一指后();
        return await FromResult(false);
    }

    private static async Task<bool> 大招前纷争(byte[] bts, Size size)
    {
        if (根据图片以及类别使用物品(物品_虚灵之刃, bts, size)) Delay(等待延迟);
        if (根据图片以及类别使用物品(物品_纷争, bts, size)) Delay(等待延迟);
        if (根据图片以及类别使用物品(物品_红杖, bts, size)) Delay(等待延迟);
        if (根据图片以及类别使用物品(物品_红杖2, bts, size)) Delay(等待延迟);
        if (根据图片以及类别使用物品(物品_红杖3, bts, size)) Delay(等待延迟);
        if (根据图片以及类别使用物品(物品_红杖4, bts, size)) Delay(等待延迟);
        if (根据图片以及类别使用物品(物品_红杖5, bts, size)) Delay(等待延迟);
        return await FromResult(false);
    }

    private static async Task<bool> 推推破林肯秒羊(byte[] bts, Size size)
    {
        if (根据图片以及类别使用物品(物品_推推棒, bts, size))
        {
            Delay(等待延迟);
            return await FromResult(true);
        }

        KeyPress((uint)Keys.W);
        return await FromResult(false);
    }

    #endregion

    #region 沉默

    private static async Task<bool> 奥数诅咒去后摇(byte[] bts, Size size)
    {
        static void 奥数诅咒后(in byte[] bts, Size size)
        {
            _全局时间q = -1;
            // RightClick();
            // KeyPress((uint)Keys.A);
            switch (_全局模式q)
            {
                case < 1:
                    大招前纷争(bts, size);
                    KeyPress((uint)Keys.E);
                    break;
                case 1:
                    大招前纷争(bts, size);
                    Delay(1300);
                    KeyPress((uint)Keys.E);
                    break;
                case 2:
                    KeyPress((uint)Keys.A);
                    break;
            }
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            奥数诅咒后(bts, size);
            return await FromResult(false);
        }

        var q4 = 获取q4左下角颜色(bts, size);

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        奥数诅咒后(bts, size);
        return await FromResult(false);
    }

    private static async Task<bool> 遗言去后摇(byte[] bts, Size size)
    {
        static void 遗言后()
        {
            _全局时间e = -1;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间e > 1200 && _全局时间e != -1)
        {
            遗言后();
            return await FromResult(false);
        }

        var e4 = 获取e4左下角颜色(bts, size);

        if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        遗言后();
        return await FromResult(false);
    }

    #endregion

    #region 戴泽

    private static async Task<bool> 剧毒之触去后摇(byte[] bts, Size size)
    {
        static void 剧毒之触后()
        {
            _全局时间q = -1;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            剧毒之触后();
            return await FromResult(false);
        }

        var q5 = 获取q5左下角颜色(bts, size);

        if (ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        剧毒之触后();
        return await FromResult(false);
    }

    private static async Task<bool> 薄葬去后摇(byte[] bts, Size size)
    {
        static void 薄葬后()
        {
            _全局时间w = -1;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
        {
            薄葬后();
            return await FromResult(false);
        }

        var w5 = 获取w5左下角颜色(bts, size);

        if (ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        薄葬后();
        return await FromResult(false);
    }

    private static async Task<bool> 暗影波去后摇(byte[] bts, Size size)
    {
        static void 暗影波后()
        {
            _全局时间e = -1;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间e > 1200 && _全局时间e != -1)
        {
            暗影波后();
            return await FromResult(false);
        }

        var e5 = 获取e5左下角颜色(bts, size);

        if (ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        暗影波后();
        return await FromResult(false);
    }

    private static async Task<bool> 善咒去后摇(byte[] bts, Size size)
    {
        static void 善咒后()
        {
            _全局时间d = -1;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间d > 1200 && _全局时间d != -1)
        {
            善咒后();
            return await FromResult(false);
        }

        var d5 = 获取d5左下角颜色(bts, size);

        if (ColorAEqualColorB(d5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        善咒后();
        return await FromResult(false);
    }

    private static async Task<bool> 邪能去后摇(byte[] bts, Size size)
    {
        static void 邪能后()
        {
            _全局时间r = -1;
            // RightClick();
            KeyPress((uint)Keys.A);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
        {
            邪能后();
            return await FromResult(false);
        }

        var r5 = 获取r5左下角颜色(bts, size);

        if (ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);

        邪能后();
        return await FromResult(false);
    }

    #endregion

    #region 双头龙

    private static async Task<bool> 冰火交加去后摇(byte[] bts, Size size)
    {
        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);

        static void 冰火交加后()
        {
            _全局时间q = -1;
            // RightClick();
            KeyPress((uint)Keys.D2);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            冰火交加后();
            return await FromResult(false);
        }

        if (_是否魔晶)
        {
            if (ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
        }
        else
        {
            if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        }

        冰火交加后();
        return await FromResult(false);
    }

    private static async Task<bool> 冰封路径去后摇(byte[] bts, Size size)
    {
        var w4 = 获取w4左下角颜色(bts, size);
        var w5 = 获取w5左下角颜色(bts, size);

        static void 冰封路径后()
        {
            _全局时间w = -1;
            // RightClick();
            KeyPress((uint)Keys.D2);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
        {
            冰封路径后();
            return await FromResult(false);
        }

        if (_是否魔晶)
        {
            if (ColorAEqualColorB(w5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
        }
        else
        {
            if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        }

        冰封路径后();
        return await FromResult(false);
    }

    private static async Task<bool> 烈焰焚身去后摇(byte[] bts, Size size)
    {
        var r4 = 获取r4左下角颜色(bts, size);
        var r5 = 获取r5左下角颜色(bts, size);

        static void 烈焰焚身后()
        {
            _全局时间r = -1;
            // RightClick();
            KeyPress((uint)Keys.D2);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
        {
            烈焰焚身后();
            return await FromResult(false);
        }

        if (_是否魔晶)
        {
            if (ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
        }
        else
        {
            if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        }

        烈焰焚身后();
        return await FromResult(false);
    }

    private static async Task<bool> 吹风接冰封路径(byte[] bts, Size size)
    {
        if (根据图片以及类别使用物品(物品_吹风CD, bts, size))
        {
            Delay(等待延迟);
            return await FromResult(true);
        }

        if (!RegPicture(物品_吹风CD, bts, size) && _全局时间 == -1) 初始化全局时间(ref _全局时间);

        if (获取当前时间毫秒() - _全局时间 < 2500 - 650 - 600) return await FromResult(true);

        KeyPress((uint)Keys.W);
        _全局时间 = -1;
        return await FromResult(false);
    }

    #endregion

    #region 巫医

    private static async Task<bool> 麻痹药剂去后摇(byte[] bts, Size size)
    {
        var q4 = 获取q4左下角颜色(bts, size);
        var q5 = 获取q5左下角颜色(bts, size);

        static void 麻痹药剂后()
        {
            _全局时间q = -1;
            // RightClick();
            switch (_全局模式q)
            {
                case 1:
                    KeyPress((uint)Keys.E);
                    KeyPress((uint)Keys.D2);
                    break;
                case 0:
                    KeyPress((uint)Keys.A);
                    break;
            }
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            麻痹药剂后();
            return await FromResult(false);
        }

        if (_是否魔晶)
        {
            if (ColorAEqualColorB(q5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
        }
        else
        {
            if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        }

        麻痹药剂后();
        return await FromResult(false);
    }

    private static async Task<bool> 巫蛊咒术去后摇(byte[] bts, Size size)
    {
        var e4 = 获取e4左下角颜色(bts, size);
        var e5 = 获取e5左下角颜色(bts, size);

        static void 巫蛊咒术后(in byte[] bts1, Size size)
        {
            _全局时间q = -1;
            // RightClick();
            KeyPress((uint)Keys.A);

            根据图片以及类别使用物品(物品_魂之灵龛, bts1, size, _技能数量);
            根据图片以及类别使用物品(物品_影之灵龛, bts1, size, _技能数量);
            根据图片以及类别使用物品(物品_纷争, bts1, size, _技能数量);
        }

        // 超时则切回 总体释放时间
        if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
        {
            巫蛊咒术后(bts, size);
            return await FromResult(false);
        }

        if (_是否魔晶)
        {
            if (ColorAEqualColorB(e5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
        }
        else
        {
            if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        }

        巫蛊咒术后(bts, size);
        return await FromResult(false);
    }

    private static async Task<bool> 死亡守卫隐身(byte[] bts, Size size)
    {
        var r4 = 获取r4左下角颜色(bts, size);
        var r5 = 获取r5左下角颜色(bts, size);

        static void 死亡守卫后(in byte[] bts1, Size size)
        {
            _全局时间r = -1;

            if (!RegPicture(物品_暗影护符buff, bts1, size)) 根据图片以及类别自我使用物品(物品_暗影护符, bts1, size, _技能数量);
        }

        if (_是否魔晶)
        {
            if (ColorAEqualColorB(r5, SimpleColor.FromRgb(45, 52, 59), 0)) return await FromResult(true);
        }
        else
        {
            if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);
        }

        死亡守卫后(bts, size);
        return await FromResult(false);
    }

    #endregion

    #region 女王

    private static async Task<bool> 暗影突袭去后摇(byte[] bts, Size size)
    {
        var q4 = 获取q4左下角颜色(bts, size);

        static void 暗影突袭后()
        {
            _全局时间q = -1;
            KeyPress((uint)Keys.A);
        }

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        暗影突袭后();
        return await FromResult(false);
    }

    private static async Task<bool> 闪烁去后摇(byte[] bts, Size size)
    {
        var w4 = 获取w4左下角颜色(bts, size);

        static void 闪烁后()
        {
            _全局时间w = -1;
            RightClick();
        }

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        闪烁后();
        return await FromResult(false);
    }

    private static async Task<bool> 痛苦尖叫去后摇(byte[] bts, Size size)
    {
        var e4 = 获取e4左下角颜色(bts, size);

        static void 痛苦尖叫后()
        {
            _全局时间e = -1;
            KeyPress((uint)Keys.A);
        }

        if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        痛苦尖叫后();
        return await FromResult(false);
    }

    private static async Task<bool> 冲击波去后摇(byte[] bts, Size size)
    {
        var r4 = 获取r4左下角颜色(bts, size);

        static void 冲击波后()
        {
            _全局时间r = -1;
            KeyPress((uint)Keys.A);
        }

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        冲击波后();
        return await FromResult(false);
    }

    #endregion

    #region 干扰者

    private static async Task<bool> 风雷之击去后摇(byte[] bts, Size size)
    {
        var q4 = 获取q4左下角颜色(bts, size);

        static void 风雷之击后()
        {
            _全局时间q = -1;
            switch (_全局模式q)
            {
                case 0:
                    RightClick();
                    break;
                case 1:
                    KeyPress((uint)Keys.R);
                    break;
            }
        }

        if (ColorAEqualColorB(q4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        风雷之击后();
        return await FromResult(false);
    }

    private static async Task<bool> 静态风暴去后摇(byte[] bts, Size size)
    {
        var r4 = 获取r4左下角颜色(bts, size);

        static void 静态风暴后()
        {
            _全局时间q = -1;
            switch (_全局模式q)
            {
                case 0:
                    KeyPress((uint)Keys.Q);
                    break;
                case 1:
                    KeyPress((uint)Keys.E);
                    break;
            }
        }

        if (ColorAEqualColorB(r4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        静态风暴后();
        return await FromResult(false);
    }

    private static async Task<bool> 恶念瞥视去后摇(byte[] bts, Size size)
    {
        var w4 = 获取w4左下角颜色(bts, size);

        static void 恶念瞥视后()
        {
            _全局时间w = -1;
            RightClick();
        }

        if (ColorAEqualColorB(w4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        恶念瞥视后();
        return await FromResult(false);
    }

    private static async Task<bool> 动能力场去后摇(byte[] bts, Size size)
    {
        var e4 = 获取e4左下角颜色(bts, size);

        static void 动能力场后()
        {
            _全局时间e = -1;
            RightClick();
        }

        if (ColorAEqualColorB(e4, SimpleColor.FromRgb(65, 74, 81), 0)) return await FromResult(true);

        动能力场后();
        return await FromResult(false);
    }

    #endregion

    #endregion

    #region 通用

    #region 循环

    private static async Task 一般程序循环()
    {
        while (_总循环条件)
            if (_循环内获取图片 != null && _总开关条件)
            {
                _循环内获取图片(); // 更新全局Bitmap

                if (_中断条件) continue; // 中断则跳过循环

                if (_条件1 && _条件根据图片委托1 != null)
                    await Run(async() => { _条件1 = await _条件根据图片委托1(_全局bts, _全局size); }).ConfigureAwait(false);

                if (_条件2 && _条件根据图片委托2 != null)
                    await Run(async () => { _条件2 = await _条件根据图片委托2(_全局bts, _全局size); }).ConfigureAwait(false);

                if (_条件3 && _条件根据图片委托3 != null)
                    await Run(async () => { _条件3 = await _条件根据图片委托3(_全局bts, _全局size); }).ConfigureAwait(false);

                if (_条件4 && _条件根据图片委托4 != null)
                    await Run(async () => { _条件4 = await _条件根据图片委托4(_全局bts, _全局size); }).ConfigureAwait(false);

                if (_条件5 && _条件根据图片委托5 != null)
                    await Run(async () => { _条件5 = await _条件根据图片委托5(_全局bts, _全局size); }).ConfigureAwait(false);

                if (_条件6 && _条件根据图片委托6 != null)
                    await Run(async () => { _条件6 = await _条件根据图片委托6(_全局bts, _全局size); }).ConfigureAwait(false);

                if (_条件7 && _条件根据图片委托7 != null)
                    await Run(async () => { _条件7 = await _条件根据图片委托7(_全局bts, _全局size); }).ConfigureAwait(false);

                if (_条件8 && _条件根据图片委托8 != null)
                    await Run(async () => { _条件8 = await _条件根据图片委托8(_全局bts, _全局size); }).ConfigureAwait(false);

                switch (_条件保持假腿)
                {
                    case true when _条件开启切假腿:
                        {
                            if (_条件假腿敏捷)
                                await Run(() =>
                                {
                                    if (RegPicture(物品_假腿_敏捷腿, _全局bts, _全局size)) return;
                                    if (_切假腿中) return;
                                    _切假腿中 = true;
                                    切敏捷腿(_全局bts, _全局size, _技能数量);
                                    Run(() =>
                                    {
                                        Delay(250);
                                        _切假腿中 = false;
                                    }).ConfigureAwait(false);
                                }).ConfigureAwait(false);
                            else
                                await Run(() =>
                                {
                                    if (RegPicture(物品_假腿_力量腿, _全局bts, _全局size)) return;
                                    if (_切假腿中) return;
                                    _切假腿中 = true;
                                    切力量腿(_全局bts, _全局size, _技能数量);
                                    Run(() =>
                                    {
                                        Delay(250);
                                        _切假腿中 = false;
                                    }).ConfigureAwait(false);
                                }).ConfigureAwait(false);
                            break;
                        }
                }

                // 优化着 优化着 直接不行了 必须等待 否则直接卡死
                await Task.Delay(1);
            }
    }

    private static async Task 无物品状态初始化()
    {
        _循环内获取图片 ??= 获取图片_1;
        await 一般程序循环();
    }

    #region 取消所有功能

    private static void 取消所有功能()
    {
        _总循环条件 = false;
        _循环条件1 = false;
        _循环条件2 = false;
        _中断条件 = false;
        _丢装备条件 = false;
        _条件1 = false;
        _条件2 = false;
        _条件3 = false;
        _条件4 = false;
        _条件5 = false;
        _条件6 = false;
        _条件7 = false;
        _条件8 = false;
        _条件开启切假腿 = false;
        _条件保持假腿 = false;
        _条件假腿敏捷 = false;
        _是否魔晶 = false;
        _是否a杖 = false;
        _条件走a = false;
        _全局步骤 = 0;
        _全局步骤q = 0;
        _全局步骤w = 0;
        _全局步骤e = 0;
        _全局步骤r = 0;
        _全局步骤d = 0;
        _全局步骤f = 0;
        _全局模式 = 0;
        _全局模式q = 0;
        _全局模式w = 0;
        _全局模式e = 0;
        _全局模式r = 0;
        _全局模式d = 0;
        _全局模式f = 0;
        _全局时间 = -1;
        _全局时间q = -1;
        _全局时间w = -1;
        _全局时间e = -1;
        _全局时间r = -1;
        _全局时间d = -1;
        _全局时间f = -1;
        _攻击速度 = 0;
        _基础攻击前摇 = 0;
        _指定地点p = new Point(0, 0);
        _指定地点q = new Point(0, 0);
        _指定地点w = new Point(0, 0);
        _指定地点e = new Point(0, 0);
        _指定地点r = new Point(0, 0);
        _指定地点d = new Point(0, 0);
        _指定地点f = new Point(0, 0);
        _技能数量 = "4";

        _条件根据图片委托1 = null;
        _条件根据图片委托2 = null;
        _条件根据图片委托3 = null;
        _条件根据图片委托4 = null;
        _条件根据图片委托5 = null;
        _条件根据图片委托6 = null;
        _条件根据图片委托7 = null;
        _条件根据图片委托8 = null;
        _切假腿中 = false;
    }

    #endregion

    #region 获取图片

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private static void 获取图片_1()
    {
        // 750 856 657 217 基本所有技能状态物品，7-8ms延迟
        // 具体点则为起始坐标点加与其的差值
        _全局图像 ??= new Bitmap(截图模式1W, 截图模式1H);
        CaptureScreen(截图模式1X, 截图模式1Y, ref _全局图像);
        _全局size = new Size(截图模式1W, 截图模式1H);
        GetBitmapByte(_全局图像, ref _全局bts);
    }

    private static void 获取图片_2()
    {
        // 0 0 1920 1080 全屏，25-36ms延迟
        // 具体点则为起始坐标点加与其的差值
        _全局图像 ??= new Bitmap(1920, 1080);
        CaptureScreen(0, 0, ref _全局图像);
        _全局size = new Size(1920, 1080);
        GetBitmapByte(_全局图像, ref _全局bts);
    }

    #endregion

    #endregion

    #region 快速回城

    private static async Task<bool> 快速回城(byte[] bts, Size size)
    {
        if (RegPicture(物品_TP效果, bts, size)) return await FromResult(false);

        KeyPress((uint)Keys.T);
        Delay(等待延迟);
        KeyPress((uint)Keys.T);

        return await FromResult(true);
    }

    #endregion

    #region 泉水状态喝瓶子 已经是版本过去了

    private static void 泉水状态喝瓶()
    {
        Delay(400);

        for (var i = 1; i <= 4; i++)
        {
            KeyPress((uint)Keys.C);
            Delay(587);
        }
    }

    private static void 泉水状态喂瓶()
    {
        Delay(3000);

        var time = 获取当前时间毫秒();

        for (var i = 1; i <= 10; i++)
        {
            if (获取当前时间毫秒() - time > 1850) return;

            KeyDown((uint)Keys.LControlKey);
            KeyDown((uint)Keys.C);
            KeyUp((uint)Keys.LControlKey);
            KeyUp((uint)Keys.C);

            Delay(587);
        }
    }

    #endregion

    #region 指定地点

    private static void 指定地点()
    {
        _指定地点p = MousePosition;

        Delay(等待延迟);
        KeyDown((uint)Keys.LControlKey);
        Delay(等待延迟);
        KeyPress((uint)Keys.D9);
        Delay(等待延迟);
        KeyUp((uint)Keys.LControlKey);
    }

    #endregion

    #region 跳刀

    /// <summary>
    ///     用于快速先手无转身
    /// </summary>
    /// <returns></returns>
    private static Point 正面跳刀_无转身(in byte[] bts, Size size)
    {
        // 坐标
        var mousePosition = MousePosition;

        // X间距
        double moveX = 0;
        // Y间距，自动根据X调整
        double moveY = 0;

        var p = 快速获取自身坐标();

#if DEBUG
        TTS.Speak(string.Concat("自身坐标为:", p.X + 55, "  ", p.Y + 80));
        Delay(2000);
#endif
        double realX = p.X + 55;
        double realY = p.Y + 80;

        moveY = mousePosition.Y > realY ? -60 + mousePosition.Y : 60 + mousePosition.Y;
        moveX = mousePosition.X > realX ? -80 + mousePosition.X : 80 + mousePosition.X;


        if (Math.Abs(mousePosition.Y - realY) <= 180.0) moveY = mousePosition.Y;

        if (Math.Abs(mousePosition.X - realX) <= 180.0) moveX = mousePosition.X;

        return new Point(Convert.ToInt16(moveX), Convert.ToInt16(moveY));
    }

    #endregion

    #region 续走A

    private static void 续走A()
    {
        var 实际前摇 = _基础攻击前摇 * 100 * 1000 / _攻击速度;
        var 等待前摇 = Convert.ToInt32(实际前摇);
        var 实际间隔 = 1.7 * 100 * 1000 / _攻击速度;
        KeyPress((uint)Keys.M);
        var 等待间隔 = Convert.ToInt32(实际间隔);
        Delay(等待间隔 - 等待前摇);
        KeyPress((uint)Keys.A);
    }

    #endregion

    #region 扔装备

    private void 批量扔装备()
    {
        KeyPress((uint)Keys.S);
        Delay(40);
        KeyPress((uint)Keys.F1);
        Delay(40);
        KeyPress((uint)Keys.F1);

        using var list1 = new PooledList<string>(tb_丢装备.Text.Split(','));

        try
        {
            switch (_技能数量)
            {
                case "6":
                    foreach (var t in list1)
                        switch (t)
                        {
                            case "1":
                                扔装备(new Point(1191, 963));
                                break;
                            case "2":
                                扔装备(new Point(1259, 963));
                                break;
                            case "3":
                                扔装备(new Point(1325, 963));
                                break;
                            case "4":
                                扔装备(new Point(1191, 1011));
                                break;
                            case "5":
                                扔装备(new Point(1259, 1011));
                                break;
                            case "6":
                                扔装备(new Point(1325, 1011));
                                break;
                            case "7":
                                扔装备(new Point(1384, 994));
                                break;
                        }

                    break;
                case "4":
                    foreach (var t in list1)
                        switch (t)
                        {
                            case "1":
                                扔装备(new Point(1145, 966));
                                break;
                            case "2":
                                扔装备(new Point(1214, 963));
                                break;
                            case "3":
                                扔装备(new Point(1288, 963));
                                break;
                            case "4":
                                扔装备(new Point(1145, 1011));
                                break;
                            case "5":
                                扔装备(new Point(1214, 1011));
                                break;
                            case "6":
                                扔装备(new Point(1288, 1011));
                                break;
                            case "7":
                                扔装备(new Point(1337, 994));
                                break;
                        }

                    break;
                case "5":
                    foreach (var t in list1)
                        switch (t)
                        {
                            case "1":
                                扔装备(new Point(1160, 966));
                                break;
                            case "2":
                                扔装备(new Point(1227, 963));
                                break;
                            case "3":
                                扔装备(new Point(1295, 963));
                                break;
                            case "4":
                                扔装备(new Point(1160, 1011));
                                break;
                            case "5":
                                扔装备(new Point(1227, 1011));
                                break;
                            case "6":
                                扔装备(new Point(1295, 1011));
                                break;
                            case "7":
                                扔装备(new Point(1352, 994));
                                break;
                        }

                    break;
            }
        }
        catch (Exception)
        {
            // ignored
        }

        list1.Dispose();
    }

    private static void 扔装备(Point p)
    {
        MouseMove(p);
        LeftDown();
        Delay(40);
        MouseMove(new Point(p.X + 5, p.Y + 5));
        Delay(40);
        KeyDown((uint)Keys.Y);
        Delay(40);
        LeftUp();
        KeyUp((uint)Keys.Y);
        Delay(40);
    }

    private void 捡装备()
    {
        using var list1 = new PooledList<string>(tb_丢装备.Text.Split(','));
        KeyDown((uint)Keys.Y);
        Delay(40);
        for (var i = 0; i < list1.Count + 2; i++)
        {
            RightClick();
            Delay(100);
        }

        list1.Dispose();
        KeyUp((uint)Keys.Y);
    }

    #endregion

    #region 切假腿

    private static void 切智力腿(string mode = "4")
    {
        var 图像 = new Bitmap(截图模式1W, 截图模式1H);
        CaptureScreen(截图模式1X, 截图模式1Y, ref 图像);
        GetBitmapByte(图像, ref _全局假腿bts);
        var size = new Size(截图模式1W, 截图模式1H);

        切智力腿(_全局假腿bts, size, mode);
    }

    private static void 切敏捷腿(string mode = "4")
    {
        var 图像 = new Bitmap(截图模式1W, 截图模式1H);
        CaptureScreen(截图模式1X, 截图模式1Y, ref 图像);
        GetBitmapByte(图像, ref _全局假腿bts);
        var size = new Size(截图模式1W, 截图模式1H);

        切敏捷腿(_全局假腿bts, size, mode);
    }

    private static void 切力量腿(string mode = "4")
    {
        var 图像 = new Bitmap(截图模式1W, 截图模式1H);
        CaptureScreen(截图模式1X, 截图模式1Y, ref 图像);
        GetBitmapByte(图像, ref _全局假腿bts);
        var size = new Size(截图模式1W, 截图模式1H);

        切力量腿(_全局假腿bts, size, mode);
    }

    private static bool 切智力腿(in byte[] parByte, Size size, string mode = "4")
    {
        var 切腿成功 = 根据图片以及类别使用物品(物品_假腿_力量腿, parByte, size, mode) ||
                   根据图片以及类别使用物品多次(物品_假腿_敏捷腿, parByte, size, 2, 0, mode);
        return 切腿成功;
    }

    private static bool 切敏捷腿(in byte[] parByte, Size size, string mode = "4")
    {
        var 切腿成功 = 根据图片以及类别使用物品(物品_假腿_智力腿, parByte, size, mode) ||
                   根据图片以及类别使用物品多次(物品_假腿_力量腿, parByte, size, 2, 0, mode);
        return 切腿成功;
    }

    private static bool 切力量腿(in byte[] parByte, Size size, string mode = "4")
    {
        var 切腿成功 = 根据图片以及类别使用物品(物品_假腿_敏捷腿, parByte, size, mode) ||
                   根据图片以及类别使用物品多次(物品_假腿_智力腿, parByte, size, 2, 0, mode);
        return 切腿成功;
    }

    private static bool 切敏捷腿循环(in byte[] parByte, Size size, string mode = "4")
    {
        var 切腿成功 = 根据图片以及类别使用物品(物品_假腿_智力腿, parByte, size, mode) ||
                   根据图片以及类别使用物品(物品_假腿_力量腿, parByte, size, mode);
        return 切腿成功;
    }

    private static bool 切力量腿循环(in byte[] parByte, Size size, string mode = "4")
    {
        var 切腿成功 = 根据图片以及类别使用物品(物品_假腿_敏捷腿, parByte, size, mode) ||
                   根据图片以及类别使用物品(物品_假腿_智力腿, parByte, size, mode);
        return 切腿成功;
    }

    #endregion

    #region 切臂章

    private static void 切臂章()
    {
        KeyPress((uint)Keys.Z);
        KeyPress((uint)Keys.Z);
    }

    #endregion

    #region 分身一齐攻击

    //private void 分身一齐攻击()
    //{
    //    Delay(140);
    //    KeyDown((uint)Keys.LControlKey);
    //    KeyPress((uint)Keys.A);
    //    KeyUp((uint)Keys.LControlKey);
    //}

    #endregion

    #region 死灵射手净化

    //private static void 死灵射手净化()
    //{
    //    KeyPress((uint)Keys.D1);
    //    Delay(等待延迟);
    //    KeyPress((uint)Keys.Q);
    //    Delay(等待延迟);
    //    KeyPress((uint)Keys.F1);
    //}

    #endregion

    #region 使用物品

    private static bool 根据图片以及类别使用物品(Bitmap bp, in byte[] bts, Size size, string mode = "4", double matchRate = 0.8)
    {
        //var list = RegPicturePoint(bp, bts, size, matchRate);
        //if (list.Count <= 0) return await FromResult(false);
        //根据物品位置按键(list[0], mode);
        //list.Dispose();
        //return await FromResult(true);

        var p = RegPicturePointR(bp, bts, size);
        if (p.X + p.Y <= 0) return false;
        根据物品位置按键(p, mode);
        return true;
    }

    /// <summary>
    ///     用时4-5ms左右
    /// </summary>
    /// <param name="bp"></param>
    /// <param name="bts"></param>
    /// <param name="size"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    private static bool 根据图片以及类别自我使用物品(Bitmap bp, in byte[] bts, Size size, string mode = "4")
    {
        //var list = RegPicturePoint(bp, bts, size);
        //if (list.Count <= 0) return await FromResult(false);
        //根据物品位置按键自我(list[0], mode);
        //list.Dispose();
        //return await FromResult(true);

        var p = RegPicturePointR(bp, bts, size);
        if (p.X + p.Y <= 0) return false;
        根据物品位置按键自我(p, mode);
        return true;
    }

    private static bool 根据图片以及类别队列使用物品(Bitmap bp, in byte[] bts, Size size, string mode = "4")
    {
        //var list = RegPicturePoint(bp, bts, size);
        //if (list.Count <= 0) return await FromResult(false);
        //根据物品位置按键队列(list[0], mode);
        //list.Dispose();
        //return await FromResult(true);

        var p = RegPicturePointR(bp, bts, size);
        if (p.X + p.Y <= 0) return false;
        根据物品位置按键队列(p, mode);
        return true;
    }

    private static bool 根据图片以及类别使用物品多次(Bitmap bp, byte[] bts, Size size, int times, int delay, string mode = "4")
    {
        var p = RegPicturePointR(bp, bts, size);
        if (p.X + p.Y <= 0)
            return true;

        for (var i = 0; i < times; i++)
        {
            根据物品位置按键(p, mode);
            if (i == times - 1) break;

            Delay(delay);
        }

        return true;

        //var list = RegPicturePoint(bp, bts, size);
        //if (list.Count <= 0) return await FromResult(false);
        //for (var i = 0; i < times; i++)
        //{
        //    根据物品位置按键(list[0], mode);
        //    if (i == times - 1)
        //    {
        //        break;
        //    }
        //    Delay(delay);
        //}

        //list.Dispose();
        //return await FromResult(true);
    }

    #region 根据物品使用

    private static void 根据物品位置按键(Point p, string mode = "4")
    {
        var x = p.X + 截图模式1X;
        var y = p.Y + 截图模式1Y;

        const int 第一行物品底部y = 987;

        switch (mode)
        {
            case "4":
                if (x < 1184)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPress((uint)Keys.V);
                        break;
                    }

                    KeyPress((uint)Keys.Z);
                    break;
                }

                if (x >= 1250)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPress((uint)Keys.Space);
                        break;
                    }

                    KeyPress((uint)Keys.C);
                    break;
                }

                if (y > 第一行物品底部y)
                {
                    KeyPress((uint)Keys.B);
                    break;
                }

                KeyPress((uint)Keys.X);
                break;
            case "5":
                if (x < 1200)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPress((uint)Keys.V);
                        break;
                    }

                    KeyPress((uint)Keys.Z);
                    break;
                }

                if (x >= 1266)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPress((uint)Keys.Space);
                        break;
                    }

                    KeyPress((uint)Keys.C);
                    break;
                }

                if (y > 第一行物品底部y)
                {
                    KeyPress((uint)Keys.B);
                    break;
                }

                KeyPress((uint)Keys.X);
                break;
            case "6":
                if (x < 1228)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPress((uint)Keys.V);
                        break;
                    }

                    KeyPress((uint)Keys.Z);
                    break;
                }

                if (x >= 1294)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPress((uint)Keys.Space);
                        break;
                    }

                    KeyPress((uint)Keys.C);
                    break;
                }

                if (y > 第一行物品底部y)
                {
                    KeyPress((uint)Keys.B);
                    break;
                }

                KeyPress((uint)Keys.X);
                break;
        }
    }

    private static void 根据物品位置按键自我(Point p, string mode = "4")
    {
        var x = p.X + 截图模式1X;
        var y = p.Y + 截图模式1Y;

        const int 第一行物品底部y = 987;

        switch (mode)
        {
            case "4":
                if (x < 1184)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressAlt((uint)Keys.V);
                        break;
                    }

                    KeyPressAlt((uint)Keys.Z);
                    break;
                }

                if (x >= 1250)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressAlt((uint)Keys.Space);
                        break;
                    }

                    KeyPressAlt((uint)Keys.C);
                    break;
                }

                if (y > 第一行物品底部y)
                {
                    KeyPressAlt((uint)Keys.B);
                    break;
                }

                KeyPressAlt((uint)Keys.X);
                break;
            case "5":
                if (x < 1200)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressAlt((uint)Keys.V);
                        break;
                    }

                    KeyPressAlt((uint)Keys.Z);
                    break;
                }

                if (x >= 1266)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressAlt((uint)Keys.Space);
                        break;
                    }

                    KeyPressAlt((uint)Keys.C);
                    break;
                }

                if (y > 第一行物品底部y)
                {
                    KeyPressAlt((uint)Keys.B);
                    break;
                }

                KeyPressAlt((uint)Keys.X);
                break;
            case "6":
                if (x < 1228)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressAlt((uint)Keys.V);
                        break;
                    }

                    KeyPressAlt((uint)Keys.Z);
                    break;
                }

                if (x >= 1294)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressAlt((uint)Keys.Space);
                        break;
                    }

                    KeyPressAlt((uint)Keys.C);
                    break;
                }

                if (y > 第一行物品底部y)
                {
                    KeyPressAlt((uint)Keys.B);
                    break;
                }

                KeyPressAlt((uint)Keys.X);
                break;
        }
    }

    private static void 根据物品位置按键队列(Point p, string mode = "4")
    {
        var x = p.X + 截图模式1X;
        var y = p.Y + 截图模式1Y;

        const int 第一行物品底部y = 987;

        switch (mode)
        {
            case "4":
                if (x < 1184)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressWhile((uint)Keys.V, (uint)Keys.LShiftKey);
                        break;
                    }

                    KeyPressWhile((uint)Keys.Z, (uint)Keys.LShiftKey);
                    break;
                }

                if (x >= 1250)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressWhile((uint)Keys.Space, (uint)Keys.LShiftKey);
                        break;
                    }

                    KeyPressWhile((uint)Keys.C, (uint)Keys.LShiftKey);
                    break;
                }

                if (y > 第一行物品底部y)
                {
                    KeyPressWhile((uint)Keys.B, (uint)Keys.LShiftKey);
                    break;
                }

                KeyPressWhile((uint)Keys.X, (uint)Keys.LShiftKey);
                break;
            case "5":
                if (x < 1200)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressWhile((uint)Keys.V, (uint)Keys.LShiftKey);
                        break;
                    }

                    KeyPressWhile((uint)Keys.Z, (uint)Keys.LShiftKey);
                    break;
                }

                if (x >= 1266)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressWhile((uint)Keys.Space, (uint)Keys.LShiftKey);
                        break;
                    }

                    KeyPressWhile((uint)Keys.C, (uint)Keys.LShiftKey);
                    break;
                }

                if (y > 第一行物品底部y)
                {
                    KeyPressWhile((uint)Keys.B, (uint)Keys.LShiftKey);
                    break;
                }

                KeyPressWhile((uint)Keys.X, (uint)Keys.LShiftKey);
                break;
            case "6":
                if (x < 1228)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressWhile((uint)Keys.V, (uint)Keys.LShiftKey);
                        break;
                    }

                    KeyPressWhile((uint)Keys.Z, (uint)Keys.LShiftKey);
                    break;
                }

                if (x >= 1294)
                {
                    if (y > 第一行物品底部y)
                    {
                        KeyPressWhile((uint)Keys.Space, (uint)Keys.LShiftKey);
                        break;
                    }

                    KeyPressWhile((uint)Keys.C, (uint)Keys.LShiftKey);
                    break;
                }

                if (y > 第一行物品底部y)
                {
                    KeyPressWhile((uint)Keys.B, (uint)Keys.LShiftKey);
                    break;
                }

                KeyPressWhile((uint)Keys.X, (uint)Keys.LShiftKey);
                break;
        }
    }

    #endregion

    #endregion

    #region buff或者装备

    private static async Task<bool> 智力跳刀buff(byte[] bts, Size size)
    {
        return RegPicture(物品_跳刀_智力跳刀BUFF, bts, size) ? await FromResult(true) : await FromResult(false);
    }

    /// <summary>
    /// </summary>
    /// <param name="bts">数组</param>
    /// <param name="size">大小</param>
    /// <returns></returns>
    private static bool 阿哈利姆神杖(byte[] bts, Size size)
    {
        var 技能点颜色 = SimpleColor.FromRgb(28, 193, 254);

        if (GetSPixelBytes(bts, size, 1077 - 截图模式1X, 963 - 截图模式1Y).Equals(技能点颜色))
            return true;
        // 4技能魔晶

        return GetSPixelBytes(bts, size, 1093 - 截图模式1X, 963 - 截图模式1Y).Equals(技能点颜色) ||
               GetSPixelBytes(bts, size, 1121 - 截图模式1X, 963 - 截图模式1Y).Equals(技能点颜色);
        // 5技能A帐魔晶（A帐魔晶6技能） 6技能魔晶A
    }

    /// <summary>
    /// </summary>
    /// <param name="bp">指定图片</param>
    /// <returns></returns>
    private static bool 阿哈利姆魔晶(byte[] bts, Size size)
    {
        var x = 截图模式1X;
        var y = 截图模式1Y;

        var 技能点颜色 = SimpleColor.FromRgb(37, 181, 255);

        if (GetSPixelBytes(bts, size, 1077 - x, 996 - y).Equals(技能点颜色))
            return true;
        // 4技能魔晶

        return GetSPixelBytes(bts, size, 1093 - x, 996 - y).Equals(技能点颜色) ||
               GetSPixelBytes(bts, size, 1121 - x, 996 - y).Equals(技能点颜色);
        // 5技能A帐魔晶（A帐魔晶6技能） // 6技能魔晶A
    }


    private static void 渐隐期间放技能(uint c, int delay)
    {
        根据图片以及类别自我使用物品(物品_暗影护符, _全局bts, _全局size, _技能数量);
        Delay(delay);
        KeyPress(c);
    }

    #endregion

    #region 经过时间播报

    /// <summary>
    ///     用于播报
    /// </summary>
    /// <param name="ln">对比时间</param>
    /// <param name="delay">等待ms后播放</param>
    private static async Task 检测时间播报(long ln, int delay)
    {
        var a = 获取当前时间毫秒() - ln;

        await Run(() =>
        {
            Delay(delay);
            TTS.Speak(string.Concat("经过时间", a));
        });
    }

    #endregion

    #region 图片识别

    #region Dota2技能物品识别

    #region 初版找图

    /// <summary>
    ///     用于特定位置找图，实际不太行，主要延迟集中在截图方面
    /// </summary>
    /// <param name="bp">图片</param>
    /// <param name="position">位置</param>
    /// <param name="ablityCount">拥有技能数 4（基本技能） 5（魔晶加技能 或者船长之类的单A杖） 6（6技能基本形态） 7 其中7是6技能先出A杖没出魔晶(小松鼠之类短缺一点的) 8 是船长之类的超长6技能</param>
    /// <param name="matchRate">匹配率</param>
    /// <returns></returns>
    private static bool RegPicture(Bitmap bp, string position, int ablityCount = 4, double matchRate = 0.9)
    {
        var x = 0;
        var y = 0;
        var width = 0;
        var height = 0;

        switch (ablityCount)
        {
            case 4:
                switch (position)
                {
                    case "Q":
                        x = 798;
                        y = 943;
                        width = 58;
                        height = 64;
                        break;
                    case "W":
                        x = 864;
                        y = 943;
                        width = 58;
                        height = 64;
                        break;
                    case "E":
                        x = 930;
                        y = 943;
                        width = 58;
                        height = 64;
                        break;
                    case "R":
                        x = 994;
                        y = 943;
                        width = 58;
                        height = 64;
                        break;
                    case "Z":
                        x = 1117;
                        y = 941;
                        width = 58;
                        height = 64;
                        break;
                    case "X":
                        x = 1185;
                        y = 941;
                        width = 61;
                        height = 47;
                        break;
                    case "C":
                        x = 1250;
                        y = 941;
                        width = 61;
                        height = 47;
                        break;
                    case "V":
                        x = 1117;
                        y = 990;
                        width = 61;
                        height = 47;
                        break;
                    case "B":
                        x = 1185;
                        y = 990;
                        width = 61;
                        height = 47;
                        break;
                    case "G":
                        x = 1313;
                        y = 968;
                        width = 47;
                        height = 47;
                        break;
                    case "SPACE":
                        x = 1250;
                        y = 990;
                        width = 61;
                        height = 47;
                        break;
                }

                break;
            case 5:
                switch (position)
                {
                    case "Q":
                        x = 766;
                        y = 945;
                        width = 58;
                        height = 64;
                        break;
                    case "W":
                        x = 831;
                        y = 945;
                        width = 58;
                        height = 64;
                        break;
                    case "E":
                        x = 898;
                        y = 945;
                        width = 58;
                        height = 64;
                        break;
                    case "D":
                        x = 962;
                        y = 945;
                        width = 58;
                        height = 64;
                        break;
                    case "R":
                        x = 1026;
                        y = 945;
                        width = 58;
                        height = 64;
                        break;
                    case "Z":
                        x = 1151;
                        y = 942;
                        width = 58;
                        height = 64;
                        break;
                    case "X":
                        x = 1217;
                        y = 942;
                        width = 61;
                        height = 47;
                        break;
                    case "C":
                        x = 1283;
                        y = 942;
                        width = 61;
                        height = 47;
                        break;
                    case "V":
                        x = 1151;
                        y = 991;
                        width = 61;
                        height = 47;
                        break;
                    case "B":
                        x = 1217;
                        y = 991;
                        width = 61;
                        height = 47;
                        break;
                    case "G":
                        x = 1350;
                        y = 970;
                        width = 47;
                        height = 47;
                        break;
                    case "SPACE":
                        x = 1283;
                        y = 991;
                        width = 61;
                        height = 47;
                        break;
                }

                break;
            case 6:
                switch (position)
                {
                    case "Q":
                        x = 753;
                        y = 940;
                        width = 56;
                        height = 56;
                        break;
                    case "W":
                        x = 811;
                        y = 940;
                        width = 56;
                        height = 56;
                        break;
                    case "E":
                        x = 870;
                        y = 940;
                        width = 56;
                        height = 56;
                        break;
                    case "D":
                        x = 927;
                        y = 940;
                        width = 56;
                        height = 56;
                        break;
                    case "F":
                        x = 985;
                        y = 940;
                        width = 56;
                        height = 56;
                        break;
                    case "R":
                        x = 1043;
                        y = 940;
                        width = 56;
                        height = 56;
                        break;
                    case "Z":
                        x = 1162;
                        y = 940;
                        width = 60;
                        height = 46;
                        break;
                    case "X":
                        x = 1228;
                        y = 940;
                        width = 60;
                        height = 46;
                        break;
                    case "C":
                        x = 1294;
                        y = 940;
                        width = 60;
                        height = 46;
                        break;
                    case "V":
                        x = 1162;
                        y = 991;
                        width = 61;
                        height = 47;
                        break;
                    case "B":
                        x = 1228;
                        y = 991;
                        width = 61;
                        height = 47;
                        break;
                    case "G":
                        x = 1360;
                        y = 968;
                        width = 47;
                        height = 47;
                        break;
                    case "SPACE":
                        x = 1294;
                        y = 991;
                        width = 61;
                        height = 47;
                        break;
                }

                break;
            case 7:
                switch (position)
                {
                    case "Q":
                        x = 785;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "W":
                        x = 842;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "E":
                        x = 898;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "F":
                        x = 957;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "R":
                        x = 1015;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "Z":
                        x = 1133;
                        y = 942;
                        width = 61;
                        height = 47;
                        break;
                    case "X":
                        x = 1199;
                        y = 941;
                        width = 61;
                        height = 47;
                        break;
                    case "C":
                        x = 1265;
                        y = 942;
                        width = 61;
                        height = 47;
                        break;
                    case "V":
                        x = 1133;
                        y = 991;
                        width = 60;
                        height = 47;
                        break;
                    case "B":
                        x = 1199;
                        y = 991;
                        width = 60;
                        height = 47;
                        break;
                    case "G":
                        x = 1331;
                        y = 968;
                        width = 47;
                        height = 47;
                        break;
                    case "SPACE":
                        x = 1265;
                        y = 991;
                        width = 60;
                        height = 47;
                        break;
                }

                break;
            case 8:
                switch (position)
                {
                    case "Q":
                        x = 738;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "W":
                        x = 803;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "E":
                        x = 868;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "F":
                        x = 997;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "R":
                        x = 1061;
                        y = 940;
                        width = 57;
                        height = 57;
                        break;
                    case "Z":
                        x = 1083;
                        y = 942;
                        width = 61;
                        height = 47;
                        break;
                    case "X":
                        x = 1250;
                        y = 941;
                        width = 61;
                        height = 47;
                        break;
                    case "C":
                        x = 1316;
                        y = 942;
                        width = 61;
                        height = 47;
                        break;
                    case "V":
                        x = 1083;
                        y = 991;
                        width = 60;
                        height = 47;
                        break;
                    case "B":
                        x = 1250;
                        y = 991;
                        width = 60;
                        height = 47;
                        break;
                    case "G":
                        x = 1385;
                        y = 972;
                        width = 47;
                        height = 47;
                        break;
                    case "SPACE":
                        x = 1316;
                        y = 991;
                        width = 60;
                        height = 47;
                        break;
                }

                break;
        }

        return FindPictureParallel(bp, CaptureScreen(x, y, width, height), matchRate: matchRate).Count > 0;
    }

    #endregion

    #region 新版找图

    #region 是否存在图片

    /// <summary>
    ///     基本上只需要1-9ms不等的延迟
    /// </summary>
    /// <param name="bp">原始图片</param>
    /// <param name="bp1">对比图片</param>
    /// <param name="matchRate">匹配率</param>
    /// <returns></returns>
    private static bool RegPicture(Bitmap bp, Bitmap bp1, double matchRate = 0.8)
    {
        return FindPictureParallel(bp, new Bitmap(bp1), matchRate: matchRate).Count > 0;
    }

    /// <summary>
    ///     基本上只需要1ms左右的延迟
    /// </summary>
    /// <param name="bp"></param>
    /// <param name="bts"></param>
    /// <param name="size"></param>
    /// <param name="matchRate"></param>
    /// <returns></returns>
    private static bool RegPicture(Bitmap bp, in byte[] bts, Size size, double matchRate = 0.8)
    {
        var p = RegPicturePointR(bp, bts, size, matchRate);
        return p.X + p.Y > 0;
        //return FindBytesParallel(GetBitmapByte(bp), bp.Size, bts, size, matchRate: matchRate).Count > 0;
    }

    /// <summary>
    ///     需要图片完全相似
    /// </summary>
    /// <param name="bp"></param>
    /// <param name="bp1"></param>
    /// <param name="matchRate"></param>
    /// <returns></returns>
    private static bool RegPicture_small(Bitmap bp, Bitmap bp1, double matchRate = 0.7)
    {
        return RegPictrueSmall(bp, new Bitmap(bp1), matchRate);
    }

    //private static bool RegPicture(Bitmap bp, int x, int y, int width, int height, double matchRate = 0.8)
    //{
    //    return FindPictureParallel(bp, CaptureScreen(x, y, width, height), matchRate: matchRate).Count > 0;
    //}

    #endregion

    #region 返回图片实际坐标

    private static Point RegPicturePoint(Bitmap bp, int x, int y, int width, int height, double matchRate = 0.8)
    {
        try
        {
            var p = FindPictureParallel(bp, CaptureScreen(x, y, width, height), matchRate: matchRate)[0];
            return new Point(x + p.X, y + p.Y);
        }
        catch
        {
            // ignored
        }

        return new Point(-1, -1);
    }

    private static PooledList<Point> RegPicturePoint(Bitmap bp, in byte[] bts, Size size, double matchRate = 0.8)
    {
        try
        {
            return FindBytesParallel(GetBitmapByte(bp), bp.Size, bts, size, matchRate: matchRate);
        }
        catch
        {
            // ignored
        }

        return new PooledList<Point>();
    }

    /// <summary>
    ///     稳定后延迟在0.2ms左右，相当的快了，而且不存在并行的各种毛病
    /// </summary>
    /// <param name="bp">用来对比的图片</param>
    /// <param name="bts">大图数组</param>
    /// <param name="size">大图尺寸</param>
    /// <param name="matchRate">匹配率</param>
    /// <returns></returns>
    private static Point RegPicturePointR(Bitmap bp, in byte[] bts, Size size, double matchRate = 0.8)
    {
        try
        {
            var bts1 = GetBitmapByte(bp);
            UIntPtr binr = (nuint)bts.Length;
            UIntPtr binr1 = (nuint)bts1.Length;
            var t = FindBytesR(bts, binr, Tuple.Create((uint)截图模式1W, (uint)截图模式1H), bts1, binr1,
                Tuple.Create((uint)bp.Size.Width, (uint)bp.Size.Height), 0.8);
            return new Point((int)t.Item1, (int)t.Item2);
        }
        catch
        {
            // ignored
        }

        return new Point(0, 0);
    }

    #endregion

    #region 返回数组对应颜色

    private static Color GetPixelBytes(in byte[] bts, Size size, int x, int y)
    {
        var subIndex = y * size.Width * 3 + x * 3;
        return Color.FromArgb(bts[subIndex + 2],
            bts[subIndex + 1], bts[subIndex]);
    }

    private static SimpleColor GetSPixelBytes(in byte[] bts, Size size, int x, int y)
    {
        var subIndex = y * size.Width * 3 + x * 3;
        return SimpleColor.FromRgb(bts[subIndex + 2],
            bts[subIndex + 1], bts[subIndex]);
    }

    private static Color GetSPixelBytes(in byte[] bts, Size size, in Point p)
    {
        var subIndex = p.Y * size.Width * 3 + p.X * 3;
        return Color.FromArgb(bts[subIndex + 2],
            bts[subIndex + 1], bts[subIndex]);
    }

    #endregion

    #region 获取图片文字

    private static string 获取图片文字(byte[] bts)
    {
        return _PaddleOcrEngine.DetectText(bts).Text;
    }

    private static string 获取图片文字(Bitmap bp)
    {
        return _PaddleOcrEngine.DetectText(bp).Text;
    }

    private static string 获取图片文字(int x, int y, int width, int height)
    {
        return _PaddleOcrEngine.DetectText(CaptureScreen(x, y, width, height)).Text;
    }

    #endregion

    #endregion

    #endregion

    #endregion

    #region 记录买活

    //private static void 记录买活()
    //{
    //    var 计时颜色 = SimpleColor.FromRgb(14, 19, 24);

    //    while (true)
    //    {
    //        if (RegPicture(Resource_Picture.播报_买活, 549, 41, 52, 21) && !CaptureColor(559, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;
    //            while (!CaptureColor(559, 76).Equals(计时颜色))
    //            {
    //                MouseMove(559, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        if (RegPicture(Resource_Picture.播报_买活, 613, 41, 52, 21) && !CaptureColor(623, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;

    //            while (!CaptureColor(623, 76).Equals(计时颜色))
    //            {
    //                MouseMove(623, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        if (RegPicture(Resource_Picture.播报_买活, 674, 41, 52, 21) && !CaptureColor(688, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;

    //            while (!CaptureColor(688, 76).Equals(计时颜色))
    //            {
    //                MouseMove(688, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        if (RegPicture(Resource_Picture.播报_买活, 735, 41, 52, 21) && !CaptureColor(749, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;

    //            while (!CaptureColor(749, 76).Equals(计时颜色))
    //            {
    //                MouseMove(749, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        if (RegPicture(Resource_Picture.播报_买活, 797, 41, 52, 21) && !CaptureColor(811, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;

    //            while (!CaptureColor(811, 76).Equals(计时颜色))
    //            {
    //                MouseMove(811, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        if (RegPicture(Resource_Picture.播报_买活, 1060, 41, 52, 21) && !CaptureColor(1073, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;

    //            while (!CaptureColor(1073, 76).Equals(计时颜色))
    //            {
    //                MouseMove(1073, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        if (RegPicture(Resource_Picture.播报_买活, 1124, 41, 52, 21) && !CaptureColor(1137, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;

    //            while (!CaptureColor(1137, 76).Equals(计时颜色))
    //            {
    //                MouseMove(1137, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        if (RegPicture(Resource_Picture.播报_买活, 1185, 41, 52, 21) && !CaptureColor(1198, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;

    //            while (!CaptureColor(1198, 76).Equals(计时颜色))
    //            {
    //                MouseMove(1198, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        if (RegPicture(Resource_Picture.播报_买活, 1248, 41, 52, 21) && !CaptureColor(1261, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;

    //            while (!CaptureColor(1261, 76).Equals(计时颜色))
    //            {
    //                MouseMove(1261, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        if (RegPicture(Resource_Picture.播报_买活, 1308, 41, 52, 21) && !CaptureColor(1321, 76).Equals(计时颜色))
    //        {
    //            var p = MousePosition;

    //            while (!CaptureColor(1321, 76).Equals(计时颜色))
    //            {
    //                MouseMove(1321, 76);
    //                Thread.Sleep(30);
    //                LeftClick();
    //                Thread.Sleep(30);
    //            }

    //            MouseMove(p);
    //        }

    //        Thread.Sleep(100);
    //    }
    //}

    #endregion

    #region 记录肉山

    private static void 获取时间肉山()
    {
        快速发言("肉山");
    }

    #endregion

    #region 记录塔防

    private static void 获取时间塔防()
    {
        var str = 获取图片文字(CaptureScreen(930, 21, 58, 16));
        str = string.Concat("塔防刷新", str.Replace("：", ":"));
        Delay(500);
        快速发言(str);
    }

    #endregion

    #region 快速发言

    private static void 快速发言(string str)
    {
        Clipboard.SetText(str);
        KeyPress((uint)Keys.Enter);
        KeyDown((uint)Keys.LControlKey);
        KeyPress((uint)Keys.V);
        KeyUp((uint)Keys.LControlKey);
        Delay(等待延迟);
        KeyPress((uint)Keys.Enter);
        Delay(等待延迟);
    }

    #endregion

    #region 快速选择敌方英雄

    /// <summary>
    ///     基本需要时间 50ms 左右
    /// </summary>
    /// <param name="width"></param>
    /// <param name="hight"></param>
    /// <param name="type">模拟移动1 直接到位0</param>
    /// <param name="type1">无视野跳刀 1 有视野 0</param>
    /// <returns></returns>
    private static bool 快速选择敌方英雄(int width = 1920, int hight = 1080, int type = 0, int type1 = 0)
    {
        var p = MousePosition;
        var x = 0;
        var y = 0;

        if (width == 1920 || hight == 1080)
        {
            x = 0;
            y = 0;
        }
        else
        {
            x = p.X - width / 2 < 0 ? 0 : p.X - width / 2;
            y = p.Y - hight / 2 < 0 ? 0 : p.Y - hight / 2;
        }

        var bp = new Bitmap(width, hight);
        var size = new Size(width, hight);
        if (type1 == 1)
            Delay(300); // 基本延迟用于迷雾显示

        CaptureScreen(x, y, ref bp);
        var bytes = new byte[3 * width * hight];
        GetBitmapByte(bp, ref bytes);

        var list = 获取敌方坐标(size, bytes);

        var 偏移x = 1920;
        var 偏移y = 1080;

#if DEBUG
        foreach (var item in list)
        {
            MouseMove(item.X + x + 50, item.Y + y + 80);
            TTS.Speak(string.Concat("坐标X", item.X + x + 50, "坐标y", item.Y + y + 80));
            Delay(2000);
        }
#endif

        foreach (var item in list.Where(item =>
                     Math.Pow(item.X + x - p.X + 50, 2) + Math.Pow(item.Y + y - p.Y + 80, 2) <
                     Math.Pow(偏移x, 2) + Math.Pow(偏移y, 2)))
        {
            偏移x = item.X + x + 50 - p.X;
            偏移y = item.Y + y + 80 - p.Y;
        }

        if (list.Count > 0)
        {
            if (type == 1)
                MouseMoveSim(p.X + 偏移x, p.Y + 偏移y);
            else
                MouseMove(p.X + 偏移x, p.Y + 偏移y);
        }

        list.Dispose();

        return true;
    }

    #endregion

    #region 获取敌方坐标

    private static PooledList<Point> 获取敌方坐标(Size size, in byte[] bytes)
    {
        var colors = new PooledList<SimpleColor>();
        var points = new PooledList<Point>();

        colors.Add(SimpleColor.FromRgb(58, 28, 27));
        points.Add(new Point(0, 0));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(0, 1));

        colors.Add(SimpleColor.FromRgb(58, 28, 27));
        points.Add(new Point(99, 0));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(99, 1));

        colors.Add(SimpleColor.FromRgb(58, 28, 27));
        points.Add(new Point(100, 0));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(100, 1));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(0, 12));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(0, 13));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(99, 12));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(99, 13));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(100, 12));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(100, 13));
#if DEBUG
        初始化全局时间(ref _全局时间);
#endif

        var list1 = FindColors(colors, points, bytes, size, 1);
#if DEBUG
        检测时间播报(_全局时间, 0);
        //TTS.Speak(string.Concat("1找到",list1.Count));
        Delay(2000);
#endif
        return list1;
    }

    #endregion

    #region 快速获取自身坐标

    private static Point 快速获取自身坐标(int width = 1920, int hight = 1080)
    {
        var bp = new Bitmap(width, hight);
        var size = new Size(width, hight);

        CaptureScreen(0, 0, ref bp);
        var bytes = Array.Empty<byte>();
        GetBitmapByte(bp, ref bytes);
        return 获取自身坐标(size, bytes);
    }

    #endregion

    #region 获取自身坐标

    private static Point 获取自身坐标(Size size, in byte[] bytes)
    {
        var colors = new PooledList<SimpleColor>();
        var points = new PooledList<Point>();

        colors.Add(SimpleColor.FromRgb(35, 77, 35));
        points.Add(new Point(0, 0));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(0, 1));

        colors.Add(SimpleColor.FromRgb(33, 70, 33));
        points.Add(new Point(107, 0));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(107, 1));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(0, 18));

        colors.Add(SimpleColor.FromRgb(0, 1, 0));
        points.Add(new Point(0, 19));

        colors.Add(SimpleColor.FromRgb(0, 0, 0));
        points.Add(new Point(107, 18));

        colors.Add(SimpleColor.FromRgb(0, 1, 0));
        points.Add(new Point(107, 19));
#if DEBUG
        初始化全局时间(ref _全局时间);
#endif
        var list1 = FindColors(colors, points, bytes, size, 1);
#if DEBUG
        检测时间播报(_全局时间, 0);
        //TTS.Speak(string.Concat("1找到",list1.Count));
        Delay(2000);
#endif

        return list1.Count > 0 ? list1[0] : new Point();
    }

    #endregion

    #endregion

    #region 测试_捕捉颜色

    #region 基本不使用

    private void 测试其他功能()
    {
        var time = 获取当前时间毫秒();
        tb_y.Text = (获取当前时间毫秒() - time).ToString();
    }

    #endregion

    #region 捕捉颜色

    private void 测试方法_寻找大勋章()
    {
        var size = new Size(截图模式1W, 截图模式1H);
        var bitmap = new Bitmap(截图模式1W, 截图模式1H);
        var bts = new byte[3 * 截图模式1W * 截图模式1H];
        CaptureScreen(截图模式1X, 截图模式1Y, ref bitmap);
        GetBitmapByte(bitmap, ref bts);

        var time1 = 获取当前时间毫秒();

        //TTS.Speak(string.Concat("开始对比"));
        for (var i = 0; i < 100; i++)
        {
            var t = RegPicturePointR(物品_炎阳勋章, bts, size);
        }

        //TTS.Speak(string.Concat("找到的x坐标", t.X + 截图模式1X, "找到的y坐标", t.Y + 截图模式1Y));
        tb_攻速.Text = string.Concat(获取当前时间毫秒() - time1);
    }

    private void 捕捉颜色()
    {
        var time = 获取当前时间毫秒();

        var colors = new PooledList<SimpleColor>();
        var longs = new PooledList<long>();

        var size = new Size(截图模式1W, 截图模式1H);
        var bitmap = new Bitmap(截图模式1W, 截图模式1H);
        var bts = new byte[3 * 截图模式1W * 截图模式1H];
        // var time1 = 获取当前时间毫秒();
        CaptureScreen(截图模式1X, 截图模式1Y, ref bitmap);
        GetBitmapByte(bitmap, ref bts);
        // tb_攻速.Text = string.Concat(获取当前时间毫秒() - time1);

        while (true)
        {
            CaptureScreen(截图模式1X, 截图模式1Y, ref bitmap);
            GetBitmapByte(bitmap, ref bts);

            var q4 = 获取q4颜色(bts, size);
            var q41 = 获取q4左下角颜色(bts, size);
            var q5 = 获取q5颜色(bts, size);
            var q51 = 获取q5左下角颜色(bts, size);
            var q6 = 获取q6颜色(bts, size);
            var q61 = 获取q6左下角颜色(bts, size);

            var w4 = 获取w4颜色(bts, size);
            var w41 = 获取w4左下角颜色(bts, size);
            var w5 = 获取w5颜色(bts, size);
            var w51 = 获取w5左下角颜色(bts, size);
            var w6 = 获取w6颜色(bts, size);
            var w61 = 获取w6左下角颜色(bts, size);

            var e4 = 获取e4颜色(bts, size);
            var e41 = 获取e4左下角颜色(bts, size);
            var e5 = 获取e5颜色(bts, size);
            var e51 = 获取e5左下角颜色(bts, size);
            var e6 = 获取e6颜色(bts, size);
            var e61 = 获取e6左下角颜色(bts, size);

            var r4 = 获取r4颜色(bts, size);
            var r41 = 获取r4左下角颜色(bts, size);
            var r5 = 获取r5颜色(bts, size);
            var r51 = 获取r5左下角颜色(bts, size);
            var r6 = 获取r6颜色(bts, size);
            var r61 = 获取r6左下角颜色(bts, size);

            var d5 = 获取d5颜色(bts, size);
            var d51 = 获取d5左下角颜色(bts, size);
            var d6 = 获取d6颜色(bts, size);
            var d61 = 获取d6左下角颜色(bts, size);

            var f6 = 获取f6颜色(bts, size);
            var f61 = 获取f6左下角颜色(bts, size);

            var p = tb_丢装备.Text.Trim() switch
            {
                "q4" => q4,
                "q41" => q41,
                "q5" => q5,
                "q51" => q51,
                "q6" => q6,
                "q61" => q61,
                "w4" => w4,
                "w41" => w41,
                "w5" => w5,
                "w51" => w51,
                "w6" => w6,
                "w61" => w61,
                "e4" => e4,
                "e41" => e41,
                "e5" => e5,
                "e51" => e51,
                "e6" => e6,
                "e61" => e61,
                "r4" => r4,
                "r41" => r41,
                "r5" => r5,
                "r51" => r51,
                "r6" => r6,
                "r61" => r61,
                "d5" => d5,
                "d51" => d51,
                "d6" => d6,
                "d61" => d61,
                "f6" => f6,
                "f61" => f61,
                _ => q4
            };

            if (colors.Count == 0 || !colors[^1].Equals(p))
                if (!ColorAEqualColorB(p, SimpleColor.FromRgb(9, 10, 16), 5, 6, 12))
                {
                    colors.Add(p);
                    longs.Add(获取当前时间毫秒() - time);
                }

            //if (
            //    ColorAEqualColorB(q5, SimpleColor.FromRgb(99,170,68), 3, 5, 20)
            ////    &
            ////    !ColorAEqualColorB(q4_n, SimpleColor.FromRgb(50, 21, 23), 0) // 沉默不释放
            //    )
            //{
            //    KeyPress((uint)Keys.Q);
            //}

            if (获取当前时间毫秒() - time <= int.Parse(tb_状态抗性.Text.Trim())) continue;
            break;
        }

        tb_x.Text = colors.Aggregate("",
            (current, color) => string.Concat(current, color.R.ToString(), ",", color.G.ToString(), ",",
                color.B.ToString(), "|"));

        var str = "";
        for (var i = 1; i < longs.Count; i++) // 丢弃第一个颜色获取时间
            str = string.Concat(str, i > 0 ? (longs[i] - longs[i - 1]).ToString() : longs[0].ToString(), "|");

        tb_y.Text = str;

        colors.Dispose();
        longs.Dispose();

        TTS.Speak("完成");
    }

    #region 获取颜色

    /***
        代表	q左	q右	w左	w右	e左	e右	r左	r右
        宙斯	799	856	864	921	929	986	994	1051
        827.5		892.5		957.5		1022.5
        代表	q左	q右	w左	w右	e左	e右	d左	d右	r左	r右
        宙斯A杖	784	837	842	895	900	953	958	1011	1016	1069
        810.5		868.5		926.5		984.5		1042.5
        代表	q左	q右	w左	w右	e左	e右	d左	d右	f左	f右	r左	r右
        小松鼠魔晶A杖	754	807	812	865	870	923	928	981	986	1039	1044	1097
        780.5		838.5		896.5		954.5		1012.5		1070.5

        5、6 技能
        技能坐标 左下角 +3 边框长度

        沉默 恐惧 不能释放 颜色 吹风等其他控制依旧是原色
        25,29,32

        默认颜色 不释放 释放中 都不变色
        变回改色说明已可以释放，优先级高于白色刷新，且后续不变色
        45,52,59

        4技能
        技能坐标 左下角 +4 边框长度

        沉默 恐惧 不能释放 颜色 吹风等其他控制依旧是原色
        14,18,20

        默认颜色 不释放 释放中 都不变色
        改变颜色说明已经释放，只要释放出后颜色就会改变无关框大小
        变回改色说明已可以释放，优先级高于白色刷新，且后续不变色
        65,74,81

        逻辑上是简单而且通用，但延时方面稍微逊色
        基本延时在40-90之间
        反观直接判断技能颜色延时在22-40之间
        但检测到释放后还需要等待一点时间，且不同饰品颜色不同
        释放后CD的话，变化颜色也不统一
     ***/
    private static SimpleColor 获取q4颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 828 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取q4左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 803 - 截图模式1X, 997 - 截图模式1Y);
    }

    private static SimpleColor 获取q5颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 811 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取q5左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 787 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取q6颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 781 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取q6左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 757 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取w4颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 893 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取w4左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 868 - 截图模式1X, 997 - 截图模式1Y);
    }

    private static SimpleColor 获取w4开关颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 863 - 截图模式1X, 1003 - 截图模式1Y);
    }

    private static SimpleColor 获取w5颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 869 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取w5左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 845 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取w5开关颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 843 - 截图模式1X, 996 - 截图模式1Y);
    }

    private static SimpleColor 获取w6颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 839 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取w6左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 815 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取e4颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 958 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取e4左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 933 - 截图模式1X, 997 - 截图模式1Y);
    }

    private static SimpleColor 获取e5颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 927 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取e5左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 903 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取e6颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 897 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取e6左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 873 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取r4颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 1023 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取r4左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 998 - 截图模式1X, 997 - 截图模式1Y);
    }

    private static SimpleColor 获取r5颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 1043 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取r5左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 1019 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取r6颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 1071 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取r6左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 1047 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取d5颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 985 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取d5左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 961 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取d6颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 955 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取d6左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 931 - 截图模式1X, 994 - 截图模式1Y);
    }

    private static SimpleColor 获取f6颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 1013 - 截图模式1X, 956 - 截图模式1Y);
    }

    private static SimpleColor 获取f6左下角颜色(in byte[] bts, Size size)
    {
        return GetSPixelBytes(bts, size, 989 - 截图模式1X, 994 - 截图模式1Y);
    }

    #endregion

    #endregion

    #endregion

    #endregion

    #region 获取当前时间毫秒

    private static long 获取当前时间毫秒()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    private static void 初始化全局时间(ref long time)
    {
        time = 获取当前时间毫秒();
    }

    #endregion

    #region 其他

    #endregion

    #region 页面初始化和注销

    /// <summary>
    ///     页面初始化
    /// </summary>
    public Form2()
    {
        InitializeComponent();
        StartListen();
    }

    /// <summary>
    ///     开始监听和初始化模拟
    /// </summary>
    private int StartListen()
    {
        //// 设置线程池数量，最小要大于CPU核心数，最大不要太大就完事了，一般自动就行，手动反而影响性能
        //ThreadPool.SetMinThreads(12, 18);
        //ThreadPool.SetMaxThreads(48, 36);

        // 设置程序为HIGH程序，REALTIME循环延时
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

        var i = 1;

        //_myKeyEventHandeler = Hook_KeyDown;
        //_kHook.KeyDownEvent += _myKeyEventHandeler; // 绑定对应处理函数
        //_kHook.Start(); // 安装键盘钩子

        //_mGlobalHook.KeyDown += Hook_KeyDown;

        _hookUser.HookScope = HookUserActivity.HookScopeType.GlobalScope;
        _hookUser.ActivityForm = this;
        //hook.OnMouseActivity += Hook_OnMouseActivity;
        _hookUser.KeyDown += Hook_KeyDown;
        //hook.KeyPress += Hook_KeyPress;
        //hook.KeyUp += Hook_KeyUp; 
        _hookUser.Start(false, true);

        //WinIO32.Initialize();

        // 初始化键盘鼠标模拟，仅模仿系统函数，winIo 和 WinRing0 需要额外的操作
        i += KeyboardMouseSimulateDriverAPI.Initialize((uint)SimulateWays.Event);

        Delay(500);

        // 设置窗体显示在最上层
        i = SetWindowPos(Handle, -1, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0010 | 0x0080);

        // 设置本窗体为活动窗体
        SetActiveWindow(Handle);
        SetForegroundWindow(Handle);

        // 设置窗体置顶
        TopMost = true;

        // 338 987 只显示四行
        // 设置窗口位置
        Location = new Point(338, 987);

        if (tb_name.Text.Trim() == "测试")
        {
            Location = new Point(338, 797);
            label3.Text = "模式例:q4";
            label7.Text = "超时时间";
            tb_丢装备.Text = "q4";
            tb_状态抗性.Text = "2000";
        }


        //Task.await Run(记录买活);

        // 用于初始捕捉
        获取图片_1();

        // 用于文字识别
        初始化PaddleOcr();

        Run(() =>
        {
            _总开关条件 = !_总开关条件;
            TTS.Speak(_总开关条件 ? "开启功能" : "关闭功能");
        });

        return i;
    }

    private static void 初始化PaddleOcr()
    {
        ////自带轻量版中英文模型
        //OCRModelConfig config = null;
        //服务器中英文模型
        var config = new OCRModelConfig();
        var root = Environment.CurrentDirectory;
        var modelPathroot = root + @"\inference1";
        config.det_infer = modelPathroot + @"\ch_PP-OCRv3_det_infer";
        config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer";
        config.rec_infer = modelPathroot + @"\ch_PP-OCRv3_rec_infer";
        config.keys = modelPathroot + @"\ppocr_keys.txt";

        //var oCrParameter = new OCRParameter();

        var oCRParameter = new OCRParameter
        {
            numThread = 6, //预测并发线程数
            Enable_mkldnn = 1, //web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
            use_angle_cls = 1, //是否开启方向检测，用于检测识别180旋转
            det_db_score_mode = 1 //是否使用多段线，即文字区域是用多段线还是用矩形，
        };

        //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。  
        _PaddleOcrEngine = new PaddleOCREngine(config, oCRParameter);
    }

    /// <summary>
    ///     取消监听和注销模拟
    /// </summary>
    private void StopListen()
    {
        //_kHook.KeyDownEvent -= _myKeyEventHandeler; // 取消按键事件
        //_myKeyEventHandeler = null;
        //_kHook.Stop(); // 关闭键盘钩子

        //It is recommened to dispose it
        //_mGlobalHook.Dispose();

        _hookUser.Stop();

        // 注销按键模拟
        KeyboardMouseSimulateDriverAPI.Uninitialize();

        //WinIO32.Shutdown();

        // 释放识别功能
        _PaddleOcrEngine?.Dispose();
    }

    /// <summary>
    ///     页面关闭运行，释放
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        StopListen();
    }

    #endregion

    #region 模拟按键

    /// <summary>
    ///     单个耗时 0.8-1ms
    /// </summary>
    private static void RightClick()
    {
        //SimEnigo.Rightlick();
        KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.RightDown);
        KeyboardMouseSimulateDriverAPI.MouseUp((uint)Dota2Simulator.MouseButtons.RightUp);
    }

    private static void LeftClick()
    {
        //SimEnigo.LeftClick();
        KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.LeftDown);
        KeyboardMouseSimulateDriverAPI.MouseUp((uint)Dota2Simulator.MouseButtons.LeftUp);
    }

    private static void LeftDown()
    {
        KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.LeftDown);
    }

    private static void LeftUp()
    {
        KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.LeftUp);
    }

    //private new static void KeyUp(uint key)
    //{
    //    KeyboardMouseSimulateDriverAPI.KeyUp(key);
    //}

    //private new static void KeyDown(uint key)
    //{
    //    KeyboardMouseSimulateDriverAPI.KeyDown(key);
    //}

    ///// <summary>
    /////     单次操作大约需要7ms
    ///// </summary>
    ///// <param name="key"></param>
    //private new static void KeyPress(uint key)
    //{
    //    KeyDown(key);
    //    KeyUp(key);
    //}

    /// <summary>
    ///     单个耗时 0.8-1ms
    /// </summary>
    /// <param name="key"></param>
    private new static void KeyPress(uint key)
    {
        SimEnigo.KeyPress(key);
    }

    private new static void KeyUp(uint key)
    {
        SimEnigo.KeyUp(key);
    }

    private new static void KeyDown(uint key)
    {
        SimEnigo.KeyDown(key);
    }

    /// <summary>
    ///     优化原先逻辑，原先默认key_click 需要等待30ms，现取消
    /// </summary>
    /// <param name="key"></param>
    /// <param name="key1">(uint)Keys.LShiftKey</param>
    private static void KeyPressWhile(uint key, uint key1)
    {
        SimEnigo.KeyPressWhile(key, key1);
    }

    /// <summary>
    ///     优化原先逻辑，原先默认key_click 需要等待30ms，现取消
    /// </summary>
    /// <param name="key"></param>
    private static void KeyPressAlt(uint key)
    {
        SimEnigo.KeyPressAlt(key);
    }

    private static void ShiftKeyPress(uint key)
    {
        SimEnigo.KeyPressWhile(key, (uint)Keys.LShiftKey);
    }

    private new static void MouseMove(int x, int y, bool relative = false)
    {
        if (relative)
            SimEnigo.MouseMoveRelative(x, y);
        else
            SimEnigo.MouseMove(x, y);
        //if (relative)
        //{
        //    var p = MousePosition;
        //    X += p.X;
        //    Y += p.Y;
        //}

        //KeyboardMouseSimulateDriverAPI.MouseMove(X, Y, !relative);
    }

    private static void MouseMoveSim(int X, int Y)
    {
        var p = MousePosition;

        var def_x = (X - p.X) / 15;
        var def_y = (Y - p.Y) / 15;

        for (var i = 1; i < 15; i++)
        {
            Delay(1);
            SimEnigo.MouseMove(p.X + def_x * i, p.Y + def_y * i);
            //KeyboardMouseSimulateDriverAPI.MouseMove(p.X + def_x * i, p.Y + def_y * i, !relative);
        }

        SimEnigo.MouseMove(X, Y);
        //KeyboardMouseSimulateDriverAPI.MouseMove(X, Y, !relative);
    }

    private new static void MouseMove(Point p, bool relative = false)
    {
        var X = p.X;
        var Y = p.Y;

        if (relative)
        {
            var p1 = MousePosition;
            X += p1.X;
            Y += p1.Y;
        }

        //SimEnigo.MouseMove(X, Y);
        KeyboardMouseSimulateDriverAPI.MouseMove(X, Y, !relative);
    }

    #endregion

    #region 未使用的使用方法

    //// 初始化
    //Parameters stParameter = new Parameters();
    //stParameter.m_nCursorPositionX = 500;
    //stParameter.m_nCursorPositionY = 400;

    //stParameter.m_nMouseButtons = KeyboardMouseSimulatorDemo.MouseButtons.LeftUp;
    //stParameter.m_nMouseButtons = KeyboardMouseSimulatorDemo.MouseButtons.RightDown;
    //stParameter.m_nMouseButtons = KeyboardMouseSimulatorDemo.MouseButtons.Move;


    //// true 绝对移动 falsh 相对移动
    //KeyboardMouseSimulateDriverAPI.MouseMove(500, 500, true);

    //// 左键单击
    //KeyboardMouseSimulateDriverAPI.MouseDown((uint)KeyboardMouseSimulatorDemo.MouseButtons.LeftDown);
    //KeyboardMouseSimulateDriverAPI.MouseUp((uint)KeyboardMouseSimulatorDemo.MouseButtons.LeftUp);

    //// 右键单击
    //KeyboardMouseSimulateDriverAPI.MouseDown((uint)KeyboardMouseSimulatorDemo.MouseButtons.RightDown);
    //KeyboardMouseSimulateDriverAPI.MouseUp((uint)KeyboardMouseSimulatorDemo.MouseButtons.RightUp);

    //// 按空格键
    //KeyboardMouseSimulateDriverAPI.KeyDown((uint)Keys.Space);
    //KeyboardMouseSimulateDriverAPI.KeyUp((uint)Keys.Space);

    #endregion
}