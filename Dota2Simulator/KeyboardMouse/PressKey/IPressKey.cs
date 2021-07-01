namespace TestKeyboard.PressKey
{
    interface IPressKey
    {
        bool Initialize(EnumWindowsType winType);

        void KeyPress(char key);

        void KeyDown(char key);

        void KeyUp(char key);
    }
}
