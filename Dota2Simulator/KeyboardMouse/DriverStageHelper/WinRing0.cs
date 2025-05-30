﻿using System;
using System.Runtime.InteropServices;

namespace TestKeyboard.DriverStageHelper
{
    /// <summary>
    ///     驱动级帮助类WinRing0
    ///     WinRing0使用条件：
    ///     1.使用PS/2键盘
    ///     2.将WinRing0x64.dll和WinRing0x64.sys拷贝到程序运行的根目录下
    ///     3.使用管理员打开程序
    /// </summary>
    internal class WinRing0
    {
        private static Ols ols;

        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint Ucode, uint uMapType);

        public static bool init()
        {
            ols = new Ols();
            return ols.GetStatus() == (uint)Ols.Status.NO_ERROR;
        }

        private static void KBCWait4IBE()
        {
            byte dwVal = 0;
            do
            {
                _ = ols.ReadIoPortByteEx(0x64, ref dwVal);
            } while ((dwVal & 0x2) > 0);
        }

        public static void KeyDown(char ch)
        {
            Console.WriteLine("Down ch 16进制: {0:X}", (byte)ch);
            int btScancode = MapVirtualKey(ch, 0);
            Console.WriteLine(btScancode);
            Console.WriteLine("Down btScancode 16进制: {0:X}", (byte)btScancode);
            KBCWait4IBE();
            ols.WriteIoPortByte(0x64, 0xd2);
            KBCWait4IBE();
            ols.WriteIoPortByte(0x60, (byte)btScancode);
        }

        public static void KeyUp(char ch)
        {
            int btScancode = MapVirtualKey(ch, 0);
            KBCWait4IBE();
            ols.WriteIoPortByte(0x64, 0xd2);
            KBCWait4IBE();
            ols.WriteIoPortByte(0x60, (byte)(btScancode | 0x80));
            //Console.WriteLine("Up 16进制: {0:X}", (byte)(btScancode | 0x80));
        }
    }

    internal class Ols : IDisposable
    {
        private const string dllNameX64 = "WinRing0x64.dll";
        private const string dllName = "WinRing0.dll";

        // for this support library
        internal enum Status
        {
            NO_ERROR = 0,
            DLL_NOT_FOUND = 1,
            DLL_INCORRECT_VERSION = 2,
            DLL_INITIALIZE_ERROR = 3
        }

        // for WinRing0
        internal enum OlsDllStatus
        {
            OLS_DLL_NO_ERROR = 0,
            OLS_DLL_UNSUPPORTED_PLATFORM = 1,
            OLS_DLL_DRIVER_NOT_LOADED = 2,
            OLS_DLL_DRIVER_NOT_FOUND = 3,
            OLS_DLL_DRIVER_UNLOADED = 4,
            OLS_DLL_DRIVER_NOT_LOADED_ON_NETWORK = 5,
            OLS_DLL_UNKNOWN_ERROR = 9
        }

        // for WinRing0
        internal enum OlsDriverType
        {
            OLS_DRIVER_TYPE_UNKNOWN = 0,
            OLS_DRIVER_TYPE_WIN_9X = 1,
            OLS_DRIVER_TYPE_WIN_NT = 2,
            OLS_DRIVER_TYPE_WIN_NT4 = 3, // Obsolete
            OLS_DRIVER_TYPE_WIN_NT_X64 = 4,
            OLS_DRIVER_TYPE_WIN_NT_IA64 = 5
        }

        // for WinRing0
        internal enum OlsErrorPci : uint
        {
            OLS_ERROR_PCI_BUS_NOT_EXIST = 0xE0000001,
            OLS_ERROR_PCI_NO_DEVICE = 0xE0000002,
            OLS_ERROR_PCI_WRITE_CONFIG = 0xE0000003,
            OLS_ERROR_PCI_READ_CONFIG = 0xE0000004
        }

        // Bus Number, Device Number and Function Number to PCI Device Address
        public static uint PciBusDevFunc(uint bus, uint dev, uint func)
        {
            return ((bus & 0xFF) << 8) | ((dev & 0x1F) << 3) | (func & 7);
        }

        // PCI Device Address to Bus Number
        public static uint PciGetBus(uint address)
        {
            return (address >> 8) & 0xFF;
        }

        // PCI Device Address to Device Number
        public static uint PciGetDev(uint address)
        {
            return (address >> 3) & 0x1F;
        }

        // PCI Device Address to Function Number
        public static uint PciGetFunc(uint address)
        {
            return address & 7;
        }

        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string lpFileName);


        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        private IntPtr module = IntPtr.Zero;
        private readonly uint status = (uint)Status.NO_ERROR;

        public Ols()
        {
            string fileName = IntPtr.Size == 8 ? dllNameX64 : dllName;
            module = LoadLibrary(fileName);
            if (module == IntPtr.Zero)
            {
                status = (uint)Status.DLL_NOT_FOUND;
            }
            else
            {
                GetDllStatus = (_GetDllStatus)GetDelegate("GetDllStatus", typeof(_GetDllStatus));
                GetDllVersion = (_GetDllVersion)GetDelegate("GetDllVersion", typeof(_GetDllVersion));
                GetDriverVersion = (_GetDriverVersion)GetDelegate("GetDriverVersion", typeof(_GetDriverVersion));
                GetDriverType = (_GetDriverType)GetDelegate("GetDriverType", typeof(_GetDriverType));

                InitializeOls = (_InitializeOls)GetDelegate("InitializeOls", typeof(_InitializeOls));
                DeinitializeOls = (_DeinitializeOls)GetDelegate("DeinitializeOls", typeof(_DeinitializeOls));

                IsCpuid = (_IsCpuid)GetDelegate("IsCpuid", typeof(_IsCpuid));
                IsMsr = (_IsMsr)GetDelegate("IsMsr", typeof(_IsMsr));
                IsTsc = (_IsTsc)GetDelegate("IsTsc", typeof(_IsTsc));
                Hlt = (_Hlt)GetDelegate("Hlt", typeof(_Hlt));
                HltTx = (_HltTx)GetDelegate("HltTx", typeof(_HltTx));
                HltPx = (_HltPx)GetDelegate("HltPx", typeof(_HltPx));
                Rdmsr = (_Rdmsr)GetDelegate("Rdmsr", typeof(_Rdmsr));
                RdmsrTx = (_RdmsrTx)GetDelegate("RdmsrTx", typeof(_RdmsrTx));
                RdmsrPx = (_RdmsrPx)GetDelegate("RdmsrPx", typeof(_RdmsrPx));
                Wrmsr = (_Wrmsr)GetDelegate("Wrmsr", typeof(_Wrmsr));
                WrmsrTx = (_WrmsrTx)GetDelegate("WrmsrTx", typeof(_WrmsrTx));
                WrmsrPx = (_WrmsrPx)GetDelegate("WrmsrPx", typeof(_WrmsrPx));
                Rdpmc = (_Rdpmc)GetDelegate("Rdpmc", typeof(_Rdpmc));
                RdpmcTx = (_RdpmcTx)GetDelegate("RdpmcTx", typeof(_RdpmcTx));
                RdpmcPx = (_RdpmcPx)GetDelegate("RdpmcPx", typeof(_RdpmcPx));
                Cpuid = (_Cpuid)GetDelegate("Cpuid", typeof(_Cpuid));
                CpuidTx = (_CpuidTx)GetDelegate("CpuidTx", typeof(_CpuidTx));
                CpuidPx = (_CpuidPx)GetDelegate("CpuidPx", typeof(_CpuidPx));
                Rdtsc = (_Rdtsc)GetDelegate("Rdtsc", typeof(_Rdtsc));
                RdtscTx = (_RdtscTx)GetDelegate("RdtscTx", typeof(_RdtscTx));
                RdtscPx = (_RdtscPx)GetDelegate("RdtscPx", typeof(_RdtscPx));

                ReadIoPortByte = (_ReadIoPortByte)GetDelegate("ReadIoPortByte", typeof(_ReadIoPortByte));
                ReadIoPortWord = (_ReadIoPortWord)GetDelegate("ReadIoPortWord", typeof(_ReadIoPortWord));
                ReadIoPortDword = (_ReadIoPortDword)GetDelegate("ReadIoPortDword", typeof(_ReadIoPortDword));
                ReadIoPortByteEx = (_ReadIoPortByteEx)GetDelegate("ReadIoPortByteEx", typeof(_ReadIoPortByteEx));
                ReadIoPortWordEx = (_ReadIoPortWordEx)GetDelegate("ReadIoPortWordEx", typeof(_ReadIoPortWordEx));
                ReadIoPortDwordEx = (_ReadIoPortDwordEx)GetDelegate("ReadIoPortDwordEx", typeof(_ReadIoPortDwordEx));

                WriteIoPortByte = (_WriteIoPortByte)GetDelegate("WriteIoPortByte", typeof(_WriteIoPortByte));
                WriteIoPortWord = (_WriteIoPortWord)GetDelegate("WriteIoPortWord", typeof(_WriteIoPortWord));
                WriteIoPortDword = (_WriteIoPortDword)GetDelegate("WriteIoPortDword", typeof(_WriteIoPortDword));
                WriteIoPortByteEx = (_WriteIoPortByteEx)GetDelegate("WriteIoPortByteEx", typeof(_WriteIoPortByteEx));
                WriteIoPortWordEx = (_WriteIoPortWordEx)GetDelegate("WriteIoPortWordEx", typeof(_WriteIoPortWordEx));
                WriteIoPortDwordEx = (_WriteIoPortDwordEx)GetDelegate("WriteIoPortDwordEx", typeof(_WriteIoPortDwordEx));

                SetPciMaxBusIndex = (_SetPciMaxBusIndex)GetDelegate("SetPciMaxBusIndex", typeof(_SetPciMaxBusIndex));
                ReadPciConfigByte = (_ReadPciConfigByte)GetDelegate("ReadPciConfigByte", typeof(_ReadPciConfigByte));
                ReadPciConfigWord = (_ReadPciConfigWord)GetDelegate("ReadPciConfigWord", typeof(_ReadPciConfigWord));
                ReadPciConfigDword = (_ReadPciConfigDword)GetDelegate("ReadPciConfigDword", typeof(_ReadPciConfigDword));
                ReadPciConfigByteEx =
                    (_ReadPciConfigByteEx)GetDelegate("ReadPciConfigByteEx", typeof(_ReadPciConfigByteEx));
                ReadPciConfigWordEx =
                    (_ReadPciConfigWordEx)GetDelegate("ReadPciConfigWordEx", typeof(_ReadPciConfigWordEx));
                ReadPciConfigDwordEx =
                    (_ReadPciConfigDwordEx)GetDelegate("ReadPciConfigDwordEx", typeof(_ReadPciConfigDwordEx));
                WritePciConfigByte = (_WritePciConfigByte)GetDelegate("WritePciConfigByte", typeof(_WritePciConfigByte));
                WritePciConfigWord = (_WritePciConfigWord)GetDelegate("WritePciConfigWord", typeof(_WritePciConfigWord));
                WritePciConfigDword =
                    (_WritePciConfigDword)GetDelegate("WritePciConfigDword", typeof(_WritePciConfigDword));
                WritePciConfigByteEx =
                    (_WritePciConfigByteEx)GetDelegate("WritePciConfigByteEx", typeof(_WritePciConfigByteEx));
                WritePciConfigWordEx =
                    (_WritePciConfigWordEx)GetDelegate("WritePciConfigWordEx", typeof(_WritePciConfigWordEx));
                WritePciConfigDwordEx =
                    (_WritePciConfigDwordEx)GetDelegate("WritePciConfigDwordEx", typeof(_WritePciConfigDwordEx));
                FindPciDeviceById = (_FindPciDeviceById)GetDelegate("FindPciDeviceById", typeof(_FindPciDeviceById));
                FindPciDeviceByClass =
                    (_FindPciDeviceByClass)GetDelegate("FindPciDeviceByClass", typeof(_FindPciDeviceByClass));

#if _PHYSICAL_MEMORY_SUPPORT
                        ReadDmiMemory = (_ReadDmiMemory)GetDelegate("ReadDmiMemory", typeof(_ReadDmiMemory));
                        ReadPhysicalMemory =
         (_ReadPhysicalMemory)GetDelegate("ReadPhysicalMemory", typeof(_ReadPhysicalMemory));
                        WritePhysicalMemory =
         (_WritePhysicalMemory)GetDelegate("WritePhysicalMemory", typeof(_WritePhysicalMemory));
#endif
                if (!(
                        GetDllStatus != null
                        && GetDllVersion != null
                        && GetDriverVersion != null
                        && GetDriverType != null
                        && InitializeOls != null
                        && DeinitializeOls != null
                        && IsCpuid != null
                        && IsMsr != null
                        && IsTsc != null
                        && Hlt != null
                        && HltTx != null
                        && HltPx != null
                        && Rdmsr != null
                        && RdmsrTx != null
                        && RdmsrPx != null
                        && Wrmsr != null
                        && WrmsrTx != null
                        && WrmsrPx != null
                        && Rdpmc != null
                        && RdpmcTx != null
                        && RdpmcPx != null
                        && Cpuid != null
                        && CpuidTx != null
                        && CpuidPx != null
                        && Rdtsc != null
                        && RdtscTx != null
                        && RdtscPx != null
                        && ReadIoPortByte != null
                        && ReadIoPortWord != null
                        && ReadIoPortDword != null
                        && ReadIoPortByteEx != null
                        && ReadIoPortWordEx != null
                        && ReadIoPortDwordEx != null
                        && WriteIoPortByte != null
                        && WriteIoPortWord != null
                        && WriteIoPortDword != null
                        && WriteIoPortByteEx != null
                        && WriteIoPortWordEx != null
                        && WriteIoPortDwordEx != null
                        && SetPciMaxBusIndex != null
                        && ReadPciConfigByte != null
                        && ReadPciConfigWord != null
                        && ReadPciConfigDword != null
                        && ReadPciConfigByteEx != null
                        && ReadPciConfigWordEx != null
                        && ReadPciConfigDwordEx != null
                        && WritePciConfigByte != null
                        && WritePciConfigWord != null
                        && WritePciConfigDword != null
                        && WritePciConfigByteEx != null
                        && WritePciConfigWordEx != null
                        && WritePciConfigDwordEx != null
                        && FindPciDeviceById != null
                        && FindPciDeviceByClass != null
#if _PHYSICAL_MEMORY_SUPPORT
                        && ReadDmiMemory != null
                        && ReadPhysicalMemory != null
                        && WritePhysicalMemory != null
#endif
                    ))
                {
                    status = (uint)Status.DLL_INCORRECT_VERSION;
                }

                if (InitializeOls() == 0)
                {
                    status = (uint)Status.DLL_INITIALIZE_ERROR;
                }
            }
        }

        public uint GetStatus()
        {
            return status;
        }

        public void Dispose()
        {
            if (module != IntPtr.Zero)
            {
                DeinitializeOls();
                _ = FreeLibrary(module);
                module = IntPtr.Zero;
            }
        }

        public Delegate GetDelegate(string procName, Type delegateType)
        {
            nint ptr = GetProcAddress(module, procName);
            if (ptr != IntPtr.Zero)
            {
                Delegate d = Marshal.GetDelegateForFunctionPointer(ptr, delegateType);
                return d;
            }

            int result = Marshal.GetHRForLastWin32Error();
            throw Marshal.GetExceptionForHR(result);
        }

        //-----------------------------------------------------------------------------
        // DLL Information
        //-----------------------------------------------------------------------------
        internal delegate uint _GetDllStatus();

        internal delegate uint _GetDllVersion(ref byte major, ref byte minor, ref byte revision, ref byte release);

        internal delegate uint _GetDriverVersion(ref byte major, ref byte minor, ref byte revision, ref byte release);

        internal delegate uint _GetDriverType();

        internal delegate int _InitializeOls();

        internal delegate void _DeinitializeOls();

        public _GetDllStatus GetDllStatus;
        public _GetDriverType GetDriverType;
        public _GetDllVersion GetDllVersion;
        public _GetDriverVersion GetDriverVersion;

        public _InitializeOls InitializeOls;
        public _DeinitializeOls DeinitializeOls;

        //-----------------------------------------------------------------------------
        // CPU
        //-----------------------------------------------------------------------------
        internal delegate int _IsCpuid();

        internal delegate int _IsMsr();

        internal delegate int _IsTsc();

        internal delegate int _Hlt();

        internal delegate int _HltTx(UIntPtr threadAffinityMask);

        internal delegate int _HltPx(UIntPtr processAffinityMask);

        internal delegate int _Rdmsr(uint index, ref uint eax, ref uint edx);

        internal delegate int _RdmsrTx(uint index, ref uint eax, ref uint edx, UIntPtr threadAffinityMask);

        internal delegate int _RdmsrPx(uint index, ref uint eax, ref uint edx, UIntPtr processAffinityMask);

        internal delegate int _Wrmsr(uint index, uint eax, uint edx);

        internal delegate int _WrmsrTx(uint index, uint eax, uint edx, UIntPtr threadAffinityMask);

        internal delegate int _WrmsrPx(uint index, uint eax, uint edx, UIntPtr processAffinityMask);

        internal delegate int _Rdpmc(uint index, ref uint eax, ref uint edx);

        internal delegate int _RdpmcTx(uint index, ref uint eax, ref uint edx, UIntPtr threadAffinityMask);

        internal delegate int _RdpmcPx(uint index, ref uint eax, ref uint edx, UIntPtr processAffinityMask);

        internal delegate int _Cpuid(uint index, ref uint eax, ref uint ebx, ref uint ecx, ref uint edx);

        internal delegate int _CpuidTx(uint index, ref uint eax, ref uint ebx, ref uint ecx, ref uint edx,
            UIntPtr threadAffinityMask);

        internal delegate int _CpuidPx(uint index, ref uint eax, ref uint ebx, ref uint ecx, ref uint edx,
            UIntPtr processAffinityMask);

        internal delegate int _Rdtsc(ref uint eax, ref uint edx);

        internal delegate int _RdtscTx(ref uint eax, ref uint edx, UIntPtr threadAffinityMask);

        internal delegate int _RdtscPx(ref uint eax, ref uint edx, UIntPtr processAffinityMask);

        public _IsCpuid IsCpuid;
        public _IsMsr IsMsr;
        public _IsTsc IsTsc;
        public _Hlt Hlt;
        public _HltTx HltTx;
        public _HltPx HltPx;
        public _Rdmsr Rdmsr;
        public _RdmsrTx RdmsrTx;
        public _RdmsrPx RdmsrPx;
        public _Wrmsr Wrmsr;
        public _WrmsrTx WrmsrTx;
        public _WrmsrPx WrmsrPx;
        public _Rdpmc Rdpmc;
        public _RdpmcTx RdpmcTx;
        public _RdpmcPx RdpmcPx;
        public _Cpuid Cpuid;
        public _CpuidTx CpuidTx;
        public _CpuidPx CpuidPx;
        public _Rdtsc Rdtsc;
        public _RdtscTx RdtscTx;
        public _RdtscPx RdtscPx;

        //-----------------------------------------------------------------------------
        // I/O
        //-----------------------------------------------------------------------------
        internal delegate byte _ReadIoPortByte(ushort port);

        internal delegate ushort _ReadIoPortWord(ushort port);

        internal delegate uint _ReadIoPortDword(ushort port);

        public _ReadIoPortByte ReadIoPortByte;
        public _ReadIoPortWord ReadIoPortWord;
        public _ReadIoPortDword ReadIoPortDword;

        internal delegate int _ReadIoPortByteEx(ushort port, ref byte value);

        internal delegate int _ReadIoPortWordEx(ushort port, ref ushort value);

        internal delegate int _ReadIoPortDwordEx(ushort port, ref uint value);

        public _ReadIoPortByteEx ReadIoPortByteEx;
        public _ReadIoPortWordEx ReadIoPortWordEx;
        public _ReadIoPortDwordEx ReadIoPortDwordEx;

        internal delegate void _WriteIoPortByte(ushort port, byte value);

        internal delegate void _WriteIoPortWord(ushort port, ushort value);

        internal delegate void _WriteIoPortDword(ushort port, uint value);

        public _WriteIoPortByte WriteIoPortByte;
        public _WriteIoPortWord WriteIoPortWord;
        public _WriteIoPortDword WriteIoPortDword;

        internal delegate int _WriteIoPortByteEx(ushort port, byte value);

        internal delegate int _WriteIoPortWordEx(ushort port, ushort value);

        internal delegate int _WriteIoPortDwordEx(ushort port, uint value);

        public _WriteIoPortByteEx WriteIoPortByteEx;
        public _WriteIoPortWordEx WriteIoPortWordEx;
        public _WriteIoPortDwordEx WriteIoPortDwordEx;

        //-----------------------------------------------------------------------------
        // PCI
        //-----------------------------------------------------------------------------
        internal delegate void _SetPciMaxBusIndex(byte max);

        public _SetPciMaxBusIndex SetPciMaxBusIndex;

        internal delegate byte _ReadPciConfigByte(uint pciAddress, byte regAddress);

        internal delegate ushort _ReadPciConfigWord(uint pciAddress, byte regAddress);

        internal delegate uint _ReadPciConfigDword(uint pciAddress, byte regAddress);

        public _ReadPciConfigByte ReadPciConfigByte;
        public _ReadPciConfigWord ReadPciConfigWord;
        public _ReadPciConfigDword ReadPciConfigDword;

        internal delegate int _ReadPciConfigByteEx(uint pciAddress, uint regAddress, ref byte value);

        internal delegate int _ReadPciConfigWordEx(uint pciAddress, uint regAddress, ref ushort value);

        internal delegate int _ReadPciConfigDwordEx(uint pciAddress, uint regAddress, ref uint value);

        public _ReadPciConfigByteEx ReadPciConfigByteEx;
        public _ReadPciConfigWordEx ReadPciConfigWordEx;
        public _ReadPciConfigDwordEx ReadPciConfigDwordEx;

        internal delegate void _WritePciConfigByte(uint pciAddress, byte regAddress, byte value);

        internal delegate void _WritePciConfigWord(uint pciAddress, byte regAddress, ushort value);

        internal delegate void _WritePciConfigDword(uint pciAddress, byte regAddress, uint value);

        public _WritePciConfigByte WritePciConfigByte;
        public _WritePciConfigWord WritePciConfigWord;
        public _WritePciConfigDword WritePciConfigDword;

        internal delegate int _WritePciConfigByteEx(uint pciAddress, uint regAddress, byte value);

        internal delegate int _WritePciConfigWordEx(uint pciAddress, uint regAddress, ushort value);

        internal delegate int _WritePciConfigDwordEx(uint pciAddress, uint regAddress, uint value);

        public _WritePciConfigByteEx WritePciConfigByteEx;
        public _WritePciConfigWordEx WritePciConfigWordEx;
        public _WritePciConfigDwordEx WritePciConfigDwordEx;

        internal delegate uint _FindPciDeviceById(ushort vendorId, ushort deviceId, byte index);

        internal delegate uint _FindPciDeviceByClass(byte baseClass, byte subClass, byte programIf, byte index);

        public _FindPciDeviceById FindPciDeviceById;
        public _FindPciDeviceByClass FindPciDeviceByClass;

        //-----------------------------------------------------------------------------
        // Physical Memory (unsafe)
        //-----------------------------------------------------------------------------
#if _PHYSICAL_MEMORY_SUPPORT
                public unsafe delegate uint _ReadDmiMemory(byte* buffer, uint count, uint unitSize);
                public _ReadDmiMemory ReadDmiMemory;

                public unsafe delegate uint _ReadPhysicalMemory(UIntPtr address, byte* buffer, uint count, uint unitSize);
                public unsafe delegate uint _WritePhysicalMemory(UIntPtr address, byte* buffer, uint count, uint unitSize);

                public _ReadPhysicalMemory ReadPhysicalMemory;
                public _WritePhysicalMemory WritePhysicalMemory;
#endif
    }
}