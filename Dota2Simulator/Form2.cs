using Collections.Pooled;
using Dota2Simulator.GameAutomation.Ports;
using Dota2Simulator.Games;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.Vision.Ocr;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using static Dota2Simulator.Infrastructure.Native.Win32.SetWindowTop;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Point = System.Drawing.Point;

namespace Dota2Simulator
{
    internal partial class Form2 : Form
    {
#if DOTA2
        #region 组合根注入
        /// <summary>Phase 6 AppContainer 注入入口 — DOTA2 构建专属。LOL/HF2 走无参构造器。</summary>
        private readonly CompositionRoot.AppContainer? _app;
        #endregion
#endif

        /// <summary>
        /// Phase 12 Chunk 2: 三引擎统一字段 (DOTA2=GameSession / LOL=LolEngine / HF2=Hf2Engine).
        /// ctor 内 #if 分支装配; Hook_KeyDown / Tb_name_TextChanged 0 #if 单行调度.
        /// </summary>
        private IGameEngine? _engine;

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

            // Phase 12 Chunk 2: 三引擎统一入站 —— DOTA2/LOL/HF2 走同一 IGameEngine 接口, 0 #if 分支适配.
            await _engine!.HandleKeyAsync(tb_name.Text.Trim(), e).ConfigureAwait(true);
        }

        #endregion

        #region 界面控件事件

        #region 更改名字取消功能

        private void Tb_name_TextChanged(object sender, EventArgs e)
        {
            // Phase 12 Chunk 2: 三引擎统一 CancelAll —— DOTA2 走 GameSession.CancelAll (内调 _host.取消所有功能), LOL/HF2 stub no-op.
            _engine?.CancelAll();
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
        ///     页面初始化（无参 — WinForms 设计器需要，LOL/HF2 构建走此路径）
        /// </summary>
        public Form2()
        {
            // Phase 10A Chunk C R2 时序实测 - 与 [ModuleInit] 时间戳对照确认 SHA1 manifest 注册早于 Form2.ctor
            Console.WriteLine($"[Form2.ctor] {DateTime.Now.Ticks}");

            InitializeComponent();

#if LOL
            // Phase 19G-1: LolEngine 装配经 AdapterFactory 共享 SSOT (与 DOTA2 AppContainer 对称).
            _engine = new Games.LOL.LolEngine(CompositionRoot.AdapterFactory.CreateInput(), CompositionRoot.AdapterFactory.CreateVision(), CompositionRoot.AdapterFactory.CreateUi(this));
#endif

#if HF2
            // Phase 19G-1: Hf2Engine 装配经 AdapterFactory 共享 SSOT.
            _engine = new Games.HF2.Hf2Engine(CompositionRoot.AdapterFactory.CreateInput(), CompositionRoot.AdapterFactory.CreateVision(), CompositionRoot.AdapterFactory.CreateUi(this));
#endif

            StartListen();
        }

#if DOTA2
        /// <summary>
        ///     Phase 6 组合根注入构造器（DOTA2 构建专属）。
        ///     Program.cs 构造 AppContainer 后调本路径，把 GameSession 实例注入。
        /// </summary>
        public Form2(CompositionRoot.AppContainer container) : this()
        {
            _app = container ?? throw new ArgumentNullException(nameof(container));
            // D1: 此时 InitializeComponent 已跑过（无参 base ctor 内），tb_* 字段可用——
            // 把 this 注给 AppContainer，让 BindUi 构造 Form2UiInvoker + set Common.UiInvoker。
            _app.BindUi(this);
            // Phase 12 Chunk 2: BindUi 后 GameSession 已 new, _engine 装配 IGameEngine (GameSession 实现).
            _engine = _app.GameEngine;
            // Phase 11 P5: 原 StartListen 末调用迁此 (BindUi 后 HeroLoopHost 已 new), 初始化获取截图 避免一开始的黑色.
            _app.HeroLoopHost!.获取图片_2();
        }
#endif

        /// <summary>
        ///     页面关闭运行，释放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopListen();
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
            _ = SetWindowPos(Handle, -1, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0010 | 0x0080);

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

            // Phase 11 P5: 原 Common.HeroLoopHost!.获取图片_2() 调用迁到 Form2(AppContainer) ctor 内 BindUi 后
            // (StartListen 由 base ctor Form2() 调, 此时 _app 仍 null, HeroLoopHost 也未 new).
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

        #region 点击显示全部
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
        #endregion
    }
}