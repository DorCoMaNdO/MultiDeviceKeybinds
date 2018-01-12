using Interceptor;
using Microsoft.Win32;
using MultiKeyboardHook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

        /// <summary>
        /// The set of valid MapTypes used in MapVirtualKey
        /// </summary>
        public enum MapVirtualKeyMapTypes : uint
        {
            /// <summary>
            /// uCode is a virtual-key code and is translated into a scan code.
            /// If it is a virtual-key code that does not distinguish between left- and
            /// right-hand keys, the left-hand scan code is returned.
            /// If there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_VSC = 0x00,

            /// <summary>
            /// uCode is a scan code and is translated into a virtual-key code that
            /// does not distinguish between left- and right-hand keys. If there is no
            /// translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK = 0x01,

            /// <summary>
            /// uCode is a virtual-key code and is translated into an unshifted
            /// character value in the low-order word of the return value. Dead keys (diacritics)
            /// are indicated by setting the top bit of the return value. If there is no
            /// translation, the function returns 0.
            /// </summary>
            MAPVK_VK_TO_CHAR = 0x02,

            /// <summary>
            /// Windows NT/2000/XP: uCode is a scan code and is translated into a
            /// virtual-key code that distinguishes between left- and right-hand keys. If
            /// there is no translation, the function returns 0.
            /// </summary>
            MAPVK_VSC_TO_VK_EX = 0x03,

            /// <summary>
            /// Not currently documented
            /// </summary>
            MAPVK_VK_TO_VSC_EX = 0x04
        }

        private RawInputHook Hook { get { return Program.Hook; } }
        private Input Interception { get { return Program.Interception; } set { Program.Interception = value; } }

        internal Dictionary<string, ICondition> Conditions;
        internal Dictionary<string, IMacro> Macros;

        internal KeybindForm KeybindForm;

        internal Dictionary<string, KeybindDevice> KeybindDevices = new Dictionary<string, KeybindDevice>();
        public KeybindDevice[] Devices { get { lock (KeybindDevices) return KeybindDevices.Values.ToArray(); } }

        private Dictionary<string, List<Keybind>> KeybindsToInvoke = new Dictionary<string, List<Keybind>>();

        private IntPtr handle;
        private bool Loaded = false;

        public MainForm()
        {
            Program.MainForm = this;

            InitializeComponent();

            handle = Handle;

            InputInterceptionModeComboBox.SelectedIndex = (int)Program.Settings.InputInterceptionMode;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)) StartWithWindowsCheckBox.Checked = key.GetValue(Application.ProductName) is string path && path.Equals($"\"{Application.ExecutablePath}\"");

            ShowConsoleCheckBox.Checked = Program.Settings.ShowConsole;
#warning TODO: Cleanup multiple input interception modes implementation, create an abstract class for different implementations to inherit
            if (Program.Settings.InputInterceptionMode == InputInterceptionMode.Hook)
            {
                while (!Hook.Install(Handle))
                {
                    if (MessageBox.Show("Hook installation failed.", "Multi Device Keybinds", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                    {
                        Environment.Exit(0);

                        return;
                    }
                }

                if (Program.RunningAsAdmin)
                {
                    Process p = null;

                    new Thread(() =>
                    {
                        while (!Program.Closing)
                        {
                            p = CreateSubprocess(Handle);

                            p.WaitForExit();
                        }
                    })
                    {
                        IsBackground = true,
                        Name = "Subprocess loop thread"
                    }.Start();

                    while (p == null) Thread.Sleep(1);
                }

                Console.WriteLine();

                Hook.OnKeyPress += RawInputHook_KeyPressed;
                Hook.HookDisabledOnKeyPress += RawInputHook_KeyPressed;

                Hook.OnKeyPress += Keybinds_OnKeyPress;
                Hook.HandledKeyPress += Keybinds_HandledKeyPress;
            }
            else if (Program.Settings.InputInterceptionMode == InputInterceptionMode.Interception)
            {
                Hook.EnumerateDevices();

                Interception = new Input
                {
                    KeyboardFilterMode = KeyboardFilterMode.All
                };

                Interception.Load();

                Interception.OnKeyPressed += Interception_OnKeyPressed;
            }

            KeybindForm = new KeybindForm();

            Conditions = ConditionLoader.GetConditions();

            Macros = MacroLoader.GetMacros();

            foreach (ICondition condition in Conditions.Values) KeybindForm.ConditionComboBox.Items.Add(new IConditionWrapper(condition));

            foreach (IMacro macro in Macros.Values) KeybindForm.MacroComboBox.Items.Add(new IMacroWrapper(macro));

            RawInputHook_DeviceListUpdated(null, Hook.Devices);
            Hook.DeviceListUpdated += RawInputHook_DeviceListUpdated;

            Loaded = true;
        }

        private static Process CreateSubprocess(IntPtr Handle)
        {
            PipeSecurity ps = new PipeSecurity();
            ps.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite, AccessControlType.Allow));

            NamedPipeServerStream server = new NamedPipeServerStream($"MultiDeviceKeybinds_{Process.GetCurrentProcess().Id}", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, ps);

            Process subprocess = WinSafer.CreateSaferProcess(Assembly.GetEntryAssembly().Location, $"-hookonly {Process.GetCurrentProcess().Id} {(long)Handle}", SaferLevel.NormalUser, IntegrityLevel.Medium);

            try
            {
                Console.Write("[ADMIN] Waiting for subprocess connection... ");

                server.WaitForConnection();

                Console.WriteLine("Subprocess connected.");

                Program.PipeWriter = new StreamWriter(server);
                Program.PipeReader = new StreamReader(server);

                Program.PipeWriter.AutoFlush = true;

                Console.Write("[ADMIN] Wait for sync... ");

                Program.PipeWriter.WriteLine("PING");

                server.WaitForPipeDrain();

                while (Program.PipeReader.ReadLine() is string output && !output.Equals("PONG", StringComparison.InvariantCultureIgnoreCase)) { }

                Console.WriteLine("Synced.\r\n");

                PipeIO("ADMIN", server);
            }
            catch (IOException e)
            {
                Console.WriteLine($"[ADMIN] Error: {e}");
            }

            return subprocess;
        }

        internal static void PipeIO(string prefix, PipeStream pipe)
        {
            new Thread(() =>
            {
                try
                {
                    while (Program.PipeReader.ReadLine() is string output)
                    {
                        Message m = JsonConvert.DeserializeObject<Message>(output);

                        if (Program.MainForm != null)
                        {
                            Console.WriteLine($"INPUT FROM SUBPROCESS");

                            if (Program.Hook.ProcessHookMessage(ref m)) Program.PipeWriter?.WriteLine(JsonConvert.SerializeObject(m));
                        }
                        else if (Program.UnelevatedHookForm != null)
                        {
                            lock (Program.UnelevatedHookForm.MessageQueue) Program.UnelevatedHookForm.MessageQueue.Enqueue(m);

                            //Console.WriteLine(Program.UnelevatedHookForm.WaitHandle.WaitOne(0));

                            Program.UnelevatedHookForm.WaitHandle.Set();
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine($"[{prefix} READER] Error: {e.Message}");
                }
                finally
                {
                    //Console.WriteLine($"[{prefix}] Reader closing");

                    try
                    {
                        Program.PipeReader.BaseStream.Dispose();

                        Program.PipeReader.Dispose();
                    }
                    catch
                    {
                    }

                    Program.PipeReader = null;
                }
            })
            {
                IsBackground = true,
                Name = "Pipe reader thread"
            }.Start();
        }

        private void UpdateKeyPress(KeybindDevice device, Keys key, KeyState state)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateKeyPress(device, key, state)));

                return;
            }

            KeyPressOutputLabel.Text = $"Device name: {device.Name}\r\n" +
                      $"Device ID: {device.ID}\r\n" +
                      $"Key: {key}\r\n" +
                      $"State: {state}\r\n";

            DevicesListView.SuspendLayout();

            foreach (DeviceListViewItem item in DevicesListView.Items)
            {
                item.BackColor = item.Device.ID == device.ID ? Color.Green : SystemColors.Window;
                item.ForeColor = item.Device.ID == device.ID ? Color.White : Color.Black;
            }

            DevicesListView.ResumeLayout();
        }

        private void RawInputHook_KeyPressed(object sender, RawInputKeyPressEventArgs e)
        {
            KeybindDevice device = null;
            lock (KeybindDevices)
            {
                if (KeybindDevices.ContainsKey(e.Device.ID))
                {
                    device = KeybindDevices[e.Device.ID];

                    device.Pressed = e.Device.Pressed;
                    device.LastPressed = e.Device.LastPressed;
                }
            }

            if (Program.ForegroundWindow != Handle) return;

            UpdateKeyPress(device, e.CorrectedKey, (KeyState)e.KeyPressState);
        }

        private void Interception_OnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            Keys key = (Keys)MapVirtualKeyEx((uint)e.Key, (uint)MapVirtualKeyMapTypes.MAPVK_VSC_TO_VK_EX, IntPtr.Zero);

            if (e.State.HasFlag(Interceptor.KeyState.E0))
            {
                if (key == Keys.LControlKey)
                {
                    key = Keys.RControlKey;
                }
                else if (key == Keys.LMenu)
                {
                    key = Keys.RMenu;
                }
            }

            KeyState state = e.State.HasFlag(Interceptor.KeyState.Up) ? KeyState.Break : KeyState.Make, lastState = KeyState.Break;

            IntPtr hardwareidptr = Marshal.AllocHGlobal(512);

            int hardwareidlength = InterceptionDriver.GetHardwareId(Interception.Context, Interception.DeviceID, hardwareidptr, 512);
            string hardwareid = Marshal.PtrToStringAuto(hardwareidptr);

            Marshal.FreeHGlobal(hardwareidptr);

            hardwareid = $@"\\?\{hardwareid.Replace('\\', '#')}#";

            int revidx = hardwareid.IndexOf("&REV_", StringComparison.InvariantCultureIgnoreCase);
            if (revidx > -1) hardwareid = $"{hardwareid.Substring(0, revidx)}{hardwareid.Substring(revidx + 9)}";

            KeybindDevice device = null;
            lock (KeybindDevices) device = KeybindDevices.Values.FirstOrDefault(d => d.ID.StartsWith(hardwareid, StringComparison.InvariantCultureIgnoreCase));

            if (device != null)
            {
                lastState = device.Pressed.Contains(key) ? KeyState.Make : KeyState.Break;

                List<Keys> pressed = device.Pressed.ToList();

                if (state == KeyState.Make && !pressed.Contains(key))
                {
                    pressed.Add(key);
                }
                else if (state == KeyState.Break && pressed.Contains(key))
                {
                    pressed.RemoveAll(k => k == key);
                }

                device.LastPressed = device.Pressed;
                device.Pressed = pressed.ToArray();

                Console.WriteLine($"{device.Name} {hardwareid} {key} {state}");

                if (Program.ForegroundWindow == handle)
                {
                    UpdateKeyPress(device, key, state);

                    return;
                }
                else if (KeybindForm.SetKeys(device, state))
                {
                    e.Handled = true;

                    return;
                }

                string guid = Guid.NewGuid().ToString();

                e.Handled = InvokeKeybindsCondition(guid, device, key, state, lastState);

                if (e.Handled) Console.WriteLine($"BLOCKED {key} {state}");

                new Thread(() =>
                {
                    InvokeKeybindsMacro(guid, device, key, state, lastState);
                }).Start();
            }
        }

        private void InputInterceptionModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded) return;

            MessageBox.Show("Restart the program for the changes to take effect.", "Multi Device Keybinds");

            Program.Settings.InputInterceptionMode = (InputInterceptionMode)InputInterceptionModeComboBox.SelectedIndex;

            Program.Settings.Save();
        }

        private void StartWithWindowsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!Loaded) return;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (StartWithWindowsCheckBox.Checked)
                {
                    key.SetValue(Application.ProductName, $"\"{Application.ExecutablePath}\"");
                }
                else if (key.GetValue(Application.ProductName) != null)
                {
                    key.DeleteValue(Application.ProductName);
                }
            }
        }

        private void ShowConsoleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowConsoleCheckBox.Checked)
            {
                Program.ShowConsole();
            }
            else
            {
                Program.HideConsole();
            }

            if (!Loaded) return;

            Program.Settings.ShowConsole = ShowConsoleCheckBox.Checked;

            Program.Settings.Save();
        }

        private void Keybinds_OnKeyPress(object sender, RawInputKeyPressEventArgs e)
        {
            KeybindDevice device = null;
            lock (KeybindDevices) if (KeybindDevices.ContainsKey(e.Device.ID)) device = KeybindDevices[e.Device.ID];

            InvokeKeybindsCondition(e.GUID, device, e.CorrectedKey, (KeyState)e.KeyPressState, (KeyState)e.LastKeyPressState);
        }

        private bool InvokeKeybindsCondition(string guid, KeybindDevice device, Keys pressedKey, KeyState state, KeyState lastState)
        {
            bool handled = false;

            Keybind[] keybinds = null;

            lock (device?.Keybinds) keybinds = device.Keybinds.ToArray();

            if (keybinds?.Length > 0)
            {
                foreach (Keybind keybind in keybinds)
                {
                    if (!keybind.Enabled || keybind.Keys == null) continue;

                    IEnumerable<Keys> pressed = state == KeyState.Make ? device.Pressed : device.LastPressed;

                    if (pressed.Count() < keybind.Keys.Count || !keybind.ActivateIfMoreKeysPressed && pressed.Count() > keybind.Keys.Count) continue;

                    bool invoke = true;

                    if (keybind.MatchKeysOrder)
                    {
                        for (int i = 0; i < keybind.Keys.Count; i++)
                        {
                            if (pressed.ElementAt(i) != keybind.Keys[i])
                            {
                                invoke = false;

                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (Keys key in keybind.Keys)
                        {
                            if (!pressed.Contains(key))
                            {
                                invoke = false;

                                break;
                            }
                        }
                    }

                    if (!invoke) continue;

#warning condition might need to change to include test for the current key being in the keybind's keys
                    if (!keybind.ActivateIfMoreKeysPressed)
                    {
                        foreach (Keys key in pressed)
                        {
                            if (!keybind.Keys.Contains(key))
                            {
                                invoke = false;

                                break;
                            }
                        }
                    }

                    if (!invoke) continue;

                    bool condition = keybind.TestCondition(keybind.Device, pressedKey, state, lastState);

                    if (condition)
                    {
                        if (state == KeyState.Make && lastState != KeyState.Make && !keybind.ActivateOnKeyDown || state == KeyState.Make && lastState == KeyState.Make && !keybind.ActivateOnHold || state == KeyState.Break && !keybind.ActivateOnKeyUp)
                        {
                            if (keybind.Keys.Count > 0) handled = true;

                            continue;
                        }

                        handled = true;

                        lock (KeybindsToInvoke)
                        {
                            if (!KeybindsToInvoke.ContainsKey(guid)) KeybindsToInvoke.Add(guid, new List<Keybind>());

                            KeybindsToInvoke[guid].Add(keybind);
                        }

                        if (!keybind.AllowOtherKeybinds) break;

                        //continue;
                    }

                    //if (state == KeyState.Make && lastState != KeyState.Make && keybind.BlockKeyDown || state == KeyState.Make && lastState == KeyState.Make && keybind.BlockKeyHold || state == KeyState.Break && keybind.BlockKeyUp) handled = true;
                }
            }

            return handled;
        }

        private void Keybinds_HandledKeyPress(object sender, RawInputKeyPressEventArgs e)
        {
            KeybindDevice device = null;
            lock (KeybindDevices) if (KeybindDevices.ContainsKey(e.Device.ID)) device = KeybindDevices[e.Device.ID];

            InvokeKeybindsMacro(e.GUID, device, e.CorrectedKey, (KeyState)e.KeyPressState, (KeyState)e.LastKeyPressState);
        }

        private bool InvokeKeybindsMacro(string guid, KeybindDevice device, Keys pressedKey, KeyState state, KeyState lastState)
        {
            bool handled = false;

            List<Keybind> keybinds = null;

            lock (KeybindsToInvoke)
            {
                if (KeybindsToInvoke.ContainsKey(guid))
                {
                    keybinds = KeybindsToInvoke[guid];

                    KeybindsToInvoke.Remove(guid);
                }
            }

            if (keybinds != null) foreach (Keybind keybind in keybinds) if (keybind.PerformMacro(keybind.Device, pressedKey, state, lastState)) handled = true;

            return handled;
        }

        private void RawInputHook_DeviceListUpdated(object sender, Device[] devices)
        {
            Device selected = DevicesListView.SelectedItems.Count > 0 ? ((DeviceListViewItem)DevicesListView.SelectedItems[0]).Device : null;

            DevicesListView.SuspendLayout();

            DevicesListView.SelectedItems.Clear();

            DevicesListView.Items.Clear();

            lock (KeybindDevices)
            {
                foreach (string id in KeybindDevices.Keys) KeybindDevices[id].Connected = false;

                foreach (Device device in devices)
                {
                    if (device.Handle == IntPtr.Zero) continue;

                    if (!KeybindDevices.ContainsKey(device.ID)) KeybindDevices.Add(device.ID, KeybindDevice.Load(KeybindDevice.GetFileName(device.ID)) ?? new KeybindDevice(device));

                    KeybindDevice d = KeybindDevices[device.ID];

                    d.Handle = device.Handle;

                    d.Connected = true;
                }

                string folder = Path.Combine(Program.Location, "Keybinds");
                IEnumerable<string> files = KeybindDevices.Keys.Select(id => KeybindDevice.GetFileName(id));
                foreach (string file in Directory.GetFiles(folder, "*.json", SearchOption.TopDirectoryOnly))
                {
                    string filename = Path.GetFileName(file);
                    if (!files.Contains(filename))
                    {
                        try
                        {
                            KeybindDevice d = KeybindDevice.Load(filename);

                            if (KeybindDevice.GetFileName(d.ID) == filename && !KeybindDevices.ContainsKey(d.ID) && d.Keybinds.Count > 0) KeybindDevices.Add(d.ID, d);
                        }
                        catch
                        {
                        }
                    }
                }

                foreach (string id in KeybindDevices.Keys.ToArray())
                {
                    if (KeybindDevices[id].Keybinds.Count > 0 || KeybindDevices[id].Connected)
                    {
                        if (!KeybindDevices[id].Hidden) DevicesListView.Items.Add(new DeviceListViewItem(KeybindDevices[id]));

                        continue;
                    }

                    KeybindDevices.Remove(id);
                }

                if (KeybindForm.Device != null && !KeybindDevices.ContainsKey(KeybindForm.Device.ID)) KeybindForm.DialogResult = DialogResult.Cancel;

                if (selected != null && !KeybindDevices.ContainsKey(selected.ID)) ClearKeybinds();
            }

            DevicesListView.ResumeLayout();
        }

        private void DevicesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddKeybindButton.Enabled = DevicesListView.SelectedItems.Count > 0;

            if (DevicesListView.SelectedItems.Count > 0)
            {
                ShowDeviceKeybinds(((DeviceListViewItem)DevicesListView.SelectedItems[0]).Device.ID);
            }
            else
            {
                ClearKeybinds();
            }
        }

        private void DevicesListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            ListViewHitTestInfo hit = DevicesListView.HitTest(e.Location);
            if (hit.Item != null)
            {
                KeybindDevice device = ((DeviceListViewItem)hit.Item).Device;

                if (MessageBox.Show($"Name: {device.Name}\r\n" +
                    $"ID: {device.ID}\r\n\r\n" +
                    $"Are you sure you want to hide this device?", "Hide device confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

                device.Hidden = true;

                device.Save();

                ClearKeybinds();

                DevicesListView.SuspendLayout();

                DevicesListView.Items.Remove(hit.Item);

                DevicesListView.ResumeLayout();
            }
        }

        private void DevicesListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void ClearKeybinds()
        {
            KeybindsListView.SuspendLayout();

            KeybindsListView.SelectedItems.Clear();

            KeybindsListView.Items.Clear();

            KeybindsListView.ResumeLayout();
        }

        private void ShowDeviceKeybinds(string deviceID)
        {
            ClearKeybinds();

            KeybindsListView.SuspendLayout();

            lock (KeybindDevices) if (KeybindDevices.ContainsKey(deviceID)) foreach (Keybind keybind in KeybindDevices[deviceID].Keybinds) KeybindsListView.Items.Add(new KeybindListViewItem(keybind));

            KeybindsListView.ResumeLayout();
        }

        private void KeybindsListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hit = KeybindsListView.HitTest(e.Location);
            if (hit.Item != null) EditKeybindButton.PerformClick();
        }

        private void KeybindsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            KeybindListViewItem item = (KeybindListViewItem)e.Item;

            Keybind keybind = item.Keybind;
            Device device = keybind.Device;

            keybind.Enabled = item.Checked;

            KeybindsListView.SuspendLayout();

            item.Update();

            KeybindsListView.ResumeLayout();

            if (device != null) lock (KeybindDevices) KeybindDevices[device.ID].Save();
        }

        private void KeybindsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditKeybindButton.Enabled = RemoveKeybindButton.Enabled = KeybindsListView.SelectedItems.Count > 0;
        }

        private void AddKeybindButton_Click(object sender, EventArgs e)
        {
            if (DevicesListView.SelectedItems.Count > 0)
            {
                KeybindDevice device;
                lock (KeybindDevices) device = KeybindDevices[((DeviceListViewItem)DevicesListView.SelectedItems[0]).Device.ID];

                Keybind keybind = KeybindForm.Add(this, device);

                if (keybind != null)
                {
                    device.Keybinds.Add(keybind);

                    device.Save();

                    KeybindsListView.Items.Add(new KeybindListViewItem(keybind));
                }
            }
        }

        private void EditKeybindButton_Click(object sender, EventArgs e)
        {
            if (DevicesListView.SelectedItems.Count > 0 && KeybindsListView.SelectedItems.Count > 0)
            {
                KeybindListViewItem selected = ((KeybindListViewItem)KeybindsListView.SelectedItems[0]);

                KeybindDevice device;
                lock (KeybindDevices) device = KeybindDevices[((DeviceListViewItem)DevicesListView.SelectedItems[0]).Device.ID];
                Keybind selectedKeybind = selected.Keybind;

                Keybind keybind = KeybindForm.Edit(this, device, selected.Keybind);

                if (keybind != null)
                {
                    int index = -1;
                    if (device.Keybinds.Contains(selectedKeybind))
                    {
                        index = device.Keybinds.IndexOf(selectedKeybind);

                        device.Keybinds.Remove(selectedKeybind);
                    }

                    selected.Keybind = keybind;

                    if (index == -1)
                    {
                        device.Keybinds.Add(keybind);
                    }
                    else
                    {
                        device.Keybinds.Insert(index, keybind);
                    }

                    device.Save();

                    KeybindsListView.SuspendLayout();

                    selected.Update();

                    KeybindsListView.ResumeLayout();
                }
            }
        }

        private void RemoveKeybindButton_Click(object sender, EventArgs e)
        {
            if (DevicesListView.SelectedItems.Count > 0 && KeybindsListView.SelectedItems.Count > 0)
            {
                KeybindListViewItem selected = ((KeybindListViewItem)KeybindsListView.SelectedItems[0]);
                string device = ((DeviceListViewItem)DevicesListView.SelectedItems[0]).Device.ID;
                Keybind selectedKeybind = selected.Keybind;

                if (MessageBox.Show($"Are you sure you want to remove the\r\n" +
                    $"Keybind \"{selectedKeybind.Name}\"?", "Keybind removal confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

                lock (KeybindDevices)
                {
                    if (KeybindDevices[device].Keybinds.Contains(selectedKeybind))
                    {
                        KeybindDevices[device].Keybinds.Remove(selectedKeybind);

                        KeybindDevices[device].Save();
                    }
                }

                KeybindsListView.Items.Remove(selected);
            }
        }
    }
}