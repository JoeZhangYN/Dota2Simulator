using Dota2Simulator.Picture_Dota2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Dota2Simulator.PictureProcessing;
using static Dota2Simulator.SetWindowTop;   

namespace Dota2Simulator
{
    public partial class Form2 : Form
    {
        #region 局部全局变量
        /// <summary>
        ///     用于捕获按键
        /// </summary>
        private readonly KeyboardHook k_hook = new();

        /// <summary>
        ///     取消
        /// </summary>
        private bool b_cancel;

        /// <summary>
        ///     循环计数1
        /// </summary>
        private bool loop_bool_1 = false;

        /// <summary>
        ///     循环计数2
        /// </summary>
        private bool loop_bool_2 = false;

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
        private long 全局时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        #endregion

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
            #region 力量

            #region 军团

            if (tb_name.Text == "军团")
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
                if (e.KeyValue == (uint)Keys.E)
                {
                    label1.Text = "E";

                    切智力腿();

                    Task.Run(跳吼);
                }

                else if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";

                    切智力腿();
                }

                else if (e.KeyValue == (uint)Keys.W)
                {
                    label1.Text = "W";

                    切智力腿();

                    Task.Run(战斗饥渴取消后摇);
                }

                else if (e.KeyValue == (uint)Keys.R)
                {
                    label1.Text = "R";

                    切智力腿();

                    Task.Run(淘汰之刃后);
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
                if (e.KeyValue == (uint)Keys.D2)
                {
                    label1.Text = "D2";

                    loop_bool_1 = !loop_bool_1;

                    Task.Run(循环针刺);
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    loop_bool_2 = !loop_bool_2;

                    label1.Text = "D3";

                    Task.Run(A杖鼻涕);
                }
                else if (e.KeyValue == (uint)Keys.H)
                {
                    loop_bool_1 = false;
                    loop_bool_2 = false;
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
                    切智力腿();
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
                else if (e.KeyValue == (uint)Keys.Z)
                {
                    label1.Text = "Z";

                    if (RegPicture(Resource_Picture.物品_魂戒CD, "Z"))
                    {
                        切力量腿();
                        Task.Run(魂戒智力);
                    }
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

            #endregion

            #region 智力

            #region 黑鸟

            else if (tb_name.Text == "黑鸟")
            {
                if (e.KeyValue == (uint)Keys.D)
                {
                    label1.Text = "D";
                    b_cancel = false;
                    Task.Run(G_yxc_y);
                }
                else if (e.KeyValue == (uint)Keys.F)
                {
                    label1.Text = "F";
                    b_cancel = false;
                    Task.Run(G_yxc_cg);
                }
                else if (e.KeyValue == (uint)Keys.H)
                {
                    b_cancel = true;
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

            #region 蓝猫

            else if (tb_name.Text.Trim() == "蓝猫")
            {
                if (e.KeyValue == (uint)Keys.W)
                {
                    label1.Text = "W";
                    Task.Run(拉接平A);
                }

                else if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";
                    Task.Run(残影接平A);
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    label1.Text = "R";
                    Task.Run(滚接平A);
                }
                else if (e.KeyValue == (uint)Keys.F)
                {
                    label1.Text = "F";
                    Task.Run(原地滚A);
                }
                else if (e.KeyValue == (uint)Keys.D)
                {
                    label1.Text = "F";
                    Task.Run(泉水出来喝瓶);
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
            }

            #endregion

            #region 拉席克

            else if (tb_name.Text.Trim() == "拉席克")
            {
                if (e.KeyValue == (uint)Keys.F)
                {
                    label1.Text = "F";

                    b_cancel = false;

                    Task.Run(吹风接撕裂大地);
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    b_cancel = true;
                }
            }

            #endregion

            #region 暗影萨满

            else if (tb_name.Text.Trim() == "暗影萨满")
            {
                if (e.KeyValue == (uint)Keys.Q)
                {
                    label1.Text = "Q";

                    Task.Run(苍穹振击取消后摇);
                }
                else if (e.KeyValue == (uint)Keys.W)
                {
                    label1.Text = "W";

                    Task.Run(变羊取消后摇);
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    label1.Text = "R";

                    Task.Run(释放群蛇守卫取消后摇);
                }
                else if (e.KeyValue == (uint)Keys.D)
                {
                    label1.Text = "D";

                    Task.Run(暗夜萨满最大化控制链);
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    loop_bool_2 = false;
                }
            }

            #endregion

            #region 小仙女

            else if (tb_name.Text.Trim() == "小仙女")
            {
                if (e.KeyValue == (uint)Keys.D2)
                {
                    label1.Text = "D2";

                    loop_bool_2 = true;

                    Task.Run(诅咒皇冠吹风);
                }
                if (e.KeyValue == (uint)Keys.D9)
                {
                    label1.Text = "D3";

                    loop_bool_2 = true;

                    Task.Run(作祟暗影之境最大化伤害);
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    loop_bool_2 = false;
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
                if (e.KeyValue == (uint)Keys.D2)
                {
                    loop_bool_1 = true;

                    Task.Run(循环奥数鹰隼);
                }
                else if (e.KeyValue == (uint)Keys.S)
                {
                    loop_bool_1 = false;
                    loop_bool_2 = false;
                }
                else if (e.KeyValue == (uint)Keys.Q)
                {
                    // Task.Run(奥数鹰隼取消后摇);
                }
                else if (e.KeyValue == (uint)Keys.D3)
                {
                    loop_bool_2 = true;

                    Task.Run(天怒秒人连招);
                }

            }

            #endregion

            #region 炸弹人

            else if (tb_name.Text.Trim() == "炸弹人")
            {
                if (e.KeyValue == (uint)Keys.Space)
                {
                    魂戒丢装备();
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

                    b_cancel = false;

                    Task.Run(一键保存图片);
                }

                if (e.KeyValue == (uint)Keys.A)
                {
                    label1.Text = "A";
                    b_cancel = true;
                }
            }

            #endregion

            #region 切假腿

            else if (tb_name.Text.Trim() == "切假腿")
            {
                if (e.KeyValue == (uint)Keys.Q || e.KeyValue == (uint)Keys.W || e.KeyValue == (uint)Keys.D || e.KeyValue == (uint)Keys.F || e.KeyValue == (uint)Keys.R)
                    切智力腿();
            }

            #endregion
        }

        #endregion

        #region Dota2具体实现

        #region 力量

        #region 斧王

        private void 跳吼()
        {
            while (RegPicture(Resource_Picture.物品_刃甲, "X"))
            {
                KeyPress((uint)Keys.X);
                Delay(30);
            }

            KeyPress((uint)Keys.Space);


            while (RegPicture(Resource_Picture.斧王_狂战士之吼, "Q") || RegPicture(Resource_Picture.斧王_狂战士之吼_金色饰品, "Q"))
            {
                KeyPress((uint)Keys.Q);
                Delay(30);
            }

            KeyDown((uint)Keys.LControlKey);
            KeyPress((uint)Keys.A);
            KeyUp((uint)Keys.LControlKey);

            Delay(430);

            切敏捷腿();

            if (RegPicture(Resource_Picture.物品_分身, "Z"))
            {
                KeyPress((uint)Keys.Z);
                分身一齐攻击();
            }
        }

        private void 战斗饥渴取消后摇()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var w_down = 0;
            while (w_down == 0)
            {
                if (RegPicture(Resource_Picture.斧王_战斗饥渴, "W"))
                {
                    Delay(302);
                    KeyPress((uint)Keys.A);
                    w_down = 1;

                    切敏捷腿();

                    Delay(200);

                    切敏捷腿();
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
            }
        }

        private void 淘汰之刃后()
        {
            Delay(200);

            切敏捷腿();

            Delay(200);

            切敏捷腿();

            Delay(200);

            切敏捷腿();
        }

        #endregion

        #region 军团

        private void 决斗()
        {
            //var p = 正面跳刀_无转身();

            if (RegPicture(Resource_Picture.物品_魂戒CD, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Delay(30);
            }

            if (RegPicture(Resource_Picture.物品_臂章, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Delay(30);
            }

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

            if (RegPicture(Resource_Picture.物品_黑皇, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Delay(30);
            }

            if (RegPicture(Resource_Picture.物品_相位, "B"))
            {
                KeyPress((uint)Keys.B);
                Delay(30);
            }

            if (RegPicture(Resource_Picture.物品_相位, "C"))
            {
                KeyPress((uint)Keys.C);
                Delay(30);
            }

            if (RegPicture(Resource_Picture.物品_刃甲, "X"))
            {
                KeyPress((uint)Keys.X);
                Delay(30);
            }

            //var point = MousePosition;

            //MouseMove(p.X, p.Y);

            KeyPress((uint)Keys.Space);

            //Delay(5);

            //MouseMove(point.X, point.Y);

            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.物品_否决, "X"))
            {
                KeyPress((uint)Keys.X);
                Delay(30);
                KeyPress((uint)Keys.A);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 150) break;
            }

            while (RegPicture(Resource_Picture.物品_天堂, "C"))
            {
                KeyPress((uint)Keys.C);
                Delay(30);
                KeyPress((uint)Keys.A);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 150) break;
            }

            while (RegPicture(Resource_Picture.物品_勇气, "V"))
            {
                KeyPress((uint)Keys.V);
                Delay(30);
                KeyPress((uint)Keys.A);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 150) break;
            }

            while (RegPicture(Resource_Picture.物品_炎阳, "V"))
            {
                KeyPress((uint)Keys.V);
                Delay(30);
                KeyPress((uint)Keys.A);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 150) break;
            }

            while (RegPicture(Resource_Picture.军团_决斗CD, "R"))
            {
                KeyPress((uint)Keys.R);
                Delay(30);

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
                if (FindPicture(Resource_Picture.孽主_释放深渊, CaptureScreen(857, 939, 70, 72)).Count > 0)
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

        private void 循环针刺()
        {
            while (loop_bool_1)
            {
                if (RegPicture(Resource_Picture.钢背_针刺CD, "W"))
                {
                    KeyPress((uint)Keys.W);
                    label1.Text = "D22";
                    Delay(300);
                }

                else if (RegPicture(Resource_Picture.钢背_针刺刚CD好, "W"))
                {
                    KeyPress((uint)Keys.W);
                    label1.Text = "D22";
                    Delay(300);
                }

                else if (RegPicture(Resource_Picture.钢背_针刺CD_5, "W", 5))
                {
                    KeyPress((uint)Keys.W);
                    label1.Text = "D22";
                    Delay(300);
                }

                Delay(15);
            }
        }

        private void A杖鼻涕()
        {
            if (阿哈利姆神杖())
                while (loop_bool_2)
                {
                    if (RegPicture(Resource_Picture.钢背_鼻涕CD, "Q"))
                    {
                        KeyPress((uint)Keys.Q);
                        label1.Text = "D32";
                        Delay(300);
                    }

                    if (RegPicture(Resource_Picture.钢背_鼻涕CD_5, "Q", 5))
                    {
                        KeyPress((uint)Keys.Q);
                        label1.Text = "D32";
                        Delay(300);
                    }

                    Delay(15);
                }
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
        //        if (FindPicture(Resource_Picture.吹风CD, CaptureScreen(1291, 991, 60, 45)).Count > 0)
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
            while (RegPicture(Resource_Picture.物品_勇气, "C") || RegPicture(Resource_Picture.物品_炎阳, "C") || RegPicture(Resource_Picture.物品_紫苑, "C") || RegPicture(Resource_Picture.物品_血棘, "C") || RegPicture(Resource_Picture.物品_羊刀, "C"))
            {
                KeyPress((uint)Keys.C);
                Delay(30);
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.物品_勇气, "Z") || RegPicture(Resource_Picture.物品_炎阳, "Z") || RegPicture(Resource_Picture.物品_紫苑, "Z") || RegPicture(Resource_Picture.物品_血棘, "Z") || RegPicture(Resource_Picture.物品_羊刀, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Delay(30);
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.物品_勇气, "SPACE") || RegPicture(Resource_Picture.物品_炎阳, "SPACE") || RegPicture(Resource_Picture.物品_紫苑, "SPACE") || RegPicture(Resource_Picture.物品_血棘, "SPACE") || RegPicture(Resource_Picture.物品_羊刀, "SPACE"))
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

        private void 魂戒智力()
        {
            Delay(100);

            切智力腿();
        }

        #endregion

        #region 小松鼠

        private void 捆接种树()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            if (RegPicture(Resource_Picture.物品_纷争, "C") || RegPicture(Resource_Picture.物品_纷争_7, "C", 7))
            {
                KeyPress((uint)Keys.C);
            }

            KeyDown((uint)Keys.LShiftKey);
            KeyPress((uint)Keys.W);
            KeyUp((uint)Keys.LShiftKey);

            var q_down = 0;

            while (q_down == 0)
            {
                if (RegPicture(Resource_Picture.小松鼠_释放野地奇袭, "W") || RegPicture(Resource_Picture.小松鼠_释放野地奇袭_7, "W", 7))
                {
                    Delay(85);
                    KeyPress((uint)Keys.Q);
                    q_down = 1;
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 3600) break;
            }
        }

        private void 飞镖接捆接种树()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            if (RegPicture(Resource_Picture.物品_纷争, "C") || RegPicture(Resource_Picture.物品_纷争_7, "C", 7))
            {
                KeyPress((uint)Keys.C);
            }

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
            if (FindPicture(Resource_Picture.巨魔_远斧头, CaptureScreen(839, 939, 70, 72)).Count > 0)
            {
                KeyPress((uint)Keys.Q);

                Delay(50);

                KeyPress((uint)Keys.W);

                Delay(205);

                RightClick();

                KeyPress((uint)Keys.Q);
            }
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
                    || RegPicture(Resource_Picture.TB_释放倒影_6, "Q", 6) || RegPicture(Resource_Picture.TB_释放倒影_7, "Q", 7))
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
                    || RegPicture(Resource_Picture.TB_释放幻惑_6, "W", 6) || RegPicture(Resource_Picture.TB_释放幻惑_7, "W", 7))
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
                    || RegPicture(Resource_Picture.TB_释放断魂_6, "R", 6) || RegPicture(Resource_Picture.TB_释放断魂_7, "R", 7))
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

        #endregion

        #region 智力

        #region 黑鸟

        private void G_yxc_cg()
        {
            G_yxc(245, 600, 385);
        }

        private void G_yxc_y()
        {
            G_yxc(245, 990, 0);
        }

        private void G_yxc(int dyd, int yd, int dd)
        {
            if (RegPicture(Resource_Picture.物品_纷争, "C"))
            {
                KeyPress((uint)Keys.C);
            }

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
                    if (b_cancel) break;

                    KeyPress((uint)Keys.S);

                    Delay(dd);
                    if (b_cancel) break;
                    KeyPress((uint)Keys.Space);
                }
            }
        }

        #endregion

        #region 谜团

        private void 跳秒接午夜凋零黑洞()
        {
            if (RegPicture(Resource_Picture.物品_黑皇, "Z"))
            {
                KeyPress((uint)Keys.Z);
            }

            if (RegPicture(Resource_Picture.物品_纷争, "C"))
            {
                KeyPress((uint)Keys.C);
            }

            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.物品_跳刀, "SPACE") || RegPicture(Resource_Picture.物品_跳刀_智力跳刀, "SPACE"))
            {
                Delay(30);
                KeyPress((uint)Keys.Space);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 200) break;
            }

            time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.谜团_午夜凋零CD, "E"))
            {
                KeyPress((uint)Keys.E);
                Delay(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 500) break;
            }

            while (RegPicture(Resource_Picture.谜团_黑洞CD, "R"))
            {
                KeyPress((uint)Keys.R);
                Delay(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1000) break;
            }

            Delay(30);

            KeyDown((uint)Keys.LControlKey);

            KeyPress((uint)Keys.A);

            KeyUp((uint)Keys.LControlKey);
        }

        private void 刷新接凋零黑洞()
        {
            KeyPress((uint)Keys.X);
            for (int i = 0; i < 5; i++)
            {
                Delay(30);
                KeyPress((uint)Keys.Z);
                KeyPress((uint)Keys.E);
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
                if (FindPicture(Resource_Picture.冰女_释放冰封禁制, CaptureScreen(859, 939, 64, 62)).Count > 0)
                {
                    Delay(365);
                    KeyPress((uint)Keys.Space);
                    w_down = 1;
                }
        }

        #endregion

        #region 火女

        //private static void 吹风接t()
        //{
        //    var w_down = 0;

        //    while (w_down == 0)
        //        if (FindPicture(Resource_Picture.释放冰封禁制, CaptureScreen(859, 939, 64, 62)).Count > 0)
        //        {
        //            delay(365);
        //            KeyPress((uint)Keys.Space);
        //            w_down = 1;
        //        }
        //}

        #endregion

        #region 蓝猫

        private void 拉接平A()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var w_down = 0;
            while (w_down == 0)
            {
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1500) break;

                if (RegPicture(Resource_Picture.蓝猫_释放电子漩涡, "W"))
                {
                    Task.Run(() =>
                    {
                        Delay(120);
                        KeyPress((uint)Keys.A);
                    });

                    w_down = 1;
                }
            }
        }

        private void 残影接平A()
        {
            Delay(50);
            KeyPress((uint)Keys.A);
        }

        private void 滚接平A()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var r_down = 0;
            while (r_down == 0)
            {
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1500) break;

                if (RegPicture(Resource_Picture.蓝猫_释放球状闪电_红, "R"))
                {
                    Task.Run(() =>
                    {
                        Delay(150);
                        KeyPress((uint)Keys.A);
                    });
                    r_down = 1;
                }
            }
        }

        private void 原地滚A()
        {
            KeyPress((uint)Keys.A);

            Point p = MousePosition;

            var x_差 = p.X - 623;
            var y_差 = p.Y - 1000;
            x_差 /= 8;
            y_差 /= 8;

            for (int i = 1; i <= 8; i++)
            {
                MouseMove(p.X - (x_差 * i), p.Y - (y_差 * i));
                Delay(3);
            }

            MouseMove(623, 1000);
            Delay(30);
            KeyPress((uint)Keys.R);

            for (int i = 1; i <= 8; i++)
            {
                MouseMove(623 + (x_差 * i), 1000 + (y_差 * i));
                Delay(3);
            }

            MouseMove(p.X, p.Y);

        }

        private void 泉水出来喝瓶()
        {
            Delay(200);

            for (int i = 1; i <= 4; i++)
            {
                KeyPress((uint)Keys.C);
                Delay(650);
            }
        }

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
                        if (b_cancel) break;
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

        private void 苍穹振击取消后摇()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var q_down = 0;

            while (q_down == 0)
            {
                if (RegPicture(Resource_Picture.暗影萨满_释放苍穹振击, "Q"))
                {
                    Delay(110);
                    q_down = 1;
                    KeyPress((uint)Keys.A);
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
                    Delay(110);
                    q_down = 1;
                    KeyPress((uint)Keys.A);
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
            }
        }

        private void 变羊取消后摇()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var w_down = 0;

            if (RegPicture(Resource_Picture.暗影萨满_捆绑施法中, 767, 726, 85, 85).Count > 0)
            {
                return;
            }

            while (w_down == 0)
            {
                if (!RegPicture(Resource_Picture.暗影萨满_妖术CD, "W"))
                {
                    label1.Text = "WW";
                    w_down = 1;
                    KeyPress((uint)Keys.A);
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
            }
        }

        private void 暗夜萨满最大化控制链()
        {
            var times = 1.0;
            var time = 0;

            if (RegPicture(Resource_Picture.物品_祭礼长袍_4, "G"))
            {
                times *= 1.1;
            }

            if (RegPicture(Resource_Picture.物品_永恒遗物_4, "G"))
            {
                times *= 1.25;
            }

            times *= ((100 - (Convert.ToDouble(tb_丢装备.Text))) / 100);

            Color 技能点颜色 = Color.FromArgb(255, 203, 183, 124);

            if (CaptureColor(909, 1008).Equals(技能点颜色))
            {
                time = 3500;
            }
            else if (CaptureColor(897, 1008).Equals(技能点颜色))
            {
                time = 2750;
            }
            else if (CaptureColor(885, 1008).Equals(技能点颜色))
            {
                time = 2000;
            }
            else if (CaptureColor(875, 1008).Equals(技能点颜色))
            {
                time = 1250;
            }

            KeyPress((uint)Keys.W);

            loop_bool_2 = true;

            Task.Run(() => 
            {
                var time1 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                var w_down = 0;

                while (w_down == 0)
                {
                    if (!RegPicture(Resource_Picture.暗影萨满_妖术CD, "W")) w_down = 1;

                    if (!loop_bool_2) return;

                    if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time1 > 1200) break;
                }

                Delay(Convert.ToInt32(time * times) - 365);

                if (!loop_bool_2) return;

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
                    全局时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
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
                    Delay(阿哈利姆魔晶() ? 410 : 1410);  // 大部分技能抬手都是0.2-0.3之间
                    if (!loop_bool_2) return;

                    if (RegPicture(Resource_Picture.物品_吹风_7, "SPACE", 7))
                    {
                        KeyPress((uint)Keys.Space);
                        KeyPress((uint)Keys.M);

                        Delay(2500);
                        if (!loop_bool_2) return;
                        作祟暗影之境最大化伤害();
                    }

                    w_down = 1;
                }
            }
        }

        private void 作祟暗影之境最大化伤害()
        {
            // 释放纷争，增加大量伤害
            if (RegPicture(Resource_Picture.物品_纷争_7, "C", 7))
            {
                KeyPress((uint)Keys.C);
            }

            KeyPress((uint)Keys.M);
            Delay(30);
            KeyPress((uint)Keys.D);
            Delay(30);
            KeyPress((uint)Keys.W);
            Delay(30);
            KeyPress((uint)Keys.W);

            var 暗影之境_开始时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            if (阿哈利姆神杖())
            {
                Delay(400);
                KeyPress((uint)Keys.A);
            }
            else
            {
                while (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 暗影之境_开始时间 < 4500 || !loop_bool_2) { }
                if (!loop_bool_2) return;
                KeyPress((uint)Keys.A);
            }
        }

        private void 皇冠接花控制衔接()
        {
            var 晕眩时间 = 1750;

            Color 技能点颜色 = Color.FromArgb(255, 203, 183, 124);
            if (CaptureColor(908, 1004).Equals(技能点颜色))
                晕眩时间 = 1750;

            技能点颜色 = Color.FromArgb(255, 203, 183, 124);
            if (CaptureColor(920, 1004).Equals(技能点颜色))
                晕眩时间 = 2250;

            技能点颜色 = Color.FromArgb(255, 180, 162, 107);
            if (CaptureColor(931, 1005).Equals(技能点颜色))
                晕眩时间 = 2750;

            技能点颜色 = Color.FromArgb(255, 180, 162, 107);
            if (CaptureColor(931, 1005).Equals(技能点颜色))
                晕眩时间 = 2750;

            技能点颜色 = Color.FromArgb(255, 203, 183, 124);
            if (CaptureColor(944, 1004).Equals(技能点颜色))
                晕眩时间 = 3250;

            技能点颜色 = Color.FromArgb(255, 246, 175, 57);
            if (CaptureColor(759, 988).Equals(技能点颜色))
                晕眩时间 += 600;

            // 950 是第一朵花生效时间,
            while (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 全局时间 < (晕眩时间 + (阿哈利姆魔晶() ? 3000 : 4000) - 950) || !loop_bool_2) { }
            if (!loop_bool_2) return;
            MouseMove(MousePosition.X - 120, MousePosition.Y);
            KeyPress((uint)Keys.Q);
            LeftClick();
        }

        #endregion

        #region 天怒
        private void 循环奥数鹰隼()
        {
            while (loop_bool_1)
            {
                if (RegPicture(Resource_Picture.天怒_魔法鹰隼_金饰品, "Q") || RegPicture(Resource_Picture.天怒_魔法鹰隼_金饰品_刚CD好, "Q"))
                {
                    KeyPress((uint)Keys.Q);
                    label1.Text = "D32";
                    Delay(200);
                }

                Delay(15);
            }
        }
        private void 奥数鹰隼取消后摇()
        {
            var 总开始时间 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var q_down = 0;
            while (q_down == 0)
            {
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 总开始时间 > 1200) return;

                if (RegPicture(Resource_Picture.天怒_释放魔法鹰隼_金饰品, "Q"))
                {
                    Delay(100);
                    KeyPress((uint)Keys.M);
                    q_down = 1;
                }

                Delay(15);
            }
        }

        private void 天怒秒人连招()
        {
            if (RegPicture(Resource_Picture.物品_羊刀_4, "Z") && loop_bool_2)
            {
                KeyPress((uint)Keys.Z);

                var 超时标准 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                while (!RegPicture(Resource_Picture.物品_羊刀_4_进入CD, "Z") && loop_bool_2)
                {
                    if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 超时标准 > 2000) return;
                }
            }

            if (RegPicture(Resource_Picture.物品_虚灵之刃_4, "X") && loop_bool_2)
            {
                KeyPress((uint)Keys.X);

                var 超时标准 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                while (!RegPicture(Resource_Picture.物品_虚灵之刃_4_进入CD, "X") && loop_bool_2)
                {
                    if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 超时标准 > 2000) return;
                }
            }

            if (RegPicture(Resource_Picture.物品_虚灵之刃_4, "X") && loop_bool_2)
            {
                KeyPress((uint)Keys.X);

                var 超时标准 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                while (!RegPicture(Resource_Picture.物品_虚灵之刃_4_进入CD, "X") && loop_bool_2)
                {
                    if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 超时标准 > 2000) return;
                }
            }

            if ((RegPicture(Resource_Picture.物品_阿托斯之棍_4, "SPACE") || RegPicture(Resource_Picture.物品_缚灵锁_4, "SPACE")) && loop_bool_2)
            {
                KeyPress((uint)Keys.Space);

                var 超时标准 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                while ((!RegPicture(Resource_Picture.物品_阿托斯之棍_4_进入CD, "SPACE") && !RegPicture(Resource_Picture.物品_缚灵锁_4_进入CD, "SPACE")) && loop_bool_2)
                {
                    if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 超时标准 > 2000) return;
                }
            }


            if (RegPicture(Resource_Picture.物品_纷争, "C") && loop_bool_2)
            {
                KeyPress((uint)Keys.C);
            }

            KeyPress((uint)Keys.W);

            tb_状态抗性.Text = "Q";

            if (RegPicture(Resource_Picture.天怒_魔法鹰隼_金饰品, "Q") && loop_bool_2)
            {
                KeyPress((uint)Keys.Q);

                var 超时标准 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                while (!RegPicture(Resource_Picture.天怒_释放魔法鹰隼_金饰品, "Q") && loop_bool_2)
                {
                    if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 超时标准 > 2000) return;
                }
            }

            tb_状态抗性.Text = "E";

            Delay(100);  // 施法前腰

            if (RegPicture(Resource_Picture.天怒_上古封印, "E") && loop_bool_2)
            {
                KeyPress((uint)Keys.E);

                var 超时标准 = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                while (!RegPicture(Resource_Picture.天怒_释放上古封印, "E") && loop_bool_2)
                {
                    if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - 超时标准 > 2000) return;
                }
            }            

            tb_状态抗性.Text = "R";

            Delay(100);  // 施法前腰

            if (RegPicture(Resource_Picture.天怒_神秘之耀, "R") && loop_bool_2)
            {
                KeyPress((uint)Keys.R);
            }

            loop_bool_2 = false;
        }

        #endregion

        #region 炸弹人
        private void 魂戒丢装备()
        {
            批量扔装备();
        }

        #endregion

        #endregion

        #region 通用

        #region 跳刀

        /// <summary>
        ///     用于快速先手无转身
        /// </summary>
        /// <returns></returns>
        private Point 正面跳刀_无转身()
        {
            // 坐标
            var mousePosition = MousePosition;

            var list = FindPicture(Resource_Picture.血量_自身血量, CaptureScreen(0, 0, 1920, 1080), matchRate: 0.6);

            // X间距
            double move_X = 0;
            // Y间距，自动根据X调整
            double move_Y = 0;

            if (list.Count > 0)
            {
                double realX = list[0].X + 55;
                double realY = list[0].Y + 117;

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

                textBox3.Text = move_X.ToString();
                textBox4.Text = move_Y.ToString();
            }

            return new Point(Convert.ToInt16(move_X), Convert.ToInt16(move_Y));
        }

        #endregion

        #region 扔装备

        private void 批量扔装备()
        {
            var list_1 = tb_丢装备.Text.Split(',');

            var list = RegPicture(Resource_Picture.血量_自身血量, 0, 0, 1920, 1080, 0.8);

            if (list.Count > 0)
            {
                var point = new Point(list[0].X + 55, list[0].Y + 117);
                tb_状态抗性.Text = point.X.ToString() + " " + point.Y.ToString();
                try
                {
                    switch (list_1[0])
                    {
                        case "6":
                            for (int i = 1; i < list_1.Length; i++)
                            {
                                switch (list_1[i])
                                {
                                    case "1":
                                        扔装备(new Point(1191, 963), point);
                                        break;
                                    case "2":
                                        扔装备(new Point(1259, 963), point);
                                        break;
                                    case "3":
                                        扔装备(new Point(1325, 963), point);
                                        break;
                                    case "4":
                                        扔装备(new Point(1191, 1011), point);
                                        break;
                                    case "5":
                                        扔装备(new Point(1259, 1011), point);
                                        break;
                                    case "6":
                                        扔装备(new Point(1325, 1011), point);
                                        break;
                                    case "7":
                                        扔装备(new Point(1384, 994), point);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        case "4":
                            for (int i = 1; i < list_1.Length; i++)
                            {
                                switch (list_1[i])
                                {
                                    case "1":
                                        扔装备(new Point(1145, 966), point);
                                        break;
                                    case "2":
                                        扔装备(new Point(1214, 963), point);
                                        break;
                                    case "3":
                                        扔装备(new Point(1288, 963), point);
                                        break;
                                    case "4":
                                        扔装备(new Point(1145, 1011), point);
                                        break;
                                    case "5":
                                        扔装备(new Point(1214, 1011), point);
                                        break;
                                    case "6":
                                        扔装备(new Point(1288, 1011), point);
                                        break;
                                    case "7":
                                        扔装备(new Point(1337, 994), point);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void 扔装备(Point p, Point p1)
        {            
            MouseMove(p);
            LeftDown();
            Delay(40);
            MouseMove(p1);
            Delay(40);
            LeftUp();
            Delay(40);
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
            return RegPicture(Resource_Picture.物品_跳刀_智力跳刀BUFF, 400, 865, 1000, 60).Count > 0;
        }

        private static bool 阿哈利姆神杖()
        {
            Color 技能点颜色 = Color.FromArgb(255, 32, 183, 249);
            if (CaptureColor(1078, 958).Equals(技能点颜色))
                return true;
            // 4技能A杖

            技能点颜色 = Color.FromArgb(255, 30, 188, 252);
            if (CaptureColor(1094, 960).Equals(技能点颜色))
                return true;
            // 5技能A杖

            技能点颜色 = Color.FromArgb(255, 30, 189, 253);
            if (CaptureColor(1110, 960).Equals(技能点颜色))
                return true;
            // 5技能A杖

            技能点颜色 = Color.FromArgb(255, 30, 187, 250);
            return CaptureColor(1122, 959).Equals(技能点颜色);
            // 6技能A杖
        }

        private static bool 阿哈利姆魔晶()
        {
            Color 技能点颜色 = Color.FromArgb(255, 34, 186, 254);

            if (CaptureColor(1094, 995).Equals(技能点颜色))
                return true;
            // 7技能魔晶

            技能点颜色 = Color.FromArgb(255, 29, 187, 255);

            if (CaptureColor(1110, 994).Equals(技能点颜色))
                return true;
            // 6技能魔晶无A

            技能点颜色 = Color.FromArgb(255, 28, 187, 255);

            if (CaptureColor(1121, 993).Equals(技能点颜色))
                return true;
            // 6技能魔晶A

            技能点颜色 = Color.FromArgb(255, 28, 187, 255);

            if (CaptureColor(1077, 993).Equals(技能点颜色))
                return true;
            // 4技能魔晶

            技能点颜色 = Color.FromArgb(255, 30, 187, 254);

            if (CaptureColor(1111, 994).Equals(技能点颜色))
                return true;
            // 5技能魔晶

            return false;
        }

        #endregion

        #region 图片识别

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bp">图片</param>
        /// <param name="position">位置</param>
        /// <param name="ablityCount">拥有技能数 4 5 6 7 其中7是6技能先出A杖没出魔晶</param>
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
            }

            return FindPicture(bp, CaptureScreen(x, y, width, height), matchRate: matchRate).Count > 0;
        }

        private static List<Point> RegPicture(Bitmap bp, int x, int y, int width, int height, double matchRate = 0.9)
        {
            return FindPicture(bp, CaptureScreen(x, y, width, height), matchRate: matchRate);
        }

        #endregion

        #endregion

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
            while (RegPicture(Resource1.最后一张, 1132, 947, 40, 40, 1).Count == 0)
            {
                KeyDown((uint)Keys.LControlKey);

                KeyPress((uint)Keys.S);

                KeyUp((uint)Keys.LControlKey);

                Delay(250);

                KeyPress((uint)Keys.Enter);

                Delay(500);

                KeyPress((uint)Keys.Right);

                Delay(500);

                if (b_cancel) break;
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
            Change_Pic();
            StartListen();
        }

        /// <summary>
        ///     开始监听和初始化模拟
        /// </summary>
        public int StartListen()
        {
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

            // 设置窗口位置
            Location = new Point(338, 1013);

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
            KeyboardMouseSimulateDriverAPI.MouseDown((uint)Dota2Simulator.MouseButtons.RightDown);
            KeyboardMouseSimulateDriverAPI.MouseUp((uint)Dota2Simulator.MouseButtons.RightUp);
        }

        private static void LeftClick()
        {
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

        private new static void KeyUp(uint key)
        {
            KeyboardMouseSimulateDriverAPI.KeyUp(key);
        }

        private new static void KeyDown(uint key)
        {
            KeyboardMouseSimulateDriverAPI.KeyDown(key);
        }

        private new static void KeyPress(uint key)
        {
            KeyDown(key);
            KeyUp(key);
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

        #region 延时

        /// <summary>
        /// 精简延迟实现
        /// </summary>
        /// <param name="delay"></param>
        private static void Delay(int delay)
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            while (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= delay) { }
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
}