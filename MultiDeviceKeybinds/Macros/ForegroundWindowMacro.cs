using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    class ForegroundWindowMacro : IMacro
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public delegate bool EnumWindowsCallback(IntPtr hwnd, string lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumWindowsCallback lpfn, string lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, long nMaxCount);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowEnum showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        public string Name { get { return "Set Foreground Window"; } }
        public string Description { get { return "Sets the foreground window to a certain process' window"; } }
        public string ArgumentsTaken { get { return "(string) process name\r\n[string] window class name"; } }

        public bool Perform(KeybindDevice device, Keys key, Keys correctedKey, KeyState state, KeyState lastState, string guid, params object[] args)
        {
            if (args.Length >= 1 && args.Length <= 3 && args[0] is string processname)
            {
                List<IntPtr> windows = new List<IntPtr>();

                string classname = args.Length > 1 ? args[1] as string : null;

                //IntPtr foreground = args.Length > 2 ? args[2] as IntPtr? ?? GetForegroundWindow() : GetForegroundWindow();

                Process[] processes = Process.GetProcessesByName(processname);
                foreach (Process p in processes)
                {
                    foreach (ProcessThread t in p.Threads)
                    {
                        EnumThreadWindows(t.Id, (hwnd, lParam) =>
                        {
                            if (string.IsNullOrEmpty(classname))
                            {
                                windows.Add(hwnd);

                                return true;
                            }

                            string hwndclass = GetClassNameOfWindow(hwnd);
                            if (hwndclass != null && hwndclass.Length == classname.Length && hwndclass.Equals(classname, StringComparison.InvariantCultureIgnoreCase)) windows.Add(hwnd);

                            return true;
                        }, "");
                    }
                }

                Dictionary<int, IntPtr> Zorder = new Dictionary<int, IntPtr>();
                foreach (IntPtr hwnd in windows)
                {
                    int z = GetWindowZOrder(hwnd);

                    if (z == -1) continue;

                    Zorder[z] = hwnd;
                }

                RestoreWindow(Zorder.OrderBy(kv => kv.Key).ElementAt(Zorder.Count - 1).Value);

                return true;
            }

            return false;
        }

        private void RestoreWindow(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement(hwnd, ref placement);

            // Check if window is minimized
            if (placement.showCmd == ShowWindowEnum.ShowMinimized)
            {
                //the window is hidden so we restore it
                ShowWindow(hwnd, ShowWindowEnum.Restore);
            }

            //set user's focus to the window
            //SetForegroundWindow(hwnd);
            Program.ForegroundWindow = hwnd;
        }

        private int GetWindowZOrder(IntPtr hwnd)
        {
            int z = -1;
            for (IntPtr h = hwnd; h != IntPtr.Zero; h = GetWindow(h, 3)) z++;

            return z;
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