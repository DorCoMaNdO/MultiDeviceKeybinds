using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    class ForegroundWindowCondition : ICondition
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, long nMaxCount);

        public string Name { get { return "Is Foreground Window"; } }
        public string Description { get { return "Checks if a process' window is the foreground window"; } }
        public string ArgumentsTaken { get { return "(string) process name\r\n[string] window class name"; } }

        public bool Test(KeybindDevice device, Keys key, Keys correctedKey, KeyState state, KeyState lastState, string guid, params object[] args)
        {
            if (args.Length < 1 || !(args[0] is string processname)) return false;

            IntPtr hwnd = Program.ForegroundWindow;

            try
            {
                GetWindowThreadProcessId(hwnd, out int pid);

                Process p = Process.GetProcessById(pid);

                bool match = p.ProcessName.Length == processname.Length && p.ProcessName.Equals(processname, StringComparison.InvariantCultureIgnoreCase);

                if (match && args.Length > 1 && args[1] is string classname)
                {
                    string hwndclass = GetClassNameOfWindow(hwnd);

                    match = hwndclass != null && classname.Length == hwndclass.Length && classname.Equals(hwndclass, StringComparison.InvariantCultureIgnoreCase);
                }

                return match;
            }
            catch
            {
            }

            return false;
        }

        private static string GetClassNameOfWindow(IntPtr hwnd)
        {
            try
            {
                const int ClassnameMaxLength = 256;
                StringBuilder classText = new StringBuilder(ClassnameMaxLength);
                GetClassName(hwnd, classText, ClassnameMaxLength);

                return classText.ToString();
            }
            catch
            {
            }

            return null;
        }
    }
}