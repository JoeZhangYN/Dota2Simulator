using System;
using System.Runtime.InteropServices;

namespace Dota2Simulator
{
    internal static class SetWindowTop
    {
        // 设置此窗体为活动窗体：
        // 将创建指定窗口的线程带到前台并激活该窗口。键盘输入直接指向窗口，并为用户更改各种视觉提示。
        // 系统为创建前台窗口的线程分配的优先级略高于其他线程。
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // 设置此窗体为活动窗体：
        // 激活窗口。窗口必须附加到调用线程的消息队列。
        [DllImport("user32.dll", EntryPoint = "SetActiveWindow")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        // 设置窗体位置
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int width, int height,
            int flags);
    }
}