using SharpHook;
using SharpHook.Data;
using SharpHook.Native;
using System;

namespace CSharpSnakeProject.Input
{
    public static class KeyboardHelper
    {
        private static readonly TaskPoolGlobalHook _hook = new TaskPoolGlobalHook();
        private static volatile bool _isShiftPressed = false;

        static KeyboardHelper()
        {
            _hook.KeyPressed += OnKeyPressed;
            _hook.KeyReleased += OnKeyReleased;
            _hook.RunAsync();
        }

        private static void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            if (e.Data.KeyCode == KeyCode.VcLeftShift || e.Data.KeyCode == KeyCode.VcRightShift)
            {
                _isShiftPressed = true;
            }
        }

        private static void OnKeyReleased(object? sender, KeyboardHookEventArgs e)
        {
            if (e.Data.KeyCode == KeyCode.VcLeftShift || e.Data.KeyCode == KeyCode.VcRightShift)
            {
                _isShiftPressed = false;
            }
        }

        public static bool IsShiftHeld()
        {
            return _isShiftPressed;
        }
    }
}
