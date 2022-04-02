using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.Picture_Dota2;
using static Dota2Simulator.OCR;
using static Dota2Simulator.PictureProcessing;
using static Dota2Simulator.SetWindowTop;

namespace Dota2Simulator;

public partial class Form2 : Form
{
    /// 动态肖像 法线贴图 地面视差 主界面高画质 计算器渲染器 纹理、特效、阴影 中 渲染 70%
    ///


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
        #region 打字时屏蔽功能

        if (1==0
            // && CaptureColor(572, 771).Equals(Color.FromArgb(255, 237, 222, 190))
            )
        {
        }

        #endregion

        else
        {
            #region 记录时间

            //if (e.KeyValue == (uint)Keys.NumPad1)
            //{
            //    获取时间肉山();
            //}
            //else if (e.KeyValue == (uint)Keys.NumPad3)
            //{
            //    获取时间塔防();
            //}

            #endregion

            #region 力量

            #region 船长

            if (tb_name.Text == "船长")
            {
                if (e.KeyValue == (uint)Keys.D2)
                {
                    label1.Text = "D2";

                    KeyPress((uint)Keys.Q);

                    Task.Run(洪流接x回);
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    label1.Text = "D3";

                    Task.Run(最大化x伤害控制);
                }
                else if (e.KeyValue == (uint)Keys.D4)
                {
                    label1.Text = "D4";

                    KeyPress((uint)Keys.Q);

                    Task.Run(洪流接船);
                }
            }

            #endregion

            #region 军团

            else if (tb_name.Text == "军团")
            {
                if (e.KeyValue == (uint)Keys.E)
                {
                    label1.Text = "E";

                    Task.Run(决斗);
                }
                else if (e.KeyValue == (uint)Keys.D1)
                {
                    mod_int = 1;
                    TTS.Speak("左下决斗");
                }
                else if (e.KeyValue == (uint)Keys.D2)
                {
                    mod_int = 2;
                    TTS.Speak("左上决斗");
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    mod_int = 3;
                    TTS.Speak("右下决斗");
                }
                else if (e.KeyValue == (uint)Keys.D4)
                {
                    mod_int = 4;
                    TTS.Speak("右上决斗");
                }
            }

            #endregion

            #region 斧王

            else if (tb_name.Text == "斧王")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 跳吼;

                if (条件根据图片委托2 == null)
                    条件根据图片委托2 = 战斗饥渴取消后摇;

                if (e.KeyValue == (uint)Keys.E)
                {
                    条件1 = true;
                    中断条件 = false;
                }
                else if (e.KeyValue == (uint)Keys.W)
                {
                    条件2 = true;
                    中断条件 = false;
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    条件1 = false;
                    条件2= false;
                    中断条件 = true;
                }
            }

            #endregion

            #region 孽主

            else if (tb_name.Text == "孽主")
            {
                if (e.KeyValue == (uint)Keys.E)
                {
                    label1.Text = "E";

                    Task.Run(深渊火雨阿托斯);
                }
            }

            #endregion

            #region 哈斯卡

            else if (tb_name.Text.Trim() == "哈斯卡")
            {
                if (e.KeyValue == (uint)Keys.D2)
                {
                    label1.Text = "D2";

                    Task.Run(切臂章);
                }
                else if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";

                    Task.Run(心炎平A);
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    label1.Text = "R";

                    if (RegPicture(Resource_Picture.物品_臂章, "Z"))
                    {
                        KeyPress((uint)Keys.Z);
                        Delay(30);
                    }

                    Task.Run(牺牲平A刃甲);
                }
            }

            #endregion

            #region 海民

            else if (tb_name.Text.Trim() == "海民")
            {
                if (e.KeyValue == (uint)Keys.G)
                {
                    label1.Text = "G";

                    Task.Run(跳接勋章接摔角行家);
                }
            }

            #endregion

            #region 钢背

            else if (tb_name.Text.Trim() == "钢背")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 鼻涕针刺循环;

                if (e.KeyValue == (uint)Keys.D2)
                {
                    if (!条件1)
                        条件1 = true;

                    循环条件1 = !循环条件1;
                    // 基本上魂戒可以放4下，只浪费10点蓝
                    // 配合一次鼻涕就一次也不浪费
                    if (循环条件1)
                        if (RegPicture(Resource_Picture.物品_魂戒CD, "C") || RegPicture(Resource_Picture.物品_魂戒CD_5, "C", 5))
                            KeyPress((uint)Keys.C);
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    if (!条件1)
                        条件1 = true;

                    循环条件2 = !循环条件2;
                }
                else if (e.KeyValue == (uint)Keys.H)
                {
                    循环条件1 = false;
                    循环条件2 = false;
                }
            }

            #endregion

            #region 猛犸

            else if (tb_name.Text.Trim() == "猛犸")
            {
                if (e.KeyValue == (uint)Keys.F)
                {
                    label1.Text = "F";

                    Task.Run(跳拱指定地点);
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    label1.Text = "F";

                    Task.Run(指定地点);
                }
            }

            #endregion

            #endregion

            #region 敏捷

            #region 露娜

            if (tb_name.Text == "露娜")
            {
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";

                    切智力腿();

                    Task.Run(月光后敏捷平A);
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    label1.Text = "R";

                    切智力腿();

                    Task.Run(月蚀后敏捷平A);
                }
            }

            #endregion

            #region 影魔

            if (tb_name.Text == "影魔")
            {
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
            }

            #endregion

            #region 敌法

            else if (tb_name.Text == "敌法")
            {
                if (e.KeyValue == (uint) Keys.W)
                {
                    label1.Text = "W";
                    切智力腿();
                    Task.Run(闪烁敏捷);
                }
                else if (e.KeyValue == (uint) Keys.E)
                {
                    label1.Text = "E";
                    // 太过明显,故不使用
                    //切智力腿();
                    //Task.Run(法术反制敏捷);
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    label1.Text = "R";
                    //切智力腿();
                    Task.Run(法力虚空取消后摇);
                }
                else if (e.KeyValue == (uint) Keys.X)
                {
                    label1.Text = "X";
                    Task.Run(分身一齐攻击);
                }
            }

            #endregion

            #region 巨魔

            else if (tb_name.Text.Trim() == "巨魔")
            {
                if (e.KeyValue == (uint) Keys.D)
                {
                    label1.Text = "D";

                    Task.Run(远程飞斧);
                }
            }

            #endregion

            #region 小骷髅

            else if (tb_name.Text.Trim() == "小骷髅")
            {
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";

                    切智力腿();

                    Task.Run(扫射接勋章);
                }
                else if (e.KeyValue == (uint) Keys.E)
                {
                    label1.Text = "E";

                    切智力腿();
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    label1.Text = "R";

                    切智力腿();
                }
            }

            #endregion

            #region 小松鼠

            else if (tb_name.Text.Trim() == "小松鼠")
            {
                if (e.KeyValue == (uint) Keys.D2)
                {
                    label1.Text = "D2";

                    Task.Run(捆接种树);
                }
                else if (e.KeyValue == (uint) Keys.D3)
                {
                    label1.Text = "D3";

                    Task.Run(飞镖接捆接种树);
                }
            }

            #endregion

            #region 拍拍

            else if (tb_name.Text.Trim() == "拍拍")
            {
                if (e.KeyValue == (uint) Keys.W)
                {
                    label1.Text = "W";

                    切智力腿();

                    Task.Run(超强力量平A);
                }
                else if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";

                    切智力腿();

                    Task.Run(震撼大地接平A);
                }
            }

            #endregion

            #region 小鱼人

            else if (tb_name.Text.Trim() == "小鱼人")
            {
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "黑暗契约";

                    切智力腿();

                    Task.Run(黑暗契约力量);
                }
                else if (e.KeyValue == (uint) Keys.W || e.KeyValue == (uint) Keys.R)
                {
                    label1.Text = "释放接平A";

                    切智力腿();

                    Task.Run(跳水敏捷);
                }

                else if (e.KeyValue == (uint) Keys.D2)
                {
                    label1.Text = "W";

                    切智力腿();

                    // 径直移动键位
                    KeyDown((uint) Keys.L);

                    // 径直移动
                    RightClick();

                    // 基本上180°310  90°170 75°135 转身定点，配合A杖效果极佳
                    Delay(170);

                    KeyUp((uint) Keys.L);

                    KeyPress((uint) Keys.W);
                }
                else if (e.KeyValue == (uint) Keys.D)
                {
                    label1.Text = "D";

                    切智力腿();

                    Task.Run(深海护罩敏捷);
                }
            }

            #endregion

            #region 猴子

            else if (tb_name.Text.Trim() == "猴子")
            {
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";

                    切智力腿();

                    Task.Run(灵魂之矛敏捷);
                }
                else if (e.KeyValue == (uint) Keys.D)
                {
                    label1.Text = "D";

                    切智力腿();

                    KeyPress((uint) Keys.W);

                    Task.Run(神行百变敏捷);
                }
                else if (e.KeyValue == (uint) Keys.F)
                {
                    label1.Text = "F";

                    RightClick();

                    Task.Run(后撤触发冲锋);
                }
            }

            #endregion

            #region 水人

            else if (tb_name.Text == "水人")
            {
            }

            #endregion

            #region 幻刺

            else if (tb_name.Text == "幻刺")
            {
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";
                    切智力腿();
                    Task.Run(窒息短匕敏捷);
                }
                else if (e.KeyValue == (uint) Keys.W)
                {
                    label1.Text = "W";
                    切智力腿();
                    Task.Run(幻影突袭敏捷);
                }
                else if (e.KeyValue == (uint) Keys.E)
                {
                    label1.Text = "E";
                    切智力腿();
                    Task.Run(魅影无形敏捷);
                }
                else if (e.KeyValue == (uint) Keys.D)
                {
                    label1.Text = "E";
                    切智力腿();
                    Task.Run(刀阵旋风敏捷);
                }
            }

            #endregion

            #region 虚空

            else if (tb_name.Text == "虚空")
            {
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";
                    切智力腿();
                    Task.Run(时间漫游敏捷);
                }
                else if (e.KeyValue == (uint) Keys.W)
                {
                    label1.Text = "W";
                    切智力腿();
                    Task.Run(时间膨胀敏捷);
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    label1.Text = "R";
                    切智力腿();
                    Task.Run(时间结界敏捷);
                }
            }

            #region TB

            else if (tb_name.Text == "TB")
            {
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";
                    切智力腿();
                    Task.Run(倒影敏捷);
                }
                else if (e.KeyValue == (uint) Keys.W)
                {
                    label1.Text = "W";
                    切智力腿();
                    Task.Run(幻惑敏捷);
                }
                else if (e.KeyValue == (uint) Keys.E || e.KeyValue == (uint) Keys.F)
                {
                    label1.Text = "E";
                    切智力腿();
                    Task.Run(魔化敏捷);
                }
                else if (e.KeyValue == (uint) Keys.D)
                {
                    label1.Text = "D";
                    Task.Run(恶魔狂热去后摇);
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    label1.Text = "R";
                    切智力腿();
                    Task.Run(断魂敏捷);
                }
            }

            #endregion

            #endregion

            #region 赏金

            else if (tb_name.Text == "赏金")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 飞镖接平A;

                if (条件根据图片委托2 == null)
                    条件根据图片委托2 = 标记去后摇;

                if (e.KeyValue == (uint) Keys.Q)
                    条件1 = true;
                else if (e.KeyValue == (uint) Keys.R) 条件2 = true;
            }

            #endregion

            #region 剧毒

            else if (tb_name.Text == "剧毒")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 瘴气去后摇;

                if (条件根据图片委托2 == null)
                    条件根据图片委托2 = 蛇棒去后摇;

                if (条件根据图片委托3 == null)
                    条件根据图片委托3 = 剧毒新星去后摇;

                if (条件根据图片委托4 == null)
                    条件根据图片委托4 = 循环蛇棒;

                if (e.KeyValue == (uint) Keys.Q)
                {
                    中断条件 = false;
                    条件1 = true;
                }
                else if (e.KeyValue == (uint) Keys.E)
                {
                    中断条件 = false;
                    条件2 = true;
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    中断条件 = false;
                    条件3 = true;
                }
                else if (e.KeyValue == (uint) Keys.D3)
                {
                    中断条件 = false;
                    循环条件1 = !循环条件1;
                    if (循环条件1)
                        条件4 = true;
                }
                else if (e.KeyValue == (uint) Keys.S)
                {
                    中断条件 = true;
                    条件1 = false;
                    条件2 = false;
                    条件3 = false;
                    条件4 = false;
                }
            }

            #endregion

            #endregion

            #region 智力

            #region 黑鸟

            else if (tb_name.Text == "黑鸟")
            {
                if (e.KeyValue == (uint) Keys.D)
                {
                    label1.Text = "D";
                    中断条件 = false;
                    Task.Run(G_yxc_y);
                    // 普通砸锤
                }
                else if (e.KeyValue == (uint) Keys.F)
                {
                    label1.Text = "F";
                    中断条件 = false;
                    Task.Run(G_yxc_cg);
                }
                else if (e.KeyValue == (uint) Keys.D2)
                {
                    Task.Run(关接跳);
                }
                else if (e.KeyValue == (uint) Keys.H)
                {
                    中断条件 = true;
                }
            }

            #endregion

            #region 谜团

            else if (tb_name.Text == "谜团")
            {
                if (e.KeyValue == (uint) Keys.D)
                {
                    label1.Text = "D";
                    Task.Run(跳秒接午夜凋零黑洞);
                }
                else if (e.KeyValue == (uint) Keys.F)
                {
                    label1.Text = "F";
                    Task.Run(刷新接凋零黑洞);
                }
            }

            #endregion

            #region 冰女

            else if (tb_name.Text.Trim() == "冰女")
            {
                if (e.KeyValue == (uint) Keys.E)
                {
                    label1.Text = "E";

                    Task.Run(冰封禁制接陨星锤);
                }
            }

            #endregion

            #region 火女

            else if (tb_name.Text.Trim() == "火女")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 龙破斩去后摇;

                if (条件根据图片委托2 == null)
                    条件根据图片委托2 = 光击阵去后摇;

                if (条件根据图片委托3 == null)
                    条件根据图片委托3 = 神灭斩去后摇;

                if (e.KeyValue == (uint) Keys.Q)
                {
                    中断条件 = false;
                    条件1 = true;
                }
                else if (e.KeyValue == (uint) Keys.W)
                {
                    中断条件 = false;
                    条件2 = true;
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    中断条件 = false;
                    条件3 = true;
                }
                else if (e.KeyValue == (uint) Keys.S)
                {
                    中断条件 = true;
                    条件1 = false;
                    条件2 = false;
                    条件3 = false;
                }
            }

            #endregion

            #region 蓝猫

            else if (tb_name.Text.Trim() == "蓝猫")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 拉接平A;

                if (条件根据图片委托2 == null)
                    条件根据图片委托2 = 滚接平A;

                if (条件根据图片委托3 == null)
                    条件根据图片委托3 = 快速回城;

                if (条件根据图片委托4 == null)
                    条件根据图片委托4 = 魂戒力量智力;

                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";
                    Task.Run(残影接平A);
                }
                else if (e.KeyValue == (uint) Keys.W)
                {
                    中断条件 = false;
                    条件1 = true;
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    中断条件 = false;
                    条件2 = true;
                }
                else if (e.KeyValue == (uint) Keys.D4)
                {
                    中断条件 = false;
                    条件3 = true;
                }
                else if (e.KeyValue == (uint) Keys.D2)
                {
                    中断条件 = false;
                    条件4 = true;
                }
                else if (e.KeyValue == (uint) Keys.S)
                {
                    中断条件 = true;
                    条件1 = false;
                    条件2 = false;
                    条件3 = false;
                    条件4 = false;
                }
                //else if (e.KeyValue == (uint) Keys.F)
                //{
                //    label1.Text = "F";
                //    Task.Run(原地滚A);
                //}
                else if (e.KeyValue == (uint) Keys.D)
                {
                    Task.Run(泉水状态喝瓶);
                }
                else if (e.KeyValue == (uint) Keys.F)
                {
                    if (!丢装备条件)
                    {
                        Task.Run(批量扔装备);
                        丢装备条件 = !丢装备条件;
                    }
                    else
                    {
                        Task.Run(捡装备);
                        丢装备条件 = !丢装备条件;
                    }
                }
            }

            #endregion

            #region 宙斯

            else if (tb_name.Text.Trim() == "宙斯")
            {
                // 弧形闪电和雷击都是不朽
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";

                    Task.Run(弧形闪电去后摇);
                }
                else if (e.KeyValue == (uint) Keys.W)
                {
                    label1.Text = "W";

                    Task.Run(雷击去后摇);
                }
            }

            #endregion

            #region 卡尔

            else if (tb_name.Text.Trim() == "卡尔")
            {
                if (e.KeyValue == (uint) Keys.D2)
                {
                    label1.Text = "D2";

                    Task.Run(三冰对线);
                }
                else if (e.KeyValue == (uint) Keys.D3)
                {
                    label1.Text = "D2";

                    Task.Run(三火平A);
                }
                else if (e.KeyValue == (uint) Keys.D1)
                {
                    label1.Text = "D2";

                    Task.Run(三雷幽灵);
                }
                else if (e.KeyValue == (uint)Keys.D4)
                {
                    label1.Text = "D2";

                    Task.Run(吹风天火);
                }
            }

            #endregion

            #region 拉席克

            else if (tb_name.Text.Trim() == "拉席克")
            {
                if (e.KeyValue == (uint) Keys.F)
                {
                    label1.Text = "F";

                    中断条件 = false;

                    Task.Run(吹风接撕裂大地);
                }
                else if (e.KeyValue == (uint) Keys.S)
                {
                    中断条件 = true;
                }
            }

            #endregion

            #region 暗影萨满

            else if (tb_name.Text.Trim() == "暗影萨满")
            {
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";

                    Task.Run(苍穹振击取消后摇);
                }
                else if (e.KeyValue == (uint) Keys.W)
                {
                    label1.Text = "W";

                    Task.Run(变羊取消后摇);
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    label1.Text = "R";

                    Task.Run(释放群蛇守卫取消后摇);
                }
                else if (e.KeyValue == (uint) Keys.D)
                {
                    label1.Text = "D";

                    Task.Run(暗夜萨满最大化控制链);
                }
                else if (e.KeyValue == (uint) Keys.S)
                {
                    循环条件2 = false;
                }
            }

            #endregion

            #region 小仙女

            else if (tb_name.Text.Trim() == "小仙女")
            {
                if (e.KeyValue == (uint) Keys.D2)
                {
                    label1.Text = "D2";

                    循环条件2 = true;

                    Task.Run(诅咒皇冠吹风);
                }

                if (e.KeyValue == (uint) Keys.D9)
                {
                    label1.Text = "D3";

                    循环条件2 = true;

                    Task.Run(作祟暗影之境最大化伤害);
                }
                else if (e.KeyValue == (uint) Keys.S)
                {
                    循环条件2 = false;
                }
                else if (e.KeyValue == (uint) Keys.E)
                {
                    Task.Run(皇冠延时计时);
                }
            }

            #endregion

            #region 天怒

            else if (tb_name.Text.Trim() == "天怒")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托2 == null)
                    条件根据图片委托2 = 天怒秒人连招;

                if (e.KeyValue == (uint) Keys.D3)
                    条件2 = true;
                //else if (e.KeyValue == (uint) Keys.D2)
                //    条件3 = true;
                else if (e.KeyValue == (uint) Keys.S)
                    for (var i = 0; i < 2; i++)
                    {
                        条件根据图片委托2 = null;
                        条件3 = false;
                        条件2 = false;
                        Delay(60); // 等待程序内延迟结束
                    }
            }

            #endregion

            #region 炸弹人

            else if (tb_name.Text.Trim() == "炸弹人")
            {
                if (e.KeyValue == (uint) Keys.D2) 魂戒丢装备();
            }

            #endregion

            #region 神域

            else if (tb_name.Text.Trim() == "神域")
            {
                if (e.KeyValue == (uint) Keys.W)
                    Task.Run(命运敕令去后摇);
                else if (e.KeyValue == (uint) Keys.E)
                    Task.Run(涤罪之焰去后摇);
                else if (e.KeyValue == (uint) Keys.R) Task.Run(虚妄之诺去后摇);
            }

            #endregion

            #region 修补匠

            else if (tb_name.Text.Trim() == "修补匠")
            {
                if (e.KeyValue == (uint) Keys.R)
                {
                    KeyPress((uint) Keys.C);
                    KeyPress((uint) Keys.V);
                    Task.Run(刷新完跳);
                }
                else if (e.KeyValue == (uint) Keys.D1)
                {
                    条件1 = !条件1;
                    if (条件1)
                        TTS.Speak("开启刷导弹");
                    else
                        TTS.Speak("关闭刷导弹");
                }
                else if (e.KeyValue == (uint) Keys.D2)
                {
                    条件2 = !条件2;
                    if (条件2)
                        TTS.Speak("开启刷跳");
                    else
                        TTS.Speak("关闭刷跳");
                }
                else if (e.KeyValue == (uint) Keys.D3)
                {
                    条件3 = !条件3;
                    if (条件3)
                        TTS.Speak("开启希瓦");
                    else
                        TTS.Speak("关闭希瓦");
                }
                else if (e.KeyValue == (uint) Keys.X)
                {
                    Task.Run(推推接刷新);
                }
                else if (e.KeyValue == (uint) Keys.D1)
                {
                    Task.Run(检测敌方英雄自动导弹);
                }
            }

            #endregion

            #region 莱恩

            else if (tb_name.Text.Trim() == "莱恩")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 莱恩羊接技能;


                if (e.KeyValue == (uint)Keys.D3)
                {
                    if (!条件4)
                    {
                        条件4 = true;
                        TTS.Speak("开启羊接吸");
                    }
                    else
                    {
                        条件4 = false;
                        TTS.Speak("开启羊接A");
                    }
                }
                else if (e.KeyValue == (uint)Keys.W)
                {
                    条件1 = true;
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    条件1 = false;
                }
                else if (e.KeyValue == (uint) Keys.R)
                {
                    大招前纷争(new Bitmap(全局图像));
                }
                else if (e.KeyValue == (uint) Keys.D2)
                {
                    推推破林肯秒羊(new Bitmap(全局图像));
                    KeyPress((uint) Keys.W);
                }
            }

            #endregion

            #endregion

            #region 保存微信图片

            else if (tb_name.Text.Trim() == "微信图片")
            {
                if (e.KeyValue == (uint) Keys.Q)
                {
                    label1.Text = "Q";

                    中断条件 = false;

                    Task.Run(一键保存图片);
                }

                if (e.KeyValue == (uint) Keys.A)
                {
                    label1.Text = "A";
                    中断条件 = true;
                }
            }

            #endregion

            #region 切假腿

            else if (tb_name.Text.Trim() == "切假腿")
            {
                if (e.KeyValue == (uint) Keys.Q || e.KeyValue == (uint) Keys.W || e.KeyValue == (uint) Keys.E ||
                    e.KeyValue == (uint) Keys.D || e.KeyValue == (uint) Keys.F || e.KeyValue == (uint) Keys.R)
                    切智力腿();
            }

            #endregion

            #region 测试

            else if (tb_name.Text.Trim() == "测试")
            {
                if (e.KeyValue == (uint) Keys.X)
                {
                    tb_状态抗性.Text = "";
                    tb_丢装备.Text = "";
                    捕捉颜色();
                }
            }

            #endregion
        }
    }

    #endregion

    #region 起始加载图片

    /// <summary>
    ///     更改起始加载图片
    /// </summary>
    private void Change_Pic()
    {
        switch (tb_name.Text)
        {
            case "影魔":
                pictureBox1.Image = Resource_Picture.物品_吹风CD;
                break;
            case "黑鸟":
                pictureBox1.Image = Resource_Picture.黑鸟_关释放;
                break;
            case "军团":
                pictureBox1.Image = Resource_Picture.军团_释放强攻;
                break;
            case "孽主":
                pictureBox1.Image = Resource_Picture.孽主_释放深渊;
                break;
            case "巨魔":
                pictureBox1.Image = Resource_Picture.巨魔_远斧头;
                break;
            case "冰女":
                pictureBox1.Image = Resource_Picture.冰女_释放冰封禁制;
                break;
            case "蓝猫":
                pictureBox1.Image = Resource_Picture.蓝猫_释放电子漩涡;
                break;
            case "小骷髅":
                pictureBox1.Image = Resource_Picture.物品_假腿_敏捷腿;
                break;
            case "小鱼人":
            case "拍拍":
                pictureBox1.Image = Resource_Picture.物品_假腿_敏捷腿;
                break;
            case "宙斯":
                pictureBox1.Image = Resource_Picture.宙斯_释放弧形闪电;
                break;
        }
    }

    #endregion

    #region 其他

    private void 一键保存图片()
    {
        while (!RegPicture(Resource1.最后一张, 1132, 947, 40, 40, 1))
        {
            KeyDown((uint) Keys.LControlKey);

            KeyPress((uint) Keys.S);

            KeyUp((uint) Keys.LControlKey);

            Delay(250);

            KeyPress((uint) Keys.Enter);

            Delay(500);

            KeyPress((uint) Keys.Right);

            Delay(500);

            if (中断条件) break;
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
        if (time == -1) time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        while (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= delay)
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


    #region 局部全局变量

    /// <summary>
    ///     用于捕获按键
    /// </summary>
    private readonly KeyboardHook k_hook = new();

    /// <summary>
    ///     循环计数total
    /// </summary>
    private bool 总循环条件;

    /// <summary>
    ///     循环计数1
    /// </summary>
    private bool 循环条件1;

    /// <summary>
    ///     循环计数2
    /// </summary>
    private bool 循环条件2;

    /// <summary>
    ///     全局图像
    /// </summary>
    private static Bitmap 全局图像;

    /// <summary>
    ///     获取图片委托
    /// </summary>
    /// <returns></returns>
    private delegate void GetBitmap();

    /// <summary>
    ///     获取图片委托
    /// </summary>
    private GetBitmap getBitmap;

    /// <summary>
    ///     条件委托图片
    /// </summary>
    /// <param name="bp"></param>
    private delegate bool condition_delegate_bitmap(Bitmap bp);

    /// <summary>
    ///     条件1委托
    /// </summary>
    private condition_delegate_bitmap 条件根据图片委托1;

    /// <summary>
    ///     条件2委托
    /// </summary>
    private condition_delegate_bitmap 条件根据图片委托2;

    /// <summary>
    ///     条件3委托
    /// </summary>
    private condition_delegate_bitmap 条件根据图片委托3;

    /// <summary>
    ///     条件4委托
    /// </summary>
    private condition_delegate_bitmap 条件根据图片委托4;

    /// <summary>
    ///     条件5委托
    /// </summary>
    private condition_delegate_bitmap 条件根据图片委托5;

    /// <summary>
    ///     条件6委托
    /// </summary>
    private condition_delegate_bitmap 条件根据图片委托6;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private bool 条件1;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private bool 条件2;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private bool 条件3;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private bool 条件4;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private bool 条件5;

    /// <summary>
    ///     条件布尔
    /// </summary>
    private bool 条件6;

    /// <summary>
    ///     中断条件布尔
    /// </summary>
    private bool 中断条件;

    /// <summary>
    ///     丢装备条件布尔
    /// </summary>
    private bool 丢装备条件;


    ///// <summary>
    /////     模拟按键
    ///// </summary>
    //private readonly IPressKey mPressKey;

    /// <summary>
    ///     按键钩子，用于捕获按下的键
    /// </summary>
    private KeyEventHandler myKeyEventHandeler; //按键钩子

    ///// <summary>
    /////     用于生成随机数
    ///// </summary>
    //private RandomGenerator randomGenerator = new();

    /// <summary>
    ///     全局时间
    /// </summary>
    //private long 全局时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

    /// <summary>
    ///     用于跳拱地点
    /// </summary>
    private static Point 指定地点_P = new(0, 0);

    /// <summary>
    ///     用于不同设定
    /// </summary>
    private static int mod_int;

    /// <summary>
    ///     用于寻找最佳延迟
    /// </summary>
    private static int delay_time = 0;

    #endregion

    #region Dota2具体实现

    #region 力量

    #region 猛犸

    private static void 跳拱指定地点()
    {
        KeyPress((uint) Keys.Space);
        Delay(30);
        KeyPress((uint) Keys.D9);
        MouseMove(指定地点_P);
        Delay(30);
        KeyPress((uint) Keys.E);
        Delay(30);
        KeyPress((uint) Keys.D9);
    }

    #endregion

    #region 船长

    private static void 洪流接x回()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;
        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.船长_释放洪流_4, "Q") || RegPicture(Resource_Picture.船长_释放洪流_5, "Q", 5)
                                                            || RegPicture(Resource_Picture.船长_释放洪流_8, "Q", 8))
            {
                Delay(1200);
                KeyPress((uint) Keys.E);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1300) break;
        }
    }

    private static void 洪流接船()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;
        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.船长_释放洪流_4, "Q") || RegPicture(Resource_Picture.船长_释放洪流_5, "Q", 5)
                                                            || RegPicture(Resource_Picture.船长_释放洪流_8, "Q", 8))
            {
                Delay(45);
                KeyPress((uint) Keys.R);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1300) break;
        }
    }

    private void 最大化x伤害控制()
    {
        KeyPress((uint) Keys.E);

        var all_down = 0;
        while (all_down == 0)
            if (RegPicture(Resource_Picture.船长_释放x标记_4, "E") || RegPicture(Resource_Picture.船长_释放x标记_5, "E", 5)
                                                             || RegPicture(Resource_Picture.船长_释放x标记_8, "E", 8))
            {
                var kx = Convert.ToDouble(tb_状态抗性.Text.Trim());

                var x_持续时间 = Convert.ToInt32(4000 * (100 - kx) / 100);

                if (RegPicture(Resource_Picture.物品_陨星锤_4, "SPACE") || RegPicture(Resource_Picture.物品_陨星锤_5, "SPACE", 5)
                                                                   || RegPicture(Resource_Picture.物品_陨星锤_8, "SPACE", 8))
                {
                    if (x_持续时间 >= 3000)
                        Delay(x_持续时间 - 3000 + 60);
                    // 增加延时，因为时间对不上。。

                    KeyPress((uint) Keys.Space);

                    等待陨星锤结束();

                    KeyPress((uint) Keys.D4);
                }
                else
                {
                    if (x_持续时间 >= 2000)
                        Delay(x_持续时间 - 2000 + 60);
                    // 增加延时，因为时间对不上。。

                    KeyPress((uint) Keys.D4);
                }

                all_down = 1;
            }
    }

    #endregion

    #region 斧王

    private static bool 跳吼(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.物品_刃甲, bp))
            KeyPress((uint) Keys.Z);
        //Delay(30);

        KeyPress((uint) Keys.Space); 

        var s_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        if (RegPicture(Resource_Picture.斧王_狂战士之吼, bp) || RegPicture(Resource_Picture.斧王_狂战士之吼_金色饰品, bp))
        {
            KeyPress((uint) Keys.Q);
            Delay(550, s_time);
            //RightClick();
            KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    private static bool 战斗饥渴取消后摇(Bitmap bp)
    {
        var s_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        // 检测刚释放完毕
        if (RegPicture(Resource_Picture.斧王_释放战斗饥渴_不朽, bp))
        {
            Delay(290, s_time);
            //RightClick();
            KeyPress((uint) Keys.A);
            return false;
        }

        return true;
    }

    #endregion

    #region 军团

    private void 决斗()
    {
        单次使用装备(Resource_Picture.物品_魂戒CD);

        单次使用装备(Resource_Picture.物品_臂章);

        var wDown = 0;

        if (RegPicture(Resource_Picture.军团_强攻CD, "W"))
        {
            KeyPress((uint) Keys.D);
            KeyPress((uint) Keys.D);

            while (wDown == 0)
                if (RegPicture(Resource_Picture.军团_释放强攻, "W"))
                {
                    Delay(95);
                    wDown = 1;
                }
        }

        单次使用装备(Resource_Picture.物品_黑皇);

        单次使用装备(Resource_Picture.物品_相位);

        单次使用装备(Resource_Picture.物品_刃甲);

        //var p = 正面跳刀_无转身();

        if (mod_int == 0)
        {
            KeyPress((uint) Keys.Space);
        }
        else if (mod_int == 1)
        {
            var point = MousePosition;
            MouseMove(point.X - 50, point.Y + 50);
            KeyPress((uint) Keys.Space);
            Delay(5);
            MouseMove(point.X, point.Y);
        }
        else if (mod_int == 2)
        {
            var point = MousePosition;
            MouseMove(point.X - 50, point.Y - 50);
            KeyPress((uint) Keys.Space);
            Delay(5);
            MouseMove(point.X, point.Y);
        }
        else if (mod_int == 3)
        {
            var point = MousePosition;
            MouseMove(point.X + 50, point.Y + 50);
            KeyPress((uint) Keys.Space);
            Delay(5);
            MouseMove(point.X, point.Y);
        }
        else if (mod_int == 4)
        {
            var point = MousePosition;
            MouseMove(point.X + 50, point.Y - 50);
            KeyPress((uint) Keys.Space);
            Delay(5);
            MouseMove(point.X, point.Y);
        }

        持续使用装备直到超时(Resource_Picture.物品_否决, 150);

        持续使用装备直到超时(Resource_Picture.物品_天堂, 150);

        持续使用装备直到超时(Resource_Picture.物品_勇气, 150);

        持续使用装备直到超时(Resource_Picture.物品_炎阳, 150);

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.军团_决斗CD, "R"))
        {
            KeyPress((uint) Keys.R);
            Delay(30);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 450) break;
        }

        KeyPress((uint) Keys.A);
    }

    #endregion

    #region 孽主

    private void 深渊火雨阿托斯()
    {
        var all_done = 0;

        KeyPress((uint) Keys.W);

        while (all_done == 0)
            if (RegPicture(Resource_Picture.孽主_释放深渊, 857, 939, 70, 72))
            {
                Delay(400);
                KeyPress((uint) Keys.A);

                KeyPress((uint) Keys.Q);

                Delay(640);

                KeyPress((uint) Keys.A);
                Delay(800);

                KeyPress((uint) Keys.X);

                all_done = 1;
            }
    }

    #endregion

    #region 哈斯卡

    private void 心炎平A()
    {
        KeyPress((uint) Keys.A);

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.哈斯卡_释放心炎, "Q"))
            {
                Delay(110);

                KeyPress((uint) Keys.A);

                q_down = 1;

                label1.Text = "QQQ";
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

            break;
        }
    }

    private void 牺牲平A刃甲()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.哈斯卡_释放牺牲, "R"))
            {
                Delay(400);

                while (RegPicture(Resource_Picture.物品_刃甲, "X"))
                {
                    KeyPress((uint) Keys.X);
                    Delay(30);
                }

                q_down = 1;

                label1.Text = "RRR";
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 900) continue;

            break;
        }
    }

    #endregion

    #region 海民

    private void 跳接勋章接摔角行家()
    {
        if (RegPicture(Resource_Picture.物品_臂章, "Z"))
        {
            KeyPress((uint) Keys.Z);
            Delay(30);
        }

        var p = 正面跳刀_无转身();

        var point = MousePosition;

        MouseMove(p.X, p.Y);

        // 跳刀空格
        KeyPress((uint) Keys.Space);

        Delay(5);

        MouseMove(point.X, point.Y);

        // 勋章放C
        KeyPress((uint) Keys.C);
        KeyPress((uint) Keys.E);
        KeyPress((uint) Keys.A);
    }

    #endregion

    #region 钢背

    private bool 鼻涕针刺循环(Bitmap bp)
    {
        bool 是否魔晶, 是否A杖;

        var x = 750;
        var y = 856;
        var q5 = bp.GetPixel(775 - x, 994 - y);
        var q4 = bp.GetPixel(807 - x, 994 - y);
        var w5 = bp.GetPixel(839 - x, 994 - y);
        var w4 = bp.GetPixel(871 - x, 994 - y);
        是否魔晶 = 阿哈利姆魔晶(bp);
        是否A杖 = 阿哈利姆神杖(bp);

        if (循环条件1)
        {
            if (是否魔晶)
            {
                if (ColorAEqualColorB(w5, Color.FromArgb(255, 79, 74, 73), 8)
                   )
                {
                    KeyPress((uint) Keys.W);
                    Delay(30);
                }

                if (RegPicture(Resource_Picture.钢背_针刺CD_5, bp))
                {
                    KeyPress((uint) Keys.W);
                    Delay(30);
                }
            }
            else
            {
                if (ColorAEqualColorB(w4, Color.FromArgb(255, 80, 76, 75), 8)
                   )
                {
                    KeyPress((uint) Keys.W);
                    Delay(30);
                }

                if (RegPicture(Resource_Picture.钢背_针刺CD, bp))
                {
                    KeyPress((uint) Keys.W);
                    Delay(30);
                }
            }
        }

        if (是否A杖 && 循环条件2)
        {
            if (是否魔晶)
            {
                if (
                    ColorAEqualColorB(q5, Color.FromArgb(255, 64, 61, 55)) // 不朽颜色变化
                )
                {
                    KeyPress((uint) Keys.Q);
                    Delay(30);
                }

                if (RegPicture(Resource_Picture.钢背_鼻涕CD_5_不朽, bp))
                {
                    KeyPress((uint) Keys.Q);
                    Delay(30);
                }
            }
            else
            {
                if (
                    ColorAEqualColorB(q4, Color.FromArgb(255, 64, 61, 55)) // 不朽颜色变化
                )
                {
                    KeyPress((uint) Keys.Q);
                    Delay(30);
                }

                if (RegPicture(Resource_Picture.钢背_鼻涕CD_不朽, bp))
                {
                    KeyPress((uint) Keys.Q);
                    Delay(30);
                }
            }
        }

        return true;
    }

    #endregion

    #endregion

    #region 敏捷

    #region 露娜

    private void 月光后敏捷平A()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.露娜_释放月光, "Q"))
            {
                Delay(100);
                切敏捷腿();
                KeyPress((uint) Keys.A);

                q_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 1100) continue;

            切敏捷腿();

            break;
        }
    }

    private void 月蚀后敏捷平A()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var r_down = 0;

        while (r_down == 0)
        {
            if (RegPicture(Resource_Picture.露娜_释放月蚀, "R"))
            {
                Delay(100);
                切敏捷腿();
                KeyPress((uint) Keys.A);

                r_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 1100) continue;

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

    private void 扫射接勋章()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        // 勋章放在c位置
        while (RegPicture(Resource_Picture.物品_勇气, "C") || RegPicture(Resource_Picture.物品_炎阳, "C") ||
               RegPicture(Resource_Picture.物品_紫苑, "C") || RegPicture(Resource_Picture.物品_血棘, "C") ||
               RegPicture(Resource_Picture.物品_羊刀, "C"))
        {
            KeyPress((uint) Keys.C);
            Delay(30);
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
        }

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.物品_勇气, "Z") || RegPicture(Resource_Picture.物品_炎阳, "Z") ||
               RegPicture(Resource_Picture.物品_紫苑, "Z") || RegPicture(Resource_Picture.物品_血棘, "Z") ||
               RegPicture(Resource_Picture.物品_羊刀, "Z"))
        {
            KeyPress((uint) Keys.Z);
            Delay(30);
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
        }

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.物品_勇气, "SPACE") || RegPicture(Resource_Picture.物品_炎阳, "SPACE") ||
               RegPicture(Resource_Picture.物品_紫苑, "SPACE") || RegPicture(Resource_Picture.物品_血棘, "SPACE") ||
               RegPicture(Resource_Picture.物品_羊刀, "SPACE"))
        {
            KeyPress((uint) Keys.Space);
            Delay(30);
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
        }

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        // 否决放在x
        while (RegPicture(Resource_Picture.物品_否决, "X"))
        {
            KeyPress((uint) Keys.X);
            Delay(30);
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
        }

        KeyPress((uint) Keys.A);

        Delay(100);

        切敏捷腿();
    }

    private static void 魂戒魔棒智力(Bitmap bp)
    {
        Delay(100);
        切智力腿(bp);
    }

    #endregion

    #region 小松鼠

    private void 捆接种树()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        if (RegPicture(Resource_Picture.物品_纷争, 全局图像) || RegPicture(Resource_Picture.物品_纷争_7, 全局图像))
            ShiftKeyPress((uint)Keys.C);

        ShiftKeyPress((uint)Keys.Q);

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.小松鼠_释放爆栗出击, 全局图像) || RegPicture(Resource_Picture.小松鼠_释放野地奇袭_7, 全局图像))
            {
                Delay(85);
                ShiftKeyPress((uint) Keys.W);
                q_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 3600) break;
        }
    }

    private void 飞镖接捆接种树()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        if (RegPicture(Resource_Picture.物品_纷争, "C") || RegPicture(Resource_Picture.物品_纷争_7, "C", 7))
            KeyPress((uint) Keys.C);

        var f_down = false;
        var w_down = false;

        KeyPress((uint) Keys.F);

        while (!f_down)
        {
            if (RegPicture(Resource_Picture.小松鼠_猎手旋标, "F", 7))
            {
                Delay(107);
                KeyPress((uint) Keys.W);
                f_down = true;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 3600) break;
        }

        while (!w_down)
        {
            if (RegPicture(Resource_Picture.小松鼠_释放野地奇袭_7, "W", 7))
            {
                Delay(85);
                KeyPress((uint) Keys.Q);
                w_down = true;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1000) break;
        }
    }

    #endregion

    #region 拍拍

    private void 超强力量平A()
    {
        var w_down = 0;

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.拍拍_释放超强力量, "W"))
            {
                Delay(79);
                切敏捷腿();
                KeyPress((uint) Keys.A);

                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 500) break;
        }
    }

    private void 震撼大地接平A()
    {
        KeyPress((uint) Keys.A);
        Delay(200);
        切敏捷腿();
    }

    #endregion

    #region 巨魔

    private void 远程飞斧()
    {
        KeyPress((uint) Keys.Q);

        Delay(150);

        KeyPress((uint) Keys.W);

        Delay(205);

        RightClick();

        KeyPress((uint) Keys.Q);
    }

    #endregion

    #region 小鱼人

    private void 黑暗契约力量()
    {
        // 为了避免切太快导致实际上还是敏捷腿
        Delay(150);

        KeyPress((uint) Keys.A);

        切敏捷腿();

        #region 切智力后力量后敏捷，实际作用前期少减12点血。

        //delay(1424);
        //切力量腿();
        //delay(930);
        //切敏捷腿();
        //delay(300);
        //切敏捷腿();
        //delay(300);
        //切敏捷腿();

        #endregion
    }

    private void 跳水敏捷()
    {
        // 为了避免切太快导致实际上还是敏捷腿
        Delay(180);

        KeyPress((uint) Keys.A);

        切敏捷腿();
    }

    private void 深海护罩敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var d_down = false;

        while (!d_down)
        {
            if (RegPicture(Resource_Picture.小鱼人_释放深海护罩, "D", 5))
            {
                Delay(75);

                d_down = true;

                KeyPress((uint) Keys.A);

                切敏捷腿();
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 800) break;
        }
    }

    #endregion

    #region 猴子

    private void 灵魂之矛敏捷()
    {
        var w_down = 0;
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.猴子_释放灵魂之矛, "Q"))
            {
                Delay(105);
                切敏捷腿();
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 2000) break;
        }
    }

    private void 神行百变敏捷()
    {
        var w_down = 0;
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.猴子_释放神行百变, "W"))
            {
                Delay(1100);
                切敏捷腿();
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 2000) break;
        }
    }

    private void 后撤触发冲锋()
    {
        Delay(550);
        KeyPress((uint) Keys.A);
    }

    #endregion

    #region 水人

    #endregion

    #region 敌法

    private void 闪烁敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.敌法_释放闪烁_4, "W") || RegPicture(Resource_Picture.敌法_释放信仰之源闪烁_4, "W") ||
                RegPicture(Resource_Picture.敌法_释放闪烁_7, "W", 7) || RegPicture(Resource_Picture.敌法_释放信仰之源闪烁_7, "W", 7))
            {
                Delay(95);
                KeyPress((uint) Keys.A);
                切敏捷腿();

                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

            切敏捷腿();

            break;
        }
    }

    //private static void 法术反制敏捷()
    //{
    //    Delay(10);
    //    KeyPress((uint)Keys.V);
    //}

    private void 法力虚空取消后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.敌法_释放法力虚空_4, "R") || RegPicture(Resource_Picture.敌法_释放碎颅锤法力虚空_4, "R") ||
                RegPicture(Resource_Picture.敌法_释放法力虚空_7, "R", 7) || RegPicture(Resource_Picture.敌法_释放碎颅锤法力虚空_7, "R", 7))
            {
                Delay(100);

                KeyPress((uint) Keys.A);

                切敏捷腿();

                Delay(50);

                KeyPress((uint) Keys.A);

                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

            切敏捷腿();

            break;
        }
    }

    #endregion

    #region 幻刺

    private void 窒息短匕敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.幻刺_窒息短匕_4, "Q") || RegPicture(Resource_Picture.幻刺_窒息短匕_5, "Q", 5))
            {
                Delay(150);
                KeyPress((uint) Keys.A);
                切敏捷腿();
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

            切敏捷腿();
            break;
        }
    }

    private void 幻影突袭敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.幻刺_幻影突袭_4, "W") || RegPicture(Resource_Picture.幻刺_幻影突袭_5, "W", 5))
            {
                Delay(125);
                KeyPress((uint) Keys.A);
                切敏捷腿();
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

            切敏捷腿();
            break;
        }
    }

    private void 魅影无形敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.幻刺_魅影无形_4, "E") || RegPicture(Resource_Picture.幻刺_魅影无形_5, "E", 5))
            {
                Delay(200);
                KeyPress((uint) Keys.A);
                切敏捷腿();
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

            切敏捷腿();
            break;
        }
    }

    private void 刀阵旋风敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.幻刺_刀阵旋风_5, "D", 5))
            {
                Delay(200);
                KeyPress((uint) Keys.A);
                切敏捷腿();
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

            切敏捷腿();
            break;
        }
    }

    #endregion

    #region 虚空

    private void 时间漫游敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.虚空_释放时间漫游_4, "Q") || RegPicture(Resource_Picture.虚空_释放时间漫游_5, "Q", 5))
            {
                Delay(450);
                KeyPress((uint) Keys.V);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

            切敏捷腿();
            break;
        }
    }

    private void 时间膨胀敏捷()
    {
        Delay(180);
        KeyPress((uint) Keys.V);
        KeyPress((uint) Keys.A);
    }

    private void 时间结界敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.虚空_释放时间结界_4, "R") || RegPicture(Resource_Picture.虚空_释放时间结界_5, "R", 5))
            {
                Delay(375);
                KeyPress((uint) Keys.V);
                KeyPress((uint) Keys.A);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 1200) continue;

            切敏捷腿();
            break;
        }
    }

    #endregion

    #region TB

    private void 倒影敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.TB_释放倒影_4, "Q") || RegPicture(Resource_Picture.TB_释放倒影_5, "Q", 5)
                                                            || RegPicture(Resource_Picture.TB_释放倒影_6, "Q", 6) ||
                                                            RegPicture(Resource_Picture.TB_释放倒影_7, "Q", 7))
            {
                Delay(150);
                KeyPress((uint) Keys.V);
                KeyPress((uint) Keys.A);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 1600) continue;

            切敏捷腿();
            break;
        }
    }

    private void 幻惑敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.TB_释放幻惑_4, "W") || RegPicture(Resource_Picture.TB_释放幻惑_5, "W", 5)
                                                            || RegPicture(Resource_Picture.TB_释放幻惑_6, "W", 6) ||
                                                            RegPicture(Resource_Picture.TB_释放幻惑_7, "W", 7))
            {
                Delay(85);
                KeyPress((uint) Keys.V);
                KeyPress((uint) Keys.A);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 1600) continue;

            切敏捷腿();
            break;
        }
    }

    private void 魔化敏捷()
    {
        Delay(450);
        KeyPress((uint) Keys.V);
        KeyPress((uint) Keys.A);
    }

    private void 恶魔狂热去后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.TB_恶魔狂热_5, "D", 5) || RegPicture(Resource_Picture.TB_恶魔狂热_6, "D", 6))
            {
                Delay(150);
                KeyPress((uint) Keys.A);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 1600) continue;

            break;
        }
    }

    private void 断魂敏捷()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.TB_释放断魂_4, "R") || RegPicture(Resource_Picture.TB_释放断魂_5, "R", 5)
                                                            || RegPicture(Resource_Picture.TB_释放断魂_6, "R", 6) ||
                                                            RegPicture(Resource_Picture.TB_释放断魂_7, "R", 7))
            {
                Delay(150);
                KeyPress((uint) Keys.V);
                KeyPress((uint) Keys.A);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 1600) continue;

            切敏捷腿();
            break;
        }
    }

    #endregion

    #region 赏金

    private static bool 飞镖接平A(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.赏金_释放飞镖, bp) || RegPicture(Resource_Picture.赏金_释放飞镖_双刀, bp))
        {
            Delay(105);
            RightClick();
            return false;
        }

        return true;
    }

    private static bool 标记去后摇(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.赏金_释放标记, bp) || RegPicture(Resource_Picture.赏金_释放标记_不朽, bp))
        {
            Delay(110);
            RightClick();
            return false;
        }

        return true;
    }

    #endregion

    #region 剧毒

    private bool 循环蛇棒(Bitmap bp)
    {
        var x = 750;
        var y = 856;

        if (ColorAEqualColorB(bp.GetPixel(942 - x, 989 - y), Color.FromArgb(255, 153, 161, 70), 29)) // (942,989)
        {
            KeyPress((uint) Keys.E);
            Delay(30);
        }
        else
        {
            if (RegPicture(Resource_Picture.剧毒_蛇棒_CD_不朽, bp) || RegPicture(Resource_Picture.剧毒_蛇棒_CD, bp))
            {
                KeyPress((uint) Keys.E);
                Delay(30);
            }
        }

        return 循环条件1;
    }

    private static bool 蛇棒去后摇(Bitmap bp)
    {
        if (!RegPicture(Resource_Picture.剧毒_蛇棒_CD_不朽, bp) && !RegPicture(Resource_Picture.剧毒_蛇棒_CD, bp))
        {
            //KeyDown((uint)Keys.LControlKey);
            //KeyDown((uint)Keys.A);
            //KeyUp((uint)Keys.LControlKey);
            //KeyUp((uint)Keys.A);
            RightClick();
            Delay(30);
            return false;
        }

        return true;
    }

    private static bool 瘴气去后摇(Bitmap bp)
    {
        var x = 750;
        var y = 856;
        var c = bp.GetPixel(820 - x, 957 - y); // (820,957)

        if (c.Equals(Color.FromArgb(255, 193, 207, 21))) return true; // 不朽CD好颜色
        if (c.Equals(Color.FromArgb(255, 68, 39, 23))) return true; // 普通CD好颜色

        // 瘴气进入CD颜色
        if (
            ColorAEqualColorB(c, Color.FromArgb(255, 14, 10, 8)) // 普通最短值3
            || ColorAEqualColorB(c, Color.FromArgb(255, 60, 62, 39)) // 不朽最短值6
        )
        {
            KeyPress((uint) Keys.A);
            return false;
        }

        return true;
    }

    private static bool 剧毒新星去后摇(Bitmap bp)
    {
        var x = 750;
        var y = 856;
        var c = bp.GetPixel(1016 - x, 957 - y); // (1016,957)
        // 剧毒新星最佳匹配
        if (ColorAEqualColorB(c, Color.FromArgb(255, 74, 78, 52))) // 最短值5
        {
            RightClick();
            return false;
        }

        return true;
    }

    #endregion

    #endregion

    #region 智力

    #region 黑鸟

    private void G_yxc_cg()
    {
        //G_yxc(245, 550, 435);
        KeyPress((uint) Keys.Space);
        G_yxc_y();
    }

    private void G_yxc_y()
    {
        G_yxc(245, 550, 300);
    }

    private void 关接跳()
    {
        KeyPress((uint) Keys.L);
        KeyPress((uint) Keys.L);

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        Delay(4030);

        var w_down = 0;
        while (w_down == 0)
        {
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 4100) break;

            KeyPress((uint) Keys.Space);
        }
    }

    private void G_yxc(int dyd, int yd, int dd)
    {
        if (RegPicture(Resource_Picture.物品_纷争, "C")) KeyPress((uint) Keys.C);

        KeyPress((uint) Keys.W);

        var w_down = 0;

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (w_down == 0)
        {
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 6000) break;

            if (RegPicture(Resource_Picture.黑鸟_关释放, "W"))
            {
                w_down = 1;
                Delay(dyd);
                RightClick();

                Delay(yd);
                if (中断条件) break;

                KeyPress((uint) Keys.S);

                Delay(dd);
                if (中断条件) break;
                KeyPress((uint) Keys.V);
                KeyPress((uint) Keys.X);
            }
        }
    }

    #endregion

    #region 谜团

    private void 跳秒接午夜凋零黑洞()
    {
        if (RegPicture(Resource_Picture.物品_黑皇, "Z")) KeyPress((uint) Keys.Z);

        if (RegPicture(Resource_Picture.物品_纷争, "C")) KeyPress((uint) Keys.C);

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.物品_跳刀, "SPACE") || RegPicture(Resource_Picture.物品_跳刀_智力跳刀, "SPACE"))
        {
            Delay(15);
            KeyPress((uint) Keys.Space);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 300) break;
        }

        var 午夜凋零_i = false;

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.谜团_午夜凋零CD, "E"))
        {
            KeyPress((uint) Keys.E);
            Delay(15);
            午夜凋零_i = true;
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 500) break;
        }

        if (午夜凋零_i) Delay(智力跳刀BUFF() ? 45 : 75);

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.谜团_黑洞CD, "R"))
        {
            KeyPress((uint) Keys.R);
            Delay(15);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 500) break;
        }

        Delay(30);

        //KeyDown((uint)Keys.LControlKey);

        //KeyPress((uint)Keys.A);

        //KeyUp((uint)Keys.LControlKey);
    }

    private void 刷新接凋零黑洞()
    {
        KeyPress((uint) Keys.X);

        for (var i = 0; i < 2; i++)
        {
            Delay(30);
            KeyPress((uint) Keys.Z);
            KeyPress((uint) Keys.V);
            KeyPress((uint) Keys.R);
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
        KeyPress((uint) Keys.W);

        var w_down = 0;

        while (w_down == 0)
            if (RegPicture(Resource_Picture.冰女_释放冰封禁制, 859, 939, 64, 62))
            {
                Delay(365);
                KeyPress((uint) Keys.Space);
                w_down = 1;
            }
    }

    #endregion

    #region 火女

    private static bool 龙破斩去后摇(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.火女_释放龙破斩, bp))
        {
            Delay(120);
            RightClick(); // 移动反应更快，更好检测最短延时
            return false;
        }

        return true;
    }

    private static bool 光击阵去后摇(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.火女_释放光击阵, bp))
        {
            Delay(120);
            RightClick(); // 移动反应更快，更好检测最短延时
            return false;
        }

        return true;
    }

    private static bool 神灭斩去后摇(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.火女_释放神灭斩, bp))
        {
            Delay(120);
            RightClick(); // 移动反应更快，更好检测最短延时
            return false;
        }

        return true;
    }

    #endregion

    #region 蓝猫

    private static bool 拉接平A(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.蓝猫_释放电子漩涡, bp))
        {
            Delay(115);
            //RightClick();
            KeyPress((uint) Keys.A);
            return false;
        }

        return true;
    }

    private void 残影接平A()
    {
        Delay(30);
        KeyPress((uint) Keys.A);
    }

    private static bool 滚接平A(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.蓝猫_释放球状闪电_红, bp) || RegPicture(Resource_Picture.蓝猫_释放球状闪电, bp))
        {
            Delay(117);
            //RightClick();
            KeyPress((uint) Keys.A);
            return false;
        }

        return true;
    }

    #region 未使用

    private void 原地滚A()
    {
        KeyPress((uint) Keys.A);

        var p = MousePosition;

        var x_差 = p.X - 623;
        var y_差 = p.Y - 1000;
        x_差 /= 8;
        y_差 /= 8;

        for (var i = 1; i <= 8; i++)
        {
            MouseMove(p.X - x_差 * i, p.Y - y_差 * i);
            Delay(3);
        }

        MouseMove(623, 1000);
        Delay(30);
        KeyPress((uint) Keys.R);

        for (var i = 1; i <= 8; i++)
        {
            MouseMove(623 + x_差 * i, 1000 + y_差 * i);
            Delay(3);
        }

        MouseMove(p.X, p.Y);
    }

    #endregion

    #endregion

    #region 宙斯

    private void 弧形闪电去后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.宙斯_释放弧形闪电, "Q"))
            {
                q_down = 1;
                Delay(智力跳刀BUFF() ? 50 : 80);
                KeyPress((uint) Keys.A);

                label1.Text = "QQQ";
            }
            else if (RegPicture(Resource_Picture.宙斯_雷云后释放弧形闪电, "Q", 5))
            {
                q_down = 1;
                Delay(智力跳刀BUFF() ? 50 : 80);
                KeyPress((uint) Keys.A);

                label1.Text = "QQQ";
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1500) break;
        }
    }

    private void 雷击去后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (RegPicture(Resource_Picture.宙斯_释放雷击, "W"))
            {
                w_down = 1;
                Delay(智力跳刀BUFF() ? 70 : 125);
                KeyPress((uint) Keys.A);

                label1.Text = "WWW";
            }
            else if (RegPicture(Resource_Picture.宙斯_雷云后释放雷击, "W", 5))
            {
                w_down = 1;
                Delay(智力跳刀BUFF() ? 70 : 125);
                KeyPress((uint) Keys.A);

                label1.Text = "WWW";
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1500) break;
        }
    }

    #endregion

    #region 卡尔

    private void 三冰对线()
    {
        KeyPress((uint) Keys.Q);
        Delay(30);
        KeyPress((uint) Keys.Q);
        Delay(30);
        KeyPress((uint) Keys.Q);
        Delay(30);
    }

    private void 三火平A()
    {
        KeyPress((uint) Keys.E);
        Delay(30);
        KeyPress((uint) Keys.E);
        Delay(30);
        KeyPress((uint) Keys.E);
        Delay(30);
    }

    private void 三雷幽灵()
    {
        KeyPress((uint) Keys.Q);
        Delay(30);
        KeyPress((uint) Keys.Q);
        Delay(30);
        KeyPress((uint) Keys.W);
        Delay(30);
        KeyPress((uint) Keys.R);
        Delay(30);
        KeyPress((uint) Keys.W);
        Delay(30);
        KeyPress((uint) Keys.W);
        Delay(30);
        KeyPress((uint) Keys.D);
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
        if (RegPicture(Resource_Picture.物品_吹风CD完, "SPACE"))
        {
            label1.Text = "FF";
            KeyPress((uint) Keys.Space);
            Delay(30);

            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (all_down == 0)
            {
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 4000) break;

                if (RegPicture(Resource_Picture.物品_吹风CD, "SPACE"))
                {
                    label1.Text = "FFF";
                    if (RegPicture(Resource_Picture.物品_纷争, "C")) KeyPress((uint) Keys.C);
                    Delay(80);
                    KeyPress((uint) Keys.H);
                    Delay(1280);
                    if (中断条件) break;
                    KeyPress((uint) Keys.Q);
                    Delay(760);
                    KeyPress((uint) Keys.R);
                    all_down = 1;
                }

                Delay(50);
            }
        }
    }

    #endregion

    #region 暗影萨满

    private void 苍穹振击取消后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.暗影萨满_释放苍穹振击, "Q"))
            {
                Delay(200);
                q_down = 1;
                KeyPress((uint) Keys.A);
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
        }
    }

    private void 释放群蛇守卫取消后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.暗影萨满_释放群蛇守卫, "R"))
            {
                Delay(200);
                q_down = 1;
                KeyPress((uint) Keys.A);
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
        }
    }

    private void 变羊取消后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        if (RegPicture(Resource_Picture.暗影萨满_捆绑施法中, 767, 726, 85, 85)) return;

        while (w_down == 0)
        {
            if (!RegPicture(Resource_Picture.暗影萨满_妖术CD, "W"))
            {
                label1.Text = "WW";
                w_down = 1;
                KeyPress((uint) Keys.A);
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
        }
    }

    private void 暗夜萨满最大化控制链()
    {
        var times = 1.0;
        var time = 0;

        if (RegPicture(Resource_Picture.物品_祭礼长袍_4, "G")) times *= 1.1;

        if (RegPicture(Resource_Picture.物品_永恒遗物_4, "G")) times *= 1.25;

        times *= (100 - Convert.ToDouble(tb_状态抗性.Text)) / 100;

        var 技能点颜色 = Color.FromArgb(255, 203, 183, 124);

        if (CaptureColor(909, 1008).Equals(技能点颜色))
            time = 3500;
        else if (CaptureColor(897, 1008).Equals(技能点颜色))
            time = 2750;
        else if (CaptureColor(885, 1008).Equals(技能点颜色))
            time = 2000;
        else if (CaptureColor(875, 1008).Equals(技能点颜色)) time = 1250;

        KeyPress((uint) Keys.W);

        循环条件2 = true;

        Task.Run(() =>
        {
            var time1 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var w_down = 0;

            while (w_down == 0)
            {
                if (!RegPicture(Resource_Picture.暗影萨满_妖术CD, "W")) w_down = 1;

                if (!循环条件2) return;

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time1 > 1200) break;
            }

            Delay(Convert.ToInt32(time * times) - 400);

            if (!循环条件2) return;

            KeyPress((uint) Keys.E);
        });
    }

    #endregion

    #region 小仙女

    private void 皇冠延时计时()
    {
        var 总开始时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var w_down = 0;

        while (w_down == 0)
        {
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 总开始时间 > 2000) return;

            if (RegPicture(Resource_Picture.小仙女_释放诅咒皇冠_不朽, "E", 7))
            {
                // 全局时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                Delay(100);
                KeyPress((uint) Keys.M);
            }
        }
    }

    private void 诅咒皇冠吹风()
    {
        var 总开始时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        KeyPress((uint) Keys.E);

        var w_down = 0;

        while (w_down == 0)
        {
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 总开始时间 > 2000) return;

            if (RegPicture(Resource_Picture.小仙女_释放诅咒皇冠_不朽, "E", 7))
            {
                // Delay(阿哈利姆魔晶() ? 410 : 1410);  // 大部分技能抬手都是0.2-0.3之间
                if (!循环条件2) return;

                if (RegPicture(Resource_Picture.物品_吹风_7, "SPACE", 7))
                {
                    KeyPress((uint) Keys.Space);
                    KeyPress((uint) Keys.M);

                    Delay(2500);
                    if (!循环条件2) return;
                    作祟暗影之境最大化伤害();
                }

                w_down = 1;
            }
        }
    }

    private void 作祟暗影之境最大化伤害()
    {
        // 释放纷争，增加大量伤害
        if (RegPicture(Resource_Picture.物品_纷争_7, "C", 7)) KeyPress((uint) Keys.C);

        KeyPress((uint) Keys.M);
        Delay(30);
        KeyPress((uint) Keys.D);
        Delay(30);
        KeyPress((uint) Keys.W);
        Delay(30);
        KeyPress((uint) Keys.W);

        //var 暗影之境_开始时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        //if (阿哈利姆神杖())
        //{
        //    Delay(400);
        //    KeyPress((uint)Keys.A);
        //}
        //else
        //{
        //    while (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 暗影之境_开始时间 < 4500 || !loop_bool_2) { }
        //    if (!loop_bool_2) return;
        //    KeyPress((uint)Keys.A);
        //}
    }

    //private void 皇冠接花控制衔接()
    //{
    //    var 晕眩时间 = 1750;

    //    Color 技能点颜色 = Color.FromArgb(255, 203, 183, 124);
    //    if (CaptureColor(908, 1004).Equals(技能点颜色))
    //        晕眩时间 = 1750;

    //    技能点颜色 = Color.FromArgb(255, 203, 183, 124);
    //    if (CaptureColor(920, 1004).Equals(技能点颜色))
    //        晕眩时间 = 2250;

    //    技能点颜色 = Color.FromArgb(255, 180, 162, 107);
    //    if (CaptureColor(931, 1005).Equals(技能点颜色))
    //        晕眩时间 = 2750;

    //    技能点颜色 = Color.FromArgb(255, 180, 162, 107);
    //    if (CaptureColor(931, 1005).Equals(技能点颜色))
    //        晕眩时间 = 2750;

    //    技能点颜色 = Color.FromArgb(255, 203, 183, 124);
    //    if (CaptureColor(944, 1004).Equals(技能点颜色))
    //        晕眩时间 = 3250;

    //    技能点颜色 = Color.FromArgb(255, 246, 175, 57);
    //    if (CaptureColor(759, 988).Equals(技能点颜色))
    //        晕眩时间 += 600;

    //    // 950 是第一朵花生效时间,
    //    while (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 全局时间 < (晕眩时间 + (阿哈利姆魔晶() ? 3000 : 4000) - 950) || !loop_bool_2) { }
    //    if (!loop_bool_2) return;
    //    MouseMove(MousePosition.X - 120, MousePosition.Y);
    //    KeyPress((uint)Keys.Q);
    //    LeftClick();
    //}

    #endregion

    #region 天怒

    private bool 循环奥数鹰隼(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.天怒_魔法鹰隼_金饰品, bp)) KeyPress((uint) Keys.Q);

        return 循环条件1;
    }

    private bool 天怒秒人连招(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.物品_血精石_4, bp))
        {
            KeyPress((uint)Keys.B);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.物品_虚灵之刃_4, bp))
        {
            KeyPress((uint) Keys.X);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.物品_羊刀_4, bp))
        {
            KeyPress((uint) Keys.Z);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.物品_阿托斯之棍_4, bp) || RegPicture_small(Resource_Picture.物品_缚灵锁_4, bp))
        {
            KeyPress((uint) Keys.Space);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.物品_纷争, bp))
        {
            KeyPress((uint) Keys.C);
            Delay(20);
        }

        Delay(15);
        KeyPress((uint) Keys.W);


        if (RegPicture(Resource_Picture.天怒_魔法鹰隼_金饰品, bp))
        {
            KeyPress((uint) Keys.Q);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.天怒_上古封印, bp))
        {
            KeyPress((uint) Keys.E);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.天怒_神秘之耀, bp))
        {
            KeyPress((uint) Keys.R);
            Delay(20);
            return true;
        }

        return false;
    }

    #endregion

    #region 炸弹人

    private void 魂戒丢装备()
    {
        批量扔装备();
        KeyPress((uint) Keys.Space);
        捡装备();
    }

    #endregion

    #region 神域

    private void 命运敕令去后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.神域_释放命运敕令, "W"))
            {
                q_down = 1;
                Delay(145);
                RightClick();
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1200) break;
        }
    }

    private void 涤罪之焰去后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.神域_释放涤罪之焰, "E"))
            {
                q_down = 1;
                Delay(95);
                RightClick();
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1200) break;
        }
    }

    private void 虚妄之诺去后摇()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.神域_释放虚妄之诺, "R"))
            {
                q_down = 1;
                Delay(105);
                RightClick();
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1200) break;
        }
    }

    #endregion

    #region 修补匠

    private static async Task 检测敌方英雄自动导弹()
    {
        Task t = new(() =>
        {
            if (RegPicture(Resource_Picture.血量_敌人血量, 0, 0, 1920, 1080, 0.6))
            {
                KeyPress((uint) Keys.W);
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
            if (RegPicture(Resource_Picture.物品_希瓦CD_6, "Z", 6) || RegPicture(Resource_Picture.物品_希瓦CD_7, "Z", 7))
                KeyPress((uint) Keys.Z);
        });
        t.Start();
        await t;
    }

    private static void 推推接刷新()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        var x_down = 0;
        while (x_down == 0)
        {
            if (RegPicture(Resource_Picture.物品_推推BUFF, 400, 865, 1000, 60))
            {
                KeyPress((uint) Keys.R);
                x_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 500) break;
        }
    }


    private async void 刷新完跳()
    {
        var all_down = 0;
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        while (all_down == 0)
        {
            var r_down = 0;
            if (RegPicture(Resource_Picture.修补匠_再填装施法, 773, 727, 75, 77, 0.7))
            {
                if (条件3)
                    await 检测希瓦();
                while (r_down == 0)
                    if (!RegPicture(Resource_Picture.修补匠_再填装施法, 773, 727, 75, 77, 0.7))
                    {
                        r_down = 1;
                        all_down = 1;
                        if (条件1)
                            await 检测敌方英雄自动导弹();
                        if (条件2)
                        {
                            Delay(60);
                            KeyPress((uint) Keys.Space);
                        }
                    }
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
        }
    }

    #endregion

    #region 莱恩

    private bool 莱恩羊接技能(Bitmap bp)
    {
        if (!RegPicture(Resource_Picture.莱恩_羊, bp) && !RegPicture(Resource_Picture.莱恩_羊_鱼, bp))
        {
            if (条件4)
                KeyPress((uint)Keys.E);
            else
                KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    private static bool 大招前纷争(Bitmap bp)
    {
        // 纷争放在C键
        if (RegPicture(Resource_Picture.物品_纷争, bp))
        {
            KeyPress((uint) Keys.C);
            return false;
        }

        return true;
    }

    private static bool 推推破林肯秒羊(Bitmap bp)
    {
        // 纷争放在C键
        if (RegPicture(Resource_Picture.物品_推推棒, bp))
        {
            KeyPress((uint) Keys.X);
            return false;
        }

        return true;
    }

    #endregion

    #endregion

    #region 通用

    private async void 一般程序循环()
    {
        while (总循环条件)
            if (getBitmap != null)
            {
                getBitmap(); // 更新全局Bitmap

                if (中断条件) continue; // 中断则跳过循环

                using var bp = new Bitmap(全局图像);

                if (条件1 && 条件根据图片委托1 != null)
                    await Task.Run(() => { 条件1 = 条件根据图片委托1(bp); });

                if (条件2 && 条件根据图片委托2 != null)
                    await Task.Run(() => { 条件2 = 条件根据图片委托2(bp); });

                if (条件3 && 条件根据图片委托3 != null)
                    await Task.Run(() => { 条件3 = 条件根据图片委托3(bp); });

                if (条件4 && 条件根据图片委托4 != null)
                    await Task.Run(() => { 条件4 = 条件根据图片委托4(bp); });

                if (条件5 && 条件根据图片委托5 != null)
                    await Task.Run(() => { 条件5 = 条件根据图片委托5(bp); });

                if (条件6 && 条件根据图片委托6 != null)
                    await Task.Run(() => { 条件6 = 条件根据图片委托6(bp); });
            }
    }

    private void 无物品状态初始化()
    {
        if (getBitmap == null)
            getBitmap = 获取图片_1;
        Task.Run(() =>
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            一般程序循环();
        });
    }

    private void 取消所有功能()
    {
        总循环条件 = false;
        循环条件1 = false;
        循环条件2 = false;
        中断条件 = false;
        丢装备条件 = false;
        条件1 = false;
        条件2 = false;
        条件3 = false;
        条件4 = false;
        条件5 = false;
        条件6 = false;

        条件根据图片委托1 = null;
        条件根据图片委托2 = null;
        条件根据图片委托3 = null;
        条件根据图片委托4 = null;
        条件根据图片委托5 = null;
        条件根据图片委托6 = null;

        delay_time = 0;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private void 获取图片_1()
    {
        // 750 856 653 217 基本所有技能状态物品，7-8ms延迟
        // 具体点则为起始坐标点加与其的差值
        if (全局图像 == null) 全局图像 = new Bitmap(653, 217);
        CaptureScreen(750, 856, ref 全局图像);
    }

    private void 获取图片_2()
    {
        // 0 0 1920 1080 全屏，25-36ms延迟
        // 具体点则为起始坐标点加与其的差值
        if (全局图像 == null) 全局图像 = new Bitmap(1920, 1080);
        CaptureScreen(0, 0, ref 全局图像);
    }

    #region 快速回城

    private static bool 快速回城(Bitmap bp)
    {
        if (!RegPicture(Resource_Picture.物品_TP效果, bp))
        {
            KeyPress((uint) Keys.T);
            Delay(30);
            KeyPress((uint) Keys.T);

            return true;
        }

        return false;
    }

    #endregion

    #region 泉水状态喝瓶子

    private void 泉水状态喝瓶()
    {
        Delay(400);

        for (var i = 1; i <= 4; i++)
        {
            KeyPress((uint) Keys.C);
            Delay(587);
        }
    }

    private static void 泉水状态喂瓶()
    {
        Delay(3000);

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        for (var i = 1; i <= 10; i++)
        {
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1850) return;

            KeyDown((uint) Keys.LControlKey);
            KeyDown((uint) Keys.C);
            KeyUp((uint) Keys.LControlKey);
            KeyUp((uint) Keys.C);

            Delay(587);
        }
    }

    #endregion

    #region 魂戒力量智力

    private static bool 魂戒力量智力(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.物品_魂戒CD, bp))
        {
            切力量腿(bp);
            KeyPress((uint) Keys.X);
            KeyPress((uint) Keys.V);
            return false;
        }

        return true;
    }

    #endregion

    #region 指定地点

    private static void 指定地点()
    {
        指定地点_P = MousePosition;
        Delay(30);
        KeyDown((uint) Keys.LControlKey);
        Delay(30);
        KeyPress((uint) Keys.D9);
        Delay(30);
        KeyUp((uint) Keys.LControlKey);
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
        double move_X = 0;
        // Y间距，自动根据X调整
        double move_Y = 0;

        if (RegPicture(Resource_Picture.血量_自身血量, 0, 0, 1920, 1080, 0.6))
        {
            var p = RegPicturePoint(Resource_Picture.血量_自身血量, 0, 0, 1920, 1080, 0.6);
            double realX = p.X + 55;
            double realY = p.Y + 117;

            //textBox4.Text = realX.ToString();
            //tb_delay.Text = realY.ToString();

            // 线性求解，保证移动鼠标为直线间的一点，无转身
            var a = (mousePosition.Y - realY) / (mousePosition.X - realX);

            move_X = realX < mousePosition.X ? -60 : 60;

            move_Y = a * move_X;

            if (Math.Abs(move_Y) > 60)
            {
                move_Y = 60;
                move_X = move_Y / a;
            }

            // 确保正负值正确
            move_Y = Convert.ToDouble(mousePosition.Y) +
                     (realY < mousePosition.Y ? -Math.Abs(move_Y) : Math.Abs(move_Y));
            move_X = Convert.ToDouble(mousePosition.X) +
                     (realX < mousePosition.X ? -Math.Abs(move_X) : Math.Abs(move_X));

            tb_x.Text = move_X.ToString();
            tb_y.Text = move_Y.ToString();
        }

        return new Point(Convert.ToInt16(move_X), Convert.ToInt16(move_Y));
    }

    #endregion

    #region 使用装备

    private static void 单次使用装备(Bitmap 匹配图像, int ablityCount = 4, int 等待时间 = 30)
    {
        if (RegPicture(new Bitmap(匹配图像), "Z", ablityCount))
        {
            KeyPress((uint) Keys.Z);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "X", ablityCount))
        {
            KeyPress((uint) Keys.X);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "C", ablityCount))
        {
            KeyPress((uint) Keys.C);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "V", ablityCount))
        {
            KeyPress((uint) Keys.V);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "B", ablityCount))
        {
            KeyPress((uint) Keys.B);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "SPACE", ablityCount))
        {
            KeyPress((uint) Keys.Space);
            Delay(等待时间);
        }
    }

    private static void 持续使用装备直到超时(Bitmap 匹配图像, int 超时时间, int ablityCount = 4, int 等待时间 = 30)
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(new Bitmap(匹配图像), "Z", ablityCount))
        {
            KeyPress((uint) Keys.Z);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "X", ablityCount))
        {
            KeyPress((uint) Keys.X);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "C", ablityCount))
        {
            KeyPress((uint) Keys.C);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "V", ablityCount))
        {
            KeyPress((uint) Keys.V);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "B", ablityCount))
        {
            KeyPress((uint) Keys.B);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "SPACE", ablityCount))
        {
            KeyPress((uint) Keys.Space);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }
    }

    #endregion

    #region 扔装备

    private void 批量扔装备()
    {
        KeyPress((uint) Keys.S);
        Delay(40);
        KeyPress((uint) Keys.F1);
        Delay(40);
        KeyPress((uint) Keys.F1);

        var list_1 = tb_丢装备.Text.Split(',');

        try
        {
            switch (list_1[0])
            {
                case "6":
                    for (var i = 1; i < list_1.Length; i++)
                        switch (list_1[i])
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
                    for (var i = 1; i < list_1.Length; i++)
                        switch (list_1[i])
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
        }
    }

    private static void 扔装备(Point p)
    {
        MouseMove(p);
        LeftDown();
        Delay(40);
        MouseMove(new Point(p.X + 5, p.Y + 5));
        Delay(40);
        KeyDown((uint) Keys.Y);
        Delay(40);
        LeftUp();
        KeyUp((uint) Keys.Y);
        Delay(40);
    }

    private void 捡装备()
    {
        var list_1 = tb_丢装备.Text.Split(',');
        KeyDown((uint) Keys.Y);
        Delay(40);
        for (var i = 1; i < list_1.Length; i++)
        {
            RightClick();
            Delay(100);
        }

        KeyUp((uint) Keys.Y);
    }

    #endregion

    #region 切假腿

    private static void 切智力腿()
    {
        if (RegPicture(Resource_Picture.物品_假腿_力量腿, "V")
            || RegPicture(Resource_Picture.物品_假腿_力量腿_5, "V", 5)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_6, "V", 6)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_7, "V", 7))
        {
            KeyPress((uint) Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_敏捷腿, "V")
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_5, "V", 5)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_6, "V", 6)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_7, "V", 7))
        {
            KeyPress((uint) Keys.V);
            //delay(30);
            KeyPress((uint) Keys.V);
        }
    }

    private static void 切敏捷腿()
    {
        if (RegPicture(Resource_Picture.物品_假腿_力量腿, "V")
            || RegPicture(Resource_Picture.物品_假腿_力量腿_5, "V", 5)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_6, "V", 6)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_7, "V", 7))
        {
            KeyPress((uint) Keys.V);
            //delay(30);
            KeyPress((uint) Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_智力腿, "V")
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_5, "V", 5)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_6, "V", 6)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_7, "V", 7))
        {
            KeyPress((uint) Keys.V);
        }
    }


    private static void 切力量腿()
    {
        if (RegPicture(Resource_Picture.物品_假腿_敏捷腿, "V")
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_5, "V", 5)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_6, "V", 6)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_7, "V", 7))
        {
            KeyPress((uint) Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_智力腿, "V")
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_5, "V", 5)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_6, "V", 6)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_7, "V", 7))
        {
            KeyPress((uint) Keys.V);
            //delay(30);
            KeyPress((uint) Keys.V);
        }
    }


    private static bool 切敏捷腿(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.物品_假腿_力量腿, bp)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_5, bp)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_6, bp)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_7, bp))
        {
            KeyPress((uint) Keys.V);
            KeyPress((uint) Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_智力腿, bp)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_5, bp)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_6, bp)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_7, bp))
        {
            KeyPress((uint) Keys.V);
        }

        return true;
    }

    private static bool 切智力腿(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.物品_假腿_力量腿, bp)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_5, bp)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_6, bp)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_7, bp))
        {
            KeyPress((uint) Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_敏捷腿, bp)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_5, bp)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_6, bp)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_7, bp))
        {
            KeyPress((uint) Keys.V);
            KeyPress((uint) Keys.V);
        }

        return true;
    }

    private static bool 切力量腿(Bitmap bp)
    {
        if (RegPicture(Resource_Picture.物品_假腿_敏捷腿, bp)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_5, bp)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_6, bp)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_7, bp))
        {
            KeyPress((uint) Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_智力腿, bp)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_5, bp)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_6, bp)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_7, bp))
        {
            KeyPress((uint) Keys.V);
            KeyPress((uint) Keys.V);
        }

        return true;
    }

    #endregion

    #region 切臂章

    private void 切臂章()
    {
        KeyPress((uint) Keys.Z);
        KeyPress((uint) Keys.Z);
    }

    #endregion

    #region 分身一齐攻击

    private void 分身一齐攻击()
    {
        Delay(140);
        KeyDown((uint) Keys.LControlKey);
        KeyPress((uint) Keys.A);
        KeyUp((uint) Keys.LControlKey);
    }

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

    #region buff或者装备

    private static bool 智力跳刀BUFF()
    {
        return RegPicture(Resource_Picture.物品_跳刀_智力跳刀BUFF, 400, 865, 1000, 60);
    }

    /// <summary>
    /// </summary>
    /// <param name="bp">指定图片</param>
    /// <returns></returns>
    private static bool 阿哈利姆神杖(Bitmap bp)
    {
        var x = 750;
        var y = 856;

        var 技能点颜色 = Color.FromArgb(255, 32, 183, 249);
        if (bp.GetPixel(1078 - x, 958 - y).Equals(技能点颜色))
            return true;
        // 4技能A杖

        技能点颜色 = Color.FromArgb(255, 30, 188, 252);
        if (bp.GetPixel(1094 - x, 960 - y).Equals(技能点颜色))
            return true;
        // 5技能A杖

        技能点颜色 = Color.FromArgb(255, 30, 189, 253);
        if (bp.GetPixel(1110 - x, 960 - y).Equals(技能点颜色))
            return true;
        // 5技能A杖

        技能点颜色 = Color.FromArgb(255, 31, 188, 253);

        if (bp.GetPixel(1143 - x, 959 - y).Equals(技能点颜色))
            return true;
        // 8技能A杖

        技能点颜色 = Color.FromArgb(255, 30, 187, 250);
        return bp.GetPixel(1122 - x, 959 - y).Equals(技能点颜色);
        // 6技能A杖
    }

    /// <summary>
    /// </summary>
    /// <param name="bp">指定图片</param>
    /// <returns></returns>
    private static bool 阿哈利姆魔晶(Bitmap bp)
    {
        var x = 750;
        var y = 856;

        var 技能点颜色 = Color.FromArgb(255, 34, 186, 254);

        if (bp.GetPixel(1094 - x, 995 - y).Equals(技能点颜色))
            return true;
        // 7技能魔晶

        技能点颜色 = Color.FromArgb(255, 29, 188, 255);

        if (bp.GetPixel(1144 - x, 993 - y).Equals(技能点颜色))
            return true;
        // 8技能魔晶

        技能点颜色 = Color.FromArgb(255, 29, 187, 255);

        if (bp.GetPixel(1110 - x, 994 - y).Equals(技能点颜色))
            return true;
        // 6技能魔晶无A

        技能点颜色 = Color.FromArgb(255, 28, 187, 255);

        if (bp.GetPixel(1121 - x, 993 - y).Equals(技能点颜色))
            return true;
        // 6技能魔晶A

        技能点颜色 = Color.FromArgb(255, 28, 187, 255);

        if (bp.GetPixel(1077 - x, 993 - y).Equals(技能点颜色))
            return true;
        // 4技能魔晶

        技能点颜色 = Color.FromArgb(255, 30, 187, 254);
        return bp.GetPixel(1111 - x, 994 - y).Equals(技能点颜色);
        // 5技能魔晶
    }

    private static void 等待陨星锤结束()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        var wait_i = 0;
        while (wait_i == 0)
        {
            if (RegPicture(Resource_Picture.物品_释放陨星锤_持续施法, 785, 744, 51, 42))
            {
                Delay(2350);
                wait_i = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 4000) break;
        }
    }

    #endregion

    #region 图片识别

    #region Dota2技能物品识别

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
        int time;
        try
        {
            time = Convert.ToInt32(识别英文文字(ToGray(MethodBaseOnMemory(CaptureScreen(930, 21, 58, 16)))).Replace(":", ""));
        }
        catch
        {
            time = 0;
        }

        if (time == 0) return;
        time += 800;
        var time_str = time.ToString();
        var str = string.Concat("塔防刷新", time_str[..^2], ":", time_str[^2..]);
        Delay(500);
        快速发言(str);
    }

    #endregion

    #region 快速发言

    private static void 快速发言(string str)
    {
        Clipboard.SetText(str);
        KeyPress((uint) Keys.Enter);
        KeyDown((uint) Keys.LControlKey);
        KeyPress((uint) Keys.V);
        KeyUp((uint) Keys.LControlKey);
        Delay(30);
        KeyPress((uint) Keys.Enter);
        Delay(30);
    }

    #endregion

    #endregion

    #region 测试_捕捉颜色

    private void 捕捉颜色()
    {
        //KeyPress((uint)Keys.E);

        //ArrayList al = new();
        //ArrayList ax = new();

        //string str = string.Empty;


        Stopwatch stopWatch = new();

        stopWatch.Start();

        //Parallel.Invoke(
        //    () => { Bitmap bp = new(308, 73); CaptureScreen(792, 941, ref bp); bp.Dispose(); },
        //    () => { Bitmap bp = new(200, 97); CaptureScreen(1116, 941, ref bp); bp.Dispose(); }
        //    );

        //stopWatch.Stop();

        //tb_x.Text = string.Concat("两个并行用时", stopWatch.ElapsedMilliseconds);

        //stopWatch.Reset();

        //stopWatch.Start();

        //Bitmap bp = CaptureScreen(0, 0, 1920, 1080);
        //bp.Dispose();

        //CaptureScreenOp(0, 0, 1920, 1080);
        //Bitmap bp = (Bitmap)Bitmap.FromFile("Z:\\screen.bmp");
        //bp.Dispose();

        //var bp = CaptureScreen(0, 0, 1920, 1080); // CaptureScreen(750, 856, 653, 182);
        //bp.Dispose();

        //asd();

        //// 区域捕捉
        //if (Bitmap == null) Bitmap = new Bitmap(653, 182);
        //CaptureScreen(750, 856, ref Bitmap);

        Bitmap bp = new Bitmap(653, 182);
        CaptureScreen(0, 0, ref bp);

        Stopwatch stopWatch1 = new();

        stopWatch1.Start();

        RegPicture(Resource_Picture.物品_纷争, bp);

        //object balanceLock = new object();

        //long o = 0;

        //Parallel.For<long>(0, 10, () => 0, (i, loop, subPoint) =>
        //{
        //    long k = 0;
        //    Parallel.For<long>(0, 10, () => 0, (j, loop1, subPoint1) =>
        //    {
        //        subPoint1 += j;
        //        return subPoint1;
        //    },
        //    (x) =>
        //    {
        //        Interlocked.Add(ref k, x);
        //    });
        //    subPoint += i + k;
        //    return subPoint;
        //},
        //(x) =>
        //{
        //    Interlocked.Add(ref o, x);
        //    //lock (balanceLock)
        //    //{
        //    //    tb_y.Text = x.ToString();
        //    //}
        //});

        stopWatch1.Stop();

        //tb_y.Text = o.ToString();

        tb_x.Text = string.Concat("单体用时", stopWatch1.ElapsedMilliseconds);

        //// 识别时间
        //if (Bitmap == null) Bitmap = new Bitmap(44, 14);
        //CaptureScreen(939, 22, ref Bitmap);
        //tb_丢装备.Text = OCR.识别英文数字(Bitmap);

        // 捕捉攻速
        //if (Bitmap == null) Bitmap = new Bitmap(78, 17);
        //CaptureScreen(552, 510, ref Bitmap);
        //tb_丢装备.Text = OCR.识别英文数字(Bitmap);

        stopWatch.Stop();

        tb_状态抗性.Text = string.Concat("单体用时", stopWatch.ElapsedMilliseconds);

        //while (1 == 1)
        //{
        //    Color w4, color;

        //    // 678, 935, 476, 81 所有技能，6-14ms延迟
        //    // 675, 852, 737, 226 基本所有技能状态，14ms延迟
        //    using (Bitmap bp = CaptureScreen(678, 935, 476, 81))
        //    {
        //        w4 = bp.GetPixel(264, 54); //Color w5 = CaptureColor(839, 994);
        //    }

        //    color = w4;

        //    if (al.Count == 0)
        //    {
        //        ax.Add(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time);
        //        al.Add(string.Concat(FormatColor(color), ";"));
        //    }
        //    else
        //    {
        //        //// 未CD好不记录
        //        //if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time < 1000) continue;

        //        if ((string)al[^1] != string.Concat(FormatColor(color), ";"))
        //        {
        //            ax.Add(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time);
        //            al.Add(string.Concat(FormatColor(color), ";"));
        //        }
        //    }

        //    if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 3900) break;
        //}

        //foreach (string item in al)
        //{
        //    tb_状态抗性.Text = string.Concat(tb_状态抗性.Text, item);
        //}
        //foreach (long item in ax)
        //{
        //    tb_丢装备.Text = string.Concat(tb_丢装备.Text, item.ToString(), ";");
        //}

        //if (tb_状态抗性.Text.Length == 0)
        //{
        //    tb_状态抗性.Text = "无匹配";
        //}

        //if (tb_丢装备.Text.Length == 0)
        //{
        //    tb_丢装备.Text = "无匹配";
        //}
    }

    #endregion

    #endregion

    #region 页面初始化和注销

    /// <summary>
    ///     页面初始化
    /// </summary>
    public Form2()
    {
        InitializeComponent();
        Change_Pic();
        StartListen();
    }

    /// <summary>
    ///     开始监听和初始化模拟
    /// </summary>
    public int StartListen()
    {
        //// 设置线程池数量，最小要大于CPU核心数，最大不要太大就完事了，一般自动就行，手动反而影响性能
        //ThreadPool.SetMinThreads(12, 18);
        //ThreadPool.SetMaxThreads(48, 36);

        // 设置程序为HIGH程序，REALTIME循环延时
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

        var i = 1;

        myKeyEventHandeler = Hook_KeyDown;
        k_hook.KeyDownEvent += myKeyEventHandeler; // 绑定对应处理函数
        k_hook.Start(); // 安装键盘钩子

        //WinIO32.Initialize();

        // 初始化键盘鼠标模拟，仅模仿系统函数，winIo 和 WinRing0 需要额外的操作
        i += KeyboardMouseSimulateDriverAPI.Initialize((uint) SimulateWays.Event);

        Delay(500);

        // 设置窗体显示在最上层
        i = SetWindowPos(Handle, -1, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0010 | 0x0080);

        // 设置本窗体为活动窗体
        SetActiveWindow(Handle);
        SetForegroundWindow(Handle);

        // 设置窗体置顶
        TopMost = true;

        // 338 967 xy全显示 338 1010 只显示三行
        // 设置窗口位置
        Location = new Point(338, 1010);

        if (tb_name.Text == "测试")
        {
            Location = new Point(338, 967);
        }

        //Task.Run(记录买活);

        return i;
    }

    /// <summary>
    ///     取消监听和注销模拟
    /// </summary>
    public void StopListen()
    {
        k_hook.KeyDownEvent -= myKeyEventHandeler; // 取消按键事件
        myKeyEventHandeler = null;
        k_hook.Stop(); // 关闭键盘钩子

        // 注销按键模拟
        KeyboardMouseSimulateDriverAPI.Uninitialize();

        //WinIO32.Shutdown();
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

    private static void RightClick()
    {
        KeyboardMouseSimulateDriverAPI.MouseDown((uint) Dota2Simulator.MouseButtons.RightDown);
        KeyboardMouseSimulateDriverAPI.MouseUp((uint) Dota2Simulator.MouseButtons.RightUp);
    }

    private static void LeftClick()
    {
        KeyboardMouseSimulateDriverAPI.MouseDown((uint) Dota2Simulator.MouseButtons.LeftDown);
        KeyboardMouseSimulateDriverAPI.MouseUp((uint) Dota2Simulator.MouseButtons.LeftUp);
    }

    private static void LeftDown()
    {
        KeyboardMouseSimulateDriverAPI.MouseDown((uint) Dota2Simulator.MouseButtons.LeftDown);
    }

    private static void LeftUp()
    {
        KeyboardMouseSimulateDriverAPI.MouseDown((uint) Dota2Simulator.MouseButtons.LeftUp);
    }

    private new static void KeyUp(uint key)
    {
        KeyboardMouseSimulateDriverAPI.KeyUp(key);
    }

    private new static void KeyDown(uint key)
    {
        KeyboardMouseSimulateDriverAPI.KeyDown(key);
    }

    /// <summary>
    ///     单次操作大约需要7ms
    /// </summary>
    /// <param name="key"></param>
    private new static void KeyPress(uint key)
    {
        KeyDown(key);
        KeyUp(key);
    }

    private static void ShiftKeyPress(uint key)
    {
        KeyDown((uint)Keys.LShiftKey);
        KeyDown(key);
        Delay(10);
        KeyUp(key);
        KeyUp((uint)Keys.LShiftKey);
    }

    private static void KeyPressAsync(uint key)
    {
        Task.Run(() =>
        {
            KeyDown(key);
            KeyUp(key);
        });
    }

    public new static void MouseMove(int X, int Y, bool relative = false)
    {
        if (relative)
        {
            var p = MousePosition;
            X += p.X;
            Y += p.Y;
        }

        KeyboardMouseSimulateDriverAPI.MouseMove(X, Y, !relative);
    }

    public new static void MouseMove(Point p, bool relative = false)
    {
        var X = p.X;
        var Y = p.Y;

        if (relative)
        {
            var p1 = MousePosition;
            X += p1.X;
            Y += p1.Y;
        }

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
    //KeyboardMouseSimulateDriverAPI.MouseDown((uint) KeyboardMouseSimulatorDemo.MouseButtons.LeftDown);
    //KeyboardMouseSimulateDriverAPI.MouseUp((uint) KeyboardMouseSimulatorDemo.MouseButtons.LeftUp);

    //// 右键单击
    //KeyboardMouseSimulateDriverAPI.MouseDown((uint) KeyboardMouseSimulatorDemo.MouseButtons.RightDown);
    //KeyboardMouseSimulateDriverAPI.MouseUp((uint) KeyboardMouseSimulatorDemo.MouseButtons.RightUp);

    //// 按空格键
    //KeyboardMouseSimulateDriverAPI.KeyDown((uint) Keys.Space);
    //KeyboardMouseSimulateDriverAPI.KeyUp((uint) Keys.Space);

    #endregion

}