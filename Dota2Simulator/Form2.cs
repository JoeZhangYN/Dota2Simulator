using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.Picture_Dota2;
using static Dota2Simulator.OCR;
using static Dota2Simulator.PictureProcessing;
using static Dota2Simulator.SetWindowTop;
using Dota2Simulator.KeyboardMouse;
using System.Collections.Generic;

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

        if (1 == 0
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
                    条件2 = false;
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
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";

                    切智力腿();

                    Task.Run(月光后敏捷平A);
                }
                else if (e.KeyValue == (uint)Keys.R)
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
                if (e.KeyValue == (uint)Keys.W)
                {
                    label1.Text = "W";
                    切智力腿();
                    Task.Run(闪烁敏捷);
                }
                else if (e.KeyValue == (uint)Keys.E)
                {
                    label1.Text = "E";
                    // 太过明显,故不使用
                    //切智力腿();
                    //Task.Run(法术反制敏捷);
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    label1.Text = "R";
                    //切智力腿();
                    Task.Run(法力虚空取消后摇);
                }
                else if (e.KeyValue == (uint)Keys.X)
                {
                    label1.Text = "X";
                    Task.Run(分身一齐攻击);
                }
            }

            #endregion

            #region 巨魔

            else if (tb_name.Text.Trim() == "巨魔")
            {
                if (e.KeyValue == (uint)Keys.D)
                {
                    label1.Text = "D";

                    Task.Run(远程飞斧);
                }
            }

            #endregion

            #region 小骷髅

            else if (tb_name.Text.Trim() == "小骷髅")
            {
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";

                    切智力腿();

                    Task.Run(扫射接勋章);
                }
                else if (e.KeyValue == (uint)Keys.E)
                {
                    label1.Text = "E";

                    切智力腿();
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    label1.Text = "R";

                    切智力腿();
                }
            }

            #endregion

            #region 小松鼠

            else if (tb_name.Text.Trim() == "小松鼠")
            {
                if (e.KeyValue == (uint)Keys.D2)
                {
                    label1.Text = "D2";

                    Task.Run(捆接种树);
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    label1.Text = "D3";

                    Task.Run(飞镖接捆接种树);
                }
            }

            #endregion

            #region 拍拍

            else if (tb_name.Text.Trim() == "拍拍")
            {
                if (e.KeyValue == (uint)Keys.W)
                {
                    label1.Text = "W";

                    切智力腿();

                    Task.Run(超强力量平A);
                }
                else if (e.KeyValue == (uint)Keys.Q)
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
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "黑暗契约";

                    切智力腿();

                    Task.Run(黑暗契约力量);
                }
                else if (e.KeyValue == (uint)Keys.W || e.KeyValue == (uint)Keys.R)
                {
                    label1.Text = "释放接平A";

                    切智力腿();

                    Task.Run(跳水敏捷);
                }

                else if (e.KeyValue == (uint)Keys.D2)
                {
                    label1.Text = "W";

                    切智力腿();

                    // 径直移动键位
                    KeyDown((uint)Keys.L);

                    // 径直移动
                    RightClick();

                    // 基本上180°310  90°170 75°135 转身定点，配合A杖效果极佳
                    Delay(170);

                    KeyUp((uint)Keys.L);

                    KeyPress((uint)Keys.W);
                }
                else if (e.KeyValue == (uint)Keys.D)
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
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";

                    切智力腿();

                    Task.Run(灵魂之矛敏捷);
                }
                else if (e.KeyValue == (uint)Keys.D)
                {
                    label1.Text = "D";

                    切智力腿();

                    KeyPress((uint)Keys.W);

                    Task.Run(神行百变敏捷);
                }
                else if (e.KeyValue == (uint)Keys.F)
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
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";
                    切智力腿();
                    Task.Run(窒息短匕敏捷);
                }
                else if (e.KeyValue == (uint)Keys.W)
                {
                    label1.Text = "W";
                    切智力腿();
                    Task.Run(幻影突袭敏捷);
                }
                else if (e.KeyValue == (uint)Keys.E)
                {
                    label1.Text = "E";
                    切智力腿();
                    Task.Run(魅影无形敏捷);
                }
                else if (e.KeyValue == (uint)Keys.D)
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
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";
                    切智力腿();
                    Task.Run(时间漫游敏捷);
                }
                else if (e.KeyValue == (uint)Keys.W)
                {
                    label1.Text = "W";
                    切智力腿();
                    Task.Run(时间膨胀敏捷);
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    label1.Text = "R";
                    切智力腿();
                    Task.Run(时间结界敏捷);
                }
            }

            #region TB

            else if (tb_name.Text == "TB")
            {
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";
                    切智力腿();
                    Task.Run(倒影敏捷);
                }
                else if (e.KeyValue == (uint)Keys.W)
                {
                    label1.Text = "W";
                    切智力腿();
                    Task.Run(幻惑敏捷);
                }
                else if (e.KeyValue == (uint)Keys.E || e.KeyValue == (uint)Keys.F)
                {
                    label1.Text = "E";
                    切智力腿();
                    Task.Run(魔化敏捷);
                }
                else if (e.KeyValue == (uint)Keys.D)
                {
                    label1.Text = "D";
                    Task.Run(恶魔狂热去后摇);
                }
                else if (e.KeyValue == (uint)Keys.R)
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

                if (e.KeyValue == (uint)Keys.Q)
                    条件1 = true;
                else if (e.KeyValue == (uint)Keys.R) 条件2 = true;
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

                if (e.KeyValue == (uint)Keys.Q)
                {
                    中断条件 = false;
                    条件1 = true;
                }
                else if (e.KeyValue == (uint)Keys.E)
                {
                    中断条件 = false;
                    条件2 = true;
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    中断条件 = false;
                    条件3 = true;
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    中断条件 = false;
                    循环条件1 = !循环条件1;
                    if (循环条件1)
                        条件4 = true;
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    中断条件 = true;
                    条件1 = false;
                    条件2 = false;
                    条件3 = false;
                    条件4 = false;
                }
            }

            #endregion

            #region 美杜莎

            else if (tb_name.Text == "美杜莎")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 秘术异蛇去后摇;

                if (条件根据图片委托2 == null)
                    条件根据图片委托2 = 石化凝视去后摇;

                if (e.KeyValue == (uint)Keys.W)
                {
                    切智力腿(全局bytes, 全局size);
                    中断条件 = false;
                    条件1 = true;
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    切智力腿(全局bytes, 全局size);
                    中断条件 = false;
                    条件2 = true;
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    中断条件 = true;
                    条件1 = false;
                    条件2 = false;
                }
            }

            #endregion

            #endregion

            #region 智力

            #region 黑鸟

            else if (tb_name.Text == "黑鸟")
            {
                if (e.KeyValue == (uint)Keys.D)
                {
                    label1.Text = "D";
                    中断条件 = false;
                    Task.Run(G_yxc_y);
                    // 普通砸锤
                }
                else if (e.KeyValue == (uint)Keys.F)
                {
                    label1.Text = "F";
                    中断条件 = false;
                    Task.Run(G_yxc_cg);
                }
                else if (e.KeyValue == (uint)Keys.D2)
                {
                    Task.Run(关接跳);
                }
                else if (e.KeyValue == (uint)Keys.H)
                {
                    中断条件 = true;
                }
            }

            #endregion

            #region 谜团

            else if (tb_name.Text == "谜团")
            {
                if (e.KeyValue == (uint)Keys.D)
                {
                    label1.Text = "D";
                    Task.Run(跳秒接午夜凋零黑洞);
                }
                else if (e.KeyValue == (uint)Keys.F)
                {
                    label1.Text = "F";
                    Task.Run(刷新接凋零黑洞);
                }
            }

            #endregion

            #region 冰女

            else if (tb_name.Text.Trim() == "冰女")
            {
                if (e.KeyValue == (uint)Keys.E)
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

                if (e.KeyValue == (uint)Keys.Q)
                {
                    中断条件 = false;
                    条件1 = true;
                }
                else if (e.KeyValue == (uint)Keys.W)
                {
                    中断条件 = false;
                    条件2 = true;
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    中断条件 = false;
                    条件3 = true;
                }
                else if (e.KeyValue == (uint)Keys.S)
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

                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";
                    Task.Run(残影接平A);
                }
                else if (e.KeyValue == (uint)Keys.W)
                {
                    中断条件 = false;
                    条件1 = true;
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    中断条件 = false;
                    条件2 = true;
                }
                else if (e.KeyValue == (uint)Keys.D4)
                {
                    中断条件 = false;
                    条件3 = true;
                }
                else if (e.KeyValue == (uint)Keys.D2)
                {
                    中断条件 = false;
                    条件4 = true;
                }
                else if (e.KeyValue == (uint)Keys.S)
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
                else if (e.KeyValue == (uint)Keys.D)
                {
                    Task.Run(泉水状态喝瓶);
                }
                else if (e.KeyValue == (uint)Keys.F)
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
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";

                    Task.Run(弧形闪电去后摇);
                }
                else if (e.KeyValue == (uint)Keys.W)
                {
                    label1.Text = "W";

                    Task.Run(雷击去后摇);
                }
            }

            #endregion

            #region 卡尔

            else if (tb_name.Text.Trim() == "卡尔")
            {
                if (e.KeyValue == (uint)Keys.D2)
                {
                    label1.Text = "D2";

                    Task.Run(三冰对线);
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    label1.Text = "D2";

                    Task.Run(三火平A);
                }
                else if (e.KeyValue == (uint)Keys.D1)
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
                if (e.KeyValue == (uint)Keys.F)
                {
                    label1.Text = "F";

                    中断条件 = false;

                    Task.Run(吹风接撕裂大地);
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    中断条件 = true;
                }
            }

            #endregion

            #region 暗影萨满

            else if (tb_name.Text.Trim() == "暗影萨满")
            {
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 苍穹振击取消后摇;

                if (条件根据图片委托2 == null)
                    条件根据图片委托2 = 变羊取消后摇;

                if (条件根据图片委托3 == null)
                    条件根据图片委托3 = 释放群蛇守卫取消后摇;

                if (e.KeyValue == (uint)Keys.W)
                {
                    中断条件 = false;
                    条件2 = true;
                }
                else if (e.KeyValue == (uint)Keys.Q)
                {
                    中断条件 = false;
                    条件1 = true;
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    中断条件 = false;
                    条件3 = true;
                }
                else if (e.KeyValue == (uint)Keys.D2)
                {
                    推推破林肯秒羊(全局bytes, 全局size);
                    KeyPress('w');
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    Task.Run(() => { 渐隐期间放技能('e', 800); });
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    中断条件 = true;
                    条件1 = false;
                    条件2 = false;
                    条件3 = false;
                }
            }

            #endregion

            #region 小仙女

            else if (tb_name.Text.Trim() == "小仙女")
            {
                if (e.KeyValue == (uint)Keys.D2)
                {
                    label1.Text = "D2";

                    循环条件2 = true;

                    Task.Run(诅咒皇冠吹风);
                }

                if (e.KeyValue == (uint)Keys.D9)
                {
                    label1.Text = "D3";

                    循环条件2 = true;

                    Task.Run(作祟暗影之境最大化伤害);
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    循环条件2 = false;
                }
                else if (e.KeyValue == (uint)Keys.E)
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

                if (e.KeyValue == (uint)Keys.D3)
                    条件2 = true;
                //else if (e.KeyValue == (uint) Keys.D2)
                //    条件3 = true;
                else if (e.KeyValue == (uint)Keys.S)
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
                if (!总循环条件)
                {
                    总循环条件 = true;
                    无物品状态初始化();
                }

                if (条件根据图片委托1 == null)
                    条件根据图片委托1 = 爆破后接3雷粘性炸弹;

                if (e.KeyValue == (uint)Keys.D2)
                {
                    中断条件 = false;
                    条件1 = true;

                    if (RegPicture(Resource_Picture.物品_纷争_7, 全局bytes, 全局size))
                    {
                        KeyPress('z');
                    }
                    if (RegPicture(Resource_Picture.物品_魂戒CD_5, 全局bytes, 全局size))
                    {
                        KeyPress('x');
                    }
                    KeyPress('e');
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    中断条件 = true;
                    条件1 = false;
                }
            }

            #endregion

            #region 神域

            else if (tb_name.Text.Trim() == "神域")
            {
                if (e.KeyValue == (uint)Keys.W)
                    Task.Run(命运敕令去后摇);
                else if (e.KeyValue == (uint)Keys.E)
                    Task.Run(涤罪之焰去后摇);
                else if (e.KeyValue == (uint)Keys.R) Task.Run(虚妄之诺去后摇);
            }

            #endregion

            #region 修补匠

            else if (tb_name.Text.Trim() == "修补匠")
            {
                if (e.KeyValue == (uint)Keys.R)
                {
                    KeyPress((uint)Keys.C);
                    KeyPress((uint)Keys.V);
                    Task.Run(刷新完跳);
                }
                else if (e.KeyValue == (uint)Keys.D1)
                {
                    条件1 = !条件1;
                    if (条件1)
                        TTS.Speak("开启刷导弹");
                    else
                        TTS.Speak("关闭刷导弹");
                }
                else if (e.KeyValue == (uint)Keys.D2)
                {
                    条件2 = !条件2;
                    if (条件2)
                        TTS.Speak("开启刷跳");
                    else
                        TTS.Speak("关闭刷跳");
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    条件3 = !条件3;
                    if (条件3)
                        TTS.Speak("开启希瓦");
                    else
                        TTS.Speak("关闭希瓦");
                }
                else if (e.KeyValue == (uint)Keys.X)
                {
                    Task.Run(推推接刷新);
                }
                else if (e.KeyValue == (uint)Keys.D1)
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
                else if (e.KeyValue == (uint)Keys.R)
                {
                    大招前纷争(全局bytes, 全局size);
                }
                else if (e.KeyValue == (uint)Keys.D2)
                {
                    推推破林肯秒羊(全局bytes, 全局size);
                    KeyPress((uint)Keys.W);
                }
            }

            #endregion

            #endregion

            #region 保存微信图片

            else if (tb_name.Text.Trim() == "微信图片")
            {
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";

                    中断条件 = false;

                    Task.Run(一键保存图片);
                }

                if (e.KeyValue == (uint)Keys.A)
                {
                    label1.Text = "A";
                    中断条件 = true;
                }
            }

            #endregion

            #region 切假腿

            else if (tb_name.Text.Trim() == "切假腿")
            {
                if (e.KeyValue == (uint)Keys.Q || e.KeyValue == (uint)Keys.W || e.KeyValue == (uint)Keys.E ||
                    e.KeyValue == (uint)Keys.D || e.KeyValue == (uint)Keys.F || e.KeyValue == (uint)Keys.R)
                    切智力腿();
            }

            #endregion

            #region 测试

            else if (tb_name.Text.Trim() == "测试")
            {
                if (e.KeyValue == (uint)Keys.X)
                {
                    tb_状态抗性.Text = "";
                    tb_丢装备.Text = "";
                    捕捉颜色();
                }
            }

            else if (tb_name.Text.Trim() == "测试截图")
            {
                if (e.KeyValue == (uint)Keys.X)
                {
                    捕捉颜色截图();
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
            KeyDown((uint)Keys.LControlKey);

            KeyPress((uint)Keys.S);

            KeyUp((uint)Keys.LControlKey);

            Delay(250);

            KeyPress((uint)Keys.Enter);

            Delay(500);

            KeyPress((uint)Keys.Right);

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
    ///     全局bytes
    /// </summary>
    private static byte[] 全局bytes;

    /// <summary>
    ///     全局size
    /// </summary>
    private static Size 全局size;

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
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private delegate bool condition_delegate_bitmap(byte[] bytes, Size size);

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
        KeyPress((uint)Keys.Space);
        Delay(30);
        KeyPress((uint)Keys.D9);
        MouseMove(指定地点_P);
        Delay(30);
        KeyPress((uint)Keys.E);
        Delay(30);
        KeyPress((uint)Keys.D9);
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
                KeyPress((uint)Keys.E);
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
                KeyPress((uint)Keys.R);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1300) break;
        }
    }

    private void 最大化x伤害控制()
    {
        KeyPress((uint)Keys.E);

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

    private static bool 跳吼(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.物品_刃甲, bts, size))
            KeyPress((uint)Keys.Z);
        //Delay(30);

        KeyPress((uint)Keys.Space);

        var s_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        if (RegPicture(Resource_Picture.斧王_狂战士之吼, bts, size) || RegPicture(Resource_Picture.斧王_狂战士之吼_金色饰品, bts, size))
        {
            KeyPress((uint)Keys.Q);
            Delay(550, s_time);
            //RightClick();
            KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    private static bool 战斗饥渴取消后摇(byte[] bts, Size size)
    {
        var s_time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        // 检测刚释放完毕
        if (RegPicture(Resource_Picture.斧王_释放战斗饥渴_不朽, bts, size))
        {
            Delay(290, s_time);
            //RightClick();
            KeyPress((uint)Keys.A);
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
            KeyPress((uint)Keys.D);
            KeyPress((uint)Keys.D);

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
            KeyPress((uint)Keys.Space);
            //Delay(100);
            快速选择敌方英雄();
        }

        持续使用装备直到超时(Resource_Picture.物品_否决, 150);

        持续使用装备直到超时(Resource_Picture.物品_天堂, 150);

        持续使用装备直到超时(Resource_Picture.物品_勇气, 150);

        持续使用装备直到超时(Resource_Picture.物品_炎阳, 150);

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.军团_决斗CD, "R"))
        {
            KeyPress((uint)Keys.R);
            快速选择敌方英雄();

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 450) break;
        }

        KeyPress((uint)Keys.A);
    }

    #endregion

    #region 孽主

    private void 深渊火雨阿托斯()
    {
        var all_done = 0;

        KeyPress((uint)Keys.W);

        while (all_done == 0)
            if (RegPicture(Resource_Picture.孽主_释放深渊, 857, 939, 70, 72))
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

    private void 心炎平A()
    {
        KeyPress((uint)Keys.A);

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.哈斯卡_释放心炎, "Q"))
            {
                Delay(110);

                KeyPress((uint)Keys.A);

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
                    KeyPress((uint)Keys.X);
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

    private bool 鼻涕针刺循环(byte[] bts, Size size)
    {
        bool 是否魔晶, 是否A杖;

        var x = 750;
        var y = 856;
        var q5 = GetPixelBytes(bts, size, 775 - x, 994 - y);
        var q4 = GetPixelBytes(bts, size, 807 - x, 994 - y);
        var w5 = GetPixelBytes(bts, size, 839 - x, 994 - y);
        var w4 = GetPixelBytes(bts, size, 871 - x, 994 - y);
        是否魔晶 = 阿哈利姆魔晶(bts, size);
        是否A杖 = 阿哈利姆神杖(bts, size);

        if (循环条件1)
        {
            if (是否魔晶)
            {
                if (ColorAEqualColorB(w5, Color.FromArgb(255, 79, 74, 73), 8)
                   )
                {
                    KeyPress((uint)Keys.W);
                    Delay(30);
                }

                if (RegPicture(Resource_Picture.钢背_针刺CD_5, bts, size))
                {
                    KeyPress((uint)Keys.W);
                    Delay(30);
                }
            }
            else
            {
                if (ColorAEqualColorB(w4, Color.FromArgb(255, 80, 76, 75), 8)
                   )
                {
                    KeyPress((uint)Keys.W);
                    Delay(30);
                }

                if (RegPicture(Resource_Picture.钢背_针刺CD, bts, size))
                {
                    KeyPress((uint)Keys.W);
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
                    KeyPress((uint)Keys.Q);
                    Delay(30);
                }

                if (RegPicture(Resource_Picture.钢背_鼻涕CD_5_不朽, bts, size))
                {
                    KeyPress((uint)Keys.Q);
                    Delay(30);
                }
            }
            else
            {
                if (
                    ColorAEqualColorB(q4, Color.FromArgb(255, 64, 61, 55)) // 不朽颜色变化
                )
                {
                    KeyPress((uint)Keys.Q);
                    Delay(30);
                }

                if (RegPicture(Resource_Picture.钢背_鼻涕CD_不朽, bts, size))
                {
                    KeyPress((uint)Keys.Q);
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
                KeyPress((uint)Keys.A);

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
                KeyPress((uint)Keys.A);

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
            KeyPress((uint)Keys.C);
            Delay(30);
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
        }

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.物品_勇气, "Z") || RegPicture(Resource_Picture.物品_炎阳, "Z") ||
               RegPicture(Resource_Picture.物品_紫苑, "Z") || RegPicture(Resource_Picture.物品_血棘, "Z") ||
               RegPicture(Resource_Picture.物品_羊刀, "Z"))
        {
            KeyPress((uint)Keys.Z);
            Delay(30);
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
        }

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.物品_勇气, "SPACE") || RegPicture(Resource_Picture.物品_炎阳, "SPACE") ||
               RegPicture(Resource_Picture.物品_紫苑, "SPACE") || RegPicture(Resource_Picture.物品_血棘, "SPACE") ||
               RegPicture(Resource_Picture.物品_羊刀, "SPACE"))
        {
            KeyPress((uint)Keys.Space);
            Delay(30);
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
        }

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        // 否决放在x
        while (RegPicture(Resource_Picture.物品_否决, "X"))
        {
            KeyPress((uint)Keys.X);
            Delay(30);
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
        }

        KeyPress((uint)Keys.A);

        Delay(100);

        切敏捷腿();
    }

    private static void 魂戒魔棒智力(byte[] bts, Size size)
    {
        Delay(100);
        切智力腿(bts, size);
    }

    #endregion

    #region 小松鼠

    private void 捆接种树()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        if (RegPicture(Resource_Picture.物品_纷争, 全局bytes, 全局size) || RegPicture(Resource_Picture.物品_纷争_7, 全局bytes, 全局size))
            ShiftKeyPress((uint)Keys.C);

        ShiftKeyPress((uint)Keys.Q);

        var q_down = 0;

        while (q_down == 0)
        {
            if (RegPicture(Resource_Picture.小松鼠_释放爆栗出击, 全局bytes, 全局size) || RegPicture(Resource_Picture.小松鼠_释放野地奇袭_7, 全局bytes, 全局size))
            {
                Delay(85);
                ShiftKeyPress((uint)Keys.W);
                q_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 3600) break;
        }
    }

    private void 飞镖接捆接种树()
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        if (RegPicture(Resource_Picture.物品_纷争, "C") || RegPicture(Resource_Picture.物品_纷争_7, "C", 7))
            KeyPress((uint)Keys.C);

        var f_down = false;
        var w_down = false;

        KeyPress((uint)Keys.F);

        while (!f_down)
        {
            if (RegPicture(Resource_Picture.小松鼠_猎手旋标, "F", 7))
            {
                Delay(107);
                KeyPress((uint)Keys.W);
                f_down = true;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 3600) break;
        }

        while (!w_down)
        {
            if (RegPicture(Resource_Picture.小松鼠_释放野地奇袭_7, "W", 7))
            {
                Delay(85);
                KeyPress((uint)Keys.Q);
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
                KeyPress((uint)Keys.A);

                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 500) break;
        }
    }

    private void 震撼大地接平A()
    {
        KeyPress((uint)Keys.A);
        Delay(200);
        切敏捷腿();
    }

    #endregion

    #region 巨魔

    private void 远程飞斧()
    {
        KeyPress((uint)Keys.Q);

        Delay(150);

        KeyPress((uint)Keys.W);

        Delay(205);

        RightClick();

        KeyPress((uint)Keys.Q);
    }

    #endregion

    #region 小鱼人

    private void 黑暗契约力量()
    {
        // 为了避免切太快导致实际上还是敏捷腿
        Delay(150);

        KeyPress((uint)Keys.A);

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

        KeyPress((uint)Keys.A);

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

                KeyPress((uint)Keys.A);

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
        KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.A);
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

                KeyPress((uint)Keys.A);

                切敏捷腿();

                Delay(50);

                KeyPress((uint)Keys.A);

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
                KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.V);
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
        KeyPress((uint)Keys.V);
        KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.A);
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
        KeyPress((uint)Keys.V);
        KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.A);
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
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.A);
                w_down = 1;
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 1600) continue;

            切敏捷腿();
            break;
        }
    }

    #endregion

    #region 赏金

    private static bool 飞镖接平A(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.赏金_释放飞镖, bts, size) || RegPicture(Resource_Picture.赏金_释放飞镖_双刀, bts, size))
        {
            Delay(105);
            RightClick();
            return false;
        }

        return true;
    }

    private static bool 标记去后摇(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.赏金_释放标记, bts, size) || RegPicture(Resource_Picture.赏金_释放标记_不朽, bts, size))
        {
            Delay(110);
            RightClick();
            return false;
        }

        return true;
    }

    #endregion

    #region 剧毒

    private bool 循环蛇棒(byte[] bts, Size size)
    {
        var x = 750;
        var y = 856;

        if (ColorAEqualColorB(GetPixelBytes(bts, size, 942 - x, 989 - y), Color.FromArgb(255, 153, 161, 70), 29)) // (942,989)
        {
            KeyPress((uint)Keys.E);
            Delay(30);
        }
        else
        {
            if (RegPicture(Resource_Picture.剧毒_蛇棒_CD_不朽, bts, size) || RegPicture(Resource_Picture.剧毒_蛇棒_CD, bts, size))
            {
                KeyPress((uint)Keys.E);
                Delay(30);
            }
        }

        return 循环条件1;
    }

    private static bool 蛇棒去后摇(byte[] bts, Size size)
    {
        if (!RegPicture(Resource_Picture.剧毒_蛇棒_CD_不朽, bts, size) && !RegPicture(Resource_Picture.剧毒_蛇棒_CD, bts, size))
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

    private static bool 瘴气去后摇(byte[] bts, Size size)
    {
        var x = 750;
        var y = 856;
        var c = GetPixelBytes(bts, size, 820 - x, 957 - y); // (820,957)

        if (c.Equals(Color.FromArgb(255, 193, 207, 21))) return true; // 不朽CD好颜色
        if (c.Equals(Color.FromArgb(255, 68, 39, 23))) return true; // 普通CD好颜色

        // 瘴气进入CD颜色
        if (
            ColorAEqualColorB(c, Color.FromArgb(255, 14, 10, 8)) // 普通最短值3
            || ColorAEqualColorB(c, Color.FromArgb(255, 60, 62, 39)) // 不朽最短值6
        )
        {
            KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    private static bool 剧毒新星去后摇(byte[] bts, Size size)
    {
        var x = 750;
        var y = 856;
        var c = GetPixelBytes(bts, size, 1016 - x, 957 - y); // (1016,957)
        // 剧毒新星最佳匹配
        if (ColorAEqualColorB(c, Color.FromArgb(255, 74, 78, 52))) // 最短值5
        {
            RightClick();
            return false;
        }

        return true;
    }

    #endregion

    #region 美杜莎
    private static bool 秘术异蛇去后摇(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.美杜莎_释放秘术异蛇, bts, size) || RegPicture(Resource_Picture.美杜莎_释放秘术异蛇_5, bts, size))
        {
            Delay(60);
            RightClick();
            切敏捷腿(bts, size);
            return false;
        }

        return true;
    }

    private static bool 石化凝视去后摇(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.美杜莎_释放石化凝视, bts, size) || RegPicture(Resource_Picture.美杜莎_释放石化凝视_5, bts, size))
        {
            Delay(120);
            RightClick();
            切敏捷腿(bts, size);
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

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        Delay(4030);

        var w_down = 0;
        while (w_down == 0)
        {
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 4100) break;

            KeyPress((uint)Keys.Space);
        }
    }

    private void G_yxc(int dyd, int yd, int dd)
    {
        if (RegPicture(Resource_Picture.物品_纷争, "C")) KeyPress((uint)Keys.C);

        KeyPress((uint)Keys.W);

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

                KeyPress((uint)Keys.S);

                Delay(dd);
                if (中断条件) break;
                KeyPress((uint)Keys.V);
                KeyPress((uint)Keys.X);
            }
        }
    }

    #endregion

    #region 谜团

    private void 跳秒接午夜凋零黑洞()
    {
        if (RegPicture(Resource_Picture.物品_黑皇, "Z")) KeyPress((uint)Keys.Z);

        if (RegPicture(Resource_Picture.物品_纷争, "C")) KeyPress((uint)Keys.C);

        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.物品_跳刀, "SPACE") || RegPicture(Resource_Picture.物品_跳刀_智力跳刀, "SPACE"))
        {
            Delay(15);
            KeyPress((uint)Keys.Space);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 300) break;
        }

        var 午夜凋零_i = false;

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.谜团_午夜凋零CD, "E"))
        {
            KeyPress((uint)Keys.E);
            Delay(15);
            午夜凋零_i = true;
            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 500) break;
        }

        if (午夜凋零_i) Delay(智力跳刀BUFF() ? 45 : 75);

        time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(Resource_Picture.谜团_黑洞CD, "R"))
        {
            KeyPress((uint)Keys.R);
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
            if (RegPicture(Resource_Picture.冰女_释放冰封禁制, 859, 939, 64, 62))
            {
                Delay(365);
                KeyPress((uint)Keys.Space);
                w_down = 1;
            }
    }

    #endregion

    #region 火女

    private static bool 龙破斩去后摇(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.火女_释放龙破斩, bts, size))
        {
            Delay(120);
            RightClick(); // 移动反应更快，更好检测最短延时
            return false;
        }

        return true;
    }

    private static bool 光击阵去后摇(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.火女_释放光击阵, bts, size))
        {
            Delay(120);
            RightClick(); // 移动反应更快，更好检测最短延时
            return false;
        }

        return true;
    }

    private static bool 神灭斩去后摇(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.火女_释放神灭斩, bts, size))
        {
            Delay(120);
            RightClick(); // 移动反应更快，更好检测最短延时
            return false;
        }

        return true;
    }

    #endregion

    #region 蓝猫

    private static bool 拉接平A(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.蓝猫_释放电子漩涡, bts, size))
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

    private static bool 滚接平A(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.蓝猫_释放球状闪电_红, bts, size) || RegPicture(Resource_Picture.蓝猫_释放球状闪电, bts, size))
        {
            Delay(117);
            //RightClick();
            KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    #region 未使用

    private void 原地滚A()
    {
        KeyPress((uint)Keys.A);

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
        KeyPress((uint)Keys.R);

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
                KeyPress((uint)Keys.A);

                label1.Text = "QQQ";
            }
            else if (RegPicture(Resource_Picture.宙斯_雷云后释放弧形闪电, "Q", 5))
            {
                q_down = 1;
                Delay(智力跳刀BUFF() ? 50 : 80);
                KeyPress((uint)Keys.A);

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
                KeyPress((uint)Keys.A);

                label1.Text = "WWW";
            }
            else if (RegPicture(Resource_Picture.宙斯_雷云后释放雷击, "W", 5))
            {
                w_down = 1;
                Delay(智力跳刀BUFF() ? 70 : 125);
                KeyPress((uint)Keys.A);

                label1.Text = "WWW";
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1500) break;
        }
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
        if (RegPicture(Resource_Picture.物品_吹风CD完, "SPACE"))
        {
            label1.Text = "FF";
            KeyPress((uint)Keys.Space);
            Delay(30);

            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (all_down == 0)
            {
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 4000) break;

                if (RegPicture(Resource_Picture.物品_吹风CD, "SPACE"))
                {
                    label1.Text = "FFF";
                    if (RegPicture(Resource_Picture.物品_纷争, "C")) KeyPress((uint)Keys.C);
                    Delay(80);
                    KeyPress((uint)Keys.H);
                    Delay(1280);
                    if (中断条件) break;
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

    private static bool 苍穹振击取消后摇(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.暗影萨满_释放苍穹振击, bts, size))
        {
            Delay(200);
            KeyPress('a');
            return false;
        }

        return true;
    }

    private static bool 释放群蛇守卫取消后摇(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.暗影萨满_释放群蛇守卫, bts, size))
        {
            Delay(200);
            KeyPress('a');
            return false;
        }
        return true;
    }

    private static bool 变羊取消后摇(byte[] bts, Size size)
    {
        if (!RegPicture(Resource_Picture.暗影萨满_妖术CD, bts, size))
        {
            KeyPress('a');
            return false;
        }
        return true;
    }

    private void 暗夜萨满最大化控制链(byte[] bts, Size size)
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

        KeyPress((uint)Keys.W);

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

            KeyPress((uint)Keys.E);
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
                KeyPress((uint)Keys.M);
            }
        }
    }

    private void 诅咒皇冠吹风()
    {
        var 总开始时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        KeyPress((uint)Keys.E);

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
                    KeyPress((uint)Keys.Space);
                    KeyPress((uint)Keys.M);

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
        if (RegPicture(Resource_Picture.物品_纷争_7, "C", 7)) KeyPress((uint)Keys.C);

        KeyPress((uint)Keys.M);
        Delay(30);
        KeyPress((uint)Keys.D);
        Delay(30);
        KeyPress((uint)Keys.W);
        Delay(30);
        KeyPress((uint)Keys.W);

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

    private bool 循环奥数鹰隼(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.天怒_魔法鹰隼_金饰品, bts, size)) KeyPress((uint)Keys.Q);

        return 循环条件1;
    }

    private bool 天怒秒人连招(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.物品_血精石_4, bts, size))
        {
            KeyPress((uint)Keys.B);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.物品_虚灵之刃_4, bts, size))
        {
            KeyPress((uint)Keys.X);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.物品_羊刀_4, bts, size))
        {
            KeyPress((uint)Keys.Z);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.物品_阿托斯之棍_4, bts, size) || RegPicture(Resource_Picture.物品_缚灵锁_4, bts, size))
        {
            KeyPress((uint)Keys.Space);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.物品_纷争, bts, size))
        {
            KeyPress((uint)Keys.C);
            Delay(20);
        }

        Delay(15);
        KeyPress((uint)Keys.W);


        if (RegPicture(Resource_Picture.天怒_魔法鹰隼_金饰品, bts, size))
        {
            KeyPress((uint)Keys.Q);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.天怒_上古封印, bts, size))
        {
            KeyPress((uint)Keys.E);
            Delay(20);
            return true;
        }

        if (RegPicture(Resource_Picture.天怒_神秘之耀, bts, size))
        {
            KeyPress((uint)Keys.R);
            Delay(20);
            return true;
        }

        return false;
    }

    #endregion

    #region 炸弹人

    private bool 爆破后接3雷粘性炸弹(byte[] bts,Size size)
    {
        if (RegPicture(Resource_Picture.炸弹人_释放爆破起飞, bts, size))
        {
            Delay(995);

            if (RegPicture(Resource_Picture.物品_以太_5,bts, size) || RegPicture(Resource_Picture.物品_玲珑心_5, bts, size))
            {

            }
            else
            {
                //var x = MousePosition.X;
                //var y = MousePosition.Y;

                //Delay(175);
                //MouseMove(x - 188, y + 50);
                //KeyPress('r');

                //MouseMove(619, 1002);
                //KeyPress('h');

                //KeyPress((uint)Keys.F1);
                //KeyPress((uint)Keys.F1);

                //Delay(175);
                //KeyPress('r');

                //MouseMove(790, 385);
                //Delay(175);
                //KeyPress('r');
                //Delay(175);
                //KeyPress('r');

                //MouseMove(1041, 331);
                //Delay(175);
                ////MouseMove(x + 90, y - 35);
                ////KeyPress('r');
                //KeyPress('r');
            }

            return false;
        }

        return true;
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
            if (RegPicture(Resource_Picture.物品_希瓦CD_6, "Z", 6) || RegPicture(Resource_Picture.物品_希瓦CD_7, "Z", 7))
                KeyPress((uint)Keys.Z);
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
                KeyPress((uint)Keys.R);
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
                            KeyPress((uint)Keys.Space);
                        }
                    }
            }

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
        }
    }

    #endregion

    #region 莱恩

    private bool 莱恩羊接技能(byte[] bts, Size size)
    {
        if (!RegPicture(Resource_Picture.莱恩_羊, bts, size) && !RegPicture(Resource_Picture.莱恩_羊_鱼, bts, size))
        {
            if (条件4)
                KeyPress((uint)Keys.E);
            else
                KeyPress((uint)Keys.A);
            return false;
        }

        return true;
    }

    private static bool 大招前纷争(byte[] bts, Size size)
    {
        // 纷争放在C键
        if (RegPicture(Resource_Picture.物品_纷争, bts, size))
        {
            KeyPress((uint)Keys.C);
            return false;
        }

        return true;
    }

    private static bool 推推破林肯秒羊(byte[] bts, Size size)
    {
        // 纷争放在C键
        if (RegPicture(Resource_Picture.物品_推推棒, bts, size))
        {
            KeyPress((uint)Keys.X);
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

                if (条件1 && 条件根据图片委托1 != null)
                    await Task.Run(() => { 条件1 = 条件根据图片委托1(全局bytes, 全局size); });

                if (条件2 && 条件根据图片委托2 != null)
                    await Task.Run(() => { 条件2 = 条件根据图片委托2(全局bytes, 全局size); }); 

                if (条件3 && 条件根据图片委托3 != null)
                    await Task.Run(() => { 条件3 = 条件根据图片委托3(全局bytes, 全局size); });

                if (条件4 && 条件根据图片委托4 != null)
                    await Task.Run(() => { 条件4 = 条件根据图片委托4(全局bytes, 全局size); });

                if (条件5 && 条件根据图片委托5 != null)
                    await Task.Run(() => { 条件5 = 条件根据图片委托5(全局bytes, 全局size); });

                if (条件6 && 条件根据图片委托6 != null)
                    await Task.Run(() => { 条件6 = 条件根据图片委托6(全局bytes, 全局size); });
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
        全局bytes = GetBitmapByte(全局图像);
        全局size = new Size(653, 217);
    }

    private void 获取图片_2()
    {
        // 0 0 1920 1080 全屏，25-36ms延迟
        // 具体点则为起始坐标点加与其的差值
        if (全局图像 == null) 全局图像 = new Bitmap(1920, 1080);
        CaptureScreen(0, 0, ref 全局图像);
        全局bytes = GetBitmapByte(全局图像);
        全局size = new Size(1920, 1080);
    }

    #region 快速回城

    private static bool 快速回城(byte[] bts, Size size)
    {
        if (!RegPicture(Resource_Picture.物品_TP效果, bts, size))
        {
            KeyPress((uint)Keys.T);
            Delay(30);
            KeyPress((uint)Keys.T);

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
            KeyPress((uint)Keys.C);
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

            KeyDown((uint)Keys.LControlKey);
            KeyDown((uint)Keys.C);
            KeyUp((uint)Keys.LControlKey);
            KeyUp((uint)Keys.C);

            Delay(587);
        }
    }

    #endregion

    #region 魂戒力量智力

    private static bool 魂戒力量智力(byte[] bts, Size size)
    {
        if (RegPicture(Resource_Picture.物品_魂戒CD, bts, size))
        {
            切力量腿(bts, size);
            KeyPress((uint)Keys.X);
            KeyPress((uint)Keys.V);
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
            KeyPress((uint)Keys.Z);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "X", ablityCount))
        {
            KeyPress((uint)Keys.X);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "C", ablityCount))
        {
            KeyPress((uint)Keys.C);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "V", ablityCount))
        {
            KeyPress((uint)Keys.V);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "B", ablityCount))
        {
            KeyPress((uint)Keys.B);
            Delay(等待时间);
        }
        else if (RegPicture(new Bitmap(匹配图像), "SPACE", ablityCount))
        {
            KeyPress((uint)Keys.Space);
            Delay(等待时间);
        }
    }

    private static void 持续使用装备直到超时(Bitmap 匹配图像, int 超时时间, int ablityCount = 4, int 等待时间 = 30)
    {
        var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        while (RegPicture(new Bitmap(匹配图像), "Z", ablityCount))
        {
            KeyPress((uint)Keys.Z);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "X", ablityCount))
        {
            KeyPress((uint)Keys.X);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "C", ablityCount))
        {
            KeyPress((uint)Keys.C);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "V", ablityCount))
        {
            KeyPress((uint)Keys.V);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "B", ablityCount))
        {
            KeyPress((uint)Keys.B);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }

        while (RegPicture(new Bitmap(匹配图像), "SPACE", ablityCount))
        {
            KeyPress((uint)Keys.Space);
            Delay(等待时间);

            if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 超时时间) break;
        }
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
        KeyDown((uint)Keys.Y);
        Delay(40);
        LeftUp();
        KeyUp((uint)Keys.Y);
        Delay(40);
    }

    private void 捡装备()
    {
        var list_1 = tb_丢装备.Text.Split(',');
        KeyDown((uint)Keys.Y);
        Delay(40);
        for (var i = 1; i < list_1.Length; i++)
        {
            RightClick();
            Delay(100);
        }

        KeyUp((uint)Keys.Y);
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
            KeyPress((uint)Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_敏捷腿, "V")
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_5, "V", 5)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_6, "V", 6)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_7, "V", 7))
        {
            KeyPress((uint)Keys.V);
            //delay(30);
            KeyPress((uint)Keys.V);
        }
    }

    private static void 切敏捷腿()
    {
        if (RegPicture(Resource_Picture.物品_假腿_力量腿, "V")
            || RegPicture(Resource_Picture.物品_假腿_力量腿_5, "V", 5)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_6, "V", 6)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_7, "V", 7))
        {
            KeyPress((uint)Keys.V);
            //delay(30);
            KeyPress((uint)Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_智力腿, "V")
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_5, "V", 5)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_6, "V", 6)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_7, "V", 7))
        {
            KeyPress((uint)Keys.V);
        }
    }


    private static void 切力量腿()
    {
        if (RegPicture(Resource_Picture.物品_假腿_敏捷腿, "V")
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_5, "V", 5)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_6, "V", 6)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_7, "V", 7))
        {
            KeyPress((uint)Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_智力腿, "V")
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_5, "V", 5)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_6, "V", 6)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_7, "V", 7))
        {
            KeyPress((uint)Keys.V);
            //delay(30);
            KeyPress((uint)Keys.V);
        }
    }


    private static bool 切敏捷腿(byte[] parByte, Size size)
    {
        if (RegPicture(Resource_Picture.物品_假腿_力量腿, parByte, size)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_5, parByte, size)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_6, parByte, size)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_7, parByte, size))
        {
            KeyPress((uint)Keys.V);
            KeyPress((uint)Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_智力腿, parByte, size)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_5, parByte, size)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_6, parByte, size)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_7, parByte, size))
        {
            KeyPress((uint)Keys.V);
        }

        return true;
    }

    private static bool 切智力腿(byte[] parByte, Size size)
    {
        if (RegPicture(Resource_Picture.物品_假腿_力量腿, parByte, size)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_5, parByte, size)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_6, parByte, size)
            || RegPicture(Resource_Picture.物品_假腿_力量腿_7, parByte, size))
        {
            KeyPress((uint)Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_敏捷腿, parByte, size)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_5, parByte, size)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_6, parByte, size)
                 || RegPicture(Resource_Picture.物品_假腿_敏捷腿_7, parByte, size))
        {
            KeyPress((uint)Keys.V);
            KeyPress((uint)Keys.V);
        }

        return true;
    }

    private static bool 切力量腿(byte[] parByte, Size size)
    {
        if (RegPicture(Resource_Picture.物品_假腿_敏捷腿, parByte, size)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_5, parByte, size)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_6, parByte, size)
            || RegPicture(Resource_Picture.物品_假腿_敏捷腿_7, parByte, size))
        {
            KeyPress((uint)Keys.V);
        }
        else if (RegPicture(Resource_Picture.物品_假腿_智力腿, parByte, size)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_5, parByte, size)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_6, parByte, size)
                 || RegPicture(Resource_Picture.物品_假腿_智力腿_7, parByte, size))
        {
            KeyPress((uint)Keys.V);
            KeyPress((uint)Keys.V);
        }

        return true;
    }

    #endregion

    #region 切臂章

    private void 切臂章()
    {
        KeyPress((uint)Keys.Z);
        KeyPress((uint)Keys.Z);
    }

    #endregion

    #region 分身一齐攻击

    private void 分身一齐攻击()
    {
        Delay(140);
        KeyDown((uint)Keys.LControlKey);
        KeyPress((uint)Keys.A);
        KeyUp((uint)Keys.LControlKey);
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
    private static bool 阿哈利姆神杖(byte[] bts, Size size)
    {
        var x = 750;
        var y = 856;

        var 技能点颜色 = Color.FromArgb(255, 32, 183, 249);
        if (GetPixelBytes(bts, size, 1078 - x, 958 - y).Equals(技能点颜色))
            return true;
        // 4技能A杖

        技能点颜色 = Color.FromArgb(255, 30, 188, 252);
        if (GetPixelBytes(bts, size, 1094 - x, 960 - y).Equals(技能点颜色))
            return true;
        // 5技能A杖

        技能点颜色 = Color.FromArgb(255, 30, 189, 253);
        if (GetPixelBytes(bts, size, 1110 - x, 960 - y).Equals(技能点颜色))
            return true;
        // 5技能A杖

        技能点颜色 = Color.FromArgb(255, 31, 188, 253);

        if (GetPixelBytes(bts, size, 1143 - x, 959 - y).Equals(技能点颜色))
            return true;
        // 8技能A杖

        技能点颜色 = Color.FromArgb(255, 30, 187, 250);
        return GetPixelBytes(bts, size, 1122 - x, 959 - y).Equals(技能点颜色);
        // 6技能A杖
    }

    /// <summary>
    /// </summary>
    /// <param name="bp">指定图片</param>
    /// <returns></returns>
    private static bool 阿哈利姆魔晶(byte[] bts, Size size)
    {
        var x = 750;
        var y = 856;

        var 技能点颜色 = Color.FromArgb(255, 34, 186, 254);

        if (GetPixelBytes(bts, size, 1094 - x, 995 - y).Equals(技能点颜色))
            return true;
        // 7技能魔晶

        技能点颜色 = Color.FromArgb(255, 29, 188, 255);

        if (GetPixelBytes(bts, size, 1144 - x, 993 - y).Equals(技能点颜色))
            return true;
        // 8技能魔晶

        技能点颜色 = Color.FromArgb(255, 29, 187, 255);

        if (GetPixelBytes(bts, size, 1110 - x, 994 - y).Equals(技能点颜色))
            return true;
        // 6技能魔晶无A

        技能点颜色 = Color.FromArgb(255, 28, 187, 255);

        if (GetPixelBytes(bts, size, 1121 - x, 993 - y).Equals(技能点颜色))
            return true;
        // 6技能魔晶A

        技能点颜色 = Color.FromArgb(255, 28, 187, 255);

        if (GetPixelBytes(bts, size, 1077 - x, 993 - y).Equals(技能点颜色))
            return true;
        // 4技能魔晶

        技能点颜色 = Color.FromArgb(255, 30, 187, 254);
        return GetPixelBytes(bts, size, 1111 - x, 994 - y).Equals(技能点颜色);
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

    private static void 渐隐期间放技能(char c, int delay)
    {
        KeyPressAlt('z');
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

    private static bool RegPicture(Bitmap bp, byte[] bts, Size size, double matchRate = 0.9)
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

    private static List<Point> RegPicturePoint(Bitmap bp, byte[] btys, Size size, double matchRate = 0.9)
    {
        try
        {
            return FindBytesParallel(GetBitmapByte(bp), bp.Size, btys, size, matchRate: matchRate);
        }
        catch
        {
        }

        return new List<Point>();
    }

    #endregion

    #region 返回图片颜色数组

    /// <summary>
    ///     返回图片颜色数组
    /// </summary>
    /// <param name="subBitmap">图片</param>
    /// <returns></returns>
    private static byte[] GetBitmapByte(Bitmap subBitmap)
    {
        var subData = subBitmap.LockBits(new Rectangle(0, 0, subBitmap.Width, subBitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        var byteArrarySub = new byte[subData.Stride * subData.Height];
        Marshal.Copy(subData.Scan0, byteArrarySub, 0, subData.Stride * subData.Height);
        subBitmap.UnlockBits(subData);
        return byteArrarySub;
    }

    #endregion

    #region 返回数组对应颜色

    private static Color GetPixelBytes(byte[] bts, Size size, int x, int y)
    {
        var subIndex = x * size.Width * 4 + y * 4;
        return Color.FromArgb(bts[subIndex + 3], bts[subIndex + 2],
            bts[subIndex + 1], bts[subIndex]);
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

        //var x = p.X - (width / 2) < 0 ? 0 : p.X + (width / 2) > 1920 ? 0 : p.X - (width / 2);
        //var y = p.Y - (hight / 2) < 0 ? 0 : p.Y + (hight / 2) > 1080 ? 0 : p.Y - (hight / 2);

        var x = 0;
        var y = 0;

        var bp = new Bitmap(width, hight);
        var size = new Size(width, hight);

        CaptureScreen(x, y, ref bp);
        var bytes = GetBitmapByte(bp);

        var list = RegPicturePoint(Resource_Picture.血量_敌人等级底色, bytes, size, matchRate: 0.6);

        var 偏移x = 1920;
        var 偏移y = 1080;

        foreach (var item in list)
        {
            if (Math.Abs(item.X + x - p.X) + Math.Abs(item.Y + y - p.Y) < Math.Abs(偏移x) + Math.Abs(偏移y))
            {
                偏移x = item.X + x - p.X;
                偏移y = item.Y + y - p.Y;
            }
        }

        if (list.Count > 0)
        {
            if (type == 1)
            {
                MouseMoveSim(p.X - 45 + 偏移x, p.Y + 80 + 偏移y);
            }
            else
            {
                MouseMove(p.X - 45 + 偏移x, p.Y + 80 + 偏移y);
            }
        }

        return true;
    }

    #endregion

    #endregion

    #region 测试_捕捉颜色

    private void 捕捉颜色()
    {
        Stopwatch stopWatch = new();

        stopWatch.Start();

        //KeyPress('q');

        //for (int i = 0; i < 100; i++)
        //{
        //    获取图片_1();
        //    苍穹振击取消后摇(全局bytes, 全局size);
        //}

        快速选择敌方英雄();

        tb_x.Text = string.Concat(MousePosition.X, " ", MousePosition.Y);

        stopWatch.Stop();

        tb_状态抗性.Text = string.Concat("单体用时", stopWatch.ElapsedMilliseconds);
    }

    private void 捕捉颜色截图()
    {
        MouseMove(69, 178);
        Delay(300);
        LeftClick();

        KeyPress((uint)Keys.F2);
        Delay(1300);
        KeyPress((uint)Keys.R);
        Delay(1300);
        KeyPressControl('s');
        KeyPress((uint)Keys.Enter);
        Delay(600);

        for (int i = 0; i < 6; i++)
        {
            KeyPress((uint)Keys.Down);
        }
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
        i += KeyboardMouseSimulateDriverAPI.Initialize((uint)SimulateWays.Event);

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

        // 用于初始捕捉
        KeyPress('s');
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
        SimEnigo.Rightlick();
        //KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.RightDown);
        //KeyboardMouseSimulateDriverAPI.MouseUp((uint)Dota2Simulator.MouseButtons.RightUp);
    }

    private static void LeftClick()
    {
        SimEnigo.LeftClick();
        //KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.LeftDown);
        //KeyboardMouseSimulateDriverAPI.MouseUp((uint)Dota2Simulator.MouseButtons.LeftUp);
    }

    private static void LeftDown()
    {
        KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.LeftDown);
    }

    private static void LeftUp()
    {
        KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.LeftUp);
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
    private new static void KeyPress(char key)
    {
        SimEnigo.KeyPress(key);
    }

    private new static void KeyUp(char key)
    {
        SimEnigo.KeyUp(key);
    }

    private new static void KeyDown(char key)
    {
        SimEnigo.KeyDown(key);
    }

    private static void KeyPressAlt(char key)
    {
        SimEnigo.KeyPressAlt(key);
    }

    private static void KeyPressControl(char key)
    {
        SimEnigo.KeyPressControl(key);
    }
    private static void KeyPressShift(char key)
    {
        SimEnigo.KeyPressShift(key);
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

    public new static void MouseMove(int x, int y, bool relative = false)
    {
        if (relative)
        {
            SimEnigo.MouseMoveRelative(x, y);
        }
        else
        {
            SimEnigo.MouseMove(x, y);
        }
        //if (relative)
        //{
        //    var p = MousePosition;
        //    X += p.X;
        //    Y += p.Y;
        //}

        //KeyboardMouseSimulateDriverAPI.MouseMove(X, Y, !relative);
    }

    public new static void MouseMoveSim(int X, int Y, bool relative = false)
    {
        var p = MousePosition;

        var def_x = (X - p.X) / 15;
        var def_y = (Y - p.Y) / 15;

        for (int i = 1; i < 15; i++)
        {
            Delay(1);
            SimEnigo.MouseMove(p.X + def_x * i, p.Y + def_y * i);
            //KeyboardMouseSimulateDriverAPI.MouseMove(p.X + def_x * i, p.Y + def_y * i, !relative);
        }
        SimEnigo.MouseMove(X, Y);
        //KeyboardMouseSimulateDriverAPI.MouseMove(X, Y, !relative);
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
        SimEnigo.MouseMove(X, Y);
        //KeyboardMouseSimulateDriverAPI.MouseMove(X, Y, !relative);
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