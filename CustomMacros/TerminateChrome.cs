using MultiDeviceKeybinds;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace CustomMacros
{
    public class TerminateChrome : IMacro
    {
        public string Name { get { return "Terminate Idle Chrome Processes"; } }
        public string Description { get { return "Terminates idle Chrome processes using under 30MB of RAM"; } }
        public string ArgumentsTaken { get { return "None"; } }

        const int MB = 1024 * 1024;
        const int DefaultMaxSize = 30;

        public bool Perform(KeybindDevice device, Keys key, Keys correctedKey, KeyState state, KeyState lastState, string guid, params object[] args)
        {
            long maxSize = DefaultMaxSize;

            if (args.Length > 0 && args[0] is long size && size > 0) maxSize = size;

            long treshhold = maxSize * MB;

            List<int> PIDsToKill = new List<int>();

            string[] instanceNames = new PerformanceCounterCategory("Process").GetInstanceNames();
            foreach (string name in instanceNames)
            {
                if (name.Length < 6 || !name.StartsWith("chrome")) continue;

                using (PerformanceCounter procPIDPerfCounter = new PerformanceCounter("Process", "ID Process", name, true))
                {
                    int processID = (int)procPIDPerfCounter.RawValue;

                    using (Process process = Process.GetProcessById(processID))
                    {
                        if (process.MainWindowHandle != IntPtr.Zero) continue;

                        using (PerformanceCounter procPWSPerfCounter = new PerformanceCounter("Process", "Working Set - Private", name, true))
                        {
                            long privateWorkingSet = procPWSPerfCounter.RawValue;

                            if (treshhold > privateWorkingSet) PIDsToKill.Add(processID);
                        }
                    }
                }
            }

            foreach (int pid in PIDsToKill) using (Process process = Process.GetProcessById(pid)) process.Kill();

            return true;
        }
    }
}