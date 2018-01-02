using System;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    static class ControlExtensions
    {
        public static void InvokeIfRequired(this Control ctrl, Action action)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(action);

                return;
            }

            action();
        }

        public static IAsyncResult InvokeIfRequiredAsync(this Control ctrl, Action action)
        {
            if (ctrl.InvokeRequired) return ctrl.BeginInvoke(action);

            action();

            return null;
        }
    }
}