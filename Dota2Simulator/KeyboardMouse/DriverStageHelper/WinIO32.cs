﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TestKeyboard.DriverStageHelper
{
    /// <summary>
    ///     驱动级帮助类WinIO32
    ///     WinIO使用条件：
    ///     1.电脑启用“测试模式”，管理员打开CMD，输入bcdedit /set testsigning on打开测试模式，bcdedit /set testsigning off关闭测试模式
    ///     2.使用PS/2键盘
    ///     3.将WinIO32.dll和WinIO32.sys拷贝到程序运行的根目录下
    ///     4.注册WinIo64的签名，右键WinIo64.sys->属性->数字签名->选择签名列表的项->详细信息->查看证书->安装证书->本地计算机->下一步->将所有的证书都放入下列存储->浏览->受信任的根证书发布机构->确定->下一步->完成->提示导入成功
    ///     5.管理员打开程序
    ///     https://blog.csdn.net/no99es/article/details/50537102
    /// </summary>
    internal class WinIO32
    {
        private const int KBC_KEY_CMD = 0x64;
        private const int KBC_KEY_DATA = 0x60;


        private WinIO32()
        {
            IsInitialize = true;
        }

        private static bool IsInitialize { get; set; }

        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint Ucode, uint uMapType);

        public static void Initialize()
        {
            if (InitializeWinIo())
            {
                KBCWait4IBE();
                IsInitialize = true;
            }
            else
            {
                _ = MessageBox.Show("Load WinIO Failed!");
            }
        }

        public static void Shutdown()
        {
            if (IsInitialize)
            {
                ShutdownWinIo();
            }

            IsInitialize = false;
        }

        /// <summary>
        ///     等待键盘缓冲区为空
        /// </summary>
        private static void KBCWait4IBE()
        {
            int dwVal;
            do
            {
                _ = GetPortVal(0x64, out dwVal, 1);
            } while ((dwVal & 0x2) > 0);
        }

        /// <summary>
        ///     模拟键盘标按下
        /// </summary>
        /// <param name="vKeyCoad"></param>
        public static void KeyDown(Keys vKeyCoad)
        {
            if (!IsInitialize)
            {
                return;
            }

            int btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            //SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            //KBCWait4IBE();
            //SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            //KBCWait4IBE();
            _ = SetPortVal(KBC_KEY_CMD, 0xD2, 1);
            KBCWait4IBE();
            _ = SetPortVal(KBC_KEY_DATA, btScancode, 1);
        }

        /// <summary>
        ///     模拟键盘弹出
        /// </summary>
        /// <param name="vKeyCoad"></param>
        public static void KeyUp(Keys vKeyCoad)
        {
            if (!IsInitialize)
            {
                return;
            }

            int btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            //KBCWait4IBE();
            //SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            //KBCWait4IBE();
            //SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            KBCWait4IBE();
            _ = SetPortVal(KBC_KEY_CMD, 0xD2, 1);
            KBCWait4IBE();
            _ = SetPortVal(KBC_KEY_DATA, btScancode | 0x80, 1);
        }

        #region WinIo32.dll

        [DllImport("WinIo32.dll")]
        public static extern bool InitializeWinIo();

        [DllImport("WinIo32.dll")]
        public static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);

        [DllImport("WinIo32.dll")]
        public static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);

        [DllImport("WinIo32.dll")]
        public static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);

        [DllImport("WinIo32.dll")]
        public static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);

        [DllImport("WinIo32.dll")]
        public static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);

        [DllImport("WinIo32.dll")]
        public static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);

        [DllImport("WinIo32.dll")]
        public static extern void ShutdownWinIo();

        #endregion
    }
}