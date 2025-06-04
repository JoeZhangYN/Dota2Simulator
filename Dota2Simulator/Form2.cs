using Collections.Pooled;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.PictureProcessing.OCR;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using static Dota2Simulator.SetWindowTop;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Keys = System.Windows.Forms.Keys;
using Point = System.Drawing.Point;

namespace Dota2Simulator
{
    internal partial class Form2 : Form
    {
        #region 页面单例调用UI线程
        // 单例模式 传递Form,调用UI线程更新
        private static Form2? _instance;
        public static Form2 Instance => _instance ?? throw new InvalidOperationException("Form2未初始化");
        #endregion

        #region 触发重载

        /// <summary>
        ///     按键触发，名称指定操作
        ///     如直接写在方法内则在按键触发前生效
        ///     例如可以切假腿放技能
        ///     if (e.KeyValue == (int)Keys.A && (int)e.ModifierKeys == (int)Keys.Alt) && (int)e.ModifierKeys ==
        ///     (int)Keys.Control)
        ///     ctrl + alt + A 捕获
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Hook_KeyDown(object sender, KeyEventArgs e)
        {
            #region 使用界面 隐藏

            switch (e.KeyCode)
            {
                case Keys.End:
                    WindowState = WindowState == FormWindowState.Normal
                        ? FormWindowState.Minimized
                        : FormWindowState.Normal;
                    break;
                default:
                    break;
            }

            #endregion

#if DOTA2
            await Games.Dota2.Main.根据当前英雄增强(tb_name.Text.Trim(), e);
#endif
#if LOL
            Games.LOL.MainClass.根据当前英雄增强(tb_name.Text.Trim(), e);
#endif
#if HF2
            Games.HF2.MainClass.根据当前英雄增强(tb_name.Text.Trim(), e);
#endif
        }

        #endregion

        #region 界面控件事件

        #region 更改名字取消功能

        private void Tb_name_TextChanged(object sender, EventArgs e)
        {
#if DOTA2
            Games.Dota2.Main.取消所有功能();
#endif
        }

        #endregion

        #endregion

        #region 按键钩子

        ///// <summary>
        /////     用于捕获按键
        ///// </summary>
        //private readonly KeyboardHook _kHook = new();

        ///// <summary>
        /////     按键钩子，用于捕获按下的键
        ///// </summary>
        //private KeyEventHandler _myKeyEventHandeler; //按键钩子

        ///// <summary>
        /////     用于捕捉按键
        ///// </summary>
        //private IKeyboardMouseEvents _mGlobalHook = Hook.GlobalEvents();

        /// <summary>
        ///     用于捕捉按键 现在使用
        /// </summary>
        private readonly HookUserActivity _hookUser = new();

        #endregion

        #region 获取当前时间毫秒

        private static long 获取当前时间毫秒()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        #endregion

        #region 页面元素更改更改

        private void Tb_攻速_TextChanged(object sender, EventArgs e)
        {
        }

        private void tb_状态抗性_TextChanged(object sender, EventArgs e)
        {
        }

        private void tb_阵营_TextChanged(object sender, EventArgs e)
        {
        }

        #region 按钮点击事件

        private async void Button1_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region 测试显示颜色

        /// <summary>
        ///     显示颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tb_delay_TextChanged(object sender, EventArgs e)
        {
            if (tb_name.Text.Trim() != "测试")
            {
                return;
            }

            try
            {
                using PooledList<string> list = [.. tb_等待延迟.Text.Split(',')];
                pictureBox1.BackColor = Color.FromArgb(255, int.Parse(list[0], CultureInfo.InvariantCulture), int.Parse(list[1], CultureInfo.InvariantCulture), int.Parse(list[2], CultureInfo.InvariantCulture));
            }
            catch
            {
                // ignored
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

            _instance = this;

            StartListen();
        }

        /// <summary>
        ///     页面关闭运行，释放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopListen();
            _instance = null;
        }

        /// <summary>
        ///     开始监听和初始化模拟
        /// </summary>
        private void StartListen()
        {
            /// 设置线程池数量，最小要大于CPU核心数，
            /// 最大不要太大就完事了，一般自动就行，手动反而影响性能
            //ThreadPool.SetMinThreads(12, 18);
            //ThreadPool.SetMaxThreads(48, 36);

            // 设置程序为HIGH程序，REALTIME循环延时
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            //_myKeyEventHandeler = Hook_KeyDown;
            //_kHook.KeyDownEvent += _myKeyEventHandeler; // 绑定对应处理函数
            //_kHook.Start(); // 安装键盘钩子
            //_mGlobalHook.KeyDown += Hook_KeyDown;

            // 按键钩子全部捕获
            _hookUser.HookScope = HookUserActivity.HookScopeType.GlobalScope;
            _hookUser.ActivityForm = this;
            //hook.OnMouseActivity += Hook_OnMouseActivity;
            _hookUser.KeyDown += Hook_KeyDown;
            //hook.KeyPress += Hook_KeyPress;
            //hook.KeyUp += Hook_KeyUp; 
            _hookUser.Start(false, true);

            // 初始化Ocr
            PaddleOCR.初始化PaddleOcr();

            //// winIo 和 WinRing0 需要额外的操作
            //WinIO32.Initialize();
            //// 初始化键盘鼠标模拟，仅模仿系统函数
            //i += KeyboardMouseSimulateDriverAPI.Initialize((uint)SimulateWays.Event);

            // 设置窗体显示在最上层
            SetWindowPos(Handle, -1, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0010 | 0x0080);

            // 设置本窗体为活动窗体
            SetActiveWindow(Handle);
            SetForegroundWindow(Handle);

            // 设置窗体置顶
            TopMost = true;
            Width = 136;

            if (tb_name.Text.Trim() == "测试")
            {
                lb_阵营.Text = "模式例:q4";
                lb_状态抗性.Text = "超时时间";
                lb_攻速.Text = "位置12|13";
                lb_等待延迟.Text = "颜色";
                lb_攻速.Text = "攻速";
                tb_阵营.Text = "q41";
                tb_状态抗性.Text = "2000";
                tb_等待延迟.Text = "";
                tb_状态抗性.Text = "25";
            }
            else
            {
                lb_攻速.Text = "攻速";
                tb_状态抗性.Text = "25";
            }

            Location = new Point(338, 1060);

#if DOTA2
            Games.Dota2.Main.获取图片_2(); // 初始化获取截图 避免一开始的黑色
#endif
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

            // 取消钩子
            _hookUser.Stop();

            //// WinIO 基本没用
            //WinIO32.Shutdown();
            //// 注销按键模拟
            //KeyboardMouseSimulateDriverAPI.Uninitialize();

            // 释放PaddleOcr
            _ = PaddleOCR.释放PaddleOcr();
        }

        #endregion

        private void checkBox_showAll_CheckedChanged(object sender, EventArgs e)
        {

            // 338 987 只显示四行 1060 只显示1行 900 基本显示除了颜色框的全部
            Location = new Point(338, 987);

            if (checkBox_showAll.Checked == true)
            {
                // 850 基本显示完全
                Location = new Point(338, 850);
            }
            else
            {
                Location = new Point(338, 1060);
            }
        }
    }
}