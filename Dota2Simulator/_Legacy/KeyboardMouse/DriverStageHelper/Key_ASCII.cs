namespace Dota2Simulator.KeyboardMouse.DriverStageHelper
{
    internal class Key_ASCII
    {
        public const short A = 0x41;
        public const short B = 0x42;
        public const short C = 0x43;
        public const short D = 0x44;
        public const short E = 0x45;
        public const short F = 0x46;
        public const short G = 0x47;
        public const short H = 0x48;
        public const short I = 0x49;
        public const short J = 0x4A;
        public const short K = 0x4B;
        public const short L = 0x4C;
        public const short M = 0x4D;
        public const short N = 0x4E;
        public const short O = 0x4F;
        public const short P = 0x50;
        public const short Q = 0x51;
        public const short R = 0x52;
        public const short S = 0x53;
        public const short T = 0x54;
        public const short U = 0x55;
        public const short V = 0x56;
        public const short W = 0x57;
        public const short X = 0x58;
        public const short Y = 0x59;
        public const short Z = 0x5A;
        public const short 鼠标左键 = 0x1;
        public const short 鼠标右键 = 0x2;
        public const short Cancel = 0x3;
        public const short 鼠标中键 = 0x4;
        public const short Backspace = 0x8;
        public const short Tab = 0x9;
        public const short Clear = 0xC;
        public const short Enter = 0xD;
        public const short Shift = 0x10;
        public const short Ctrl = 0x11;
        public const short Menu = 0x12;
        public const short Pause = 0x13;
        public const short Caps = 0x14;
        public const short Esc = 0x1B;
        public const short Space = 0x20;
        public const short PageUp = 0x21;
        public const short PageDown = 0x22;
        public const short End = 0x23;
        public const short Home = 0x24;
        public const short Left = 0x25;
        public const short Up = 0x26;
        public const short Right = 0x27;
        public const short Down = 0x28;
        public const short Select = 0x29;
        public const short Print = 0x2A;
        public const short Execute = 0x2B;
        public const short Snapshot = 0x2C;
        public const short Insert = 0x2D;
        public const short Delete = 0x2E;
        public const short Help = 0x2F;
        public const short Num = 0x90;
        public const short D0 = 0x30;
        public const short D1 = 0x31;
        public const short D2 = 0x32;
        public const short D3 = 0x33;
        public const short D4 = 0x34;
        public const short D5 = 0x35;
        public const short D6 = 0x36;
        public const short D7 = 0x37;
        public const short D8 = 0x38;
        public const short D9 = 0x39;
        public const short NumPad0 = 0x60;
        public const short NumPad1 = 0x61;
        public const short NumPad02 = 0x62;
        public const short NumPad03 = 0x63;
        public const short NumPad04 = 0x64;
        public const short NumPad05 = 0x65;
        public const short NumPad06 = 0x66;
        public const short NumPad07 = 0x67;
        public const short NumPad08 = 0x68;
        public const short NumPad09 = 0x69;
        public const short F1 = 0x70;
        public const short F2 = 0x71;
        public const short F3 = 0x72;
        public const short F4 = 0x73;
        public const short F5 = 0x74;
        public const short F6 = 0x75;
        public const short F7 = 0x76;
        public const short F8 = 0x77;
        public const short F9 = 0x78;
        public const short F10 = 0x79;
        public const short F11 = 0x7A;
        public const short F12 = 0x7B;
        public const short F13 = 0x7C;
        public const short F14 = 0x7D;
        public const short F15 = 0x7E;
        public const short F16 = 0x7F;
    }


    #region ASCII

    /*
     *
     *
        20	（空格）(␠)
        21	!
        22	"
        23	#
        24	$
        25	%
        26	&
        27	'
        28	(
        29	)
        2A	*
        2B	+
        2C	,
        2D	-
        2E	.
        2F	/
        30	0
        31	1
        32	2
        33	3
        34	4
        35	5
        36	6
        37	7
        38	8
        39	9
        3A	:
        3B	;
        3C	<
        3D	=
        3E	>
        3F	?
        40	@
        5B	[
        5C	\
        5D	]
        5E	^
        5F	_
        60	`
        7B	{
        7C	|
        7D	}
        7E	~

        0x1 鼠标左键
        0x2 鼠标右键
        0x3 CANCEL 键
        0x4 鼠标中键
        0x8 BACKSPACE 键
        0x9 TAB 键
        0xC CLEAR 键
        0xD ENTER 键
        0x10 SHIFT 键
        0x11 CTRL 键
        0x12 MENU 键
        0x13 PAUSE 键
        0x14 CAPS LOCK 键
        0x1B ESC 键
        0x20 SPACEBAR 键
        0x21 PAGE UP 键
        0x22 PAGE DOWN 键
        0x23 END 键
        0x24 HOME 键
        0x25 LEFT ARROW 键
        0x26 UP ARROW 键
        0x27 RIGHT ARROW 键
        0x28 DOWN ARROW 键
        0x29 SELECT 键
        0x2A PRINT SCREEN 键
        0x2B EXECUTE 键
        0x2C SNAPSHOT 键
        0x2D INSERT 键
        0x2E DELETE 键
        0x2F HELP 键
        0x90 NUM LOCK 键

        0x41 A 键
        0x42 B 键
        0x43 C 键
        0x44 D 键
        0x45 E 键
        0x46 F 键
        0x47 G 键
        0x48 H 键
        0x49 I 键
        0x4A J 键
        0x4B K 键
        0x4C L 键
        0x4D M 键
        0x4E N 键
        0x4F O 键
        0x50 P 键
        0x51 Q 键
        0x52 R 键
        0x53 S 键
        0x54 T 键
        0x55 U 键
        0x56 V 键
        0x57 W 键
        0x58 X 键
        0x59 Y 键
        0x5A Z 键

        0x30 0 键
        0x31 1 键
        0x32 2 键
        0x33 3 键
        0x34 4 键
        0x35 5 键
        0x36 6 键
        0x37 7 键
        0x38 8 键
        0x39 9 键

        小键盘
        0x60 NumPad00 键
        0x61 NumPad01 键
        0x62 NumPad02 键
        0x63 NumPad03 键
        0x64 NumPad04 键
        0x65 NumPad05 键
        0x66 NumPad06 键
        0x67 NumPad07 键
        0x68 NumPad08 键
        0x69 NumPad09 键
        0x6A MULTIPLICATION SIGN (*) 键
        0x6B PLUS SIGN (+) 键
        0x6C ENTER 键
        0x6D MINUS SIGN (–) 键
        0x6E DECIMAL POINT (.) 键
        0x6F DIVISION SIGN (/) 键

        下列常数代表功能键：
        值     描述
        0x70 F1 键
        0x71 F2 键
        0x72 F3 键
        0x73 F4 键
        0x74 F5 键
        0x75 F6 键
        0x76 F7 键
        0x77 F8 键
        0x78 F9 键
        0x79 F10 键
        0x7A F11 键
        0x7B F12 键
        0x7C F13 键
        0x7D F14 键
        0x7E F15 键
        0x7F F16 键

        ESC键 VK_ESCAPE (27)
        回车键： VK_RETURN (13)
        TAB键： VK_TAB (9)
        Caps Lock键： VK_CAPITAL (20)
        Shift键： VK_SHIFT ($10)
        Ctrl键： VK_CONTROL (17)
        Alt键： VK_MENU (18)
        空格键： VK_SPACE ($20/32)
        退格键： VK_BACK (8)
        左徽标键： VK_LWIN (91)
        右徽标键： VK_LWIN (92)
        鼠标右键快捷键：VK_APPS (93)

        Insert键： VK_Insert (45)
        Home键： VK_HOME (36)
        Page Up： VK_PRIOR (33)
        PageDown： VK_NEXT (34)
        End键： VK_END (35)
        Delete键： VK_Delete (46)

        方向键(←)： VK_LEFT (37)
        方向键(↑)： VK_UP (38)
        方向键(→)： VK_RIGHT (39)
        方向键(↓)： VK_DOWN (40)

        F1键： VK_F1 (112)
        F2键： VK_F2 (113)
        F3键： VK_F3 (114)
        F4键： VK_F4 (115)
        F5键： VK_F5 (116)
        F6键： VK_F6 (117)
        F7键： VK_F7 (118)
        F8键： VK_F8 (119)
        F9键： VK_F9 (120)
        F10键： VK_F10 (121)
        F11键： VK_F11 (122)
        F12键： VK_F12 (123)

        Num Lock键： VK_NUMLOCK (144)
        小键盘0： VK_NUMPAD0 (96)
        小键盘1： VK_NUMPAD0 (97)
        小键盘2： VK_NUMPAD0 (98)
        小键盘3： VK_NUMPAD0 (99)
        小键盘4： VK_NUMPAD0 (100)
        小键盘5： VK_NUMPAD0 (101)
        小键盘6： VK_NUMPAD0 (102)
        小键盘7： VK_NUMPAD0 (103)
        小键盘8： VK_NUMPAD0 (104)
        小键盘9： VK_NUMPAD0 (105)
        小键盘.： VK_DECIMAL (110)
        小键盘*： VK_MULTIPLY (106)
        小键盘+： VK_MULTIPLY (107)
        小键盘-： VK_SUBTRACT (109)
        小键盘/： VK_DIVIDE (111)

        Pause Break键： VK_PAUSE (19)
        Scroll Lock键： VK_SCROLL (145)

     */

    #endregion
}