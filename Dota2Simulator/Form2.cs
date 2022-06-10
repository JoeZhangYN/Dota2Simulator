using Dota2Simulator.KeyboardMouse;
using PaddleOCRSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Collections.Pooled;
using static Dota2Simulator.Picture_Dota2.Resource_Picture;
using static Dota2Simulator.PictureProcessing;
using static Dota2Simulator.SetWindowTop;
using static System.Threading.Tasks.Task;

namespace Dota2Simulator;

public partial class Form2 : Form
{
    /// 动态肖像 法线贴图 地面视差 主界面高画质 计算器渲染器 纹理、特效、阴影 中 渲染 70%
    private const int 截图模式1X = 750;

    private const int 截图模式1Y = 856;
    private const int 截图模式1W = 657;
    private const int 截图模式1H = 217;

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
    private void Hook_KeyDown(object sender, KeyEventArgs e)
    {
        #region 打字时屏蔽功能 (未使用)

        //if (false
        //    // && CaptureColor(572, 771).Equals(Color.FromArgb(255, 237, 222, 190))
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

                Run(洪流接x回);
                break;
            case "船长" when e.KeyCode == Keys.D3:
                label1.Text = "D3";

                Run(最大化x伤害控制);
                break;
            case "船长":
            {
                if (e.KeyCode == Keys.D4)
                {
                    label1.Text = "D4";

                    KeyPress((uint)Keys.Q);

                    Run(洪流接船);
                }

                break;
            }

            #endregion

            #region 军团

            case "军团":
            {
                if (!_总循环条件)
                {
                    _总循环条件 = true;
                    无物品状态初始化();

                    if (_条件根据图片委托1 == null)
                        _条件根据图片委托1 = 决斗;
                }

                if (e.KeyCode == Keys.E)
                {
                    _全局时间 = -1;
                    _全局步骤 = 0;
                    _中断条件 = false;
                    _条件1 = true;
                }

                if (e.KeyCode == Keys.D2)
                {
                    if (_全局模式 == 0)
                    {
                        TTS.Speak("切换无视野模式");
                        _全局模式 = 1;
                    }
                    else
                    {
                        TTS.Speak("切换有视野模式");
                        _全局模式 = 0;
                    }
                }

                else if (e.KeyCode == Keys.H)
                {
                    _中断条件 = true;
                    _条件1 = false;
                }

                break;
            }

            #endregion

            #region 斧王

            case "斧王":
            {
                if (!_总循环条件)
                {
                    _总循环条件 = true;
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 跳吼;

                _条件根据图片委托2 ??= 战斗饥渴取消后摇;

                if (e.KeyCode == Keys.E)
                {
                    _条件1 = true;
                    _中断条件 = false;
                }
                else if (e.KeyCode == Keys.W)
                {
                    _条件2 = true;
                    _中断条件 = false;
                }
                else if (e.KeyCode == Keys.S)
                {
                    _条件1 = false;
                    _条件2 = false;
                    _中断条件 = true;
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

                    Run(深渊火雨阿托斯);
                }

                break;
            }

            #endregion

            #region 哈斯卡

            case "哈斯卡" when e.KeyCode == Keys.D2:
                label1.Text = "D2";

                Run(切臂章);
                break;
            case "哈斯卡" when e.KeyCode == Keys.Q:
                label1.Text = "Q";

                Run(心炎平a);
                break;
            case "哈斯卡":
            {
                if (e.KeyCode == Keys.R)
                {
                    label1.Text = "R";

                    if (RegPicture(物品_臂章, "Z"))
                    {
                        KeyPress((uint)Keys.Z);
                        Delay(30);
                    }

                    Run(牺牲平a刃甲);
                }

                break;
            }

            #endregion

            #region 海民

            case "海民":
            {
                if (e.KeyCode == Keys.G)
                {
                    label1.Text = "G";

                    Run(跳接勋章接摔角行家);
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
                    无物品状态初始化();

                    if (_条件根据图片委托1 == null)
                        _条件根据图片委托1 = 鼻涕针刺循环;
                }

                if (!_是否魔晶) _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);

                if (!_是否a杖) _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);

                switch (e.KeyCode)
                {
                    case Keys.D2:
                    {
                        if (!_条件1)
                            _条件1 = true;

                        _循环条件1 = !_循环条件1;
                        // 基本上魂戒可以放4下，只浪费10点蓝
                        // 配合一次鼻涕就一次也不浪费
                        if (_循环条件1)
                            根据图片以及类别使用物品(物品_魂戒CD, _全局bts, _全局size);
                        break;
                    }
                    case Keys.D3:
                    {
                        if (!_条件1)
                            _条件1 = true;

                        _循环条件2 = !_循环条件2;
                        break;
                    }
                    case Keys.H:
                        _循环条件1 = false;
                        _循环条件2 = false;
                        break;
                }

                break;
            }

            #endregion

            #region 猛犸

            case "猛犸" when e.KeyCode == Keys.F:
                label1.Text = "F";

                Run(跳拱指定地点);
                break;
            case "猛犸":
            {
                if (e.KeyCode == Keys.D3)
                {
                    label1.Text = "F";

                    Run(指定地点);
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
                    无物品状态初始化();
                    _全局模式 = 0;
                }

                _条件根据图片委托1 ??= 阿托斯接钩子;

                if (!_是否魔晶)
                {
                    _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                    if (_是否魔晶) _技能数量 = "5";
                }

                switch (e.KeyCode)
                {
                    case Keys.D2:
                        _条件1 = true;
                        _中断条件 = false;
                        break;
                    case Keys.E:
                        
                        break;
                    case Keys.R:
                        
                        break;
                    case Keys.S:
                        _中断条件 = true;
                        _条件1 = false;
                        break;
                    }

                break;
            }

            #endregion

            #endregion

            #region 敏捷

            #region 露娜

            case "露娜" when e.KeyCode == Keys.Q:
                label1.Text = "Q";

                切智力腿();

                Run(月光后敏捷平a);
                break;
            case "露娜":
            {
                if (e.KeyCode == Keys.R)
                {
                    label1.Text = "R";

                    切智力腿();

                    Run(月蚀后敏捷平a);
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

            #region 敌法

            case "敌法":
            {
                if (!_总循环条件)
                {
                    _总循环条件 = true;
                    无物品状态初始化();
                    _全局模式 = 0;
                }

                _条件根据图片委托2 ??= 闪烁敏捷;

                _条件根据图片委托3 ??= 法力虚空取消后摇;

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
                        初始化全局时间(ref _全局时间q);
                        _条件2 = true;
                        break;
                    case Keys.E:
                        _条件保持假腿 = false;
                        切智力腿(_技能数量);
                        Run(() =>
                        {
                            Delay(30);
                            _条件保持假腿 = true;
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
                        _条件保持假腿 = true;
                        TTS.Speak("切敏捷");
                        break;
                }

                break;
            }

            #endregion

            #region 巨魔

            case "巨魔":
            {
                if (!_总循环条件)
                {
                    _总循环条件 = true;
                    无物品状态初始化();
                    _技能数量 = "5";
                }

                _条件根据图片委托1 ??= 巨魔远程飞斧接平a后切回;

                if (!_是否魔晶)
                {
                    _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                    _技能数量 = "6";
                }

                switch (e.KeyCode)
                {
                    case Keys.W:
                    {
                        if (_全局步骤q == 0)
                        {
                            var q5 = new Point(810 - 截图模式1X, 971 - 截图模式1Y);
                            var q6 = new Point(780 - 截图模式1X, 971 - 截图模式1Y);
                            var color = Color.FromArgb(255, 56, 80, 80); // 远程形态 颜色
                            if (_是否魔晶)
                            {
                                if (!ColorAEqualColorB(color, GetPixelBytes(_全局bts, _全局size, q6.X, q6.Y), 0))
                                {
                                    KeyPress((uint)Keys.Q);
                                }
                                else
                                {
                                    _全局步骤q = 3;
                                    _全局时间 = 获取当前时间毫秒();
                                }

                            }
                            else
                            {
                                if (!ColorAEqualColorB(color, GetPixelBytes(_全局bts, _全局size, q5.X, q5.Y), 0))
                                {
                                    KeyPress((uint)Keys.Q);
                                }
                                else
                                {
                                    _全局步骤q = 3;
                                    _全局时间 = 获取当前时间毫秒();
                                }
                            }

                            _条件1 = true;
                        }

                        break;
                    }
                    case Keys.E:
                    {
                        var q5 = new Point(810 - 截图模式1X, 971 - 截图模式1Y);
                        var q6 = new Point(780 - 截图模式1X, 971 - 截图模式1Y);
                        var color = Color.FromArgb(255, 128, 51, 12); // 近战形态 颜色
                        if (_是否魔晶)
                        {
                            if (!ColorAEqualColorB(color, GetPixelBytes(_全局bts, _全局size, q6.X, q6.Y), 0))
                            {
                                KeyPress((uint)Keys.Q);
                            }
                        }
                        else
                        {
                            if (!ColorAEqualColorB(color, GetPixelBytes(_全局bts, _全局size, q5.X, q5.Y), 0))
                            {
                                KeyPress((uint)Keys.Q);
                            }
                        }

                        break;
                    }
                    case Keys.R:
                    {
                        根据图片以及类别使用物品(物品_相位, _全局bts, _全局size);
                        根据图片以及类别使用物品(物品_否决, _全局bts, _全局size);
                        根据图片以及类别使用物品(物品_散失, _全局bts, _全局size);
                        根据图片以及类别使用物品(物品_羊刀, _全局bts, _全局size);
                        根据图片以及类别使用物品(物品_紫苑, _全局bts, _全局size);
                        根据图片以及类别使用物品(物品_血棘, _全局bts, _全局size);
                        根据图片以及类别使用物品(物品_深渊之刃, _全局bts, _全局size);
                        根据图片以及类别使用物品(物品_勇气勋章, _全局bts, _全局size);
                        根据图片以及类别使用物品(物品_炎阳勋章, _全局bts, _全局size);
                        break;
                    }
                    case Keys.D2 when _全局模式 != 1:
                        _全局模式 = 1;
                        TTS.Speak("开启切假腿");
                        break;
                    case Keys.D2:
                        _全局模式 = 0;
                        TTS.Speak("关闭切假腿");
                        break;
                }

                break;
            }

            #endregion

            #region 小骷髅

            case "小骷髅":
            {
                if (!_总循环条件)
                {
                    _总循环条件 = true;
                    无物品状态初始化();
                }

                if (e.KeyCode == Keys.Q)
                    切敏捷腿();
                else if (e.KeyCode == Keys.E)
                    切智力腿();
                else if (e.KeyCode == Keys.R)
                    切智力腿();
                break;
            }
            case "小松鼠" when e.KeyCode == Keys.D2:
                label1.Text = "D2";

                Run(捆接种树);
                break;
            case "小松鼠":
            {
                if (e.KeyCode == Keys.D3)
                {
                    label1.Text = "D3";

                    Run(飞镖接捆接种树);
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
                    无物品状态初始化();
                }

                if (_条件根据图片委托1 == null)
                    _条件根据图片委托1 = 超强力量平a;

                if (_条件根据图片委托2 == null)
                    _条件根据图片委托2 = 震撼大地接平a;


                if (e.KeyCode == Keys.W)
                {
                    切智力腿();
                    _条件1 = true;
                    _全局时间 = 获取当前时间毫秒();
                }
                else if (e.KeyCode == Keys.Q)
                {
                    切智力腿();
                    _条件2 = true;
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
                    无物品状态初始化();
                    _全局模式 = 0;
                }

                _条件根据图片委托1 ??= 黑暗契约平a;

                _条件根据图片委托2 ??= 跳水a;

                _条件根据图片委托3 ??= 深海护罩a;

                _条件根据图片委托4 ??= 跳水a;

                // TODO: 

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
                        _条件保持假腿 = true;
                        TTS.Speak("切敏捷");
                        break;
                }

                break;
            }

            #endregion

            #region 猴子

            case "猴子" when e.KeyCode == Keys.Q:
                切智力腿();
                Run(灵魂之矛敏捷);
                break;
            case "猴子" when e.KeyCode == Keys.D:
                切智力腿();
                KeyPress((uint)Keys.W);
                Run(神行百变敏捷);
                break;
            case "猴子" when e.KeyCode == Keys.F:
                RightClick();
                Run(后撤触发冲锋);
                break;

            #endregion

            #region 幻刺

            case "幻刺":
            {
                if (!_总循环条件)
                {
                    _总循环条件 = true;
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 窒息短匕敏捷;

                _条件根据图片委托2 ??= 幻影突袭敏捷;

                _条件根据图片委托3 ??= 魅影无形敏捷;

                _条件根据图片委托4 ??= 刀阵旋风敏捷;

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
                        _条件1 = true;
                        break;
                    case Keys.W:
                        _条件保持假腿 = false;
                        切智力腿(_技能数量);
                        _条件2 = true;
                        break;
                    case Keys.E:
                        _条件保持假腿 = false;
                        切智力腿(_技能数量);
                        break;
                    case Keys.D:
                        _条件保持假腿 = false;
                        切智力腿(_技能数量);
                        _条件4 = true;
                        break;
                    case Keys.D3 when _条件假腿敏捷:
                        _条件假腿敏捷 = false;
                        _条件保持假腿 = true;
                        TTS.Speak("切力量");
                        break;
                    case Keys.D3:
                        _条件假腿敏捷 = true;
                        _条件保持假腿 = true;
                        TTS.Speak("切敏捷");
                        break;
                }

                break;
            }

            #endregion

            #region 虚空

            case "虚空" when e.KeyCode == Keys.Q:
                label1.Text = "Q";
                切智力腿();
                Run(时间漫游敏捷);
                break;
            case "虚空" when e.KeyCode == Keys.W:
                label1.Text = "W";
                切智力腿();
                Run(时间膨胀敏捷);
                break;
            case "虚空":
            {
                if (e.KeyCode == Keys.R)
                {
                    label1.Text = "R";
                    切智力腿();
                    Run(时间结界敏捷);
                }

                break;
            }

            #endregion

            #region TB

            case "TB" when e.KeyCode == Keys.Q:
                label1.Text = "Q";
                切智力腿();
                Run(倒影敏捷);
                break;
            case "TB" when e.KeyCode == Keys.W:
                label1.Text = "W";
                切智力腿();
                Run(幻惑敏捷);
                break;
            case "TB" when e.KeyCode == Keys.E || e.KeyCode == Keys.F:
                label1.Text = "E";
                切智力腿();
                Run(魔化敏捷);
                break;
            case "TB" when e.KeyCode == Keys.D:
                label1.Text = "D";
                Run(恶魔狂热去后摇);
                break;
            case "TB":
            {
                if (e.KeyCode == Keys.R)
                {
                    label1.Text = "R";
                    切智力腿();
                    Run(断魂敏捷);
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
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 飞镖接平a;

                _条件根据图片委托2 ??= 标记去后摇;

                switch (e.KeyCode)
                {
                    case Keys.Q:
                        _条件1 = true;
                        break;
                    case Keys.R:
                        _条件2 = true;
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
                    无物品状态初始化();
                }

                if (_条件根据图片委托1 == null)
                    _条件根据图片委托1 = 瘴气去后摇;

                if (_条件根据图片委托2 == null)
                    _条件根据图片委托2 = 蛇棒去后摇;

                if (_条件根据图片委托3 == null)
                    _条件根据图片委托3 = 剧毒新星去后摇;

                if (_条件根据图片委托4 == null)
                    _条件根据图片委托4 = 循环蛇棒;

                if (e.KeyCode == Keys.Q)
                {
                    _中断条件 = false;
                    _条件1 = true;
                }
                else if (e.KeyCode == Keys.E)
                {
                    _中断条件 = false;
                    _条件2 = true;
                }
                else if (e.KeyCode == Keys.R)
                {
                    _中断条件 = false;
                    _条件3 = true;
                }
                else if (e.KeyCode == Keys.D3)
                {
                    _中断条件 = false;
                    _循环条件1 = !_循环条件1;
                    if (_循环条件1)
                        _条件4 = true;
                }
                else if (e.KeyCode == Keys.S)
                {
                    _中断条件 = true;
                    _条件1 = false;
                    _条件2 = false;
                    _条件3 = false;
                    _条件4 = false;
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
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 秘术异蛇去后摇;

                _条件根据图片委托2 ??= 石化凝视去后摇;

                if (!_是否魔晶)
                {
                    _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                    _技能数量 = "5";
                }

                switch (e.KeyCode)
                {
                    case Keys.W:
                        切智力腿();
                        _中断条件 = false;
                        _条件1 = true;
                        break;
                    case Keys.R:
                        切智力腿();
                        _中断条件 = false;
                        _条件2 = true;
                        break;
                    case Keys.S:
                        _中断条件 = true;
                        _条件1 = false;
                        _条件2 = false;
                        break;
                }

                break;
            }

            #endregion

            #endregion

            #region 智力

            #region 黑鸟

            case "黑鸟" when e.KeyCode == Keys.D:
                label1.Text = "D";
                _中断条件 = false;
                Run(G_yxc_y);
                // 普通砸锤
                break;
            case "黑鸟" when e.KeyCode == Keys.F:
                label1.Text = "F";
                _中断条件 = false;
                Run(G_yxc_cg);
                break;
            case "黑鸟" when e.KeyCode == Keys.D2:
                Run(关接跳);
                break;
            case "黑鸟":
            {
                if (e.KeyCode == Keys.H) _中断条件 = true;

                break;
            }

            #endregion

            #region 谜团

            case "谜团" when e.KeyCode == Keys.D:
                label1.Text = "D";
                Run(跳秒接午夜凋零黑洞);
                break;
            case "谜团":
            {
                if (e.KeyCode == Keys.F)
                {
                    label1.Text = "F";
                    Run(刷新接凋零黑洞);
                }

                break;
            }

            #endregion

            #region 冰女

            case "冰女":
            {
                if (e.KeyCode == Keys.E)
                {
                    label1.Text = "E";

                    Run(冰封禁制接陨星锤);
                }

                break;
            }

            #endregion

            #region 火女

            case "火女":
            {
                if (!_总循环条件)
                {
                    _总循环条件 = true;
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 龙破斩去后摇;

                _条件根据图片委托2 ??= 光击阵去后摇;

                _条件根据图片委托3 ??= 神灭斩去后摇;

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
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 拉接平A;

                _条件根据图片委托2 ??= 滚接平A;

                _条件根据图片委托3 ??= 快速回城;

                switch (e.KeyCode)
                {
                    case Keys.Q:
                        label1.Text = "Q";
                        Run(残影接平A);
                        break;
                    case Keys.W:
                        _中断条件 = false;
                        _条件1 = true;
                        break;
                    case Keys.R:
                        _中断条件 = false;
                        _条件2 = true;
                        break;
                    case Keys.D4:
                        _中断条件 = false;
                        _条件3 = true;
                        break;
                    case Keys.D2:
                        _中断条件 = false;
                        break;
                    case Keys.S:
                        _中断条件 = true;
                        _条件1 = false;
                        _条件2 = false;
                        _条件3 = false;
                        _条件4 = false;
                        break;
                    //else if (e.KeyCode == Keys.F)
                    //{
                    //    label1.Text = "F";
                    //    Task.Run(原地滚A);
                    //}
                    case Keys.D:
                        Run(泉水状态喝瓶);
                        break;
                    case Keys.F when !_丢装备条件:
                        Run(批量扔装备);
                        _丢装备条件 = !_丢装备条件;
                        break;
                    case Keys.F:
                        Run(捡装备);
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
                    无物品状态初始化();
                }

                if (!_是否魔晶) _是否魔晶 = 阿哈利姆魔晶(_全局bts, _全局size);
                if (!_是否a杖) _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);

                _条件根据图片委托1 ??= 弧形闪电去后摇;

                _条件根据图片委托2 ??= 雷击去后摇;

                _条件根据图片委托3 ??= 弧形闪电不能释放;

                switch (e.KeyCode)
                {
                    // 弧形闪电和雷击都是不朽
                    case Keys.Q when 弧形闪电不能释放(_全局bts, _全局size):
                        _全局模式q = 1;
                        _条件3 = true;
                        break;
                    case Keys.Q:
                        _条件1 = true;
                        break;
                    case Keys.W:
                        _条件2 = true;
                        break;
                }

                break;
            }

            #endregion

            #region 卡尔

            case "卡尔" when e.KeyCode == Keys.D2:
                label1.Text = "D2";

                Run(三冰对线);
                break;
            case "卡尔" when e.KeyCode == Keys.D3:
                label1.Text = "D2";

                Run(三火平A);
                break;
            case "卡尔" when e.KeyCode == Keys.D1:
                label1.Text = "D2";

                Run(三雷幽灵);
                break;
            case "卡尔":
            {
                if (e.KeyCode == Keys.D4)
                {
                    label1.Text = "D2";

                    Run(吹风天火);
                }

                break;
            }

            #endregion

            #region 拉西克

            case "拉席克" when e.KeyCode == Keys.F:
                label1.Text = "F";

                _中断条件 = false;

                Run(吹风接撕裂大地);
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
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 苍穹振击取消后摇;

                _条件根据图片委托2 ??= 变羊取消后摇;

                _条件根据图片委托3 ??= 释放群蛇守卫取消后摇;

                switch (e.KeyCode)
                {
                    case Keys.W:
                        _中断条件 = false;
                        _条件2 = true;
                        break;
                    case Keys.Q:
                        _中断条件 = false;
                        _条件1 = true;
                        break;
                    case Keys.R:
                        _中断条件 = false;
                        _条件3 = true;
                        break;
                    case Keys.D2:
                        推推破林肯秒羊(_全局bts, _全局size);
                        KeyPress((uint)Keys.W);
                        break;
                    case Keys.D3:
                        Run(() =>
                        {
                            Run(() => { 渐隐期间放技能((uint)Keys.E, 800); });
                            if (_全局模式 != 1) return;
                            Delay(650);
                            var p = MousePosition;
                            MouseMove(_指定地点p);
                            KeyPress((uint)Keys.Space);
                            Delay(30);
                            MouseMove(p);
                            _全局模式 = 0;
                        });
                        break;
                    case Keys.D4:
                        Run(() =>
                        {
                            KeyPressAlt((uint) Keys.Z);
                        });
                        break;
                    case Keys.D5:
                        Run(() =>
                        {
                            _指定地点p = MousePosition;
                            _全局模式 = 1;
                        });
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

            #region 小仙女

            case "小仙女":
            {
                switch (e.KeyCode)
                {
                    case Keys.D2:
                        label1.Text = "D2";

                        _循环条件2 = true;

                        Run(诅咒皇冠吹风);
                        break;
                    case Keys.D9:
                        label1.Text = "D3";

                        _循环条件2 = true;

                        Run(作祟暗影之境最大化伤害);
                        break;
                    case Keys.S:
                        _循环条件2 = false;
                        break;
                    case Keys.E:
                        Run(皇冠延时计时);
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
                    无物品状态初始化();
                }

                _条件根据图片委托2 ??= 天怒秒人连招;

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
                    //else if (e.KeyCode == Keys.D2)
                    //    条件3 = true;
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
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 爆破后接3雷粘性炸弹;

                switch (e.KeyCode)
                {
                    case Keys.D2:
                    {
                        _中断条件 = false;
                        _条件1 = true;

                        if (RegPicture(物品_纷争, _全局bts, _全局size)) KeyPress((uint)Keys.Z);
                        if (RegPicture(物品_魂戒CD, _全局bts, _全局size)) KeyPress((uint)Keys.X);
                        KeyPress((uint)Keys.E);
                        break;
                    }
                    case Keys.S:
                        _中断条件 = true;
                        _条件1 = false;
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
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 命运敕令去后摇;

                _条件根据图片委托2 ??= 涤罪之焰去后摇;

                _条件根据图片委托3 ??= 虚妄之诺去后摇;

                if (!_是否魔晶)
                {
                    _是否a杖 = 阿哈利姆神杖(_全局bts, _全局size);
                    if (_是否a杖) _技能数量 = "5";
                }

                switch (e.KeyCode)
                {
                    case Keys.W:
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

            #region 修补匠

            case "修补匠" when e.KeyCode == Keys.R:
                KeyPress((uint)Keys.C);
                KeyPress((uint)Keys.V);
                Run(刷新完跳);
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
                Run(推推接刷新);
                break;
            case "修补匠":
            {
                if (e.KeyCode == Keys.D1) Run(检测敌方英雄自动导弹);

                break;
            }

            #endregion

            #region 莱恩

            case "莱恩":
            {
                if (!_总循环条件)
                {
                    _总循环条件 = true;
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 莱恩羊接技能;


                switch (e.KeyCode)
                {
                    case Keys.D3 when !_条件4:
                        _条件4 = true;
                        TTS.Speak("开启羊接吸");
                        break;
                    case Keys.D3:
                        _条件4 = false;
                        TTS.Speak("开启羊接A");
                        break;
                    case Keys.W:
                        _条件1 = true;
                        break;
                    case Keys.S:
                        _条件1 = false;
                        break;
                    case Keys.R:
                        大招前纷争(_全局bts, _全局size);
                        break;
                    case Keys.D2:
                        推推破林肯秒羊(_全局bts, _全局size);
                        KeyPress((uint)Keys.W);
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
                    无物品状态初始化();
                }

                _条件根据图片委托1 ??= 奥数诅咒去后摇;

                _条件根据图片委托2 ??= 遗言去后摇;


                switch (e.KeyCode)
                {
                    case Keys.Q:
                        _中断条件 = false;
                        _条件1 = true;
                        break;
                    case Keys.E:
                        大招前纷争(_全局bts, _全局size);
                        _中断条件 = false;
                        _条件2 = true;
                        break;
                    case Keys.S:
                        _中断条件 = true;
                        _条件1 = false;
                        _条件2 = false;
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
                //if (e.KeyCode == Keys.E)
                //{
                //    tb_状态抗性.Text = "";
                //    tb_丢装备.Text = "";
                //    测试文字识别();
                //}
                break;
            }

            #endregion
        }
    }

    #endregion

    #region 局部全局变量

    #region 循环用到

    /// <summary>
    ///     循环计数total
    /// </summary>
    private static bool _总循环条件;

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
    private static byte[] _全局bts = new byte[4 * 截图模式1W * 截图模式1H];

    /// <summary>
    ///     全局假腿bytes
    /// </summary>
    private static byte[] _全局假腿bts = new byte[4 * 截图模式1W * 截图模式1H];

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
    private delegate bool ConditionDelegateBitmap(in byte[] bytes, Size size);

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
    private static bool _条件保持假腿;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private static bool _条件假腿敏捷;

    /// <summary>
    ///     技能数量
    /// </summary>
    private static string _技能数量 = "4";

    /// <summary>
    ///     中断条件布尔
    /// </summary>
    private static bool _中断条件;

    #endregion

    #region 其他
    /// <summary>
    ///     用于捕获按键
    /// </summary>
    private readonly KeyboardHook _kHook = new();

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

    /// <summary>
    ///     按键钩子，用于捕获按下的键
    /// </summary>
    private KeyEventHandler _myKeyEventHandeler; //按键钩子

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

    private static void 跳拱指定地点()
    {
        KeyPress((uint)Keys.Space);
        Delay(30);
        KeyPress((uint)Keys.D9);
        MouseMove(_指定地点p);
        Delay(30);
        KeyPress((uint)Keys.E);
        Delay(30);
        KeyPress((uint)Keys.D9);
    }

    #endregion

    #region 船长

    private static void 洪流接x回()
    {
        var time = 获取当前时间毫秒();

        var w_down = 0;
        while (w_down == 0)
        {
            if (RegPicture(船长_释放洪流_4, "Q") || RegPicture(船长_释放洪流_5, "Q", 5)
                                           || RegPicture(船长_释放洪流_8, "Q", 8))
            {
                Delay(1200);
                KeyPress((uint)Keys.E);
                w_down = 1;
            }

            if (获取当前时间毫秒() - time > 1300) break;
        }
    }

    private static void 洪流接船()
    {
        var time = 获取当前时间毫秒();

        var w_down = 0;
        while (w_down == 0)
        {
            if (RegPicture(船长_释放洪流_4, "Q") || RegPicture(船长_释放洪流_5, "Q", 5)
                                           || RegPicture(船长_释放洪流_8, "Q", 8))
            {
                Delay(45);
                KeyPress((uint)Keys.R);
                w_down = 1;
            }

            if (获取当前时间毫秒() - time > 1300) break;
        }
    }

    private void 最大化x伤害控制()
    {
        KeyPress((uint)Keys.E);

        var all_down = 0;
        while (all_down == 0)
            if (RegPicture(船长_释放x标记_4, "E") || RegPicture(船长_释放x标记_5, "E", 5)
                                            || RegPicture(船长_释放x标记_8, "E", 8))
            {
                var kx = Convert.ToDouble(tb_状态抗性.Text.Trim());

                var x_持续时间 = Convert.ToInt32(4000 * (100 - kx) / 100);

                if (RegPicture(物品_陨星锤, "SPACE"))
                {
                    if (x_持续时间 >= 3000)
                        Delay(x_持续时间 - 3000 + 60);
                    // 增加延时，因为时间对不上。。

                    KeyPress((uint)Keys.Space);

                    等待陨星锤结束();

                    KeyPress((uint)Keys.D4);
                }
                else
                {
                    if (x_持续时间 >= 2000)
                        Delay(x_持续时间 - 2000 + 60);
                    // 增加延时，因为时间对不上。。

                    KeyPress((uint)Keys.D4);
                }

                all_down = 1;
            }
    }

    #endregion

    #region 斧王

    private static bool 跳吼(in byte[] bts, Size size)
    {
        if (RegPicture(物品_刃甲, bts, size))
            KeyPress((uint)Keys.Z);
        //Delay(30);

        KeyPress((uint)Keys.Space);

        var s_time = 获取当前时间毫秒();
        if (!RegPicture(斧王_狂战士之吼, bts, size) &&
            !RegPicture(斧王_狂战士之吼_金色饰品, bts, size)) return true;

        KeyPress((uint)Keys.Q);
        Delay(550, s_time);
        //RightClick();
        KeyPress((uint)Keys.A);
        return false;
    }

    private static bool 战斗饥渴取消后摇(in byte[] bts, Size size)
    {
        var s_time = 获取当前时间毫秒();

        // 检测刚释放完毕
        if (!RegPicture(斧王_释放战斗饥渴_不朽, bts, size)) return true;
        Delay(290, s_time);
        //RightClick();
        KeyPress((uint)Keys.A);
        return false;
    }

    #endregion

    #region 军团

    private static bool 决斗(in byte[] bts, Size size)
    {
        switch (_全局步骤)
        {
            case < 1:
            {
                _全局步骤 = 根据图片以及类别使用物品(物品_臂章, bts, size) ? 1 : 0;

                根据图片以及类别使用物品(物品_魂戒CD, bts, size);

                if (RegPicture(军团_强攻CD, bts, size))
                {
                    KeyPressAlt((uint)Keys.W);
                    Delay(260); // 去后摇
                    RightClick();
                    Delay(30);
                }

                break;
            }
            case < 2 when 根据图片以及类别使用物品(物品_刃甲, bts, size):
                return true;
            case < 2:
            {
                if (根据图片以及类别使用物品(物品_跳刀, bts, size)) _全局步骤 = 2;

                else if (根据图片以及类别使用物品(物品_跳刀_力量跳刀, bts, size)) _全局步骤 = 2;

                else if (根据图片以及类别使用物品(物品_跳刀_力量跳刀, bts, size)) _全局步骤 = 2;

                else if (根据图片以及类别使用物品(物品_跳刀_智力跳刀, bts, size)) _全局步骤 = 2;

                return true;
            }
            case < 3:
            {
                if (_全局模式 == 1)
                {
                    Delay(500);
                    快速选择敌方英雄();
                }

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

            return true;
        }

        if (_全局步骤 < 5)
        {
            if (_全局时间 < 0)
                _全局时间 = 获取当前时间毫秒();

            if (RegPicture(军团_决斗CD, bts, size))
            {
                KeyPress((uint)Keys.R);
                Delay(30);
                return 获取当前时间毫秒() - _全局时间 <= 450;
            }

            _全局步骤 = 5;
            return false;
        }

        return false;
    }

    #endregion

    #region 孽主

    private static void 深渊火雨阿托斯()
    {
        var all_done = 0;

        KeyPress((uint)Keys.W);

        while (all_done == 0)
            if (RegPicture(孽主_释放深渊, 857, 939, 70, 72))
            {
                Delay(400);
                KeyPress((uint)Keys.A);

                KeyPress((uint)Keys.Q);

                Delay(640);

                KeyPress((uint)Keys.A);
                Delay(800);

                KeyPress((uint)Keys.X);

                all_done = 1;
            }
    }

    #endregion

    #region 哈斯卡

    private void 心炎平a()
    {
        KeyPress((uint)Keys.A);

        var time = 获取当前时间毫秒();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(哈斯卡_释放心炎, "Q"))
            {
                Delay(110);

                KeyPress((uint)Keys.A);

                q_down = 1;

                label1.Text = "QQQ";
            }

            if (获取当前时间毫秒() - time <= 800) continue;

            break;
        }
    }

    private void 牺牲平a刃甲()
    {
        var time = 获取当前时间毫秒();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(哈斯卡_释放牺牲, "R"))
            {
                Delay(400);

                while (RegPicture(物品_刃甲, "X"))
                {
                    KeyPress((uint)Keys.X);
                    Delay(30);
                }

                q_down = 1;

                label1.Text = "RRR";
            }

            if (获取当前时间毫秒() - time <= 900) continue;

            break;
        }
    }

    #endregion

    #region 海民

    private void 跳接勋章接摔角行家()
    {
        if (RegPicture(物品_臂章, "Z"))
        {
            KeyPress((uint)Keys.Z);
            Delay(30);
        }

        var p = 正面跳刀_无转身();

        var point = MousePosition;

        MouseMove(p.X, p.Y);

        // 跳刀空格
        KeyPress((uint)Keys.Space);

        Delay(5);

        MouseMove(point.X, point.Y);

        // 勋章放C
        KeyPress((uint)Keys.C);
        KeyPress((uint)Keys.E);
        KeyPress((uint)Keys.A);
    }

    #endregion

    #region 钢背

    private static bool 鼻涕针刺循环(in byte[] bts, Size size)
    {
        var x = 截图模式1X;
        var y = 截图模式1Y;
        var q4 = GetPixelBytes(bts, size, 829 - x, 972 - y);
        var q4_n = GetPixelBytes(bts, size, 818 - x, 970 - y);
        var w4 = GetPixelBytes(bts, size, 871 - x, 994 - y);
        var w4_n = GetPixelBytes(bts, size, 883 - x, 977 - y);

        var q5 = GetPixelBytes(bts, size, 812 - x, 971 - y);
        var q5_n = GetPixelBytes(bts, size, 802 - x, 967 - y);
        var w5 = GetPixelBytes(bts, size, 869 - x, 971 - y);
        var w5_n = GetPixelBytes(bts, size, 859 - x, 968 - y);


        if (_循环条件1)
        {
            if (_是否魔晶)
            {
                if (
                    ColorAEqualColorB(w5, Color.FromArgb(255, 98, 79, 64), 10, 16, 23)
                    &
                    !ColorAEqualColorB(w5_n, Color.FromArgb(41, 41, 45), 0) // 沉默不释放
                )
                {
                    KeyPress((uint)Keys.W);
                    Delay(30);
                }

                else if (RegPicture(钢背_针刺CD_5, bts, size))
                {
                    KeyPress((uint)Keys.W);
                    Delay(30);
                }
            }
            else
            {
                if (
                    ColorAEqualColorB(w4, Color.FromArgb(255, 77, 49, 38), 3, 9, 12)
                    &
                    !ColorAEqualColorB(w4_n, Color.FromArgb(47, 20, 22), 0) // 沉默不释放
                )
                {
                    KeyPress((uint)Keys.W);
                    Delay(30);
                }
                else if (RegPicture(钢背_针刺CD, bts, size))
                {
                    KeyPress((uint)Keys.W);
                    Delay(30);
                }
            }
        }

        if (!_是否a杖 || !_循环条件2) return true;

        if (_是否魔晶)
        {
            if (
                ColorAEqualColorB(q5, Color.FromArgb(255, 58, 54, 13), 1, 0, 0) // 不朽颜色变化
                &
                !ColorAEqualColorB(q5_n, Color.FromArgb(52, 21, 23), 0)
                ||
                ColorAEqualColorB(q5, Color.FromArgb(255, 133, 157, 44), 3, 5, 20)
            )
            {
                KeyPress((uint)Keys.Q);
                Delay(30);
            }
            else if (RegPicture(钢背_鼻涕CD_5_不朽, bts, size))
            {
                KeyPress((uint)Keys.Q);
                Delay(30);
            }
        }
        else
        {
            if (
                ColorAEqualColorB(q4, Color.FromArgb(255, 56, 53, 17), 1, 1, 4) // 不朽
                &
                !ColorAEqualColorB(q4_n, Color.FromArgb(50, 21, 23), 0)
                ||
                ColorAEqualColorB(q4, Color.FromArgb(255, 124, 143, 44), 2, 3, 15)
            )
            {
                KeyPress((uint)Keys.Q);
                Delay(30);
            }
            else if (RegPicture(钢背_鼻涕CD_不朽, bts, size))
            {
                KeyPress((uint)Keys.Q);
                Delay(30);
            }
        }

        return true;
    }

    #endregion

    #region 屠夫
    private static bool 阿托斯接钩子(in byte[] bts, Size size)
    {
        var time = 获取当前时间毫秒();

        根据图片以及类别使用物品(物品_阿托斯之棍_4, bts, size, _技能数量);

        根据图片以及类别使用物品(物品_纷争, bts, size, _技能数量);

        var 是否以太 = RegPicture(物品_以太, bts, size);

        while (获取当前时间毫秒() - time < (是否以太 ? 420 :300))
        {
        }

        KeyPress((uint)Keys.Q);

        KeyPressWhile((uint)Keys.R,(uint)Keys.LShiftKey);

        Delay(100);

        KeyPressWhile((uint)Keys.E, (uint)Keys.LShiftKey);

        return false;
    }
    #endregion


    #endregion

    #region 敏捷

    #region 露娜

    private static void 月光后敏捷平a()
    {
        var time = 获取当前时间毫秒();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(露娜_释放月光, "Q"))
            {
                Delay(100);
                切敏捷腿();
                KeyPress((uint)Keys.A);

                q_down = 1;
            }

            if (获取当前时间毫秒() - time <= 1100) continue;

            切敏捷腿();

            break;
        }
    }

    private static void 月蚀后敏捷平a()
    {
        var time = 获取当前时间毫秒();

        var r_down = 0;

        while (r_down == 0)
        {
            if (RegPicture(露娜_释放月蚀, "R"))
            {
                Delay(100);
                切敏捷腿();
                KeyPress((uint)Keys.A);

                r_down = 1;
            }

            if (获取当前时间毫秒() - time <= 1100) continue;

            切敏捷腿();
            break;
        }
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
    //        Delay(30);
    //        if (获取当前时间毫秒() - time > 100) break;
    //    }

    //    time = 获取当前时间毫秒();

    //    while (RegPicture(Resource_Picture.物品_勇气, "Z") || RegPicture(Resource_Picture.物品_炎阳, "Z") ||
    //           RegPicture(Resource_Picture.物品_紫苑, "Z") || RegPicture(Resource_Picture.物品_血棘, "Z") ||
    //           RegPicture(Resource_Picture.物品_羊刀, "Z"))
    //    {
    //        KeyPress((uint)Keys.Z);
    //        Delay(30);
    //        if (获取当前时间毫秒() - time > 100) break;
    //    }

    //    time = 获取当前时间毫秒();

    //    while (RegPicture(Resource_Picture.物品_勇气, "SPACE") || RegPicture(Resource_Picture.物品_炎阳, "SPACE") ||
    //           RegPicture(Resource_Picture.物品_紫苑, "SPACE") || RegPicture(Resource_Picture.物品_血棘, "SPACE") ||
    //           RegPicture(Resource_Picture.物品_羊刀, "SPACE"))
    //    {
    //        KeyPress((uint)Keys.Space);
    //        Delay(30);
    //        if (获取当前时间毫秒() - time > 100) break;
    //    }

    //    time = 获取当前时间毫秒();

    //    // 否决放在x
    //    while (RegPicture(Resource_Picture.物品_否决, "X"))
    //    {
    //        KeyPress((uint)Keys.X);
    //        Delay(30);
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
        var time = 获取当前时间毫秒();

        if (RegPicture(物品_纷争, _全局bts, _全局size))
            ShiftKeyPress((uint)Keys.C);

        ShiftKeyPress((uint)Keys.Q);

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(小松鼠_释放爆栗出击, _全局bts, _全局size) ||
                RegPicture(小松鼠_释放野地奇袭_7, _全局bts, _全局size))
            {
                Delay(85);
                ShiftKeyPress((uint)Keys.W);
                q_down = 1;
            }

            if (获取当前时间毫秒() - time > 3600) break;
        }
    }

    private static void 飞镖接捆接种树()
    {
        var time = 获取当前时间毫秒();

        if (RegPicture(物品_纷争, "C"))
            KeyPress((uint)Keys.C);

        var f_down = false;
        var w_down = false;

        KeyPress((uint)Keys.F);

        while (!f_down)
        {
            if (RegPicture(小松鼠_猎手旋标, "F", 7))
            {
                Delay(107);
                KeyPress((uint)Keys.W);
                f_down = true;
            }

            if (获取当前时间毫秒() - time > 3600) break;
        }

        while (!w_down)
        {
            if (RegPicture(小松鼠_释放野地奇袭_7, "W", 7))
            {
                Delay(85);
                KeyPress((uint)Keys.Q);
                w_down = true;
            }

            if (获取当前时间毫秒() - time > 1000) break;
        }
    }

    #endregion

    #region 拍拍

    private static bool 超强力量平a(in byte[] bts, Size size)
    {
        if (RegPicture(拍拍_释放超强力量, bts, size))
        {
            Run(() =>
            {
                KeyPress((uint)Keys.A);
                Delay(120);
                切敏捷腿();
                KeyPress((uint)Keys.A);
                Delay(120);
                切敏捷腿();
            });

            return false;
        }

        if (获取当前时间毫秒() - _全局时间 <= 700 || _全局时间 == -1) return true;

        KeyPress((uint)Keys.A);
        切敏捷腿();
        _全局时间 = -1;
        return false;
    }

    private static bool 震撼大地接平a(in byte[] bts, Size size)
    {
        Run(() =>
        {
            KeyPress((uint)Keys.A);
            Delay(200);
            切敏捷腿();
        });
        return false;
    }

    #endregion

    #region 巨魔

    private static bool 巨魔远程飞斧接平a后切回(in byte[] bts, Size size)
    {
        var q5 = new Point(810 - 截图模式1X, 971 - 截图模式1Y);
        var q6 = new Point(780 - 截图模式1X, 971 - 截图模式1Y);
        var w5 = new Point(868 - 截图模式1X, 971 - 截图模式1Y);
        var w6 = new Point(838 - 截图模式1X, 971 - 截图模式1Y);

        var 远程q颜色 = Color.FromArgb(255, 56, 80, 80); // 远程形态 颜色
        var 远程飞斧可以释放颜色 = Color.FromArgb(255, 87, 105, 97); // 技能可以释放颜色
        var 释放二阶段颜色 = Color.FromArgb(255, 68, 165, 76); // 释放颜色
        var 技能进入CD颜色 = Color.FromArgb(255, 255, 255, 255); // 释放完颜色
        var A杖技能进入CD颜色 = Color.FromArgb(255, 1, 1, 1); // A杖释放完颜色

        switch (_全局步骤q)
        {
            case < 1:
            {
                if (_是否魔晶)
                {
                    if (ColorAEqualColorB(远程q颜色, GetPixelBytes(bts, size, q6), 0))
                    {
                        _全局步骤q = 1;
                    }
                }
                else if (ColorAEqualColorB(远程q颜色, GetPixelBytes(bts, size, q5), 0))
                {
                    _全局步骤q = 1;
                }

                return true;
            }
            case < 2:
            {
                if (_是否魔晶)
                {
                    if (ColorAEqualColorB(远程飞斧可以释放颜色, GetPixelBytes(bts, size, w6), 0))
                    {
                        _全局步骤q = 2;
                        _全局时间 = 获取当前时间毫秒();
                    }
                }
                else if (ColorAEqualColorB(远程飞斧可以释放颜色, GetPixelBytes(bts, size, w5), 0))
                {
                    _全局步骤q = 2;
                    _全局时间 = 获取当前时间毫秒();
                }

                return true;
            }
            case < 4:
            {
                if (_是否魔晶)
                {
                    if (ColorAEqualColorB(释放二阶段颜色, GetPixelBytes(bts, size, w6), 0))
                    {
                        _全局步骤q = 4;
                    }

                    KeyPress((uint)Keys.W);
                    Delay(30);
                }
                else
                {
                    if (ColorAEqualColorB(释放二阶段颜色, GetPixelBytes(bts, size, w5), 0))
                    {
                        _全局步骤q = 4;
                    }

                    KeyPress((uint)Keys.W);
                    Delay(30);
                }

                if (_全局时间 != -1 && 获取当前时间毫秒() - _全局时间 <= 500) return true;

                _全局时间 = -1;
                _全局步骤q = 0;
                return false;
            }
            case < 5:
            {
                if (_是否魔晶)
                {
                    if (ColorAEqualColorB(A杖技能进入CD颜色, GetPixelBytes(bts, size, w6), 0)
                        || ColorAEqualColorB(技能进入CD颜色, GetPixelBytes(bts, size, w6), 0))
                    {
                        Run(() =>
                        {
                            KeyPress((uint)Keys.A);
                            Delay(230);
                            KeyPress((uint)Keys.Q);
                            KeyPress((uint)Keys.M);
                            Delay(150);
                            KeyPress((uint)Keys.A);
                        });
                        _全局时间 = -1;
                        _全局步骤q = 0;
                        return false;
                    }
                }
                else
                {
                    if (ColorAEqualColorB(A杖技能进入CD颜色, GetPixelBytes(bts, size, w5), 0)
                        || ColorAEqualColorB(技能进入CD颜色, GetPixelBytes(bts, size, w5), 0))
                    {
                        Run(() =>
                        {
                            KeyPress((uint)Keys.A);
                            Delay(230);
                            KeyPress((uint)Keys.Q);
                            KeyPress((uint)Keys.M);
                            Delay(150);
                            KeyPress((uint)Keys.A);
                        });
                        _全局时间 = -1;
                        _全局步骤q = 0;
                        return false;
                    }
                }

                if (_全局时间 != -1 && 获取当前时间毫秒() - _全局时间 <= 500) return true;

                _全局时间 = -1;
                _全局步骤q = 0;
                return false;
            }
            default:
                return true;
        }
    }

    #endregion

    #region 小鱼人

    private static bool 黑暗契约平a(in byte[] bts, Size size)
    {
        // 超时则切回 总体释放时间 小于300毫秒
        if (获取当前时间毫秒() - _全局时间q > 300 && _全局时间q != -1)
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            return false;
        }

        var x = 截图模式1X;
        var y = 截图模式1Y;

        var q4 = GetPixelBytes(bts, size, 827 - x, 976 - y);
        var q5 = GetPixelBytes(bts, size, 810 - x, 971 - y);

        switch (_是否魔晶)
        {
            case true when ColorAEqualColorB(q5, Color.FromArgb(255, 31, 31, 62), 0): // 一般技能原色颜色
                return true;
            case true:
                _全局时间q = -1;
                // 释放前摇时长 + 最长检测时间 40足够了
                Delay(40);
                // RightClick(); // 用于测试反应
                KeyPress((uint)Keys.A);
                _条件保持假腿 = true;
                return false;

            case false when ColorAEqualColorB(q4, Color.FromArgb(255, 138, 143, 200), 0): // 一般技能原色颜色
                return true;
            case false:
                _全局时间q = -1;
                // 释放前摇时长 + 最长检测时间 40足够了
                Delay(40);
                // RightClick(); // 用于测试反应
                KeyPress((uint)Keys.A);
                _条件保持假腿 = true;
                return false;
        }
    }

    private static bool 跳水a(in byte[] bts, Size size)
    {
        KeyPress((uint)Keys.A);
        _条件保持假腿 = true;
        return false;
    }

    private static bool 深海护罩a(in byte[] bts, Size size)
    {
        Delay(300); // 前摇 wiki写着100 实际100不够
        KeyPress((uint)Keys.A);
        _条件保持假腿 = true;
        return false;
    }

    #endregion

    #region 猴子

    private void 灵魂之矛敏捷()
    {
        var w_down = 0;
        var time = 获取当前时间毫秒();

        while (w_down == 0)
        {
            if (RegPicture(猴子_释放灵魂之矛, "Q"))
            {
                Delay(105);
                切敏捷腿();
                w_down = 1;
            }

            if (获取当前时间毫秒() - time > 2000) break;
        }
    }

    private void 神行百变敏捷()
    {
        var w_down = 0;
        var time = 获取当前时间毫秒();

        while (w_down == 0)
        {
            if (RegPicture(猴子_释放神行百变, "W"))
            {
                Delay(1100);
                切敏捷腿();
                w_down = 1;
            }

            if (获取当前时间毫秒() - time > 2000) break;
        }
    }

    private void 后撤触发冲锋()
    {
        Delay(550);
        KeyPress((uint)Keys.A);
    }

    #endregion

    #region 水人

    #endregion

    #region 敌法

    private static bool 闪烁敏捷(in byte[] bts, Size size)
    {
        // 超时则切回 总体释放时间 小于300毫秒
        if (获取当前时间毫秒() - _全局时间q > 600 && _全局时间q != -1)
        {
            _全局时间q = -1;
            _条件保持假腿 = true;
            return false;
        }

        var x = 截图模式1X;
        var y = 截图模式1Y;

        var w4 = GetPixelBytes(bts, size, 892 - x, 976 - y);
        var w5 = GetPixelBytes(bts, size, 868 - x, 971 - y);

        switch (_是否a杖)
        {
            case true when !ColorAEqualColorB(w5, Color.FromArgb(255, 214, 241, 216), 15, 4, 15) // 一般释放颜色
                           && !ColorAEqualColorB(w5, Color.FromArgb(255, 186, 221, 203), 5, 3, 1): // 信仰之源颜色
                return true;
            case true:
                _全局时间q = -1;
                // 释放前摇时长 + 最长检测时间 40足够了
                Delay(340);
                // RightClick(); // 用于测试反应
                KeyPress((uint)Keys.A);
                _条件保持假腿 = true;
                return false;

            case false when !ColorAEqualColorB(w4, Color.FromArgb(255, 214, 241, 216), 15, 4, 15) // 一般释放颜色
                            && !ColorAEqualColorB(w4, Color.FromArgb(255, 186, 221, 203), 5, 3, 1): // 信仰之源颜色
                return true;
            case false:
                _全局时间q = -1;
                // 释放前摇时长 + 最长检测时间 40足够了
                Delay(340);
                // RightClick(); // 用于测试反应
                KeyPress((uint)Keys.A);
                _条件保持假腿 = true;
                return false;
        }
    }

    private static bool 法力虚空取消后摇(in byte[] bts, Size size)
    {
        // 超时则切回 总体释放时间 小于300毫秒
        if (获取当前时间毫秒() - _全局时间r > 600 && _全局时间r != -1)
        {
            _全局时间r = -1;
            _条件保持假腿 = true;
            return false;
        }

        var x = 截图模式1X;
        var y = 截图模式1Y;

        var r4 = GetPixelBytes(bts, size, 1022 - x, 976 - y);
        var r5 = GetPixelBytes(bts, size, 1042 - x, 971 - y);

        switch (_是否a杖)
        {
            case true when !ColorAEqualColorB(r5, Color.FromArgb(255, 8, 148, 33), 1, 1, 4) // 一般颜色
                           && !ColorAEqualColorB(r5, Color.FromArgb(255, 87, 167, 130), 0, 0, 0): // 碎颅锤颜色
                return true;
            case true:
            {
                _全局时间r = -1;
                // 释放前摇时长 + 最长检测时间 40足够了
                Delay(270);
                // RightClick(); // 用于测试反应
                KeyPress((uint)Keys.A);
                _条件保持假腿 = true;
                return false;
            }
            case false when !ColorAEqualColorB(r4, Color.FromArgb(255, 8, 148, 33), 1, 1, 4) // 一般颜色
                            && !ColorAEqualColorB(r4, Color.FromArgb(255, 21, 149, 60), 0, 0, 0): // 碎颅锤颜色:
                return true;
            case false:
            {
                _全局时间r = -1;
                // 释放前摇时长 + 最长检测时间 40足够了
                Delay(270);
                //RightClick(); // 用于测试反应
                KeyPress((uint)Keys.A);
                _条件保持假腿 = true;
                return false;
            }
        }
    }

    #endregion

    #region 幻刺

    private static bool 窒息短匕敏捷(in byte[] bts, Size size)
    {
        if (!RegPicture(_是否魔晶 ? 幻刺_窒息短匕_5 : 幻刺_窒息短匕_4, bts, size)) return true;

        Delay(105);
        _条件保持假腿 = true;
        RightClick();
        return false;
    }

    private static bool 幻影突袭敏捷(in byte[] bts, Size size)
    {
        if (!RegPicture(_是否魔晶 ? 幻刺_幻影突袭_5 : 幻刺_幻影突袭_4, bts, size)) return true;

        Delay(105);
        _条件保持假腿 = true;
        KeyPress((uint)Keys.A);
        return false;
    }

    private static bool 魅影无形敏捷(in byte[] bts, Size size)
    {
        if (!RegPicture(_是否魔晶 ? 幻刺_魅影无形_5 : 幻刺_魅影无形_4, bts, size)) return true;

        Delay(105);
        _条件保持假腿 = true;
        KeyPress((uint)Keys.A);
        return false;
    }

    private static bool 刀阵旋风敏捷(in byte[] bts, Size size)
    {
        if (!RegPicture(幻刺_刀阵旋风_5, bts, size)) return true;

        Delay(105);
        _条件保持假腿 = true;
        KeyPress((uint)Keys.A);
        return false;
    }

    #endregion

    #region 虚空

    private void 时间漫游敏捷()
    {
        var time = 获取当前时间毫秒();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(虚空_释放时间漫游_4, "Q") || RegPicture(虚空_释放时间漫游_5, "Q", 5))
            {
                Delay(450);
                KeyPress((uint)Keys.V);
                w_down = 1;
            }

            if (获取当前时间毫秒() - time <= 800) continue;

            切敏捷腿();
            break;
        }
    }

    private void 时间膨胀敏捷()
    {
        Delay(180);
        KeyPress((uint)Keys.V);
        KeyPress((uint)Keys.A);
    }

    private void 时间结界敏捷()
    {
        var time = 获取当前时间毫秒();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(虚空_释放时间结界_4, "R") || RegPicture(虚空_释放时间结界_5, "R", 5))
            {
                Delay(375);
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.A);
                w_down = 1;
            }

            if (获取当前时间毫秒() - time <= 1200) continue;

            切敏捷腿();
            break;
        }
    }

    #endregion

    #region TB

    private void 倒影敏捷()
    {
        var time = 获取当前时间毫秒();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(TB_释放倒影_4, "Q") || RegPicture(TB_释放倒影_5, "Q", 5)
                                           || RegPicture(TB_释放倒影_6, "Q", 6) ||
                                           RegPicture(TB_释放倒影_7, "Q", 7))
            {
                Delay(150);
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.A);
                w_down = 1;
            }

            if (获取当前时间毫秒() - time <= 1600) continue;

            切敏捷腿();
            break;
        }
    }

    private void 幻惑敏捷()
    {
        var time = 获取当前时间毫秒();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(TB_释放幻惑_4, "W") || RegPicture(TB_释放幻惑_5, "W", 5)
                                           || RegPicture(TB_释放幻惑_6, "W", 6) ||
                                           RegPicture(TB_释放幻惑_7, "W", 7))
            {
                Delay(85);
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.A);
                w_down = 1;
            }

            if (获取当前时间毫秒() - time <= 1600) continue;

            切敏捷腿();
            break;
        }
    }

    private void 魔化敏捷()
    {
        Delay(450);
        KeyPress((uint)Keys.V);
        KeyPress((uint)Keys.A);
    }

    private void 恶魔狂热去后摇()
    {
        var time = 获取当前时间毫秒();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(TB_恶魔狂热_6, "D", 6))
            {
                Delay(150);
                KeyPress((uint)Keys.A);
                w_down = 1;
            }

            if (获取当前时间毫秒() - time <= 1600) continue;

            break;
        }
    }

    private void 断魂敏捷()
    {
        var time = 获取当前时间毫秒();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(TB_释放断魂_4, "R") || RegPicture(TB_释放断魂_5, "R", 5)
                                           || RegPicture(TB_释放断魂_6, "R", 6) ||
                                           RegPicture(TB_释放断魂_7, "R", 7))
            {
                Delay(150);
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.A);
                w_down = 1;
            }

            if (获取当前时间毫秒() - time <= 1600) continue;

            切敏捷腿();
            break;
        }
    }

    #endregion

    #region 赏金

    private static bool 飞镖接平a(in byte[] bts, Size size)
    {
        if (!RegPicture(赏金_释放飞镖, bts, size) &&
            !RegPicture(赏金_释放飞镖_双刀, bts, size)) return true;
        ;
        Delay(105);
        RightClick();
        return false;
    }

    private static bool 标记去后摇(in byte[] bts, Size size)
    {
        if (!RegPicture(赏金_释放标记, bts, size) &&
            !RegPicture(赏金_释放标记_不朽, bts, size)) return true;

        Delay(110);
        RightClick();
        return false;
    }

    #endregion

    #region 剧毒

    private static bool 循环蛇棒(in byte[] bts, Size size)
    {
        var x = 截图模式1X;
        var y = 截图模式1Y;

        if (ColorAEqualColorB(GetPixelBytes(bts, size, 942 - x, 989 - y), Color.FromArgb(255, 153, 161, 70),
                29)) // (942,989)
        {
            KeyPress((uint)Keys.E);
            Delay(30);
        }
        else
        {
            if (!RegPicture(剧毒_蛇棒_CD_不朽, bts, size) &&
                !RegPicture(剧毒_蛇棒_CD, bts, size)) return _循环条件1;

            KeyPress((uint)Keys.E);
            Delay(30);
        }

        return _循环条件1;
    }

    private static bool 蛇棒去后摇(in byte[] bts, Size size)
    {
        if (RegPicture(剧毒_蛇棒_CD_不朽, bts, size) ||
            RegPicture(剧毒_蛇棒_CD, bts, size)) return true;

        RightClick();
        Delay(30);
        return false;
    }

    private static bool 瘴气去后摇(in byte[] bts, Size size)
    {
        if (RegPicture(剧毒_瘴气_CD, bts, size) ||
            RegPicture(剧毒_瘴气不朽_CD, bts, size)) return true;

        KeyPress((uint)Keys.A);
        Delay(30);
        return false;
    }

    private static bool 剧毒新星去后摇(in byte[] bts, Size size)
    {
        var x = 截图模式1X;
        var y = 截图模式1Y;
        var c = GetPixelBytes(bts, size, 1016 - x, 957 - y); // (1016,957)
        // 剧毒新星最佳匹配
        if (!ColorAEqualColorB(c, Color.FromArgb(255, 74, 78, 52))) return true;
        RightClick();
        return false;
    }

    #endregion

    #region 美杜莎

    private static bool 秘术异蛇去后摇(in byte[] bts, Size size)
    {
        if (!RegPicture(美杜莎_释放秘术异蛇, bts, size) &&
            !RegPicture(美杜莎_释放秘术异蛇_5, bts, size)) return true;

        Delay(60);
        RightClick();
        切敏捷腿(_技能数量);
        return false;
    }

    private static bool 石化凝视去后摇(in byte[] bts, Size size)
    {
        if (!RegPicture(美杜莎_释放石化凝视, bts, size) &&
            !RegPicture(美杜莎_释放石化凝视_5, bts, size)) return true;

        Delay(120);
        RightClick();
        切敏捷腿(_技能数量);
        return false;
    }

    #endregion

    #endregion

    #region 智力

    #region 黑鸟

    private void G_yxc_cg()
    {
        //G_yxc(245, 550, 435);
        KeyPress((uint)Keys.Space);
        G_yxc_y();
    }

    private void G_yxc_y()
    {
        G_yxc(245, 550, 300);
    }

    private void 关接跳()
    {
        KeyPress((uint)Keys.L);
        KeyPress((uint)Keys.L);

        var time = 获取当前时间毫秒();

        Delay(4030);

        var w_down = 0;
        while (w_down == 0)
        {
            if (获取当前时间毫秒() - time > 4100) break;

            KeyPress((uint)Keys.Space);
        }
    }

    private static void G_yxc(int dyd, int yd, int dd)
    {
        if (RegPicture(物品_纷争, "C")) KeyPress((uint)Keys.C);

        KeyPress((uint)Keys.W);

        var w_down = 0;

        var time = 获取当前时间毫秒();

        while (w_down == 0)
        {
            if (获取当前时间毫秒() - time > 6000) break;

            if (RegPicture(黑鸟_关释放, "W"))
            {
                w_down = 1;
                Delay(dyd);
                RightClick();

                Delay(yd);
                if (_中断条件) break;

                KeyPress((uint)Keys.S);

                Delay(dd);
                if (_中断条件) break;
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.X);
            }
        }
    }

    #endregion

    #region 谜团

    private void 跳秒接午夜凋零黑洞()
    {
        if (RegPicture(物品_黑皇, "Z")) KeyPress((uint)Keys.Z);

        if (RegPicture(物品_纷争, "C")) KeyPress((uint)Keys.C);

        var time = 获取当前时间毫秒();

        while (RegPicture(物品_跳刀, "SPACE") || RegPicture(物品_跳刀_智力跳刀, "SPACE"))
        {
            Delay(15);
            KeyPress((uint)Keys.Space);

            if (获取当前时间毫秒() - time > 300) break;
        }

        time = 获取当前时间毫秒();

        while (RegPicture(谜团_午夜凋零CD, "E"))
        {
            KeyPress((uint)Keys.E);
            Delay(15);
            if (获取当前时间毫秒() - time > 500) break;
        }

        //if (午夜凋零_i) Delay(智力跳刀BUFF() ? 45 : 75);

        time = 获取当前时间毫秒();

        while (RegPicture(谜团_黑洞CD, "R"))
        {
            KeyPress((uint)Keys.R);
            Delay(15);

            if (获取当前时间毫秒() - time > 500) break;
        }

        Delay(30);

        //KeyDown((uint)Keys.LControlKey);

        //KeyPress((uint)Keys.A);

        //KeyUp((uint)Keys.LControlKey);
    }

    private void 刷新接凋零黑洞()
    {
        KeyPress((uint)Keys.X);

        for (var i = 0; i < 2; i++)
        {
            Delay(30);
            KeyPress((uint)Keys.Z);
            KeyPress((uint)Keys.V);
            KeyPress((uint)Keys.R);
        }
    }

    #endregion

    #region 冰女

    /// <summary>
    ///     基本无缝控制
    ///     但因为本身是软控制，不要强求
    /// </summary>
    private void 冰封禁制接陨星锤()
    {
        KeyPress((uint)Keys.W);

        var w_down = 0;

        while (w_down == 0)
            if (RegPicture(冰女_释放冰封禁制, 859, 939, 64, 62))
            {
                Delay(365);
                KeyPress((uint)Keys.Space);
                w_down = 1;
            }
    }

    #endregion

    #region 火女

    private static bool 龙破斩去后摇(in byte[] bts, Size size)
    {
        if (RegPicture(火女_释放龙破斩, bts, size))
        {
            Delay(120);
            RightClick(); // 移动反应更快，更好检测最短延时
            return false;
        }

        return true;
    }

    private static bool 光击阵去后摇(in byte[] bts, Size size)
    {
        if (RegPicture(火女_释放光击阵, bts, size))
        {
            Delay(120);
            RightClick(); // 移动反应更快，更好检测最短延时
            return false;
        }

        return true;
    }

    private static bool 神灭斩去后摇(in byte[] bts, Size size)
    {
        if (RegPicture(火女_释放神灭斩, bts, size))
        {
            Delay(120);
            RightClick(); // 移动反应更快，更好检测最短延时
            return false;
        }

        return true;
    }

    #endregion

    #region 蓝猫

    private static bool 拉接平A(in byte[] bts, Size size)
    {
        if (RegPicture(蓝猫_释放电子漩涡, bts, size))
        {
            Delay(115);
            //RightClick();
            KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    private void 残影接平A()
    {
        Delay(30);
        KeyPress((uint)Keys.A);
    }

    private static bool 滚接平A(in byte[] bts, Size size)
    {
        if (RegPicture(蓝猫_释放球状闪电_红, bts, size) || RegPicture(蓝猫_释放球状闪电, bts, size))
        {
            Delay(117);
            //RightClick();
            KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    #endregion

    #region 宙斯

    private static bool 弧形闪电去后摇(in byte[] bts, Size size)
    {
        var x = 截图模式1X;
        var y = 截图模式1Y;

        var q4 = GetPixelBytes(bts, size, 827 - x, 976 - y);
        var q5 = GetPixelBytes(bts, size, 810 - x, 971 - y);
        var q6 = GetPixelBytes(bts, size, 780 - x, 971 - y);

        if (_是否魔晶 && _是否a杖)
        {
            if (!ColorAEqualColorB(q6, Color.FromArgb(255, 61, 156, 155), 5, 12, 10)) return true;
            Delay(220);
            RightClick();
            KeyPress((uint)Keys.S);
            return false;
        }

        if (_是否魔晶 || _是否a杖)
        {
            if (!ColorAEqualColorB(q5, Color.FromArgb(255, 61, 156, 155), 5, 12, 10)) return true;
            Delay(220);
            RightClick();
            KeyPress((uint)Keys.S);
            return false;
        }
        else
        {
            if (!ColorAEqualColorB(q4, Color.FromArgb(255, 99, 170, 68))) return true;
            Delay(220);
            RightClick();
            KeyPress((uint)Keys.S);
            return false;
        }
    }

    private static bool 弧形闪电不能释放(in byte[] bts, Size size)
    {
        /// 逻辑 
        /// 先检测是否可以释放
        /// 如果可以则返回false
        /// 外部触发原本逻辑
        /// 否则改全局模式为1，并重复触发本逻辑
        /// 直到可以释放，改回全局模式0，再延迟释放释放
        /// 用不同的循环条件，避免不可预知的时序错误

        var x = 截图模式1X;
        var y = 截图模式1Y;

        var q4 = GetPixelBytes(bts, size, 827 - x, 976 - y);
        var q5 = GetPixelBytes(bts, size, 810 - x, 971 - y);
        var q6 = GetPixelBytes(bts, size, 780 - x, 971 - y);

        if (_是否魔晶 && _是否a杖)
        {
            if (ColorAEqualColorB(q6, Color.FromArgb(255, 73, 112, 181), 1, 1, 1)) // 不朽6电弧
            {
                if (_全局模式q == 0) return false;

                if (_全局模式q == 1)
                {
                    _全局模式q = 0;
                    Run(() => { KeyPress((uint)Keys.Q); });
                    return false;
                }
            }
        }
        else if (_是否魔晶 || _是否a杖)
        {
            if (ColorAEqualColorB(q5, Color.FromArgb(255, 73, 112, 181), 1, 1, 1)) // 不朽5电弧
            {
                if (_全局模式q == 0) return false;

                if (_全局模式q == 1)
                {
                    _全局模式q = 0;
                    Run(() => { KeyPress((uint)Keys.Q); });
                    return false;
                }
            }
        }
        else
        {
            if (ColorAEqualColorB(q4, Color.FromArgb(255, 125, 120, 89), 1, 1, 4)) // 不朽4电弧
            {
                if (_全局模式q == 0) return false;

                if (_全局模式q == 1)
                {
                    _全局模式q = 0;
                    Run(() => { KeyPress((uint)Keys.Q); });
                    return false;
                }
            }
        }

        return true;
    }

    private static bool 雷击去后摇(in byte[] bts, Size size)
    {
        var x = 截图模式1X;
        var y = 截图模式1Y;

        var w4 = GetPixelBytes(bts, size, 892 - x, 976 - y);
        var w5 = GetPixelBytes(bts, size, 868 - x, 971 - y);
        var w6 = GetPixelBytes(bts, size, 838 - x, 971 - y);

        if (_是否魔晶 && _是否a杖)
        {
            if (ColorAEqualColorB(w6, Color.FromArgb(255, 207, 244, 217), 14, 3, 14)) // 普通6雷击
            {
                Delay(325);
                RightClick();
                return false;
            }
        }
        else if (_是否魔晶 || _是否a杖)
        {
            if (ColorAEqualColorB(w5, Color.FromArgb(255, 207, 244, 217), 14, 3, 14)) // 普通5雷击
            {
                Delay(325);
                RightClick();
                return false;
            }
        }
        else
        {
            if (ColorAEqualColorB(w4, Color.FromArgb(255, 116, 228, 203))) // 普通4雷击
            {
                Delay(325);
                RightClick();
                return false;
            }
        }

        return true;
    }

    #endregion

    #region 卡尔

    private void 三冰对线()
    {
        KeyPress((uint)Keys.Q);
        Delay(30);
        KeyPress((uint)Keys.Q);
        Delay(30);
        KeyPress((uint)Keys.Q);
        Delay(30);
    }

    private void 三火平A()
    {
        KeyPress((uint)Keys.E);
        Delay(30);
        KeyPress((uint)Keys.E);
        Delay(30);
        KeyPress((uint)Keys.E);
        Delay(30);
    }

    private void 三雷幽灵()
    {
        KeyPress((uint)Keys.Q);
        Delay(30);
        KeyPress((uint)Keys.Q);
        Delay(30);
        KeyPress((uint)Keys.W);
        Delay(30);
        KeyPress((uint)Keys.R);
        Delay(30);
        KeyPress((uint)Keys.W);
        Delay(30);
        KeyPress((uint)Keys.W);
        Delay(30);
        KeyPress((uint)Keys.D);
    }

    private void 吹风天火()
    {
        KeyPress((uint)Keys.W);
        Delay(30);
        KeyPress((uint)Keys.W);
        Delay(30);
        KeyPress((uint)Keys.Q);
        Delay(30);
        KeyPress((uint)Keys.R);
        Delay(30);
        KeyPress((uint)Keys.D);
        Delay(30);
        KeyPress((uint)Keys.E);
        Delay(30);
        KeyPress((uint)Keys.E);
        Delay(30);
        KeyPress((uint)Keys.E);
        Delay(30);
        KeyPress((uint)Keys.R);
        Delay(600);
        KeyPress((uint)Keys.D);
    }

    #endregion

    #region 拉席克

    private void 吹风接撕裂大地()
    {
        var all_down = 0;
        if (RegPicture(物品_吹风CD完, "SPACE"))
        {
            label1.Text = "FF";
            KeyPress((uint)Keys.Space);
            Delay(30);

            var time = 获取当前时间毫秒();

            while (all_down == 0)
            {
                if (获取当前时间毫秒() - time > 4000) break;

                if (RegPicture(物品_吹风CD, "SPACE"))
                {
                    label1.Text = "FFF";
                    if (RegPicture(物品_纷争, "C")) KeyPress((uint)Keys.C);
                    Delay(80);
                    KeyPress((uint)Keys.H);
                    Delay(1280);
                    if (_中断条件) break;
                    KeyPress((uint)Keys.Q);
                    Delay(760);
                    KeyPress((uint)Keys.R);
                    all_down = 1;
                }

                Delay(50);
            }
        }
    }

    #endregion

    #region 暗影萨满

    private static bool 苍穹振击取消后摇(in byte[] bts, Size size)
    {
        if (RegPicture(暗影萨满_释放苍穹振击, bts, size))
        {
            Delay(200);
            KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    private static bool 释放群蛇守卫取消后摇(in byte[] bts, Size size)
    {
        if (RegPicture(暗影萨满_释放群蛇守卫, bts, size))
        {
            Delay(200);
            KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    private static bool 变羊取消后摇(in byte[] bts, Size size)
    {
        if (!RegPicture(暗影萨满_妖术CD, bts, size))
        {
            KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    private void 暗夜萨满最大化控制链(in byte[] bts, Size size)
    {
        var times = 1.0;
        var time = 0;

        if (RegPicture(物品_祭礼长袍, bts, size)) times *= 1.1;

        if (RegPicture(物品_永恒遗物, bts, size)) times *= 1.25;

        times *= (100 - Convert.ToDouble(tb_状态抗性.Text)) / 100;

        var 技能点颜色 = Color.FromArgb(255, 203, 183, 124);

        if (CaptureColor(909, 1008).Equals(技能点颜色))
            time = 3500;
        else if (CaptureColor(897, 1008).Equals(技能点颜色))
            time = 2750;
        else if (CaptureColor(885, 1008).Equals(技能点颜色))
            time = 2000;
        else if (CaptureColor(875, 1008).Equals(技能点颜色)) time = 1250;

        KeyPress((uint)Keys.W);

        _循环条件2 = true;

        Run(() =>
        {
            var time1 = 获取当前时间毫秒();

            var w_down = 0;

            while (w_down == 0)
            {
                if (!RegPicture(暗影萨满_妖术CD, "W")) w_down = 1;

                if (!_循环条件2) return;

                if (获取当前时间毫秒() - time1 > 1200) break;
            }

            Delay(Convert.ToInt32(time * times) - 400);

            if (!_循环条件2) return;

            KeyPress((uint)Keys.E);
        });
    }

    #endregion

    #region 小仙女

    private void 皇冠延时计时()
    {
        var 总开始时间 = 获取当前时间毫秒();

        var w_down = 0;

        while (w_down == 0)
        {
            if (获取当前时间毫秒() - 总开始时间 > 2000) return;

            if (RegPicture(小仙女_释放诅咒皇冠_不朽, "E", 7))
            {
                // 全局时间 = 获取当前时间毫秒();
                Delay(100);
                KeyPress((uint)Keys.M);
            }
        }
    }

    private void 诅咒皇冠吹风()
    {
        var 总开始时间 = 获取当前时间毫秒();

        KeyPress((uint)Keys.E);

        var w_down = 0;

        while (w_down == 0)
        {
            if (获取当前时间毫秒() - 总开始时间 > 2000) return;

            if (RegPicture(小仙女_释放诅咒皇冠_不朽, "E", 7))
            {
                // Delay(阿哈利姆魔晶() ? 410 : 1410);  // 大部分技能抬手都是0.2-0.3之间
                if (!_循环条件2) return;

                if (RegPicture(物品_吹风, "SPACE", 7))
                {
                    KeyPress((uint)Keys.Space);
                    KeyPress((uint)Keys.M);

                    Delay(2500);
                    if (!_循环条件2) return;
                    作祟暗影之境最大化伤害();
                }

                w_down = 1;
            }
        }
    }

    private static void 作祟暗影之境最大化伤害()
    {
        // 释放纷争，增加大量伤害
        if (RegPicture(物品_纷争, "C", 7)) KeyPress((uint)Keys.C);

        KeyPress((uint)Keys.M);
        Delay(30);
        KeyPress((uint)Keys.D);
        Delay(30);
        KeyPress((uint)Keys.W);
        Delay(30);
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

    private static bool 循环奥数鹰隼(in byte[] bts, Size size)
    {
        var x = 截图模式1X;
        var y = 截图模式1Y;
        var q4 = GetPixelBytes(bts, size, 827 - x, 976 - y);
        var q5 = GetPixelBytes(bts, size, 810 - x, 971 - y);

        return _循环条件1;
    }

    private static bool 天怒秒人连招(in byte[] bts, Size size)
    {
        var x = 截图模式1X;
        var y = 截图模式1Y;
        // 最好的找色为CD读条的白色文字

        var q4 = GetPixelBytes(bts, size, 827 - x, 976 - y);
        var q5 = GetPixelBytes(bts, size, 810 - x, 971 - y);
        var e4 = GetPixelBytes(bts, size, 957 - x, 976 - y);
        var e5 = GetPixelBytes(bts, size, 926 - x, 971 - y);
        var r4 = GetPixelBytes(bts, size, 1022 - x, 976 - y);
        var r5 = GetPixelBytes(bts, size, 1042 - x, 971 - y);

        var buff = 智力跳刀buff(bts, size);

        switch (_全局步骤)
        {
            case < 1:
            {
                KeyPress((uint) Keys.W);
                _全局步骤 = 1;
                return true;
            }
            case < 2 when 根据图片以及类别使用物品(物品_血精石, bts, size, _技能数量):
            {
                Delay(30);
                RightClick();
                _全局步骤 = 2;
                return true;
            }

            case < 3 when 根据图片以及类别使用物品(物品_虚灵之刃, bts, size, _技能数量):
            {
                Delay(30);
                RightClick();
                _全局步骤 = 3;
                return true;
            }

            case < 4 when 根据图片以及类别使用物品(物品_虚灵之刃, bts, size, _技能数量):
            {
                Delay(30);
                RightClick();
                _全局步骤 = 4;
                return true;
            }

            case < 5 when 根据图片以及类别使用物品(物品_羊刀, bts, size, _技能数量):
            {
                Delay(30);
                RightClick();
                _全局步骤 = 5;
                return true;
            }


            case < 6 when 根据图片以及类别使用物品(物品_纷争, bts, size, _技能数量):
            {
                Delay(30);
                RightClick();
                _全局步骤 = 6;
                return true;
            }

            case < 7 when 根据图片以及类别使用物品(物品_阿托斯之棍_4, bts, size, _技能数量):
            {
                Delay(30);
                RightClick();
                _全局步骤 = 7;
                return true;
            }

            case < 8 when 根据图片以及类别使用物品(物品_缚灵锁_4, bts, size, _技能数量):
            {
                Delay(30);
                RightClick();
                _全局步骤 = 8;
                return true;
            }
                
            case < 9 when
                (_是否魔晶 &&
                 (ColorAEqualColorB(q5, Color.FromArgb(255, 191, 134, 35), 0) // 金色饰品 可释放
                  || ColorAEqualColorB(q5, Color.FromArgb(255, 39, 50, 32), 0)) // 普通饰品 可释放
                )
                ||
                (!_是否魔晶 &&
                 (ColorAEqualColorB(q4, Color.FromArgb(255, 146, 91, 3), 0) // 金色饰品 可释放
                  || ColorAEqualColorB(q4, Color.FromArgb(255, 29, 37, 21), 0)) // 普通饰品 可释放
                ):
            {
                KeyPress((uint) Keys.Q);
                _全局步骤 = 9;
                return true;
            }

            case < 10 when
                (_是否魔晶 &&
                 (ColorAEqualColorB(q5, Color.FromArgb(255, 152, 176, 26), 0) // 金色饰品 正在释放
                  || ColorAEqualColorB(q5, Color.FromArgb(255, 29, 151, 23), 0))
                ) // 普通饰品 正在释放
                ||
                (!_是否魔晶 &&
                 (ColorAEqualColorB(q4, Color.FromArgb(255, 115, 161, 2), 0) // 金色饰品  正在释放
                  || ColorAEqualColorB(q4, Color.FromArgb(255, 21, 149, 14), 0)) // 普通饰品 正在释放
                ):
            {
                Delay(buff ? 50 : 100);
                RightClick();
                _全局步骤 = 10;
                return true;
            }
                
            case 10 when
                (_是否魔晶 &&
                 ColorAEqualColorB(e5, Color.FromArgb(255, 114, 226, 220), 0)) // 上古封印 可以释放
                ||
                (!_是否魔晶 &&
                 ColorAEqualColorB(e4, Color.FromArgb(255, 67, 153, 147), 0)): // 上古封印 可以释放
            {
                KeyPress((uint) Keys.E);
                _全局步骤 = 11;
                return true;
            }

            case < 12 when
                (_是否魔晶 &&
                 ColorAEqualColorB(e5, Color.FromArgb(255, 89, 224, 175), 0)) // 上古封印 正在释放
                ||
                (!_是否魔晶 &&
                 ColorAEqualColorB(e4, Color.FromArgb(255, 52, 184, 116), 0)): // 上古封印 正在释放
            {
                Delay(buff ? 50 : 100);
                RightClick();
                _全局步骤 = 12;
                return true;
            }

            case 12 when
                (_是否魔晶 &&
                 ColorAEqualColorB(r5, Color.FromArgb(255, 182, 203, 170), 0)) // 神秘之耀 可以释放
                ||
                (!_是否魔晶 &&
                 ColorAEqualColorB(r4, Color.FromArgb(255, 105, 127, 91), 0)): // 神秘之耀 可以释放
            {
                KeyPress((uint) Keys.R);
                _全局步骤 = 13;
                return true;
            }

            case 13:
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region 炸弹人

    private bool 爆破后接3雷粘性炸弹(in byte[] bts, Size size)
    {
        if (RegPicture(炸弹人_释放爆破起飞, bts, size))
        {
            Delay(995);

            if (RegPicture(物品_以太, bts, size) || RegPicture(物品_玲珑心, bts, size))
            {
            }

            return false;
        }

        return true;
    }

    #endregion

    #region 神域

    private static bool 命运敕令去后摇(in byte[] bts, Size size)
    {
        if (!RegPicture(神域_释放命运敕令, bts, size)) return true;

        Delay(145);
        RightClick();
        return false;
    }

    private static bool 涤罪之焰去后摇(in byte[] bts, Size size)
    {
        if (!RegPicture(神域_释放涤罪之焰, bts,size)) return true;
        Delay(95);
        RightClick();
        return false;
    }

    private static bool 虚妄之诺去后摇(in byte[] bts, Size size)
    {
        if (!RegPicture(神域_释放虚妄之诺, bts, size)) return true;
        Delay(105);
        RightClick();
        return false;
    }

    #endregion

    #region 修补匠

    private static async Task 检测敌方英雄自动导弹()
    {
        Task t = new(() =>
        {
            if (RegPicture(血量_敌人血量, 0, 0, 1920, 1080, 0.6))
            {
                KeyPress((uint)Keys.W);
                Delay(40);
            }
        });
        t.Start();
        await t;
    }

    private static async Task 检测希瓦()
    {
        Task t = new(() =>
        {
            Delay(120);
            if (RegPicture(物品_希瓦CD_6, "Z", 6) || RegPicture(物品_希瓦CD_7, "Z", 7))
                KeyPress((uint)Keys.Z);
        });
        t.Start();
        await t;
    }

    private static void 推推接刷新()
    {
        var time = 获取当前时间毫秒();
        var x_down = 0;
        while (x_down == 0)
        {
            if (RegPicture(物品_推推BUFF, 400, 865, 1000, 60))
            {
                KeyPress((uint)Keys.R);
                x_down = 1;
            }

            if (获取当前时间毫秒() - time > 500) break;
        }
    }


    private static async void 刷新完跳()
    {
        var all_down = 0;
        var time = 获取当前时间毫秒();
        while (all_down == 0)
        {
            var r_down = 0;
            if (RegPicture(修补匠_再填装施法, 773, 727, 75, 77, 0.7))
            {
                if (_条件3)
                    await 检测希瓦();
                while (r_down == 0)
                    if (!RegPicture(修补匠_再填装施法, 773, 727, 75, 77, 0.7))
                    {
                        r_down = 1;
                        all_down = 1;
                        if (_条件1)
                            await 检测敌方英雄自动导弹();
                        if (_条件2)
                        {
                            Delay(60);
                            KeyPress((uint)Keys.Space);
                        }
                    }
            }

            if (获取当前时间毫秒() - time > 700) break;
        }
    }

    #endregion

    #region 莱恩

    private static bool 莱恩羊接技能(in byte[] bts, Size size)
    {
        if (RegPicture(莱恩_羊, bts, size) || RegPicture(莱恩_羊_鱼, bts, size)) return true;

        if (_条件4)
            KeyPress((uint)Keys.E);
        else
            KeyPress((uint)Keys.A);
        return false;
    }

    private static bool 大招前纷争(in byte[] bts, Size size)
    {
        if (根据图片以及类别使用物品(物品_虚灵之刃, bts, size)) Delay(30);
        if (根据图片以及类别使用物品(物品_纷争, bts, size)) Delay(30);
        if (根据图片以及类别使用物品(物品_红杖, bts, size)) Delay(30);
        if (根据图片以及类别使用物品(物品_红杖2, bts, size)) Delay(30);
        if (根据图片以及类别使用物品(物品_红杖3, bts, size)) Delay(30);
        if (根据图片以及类别使用物品(物品_红杖4, bts, size)) Delay(30);
        if (根据图片以及类别使用物品(物品_红杖5, bts, size)) Delay(30);
        return false;
    }

    private static bool 推推破林肯秒羊(in byte[] bts, Size size)
    {
        根据图片以及类别使用物品(物品_推推棒, bts, size);
        return false;
    }

    #endregion

    #region 沉默

    private static bool 奥数诅咒去后摇(in byte[] bts, Size size)
    {
        if (!RegPicture(沉默_释放奥数诅咒, bts, size)) return true;

        Delay(175);
        KeyPress((uint)Keys.A);
        return false;
    }

    private static bool 遗言去后摇(in byte[] bts, Size size)
    {
        if (!RegPicture(沉默_释放遗言, bts, size)) return true;
        Delay(175);
        KeyPress((uint)Keys.A);
        return false;
    }

    #endregion

    #endregion

    #region 通用

    #region 循环

    private static async void 一般程序循环()
    {
        var 切腿lock = new object();

        while (_总循环条件)
            if (_循环内获取图片 != null)
            {

                _循环内获取图片(); // 更新全局Bitmap

                if (_中断条件) continue; // 中断则跳过循环

                if (_条件1 && _条件根据图片委托1 != null)
                    await Run(() => { _条件1 = _条件根据图片委托1(_全局bts, _全局size); });

                if (_条件2 && _条件根据图片委托2 != null)
                    await Run(() => { _条件2 = _条件根据图片委托2(_全局bts, _全局size); });

                if (_条件3 && _条件根据图片委托3 != null)
                    await Run(() => { _条件3 = _条件根据图片委托3(_全局bts, _全局size); });

                if (_条件4 && _条件根据图片委托4 != null)
                    await Run(() => { _条件4 = _条件根据图片委托4(_全局bts, _全局size); });

                if (_条件5 && _条件根据图片委托5 != null)
                    await Run(() => { _条件5 = _条件根据图片委托5(_全局bts, _全局size); });

                if (_条件6 && _条件根据图片委托6 != null)
                    await Run(() => { _条件6 = _条件根据图片委托6(_全局bts, _全局size); });

                if (!_条件保持假腿) continue;

                if (_条件假腿敏捷)
                {
                    await Run(() =>
                    {
                        if (RegPicture(物品_假腿_敏捷腿, _全局bts, _全局size)) return;
                        _条件保持假腿 = false;
                        lock (切腿lock)
                        {
                            切敏捷腿(_全局bts, _全局size, _技能数量);
                            Delay(185); // 2次切假腿稳定时间
                            _条件保持假腿 = true;
                        }
                    });
                }
                else
                {
                    await Run(() =>
                    {
                        if (RegPicture(物品_假腿_力量腿, _全局bts, _全局size)) return;
                        _条件保持假腿 = false;
                        lock (切腿lock)
                        {
                            切力量腿(_全局bts, _全局size, _技能数量);
                            Delay(185); // 2次切假腿稳定时间
                            _条件保持假腿 = true;
                        }
                    });
                }
            }
    }

    private static void 无物品状态初始化()
    {
        _循环内获取图片 ??= 获取图片_1;

        Run(() =>
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            一般程序循环();
        });
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
        _条件保持假腿 = false;
        _条件假腿敏捷 = false;
        _是否魔晶 = false;
        _是否a杖 = false;
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
        _全局模式 = 0;
        _全局模式d = 0;
        _全局模式f = 0;
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

    private static bool 快速回城(in byte[] bts, Size size)
    {
        if (RegPicture(物品_TP效果, bts, size)) return false;

        KeyPress((uint)Keys.T);
        Delay(30);
        KeyPress((uint)Keys.T);

        return true;
    }

    #endregion

    #region 泉水状态喝瓶子

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

        Delay(30);
        KeyDown((uint)Keys.LControlKey);
        Delay(30);
        KeyPress((uint)Keys.D9);
        Delay(30);
        KeyUp((uint)Keys.LControlKey);
    }

    #endregion

    #region 跳刀

    /// <summary>
    ///     用于快速先手无转身
    /// </summary>
    /// <returns></returns>
    private Point 正面跳刀_无转身()
    {
        // 坐标
        var mousePosition = MousePosition;

        // X间距
        double moveX = 0;
        // Y间距，自动根据X调整
        double moveY = 0;

        if (!RegPicture(血量_自身血量, 0, 0, 1920, 1080, 0.6))
            return new Point(Convert.ToInt16(moveX), Convert.ToInt16(moveY));

        var p = RegPicturePoint(血量_自身血量, 0, 0, 1920, 1080, 0.6);
        double realX = p.X + 55;
        double realY = p.Y + 117;

        //textBox4.Text = realX.ToString();
        //tb_delay.Text = realY.ToString();

        // 线性求解，保证移动鼠标为直线间的一点，无转身
        var a = (mousePosition.Y - realY) / (mousePosition.X - realX);

        moveX = realX < mousePosition.X ? -60 : 60;

        moveY = a * moveX;

        if (Math.Abs(moveY) > 60)
        {
            moveY = 60;
            moveX = moveY / a;
        }

        // 确保正负值正确
        moveY = Convert.ToDouble(mousePosition.Y) +
                (realY < mousePosition.Y ? -Math.Abs(moveY) : Math.Abs(moveY));
        moveX = Convert.ToDouble(mousePosition.X) +
                (realX < mousePosition.X ? -Math.Abs(moveX) : Math.Abs(moveX));

        tb_x.Text = moveX.ToString();
        tb_y.Text = moveY.ToString();

        return new Point(Convert.ToInt16(moveX), Convert.ToInt16(moveY));
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
            switch (list1[0])
            {
                case "6":
                    for (var i = 1; i < list1.Count; i++)
                        switch (list1[i])
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
                    for (var i = 1; i < list1.Count; i++)
                        switch (list1[i])
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
        for (var i = 1; i < list1.Count; i++)
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

    private static bool 切敏捷腿(in byte[] parByte, Size size, string mode = "4")
    {
        return 根据图片以及类别使用物品(物品_假腿_智力腿, parByte, size, mode) ||
               根据图片以及类别使用物品多次(物品_假腿_力量腿, parByte, size, 2, 15, mode);
    }

    private static bool 切智力腿(in byte[] parByte, Size size, string mode = "4")
    {
        return 根据图片以及类别使用物品(物品_假腿_力量腿, parByte, size, mode) ||
               根据图片以及类别使用物品多次(物品_假腿_敏捷腿, parByte, size, 2, 15, mode);
    }

    private static bool 切力量腿(in byte[] parByte, Size size, string mode = "4")
    {
        return 根据图片以及类别使用物品(物品_假腿_敏捷腿, parByte, size, mode) ||
               根据图片以及类别使用物品多次(物品_假腿_智力腿, parByte, size, 2, 15, mode);
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
    //    delay(30);
    //    KeyPress((uint)Keys.Q);
    //    delay(30);
    //    KeyPress((uint)Keys.F1);
    //}

    #endregion

    #region 使用物品

    private static bool 根据图片以及类别使用物品(Bitmap bp, in byte[] bts, Size size, string mode = "4")
    {
        var list = RegPicturePoint(bp, bts, size);
        if (list.Count <= 0) return false;
        根据物品位置按键(list[0], mode);
        list.Dispose();
        return true;
    }

    private static bool 根据图片以及类别使用物品多次(Bitmap bp, byte[] bts, Size size, int times, int delay, string mode = "4")
    {
        var list = RegPicturePoint(bp, bts, size);
        if (list.Count <= 0) return false;
        for (var i = 0; i < times; i++)
        {
            根据物品位置按键(list[0], mode);
            Delay(delay);
        }

        list.Dispose();
        return true;
    }

    private static void 根据物品位置按键(Point p, string mode = "4")
    {
        var x = p.X + 截图模式1X;
        var y = p.Y + 截图模式1Y;

        const int 第二行物品y = 990;

        switch (mode)
        {
            case "4":
                if (x < 1184)
                {
                    if (y > 第二行物品y)
                    {
                        KeyPress((uint)Keys.V);
                        break;
                    }

                    KeyPress((uint)Keys.Z);
                    break;
                }

                if (x > 1249)
                {
                    if (y > 第二行物品y)
                    {
                        KeyPress((uint)Keys.Space);
                        break;
                    }

                    KeyPress((uint)Keys.C);
                    break;
                }

                if (y > 第二行物品y)
                {
                    KeyPress((uint)Keys.B);
                    break;
                }

                KeyPress((uint)Keys.X);
                break;
            case "5":
                if (x < 1200)
                {
                    if (y > 第二行物品y)
                    {
                        KeyPress((uint)Keys.V);
                        break;
                    }

                    KeyPress((uint)Keys.Z);
                    break;
                }

                if (x > 1266)
                {
                    if (y > 第二行物品y)
                    {
                        KeyPress((uint)Keys.Space);
                        break;
                    }

                    KeyPress((uint)Keys.C);
                    break;
                }

                if (y > 第二行物品y)
                {
                    KeyPress((uint)Keys.B);
                    break;
                }

                KeyPress((uint)Keys.X);
                break;
            case "6":
                if (x < 1228)
                {
                    if (y > 第二行物品y)
                    {
                        KeyPress((uint)Keys.V);
                        break;
                    }

                    KeyPress((uint)Keys.Z);
                    break;
                }

                if (x > 1294)
                {
                    if (y > 第二行物品y)
                    {
                        KeyPress((uint)Keys.Space);
                        break;
                    }

                    KeyPress((uint)Keys.C);
                    break;
                }

                if (y > 第二行物品y)
                {
                    KeyPress((uint)Keys.B);
                    break;
                }

                KeyPress((uint)Keys.X);
                break;
        }
    }

    #endregion

    #region buff或者装备

    private static bool 智力跳刀buff(in byte[] bts, Size size)
    {
        return RegPicture(物品_跳刀_智力跳刀BUFF, bts, size);
    }

    /// <summary>
    /// </summary>
    /// <param name="bp">指定图片</param>
    /// <returns></returns>
    private static bool 阿哈利姆神杖(byte[] bts, Size size)
    {
        var x = 截图模式1X;
        var y = 截图模式1Y;

        var 技能点颜色 = Color.FromArgb(255, 28, 193, 254);

        if (GetPixelBytes(bts, size, 1077 - x, 963 - y).Equals(技能点颜色))
            return true;
        // 4技能魔晶

        return GetPixelBytes(bts, size, 1093 - x, 963 - y).Equals(技能点颜色) ||
               GetPixelBytes(bts, size, 1121 - x, 963 - y).Equals(技能点颜色);
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

        var 技能点颜色 = Color.FromArgb(255, 37, 181, 255);

        if (GetPixelBytes(bts, size, 1077 - x, 996 - y).Equals(技能点颜色))
            return true;
        // 4技能魔晶

        return GetPixelBytes(bts, size, 1093 - x, 996 - y).Equals(技能点颜色) ||
               GetPixelBytes(bts, size, 1121 - x, 996 - y).Equals(技能点颜色);
        // 5技能A帐魔晶（A帐魔晶6技能） // 6技能魔晶A
    }

    private static void 等待陨星锤结束()
    {
        var time = 获取当前时间毫秒();
        var wait_i = 0;
        while (wait_i == 0)
        {
            if (RegPicture(物品_释放陨星锤_持续施法, 785, 744, 51, 42))
            {
                Delay(2350);
                wait_i = 1;
            }

            if (获取当前时间毫秒() - time > 4000) break;
        }
    }

    private static void 渐隐期间放技能(uint c, int delay)
    {
        KeyPressAlt((uint)Keys.Z);
        Delay(delay);
        KeyPress(c);
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

    /// <summary>
    ///     基本上只需要1-9ms不等的延迟
    /// </summary>
    /// <param name="bp">原始图片</param>
    /// <param name="bp1">对比图片</param>
    /// <param name="matchRate">匹配率</param>
    /// <returns></returns>
    private static bool RegPicture(Bitmap bp, Bitmap bp1, double matchRate = 0.9)
    {
        return FindPictureParallel(bp, new Bitmap(bp1), matchRate: matchRate).Count > 0;
    }

    private static bool RegPicture(Bitmap bp, in byte[] bts, Size size, double matchRate = 0.9)
    {
        return FindBytesParallel(GetBitmapByte(bp), bp.Size, bts, size, matchRate: matchRate).Count > 0;
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

    #endregion

    #endregion

    #region 是否存在图片

    private static bool RegPicture(Bitmap bp, int x, int y, int width, int height, double matchRate = 0.9)
    {
        return FindPictureParallel(bp, CaptureScreen(x, y, width, height), matchRate: matchRate).Count > 0;
    }

    #endregion

    #region 返回图片实际坐标

    private static Point RegPicturePoint(Bitmap bp, int x, int y, int width, int height, double matchRate = 0.9)
    {
        try
        {
            var p = FindPictureParallel(bp, CaptureScreen(x, y, width, height), matchRate: matchRate)[0];
            return new Point(x + p.X, y + p.Y);
        }
        catch
        {
        }

        return new Point(-1, -1);
    }

    private static PooledList<Point> RegPicturePoint(Bitmap bp, in byte[] bts, Size size, double matchRate = 0.9)
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

    #endregion

    #region 返回数组对应颜色

    private static Color GetPixelBytes(in byte[] bts, Size size, int x, int y)
    {
        var subIndex = y * size.Width * 4 + x * 4;
        return Color.FromArgb(bts[subIndex + 3], bts[subIndex + 2],
            bts[subIndex + 1], bts[subIndex]);
    }

    private static Color GetPixelBytes(in byte[] bts, Size size, in Point p)
    {
        var subIndex = p.Y * size.Width * 4 + p.X * 4;
        return Color.FromArgb(bts[subIndex + 3], bts[subIndex + 2],
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

    #endregion

    #endregion

    #region 记录买活

    //private static void 记录买活()
    //{
    //    var 计时颜色 = Color.FromArgb(255, 14, 19, 24);

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
        Delay(30);
        KeyPress((uint)Keys.Enter);
        Delay(30);
    }

    #endregion

    #region 快速选择敌方英雄

    /// <summary>
    ///     基本需要时间 50ms 左右
    /// </summary>
    /// <param name="width"></param>
    /// <param name="hight"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool 快速选择敌方英雄(int width = 1920, int hight = 1080, int type = 0)
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

        CaptureScreen(x, y, ref bp);
        var bytes = new byte[4 * width * hight];
        GetBitmapByte(bp, ref bytes);

        var list = 获取敌方坐标(size, bytes);

        var 偏移x = 1920;
        var 偏移y = 1080;

        foreach (var item in list.Where(item =>
                     Math.Abs(item.X + x - p.X) + Math.Abs(item.Y + y - p.Y) < Math.Abs(偏移x) + Math.Abs(偏移y)))
        {
            偏移x = item.X + x - p.X;
            偏移y = item.Y + y - p.Y;
        }

        if (list.Count > 0)
        {
            if (type == 1)
                MouseMoveSim(p.X - 35 + 偏移x, p.Y + 80 + 偏移y);
            else
                MouseMove(p.X - 35 + 偏移x, p.Y + 80 + 偏移y);
        }

        list.Dispose();

        return true;
    }

    private static PooledList<Point> 获取敌方坐标(Size size, byte[] bytes)
    {
        return RegPicturePoint(血量_敌人等级底色, bytes, size, 0.3);
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

    private void 捕捉颜色()
    {
        var time = 获取当前时间毫秒();

        var colors = new PooledList<Color>();
        var longs = new PooledList<long>();

        var size = new Size(截图模式1W, 截图模式1H);
        var bitmap = new Bitmap(截图模式1W, 截图模式1H);
        CaptureScreen(截图模式1X, 截图模式1Y, ref bitmap);
        var bts = new byte[4 * 截图模式1W * 截图模式1H];
        GetBitmapByte(bitmap, ref bts);

        var x = 截图模式1X;
        var y = 截图模式1Y;
        // 最好的找色为CD读条的白色文字

        while (true)
        {
            CaptureScreen(截图模式1X, 截图模式1Y, ref bitmap);
            GetBitmapByte(bitmap, ref bts);

            var q4 = GetPixelBytes(bts, size, 827 - x, 976 - y);
            var q5 = GetPixelBytes(bts, size, 810 - x, 971 - y);
            var q6 = GetPixelBytes(bts, size, 780 - x, 971 - y);

            var w4 = GetPixelBytes(bts, size, 892 - x, 976 - y);
            var w5 = GetPixelBytes(bts, size, 868 - x, 971 - y);
            var w6 = GetPixelBytes(bts, size, 838 - x, 971 - y);

            var e4 = GetPixelBytes(bts, size, 957 - x, 976 - y);
            var e5 = GetPixelBytes(bts, size, 926 - x, 971 - y);
            var e6 = GetPixelBytes(bts, size, 896 - x, 971 - y);

            var r4 = GetPixelBytes(bts, size, 1022 - x, 976 - y);
            var r5 = GetPixelBytes(bts, size, 1042 - x, 971 - y);
            var r6 = GetPixelBytes(bts, size, 1070 - x, 971 - y);

            var d5 = GetPixelBytes(bts, size, 984 - x, 971 - y);
            var d6 = GetPixelBytes(bts, size, 954 - x, 971 - y);

            var f6 = GetPixelBytes(bts, size, 1012 - x, 971 - y);

            var p = tb_丢装备.Text.Trim() switch
            {
                "q4" => q4,
                "q5" => q5,
                "q6" => q6,
                "w4" => w4,
                "w5" => w5,
                "w6" => w6,
                "e4" => e4,
                "e5" => e5,
                "e6" => e6,
                "r4" => r4,
                "r5" => r5,
                "r6" => r6,
                "d5" => d5,
                "d6" => d6,
                "f6" => f6,
                _ => q4
            };

            if (colors.Count == 0 || !colors[^1].Equals(p))
            {
                colors.Add(p);
                longs.Add(获取当前时间毫秒() - time);
            }

            //if (
            //    ColorAEqualColorB(q5, Color.FromArgb(255, 99,170,68), 3, 5, 20)
            ////    &
            ////    !ColorAEqualColorB(q4_n, Color.FromArgb(50, 21, 23), 0) // 沉默不释放
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
    }

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
        if (time == -1)
            time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    #endregion

    #region 其他

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

    private void button1_Click(object sender, EventArgs e)
    {
        Run(捕捉颜色);
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

        _myKeyEventHandeler = Hook_KeyDown;
        _kHook.KeyDownEvent += _myKeyEventHandeler; // 绑定对应处理函数
        _kHook.Start(); // 安装键盘钩子

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

        // 338 1010 只显示三行
        // 设置窗口位置
        Location = new Point(338, 1010);

        if (tb_name.Text.Trim() == "测试")
        {
            Location = new Point(338, 820);
            label3.Text = "模式例:q4";
            label7.Text = "超时时间";
        }

        //Task.Run(记录买活);

        // 用于初始捕捉
        获取图片_1();

        // 用于文字识别
        初始化PaddleOcr();

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

        OCRParameter oCRParameter = new OCRParameter();
        oCRParameter.numThread = 6; //预测并发线程数
        oCRParameter.Enable_mkldnn = 1; //web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
        oCRParameter.use_angle_cls = 1; //是否开启方向检测，用于检测识别180旋转
        oCRParameter.det_db_score_mode = 1; //是否使用多段线，即文字区域是用多段线还是用矩形，

        //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。  
        _PaddleOcrEngine = new PaddleOCREngine(config, oCRParameter);
    }

    /// <summary>
    ///     取消监听和注销模拟
    /// </summary>
    private void StopListen()
    {
        _kHook.KeyDownEvent -= _myKeyEventHandeler; // 取消按键事件
        _myKeyEventHandeler = null;
        _kHook.Stop(); // 关闭键盘钩子

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

    private static void KeyPressWhile(uint key, uint key1)
    {
        SimEnigo.KeyPressWhile(key, key1);
    }

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