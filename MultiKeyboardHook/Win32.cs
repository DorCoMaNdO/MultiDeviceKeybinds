using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MultiKeyboardHook
{
    internal enum DataCommand : uint
    {
        RID_HEADER = 0x10000005, // Get the header information from the RAWINPUT structure.
        RID_INPUT = 0x10000003 // Get the raw data from the RAWINPUT structure.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputHeader
    {
        public uint dwType; // Type of raw input (RIM_TYPEHID 2, RIM_TYPEKEYBOARD 1, RIM_TYPEMOUSE 0)
        public uint dwSize; // Size in bytes of the entire input packet of data. This includes RAWINPUT plus possible extra input reports in the RAWHID variable length array. 
        public IntPtr hDevice; // A handle to the device generating the raw input data. 
        public IntPtr wParam; // RIM_INPUT 0 if input occurred while application was in the foreground else RIM_INPUTSINK 1 if it was not.
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct RawMouse
    {
        [FieldOffset(0)]
        public ushort usFlags;
        [FieldOffset(4)]
        public uint ulButtons;
        [FieldOffset(4)]
        public ushort usButtonFlags;
        [FieldOffset(6)]
        public ushort usButtonData;
        [FieldOffset(8)]
        public uint ulRawButtons;
        [FieldOffset(12)]
        public int lLastX;
        [FieldOffset(16)]
        public int lLastY;
        [FieldOffset(20)]
        public uint ulExtraInformation;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawKeyboard
    {
        public ushort Makecode; // Scan code from the key depression
        public ushort Flags; // One or more of RI_KEY_MAKE, RI_KEY_BREAK, RI_KEY_E0, RI_KEY_E1
        private readonly ushort Reserved; // Always 0
        public ushort VKey; // Virtual Key Code
        public uint Message; // Corresponding Windows message - WM_KEYDOWN, WM_SYASKEYDOWN, etc
        public uint ExtraInformation; // The device-specific addition information for the event (seems to always be zero for keyboards)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawHID
    {
        public uint dwSizeHid;
        public uint dwCount;
        public byte bRawData;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct RawData
    {
        [FieldOffset(0)]
        internal RawMouse mouse;
        [FieldOffset(0)]
        internal RawKeyboard keyboard;
        [FieldOffset(0)]
        internal RawHID hid;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct InputData
    {
        public RawInputHeader header; // 64 bit header size: 24  32 bit the header size: 16
        public RawData data; // Creating the rest in a struct allows the header size to align correctly for 32/64 bit
    }

    internal enum RawInputDeviceInfo : uint
    {
        RIDI_DEVICENAME = 0x20000007,
        RIDI_DEVICEINFO = 0x2000000b,
        PREPARSEDDATA = 0x20000005
    }

    internal enum HIDUsagePage : ushort
    {
        UNDEFINED = 0x00, // Unknown usage page
        GENERIC = 0x01, // Generic desktop controls
        SIMULATION = 0x02, // Simulation controls
        VR = 0x03, // Virtual reality controls
        SPORT = 0x04, // Sports controls
        GAME = 0x05, // Games controls
        KEYBOARD = 0x07, // Keyboard controls
    }

    internal enum HIDUsage : ushort
    {
        Undefined = 0x00, // Unknown usage
        Pointer = 0x01, // Pointer
        Mouse = 0x02, // Mouse
        Joystick = 0x04, // Joystick
        Gamepad = 0x05, // Game Pad
        Keyboard = 0x06, // Keyboard
        Keypad = 0x07, // Keypad
        SystemControl = 0x80, // Muilt-axis Controller
        Tablet = 0x80, // Tablet PC controls
        Consumer = 0x0C, // Consumer
    }

    [Flags]
    internal enum RawInputDeviceFlags
    {
        NONE = 0, // No flags
        REMOVE = 0x00000001, // Removes the top level collection from the inclusion list. This tells the operating system to stop reading from a device which matches the top level collection. 
        EXCLUDE = 0x00000010, // Specifies the top level collections to exclude when reading a complete usage page. This flag only affects a TLC whose usage page is already specified with PageOnly.
        PAGEONLY = 0x00000020, // Specifies all devices whose top level collection is from the specified UsagePage. Note that Usage must be zero. To exclude a particular top level collection, use Exclude.
        NOLEGACY = 0x00000030, // Prevents any devices specified by UsagePage or Usage from generating legacy messages. This is only for the mouse and keyboard.
        INPUTSINK = 0x00000100, // Enables the caller to receive the input even when the caller is not in the foreground. Note that WindowHandle must be specified.
        CAPTUREMOUSE = 0x00000200, // Mouse button click does not activate the other window.
        NOHOTKEYS = 0x00000200, // Application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. NoHotKeys can be specified even if NoLegacy is not specified and WindowHandle is NULL.
        APPKEYS = 0x00000400, // Application keys are handled. NOLEGACY must be specified. Keyboard only.

        // Enables the caller to receive input in the background only if the foreground application does not process it. 
        // In other words, if the foreground application is not registered for raw input, then the background application that is registered will receive the input.
        EXINPUTSINK = 0x00001000,
        DEVNOTIFY = 0x00002000
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDevice
    {
        internal HIDUsagePage UsagePage;
        internal HIDUsage Usage;
        internal RawInputDeviceFlags Flags;
        internal IntPtr Target;
    }

    internal enum DeviceNotification
    {
        DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000, // The hRecipient parameter is a window handle
        DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001, // The hRecipient parameter is a service status handle
        DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004 // Notifies the recipient of device interface events for all device interface classes. (The dbcc_classguid member is ignored.), this value can be used only if the dbch_devicetype member is DBT_DEVTYP_DEVICEINTERFACE.
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeMessage
    {
        public IntPtr handle;
        public uint msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point p;
    }

    internal enum DeviceType : uint
    {
        Mouse = 0,
        Keyboard = 1,
        HID = 2
    }

    internal static class Win32
    {
        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int GetRawInputData(IntPtr hRawInput, DataCommand command, [Out] out InputData buffer, [In, Out] ref int size, int cbSizeHeader);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int GetRawInputData(IntPtr hRawInput, DataCommand command, [Out] IntPtr pData, [In, Out] ref int size, int sizeHeader);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfo command, IntPtr pData, ref uint size);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint numberDevices, uint size);

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern bool RegisterRawInputDevices(RawInputDevice[] pRawInputDevice, uint numberDevices, uint size);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, DeviceNotification flags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool UnregisterDeviceNotification(IntPtr handle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        internal static readonly Guid GUID_DEVINTERFACE_HID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");

        internal const int DBT_DEVICEARRIVAL = 0x8000;
        internal const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        internal const int KEYBOARD_OVERRUN_MAKE_CODE = 0xFF;

        internal const int WM_KEYDOWN = 0x0100;
        internal const int WM_KEYUP = 0x0101;
        internal const int WM_SYSKEYDOWN = 0x0104;
        internal const int WM_INPUT = 0x00FF;

        internal const int WM_USB_DEVICECHANGE = 0x0219;

        internal const int WM_APP = 0x8000;

        //internal const int RI_KEY_MAKE = 0x00;
        internal const int RI_KEY_BREAK = 0x01;
        internal const int RI_KEY_E0 = 0x02;
        //internal const int RI_KEY_E1 = 0x04;

        internal const int VK_SHIFT = 0x10;
        internal const int VK_CONTROL = 0x11;
        internal const int VK_MENU = 0x12;

        internal const int VK_LSHIFT = 0xA0;
        internal const int VK_RSHIFT = 0xA1;
        internal const int VK_LCONTROL = 0xA2;
        internal const int VK_RCONTROL = 0xA3;
        internal const int VK_LMENU = 0xA4;
        internal const int VK_RMENU = 0xA5;

        internal const int VK_ZOOM = 0xFB;

        internal const int SC_SHIFT_R = 0x36;
        //internal const int SC_SHIFT_L = 0x2a;

        //internal const int RIM_INPUT = 0x00;

        internal static string GetDeviceType(DeviceType device)
        {
            string deviceType;

            switch (device)
            {
                case DeviceType.Mouse: deviceType = "MOUSE"; break;
                case DeviceType.Keyboard: deviceType = "KEYBOARD"; break;
                case DeviceType.HID: deviceType = "HID"; break;
                default: deviceType = "UNKNOWN"; break;
            }

            return deviceType;
        }

        internal static string GetDeviceType(uint device)
        {
            return GetDeviceType((DeviceType)device);
        }

        private static RegistryKey GetDeviceKey(string device)
        {
            string[] split = device.Substring(4).Split('#');

            string classCode = split[0];
            string subClassCode = split[1];
            string protocolCode = split[2];

            return Registry.LocalMachine.OpenSubKey($@"System\CurrentControlSet\Enum\{classCode}\{subClassCode}\{protocolCode}");
        }

        internal static string GetDeviceDescription(string device)
        {
            string deviceDesc;

            try
            {
                RegistryKey deviceKey = GetDeviceKey(device);

                deviceDesc = deviceKey.GetValue("DeviceDesc").ToString();
                deviceDesc = deviceDesc.Substring(deviceDesc.IndexOf(';') + 1);
            }
            catch
            {
                deviceDesc = "Device is malformed unable to look up in the registry";
            }

            return deviceDesc;
        }
    }
}