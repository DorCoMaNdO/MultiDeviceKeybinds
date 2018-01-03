using System.Diagnostics;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    class ProcessStartMacro : IMacro
    {
        public string Name { get { return "Start Process"; } }
        public string Description { get { return "Launches a new process."; } }
        public string ArgumentsTaken { get { return "(string) file path\r\n[string] arguments"; } }

        public bool Perform(KeybindDevice device, Keys key, Keys correctedKey, KeyState state, KeyState lastState, string guid, params object[] args)
        {
            if (args.Length < 1 || !(args[0] is string path)) return false;

            try
            {
                Process.Start(path, args.Length > 1 && args[1] is string arguments ? arguments : "");

                return true;
            }
            catch
            {
            }

            return false;
        }
    }
}