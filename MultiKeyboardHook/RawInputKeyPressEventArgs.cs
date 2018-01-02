using System;
using System.Windows.Forms;

namespace MultiKeyboardHook
{
    /// <summary>
    /// The state of the key.
    /// </summary>
    public enum KeyState
    {
        /// <summary>
        /// The key is not pressed.
        /// </summary>
        Break = 0,
        /// <summary>
        /// The key is pressed.
        /// </summary>
        Make = 1
    }

    public class RawInputKeyPressEventArgs : EventArgs
    {
        /// <summary>
        /// The device that the key was (un)pressed on.
        /// </summary>
        public Device Device { get; private set; }
        /// <summary>
        /// The virtual key that was pressed.
        /// </summary>
        public int VKey { get; private set; }
        /// <summary>
        /// The key that was pressed.
        /// </summary>
        public Keys Key { get { return (Keys)VKey; } }
        /// <summary>
        /// The virtual key that was pressed, corrected for modifiers.
        /// </summary>
        public int CorrectedVKey { get; private set; }
        /// <summary>
        /// The key that was pressed, corrected for modifiers.
        /// </summary>
        public Keys CorrectedKey { get { return (Keys)CorrectedVKey; } }
        /// <summary>
        /// The state of the key.
        /// </summary>
        public KeyState KeyPressState { get; private set; }
        /// <summary>
        /// The last state of the key.
        /// </summary>
        public KeyState LastKeyPressState { get; internal set; }

        /// <summary>
        /// A unique identifier for this instance of RawInputKeyPressEventArgs, to be used to match the events OnKeyPress and HandledKeyPress when a key is handled.
        /// </summary>
        public string GUID { get; private set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Blocks this key press.
        /// </summary>
        public bool Handled { get; set; } = false;

        internal RawInputKeyPressEventArgs(RawInputHook hook, Device device, int vKey, int correctedVKey, KeyState keyPressState, KeyState lastKeyPressState = KeyState.Break)
        {
            lock (hook.lockObj)
            {
                Device = hook.DevicesByID.ContainsKey(device.ID) ? hook.DevicesByID[device.ID] : new Device(device.ID, device.Type, device.Handle, device.Name);

                Device.Pressed = device.Pressed;
            }

            VKey = vKey;

            CorrectedVKey = correctedVKey;

            KeyPressState = keyPressState;
            LastKeyPressState = lastKeyPressState;
        }
    }
}