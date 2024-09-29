using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dota2Simulator.KeyboardMouse;
using Dota2Simulator.PictureProcessing;
using Newtonsoft.Json;

namespace Dota2Simulator;

internal partial class Form1 : Form
{
    /// <summary>
    ///     全局捕获
    /// </summary>
    private readonly HookUserActivity _hookUser = new();

    /// <summary>
    ///     页面初始化
    /// </summary>
    public Form1()
    {
        InitializeComponent();

        LoadPageDate();

        // 全部捕获
        _hookUser.HookScope = HookUserActivity.HookScopeType.GlobalScope;
        _hookUser.ActivityForm = this;
        //hook.OnMouseActivity += Hook_OnMouseActivity;
        _hookUser.KeyDown += Hook_KeyDown;
        //hook.KeyPress += Hook_KeyPress;
        //hook.KeyUp += Hook_KeyUp; 
        _hookUser.Start(false, true);

        _ = comboBox3.Items.Add("冻结文书页面1");
        _ = comboBox3.Items.Add("冻结文书页面2");
        _ = comboBox3.Items.Add("协助财产通知书_部平台上报");
        _ = comboBox3.Items.Add("部平台截图");
        _ = comboBox3.Items.Add("执法办案冻结打印1");
        _ = comboBox3.Items.Add("执法办案冻结打印2");
        _ = comboBox3.Items.Add("测试");

        区分卡号();
    }

    private void 区分卡号()
    {
        string[] lines =
        [
            ""
        ];

        Dictionary<int, List<string>> groupDic = [];

        string pattern = @"^(\t+)(.*)$";

        foreach (string line in lines)
        {
            Match match = Regex.Match(line, pattern);
            if (match.Success)
            {
                int spaceCount = match.Groups[1].Value.Length;
                string str = match.Groups[2].Value;

                if (!groupDic.TryGetValue(spaceCount, out List<string> value))
                {
                    value = [];
                    groupDic[spaceCount] = value;
                }

                value.Add(str);
            }
        }

        foreach (KeyValuePair<int, List<string>> group in groupDic)
        {
            textBox4.Text += group.Key.ToString() + Environment.NewLine + string.Join(Environment.NewLine, group.Value);
        }
    }

    /// <summary>
    ///     全局捕获事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Hook_KeyDown(object sender, KeyEventArgs e)
    {
        // 最好是不常用的快捷键
        if (e.KeyCode != Keys.F9)
        {
            return;
        }

        switch (comboBox3.Text)
        {
            case "冻结文书页面1":
                协助财产通知书_冻结1();
                break;
            case "冻结文书页面2":
                协助财产通知书_冻结2();
                break;
            case "协助财产通知书_部平台上报":
                协助财产通知书_部平台上报();
                break;
            case "部平台截图":
                部平台截图();
                break;
            case "执法办案冻结打印1":
                执法办案冻结打印1();
                break;
            case "执法办案冻结打印2":
                执法办案冻结打印2();
                break;
            case "测试":
                测试();
                break;
            default:
                break;
        }
    }

    #region 延时

    /// <summary>
    ///     精准延迟，并减少性能消耗
    /// </summary>
    /// <param name="delay">需要延迟的时间</param>
    /// <param name="time"></param>
    private static void Delay(int delay, long time = -1)
    {
        time = time == -1 ? 获取当前时间毫秒() : time;
        long endTime = time + delay;
        SpinWait spinWait = new();

        while (获取当前时间毫秒() < endTime)
        {
            int remainingTime = (int)(endTime - 获取当前时间毫秒());
            switch (remainingTime)
            {
                case > 0 and > 10:
                    Thread.Sleep(remainingTime / 2); // Sleep for half of the remaining time to avoid oversleeping
                    break;
                case > 0:
                    spinWait.SpinOnce(); // SpinWait for very short intervals
                    break;
                default:
                    break;
            }
        }
    }

    #endregion

    #region 自动化操作

    private void 协助财产通知书_冻结1()
    {
        string txt = textBox4.Text;
        string 换行 = @"(.*)(\r?\n|\r)";
        List<string> listStr = [];

        MatchCollection matchCollection = Regex.Matches(txt, 换行);

        foreach (object match in matchCollection)
        {
            string str = match.ToString()?.Trim();
            if (str == null)
            {
                continue;
            }

            str = Regex.Replace(str, @"(\r?\n|\r)", "");
            if (str.Length != 0)
            {
                listStr.Add(str);
            }
        }

        Point p = MousePosition;

        SimKeyBoard.MouseMove(2026, 589);
        SimKeyBoard.MouseLeftClick();

        Delay(50);

        Clipboard.SetText(listStr[0]);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

        Delay(50);

        for (int i = 0; i < 7; i++)
        {
            SimKeyBoard.KeyPress(Keys.Tab);
        }

        Delay(50);

        SimKeyBoard.MouseMove(2256, 770);
        SimKeyBoard.MouseLeftClick();

        SimKeyBoard.KeyPress(Keys.Tab);
        Clipboard.SetText(listStr[1]);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
        SimKeyBoard.KeyPress(Keys.Tab);
        SimKeyBoard.KeyPress(Keys.Tab);

        Delay(50);

        Clipboard.SetText(listStr[2]);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
        SimKeyBoard.KeyPress(Keys.Tab);

        Delay(50);

        Clipboard.SetText(listStr[3]);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
        SimKeyBoard.KeyPress(Keys.Tab);

        Delay(50);

        Clipboard.SetText(listStr[12]);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
        SimKeyBoard.KeyPress(Keys.Tab);
        SimKeyBoard.KeyPress(Keys.Tab);

        Delay(50);

        Clipboard.SetText(listStr[4]);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
        SimKeyBoard.KeyPress(Keys.Tab);
        SimKeyBoard.KeyPress(Keys.Tab);

        Delay(50);

        Clipboard.SetText(listStr[5]);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
        SimKeyBoard.KeyPress(Keys.Tab);

        Delay(50);

        string s1 = listStr[7].Replace("年", "-").Replace("月", "-").Replace('日', ' ');

        Clipboard.SetText(s1);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
        SimKeyBoard.KeyPress(Keys.Tab);

        Delay(50);

        s1 = listStr[8].Replace("年", "-").Replace("月", "-").Replace('日', ' ');

        Clipboard.SetText(s1);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
        SimKeyBoard.KeyPress(Keys.Tab);

        Delay(50);
        SimKeyBoard.MouseMove(2478, 1542);
        SimKeyBoard.MouseLeftClick();

        SimKeyBoard.KeyPress(Keys.Tab);
        SimKeyBoard.KeyPress(Keys.Tab);
        Clipboard.SetText(listStr[6]);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

        SimKeyBoard.MouseMove(p.X, p.Y);

        // 切换到止付文书页面2
        comboBox3.SelectedIndex = 1;
    }

    private void 协助财产通知书_冻结2()
    {
        string txt = textBox4.Text;
        string 换行 = @"(.*)(\r?\n|\r)";
        List<string> listStr = [];

        MatchCollection matchCollection = Regex.Matches(txt, 换行);

        foreach (object match in matchCollection)
        {
            string str = match.ToString()?.Trim();
            if (str == null)
            {
                continue;
            }

            str = Regex.Replace(str, @"(\r?\n|\r)", "");
            if (str.Length != 0)
            {
                listStr.Add(str);
            }
        }

        Point p = MousePosition;

        SimKeyBoard.MouseMove(2076, 233);
        SimKeyBoard.MouseLeftClick();
        Delay(50);

        for (int i = 0; i < 5; i++)
        {
            SimKeyBoard.KeyPress(Keys.Tab);
        }

        Delay(50);

        SimKeyBoard.KeyPressWhile(Keys.A, Keys.Control);
        Delay(50);
        Clipboard.SetText(listStr[9]);
        Delay(50);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

        SimKeyBoard.KeyPress(Keys.Tab);
        Delay(50);

        SimKeyBoard.KeyPressWhile(Keys.A, Keys.Control);
        Delay(50);
        Clipboard.SetText(listStr[10]);
        Delay(50);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);


        SimKeyBoard.KeyPress(Keys.Tab);
        SimKeyBoard.KeyPress(Keys.Tab);
        Delay(50);

        SimKeyBoard.KeyPressWhile(Keys.A, Keys.Control);
        Delay(50);
        Clipboard.SetText("根据受害人的陈述、转账记录等");
        Delay(50);
        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

        SimKeyBoard.MouseMove(p.X, p.Y);

        Delay(50);
        Clipboard.SetText(listStr[10]);

        // 切换到止付文书页面1
        comboBox3.SelectedIndex = 0;
    }

    private void 协助财产通知书_部平台上报()
    {
        string txt = textBox4.Text;
        string 换行 = @"(.*)(\r?\n|\r)";
        List<string> listStr = [];

        MatchCollection matchCollection = Regex.Matches(txt, 换行);

        foreach (object match in matchCollection)
        {
            string str = match.ToString()?.Trim();
            if (str == null)
            {
                continue;
            }

            str = Regex.Replace(str, @"(\r?\n|\r)", "");
            if (str.Length != 0)
            {
                listStr.Add(str);
            }
        }

        _ = Task.Run(() =>
        {
            Point p = MousePosition;

            SimKeyBoard.MouseMove(679, 684);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            Invoke(() => { Clipboard.SetText(listStr[3]); });
            SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

            SimKeyBoard.KeyPress(Keys.Tab);
            Delay(50);
            SimKeyBoard.KeyPress(Keys.Tab);

            Invoke(() => { Clipboard.SetText(listStr[2]); });
            SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);


            SimKeyBoard.KeyPress(Keys.Tab);
            Delay(50);
            SimKeyBoard.KeyPress(Keys.Tab);
            Delay(50);
            SimKeyBoard.KeyPress(Keys.Space);

            Delay(50);
            SimKeyBoard.KeyPress(Keys.Tab);

            Invoke(() => { Clipboard.SetText(listStr[4]); });
            SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

            SimKeyBoard.KeyPress(Keys.Tab);
            Delay(50);
            SimKeyBoard.KeyPress(Keys.Tab);
            Delay(50);

            // 每个冻结需要手动更改的文书号
            Invoke(() => { Clipboard.SetText(listStr[14]); });
            SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

            for (int i = 0; i < 6; i++)
            {
                SimKeyBoard.KeyPress(Keys.Tab);
            }

            Delay(50);
            Invoke(() => { Clipboard.SetText(listStr[10]); });
            SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

            Delay(250);

            int processCount = 0;

            SimKeyBoard.MouseMove(3035, 768);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            while (processCount < 1)
            {
                if (PaddleOcr.获取图片文字(2961, 892, 49, 27) == "对公")
                {
                    processCount++;
                }

                Delay(25);
            }

            SimKeyBoard.MouseMove(3070, 847);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            Delay(150);

            SimKeyBoard.MouseMove(3712, 961);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            while (processCount < 2)
            {
                if (PaddleOcr.获取图片文字(2206, 1404, 55, 31) == "取消")
                {
                    processCount++;
                }

                Delay(25);
            }

            Invoke(() => { Clipboard.SetText(listStr[3] + "_冻结.jpg"); });
            Delay(50);
            SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
            Delay(50);
            SimKeyBoard.MouseMove(2083, 1418);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            while (processCount < 3)
            {
                if (PaddleOcr.获取图片文字(368, 487, 120, 48) == "冻结信息")
                {
                    processCount++;
                }

                Delay(25);
            }

            SimKeyBoard.MouseMove(1997, 1579);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            while (processCount < 4)
            {
                if (PaddleOcr.获取图片文字(2206, 1404, 55, 31) == "取消")
                {
                    processCount++;
                }

                Delay(25);
            }


            Invoke(() => { Clipboard.SetText(listStr[3] + "_冻结呈请.jpg"); });
            Delay(50);
            SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
            Delay(50);
            SimKeyBoard.MouseMove(2083, 1418);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            SimKeyBoard.MouseMove(3059, 777);
            SimKeyBoard.MouseLeftClick();
            Delay(250);

            // 卡号为个人类型
            SimKeyBoard.MouseMove(3059, 850);
            SimKeyBoard.MouseLeftClick();
            Delay(50);


            // 经办人2
            SimKeyBoard.MouseMove(1676, 1690);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            while (processCount < 5)
            {
                if (PaddleOcr.获取图片文字(1177, 478, 180, 39) == "选择经办警官")
                {
                    processCount++;
                }

                Delay(25);
            }

            // 经办人2选择
            SimKeyBoard.MouseMove(1227, 791);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            Delay(250);

            // 经办人2确定
            SimKeyBoard.MouseMove(1980, 992);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            SimKeyBoard.MouseMove(p.X, p.Y);

            Invoke(() => { comboBox3.SelectedIndex = 4; });
        });
    }

    private static void 部平台截图()
    {
        SimKeyBoard.KeyPress(Keys.F1);
        Delay(150);

        SimKeyBoard.KeyPress(Keys.R);
        Delay(50);

        SimKeyBoard.KeyPressWhile(Keys.S, Keys.Control);
        Delay(600);

        SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);
        Delay(50);
    }

    private void 执法办案冻结打印1()
    {
        string txt = textBox4.Text;
        string 换行 = @"(.*)(\r?\n|\r)";
        List<string> listStr = [];

        MatchCollection matchCollection = Regex.Matches(txt, 换行);

        foreach (object match in matchCollection)
        {
            string str = match.ToString()?.Trim();
            if (str == null)
            {
                continue;
            }

            str = Regex.Replace(str, @"(\r?\n|\r)", "");
            if (str.Length != 0)
            {
                listStr.Add(str);
            }
        }

        Point p = MousePosition;

        SimKeyBoard.MouseMove(185, 258);
        SimKeyBoard.MouseLeftClick();
        Delay(50);

        _ = Task.Run(() =>
        {
            int processCount = 0;

            while (processCount < 1)
            {
                if (PaddleOcr.获取图片文字(1537, 737, 50, 34) == "打印")
                {
                    processCount++;
                }

                Delay(25);
            }

            SimKeyBoard.MouseMove(1907, 1078);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            SimKeyBoard.KeyPress(Keys.Back);
            Delay(50);

            SimKeyBoard.KeyPress(Keys.D1);
            Delay(50);

            SimKeyBoard.MouseMove(2111, 1317);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            while (processCount < 2)
            {
                if (PaddleOcr.获取图片文字(32, 704, 36, 27) == "放大")
                {
                    processCount++;
                }

                Delay(25);
            }

            Delay(250);

            SimKeyBoard.KeyPressWhile(Keys.S, Keys.Control);

            while (processCount < 3)
            {
                if (PaddleOcr.获取图片文字(1298, 1661, 107, 41) == "隐藏文件夹")
                {
                    processCount++;
                }

                Delay(25);
            }

            Delay(100);
            // 剪切板操作只能在UI线程
            Invoke(() => { Clipboard.SetText(listStr[3] + "_冻结"); });
            SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

            Delay(150);
            SimKeyBoard.KeyPressAlt(Keys.S);

            while (processCount < 4)
            {
                if (File.Exists("Z:\\desktop\\" + listStr[3] + "_冻结.jpg"))
                {
                    processCount++;
                }

                Delay(25);
            }

            SimKeyBoard.KeyPressAlt(Keys.F4);

            while (processCount < 5)
            {
                if (PaddleOcr.获取图片文字(50, 110, 50, 33) == "周报")
                {
                    processCount++;
                }

                Delay(25);
            }

            SimKeyBoard.MouseMove(2765, 937);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            SimKeyBoard.KeyPressWhile(Keys.W, Keys.Control);

            SimKeyBoard.MouseMove(p.X, p.Y);

            Invoke(() => { comboBox3.SelectedIndex = 5; });
        });
    }

    private void 执法办案冻结打印2()
    {
        string txt = textBox4.Text;
        string 换行 = @"(.*)(\r?\n|\r)";
        List<string> listStr = [];

        MatchCollection matchCollection = Regex.Matches(txt, 换行);

        foreach (object match in matchCollection)
        {
            string str = match.ToString()?.Trim();
            if (str == null)
            {
                continue;
            }

            str = Regex.Replace(str, @"(\r?\n|\r)", "");
            if (str.Length != 0)
            {
                listStr.Add(str);
            }
        }

        Point p = MousePosition;

        SimKeyBoard.MouseMove(185, 258);
        SimKeyBoard.MouseLeftClick();
        Delay(50);

        _ = Task.Run(() =>
        {
            int processCount = 0;

            while (processCount < 1)
            {
                if (PaddleOcr.获取图片文字(1537, 737, 50, 34) == "打印")
                {
                    processCount++;
                }

                Delay(25);
            }

            SimKeyBoard.MouseMove(2111, 1317);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            while (processCount < 2)
            {
                if (PaddleOcr.获取图片文字(32, 704, 36, 27) == "放大")
                {
                    processCount++;
                }

                Delay(25);
            }

            Delay(250);

            SimKeyBoard.KeyPressWhile(Keys.S, Keys.Control);

            while (processCount < 3)
            {
                if (PaddleOcr.获取图片文字(1298, 1661, 107, 41) == "隐藏文件夹")
                {
                    processCount++;
                }

                Delay(25);
            }

            Delay(100);
            // 剪切板操作只能在UI线程
            Invoke(() => { Clipboard.SetText(listStr[3] + "_冻结呈请"); });
            SimKeyBoard.KeyPressWhile(Keys.V, Keys.Control);

            Delay(150);
            SimKeyBoard.KeyPressAlt(Keys.S);

            while (processCount < 4)
            {
                if (File.Exists("Z:\\desktop\\" + listStr[3] + "_冻结呈请.jpg"))
                {
                    processCount++;
                }

                Delay(25);
            }

            SimKeyBoard.KeyPressAlt(Keys.F4);

            while (processCount < 5)
            {
                if (PaddleOcr.获取图片文字(50, 110, 50, 33) == "周报")
                {
                    processCount++;
                }

                Delay(25);
            }

            SimKeyBoard.MouseMove(2765, 937);
            SimKeyBoard.MouseLeftClick();
            Delay(50);

            SimKeyBoard.KeyPressWhile(Keys.W, Keys.Control);

            SimKeyBoard.MouseMove(p.X, p.Y);

            Invoke(() => { comboBox3.SelectedIndex = 2; });
        });
    }

    private static void 测试()
    {
        _ = MessageBox.Show(
            PaddleOcr.获取图片文字(50, 110, 50, 33)
        );
    }

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

    #region 全局数据类型和固定字段

    /// <summary>
    ///     默认存储路径
    /// </summary>
    public const string FilePath = "rexConfig.json";

    /// <summary>
    ///     全局共用一个对象
    /// </summary>
    public static RexGroups gRexGroups;

    /// <summary>
    ///     全局数据类型
    /// </summary>
    internal class RexGroups
    {
        public List<RexGroup> Groups { get; set; }
    }

    /// <summary>
    ///     数据组类型
    /// </summary>
    internal class RexGroup
    {
        public string Group { get; set; }
        public List<RexTitle> Titles { get; set; }
    }

    /// <summary>
    ///     详细项目类型
    /// </summary>
    internal class RexTitle
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string ReplacementsReg { get; set; }
        public string ReplacementsText { get; set; }
    }

    #endregion

    #region 页面改变现实的逻辑

    /// <summary>
    ///     存储数据
    /// </summary>
    /// <param name="rexGroups"></param>
    public static void SaveDate(RexGroups rexGroups)
    {
        JsonSerializerSettings setting = new()
        {
            StringEscapeHandling = StringEscapeHandling.Default
        };

        string jsonString = JsonConvert.SerializeObject(rexGroups, Formatting.Indented, setting);

        File.WriteAllText(FilePath, jsonString);
    }

    /// <summary>
    ///     加载数据
    /// </summary>
    /// <returns>RexGroup</returns>
    public static RexGroups LoadDate()
    {
        if (File.Exists(FilePath))
        {
            string jsonString = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<RexGroups>(jsonString);
        }

        return null;
    }

    /// <summary>
    ///     页面加载数据
    /// </summary>
    private void LoadPageDate()
    {
        gRexGroups = LoadDate();

        if (gRexGroups != null)
        {
            foreach (RexGroup rexGroup in gRexGroups.Groups)
            {
                _ = comboBox1.Items.Add(rexGroup.Group);
            }
        }
    }


    /// <summary>
    ///     全列表刷新
    /// </summary>
    private void comboBox1_Refresh()
    {
        // var selectStr = comboBox1.Text;

        comboBox1.Text = "";
        comboBox2.Text = "";

        comboBox1.Items.Clear();

        foreach (RexGroup rexGroup in gRexGroups.Groups)
        {
            _ = comboBox1.Items.Add(rexGroup.Group);
        }

        comboBox2.Items.Clear();
    }

    /// <summary>
    ///     刷新具体列表
    /// </summary>
    private void comboBox2_Refresh()
    {
        string selectStr = comboBox1.Text;

        int index = gRexGroups.Groups.FindIndex(c => c.Group == selectStr);

        if (index != -1)
        {
            comboBox2.Items.Clear();
            foreach (RexTitle rexTitle in gRexGroups.Groups[index].Titles)
            {
                _ = comboBox2.Items.Add(rexTitle.Title);
            }
        }
    }

    #endregion

    #region 页面元素事件

    /// <summary>
    ///     替换按钮绑定正则替换
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReplace_Click(object sender, EventArgs e)
    {
        textBox4.Text = Regex.Replace(textBox1.Text, textBox2.Text, textBox3.Text);
    }

    /// <summary>
    ///     关闭前保存现有
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _ = PaddleOcr.释放PaddleOcr();
        SaveDate(gRexGroups);
    }

    /// <summary>
    ///     规则组类别选择触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        comboBox2_Refresh();
    }

    /// <summary>
    ///     组明细类别选择触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (comboBox2.SelectedItem == null)
        {
            return;
        }

        string selectStr = comboBox2.SelectedItem.ToString();

        foreach (RexGroup rexGroup in gRexGroups.Groups)
        {
            foreach (RexTitle rexTitle in rexGroup.Titles)
            {
                if (rexTitle.Title == selectStr)
            {
                textBox1.Text = rexTitle.Text;
                textBox2.Text = rexTitle.ReplacementsReg;
                textBox3.Text = rexTitle.ReplacementsText;
                break;
            }
            }
        }
    }

    /// <summary>
    ///     添加更改功能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnModify_Click(object sender, EventArgs e)
    {
        string selectStrGroup = comboBox1.Text;
        string selectStrTitle = comboBox2.Text;

        int rexGroupIndex = gRexGroups.Groups.FindIndex(c => c.Group == selectStrGroup);

        if (rexGroupIndex == -1)
        {
            gRexGroups.Groups.Add(new RexGroup
            {
                Group = selectStrGroup,
                Titles =
                [
                    new RexTitle
                    {
                        Title = selectStrTitle,
                        Text = textBox1.Text,
                        ReplacementsReg = textBox2.Text,
                        ReplacementsText = textBox3.Text
                    }
                ]
            });

            comboBox1_Refresh();
        }
        else
        {
            int rexTitleIndex = gRexGroups.Groups[rexGroupIndex].Titles.FindIndex(c => c.Title == selectStrTitle);

            if (rexTitleIndex == -1)
            {
                gRexGroups.Groups[rexGroupIndex].Titles.Add(new RexTitle
                {
                    Title = selectStrTitle,
                    Text = textBox1.Text,
                    ReplacementsReg = textBox2.Text,
                    ReplacementsText = textBox3.Text
                });

                comboBox2_Refresh();
            }
            else
            {
                gRexGroups.Groups[rexGroupIndex].Titles[rexTitleIndex].Text = textBox1.Text;
                gRexGroups.Groups[rexGroupIndex].Titles[rexTitleIndex].ReplacementsReg = textBox2.Text;
                gRexGroups.Groups[rexGroupIndex].Titles[rexTitleIndex].ReplacementsText = textBox3.Text;
            }
        }
    }

    /// <summary>
    ///     删除对应列表功能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
        string selectStrGroup = comboBox1.Text;
        string selectStrTitle = comboBox2.Text;

        int rexGroupIndex = gRexGroups.Groups.FindIndex(c => c.Group == selectStrGroup);

        if (rexGroupIndex != -1)
        {
            int rexTitleIndex = gRexGroups.Groups[rexGroupIndex].Titles.FindIndex(c => c.Title == selectStrTitle);

            if (rexTitleIndex != -1)
            {
                gRexGroups.Groups[rexGroupIndex].Titles.RemoveAt(rexTitleIndex);

                comboBox2_Refresh();
            }
        }

        comboBox2.Text = "";
    }

    /// <summary>
    ///     删除对应的组功能
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDeleteGroup_Click(object sender, EventArgs e)
    {
        DialogResult result = MessageBox.Show(@"确认删除该规则组码？", @"谨慎确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            string selectStrGroup = comboBox1.Text;

            int rexGroupIndex = gRexGroups.Groups.FindIndex(c => c.Group == selectStrGroup);

            if (rexGroupIndex != -1)
            {
                gRexGroups.Groups.RemoveAt(rexGroupIndex);
                comboBox1_Refresh();
            }
        }
    }

    #endregion
}