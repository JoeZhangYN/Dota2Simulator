﻿// Games/Dota2/Main.cs
#if DOTA2

using Collections.Pooled;
using Dota2Simulator.ImageProcessingSystem;
using Dota2Simulator.KeyboardMouse;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dota2Simulator.Games.Dota2
{
    internal class Main
    {
        #region 全局变量

        /// dota 2 适配7.36 1920 * 1080 动态肖像 法线贴图 地面视差 主界面高画质 计算器渲染器 纹理、特效、阴影 中 渲染 70% 高级水面效果
        /// 最主要就是主界面高画质,其他没事
        private const int 截图模式1X = 671;
        private const int 截图模式1Y = 727;
        private const int 截图模式1W = 760;
        private const int 截图模式1H = 346;
        private static Rectangle 截图模式1Reg = new(截图模式1X, 截图模式1Y, 截图模式1W, 截图模式1H);

        private const int 截图模式2X = 0;
        private const int 截图模式2Y = 0;
        private const int 截图模式2W = 1920;
        private const int 截图模式2H = 1080;
        private static Rectangle 截图模式2Reg = new(截图模式2X, 截图模式2Y, 截图模式2W, 截图模式2H);

        private const int 等待延迟 = 33;



        // 根据截图模式不同，用于判断技能 和 物品的具体x, y值随之变化 涉及到 全局假腿，技能位置，使用物品位置，判断神杖、魔晶 中立道具
        public static int 坐标偏移x;
        public static int 坐标偏移y;



        public static readonly Dictionary<Keys, Action> 按键匹配条件更新 = new()
        {
            { Keys.Z, () => _条件z = true },
            { Keys.X, () => _条件x = true },
            { Keys.C, () => _条件c = true },
            { Keys.V, () => _条件v = true },
            { Keys.B, () => _条件b = true },
            { Keys.Space, () => _条件space = true }
        };

        #region 循环用到

        /// <summary>
        ///     循环条件
        /// </summary>
        private static bool _总循环条件;

        // 创建全局截图缓冲区
        private static byte[] _initialData = new byte[1920 * 1080 * 4];

        /// <summary>
        ///     1080p 增益状态栏 
        ///     <para>12个buff最多 单buff36像素 间隔9像素</para> 962, 857, 526, 80
        ///     <para>升级框会改变buff位置</para>
        /// </summary>
        private static Rectangle buff状态技能栏 = new(962, 826, 526, 80);

        /// <summary>
        ///     1080p 增益状态栏 
        ///     <para>12个buff最多 单buff36像素 间隔9像素</para> 962, 857, 526, 80
        ///     <para>升级框会改变buff位置</para>
        /// </summary>
        private static Rectangle debuff状态技能栏 = new(435, 826, 526, 80);

        /// <summary>
        ///     1080p 命石范围 6技能最左738 4技能最右807 单个25*25大小
        /// </summary>
        private static Rectangle 命石区域 = new(738, 945, 70, 26);


        /// <summary>
        ///     用于记录缓存全局图像句柄
        /// </summary>
        private static readonly Dictionary<string, ImageHandle> _缓存图像句柄 = new();

        /// <summary>
        ///     全局命石选择
        /// </summary>
        private static int _命石选择;

        /// <summary>
        ///     获取图片委托
        /// </summary>
        /// <returns></returns>
        private delegate bool 截图();

        /// <summary>
        ///     获取图片委托
        /// </summary>
        private static 截图 _循环内获取图片;

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public delegate Task<bool> ConditionDelegateBitmap(ImageHandle 句柄);

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
        ///     条件9委托
        /// </summary>
        public static ConditionDelegateBitmap _条件根据图片委托9;

        /// <summary>
        ///     命石委托
        /// </summary>
        private static ConditionDelegateBitmap _命石根据图片委托;

        /// <summary>
        ///     条件9委托
        /// </summary>
        public static ConditionDelegateBitmap _条件根据图片委托z;

        /// <summary>
        ///     条件9委托
        /// </summary>
        public static ConditionDelegateBitmap _条件根据图片委托x;

        /// <summary>
        ///     条件9委托
        /// </summary>
        public static ConditionDelegateBitmap _条件根据图片委托c;

        /// <summary>
        ///     条件9委托
        /// </summary>
        public static ConditionDelegateBitmap _条件根据图片委托v;

        /// <summary>
        ///     条件9委托
        /// </summary>
        public static ConditionDelegateBitmap _条件根据图片委托b;

        /// <summary>
        ///     条件9委托
        /// </summary>
        public static ConditionDelegateBitmap _条件根据图片委托space;

        /// <summary>
        ///     中断条件布尔
        /// </summary>
        public static bool _中断条件;

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
        public static bool _条件9;

        /// <summary>
        ///     条件布尔
        /// </summary>
        public static bool _条件z;

        /// <summary>
        ///     条件布尔
        /// </summary>
        public static bool _条件x;

        /// <summary>
        ///     条件布尔
        /// </summary>
        public static bool _条件c;

        /// <summary>
        ///     条件布尔
        /// </summary>
        public static bool _条件v;

        /// <summary>
        ///     条件布尔
        /// </summary>
        public static bool _条件b;

        /// <summary>
        ///     条件布尔
        /// </summary>
        public static bool _条件space;

        /// <summary>
        ///     攻击前摇
        /// </summary>
        private static double _基础攻击前摇 = 0.5;

        /// <summary>
        ///     攻击前摇
        /// </summary>
        private static double _基础攻击间隔 = 1.7;

        /// <summary>
        ///     攻击速度
        /// </summary>
        private static double _攻击速度 = 100;

        /// <summary>
        ///     当前出手时间
        /// </summary>
        private static long _实际出手时间;


        private static long _停止走A;

        private static bool _开启走A;

        /// <summary>
        ///     当前出手时间 650ms 为火女的攻击点（最长），30ms 为服务器延迟
        /// </summary>
        private static long _实际攻击前摇 = 650 + 30;

        /// <summary>
        ///     当前出手时间
        /// </summary>
        private static long _实际攻击间隔 = 1700;

        /// <summary>
        ///     能量转移
        /// </summary>
        // private static int 能量转移被动计数;

        /// <summary>
        ///     状态抗性
        /// </summary>
        private static double _状态抗性倍数;

        #endregion

        /// <summary>
        ///     仅用于快速吸引仇恨
        /// </summary>
        private static int _阵营_int; 

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
        ///     全局时间
        /// </summary>
        // private static long _全局时间f;

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

        private static readonly Lock _全局模式e_lock = new();

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

        private static int 获取全局步骤()
        {
            return _全局步骤;
        }

        private static void 设置全局步骤(int value)
        {
            _全局步骤 = value;
        }

        private static int 获取全局步骤q()
        {
            return _全局步骤q;
        }

        private static void 设置全局步骤q(int value)
        {
            _全局步骤q = value;
        }

        private static int 获取全局步骤w()
        {
            return _全局步骤w;
        }

        private static void 设置全局步骤w(int value)
        {
            _全局步骤w = value;
        }

        private static int 获取全局步骤e()
        {
            return _全局步骤e;
        }

        private static void 设置全局步骤e(int value)
        {
            _全局步骤e = value;
        }

        private static int 获取全局步骤r()
        {
            return _全局步骤r;
        }

        private static void 设置全局步骤r(int value)
        {
            _全局步骤r = value;
        }

        private static int 获取全局步骤d()
        {
            return _全局步骤d;
        }

        private static void 设置全局步骤d(int value)
        {
            _全局步骤d = value;
        }

        private static int 获取全局步骤f()
        {
            return _全局步骤f;
        }

        private static void 设置全局步骤f(int value)
        {
            _全局步骤f = value;
        }

        #endregion

        #region 根据名称修改对应按键
        public static async Task 根据当前英雄增强(string name, KeyEventArgs e)
        {
            switch (name)
            {
                #region 力量

                #region 大牛

                case "大牛":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 回音践踏去后摇;
                            _条件根据图片委托2 ??= 灵体游魂去后摇;
                            _条件根据图片委托3 ??= 裂地沟壑去后摇;
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                            Item._切假腿配置.修改配置(Keys.E, false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                // 用于回收时按W
                                SimKeyBoard.KeyPress(Keys.A);
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 发条

                case "发条":
                    {
                        if (!_总循环条件)
                        {
                            //_条件根据图片委托1 ??= 回音践踏去后摇;
                            //_条件根据图片委托2 ??= 灵体游魂去后摇;
                            //_条件根据图片委托3 ??= 裂地沟壑去后摇;
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                            //Item._切假腿配置.修改配置(Keys.E, false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                // 用于回收时按W
                                SimKeyBoard.KeyPress(Keys.A);
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 尸王

                case "尸王":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 腐朽去后摇;
                            _条件根据图片委托2 ??= 噬魂去后摇;
                            _条件根据图片委托3 ??= 墓碑去后摇;
                            _条件根据图片委托4 ??= 血肉傀儡去后摇;
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                        }

                        break;
                    }

                # endregion

                #region 伐木机

                case "伐木机":
                    {
                        if (!_总循环条件)
                        {
                            _命石根据图片委托 ??= 伐木机获取命石;
                            _条件根据图片委托1 ??= 死亡旋风去后摇;
                            _条件根据图片委托2 ??= 伐木聚链去后摇;
                            _条件根据图片委托3 ??= 锯齿轮旋去后摇;
                            _条件根据图片委托4 ??= 喷火装置去后摇;
                            _条件根据图片委托5 ??= 锯齿飞轮去后摇;
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.D:
                                if (_命石选择 == 2)
                                {
                                    _条件3 = true;
                                }

                                break;
                            case Keys.F:
                                _条件4 = true;
                                break;
                            case Keys.R:
                                _条件5 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 全能

                case "全能":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 洗礼去后摇;
                            _条件根据图片委托2 ??= 驱逐去后摇;
                            _条件根据图片委托3 ??= 守护天使去后摇;
                            _总循环条件 = true;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 军团

                case "军团":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 压倒性优势去后摇;
                            _条件根据图片委托2 ??= 强攻去后摇;
                            _条件根据图片委托3 ??= 决斗;
                            _条件根据图片委托4 ??= 决斗去后摇;
                            _总循环条件 = true;
                            _全局步骤 = -1;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.F:
                                if (_全局步骤 == -1)
                                {
                                    _全局步骤 = 0;
                                    _条件3 = true;
                                }

                                break;
                            case Keys.D2:
                                _全局模式 = 1 - _全局模式;
                                TTS.TTS.Speak(_全局模式 == 1 ? "跳刀决斗" : "直接决斗");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 骷髅王

                case "骷髅王":
                    {
                        if (!_总循环条件)
                        {
                            _命石根据图片委托 ??= 骷髅王获取命石;
                            _条件根据图片委托1 ??= 冥火爆击去后摇;
                            _条件根据图片委托2 ??= 白骨守卫去后摇;
                            _总循环条件 = true;
                            Item._切假腿配置.修改配置(Keys.W, false);
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                if (_命石选择 == 1)
                                {
                                    Item._切假腿配置.修改配置(Keys.W, true);
                                    _条件2 = true;
                                }

                                break;
                        }

                        break;
                    }

                #endregion

                #region 人马

                case "人马":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 马蹄践踏接平A;
                            _条件根据图片委托2 ??= 双刃剑去后摇;
                            _总循环条件 = true;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
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

                #region 哈斯卡

                case "哈斯卡":
                    if (!_总循环条件)
                    {
                        _条件根据图片委托1 ??= 心炎去后摇;
                        _条件根据图片委托2 ??= 牺牲去后摇;
                        _总循环条件 = true;
                        Item._切假腿配置.修改配置(Keys.E, false);
                        await 状态初始化().ConfigureAwait(false);
                    }

                    await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                #endregion

                #region 小狗

                case "小狗":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 狂暴去后摇;
                            _条件根据图片委托2 ??= 撕裂伤口去后摇;
                            _总循环条件 = true;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
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

                #region 土猫

                case "土猫":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 巨石冲击去后摇;
                            _条件根据图片委托2 ??= 地磁之握去后摇;
                            _条件根据图片委托3 ??= 磁化去后摇;
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 孽主

                case "孽主":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 火焰风暴去后摇;
                            _条件根据图片委托2 ??= 怨念深渊去后摇;
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
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

                #region 小小

                case "小小":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 山崩去后摇;
                            _条件根据图片委托2 ??= 投掷去后摇;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            Item._切假腿配置.修改配置(Keys.R, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
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

                #region 海民

                case "海民":
                    {
                        if (!_总循环条件)
                        {
                            _命石根据图片委托 ??= 海民获取命石;
                            _条件根据图片委托1 ??= 冰片去后摇;
                            _条件根据图片委托2 ??= 摔角行家去后摇;
                            _条件根据图片委托3 ??= 海象神拳接雪球;
                            _条件根据图片委托4 ??= 酒友去后摇;
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                switch (_命石选择)
                                {
                                    case 1:
                                        _条件2 = true;
                                        break;
                                    case 2:
                                        _条件4 = true;
                                        break;
                                }

                                break;
                            case Keys.F:
                                if (_命石选择 == 1)
                                {
                                    Skill.DOTA2释放CD就绪技能(Keys.E, GlobalScreenCapture.GetCurrentHandle());
                                }

                                _条件3 = true;
                                break;
                            case Keys.D2:
                                _指定地点d = Control.MousePosition;
                                TTS.TTS.Speak("确定指定地点");
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
                            _条件根据图片委托1 ??= 钩子去僵直;
                            _条件根据图片委托2 ??= 肢解检测状态;
                            _条件根据图片委托3 ??= 快速接肢解;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.R:
                                _条件2 = true;
                                break;
                            case Keys.D2:
                                _全局模式q = 1 - _全局模式q;
                                TTS.TTS.Speak(_全局模式q == 1 ? "勾接咬" : "勾接平A");
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
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒);
                                _条件1 = true;
                                break;
                            case Keys.W:
                                Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒);
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件4 = true;
                                break;
                            case Keys.R:
                                Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒);
                                _条件3 = true;
                                break;
                            case Keys.D4:
                                _全局模式q = 1 - _全局模式q;
                                TTS.TTS.Speak(_全局模式q == 1 ? "吼接刃甲" : "吼不接刃甲");
                                break;
                            case Keys.D3:
                                快速触发激怒();
                                break;
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
                            _条件根据图片委托2 ??= 踩去后摇;
                            _条件根据图片委托3 ??= 雾霭去后摇;
                            _条件根据图片委托4 ??= 跳刀接踩;
                            // Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.E:
                                _条件4 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 斯温

                case "斯温":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 风暴之拳去后摇;
                            _条件根据图片委托2 ??= 战吼去后摇;
                            _条件根据图片委托3 ??= 神之力量去后摇;
                            await 状态初始化().ConfigureAwait(false);
                            Item._切假腿配置.修改配置(Keys.W, false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 船长

                case "船长":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 洪流接x回;
                            _条件根据图片委托2 ??= x释放后相关逻辑;
                            _条件根据图片委托3 ??= x2次释放后;
                            _条件根据图片委托4 ??= 立即释放洪流;
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                if (_全局步骤e == 1)
                                {
                                    return;
                                }
                                _条件2 = true;
                                break;
                            case Keys.D2:
                                设置全局步骤r(1);
                                SimKeyBoard.KeyPress(Keys.E);
                                break;
                        }

                        break;
                    }

                #endregion

                #region 夜魔

                case "夜魔":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 虚空去后摇;
                            _条件根据图片委托2 ??= 伤残恐惧去后摇;
                            _条件根据图片委托3 ??= 黑暗飞升去后摇;
                            _条件根据图片委托4 ??= 暗夜猎影去后摇;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.E, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                if (Item._是否魔晶)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 树精

                case "树精":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 自然卷握去后摇;
                            _条件根据图片委托2 ??= 寄生种子去后摇;
                            _条件根据图片委托3 ??= 活体护甲去后摇;
                            _条件根据图片委托4 ??= 丛林之眼去后摇;
                            _条件根据图片委托5 ??= 疯狂生长去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.D:
                                if (Item._是否神杖)
                                {
                                    Item._切假腿配置.修改配置(Keys.D, true);
                                    _条件4 = true;
                                }

                                break;
                            case Keys.R:
                                _条件5 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 混沌

                case "混沌":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 混乱之箭去后摇;
                            _条件根据图片委托2 ??= 实相裂隙去后摇;
                            _条件根据图片委托3 ??= 混沌之军去后摇;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                Item.根据图片使用物品(Dota2_Pictrue.物品.紫苑);
                                Item.根据图片使用物品(Dota2_Pictrue.物品.血棘);
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.D2:
                                _全局模式q = 1 - _全局模式q;
                                TTS.TTS.Speak(_全局模式q == 1 ? "混乱之箭接拉" : "混乱之箭接A");
                                break;
                            case Keys.D3:
                                await 切臂章().ConfigureAwait(true);
                                break;
                        }

                        break;
                    }

                #endregion

                #region 马尔斯

                case "马尔斯":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 战神迅矛去后摇;
                            _条件根据图片委托2 ??= 神之谴击去后摇;
                            _条件根据图片委托3 ??= 热血竞技场去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.D2:
                                _全局模式q = 1 - _全局模式q;
                                TTS.TTS.Speak(_全局模式q == 1 ? "矛接大招" : "矛不接大招");
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
                            _条件根据图片委托2 ??= 上界重锤去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                _条件2 = true;
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
                            //_条件根据图片委托1 ??= 鼻涕去后摇;
                            //// _条件根据图片委托2 ??= 针刺循环; 已优化不需要
                            //_条件根据图片委托3 ??= 毛团去后摇;
                            //_条件根据图片委托4 ??= 钢毛后背去后摇;
                            //_条件根据图片委托5 ??= 扫射切回假腿;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.D, true);
                                }

                                if (Item._是否神杖)
                                {
                                    Item._切假腿配置.修改配置(Keys.E, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.D:
                                if (Item._是否魔晶)
                                {
                                    _条件3 = true;
                                }

                                break;
                            case Keys.E:
                                if (Item._是否神杖)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.W:
                                _条件5 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 龙骑

                case "龙骑":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 喷火去后摇;
                            _条件根据图片委托2 ??= 神龙摆尾去后摇;
                            _条件根据图片委托3 ??= 变龙去后摇;
                            _条件根据图片委托4 ??= 火球去后摇;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            _基础攻击前摇 = 0.5;
                            _基础攻击间隔 = 1.6;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.D, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.D:
                                if (Item._是否魔晶)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.D2:
                                _全局模式w = 1 - _全局模式w;
                                TTS.TTS.Speak("W接" + (_全局模式w == 1 ? "火球" : "喷火"));
                                break;
                            case Keys.D3:
                                _全局模式d = 1 - _全局模式d;
                                TTS.TTS.Speak("火球" + (_全局模式d == 1 ? "接" : "不接") + "喷火");
                                break;
                        }

                        break;
                    }

                #endregion

                #endregion

                #region 敏捷

                #region 小骷髅

                case "小骷髅":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 扫射去后摇;
                            _条件根据图片委托2 ??= 焦油去后摇;
                            _条件根据图片委托3 ??= 死亡契约去后摇;
                            _条件根据图片委托4 ??= 骨隐步去后摇;
                            _条件根据图片委托5 ??= 炽烈火雨去后摇;
                            _条件根据图片委托6 ??= 骷髅之军去后摇;
                            _基础攻击前摇 = 0.4;
                            _基础攻击间隔 = 1.7;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.D, true, "敏捷");
                                }

                                if (Item._是否神杖)
                                {
                                    Item._切假腿配置.修改配置(Keys.F, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.D:
                                if (Item._是否魔晶)
                                {
                                    _条件5 = true;
                                }

                                break;
                            case Keys.F:
                                if (Item._是否神杖)
                                {
                                    _条件6 = true;
                                }

                                break;
                            case Keys.D2:
                                _全局模式q = 1 - _全局模式q;
                                TTS.TTS.Speak(_全局模式q == 1 ? "无脑接道具" : "手动道具");
                                break;
                            case Keys.D3:
                                if (Item._是否魔晶)
                                {
                                    _全局模式f = 1 - _全局模式f;
                                    TTS.TTS.Speak(_全局模式f == 1 ? "炽烈火雨隐身" : "炽烈火雨不隐身");
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
                            _条件根据图片委托3 ??= 冰川去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                _全局模式e = ColorExtensions.ColorAEqualColorB(获取指定位置颜色(738, 957, GlobalScreenCapture.GetCurrentHandle()),
                                    Color.FromArgb(246, 178, 60), 0) || ColorExtensions.ColorAEqualColorB(
                                    获取指定位置颜色(722, 957, GlobalScreenCapture.GetCurrentHandle()),
                                    Color.FromArgb(246, 178, 60), 0)
                                    ? 1
                                    : 0;
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.F, true);
                                }

                                break;
                            case Keys.D:
                                if (Item._条件开启切假腿)
                                {
                                    _全局模式 = 1 - _全局模式;
                                    switch (_全局模式)
                                    {
                                        case 0:
                                            await Item.技能释放前切假腿("智力").ConfigureAwait(true);
                                            TTS.TTS.Speak("开启冰箭");
                                            break;
                                        default:
                                            Item.要求保持假腿();
                                            TTS.TTS.Speak("关闭冰箭");
                                            break;
                                    }
                                }

                                break;
                            case Keys.W:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.F:
                                if (Item._是否魔晶)
                                {
                                    _条件3 = true;
                                }

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
                            _条件根据图片委托1 ??= 旋风飞斧远去后摇;
                            _条件根据图片委托2 ??= 旋风飞斧近去后摇;
                            _条件根据图片委托3 ??= 战斗专注去后摇;
                            Item._切假腿配置.修改配置(Keys.Q, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
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
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否神杖)
                                {
                                    Item._切假腿配置.修改配置(Keys.D, true);
                                }
                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                // 触发激怒机制，2.3秒内不吸引仇恨
                                SimKeyBoard.KeyPress(Keys.A);
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.D:
                                if (Item._是否神杖)
                                {
                                    _条件4 = true;
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
                            _条件根据图片委托2 ??= 神行百变选择幻象;
                            Item._切假腿配置.修改配置(Keys.W, true, "力量");
                            Item._切假腿配置.修改配置(Keys.E, false);
                            Item._切假腿配置.修改配置(Keys.R, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                if (!Skill.DOTA2判断状态技能是否启动(Keys.E, GlobalScreenCapture.GetCurrentHandle()))
                                {
                                    SimKeyBoard.KeyPress(Keys.E);
                                }

                                _条件1 = true;
                                break;
                            case Keys.W:
                                if (!Skill.DOTA2判断状态技能是否启动(Keys.E, GlobalScreenCapture.GetCurrentHandle()))
                                {
                                    SimKeyBoard.KeyPress(Keys.E);
                                }

                                _条件2 = true;
                                break;
                            case Keys.R:
                                if (!Skill.DOTA2判断状态技能是否启动(Keys.E, GlobalScreenCapture.GetCurrentHandle()))
                                {
                                    SimKeyBoard.KeyPress(Keys.E);
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
                            _条件根据图片委托3 ??= 空降去后摇;
                            _条件根据图片委托4 ??= 折射去后摇;
                            Item._切假腿配置.修改配置(Keys.W, false);
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.E, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.R:
                                _条件2 = true;
                                break;
                            case Keys.D:
                                _条件3 = true;
                                break;
                            case Keys.E:
                                _条件4 = true;
                                break;
                            case Keys.D2:
                                _全局模式f = 1 - _全局模式f;
                                TTS.TTS.Speak(_全局模式f == 1 ? "如影随形分身" : "关闭随形分身");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 影魔

                case "影魔":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= z炮去后摇;
                            _条件根据图片委托2 ??= x炮去后摇;
                            _条件根据图片委托3 ??= c炮去后摇;
                            _条件根据图片委托4 ??= 灵魂盛宴去后摇;
                            //_条件根据图片委托5 ??= 如影随形去后摇;
                            Item._切假腿配置.修改配置(Keys.F, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.D:
                                _条件4 = true;
                                break;
                            case Keys.R:
                                _条件5 = true;
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
                            _条件根据图片委托6 ??= 断魂敏捷;
                            Item._切假腿配置.修改配置(Keys.W, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否神杖)
                                {
                                    Item._切假腿配置.修改配置(Keys.F, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.D:
                                if (Item._是否魔晶)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.F:
                                if (Item._是否神杖)
                                {
                                    _条件5 = true;
                                }

                                break;
                            case Keys.R:
                                _条件6 = true;
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
                            _条件根据图片委托1 ??= 闪烁敏捷;
                            _条件根据图片委托2 ??= 法术反制敏捷;
                            _条件根据图片委托3 ??= 法力虚空取消后摇;
                            _条件根据图片委托4 ??= 友军法术反制敏捷;
                            Item._切假腿配置.修改配置(Keys.Q, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.D, true);
                                }

                                break;
                            case Keys.W:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.D:
                                if (Item._是否魔晶)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.D2:
                                _全局模式w = 1;
                                TTS.TTS.Speak("闪烁分身晕锤一次");
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
                            _条件根据图片委托1 ??= 黑暗契约去后摇;
                            _条件根据图片委托2 ??= 跳水去后摇;
                            _条件根据图片委托3 ??= 深海护罩去后摇;
                            _条件根据图片委托4 ??= 暗影之舞去后摇;
                            // 能量转移被动计数 = 0;
                            _基础攻击间隔 = 1.7;
                            _基础攻击前摇 = 0.5;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.D, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.D:
                                if (Item._是否魔晶)
                                {
                                    _条件3 = true;
                                }

                                break;
                            case Keys.D2:
                                // 径直移动键位
                                SimKeyBoard.KeyDown(Keys.L);
                                Common.Delay(等待延迟);
                                SimKeyBoard.MouseRightClick();
                                Common.Delay(等待延迟);
                                SimKeyBoard.KeyUp(Keys.L);
                                // 基本上180°310  90°170 75°135 转身定点，配合A杖效果极佳
                                Common.Delay(110);
                                SimKeyBoard.KeyPress(Keys.W);
                                break;
                        }

                        break;
                    }

                #endregion

                #region 小松鼠

                case "小松鼠":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 爆栗出击去后摇;
                            _条件根据图片委托2 ??= 野地奇袭去后摇;
                            _条件根据图片委托3 ??= 一箭穿心;
                            _条件根据图片委托4 ??= 猎手旋标去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.F, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.F:
                                if (Item._是否魔晶)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.D2:
                                _全局模式w = 1 - _全局模式w;
                                TTS.TTS.Speak(_全局模式w == 0 ? "种树接平A" : "种树接捆");
                                break;
                            case Keys.D3:
                                _全局模式e = 1 - _全局模式e;
                                TTS.TTS.Speak(_全局模式e == 0 ? "捆接平A" : "捆接种树");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 火猫

                case "火猫":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 无影拳后续处理;
                            _条件根据图片委托2 ??= 炎阳索去后摇;
                            _条件根据图片委托3 ??= 烈火罩去后摇;
                            _条件根据图片委托4 ??= 激活残焰去后摇;
                            Item._切假腿配置.修改配置(Keys.D, true);
                            Item._切假腿配置.修改配置(Keys.R, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                _条件1 = true;
                                await Task.Run(() =>
                                {
                                    Common.Delay(330);
                                    Item.要求保持假腿();
                                }).ConfigureAwait(false);
                                break;
                            case Keys.Q:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.D:
                                _条件4 = true;
                                break;
                            case Keys.D2:
                                _全局模式w = 1 - _全局模式w;
                                TTS.TTS.Speak(_全局模式w == 0 ? "不接捆" : "接捆");
                                break;
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
                            _条件根据图片委托1 ??= 震撼大地去后摇;
                            _条件根据图片委托2 ??= 超强力量去后摇;
                            _条件根据图片委托3 ??= 跳拍;
                            _条件根据图片委托4 ??= 狂怒去后摇;
                            await 状态初始化().ConfigureAwait(false);
                            Item._切假腿配置.修改配置(Keys.E, false);
                            Item._切假腿配置.修改配置(Keys.R, false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
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
                            Item._切假腿配置.修改配置(Keys.W, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        Item._切假腿配置.修改配置(Keys.D, Item._是否魔晶);
                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.D:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 飞机

                case "飞机":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            //_条件根据图片委托1 ??= 火箭弹幕敏捷;
                            //_条件根据图片委托2 ??= 追踪导弹敏捷;
                            //_条件根据图片委托3 ??= 高射火炮敏捷;
                            //_条件根据图片委托4 ??= 召唤飞弹敏捷;
                            _条件根据图片委托5 ??= 循环火箭弹幕;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {                            
                            //case Keys.Q:
                            //    _条件1 = true;
                            //    break;
                            //case Keys.W:
                            //    _条件2 = true;
                            //    break;
                            //case Keys.E:
                            //    _条件3 = true;
                            //    break;
                            //case Keys.R:
                            //    _条件4 = true;
                            //    break;
                            case Keys.D3:
                                _条件5 = !_条件5;
                                TTS.TTS.Speak(_条件5 ? "循环弹幕" : "关闭弹幕");
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
                            _条件根据图片委托2 ??= 罗网剑阵去后摇;
                            _条件根据图片委托3 ??= 石化凝视去后摇;
                            Item._切假腿配置.修改配置(Keys.Q, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
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
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 血魔

                case "血魔":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 血祭去后摇;
                            _条件根据图片委托2 ??= 割裂去后摇;
                            _条件根据图片委托3 ??= 血怒去后摇;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        if (e.KeyValue == (int)Keys.Q && (int)e.Modifiers == (int)Keys.Alt)
                        {
                            _条件3 = true;
                        }

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                _条件1 = true;
                                break;
                            case Keys.R:
                                _条件2 = true;
                                break;
                            case Keys.Q:
                                _条件3 = true;
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
                            Item._切假腿配置.修改配置(Keys.W, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.R:
                                _条件2 = true;
                                break;
                            case Keys.D2:
                                _条件3 = !_条件3;
                                TTS.TTS.Speak(_条件3 ? "循环标记" : "不循环标记");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 电棍

                case "电棍":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 等离子场去后摇;
                            _条件根据图片委托2 ??= 静电连接去后摇;
                            _条件根据图片委托3 ??= 风暴之眼去后摇;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 露娜

                case "露娜":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 月光后敏捷平a;
                            _条件根据图片委托2 ??= 月刃后敏捷平a;
                            _条件根据图片委托3 ??= 月蚀后敏捷平a;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 大圣

                case "大圣":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 棒击大地去后摇;
                            _条件根据图片委托2 ??= 乾坤之跃敏捷;
                            _条件根据图片委托3 ??= 猴子猴孙敏捷;
                            _条件根据图片委托4 ??= 大圣无限跳跃;
                            Item._切假腿配置.修改配置(Keys.Q, false);
                            Item._切假腿配置.修改配置(Keys.W, false);
                            Skill.重复按键执行间隔阈值 = 100;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.D3:
                                _条件4 = !_条件4;
                                TTS.TTS.Speak(_条件4 ? "开启无限跳跃" : "关闭无限跳跃");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 剃刀

                case "剃刀":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            //_条件根据图片委托1 ??= 棒击大地去后摇;
                            //_条件根据图片委托2 ??= 乾坤之跃敏捷;
                            //_条件根据图片委托3 ??= 猴子猴孙敏捷;
                            //_条件根据图片委托4 ??= 大圣无限跳跃;
                            //Item._切假腿配置.修改配置(Keys.Q, false);
                            //Item._切假腿配置.修改配置(Keys.W, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #endregion

                #region 智力

                #region 修补匠

                case "修补匠":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 光法

                case "光法":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 减少300毫秒蓄力;
                            _条件根据图片委托2 ??= 炎阳之缚去后摇;
                            _条件根据图片委托3 ??= 查克拉魔法去后摇;
                            _条件根据图片委托4 ??= 循环查克拉;
                            _条件根据图片委托5 ??= 致盲之光去后摇;
                            _条件根据图片委托6 ??= 灵光去后摇接炎阳;
                            Skill.重复按键执行间隔阈值 = 100;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        if (e.KeyValue == (int)Keys.E && (int)e.Modifiers == (int)Keys.Alt)
                        {
                            _条件4 = true;
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
                                _条件3 = true;
                                break;
                            case Keys.D:
                                _条件5 = true;
                                break;
                            case Keys.F:
                                _条件6 = true;
                                break;
                            case Keys.D2:
                                _条件4 = !_条件4;
                                TTS.TTS.Speak(_条件4 ? "开启循环查克拉" : "关闭循环查克拉");
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
                            _条件根据图片委托3 ??= 奥数鹰隼去后摇;
                            _条件根据图片委托4 ??= 上古封印去后摇;
                            _条件根据图片委托5 ??= 神秘之耀去后摇;
                            _条件根据图片委托6 ??= 震荡光弹去后摇;
                            Skill.重复按键执行间隔阈值 = 100;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件3 = true;
                                break;
                            case Keys.W:
                                _条件6 = true;
                                break;
                            case Keys.E:
                                _条件4 = true;
                                break;
                            case Keys.R:
                                _条件5 = true;
                                break;
                            case Keys.D2:
                                _条件1 = !_条件1;
                                TTS.TTS.Speak(_条件1 ? "循环鹰隼" : "不循环鹰隼");
                                break;
                            case Keys.D3:
                                _全局步骤 = 1;
                                _条件2 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 墨客

                case "墨客":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 命运之笔去后摇;
                            _条件根据图片委托2 ??= 幻影之拥去后摇;
                            _条件根据图片委托3 ??= 墨泳去后摇;
                            _条件根据图片委托4 ??= 缚魂去后摇;
                            _条件根据图片委托5 ??= 暗绘去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        if (e.KeyValue == (int)Keys.E && (int)e.Modifiers == (int)Keys.Alt)
                        {
                            _条件3 = true;
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
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.D:
                                _条件5 = true;
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
                            _条件根据图片委托3 ??= 神圣一跳去后摇;
                            _条件根据图片委托4 ??= 雷神之怒去后摇;
                            _条件根据图片委托5 ??= 雷云去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.D:
                                _条件5 = true;
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
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 巫妖

                case "巫妖":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 寒霜爆发去后摇;
                            _条件根据图片委托2 ??= 冰霜魔盾去后摇;
                            _条件根据图片委托3 ??= 阴邪凝视去后摇;
                            _条件根据图片委托4 ??= 连环霜冻去后摇;
                            _条件根据图片委托5 ??= 寒冰尖柱去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        if (e.KeyValue == (int)Keys.W && (int)e.Modifiers == (int)Keys.Alt)
                        {
                            _条件2 = true;
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
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.D:
                                _条件5 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 帕克

                case "帕克":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 幻象法球去后摇;
                            _条件根据图片委托2 ??= 新月之痕去后摇;
                            _条件根据图片委托4 ??= 梦境缠绕去后摇;
                            _条件根据图片委托5 ??= 灵动之翼定位;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        if (e.KeyValue == (int)Keys.W && (int)e.Modifiers == (int)Keys.Control)
                        {
                            _条件2 = true;
                        }

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.D:
                                _条件5 = true;
                                break;
                            case Keys.D2:
                                _全局模式d = 1 - _全局模式d;
                                TTS.TTS.Speak(_全局模式d == 1 ? "传" : "不传");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 骨法

                case "骨法":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 幽冥轰爆去后摇;
                            _条件根据图片委托2 ??= 衰老去后摇;
                            _条件根据图片委托3 ??= 幽冥守卫去后摇;
                            _条件根据图片委托4 ??= 生命吸取去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                Item.根据图片使用物品(Dota2_Pictrue.物品.希瓦);
                                Item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
                                _条件4 = true;
                                break;
                            case Keys.D2:
                                _全局模式r = 1 - _全局模式r;
                                TTS.TTS.Speak(_全局模式r == 1 ? "吸取接衰老" : "吸取不接衰老");
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
                            _条件根据图片委托5 ??= 静态风暴动能立场风雷之击;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.D2:
                                设置全局步骤w(0);
                                _条件5 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 黑鸟

                case "黑鸟":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 神智之蚀去后摇;
                            _条件根据图片委托2 ??= 关接跳;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        switch (e.KeyCode)
                        {
                            case Keys.D:
                                SimKeyBoard.KeyPress(Keys.W);
                                _条件1 = true;
                                break;
                            case Keys.W:
                                Item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件2 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 谜团

                case "谜团":
                    {
                        if (e.KeyCode == Keys.F)
                        {
                            await Task.Run(刷新接凋零黑洞).ConfigureAwait(true);
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
                            await 状态初始化().ConfigureAwait(false);
                        }


                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
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
                            await 状态初始化().ConfigureAwait(false);
                        }


                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                await Task.Run(残影接平A).ConfigureAwait(true);
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
                        }

                        break;
                    }

                #endregion

                #region 卡尔

                case "卡尔":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 三冰对线;
                            _条件根据图片委托2 ??= 三雷对线;
                            _条件根据图片委托3 ??= 三雷幽灵;
                            _条件根据图片委托4 ??= 极冷吹风陨星锤雷暴;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.D1:
                                _条件1 = true;
                                break;
                            case Keys.D2:
                                _条件2 = true;
                                break;
                            case Keys.D3:
                                _条件3 = true;
                                break;
                            case Keys.D4:
                                _条件4 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 拉席克

                case "拉席克":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 撕裂大地去后摇;
                            _条件根据图片委托2 ??= 恶魔敕令去后摇;
                            _条件根据图片委托3 ??= 闪电风暴去后摇;
                            _条件根据图片委托4 ??= 脉冲新星去后摇;
                            _条件根据图片委托5 ??= 虚无主义去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.D:
                                _条件5 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 术士

                case "术士":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 致命链接去后摇;
                            _条件根据图片委托2 ??= 暗言术去后摇;
                            _条件根据图片委托3 ??= 混乱之祭去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }


                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                Item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                Common.初始化全局时间(ref _全局时间e);
                                break;
                            case Keys.R:
                                Common.初始化全局时间(ref _全局时间r);
                                _条件3 = true;
                                break;
                        }

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
                            await 状态初始化().ConfigureAwait(false);
                        }


                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.物品.中立_祭礼长袍, GlobalScreenCapture.GetCurrentHandle(), Item.获取中立TP范围(Skill._技能数量)))
                                {
                                    _状态抗性倍数 *= 1.1;
                                }

                                if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.物品.中立_永恒遗物, GlobalScreenCapture.GetCurrentHandle(), Item.获取中立TP范围(Skill._技能数量)))
                                {
                                    _状态抗性倍数 *= 1.2;
                                }

                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件5 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.D1:
                                switch (_全局模式w)
                                {
                                    case 0:
                                        _全局模式w = 1;
                                        TTS.TTS.Speak("羊拉");
                                        break;
                                    case 1:
                                        _全局模式w = 2;
                                        TTS.TTS.Speak("羊电");
                                        break;
                                    case 2:
                                        _全局模式w = 3;
                                        TTS.TTS.Speak("羊电拉");
                                        break;
                                    case 3:
                                        _全局模式w = 4;
                                        TTS.TTS.Speak("羊电大拉");
                                        break;
                                    case 4:
                                        _全局模式w = 0;
                                        TTS.TTS.Speak("羊接平A");
                                        break;
                                }

                                break;
                            case Keys.D2:
                                _条件4 = true;
                                break;
                            case Keys.D3:
                                _全局模式q = 1 - _全局模式q;
                                TTS.TTS.Speak(_全局模式q == 0 ? "羊" : "电羊");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 小仙女

                case "小仙女":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托5 ??= 无限暗影之境;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F:
                                if (Item._是否魔晶)
                                {
                                    _条件4 = true;
                                }
                                break;
                            case Keys.D3:
                                _条件5 = !_条件5;
                                TTS.TTS.Speak(_条件5 ? "续暗影" : "不续暗影");
                                break;
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
                            await 状态初始化().ConfigureAwait(false);
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
                                Item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
                                _条件3 = true;
                                break;
                            case Keys.D2:
                                _全局模式e = 1 - _全局模式e;
                                TTS.TTS.Speak(_全局模式e == 0 ? "起飞后不接3连炸弹" : "起飞后接3连炸弹");
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
                            _条件根据图片委托5 ??= 天命之雨去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                _条件1 = true;
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

                #region 莱恩

                case "莱恩":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 莱恩羊接技能;
                            _条件根据图片委托2 ??= 死亡一指去后摇;
                            _条件根据图片委托3 ??= 推推破林肯秒羊;
                            _条件根据图片委托4 ??= 羊刺刷新秒人;

                            await 状态初始化().ConfigureAwait(false);
                        }


                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                _条件1 = true;
                                break;
                            case Keys.R:
                                await 大招前纷争(GlobalScreenCapture.GetCurrentHandle()).ConfigureAwait(true);
                                _条件2 = true;
                                break;
                            case Keys.E:
                                break;
                            case Keys.D2:
                                _条件3 = true;
                                break;
                            case Keys.S:
                                _中断条件 = true;
                                break;
                            case Keys.D3:
                                _条件4 = true;
                                break;
                            case Keys.D4 when !_条件5:
                                _条件5 = true;
                                TTS.TTS.Speak("开启刷新秒人");
                                break;
                            case Keys.D4:
                                _条件5 = false;
                                TTS.TTS.Speak("关闭刷新秒人");
                                break;
                            case Keys.D5 when !_条件6:
                                _条件6 = true;
                                TTS.TTS.Speak("开启羊接吸");
                                break;
                            case Keys.D5:
                                _条件6 = false;
                                TTS.TTS.Speak("开启羊接A");
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
                            _条件根据图片委托2 ??= 全领域沉默去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }


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

                #region 戴泽

                case "戴泽":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 剧毒之触去后摇;
                            _条件根据图片委托2 ??= 薄葬去后摇;
                            _条件根据图片委托3 ??= 暗影波去后摇;
                            _条件根据图片委托4 ??= 邪能去后摇;
                            _基础攻击前摇 = 0.3;
                            _基础攻击间隔 = 1.7;
                            await 状态初始化().ConfigureAwait(false);
                        }


                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                Common.初始化全局时间(ref _全局时间q);
                                _条件1 = true;
                                break;
                            case Keys.W:
                                Common.初始化全局时间(ref _全局时间w);
                                _条件2 = true;
                                break;
                            case Keys.E:
                                Common.初始化全局时间(ref _全局时间e);
                                _条件3 = true;
                                break;
                            case Keys.R:
                                Common.初始化全局时间(ref _全局时间r);
                                _条件4 = true;
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
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.D3:
                                _条件4 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 奶绿

                case "奶绿":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 弹无虚发去后摇;
                            _条件根据图片委托2 ??= 唤魂去后摇;
                            _条件根据图片委托3 ??= 越界去后摇;
                            _条件根据图片委托4 ??= 临别一枪去后摇;
                            _条件根据图片委托5 ??= 祭台去后摇;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
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
                            await 状态初始化().ConfigureAwait(false);
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
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.S:
                                _中断条件 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 蓝胖

                case "蓝胖":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 火焰轰爆去后摇;
                            _条件根据图片委托2 ??= 引燃去后摇;
                            _条件根据图片委托3 ??= 嗜血术去后摇;
                            _条件根据图片委托4 ??= 烈火护盾去后摇;
                            _条件根据图片委托5 ??= 未精通火焰轰爆去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }


                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.F:
                                _条件4 = true;
                                break;
                            case Keys.D:
                                _条件5 = true;
                                break;
                            case Keys.D2:
                                _全局模式w = 1 - _全局模式w;
                                TTS.TTS.Speak(_全局模式w == 0 ? "引燃接轰爆" : "引燃不接轰爆");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 祸乱之源

                case "祸乱之源":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 虚弱去后摇;
                            _条件根据图片委托2 ??= 噬脑去后摇;
                            _条件根据图片委托3 ??= 噩梦接平A锤;
                            await 状态初始化().ConfigureAwait(false);
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
                                Color 技能点颜色 = Color.FromArgb(203, 183, 124);
                                _全局时间 = 0;
                                if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(971, 1008, GlobalScreenCapture.GetCurrentHandle()), 技能点颜色, 0))
                                {
                                    _全局时间 = 7000;
                                }
                                else if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(964, 1008, GlobalScreenCapture.GetCurrentHandle()), 技能点颜色, 0))
                                {
                                    _全局时间 = 6000;
                                }
                                else if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(947, 1008, GlobalScreenCapture.GetCurrentHandle()), 技能点颜色, 0))
                                {
                                    _全局时间 = 5000;
                                }
                                else if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(935, 1008, GlobalScreenCapture.GetCurrentHandle()), 技能点颜色, 0))
                                {
                                    _全局时间 = 4000;
                                }

                                _条件3 = true;
                                break;
                            case Keys.D2:
                                _全局模式e = 1 - _全局模式e;
                                TTS.TTS.Speak(_全局模式e == 0 ? "睡不接陨星锤" : "睡接陨星锤");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 瘟疫法师

                case "瘟疫法师":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 死亡脉冲去后摇;
                            _条件根据图片委托2 ??= 幽魂护罩去后摇;
                            _条件根据图片委托3 ??= 死神镰刀去后摇;
                            _条件根据图片委托5 ??= 循环死亡脉冲;
                            Skill.重复按键执行间隔阈值 = 100;
                            Item._切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (Item._是否魔晶)
                                {
                                    Item._切假腿配置.修改配置(Keys.F, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.F:
                                if (Item._是否魔晶)
                                {
                                    _条件4 = true;
                                }
                                break;
                            case Keys.D3:
                                _条件5 = !_条件5;
                                TTS.TTS.Speak(_条件5 ? "循环脉冲" : "终止循环");
                                break;
                        }

                        break;
                    }

                #endregion

                #endregion

                #region 全才

                #region 剧毒

                case "剧毒":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 瘴气去后摇;
                            _条件根据图片委托2 ??= 蛇棒去后摇;
                            _条件根据图片委托3 ??= 恶性瘟疫去后摇;
                            _条件根据图片委托4 ??= 循环蛇棒;
                            Skill.重复按键执行间隔阈值 = 100;
                            await 状态初始化().ConfigureAwait(false);
                            Item._切假腿配置.修改配置(Keys.W, false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.E:
                                _条件2 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
                                break;
                            case Keys.D3:
                                {
                                    _条件4 = !_条件4;
                                    TTS.TTS.Speak(_条件4 ? "循环蛇棒" : "终止循环");
                                    break;
                                }
                            case Keys.S:
                                _中断条件 = true;
                                _条件1 = false;
                                _条件2 = false;
                                _条件3 = false;
                                _条件4 = false;
                                _条件5 = false;
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
                            _条件根据图片委托1 = 震荡波去后摇;
                            _条件根据图片委托2 = 授予力量去后摇;
                            _条件根据图片委托3 = 巨角冲撞去后摇;
                            _条件根据图片委托4 = 两级反转去后摇;
                            _条件根据图片委托5 = 长角抛物去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                            case Keys.D:
                                _条件5 = true;
                                break;
                            case Keys.F:
                                await Task.Run(跳拱指定地点).ConfigureAwait(false);
                                break;
                            case Keys.D2:
                                await Task.Run(指定地点).ConfigureAwait(false);
                                break;
                        }

                        break;
                    }

                #endregion

                #region 狼人

                case "狼人":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 招狼去后摇;
                            _条件根据图片委托2 ??= 嚎叫去后摇;
                            _条件根据图片委托3 ??= 撕咬去后摇;
                            _条件根据图片委托4 ??= 变狼去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.D:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region 紫猫

                case "紫猫":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            //_条件根据图片委托1 ??= 共鸣脉冲去后摇;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.D:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件4 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #region VS

                case "VS":
                    {
                        if (!_总循环条件)
                        {
                            _条件根据图片委托1 ??= 魔法箭去后摇;
                            _条件根据图片委托2 ??= 恐怖波动去后摇;
                            _条件根据图片委托3 ??= 移形换位去后摇;
                            _总循环条件 = true;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                _条件3 = true;
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
                            _条件根据图片委托2 ??= 幽魂检测;
                            _条件根据图片委托3 ??= 循环续过载;
                            Skill.重复按键执行间隔阈值 = 150;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                {
                                    if (Item._是否神杖)
                                    {
                                        break;
                                    }

                                    _条件2 = true;
                                    break;
                                }
                            case Keys.D3:
                                {
                                    _条件3 = !_条件3;
                                    TTS.TTS.Speak(_条件3 ? "开启续过载" : "关闭续过载");
                                    break;
                                }
                        }

                        break;
                    }

                #endregion

                #region 马西

                case "马西":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托2 ??= 幽魂检测;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                {
                                    if (Item._是否神杖)
                                    {
                                        break;
                                    }

                                    _条件2 = true;
                                    break;
                                }
                        }

                        break;
                    }

                #endregion

                #region 小强

                case "小强":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            //_条件根据图片委托1 ??= 穿刺去后摇;
                            //_条件根据图片委托2 ??= 神智爆裂去后摇;
                            //_条件根据图片委托3 ??= 尖刺外壳去后摇;
                            //_条件根据图片委托4 ??= 复仇接穿刺;
                            _条件根据图片委托5 ??= 循环接爆裂;
                            Skill.重复按键执行间隔阈值 = 150;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            //case Keys.Q:
                            //    _条件1 = true;
                            //    break;
                            //case Keys.W:
                            //    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃));
                            //    Common.Delay(33 * (Item.根据图片使用物品(Dota2_Pictrue.物品.红杖) +
                            //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2) +
                            //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3) +
                            //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4) +
                            //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)));
                            //    _条件2 = true;
                            //    break;
                            //case Keys.E:
                            //    _条件3 = true;
                            //    break;
                            //case Keys.R:
                            //    设置全局步骤r(0);
                            //    // _条件4 = true;
                            //    break;
                            case Keys.D3:
                                _条件5 = !_条件5;
                                TTS.TTS.Speak(_条件5 ? "循环爆裂" : "终止循环");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 沙王

                case "沙王":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            Skill.重复按键执行间隔阈值 = 150;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await Item.根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            //case Keys.Q:
                            //    _条件1 = true;
                            //    break;
                            //case Keys.W:
                            //    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃));
                            //    Common.Delay(33 * (Item.根据图片使用物品(Dota2_Pictrue.物品.红杖) +
                            //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2) +
                            //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3) +
                            //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4) +
                            //                Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)));
                            //    _条件2 = true;
                            //    break;
                            //case Keys.E:
                            //    _条件3 = true;
                            //    break;
                            //case Keys.R:
                            //    设置全局步骤r(0);
                            //    // _条件4 = true;
                            //    break;
                        }

                        break;
                    }

                #endregion

                #endregion

                #region 测试

                case "测试":
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.D1:
                                _ = Task.Run(测试其他功能).ConfigureAwait(true);
                                break;
                            case Keys.D2:
                                _ = Task.Run(() => { Skill.捕捉颜色().Start(); }).ConfigureAwait(false);
                                break;
                            case Keys.D3:
                                _ = Task.Run(() => { Skill.捕捉颜色().Start(); }).ConfigureAwait(false);
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
                                Common.Main_Form.Invoke(() =>
                                {
                                    text = Common.Main_Form.tb_阵营.Text.ToLower(CultureInfo.CurrentCulture); // 转换为小写，确保匹配时忽略大小写
                                });

                                foreach (KeyValuePair<char, Keys> kvp in keyMapping)
                                {
                                    if (text.Contains(kvp.Key))
                                    {
                                        SimKeyBoard.KeyPress(kvp.Value);
                                        break; // 如果找到匹配项，退出循环
                                    }
                                }

                                break;
                            case Keys.D4:
                                await Task.Run(() => Skill.测试方法(802, 946)).ConfigureAwait(false);
                                break;
                        }

                        break;
                    }

                    #endregion
            }
        }
        #endregion

        #region 获取指定位置颜色

        public static Task<Color> 获取指定位置颜色(int x, int y)
        {
            _ = 获取图片_2();

            return Task.FromResult(GlobalScreenCapture.GetColor(x, y));
        }

        public static Color 获取指定位置颜色(int x, int y, in ImageHandle 句柄)
        {
            return ImageManager.GetColor(in 句柄, x - 坐标偏移x, y - 坐标偏移y);
        }

        #endregion

        #region Dota2具体实现

        #region 力量

        #region 大牛

        private static async Task<bool> 回音践踏去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1, 判断成功后延时: 1300).ConfigureAwait(true);
        }

        private static async Task<bool> 灵体游魂去后摇(ImageHandle 句柄)
        {
            return await Skill.释放技能后替换图标技能后续(Keys.W, 获取全局步骤w, 设置全局步骤w).ConfigureAwait(true);
        }

        private static async Task<bool> 裂地沟壑去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 尸王

        private static async Task<bool> 腐朽去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 噬魂去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 墓碑去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 血肉傀儡去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 伐木机

        private static async Task<bool> 伐木机获取命石(ImageHandle 句柄)
        {
            if (_命石选择 == 0)
            {
                if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.命石.伐木机_碎木击, GlobalScreenCapture.GetCurrentHandle(), 命石区域))
                {
                    _命石选择 = 1;
                }
                else if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.命石.伐木机_锯齿轮旋, GlobalScreenCapture.GetCurrentHandle(), 命石区域))
                {
                    _命石选择 = 2;
                    Item._切假腿配置.修改配置(Keys.D, true);
                }
            }

            _命石根据图片委托 = null;
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 死亡旋风去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 伐木聚链去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 锯齿轮旋去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 锯齿飞轮去后摇(ImageHandle 句柄)
        {
            return await Skill.释放技能后替换图标技能后续(Keys.R, 获取全局步骤r, 设置全局步骤r).ConfigureAwait(true);
        }

        private static async Task<bool> 喷火装置去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.F, 0).ConfigureAwait(true);
        }

        #endregion

        #region 全能

        private static async Task<bool> 洗礼去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 驱逐去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 守护天使去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 军团

        private static async Task<bool> 决斗(ImageHandle 句柄)
        {
            return await Task.Run(async () =>
            {
                int 步骤 = 获取全局步骤();

                switch (步骤)
                {
                    case < 1:
                        {
                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.臂章));
                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒));
                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.相位鞋));

                            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
                            {
                                SimKeyBoard.KeyPressAlt(Keys.W);
                                return await Task.FromResult(true).ConfigureAwait(true);
                            }

                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.刃甲));

                            设置全局步骤(1);
                            return await Task.FromResult(true).ConfigureAwait(true);
                        }
                    case < 2 when 步骤 == 1:
                        {
                            Common.Delay(33 *
                                  (
                                      Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀)
                                      + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀)
                                      + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀)
                                      + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀)
                                  ));

                            设置全局步骤(2);

                            return await Task.FromResult(true).ConfigureAwait(true);
                        }
                    case < 3:
                        {
                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.紫苑));
                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.血棘));
                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.否决));
                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.散失));
                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.散魂));
                            Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.深渊之刃));

                            设置全局步骤(3);

                            return await Task.FromResult(true).ConfigureAwait(true);
                        }

                    case < 4:
                        {
                            // 触发激怒，让周围的小兵都攻击你
                            SimKeyBoard.KeyPress(Keys.A);

                            if (Skill.DOTA2释放CD就绪技能(Keys.R, in 句柄))
                            {
                                Common.Delay(60);
                                return await Task.FromResult(true).ConfigureAwait(true);
                            }

                            设置全局步骤(-1);
                            return await Task.FromResult(false).ConfigureAwait(true);
                        }
                }

                return await Task.FromResult(false).ConfigureAwait(true);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 压倒性优势去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 强攻去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 决斗去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1, 要接的按键: Keys.Q).ConfigureAwait(true);
        }

        #endregion

        #region 骷髅王

        private static async Task<bool> 骷髅王获取命石(ImageHandle 句柄)
        {
            if (_命石选择 == 0)
            {
                _命石选择 = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.命石.骷髅王_白骨守卫, GlobalScreenCapture.GetCurrentHandle(), 命石区域) ? 1 : 2;
            }

            _命石根据图片委托 = null;
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 冥火爆击去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 白骨守卫去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        #endregion

        #region 人马

        private static async Task<bool> 马蹄践踏接平A(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 双刃剑去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        #endregion

        #region 哈斯卡

        private static async Task<bool> 心炎去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 牺牲去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.R, () =>
            {
                SimKeyBoard.MouseRightClick();

                if (Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄))
                {
                    return;
                }

                SimKeyBoard.KeyPress(Keys.A);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 小狗

        private static async Task<bool> 狂暴去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 撕裂伤口去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        #endregion

        #region 土猫

        private static async Task<bool> 巨石冲击去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 地磁之握去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 磁化去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 孽主

        private static async Task<bool> 火焰风暴去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 怨念深渊去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.W, () =>
            {
                SimKeyBoard.MouseRightClick();
                if (Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄))
                {
                    return;
                }

                SimKeyBoard.KeyPress(Keys.A);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 小小

        private static async Task<bool> 山崩去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 投掷去后摇(ImageHandle 句柄)
        {
            return await Task.Run(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    Common.Delay(20);
                    Skill.通用技能后续动作();
                }

                return false;
            }).ConfigureAwait(true);
        }

        #endregion

        #region 海民

        private static async Task<bool> 海民获取命石(ImageHandle 句柄)
        {
            if (_命石选择 == 0)
            {
                _命石选择 = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.命石.海民_酒友, GlobalScreenCapture.GetCurrentHandle(), 命石区域) ? 2 : 1;
            }

            _命石根据图片委托 = null;
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 冰片去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 摔角行家去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 酒友去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
        }

        // 基本完美了。。。
        private static async Task<bool> 海象神拳接雪球(ImageHandle 句柄)
        {
            return await Skill.法球技能进入CD后续(Keys.R, () =>
            {
                Point p = Control.MousePosition;
                for (int i = 0; i < 2; i++)
                {
                    Common.Delay(33);
                    SimKeyBoard.MouseMove(p.X, p.Y - 60 * i);
                    SimKeyBoard.KeyPress(Keys.W);
                }

                _ = Task.Run(() =>
                {
                    Common.Delay(100);
                    SimKeyBoard.MouseMove(p);
                    Common.Delay(850);
                    if (_中断条件)
                    {
                        return;
                    }

                    SimKeyBoard.KeyDown(Keys.D);
                    Common.Delay(100);
                    SimKeyBoard.MouseMove(_指定地点d);
                    Common.Delay(100);
                    SimKeyBoard.KeyUp(Keys.D);
                    Common.Delay(600);
                    SimKeyBoard.KeyPress(Keys.W);
                });
            }).ConfigureAwait(true);
        }

        #endregion

        #region 屠夫

        // 钩子出手后，就可以用W，但其他技能无法释放且物品会被锁闭，可以通过判断锁闭的状态
        private static async Task<bool> 钩子去僵直(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.Q, () =>
            {
                if (!Skill.DOTA2判断状态技能是否启动(Keys.W, in 句柄))
                {
                    SimKeyBoard.KeyPress(Keys.W);
                }

                SimKeyBoard.KeyPress(Keys.A);
                if (_全局模式q == 1)
                {
                    _条件3 = true;
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 肢解检测状态(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.R, () =>
            {
                if (!Skill.DOTA2判断状态技能是否启动(Keys.W, in 句柄))
                {
                    SimKeyBoard.KeyPress(Keys.W);
                }

                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.纷争);
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.希瓦);
            }).ConfigureAwait(true);
        }

        // 技能颜色虽然变了，但是CD状态的颜色没变，
        // 钩可以直接接咬，但期间物品还是锁闭的
        // 解决。
        private static async Task<bool> 快速接肢解(ImageHandle 句柄)
        {
            return await Item.所有物品可用后续(句柄, () =>
            {
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.纷争));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.希瓦));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃));
                Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.否决));

                Common.Delay(33 *
                      (Item.根据图片使用物品(Dota2_Pictrue.物品.红杖) +
                       Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2) +
                       Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3) +
                       Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4) +
                       Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)));

                SimKeyBoard.KeyPress(Keys.R);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 斧王

        private static async Task<bool> 吼去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.Q, () =>
            {
                if (_全局模式q == 1)
                {
                    _ = Item.根据图片使用物品(Dota2_Pictrue.物品.刃甲);
                }
                // 触发激怒
                SimKeyBoard.KeyPress(Keys.A);
                SimKeyBoard.KeyPress(Keys.W);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 战斗饥渴去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 淘汰之刃去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 跳吼(ImageHandle 句柄)
        {
            if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀) == 1)
            {
                Common.Delay(等待延迟);
            }

            _ = Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄);

            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 大鱼人

        private static async Task<bool> 跳刀接踩(ImageHandle 句柄)
        {
            if (Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒) == 1)
            {
                Common.Delay(等待延迟);
            }

            if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀) == 1
               )
            {
                Common.Delay(等待延迟);
            }

            _ = Skill.DOTA2释放CD就绪技能(Keys.W, in 句柄);

            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 守卫冲刺去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 踩去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1, 要接的按键: Item._是否魔晶 ? Keys.A : Keys.R).ConfigureAwait(true);
        }

        private static async Task<bool> 雾霭去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 斯温

        private static async Task<bool> 风暴之拳去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 战吼去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 神之力量去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 船长

        private static async Task<bool> 洪流接x回(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.Q, () =>
            {
                _条件4 = false;
                Common.初始化全局时间(ref _全局时间q);

                // 如果E已经释放
                if (!_中断条件 && 获取全局步骤e() == 1)
                {
                    // 1600 延迟 返回200施法时间
                    Common.Delay(1350, _全局时间q);
                    _条件4 = false;
                    SimKeyBoard.KeyPress(Keys.E);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> x释放后相关逻辑(ImageHandle 句柄)
        {
            // 释放x后放船，x的时间3秒，船0.3秒，3.1秒延迟，控制还是得靠水起来
            return await Skill.主动技能释放后续(Keys.E, () =>
            {
                int 步骤e = 获取全局步骤e();

                if (步骤e == 1) return;

                Common.初始化全局时间(ref _全局时间e);

                if (获取全局步骤r() == 1)
                {
                    SimKeyBoard.KeyPress(Keys.R);
                    设置全局步骤r(0);
                }

                lock (_全局模式e_lock)
                {
                    设置全局步骤e(1);
                    _条件3 = true;
                }

                int 等待时间 = (int)Math.Floor(3000 * _状态抗性倍数) - 1670;
                Common.Delay(等待时间, _全局时间e);
                _条件4 = true;
            }).ConfigureAwait(true);
        }

        private static async Task<bool> x2次释放后(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.E, () =>
            {
                lock (_全局模式e_lock)
                {
                    // 玲珑心，释放完后至少再等6秒，等2秒基本完事
                    // 因为释放q后，会再释放一次E
                    // 等待说明E已经释放过一次，还在有效范围内
                    Common.Delay(2000);
                    设置全局步骤e(0);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 立即释放洪流(ImageHandle 句柄)
        {
            return await Skill.主动技能已就绪后续(Keys.Q, () => { SimKeyBoard.KeyPress(Keys.Q); }).ConfigureAwait(true);
        }

        #endregion

        #region 夜魔

        private static async Task<bool> 虚空去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 伤残恐惧去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 暗夜猎影去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 黑暗飞升去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 树精

        private static async Task<bool> 自然卷握去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 寄生种子去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 活体护甲去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 丛林之眼去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 疯狂生长去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 混沌

        private static async Task<bool> 混乱之箭去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1, 要接的按键: _全局模式q == 1 ? Keys.W : Keys.A).ConfigureAwait(true);
        }

        private static async Task<bool> 实相裂隙去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 11).ConfigureAwait(true);
        }

        private static async Task<bool> 混沌之军去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 马尔斯

        private static async Task<bool> 战神迅矛去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.Q, () =>
            {
                if (_全局模式q == 1)
                {
                    SimKeyBoard.KeyPress(Keys.R);
                }
                else
                {
                    Skill.通用技能后续动作();
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 神之谴击去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 热血竞技场去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.R, () =>
            {
                if (Skill.判断技能状态(Keys.E, 句柄, Skill.技能类型.状态))
                {
                    SimKeyBoard.KeyPress(Keys.E);
                }

                Skill.通用技能后续动作();
            }).ConfigureAwait(true);
        }

        #endregion

        #region 破晓晨星

        private static async Task<bool> 上界重锤去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        #endregion

        #region 钢背

        private static async Task<bool> 鼻涕去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 扫射切回假腿(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 10).ConfigureAwait(true);
        }

        //private static async Task<bool> 针刺循环(ImageHandle 句柄)
        //{
        //    _ = await 主动技能已就绪后续(Keys.W, () =>
        //    {
        //        SimKeyBoard.KeyPress(Keys.W);
        //        Common.Delay(等待延迟);
        //    }).ConfigureAwait(true);
        //    return await Task.FromResult(_循环条件1).ConfigureAwait(true);
        //}

        private static async Task<bool> 毛团去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 钢毛后背去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0, false).ConfigureAwait(true);
        }

        #endregion

        #region 龙骑

        private static async Task<bool> 喷火去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 神龙摆尾去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.W, () =>
            {
                SimKeyBoard.KeyPress(Keys.A);
                _ = _全局模式w == 1 && Item._是否魔晶 ? Skill.DOTA2释放CD就绪技能(Keys.D, in 句柄) : Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄);

                Item.要求保持假腿();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 变龙去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 火球去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 0, 要接的按键: _全局模式d == 1 && Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄) ? Keys.Q : Keys.A)
                .ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region 敏捷

        #region 小骷髅

        private static async Task<bool> 扫射去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.Q, () =>
            {
                if (_全局模式q == 1)
                {
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.散失));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.散魂));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.否决));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.紫苑));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.血棘));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.羊刀));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.阿托斯之棍));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.缚灵锁));
                }

                Skill.通用技能后续动作();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 焦油去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.W, () =>
            {
                _ = Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄);
                Skill.通用技能后续动作();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 死亡契约去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.E, SimKeyBoard.MouseRightClick).ConfigureAwait(true);
        }

        private static async Task<bool> 骨隐步去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.R, SimKeyBoard.MouseRightClick).ConfigureAwait(true);
        }

        private static async Task<bool> 炽烈火雨去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.F, () =>
            {
                // 持续时间施法，其实啥也不用管？
                if (_全局模式f == 1)
                {
                    Common.Delay(0);
                    SimKeyBoard.KeyPress(Keys.R);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 骷髅之军去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.F, 0).ConfigureAwait(true);
        }

        #endregion

        #region 小黑

        private static async Task<bool> 狂风去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.W, () =>
            {
                Skill.通用技能后续动作();

                if (_全局模式 == 1)
                {
                    Item._需要切假腿 = false;
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 数箭齐发去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.E, () =>
            {
                Common.Delay(_全局模式e == 1 ? 2600 : 1300);
                SimKeyBoard.KeyPress(Keys.S);
                Skill.通用技能后续动作();

                if (_全局模式 == 1)
                {
                    Item._需要切假腿 = false;
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 冰川去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.F, () =>
            {
                Skill.通用技能后续动作();

                if (_全局模式 == 1)
                {
                    Item._需要切假腿 = false;
                }
            }).ConfigureAwait(true);
        }

        #endregion

        #region 巨魔

        private static async Task<bool> 旋风飞斧远去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 旋风飞斧近去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 战斗专注去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 幻刺

        private static async Task<bool> 窒息短匕敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 幻影突袭敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 魅影无形敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0, false).ConfigureAwait(true);
        }

        private static async Task<bool> 刀阵旋风敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
        }

        #endregion

        #region 猴子

        private static async Task<bool> 灵魂之矛敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 神行百变选择幻象(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.W, () =>
            {
                Common.Delay(1000);
                SimKeyBoard.KeyPress(Keys.D1);
                Common.Delay(等待延迟);
                SimKeyBoard.MouseRightClick();
                SimKeyBoard.KeyPress(Keys.F1);
                Item.要求保持假腿();
            }).ConfigureAwait(true);
        }

        #endregion

        #region 幽鬼

        private static async Task<bool> 幽鬼之刃去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1, false).ConfigureAwait(true);
        }

        private static async Task<bool> 如影随形去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.R, () =>
            {
                if (_全局模式f == 1)
                {
                    SimKeyBoard.KeyPress(Keys.D);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 空降去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.D, () =>
            {
                if (_全局模式f == 1)
                {
                    if (Item.根据图片使用物品(Dota2_Pictrue.物品.幻影斧) == 1)
                    {
                        分身一齐攻击();
                    }

                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.否决));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.紫苑));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.血棘));
                }

                Item.要求保持假腿();

                SimKeyBoard.KeyPress(Keys.A);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 折射去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        #endregion

        #region 影魔
        private static async Task<bool> z炮去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1, false).ConfigureAwait(true);
        }

        private static async Task<bool> x炮去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1, false).ConfigureAwait(true);
        }

        private static async Task<bool> c炮去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 1, false).ConfigureAwait(true);
        }
        private static async Task<bool> 灵魂盛宴去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 1, false).ConfigureAwait(true);
        }

        //private static void 吹风摇大()
        //{
        //    var w_down = 0;

        //    SimKeyBoard.KeyPress(Keys.Space);

        //    while (w_down == 0)
        //        if (ImageFinder.FindImageBool(Resource_Picture.吹风CD, 1291, 991, 60, 45))
        //        {
        //            w_down = 1;
        //            SimKeyBoard.KeyPress(Keys.M);
        //            Common.Delay(830);
        //            SimKeyBoard.KeyPress(Keys.R);
        //        }
        //}

        #endregion

        #region TB

        private static async Task<bool> 倒影敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 幻惑敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 魔化敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 恶魔狂热去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 恐怖心潮敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.F, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 断魂敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 敌法

        private static async Task<bool> 闪烁敏捷(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.W, () =>
            {
                if (_全局模式w == 1)
                {
                    _ = Item.根据图片使用物品(Dota2_Pictrue.物品.幻影斧);
                    分身一齐攻击();
                    _ = Item.根据图片使用物品(Dota2_Pictrue.物品.深渊之刃);
                    _全局模式w = 0;
                }

                Skill.通用技能后续动作();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 法力虚空取消后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 法术反制敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 10).ConfigureAwait(true);
        }

        private static async Task<bool> 友军法术反制敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 10).ConfigureAwait(true);
        }

        #endregion

        #region 小鱼人

        private static async Task<bool> 黑暗契约去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 跳水去后摇(ImageHandle 句柄)
        {
            _ = Task.Run(() =>
            {
                Skill.通用技能后续动作(是否保持假腿: false);
                Item._需要切假腿 = false;
                Common.Delay(200);
                Item.要求保持假腿();
            });
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 深海护罩去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 暗影之舞去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 小松鼠

        private static async Task<bool> 爆栗出击去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.Q, () =>
            {
                if (_全局模式w == 1)
                {
                    SimKeyBoard.KeyPress(Keys.W);
                }
                else
                {
                    Skill.通用技能后续动作();
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 野地奇袭去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.W, () =>
            {
                if (_全局模式e == 1)
                {
                    SimKeyBoard.KeyPress(Keys.Q);
                }
                else
                {
                    Skill.通用技能后续动作();
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 一箭穿心(ImageHandle 句柄)
        {
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 猎手旋标去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.F, 1).ConfigureAwait(true);
        }

        #endregion

        #region 火猫

        private static async Task<bool> 无影拳后续处理(ImageHandle 句柄)
        {
            bool b = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.火猫_无影拳, in 句柄, buff状态技能栏);

            if (b)
            {
                if (_全局模式w == 1)
                {
                    SimKeyBoard.KeyPress(Keys.Q);
                }

                Item.要求保持假腿();

                SimKeyBoard.KeyPress(Keys.A);
            }

            return await Task.FromResult(!b).ConfigureAwait(true);
        }

        private static async Task<bool> 炎阳索去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 烈火罩去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 激活残焰去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
        }

        #endregion

        #region 拍拍

        private static async Task<bool> 超强力量去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 震撼大地去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 狂怒去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 跳拍(ImageHandle 句柄)
        {
            _ = Task.Run(() =>
            {
                if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_力量跳刀)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_敏捷跳刀) == 1)
                {
                    SimKeyBoard.KeyPress(Keys.A);

                    _ = Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄);
                }
            });

            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 火枪

        private static async Task<bool> 流霰弹去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 瞄准去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.E, () =>
            {
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.疯狂面具);

                Skill.通用技能后续动作();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 暗杀去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 震荡手雷去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
        }

        #endregion

        #region 飞机

        private static async Task<bool> 火箭弹幕敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 追踪导弹敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 高射火炮敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 召唤飞弹敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }
        
        private static async Task<bool> 循环火箭弹幕(ImageHandle 句柄)
        {
            if (Common.获取当前时间毫秒()-_全局时间q > 400)
            await Skill.主动技能已就绪后续(Keys.Q, () =>
            {
                SimKeyBoard.KeyPress(Keys.Q);
                _全局时间q = Common.获取当前时间毫秒();
            }).ConfigureAwait(true);

            return await Task.FromResult(_条件5);
        }

        #endregion

        #region 美杜莎

        private static async Task<bool> 秘术异蛇去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 罗网剑阵去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 石化凝视去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 虚空

        private static async Task<bool> 时间漫游敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 时间膨胀敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 时间结界敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 血魔

        private static async Task<bool> 血祭去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.W, () =>
            {
                SimKeyBoard.MouseRightClick();
                SimKeyBoard.KeyPress(Keys.A);

                Item.要求保持假腿();

                Common.Delay(2400);
                Point p = Control.MousePosition;
                SimKeyBoard.MouseMove(601, 988);
                if (Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄))
                {
                    SimKeyBoard.MouseMove(p);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 割裂去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 血怒去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        #endregion

        #region 赏金

        private static async Task<bool> 飞镖接平a(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 标记去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 循环标记(ImageHandle 句柄)
        {
            await Skill.技能通用判断(Keys.R, 2).ConfigureAwait(true);
            return await Task.FromResult(_条件3).ConfigureAwait(true);
        }

        #endregion

        #region 电棍

        private static async Task<bool> 等离子场去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 静电连接去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 风暴之眼去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 露娜

        private static async Task<bool> 月光后敏捷平a(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 月刃后敏捷平a(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 月蚀后敏捷平a(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 大圣

        private static async Task<bool> 棒击大地去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 乾坤之跃敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 猴子猴孙敏捷(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 大圣无限跳跃(ImageHandle 句柄)
        {
            await Skill.技能通用判断(Keys.W, 2).ConfigureAwait(true);
            return await Task.FromResult(_条件4).ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region 智力

        #region 修补匠

        private static void 检测敌方英雄自动导弹()
        {
            //Task t = new(() =>
            //{
            //    if (ImageFinder.FindImageBool(血量_敌人血量, 0, 0, 1920, 1080, 0.6))
            //    {
            //        SimKeyBoard.KeyPress( Keys.W);
            //        Common.Delay(40);
            //    }
            //});
            //t.Start();
            //await t;
        }

        private static void 推推接刷新()
        {
            //long time = Common.获取当前时间毫秒();
            //int x_down = 0;
            //while (x_down == 1)
            //{
            //    //if (ImageFinder.FindImageBool(Dota2_Pictrue.物品.推推BUFF, 400, 865, 1000, 60))
            //    //{
            //    //    SimKeyBoard.KeyPress( Keys.R);
            //    //    x_down = 1;
            //    //}
            //    if (Common.获取当前时间毫秒() - time > 500)
            //    {
            //        break;
            //    }
            //}
        }


        private static void 刷新完跳()
        {
            //int all_down = 0;
            //long time = Common.获取当前时间毫秒();
            //while (all_down == 1)
            //{
            //    //var r_down = 0;
            //    //if (ImageFinder.FindImageBool(修补匠_再填装施法, 773, 727, 75, 77, 0.7))
            //    //{
            //    //    if (_条件3)
            //    //        await 检测希瓦();
            //    //    while (r_down == 0)
            //    //        if (!ImageFinder.FindImageBool(修补匠_再填装施法, 773, 727, 75, 77, 0.7))
            //    //        {
            //    //            r_down = 1;
            //    //            all_down = 1;
            //    //            if (_条件1)
            //    //                await 检测敌方英雄自动导弹();
            //    //            if (_条件2)
            //    //            {
            //    //                Common.Delay(60);
            //    //                SimKeyBoard.KeyPress(Keys.Space);
            //    //            }
            //    //        }
            //    //}
            //    if (Common.获取当前时间毫秒() - time > 700)
            //    {
            //        break;
            //    }
            //}
        }

        #endregion

        #region 光法

        private static async Task<bool> 减少300毫秒蓄力(ImageHandle 句柄)
        {
            int 全局步骤 = 获取全局步骤q();

            switch (全局步骤)
            {
                case 1:
                    return await Skill.主动技能进入CD后续(Keys.Q, () =>
                    {
                        设置全局步骤q(0);
                        Item._切假腿配置.修改配置(Keys.Q, true);
                    }).ConfigureAwait(true);
                default:
                    设置全局步骤q(1);
                    if (ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.光法_大招, GlobalScreenCapture.GetCurrentHandle(), buff状态技能栏))
                    {
                        Item._切假腿配置.修改配置(Keys.Q, false);
                        SimKeyBoard.MouseRightClick();
                    }

                    _ = Task.Run(() =>
                    {
                        Common.Delay(2700);
                        SimKeyBoard.KeyPress(Keys.Q);
                    }).ConfigureAwait(false);

                    return await Task.FromResult(true).ConfigureAwait(true);
            }
        }

        private static async Task<bool> 炎阳之缚去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 查克拉魔法去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 致盲之光去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 灵光去后摇接炎阳(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.F, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 循环查克拉(ImageHandle 句柄)
        {
            await Skill.技能通用判断(Keys.E, 2).ConfigureAwait(true);
            return await Task.FromResult(_条件4).ConfigureAwait(true);
        }

        #endregion

        #region 天怒

        private static async Task<bool> 循环奥数鹰隼(ImageHandle 句柄)
        {
            await Skill.技能通用判断(Keys.Q, 2).ConfigureAwait(true);
            return await Task.FromResult(_条件1).ConfigureAwait(true);
        }

        private static async Task<bool> 天怒秒人连招(ImageHandle 句柄)
        {
            int 步骤 = 获取全局步骤();

            switch (步骤)
            {
                case < 2:
                    if (Skill.DOTA2释放CD就绪技能(Keys.W, in 句柄))
                    {
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }

                    if (Skill.DOTA2释放CD就绪技能(Keys.E, in 句柄))
                    {
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }

                    if (Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄))
                    {
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }

                    Common.Delay(0 * Item.根据图片使用物品(Dota2_Pictrue.物品.阿托斯之棍));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.缚灵锁));
                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃));

                    Common.Delay(33 * (
                        Item.根据图片使用物品(Dota2_Pictrue.物品.红杖)
                        + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2)
                        + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3)
                        + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4)
                        + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)));

                    Common.Delay(33 * Item.根据图片使用物品(Dota2_Pictrue.物品.羊刀));

                    设置全局步骤(2);

                    return await Task.FromResult(true).ConfigureAwait(true);
                case < 3:
                    {
                        if (Skill.DOTA2释放CD就绪技能(Keys.R, in 句柄))
                        {
                            return await Task.FromResult(true).ConfigureAwait(true);
                        }

                        _条件1 = true;
                        设置全局步骤(3);

                        return await Task.FromResult(false).ConfigureAwait(true);
                    }
            }

            return await Task.FromResult(true).ConfigureAwait(true);
        }

        private static async Task<bool> 奥数鹰隼去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 上古封印去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 神秘之耀去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 震荡光弹去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        #endregion

        #region 墨客

        private static async Task<bool> 命运之笔去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 幻影之拥去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 墨泳去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 缚魂去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 暗绘去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
        }

        #endregion

        #region 宙斯

        private static async Task<bool> 弧形闪电去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 雷击去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 神圣一跳去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 雷神之怒去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 雷云去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
        }

        #endregion

        #region 巫医

        private static async Task<bool> 麻痹药剂去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1, 要接的按键: Keys.E).ConfigureAwait(true);
        }

        private static async Task<bool> 巫蛊咒术去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.E, () =>
            {
                SimKeyBoard.KeyPress(Keys.A);
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.魂之灵龛);
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.影之灵龛);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 死亡守卫隐身(ImageHandle 句柄)
        {
            return await Skill.主动技能释放后续(Keys.R, () =>
            {
                _ = Item.根据图片自我使用物品(Dota2_Pictrue.物品.微光披风);
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.隐刀);
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.大隐刀);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 巫妖

        private static async Task<bool> 寒霜爆发去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, _全局步骤e > 0 ? 11 : 1).ConfigureAwait(true);
        }

        private static async Task<bool> 冰霜魔盾去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, _全局步骤e > 0 ? 11 : 1, false).ConfigureAwait(true);
        }

        private static async Task<bool> 阴邪凝视去后摇(ImageHandle 句柄)
        {
            if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
            {
                设置全局步骤e(0);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            return await Task.Run(async () =>
            {
                if (_全局步骤e == 0)
                {
                    设置全局步骤e(1);
                    return await Task.FromResult(true).ConfigureAwait(true);
                }
                else if (_全局步骤e == 1)
                {
                    _ = Task.Run(() =>
                    {
                        Common.Delay(200);
                        设置全局步骤e(2);
                    });
                    return await Task.FromResult(true).ConfigureAwait(true);
                }
                else
                {
                    if (!Skill.DOTA2判断是否持续施法(in 句柄))
                    {
                        设置全局步骤e(0);
                        SimKeyBoard.KeyPress(Keys.A);
                        _ = Item.根据图片使用物品(Dota2_Pictrue.物品.羊刀);
                        return await Task.FromResult(false).ConfigureAwait(true);
                    }
                    else
                    {
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }
                }
            }).ConfigureAwait(false);
        }

        private static async Task<bool> 连环霜冻去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, _全局步骤e > 0 ? 11 : 1).ConfigureAwait(true);
        }

        private static async Task<bool> 寒冰尖柱去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, _全局步骤e > 0 ? 10 : 0).ConfigureAwait(true);
        }

        #endregion

        #region 帕克

        private static async Task<bool> 幻象法球去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.Q, () =>
            {
                设置全局步骤q(1);
                Common.Delay(3400);
                if (_全局模式d == 1)
                {
                    SimKeyBoard.KeyPress(Keys.D);
                }

                设置全局步骤q(0);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 新月之痕去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 梦境缠绕去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 灵动之翼定位(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.D, () =>
            {
                SimKeyBoard.KeyPress(Keys.F1);
                SimKeyBoard.KeyPress(Keys.F1);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 骨法

        private static async Task<bool> 幽冥轰爆去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, _全局步骤r > 0 ? 10 : 0).ConfigureAwait(true);
        }

        private static async Task<bool> 衰老去后摇(ImageHandle 句柄)
        {
            return await Skill.主动技能进入CD后续(Keys.W, () =>
            {
                Common.Delay(33 * (
                    Item.根据图片使用物品(Dota2_Pictrue.物品.红杖)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4)
                    + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)
                ));
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 幽冥守卫去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, _全局步骤r > 0 ? 10 : 0).ConfigureAwait(true);
        }

        private static async Task<bool> 生命吸取去后摇(ImageHandle 句柄)
        {
            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                设置全局步骤r(0);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            return await Task.Run(async () =>
            {
                if (_全局步骤r == 0)
                {
                    if (_全局模式r == 1)
                    {
                        SimKeyBoard.KeyPress(Keys.W);
                    }

                    设置全局步骤r(1);
                    return await Task.FromResult(true).ConfigureAwait(true);
                }
                else if (_全局步骤r == 1)
                {
                    _ = Task.Run(() =>
                    {
                        Common.Delay(200);
                        设置全局步骤r(2);
                    });
                    return await Task.FromResult(true).ConfigureAwait(true);
                }
                else
                {
                    if (!Skill.DOTA2判断是否持续施法(in 句柄))
                    {
                        设置全局步骤r(0);
                        SimKeyBoard.KeyPress(Keys.A);
                        return await Task.FromResult(false).ConfigureAwait(true);
                    }
                    else
                    {
                        return await Task.FromResult(true).ConfigureAwait(true);
                    }
                }
            }).ConfigureAwait(false);
        }

        #endregion

        #region 干扰者

        private static async Task<bool> 风雷之击去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 静态风暴去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 恶念瞥视去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0, false).ConfigureAwait(true);
        }

        private static async Task<bool> 动能力场去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0, false).ConfigureAwait(true);
        }

        private static async Task<bool> 静态风暴动能立场风雷之击(ImageHandle 句柄)
        {
            return Skill.DOTA2释放CD就绪技能(Keys.R, in 句柄)
                ? await Task.FromResult(true).ConfigureAwait(true)
                : Skill.DOTA2释放CD就绪技能(Keys.E, in 句柄)
                    ? await Task.FromResult(true).ConfigureAwait(true)
                    : Skill.DOTA2释放CD就绪技能(Keys.Q, in 句柄)
                        ? await Task.FromResult(true).ConfigureAwait(true)
                        : await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 黑鸟

        // todo 逻辑修改
        private static async Task<bool> 关接陨星锤(ImageHandle 句柄)
        {
            int time = 0;

            Color 技能点颜色 = Color.FromArgb(203, 183, 124);

            if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(909, 1008, in 句柄), 技能点颜色, 0))
            {
                time = 4000;
            }
            else if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(897, 1008, in 句柄), 技能点颜色, 0))
            {
                time = 3250;
            }

            static void 关后(int time, in ImageHandle 句柄)
            {
                Common.Delay(110);
                Common.初始化全局时间(ref _全局时间w);
                SimKeyBoard.MouseRightClick();
                Common.Delay(150);
                SimKeyBoard.KeyPress(Keys.S);
                Common.Delay(time - 3000, _全局时间w);
                if (!_中断条件)
                {
                    _ = Item.根据图片使用物品(Dota2_Pictrue.物品.陨星锤);
                }
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            关后(time, in 句柄);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 神智之蚀去后摇(ImageHandle 句柄)
        {
            static void 神智之蚀后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            神智之蚀后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 关接跳(ImageHandle 句柄)
        {
            return Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀) == 1
                ? await Task.FromResult(false).ConfigureAwait(true)
                : await Task.FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region 谜团

        private static void 跳秒接午夜凋零黑洞()
        {
            //if (ImageFinder.FindImageBool(Dota2_Pictrue.物品.黑皇杖"), "Z) SimKeyBoard.KeyPress( Keys.Z);

            //if (ImageFinder.FindImageBool(Dota2_Pictrue.物品.纷争"), "C) SimKeyBoard.KeyPress( Keys.C);

            //var time = Common.获取当前时间毫秒();

            //while (ImageFinder.FindImageBool(Dota2_Pictrue.物品.跳刀"), "SPACE") || ImageFinder.FindImageBool(缓存嵌入的图片("物品.跳刀_智力跳刀"), "SPACE)
            //{
            //    Common.Delay(15);
            //    SimKeyBoard.KeyPress( Keys.Space);

            //    if (Common.获取当前时间毫秒() - time > 300) break;
            //}

            Common.Delay(等待延迟);

            //SimKeyBoard.KeyDown(Keys.LControl);

            //SimKeyBoard.KeyPress(Keys.A);

            //SimKeyBoard.KeyUp(Keys.LControl);
        }

        private static void 刷新接凋零黑洞()
        {
            SimKeyBoard.KeyPress(Keys.X);

            for (int i = 0; i < 2; i++)
            {
                Common.Delay(等待延迟);
                SimKeyBoard.KeyPress(Keys.Z);
                SimKeyBoard.KeyPress(Keys.V);
                SimKeyBoard.KeyPress(Keys.R);
            }
        }

        #endregion

        #region 冰女

        #endregion

        #region 火女

        private static async Task<bool> 龙破斩去后摇(ImageHandle 句柄)
        {
            static void 龙破斩后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            龙破斩后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 光击阵去后摇(ImageHandle 句柄)
        {
            static void 光击阵后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            光击阵后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 神灭斩去后摇(ImageHandle 句柄)
        {
            static void 神灭斩后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            神灭斩后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 拉席克

        private static async Task<bool> 撕裂大地去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 恶魔敕令去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 闪电风暴去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 脉冲新星去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 虚无主义去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
        }

        #endregion

        #region 蓝猫

        private static async Task<bool> 拉接平A(ImageHandle 句柄)
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        private static void 残影接平A()
        {
            Common.Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.A);
        }

        private static async Task<bool> 滚接平A(ImageHandle 句柄)
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region 卡尔

        private static async Task<bool> 三冰对线(ImageHandle 句柄)
        {
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 三雷对线(ImageHandle 句柄)
        {
            return await Task.FromResult(false).ConfigureAwait(true);
        }


        private static async Task<bool> 三雷幽灵(ImageHandle 句柄)
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        private static async Task<bool> 极冷吹风陨星锤雷暴(ImageHandle 句柄)
        {
            return await Task.FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region 术士

        private static async Task<bool> 致命链接去后摇(ImageHandle 句柄)
        {
            static void 致命链接后()
            {
                Skill.通用技能后续动作(false);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            致命链接后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 暗言术去后摇(ImageHandle 句柄)
        {
            static void 暗言术后()
            {
                Skill.通用技能后续动作(false);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            暗言术后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 混乱之祭去后摇(ImageHandle 句柄)
        {
            static void 混乱之祭后()
            {
                Skill.通用技能后续动作(false);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            混乱之祭后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 暗影萨满

        /// <summary>
        ///     前摇时间基本在
        /// </summary>
        /// <param name="bts"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static async Task<bool> 苍穹振击取消后摇(ImageHandle 句柄)
        {
            static void 苍穹振击后()
            {
                switch (_全局模式q)
                {
                    case 1:
                        SimKeyBoard.KeyPress(Keys.W);
                        break;
                    default:
                        SimKeyBoard.KeyPress(Keys.A);
                        break;
                }
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            苍穹振击后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     前摇时间基本再380-450 之间
        /// </summary>
        /// <param name="bts"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static async Task<bool> 枷锁持续施法隐身(ImageHandle 句柄)
        {
            static void 枷锁后(in ImageHandle 句柄)
            {
            }

            if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            枷锁后(in 句柄);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 释放群蛇守卫取消后摇(ImageHandle 句柄)
        {
            static void 群蛇守卫后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            群蛇守卫后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 变羊取消后摇(ImageHandle 句柄)
        {
            static void 萨满变羊后(ImageHandle 句柄)
            {
                Common.初始化全局时间(ref _全局时间w);

                Task.Run(() =>
                {
                    int time = 1250;

                    Color 技能点颜色 = Color.FromArgb(203, 183, 124);

                    if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(909, 1008, in 句柄), 技能点颜色, 0))
                    {
                        time = 3400;
                    }
                    else if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(897, 1008, in 句柄), 技能点颜色, 0))
                    {
                        time = 2650;
                    }
                    else if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(885, 1008, in 句柄), 技能点颜色, 0))
                    {
                        time = 1900;
                    }
                    else if (ColorExtensions.ColorAEqualColorB(获取指定位置颜色(875, 1008, in 句柄), 技能点颜色, 0))
                    {
                        time = 1150;
                    }

                    time = Convert.ToInt32(_状态抗性倍数 * time);

                    TTS.TTS.Speak(string.Concat("延时", time.ToString()));

                    SimKeyBoard.KeyPress(Keys.A);

                    switch (_全局模式w)
                    {
                        case 1:
                            Common.Delay(time - 435, _全局时间w);
                            SimKeyBoard.KeyPress(Keys.E);
                            break;
                        case 2:
                            SimKeyBoard.KeyPress(Keys.Q);
                            break;
                        case 3:
                            SimKeyBoard.KeyPress(Keys.Q);
                            Common.Delay(time - 435, _全局时间w);
                            SimKeyBoard.KeyPress(Keys.E);
                            break;
                        case 4:
                            SimKeyBoard.KeyPress(Keys.R);
                            Common.Delay(400);
                            SimKeyBoard.KeyPress(Keys.Q);
                            Common.Delay(time - 435, _全局时间w);
                            SimKeyBoard.KeyPress(Keys.E);
                            break;
                    }
                });
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            萨满变羊后(句柄);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 小仙女

        private static async Task<bool> 无限暗影之境(ImageHandle 句柄)
        {
            await Skill.技能通用判断(Keys.W, 2).ConfigureAwait(true);
            return await Task.FromResult(_条件5).ConfigureAwait(true);
        }

        #endregion

        #region 炸弹人

        private static async Task<bool> 粘性炸弹去后摇(ImageHandle 句柄)
        {
            static void 粘性炸弹后()
            {
                //RightClick();
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            粘性炸弹后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 活性电击去后摇(ImageHandle 句柄)
        {
            static void 活性电击后()
            {
                //RightClick();
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            活性电击后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 爆破起飞去后摇(ImageHandle 句柄)
        {
            static void 爆破起飞后()
            {
                //RightClick();
                SimKeyBoard.KeyPress(Keys.A);
                Common.Delay(750);

                switch (_全局模式e)
                {
                    case 1:
                        _条件4 = true;
                        _指定地点r = Control.MousePosition;
                        Common.初始化全局时间(ref _全局时间r);
                        break;
                    case 0:
                        break;
                }
            }

            if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            爆破起飞后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        // todo 逻辑修改
        private static async Task<bool> 爆破后接3雷粘性炸弹(ImageHandle 句柄)
        {
            if (Common.获取当前时间毫秒() - _全局时间r >= 3000)
            {
                _全局时间r = -1;
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 神域

        private static async Task<bool> 命运敕令去后摇(ImageHandle 句柄)
        {
            static async Task 命运敕令后()
            {
                await Task.Run(SimKeyBoard.MouseRightClick).ConfigureAwait(true);

                // SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            命运敕令后().Start();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 涤罪之焰去后摇(ImageHandle 句柄)
        {
            static async Task 涤罪之焰后()
            {
                await Task.Run(() => { SimKeyBoard.KeyPress(Keys.A); }).ConfigureAwait(true);
                // RightClick();
            }

            if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            涤罪之焰后().Start();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 虚妄之诺去后摇(ImageHandle 句柄)
        {
            static async Task 虚妄之诺后()
            {
                await Task.Run(() => { SimKeyBoard.KeyPress(Keys.A); }).ConfigureAwait(true);
                // SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            虚妄之诺后().Start();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 天命之雨去后摇(ImageHandle 句柄)
        {
            static void 天命之雨后()
            {
                SimKeyBoard.MouseRightClick();
                // SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.D, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            天命之雨后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 莱恩

        private static async Task<bool> 莱恩羊接技能(ImageHandle 句柄)
        {
            static void 莱恩羊后()
            {
                if (_条件6)
                {
                    SimKeyBoard.KeyPress(Keys.E);
                }
                else
                {
                    SimKeyBoard.KeyPress(Keys.A);
                }
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            莱恩羊后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 死亡一指去后摇(ImageHandle 句柄)
        {
            static void 死亡一指后()
            {
                //RightClick();
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            死亡一指后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 大招前纷争(ImageHandle 句柄)
        {
            Common.Delay(33 * (
                Item.根据图片使用物品(Dota2_Pictrue.物品.虚灵之刃)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.纷争)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖2)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖3)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖4)
                + Item.根据图片使用物品(Dota2_Pictrue.物品.红杖5)
            ));
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 推推破林肯秒羊(ImageHandle 句柄)
        {
            if (Item.根据图片使用物品(Dota2_Pictrue.物品.推推棒) == 1)
            {
                Common.Delay(等待延迟);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            SimKeyBoard.KeyPress(Keys.W);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 羊刺刷新秒人(ImageHandle 句柄)
        {
            int 步骤 = 获取全局步骤();

            if (步骤 == 1)
            {
                if (Item.根据图片使用物品(Dota2_Pictrue.物品.奥术鞋) == 1)
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒) == 1)
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
                {
                    SimKeyBoard.KeyPress(Keys.Q);
                    Common.Delay(60);
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
                {
                    SimKeyBoard.KeyPress(Keys.R);
                    Common.Delay(60);
                    return await Task.FromResult(true).ConfigureAwait(true);
                }
            }
            else if (步骤 == 0)
            {
                if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀) == 1)
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Item.根据图片使用物品(Dota2_Pictrue.物品.跳刀_智力跳刀) == 1)
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
                {
                    SimKeyBoard.KeyPress(Keys.W);
                    Common.Delay(等待延迟);
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
                {
                    SimKeyBoard.KeyPress(Keys.Q);
                    Common.Delay(60);
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Item.根据图片使用物品(Dota2_Pictrue.物品.奥术鞋) == 1)
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Item.根据图片使用物品(Dota2_Pictrue.物品.魂戒) == 1)
                {
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
                {
                    SimKeyBoard.KeyPress(Keys.R);
                    Common.Delay(60);
                    return await Task.FromResult(true).ConfigureAwait(true);
                }

                if (_条件5 && Item.根据图片使用物品(Dota2_Pictrue.物品.刷新球) == 1)
                {
                    设置全局步骤(1);
                    Common.Delay(120);
                    return await Task.FromResult(true).ConfigureAwait(true);
                }
            }

            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 沉默

        private static async Task<bool> 奥数诅咒去后摇(ImageHandle 句柄)
        {
            static async void 奥数诅咒后(ImageHandle 句柄)
            {
                _全局时间q = -1;
                // RightClick();
                // SimKeyBoard.KeyPress(Keys.A);
                switch (_全局模式q)
                {
                    case < 1:
                        _ = await 大招前纷争(句柄).ConfigureAwait(true);
                        SimKeyBoard.KeyPress(Keys.E);
                        break;
                    case 1:
                        _ = await 大招前纷争(句柄).ConfigureAwait(true);
                        Common.Delay(1300);
                        SimKeyBoard.KeyPress(Keys.E);
                        break;
                    case 2:
                        SimKeyBoard.KeyPress(Keys.A);
                        break;
                }
            }

            // 超时则切回 总体释放时间
            if (Common.获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
            {
                奥数诅咒后(句柄);
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            奥数诅咒后(句柄);
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 全领域沉默去后摇(ImageHandle 句柄)
        {
            static void 全领域沉默后()
            {
                _全局时间r = -1;
                // RightClick();
                SimKeyBoard.KeyPress(Keys.A);
            }

            // 超时则切回 总体释放时间
            if (Common.获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
            {
                全领域沉默后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            全领域沉默后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 戴泽

        private static async Task<bool> 剧毒之触去后摇(ImageHandle 句柄)
        {
            static void 剧毒之触后()
            {
                _全局时间q = -1;
                Skill.通用技能后续动作();
            }

            // 超时则切回 总体释放时间
            if (Common.获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
            {
                剧毒之触后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            剧毒之触后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 薄葬去后摇(ImageHandle 句柄)
        {
            static void 薄葬后()
            {
                _全局时间w = -1;
                Skill.通用技能后续动作();
            }

            // 超时则切回 总体释放时间
            if (Common.获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
            {
                薄葬后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            薄葬后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 暗影波去后摇(ImageHandle 句柄)
        {
            static void 暗影波后()
            {
                _全局时间e = -1;
                Skill.通用技能后续动作();
            }

            // 超时则切回 总体释放时间
            if (Common.获取当前时间毫秒() - _全局时间e > 1200 && _全局时间e != -1)
            {
                暗影波后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            暗影波后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 邪能去后摇(ImageHandle 句柄)
        {
            static void 邪能后()
            {
                _全局时间r = -1;
                Skill.通用技能后续动作();
            }

            // 超时则切回 总体释放时间
            if (Common.获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
            {
                邪能后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            邪能后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 双头龙

        private static async Task<bool> 冰火交加去后摇(ImageHandle 句柄)
        {
            static void 冰火交加后()
            {
                _全局时间q = -1;
                SimKeyBoard.MouseRightClick();
            }

            // 超时则切回 总体释放时间
            if (Common.获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
            {
                冰火交加后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            冰火交加后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 冰封路径去后摇(ImageHandle 句柄)
        {
            static void 冰封路径后()
            {
                _全局时间w = -1;
                SimKeyBoard.MouseRightClick();
            }

            // 超时则切回 总体释放时间
            if (Common.获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
            {
                冰封路径后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            冰封路径后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 烈焰焚身去后摇(ImageHandle 句柄)
        {
            static void 烈焰焚身后()
            {
                _全局时间r = -1;
                // RightClick();
            }

            // 超时则切回 总体释放时间
            if (Common.获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
            {
                烈焰焚身后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            烈焰焚身后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 吹风接冰封路径(ImageHandle 句柄)
        {
            if (Item.根据图片使用物品(Dota2_Pictrue.物品.吹风) == 1)
            {
                Common.Delay(等待延迟);
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            if (!ImageFinder.FindImageInRegionBool(Dota2_Pictrue.物品.吹风, in 句柄, Item.获取物品范围(Skill._技能数量)) && _全局时间 == -1)
            {
                Common.初始化全局时间(ref _全局时间);
            }

            if (Common.获取当前时间毫秒() - _全局时间 < 2500 - 650 - 600)
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            SimKeyBoard.KeyPress(Keys.W);
            _全局时间 = -1;
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 奶绿

        private static async Task<bool> 弹无虚发去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 唤魂去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 越界去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0, 判断成功后延时: 360).ConfigureAwait(true);
        }

        private static async Task<bool> 临别一枪去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.D, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 祭台去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.F, 0).ConfigureAwait(true);
        }

        #endregion

        #region 女王

        private static async Task<bool> 暗影突袭去后摇(ImageHandle 句柄)
        {
            static void 暗影突袭后()
            {
                _全局时间q = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            暗影突袭后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 闪烁去后摇(ImageHandle 句柄)
        {
            static void 闪烁后()
            {
                _全局时间w = -1;
                SimKeyBoard.MouseRightClick();
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            闪烁后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 痛苦尖叫去后摇(ImageHandle 句柄)
        {
            static void 痛苦尖叫后()
            {
                _全局时间e = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            痛苦尖叫后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 冲击波去后摇(ImageHandle 句柄)
        {
            static void 冲击波后()
            {
                _全局时间r = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.R, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            冲击波后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 蓝胖

        private static async Task<bool> 火焰轰爆去后摇(ImageHandle 句柄)
        {
            static void 火焰轰爆后()
            {
                SimKeyBoard.MouseRightClick();
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            火焰轰爆后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 引燃去后摇(ImageHandle 句柄)
        {
            static void 引燃后()
            {
                switch (_全局模式w)
                {
                    case 1:
                        SimKeyBoard.KeyPress(Keys.Q);
                        break;
                    default:
                        SimKeyBoard.MouseRightClick();
                        break;
                }
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            引燃后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 嗜血术去后摇(ImageHandle 句柄)
        {
            static void 嗜血术后()
            {
                SimKeyBoard.MouseRightClick();
            }

            if (Skill.DOTA2判断技能是否CD(Keys.E, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            嗜血术后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 未精通火焰轰爆去后摇(ImageHandle 句柄)
        {
            static void 未精通火焰轰爆后()
            {
                SimKeyBoard.MouseRightClick();
            }

            if (Skill.DOTA2判断技能是否CD(Keys.D, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            未精通火焰轰爆后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 烈火护盾去后摇(ImageHandle 句柄)
        {
            static void 烈火护盾后()
            {
                SimKeyBoard.MouseRightClick();
            }

            if (Skill.DOTA2判断技能是否CD(Keys.F, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            烈火护盾后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 祸乱之源

        private static async Task<bool> 虚弱去后摇(ImageHandle 句柄)
        {
            static void 虚弱后()
            {
                Skill.通用技能后续动作(false);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            虚弱后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 噬脑去后摇(ImageHandle 句柄)
        {
            static void 噬脑后()
            {
                Skill.通用技能后续动作();
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            噬脑后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 噩梦接平A锤(ImageHandle 句柄)
        {
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 瘟疫法师

        private static async Task<bool> 死亡脉冲去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 0, 是否接按键: false).ConfigureAwait(true);
        }

        private static async Task<bool> 幽魂护罩去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 0, 是否接按键: false).ConfigureAwait(true);
        }

        private static async Task<bool> 死神镰刀去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 循环死亡脉冲(ImageHandle 句柄)
        {
            await Skill.技能通用判断(Keys.Q, 2).ConfigureAwait(true);
            return await Task.FromResult(_条件5).ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region 全才

        #region 剧毒

        private static async Task<bool> 瘴气去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 蛇棒去后摇(ImageHandle 句柄)
        {
            SimKeyBoard.MouseRightClick();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 恶性瘟疫去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 循环蛇棒(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.E, 2).ConfigureAwait(true);
        }

        #endregion

        #region 猛犸

        private static async Task<bool> 震荡波去后摇(ImageHandle 句柄)
        {
            static void 震荡波后()
            {
                Skill.通用技能后续动作();
            }

            震荡波后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 授予力量去后摇(ImageHandle 句柄)
        {
            static void 授予力量后()
            {
                Skill.通用技能后续动作();
            }

            授予力量后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 巨角冲撞去后摇(ImageHandle 句柄)
        {
            static void 巨角冲撞后()
            {
                Skill.通用技能后续动作();
            }

            巨角冲撞后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 长角抛物去后摇(ImageHandle 句柄)
        {
            static void 长角抛物后()
            {
                Skill.通用技能后续动作();
            }

            长角抛物后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 两级反转去后摇(ImageHandle 句柄)
        {
            static void 两级反转后()
            {
                Skill.通用技能后续动作();
            }

            两级反转后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        // todo 逻辑优化 有鱼叉
        private static void 跳拱指定地点()
        {
            SimKeyBoard.KeyPress(Keys.Space);
            Common.Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.D9);
            SimKeyBoard.MouseMove(_指定地点p);
            Common.Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.E);
            Common.Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.D9);
        }

        #endregion

        #region 狼人

        private static async Task<bool> 招狼去后摇(ImageHandle 句柄)
        {
            static void 招狼后()
            {
                _全局时间q = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Common.获取当前时间毫秒() - _全局时间q > 400 && _全局时间q != -1 && Item._条件开启切假腿)
            {
                招狼后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.Q, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            招狼后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 嚎叫去后摇(ImageHandle 句柄)
        {
            static void 嚎叫后()
            {
                _全局时间w = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Common.获取当前时间毫秒() - _全局时间w > 400 && _全局时间w != -1 && Item._条件开启切假腿)
            {
                嚎叫后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.W, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            嚎叫后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 撕咬去后摇(ImageHandle 句柄)
        {
            static void 撕咬后()
            {
                _全局时间d = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (Common.获取当前时间毫秒() - _全局时间d > 400 && _全局时间d != -1 && Item._条件开启切假腿)
            {
                撕咬后();
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            if (Skill.DOTA2判断技能是否CD(Keys.D, in 句柄))
            {
                return await Task.FromResult(true).ConfigureAwait(true);
            }

            撕咬后();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 变狼去后摇(ImageHandle 句柄)
        {
            if (Common.获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1 && Item._条件开启切假腿)
            {
                _全局时间r = -1;
                return await Task.FromResult(false).ConfigureAwait(true);
            }

            return await Task.FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region VS

        private static async Task<bool> 魔法箭去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 恐怖波动去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 移形换位去后摇(ImageHandle 句柄)
        {
            return await Skill.技能通用判断(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 小精灵

        private static async Task<bool> 幽魂检测(ImageHandle 句柄)
        {
            return ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.小精灵_幽魂, in 句柄, buff状态技能栏)
                ? await Task.FromResult(true).ConfigureAwait(true)
                : await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 循环续过载(ImageHandle 句柄)
        {
            bool guozai = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.小精灵_过载, in 句柄, buff状态技能栏);
            if (guozai)
            {
                _全局步骤e = 3;
                return await Task.FromResult(_条件3).ConfigureAwait(true);
            }
            else
            {
                await Skill.技能通用判断(Keys.E, 2).ConfigureAwait(true);
                return await Task.FromResult(_条件3).ConfigureAwait(true);
            }
        }

        #endregion

        #region 小强

        //private static async Task<bool> 穿刺去后摇(ImageHandle 句柄)
        //{
        //    return await Skill.技能通用判断(Keys.Q, 1).ConfigureAwait(true);
        //}

        //private static async Task<bool> 神智爆裂去后摇(ImageHandle 句柄)
        //{
        //    return await Skill.技能通用判断(Keys.W, 1).ConfigureAwait(true);
        //}

        //private static async Task<bool> 尖刺外壳去后摇(ImageHandle 句柄)
        //{
        //    return await Skill.技能通用判断(Keys.E, 0, false).ConfigureAwait(true);
        //}

        //private static async Task<bool> 复仇接穿刺(ImageHandle 句柄)
        //{
        //    if (_全局步骤r == 0)
        //    {
        //        bool 技能释放 = Skill.DOTA2判断技能是否CD(Keys.R, 句柄);
        //        if (!技能释放)
        //        {
        //            设置全局步骤r(1);
        //        }

        //        return await Task.FromResult(true).ConfigureAwait(true);
        //    }
        //    else if (_全局步骤r == 1)
        //    {
        //        _ = Task.Run(() =>
        //        {
        //            Common.Delay(300);
        //            设置全局步骤r(2);
        //        });
        //        return await Task.FromResult(true).ConfigureAwait(true);
        //    }
        //    else
        //    {
        //        bool 大招状态 = ImageFinder.FindImageInRegionBool(Dota2_Pictrue.Buff.小强_大招, 句柄, buff状态技能栏);
        //        if (大招状态)
        //        {
        //            return await Task.FromResult(true).ConfigureAwait(true);
        //        }

        //        if (Skill.DOTA2释放CD就绪技能(Keys.Q, 句柄))
        //        {
        //            return await Task.FromResult(true).ConfigureAwait(true);
        //        }

        //        设置全局步骤r(0);
        //        return await Task.FromResult(false).ConfigureAwait(true);
        //    }
        //}

        private static async Task<bool> 循环接爆裂(ImageHandle 句柄)
        {
            await Skill.技能通用判断(Keys.W, 2).ConfigureAwait(true);
            return await Task.FromResult(_条件5).ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region 其他功能

        #region 吃恢复道具臂章

        private static async Task 切臂章()
        {
            Keys key = Item.根据图片获取物品按键(Dota2_Pictrue.物品.臂章_开启);
            if (key != Keys.Escape)
            {
                SimKeyBoard.KeyPress(key);
                Common.Delay(15);
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.魔棒);
                _ = Item.根据图片自我使用物品(Dota2_Pictrue.物品.吊坠);
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.仙草);
                _ = Item.根据图片使用物品(Dota2_Pictrue.物品.假腿_力量腿);
                Common.Delay(15);
                SimKeyBoard.KeyPress(key);
                Item._条件假腿敏捷 = false;
                Item.要求保持假腿();

                _ = await Task.FromResult(false).ConfigureAwait(true);
            }
        }

        #endregion

        #region 快速触发激怒

        private static void 快速触发激怒()
        {
            var 原始位置 = Control.MousePosition;

            for (int i = 0; i < 10; i++)
            {
                SimKeyBoard.MouseMove(575 + 515 + 61 * i, 20);
                SimKeyBoard.KeyPress(Keys.A);
                Common.Delay(2);
            }

            SimKeyBoard.MouseMove(原始位置);
        }

        #endregion

        #region 泉水状态喝瓶子 已经是版本过去了

        //private static void 泉水状态喝瓶()
        //{
        //    Common.Delay(400);

        //    for (var i = 1; i <= 4; i++)
        //    {
        //        SimKeyBoard.KeyPress(Keys.C);
        //        Common.Delay(587);
        //    }
        //}

        //private static void 泉水状态喂瓶()
        //{
        //    Common.Delay(3300);

        //    var time = Common.获取当前时间毫秒();

        //    for (var i = 1; i <= 10; i++)
        //    {
        //        if (Common.获取当前时间毫秒() - time > 1850) return;

        //        SimKeyBoard.KeyDown(Keys.LControl);
        //        SimKeyBoard.KeyDown(Keys.C);
        //        SimKeyBoard.KeyUp(Keys.LControl);
        //        SimKeyBoard.KeyUp(Keys.C);

        //        Common.Delay(587);
        //    }
        //}

        #endregion

        #region 指定地点

        private static void 指定地点()
        {
            _指定地点p = Control.MousePosition;

            Common.Delay(等待延迟);
            SimKeyBoard.KeyDown(Keys.Control);
            Common.Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.D9);
            Common.Delay(等待延迟);
            SimKeyBoard.KeyUp(Keys.Control);
        }

        #endregion

        #region 跳刀

        /// <summary>
        ///     用于快速先手无转身
        /// </summary>
        /// <returns></returns>
        private static async Task<Point> 正面跳刀_无转身(ImageHandle 句柄)
        {
            // 坐标
            Point mousePosition = Control.MousePosition;

            // X间距
            double moveX = 0;
            // Y间距，自动根据X调整
            double moveY = 0;

            Point p = await 快速获取自身坐标().ConfigureAwait(true);

            TTS.TTS.Speak(string.Concat("自身坐标为:", p.X + 55, "  ", p.Y + 80));
            Common.Delay(2000);

            double realX = p.X + 55;
            double realY = p.Y + 80;

            moveY = mousePosition.Y > realY ? -60 + mousePosition.Y : 60 + mousePosition.Y;
            moveX = mousePosition.X > realX ? -80 + mousePosition.X : 80 + mousePosition.X;


            if (Math.Abs(mousePosition.Y - realY) <= 180.0)
            {
                moveY = mousePosition.Y;
            }

            if (Math.Abs(mousePosition.X - realX) <= 180.0)
            {
                moveX = mousePosition.X;
            }

            return await Task.FromResult(new Point(Convert.ToInt16(moveX), Convert.ToInt16(moveY))).ConfigureAwait(true);
        }

        #endregion

        #region 续走A

        /// <summary>
        ///     根据攻速去后摇
        /// </summary>
        /// <param name="额外攻速"></param>
        /// <returns></returns>
        private static void 走A去等待后摇()
        {
            SpinWait spinWait = new();
            bool 等待平A = false;
            while (true)
            {
                if (!_开启走A)
                {
                    return;
                }

                if (_停止走A > 0)
                {
                    _停止走A = 0;
                    break;
                }

                long currentTime = Common.获取当前时间毫秒();

                long remainingTime;
                if (!等待平A)
                {
                    // A+等待前摇 = 实际出手时间， 实际出手时间+等待间隔 = 下一次攻击开始时间 转身速率0.7 转180°157ms延时 
                    remainingTime = _实际出手时间 - currentTime + 180;

                    if (remainingTime > 10)
                    {
                        Thread.Sleep((int)(remainingTime / 2)); // 睡眠2分之一
                    }
                    else if (remainingTime > 2)
                    {
                        Thread.Sleep(1); // 如果剩余时间小于10毫秒，但大于2毫秒，则睡眠1毫秒
                    }
                    else
                    {
                        if (remainingTime <= 0)
                        {
                            SimKeyBoard.KeyPress(Keys.M);
                            等待平A = true;
                            continue;
                        }

                        spinWait.SpinOnce(); // SpinWait for very short intervals
                    }
                }
                else
                {
                    // 攻击前摇算在攻击间隔里面
                    remainingTime = _实际出手时间 + _实际攻击间隔 - _实际攻击前摇 - currentTime;

                    if (remainingTime > 10)
                    {
                        Thread.Sleep((int)(remainingTime / 2)); // 睡眠2分之一
                    }
                    else if (remainingTime > 2)
                    {
                        Thread.Sleep(1); // 如果剩余时间小于10毫秒，但大于2毫秒，则睡眠1毫秒
                    }
                    else
                    {
                        if (remainingTime <= 0)
                        {
                            SimKeyBoard.KeyPress(Keys.A);
                            break;
                        }

                        spinWait.SpinOnce(); // SpinWait for very short intervals
                    }
                }
            }
        }

        #endregion

        #region 扔装备

        private void 批量扔装备()
        {
            //SimKeyBoard.KeyPress(Keys.S);
            //Common.Delay(40);
            //SimKeyBoard.KeyPress(Keys.F1);
            //Common.Delay(40);
            //SimKeyBoard.KeyPress(Keys.F1);

            //using PooledList<string> list1 = [.. tb_阵营.Text.Split(',')];

            //try
            //{
            //    switch (Skill._技能数量)
            //    {
            //        case 6:
            //            foreach (string t in list1)
            //            {
            //                switch (t)
            //                {
            //                    case "1":
            //                        扔装备(new Point(1191, 963));
            //                        break;
            //                    case "2":
            //                        扔装备(new Point(1259, 963));
            //                        break;
            //                    case "3":
            //                        扔装备(new Point(1325, 963));
            //                        break;
            //                    case "4":
            //                        扔装备(new Point(1191, 1011));
            //                        break;
            //                    case "5":
            //                        扔装备(new Point(1259, 1011));
            //                        break;
            //                    case "6":
            //                        扔装备(new Point(1325, 1011));
            //                        break;
            //                    case "7":
            //                        扔装备(new Point(1384, 994));
            //                        break;
            //                }
            //            }

            //            break;
            //        case 4:
            //            foreach (string t in list1)
            //            {
            //                switch (t)
            //                {
            //                    case "1":
            //                        扔装备(new Point(1145, 966));
            //                        break;
            //                    case "2":
            //                        扔装备(new Point(1214, 963));
            //                        break;
            //                    case "3":
            //                        扔装备(new Point(1288, 963));
            //                        break;
            //                    case "4":
            //                        扔装备(new Point(1145, 1011));
            //                        break;
            //                    case "5":
            //                        扔装备(new Point(1214, 1011));
            //                        break;
            //                    case "6":
            //                        扔装备(new Point(1288, 1011));
            //                        break;
            //                    case "7":
            //                        扔装备(new Point(1337, 994));
            //                        break;
            //                }
            //            }

            //            break;
            //        case 5:
            //            foreach (string t in list1)
            //            {
            //                switch (t)
            //                {
            //                    case "1":
            //                        扔装备(new Point(1160, 966));
            //                        break;
            //                    case "2":
            //                        扔装备(new Point(1227, 963));
            //                        break;
            //                    case "3":
            //                        扔装备(new Point(1295, 963));
            //                        break;
            //                    case "4":
            //                        扔装备(new Point(1160, 1011));
            //                        break;
            //                    case "5":
            //                        扔装备(new Point(1227, 1011));
            //                        break;
            //                    case "6":
            //                        扔装备(new Point(1295, 1011));
            //                        break;
            //                    case "7":
            //                        扔装备(new Point(1352, 994));
            //                        break;
            //                }
            //            }

            //            break;
            //    }
            //}
            //catch (Exception)
            //{
            //    // ignored
            //}

            //list1.Dispose();
        }

        private static void 扔装备(Point p)
        {
            SimKeyBoard.MouseMove(p);
            SimKeyBoard.MouseLeftDown();
            Common.Delay(40);
            SimKeyBoard.MouseMove(new Point(p.X + 5, p.Y + 5));
            Common.Delay(40);
            SimKeyBoard.KeyDown(Keys.Y);
            Common.Delay(40);
            SimKeyBoard.MouseLeftUp();
            SimKeyBoard.KeyUp(Keys.Y);
            Common.Delay(40);
        }

        private void 捡装备()
        {
            //using PooledList<string> list1 = new(tb_阵营.Text.Split(','));
            //SimKeyBoard.KeyDown(Keys.Y);
            //Common.Delay(40);
            //for (int i = 0; i < list1.Count + 2; i++)
            //{
            //    SimKeyBoard.MouseRightClick();
            //    Common.Delay(100);
            //}

            //list1.Dispose();
            //SimKeyBoard.KeyUp(Keys.Y);
        }

        #endregion

        #region 分身一齐攻击

        /// <summary>
        ///     因为有0.1秒的分裂时间，所以必须等待
        /// </summary>
        private static void 分身一齐攻击()
        {
            Common.Delay(140);
            SimKeyBoard.KeyDown(Keys.Control);
            SimKeyBoard.KeyPress(Keys.A);
            SimKeyBoard.KeyUp(Keys.Control);
        }

        #endregion

        #endregion

        #region 经过时间播报

        /// <summary>
        ///     用于播报
        /// </summary>
        /// <param name="ln">对比时间</param>
        /// <param name="Common.Delay">等待ms后播放</param>
        private static async Task 检测时间播报(long ln, int 延迟)
        {
            long a = Common.获取当前时间毫秒() - ln;

            await Task.Run(() =>
            {
                Common.Delay(延迟);
                TTS.TTS.Speak(string.Concat("经过时间", a));
            }).ConfigureAwait(false);
        }

        #endregion

        #region 记录买活

        /*
        private static void 记录买活()
        {
            var 计时颜色 = Color.FromArgb(14, 19, 24);

            while (true)
            {
                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 549, 41, 52, 21) && !CaptureColor(559, 76).Equals(计时颜色))
                {
                    var p = MousePosition;
                    while (!CaptureColor(559, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(559, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 613, 41, 52, 21) && !CaptureColor(623, 76).Equals(计时颜色))
                {
                    var p = MousePosition;

                    while (!CaptureColor(623, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(623, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 674, 41, 52, 21) && !CaptureColor(688, 76).Equals(计时颜色))
                {
                    var p = MousePosition;

                    while (!CaptureColor(688, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(688, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 735, 41, 52, 21) && !CaptureColor(749, 76).Equals(计时颜色))
                {
                    var p = MousePosition;

                    while (!CaptureColor(749, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(749, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 797, 41, 52, 21) && !CaptureColor(811, 76).Equals(计时颜色))
                {
                    var p = MousePosition;

                    while (!CaptureColor(811, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(811, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 1060, 41, 52, 21) && !CaptureColor(1073, 76).Equals(计时颜色))
                {
                    var p = MousePosition;

                    while (!CaptureColor(1073, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(1073, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 1124, 41, 52, 21) && !CaptureColor(1137, 76).Equals(计时颜色))
                {
                    var p = MousePosition;

                    while (!CaptureColor(1137, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(1137, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 1185, 41, 52, 21) && !CaptureColor(1198, 76).Equals(计时颜色))
                {
                    var p = MousePosition;

                    while (!CaptureColor(1198, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(1198, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 1248, 41, 52, 21) && !CaptureColor(1261, 76).Equals(计时颜色))
                {
                    var p = MousePosition;

                    while (!CaptureColor(1261, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(1261, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                if (ImageFinder.FindImageBool(Resource_Picture.播报_买活, 1308, 41, 52, 21) && !CaptureColor(1321, 76).Equals(计时颜色))
                {
                    var p = MousePosition;

                    while (!CaptureColor(1321, 76).Equals(计时颜色))
                    {
                        KeyBoardSim.SimKeyBoard.MouseMove(1321, 76);
                        Thread.Sleep(30);
                        LeftClick();
                        Thread.Sleep(30);
                    }

                    KeyBoardSim.SimKeyBoard.MouseMove(p);
                }

                Thread.Sleep(100);
            }
        }
        */

        #endregion

        #region 记录肉山

        private static void 获取时间肉山()
        {
            快速发言("肉山");
        }

        #endregion

        #region 记录塔防

        /*
        private static void 获取时间塔防()
        {
            using Bitmap bp = CaptureScreen(930, 21, 58, 16);
            string str = PaddleOcr.获取图片文字(bp);
            str = string.Concat("塔防刷新", str.Replace("：", ":"));
            Common.Delay(500);
            快速发言(str);
        }
        */
        #endregion

        #region 快速发言

        private static void 快速发言(string str)
        {
            Clipboard.SetText(str);
            SimKeyBoard.KeyPress(Keys.Enter);
            SimKeyBoard.KeyDown(Keys.Control);
            SimKeyBoard.KeyPress(Keys.V);
            SimKeyBoard.KeyUp(Keys.Control);
            Common.Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.Enter);
            Common.Delay(等待延迟);
        }

        #endregion

        #region 快速选择敌方英雄

        /// <summary>
        ///     基本需要时间 50ms 左右
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="type">模拟移动1 直接到位0</param>
        /// <param name="type1">无视野跳刀 1 有视野 0</param>
        /// <returns></returns>
        private static async Task<bool> 快速选择敌方英雄(int width = 1920, int height = 1080, int type = 0, int type1 = 0)
        {
            Point p = Control.MousePosition;
            int x = 0;
            int y = 0;

            if (width == 1920 || height == 1080)
            {
                x = 0;
                y = 0;
            }
            else
            {
                x = p.X - width / 2 < 0 ? 0 : p.X - width / 2;
                y = p.Y - height / 2 < 0 ? 0 : p.Y - height / 2;
            }

            if (type1 == 1)
            {
                Common.Delay(330); // 基本延迟用于迷雾显示
            }

            PooledList<Point> list = 获取敌方坐标(GlobalScreenCapture.GetCurrentHandle());

            int 偏移x = 1920;
            int 偏移y = 1080;

            foreach (Point item in list)
            {
                SimKeyBoard.MouseMoveSim(item.X + x + 50, item.Y + y + 80);
                TTS.TTS.Speak(string.Concat("坐标X", item.X + x + 50, "坐标y", item.Y + y + 80));
                Common.Delay(2000);
            }


            foreach (Point item in list.Where(item =>
                         Math.Pow(item.X + x - p.X + 50, 2) + Math.Pow(item.Y + y - p.Y + 80, 2) <
                         Math.Pow(偏移x, 2) + Math.Pow(偏移y, 2)))
            {
                偏移x = item.X + x + 50 - p.X;
                偏移y = item.Y + y + 80 - p.Y;
            }

            if (list.Count > 0)
            {
                if (type == 1)
                {
                    SimKeyBoard.MouseMoveSim(p.X + 偏移x, p.Y + 偏移y);
                }
                else
                {
                    SimKeyBoard.MouseMove(p.X + 偏移x, p.Y + 偏移y);
                }
            }

            list.Dispose();

            return await Task.FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region 获取敌方坐标

        private static PooledList<Point> 获取敌方坐标(ImageHandle 句柄)
        {
            PooledList<Color> colors = [];
            PooledList<Point> points = [];

            colors.Add(Color.FromArgb(58, 28, 27));
            points.Add(new Point(0, 0));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(0, 1));

            colors.Add(Color.FromArgb(58, 28, 27));
            points.Add(new Point(99, 0));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(99, 1));

            colors.Add(Color.FromArgb(58, 28, 27));
            points.Add(new Point(100, 0));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(100, 1));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(0, 12));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(0, 13));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(99, 12));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(99, 13));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(100, 12));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(100, 13));
            Common.初始化全局时间(ref _全局时间);


            PooledList<Point> list1 = ImageManager.FindColors(colors, points, 句柄, 1);

            检测时间播报(_全局时间, 0);
            //Tts.TTS.TTS.Speak(string.Concat("1找到",list1.Count));
            Common.Delay(2000);

            return list1;
        }

        #endregion

        #region 快速获取自身坐标

        private static async Task<Point> 快速获取自身坐标(int width = 1920, int height = 1080)
        {
            return await Task.FromResult(获取自身坐标(GlobalScreenCapture.GetCurrentHandle())).ConfigureAwait(true);
        }

        #endregion

        #region 获取自身坐标

        private static Point 获取自身坐标(ImageHandle 句柄)
        {
            PooledList<Color> colors = [];
            PooledList<Point> points = [];

            colors.Add(Color.FromArgb(35, 77, 35));
            points.Add(new Point(0, 0));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(0, 1));

            colors.Add(Color.FromArgb(33, 70, 33));
            points.Add(new Point(107, 0));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(107, 1));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(0, 18));

            colors.Add(Color.FromArgb(0, 1, 0));
            points.Add(new Point(0, 19));

            colors.Add(Color.FromArgb(0, 0, 0));
            points.Add(new Point(107, 18));

            colors.Add(Color.FromArgb(0, 1, 0));
            points.Add(new Point(107, 19));

            Common.初始化全局时间(ref _全局时间);

            PooledList<Point> list1 = ImageManager.FindColors(colors, points, 句柄, 1);

            检测时间播报(_全局时间, 0);
            //Tts.TTS.TTS.Speak(string.Concat("1找到",list1.Count));
            Common.Delay(2000);


            return list1.Count > 0 ? list1[0] : new Point();
        }

        #endregion

        #endregion

        #region 测试_捕捉颜色

        #region 延时测试

        private static void 测试其他功能()
        {
            _全局时间 = Common.获取当前时间毫秒();

            //using (var duplicator = new DesktopDuplicator())
            //{
            //    for (int i = 0; i < 3; i++)
            //    {
            //        duplicator.Capture(new Rectangle(截图模式1X, 截图模式1Y, 截图模式1W, 截图模式1H));
            //    }
            //}

            //var i1 = Convert.ToInt32(tb_攻速.Text.Trim());

            Item.保存当前物品();

            Common.Main_Form?.Invoke(() => { Common.Main_Form.tb_y.Text = (Common.获取当前时间毫秒() - _全局时间).ToString(CultureInfo.InvariantCulture); });

            TTS.TTS.Speak("完成");
        }

        #endregion

        #region 旧找色

        /***
         *  旧使用说明
         *  物品 4 1118 943 1309 1034
         *  物品 5 1134 943 1324 1034
         *  物品 6 1162 943 1353 1034
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
            技能坐标 右上角 +3 边框长度

            沉默 恐惧 不能释放 颜色 吹风等其他控制依旧是原色
            25,29,32

            默认颜色 不释放 释放中 都不变色
            变回改色说明已可以释放，优先级高于白色刷新，且后续不变色
            45,52,59

            4技能
            技能坐标 右上角 +4 边框长度

            沉默 恐惧 不能释放 颜色 吹风等其他控制依旧是原色
            14, 18, 20

            默认颜色 不释放 释放中 都不变色
            改变颜色说明已经释放，只要释放出后颜色就会改变无关框大小
            变回改色说明已可以释放，优先级高于白色刷新，且后续不变色
            4 技能 65, 74, 81
            5 6 技能 45, 52, 59

            4技能 67, 76, 84 最右上角（性能优化）
            5 6 技能 55, 62, 70  54, 61, 69 最右上角（性能优化）

            状态颜色 11,17,8 未开启 0,129,0 已开启

            逻辑上是简单而且通用，但延时方面稍微逊色（优化后改善）
            新通过顶点颜色匹配，获取原来的颜色，然后判断是否释放颜色

            4释放颜色，为起始颜色
            new R = R * R * 0.0001 + 0.7667 * R 颜色容差2.5065 保险3
            new G = G * G * 0.0014 + 0.0251 * G 颜色容差2.0513 保险3
            new B = G * G * 0.0002 + 0.7586 * G 颜色容差2.8620 保险3

            比技能CD更加有用，判断更快，6技能通用

            技能图标右侧几个像素颜色，此颜色为CD就绪，反之进入CD
            数组找色基本不耗时间

            主要还是看实现，上面的已经很大的变化了
         ***/

        #endregion

        #endregion

        #region 通用循环

        #region 循环

        private readonly record struct 条件配置项(
            int 索引,
            string 名称,
            Func<bool> 获取条件,
            Action<bool> 设置条件,
            ConditionDelegateBitmap 委托);

        private static async Task 一般程序循环()
        {
            // 条件配置
            var 条件配置 = new[]
            {
        new 条件配置项(0, "条件1", () => _条件1, val => _条件1 = val, _条件根据图片委托1),
        new 条件配置项(1, "条件2", () => _条件2, val => _条件2 = val, _条件根据图片委托2),
        new 条件配置项(2, "条件3", () => _条件3, val => _条件3 = val, _条件根据图片委托3),
        new 条件配置项(3, "条件4", () => _条件4, val => _条件4 = val, _条件根据图片委托4),
        new 条件配置项(4, "条件5", () => _条件5, val => _条件5 = val, _条件根据图片委托5),
        new 条件配置项(5, "条件6", () => _条件6, val => _条件6 = val, _条件根据图片委托6),
        new 条件配置项(6, "条件7", () => _条件7, val => _条件7 = val, _条件根据图片委托7),
        new 条件配置项(7, "条件8", () => _条件8, val => _条件8 = val, _条件根据图片委托8),
        new 条件配置项(8, "条件9", () => _条件9, val => _条件9 = val, _条件根据图片委托9),
        new 条件配置项(9, "条件z", () => _条件z, val => _条件z = val, _条件根据图片委托z),
        new 条件配置项(10, "条件x", () => _条件x, val => _条件x = val, _条件根据图片委托x),
        new 条件配置项(11, "条件c", () => _条件c, val => _条件c = val, _条件根据图片委托c),
        new 条件配置项(12, "条件v", () => _条件v, val => _条件v = val, _条件根据图片委托v),
        new 条件配置项(13, "条件b", () => _条件b, val => _条件b = val, _条件根据图片委托b),
        new 条件配置项(14, "条件space", () => _条件space, val => _条件space = val, _条件根据图片委托space)
    };

            const int 主循环间隔 = 1;
            const int 中断检查间隔 = 1;

            while (_总循环条件)
            {
                try
                {
                    if (_中断条件)
                    {
                        await Task.Delay(中断检查间隔).ConfigureAwait(true);
                        continue;
                    }

                    if (_循环内获取图片 is null)
                    {
                        await Task.Delay(主循环间隔).ConfigureAwait(true);
                        continue;
                    }

                    // 获取图片数据
                    _循环内获取图片();

                    // 处理命石相关逻辑
                    if (_命石根据图片委托 is not null)
                    {
                        await _命石根据图片委托(GlobalScreenCapture.GetCurrentHandle()).ConfigureAwait(true);
                    }

                    // 获取技能颜色信息
                    Skill.DOTA2获取所有释放技能前颜色(GlobalScreenCapture.GetCurrentHandle());

                    // 核心逻辑：处理条件更新，包括运行期外部修改
                    await 处理条件更新_带外部变化检测(条件配置).ConfigureAwait(true);

                    // 处理假腿切换逻辑
                    await 处理假腿切换().ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    Common.Main_Logger.Error($"主循环异常: {ex.Message}");
                }

                await Task.Delay(主循环间隔).ConfigureAwait(true);
            }
        }

        // 重新设计的条件更新处理 - 支持运行期外部修改检测
        private static async Task 处理条件更新_带外部变化检测(条件配置项[] 条件配置)
        {
            // 1. 获取初始条件状态
            bool[] 条件数组 = [.. 条件配置.Select(配置 => 配置.获取条件())];
            bool[] 原始数组 = (bool[])条件数组.Clone();

            // 2. 启动初始的并行委托执行
            var 更新任务 = 更新条件数组(条件数组, 条件配置);

            // 3. 关键逻辑：在委托执行期间，持续检查外部条件变化
            var 已处理的新条件 = new HashSet<int>(); // 记录已处理的新true条件

            while (!更新任务.IsCompleted)
            {
                // 检查是否有新的条件被外部设置为true
                for (int i = 0; i < 条件配置.Length; i++)
                {
                    bool 当前条件 = 条件配置[i].获取条件();

                    // 如果这个条件现在是true，且原始状态是false，且还没有处理过
                    if (当前条件 && !原始数组[i] && !已处理的新条件.Contains(i))
                    {
                        // 标记为已处理
                        已处理的新条件.Add(i);

                        // 为这个新的true条件启动委托执行
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                bool 结果 = await 检查条件并执行委托(true, 条件配置[i].委托).ConfigureAwait(false);
                                if (当前条件) // 再次检查条件是否仍为true（可能被其他委托修改）
                                {
                                    条件配置[i].设置条件(结果);
                                }
                            }
                            catch (Exception ex)
                            {
                                Common.Main_Logger.Error($"新条件委托执行失败 [{条件配置[i].名称}]: {ex.Message}");
                            }
                        });
                    }
                }

                await Task.Delay(1).ConfigureAwait(true); // 避免CPU占用过高
            }

            // 4. 等待初始任务完成
            await 更新任务.ConfigureAwait(true);

            // 5. 应用初始条件的检测结果
            for (int i = 0; i < 原始数组.Length; i++)
            {
                if (原始数组[i])  // 只有原始条件为true时才更新
                {
                    条件配置[i].设置条件(条件数组[i]);
                }
            }
        }

        // 并行执行初始条件的委托检测
        private static async Task 更新条件数组(bool[] 条件数组, 条件配置项[] 条件配置)
        {
            var 检测任务 = 条件配置.Select(async (配置, index) =>
            {
                try
                {
                    bool 新条件 = await 检查条件并执行委托(条件数组[index], 配置.委托).ConfigureAwait(false);
                    条件数组[index] = 新条件;
                }
                catch (Exception ex)
                {
                    Common.Main_Logger.Error($"条件检测失败 [{配置.名称}]: {ex.Message}");
                }
            });

            await Task.WhenAll(检测任务).ConfigureAwait(false);
        }

        // 保持原有的检查条件并执行委托逻辑
        private static async Task<bool> 检查条件并执行委托(bool 条件, ConditionDelegateBitmap 委托)
        {
            return 条件 && 委托 is not null ? await 委托(GlobalScreenCapture.GetCurrentHandle()).ConfigureAwait(true) : 条件;
        }

        private static async Task 处理假腿切换()
        {
            if (Item._条件保持假腿 && Item._条件开启切假腿 && Item._需要切假腿)
            {
                await 切假腿处理(Item._条件假腿敏捷 ? "敏捷" : "力量").ConfigureAwait(true);
            }
        }

        private static async Task 切假腿处理(string 假腿类型)
        {
            if (Item._切假腿中)
            {
                return;
            }

            await Task.Run(async () =>
            {
                ImageHandle 句柄 = 假腿类型 switch
                {
                    "敏捷" => Dota2_Pictrue.物品.假腿_敏捷腿,
                    "力量" => Dota2_Pictrue.物品.假腿_力量腿,
                    _ => throw new NotImplementedException()
                };

                if (ImageFinder.FindImageInRegionBool(句柄, GlobalScreenCapture.GetCurrentHandle(), Item.获取物品范围(Skill._技能数量)))
                {
                    return;
                }

                Item._切假腿中 = true;
                _ = await Item.切假腿类型(假腿类型).ConfigureAwait(true);
                await Task.Run(() =>
                {
                    Common.Delay(100);
                    Item._切假腿中 = false;
                    Item._需要切假腿 = false; // 切假腿完毕，无需再切
                }).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        private static async Task 状态初始化()
        {
#if Silt
            _条件根据图片委托8 ??= Silt.Main.有书吃书;
            _条件8 = true;
#endif
            _循环内获取图片 ??= 获取图片_2;
            await 一般程序循环().ConfigureAwait(true);
        }

        #region 取消所有功能

        public static void 取消所有功能()
        {
            _总循环条件 = false;
            _循环内获取图片 = null;
            _中断条件 = false;

            // _丢装备条件 = false;

            _条件1 = false;
            _条件2 = false;
            _条件3 = false;
            _条件4 = false;
            _条件5 = false;
            _条件6 = false;
            _条件7 = false;
            _条件8 = false;
            _条件9 = false;
            _条件根据图片委托1 = null;
            _条件根据图片委托2 = null;
            _条件根据图片委托3 = null;
            _条件根据图片委托4 = null;
            _条件根据图片委托5 = null;
            _条件根据图片委托6 = null;
            _条件根据图片委托7 = null;
            _条件根据图片委托8 = null;
            _条件根据图片委托9 = null;

            _ = Item.重置耗蓝物品委托和条件();

            _命石选择 = 0;
            _命石根据图片委托 = null;

            Item._条件开启切假腿 = false;
            Item._条件保持假腿 = false;
            Item._条件假腿敏捷 = false;
            Item._切假腿中 = false;
            Item._需要切假腿 = false;

            Item._是否魔晶 = false;
            Item._是否神杖 = false;

            设置全局步骤(0);
            设置全局步骤q(0);
            设置全局步骤w(0);
            设置全局步骤e(0);
            设置全局步骤r(0);
            设置全局步骤d(0);
            设置全局步骤f(0);

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

            _攻击速度 = 100;
            _基础攻击前摇 = 0.3;
            _基础攻击间隔 = 1.7;
            _实际出手时间 = 0;
            _停止走A = 0;
            _开启走A = false;
            _实际攻击前摇 = 0;
            _实际攻击间隔 = 0;

            _指定地点p = new Point(0, 0);
            _指定地点q = new Point(0, 0);
            _指定地点w = new Point(0, 0);
            _指定地点e = new Point(0, 0);
            _指定地点r = new Point(0, 0);
            _指定地点d = new Point(0, 0);
            _指定地点f = new Point(0, 0);

            Skill.重置所有技能判断();

            // 重置切假腿配置
            Item._切假腿配置 = new Item.技能切假腿配置();
            Item._假腿按键 = Keys.Escape;


        }

        #endregion

        #region 获取图片

        /// <summary>
        ///     初始化全屏捕捉
        ///     <para>图像信息赋值给GlobalScreenCapture.GetCurrentHandle()</para>
        /// </summary>
        /// <param name="rectangle">截图区域,自动调整偏移量</param>
        private static void 执行屏幕捕捉捕捉(in Rectangle rectangle)
        {
            GlobalScreenCapture.CaptureScreen(rectangle);
        }

        /// <summary>
        ///     获取时间为6.92ms，占程序的大头
        /// </summary>
        /// <returns></returns>
        private static bool 获取图片_1()
        {
            // 最新通过DesktopDuplicator 实现更快速的全屏捕获
            // 671 856 760 217 基本所有技能状态物品 .net9 6-7ms 延迟。
            // 671 727 760 346 所有技能状态物品加上施法状态 .net9 6-7ms fps142+
            // 初始化全局图像和数组
            //_全局图像 ??= new Bitmap(截图模式1W, 截图模式1H);

            /* 
            if (GlobalScreenCapture.GetCurrentHandle() == null || GlobalScreenCapture.GetCurrentHandle().图片尺寸.Width + GlobalScreenCapture.GetCurrentHandle().图片尺寸.Height == 0)
            {
                GlobalScreenCapture.GetCurrentHandle() = new ImageHandle(new byte[截图模式1W * 截图模式1H * 4], new Size(截图模式1W, 截图模式1H));
            }

            // 更新字节数组
            CaptureScreen_固定数组(GlobalScreenCapture.GetCurrentHandle(), 截图模式1X, 截图模式1Y);

            //// 捕获屏幕
            //CaptureScreen_固定大小(ref _全局图像, 截图模式1X, 截图模式1Y);

            //// 获取位图字节数组
            //GetBitmapByte_固定数组(in _全局图像, ref GlobalScreenCapture.GetCurrentHandle());

            // 直接返回已完成的任务，减少异步开销
            return await Task.FromResult(true).ConfigureAwait(true);
            */

            执行屏幕捕捉捕捉(截图模式1Reg);

            return true;
        }

        public static bool 获取图片_2()
        {
            执行屏幕捕捉捕捉(截图模式2Reg);

            return true;

            /*
            // 优化之后16-17ms fps 58+
            //_全局图像 ??= new Bitmap(1920, 1080);

            //if (GlobalScreenCapture.GetCurrentHandle() == null || GlobalScreenCapture.GetCurrentHandle().图片尺寸.Width == 截图模式1W || GlobalScreenCapture.GetCurrentHandle().图片尺寸.Height == 截图模式1H)
            //{
            //    GlobalScreenCapture.GetCurrentHandle() = new ImageHandle(new byte[1920 * 1080 * 4], new Size(1920, 1080));
            //}

            //// 更新字节数组
            //CaptureScreen_固定数组(GlobalScreenCapture.GetCurrentHandle(), 0, 0);

            ////// 捕获屏幕
            ////CaptureScreen_固定大小(ref _全局图像, 0, 0);

            ////// 获取位图字节数组
            ////GetBitmapByte_固定数组(in _全局图像, ref GlobalScreenCapture.GetCurrentHandle());

            //// 直接返回已完成的任务，减少异步开销
            //return await Task.FromResult(true).ConfigureAwait(true);
            */
        }

        // 清理资源
        private static void CleanupImageSystem()
        {
            // 释放所有缓存的图像
            foreach (var handle in _缓存图像句柄.Values)
            {
                ImageManager.ReleaseImage(handle);
            }
            _缓存图像句柄.Clear();

            // 释放全局图像
            if (GlobalScreenCapture.GetCurrentHandle().IsValid)
            {
                ImageManager.ReleaseImage(GlobalScreenCapture.GetCurrentHandle());
            }

            // 执行完整清理
            ImageSystemMonitor.PerformFullCleanup();
        }

        #endregion

#endregion

#endregion
    }
}

#endif