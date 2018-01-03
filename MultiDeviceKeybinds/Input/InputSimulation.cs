using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace MultiDeviceKeybinds
{
    public static class InputSimulation
    {
        private static InputSimulator InputSimulator = new InputSimulator();

        public static readonly int KeyboardDelay = 1;
        public static readonly int KeyboardSpeed = 31;

        static InputSimulation()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Keyboard"))
            {
                if (!int.TryParse((string)key.GetValue("KeyboardDelay", "1"), out KeyboardDelay)) KeyboardDelay = 1;
                if (!int.TryParse((string)key.GetValue("KeyboardSpeed", "31"), out KeyboardSpeed)) KeyboardSpeed = 31;
            }
        }

        internal static void KeyDown(VirtualKeyCode key)
        {
            InputSimulator.Keyboard.KeyDown(key);
        }

        public static void KeyDown(Keys key)
        {
            KeyDown((VirtualKeyCode)key);
        }

        internal static void SetKeyState(VirtualKeyCode key, KeyState state)
        {
            if (state == KeyState.Make)
            {
                KeyDown(key);
            }
            else
            {
                KeyUp(key);
            }
        }

        public static void SetKeyState(Keys key, KeyState state)
        {
            SetKeyState((VirtualKeyCode)key, state);
        }

        internal static void KeyPress(VirtualKeyCode key)
        {
            SetKeyState(key, KeyState.Make);
            SetKeyState(key, KeyState.Break);
        }

        public static void KeyPress(Keys key)
        {
            KeyPress((VirtualKeyCode)key);
        }

        public static void HoldKey(Keys key, int duration)
        {
            int initialDelay = KeyboardDelay * 1000, delay = KeyboardSpeed;

            KeyDown(key);

            if (duration > initialDelay)
            {
                Sleep(initialDelay);

                int repeat = (duration - initialDelay) / delay;
                while (repeat-- > 0)
                {
                    KeyDown(key);

                    Sleep(delay);
                }
            }
            else
            {
                Sleep(duration);
            }

            KeyUp(key);
        }

        public static void Sleep(int duration)
        {
            InputSimulator.Keyboard.Sleep(duration);
        }

        public static void Write(string text)
        {
            InputSimulator.Keyboard.TextEntry(text);
        }

        internal static void KeyUp(VirtualKeyCode key)
        {
            InputSimulator.Keyboard.KeyUp(key);
        }

        public static void KeyUp(Keys key)
        {
            KeyUp((VirtualKeyCode)key);
        }

        internal static void ModifiedKeyStroke(VirtualKeyCode modifier, IEnumerable<VirtualKeyCode> keys)
        {
            InputSimulator.Keyboard.ModifiedKeyStroke(modifier, keys);
        }

        public static void ModifiedKeyStroke(Keys modifier, IEnumerable<Keys> keys)
        {
            ModifiedKeyStroke((VirtualKeyCode)modifier, keys.Select(k => (VirtualKeyCode)k));
        }

        internal static void ModifiedKeyStroke(VirtualKeyCode modifier, VirtualKeyCode key)
        {
            InputSimulator.Keyboard.ModifiedKeyStroke(modifier, key);
        }

        public static void ModifiedKeyStroke(Keys modifier, Keys key)
        {
            ModifiedKeyStroke((VirtualKeyCode)modifier, (VirtualKeyCode)key);
        }

        internal static void ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifiers, IEnumerable<VirtualKeyCode> keys)
        {
            InputSimulator.Keyboard.ModifiedKeyStroke(modifiers, keys);
        }

        public static void ModifiedKeyStroke(IEnumerable<Keys> modifiers, IEnumerable<Keys> keys)
        {
            ModifiedKeyStroke(modifiers.Select(m => (VirtualKeyCode)m), keys.Select(k => (VirtualKeyCode)k));
        }

        internal static void ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifiers, VirtualKeyCode key)
        {
            InputSimulator.Keyboard.ModifiedKeyStroke(modifiers, key);
        }

        public static void ModifiedKeyStroke(IEnumerable<Keys> modifiers, Keys key)
        {
            ModifiedKeyStroke(modifiers.Select(m => (VirtualKeyCode)m), (VirtualKeyCode)key);
        }
    }
}
