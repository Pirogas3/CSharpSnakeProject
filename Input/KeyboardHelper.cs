using System.Runtime.InteropServices;

namespace CSharpSnakeProject.Input
{
    public static class KeyboardHelper
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        public static bool IsShiftHeld()
        {
            return (GetAsyncKeyState(0x10) & 0x8000) != 0;
        }
    }
}
