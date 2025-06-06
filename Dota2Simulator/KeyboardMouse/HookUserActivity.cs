﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dota2Simulator.KeyboardMouse
{
    /// <summary>
    ///     This class allows you to tap keyboard and mouse and / or to detect their activity even when an
    ///     application runes in background or does not have any user interface at all. This class raises
    ///     common .NET events with KeyEventArgs and MouseEventArgs so you can easily retrieve any information you need.
    /// </summary>
    /// <summary>
    ///     This class allows you to tap keyboard and mouse and / or to detect their activity even when an
    ///     application runes in background or does not have any user interface at all. This class raises
    ///     common .NET events with KeyEventArgs and MouseEventArgs so you can easily retrieve any information you need.
    /// </summary>
    internal class HookUserActivity
    {
        #region 构造和析构函数

        /*
                /// <summary>
                /// Creates an instance of UserActivityHook object and sets mouse and keyboard hooks.
                /// </summary>
                /// <exception cref="Win32Exception">Any windows problem.</exception>
                public HookUserActivity()
                {
                    isInstallMouseHook = true;
                    isInstallKeyboardHook = true;

                }

                /// <summary>
                /// Creates an instance of UserActivityHook object and installs both or one of mouse and/or keyboard hooks and starts rasing events
                /// </summary>
                /// <param name="InstallMouseHook"><b>true</b> if mouse events must be monitored</param>
                /// <param name="InstallKeyboardHook"><b>true</b> if keyboard events must be monitored</param>
                /// <exception cref="Win32Exception">Any windows problem.</exception>
                /// <remarks>
                /// To create an instance without installing hooks call new UserActivityHook(false, false)
                /// </remarks>
                public HookUserActivity(bool InstallMouseHook, bool InstallKeyboardHook)
                {
                    isInstallMouseHook = InstallMouseHook;
                    isInstallKeyboardHook = InstallKeyboardHook;
                }
        */

        /// <summary>
        ///     Destruction.
        ///     Uninstall hooks and do not throw exceptions
        /// </summary>
        ~HookUserActivity()
        {
            //uninstall hooks and do not throw exceptions
            Stop(true, true, false);
        }

        #endregion

        #region Windows structure definitions

        /// <summary>
        ///     The POINT structure defines the x- and y- coordinates of a point.
        /// </summary>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdi/rectangl_0tiq.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            /// <summary>
            ///     Specifies the x-coordinate of the point.
            /// </summary>
            public int x;

            /// <summary>
            ///     Specifies the y-coordinate of the point.
            /// </summary>
            public int y;
        }

        /// <summary>
        ///     The MOUSEHOOKSTRUCT structure contains information about a mouse event passed to a WH_MOUSE hook procedure,
        ///     MouseProc.
        /// </summary>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            /// <summary>
            ///     Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates.
            /// </summary>
            public POINT pt;

            /// <summary>
            ///     Handle to the window that will receive the mouse message corresponding to the mouse event.
            /// </summary>
            public int hwnd;

            /// <summary>
            ///     Specifies the hit-test value. For a list of hit-test values, see the description of the WM_NCHITTEST message.
            /// </summary>
            public int wHitTestCode;

            /// <summary>
            ///     Specifies extra information associated with the message.
            /// </summary>
            public int dwExtraInfo;
        }

        /// <summary>
        ///     The MSLLHOOKSTRUCT structure contains information about a low-level keyboard input event.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class MouseLLHookStruct
        {
            /// <summary>
            ///     Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates.
            /// </summary>
            public POINT pt;

            /// <summary>
            ///     If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta.
            ///     The low-order word is reserved. A positive value indicates that the wheel was rotated forward,
            ///     away from the user; a negative value indicates that the wheel was rotated backward, toward the user.
            ///     One wheel click is defined as WHEEL_DELTA, which is 120.
            ///     If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP,
            ///     or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released,
            ///     and the low-order word is reserved. This value can be one or more of the following values. Otherwise, mouseData is
            ///     not used.
            ///     XBUTTON1
            ///     The first X button was pressed or released.
            ///     XBUTTON2
            ///     The second X button was pressed or released.
            /// </summary>
            public int mouseData;

            /// <summary>
            ///     Specifies the event-injected flag. An application can use the following value to test the mouse flags. Value
            ///     Purpose
            ///     LLMHF_INJECTED Test the event-injected flag.
            ///     0
            ///     Specifies whether the event was injected. The value is 1 if the event was injected; otherwise, it is 0.
            ///     1-15
            ///     Reserved.
            /// </summary>
            public int flags;

            /// <summary>
            ///     Specifies the time stamp for this message.
            /// </summary>
            public int time;

            /// <summary>
            ///     Specifies extra information associated with the message.
            /// </summary>
            public int dwExtraInfo;
        }


        /// <summary>
        ///     The KBDLLHOOKSTRUCT structure contains information about a low-level keyboard input event.
        /// </summary>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            /// <summary>
            ///     Specifies a virtual-key code. The code must be a value in the range 1 to 254.
            /// </summary>
            public int vkCode;

            /// <summary>
            ///     Specifies a hardware scan code for the key.
            /// </summary>
            public int scanCode;

            /// <summary>
            ///     Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            /// </summary>
            public int flags;

            /// <summary>
            ///     Specifies the time stamp for this message.
            /// </summary>
            public int time;

            /// <summary>
            ///     Specifies extra information associated with the message.
            /// </summary>
            public int dwExtraInfo;
        }

        /// <summary>
        ///     作用范围
        /// </summary>
        internal enum HookScopeType
        {
            /// <summary>
            ///     全局的级别
            /// </summary>
            GlobalScope = 1,

            /// <summary>
            ///     应用程序级别
            /// </summary>
            Application = 2,

            /// <summary>
            ///     窗口级别
            /// </summary>
            FormScope = 3
        }

        #endregion

        #region Windows API function imports

        /// <summary>
        ///     The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
        ///     You would install a hook procedure to monitor the system for certain types of events. These events
        ///     are associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">
        ///     [in] Specifies the type of hook procedure to be installed. This parameter can be one of the following values.
        /// </param>
        /// <param name="lpfn">
        ///     [in] Pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a
        ///     thread created by a different process, the lpfn parameter must point to a hook procedure in a dynamic-link
        ///     library (DLL). Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hMod">
        ///     [in] Handle to the DLL containing the hook procedure pointed to by the lpfn parameter.
        ///     The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by
        ///     the current process and if the hook procedure is within the code associated with the current process.
        /// </param>
        /// <param name="dwThreadId">
        ///     [in] Specifies the identifier of the thread with which the hook procedure is to be associated.
        ///     If this parameter is zero, the hook procedure is associated with all existing threads running in the
        ///     same desktop as the calling thread.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the handle to the hook procedure.
        ///     If the function fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hMod,
            int dwThreadId);

        /// <summary>
        ///     The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx
        ///     function.
        /// </summary>
        /// <param name="idHook">
        ///     [in] Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to
        ///     SetWindowsHookEx.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        /// <summary>
        ///     The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
        ///     A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="idHook">Ignored.</param>
        /// <param name="nCode">
        ///     [in] Specifies the hook code passed to the current hook procedure.
        ///     The next hook procedure uses this code to determine how to process the hook information.
        /// </param>
        /// <param name="wParam">
        ///     [in] Specifies the wParam value passed to the current hook procedure.
        ///     The meaning of this parameter depends on the type of hook associated with the current hook chain.
        /// </param>
        /// <param name="lParam">
        ///     [in] Specifies the lParam value passed to the current hook procedure.
        ///     The meaning of this parameter depends on the type of hook associated with the current hook chain.
        /// </param>
        /// <returns>
        ///     This value is returned by the next hook procedure in the chain.
        ///     The current hook procedure must also return this value. The meaning of the return value depends on the hook type.
        ///     For more information, see the descriptions of the individual hook procedures.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(
            int idHook,
            int nCode,
            int wParam,
            IntPtr lParam);

        /// <summary>
        ///     The CallWndProc hook procedure is an application-defined or library-defined callback
        ///     function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer
        ///     to this callback function. CallWndProc is a placeholder for the application-defined
        ///     or library-defined function name.
        /// </summary>
        /// <param name="nCode">
        ///     [in] Specifies whether the hook procedure must process the message.
        ///     If nCode is HC_ACTION, the hook procedure must process the message.
        ///     If nCode is less than zero, the hook procedure must pass the message to the
        ///     CallNextHookEx function without further processing and must return the
        ///     value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        ///     [in] Specifies whether the message was sent by the current thread.
        ///     If the message was sent by the current thread, it is nonzero; otherwise, it is zero.
        /// </param>
        /// <param name="lParam">
        ///     [in] Pointer to a CWPSTRUCT structure that contains details about the message.
        /// </param>
        /// <returns>
        ///     If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.
        ///     If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx
        ///     and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC
        ///     hooks will not receive hook notifications and may behave incorrectly as a result. If the hook
        ///     procedure does not call CallNextHookEx, the return value should be zero.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
        /// </remarks>
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        ///     The ToAscii function translates the specified virtual-key code and keyboard
        ///     state to the corresponding character or characters. The function translates the code
        ///     using the input language and physical keyboard layout identified by the keyboard layout handle.
        /// </summary>
        /// <param name="uVirtKey">
        ///     [in] Specifies the virtual-key code to be translated.
        /// </param>
        /// <param name="uScanCode">
        ///     [in] Specifies the hardware scan code of the key to be translated.
        ///     The high-order bit of this value is set if the key is up (not pressed).
        /// </param>
        /// <param name="lpbKeyState">
        ///     [in] Pointer to a 256-byte array that contains the current keyboard state.
        ///     Each element (byte) in the array contains the state of one key.
        ///     If the high-order bit of a byte is set, the key is down (pressed).
        ///     The low bit, if set, indicates that the key is toggled on. In this function,
        ///     only the toggle bit of the CAPS LOCK key is relevant. The toggle state
        ///     of the NUM LOCK and SCROLL LOCK keys is ignored.
        /// </param>
        /// <param name="lpwTransKey">
        ///     [out] Pointer to the buffer that receives the translated character or characters.
        /// </param>
        /// <param name="fuState">
        ///     [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise.
        /// </param>
        /// <returns>
        ///     If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values.
        ///     Value Meaning
        ///     0 The specified virtual key has no translation for the current state of the keyboard.
        ///     1 One character was copied to the buffer.
        ///     2 Two characters were copied to the buffer. This usually happens when a dead-key character
        ///     (accent or diacritic) stored in the keyboard layout cannot be composed with the specified
        ///     virtual key to form a single character.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpwTransKey,
            int fuState);

        /// <summary>
        ///     The GetKeyboardState function copies the status of the 256 virtual keys to the
        ///     specified buffer.
        /// </summary>
        /// <param name="pbKeyState">
        ///     [in] Pointer to a 256-byte array that contains keyboard key states.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is nonzero.
        ///     If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);


        /// <summary>
        ///     获取（焦点、活动）窗口的句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        ///     获取指定窗口句柄的进程ID
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="ID">进程ID号</param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        /// <summary>
        ///     获取窗口句柄，我一般都是用lpClassName来获取特定窗口，
        ///     曾经使用过lpWindowName，好像无法获取到窗口的句柄，不知道为什么；
        ///     ps：可以使用vs自带的spy++工具来查看特定窗口的类名称
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [DllImport("User32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #endregion

        #region Windows constants

        //values from Winuser.h in Microsoft SDK.
        /// <summary>
        ///     Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        private const int WH_MOUSE_LL = 14;

        /// <summary>
        ///     Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        private const int WH_KEYBOARD_LL = 13;

        /// <summary>
        ///     Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure.
        /// </summary>
        private const int WH_MOUSE = 7;

        /// <summary>
        ///     Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook
        ///     procedure.
        /// </summary>
        private const int WH_KEYBOARD = 2;

        /// <summary>
        ///     The WM_MOUSEMOVE message is posted to a window when the cursor moves.
        /// </summary>
        private const int WM_MOUSEMOVE = 0x200;

        /// <summary>
        ///     The WM_LBUTTONDOWN message is posted when the user presses the left mouse button
        /// </summary>
        private const int WM_LBUTTONDOWN = 0x201;

        /// <summary>
        ///     The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        private const int WM_RBUTTONDOWN = 0x204;

        /// <summary>
        ///     The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button
        /// </summary>
        private const int WM_MBUTTONDOWN = 0x207;

        /// <summary>
        ///     The WM_LBUTTONUP message is posted when the user releases the left mouse button
        /// </summary>
        private const int WM_LBUTTONUP = 0x202;

        /// <summary>
        ///     The WM_RBUTTONUP message is posted when the user releases the right mouse button
        /// </summary>
        private const int WM_RBUTTONUP = 0x205;

        /// <summary>
        ///     The WM_MBUTTONUP message is posted when the user releases the middle mouse button
        /// </summary>
        private const int WM_MBUTTONUP = 0x208;

        /// <summary>
        ///     The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button
        /// </summary>
        private const int WM_LBUTTONDBLCLK = 0x203;

        /// <summary>
        ///     The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button
        /// </summary>
        private const int WM_RBUTTONDBLCLK = 0x206;

        /// <summary>
        ///     The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        private const int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>
        ///     The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel.
        /// </summary>
        private const int WM_MOUSEWHEEL = 0x020A;

        /// <summary>
        ///     The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem
        ///     key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        private const int WM_KEYDOWN = 0x100;

        /// <summary>
        ///     The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem
        ///     key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed,
        ///     or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        private const int WM_KEYUP = 0x101;

        /// <summary>
        ///     The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user
        ///     presses the F10 key (which activates the menu bar) or holds down the ALT key and then
        ///     presses another key. It also occurs when no window currently has the keyboard focus;
        ///     in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that
        ///     receives the message can distinguish between these two contexts by checking the context
        ///     code in the lParam parameter.
        /// </summary>
        private const int WM_SYSKEYDOWN = 0x104;

        /// <summary>
        ///     The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user
        ///     releases a key that was pressed while the ALT key was held down. It also occurs when no
        ///     window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent
        ///     to the active window. The window that receives the message can distinguish between
        ///     these two contexts by checking the context code in the lParam parameter.
        /// </summary>
        private const int WM_SYSKEYUP = 0x105;

        /// <summary>
        ///     Shift键
        /// </summary>
        private const byte VK_SHIFT = 0x10;

        /// <summary>
        ///     Ctrl键
        /// </summary>
        private const byte VK_CONTROL = 0x11;

        /// <summary>
        ///     Alt键
        /// </summary>
        private const byte VK_MENU = 0x12;

        /// <summary>
        ///     Caps Lock键
        /// </summary>
        private const byte VK_CAPITAL = 0x14;

        /// <summary>
        ///     Num Lock键
        /// </summary>
        private const byte VK_NUMLOCK = 0x90;

        #endregion

        #region event Object

        /// <summary>
        ///     Occurs when the user moves the mouse, presses any mouse button or scrolls the wheel
        /// </summary>
        public event MouseEventHandler OnMouseActivity;

        /// <summary>
        ///     Occurs when the user presses a key
        /// </summary>
        public event KeyEventHandler KeyDown;

        /// <summary>
        ///     Occurs when the user presses and releases
        /// </summary>
        public event KeyPressEventHandler KeyPress;

        /// <summary>
        ///     Occurs when the user releases a key
        /// </summary>
        public event KeyEventHandler KeyUp;

        #endregion

        #region Public Property

        /// <summary>
        ///     钩子的作用范围
        /// </summary>
        public HookScopeType HookScope { set; get; }

        /// <summary>
        ///     当HookScopeType设置为Form的时候，需指定钩子监听的窗口；
        ///     当指定的窗口为活动窗口，则监听和处理对应的注册事件
        /// </summary>
        public Form ActivityForm { set; get; }

        #endregion

        #region private field

        /// <summary>
        ///     Stores the handle to the mouse hook procedure.
        /// </summary>
        private int hMouseHook;

        /// <summary>
        ///     Stores the handle to the keyboard hook procedure.
        /// </summary>
        private int hKeyboardHook;

        /// <summary>
        ///     是否安装鼠标钩子
        /// </summary>
        private bool isInstallMouseHook;

        /// <summary>
        ///     是否安装键盘钩子
        /// </summary>
        private bool isInstallKeyboardHook;

        /// <summary>
        ///     Declare MouseHookProcedure as HookProc type.
        /// </summary>
        private static HookProc MouseHookProcedure;

        /// <summary>
        ///     Declare KeyboardHookProcedure as HookProc type.
        /// </summary>
        private static HookProc KeyboardHookProcedure;

        #endregion

        #region public Method

        /// <summary>
        ///     Installs both mouse and keyboard hooks and starts raising events
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Start()
        {
            isInstallMouseHook = isInstallKeyboardHook = true;
            Start(isInstallMouseHook, isInstallKeyboardHook);
        }

        /// <summary>
        ///     Installs both or one of mouse and/or keyboard hooks and starts raising events
        /// </summary>
        /// <param name="InstallMouseHook"><b>true</b> if mouse events must be monitored</param>
        /// <param name="InstallKeyboardHook"><b>true</b> if keyboard events must be monitored</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Start(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            // install Mouse hook only if it is not installed and must be installed
            if (hMouseHook == 0 && InstallMouseHook)
            {
                // Create an instance of HookProc.
                MouseHookProcedure = new HookProc(MouseHookProc);
                //install hook
                hMouseHook = SetWindowsHookEx(
                    WH_MOUSE_LL,
                    MouseHookProcedure,
                    Marshal.GetHINSTANCE(
                        Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //If SetWindowsHookEx fails.
                if (hMouseHook == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup
                    Stop(true, false, false);
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }

            // install Keyboard hook only if it is not installed and must be installed
            if (hKeyboardHook == 0 && InstallKeyboardHook)
            {
                // Create an instance of HookProc.
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                //install hook
                hKeyboardHook = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    KeyboardHookProcedure,
                    Marshal.GetHINSTANCE(
                        Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //If SetWindowsHookEx fails.
                if (hKeyboardHook == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup
                    Stop(false, true, false);
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        /// <summary>
        ///     Stops monitoring both mouse and keyboard events and rasing events.
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Stop()
        {
            Stop(true, true, true);
        }

        /// <summary>
        ///     Stops monitoring both or one of mouse and/or keyboard events and rasing events.
        /// </summary>
        /// <param name="UninstallMouseHook"><b>true</b> if mouse hook must be uninstalled</param>
        /// <param name="UninstallKeyboardHook"><b>true</b> if keyboard hook must be uninstalled</param>
        /// <param name="ThrowExceptions"><b>true</b> if exceptions which occured during uninstalling must be thrown</param>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        public void Stop(bool UninstallMouseHook, bool UninstallKeyboardHook, bool ThrowExceptions)
        {
            //if mouse hook set and must be uninstalled
            if (hMouseHook != 0 && UninstallMouseHook)
            {
                //uninstall hook
                int retMouse = UnhookWindowsHookEx(hMouseHook);
                //reset invalid handle
                hMouseHook = 0;
                //if failed and exception must be thrown
                if (retMouse == 0 && ThrowExceptions)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }

            //if keyboard hook set and must be uninstalled
            if (hKeyboardHook != 0 && UninstallKeyboardHook)
            {
                //uninstall hook
                int retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                //reset invalid handle
                hKeyboardHook = 0;
                //if failed and exception must be thrown
                if (retKeyboard == 0 && ThrowExceptions)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        #endregion

        #region private Method

        /// <summary>
        ///     A callback function which will be called every time a mouse activity detected.
        /// </summary>
        /// <param name="nCode">
        ///     [in] Specifies whether the hook procedure must process the message.
        ///     If nCode is HC_ACTION, the hook procedure must process the message.
        ///     If nCode is less than zero, the hook procedure must pass the message to the
        ///     CallNextHookEx function without further processing and must return the
        ///     value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        ///     [in] Specifies whether the message was sent by the current thread.
        ///     If the message was sent by the current thread, it is nonzero; otherwise, it is zero.
        /// </param>
        /// <param name="lParam">
        ///     [in] Pointer to a CWPSTRUCT structure that contains details about the message.
        /// </param>
        /// <returns>
        ///     If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.
        ///     If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx
        ///     and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC
        ///     hooks will not receive hook notifications and may behave incorrectly as a result. If the hook
        ///     procedure does not call CallNextHookEx, the return value should be zero.
        /// </returns>
        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            // if ok and someone listens to our events
            if (nCode >= 0 && OnMouseActivity != null)
            {
                //Marshall the data from callback.
                MouseLLHookStruct mouseHookStruct = Marshal.PtrToStructure<MouseLLHookStruct>(lParam);

                //detect button clicked
                System.Windows.Forms.MouseButtons button = System.Windows.Forms.MouseButtons.None;
                short mouseDelta = 0;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        //case WM_LBUTTONUP: 
                        //case WM_LBUTTONDBLCLK: 
                        button = System.Windows.Forms.MouseButtons.Left;
                        break;
                    case WM_RBUTTONDOWN:
                        //case WM_RBUTTONUP: 
                        //case WM_RBUTTONDBLCLK: 
                        button = System.Windows.Forms.MouseButtons.Right;
                        break;
                    case WM_MOUSEWHEEL:
                        //If the message is WM_MOUSEWHEEL, the high-order word of mouseData member is the wheel delta. 
                        //One wheel click is defined as WHEEL_DELTA, which is 120. 
                        //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                        mouseDelta = (short)((mouseHookStruct.mouseData >> 16) & 0xffff);
                        //TODO: X BUTTONS (I havent them so was unable to test)
                        //If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
                        //or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
                        //and the low-order word is reserved. This value can be one or more of the following values. 
                        //Otherwise, mouseData is not used. 
                        break;
                }

                //double clicks
                int clickCount = 0;
                if (button != System.Windows.Forms.MouseButtons.None)
                {
                    clickCount = wParam == WM_LBUTTONDBLCLK || wParam == WM_RBUTTONDBLCLK ? 2 : 1;
                }

                //generate event 
                MouseEventArgs e = new(
                    button,
                    clickCount,
                    mouseHookStruct.pt.x,
                    mouseHookStruct.pt.y,
                    mouseDelta);
                //raise it
                OnMouseActivity(this, e);
            }

            //call next hook
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }

        /// <summary>
        ///     A callback function which will be called every time a keyboard activity detected.
        /// </summary>
        /// <param name="nCode">
        ///     [in] Specifies whether the hook procedure must process the message.
        ///     If nCode is HC_ACTION, the hook procedure must process the message.
        ///     If nCode is less than zero, the hook procedure must pass the message to the
        ///     CallNextHookEx function without further processing and must return the
        ///     value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        ///     [in] Specifies whether the message was sent by the current thread.
        ///     If the message was sent by the current thread, it is nonzero; otherwise, it is zero.
        /// </param>
        /// <param name="lParam">
        ///     [in] Pointer to a CWPSTRUCT structure that contains details about the message.
        /// </param>
        /// <returns>
        ///     If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx.
        ///     If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx
        ///     and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC
        ///     hooks will not receive hook notifications and may behave incorrectly as a result. If the hook
        ///     procedure does not call CallNextHookEx, the return value should be zero.
        /// </returns>
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            int handling = 0;

            if (HookScope == HookScopeType.FormScope && Form.ActiveForm == ActivityForm)
            {
                handling = KeyboardEventHandling(nCode, wParam, lParam);
            }

            if (HookScope == HookScopeType.Application && Environment.ProcessId == GetForegroundWindowProcessID())
            {
                handling = KeyboardEventHandling(nCode, wParam, lParam);
            }

            if (HookScope == HookScopeType.GlobalScope)
            {
                handling = KeyboardEventHandling(nCode, wParam, lParam);
            }

            return handling;
        }

        /// <summary>
        ///     根据用户注册的按键事件，进行逐个事件的处理
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int KeyboardEventHandling(int nCode, int wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            bool handled = false;
            //it was ok and someone listens to events
            if (nCode >= 0 && (KeyDown != null || KeyUp != null || KeyPress != null))
            {
                short vShift = GetKeyState(VK_SHIFT);
                short vControl = GetKeyState(VK_CONTROL);
                short vAlt = GetKeyState(VK_MENU);
                short vCapital = GetKeyState(VK_CAPITAL);

                log("---------------");
                log("vShift = " + Convert.ToString(vShift, 2));
                log("vControl = " + Convert.ToString(vControl, 2));
                log("vAlt = " + Convert.ToString(vAlt, 2));
                log("vCapital = " + Convert.ToString(vCapital, 2));

                //read structure KeyboardHookStruct at lParam
                KeyboardHookStruct MyKeyboardHookStruct = Marshal.PtrToStructure<KeyboardHookStruct>(lParam);
                //raise KeyDown
                if (KeyDown != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new(
                        keyData |
                        ((vShift & 0x80) == 0x80 ? Keys.Shift : Keys.None) |
                        ((vControl & 0x80) == 0x80 ? Keys.Control : Keys.None) |
                        ((vAlt & 0x80) == 0x80 ? Keys.Alt : Keys.None)
                    );

                    KeyDown(this, e);
                    handled = handled || e.Handled;
                }

                // raise KeyPress
                if (KeyPress != null && wParam == WM_KEYDOWN)
                {
                    bool isDownShift = (vShift & 0x80) == 0x80;
                    bool isDownCapslock = vCapital != 0;

                    byte[] keyState = new byte[256];
                    _ = GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode,
                            MyKeyboardHookStruct.scanCode,
                            keyState,
                            inBuffer,
                            MyKeyboardHookStruct.flags) == 1)
                    {
                        char key = (char)inBuffer[0];
                        if (isDownCapslock ^ isDownShift && char.IsLetter(key))
                        {
                            key = char.ToUpper(key);
                        }

                        KeyPressEventArgs e = new(key);
                        KeyPress(this, e);
                        handled = handled || e.Handled;
                    }
                }

                // raise KeyUp
                if (KeyUp != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new(
                        keyData |
                        ((vShift & 0x81) == 0x81 ? Keys.Shift : Keys.None) |
                        ((vControl & 0x81) == 0x81 ? Keys.Control : Keys.None) |
                        ((vAlt & 0x81) == 0x81 ? Keys.Alt : Keys.None)
                    );
                    KeyUp(this, e);
                    handled = handled || e.Handled;
                }
            }

            //if event handled in application do not handoff to other listeners
            return handled ? 1 : CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        /// <summary>
        ///     获取活动窗口应用程序的进程ID
        ///     根据窗口句柄来获取进程ID。
        /// </summary>
        /// <returns></returns>
        private static int GetForegroundWindowProcessID()
        {
            _ = GetWindowThreadProcessId(GetForegroundWindow(), out int processID);
            return processID;
        }

        /// <summary>
        ///     在输出窗口显示调试信息
        /// </summary>
        /// <param name="msg"></param>
        private static void log(string msg)
        {
            Console.WriteLine(msg);
        }

        #endregion
    }
}