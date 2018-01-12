using SHDocVw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    class OpenFolderMacro : IMacro
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

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

        public string Name { get { return "Open Folder"; } }
        public string Description { get { return "Opens a folder or brings it to front if it's already open."; } }
        public string ArgumentsTaken { get { return "(string) path"; } }

        public bool Perform(KeybindDevice device, Keys key, KeyState state, KeyState lastState, string guid, params object[] args)
        {
            if (args.Length < 1 || !(args[0] is string path) || string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return false;

            path = path.Replace("/", "\\").TrimEnd('\\');

            IntPtr hwnd = IntPtr.Zero;
            
            ShellWindows shellWindows = new ShellWindows();

            foreach (InternetExplorer ie in shellWindows)
            {
                if (!Path.GetFileNameWithoutExtension(ie.FullName).ToLower().Equals("explorer")) continue;

                try
                {
                    string folderpath = new Uri(ie.LocationURL).LocalPath;

                    if (folderpath == path)
                    {
                        hwnd = (IntPtr)ie.HWND;

                        break;
                    }
                }
                catch
                {
                }
            }

            if (hwnd == IntPtr.Zero)
            {
                Process.Start(path);
            }
            else
            {
                RestoreWindow(hwnd);
            }

            return true;
        }

        private void RestoreWindow(IntPtr hwnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement(hwnd, ref placement);

            if (placement.showCmd == ShowWindowEnum.ShowMinimized) ShowWindow(hwnd, ShowWindowEnum.Restore);

            Program.ForegroundWindow = hwnd;
        }
    }
}