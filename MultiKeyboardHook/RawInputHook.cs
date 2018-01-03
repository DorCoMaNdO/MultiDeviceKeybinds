using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MultiKeyboardHook
{
    internal enum BroadcastDeviceType
    {
        DBT_DEVTYP_OEM = 0,
        DBT_DEVTYP_DEVNODE = 1,
        DBT_DEVTYP_VOLUME = 2,
        DBT_DEVTYP_PORT = 3,
        DBT_DEVTYP_NET = 4,
        DBT_DEVTYP_DEVICEINTERFACE = 5,
        DBT_DEVTYP_HANDLE = 6,
    }

    internal struct BroadcastDeviceInterface
    {
        public Int32 DbccSize;
        public BroadcastDeviceType BroadcastDeviceType;
#pragma warning disable 0649
        public Int32 DbccReserved;
        public Guid DbccClassguid;
        public char DbccName;
#pragma warning restore 0649
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDeviceList
    {
        public IntPtr hDevice;
        public uint dwType;
    }

    public delegate void RawInputKeyPressEvent(object sender, RawInputKeyPressEventArgs e);
    public delegate void DeviceListUpdatedEvent(object sender, Device[] devices);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate bool InstallHook(IntPtr handle);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate bool UninstallHook();

    public class RawInputHook : NativeWindow, IDisposable
    {
        public const int WM_HOOK = Win32.WM_APP + 1;
        private const int PM_REMOVE = 0x0001;

        public const int MaxWaitingTime = 100;
        private const int MaxBuffer = 15, MaxTimeouts = 10, MaxTimeoutsWaitingTime = 2;

        private string NativeHookPath;
        private UnmanagedLibrary NativeHook;
        private InstallHook InstallHook;
        private UninstallHook UninstallHook;

        private int Timeouts = 0;
        //private bool SkipTimedOutRecords = false;

        private RawInputDevice[] RawInputDevices;
        private IntPtr DeviceNotifyHandle;

        private Queue<DecisionRecord> DecisionBuffer = new Queue<DecisionRecord>();

        internal readonly object lockObj = new object();
        internal Dictionary<IntPtr, Device> DevicesByHandle = new Dictionary<IntPtr, Device>();
        internal Dictionary<string, Device> DevicesByID = new Dictionary<string, Device>();
        public Device[] Devices { get { lock (lockObj) return DevicesByHandle.Values.ToArray(); } }
        public bool RawInputInstalled { get; private set; }
        public bool Enabled = true;

        /// <summary>
        /// Raised when for every key down/key up.
        /// 
        /// <para>Processing done in this event should be regarding whether the key should be blocked, further
        /// processing may cause input lag.</para>
        /// </summary>
        public event RawInputKeyPressEvent OnKeyPress;
        /// <summary>
        /// Raised when for every key down/key up when Enabled is set to false.
        /// 
        /// <para>Minimal processing should be done in this event.</para>
        /// </summary>
        public event RawInputKeyPressEvent HookDisabledOnKeyPress;
        /// <summary>
        /// Raised after a key down/key up was decided to be handled.
        /// 
        /// <para>The passed GUID string in RawInputKeyPressEventArgs will match the string from OnKeyPress
        /// which can be used to identify whether a handled key was sourced by the previously raised OnKeyPress.</para>
        /// 
        /// <para>Any sort of processing can be done with this event as it runs in a new thread and should not
        /// interrupt the main thread from blocking the input as requested.</para>
        /// </summary>
        public event RawInputKeyPressEvent HandledKeyPress;
        /// <summary>
        /// Raised after a device is connected/disconnected.
        /// </summary>
        public event DeviceListUpdatedEvent DeviceListUpdated;

        public RawInputHook(string path)
        {
            foreach (string file in Directory.GetFiles(path, "NativeHook_*.dll")) if (!IsFileLocked(file)) File.Delete(file);

            NativeHookPath = Path.Combine(path, "NativeHook.dll");

            if (File.Exists(NativeHookPath) && IsFileLocked(NativeHookPath)) while (File.Exists(NativeHookPath)) NativeHookPath = Path.Combine(path, $"NativeHook_{Guid.NewGuid().ToString()}.dll");

            if (!File.Exists(NativeHookPath)) File.WriteAllBytes(NativeHookPath, GetBytes("NativeHook.dll"));

            NativeHook = new UnmanagedLibrary(NativeHookPath);

            InstallHook = NativeHook.GetUnmanagedFunction<InstallHook>("InstallHook");
            UninstallHook = NativeHook.GetUnmanagedFunction<UninstallHook>("UninstallHook");
        }

        public RawInputHook() : this(AppDomain.CurrentDomain.BaseDirectory)
        {
        }

        public void Dispose()
        {
            Uninstall();

            NativeHook?.Dispose();

            if (!IsFileLocked(NativeHookPath)) File.Delete(NativeHookPath);
        }

        private bool IsFileLocked(string FileLocation, FileShare fs = FileShare.None)
        {
            FileInfo file = new FileInfo(FileLocation);
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, fs);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null) stream.Close();
            }

            //file is not locked
            return false;
        }

        private byte[] GetBytes(string resource, bool fullname = false)
        {
            if (!fullname) resource = Assembly.GetExecutingAssembly().GetManifestResourceNames().FirstOrDefault(name => name.Length >= resource.Length && name.EndsWith(resource, StringComparison.InvariantCultureIgnoreCase)) ?? resource;

            byte[] buffer = null;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
            {
                if (stream == null) throw new Exception($"{resource} is not found in Embedded Resources.");

                buffer = new byte[stream.Length];

                stream.Read(buffer, 0, (int)stream.Length);
            }

            return buffer;
        }

        public bool Install(IntPtr hwnd, bool rawinput = true)
        {
            bool installed = InstallHook(hwnd);

            if (installed)
            {
                if (rawinput && !RawInputInstalled)
                {
                    RawInputInstalled = true;

                    AssignHandle(hwnd);

                    RawInputDevices = new[]
                    {
                        new RawInputDevice()
                        {
                            UsagePage = HIDUsagePage.GENERIC,
                            Usage = HIDUsage.Keyboard,
                            Flags = RawInputDeviceFlags.INPUTSINK | RawInputDeviceFlags.DEVNOTIFY,
                            Target = hwnd
                        }
                    };

                    if (!Win32.RegisterRawInputDevices(RawInputDevices, (uint)RawInputDevices.Length, (uint)Marshal.SizeOf(RawInputDevices[0]))) throw new ApplicationException("Failed to register raw input device(s).");

                    EnumerateDevices();

                    BroadcastDeviceInterface bdi = new BroadcastDeviceInterface();
                    bdi.DbccSize = Marshal.SizeOf(bdi);
                    bdi.BroadcastDeviceType = BroadcastDeviceType.DBT_DEVTYP_DEVICEINTERFACE;
                    bdi.DbccClassguid = Win32.GUID_DEVINTERFACE_HID;

                    IntPtr mem = IntPtr.Zero, notifyHandle = IntPtr.Zero;
                    try
                    {
                        mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BroadcastDeviceInterface)));
                        Marshal.StructureToPtr(bdi, mem, false);

                        notifyHandle = Win32.RegisterDeviceNotification(hwnd, mem, DeviceNotification.DEVICE_NOTIFY_WINDOW_HANDLE);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Registration for device notifications Failed. Error: {Marshal.GetLastWin32Error()}");
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(mem);
                    }

                    if (notifyHandle == IntPtr.Zero) Console.WriteLine($"Registration for device notifications Failed. Error: {Marshal.GetLastWin32Error()}");

                    DeviceNotifyHandle = notifyHandle;
                }
            }

            return installed;
        }

        public bool Uninstall()
        {
            bool uninstalled = UninstallHook();

            if (uninstalled)
            {
                if (RawInputInstalled)
                {
                    RawInputInstalled = false;

                    for (int i = 0; i < RawInputDevices.Length; i++)
                    {
                        RawInputDevices[i].Flags = RawInputDeviceFlags.REMOVE;
                        RawInputDevices[i].Target = IntPtr.Zero;
                    }

                    if (!Win32.RegisterRawInputDevices(RawInputDevices, (uint)RawInputDevices.Length, (uint)Marshal.SizeOf(RawInputDevices[0]))) throw new ApplicationException("Failed to unregister raw input device(s).");

                    Win32.UnregisterDeviceNotification(DeviceNotifyHandle);

                    ReleaseHandle();
                }
            }

            return uninstalled;
        }

        internal void Rehook()
        {
            UninstallHook();

            InstallHook(Handle);
        }

        private void EnumerateDevices()
        {
            lock (lockObj)
            {
                DevicesByHandle.Clear();
                DevicesByID.Clear();

                Device globalDevice = new Device("Global Keyboard", Win32.GetDeviceType(DeviceType.Keyboard), "Fake Keyboard, some keys are sent to RawInput with a handle of zero.");

                DevicesByHandle.Add(globalDevice.Handle, globalDevice);
                DevicesByID.Add(globalDevice.ID, globalDevice);

                uint deviceCount = 0;
                int dwSize = (Marshal.SizeOf(typeof(RawInputDeviceList)));

                if (Win32.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
                {
                    IntPtr pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
                    Win32.GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

                    for (var i = 0; i < deviceCount; i++)
                    {
                        RawInputDeviceList rid = (RawInputDeviceList)Marshal.PtrToStructure(new IntPtr((pRawInputDeviceList.ToInt64() + (dwSize * i))), typeof(RawInputDeviceList));

                        uint pcbSize = 0;

                        Win32.GetRawInputDeviceInfo(rid.hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

                        if (pcbSize <= 0) continue;

                        IntPtr pData = Marshal.AllocHGlobal((int)pcbSize);

                        Win32.GetRawInputDeviceInfo(rid.hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, pData, ref pcbSize);

                        string deviceID = Marshal.PtrToStringAnsi(pData);

                        if (rid.dwType == (uint)DeviceType.Keyboard || rid.dwType == (uint)DeviceType.HID)
                        {
                            string deviceName = Win32.GetDeviceDescription(deviceID);

                            Device device = new Device(deviceID, Win32.GetDeviceType(rid.dwType), rid.hDevice, deviceName);

                            if (!DevicesByHandle.ContainsKey(rid.hDevice)) DevicesByHandle.Add(rid.hDevice, device);

                            if (!DevicesByID.ContainsKey(device.ID)) DevicesByID.Add(device.ID, device);
                        }

                        Marshal.FreeHGlobal(pData);
                    }

                    Marshal.FreeHGlobal(pRawInputDeviceList);

                    Console.WriteLine($"Found {DevicesByHandle.Count - 1} device(s)");

                    DeviceListUpdated?.Invoke(this, Devices);

                    return;
                }
            }

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        private void ProcessRawInput(IntPtr hDevice)
        {
            if (DevicesByHandle.Count == 0) return;

            int dwSize = 0;
            Win32.GetRawInputData(hDevice, DataCommand.RID_INPUT, IntPtr.Zero, ref dwSize, Marshal.SizeOf(typeof(RawInputHeader)));

            if (dwSize != Win32.GetRawInputData(hDevice, DataCommand.RID_INPUT, out InputData rawBuffer, ref dwSize, Marshal.SizeOf(typeof(RawInputHeader))))
            {
                Console.WriteLine("Error getting the rawinput buffer");

                return;
            }

            int virtualKey = rawBuffer.data.keyboard.VKey;
            int makeCode = rawBuffer.data.keyboard.Makecode;
            int flags = rawBuffer.data.keyboard.Flags;

            if (virtualKey == Win32.KEYBOARD_OVERRUN_MAKE_CODE) return;

            Device device;

            lock (lockObj)
            {
                if (!DevicesByHandle.ContainsKey(rawBuffer.header.hDevice))
                {
                    Console.WriteLine($"Handle: {rawBuffer.header.hDevice} was not in the device list.");

                    return;
                }

                device = DevicesByHandle[rawBuffer.header.hDevice];
            }

            bool isE0BitSet = (flags & Win32.RI_KEY_E0) != 0;
            bool isBreakBitSet = (flags & Win32.RI_KEY_BREAK) != 0;

            RawInputKeyPressEventArgs args = new RawInputKeyPressEventArgs(this, device, virtualKey, VirtualKeyCorrection(rawBuffer.header.hDevice, virtualKey, isE0BitSet, makeCode), isBreakBitSet ? KeyState.Break : KeyState.Make);

            args.LastKeyPressState = device.Pressed.Contains(args.CorrectedKey) ? KeyState.Make : KeyState.Break;

            List<Keys> pressed = device.Pressed.ToList();

            if (args.KeyPressState == KeyState.Make && !pressed.Contains(args.CorrectedKey))
            {
                pressed.Add(args.CorrectedKey);
            }
            else if (args.KeyPressState == KeyState.Break && pressed.Contains(args.CorrectedKey))
            {
                pressed.RemoveAll(k => k == args.CorrectedKey);
            }

            device.LastPressed = device.Pressed;
            device.Pressed = pressed.ToArray();

            ProcessKeyPress(args);
        }

        private static int VirtualKeyCorrection(IntPtr hDevice, int virtualKey, bool isE0BitSet, int makeCode)
        {
            int correctedVKey = virtualKey;

            if (hDevice != IntPtr.Zero)
            {
                switch (virtualKey)
                {
                    case Win32.VK_CONTROL:
                        correctedVKey = isE0BitSet ? Win32.VK_RCONTROL : Win32.VK_LCONTROL;

                        break;
                    case Win32.VK_MENU:
                        correctedVKey = isE0BitSet ? Win32.VK_RMENU : Win32.VK_LMENU;

                        break;
                    case Win32.VK_SHIFT:
                        correctedVKey = makeCode == Win32.SC_SHIFT_R ? Win32.VK_RSHIFT : Win32.VK_LSHIFT;

                        break;
                    default:
                        correctedVKey = virtualKey;

                        break;
                }
            }

            return correctedVKey;
        }

        private void EnqueueDecision(int keycode, KeyState state, bool decision, IntPtr deviceHandle)
        {
            lock (DecisionBuffer)
            {
                DecisionBuffer.Enqueue(new DecisionRecord(keycode, state, decision, deviceHandle));

                if (DecisionBuffer.Where(d => d.DeviceHandle != IntPtr.Zero).Count() > MaxBuffer)
                {
                    Console.WriteLine($"OVER {MaxBuffer} MESSAGES WEREN'T PROCESSED, REHOOKING");

                    Rehook();

                    DecisionBuffer.Clear();
                }
            }
        }

        private DecisionRecord GetRecord(ushort virtualKeyCode, KeyState state)
        {
            lock (DecisionBuffer)
            {
                int currentTime = Environment.TickCount;

                for (int i = 0; i < DecisionBuffer.Count; i++)
                {
                    DecisionRecord record = DecisionBuffer.ElementAt(i);

                    //if (SkipTimedOutRecords)
                    //{
#warning temporarily(?) disabled, breaks with macros sending keys
                    //if (record.DeviceHandle != IntPtr.Zero && GetTickCountDifference(record.TimeCreated, currentTime) > MaxWaitingTime) continue;
                    //}

                    if (record.KeyCode == virtualKeyCode && record.State == state)
                    {
                        while (i >= 0)
                        {
                            DecisionBuffer.Dequeue();

                            i--;
                        }

                        return record;
                    }
                }

                DecisionRecord timedoutrecord;
                while (DecisionBuffer.Count > 0 && (timedoutrecord = DecisionBuffer.ElementAt(0)).DeviceHandle != IntPtr.Zero && GetTickCountDifference(timedoutrecord.TimeCreated, currentTime) > MaxWaitingTime) DecisionBuffer.Dequeue();
            }

            return null;
        }

        private static int GetTickCountDifference(int first, int second)
        {
            return (int)(second < first ? uint.MaxValue - first + second : second - first);
        }

        public bool ProcessHookMessage(ref Message m)
        {
            try
            {
                ushort virtualKeyCode = (ushort)m.WParam;
                Keys key = (Keys)virtualKeyCode;

                if (key == Keys.Packet) return false;

                KeyState state = (KeyState)(((long)m.LParam & 0x80000000) > 0 ? 0 : 1);

                DecisionRecord record = GetRecord(virtualKeyCode, state);

                if (record == null && Timeouts < MaxTimeouts)
                {
                    bool reported = false;

                    int currentTime = Environment.TickCount, startTime = Environment.TickCount;

                    while (record == null)
                    {
                        bool peeked = false;
                        NativeMessage rawMessage;
                        while (!(peeked = Win32.PeekMessage(out rawMessage, Handle, Win32.WM_INPUT, Win32.WM_INPUT, PM_REMOVE)))
                        {
                            currentTime = Environment.TickCount;

                            if (!reported && GetTickCountDifference(startTime, currentTime) > 0)
                            {
                                Console.WriteLine($"UNRECORDED INPUT {key} {state}");

                                reported = true;
                            }

                            if (GetTickCountDifference(startTime, currentTime) > MaxWaitingTime)
                            {
                                Console.WriteLine($"IDLE FOR RAW INPUT TIMED OUT {key} {state}");

                                Timeouts++;

                                break;
                            }
                        }

                        if (!peeked) break;

                        if (!reported && GetTickCountDifference(startTime, currentTime) > 0)
                        {
                            Console.WriteLine($"UNRECORDED INPUT {key} {state}");

                            reported = true;
                        }

                        if (rawMessage.msg == Win32.WM_INPUT)
                        {
                            ProcessRawInput(rawMessage.lParam);

                            record = GetRecord(virtualKeyCode, state);
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (reported && record != null) Console.WriteLine($"IDLE FOR RAW INPUT SUCCESSFUL, WAITED FOR {(currentTime < startTime ? uint.MaxValue - startTime + currentTime : currentTime - startTime)}ms - {key} {state}");
                }

                if (record != null) Timeouts = 0;

                Console.WriteLine($"Hook: {key} {state}");

                bool block = (record?.Decision).GetValueOrDefault();

                m.Result = (IntPtr)(block ? 1 : 0);

                if (block) Console.WriteLine($"Key press: {key} {state} is being blocked!");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == RawInputHook.WM_HOOK)
            {
                if (ProcessHookMessage(ref m)) return;
            }
            else if (m.Msg == Win32.WM_INPUT)
            {
                ProcessRawInput(m.LParam);
            }
            else if (m.Msg == Win32.WM_USB_DEVICECHANGE && ((int)m.WParam == Win32.DBT_DEVICEARRIVAL || (int)m.WParam == Win32.DBT_DEVICEREMOVECOMPLETE))
            {
                Console.WriteLine($"USB Device {((int)m.WParam == Win32.DBT_DEVICEARRIVAL ? "Arrival" : "Removal")}");

                EnumerateDevices();
            }

            base.WndProc(ref m);
        }

        private void ProcessKeyPress(RawInputKeyPressEventArgs args)
        {
            Console.WriteLine($"{args.Device.Name} {args.Device.ID} {args.CorrectedKey} {args.KeyPressState}");

            if (args.Device.Handle == IntPtr.Zero)
            {
                EnqueueDecision(args.VKey, args.KeyPressState, false, IntPtr.Zero);

                return;
            }

            Delegate[] handlers;

            if (!Enabled)
            {
                handlers = HookDisabledOnKeyPress?.GetInvocationList();
                if (handlers != null)
                {
                    foreach (RawInputKeyPressEvent handler in handlers)
                    {
                        handler.Invoke(this, args);

                        if (args.Handled) break;
                    }
                }

                EnqueueDecision(args.VKey, args.KeyPressState, args.Handled, args.Device.Handle);

                return;
            }

            handlers = OnKeyPress?.GetInvocationList();
            if (handlers != null)
            {
                foreach (RawInputKeyPressEvent handler in handlers)
                {
                    handler.Invoke(this, args);

                    if (args.Handled) break;
                }
            }

            EnqueueDecision(args.VKey, args.KeyPressState, args.Handled, args.Device.Handle);

            if (args.Handled && (handlers = HandledKeyPress?.GetInvocationList()) != null)
            {
                args.Handled = false;

                new Thread(() =>
                {
                    foreach (RawInputKeyPressEvent handler in handlers)
                    {
                        handler.Invoke(this, args);

                        if (args.Handled) break;
                    }
                })
                {
                    IsBackground = true,
                    Name = "HandledKeyPress thread"
                }.Start();
            }
        }
    }
}