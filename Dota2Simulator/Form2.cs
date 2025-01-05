// #define 检测延时
// #define DEBUG

#define DOTA2
// #define HF2
// #define LOL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Collections.Pooled;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.PictureProcessing;
using Dota2Simulator.TTS;
using NLog;
using static Dota2Simulator.PictureProcessing.PictureProcessing;
using static Dota2Simulator.SetWindowTop;
using static System.Threading.Tasks.Task;
using Clipboard = System.Windows.Forms.Clipboard;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Keys = System.Windows.Forms.Keys;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Dota2Simulator
{
    internal partial class Form2 : Form
    {
        // 创建一个静态的 Logger 实例
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

#if LOL
            /// LOL 最高画质 1920 * 1080
            private const int 截图模式1X = 647;
            private const int 截图模式1Y = 941;
            private const int 截图模式1W = 642;
            private const int 截图模式1H = 130;
            private const int 等待延迟 = 6;

            // 根据截图模式不同，用于判断技能 和 物品的具体x, y值随之变化
            private const int 坐标偏移x = 647;
            private const int 坐标偏移y = 941;

            // 技能下部颜色，用于检测是否CD
            private static Color 技能CD颜色 = Color.FromArgb(112, 85, 38);

#endif

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
            #region 打字时屏蔽功能 (未使用)

            #endregion

            #region 记录时间 (未使用)

            #endregion

            #region 隐藏界面

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

            switch (tb_name.Text.Trim())
            {
                #region Dota2

#if DOTA2

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
                            _切假腿配置.修改配置(Keys.E, false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                Tts.Speak(_全局模式 == 1 ? "跳刀决斗" : "直接决斗");
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
                            _切假腿配置.修改配置(Keys.W, false);
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                if (_命石选择 == 1)
                                {
                                    _切假腿配置.修改配置(Keys.W, true);
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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                        _切假腿配置.修改配置(Keys.E, false);
                        await 状态初始化().ConfigureAwait(false);
                    }

                    await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.E, false);
                            _切假腿配置.修改配置(Keys.R, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                    DOTA2释放CD就绪技能(Keys.E, in _全局数组);
                                }

                                _条件3 = true;
                                break;
                            case Keys.D2:
                                _指定地点d = MousePosition;
                                Tts.Speak("确定指定地点");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                Tts.Speak(_全局模式q == 1 ? "勾接咬" : "勾接平A");
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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                根据图片使用物品(物品_魂戒_数组);
                                _条件1 = true;
                                break;
                            case Keys.W:
                                根据图片使用物品(物品_魂戒_数组);
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件4 = true;
                                break;
                            case Keys.R:
                                根据图片使用物品(物品_魂戒_数组);
                                _条件3 = true;
                                break;
                            case Keys.D2:
                                _全局模式q = 1 - _全局模式q;
                                Tts.Speak(_全局模式q == 1 ? "吼接刃甲" : "吼不接刃甲");
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
                            // _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.W, false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.E, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                if (_是否魔晶)
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                if (_是否神杖)
                                {
                                    _切假腿配置.修改配置(Keys.D, true);
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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                根据图片使用物品(物品_紫苑_数组);
                                根据图片使用物品(物品_血棘_数组);
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
                                Tts.Speak(_全局模式q == 1 ? "混乱之箭接拉" : "混乱之箭接A");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                Tts.Speak(_全局模式q == 1 ? "矛接大招" : "矛不接大招");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _条件根据图片委托1 ??= 鼻涕去后摇;
                            _条件根据图片委托2 ??= 针刺循环;
                            _条件根据图片委托3 ??= 毛团去后摇;
                            _条件根据图片委托4 ??= 钢毛后背去后摇;
                            _条件根据图片委托5 ??= 扫射切回假腿;
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.D, true);
                                }

                                if (_是否神杖)
                                {
                                    _切假腿配置.修改配置(Keys.E, true);
                                }

                                break;
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.D:
                                if (_是否魔晶)
                                {
                                    _条件3 = true;
                                }

                                break;
                            case Keys.E:
                                if (_是否神杖)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.W:
                                _条件5 = true;
                                break;
                            case Keys.D2:
                                if (!_条件2)
                                {
                                    _条件2 = true;
                                }

                                _循环条件1 = !_循环条件1;
                                if (_循环条件1)
                                {
                                    Tts.Speak("循环扫射");
                                }

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
                            _切假腿配置.修改配置(Keys.E, false);
                            _基础攻击前摇 = 0.5;
                            _基础攻击间隔 = 1.6;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.D, true);
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
                                if (_是否魔晶)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.D2:
                                _全局模式w = 1 - _全局模式w;
                                Tts.Speak("W接" + (_全局模式w == 1 ? "火球" : "喷火"));
                                break;
                            case Keys.D3:
                                _全局模式d = 1 - _全局模式d;
                                Tts.Speak("火球" + (_全局模式d == 1 ? "接" : "不接") + "喷火");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.D, true, "敏捷");
                                }

                                if (_是否神杖)
                                {
                                    _切假腿配置.修改配置(Keys.F, true);
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
                                if (_是否魔晶)
                                {
                                    _条件5 = true;
                                }

                                break;
                            case Keys.F:
                                if (_是否神杖)
                                {
                                    _条件6 = true;
                                }

                                break;
                            case Keys.D2:
                                _全局模式q = 1 - _全局模式q;
                                Tts.Speak(_全局模式q == 1 ? "无脑接道具" : "手动道具");
                                break;
                            case Keys.D3:
                                if (_是否魔晶)
                                {
                                    _全局模式f = 1 - _全局模式f;
                                    Tts.Speak(_全局模式f == 1 ? "炽烈火雨隐身" : "炽烈火雨不隐身");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                _全局模式e = ColorAEqualColorB(获取指定位置颜色(738, 957, in _全局数组),
                                    Color.FromArgb(246, 178, 60), 0) || ColorAEqualColorB(
                                    获取指定位置颜色(722, 957, in _全局数组),
                                    Color.FromArgb(246, 178, 60), 0)
                                    ? 1
                                    : 0;
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.F, true);
                                }

                                break;
                            case Keys.D:
                                if (_条件开启切假腿)
                                {
                                    _全局模式 = 1 - _全局模式;
                                    switch (_全局模式)
                                    {
                                        case 0:
                                            await 技能释放前切假腿("智力").ConfigureAwait(true);
                                            Tts.Speak("开启冰箭");
                                            break;
                                        default:
                                            要求保持假腿();
                                            Tts.Speak("关闭冰箭");
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
                                if (_是否魔晶)
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
                            _切假腿配置.修改配置(Keys.Q, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                // todo 幻刺可以通过命石判断..但另一边基本没人选
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.D, true);
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
                                if (_是否魔晶)
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
                            _切假腿配置.修改配置(Keys.W, true, "力量");
                            _切假腿配置.修改配置(Keys.E, false);
                            _切假腿配置.修改配置(Keys.R, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                if (!DOTA2判断状态技能是否启动(Keys.E, in _全局数组))
                                {
                                    SimKeyBoard.KeyPress(Keys.E);
                                }

                                _条件1 = true;
                                break;
                            case Keys.W:
                                if (!DOTA2判断状态技能是否启动(Keys.E, in _全局数组))
                                {
                                    SimKeyBoard.KeyPress(Keys.E);
                                }

                                _条件2 = true;
                                break;
                            case Keys.R:
                                if (!DOTA2判断状态技能是否启动(Keys.E, in _全局数组))
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
                            _切假腿配置.修改配置(Keys.W, false);
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.E, true);
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
                                Tts.Speak(_全局模式f == 1 ? "如影随形分身" : "关闭随形分身");
                                break;
                        }

                        break;
                    }

                #endregion

                #region 影魔

                // todo 待修改
                case "影魔":
                    //if (e.KeyValue == Keys.Num)
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
                            _切假腿配置.修改配置(Keys.W, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否神杖)
                                {
                                    _切假腿配置.修改配置(Keys.F, true);
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
                                if (_是否魔晶)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.F:
                                if (_是否神杖)
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
                            _切假腿配置.修改配置(Keys.Q, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.D, true);
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
                                if (_是否魔晶)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.D2:
                                _全局模式w = 1;
                                Tts.Speak("闪烁分身晕锤一次");
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
                            能量转移被动计数 = 0;
                            _基础攻击间隔 = 1.7;
                            _基础攻击前摇 = 0.5;
                            lb_状态抗性.Text = "转移层数";
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.D, true);
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
                                if (_是否魔晶)
                                {
                                    _条件3 = true;
                                }

                                break;
                            case Keys.D2:
                                // 径直移动键位
                                SimKeyBoard.KeyDown(Keys.L);
                                Delay(等待延迟);
                                SimKeyBoard.MouseRightClick();
                                Delay(等待延迟);
                                SimKeyBoard.KeyUp(Keys.L);
                                // 基本上180°310  90°170 75°135 转身定点，配合A杖效果极佳
                                Delay(110);
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.F, true);
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
                                if (_是否魔晶)
                                {
                                    _条件4 = true;
                                }

                                break;
                            case Keys.D2:
                                _全局模式w = 1 - _全局模式w;
                                Tts.Speak(_全局模式w == 0 ? "种树接平A" : "种树接捆");
                                break;
                            case Keys.D3:
                                _全局模式e = 1 - _全局模式e;
                                Tts.Speak(_全局模式e == 0 ? "捆接平A" : "捆接种树");
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
                            _切假腿配置.修改配置(Keys.D, true);
                            _切假腿配置.修改配置(Keys.R, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                _条件1 = true;
                                await Run(() =>
                                {
                                    Delay(330);
                                    要求保持假腿();
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
                                Tts.Speak(_全局模式w == 0 ? "不接捆" : "接捆");
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
                            _切假腿配置.修改配置(Keys.E, false);
                            _切假腿配置.修改配置(Keys.R, false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.W, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        _切假腿配置.修改配置(Keys.D, _是否魔晶);
                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _条件根据图片委托1 ??= 火箭弹幕敏捷;
                            _条件根据图片委托2 ??= 追踪导弹敏捷;
                            _条件根据图片委托3 ??= 高射火炮敏捷;
                            _条件根据图片委托4 ??= 召唤飞弹敏捷;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                #region 美杜莎

                case "美杜莎":
                    {
                        if (!_总循环条件)
                        {
                            _总循环条件 = true;
                            _条件根据图片委托1 ??= 秘术异蛇去后摇;
                            _条件根据图片委托2 ??= 罗网剑阵去后摇;
                            _条件根据图片委托3 ??= 石化凝视去后摇;
                            _切假腿配置.修改配置(Keys.Q, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.W, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                _循环条件1 = _条件3;
                                Tts.Speak(_循环条件1 ? "循环标记" : "不循环标记");
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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.Q, false);
                            _切假腿配置.修改配置(Keys.W, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            case Keys.D2:
                                _条件4 = !_条件4;
                                _循环条件1 = _条件4;
                                Tts.Speak(_循环条件1 ? "开启无限跳跃" : "关闭无限跳跃");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                _循环条件1 = _条件4;
                                Tts.Speak(_循环条件1 ? "+开启循环查克拉" : "关闭循环查克拉");
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
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                _循环条件1 = _条件1;
                                Tts.Speak(_循环条件1 ? "循环鹰隼" : "不循环鹰隼");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                Tts.Speak(_全局模式d == 1 ? "传" : "不传");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                根据图片使用物品(物品_希瓦_数组);
                                根据图片使用物品(物品_纷争_数组);
                                _条件4 = true;
                                break;
                            case Keys.D2:
                                _全局模式r = 1 - _全局模式r;
                                Tts.Speak(_全局模式r == 1 ? "吸取接衰老" : "吸取不接衰老");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                根据图片使用物品(物品_纷争_数组);
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
                            label1.Text = "F";
                            await Run(刷新接凋零黑洞).ConfigureAwait(true);
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
                                await Run(残影接平A).ConfigureAwait(true);
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                根据图片使用物品(物品_纷争_数组);
                                _条件1 = true;
                                break;
                            case Keys.W:
                                _条件2 = true;
                                break;
                            case Keys.E:
                                初始化全局时间(ref _全局时间e);
                                break;
                            case Keys.R:
                                初始化全局时间(ref _全局时间r);
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
                                if (RegPicture(物品_祭礼长袍_数组, _全局数组))
                                {
                                    _状态抗性倍数 *= 1.1;
                                }

                                if (RegPicture(物品_永恒遗物_数组, _全局数组))
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
                                        Tts.Speak("羊拉");
                                        break;
                                    case 1:
                                        _全局模式w = 2;
                                        Tts.Speak("羊电");
                                        break;
                                    case 2:
                                        _全局模式w = 3;
                                        Tts.Speak("羊电拉");
                                        break;
                                    case 3:
                                        _全局模式w = 4;
                                        Tts.Speak("羊电大拉");
                                        break;
                                    case 4:
                                        _全局模式w = 0;
                                        Tts.Speak("羊接平A");
                                        break;
                                }

                                break;
                            case Keys.D2:
                                _条件4 = true;
                                break;
                            case Keys.D3:
                                _全局模式q = 1 - _全局模式q;
                                Tts.Speak(_全局模式q == 0 ? "羊" : "电羊");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F:
                                if (_是否魔晶)
                                {
                                    _条件4 = true;
                                }
                                break;
                            case Keys.D4:
                                _循环条件1 = !_循环条件1;
                                _条件5 = true;
                                Tts.Speak(_循环条件1 ? "续暗影" : "不续暗影");
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
                                根据图片使用物品(物品_纷争_数组);
                                _条件3 = true;
                                break;
                            case Keys.D2:
                                _全局模式e = 1 - _全局模式e;
                                Tts.Speak(_全局模式e == 0 ? "起飞后不接3连炸弹" : "起飞后接3连炸弹");
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                await 大招前纷争(_全局数组).ConfigureAwait(true);
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
                                Tts.Speak("开启刷新秒人");
                                break;
                            case Keys.D4:
                                _条件5 = false;
                                Tts.Speak("关闭刷新秒人");
                                break;
                            case Keys.D5 when !_条件6:
                                _条件6 = true;
                                Tts.Speak("开启羊接吸");
                                break;
                            case Keys.D5:
                                _条件6 = false;
                                Tts.Speak("开启羊接A");
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
                            case Keys.R:
                                初始化全局时间(ref _全局时间r);
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            _切假腿配置.修改配置(Keys.E, false);
                            _切假腿配置.修改配置(Keys.F, true);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否神杖)
                                {
                                    _切假腿配置.修改配置(Keys.D, true);
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
                                _条件4 = true;
                                break;
                            case Keys.F:
                                _条件5 = true;
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
                                Tts.Speak(_全局模式w == 0 ? "引燃接轰爆" : "引燃不接轰爆");
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
                                if (ColorAEqualColorB(获取指定位置颜色(971, 1008, in _全局数组), 技能点颜色, 0))
                                {
                                    _全局时间 = 7000;
                                }
                                else if (ColorAEqualColorB(获取指定位置颜色(964, 1008, in _全局数组), 技能点颜色, 0))
                                {
                                    _全局时间 = 6000;
                                }
                                else if (ColorAEqualColorB(获取指定位置颜色(947, 1008, in _全局数组), 技能点颜色, 0))
                                {
                                    _全局时间 = 5000;
                                }
                                else if (ColorAEqualColorB(获取指定位置颜色(935, 1008, in _全局数组), 技能点颜色, 0))
                                {
                                    _全局时间 = 4000;
                                }

                                _条件3 = true;
                                break;
                            case Keys.D2:
                                _全局模式e = 1 - _全局模式e;
                                Tts.Speak(_全局模式e == 0 ? "睡不接陨星锤" : "睡接陨星锤");
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
                            _条件根据图片委托5 ??= 无限死亡脉冲;
                            _切假腿配置.修改配置(Keys.E, false);
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.F1:
                                if (_是否魔晶)
                                {
                                    _切假腿配置.修改配置(Keys.F, true);
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
                                if (_是否魔晶)
                                {
                                    _条件4 = true;
                                }
                                break;
                            case Keys.D6:
                                _循环条件1 = !_循环条件1;
                                _条件5 = true;
                                Tts.Speak(_循环条件1 ? "续脉冲" : "不续脉冲");
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
                            await 状态初始化().ConfigureAwait(false);
                            _切假腿配置.修改配置(Keys.W, false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                                await Run(跳拱指定地点).ConfigureAwait(false);
                                break;
                            case Keys.D2:
                                await Run(指定地点).ConfigureAwait(false);
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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

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
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                {
                                    if (_是否神杖)
                                    {
                                        break;
                                    }

                                    _条件2 = true;
                                    break;
                                }
                            case Keys.D2:
                                {
                                    if (_循环条件1)
                                    {
                                        _条件1 = false;
                                        _循环条件1 = false;
                                        Tts.Speak("关闭续勋章");
                                    }
                                    else
                                    {
                                        _条件1 = true;
                                        _循环条件1 = true;
                                        Tts.Speak("开启续勋章");
                                    }

                                    break;
                                }
                            case Keys.D3:
                                {
                                    if (_选择队友头像 < 9)
                                    {
                                        _选择队友头像 += 1;
                                    }
                                    else
                                    {
                                        _选择队友头像 = 0;
                                    }

                                    Tts.Speak(string.Concat("选择第", _选择队友头像 + 1, "个人"));
                                    break;
                                }
                            case Keys.D4:
                                {
                                    if (_循环条件2)
                                    {
                                        _条件3 = false;
                                        _循环条件2 = false;
                                        Tts.Speak("关闭续过载");
                                    }
                                    else
                                    {
                                        _条件3 = true;
                                        _循环条件2 = true;
                                        Tts.Speak("开启续过载");
                                    }

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

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.W:
                                {
                                    if (_是否神杖)
                                    {
                                        break;
                                    }

                                    _条件2 = true;
                                    break;
                                }
                            case Keys.D2:
                                {
                                    if (_循环条件1)
                                    {
                                        _条件1 = false;
                                        _循环条件1 = false;
                                        Tts.Speak("关闭续勋章");
                                    }
                                    else
                                    {
                                        _条件1 = true;
                                        _循环条件1 = true;
                                        Tts.Speak("开启续勋章");
                                    }

                                    break;
                                }
                            case Keys.D3:
                                {
                                    if (_选择队友头像 < 9)
                                    {
                                        _选择队友头像 += 1;
                                    }
                                    else
                                    {
                                        _选择队友头像 = 0;
                                    }

                                    Tts.Speak(string.Concat("选择第", _选择队友头像 + 1, "个人"));
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
                            _条件根据图片委托1 ??= 穿刺去后摇;
                            _条件根据图片委托2 ??= 神智爆裂去后摇;
                            _条件根据图片委托3 ??= 尖刺外壳去后摇;
                            _条件根据图片委托4 ??= 复仇接穿刺;
                            await 状态初始化().ConfigureAwait(false);
                        }

                        await 根据按键判断技能释放前通用逻辑(e).ConfigureAwait(true);

                        switch (e.KeyCode)
                        {
                            case Keys.Q:
                                _条件1 = true;
                                break;
                            case Keys.W:
                                Delay(33 * 根据图片使用物品(物品_虚灵之刃_数组));
                                Delay(33 * (根据图片使用物品(物品_红杖_数组) +
                                            根据图片使用物品(物品_红杖2_数组) +
                                            根据图片使用物品(物品_红杖3_数组) +
                                            根据图片使用物品(物品_红杖4_数组) +
                                            根据图片使用物品(物品_红杖5_数组)));
                                _条件2 = true;
                                break;
                            case Keys.E:
                                _条件3 = true;
                                break;
                            case Keys.R:
                                设置全局步骤r(0);
                                // _条件4 = true;
                                break;
                        }

                        break;
                    }

                #endregion

                #endregion

#endif

                #endregion

                #region LOL

#if LOL
                    #region 魔腾

                    case "魔腾":
                        {
                            if (!_总循环条件)
                            {
                                _总循环条件 = true;
                                _条件根据图片委托1 ??= 梦魇之径接平A;
                                _条件根据图片委托2 ??= 无言恐惧接梦魇之径;
                                _条件根据图片委托3 ??= 鬼影重重接无言恐惧;
                                _条件根据图片委托4 ??= 重复释放无言恐惧;
                                await 无物品状态初始化().ConfigureAwait(false);
                            }


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

                    #region 男枪

                    case "男枪":
                        {
                            if (!_总循环条件)
                            {
                                _总循环条件 = true;
                                _条件根据图片委托1 ??= 穷途末路接平A;
                                _条件根据图片委托2 ??= 烟雾弹接平A;
                                _条件根据图片委托3 ??= 快速拔枪接平A;
                                _条件根据图片委托4 ??= 终极爆弹接平A;
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
                                    _条件2 = true;
                                    break;
                                case Keys.R:
                                    _条件3 = true;
                                    break;
                            }
                            break;
                        }

                    #endregion

#endif

                #endregion

                #region HF2

#if HF2
                    case "hf2":
                        {
                            switch (e.KeyCode)
                            {
                                case Keys.NumPad1:
                                    Run(HF2_补给);
                                    break;
                                case Keys.NumPad2:
                                    Run(HF2_救援);
                                    break;
                                case Keys.NumPad3:
                                    Run(HF2_飞鹰_空袭);
                                    break;
                                case Keys.NumPad5:
                                    Run(HF2_飞鹰_110);
                                    break;
                                case Keys.NumPad6:
                                    Run(HF2_飞鹰_重填装);
                                    break;
                            }

                            break;
                        }
#endif

                #endregion

                #region 其他

                case "测试":
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.D1:
                                _ = Run(测试其他功能).ConfigureAwait(true);
                                break;
                            case Keys.D2:
                                _ = Run(() => { 捕捉颜色().Start(); }).ConfigureAwait(false);
                                Delay(100);
                                Dictionary<char, Keys> keyMapping = new()
                            {
                                { 'q', Keys.Q },
                                { 'w', Keys.W },
                                { 'e', Keys.E },
                                { 'r', Keys.R },
                                { 'd', Keys.D },
                                { 'f', Keys.F }
                            };

                                string text = tb_阵营.Text.ToLower(CultureInfo.CurrentCulture); // 转换为小写，确保匹配时忽略大小写

                                foreach (KeyValuePair<char, Keys> kvp in keyMapping)
                                {
                                    if (text.Contains(kvp.Key))
                                    {
                                        SimKeyBoard.KeyPress(kvp.Value);
                                        break; // 如果找到匹配项，退出循环
                                    }
                                }

                                break;
                            case Keys.D3:
                                await Run(() => 测试方法(953, 764)).ConfigureAwait(false);
                                break;
                            case Keys.D4:
                                await Run(async () =>
                                {
                                    _ = await 获取图片_1().ConfigureAwait(true);
                                    Tts.Speak($"技能数：{获取当前技能数量(in _全局数组)}");
                                }).ConfigureAwait(false);
                                break;
                        }

                        break;
                    }

                    #endregion
            }
        }

        /// <summary>
        ///     100次运行，6ms...
        ///     还要什么自行车，全部添加判断
        /// </summary>
        /// <param name="数组"></param>
        /// <returns></returns>
        public static async Task<bool> 设置当前技能数量()
        {
            int i = 获取当前技能数量(in _全局数组);
            if (i != 0)
            {
                // 技能数量改变后，技能判断色全部重置
                // 只有两种情况需要重置，一种是还没学，到学习了
                // 另一种是技能改变了
                if (_技能数量 != i)
                {
                    _技能数量 = i;

                    // 重置所有技能判断
                    重置所有技能判断();

                    // 获取当前技能颜色
                    _ = DOTA2获取所有释放技能前颜色(in _全局数组);
                }
            }

            return await FromResult(i != 0).ConfigureAwait(true);
        }

        private static 技能切假腿配置 _切假腿配置 = new();
        private static Keys _假腿按键 = Keys.Escape;

        internal class 技能切假腿配置
        {
            public 技能切假腿配置()
            {
                切假腿配置 = new Dictionary<Keys, (bool, string)>
                {
                    { Keys.Q, (true, "智力") },
                    { Keys.W, (true, "智力") },
                    { Keys.E, (true, "智力") },
                    { Keys.R, (true, "智力") },
                    { Keys.D, (false, "智力") },
                    { Keys.F, (false, "智力") },
                    { Keys.Z, (false, "智力") },
                    { Keys.X, (false, "智力") },
                    { Keys.C, (false, "智力") },
                    { Keys.V, (false, "智力") },
                    { Keys.B, (false, "智力") },
                    { Keys.Space, (false, "智力") }
                };
            }

            public Dictionary<Keys, (bool 是否激活, string 假腿类型)> 切假腿配置 { get; }

            public void 修改配置(Keys key, bool 是否激活, string 假腿类型 = "智力")
            {
                if (切假腿配置.ContainsKey(key))
                {
                    切假腿配置[key] = (是否激活, 假腿类型);
                }
                else
                {
                    切假腿配置.Add(key, (是否激活, 假腿类型));
                }
            }
        }

        private static Form _form;

        private static readonly Dictionary<Keys, Action> 按键匹配条件更新 = new()
        {
            { Keys.Z, () => _条件z = true },
            { Keys.X, () => _条件x = true },
            { Keys.C, () => _条件c = true },
            { Keys.V, () => _条件v = true },
            { Keys.B, () => _条件b = true },
            { Keys.Space, () => _条件space = true }
        };

        private static async Task 根据按键判断技能释放前通用逻辑(KeyEventArgs e)
        {
            _ = await 设置当前技能数量().ConfigureAwait(true);
            _存在假腿 = 获取当前假腿按键();
            _是否神杖 = 阿哈利姆神杖(in _全局数组);
            _是否魔晶 = 阿哈利姆魔晶(in _全局数组);

            switch (e.KeyCode)
            {
                case Keys.F1:
                    _ = 重置耗蓝物品委托和条件();
                    _ = 获取当前耗蓝物品并设置切假腿();

                    _ = 获取状态抗性();
                    _攻击速度 = 获取当前攻击速度();
                    // 保留4位小数，例如：0.5攻击前摇，169攻速，四舍五入成0.2959
                    _实际攻击前摇 = Convert.ToInt32(Math.Round(_基础攻击前摇 * 100 / _攻击速度, 4) * 1000) + 33; // 保证能A出来，添加服务器延迟
                    _实际攻击间隔 = Convert.ToInt32(Math.Round(_基础攻击间隔 * 100 / _攻击速度, 4) * 1000);
                    // Logger.Info($"_实际攻击前摇{_实际攻击前摇}");
                    // Logger.Info($"_实际攻击间隔{_实际攻击间隔}");
                    break;
                case Keys.Escape:
                    _中断条件 = !_中断条件;
                    Tts.Speak($"{(_中断条件 ? "中断" : "继续")}运行");
                    break;
                case Keys.NumPad7 when _存在假腿:
                    切换假腿状态();
                    break;
                case Keys.NumPad8 when _存在假腿:
                    切换保持假腿状态();
                    break;
                case Keys.NumPad9 when _存在假腿:
                    取消所有功能();
                    break;
                case var _ when e.KeyCode == _假腿按键:
                    return;
                default:
                    if (!_存在假腿)
                    {
                        return;
                    }

                    await 技能释放前切假腿(e, _切假腿配置).ConfigureAwait(true);
                    if (按键匹配条件更新.TryGetValue(e.KeyCode, out Action value))
                    {
                        value.Invoke();
                    }

                    break;
            }
            //else if (e.KeyCode == Keys.NumPad6)
            //{
            //    _开启走A = !_开启走A;
            //    Tts.Speak(_开启走A ? "开启走A" : "关闭走A");
            //}
            //else if (e.KeyCode == Keys.A)
            //{
            //    var 现在的时间 = 获取当前时间毫秒();
            //    if (现在的时间 - _实际出手时间 >= _实际攻击间隔 - _实际攻击前摇)
            //    {
            //        _实际出手时间 = 现在的时间 + _实际攻击前摇;
            //        Run(走A去等待后摇);
            //    }
            //}
            //else if (e.KeyCode == Keys.S)
            //{
            //    _停止走A = 1;
            //}
        }

        private static async Task 技能释放前切假腿(KeyEventArgs e, 技能切假腿配置 配置)
        {
            if (配置.切假腿配置.TryGetValue(e.KeyCode, out (bool 是否激活, string 假腿类型) 配置值) && 配置值.是否激活)
            {
                await 技能释放前切假腿(配置值.假腿类型).ConfigureAwait(true);
            }
        }

        private static void 要求保持假腿()
        {
            _条件保持假腿 = _条件开启切假腿;
            _需要切假腿 = true;
        }

        private static void 切换假腿状态()
        {
            _条件假腿敏捷 = !_条件假腿敏捷;
            要求保持假腿();
            Tts.Speak(_条件假腿敏捷 ? "切敏捷" : "切力量");
        }

        private static void 切换保持假腿状态()
        {
            if (!_条件保持假腿 && _条件开启切假腿)
            {
                要求保持假腿();
            }
            else
            {
                _条件开启切假腿 = !_条件开启切假腿;
                要求保持假腿();
            }

            Tts.Speak(_条件保持假腿 ? "保持假腿" : "不保持假腿");
        }

        #endregion

        #region 延时

        /// <summary>
        ///     精准延迟，并减少性能消耗
        /// </summary>
        /// <param name="delay">需要延迟的时间</param>
        /// <param name="time"></param>
        private static void Delay(int delay, long time = -1)
        {
            if (delay <= 0)
            {
                return;
            }

            time = time == -1 ? 获取当前时间毫秒() : time;
            long endTime = time + delay;
            SpinWait spinWait = new();

            while (true)
            {
                long currentTime = 获取当前时间毫秒();
                if (currentTime >= endTime)
                {
                    break;
                }

                long remainingTime = endTime - currentTime;

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
                    spinWait.SpinOnce(); // SpinWait for very short intervals
                }
            }
        }

        private static void Delay(long delay, long time = -1)
        {
            if (delay <= 0)
            {
                return;
            }

            time = time == -1 ? 获取当前时间毫秒() : time;
            long endTime = time + delay;
            SpinWait spinWait = new();

            while (true)
            {
                long currentTime = 获取当前时间毫秒();
                if (currentTime >= endTime)
                {
                    break;
                }

                long remainingTime = endTime - currentTime;

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
                    spinWait.SpinOnce(); // SpinWait for very short intervals
                }
            }
        }

        #endregion

        #region 更改名字取消功能

        private void Tb_name_TextChanged(object sender, EventArgs e)
        {
            取消所有功能();
        }

        #endregion

        #region 更改阵营

        private void tb_阵营_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _阵营_int = Convert.ToInt16(tb_阵营.Text.Trim());
            }
            catch
            {
            }
            finally
            {
                _阵营_int = 0;
            }
        }

        #endregion

        #region 按钮捕捉颜色

        private async void Button1_Click(object sender, EventArgs e)
        {
            //await Run(捕捉颜色);
            //await Run(HF2_补给);
            long a = 获取当前时间毫秒();
            string str = PaddleOcr.获取图片文字(399, 1153, 899, 417);
            _ = MessageBox.Show(str + "耗时:" + (获取当前时间毫秒() - a));

            await Run(() => { Delay(1); }).ConfigureAwait(true);
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
                using PooledList<string> list = [.. tb_delay.Text.Split(',')];
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
        ///     循环计数1
        /// </summary>
        private static bool _循环条件1;

        private static bool 获取循环条件1()
        {
            return _循环条件1;
        }

        private static bool 设置循环条件1(bool 条件)
        {
            return _循环条件1 = 条件;
        }

        /// <summary>
        ///     循环计数2
        /// </summary>
        private static bool _循环条件2;

        /// <summary>
        ///     全局图像
        /// </summary>
        private static Bitmap _全局图像;

        /// <summary>
        ///     _全局数组
        /// </summary>
        private static 字节数组包含长宽 _全局数组;

        /// <summary>
        ///     _全局数组
        /// </summary>
        private static 字节数组包含长宽 _全局假腿数组;

        /// dota 2 适配7.36 1920 * 1080 动态肖像 法线贴图 地面视差 主界面高画质 计算器渲染器 纹理、特效、阴影 中 渲染 70% 高级水面效果
        private const int 截图模式1X = 671;

        private const int 截图模式1Y = 727;
        private const int 截图模式1W = 760;
        private const int 截图模式1H = 346;
        private const int 等待延迟 = 33;

        // 根据截图模式不同，用于判断技能 和 物品的具体x, y值随之变化 涉及到 全局假腿，技能位置，使用物品位置，判断神杖、魔晶 中立道具
        private const int 坐标偏移x = 671;
        private const int 坐标偏移y = 727;

        /// <summary>
        ///     全局命石选择
        /// </summary>
        private static int _命石选择;

        /// <summary>
        ///     获取图片委托
        /// </summary>
        /// <returns></returns>
        private delegate Task<bool> GetBitmap();

        /// <summary>
        ///     获取图片委托
        /// </summary>
        private static GetBitmap _循环内获取图片;

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private delegate Task<bool> ConditionDelegateBitmap(字节数组包含长宽 数组);

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
        private static ConditionDelegateBitmap _条件根据图片委托9;

        /// <summary>
        ///     命石委托
        /// </summary>
        private static ConditionDelegateBitmap _命石根据图片委托;

        /// <summary>
        ///     条件9委托
        /// </summary>
        private static ConditionDelegateBitmap _条件根据图片委托z;

        /// <summary>
        ///     条件9委托
        /// </summary>
        private static ConditionDelegateBitmap _条件根据图片委托x;

        /// <summary>
        ///     条件9委托
        /// </summary>
        private static ConditionDelegateBitmap _条件根据图片委托c;

        /// <summary>
        ///     条件9委托
        /// </summary>
        private static ConditionDelegateBitmap _条件根据图片委托v;

        /// <summary>
        ///     条件9委托
        /// </summary>
        private static ConditionDelegateBitmap _条件根据图片委托b;

        /// <summary>
        ///     条件9委托
        /// </summary>
        private static ConditionDelegateBitmap _条件根据图片委托space;

        /// <summary>
        ///     中断条件布尔
        /// </summary>
        private static bool _中断条件;

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
        private static bool _条件9;

        /// <summary>
        ///     条件布尔
        /// </summary>
        private static bool _条件z;

        /// <summary>
        ///     条件布尔
        /// </summary>
        private static bool _条件x;

        /// <summary>
        ///     条件布尔
        /// </summary>
        private static bool _条件c;

        /// <summary>
        ///     条件布尔
        /// </summary>
        private static bool _条件v;

        /// <summary>
        ///     条件布尔
        /// </summary>
        private static bool _条件b;

        /// <summary>
        ///     条件布尔
        /// </summary>
        private static bool _条件space;

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
        ///     条件布尔，因为切换到别人身上，假腿类型可能不同，但实际上不需要切假腿
        /// </summary>
        private static bool _需要切假腿;

        /// <summary>
        ///     _技能数量
        /// </summary>
        private static int _技能数量 = 4;

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
        private static int 能量转移被动计数;

        /// <summary>
        ///     状态抗性
        /// </summary>
        private static double _状态抗性倍数;

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

        ///// <summary>
        /////     按键钩子，用于捕获按下的键
        ///// </summary>
        //private KeyEventHandler _myKeyEventHandeler; //按键钩子

        ///// <summary>
        /////     用于捕捉按键
        ///// </summary>
        //private IKeyboardMouseEvents _mGlobalHook = Hook.GlobalEvents();

        /// <summary>
        ///     用于捕捉按键
        /// </summary>
        private readonly HookUserActivity _hookUser = new();

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
        private static bool _是否神杖;

        /// <summary>
        ///     全局存在假腿
        /// </summary>
        private static bool _存在假腿;

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

        /// <summary>
        ///     仅用于快速激怒
        /// </summary>
        private static int _阵营_int;

        #endregion

        #endregion

        #region Dota2具体实现

#if DOTA2

        #region 通用

        #region 技能

        /// <summary>
        ///     技能释放完毕后处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为技能释放完那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为技能释放完那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>技能未释放、释放中返回真，释放完毕执行逻辑返回假</returns>
        private static async Task<bool> 主动技能释放后续(Keys skill, Action afterAction)
        {
            if (DOTA2对比释放技能前后颜色(skill, in _全局数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            _ = Run(afterAction).ConfigureAwait(true);
            return await FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     主动技能进入CD后处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为技能进入CD那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为技能进入CD那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>主动技能CD就绪返回真，进入CD执行逻辑后返回假</returns>
        private static async Task<bool> 主动技能进入CD后续(Keys skill, Action afterAction)
        {
            if (DOTA2判断技能是否CD(skill, in _全局数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            _ = Run(afterAction).ConfigureAwait(false);
            return await FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     主动技能CD完毕处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为技能转好CD那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为技能转好CD那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>主动技能CD未就绪返回真，CD就绪执行逻辑后返回假</returns>
        private static async Task<bool> 主动技能已就绪后续(Keys skill, Action afterAction)
        {
            if (!DOTA2判断技能是否CD(skill, in _全局数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            _ = Run(afterAction).ConfigureAwait(false);
            return await FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     法球技能进入CD处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为法球进入CD那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为法球进入CD那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>法球技能CD就绪返回真，进入CD执行逻辑后返回假</returns>
        private static async Task<bool> 法球技能进入CD后续(Keys skill, Action afterAction)
        {
            if (DOTA2判断法球技能是否CD(skill, in _全局数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            _ = Run(afterAction).ConfigureAwait(false);
            return await FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     状态技能启动后处理后续
        ///     <para>字节数组为副本</para>
        ///     <para>为状态技能启动那一刻的图片</para>
        /// </summary>
        /// <param name="skill">技能位置</param>
        /// <param name="数组">字节数组为副本，为状态技能启动那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>状态技能未启动返回真，启动后逻辑后返回假</returns>
        private static async Task<bool> 状态技能启动后续(Keys skill, Action afterAction)
        {
            if (!DOTA2判断状态技能是否启动(skill, _全局数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            _ = Run(afterAction).ConfigureAwait(false);
            return await FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     使用技能后通用后续
        ///     <para><paramref name="判断模式" /> 0 主动技能进入CD 1 释放技能有抬手</para>
        ///     <para> 10 进入CD仅切回假腿 11 释放技能仅切回假腿</para>
        /// </summary>
        /// <param name="技能键"></param>
        /// <param name="数组"></param>
        /// <param name="判断模式">0 无充能进入CD 1 释放技能有抬手</param>
        /// <param name="是否接A"></param>
        /// <returns></returns>
        private static async Task<bool> 使用技能后通用后续(Keys 技能键, int 判断模式, bool 是否接按键 = true, Keys 要接的按键 = Keys.A,
            int 判断成功后延时 = 0)
        {
            void 技能后续动作()
            {
                通用技能后续动作(是否接按键, 要接的按键, 判断成功后延时);
            }

            void 切回假腿()
            {
                后续切回假腿(是否接按键, 要接的按键, 判断成功后延时);
            }

            Func<Task<bool>> 使用技能 = 判断模式 switch
            {
                0 => () => 主动技能进入CD后续(技能键, 技能后续动作),
                1 => () => 主动技能释放后续(技能键, 技能后续动作),
                10 => () => 主动技能进入CD后续(技能键, 切回假腿),
                11 => () => 主动技能释放后续(技能键, 切回假腿),
                _ => throw new ArgumentException("无效的判断模式")
            };

            return await 使用技能().ConfigureAwait(true);
        }

        /// <summary>
        ///     释放技能后替换图标技能后续
        /// </summary>
        /// <param name="key"></param>
        /// <param name="数组"></param>
        /// <param name="获取全局步骤"></param>
        /// <param name="设置全局步骤"></param>
        /// <returns></returns>
        private static async Task<bool> 释放技能后替换图标技能后续(Keys key, Func<int> 获取全局步骤, Action<int> 设置全局步骤)
        {
            int 全局步骤 = 获取全局步骤();

            switch (全局步骤)
            {
                case 1:
                    return await 主动技能进入CD后续(key, () =>
                    {
                        设置全局步骤(0);
                        _切假腿配置.修改配置(key, true);
                    }).ConfigureAwait(true);
                default:
                    _ = await 主动技能释放后续(key, () =>
                    {
                        设置全局步骤(1);
                        通用技能后续动作();
                        _切假腿配置.修改配置(key, false);
                    }).ConfigureAwait(true);

                    return await FromResult(true).ConfigureAwait(true);
            }
        }


        /// <summary>
        ///     传入参数是移动接A，否A接移动
        ///     <para><paramref name="是否接A" /> 是移动接A，否A接移动 默认是</para>
        /// </summary>
        /// <param name="是否接A"></param>
        private static void 通用技能后续动作(bool 是否接按键 = true, Keys 要接的按键 = Keys.A, int 等待的延迟 = 0, bool 是否保持假腿 = true)
        {
            _ = Run(() =>
            {
                Delay(等待的延迟);

                if (是否保持假腿)
                {
                    要求保持假腿();
                }

                if (是否接按键)
                {
                    SimKeyBoard.MouseRightClick();
                    SimKeyBoard.KeyPress(要接的按键);
                }
                else
                {
                    SimKeyBoard.KeyPress(Keys.A);
                    SimKeyBoard.MouseRightClick();
                }
            });
        }

        private static void 后续切回假腿(bool 是否接按键 = true, Keys 要接的按键 = Keys.A, int 等待的延迟 = 0)
        {
            _ = Run(() =>
            {
                Delay(等待的延迟);
                要求保持假腿();
            });
        }

        /// <summary>
        ///     所有物品可使用后续
        ///     <para>字节数组为副本</para>
        ///     <para>为物品可释放那一刻的图片</para>
        /// </summary>
        /// <param name="序号"></param>
        /// <param name="数组">字节数组为副本，为物品解除锁闭那一刻的图片</param>
        /// <param name="afterAction"></param>
        /// <returns>
        ///     物品被锁闭后赋值状态后返回真
        ///     <para>(如未被锁闭一直返回真)</para>
        ///     <para>解除锁闭处理逻辑后返回假</para>
        /// </returns>
        private static async Task<bool> 所有物品可用后续(字节数组包含长宽 数组, Action afterAction)
        {
            if (DOTA2判断任意物品是否锁闭(in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            _ = Run(afterAction).ConfigureAwait(false);
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 物品

        private static async Task<bool> 处理物品进入CD(int 序号, 字节数组包含长宽 数组)
        {
            if (DOTA2判断序号物品是否CD(序号, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            _ = Run(() =>
            {
                Delay(60);
                要求保持假腿();
            }).ConfigureAwait(false);

            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 物品z进入CD(字节数组包含长宽 数组)
        {
            return await 处理物品进入CD(0, 数组).ConfigureAwait(true);
        }

        private static async Task<bool> 物品x进入CD(字节数组包含长宽 数组)
        {
            return await 处理物品进入CD(1, 数组).ConfigureAwait(true);
        }

        private static async Task<bool> 物品c进入CD(字节数组包含长宽 数组)
        {
            return await 处理物品进入CD(2, 数组).ConfigureAwait(true);
        }

        private static async Task<bool> 物品v进入CD(字节数组包含长宽 数组)
        {
            return await 处理物品进入CD(3, 数组).ConfigureAwait(true);
        }

        private static async Task<bool> 物品b进入CD(字节数组包含长宽 数组)
        {
            return await 处理物品进入CD(4, 数组).ConfigureAwait(true);
        }

        private static async Task<bool> 物品space进入CD(字节数组包含长宽 数组)
        {
            return await 处理物品进入CD(5, 数组).ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region 力量

        #region 大牛

        private static async Task<bool> 回音践踏去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1, 判断成功后延时: 1300).ConfigureAwait(true);
        }

        private static async Task<bool> 灵体游魂去后摇(字节数组包含长宽 数组)
        {
            return await 释放技能后替换图标技能后续(Keys.W, 获取全局步骤w, 设置全局步骤w).ConfigureAwait(true);
        }

        private static async Task<bool> 裂地沟壑去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 尸王

        private static async Task<bool> 腐朽去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 噬魂去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 墓碑去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 血肉傀儡去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 伐木机

        private static async Task<bool> 伐木机获取命石(字节数组包含长宽 数组)
        {
            if (_命石选择 == 0)
            {
                if (RegPicture(命石_伐木机_碎木击_数组, in _全局数组, 15))
                {
                    _命石选择 = 1;
                }
                else if (RegPicture(命石_伐木机_锯齿轮旋_数组, in _全局数组, 15))
                {
                    _命石选择 = 2;
                    _切假腿配置.修改配置(Keys.D, true);
                }
            }

            _命石根据图片委托 = null;
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 死亡旋风去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 伐木聚链去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 锯齿轮旋去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 锯齿飞轮去后摇(字节数组包含长宽 数组)
        {
            return await 释放技能后替换图标技能后续(Keys.R, 获取全局步骤r, 设置全局步骤r).ConfigureAwait(true);
        }

        private static async Task<bool> 喷火装置去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.F, 0).ConfigureAwait(true);
        }

        #endregion

        #region 全能

        private static async Task<bool> 洗礼去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 驱逐去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 守护天使去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 军团

        private static async Task<bool> 决斗(字节数组包含长宽 数组)
        {
            return await Run(async () =>
            {
                int 步骤 = 获取全局步骤();

                switch (步骤)
                {
                    case < 1:
                        {
                            Delay(33 * 根据图片使用物品(物品_臂章_数组));
                            Delay(33 * 根据图片使用物品(物品_魂戒_数组));
                            Delay(33 * 根据图片使用物品(物品_相位鞋_数组));

                            if (DOTA2判断技能是否CD(Keys.W, in 数组))
                            {
                                SimKeyBoard.KeyPressAlt(Keys.W);
                                return await FromResult(true).ConfigureAwait(true);
                            }

                            Delay(33 * 根据图片使用物品(物品_刃甲_数组));

                            设置全局步骤(1);
                            return await FromResult(true).ConfigureAwait(true);
                        }
                    case < 2 when 步骤 == 1:
                        {
                            Delay(33 *
                                  (
                                      根据图片使用物品(物品_跳刀_数组)
                                      + 根据图片使用物品(物品_跳刀_力量跳刀_数组)
                                      + 根据图片使用物品(物品_跳刀_智力跳刀_数组)
                                      + 根据图片使用物品(物品_跳刀_敏捷跳刀_数组)
                                  ));

                            设置全局步骤(2);

                            return await FromResult(true).ConfigureAwait(true);
                        }
                    case < 3:
                        {
                            Delay(33 * 根据图片使用物品(物品_紫苑_数组));
                            Delay(33 * 根据图片使用物品(物品_血棘_数组));
                            Delay(33 * 根据图片使用物品(物品_否决_数组));
                            Delay(33 * 根据图片使用物品(物品_散失_数组));
                            Delay(33 * 根据图片使用物品(物品_散魂_数组));
                            Delay(33 * 根据图片使用物品(物品_深渊之刃_数组));

                            设置全局步骤(3);

                            return await FromResult(true).ConfigureAwait(true);
                        }

                    case < 4:
                        {
                            // 触发激怒，让周围的小兵都攻击你
                            SimKeyBoard.KeyPress(Keys.A);

                            if (DOTA2释放CD就绪技能(Keys.R, in 数组))
                            {
                                Delay(60);
                                return await FromResult(true).ConfigureAwait(true);
                            }

                            设置全局步骤(-1);
                            return await FromResult(false).ConfigureAwait(true);
                        }
                }

                return await FromResult(false).ConfigureAwait(true);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 压倒性优势去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 强攻去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 决斗去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1, 要接的按键: Keys.Q).ConfigureAwait(true);
        }

        #endregion

        #region 骷髅王

        private static async Task<bool> 骷髅王获取命石(字节数组包含长宽 数组)
        {
            if (_命石选择 == 0)
            {
                _ = await 获取图片_1().ConfigureAwait(true);
                _命石选择 = RegPicture(命石_骷髅王_白骨守卫_数组, in _全局数组, 15) ? 1 : 2;
            }

            _命石根据图片委托 = null;
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 冥火爆击去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 白骨守卫去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        #endregion

        #region 人马

        private static async Task<bool> 马蹄践踏接平A(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 双刃剑去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        #endregion

        #region 哈斯卡

        private static async Task<bool> 心炎去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 牺牲去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.R, () =>
            {
                SimKeyBoard.MouseRightClick();

                if (DOTA2释放CD就绪技能(Keys.Q, in 数组))
                {
                    return;
                }

                SimKeyBoard.KeyPress(Keys.A);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 小狗

        private static async Task<bool> 狂暴去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 撕裂伤口去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        #endregion

        #region 土猫

        private static async Task<bool> 巨石冲击去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 地磁之握去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 磁化去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 孽主

        private static async Task<bool> 火焰风暴去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 怨念深渊去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.W, () =>
            {
                SimKeyBoard.MouseRightClick();
                if (DOTA2释放CD就绪技能(Keys.Q, in 数组))
                {
                    return;
                }

                SimKeyBoard.KeyPress(Keys.A);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 小小

        private static async Task<bool> 山崩去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 投掷去后摇(字节数组包含长宽 数组)
        {
            return await Run(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    Delay(20);
                    通用技能后续动作();
                }

                return false;
            }).ConfigureAwait(true);
        }

        #endregion

        #region 海民

        private static async Task<bool> 海民获取命石(字节数组包含长宽 数组)
        {
            if (_命石选择 == 0)
            {
                _命石选择 = RegPicture(命石_海民_酒友_数组, in _全局数组, 15) ? 2 : 1;
            }

            _命石根据图片委托 = null;
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 冰片去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 摔角行家去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 酒友去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 1).ConfigureAwait(true);
        }

        // 基本完美了。。。
        private static async Task<bool> 海象神拳接雪球(字节数组包含长宽 数组)
        {
            return await 法球技能进入CD后续(Keys.R, () =>
            {
                Point p = MousePosition;
                for (int i = 0; i < 2; i++)
                {
                    Delay(33);
                    SimKeyBoard.MouseMove(p.X, p.Y - (60 * i));
                    SimKeyBoard.KeyPress(Keys.W);
                }

                _ = Run(() =>
                {
                    Delay(100);
                    SimKeyBoard.MouseMove(p);
                    Delay(850);
                    if (_中断条件)
                    {
                        return;
                    }

                    SimKeyBoard.KeyDown(Keys.D);
                    Delay(100);
                    SimKeyBoard.MouseMove(_指定地点d);
                    Delay(100);
                    SimKeyBoard.KeyUp(Keys.D);
                    Delay(600);
                    SimKeyBoard.KeyPress(Keys.W);
                });
            }).ConfigureAwait(true);
        }

        #endregion

        #region 屠夫

        // 钩子出手后，就可以用W，但其他技能无法释放且物品会被锁闭，可以通过判断锁闭的状态
        private static async Task<bool> 钩子去僵直(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.Q, () =>
            {
                if (!DOTA2判断状态技能是否启动(Keys.W, in 数组))
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

        private static async Task<bool> 肢解检测状态(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.R, () =>
            {
                if (!DOTA2判断状态技能是否启动(Keys.W, in 数组))
                {
                    SimKeyBoard.KeyPress(Keys.W);
                }

                _ = 根据图片使用物品(物品_纷争_数组);
                _ = 根据图片使用物品(物品_希瓦_数组);
            }).ConfigureAwait(true);
        }

        // 技能颜色虽然变了，但是CD状态的颜色没变，
        // 钩可以直接接咬，但期间物品还是锁闭的
        // 解决。
        private static async Task<bool> 快速接肢解(字节数组包含长宽 数组)
        {
            return await 所有物品可用后续(数组, () =>
            {
                Delay(33 * 根据图片使用物品(物品_纷争_数组));
                Delay(33 * 根据图片使用物品(物品_希瓦_数组));
                Delay(33 * 根据图片使用物品(物品_虚灵之刃_数组));
                Delay(33 * 根据图片使用物品(物品_否决_数组));

                Delay(33 *
                      (根据图片使用物品(物品_红杖_数组) +
                       根据图片使用物品(物品_红杖2_数组) +
                       根据图片使用物品(物品_红杖3_数组) +
                       根据图片使用物品(物品_红杖4_数组) +
                       根据图片使用物品(物品_红杖5_数组)));

                SimKeyBoard.KeyPress(Keys.R);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 斧王

        private static async Task<bool> 吼去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.Q, () =>
            {
                if (_全局模式q == 1)
                {
                    _ = 根据图片使用物品(物品_刃甲_数组);
                }
                // 触发激怒
                SimKeyBoard.KeyPress(Keys.A);
                SimKeyBoard.KeyPress(Keys.W);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 战斗饥渴去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 淘汰之刃去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 跳吼(字节数组包含长宽 数组)
        {
            if (根据图片使用物品(物品_跳刀_数组)
                + 根据图片使用物品(物品_跳刀_力量跳刀_数组)
                + 根据图片使用物品(物品_跳刀_智力跳刀_数组)
                + 根据图片使用物品(物品_跳刀_敏捷跳刀_数组) == 1)
            {
                Delay(等待延迟);
            }

            _ = DOTA2释放CD就绪技能(Keys.Q, in 数组);

            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 大鱼人

        private static async Task<bool> 跳刀接踩(字节数组包含长宽 数组)
        {
            if (根据图片使用物品(物品_魂戒_数组) == 1)
            {
                Delay(等待延迟);
            }

            if (根据图片使用物品(物品_跳刀_数组)
                + 根据图片使用物品(物品_跳刀_敏捷跳刀_数组)
                + 根据图片使用物品(物品_跳刀_智力跳刀_数组)
                + 根据图片使用物品(物品_跳刀_力量跳刀_数组) == 1
               )
            {
                Delay(等待延迟);
            }

            _ = DOTA2释放CD就绪技能(Keys.W, in 数组);

            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 守卫冲刺去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 踩去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1, 要接的按键: _是否魔晶 ? Keys.A : Keys.R).ConfigureAwait(true);
        }

        private static async Task<bool> 雾霭去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 斯温

        private static async Task<bool> 风暴之拳去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 战吼去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 神之力量去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 船长

        private static async Task<bool> 洪流接x回(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.Q, () =>
            {
                _条件4 = false;
                初始化全局时间(ref _全局时间q);

                int 步骤e = 获取全局步骤e();

                // 如果E已经释放
                if (!_中断条件 && 步骤e == 1)
                {
                    // 1600 延迟 返回200施法时间
                    Delay(1350, _全局时间q);
                    _条件4 = false;
                    SimKeyBoard.KeyPress(Keys.E);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> x释放后相关逻辑(字节数组包含长宽 数组)
        {
            // 释放x后放船，x的时间3秒，船0.3秒，3.1秒延迟，控制还是得靠水起来
            return await 主动技能释放后续(Keys.E, () =>
            {
                int 步骤e = 获取全局步骤e();

                if (步骤e == 1)
                {
                    return;
                }

                初始化全局时间(ref _全局时间e);

                int 步骤r = 获取全局步骤r();

                if (步骤r == 1)
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
                Delay(等待时间, _全局时间e);
                _条件4 = true;
            }).ConfigureAwait(true);
        }

        private static async Task<bool> x2次释放后(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.E, () =>
            {
                lock (_全局模式e_lock)
                {
                    // 玲珑心，释放完后至少再等6秒，等2秒基本完事
                    // 因为释放q后，会再释放一次E
                    // 等待说明E已经释放过一次，还在有效范围内
                    Delay(2000);
                    设置全局步骤e(0);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 立即释放洪流(字节数组包含长宽 数组)
        {
            return await 主动技能已就绪后续(Keys.Q, () => { SimKeyBoard.KeyPress(Keys.Q); }).ConfigureAwait(true);
        }

        #endregion

        #region 夜魔

        private static async Task<bool> 虚空去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 伤残恐惧去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 暗夜猎影去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 黑暗飞升去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 树精

        private static async Task<bool> 自然卷握去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 寄生种子去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 活体护甲去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 丛林之眼去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 疯狂生长去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 混沌

        private static async Task<bool> 混乱之箭去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1, 要接的按键: _全局模式q == 1 ? Keys.W : Keys.A).ConfigureAwait(true);
        }

        private static async Task<bool> 实相裂隙去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 11).ConfigureAwait(true);
        }

        private static async Task<bool> 混沌之军去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 马尔斯

        private static async Task<bool> 战神迅矛去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.Q, () =>
            {
                if (_全局模式q == 1)
                {
                    SimKeyBoard.KeyPress(Keys.R);
                }
                else
                {
                    通用技能后续动作();
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 神之谴击去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 热血竞技场去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.R, () =>
            {
                if (判断技能状态(Keys.E, 数组, 技能类型.状态))
                {
                    SimKeyBoard.KeyPress(Keys.E);
                }

                通用技能后续动作();
            }).ConfigureAwait(true);
        }

        #endregion

        #region 破晓晨星

        private static async Task<bool> 上界重锤去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        #endregion

        #region 钢背

        private static async Task<bool> 鼻涕去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 扫射切回假腿(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 10).ConfigureAwait(true);
        }

        private static async Task<bool> 针刺循环(字节数组包含长宽 数组)
        {
            _ = await 主动技能已就绪后续(Keys.W, () =>
            {
                SimKeyBoard.KeyPress(Keys.W);
                Delay(等待延迟);
            }).ConfigureAwait(true);
            return await FromResult(_循环条件1).ConfigureAwait(true);
        }

        private static async Task<bool> 毛团去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 钢毛后背去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0, false).ConfigureAwait(true);
        }

        #endregion

        #region 龙骑

        private static async Task<bool> 喷火去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 神龙摆尾去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.W, () =>
            {
                SimKeyBoard.KeyPress(Keys.A);
                _ = _全局模式w == 1 && _是否魔晶 ? DOTA2释放CD就绪技能(Keys.D, in 数组) : DOTA2释放CD就绪技能(Keys.Q, in 数组);

                要求保持假腿();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 变龙去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 火球去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 0, 要接的按键: _全局模式d == 1 && DOTA2判断技能是否CD(Keys.Q, in 数组) ? Keys.Q : Keys.A)
                .ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region 敏捷

        #region 小骷髅

        private static async Task<bool> 扫射去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.Q, () =>
            {
                if (_全局模式q == 1)
                {
                    Delay(33 * 根据图片使用物品(物品_散失_数组));
                    Delay(33 * 根据图片使用物品(物品_散魂_数组));
                    Delay(33 * 根据图片使用物品(物品_否决_数组));
                    Delay(33 * 根据图片使用物品(物品_紫苑_数组));
                    Delay(33 * 根据图片使用物品(物品_血棘_数组));
                    Delay(33 * 根据图片使用物品(物品_羊刀_数组));
                    Delay(33 * 根据图片使用物品(物品_阿托斯之棍_数组));
                    Delay(33 * 根据图片使用物品(物品_缚灵锁_数组));
                }

                通用技能后续动作();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 焦油去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.W, () =>
            {
                _ = DOTA2释放CD就绪技能(Keys.Q, in 数组);
                通用技能后续动作();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 死亡契约去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.E, SimKeyBoard.MouseRightClick).ConfigureAwait(true);
        }

        private static async Task<bool> 骨隐步去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.R, SimKeyBoard.MouseRightClick).ConfigureAwait(true);
        }

        private static async Task<bool> 炽烈火雨去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.F, () =>
            {
                // 持续时间施法，其实啥也不用管？
                if (_全局模式f == 1)
                {
                    Delay(0);
                    SimKeyBoard.KeyPress(Keys.R);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 骷髅之军去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.F, 0).ConfigureAwait(true);
        }

        #endregion

        #region 小黑

        private static async Task<bool> 狂风去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.W, () =>
            {
                通用技能后续动作();

                if (_全局模式 == 1)
                {
                    _需要切假腿 = false;
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 数箭齐发去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.E, () =>
            {
                Delay(_全局模式e == 1 ? 2600 : 1300);
                SimKeyBoard.KeyPress(Keys.S);
                通用技能后续动作();

                if (_全局模式 == 1)
                {
                    _需要切假腿 = false;
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 冰川去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.F, () =>
            {
                通用技能后续动作();

                if (_全局模式 == 1)
                {
                    _需要切假腿 = false;
                }
            }).ConfigureAwait(true);
        }

        #endregion

        #region 巨魔

        private static async Task<bool> 旋风飞斧远去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 旋风飞斧近去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 战斗专注去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 幻刺

        private static async Task<bool> 窒息短匕敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 幻影突袭敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 魅影无形敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0, false).ConfigureAwait(true);
        }

        private static async Task<bool> 刀阵旋风敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 0).ConfigureAwait(true);
        }

        #endregion

        #region 猴子

        private static async Task<bool> 灵魂之矛敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 神行百变选择幻象(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.W, () =>
            {
                Delay(1000);
                SimKeyBoard.KeyPress(Keys.D1);
                Delay(等待延迟);
                SimKeyBoard.MouseRightClick();
                SimKeyBoard.KeyPress(Keys.F1);
                要求保持假腿();
            }).ConfigureAwait(true);
        }

        #endregion

        #region 幽鬼

        private static async Task<bool> 幽鬼之刃去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1, false).ConfigureAwait(true);
        }

        private static async Task<bool> 如影随形去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.R, () =>
            {
                if (_全局模式f == 1)
                {
                    SimKeyBoard.KeyPress(Keys.D);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 空降去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.D, () =>
            {
                if (_全局模式f == 1)
                {
                    if (根据图片使用物品(物品_幻影斧_数组) == 1)
                    {
                        分身一齐攻击();
                    }

                    Delay(33 * 根据图片使用物品(物品_否决_数组));
                    Delay(33 * 根据图片使用物品(物品_紫苑_数组));
                    Delay(33 * 根据图片使用物品(物品_血棘_数组));
                }

                要求保持假腿();

                SimKeyBoard.KeyPress(Keys.A);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 折射去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        #endregion

        #region 影魔

        //private static void 吹风摇大()
        //{
        //    var w_down = 0;

        //    KeyPress(Keys.Space);

        //    while (w_down == 0)
        //        if (RegPicture(Resource_Picture.吹风CD, 1291, 991, 60, 45))
        //        {
        //            w_down = 1;
        //            KeyPress(Keys.M);
        //            delay(830);
        //            KeyPress(Keys.R);
        //        }
        //}

        #endregion

        #region TB

        private static async Task<bool> 倒影敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 幻惑敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 魔化敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 恶魔狂热去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 恐怖心潮敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.F, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 断魂敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 敌法

        private static async Task<bool> 闪烁敏捷(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.W, () =>
            {
                if (_全局模式w == 1)
                {
                    _ = 根据图片使用物品(物品_幻影斧_数组);
                    分身一齐攻击();
                    _ = 根据图片使用物品(物品_深渊之刃_数组);
                    _全局模式w = 0;
                }

                通用技能后续动作();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 法力虚空取消后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 法术反制敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 10).ConfigureAwait(true);
        }

        private static async Task<bool> 友军法术反制敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 10).ConfigureAwait(true);
        }

        #endregion

        #region 小鱼人

        private static async Task<bool> 黑暗契约去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 跳水去后摇(字节数组包含长宽 数组)
        {
            _ = Run(() =>
            {
                通用技能后续动作(是否保持假腿: false);
                _需要切假腿 = false;
                Delay(200);
                要求保持假腿();
            });
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 深海护罩去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 暗影之舞去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 小松鼠

        private static async Task<bool> 爆栗出击去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.Q, () =>
            {
                if (_全局模式w == 1)
                {
                    SimKeyBoard.KeyPress(Keys.W);
                }
                else
                {
                    通用技能后续动作();
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 野地奇袭去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.W, () =>
            {
                if (_全局模式e == 1)
                {
                    SimKeyBoard.KeyPress(Keys.Q);
                }
                else
                {
                    通用技能后续动作();
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 一箭穿心(字节数组包含长宽 数组)
        {
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 猎手旋标去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.F, 1).ConfigureAwait(true);
        }

        #endregion

        #region 火猫

        private static async Task<bool> 无影拳后续处理(字节数组包含长宽 数组)
        {
            bool b = RegPicture(Buff_火猫_无影拳_数组, in 数组);

            if (b)
            {
                if (_全局模式w == 1)
                {
                    SimKeyBoard.KeyPress(Keys.Q);
                }

                要求保持假腿();

                SimKeyBoard.KeyPress(Keys.A);
            }

            return await FromResult(!b).ConfigureAwait(true);
        }

        private static async Task<bool> 炎阳索去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 烈火罩去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 激活残焰去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 1).ConfigureAwait(true);
        }

        #endregion

        #region 拍拍

        private static async Task<bool> 超强力量去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 震撼大地去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 狂怒去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 跳拍(字节数组包含长宽 数组)
        {
            _ = Run(() =>
            {
                if (根据图片使用物品(物品_跳刀_数组)
                    + 根据图片使用物品(物品_跳刀_力量跳刀_数组)
                    + 根据图片使用物品(物品_跳刀_敏捷跳刀_数组) == 1)
                {
                    SimKeyBoard.KeyPress(Keys.A);

                    _ = DOTA2释放CD就绪技能(Keys.Q, in 数组);
                }
            });

            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 火枪

        private static async Task<bool> 流霰弹去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 瞄准去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.E, () =>
            {
                _ = 根据图片使用物品(物品_疯狂面具_数组);

                通用技能后续动作();
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 暗杀去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 震荡手雷去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 1).ConfigureAwait(true);
        }

        #endregion

        #region 飞机

        private static async Task<bool> 火箭弹幕敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 追踪导弹敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 高射火炮敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 召唤飞弹敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 美杜莎

        private static async Task<bool> 秘术异蛇去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 罗网剑阵去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 石化凝视去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 虚空

        private static async Task<bool> 时间漫游敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 时间膨胀敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 时间结界敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 血魔

        private static async Task<bool> 血祭去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.W, () =>
            {
                SimKeyBoard.MouseRightClick();
                SimKeyBoard.KeyPress(Keys.A);

                要求保持假腿();

                Delay(2400);
                Point p = MousePosition;
                SimKeyBoard.MouseMove(601, 988);
                if (DOTA2释放CD就绪技能(Keys.Q, in 数组))
                {
                    SimKeyBoard.MouseMove(p);
                }
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 割裂去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 血怒去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        #endregion

        #region 赏金

        private static async Task<bool> 飞镖接平a(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 标记去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, _循环条件1 ? 0 : 1).ConfigureAwait(true);
        }

        private static async Task<bool> 循环标记(字节数组包含长宽 数组)
        {
            int 步骤 = 获取全局步骤r();
            _ = await 主动技能已就绪后续(Keys.R, () =>
            {
                if (步骤 == 0)
                {
                    设置全局步骤(1);
                    SimKeyBoard.KeyPress(Keys.R);
                    Delay(200);
                    设置全局步骤(0);
                }
            }).ConfigureAwait(true);

            return _循环条件1;
        }

        #endregion

        #region 电棍

        private static async Task<bool> 等离子场去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 静电连接去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 风暴之眼去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        #endregion

        #region 露娜

        private static async Task<bool> 月光后敏捷平a(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 月刃后敏捷平a(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 月蚀后敏捷平a(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 大圣

        private static async Task<bool> 棒击大地去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 乾坤之跃敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 猴子猴孙敏捷(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 大圣无限跳跃(字节数组包含长宽 数组)
        {
            int 步骤 = 获取全局步骤w();

            _ = await 主动技能已就绪后续(Keys.W, () =>
            {
                if (步骤 == 0)
                {
                    设置全局步骤w(1);
                    SimKeyBoard.KeyPress(Keys.W);
                    Delay(200);
                    设置全局步骤w(0);
                }
            }).ConfigureAwait(true);

            return await FromResult(_循环条件1).ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region 智力

        #region 修补匠

        private static void 检测敌方英雄自动导弹()
        {
            //Task t = new(() =>
            //{
            //    if (RegPicture(血量_敌人血量, 0, 0, 1920, 1080, 0.6))
            //    {
            //        KeyPress( Keys.W);
            //        Delay(40);
            //    }
            //});
            //t.Start();
            //await t;
        }

        private static void 推推接刷新()
        {
            //long time = 获取当前时间毫秒();
            //int x_down = 0;
            //while (x_down == 1)
            //{
            //    //if (RegPicture(物品_推推BUFF_数组, 400, 865, 1000, 60))
            //    //{
            //    //    KeyPress( Keys.R);
            //    //    x_down = 1;
            //    //}
            //    if (获取当前时间毫秒() - time > 500)
            //    {
            //        break;
            //    }
            //}
        }


        private static void 刷新完跳()
        {
            //int all_down = 0;
            //long time = 获取当前时间毫秒();
            //while (all_down == 1)
            //{
            //    //var r_down = 0;
            //    //if (RegPicture(修补匠_再填装施法, 773, 727, 75, 77, 0.7))
            //    //{
            //    //    if (_条件3)
            //    //        await 检测希瓦();
            //    //    while (r_down == 0)
            //    //        if (!RegPicture(修补匠_再填装施法, 773, 727, 75, 77, 0.7))
            //    //        {
            //    //            r_down = 1;
            //    //            all_down = 1;
            //    //            if (_条件1)
            //    //                await 检测敌方英雄自动导弹();
            //    //            if (_条件2)
            //    //            {
            //    //                Delay(60);
            //    //                KeyPress(Keys.Space);
            //    //            }
            //    //        }
            //    //}
            //    if (获取当前时间毫秒() - time > 700)
            //    {
            //        break;
            //    }
            //}
        }

        #endregion

        #region 光法

        private static async Task<bool> 减少300毫秒蓄力(字节数组包含长宽 数组)
        {
            int 全局步骤 = 获取全局步骤q();

            switch (全局步骤)
            {
                case 1:
                    return await 主动技能进入CD后续(Keys.Q, () =>
                    {
                        设置全局步骤q(0);
                        _切假腿配置.修改配置(Keys.Q, true);
                    }).ConfigureAwait(true);
                default:
                    设置全局步骤q(1);
                    if (RegPicture(Buff_光法_大招_数组, _全局数组))
                    {
                        _切假腿配置.修改配置(Keys.Q, false);
                        SimKeyBoard.MouseRightClick();
                    }

                    _ = Run(() =>
                    {
                        Delay(2700);
                        SimKeyBoard.KeyPress(Keys.Q);
                    }).ConfigureAwait(false);

                    return await FromResult(true).ConfigureAwait(true);
            }
        }

        private static async Task<bool> 炎阳之缚去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 查克拉魔法去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 致盲之光去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 灵光去后摇接炎阳(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.F, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 循环查克拉(字节数组包含长宽 数组)
        {
            _ = await 主动技能已就绪后续(Keys.E, () =>
            {
                if (_全局步骤e == 0)
                {
                    _全局步骤e = 1;
                    SimKeyBoard.KeyPress(Keys.E);
                    Delay(200);
                    _全局步骤e = 0;
                }
            }).ConfigureAwait(true);

            return _循环条件1;
        }

        #endregion

        #region 天怒

        private static async Task<bool> 循环奥数鹰隼(字节数组包含长宽 数组)
        {
            _ = await 主动技能已就绪后续(Keys.Q, () =>
            {
                int 步骤 = 获取全局步骤q();

                if (步骤 == 0)
                {
                    设置全局步骤q(1);
                    SimKeyBoard.KeyPress(Keys.Q);
                    Delay(200);
                    设置全局步骤q(0);
                }
            }).ConfigureAwait(true);

            return true;
        }

        private static async Task<bool> 天怒秒人连招(字节数组包含长宽 数组)
        {
            int 步骤 = 获取全局步骤();

            switch (步骤)
            {
                case < 2:
                    if (DOTA2释放CD就绪技能(Keys.W, in 数组))
                    {
                        return await FromResult(true).ConfigureAwait(true);
                    }

                    if (DOTA2释放CD就绪技能(Keys.E, in 数组))
                    {
                        return await FromResult(true).ConfigureAwait(true);
                    }

                    if (DOTA2释放CD就绪技能(Keys.Q, in 数组))
                    {
                        return await FromResult(true).ConfigureAwait(true);
                    }

                    Delay(0 * 根据图片使用物品(物品_阿托斯之棍_数组));
                    Delay(33 * 根据图片使用物品(物品_缚灵锁_数组));
                    Delay(33 * 根据图片使用物品(物品_虚灵之刃_数组));

                    Delay(33 * (
                        根据图片使用物品(物品_红杖_数组)
                        + 根据图片使用物品(物品_红杖2_数组)
                        + 根据图片使用物品(物品_红杖3_数组)
                        + 根据图片使用物品(物品_红杖4_数组)
                        + 根据图片使用物品(物品_红杖5_数组)));

                    Delay(33 * 根据图片使用物品(物品_羊刀_数组));

                    设置全局步骤(2);

                    return await FromResult(true).ConfigureAwait(true);
                case < 3:
                    {
                        if (DOTA2释放CD就绪技能(Keys.R, in 数组))
                        {
                            return await FromResult(true).ConfigureAwait(true);
                        }

                        _条件1 = true;
                        _循环条件1 = true;
                        设置全局步骤(3);

                        return await FromResult(false).ConfigureAwait(true);
                    }
            }

            return await FromResult(true).ConfigureAwait(true);
        }

        private static async Task<bool> 奥数鹰隼去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 上古封印去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 神秘之耀去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 震荡光弹去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        #endregion

        #region 墨客

        private static async Task<bool> 命运之笔去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 幻影之拥去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 墨泳去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 缚魂去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 暗绘去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 0).ConfigureAwait(true);
        }

        #endregion

        #region 宙斯

        private static async Task<bool> 弧形闪电去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 雷击去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 神圣一跳去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 雷神之怒去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 雷云去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 0).ConfigureAwait(true);
        }

        #endregion

        #region 巫医

        private static async Task<bool> 麻痹药剂去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1, 要接的按键: Keys.E).ConfigureAwait(true);
        }

        private static async Task<bool> 巫蛊咒术去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.E, () =>
            {
                SimKeyBoard.KeyPress(Keys.A);
                _ = 根据图片使用物品(物品_魂之灵龛_数组);
                _ = 根据图片使用物品(物品_影之灵龛_数组);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 死亡守卫隐身(字节数组包含长宽 数组)
        {
            return await 主动技能释放后续(Keys.R, () =>
            {
                _ = 根据图片自我使用物品(物品_微光披风_数组);
                _ = 根据图片使用物品(物品_隐刀_数组);
                _ = 根据图片使用物品(物品_大隐刀_数组);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 巫妖

        private static async Task<bool> 寒霜爆发去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, _全局步骤e > 0 ? 11 : 1).ConfigureAwait(true);
        }

        private static async Task<bool> 冰霜魔盾去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, _全局步骤e > 0 ? 11 : 1, false).ConfigureAwait(true);
        }

        private static async Task<bool> 阴邪凝视去后摇(字节数组包含长宽 数组)
        {
            if (DOTA2判断技能是否CD(Keys.E, in 数组))
            {
                设置全局步骤e(0);
                return await FromResult(true).ConfigureAwait(true);
            }

            return await Run(async () =>
            {
                if (_全局步骤e == 0)
                {
                    设置全局步骤e(1);
                    return await FromResult(true).ConfigureAwait(true);
                }
                else if (_全局步骤e == 1)
                {
                    _ = Run(() =>
                    {
                        Delay(200);
                        设置全局步骤e(2);
                    });
                    return await FromResult(true).ConfigureAwait(true);
                }
                else
                {
                    if (!DOTA2判断是否持续施法(in 数组))
                    {
                        设置全局步骤e(0);
                        SimKeyBoard.KeyPress(Keys.A);
                        _ = 根据图片使用物品(物品_羊刀_数组);
                        return await FromResult(false).ConfigureAwait(true);
                    }
                    else
                    {
                        return await FromResult(true).ConfigureAwait(true);
                    }
                }
            }).ConfigureAwait(false);
        }

        private static async Task<bool> 连环霜冻去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, _全局步骤e > 0 ? 11 : 1).ConfigureAwait(true);
        }

        private static async Task<bool> 寒冰尖柱去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, _全局步骤e > 0 ? 10 : 0).ConfigureAwait(true);
        }

        #endregion

        #region 帕克

        private static async Task<bool> 幻象法球去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.Q, () =>
            {
                设置全局步骤q(1);
                Delay(3400);
                if (_全局模式d == 1)
                {
                    SimKeyBoard.KeyPress(Keys.D);
                }

                设置全局步骤q(0);
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 新月之痕去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 梦境缠绕去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 灵动之翼定位(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.D, () =>
            {
                SimKeyBoard.KeyPress(Keys.F1);
                SimKeyBoard.KeyPress(Keys.F1);
            }).ConfigureAwait(true);
        }

        #endregion

        #region 骨法

        private static async Task<bool> 幽冥轰爆去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, _全局步骤r > 0 ? 10 : 0).ConfigureAwait(true);
        }

        private static async Task<bool> 衰老去后摇(字节数组包含长宽 数组)
        {
            return await 主动技能进入CD后续(Keys.W, () =>
            {
                Delay(33 * (
                    根据图片使用物品(物品_红杖_数组)
                    + 根据图片使用物品(物品_红杖2_数组)
                    + 根据图片使用物品(物品_红杖3_数组)
                    + 根据图片使用物品(物品_红杖4_数组)
                    + 根据图片使用物品(物品_红杖5_数组)
                ));
            }).ConfigureAwait(true);
        }

        private static async Task<bool> 幽冥守卫去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, _全局步骤r > 0 ? 10 : 0).ConfigureAwait(true);
        }

        private static async Task<bool> 生命吸取去后摇(字节数组包含长宽 数组)
        {
            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                设置全局步骤r(0);
                return await FromResult(true).ConfigureAwait(true);
            }

            return await Run(async () =>
            {
                if (_全局步骤r == 0)
                {
                    if (_全局模式r == 1)
                    {
                        SimKeyBoard.KeyPress(Keys.W);
                    }

                    设置全局步骤r(1);
                    return await FromResult(true).ConfigureAwait(true);
                }
                else if (_全局步骤r == 1)
                {
                    _ = Run(() =>
                    {
                        Delay(200);
                        设置全局步骤r(2);
                    });
                    return await FromResult(true).ConfigureAwait(true);
                }
                else
                {
                    if (!DOTA2判断是否持续施法(in 数组))
                    {
                        设置全局步骤r(0);
                        SimKeyBoard.KeyPress(Keys.A);
                        return await FromResult(false).ConfigureAwait(true);
                    }
                    else
                    {
                        return await FromResult(true).ConfigureAwait(true);
                    }
                }
            }).ConfigureAwait(false);
        }

        #endregion

        #region 干扰者

        private static async Task<bool> 风雷之击去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 静态风暴去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 恶念瞥视去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0, false).ConfigureAwait(true);
        }

        private static async Task<bool> 动能力场去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0, false).ConfigureAwait(true);
        }

        private static async Task<bool> 静态风暴动能立场风雷之击(字节数组包含长宽 数组)
        {
            return DOTA2释放CD就绪技能(Keys.R, in 数组)
                ? await FromResult(true).ConfigureAwait(true)
                : DOTA2释放CD就绪技能(Keys.E, in 数组)
                    ? await FromResult(true).ConfigureAwait(true)
                    : DOTA2释放CD就绪技能(Keys.Q, in 数组)
                        ? await FromResult(true).ConfigureAwait(true)
                        : await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 黑鸟

        // todo 逻辑修改
        private static async Task<bool> 关接陨星锤(字节数组包含长宽 数组)
        {
            int time = 0;

            Color 技能点颜色 = Color.FromArgb(203, 183, 124);

            if (ColorAEqualColorB(获取指定位置颜色(909, 1008, in 数组), 技能点颜色, 0))
            {
                time = 4000;
            }
            else if (ColorAEqualColorB(获取指定位置颜色(897, 1008, in 数组), 技能点颜色, 0))
            {
                time = 3250;
            }

            static void 关后(int time, in 字节数组包含长宽 数组)
            {
                Delay(110);
                初始化全局时间(ref _全局时间w);
                SimKeyBoard.MouseRightClick();
                Delay(150);
                SimKeyBoard.KeyPress(Keys.S);
                Delay(time - 3000, _全局时间w);
                if (!_中断条件)
                {
                    _ = 根据图片使用物品(物品_陨星锤_数组);
                }
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            关后(time, in 数组);
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 神智之蚀去后摇(字节数组包含长宽 数组)
        {
            static void 神智之蚀后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            神智之蚀后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 关接跳(字节数组包含长宽 数组)
        {
            return 根据图片使用物品(物品_跳刀_数组) == 1
                ? await FromResult(false).ConfigureAwait(true)
                : await FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region 谜团

        private static void 跳秒接午夜凋零黑洞()
        {
            //if (RegPicture(物品_黑黄杖"), "Z_数组) KeyPress( Keys.Z);

            //if (RegPicture(物品_纷争"), "C_数组) KeyPress( Keys.C);

            //var time = 获取当前时间毫秒();

            //while (RegPicture(物品_跳刀"), "SPACE") || RegPicture(获取嵌入的图片("物品_跳刀_智力跳刀"), "SPACE_数组)
            //{
            //    Delay(15);
            //    KeyPress( Keys.Space);

            //    if (获取当前时间毫秒() - time > 300) break;
            //}

            Delay(等待延迟);

            //KeyDown(Keys.LControl);

            //KeyPress(Keys.A);

            //KeyUp(Keys.LControl);
        }

        private void 刷新接凋零黑洞()
        {
            SimKeyBoard.KeyPress(Keys.X);

            for (int i = 0; i < 2; i++)
            {
                Delay(等待延迟);
                SimKeyBoard.KeyPress(Keys.Z);
                SimKeyBoard.KeyPress(Keys.V);
                SimKeyBoard.KeyPress(Keys.R);
            }
        }

        #endregion

        #region 冰女

        #endregion

        #region 火女

        private static async Task<bool> 龙破斩去后摇(字节数组包含长宽 数组)
        {
            static void 龙破斩后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            龙破斩后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 光击阵去后摇(字节数组包含长宽 数组)
        {
            static void 光击阵后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            光击阵后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 神灭斩去后摇(字节数组包含长宽 数组)
        {
            static void 神灭斩后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            神灭斩后();
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 拉席克

        private static async Task<bool> 撕裂大地去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 恶魔敕令去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 闪电风暴去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 脉冲新星去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 虚无主义去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 0).ConfigureAwait(true);
        }

        #endregion

        #region 蓝猫

        private static async Task<bool> 拉接平A(字节数组包含长宽 数组)
        {
            return await FromResult(true).ConfigureAwait(true);
        }

        private void 残影接平A()
        {
            Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.A);
        }

        private static async Task<bool> 滚接平A(字节数组包含长宽 数组)
        {
            return await FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region 卡尔

        private static async Task<bool> 三冰对线(字节数组包含长宽 数组)
        {
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 三雷对线(字节数组包含长宽 数组)
        {
            return await FromResult(false).ConfigureAwait(true);
        }


        private static async Task<bool> 三雷幽灵(字节数组包含长宽 数组)
        {
            return await FromResult(true).ConfigureAwait(true);
        }

        private static async Task<bool> 极冷吹风陨星锤雷暴(字节数组包含长宽 数组)
        {
            return await FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region 术士

        private static async Task<bool> 致命链接去后摇(字节数组包含长宽 数组)
        {
            static void 致命链接后()
            {
                通用技能后续动作(false);
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            致命链接后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 暗言术去后摇(字节数组包含长宽 数组)
        {
            static void 暗言术后()
            {
                通用技能后续动作(false);
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            暗言术后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 混乱之祭去后摇(字节数组包含长宽 数组)
        {
            static void 混乱之祭后()
            {
                通用技能后续动作(false);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            混乱之祭后();
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 暗影萨满

        /// <summary>
        ///     前摇时间基本在
        /// </summary>
        /// <param name="bts"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static async Task<bool> 苍穹振击取消后摇(字节数组包含长宽 数组)
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

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            苍穹振击后();
            return await FromResult(false).ConfigureAwait(true);
        }

        /// <summary>
        ///     前摇时间基本再380-450 之间
        /// </summary>
        /// <param name="bts"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static async Task<bool> 枷锁持续施法隐身(字节数组包含长宽 数组)
        {
            static void 枷锁后(in 字节数组包含长宽 数组)
            {
            }

            if (DOTA2判断技能是否CD(Keys.E, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            枷锁后(in 数组);
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 释放群蛇守卫取消后摇(字节数组包含长宽 数组)
        {
            static void 群蛇守卫后()
            {
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            群蛇守卫后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 变羊取消后摇(字节数组包含长宽 数组)
        {
            static void 萨满变羊后(字节数组包含长宽 数组)
            {
                初始化全局时间(ref _全局时间w);

                Run(() =>
                {
                    int time = 1250;

                    Color 技能点颜色 = Color.FromArgb(203, 183, 124);

                    if (ColorAEqualColorB(获取指定位置颜色(909, 1008, in 数组), 技能点颜色, 0))
                    {
                        time = 3400;
                    }
                    else if (ColorAEqualColorB(获取指定位置颜色(897, 1008, in 数组), 技能点颜色, 0))
                    {
                        time = 2650;
                    }
                    else if (ColorAEqualColorB(获取指定位置颜色(885, 1008, in 数组), 技能点颜色, 0))
                    {
                        time = 1900;
                    }
                    else if (ColorAEqualColorB(获取指定位置颜色(875, 1008, in 数组), 技能点颜色, 0))
                    {
                        time = 1150;
                    }

                    time = Convert.ToInt32(_状态抗性倍数 * time);
#if 检测延时
                        Tts.Speak(string.Concat("延时", time.ToString()));
#endif

                    SimKeyBoard.KeyPress(Keys.A);

                    switch (_全局模式w)
                    {
                        case 1:
                            Delay(time - 435, _全局时间w);
                            SimKeyBoard.KeyPress(Keys.E);
                            break;
                        case 2:
                            SimKeyBoard.KeyPress(Keys.Q);
                            break;
                        case 3:
                            SimKeyBoard.KeyPress(Keys.Q);
                            Delay(time - 435, _全局时间w);
                            SimKeyBoard.KeyPress(Keys.E);
                            break;
                        case 4:
                            SimKeyBoard.KeyPress(Keys.R);
                            Delay(400);
                            SimKeyBoard.KeyPress(Keys.Q);
                            Delay(time - 435, _全局时间w);
                            SimKeyBoard.KeyPress(Keys.E);
                            break;
                    }
                });
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            萨满变羊后(数组);
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 小仙女

        private static async Task<bool> 无限暗影之境(字节数组包含长宽 数组)
        {
            _ = await 主动技能已就绪后续(Keys.W, () =>
            {
                SimKeyBoard.KeyPress(Keys.W);
                Delay(50);
            }).ConfigureAwait(true);
            return await FromResult(_循环条件1).ConfigureAwait(true);
        }

        #endregion

        #region 炸弹人

        private static async Task<bool> 粘性炸弹去后摇(字节数组包含长宽 数组)
        {
            static void 粘性炸弹后()
            {
                //RightClick();
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            粘性炸弹后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 活性电击去后摇(字节数组包含长宽 数组)
        {
            static void 活性电击后()
            {
                //RightClick();
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            活性电击后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 爆破起飞去后摇(字节数组包含长宽 数组)
        {
            static void 爆破起飞后()
            {
                //RightClick();
                SimKeyBoard.KeyPress(Keys.A);
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

            if (DOTA2判断技能是否CD(Keys.E, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            爆破起飞后();
            return await FromResult(false).ConfigureAwait(true);
        }

        // todo 逻辑修改
        private static async Task<bool> 爆破后接3雷粘性炸弹(字节数组包含长宽 数组)
        {
            if (获取当前时间毫秒() - _全局时间r >= 3000)
            {
                _全局时间r = -1;
                return await FromResult(false).ConfigureAwait(true);
            }

            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 神域

        private static async Task<bool> 命运敕令去后摇(字节数组包含长宽 数组)
        {
            static async Task 命运敕令后()
            {
                await Run(SimKeyBoard.MouseRightClick).ConfigureAwait(true);

                // KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            命运敕令后().Start();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 涤罪之焰去后摇(字节数组包含长宽 数组)
        {
            static async Task 涤罪之焰后()
            {
                await Run(() => { SimKeyBoard.KeyPress(Keys.A); }).ConfigureAwait(true);
                // RightClick();
            }

            if (DOTA2判断技能是否CD(Keys.E, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            涤罪之焰后().Start();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 虚妄之诺去后摇(字节数组包含长宽 数组)
        {
            static async Task 虚妄之诺后()
            {
                await Run(() => { SimKeyBoard.KeyPress(Keys.A); }).ConfigureAwait(true);
                // KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            虚妄之诺后().Start();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 天命之雨去后摇(字节数组包含长宽 数组)
        {
            static void 天命之雨后()
            {
                SimKeyBoard.MouseRightClick();
                // KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.D, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            天命之雨后();
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 莱恩

        private static async Task<bool> 莱恩羊接技能(字节数组包含长宽 数组)
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

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            莱恩羊后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 死亡一指去后摇(字节数组包含长宽 数组)
        {
            static void 死亡一指后()
            {
                //RightClick();
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            死亡一指后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 大招前纷争(字节数组包含长宽 数组)
        {
            Delay(33 * (
                根据图片使用物品(物品_虚灵之刃_数组)
                + 根据图片使用物品(物品_纷争_数组)
                + 根据图片使用物品(物品_红杖_数组)
                + 根据图片使用物品(物品_红杖2_数组)
                + 根据图片使用物品(物品_红杖3_数组)
                + 根据图片使用物品(物品_红杖4_数组)
                + 根据图片使用物品(物品_红杖5_数组)
            ));
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 推推破林肯秒羊(字节数组包含长宽 数组)
        {
            if (根据图片使用物品(物品_推推棒_数组) == 1)
            {
                Delay(等待延迟);
                return await FromResult(true).ConfigureAwait(true);
            }

            SimKeyBoard.KeyPress(Keys.W);
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 羊刺刷新秒人(字节数组包含长宽 数组)
        {
            int 步骤 = 获取全局步骤();

            if (步骤 == 1)
            {
                if (根据图片使用物品(物品_奥术鞋_数组) == 1)
                {
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (根据图片使用物品(物品_魂戒_数组) == 1)
                {
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (DOTA2判断技能是否CD(Keys.Q, in 数组))
                {
                    SimKeyBoard.KeyPress(Keys.Q);
                    Delay(60);
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (DOTA2判断技能是否CD(Keys.R, in 数组))
                {
                    SimKeyBoard.KeyPress(Keys.R);
                    Delay(60);
                    return await FromResult(true).ConfigureAwait(true);
                }
            }
            else if (步骤 == 0)
            {
                if (根据图片使用物品(物品_跳刀_数组) == 1)
                {
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (根据图片使用物品(物品_跳刀_智力跳刀_数组) == 1)
                {
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (DOTA2判断技能是否CD(Keys.W, in 数组))
                {
                    SimKeyBoard.KeyPress(Keys.W);
                    Delay(等待延迟);
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (DOTA2判断技能是否CD(Keys.Q, in 数组))
                {
                    SimKeyBoard.KeyPress(Keys.Q);
                    Delay(60);
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (根据图片使用物品(物品_奥术鞋_数组) == 1)
                {
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (根据图片使用物品(物品_魂戒_数组) == 1)
                {
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (DOTA2判断技能是否CD(Keys.R, in 数组))
                {
                    SimKeyBoard.KeyPress(Keys.R);
                    Delay(60);
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (_条件5 && 根据图片使用物品(物品_刷新球_数组) == 1)
                {
                    设置全局步骤(1);
                    Delay(120);
                    return await FromResult(true).ConfigureAwait(true);
                }
            }

            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 沉默

        private static async Task<bool> 奥数诅咒去后摇(字节数组包含长宽 数组)
        {
            static async void 奥数诅咒后(字节数组包含长宽 数组)
            {
                _全局时间q = -1;
                // RightClick();
                // KeyPress(Keys.A);
                switch (_全局模式q)
                {
                    case < 1:
                        _ = await 大招前纷争(数组).ConfigureAwait(true);
                        SimKeyBoard.KeyPress(Keys.E);
                        break;
                    case 1:
                        _ = await 大招前纷争(数组).ConfigureAwait(true);
                        Delay(1300);
                        SimKeyBoard.KeyPress(Keys.E);
                        break;
                    case 2:
                        SimKeyBoard.KeyPress(Keys.A);
                        break;
                }
            }

            // 超时则切回 总体释放时间
            if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
            {
                奥数诅咒后(数组);
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            奥数诅咒后(数组);
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 全领域沉默去后摇(字节数组包含长宽 数组)
        {
            static void 全领域沉默后()
            {
                _全局时间r = -1;
                // RightClick();
                SimKeyBoard.KeyPress(Keys.A);
            }

            // 超时则切回 总体释放时间
            if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
            {
                全领域沉默后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            全领域沉默后();
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 戴泽

        private static async Task<bool> 剧毒之触去后摇(字节数组包含长宽 数组)
        {
            static void 剧毒之触后()
            {
                _全局时间q = -1;
                通用技能后续动作();
            }

            // 超时则切回 总体释放时间
            if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
            {
                剧毒之触后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            剧毒之触后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 薄葬去后摇(字节数组包含长宽 数组)
        {
            static void 薄葬后()
            {
                _全局时间w = -1;
                通用技能后续动作();
            }

            // 超时则切回 总体释放时间
            if (获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
            {
                薄葬后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            薄葬后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 暗影波去后摇(字节数组包含长宽 数组)
        {
            static void 暗影波后()
            {
                _全局时间e = -1;
                通用技能后续动作();
            }

            // 超时则切回 总体释放时间
            if (获取当前时间毫秒() - _全局时间e > 1200 && _全局时间e != -1)
            {
                暗影波后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.E, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            暗影波后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 邪能去后摇(字节数组包含长宽 数组)
        {
            static void 邪能后()
            {
                _全局时间r = -1;
                通用技能后续动作();
            }

            // 超时则切回 总体释放时间
            if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
            {
                邪能后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            邪能后();
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 双头龙

        private static async Task<bool> 冰火交加去后摇(字节数组包含长宽 数组)
        {
            static void 冰火交加后()
            {
                _全局时间q = -1;
                SimKeyBoard.MouseRightClick();
            }

            // 超时则切回 总体释放时间
            if (获取当前时间毫秒() - _全局时间q > 1200 && _全局时间q != -1)
            {
                冰火交加后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            冰火交加后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 冰封路径去后摇(字节数组包含长宽 数组)
        {
            static void 冰封路径后()
            {
                _全局时间w = -1;
                SimKeyBoard.MouseRightClick();
            }

            // 超时则切回 总体释放时间
            if (获取当前时间毫秒() - _全局时间w > 1200 && _全局时间w != -1)
            {
                冰封路径后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            冰封路径后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 烈焰焚身去后摇(字节数组包含长宽 数组)
        {
            static void 烈焰焚身后()
            {
                _全局时间r = -1;
                // RightClick();
            }

            // 超时则切回 总体释放时间
            if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1)
            {
                烈焰焚身后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            烈焰焚身后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 吹风接冰封路径(字节数组包含长宽 数组)
        {
            if (根据图片使用物品(物品_吹风_数组) == 1)
            {
                Delay(等待延迟);
                return await FromResult(true).ConfigureAwait(true);
            }

            if (!RegPicture(物品_吹风_数组, in 数组) && _全局时间 == -1)
            {
                初始化全局时间(ref _全局时间);
            }

            if (获取当前时间毫秒() - _全局时间 < 2500 - 650 - 600)
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            SimKeyBoard.KeyPress(Keys.W);
            _全局时间 = -1;
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 奶绿

        private static async Task<bool> 弹无虚发去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 唤魂去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 越界去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0, 判断成功后延时: 360).ConfigureAwait(true);
        }

        private static async Task<bool> 临别一枪去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.D, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 祭台去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.F, 0).ConfigureAwait(true);
        }

        #endregion

        #region 女王

        private static async Task<bool> 暗影突袭去后摇(字节数组包含长宽 数组)
        {
            static void 暗影突袭后()
            {
                _全局时间q = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            暗影突袭后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 闪烁去后摇(字节数组包含长宽 数组)
        {
            static void 闪烁后()
            {
                _全局时间w = -1;
                SimKeyBoard.MouseRightClick();
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            闪烁后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 痛苦尖叫去后摇(字节数组包含长宽 数组)
        {
            static void 痛苦尖叫后()
            {
                _全局时间e = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.E, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            痛苦尖叫后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 冲击波去后摇(字节数组包含长宽 数组)
        {
            static void 冲击波后()
            {
                _全局时间r = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (DOTA2判断技能是否CD(Keys.R, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            冲击波后();
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 蓝胖

        private static async Task<bool> 火焰轰爆去后摇(字节数组包含长宽 数组)
        {
            static void 火焰轰爆后()
            {
                SimKeyBoard.MouseRightClick();
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            火焰轰爆后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 引燃去后摇(字节数组包含长宽 数组)
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

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            引燃后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 嗜血术去后摇(字节数组包含长宽 数组)
        {
            static void 嗜血术后()
            {
                SimKeyBoard.MouseRightClick();
            }

            if (DOTA2判断技能是否CD(Keys.E, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            嗜血术后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 未精通火焰轰爆去后摇(字节数组包含长宽 数组)
        {
            static void 未精通火焰轰爆后()
            {
                SimKeyBoard.MouseRightClick();
            }

            if (DOTA2判断技能是否CD(Keys.D, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            未精通火焰轰爆后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 烈火护盾去后摇(字节数组包含长宽 数组)
        {
            static void 烈火护盾后()
            {
                SimKeyBoard.MouseRightClick();
            }

            if (DOTA2判断技能是否CD(Keys.F, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            烈火护盾后();
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 祸乱之源

        private static async Task<bool> 虚弱去后摇(字节数组包含长宽 数组)
        {
            static void 虚弱后()
            {
                通用技能后续动作(false);
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            虚弱后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 噬脑去后摇(字节数组包含长宽 数组)
        {
            static void 噬脑后()
            {
                通用技能后续动作();
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            噬脑后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 噩梦接平A锤(字节数组包含长宽 数组)
        {
            return await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 瘟疫法师

        private static async Task<bool> 死亡脉冲去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 0, 是否接按键: false).ConfigureAwait(true);
        }

        private static async Task<bool> 幽魂护罩去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 0, 是否接按键:false).ConfigureAwait(true);
        }

        private static async Task<bool> 死神镰刀去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 0).ConfigureAwait(true);
        }

        private static async Task<bool> 无限死亡脉冲(字节数组包含长宽 数组)
        {
            _ = await 主动技能已就绪后续(Keys.Q, () =>
            {
                SimKeyBoard.KeyPress(Keys.Q);
                Delay(50);
            }).ConfigureAwait(true);
            return await FromResult(_循环条件1).ConfigureAwait(true);
        }

        #endregion

        #endregion

        #region 全才

        #region 剧毒

        private static async Task<bool> 瘴气去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 蛇棒去后摇(字节数组包含长宽 数组)
        {
            SimEnigo.MouseRightClick();
            return await Task.FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 恶性瘟疫去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 循环蛇棒(字节数组包含长宽 数组)
        {
            _ = await 主动技能已就绪后续(Keys.E, () =>
            {
                SimKeyBoard.KeyPress(Keys.E);
                Delay(50);
            }).ConfigureAwait(true);
            return await FromResult(_循环条件1).ConfigureAwait(true);
        }

        #endregion

        #region 猛犸

        private static async Task<bool> 震荡波去后摇(字节数组包含长宽 数组)
        {
            static void 震荡波后()
            {
                通用技能后续动作();
            }

            震荡波后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 授予力量去后摇(字节数组包含长宽 数组)
        {
            static void 授予力量后()
            {
                通用技能后续动作();
            }

            授予力量后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 巨角冲撞去后摇(字节数组包含长宽 数组)
        {
            static void 巨角冲撞后()
            {
                通用技能后续动作();
            }

            巨角冲撞后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 长角抛物去后摇(字节数组包含长宽 数组)
        {
            static void 长角抛物后()
            {
                通用技能后续动作();
            }

            长角抛物后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 两级反转去后摇(字节数组包含长宽 数组)
        {
            static void 两级反转后()
            {
                通用技能后续动作();
            }

            两级反转后();
            return await FromResult(false).ConfigureAwait(true);
        }

        // todo 逻辑优化 有鱼叉
        private static void 跳拱指定地点()
        {
            SimKeyBoard.KeyPress(Keys.Space);
            Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.D9);
            SimKeyBoard.MouseMove(_指定地点p);
            Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.E);
            Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.D9);
        }

        #endregion

        #region 狼人

        private static async Task<bool> 招狼去后摇(字节数组包含长宽 数组)
        {
            static void 招狼后()
            {
                _全局时间q = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (获取当前时间毫秒() - _全局时间q > 400 && _全局时间q != -1 && _条件开启切假腿)
            {
                招狼后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.Q, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            招狼后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 嚎叫去后摇(字节数组包含长宽 数组)
        {
            static void 嚎叫后()
            {
                _全局时间w = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (获取当前时间毫秒() - _全局时间w > 400 && _全局时间w != -1 && _条件开启切假腿)
            {
                嚎叫后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.W, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            嚎叫后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 撕咬去后摇(字节数组包含长宽 数组)
        {
            static void 撕咬后()
            {
                _全局时间d = -1;
                SimKeyBoard.KeyPress(Keys.A);
            }

            if (获取当前时间毫秒() - _全局时间d > 400 && _全局时间d != -1 && _条件开启切假腿)
            {
                撕咬后();
                return await FromResult(false).ConfigureAwait(true);
            }

            if (DOTA2判断技能是否CD(Keys.D, in 数组))
            {
                return await FromResult(true).ConfigureAwait(true);
            }

            撕咬后();
            return await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 变狼去后摇(字节数组包含长宽 数组)
        {
            if (获取当前时间毫秒() - _全局时间r > 1200 && _全局时间r != -1 && _条件开启切假腿)
            {
                _全局时间r = -1;
                return await FromResult(false).ConfigureAwait(true);
            }

            return await FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region VS

        private static async Task<bool> 魔法箭去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 恐怖波动去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 移形换位去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.R, 1).ConfigureAwait(true);
        }

        #endregion

        #region 小精灵

        private static async Task<bool> 幽魂检测(字节数组包含长宽 数组)
        {
            return RegPicture(Buff_小精灵_幽魂_数组, in 数组)
                ? await FromResult(true).ConfigureAwait(true)
                : await FromResult(false).ConfigureAwait(true);
        }

        private static async Task<bool> 循环续过载(字节数组包含长宽 数组)
        {
            bool guozai = RegPicture(Buff_小精灵_过载_数组, in 数组);
            if (guozai)
            {
                _全局步骤e = 0;
                return await FromResult(_循环条件2).ConfigureAwait(true);
            }

            _ = await 主动技能已就绪后续(Keys.E, () =>
            {
                if (_全局步骤e == 1)
                {
                    Delay(200);
                }
                SimKeyBoard.KeyPress(Keys.E);
                _全局步骤e = 1;
            }).ConfigureAwait(true);
            return await FromResult(_循环条件2).ConfigureAwait(true);
        }

        #endregion

        #region 小强

        private static async Task<bool> 穿刺去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.Q, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 神智爆裂去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.W, 1).ConfigureAwait(true);
        }

        private static async Task<bool> 尖刺外壳去后摇(字节数组包含长宽 数组)
        {
            return await 使用技能后通用后续(Keys.E, 0, false).ConfigureAwait(true);
        }

        private static async Task<bool> 复仇接穿刺(字节数组包含长宽 数组)
        {
            if (_全局步骤r == 0)
            {
                bool 技能释放 = DOTA2判断技能是否CD(Keys.R, 数组);
                if (!技能释放)
                {
                    设置全局步骤r(1);
                }

                return await FromResult(true).ConfigureAwait(true);
            }
            else if (_全局步骤r == 1)
            {
                _ = Run(() =>
                {
                    Delay(300);
                    设置全局步骤r(2);
                });
                return await FromResult(true).ConfigureAwait(true);
            }
            else
            {
                bool 大招状态 = RegPicture(Buff_小强_大招_数组, 数组);
                if (大招状态)
                {
                    return await FromResult(true).ConfigureAwait(true);
                }

                if (DOTA2释放CD就绪技能(Keys.Q, 数组))
                {
                    return await FromResult(true).ConfigureAwait(true);
                }

                设置全局步骤r(0);
                return await FromResult(false).ConfigureAwait(true);
            }
        }

        #endregion

        #endregion

        #region 其他功能

        #region 吃恢复道具臂章

        private static async Task 切臂章()
        {
            Keys key = 根据图片获取物品按键(物品_臂章_开启_数组);
            SimKeyBoard.KeyPress(key);
            Delay(15);
            _ = 根据图片使用物品(物品_魔棒_数组);
            _ = 根据图片自我使用物品(物品_吊坠_数组);
            _ = 根据图片使用物品(物品_仙草_数组);
            _ = 根据图片使用物品(物品_假腿_力量腿_数组);
            Delay(15);
            SimKeyBoard.KeyPress(key);
            _条件假腿敏捷 = false;
            要求保持假腿();

            _ = await FromResult(false).ConfigureAwait(true);
        }

        #endregion

        #region 快速触发激怒

        private static void 快速触发激怒()
        {
            _指定地点p = MousePosition;

            for (int i = 0; i < 5; i++)
            {
                SimKeyBoard.MouseMove(575 + (_阵营_int * 515) + (61 * i), 20);
                SimKeyBoard.KeyPress(Keys.A);
                Delay(2);
            }

            SimKeyBoard.MouseMove(_指定地点p);
        }

        #endregion

        #region 泉水状态喝瓶子 已经是版本过去了

        //private static void 泉水状态喝瓶()
        //{
        //    Delay(400);

        //    for (var i = 1; i <= 4; i++)
        //    {
        //        KeyPress(Keys.C);
        //        Delay(587);
        //    }
        //}

        //private static void 泉水状态喂瓶()
        //{
        //    Delay(3300);

        //    var time = 获取当前时间毫秒();

        //    for (var i = 1; i <= 10; i++)
        //    {
        //        if (获取当前时间毫秒() - time > 1850) return;

        //        KeyDown(Keys.LControl);
        //        KeyDown(Keys.C);
        //        KeyUp(Keys.LControl);
        //        KeyUp(Keys.C);

        //        Delay(587);
        //    }
        //}

        #endregion

        #region 指定地点

        private static void 指定地点()
        {
            _指定地点p = MousePosition;

            Delay(等待延迟);
            SimKeyBoard.KeyDown(Keys.Control);
            Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.D9);
            Delay(等待延迟);
            SimKeyBoard.KeyUp(Keys.Control);
        }

        #endregion

        #region 跳刀

        /// <summary>
        ///     用于快速先手无转身
        /// </summary>
        /// <returns></returns>
        private static async Task<Point> 正面跳刀_无转身(字节数组包含长宽 数组)
        {
            // 坐标
            Point mousePosition = MousePosition;

            // X间距
            double moveX = 0;
            // Y间距，自动根据X调整
            double moveY = 0;

            Point p = await 快速获取自身坐标().ConfigureAwait(true);

#if DEBUG
                Tts.Speak(string.Concat("自身坐标为:", p.X + 55, "  ", p.Y + 80));
                Delay(2000);
#endif
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

            return await FromResult(new Point(Convert.ToInt16(moveX), Convert.ToInt16(moveY))).ConfigureAwait(true);
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

                long currentTime = 获取当前时间毫秒();

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
            SimKeyBoard.KeyPress(Keys.S);
            Delay(40);
            SimKeyBoard.KeyPress(Keys.F1);
            Delay(40);
            SimKeyBoard.KeyPress(Keys.F1);

            using PooledList<string> list1 = [.. tb_阵营.Text.Split(',')];

            try
            {
                switch (_技能数量)
                {
                    case 6:
                        foreach (string t in list1)
                        {
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
                        }

                        break;
                    case 4:
                        foreach (string t in list1)
                        {
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
                        }

                        break;
                    case 5:
                        foreach (string t in list1)
                        {
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
            SimKeyBoard.MouseMove(p);
            SimKeyBoard.MouseLeftDown();
            Delay(40);
            SimKeyBoard.MouseMove(new Point(p.X + 5, p.Y + 5));
            Delay(40);
            SimKeyBoard.KeyDown(Keys.Y);
            Delay(40);
            SimKeyBoard.MouseLeftUp();
            SimKeyBoard.KeyUp(Keys.Y);
            Delay(40);
        }

        private void 捡装备()
        {
            using PooledList<string> list1 = new(tb_阵营.Text.Split(','));
            SimKeyBoard.KeyDown(Keys.Y);
            Delay(40);
            for (int i = 0; i < list1.Count + 2; i++)
            {
                SimKeyBoard.MouseRightClick();
                Delay(100);
            }

            list1.Dispose();
            SimKeyBoard.KeyUp(Keys.Y);
        }

        #endregion

        #region 切假腿

        /// <summary>
        ///     更少更多的截图基本时间一致但对比时间有区别
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static async Task 根据技能数量更新假腿需要图片()
        {
            _全局假腿数组 = new 字节数组包含长宽(_全局数组.字节数组, _全局数组.图片尺寸);
            await Task.Delay(0).ConfigureAwait(true);
        }

        /// <summary>
        ///     6ms 主要是重新截图时间，不然的话只需要0.5ms
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static async Task 切假腿(string type)
        {
            await 根据技能数量更新假腿需要图片().ConfigureAwait(true);

            _ = await 切假腿类型(type).ConfigureAwait(true);
        }

        private static async Task 技能释放前切假腿(string 类型)
        {
            if (_条件开启切假腿 && _条件保持假腿 && _存在假腿)
            {
                _条件保持假腿 = false;
                _需要切假腿 = false;
                _ = await 切假腿类型(类型).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="parByte"></param>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private static async Task<bool> 切假腿类型(string type)
        {
            bool 切腿成功 = type switch
            {
                "智力" => 根据图片使用物品(物品_假腿_力量腿_数组) == 1 ||
                        根据图片多次使用物品(物品_假腿_敏捷腿_数组, 2, 33) == 1,
                "敏捷" => 根据图片使用物品(物品_假腿_智力腿_数组) == 1 ||
                        根据图片多次使用物品(物品_假腿_力量腿_数组, 2, 33) == 1,
                "力量" => 根据图片使用物品(物品_假腿_敏捷腿_数组) == 1 ||
                        根据图片多次使用物品(物品_假腿_智力腿_数组, 2, 33) == 1,
                _ => false
            };

            return await FromResult(切腿成功).ConfigureAwait(true);
        }

        #endregion

        #region 分身一齐攻击

        /// <summary>
        ///     因为有0.1秒的分裂时间，所以必须等待
        /// </summary>
        private static void 分身一齐攻击()
        {
            Delay(140);
            SimKeyBoard.KeyDown(Keys.Control);
            SimKeyBoard.KeyPress(Keys.A);
            SimKeyBoard.KeyUp(Keys.Control);
        }

        #endregion

        #region 用到的所有图片全局变量

        #region 状态buff

        private static 字节数组包含长宽 Buff_大牛_回魂_数组 = new();
        private static 字节数组包含长宽 Buff_光法_大招_数组 = new();
        private static 字节数组包含长宽 Buff_小精灵_幽魂_数组 = new();
        private static 字节数组包含长宽 Buff_小精灵_过载_数组 = new();
        private static 字节数组包含长宽 Buff_物品_TP_数组 = new();
        private static 字节数组包含长宽 Buff_火猫_无影拳_数组 = new();
        private static 字节数组包含长宽 Buff_小强_大招_数组 = new();

        #endregion

        #region 命石

        private static 字节数组包含长宽 命石_伐木机_碎木击_数组 = new();
        private static 字节数组包含长宽 命石_伐木机_锯齿轮旋_数组 = new();
        private static 字节数组包含长宽 命石_海民_酒友_数组 = new();
        private static 字节数组包含长宽 命石_骷髅王_白骨守卫_数组 = new();

        #endregion

        #region 英雄技能

        private static 字节数组包含长宽 技能_卡尔_幽灵漫步_数组 = new();
        private static 字节数组包含长宽 技能_卡尔_强袭飓风_数组 = new();
        private static 字节数组包含长宽 技能_卡尔_极速冷却_数组 = new();
        private static 字节数组包含长宽 技能_卡尔_电磁脉冲_数组 = new();

        #endregion

        #region 播报信息

        private static 字节数组包含长宽 播报_买活_数组 = new();
        private static 字节数组包含长宽 播报_塔防标志_数组 = new();
        private static 字节数组包含长宽 播报_盾标志_数组 = new();

        #endregion

        #region 物品

        private static 字节数组包含长宽 物品_以太_数组 = new();
        private static 字节数组包含长宽 物品_假腿_力量腿_数组 = new();
        private static 字节数组包含长宽 物品_假腿_敏捷腿_数组 = new();
        private static 字节数组包含长宽 物品_假腿_智力腿_数组 = new();
        private static 字节数组包含长宽 物品_刃甲_数组 = new();
        private static 字节数组包含长宽 物品_刷新球_数组 = new();
        private static 字节数组包含长宽 物品_否决_数组 = new();
        private static 字节数组包含长宽 物品_吹风_数组 = new();
        private static 字节数组包含长宽 物品_天堂_数组 = new();
        private static 字节数组包含长宽 物品_奥术鞋_数组 = new();
        private static 字节数组包含长宽 物品_希瓦_数组 = new();
        private static 字节数组包含长宽 物品_飓风长戟_数组 = new();
        private static 字节数组包含长宽 物品_青莲宝珠_数组 = new();
        private static 字节数组包含长宽 物品_幻影斧_数组 = new();
        private static 字节数组包含长宽 物品_影之灵龛_数组 = new();
        private static 字节数组包含长宽 物品_推推棒_数组 = new();
        private static 字节数组包含长宽 物品_微光披风_数组 = new();
        private static 字节数组包含长宽 物品_隐刀_数组 = new();
        private static 字节数组包含长宽 物品_大隐刀_数组 = new();
        private static 字节数组包含长宽 物品_散失_数组 = new();
        private static 字节数组包含长宽 物品_散魂_数组 = new();
        private static 字节数组包含长宽 物品_暗影护符_数组 = new();
        private static 字节数组包含长宽 物品_暗影护符buff_数组 = new();
        private static 字节数组包含长宽 物品_永世法衣_数组 = new();
        private static 字节数组包含长宽 物品_永恒遗物_数组 = new();
        private static 字节数组包含长宽 物品_深渊之刃_数组 = new();
        private static 字节数组包含长宽 物品_雷神之锤_数组 = new();
        private static 字节数组包含长宽 物品_雷神之锤_虚空至宝_数组 = new();
        private static 字节数组包含长宽 物品_炎阳勋章_数组 = new();
        private static 字节数组包含长宽 物品_玲珑心_数组 = new();
        private static 字节数组包含长宽 物品_魔棒_数组 = new();
        private static 字节数组包含长宽 物品_吊坠_数组 = new();
        private static 字节数组包含长宽 物品_仙草_数组 = new();
        private static 字节数组包含长宽 物品_疯狂面具_数组 = new();
        private static 字节数组包含长宽 物品_疯狂面具_虚空至宝_数组 = new();
        private static 字节数组包含长宽 物品_相位鞋_数组 = new();
        private static 字节数组包含长宽 物品_祭礼长袍_数组 = new();
        private static 字节数组包含长宽 物品_紫苑_数组 = new();
        private static 字节数组包含长宽 物品_红杖_数组 = new();
        private static 字节数组包含长宽 物品_红杖2_数组 = new();
        private static 字节数组包含长宽 物品_红杖3_数组 = new();
        private static 字节数组包含长宽 物品_红杖4_数组 = new();
        private static 字节数组包含长宽 物品_红杖5_数组 = new();
        private static 字节数组包含长宽 物品_纷争_数组 = new();
        private static 字节数组包含长宽 物品_纷争_被控_数组 = new();
        private static 字节数组包含长宽 物品_缚灵锁_数组 = new();
        private static 字节数组包含长宽 物品_羊刀_数组 = new();
        private static 字节数组包含长宽 物品_臂章_数组 = new();
        private static 字节数组包含长宽 物品_臂章_开启_数组 = new();
        private static 字节数组包含长宽 物品_虚灵_被控_数组 = new();
        private static 字节数组包含长宽 物品_虚灵之刃_数组 = new();
        private static 字节数组包含长宽 物品_血棘_数组 = new();
        private static 字节数组包含长宽 物品_血精石_数组 = new();
        private static 字节数组包含长宽 物品_跳刀_数组 = new();
        private static 字节数组包含长宽 物品_跳刀_力量跳刀_数组 = new();
        private static 字节数组包含长宽 物品_跳刀_敏捷跳刀_数组 = new();
        private static 字节数组包含长宽 物品_跳刀_智力跳刀_数组 = new();
        private static 字节数组包含长宽 物品_释放天堂_数组 = new();
        private static 字节数组包含长宽 物品_阿托斯之棍_数组 = new();
        private static 字节数组包含长宽 物品_陨星锤_数组 = new();
        private static 字节数组包含长宽 物品_魂之灵龛_数组 = new();
        private static 字节数组包含长宽 物品_魂戒_数组 = new();
        private static 字节数组包含长宽 物品_鱼叉_数组 = new();
        private static 字节数组包含长宽 物品_黑黄杖_数组 = new();

        #endregion

        #endregion

        #region 初始化DOTA2用到的图片

        private static void 初始化DOTA2用到的图片()
        {
            获取嵌入的图片("Buff_大牛_回魂", ref Buff_大牛_回魂_数组);
            获取嵌入的图片("Buff_光法_大招", ref Buff_光法_大招_数组);
            获取嵌入的图片("Buff_小精灵_幽魂", ref Buff_小精灵_幽魂_数组);
            获取嵌入的图片("Buff_小精灵_过载", ref Buff_小精灵_过载_数组);
            获取嵌入的图片("Buff_物品_TP", ref Buff_物品_TP_数组);
            获取嵌入的图片("Buff_火猫_无影拳", ref Buff_火猫_无影拳_数组);
            获取嵌入的图片("Buff_小强_大招", ref Buff_小强_大招_数组);

            获取嵌入的图片("命石_伐木机_碎木击", ref 命石_伐木机_碎木击_数组);
            获取嵌入的图片("命石_伐木机_锯齿轮旋", ref 命石_伐木机_锯齿轮旋_数组);
            获取嵌入的图片("命石_海民_酒友", ref 命石_海民_酒友_数组);
            获取嵌入的图片("命石_骷髅王_白骨守卫", ref 命石_骷髅王_白骨守卫_数组);

            获取嵌入的图片("技能_卡尔_幽灵漫步", ref 技能_卡尔_幽灵漫步_数组);
            获取嵌入的图片("技能_卡尔_强袭飓风", ref 技能_卡尔_强袭飓风_数组);
            获取嵌入的图片("技能_卡尔_极速冷却", ref 技能_卡尔_极速冷却_数组);
            获取嵌入的图片("技能_卡尔_电磁脉冲", ref 技能_卡尔_电磁脉冲_数组);

            获取嵌入的图片("播报_买活", ref 播报_买活_数组);
            获取嵌入的图片("播报_塔防标志", ref 播报_塔防标志_数组);
            获取嵌入的图片("播报_盾标志", ref 播报_盾标志_数组);

            获取嵌入的图片("物品_以太", ref 物品_以太_数组);
            获取嵌入的图片("物品_假腿_力量腿", ref 物品_假腿_力量腿_数组);
            获取嵌入的图片("物品_假腿_敏捷腿", ref 物品_假腿_敏捷腿_数组);
            获取嵌入的图片("物品_假腿_智力腿", ref 物品_假腿_智力腿_数组);
            获取嵌入的图片("物品_刃甲", ref 物品_刃甲_数组);
            获取嵌入的图片("物品_刷新球", ref 物品_刷新球_数组);
            获取嵌入的图片("物品_否决", ref 物品_否决_数组);
            获取嵌入的图片("物品_吹风", ref 物品_吹风_数组);
            获取嵌入的图片("物品_天堂", ref 物品_天堂_数组);
            获取嵌入的图片("物品_奥术鞋", ref 物品_奥术鞋_数组);
            获取嵌入的图片("物品_希瓦", ref 物品_希瓦_数组);
            获取嵌入的图片("物品_青莲宝珠", ref 物品_青莲宝珠_数组);
            获取嵌入的图片("物品_飓风长戟", ref 物品_飓风长戟_数组);
            获取嵌入的图片("物品_幻影斧", ref 物品_幻影斧_数组);
            获取嵌入的图片("物品_影之灵龛", ref 物品_影之灵龛_数组);
            获取嵌入的图片("物品_推推棒", ref 物品_推推棒_数组);
            获取嵌入的图片("物品_微光披风", ref 物品_微光披风_数组);
            获取嵌入的图片("物品_隐刀", ref 物品_隐刀_数组);
            获取嵌入的图片("物品_大隐刀", ref 物品_大隐刀_数组);
            获取嵌入的图片("物品_散失", ref 物品_散失_数组);
            获取嵌入的图片("物品_散魂", ref 物品_散魂_数组);
            获取嵌入的图片("物品_暗影护符", ref 物品_暗影护符_数组);
            获取嵌入的图片("物品_暗影护符buff", ref 物品_暗影护符buff_数组);
            获取嵌入的图片("物品_永世法衣", ref 物品_永世法衣_数组);
            获取嵌入的图片("物品_永恒遗物", ref 物品_永恒遗物_数组);
            获取嵌入的图片("物品_深渊之刃", ref 物品_深渊之刃_数组);
            获取嵌入的图片("物品_雷神之锤", ref 物品_雷神之锤_数组);
            获取嵌入的图片("物品_雷神之锤_虚空至宝", ref 物品_雷神之锤_虚空至宝_数组);
            获取嵌入的图片("物品_炎阳勋章", ref 物品_炎阳勋章_数组);
            获取嵌入的图片("物品_玲珑心", ref 物品_玲珑心_数组);
            获取嵌入的图片("物品_魔棒", ref 物品_魔棒_数组);
            获取嵌入的图片("物品_吊坠", ref 物品_吊坠_数组);
            获取嵌入的图片("物品_仙草", ref 物品_仙草_数组);
            获取嵌入的图片("物品_疯狂面具", ref 物品_疯狂面具_数组);
            获取嵌入的图片("物品_疯狂面具_虚空至宝", ref 物品_疯狂面具_虚空至宝_数组);
            获取嵌入的图片("物品_相位鞋", ref 物品_相位鞋_数组);
            获取嵌入的图片("物品_祭礼长袍", ref 物品_祭礼长袍_数组);
            获取嵌入的图片("物品_紫苑", ref 物品_紫苑_数组);
            获取嵌入的图片("物品_红杖", ref 物品_红杖_数组);
            获取嵌入的图片("物品_红杖2", ref 物品_红杖2_数组);
            获取嵌入的图片("物品_红杖3", ref 物品_红杖3_数组);
            获取嵌入的图片("物品_红杖4", ref 物品_红杖4_数组);
            获取嵌入的图片("物品_红杖5", ref 物品_红杖5_数组);
            获取嵌入的图片("物品_纷争", ref 物品_纷争_数组);
            获取嵌入的图片("物品_纷争_被控", ref 物品_纷争_被控_数组);
            获取嵌入的图片("物品_缚灵锁", ref 物品_缚灵锁_数组);
            获取嵌入的图片("物品_羊刀", ref 物品_羊刀_数组);
            获取嵌入的图片("物品_臂章", ref 物品_臂章_数组);
            获取嵌入的图片("物品_臂章_开启", ref 物品_臂章_开启_数组);
            获取嵌入的图片("物品_虚灵_被控", ref 物品_虚灵_被控_数组);
            获取嵌入的图片("物品_虚灵之刃", ref 物品_虚灵之刃_数组);
            获取嵌入的图片("物品_血棘", ref 物品_血棘_数组);
            获取嵌入的图片("物品_血精石", ref 物品_血精石_数组);
            获取嵌入的图片("物品_跳刀", ref 物品_跳刀_数组);
            获取嵌入的图片("物品_跳刀_力量跳刀", ref 物品_跳刀_力量跳刀_数组);
            获取嵌入的图片("物品_跳刀_敏捷跳刀", ref 物品_跳刀_敏捷跳刀_数组);
            获取嵌入的图片("物品_跳刀_智力跳刀", ref 物品_跳刀_智力跳刀_数组);
            获取嵌入的图片("物品_释放天堂", ref 物品_释放天堂_数组);
            获取嵌入的图片("物品_阿托斯之棍", ref 物品_阿托斯之棍_数组);
            获取嵌入的图片("物品_陨星锤", ref 物品_陨星锤_数组);
            获取嵌入的图片("物品_魂之灵龛", ref 物品_魂之灵龛_数组);
            获取嵌入的图片("物品_魂戒", ref 物品_魂戒_数组);
            获取嵌入的图片("物品_鱼叉", ref 物品_鱼叉_数组);
            获取嵌入的图片("物品_黑黄杖", ref 物品_黑黄杖_数组);
        }

        private static void 获取嵌入的图片(string bpName, ref 字节数组包含长宽 具体数组)
        {
            // 获取当前程序集
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 指定嵌入资源的命名空间和文件名
            string resourceName = $"Dota2Simulator.Picture_Dota2.{bpName}.bmp";
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using Bitmap bitmap = new(stream);

                具体数组.字节数组 = GetBitmapByte(bitmap);
                具体数组.图片尺寸 = bitmap.Size;
            }
            else
            {
                Tts.Speak($"{bpName}图片不存在");
            }
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
            long a = 获取当前时间毫秒() - ln;

            await Run(() =>
            {
                Delay(delay);
                Tts.Speak(string.Concat("经过时间", a));
            }).ConfigureAwait(false);
        }

        #endregion

        #region 记录买活

        //private static void 记录买活()
        //{
        //    var 计时颜色 = Color.FromArgb(14, 19, 24);

        //    while (true)
        //    {
        //        if (RegPicture(Resource_Picture.播报_买活, 549, 41, 52, 21) && !CaptureColor(559, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;
        //            while (!CaptureColor(559, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(559, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
        //        }

        //        if (RegPicture(Resource_Picture.播报_买活, 613, 41, 52, 21) && !CaptureColor(623, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;

        //            while (!CaptureColor(623, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(623, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
        //        }

        //        if (RegPicture(Resource_Picture.播报_买活, 674, 41, 52, 21) && !CaptureColor(688, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;

        //            while (!CaptureColor(688, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(688, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
        //        }

        //        if (RegPicture(Resource_Picture.播报_买活, 735, 41, 52, 21) && !CaptureColor(749, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;

        //            while (!CaptureColor(749, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(749, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
        //        }

        //        if (RegPicture(Resource_Picture.播报_买活, 797, 41, 52, 21) && !CaptureColor(811, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;

        //            while (!CaptureColor(811, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(811, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
        //        }

        //        if (RegPicture(Resource_Picture.播报_买活, 1060, 41, 52, 21) && !CaptureColor(1073, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;

        //            while (!CaptureColor(1073, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(1073, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
        //        }

        //        if (RegPicture(Resource_Picture.播报_买活, 1124, 41, 52, 21) && !CaptureColor(1137, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;

        //            while (!CaptureColor(1137, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(1137, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
        //        }

        //        if (RegPicture(Resource_Picture.播报_买活, 1185, 41, 52, 21) && !CaptureColor(1198, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;

        //            while (!CaptureColor(1198, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(1198, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
        //        }

        //        if (RegPicture(Resource_Picture.播报_买活, 1248, 41, 52, 21) && !CaptureColor(1261, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;

        //            while (!CaptureColor(1261, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(1261, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
        //        }

        //        if (RegPicture(Resource_Picture.播报_买活, 1308, 41, 52, 21) && !CaptureColor(1321, 76).Equals(计时颜色))
        //        {
        //            var p = MousePosition;

        //            while (!CaptureColor(1321, 76).Equals(计时颜色))
        //            {
        //                KeyBoardSim.MouseMove(1321, 76);
        //                Thread.Sleep(30);
        //                LeftClick();
        //                Thread.Sleep(30);
        //            }

        //            KeyBoardSim.MouseMove(p);
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
            using Bitmap bp = CaptureScreen(930, 21, 58, 16);
            string str = PaddleOcr.获取图片文字(bp);
            str = string.Concat("塔防刷新", str.Replace("：", ":"));
            Delay(500);
            快速发言(str);
        }

        #endregion

        #region 快速发言

        private static void 快速发言(string str)
        {
            Clipboard.SetText(str);
            SimKeyBoard.KeyPress(Keys.Enter);
            SimKeyBoard.KeyDown(Keys.Control);
            SimKeyBoard.KeyPress(Keys.V);
            SimKeyBoard.KeyUp(Keys.Control);
            Delay(等待延迟);
            SimKeyBoard.KeyPress(Keys.Enter);
            Delay(等待延迟);
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
            Point p = MousePosition;
            int x = 0;
            int y = 0;

            if (width == 1920 || height == 1080)
            {
                x = 0;
                y = 0;
            }
            else
            {
                x = p.X - (width / 2) < 0 ? 0 : p.X - (width / 2);
                y = p.Y - (height / 2) < 0 ? 0 : p.Y - (height / 2);
            }

            if (type1 == 1)
            {
                Delay(330); // 基本延迟用于迷雾显示
            }

            Size size = new(width, height);
            using Bitmap bp = await CaptureScreenAsync(x, y, size).ConfigureAwait(true);
            byte[] bytes = await GetBitmapByteAsync(bp).ConfigureAwait(true);

            PooledList<Point> list = 获取敌方坐标(size, bytes);

            int 偏移x = 1920;
            int 偏移y = 1080;

#if DEBUG
                foreach (Point item in list)
                {
                    SimKeyBoard.MouseMoveSim(item.X + x + 50, item.Y + y + 80);
                    Tts.Speak(string.Concat("坐标X", item.X + x + 50, "坐标y", item.Y + y + 80));
                    Delay(2000);
                }
#endif

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

            return await FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #region 获取敌方坐标

        private static PooledList<Point> 获取敌方坐标(Size size, in byte[] bytes)
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
#if DEBUG
                初始化全局时间(ref _全局时间);
#endif

            PooledList<Point> list1 = FindColors(colors, points, bytes, size, 1);
#if DEBUG
                检测时间播报(_全局时间, 0);
                //Tts.Speak(string.Concat("1找到",list1.Count));
                Delay(2000);
#endif
            return list1;
        }

        #endregion

        #region 快速获取自身坐标

        private static async Task<Point> 快速获取自身坐标(int width = 1920, int height = 1080)
        {
            Size size = new(width, height);
            Bitmap bp = await CaptureScreenAsync(0, 0, size).ConfigureAwait(true);
            byte[] bytes = await GetBitmapByteAsync(bp).ConfigureAwait(true);
            return await FromResult(获取自身坐标(size, bytes)).ConfigureAwait(true);
        }

        #endregion

        #region 获取自身坐标

        private static Point 获取自身坐标(Size size, in byte[] bytes)
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
#if DEBUG
                初始化全局时间(ref _全局时间);
#endif
            PooledList<Point> list1 = FindColors(colors, points, bytes, size, 1);
#if DEBUG
                检测时间播报(_全局时间, 0);
                //Tts.Speak(string.Concat("1找到",list1.Count));
                Delay(2000);
#endif

            return list1.Count > 0 ? list1[0] : new Point();
        }

        #endregion

        #endregion

#endif

        #endregion

        #region 通用循环

        #region 循环

        private static async Task 一般程序循环()
        {
            async Task 更新条件(bool[] 条件数组, ConditionDelegateBitmap[] 委托数组)
            {
                // 创建任务数组
                Task<bool>[] tasks = 条件数组.Select((条件, index) => 检查条件并执行委托(条件, 委托数组[index])).ToArray();

                // 等待所有任务完成
                bool[] results = await WhenAll(tasks).ConfigureAwait(false);

                for (int i = 0; i < 条件数组.Length; i++)
                {
                    条件数组[i] = results[i];
                }
            }

            ConditionDelegateBitmap[] 创建委托数组()
            {
                return
                [
                    _条件根据图片委托1, _条件根据图片委托2, _条件根据图片委托3, _条件根据图片委托4, _条件根据图片委托5,
                    _条件根据图片委托6, _条件根据图片委托7, _条件根据图片委托8, _条件根据图片委托9, _条件根据图片委托z,
                    _条件根据图片委托x, _条件根据图片委托c, _条件根据图片委托v, _条件根据图片委托b, _条件根据图片委托space
                ];
            }

            void 更新条件数组(bool[] 条件数组)
            {
                条件数组[0] = _条件1;
                条件数组[1] = _条件2;
                条件数组[2] = _条件3;
                条件数组[3] = _条件4;
                条件数组[4] = _条件5;
                条件数组[5] = _条件6;
                条件数组[6] = _条件7;
                条件数组[7] = _条件8;
                条件数组[8] = _条件9;
                条件数组[9] = _条件z;
                条件数组[10] = _条件x;
                条件数组[11] = _条件c;
                条件数组[12] = _条件v;
                条件数组[13] = _条件b;
                条件数组[14] = _条件space;
            }

            bool[] 条件数组 =
            [
                _条件1, _条件2, _条件3, _条件4, _条件5, _条件6, _条件7, _条件8, _条件9,
                _条件z, _条件x, _条件c, _条件v, _条件b, _条件space
            ];

            while (_总循环条件)
            {
                if (_循环内获取图片 is not null)
                {
                    if (_中断条件)
                    {
                        await Task.Delay(1).ConfigureAwait(true);
                        continue;
                    }

                    await _循环内获取图片().ConfigureAwait(true); // 更新全局Bitmap

                    // 如果有获取命石的委托
                    if (_命石根据图片委托 != null)
                    {
                        await _命石根据图片委托(_全局数组).ConfigureAwait(true);
                    }

                    DOTA2获取所有释放技能前颜色(in _全局数组);

                    // 更新条件数组
                    更新条件数组(条件数组);

                    bool[] 原始数组 = (bool[])条件数组.Clone();

                    // 委托数组不会根据修改后更新，是静态的
                    ConditionDelegateBitmap[] 委托数组 = 创建委托数组();

                    Task 更新任务 = 更新条件(条件数组, 委托数组);

                    while (!更新任务.IsCompleted)
                    {
                        // 在Task.WhenAll没有完成之前，持续更新条件数组
                        // 因为后续代码更改为纯异步，无需等待完成，即一开始更新任务就已经完成了。
                        // 根本不会更新
                        更新条件数组(条件数组);
                        await Task.Delay(1).ConfigureAwait(true); // 避免CPU占用过高
                    }

                    await 更新任务.ConfigureAwait(true); // 确保任务完成

                    if (原始数组[0])
                    {
                        _条件1 = 条件数组[0];
                    }

                    if (原始数组[1])
                    {
                        _条件2 = 条件数组[1];
                    }

                    if (原始数组[2])
                    {
                        _条件3 = 条件数组[2];
                    }

                    if (原始数组[3])
                    {
                        _条件4 = 条件数组[3];
                    }

                    if (原始数组[4])
                    {
                        _条件5 = 条件数组[4];
                    }

                    if (原始数组[5])
                    {
                        _条件6 = 条件数组[5];
                    }

                    if (原始数组[6])
                    {
                        _条件7 = 条件数组[6];
                    }

                    if (原始数组[7])
                    {
                        _条件8 = 条件数组[7];
                    }

                    if (原始数组[8])
                    {
                        _条件9 = 条件数组[8];
                    }

                    if (原始数组[9])
                    {
                        _条件z = 条件数组[9];
                    }

                    if (原始数组[10])
                    {
                        _条件x = 条件数组[10];
                    }

                    if (原始数组[11])
                    {
                        _条件c = 条件数组[11];
                    }

                    if (原始数组[12])
                    {
                        _条件v = 条件数组[12];
                    }

                    if (原始数组[13])
                    {
                        _条件b = 条件数组[13];
                    }

                    if (原始数组[14])
                    {
                        _条件space = 条件数组[14];
                    }

#if DOTA2
                    if (_条件保持假腿 && _条件开启切假腿 && _需要切假腿)
                    {
                        await 切假腿处理(_条件假腿敏捷 ? "敏捷" : "力量").ConfigureAwait(true);
                    }
#endif
                }

                await Task.Delay(1).ConfigureAwait(true);
            }
        }

        private static async Task<bool> 检查条件并执行委托(bool 条件, ConditionDelegateBitmap 委托)
        {
            return 条件 && 委托 is not null ? await 委托(_全局数组).ConfigureAwait(true) : 条件;
        }

#if DOTA2
        private static async Task 切假腿处理(string 假腿类型)
        {
            if (_切假腿中)
            {
                return;
            }

            await Run(async () =>
            {
                字节数组包含长宽 数组 = 假腿类型 switch
                {
                    "敏捷" => 物品_假腿_敏捷腿_数组,
                    "力量" => 物品_假腿_力量腿_数组,
                    _ => throw new NotImplementedException()
                };

                if (RegPicture(数组, in _全局数组))
                {
                    return;
                }

                _切假腿中 = true;
                _ = await 切假腿类型(假腿类型).ConfigureAwait(true);
                await Run(() =>
                {
                    Delay(100);
                    _切假腿中 = false;
                    _需要切假腿 = false; // 切假腿完毕，无需再切
                }).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
#endif

        private static async Task 状态初始化()
        {
            _循环内获取图片 ??= 获取图片_1;
            await 一般程序循环().ConfigureAwait(true);
        }

        #region 取消所有功能

        private static void 取消所有功能()
        {
            _总循环条件 = false;
            _循环内获取图片 = null;
            _中断条件 = false;

            _循环条件1 = false;
            _循环条件2 = false;

            _丢装备条件 = false;

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

            _ = 重置耗蓝物品委托和条件();

            _命石选择 = 0;
            _命石根据图片委托 = null;

            _条件开启切假腿 = false;
            _条件保持假腿 = false;
            _条件假腿敏捷 = false;
            _切假腿中 = false;
            _需要切假腿 = false;

            _是否魔晶 = false;
            _是否神杖 = false;

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

            重置所有技能判断();

            // 重置切假腿配置
            _切假腿配置 = new 技能切假腿配置();
            _假腿按键 = Keys.Escape;
        }

        private static void 重置指定技能判断(Keys key)
        {
            if (技能状态字典.TryGetValue(key, out 技能状态 技能状态))
            {
                技能状态.释放中判断 = false;
                技能状态.已释放变色判断 = false;
                技能状态.释放前Color = Color.Empty;
                技能状态.释放后Color = Color.Empty;
            }
        }

        private static void 重置所有技能判断()
        {
            foreach (Keys key in 技能状态字典.Keys)
            {
                技能状态 技能状态 = 技能状态字典[key];
                技能状态.释放中判断 = false;
                技能状态.已释放变色判断 = false;
                技能状态.释放前Color = Color.Empty;
                技能状态.释放后Color = Color.Empty;
            }
        }

        #endregion

        #region 获取图片

        /// <summary>
        ///     获取时间为6.92ms，占程序的大头
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> 获取图片_1()
        {
            // 671 856 760 217 基本所有技能状态物品 .net9 6-7ms 延迟。
            // 671 727 760 346 所有技能状态物品加上施法状态 .net9 6-7ms fps142+
            // 初始化全局图像和数组
            //_全局图像 ??= new Bitmap(截图模式1W, 截图模式1H);

            if (_全局数组 == null || _全局数组.图片尺寸.Width + _全局数组.图片尺寸.Height == 0)
            {
                _全局数组 = new 字节数组包含长宽(new byte[截图模式1W * 截图模式1H * 4], new Size(截图模式1W, 截图模式1H));
            }

            // 更新字节数组
            CaptureScreen_固定数组(ref _全局数组, 截图模式1X, 截图模式1Y);

            //// 捕获屏幕
            //CaptureScreen_固定大小(ref _全局图像, 截图模式1X, 截图模式1Y);

            //// 获取位图字节数组
            //GetBitmapByte_固定数组(in _全局图像, ref _全局数组);

            // 直接返回已完成的任务，减少异步开销
            return await FromResult(true).ConfigureAwait(true);
        }

        private static async Task<bool> 获取图片_2()
        {
            // 优化之后16-17ms fps 58+
            //_全局图像 ??= new Bitmap(1920, 1080);

            if (_全局数组 == null || _全局数组.图片尺寸.Width == 截图模式1W || _全局数组.图片尺寸.Height == 截图模式1H)
            {
                _全局数组 = new 字节数组包含长宽(new byte[1920 * 1080 * 4], new Size(1920, 1080));
            }

            // 更新字节数组
            CaptureScreen_固定数组(ref _全局数组, 0, 0);

            //// 捕获屏幕
            //CaptureScreen_固定大小(ref _全局图像, 0, 0);

            //// 获取位图字节数组
            //GetBitmapByte_固定数组(in _全局图像, ref _全局数组);

            // 直接返回已完成的任务，减少异步开销
            return await FromResult(true).ConfigureAwait(true);
        }

        #endregion

        #endregion

        #endregion

        #region 测试_捕捉颜色

        #region 延时测试

        private void 测试其他功能()
        {
            _全局时间 = 获取当前时间毫秒();

            //using (var duplicator = new DesktopDuplicator())
            //{
            //    for (int i = 0; i < 3; i++)
            //    {
            //        duplicator.Capture(new Rectangle(截图模式1X, 截图模式1Y, 截图模式1W, 截图模式1H));
            //    }
            //}

            //var i1 = Convert.ToInt32(tb_攻速.Text.Trim());

            //var 物品 = tb_阵营.Text switch
            //{
            //    "4" => 物品4,
            //    "5" => 物品5,
            //    "6" => 物品6,
            //    _ => 物品4
            //};

            //var a1 = 获取指定位置颜色(物品.物品锁闭x + i1, 物品.物品锁闭y).Result;
            //var a2 = 获取指定位置颜色(物品.物品锁闭x + 物品.物品间隔x + i1, 物品.物品锁闭y).Result;
            //var a3 = 获取指定位置颜色(物品.物品锁闭x + 物品.物品间隔x + 物品.物品间隔x + i1, 物品.物品锁闭y).Result;
            //var a4 = 获取指定位置颜色(物品.物品锁闭x + i1, 物品.物品锁闭y + 物品.物品间隔y).Result;
            //var a5 = 获取指定位置颜色(物品.物品锁闭x + 物品.物品间隔x + i1, 物品.物品锁闭y + 物品.物品间隔y).Result;
            //var a6 = 获取指定位置颜色(物品.物品锁闭x + 物品.物品间隔x + 物品.物品间隔x + i1, 物品.物品锁闭y+ 物品.物品间隔y).Result;

            //tb_x.Text = ($"1{a1}\r\n2{a2}\r\n3{a3}\r\n4{a4}\r\n5{a5}\r\n6{a6}\r\n");

            //Tts.Speak(PaddleOcr.获取图片文字(@":\Desktop\1.bmp"));

            //Tts.Speak(PaddleOcr.获取图片文字(647, 963, 28, 25));

            for (int i = 0; i < 200; i++)
            {
                _ = 获取图片_2().Result;
            }
            //根据图片使用物品(物品_臂章_开启_数组);
            //DOTA2获取所有释放技能前颜色(in _全局数组);
            Invoke(() => { tb_y.Text = (获取当前时间毫秒() - _全局时间).ToString(); });

            // _ = _全局数组.数组保存为图片("J:\\Desktop\\1.jpg");
        }

        #region 捕捉颜色

        #region 定制测试方法（按需求更改）

        private async Task 测试方法(int x, int y)
        {
            long time = 获取当前时间毫秒();

            PooledList<Color> colors = [];
            PooledList<long> longs = [];

            Size size = new(截图模式1W, 截图模式1H);

            Bitmap bitmap = await CaptureScreenAsync(截图模式1X, 截图模式1Y, size).ConfigureAwait(true);
            字节数组包含长宽 数组 = new(await GetBitmapByteAsync(bitmap).ConfigureAwait(true), size);

            while (true)
            {
                bitmap = await CaptureScreenAsync(截图模式1X, 截图模式1Y, size).ConfigureAwait(true);
                _ = 数组.新赋值数组(await GetBitmapByteAsync(bitmap).ConfigureAwait(true));

                Color p = 获取指定位置颜色(x, y, in 数组);

                if (colors.Count == 0 || !colors[^1].Equals(p))
                {
                    colors.Add(p);
                    longs.Add(获取当前时间毫秒() - time);
                }

                if (获取当前时间毫秒() - time <= int.Parse(tb_状态抗性.Text.Trim()))
                {
                    continue;
                }

                break;
            }

            tb_x.Text = colors.Aggregate("",
                (current, color) => string.Concat(current, color.R.ToString(), ",", color.G.ToString(), ",",
                    color.B.ToString(), "|"));

            string str = "";
            if (longs.Count > 1)
            {
                for (int i = 1; i < longs.Count; i++) // 丢弃第一个颜色获取时间
                {
                    str = string.Concat(str, i > 0 ? (longs[i] - longs[i - 1]).ToString() : longs[0].ToString(), "|");
                }
            }

            tb_y.Text = str;

            colors.Dispose();
            longs.Dispose();

            Tts.Speak("完成");
        }

        #endregion

        #region DOTA2捕捉颜色

        private async Task 捕捉颜色()
        {
            long time = 获取当前时间毫秒();

            PooledList<Color> colors = [];
            PooledList<long> longs = [];

            Size size = new(截图模式1W, 截图模式1H);

            Bitmap bitmap = await CaptureScreenAsync(截图模式1X, 截图模式1Y, size).ConfigureAwait(true);
            字节数组包含长宽 数组 = new(await GetBitmapByteAsync(bitmap).ConfigureAwait(true), size);
            //bitmap.Save("J:\\Desktop\\新建 BMP 图像.bmp");

            while (true)
            {
                bitmap = await CaptureScreenAsync(截图模式1X, 截图模式1Y, size).ConfigureAwait(true);
                _ = 数组.新赋值数组(await GetBitmapByteAsync(bitmap).ConfigureAwait(true));

                Color q4 = 获取技能释放判断颜色(Keys.Q, in 数组, 4);
                Color q5 = 获取技能释放判断颜色(Keys.Q, in 数组, 5);
                Color q6 = 获取技能释放判断颜色(Keys.Q, in 数组, 6);
                Color q41 = 获取技能进入CD判断颜色(Keys.Q, in 数组, 4);
                Color q51 = 获取技能进入CD判断颜色(Keys.Q, in 数组, 5);
                Color q61 = 获取技能进入CD判断颜色(Keys.Q, in 数组, 6);
                Color q45 = 获取推荐技能学习颜色(Keys.Q, in 数组, 4);
                Color q55 = 获取推荐技能学习颜色(Keys.Q, in 数组, 5);
                Color q65 = 获取推荐技能学习颜色(Keys.Q, in 数组, 6);

                Color w4 = 获取技能释放判断颜色(Keys.W, in 数组, 4);
                Color w5 = 获取技能释放判断颜色(Keys.W, in 数组, 5);
                Color w6 = 获取技能释放判断颜色(Keys.W, in 数组, 6);
                Color w41 = 获取技能进入CD判断颜色(Keys.W, in 数组, 4);
                Color w51 = 获取技能进入CD判断颜色(Keys.W, in 数组, 5);
                Color w61 = 获取技能进入CD判断颜色(Keys.W, in 数组, 6);
                Color w45 = 获取推荐技能学习颜色(Keys.W, in 数组, 4);
                Color w55 = 获取推荐技能学习颜色(Keys.W, in 数组, 5);
                Color w65 = 获取推荐技能学习颜色(Keys.W, in 数组, 6);

                Color e4 = 获取技能释放判断颜色(Keys.E, in 数组, 4);
                Color e5 = 获取技能释放判断颜色(Keys.E, in 数组, 5);
                Color e6 = 获取技能释放判断颜色(Keys.E, in 数组, 6);
                Color e41 = 获取技能进入CD判断颜色(Keys.E, in 数组, 4);
                Color e51 = 获取技能进入CD判断颜色(Keys.E, in 数组, 5);
                Color e61 = 获取技能进入CD判断颜色(Keys.E, in 数组, 6);
                Color e45 = 获取推荐技能学习颜色(Keys.E, in 数组, 4);
                Color e55 = 获取推荐技能学习颜色(Keys.E, in 数组, 5);
                Color e65 = 获取推荐技能学习颜色(Keys.E, in 数组, 6);

                Color r4 = 获取技能释放判断颜色(Keys.R, in 数组, 4);
                Color r5 = 获取技能释放判断颜色(Keys.R, in 数组, 5);
                Color r6 = 获取技能释放判断颜色(Keys.R, in 数组, 6);
                Color r41 = 获取技能进入CD判断颜色(Keys.R, in 数组, 4);
                Color r51 = 获取技能进入CD判断颜色(Keys.R, in 数组, 5);
                Color r61 = 获取技能进入CD判断颜色(Keys.R, in 数组, 6);
                Color r45 = 获取推荐技能学习颜色(Keys.R, in 数组, 4);
                Color r55 = 获取推荐技能学习颜色(Keys.R, in 数组, 5);
                Color r65 = 获取推荐技能学习颜色(Keys.R, in 数组, 6);

                Color d5 = 获取技能释放判断颜色(Keys.D, in 数组, 5);
                Color d6 = 获取技能释放判断颜色(Keys.D, in 数组, 6);
                Color d51 = 获取技能进入CD判断颜色(Keys.D, in 数组, 5);
                Color d61 = 获取技能进入CD判断颜色(Keys.D, in 数组, 6);
                Color d55 = 获取推荐技能学习颜色(Keys.D, in 数组, 5);
                Color d65 = 获取推荐技能学习颜色(Keys.D, in 数组, 6);

                Color f6 = 获取技能释放判断颜色(Keys.F, in 数组, 6);
                Color f61 = 获取技能进入CD判断颜色(Keys.F, in 数组, 6);
                Color f65 = 获取推荐技能学习颜色(Keys.F, in 数组, 6);


                Color p = tb_阵营.Text.Trim() switch
                {
                    "q4" => q4,
                    "q5" => q5,
                    "q6" => q6,
                    "q41" => q41,
                    "q51" => q51,
                    "q61" => q61,
                    "q45" => q45,
                    "q55" => q55,
                    "q65" => q65,

                    "w4" => w4,
                    "w5" => w5,
                    "w6" => w6,
                    "w41" => w41,
                    "w51" => w51,
                    "w61" => w61,
                    "w45" => w45,
                    "w55" => w55,
                    "w65" => w65,

                    "e4" => e4,
                    "e5" => e5,
                    "e6" => e6,
                    "e41" => e41,
                    "e51" => e51,
                    "e61" => e61,
                    "e45" => e45,
                    "e55" => e55,
                    "e65" => e65,

                    "r4" => r4,
                    "r5" => r5,
                    "r6" => r6,
                    "r41" => r41,
                    "r51" => r51,
                    "r61" => r61,
                    "r45" => r45,
                    "r55" => r55,
                    "r65" => r65,

                    "d5" => d5,
                    "d6" => d6,
                    "d51" => d51,
                    "d61" => d61,
                    "d55" => d55,
                    "d65" => d65,

                    "f6" => f6,
                    "f61" => f61,
                    "f65" => f65,
                    _ => q4
                };

                if (colors.Count == 0 || !colors[^1].Equals(p))
                {
                    colors.Add(p);
                    longs.Add(获取当前时间毫秒() - time);
                }

                // 用于排除一些颜色，但实际后续不用，会干扰颜色变化
                //if (!ColorAEqualColorB(p, Color.FromArgb(9, 10, 16), 5, 6, 12))
                //    {
                //        colors.Add(p);
                //        longs.Add(获取当前时间毫秒() - time);
                //    }

                if (获取当前时间毫秒() - time <= int.Parse(tb_状态抗性.Text.Trim()))
                {
                    continue;
                }

                break;
            }

            tb_x.Text = colors.Aggregate("",
                (current, color) => string.Concat(current, color.R.ToString(), ",", color.G.ToString(), ",",
                    color.B.ToString(), "|"));

            string str = "";
            for (int i = 1; i < longs.Count; i++) // 丢弃第一个颜色获取时间
            {
                str = string.Concat(str, i > 0 ? (longs[i] - longs[i - 1]).ToString() : longs[0].ToString(), "|");
            }

            tb_y.Text = str;

            colors.Dispose();
            longs.Dispose();

            Tts.Speak("完成");
        }

        #endregion

        #endregion

        #endregion

        #region 获取颜色

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

        #region 模块化技能

        // 模块化之后，都不用注释也能看得懂了。。
        private static readonly 技能信息 技能4 = new(
            820, 998, 65, 17, -52, 25, -49
            , 2, -2, -4, 3, 50, -45
            , Color.FromArgb(194, 198, 202), 61
            , Color.FromArgb(67, 76, 84), 1
            , Color.FromArgb(54, 62, 70), 1
            , Color.FromArgb(0, 129, 0), 0
            , Color.FromArgb(34, 40, 39), 1
            , Color.FromArgb(72, 77, 81), 2
            , Color.FromArgb(25, 30, 34), 2
            , Color.FromArgb(49, 51, 46), 2
            , Color.FromArgb(33, 36, 36), 2
            , Color.FromArgb(255, 144, 0), 8
            , [Keys.Q, Keys.W, Keys.E, Keys.R]);

        private static readonly 技能信息 技能5 = new(
            802, 995, 58, 17, -49, 24, -47
            , 3, -3, -1, 1, 49, -41
            , Color.FromArgb(196, 200, 203), 60
            , Color.FromArgb(55, 62, 70), 1, Color.FromArgb(54, 61, 69), 1
            , Color.FromArgb(0, 129, 0), 0
            , Color.FromArgb(34, 40, 39), 1
            , Color.FromArgb(75, 80, 84), 2
            , Color.FromArgb(25, 29, 34), 2
            , Color.FromArgb(49, 51, 48), 2
            , Color.FromArgb(32, 35, 37), 2
            , Color.FromArgb(255, 144, 0), 8
            , [Keys.Q, Keys.W, Keys.E, Keys.D, Keys.R]);

        private static readonly 技能信息 技能6 = new(
            774, 995, 58, 17, -49, 24, -47
            , 3, -3, -1, 1, 49, -41
            , Color.FromArgb(196, 200, 203), 60
            , Color.FromArgb(55, 62, 70), 1
            , Color.FromArgb(54, 61, 69), 1
            , Color.FromArgb(0, 129, 0), 0
            , Color.FromArgb(34, 40, 39), 1
            , Color.FromArgb(75, 80, 84), 2
            , Color.FromArgb(25, 29, 34), 2
            , Color.FromArgb(49, 51, 48), 2
            , Color.FromArgb(32, 35, 37), 2
            , Color.FromArgb(255, 144, 0), 8
            , [Keys.Q, Keys.W, Keys.E, Keys.D, Keys.F, Keys.R]);

        private class 技能信息(
            int 左下角x,
            int 左下角y,
            int 技能间隔,
            int 技能CD位置偏移x,
            int 技能CD位置偏移y,
            int 释放位置偏移x,
            int 释放位置偏移y,
            int 法球技能CD位置变化x,
            int 法球技能CD位置变化y,
            int 状态技能位置变化x,
            int 状态技能位置变化y,
            int 被动位置变化x,
            int QWERDF和被动位置变化y,
            Color 技能CD颜色,
            byte 技能CD颜色容差,
            Color 技能CD左下颜色,
            byte 技能CD左下颜色容差,
            Color 法球技能CD颜色,
            byte 法球技能颜色容差,
            Color 技能状态激活颜色,
            byte 技能状态激活颜色容差,
            Color QWERDF框颜色,
            byte QWERDF框颜色容差,
            Color 未学主动技能CD颜色,
            byte 未学主动技能CD颜色容差,
            Color 未学法球技能CD颜色,
            byte 未学法球技能CD颜色容差,
            Color 未学被动技能颜色,
            byte 未学被动技能颜色容差,
            Color 破坏被动技能颜色,
            byte 破坏被动技能颜色容差,
            Color 推荐学习技能颜色,
            byte 推荐学习技能颜色容差,
            Keys[] 技能位置)
        {
            public int 技能CD图标x { get; } = 左下角x + 技能CD位置偏移x;
            public int 技能CD图标y { get; } = 左下角y + 技能CD位置偏移y;
            public int 技能间隔 { get; } = 技能间隔;
            public int 释放变色位置x { get; } = 左下角x + 释放位置偏移x - 2; // 向左偏移2个单位，变化更明显
            public int 释放变色位置y { get; } = 左下角y + 释放位置偏移y;
            public int 法球技能CD位置x { get; } = 左下角x + 法球技能CD位置变化x;
            public int 法球技能CD位置y { get; } = 左下角y + 法球技能CD位置变化y;
            public int 状态技能位置x { get; } = 左下角x + 状态技能位置变化x;
            public int 状态技能位置y { get; } = 左下角y + 状态技能位置变化y;
            public int 被动位置x { get; } = 左下角x + 被动位置变化x;
            public int 被动位置y { get; } = 左下角y + QWERDF和被动位置变化y;
            public int QWERDF位置x { get; } = 左下角x - 1; // 向左偏移1个单位，基本颜色一致
            public int QWERDF位置y { get; } = 左下角y + QWERDF和被动位置变化y;
            public Color 技能CD颜色 { get; } = 技能CD颜色;
            public byte 技能CD颜色容差 { get; } = 技能CD颜色容差;
            public Color 技能CD左下颜色 { get; } = 技能CD左下颜色;
            public byte 技能CD左下颜色容差 { get; } = 技能CD左下颜色容差;
            public Color 法球技能CD颜色 { get; } = 法球技能CD颜色;
            public byte 法球技能颜色容差 { get; } = 法球技能颜色容差;
            public Color 技能状态激活颜色 { get; } = 技能状态激活颜色;
            public byte 技能状态激活颜色容差 { get; } = 技能状态激活颜色容差;
            public Color QWERDF框颜色 { get; } = QWERDF框颜色;
            public byte QWERDF框颜色容差 { get; } = QWERDF框颜色容差;
            public Color 未学主动技能CD颜色 { get; } = 未学主动技能CD颜色;
            public byte 未学主动技能CD颜色容差 { get; } = 未学主动技能CD颜色容差;
            public Color 未学法球技能CD颜色 { get; } = 未学法球技能CD颜色;
            public byte 未学法球技能CD颜色容差 { get; } = 未学法球技能CD颜色容差;
            public Color 未学被动技能颜色 { get; } = 未学被动技能颜色;
            public byte 未学被动技能颜色容差 { get; } = 未学被动技能颜色容差;
            public Color 破坏被动技能颜色 { get; } = 破坏被动技能颜色;
            public byte 破坏被动技能颜色容差 { get; } = 破坏被动技能颜色容差;
            public Color 推荐学习技能颜色 { get; } = 推荐学习技能颜色;
            public byte 推荐学习技能颜色容差 { get; } = 推荐学习技能颜色容差;
            public Keys[] 技能位置 { get; } = 技能位置;
        }

        /// <summary>
        ///     技能类型枚举
        /// </summary>
        private enum 技能类型
        {
            图标CD,
            法球,
            状态,
            释放变色,
            QWERDF图标,
            被动技能存在,
            破坏被动技能,
            未学主动技能,
            未学法球技能,
            推荐学习技能
        }

        #endregion

        #region 技能相关方法

        private static 技能信息 获取技能信息(int 技能数量 = 4)
        {
            return 技能数量 switch
            {
                4 => 技能4,
                5 => 技能5,
                6 => 技能6,
                _ => throw new ArgumentException("Invalid skill number")
            };
        }


        /// <summary>
        ///     获取技能的位置信息，包括坐标和颜色等。
        /// </summary>
        /// <param name="技能信息">技能信息</param>
        /// <param name="offsetX">X方向的偏移量</param>
        /// <param name="类型">技能类型</param>
        /// <returns>包含坐标和颜色等信息的元组</returns>
        private static (int x, int y, Color 颜色, byte 颜色容差) 获取技能位置信息(技能信息 技能信息, int offsetX, 技能类型 类型)
        {
            int x, y;
            Color 颜色;
            byte 颜色容差;

            switch (类型)
            {
                case 技能类型.图标CD:
                    x = 技能信息.技能CD图标x + offsetX - 坐标偏移x;
                    y = 技能信息.技能CD图标y - 坐标偏移y;
                    颜色 = 技能信息.技能CD颜色;
                    颜色容差 = 技能信息.技能CD颜色容差;
                    break;
                case 技能类型.法球:
                    x = 技能信息.法球技能CD位置x + offsetX - 坐标偏移x;
                    y = 技能信息.法球技能CD位置y - 坐标偏移y;
                    颜色 = 技能信息.法球技能CD颜色;
                    颜色容差 = 技能信息.法球技能颜色容差;
                    break;
                case 技能类型.状态:
                    x = 技能信息.状态技能位置x + offsetX - 坐标偏移x;
                    y = 技能信息.状态技能位置y - 坐标偏移y;
                    颜色 = 技能信息.技能状态激活颜色;
                    颜色容差 = 技能信息.技能状态激活颜色容差;
                    break;
                case 技能类型.释放变色:
                    x = 技能信息.释放变色位置x + offsetX - 坐标偏移x;
                    y = 技能信息.释放变色位置y - 坐标偏移y;
                    颜色 = default;
                    颜色容差 = 0;
                    break;
                case 技能类型.QWERDF图标:
                    x = 技能信息.QWERDF位置x + offsetX - 坐标偏移x;
                    y = 技能信息.QWERDF位置y - 坐标偏移y;
                    颜色 = 技能信息.QWERDF框颜色;
                    颜色容差 = 技能信息.QWERDF框颜色容差;
                    break;
                case 技能类型.被动技能存在:
                    x = 技能信息.被动位置x + offsetX - 坐标偏移x;
                    y = 技能信息.被动位置y - 坐标偏移y;
                    颜色 = 技能信息.未学被动技能颜色;
                    颜色容差 = 技能信息.未学被动技能颜色容差;
                    break;
                case 技能类型.破坏被动技能:
                    x = 技能信息.被动位置x + offsetX - 坐标偏移x;
                    y = 技能信息.被动位置y - 坐标偏移y;
                    颜色 = 技能信息.破坏被动技能颜色;
                    颜色容差 = 技能信息.破坏被动技能颜色容差;
                    break;
                case 技能类型.未学主动技能:
                    x = 技能信息.技能CD图标x + offsetX - 坐标偏移x;
                    y = 技能信息.技能CD图标y - 坐标偏移y;
                    颜色 = 技能信息.未学主动技能CD颜色;
                    颜色容差 = 技能信息.未学主动技能CD颜色容差;
                    break;
                case 技能类型.未学法球技能:
                    x = 技能信息.法球技能CD位置x + offsetX - 坐标偏移x;
                    y = 技能信息.法球技能CD位置y - 坐标偏移y;
                    颜色 = 技能信息.未学法球技能CD颜色;
                    颜色容差 = 技能信息.未学法球技能CD颜色容差;
                    break;
                case 技能类型.推荐学习技能:
                    x = 技能信息.状态技能位置x + offsetX - 坐标偏移x;
                    y = 技能信息.状态技能位置y - 坐标偏移y;
                    颜色 = 技能信息.推荐学习技能颜色;
                    颜色容差 = 技能信息.推荐学习技能颜色容差;
                    break;
                default:
                    throw new ArgumentException("未知的技能类型");
            }

            return (x, y, 颜色, 颜色容差);
        }

        private static int 获取技能位置偏移(Keys 技能位置, 技能信息 技能信息)
        {
            int index = Array.IndexOf(技能信息.技能位置, 技能位置);
            if (技能位置 == Keys.F && 技能信息 == 技能5)
            {
                index = 3; // 6技能魔晶A杖
            }

            return index == -1 ? throw new ArgumentException("Invalid skill position") : 技能信息.技能间隔 * index;
        }

        /// <summary>
        ///     <para>最后一个技能为被动时，出到A杖，光会紊乱颜色</para>
        ///     <para>最后一个不判断，判断前面几个，符合的话+1则为技能数量</para>
        /// </summary>
        /// <param name="数组"></param>
        /// <returns></returns>
        public static int 获取当前技能数量(in 字节数组包含长宽 数组)
        {
            int count_技能 = 0;

            List<技能信息> 技能列表 = [技能4, 技能5, 技能6];
            List<int> 技能数量 = [4, 5, 6];

            static void 根据数量判断是否存在技能(in 字节数组包含长宽 数组, 技能信息 技能, int i, ref int count_技能)
            {
                int 偏移 = 技能.技能间隔 * i;

                Point p_QWERDF = new(技能.QWERDF位置x + 偏移 - 坐标偏移x, 技能.QWERDF位置y - 坐标偏移y);
                Point p_主动 = new(技能.技能CD图标x + 偏移 - 坐标偏移x, 技能.技能CD图标y - 坐标偏移y);
                Point p_法球 = new(技能.法球技能CD位置x + 偏移 - 坐标偏移x, 技能.法球技能CD位置y - 坐标偏移y);
                Point p_被动 = new(技能.被动位置x + 偏移 - 坐标偏移x, 技能.被动位置y - 坐标偏移y);
                Point p_推荐 = new(技能.状态技能位置x + 偏移 - 坐标偏移x, 技能.状态技能位置y - 坐标偏移y);

                Color 获取的颜色_QWERDF = GetSPixelBytes(in 数组, p_QWERDF);
                Color 获取的颜色_主动 = GetSPixelBytes(in 数组, p_主动);
                Color 获取的颜色_法球 = GetSPixelBytes(in 数组, p_法球);
                Color 获取的颜色_被动 = GetSPixelBytes(in 数组, p_被动);
                Color 获取的颜色_推荐 = GetSPixelBytes(in 数组, p_推荐);

                bool colorMatch_QWERDF = ColorAEqualColorB(获取的颜色_QWERDF, 技能.QWERDF框颜色, 技能.QWERDF框颜色容差);
                bool colorMatch_已学主动 = ColorAEqualColorB(获取的颜色_主动, 技能.技能CD颜色, 技能.技能CD颜色容差);
                bool colorMatch_未学主动 = ColorAEqualColorB(获取的颜色_主动, 技能.未学主动技能CD颜色, 技能.未学主动技能CD颜色容差);
                bool colorMatch_已学法球 = ColorAEqualColorB(获取的颜色_法球, 技能.未学法球技能CD颜色, 技能.未学法球技能CD颜色容差);
                bool colorMatch_未学法球 = ColorAEqualColorB(获取的颜色_法球, 技能.未学法球技能CD颜色, 技能.未学法球技能CD颜色容差);
                bool colorMatch_被动 = ColorAEqualColorB(获取的颜色_被动, 技能.未学被动技能颜色, 技能.未学被动技能颜色容差);
                bool colorMatch_破坏被动 = ColorAEqualColorB(获取的颜色_被动, 技能.破坏被动技能颜色, 技能.破坏被动技能颜色容差);
                bool colorMatch_推荐 = ColorAEqualColorB(获取的颜色_推荐, 技能.推荐学习技能颜色, 技能.推荐学习技能颜色容差);

                if (colorMatch_QWERDF || colorMatch_已学主动 || colorMatch_未学主动 || colorMatch_已学法球 || colorMatch_未学法球 ||
                    colorMatch_被动 || colorMatch_推荐 || colorMatch_破坏被动)
                {
                    count_技能++;
                    return;
                }

                #region 输出判断调试用

                //string 不同色 = "";
                //if (!colorMatch_QWERDF)
                //{
                //    不同色 += $"{i + 1} QWERDF图标 :位置X:{p_QWERDF.X + 坐标偏移x},位置Y:{p_QWERDF.Y + 坐标偏移y}，RGB:{获取的颜色_QWERDF.R}, {获取的颜色_QWERDF.G}, {获取的颜色_QWERDF.B}\r\n";
                //}
                //if (!colorMatch_已学主动 && !colorMatch_未学主动)
                //{
                //    不同色 += $"{i + 1} 主动 :位置X:{p_主动.X + 坐标偏移x},位置Y:{p_主动.Y + 坐标偏移y}，RGB:{获取的颜色_主动.R}, {获取的颜色_主动.G}, {获取的颜色_主动.B}。\r\n";
                //}
                //if (!colorMatch_已学法球 && !colorMatch_未学法球)
                //{
                //    不同色 += $"{i + 1} 技能 :位置X:{p_法球.X + 坐标偏移x},位置Y:{p_法球.Y + 坐标偏移y}，RGB:{获取的颜色_法球.R}, {获取的颜色_法球.G}, {获取的颜色_法球.B}。\r\n";
                //}
                //if (!colorMatch_被动 && !colorMatch_破坏被动)
                //{
                //    不同色 += $"{i + 1} 被动技能 :位置X:{p_被动.X + 坐标偏移x},位置Y:{p_被动.Y + 坐标偏移y}，RGB:{获取的颜色_被动.R}, {获取的颜色_被动.G}, {获取的颜色_被动.B}。\r\n";
                //}
                //if (!colorMatch_推荐)
                //{
                //    不同色 += $"{i + 1} 推荐技能 :位置X:{p_推荐.X + 坐标偏移x},位置Y:{p_推荐.Y + 坐标偏移y}，RGB:{获取的颜色_被动.R}, {获取的颜色_被动.G}, {获取的颜色_被动.B}。\r\n";
                //}

                //Logger.Error(不同色);

                #endregion
            }

            for (int j = 0; j < 技能列表.Count; j++)
            {
                技能信息 当前技能 = 技能列表[j];
                count_技能 = 0;

                for (int i = 0; i < 技能数量[j] - 1; i++)
                {
                    根据数量判断是否存在技能(in 数组, 当前技能, i, ref count_技能);
                }

                if (count_技能 == 技能数量[j] - 1)
                {
                    return count_技能 + 1;
                }
            }

            Tts.Speak("技能数量异常");
            return 0;
        }

        /// <summary>
        ///     根据提供的抗性直接赋值
        ///     不用传参数
        /// </summary>
        /// <returns></returns>
        private static bool 获取状态抗性()
        {
            try
            {
                if (_form.Controls["tb_状态抗性"] is TextBox tb)
                {
                    double i = Convert.ToDouble(tb.Text.Trim());
                    _状态抗性倍数 = (100 - (i > 100 ? 0 : i)) / 100;
                }
            }
            catch
            {
                _状态抗性倍数 = 1;
                if (_form.Controls["tb_状态抗性"] is TextBox tb)
                {
                    Logger.Error($"获取状态抗性异常:{tb.Text.Trim()}");
                }
            }

            return false;
        }

        /// <summary>
        ///     判断技能状态根据
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <param name="类型">技能类型</param>
        /// <returns>如果技能在CD状态返回真，否则返回假</returns>
        private static bool 判断技能状态(Keys 技能位置, in 字节数组包含长宽 数组, 技能类型 类型 = 技能类型.图标CD)
        {
            if (_技能数量 == 4 && (技能位置 == Keys.D || 技能位置 == Keys.F))
            {
                return false;
            }

            技能信息 技能信息 = 获取技能信息(_技能数量);
            int offsetX = 获取技能位置偏移(技能位置, 技能信息);
            (int x, int y, Color 颜色, byte 颜色容差) = 获取技能位置信息(技能信息, offsetX, 类型);
            return ColorAEqualColorB(颜色, GetPixelBytes(in 数组, x, y), 颜色容差);
        }

        /// <summary>
        ///     获取指定技能位置的像素颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <param name="类型">技能类型</param>
        /// <returns>指定位置的像素颜色</returns>
        private static Color 获取技能判断颜色(Keys 技能位置, in 字节数组包含长宽 数组, int 技能数量 = 4, 技能类型 类型 = 技能类型.释放变色)
        {
            if (技能数量 == 4 && (技能位置 == Keys.D || 技能位置 == Keys.F))
            {
                return Color.Empty;
            }

            技能信息 技能信息 = 获取技能信息(技能数量);
            int offsetX = 获取技能位置偏移(技能位置, 技能信息);
            (int x, int y, Color _, byte _) = 获取技能位置信息(技能信息, offsetX, 类型);

            return GetPixelBytes(in 数组, x, y);
        }

        /// <summary>
        ///     获取用于判断技能进入CD的像素的颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <returns>的像素颜色</returns>
        private static Color 获取技能进入CD判断颜色(Keys 技能位置, in 字节数组包含长宽 数组, int 技能数量 = 4)
        {
            return 获取技能判断颜色(技能位置, in 数组, 技能数量, 技能类型.图标CD);
        }

        /// <summary>
        ///     获取用于判断技能释放的颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <returns>技能释放判断的像素颜色</returns>
        private static Color 获取技能释放判断颜色(Keys 技能位置, in 字节数组包含长宽 数组, int 技能数量 = 4)
        {
            return 获取技能判断颜色(技能位置, in 数组, 技能数量, 技能类型.释放变色);
        }

        /// <summary>
        ///     获取用于判断技能释放的颜色
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量，默认值为4</param>
        /// <returns>技能释放判断的像素颜色</returns>
        private static Color 获取推荐技能学习颜色(Keys 技能位置, in 字节数组包含长宽 数组, int 技能数量 = 4)
        {
            return 获取技能判断颜色(技能位置, in 数组, 技能数量, 技能类型.推荐学习技能);
        }

        /// <summary>
        ///     判断右上角技能是否CD，CD好了返回真，没好返回假
        ///     <para>有延时的话建议传入_全局数组</para>
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量</param>
        /// <returns>如果技能CD好了返回真，否则返回假</returns>
        private static bool DOTA2判断技能是否CD(Keys 技能位置, in 字节数组包含长宽 数组)
        {
            return 判断技能状态(技能位置, in 数组, 技能类型.图标CD);
        }

        private static bool DOTA2释放CD就绪技能(Keys 技能位置, in 字节数组包含长宽 数组)
        {
            if (判断技能状态(技能位置, in 数组, 技能类型.图标CD))
            {
                SimKeyBoard.KeyPress(技能位置);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     判断法球技能是否CD，CD好了返回真，没好返回假
        ///     <para>有延时的话建议传入_全局数组</para>
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <param name="技能数量">技能数量</param>
        /// <returns>如果法球技能CD好了返回真，否则返回假</returns>
        private static bool DOTA2判断法球技能是否CD(Keys 技能位置, in 字节数组包含长宽 数组)
        {
            return 判断技能状态(技能位置, in 数组, 技能类型.法球);
        }

        /// <summary>
        ///     判断状态技能是否启动，启动返回真，未启动返回假
        /// </summary>
        /// <param name="技能位置">技能位置</param>
        /// <param name="数组">包含长宽的字节数组</param>
        /// <returns>如果状态技能未启动返回真，否则返回假</returns>
        private static bool DOTA2判断状态技能是否启动(Keys 技能位置, in 字节数组包含长宽 数组)
        {
            return 判断技能状态(技能位置, in 数组, 技能类型.状态);
        }

        private static bool DOTA2判断是否持续施法(in 字节数组包含长宽 数组)
        {
            // 通过添加步骤来等待完全显示
            // 用于检测持续施法，施法中文字的施字颜色，10秒以内有效
            return ColorAEqualColorB(获取指定位置颜色(953, 764, in 数组), Color.FromArgb(254, 254, 254), 2);
        }

        private static void 记录技能释放信息(Keys s1, string s, bool b1, bool b2, Color c1, Color c2, Color c3)
        {
            Logger.Info($"\r\n{s1}{s}\r\n"
                                + $"判断是否释放{b1}\r\n"
                                + $"释放后变色{b2}\r\n"
                                + $"释放前{c1}\r\n"
                                + $"释放后{c2}\r\n"
                                + $"对比色{c3}\r\n");
        }

        private static void 记录技能释放信息(Keys s1, Keys 技能位置)
        {
            Logger.Info(s1);

            lock (锁字典[技能位置])
            {
                if (技能状态字典.TryGetValue(技能位置, out 技能状态 获取技能状态))
                {
                    bool 释放中判断 = 获取技能状态.释放中判断;
                    bool 已释放变色判断 = 获取技能状态.已释放变色判断;
                    Color 释放前Color = 获取技能状态.释放前Color;
                    Color 释放后Color = 获取技能状态.释放后Color;
                    Logger.Info($"\r\n获取到{s1}的技能状态\r\n"
                                + $"判断是否释放{释放中判断}\r\n"
                                + $"释放后变色{已释放变色判断}\r\n"
                                + $"释放前{释放前Color}\r\n"
                                + $"释放后{释放后Color}\r\n");
                }
                else
                {
                    Logger.Info($"获取不到{技能位置}的技能状态");
                }
            }
        }

        /// <summary>
        ///     获取CD好了的已学习技能释放前颜色
        /// </summary>
        /// <param name="技能位置"></param>
        /// <param name="数组"></param>
        /// <param name="技能数量"></param>
        /// <returns></returns>
        public static bool DOTA2获取单个释放技能前颜色(Keys 技能位置, in 字节数组包含长宽 数组)
        {
            重置指定技能判断(技能位置);

            // 获取当前技能颜色
            Color 获取当前颜色 = 获取技能释放判断颜色(技能位置, in 数组, _技能数量);
            if (判断是否更新释放技能前颜色(技能位置, 获取技能信息(_技能数量), in 数组))
            {
                更新释放前颜色(技能位置, 获取当前颜色);
            }

            return true;
        }

        /// <summary>
        ///     10000次 1-2ms
        /// </summary>
        /// <param name="数组"></param>
        /// <returns></returns>
        private static bool DOTA2获取所有释放技能前颜色(in 字节数组包含长宽 数组)
        {
            static void 更新指定释放色(Keys 技能位置, in 字节数组包含长宽 数组)
            {
                更新释放前颜色(技能位置, 获取技能释放判断颜色(技能位置, in 数组, _技能数量));
            }

            技能信息 技能信息 = 获取技能信息(_技能数量);

            foreach (Keys 位置 in 技能信息.技能位置)
            {
                if (判断是否更新释放技能前颜色(位置, 技能信息, in 数组))
                {
                    更新指定释放色(位置, in 数组);
                }
            }

            return true;
        }

        /// <summary>
        ///     当且仅当该技能为可释放已学习主动时返回真
        /// </summary>
        /// <param name="技能位置"></param>
        /// <param name="技能信息"></param>
        /// <param name="数组"></param>
        /// <returns></returns>
        private static bool 判断是否更新释放技能前颜色(Keys 技能位置, 技能信息 技能信息, in 字节数组包含长宽 数组)
        {
            int 偏移 = 获取技能位置偏移(技能位置, 技能信息);

            Point p_主动 = new(技能信息.技能CD图标x + 偏移 - 坐标偏移x, 技能信息.技能CD图标y - 坐标偏移y);

            Color 获取的颜色_主动 = GetSPixelBytes(in 数组, p_主动);

            // 主动释放CD技能
            bool colorMatch_已学主动 = ColorAEqualColorB(获取的颜色_主动, 技能信息.技能CD颜色, 技能信息.技能CD颜色容差);

            return colorMatch_已学主动;
        }

        private static void 更新释放前颜色(Keys 技能位置, Color 当前获取颜色)
        {
            // 释放判断为真时，技能正在释放或者释放完毕，
            // ，前未变色，释放完毕后 释放判断是假，
            // 还有一种情况是船长这类，释放替换型的
            // 
            // 颜色为空，一开始为空，F1更新技能数量后为空

            // CD真：释放前中（充能无效） 排除
            // 就算是充能的主动技能，只要没有学习，还是不管（基本上完美了）

            lock (锁字典[技能位置])
            {
                if (技能状态字典.TryGetValue(技能位置, out 技能状态 获取技能状态))
                {
                    bool 释放中判断 = 获取技能状态.释放中判断;
                    Color 释放前Color = 获取技能状态.释放前Color;
                    // 如果释放中判断为假并且释放前颜色为空，则更新状态
                    if (!释放中判断 && 释放前Color == Color.Empty)
                    {
                        获取技能状态.释放中判断 = false;
                        获取技能状态.已释放变色判断 = false;
                        获取技能状态.释放前Color = 当前获取颜色;
                        获取技能状态.释放后Color = Color.Empty;
                        // 记录技能释放信息($"更新释放前颜色", false, false, 当前获取颜色, Color.Empty, 当前获取颜色);
                    }
                }
            }
        }

        private static bool DOTA2对比释放技能前后颜色(Keys 技能位置, in 字节数组包含长宽 数组)
        {
            // 指向性技能CD栏基本全白
            Color 技能CD颜色 = 获取技能进入CD判断颜色(技能位置, in 数组, _技能数量);
            if (!ColorAEqualColorB(技能CD颜色, Color.FromArgb(255, 255, 255), 10))
            {
                // 获取当前技能颜色
                Color 当前释放颜色 = 获取技能释放判断颜色(技能位置, in 数组, _技能数量);

                return 处理技能释放(技能位置, 当前释放颜色);
            }
            return true;
        }

        private static readonly Dictionary<Keys, object> 锁字典 = new()
        {
            { Keys.Q, new object() },
            { Keys.W, new object() },
            { Keys.E, new object() },
            { Keys.R, new object() },
            { Keys.D, new object() },
            { Keys.F, new object() }
        };

        private static readonly Dictionary<Keys, 技能状态> 技能状态字典 = new()
        {
            { Keys.Q, new 技能状态() },
            { Keys.W, new 技能状态() },
            { Keys.E, new 技能状态() },
            { Keys.R, new 技能状态() },
            { Keys.D, new 技能状态() },
            { Keys.F, new 技能状态() }
        };

        private class 技能状态
        {
            public bool 释放中判断 { get; set; }
            public bool 已释放变色判断 { get; set; }
            public Color 释放前Color { get; set; } = Color.Empty;
            public Color 释放后Color { get; set; } = Color.Empty;
        }

        private static bool 获取技能释放状态(Keys 技能位置, out bool 释放中判断, out bool 已释放变色判断, out Color 释放前Color, out Color 释放后Color)
        {
            lock (锁字典[技能位置])
            {
                if (技能状态字典.TryGetValue(技能位置, out 技能状态 技能状态))
                {
                    释放中判断 = 技能状态.释放中判断;
                    已释放变色判断 = 技能状态.已释放变色判断;
                    释放前Color = 技能状态.释放前Color;
                    释放后Color = 技能状态.释放后Color;
                    return true;
                }

                释放中判断 = false;
                已释放变色判断 = false;
                释放前Color = Color.Empty;
                释放后Color = Color.Empty;
                return false;
            }
        }

        private static void 更新释放判断和颜色(Keys 技能位置, bool 释放中判断, bool 已释放变色判断, Color 释放前Color, Color 释放后Color)
        {
            lock (锁字典[技能位置])
            {
                if (技能状态字典.TryGetValue(技能位置, out 技能状态 技能状态))
                {
                    技能状态.释放中判断 = 释放中判断;
                    技能状态.已释放变色判断 = 已释放变色判断;
                    技能状态.释放前Color = 释放前Color;
                    技能状态.释放后Color = 释放后Color;
                }
            }
        }

        private static bool 处理技能释放(Keys 技能位置, Color 当前释放颜色)
        {
            if (!获取技能释放状态(技能位置, out bool 释放中判断, out bool 已释放变色判断, out Color 释放前Color, out Color 释放后Color))
            {
                Logger.Info("字典读取失败");
                return false;
            }
            if (释放中判断)
            {
                // 技能释放中
                if (!已释放变色判断)
                {
                    // 技能未释放完毕
                    if (ColorAEqualColorB(释放后Color, 当前释放颜色, 0))
                    {
                        更新释放判断和颜色(技能位置, true, false, 释放前Color, 释放后Color);
                        return true;
                    }
                    else if (ColorAEqualColorB(释放前Color, 当前释放颜色, 0))  // 新增：判断是否回到初始颜色
                    {
                        更新释放判断和颜色(技能位置, false, true, 释放前Color, 释放后Color);
                        // 记录技能释放信息(技能位置, "已释放完毕", false, true, 释放前Color, 释放后Color, 当前释放颜色);
                        return false;
                    }
                    else
                    {
                        更新释放判断和颜色(技能位置, false, true, 释放前Color, 释放后Color);
                        // 记录技能释放信息(技能位置, "已释放完毕", false, true, 释放前Color, 释放后Color, 当前释放颜色);
                        return false;
                    }
                }
                return false;
            }
            else
            {
                if (ColorAEqualColorB(释放前Color, 当前释放颜色, 0))
                {
                    更新释放判断和颜色(技能位置, false, false, 释放前Color, 释放后Color);
                    return true;
                }
                else
                {
                    if ((释放后Color != Color.Empty && ColorAEqualColorB(释放后Color, 当前释放颜色, 0))
                        || DOTA2释放颜色前后对比(释放前Color, 当前释放颜色))
                    {
                        更新释放判断和颜色(技能位置, true, false, 释放前Color, 当前释放颜色);
                        // 记录技能释放信息(技能位置, "释放中", true, false, 释放前Color, 当前释放颜色, 当前释放颜色);
                    }
                    return true;
                }
            }
        }

        /// <summary>
        ///     变色运算匹配，符合的返回真，不符合返回假
        ///     <para>10000次 1ms..</para>
        ///     <para><paramref name="beforColor" /> 释放前颜色</para>
        ///     <para><paramref name="afterColor" /> 释放后颜色</para>
        /// </summary>
        /// <param name="beforColor"></param>
        /// <param name="afterColor"></param>
        /// <returns></returns>
        private static bool DOTA2释放颜色前后对比(Color beforColor, Color afterColor)
        {
            if (!Avx2.IsSupported)
            {
                return Math.Abs((beforColor.R * beforColor.R * 0.0001) + (beforColor.R * 0.7656) - afterColor.R) <= 3
                       && Math.Abs((beforColor.G * beforColor.G * 0.0014) + (beforColor.G * 0.0251) + 147 - afterColor.G) <= 3
                       && Math.Abs((beforColor.B * beforColor.B * 0.0002) + (beforColor.B * 0.751) - afterColor.B) <= 3;
            }

            // 修改2处，上面数值

            // Load color components into Vector256<float>
            Vector256<float> beforColorVec = Vector256.Create(
                beforColor.R, beforColor.G, beforColor.B, 0f,
                beforColor.R, beforColor.G, beforColor.B, 0f
            );

            // 这里的数值之一

            // Compute R channel comparison
            Vector256<float> squaredBeforColorVecR = Avx.Multiply(beforColorVec, beforColorVec);
            Vector256<float> tempR = Avx.Add(Avx.Multiply(squaredBeforColorVecR, Vector256.Create(0.0001f)),
                Avx.Multiply(beforColorVec, Vector256.Create(0.7656f)));
            Vector256<float> diffR = Avx.Subtract(tempR, Vector256.Create((float)afterColor.R));
            Vector256<float> absDiffR = Avx.And(diffR, Vector256.Create(0x7FFFFFFF).AsSingle());
            float rResultValue = absDiffR.GetElement(0);
            bool rResult = rResultValue <= 3f;

            // 这里的数值之一

            // Compute G channel comparison
            Vector256<float> squaredBeforColorVecG = Avx.Multiply(beforColorVec, beforColorVec);
            Vector256<float> tempG = Avx.Add(Avx.Multiply(squaredBeforColorVecG, Vector256.Create(0.0014f)),
                Avx.Multiply(beforColorVec, Vector256.Create(0.0251f)));
            tempG = Avx.Add(tempG, Vector256.Create(147f));
            Vector256<float> diffG = Avx.Subtract(tempG, Vector256.Create((float)afterColor.G));
            Vector256<float> absDiffG = Avx.And(diffG, Vector256.Create(0x7FFFFFFF).AsSingle());
            float gResultValue = absDiffG.GetElement(1);
            bool gResult = gResultValue <= 3f;

            // 这里的数值之一

            // Compute B channel comparison
            Vector256<float> squaredBeforColorVecB = Avx.Multiply(beforColorVec, beforColorVec);
            Vector256<float> tempB = Avx.Add(Avx.Multiply(squaredBeforColorVecB, Vector256.Create(0.0002f)),
                Avx.Multiply(beforColorVec, Vector256.Create(0.751f)));
            Vector256<float> diffB = Avx.Subtract(tempB, Vector256.Create((float)afterColor.B));
            Vector256<float> absDiffB = Avx.And(diffB, Vector256.Create(0x7FFFFFFF).AsSingle());
            float bResultValue = absDiffB.GetElement(2);
            bool bResult = bResultValue <= 3f;

            return rResult && gResult && bResult;
        }

        #endregion

        #region 使用物品

        #region Resource改版前

        ///// <summary>
        /////     visual studio 改版资源浏览器，直接Bitmap没了，变成byte[]
        ///// </summary>
        ///// <param name="bp"></param>
        ///// <param name="bts"></param>
        ///// <param name="size"></param>
        ///// <param name="mode"></param>
        ///// <param name="matchRate"></param>
        ///// <returns></returns>
        //private static bool 根据图片以及类别使用物品(Bitmap bp, in 字节数组包含长宽 数组, int mode = 4, double matchRate = 0.8)
        //{
        //    var p = RegPicturePointR(bp, in 数组, matchRate);
        //    if ((p.X + p.Y <= 0) || (p.X == 245760) || (p.X == 143640)) return false;
        //    根据物品位置按键(p, mode, KeyBoardSim.KeyPress);
        //    return true;
        //}

        ///// <summary>
        /////     用时4-5ms左右
        ///// </summary>
        ///// <param name="bp"></param>
        ///// <param name="bts"></param>
        ///// <param name="size"></param>
        ///// <param name="mode"></param>
        ///// <returns></returns>
        //private static bool 根据图片以及类别自我使用物品(Bitmap bp, in 字节数组包含长宽 数组, int mode = 4)
        //{
        //    var p = RegPicturePointR(bp, in 数组);
        //    if ((p.X + p.Y <= 0) || (p.X == 245760) || (p.X == 143640)) return false;
        //    根据物品位置按键(p, mode, KeyBoardSim.KeyPressAlt);
        //    return true;
        //}

        //private static bool 根据图片以及类别队列使用物品(Bitmap bp, in 字节数组包含长宽 数组, int mode = 4)
        //{
        //    var p = RegPicturePointR(bp, in 数组);
        //    if ((p.X + p.Y <= 0) || (p.X == 245760) || (p.X == 143640)) return false;
        //    根据物品位置按键(p, mode, key => KeyBoardSim.KeyPressWhile(key, Keys.Shift));
        //    return true;
        //}

        //private static bool 根据图片以及类别使用物品多次(Bitmap bp, 字节数组包含长宽 数组, int times, int delay, int mode = 4)
        //{
        //    var p = RegPicturePointR(bp, in 数组);
        //    if ((p.X + p.Y <= 0) || (p.X == 245760) || (p.X == 143640)) return false;

        //    for (var i = 0; i < times; i++)
        //    {
        //        根据物品位置按键(p, mode, KeyBoardSim.KeyPress);
        //        if (i == times - 1) break;

        //        Delay(delay);
        //    }

        //    return true;
        //}

        #endregion

        #region 去掉Resource 模块化物品

        // 4技能 最上侧 943 最右侧 1195 CD颜色 104 104 104 最下侧 986 最左侧 1136 长度 44 宽度 60
        // 1136 1202 67 7 943 991 48 5 986 991 5
        // 最上侧 最右侧 用于判断物品是否可用
        // 1195 943

        private static readonly 物品信息 物品4 = new(1136,
            [
                Color.FromArgb(35, 36, 37), Color.FromArgb(34, 35, 35), Color.FromArgb(32, 32, 32),
                Color.FromArgb(35, 35, 35), Color.FromArgb(34, 33, 33), Color.FromArgb(31, 31, 32)
            ]
        );

        private static readonly 物品信息 物品5 = new(1150,
            [
                Color.FromArgb(35, 36, 37), Color.FromArgb(34, 35, 35), Color.FromArgb(32, 32, 32),
                Color.FromArgb(35, 35, 35), Color.FromArgb(34, 33, 33), Color.FromArgb(31, 31, 32)
            ]
        );

        private static readonly 物品信息 物品6 = new(1180,
            [
                Color.FromArgb(35, 36, 37), Color.FromArgb(34, 35, 35), Color.FromArgb(32, 32, 32),
                Color.FromArgb(35, 35, 35), Color.FromArgb(34, 33, 33), Color.FromArgb(31, 31, 32)
            ]
        );

        private class 物品信息(int 最左侧x, Color[] 物品锁闭颜色)
        {
            public int 物品最左侧x { get; } = 最左侧x;
            public int 物品最上侧y { get; } = 943; // 为灰色那条线的高度
            private int 物品长度 { get; } = 60;
            private int 物品宽度 { get; } = 44;
            private int 物品左右间隔 { get; } = 5;
            private int 物品上下间隔 { get; } = 4;
            public int 物品间隔x { get; } = 60 + 5;
            public int 物品间隔y { get; } = 44 + 4;
            public int 物品CD右上角x { get; } = 最左侧x + 60 - 1; // CD好的时候物品框的右上方
            public int 物品CD右上角y { get; } = 943; // 为灰色那条线的高度
            public Color 物品CD颜色 { get; } = Color.FromArgb(104, 104, 104);
            public byte 物品CD颜色容差 { get; } = 1; // 0
            public int 物品锁闭x { get; } = 最左侧x + 61; // 锁闭框最右方
            public int 物品锁闭y { get; } = 989; // 锁闭框下方
            public Color[] 物品锁闭颜色 { get; } = 物品锁闭颜色; // 因为无物品时，无锁闭情况，但其他物品锁闭
            public byte 物品锁闭颜色容差 { get; }
            public Keys[] 物品位置 { get; } = [Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.Space];
        }

        private static bool 判断物品状态(物品信息 物品, int 序号, in 字节数组包含长宽 数组, Point 初始位置, Color 目标颜色, byte 颜色容差)
        {
            Point 位置 = new(初始位置.X - 坐标偏移x, 初始位置.Y - 坐标偏移y);

            int 内部序号 = 序号;
            if (序号 >= 3)
            {
                内部序号 -= 3;
                位置.Y += 物品.物品间隔y;
            }

            位置.X += 内部序号 * 物品.物品间隔x;

            return ColorAEqualColorB(GetSPixelBytes(in 数组, 位置), 目标颜色, 颜色容差);
        }

        private static bool 判断物品状态(物品信息 物品, int 序号, in 字节数组包含长宽 数组, Point 初始位置, Color[] 目标颜色, byte 颜色容差)
        {
            Point 位置 = new(初始位置.X - 坐标偏移x, 初始位置.Y - 坐标偏移y);

            int 内部序号 = 序号;
            if (序号 >= 3)
            {
                内部序号 -= 3;
                位置.Y += 物品.物品间隔y;
            }

            位置.X += 内部序号 * 物品.物品间隔x;

            Color 获取的颜色 = GetSPixelBytes(in 数组, 位置);

            bool b1 = ColorAEqualColorB(获取的颜色, 目标颜色[序号], 颜色容差);
            // if (!b1) Logger.Info($"获取到物品锁闭,当前物品{序号}，目标{目标颜色[序号]} 获取{获取的颜色}"); 
            return b1;
        }

        private static bool DOTA2判断序号物品是否CD(int 序号, in 字节数组包含长宽 数组)
        {
            物品信息 物品 = 根据技能数量获取物品信息(_技能数量);
            Point 初始位置 = new(物品.物品CD右上角x, 物品.物品CD右上角y);
            Color 目标颜色 = 物品.物品CD颜色;
            byte 颜色容差 = 物品.物品CD颜色容差;

            return 判断物品状态(物品, 序号, in 数组, 初始位置, 目标颜色, 颜色容差);
        }

        private static bool DOTA2判断任意物品是否锁闭(in 字节数组包含长宽 数组)
        {
            物品信息 物品 = 根据技能数量获取物品信息(_技能数量);
            Point 初始位置 = new(物品.物品锁闭x, 物品.物品锁闭y);
            Color[] 目标颜色 = 物品.物品锁闭颜色;
            byte 颜色容差 = 物品.物品锁闭颜色容差;

            for (int i = 0; i < 6; i++)
            {
                if (!判断物品状态(物品, i, in 数组, 初始位置, 目标颜色, 颜色容差))
                {
                    return true;
                }
            }

            return false;
        }

        private static 物品信息 根据技能数量获取物品信息(int mode = 4)
        {
            return mode switch
            {
                4 => 物品4,
                5 => 物品5,
                6 => 物品6,
                _ => throw new ArgumentException("Invalid Item number")
            };
        }

        private static bool 重置耗蓝物品委托和条件()
        {
            _条件z = false;
            _条件x = false;
            _条件c = false;
            _条件v = false;
            _条件b = false;
            _条件space = false;
            _条件根据图片委托z = null;
            _条件根据图片委托x = null;
            _条件根据图片委托c = null;
            _条件根据图片委托v = null;
            _条件根据图片委托b = null;
            _条件根据图片委托space = null;
            return true;
        }

        private static bool 获取当前耗蓝物品并设置切假腿()
        {
            字节数组包含长宽[] 需切假腿物品集合 =
            [
                物品_黑黄杖_数组,
                物品_疯狂面具_数组,
                物品_疯狂面具_虚空至宝_数组,
                物品_紫苑_数组,
                物品_血棘_数组,
                物品_深渊之刃_数组,
                物品_雷神之锤_数组,
                物品_雷神之锤_虚空至宝_数组,
                物品_魂戒_数组,
                物品_鱼叉_数组,
                物品_散失_数组,
                物品_散魂_数组,
                物品_希瓦_数组,
                物品_青莲宝珠_数组,
                物品_飓风长戟_数组,
                物品_红杖_数组,
                物品_红杖2_数组,
                物品_红杖3_数组,
                物品_红杖4_数组,
                物品_红杖5_数组,
                物品_刷新球_数组,
                物品_虚灵之刃_数组
            ];

            Dictionary<Keys, Action> 物品进入CD委托 = new()
            {
                { Keys.Z, () => _条件根据图片委托z ??= 物品z进入CD },
                { Keys.X, () => _条件根据图片委托x ??= 物品x进入CD },
                { Keys.C, () => _条件根据图片委托c ??= 物品c进入CD },
                { Keys.V, () => _条件根据图片委托v ??= 物品v进入CD },
                { Keys.B, () => _条件根据图片委托b ??= 物品b进入CD },
                { Keys.Space, () => _条件根据图片委托space ??= 物品space进入CD }
            };

            foreach (字节数组包含长宽 匹配数组 in 需切假腿物品集合)
            {
                Keys key = 根据图片获取物品按键(匹配数组);
                if (物品进入CD委托.TryGetValue(key, out Action value))
                {
                    if (匹配数组 == 物品_魂戒_数组)
                    {
                        _切假腿配置.修改配置(key, true, "力量");
                    }
                    else
                    {
                        _切假腿配置.修改配置(key, true);
                    }

                    value.Invoke();
                }
            }

            return true;
        }

        // 定义一个结构体来表示颜色检查项
        private struct ColorCheck
        {
            public int Num;
            public Rectangle OcrArea; // OCR 的区域
        }

        private static double 获取当前攻击速度()
        {
            // 定义颜色检查项的列表
            List<ColorCheck> colorChecks =
            [
                new()
                {
                    Num = 4,
                    OcrArea = new Rectangle(663, 959, 28, 30)
                },
                new()
                {
                    Num = 5,
                    OcrArea = new Rectangle(647, 959, 28, 30)
                },
                new()
                {
                    Num = 6,
                    OcrArea = new Rectangle(619, 959, 28, 30)
                }
            ];

            foreach (ColorCheck check in colorChecks)
            {
                if (check.Num != _技能数量)
                {
                    continue;
                }

                string ocrResult = PaddleOcr.获取图片文字(check.OcrArea.X, check.OcrArea.Y, check.OcrArea.Width,
                    check.OcrArea.Height);
                if (double.TryParse(ocrResult, out double result))
                {
                    if (_form.Controls["tb_攻速"] is TextBox tb)
                    {
                        tb.Text = ocrResult;
                    }

                    return result;
                }
                else
                {
                    return 100.0;
                    // throw new InvalidOperationException("OCR 结果无法转换为双精度浮点数。");
                }
            }

            return 100.0;
            // 如果没有匹配的颜色，抛出异常或返回默认值
            // throw new InvalidOperationException("未找到匹配的颜色。");
        }

        private static bool 获取当前假腿按键()
        {
            字节数组包含长宽[] 假腿数组集合 =
            [
                物品_假腿_力量腿_数组,
                物品_假腿_敏捷腿_数组,
                物品_假腿_智力腿_数组
            ];

            Dictionary<Keys, (Action 清空委托, Action 重置条件)> 清空物品进入CD委托和条件映射 = new()
            {
                { Keys.Z, (() => _条件根据图片委托z = null, () => _条件z = false) },
                { Keys.X, (() => _条件根据图片委托x = null, () => _条件x = false) },
                { Keys.C, (() => _条件根据图片委托c = null, () => _条件c = false) },
                { Keys.V, (() => _条件根据图片委托v = null, () => _条件v = false) },
                { Keys.B, (() => _条件根据图片委托b = null, () => _条件b = false) },
                { Keys.Space, (() => _条件根据图片委托space = null, () => _条件space = false) }
            };

            foreach (字节数组包含长宽 假腿数组 in 假腿数组集合)
            {
                Keys key = 根据图片获取物品按键(假腿数组);
                if (key != Keys.Escape)
                {
                    _假腿按键 = key;
                    if (清空物品进入CD委托和条件映射.TryGetValue(_假腿按键, out (Action 清空委托, Action 重置条件) actions))
                    {
                        actions.清空委托.Invoke();
                        actions.重置条件.Invoke();
                    }
                }
            }

            return _假腿按键 != Keys.Escape;
        }

        public static Keys 根据图片获取物品按键(in 字节数组包含长宽 数组)
        {
            Point 位置 = RegPicturePointR(in 数组, in _全局数组);
            return 是否无效位置(位置) ? Keys.Escape : 根据位置获取按键(位置);
        }

        public static int 根据图片使用物品(in 字节数组包含长宽 数组, double matchRate = 0.8)
        {
            return 执行物品操作(数组, p => SimKeyBoard.KeyPress(根据位置获取按键(p)));
        }

        public static int 根据图片自我使用物品(in 字节数组包含长宽 数组)
        {
            return 执行物品操作(数组, p => SimKeyBoard.KeyPressAlt(根据位置获取按键(p)));
        }

        public static bool 根据图片队列使用物品(in 字节数组包含长宽 数组)
        {
            return 执行物品操作(数组, p => SimKeyBoard.KeyPressWhile(根据位置获取按键(p), Keys.Shift)) > 0;
        }

        public static int 根据图片多次使用物品(in 字节数组包含长宽 数组, int times, int delay)
        {
            Point 位置 = RegPicturePointR(in 数组, in _全局数组);
            if (是否无效位置(位置))
            {
                return 0;
            }

            Keys 按键 = 根据位置获取按键(位置);
            for (int i = 0; i < times; i++)
            {
                SimKeyBoard.KeyPress(按键);
                if (i < times - 1)
                {
                    Delay(delay);
                }
            }

            return times;
        }

        private static int 执行物品操作(in 字节数组包含长宽 数组, Action<Point> 操作)
        {
            Point 位置 = RegPicturePointR(in 数组, in _全局数组);
            if (是否无效位置(位置))
            {
                return 0;
            }

            操作(位置);
            return 1;
        }

        private static Keys 根据位置获取按键(Point 位置)
        {
            物品信息 物品 = 根据技能数量获取物品信息(_技能数量);
            return 物品.物品位置[获取物品位置序号(位置, 物品)];
        }

        private static int 获取物品位置序号(Point 位置, 物品信息 物品)
        {
            int x = 位置.X + 坐标偏移x;
            int y = 位置.Y + 坐标偏移y;
            int index = (int)Math.Floor((x - 物品.物品最左侧x) / (float)物品.物品间隔x);
            if (y - 物品.物品最上侧y >= 物品.物品间隔y)
            {
                index += 3;
            }

            return index;
        }

        #endregion

        #endregion

        #region buff或者装备

        private const int 技能4魔晶A杖x = 1096;
        private const int 技能5魔晶A杖x = 1110;
        private const int 技能6魔晶A杖x = 1140;
        private const int 技能A杖y = 959;
        private const int 技能魔晶y = 994;

        private static bool 阿哈利姆神杖(in 字节数组包含长宽 数组)
        {
            return 检查技能颜色(in 数组, [技能4魔晶A杖x, 技能5魔晶A杖x, 技能6魔晶A杖x], 技能A杖y, Color.FromArgb(30, 187, 250));
        }

        private static bool 阿哈利姆魔晶(in 字节数组包含长宽 数组)
        {
            return 检查技能颜色(in 数组, [技能4魔晶A杖x, 技能5魔晶A杖x, 技能6魔晶A杖x], 技能魔晶y, Color.FromArgb(30, 187, 254));
        }

        /// <summary>
        ///     检查技能颜色是否匹配
        /// </summary>
        /// <param name="bts">数组</param>
        /// <param name="size">大小</param>
        /// <param name="xCoords">x坐标数组</param>
        /// <param name="yCoord">y坐标</param>
        /// <param name="技能点颜色">技能点颜色</param>
        /// <returns>是否匹配</returns>
        private static bool 检查技能颜色(in 字节数组包含长宽 数组, int[] xCoords, int yCoord, Color 技能点颜色)
        {
            foreach (int xCoord in xCoords)
            {
                Color color = GetPixelBytes(in 数组, xCoord - 坐标偏移x, yCoord - 坐标偏移y);
                if (ColorAEqualColorB(color, 技能点颜色, 1))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 获取指定位置颜色

        private static async Task<Color> 获取指定位置颜色(int x, int y)
        {
            Size size = new(截图模式1W, 截图模式1H);
            Bitmap bitmap = await CaptureScreenAsync(截图模式1X, 截图模式1Y, size).ConfigureAwait(true);
            字节数组包含长宽 数组 = new(await GetBitmapByteAsync(bitmap).ConfigureAwait(true), size);
            bitmap = await CaptureScreenAsync(截图模式1X, 截图模式1Y, size).ConfigureAwait(true);
            _ = 数组.新赋值数组(await GetBitmapByteAsync(bitmap).ConfigureAwait(true));
            return GetPixelBytes(in 数组, x - 坐标偏移x, y - 坐标偏移y);
        }

        private static Color 获取指定位置颜色(int x, int y, in 字节数组包含长宽 数组)
        {
            return GetPixelBytes(in 数组, x - 坐标偏移x, y - 坐标偏移y);
        }

        #endregion

        #endregion

        #endregion

        #region 绝地潜兵2具体实现（虽然没用）

#if HF2
            private async Task HF2_补给()
            {
                KeyPress(Keys.LControl);
                KeyPress(Keys.Numown);
                KeyPress(Keys.Numown);
                KeyPress(Keys.Up);
                KeyPress(Keys.Right);
                LeftClick();
            }

            private async Task HF2_救援()
            {
                KeyPress(Keys.LControl);
                KeyPress(Keys.Up);
                KeyPress(Keys.Right);
                KeyPress(Keys.Left);
                KeyPress(Keys.Up);
                LeftClick();
            }
            private async Task HF2_背包_激光()
            {
                KeyPress(Keys.LControl);
                KeyPress(Keys.Numown);
                KeyPress(Keys.Up);
                KeyPress(Keys.Left);
                KeyPress(Keys.Up);
                KeyPress(Keys.Right);
                LeftClick();
            }

            private async Task HF2_SOS()
            {
                KeyPress(Keys.LControl);
                KeyPress(Keys.Up);
                KeyPress(Keys.Numown);
                KeyPress(Keys.Right);
                KeyPress(Keys.Up);
                LeftClick();
            }

            private async Task HF2_飞鹰_110()
            {
                KeyPress(Keys.LControl);
                KeyPress(Keys.Up);
                KeyPress(Keys.Numown);
                KeyPress(Keys.Up);
                KeyPress(Keys.Left);
            }

            private async Task HF2_飞鹰_空袭()
            {
                KeyPress(Keys.LControl);
                KeyPress(Keys.Up);
                KeyPress(Keys.Right);
                KeyPress(Keys.Numown);
                KeyPress(Keys.Right);
            }
            private async Task HF2_飞鹰_重填装()
            {
                KeyPress(Keys.LControl);
                KeyPress(Keys.Up);
                KeyPress(Keys.Up);
                KeyPress(Keys.Left);
                KeyPress(Keys.Up);
                KeyPress(Keys.Right);
            }
#endif

        #endregion

        #region LOL具体实现

#if LOL
            #region 魔腾

            private static async Task<bool> 梦魇之径接平A(字节数组包含长宽 数组)
            {
                // X 754 820 886 952 Y 1005 用于一次性技能释放CD
                var q4 = 获取指定位置颜色(751, 1005, in 数组);

                static void 梦魇之径后()
                {
                    RightClick();
                    Delay(25);
                    KeyPress(Keys.A);
                }

                if (ColorAEqualColorB(q4, 技能CD颜色, 0)) return await FromResult(true);

                梦魇之径后();
                return await FromResult(false);
            }

            private static async Task<bool> 无言恐惧接梦魇之径(字节数组包含长宽 数组)
            {
                // X 754 820 886 952 Y 1005 用于一次性技能释放CD
                var e4 = 获取指定位置颜色(886, 1005, in 数组);

                static void 无言恐惧后()
                {
                    RightClick();
                    Delay(25);
                    KeyPress(Keys.Q);
                }

                if (ColorAEqualColorB(e4, 技能CD颜色, 0)) return await FromResult(true);

                无言恐惧后();
                return await FromResult(false);
            }

            private static async Task<bool> 鬼影重重接无言恐惧(字节数组包含长宽 数组)
            {
                // X 950 Y 950 用于冲刺后检测
                var r4 = 获取指定位置颜色(950, 950, in 数组);

                static void 鬼影重重后()
                {
                    _条件4 = true;
                }

                if (!ColorAEqualColorB(r4, Color.FromArgb(1, 61, 97), 0)) return await FromResult(true);

                鬼影重重后();
                return await FromResult(false);
            }

            private static async Task<bool> 重复释放无言恐惧(字节数组包含长宽 数组)
            {
                // X 754 820 886 952 Y 1005 用于一次性技能释放CD
                var e4 = 获取指定位置颜色(886, 1005, in 数组);

                Delay(125);
                KeyPress(Keys.E);

                if (ColorAEqualColorB(e4, 技能CD颜色, 0)) return await FromResult(true);

                return await FromResult(false);
            }

            #endregion

            #region 男枪
            private static async Task<bool> 穷途末路接平A(字节数组包含长宽 数组)
            {
                // X 754 820 886 952 Y 1005 用于一次性技能释放CD
                var q4 = 获取指定位置颜色(754, 1005, in 数组);

                static void 穷途末路后()
                {
                    RightClick();
                    Delay(25);
                    KeyPress(Keys.A);
                }

                if (ColorAEqualColorB(q4, 技能CD颜色, 0)) return await FromResult(true);

                穷途末路后();
                return await FromResult(false);
            }

            private static async Task<bool> 烟雾弹接平A(字节数组包含长宽 数组)
            {
                // X 754 820 886 952 Y 1005 用于一次性技能释放CD
                var w4 = 获取指定位置颜色(820, 1005, in 数组);

                static void 烟雾弹后()
                {
                    RightClick();
                    Delay(25);
                    KeyPress(Keys.A);
                }

                if (ColorAEqualColorB(w4, 技能CD颜色, 0)) return await FromResult(true);

                烟雾弹后();
                return await FromResult(false);
            }

            private static async Task<bool> 快速拔枪接平A(字节数组包含长宽 数组)
            {
                // X 754 820 886 952 Y 1005 用于一次性技能释放CD
                var e4 = 获取指定位置颜色(886, 1005, in 数组);

                static void 快速拔枪后()
                {
                    RightClick();
                    Delay(25);
                    KeyPress(Keys.A);
                }

                if (ColorAEqualColorB(e4, 技能CD颜色, 0)) return await FromResult(true);

                快速拔枪后();
                return await FromResult(false);
            }

            private static async Task<bool> 终极爆弹接平A(字节数组包含长宽 数组)
            {
                // X 754 820 886 952 Y 1005 用于一次性技能释放CD
                var r4 = 获取指定位置颜色(952, 1005, in 数组);

                static void 终极爆弹后()
                {
                    RightClick();
                    Delay(25);
                    KeyPress(Keys.A);
                }

                if (ColorAEqualColorB(r4, 技能CD颜色, 0)) return await FromResult(true);

                终极爆弹后();
                return await FromResult(false);
            }

            #endregion

#endif

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

        #region 页面元素更改更改

        private void Tb_攻速_TextChanged(object sender, EventArgs e)
        {
            if (tb_name.Text == "测试")
            {
                return;
            }

            try
            {
                _攻击速度 = Convert.ToDouble(tb_攻速.Text);
            }
            catch
            {
                _攻击速度 = 100.0;
            }
        }

        private void 设置当前攻速()
        {
            tb_攻速.Text = _攻击速度.ToString();
        }

        private void tb_状态抗性_TextChanged(object sender, EventArgs e)
        {
            _ = 获取状态抗性();
        }

        #endregion

        #region 页面初始化和注销

        /// <summary>
        ///     页面初始化
        /// </summary>
        public Form2()
        {
            InitializeComponent();

            _form = this;

            _ = StartListen();
        }

        /// <summary>
        ///     开始监听和初始化模拟
        /// </summary>
        private int StartListen()
        {
            /// 设置线程池数量，最小要大于CPU核心数，
            /// 最大不要太大就完事了，一般自动就行，手动反而影响性能
            //ThreadPool.SetMinThreads(12, 18);
            //ThreadPool.SetMaxThreads(48, 36);

            // 设置程序为HIGH程序，REALTIME循环延时
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            int i = 1;

            //_myKeyEventHandeler = Hook_KeyDown;
            //_kHook.KeyDownEvent += _myKeyEventHandeler; // 绑定对应处理函数
            //_kHook.Start(); // 安装键盘钩子

            //_mGlobalHook.KeyDown += Hook_KeyDown;

            // 全部捕获
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

            // 设置窗体显示在最上层
            i = SetWindowPos(Handle, -1, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0010 | 0x0080);

            // 设置本窗体为活动窗体
            SetActiveWindow(Handle);
            SetForegroundWindow(Handle);

            // 设置窗体置顶
            TopMost = true;

            // 338 987 只显示四行 1047 只显示1行 900 基本显示除了颜色框的全部
            // 设置窗口位置
            Location = new Point(338, 987);
            Width = 136;

            if (tb_name.Text.Trim() == "测试")
            {
                // 850 基本显示完全
                Location = new Point(338, 850);
                lb_阵营.Text = "模式例:q4";
                lb_状态抗性.Text = "超时时间";
                lb_攻速.Text = "位置12|13";
                tb_阵营.Text = "q4";
                tb_状态抗性.Text = "2000";
                label6.Text = "颜色";
                tb_delay.Text = "";
            }
            else
            {
                lb_攻速.Text = "攻速";
                tb_状态抗性.Text = "25";
            }

            //Task.await Run(记录买活);

            // 用于初始捕捉
            _ = 获取图片_1().Result;

#if DOTA2

            初始化DOTA2用到的图片();
#endif

            PaddleOcr.初始化PaddleOcr();

            return i;
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

            // 释放PaddleOcr
            _ = PaddleOcr.释放PaddleOcr();
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
    }
}