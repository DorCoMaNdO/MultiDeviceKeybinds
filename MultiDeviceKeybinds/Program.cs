using MultiKeyboardHook;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string name);

        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private static WinEventDelegate ForegroundWindowDelegate = null;
        private static IntPtr ForegroundWindowHook = IntPtr.Zero;

        private static IntPtr foregroundWindow = IntPtr.Zero;
        public static IntPtr ForegroundWindow { get { return foregroundWindow; } set { SetForegroundWindow(value); } }

        internal static MainForm MainForm;
        internal static UnelevatedHookForm UnelevatedHookForm;

        internal static string Location { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        public static bool RunningAsAdmin { get; private set; } = false;
        public static bool Closing { get; private set; } = false;

        internal static StreamReader PipeReader;
        internal static StreamWriter PipeWriter;

        internal static RawInputHook Hook;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            EnableMenuItem(GetSystemMenu(GetConsoleWindow(), false), 0xF060, 0x00000002);

            Console.TreatControlCAsInput = true;

            Application.ApplicationExit += (sender, e) => { Closing = true; };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RunningAsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

            if (!RunningAsAdmin)
            {
                Process parent = null;

                int hookonlyindex = Array.FindIndex(args, arg => arg.Equals("-hookonly"));

                if (hookonlyindex > -1 && hookonlyindex + 3 <= args.Length && int.TryParse(args[hookonlyindex + 1], out int pid) && long.TryParse(args[hookonlyindex + 2], out long hwnd))
                {
                    try
                    {
                        parent = Process.GetProcessById(pid);
                    }
                    catch
                    {
                        Console.WriteLine("Parent process not found.");
                        Console.ReadKey();
                    }

                    if (parent != null)
                    {
                        Console.Title = "Multi Device Keybinds (Subprocess)";

                        HideConsole();

                        Hook = new RawInputHook();

                        UnelevatedHookForm form = new UnelevatedHookForm(parent);

                        Application.Run(form);

                    }

                    return;
                }
            }

            Console.Title = $"Multi Device Keybinds{(RunningAsAdmin ? " (Admin)" : "")}";

            int width = Console.WindowWidth * 2;
            if (width > Console.LargestWindowWidth) width = Console.LargestWindowWidth;

            Process current = Process.GetCurrentProcess();
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    if (p.Id == current.Id || p.MainModule.FileName.Length != current.MainModule.FileName.Length) continue;

                    if (p.MainModule.FileName.Length == current.MainModule.FileName.Length && p.MainModule.FileName.Equals(current.MainModule.FileName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine("Multi Device Keybinds is already running!\r\n\r\nPress any key to exit...");

                        Console.ReadKey(true);

                        return;
                    }
                }
                catch
                {
                }
            }

            Hook = new RawInputHook();

            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            current.PriorityClass = ProcessPriorityClass.High;
            //current.PriorityClass = ProcessPriorityClass.RealTime; // requires admin, runs as high otherwise

            Console.WindowWidth = width;

            foreach (string file in Directory.GetFiles(Location, "*.dll", SearchOption.TopDirectoryOnly).Concat(Directory.GetFiles(Location, "*.exe", SearchOption.TopDirectoryOnly))) DeleteFile(file + ":Zone.Identifier");

            Application.ApplicationExit += Application_ApplicationExit;

            ForegroundWindowDelegate = new WinEventDelegate(WinEventProc);
            ForegroundWindowHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, ForegroundWindowDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);

            Application.Run(MainForm = new MainForm());
        }

        public static void HideConsole()
        {
            ShowWindow(GetConsoleWindow(), 0);
        }

        public static void ShowConsole()
        {
            ShowWindow(GetConsoleWindow(), 1);
        }

        private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (ForegroundWindowHook != hWinEventHook || eventType != EVENT_SYSTEM_FOREGROUND || Hook == null) return;

            foregroundWindow = hwnd;

            Hook.Enabled = (MainForm == null || foregroundWindow != MainForm.Handle) && (MainForm.KeybindForm == null || foregroundWindow != MainForm.KeybindForm.Handle && !MainForm.KeybindForm.Visible);
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Hook?.Dispose();

            if (ForegroundWindowHook != IntPtr.Zero) UnhookWinEvent(ForegroundWindowHook);

            if (PipeReader != null)
            {
                try
                {
                    PipeReader.BaseStream.Dispose();

                    PipeReader.Dispose();
                }
                catch
                {
                }

                PipeReader = null;
            }

            if (PipeWriter != null)
            {
                try
                {
                    PipeWriter.BaseStream.Dispose();

                    PipeWriter.Dispose();
                }
                catch
                {
                }

                PipeWriter = null;
            }
        }
    }
}