using System;

namespace MultiKeyboardHook
{
    class DecisionRecord
    {
        public int KeyCode { get; private set; }
        public KeyState State { get; private set; }
        public bool Decision { get; private set; }
        public int TimeCreated { get; private set; }
        public IntPtr DeviceHandle { get; private set; } = IntPtr.Zero;

        public DecisionRecord(int keycode, KeyState state, bool decision, IntPtr deviceHandle)
        {
            KeyCode = keycode;
            State = state;
            Decision = decision;
            DeviceHandle = deviceHandle;

            TimeCreated = Environment.TickCount;
        }
    }
}