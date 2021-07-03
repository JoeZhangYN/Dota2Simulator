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
        private bool loop_bool_1;

        /// <summary>
        ///     循环计数2
        /// </summary>
        private bool loop_bool_2;

        ///// <summary>
        /////     模拟按键
        ///// </summary>
        //private readonly IPressKey mPressKey;

        /// <summary>
        ///     按键钩子，用于捕获按下的键
        /// </summary>
        private KeyEventHandler myKeyEventHandeler; //按键钩子


        /// <summary>
        ///     案件触发，名称指定操作
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
            if (e.KeyValue == (uint)Keys.N) Task.Run(扔装备);

            #region 力量

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

                    if (RegPicture(Resource_Picture.臂章, "Z"))
                    {
                        KeyPress((uint)Keys.Z);
                        Thread.Sleep(30);
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
                //    _taskDelay = new Task(() => Thread.Sleep(1000));

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
                    切智力腿();

                    Task.Run(法术反制敏捷);
                }
                else if (e.KeyValue == (uint)Keys.R)
                {
                    label1.Text = "R";
                    切智力腿();

                    Task.Run(法力虚空取消后摇);
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

                    if (RegPicture(Resource_Picture.魂戒CD, "Z"))
                        切力量腿();

                    Task.Run(魂戒敏捷);
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
                else if (e.KeyValue == (uint)Keys.W || e.KeyValue == (uint)Keys.R || e.KeyValue == (uint)Keys.C || e.KeyValue == (uint)Keys.Space)
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
                    Thread.Sleep(170);

                    KeyUp((uint)Keys.L);

                    KeyPress((uint)Keys.W);
                }
                else if (e.KeyValue == (uint)Keys.D)
                {
                    label1.Text = "D";

                    切智力腿();

                    Task.Run(深海护罩敏捷);
                }
                else if (e.KeyValue == (uint)Keys.X)
                {
                    label1.Text = "Item";

                    切智力腿();
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
                else if (e.KeyValue == (uint)Keys.D2)
                {
                    label1.Text = "W";

                    切智力腿();

                    KeyPress((uint)Keys.W);

                    Task.Run(神行百变敏捷);
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
                if (e.KeyValue == (uint)Keys.Q)
                    切智力腿();
                else if (e.KeyValue == (uint)Keys.W)
                    切敏捷腿();
                else if (e.KeyValue == (uint)Keys.E) 切力量腿();
            }

            #endregion
        }


        #region Dota2具体实现

        #region 力量

        #region 斧王

        private void 跳吼()
        {
            while (RegPicture(Resource_Picture.刃甲, "X"))
            {
                KeyPress((uint)Keys.X);
                Thread.Sleep(30);
            }

            KeyPress((uint)Keys.Space);


            while (RegPicture(Resource_Picture.狂战士之吼, "Q") || RegPicture(Resource_Picture.狂战士之吼_金色饰品, "Q"))
            {
                KeyPress((uint)Keys.Q);
                Thread.Sleep(30);
            }

            KeyDown((uint)Keys.LControlKey);
            KeyPress((uint)Keys.A);
            KeyUp((uint)Keys.LControlKey);

            Thread.Sleep(430);

            切敏捷腿();

            if (RegPicture(Resource_Picture.分身, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Thread.Sleep(30);
            }

            KeyDown((uint)Keys.LControlKey);
            KeyPress((uint)Keys.A);
            KeyUp((uint)Keys.LControlKey);
        }

        private void 战斗饥渴取消后摇()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var w_down = 0;
            while (w_down == 0)
            {
                if (RegPicture(Resource_Picture.战斗饥渴, "W"))
                {
                    Thread.Sleep(302);
                    KeyPress((uint)Keys.A);
                    w_down = 1;

                    切敏捷腿();

                    Thread.Sleep(200);

                    切敏捷腿();
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
            }
        }

        private void 淘汰之刃后()
        {
            Thread.Sleep(200);

            切敏捷腿();

            Thread.Sleep(200);

            切敏捷腿();

            Thread.Sleep(200);

            切敏捷腿();
        }

        #endregion

        #region 军团

        private void 决斗()
        {
            // 攻速
            var IAS = Convert.ToDouble(tb_IAS.Text) / 100;

            var p = 正面跳刀_无转身();

            if (RegPicture(Resource_Picture.魂戒CD, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Thread.Sleep(30);
            }

            if (RegPicture(Resource_Picture.臂章, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Thread.Sleep(30);
            }

            var wDown = 0;

            if (RegPicture(Resource_Picture.强攻CD, "W", matchRate: 0.7))
            {
                KeyPress((uint)Keys.D);
                KeyPress((uint)Keys.D);

                while (wDown == 0)
                    if (RegPicture(Resource_Picture.释放强攻, "W", matchRate: 0.7))
                    {
                        Thread.Sleep(95);
                        wDown = 1;
                        IAS += 1.4;
                    }
            }

            if (RegPicture(Resource_Picture.黑皇, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Thread.Sleep(30);
            }

            if (RegPicture(Resource_Picture.相位, "B"))
            {
                KeyPress((uint)Keys.B);
                Thread.Sleep(30);
            }

            if (RegPicture(Resource_Picture.相位, "C"))
            {
                KeyPress((uint)Keys.C);
                Thread.Sleep(30);
            }

            if (RegPicture(Resource_Picture.刃甲, "X"))
            {
                KeyPress((uint)Keys.X);
                Thread.Sleep(30);
            }

            var point = MousePosition;

            MouseMove(p.X, p.Y, false);

            KeyPress((uint)Keys.Space);

            Thread.Sleep(5);

            MouseMove(point.X, point.Y, false);


            if (wDown == 2)
            {
                KeyPress((uint)Keys.A);
                Thread.Sleep(30);

                Thread.Sleep(30);
                // 等待攻击结束
                Thread.Sleep(Convert.ToInt16(460 / IAS));
            }

            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.否决, "X"))
            {
                KeyPress((uint)Keys.X);
                Thread.Sleep(30);
                KeyPress((uint)Keys.A);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 150) break;
            }

            while (RegPicture(Resource_Picture.天堂, "C"))
            {
                KeyPress((uint)Keys.C);
                Thread.Sleep(30);
                KeyPress((uint)Keys.A);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 150) break;
            }

            while (RegPicture(Resource_Picture.勇气, "V"))
            {
                KeyPress((uint)Keys.V);
                Thread.Sleep(30);
                KeyPress((uint)Keys.A);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 150) break;
            }

            while (RegPicture(Resource_Picture.炎阳, "V"))
            {
                KeyPress((uint)Keys.V);
                Thread.Sleep(30);
                KeyPress((uint)Keys.A);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 150) break;
            }

            while (RegPicture(Resource_Picture.决斗CD, "R"))
            {
                KeyPress((uint)Keys.R);
                Thread.Sleep(30);

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
                if (FindPicture(Resource_Picture.释放深渊, CaptureScreen(857, 939, 70, 72)).Count > 0)
                {
                    Thread.Sleep(400);
                    KeyPress((uint)Keys.A);

                    KeyPress((uint)Keys.Q);

                    Thread.Sleep(640);

                    KeyPress((uint)Keys.A);
                    Thread.Sleep(800);

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
                    Thread.Sleep(110);

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
                    Thread.Sleep(400);

                    while (RegPicture(Resource_Picture.刃甲, "X"))
                    {
                        KeyPress((uint)Keys.X);
                        Thread.Sleep(30);
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
            if (RegPicture(Resource_Picture.臂章, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Thread.Sleep(30);
            }

            var p = 正面跳刀_无转身();

            var point = MousePosition;

            MouseMove(p.X, p.Y, false);

            // 跳刀空格
            KeyPress((uint)Keys.Space);

            Thread.Sleep(5);

            MouseMove(point.X, point.Y, false);

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
                    Thread.Sleep(400);
                }

                else if (RegPicture(Resource_Picture.钢背_针刺刚CD好, "W"))
                {
                    KeyPress((uint)Keys.W);
                    label1.Text = "D22";

                    Thread.Sleep(400);
                }

                else if (RegPicture(Resource_Picture.钢背_针刺CD_5, "W", 5))
                {
                    KeyPress((uint)Keys.W);
                    label1.Text = "D22";

                    Thread.Sleep(400);
                }

                Thread.Sleep(15);
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
                        Thread.Sleep(400);
                    }

                    if (RegPicture(Resource_Picture.钢背_鼻涕CD_5, "Q", 5))
                    {
                        KeyPress((uint)Keys.Q);
                        label1.Text = "D32";
                        Thread.Sleep(400);
                    }

                    Thread.Sleep(15);
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
                    Thread.Sleep(50);
                    切敏捷腿();
                    KeyPress((uint)Keys.A);

                    q_down = 1;
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 600) break;
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
                    Thread.Sleep(50);
                    切敏捷腿();
                    KeyPress((uint)Keys.A);

                    r_down = 1;
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 700) break;
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
        //            Thread.Sleep(830);
        //            KeyPress((uint)Keys.R);
        //        }
        //}

        #endregion

        #region 小骷髅

        private void 扫射接勋章()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            // 勋章放在c位置
            while (RegPicture(Resource_Picture.勇气, "C"))
            {
                KeyPress((uint)Keys.C);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            while (RegPicture(Resource_Picture.炎阳, "C"))
            {
                KeyPress((uint)Keys.C);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.紫苑, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            while (RegPicture(Resource_Picture.紫苑, "SPACE"))
            {
                KeyPress((uint)Keys.Space);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            while (RegPicture(Resource_Picture.血棘, "Z"))
            {
                KeyPress((uint)Keys.Z);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            while (RegPicture(Resource_Picture.血棘, "SPACE"))
            {
                KeyPress((uint)Keys.Space);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.羊刀, "SPACE"))
            {
                KeyPress((uint)Keys.Space);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            // 否决放在x
            while (RegPicture(Resource_Picture.否决, "X"))
            {
                KeyPress((uint)Keys.X);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 100) break;
            }

            KeyPress((uint)Keys.A);

            Thread.Sleep(100);

            切敏捷腿();
        }

        private void 魂戒敏捷()
        {
            Thread.Sleep(80);

            切敏捷腿();
        }

        #endregion

        #region 小松鼠

        private void 捆接种树()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            if (RegPicture(Resource_Picture.纷争, "C") || RegPicture(Resource_Picture.纷争_7, "C", 7))
            {
                KeyPress((uint)Keys.C);
            }

            KeyPress((uint)Keys.W);

            var q_down = 0;

            while (q_down == 0)
            {
                if (RegPicture(Resource_Picture.小松鼠_释放野地奇袭, "W") || RegPicture(Resource_Picture.小松鼠_释放野地奇袭_7, "W", 7))
                {
                    Thread.Sleep(85);
                    KeyPress((uint)Keys.Q);
                    q_down = 1;
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1000) break;
            }
        }

        private void 飞镖接捆接种树()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            if (RegPicture(Resource_Picture.纷争, "C") || RegPicture(Resource_Picture.纷争_7, "C", 7))
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
                    Thread.Sleep(107);
                    KeyPress((uint)Keys.W);
                    f_down = true;
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1000) break;
            }

            while (!w_down)
            {
                if (RegPicture(Resource_Picture.小松鼠_释放野地奇袭_7, "W", 7))
                {
                    Thread.Sleep(85);
                    KeyPress((uint)Keys.Q);
                    w_down = true;
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1500) break;
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
                    Thread.Sleep(79);
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
            Thread.Sleep(100);
            切敏捷腿();
        }

        #endregion

        #region 巨魔

        private void 远程飞斧()
        {
            if (FindPicture(Resource_Picture.远斧头, CaptureScreen(839, 939, 70, 72)).Count > 0)
            {
                KeyPress((uint)Keys.Q);

                Thread.Sleep(50);

                KeyPress((uint)Keys.W);

                Thread.Sleep(205);

                RightClick();

                KeyPress((uint)Keys.Q);
            }
        }

        #endregion

        #region 小鱼人

        private void 黑暗契约力量()
        {
            // 为了避免切太快导致实际上还是敏捷腿
            Thread.Sleep(150);

            KeyPress((uint)Keys.A);

            切敏捷腿();

            #region 切智力后力量后敏捷，实际作用前期少减12点血。

            //Thread.Sleep(1424);
            //切力量腿();
            //Thread.Sleep(930);
            //切敏捷腿();
            //Thread.Sleep(300);
            //切敏捷腿();
            //Thread.Sleep(300);
            //切敏捷腿();

            #endregion
        }

        private void 跳水敏捷()
        {
            // 为了避免切太快导致实际上还是敏捷腿
            Thread.Sleep(150);

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
                    Thread.Sleep(50);

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
                if (RegPicture(Resource_Picture.释放灵魂之矛, "Q"))
                {
                    Thread.Sleep(125);

                    切敏捷腿();

                    w_down = 1;
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 2000) break;
            }
        }

        private void 神行百变敏捷()
        {
            Thread.Sleep(1350);

            切敏捷腿();
        }

        #endregion

        #region 敌法

        private void 闪烁敏捷()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var w_down = 0;

            while (w_down == 0)
            {
                if (RegPicture(Resource_Picture.敌法_闪烁_4, "W") || RegPicture(Resource_Picture.敌法_信仰之源闪烁_4, "W") ||
                    RegPicture(Resource_Picture.敌法_闪烁_5, "W", 5) || RegPicture(Resource_Picture.敌法_信仰之源闪烁_5, "W", 5))
                {
                    Thread.Sleep(95);
                    KeyPress((uint)Keys.A);
                    切敏捷腿();

                    w_down = 1;
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

                切敏捷腿();

                break;
            }
        }

        private void 法术反制敏捷()
        {
            Thread.Sleep(10);
            KeyPress((uint)Keys.V);
        }

        private void 法力虚空取消后摇()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var w_down = 0;

            while (w_down == 0)
            {
                if (RegPicture(Resource_Picture.敌法_法力虚空_4, "R") || RegPicture(Resource_Picture.敌法_碎颅锤法力虚空_4, "R") ||
                    RegPicture(Resource_Picture.敌法_法力虚空_5, "R", 5) || RegPicture(Resource_Picture.敌法_碎颅锤法力虚空_5, "R", 5))
                {
                    Thread.Sleep(100);

                    KeyPress((uint)Keys.A);

                    切敏捷腿();

                    Thread.Sleep(50);

                    KeyPress((uint)Keys.A);

                    w_down = 1;
                }

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time <= 800) continue;

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
            KeyPress((uint)Keys.W);

            var w_down = 0;

            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (w_down == 0)
            {
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 6000) break;

                if (!RegPicture(Resource_Picture.黑鸟_关释放, "W")) continue;

                w_down = 1;
                Thread.Sleep(dyd);
                RightClick();

                Thread.Sleep(yd);
                if (b_cancel) break;

                KeyPress((uint)Keys.S);

                Thread.Sleep(dd);
                if (b_cancel) break;
                KeyPress((uint)Keys.Space);
            }
        }

        #endregion

        #region 谜团

        private void 跳秒接午夜凋零黑洞()
        {
            if (RegPicture(Resource_Picture.黑皇, "Z"))
            {
                KeyPress((uint)Keys.Z);
            }

            if (RegPicture(Resource_Picture.纷争, "C"))
            {
                KeyPress((uint)Keys.C);
            }

            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.跳刀, "SPACE") || RegPicture(Resource_Picture.智力跳刀, "SPACE"))
            {
                Thread.Sleep(30);
                KeyPress((uint)Keys.Space);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 200) break;
            }

            time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            while (RegPicture(Resource_Picture.午夜凋零CD, "E"))
            {
                KeyPress((uint)Keys.E);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 500) break;
            }

            while (RegPicture(Resource_Picture.黑洞CD, "R"))
            {
                KeyPress((uint)Keys.R);
                Thread.Sleep(30);

                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1000) break;
            }

            Thread.Sleep(30);

            KeyDown((uint)Keys.LControlKey);

            KeyPress((uint)Keys.A);

            KeyUp((uint)Keys.LControlKey);
        }

        private void 刷新接凋零黑洞()
        {
            KeyPress((uint)Keys.X);
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(30);
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
                if (FindPicture(Resource_Picture.释放冰封禁制, CaptureScreen(859, 939, 64, 62)).Count > 0)
                {
                    Thread.Sleep(365);
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
        //            Thread.Sleep(365);
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

                if (FindPicture(Resource_Picture.释放电子旋涡, CaptureScreen(859, 939, 64, 62)).Count > 0)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(285);
                        KeyPress((uint)Keys.A);
                    });

                    w_down = 1;
                }
            }
        }

        private void 残影接平A()
        {
            Thread.Sleep(50);
            KeyPress((uint)Keys.A);
        }

        private void 滚接平A()
        {
            var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            var r_down = 0;
            while (r_down == 0)
            {
                if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 1500) break;

                if (FindPicture(Resource_Picture.释放球状闪电, CaptureScreen(991, 939, 64, 62)).Count > 0)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(150);
                        KeyPress((uint)Keys.A);
                    });
                    r_down = 1;
                }
            }
        }

        //private void 平A接滚()
        //{
        //    KeyPress((uint)Keys.A);

        //    Thread.Sleep(Convert.ToInt16(tb_IAS.Text));

        //    KeyPress((uint)Keys.R);
        //}

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
                    Thread.Sleep(智力跳刀BUFF() ? 50 : 80);
                    KeyPress((uint)Keys.A);

                    label1.Text = "QQQ";
                }
                else if (RegPicture(Resource_Picture.宙斯_雷云后释放弧形闪电, "Q", 5))
                {
                    q_down = 1;
                    Thread.Sleep(智力跳刀BUFF() ? 50 : 80);
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
                    Thread.Sleep(智力跳刀BUFF() ? 70 : 125);
                    KeyPress((uint)Keys.A);

                    label1.Text = "WWW";
                }
                else if (RegPicture(Resource_Picture.宙斯_雷云后释放雷击, "W", 5))
                {
                    w_down = 1;
                    Thread.Sleep(智力跳刀BUFF() ? 70 : 125);
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
            Thread.Sleep(30);
            KeyPress((uint)Keys.Q);
            Thread.Sleep(30);
            KeyPress((uint)Keys.Q);
            Thread.Sleep(30);
        }

        private void 三火平A()
        {
            KeyPress((uint)Keys.E);
            Thread.Sleep(30);
            KeyPress((uint)Keys.E);
            Thread.Sleep(30);
            KeyPress((uint)Keys.E);
            Thread.Sleep(30);
        }

        private void 三雷幽灵()
        {
            KeyPress((uint)Keys.Q);
            Thread.Sleep(30);
            KeyPress((uint)Keys.Q);
            Thread.Sleep(30);
            KeyPress((uint)Keys.W);
            Thread.Sleep(30);
            KeyPress((uint)Keys.R);
            Thread.Sleep(30);
            KeyPress((uint)Keys.W);
            Thread.Sleep(30);
            KeyPress((uint)Keys.W);
            Thread.Sleep(30);
            KeyPress((uint)Keys.D);
        }

        #endregion

        #region 拉席克

        private void 吹风接撕裂大地()
        {
            var all_down = 0;
            if (RegPicture(Resource_Picture.吹风CD完, "SPACE"))
            {
                label1.Text = "FF";
                KeyPress((uint)Keys.Space);
                Thread.Sleep(30);

                var time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                while (all_down == 0)
                {
                    if (new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - time > 4000) break;

                    if (RegPicture(Resource_Picture.吹风CD, "SPACE"))
                    {
                        label1.Text = "FFF";
                        if (RegPicture(Resource_Picture.纷争, "C")) KeyPress((uint)Keys.C);
                        Thread.Sleep(80);
                        KeyPress((uint)Keys.H);
                        Thread.Sleep(1280);
                        if (b_cancel) break;
                        KeyPress((uint)Keys.Q);
                        Thread.Sleep(760);
                        KeyPress((uint)Keys.R);
                        all_down = 1;
                    }

                    Thread.Sleep(50);
                }
            }
        }

        #endregion

        #endregion



        #region 通用

        /// <summary>
        ///     用于快速先手无转身
        /// </summary>
        /// <returns></returns>
        private Point 正面跳刀_无转身()
        {
            // 坐标
            var mousePosition = MousePosition;

            var list = FindPicture(Resource_Picture.自身血量, CaptureScreen(0, 0, 1920, 1080), matchRate: 0.6);

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

        private void 扔装备()
        {
            var p = MousePosition;
            var list = RegPicture(Resource_Picture.自身血量, 0, 0, 1920, 1080, 0.8);
            if (list.Count > 0)
            {
                LeftDown();
                MouseMove(list[0].X + 55, list[0].Y + 117, false);
                Thread.Sleep(30);
                LeftUp();
                MouseMove(p.X, p.Y, false);
            }
        }

        private static void 切智力腿()
        {
            if (RegPicture(Resource_Picture.力量腿, "V") || RegPicture(Resource_Picture.力量腿_5, "V", 5))
            {
                KeyPress((uint)Keys.V);
            }
            else if (RegPicture(Resource_Picture.敏捷腿, "V") || RegPicture(Resource_Picture.敏捷腿_5, "V", 5))
            {
                KeyPress((uint)Keys.V);
                //Thread.Sleep(30);
                KeyPress((uint)Keys.V);
            }
        }

        private static void 切敏捷腿()
        {
            if (RegPicture(Resource_Picture.力量腿, "V") || RegPicture(Resource_Picture.力量腿_5, "V", 5))
            {
                KeyPress((uint)Keys.V);
                //Thread.Sleep(30);
                KeyPress((uint)Keys.V);
            }
            else if (RegPicture(Resource_Picture.智力腿, "V") || RegPicture(Resource_Picture.智力腿_5, "V", 5))
            {
                KeyPress((uint)Keys.V);
            }
        }

        private static void 切力量腿()
        {
            if (RegPicture(Resource_Picture.敏捷腿, "V") || RegPicture(Resource_Picture.敏捷腿_5, "V", 5))
            {
                KeyPress((uint)Keys.V);
            }
            else if (RegPicture(Resource_Picture.智力腿, "V") || RegPicture(Resource_Picture.智力腿_5, "V", 5))
            {
                KeyPress((uint)Keys.V);
                //Thread.Sleep(30);
                KeyPress((uint)Keys.V);
            }
        }

        private void 切臂章()
        {
            KeyPress((uint)Keys.Z);
            KeyPress((uint)Keys.Z);
        }

        //private static void 死灵射手净化()
        //{
        //    KeyPress((uint)Keys.D1);
        //    Thread.Sleep(30);
        //    KeyPress((uint)Keys.Q);
        //    Thread.Sleep(30);
        //    KeyPress((uint)Keys.F1);
        //}

        private static bool 智力跳刀BUFF()
        {
            return RegPicture(Resource_Picture.智力跳刀BUFF, 400, 865, 1000, 60).Count > 0;
        }

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

        private static bool 阿哈利姆神杖()
        {
            if (RegPicture(Resource_Picture._4格A杖效果, 1058, 944, 40, 70).Count > 0)
                return true;
            if (RegPicture(Resource_Picture._5格A杖效果, 1084, 944, 40, 70).Count > 0)
                return true;
            return RegPicture(Resource_Picture._6格A杖效果, 1102, 944, 40, 70).Count > 0;
        }

        private static List<Point> RegPicture(Bitmap bp, int x, int y, int width, int height, double matchRate = 0.9)
        {
            return FindPicture(bp, CaptureScreen(x, y, width, height), matchRate: matchRate);
        }

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
                    pictureBox1.Image = Resource_Picture.吹风CD;
                    break;
                case "黑鸟":
                    pictureBox1.Image = Resource_Picture.黑鸟_关释放;
                    break;
                case "军团":
                    pictureBox1.Image = Resource_Picture.释放强攻;
                    break;
                case "孽主":
                    pictureBox1.Image = Resource_Picture.释放深渊;
                    break;
                case "巨魔":
                    pictureBox1.Image = Resource_Picture.远斧头;
                    break;
                case "冰女":
                    pictureBox1.Image = Resource_Picture.释放冰封禁制;
                    break;
                case "蓝猫":
                    pictureBox1.Image = Resource_Picture.释放电子旋涡;
                    break;
                case "小骷髅":
                    pictureBox1.Image = Resource_Picture.敏捷腿;
                    break;
                case "小鱼人":
                case "拍拍":
                    pictureBox1.Image = Resource_Picture.敏捷腿;
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

                Thread.Sleep(250);

                KeyPress((uint)Keys.Enter);

                Thread.Sleep(500);

                KeyPress((uint)Keys.Right);

                Thread.Sleep(500);

                if (b_cancel) break;
            }
        }

        #endregion

        private void Btn_Test_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (阿哈利姆神杖())
                    if (RegPicture(Resource_Picture.钢背_鼻涕CD, "Q"))
                    {
                        KeyPress((uint)Keys.Q);
                        label1.Text = "D32";
                    }
            });
        }

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

            // 设置窗体显示在最上层
            i = SetWindowPos(Handle, -1, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0010 | 0x0080);

            // 设置本窗体为活动窗体
            SetActiveWindow(Handle);
            SetForegroundWindow(Handle);

            // 设置窗体置顶
            TopMost = true;

            //WinIO32.Initialize();

            // 初始化键盘鼠标模拟，仅模仿系统函数，winIo 和 WinRing0 需要额外的操作
            i += KeyboardMouseSimulateDriverAPI.Initialize((uint)SimulateWays.Event);

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

        public new static void MouseMove(int X, int Y, bool relative = true)
        {
            if (relative)
            {
                var p = MousePosition;
                X += p.X;
                Y += p.Y;
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
}