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
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    partial class MainForm : Form
    {
        private RawInputHook Hook { get { return Program.Hook; } }

        internal Dictionary<string, ICondition> Conditions;
        internal Dictionary<string, IMacro> Macros;

        internal KeybindForm KeybindForm;

        internal Dictionary<string, KeybindDevice> KeybindDevices = new Dictionary<string, KeybindDevice>();
        public KeybindDevice[] Devices { get { lock (KeybindDevices) return KeybindDevices.Values.ToArray(); } }

        private Dictionary<string, List<Keybind>> KeybindsToInvoke = new Dictionary<string, List<Keybind>>();

        private bool Loaded = false;

        public MainForm()
        {
            Program.MainForm = this;

            InitializeComponent();

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)) StartWithWindowsCheckBox.Checked = key.GetValue(Application.ProductName) is string path && path.Equals($"\"{Application.ExecutablePath}\"");

            while (!Hook.Install(Handle))
            {
                if (MessageBox.Show("Hook installation failed.", "Multi Device Keybinds", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                {
                    //Application.Exit();
                    Process.GetCurrentProcess().Kill();

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

            KeybindForm = new KeybindForm();

            Conditions = ConditionLoader.GetConditions();

            Macros = MacroLoader.GetMacros();

            foreach (ICondition condition in Conditions.Values) KeybindForm.ConditionComboBox.Items.Add(new IConditionWrapper(condition));

            foreach (IMacro macro in Macros.Values) KeybindForm.MacroComboBox.Items.Add(new IMacroWrapper(macro));

            RawInputHook_DeviceListUpdated(null, Hook.Devices);
            Hook.DeviceListUpdated += RawInputHook_DeviceListUpdated;

            Hook.OnKeyPress += RawInputHook_KeyPressed;
            Hook.HookDisabledOnKeyPress += RawInputHook_KeyPressed;

            Hook.OnKeyPress += Keybinds_OnKeyPress;
            Hook.HandledKeyPress += Keybinds_HandledKeyPress;

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

        private async void RawInputHook_KeyPressed(object sender, RawInputKeyPressEventArgs e)
        {
            lock (KeybindDevices)
            {
                if (KeybindDevices.ContainsKey(e.Device.ID))
                {
                    KeybindDevices[e.Device.ID].Pressed = e.Device.Pressed;
                    KeybindDevices[e.Device.ID].LastPressed = e.Device.LastPressed;
                }
            }

            if (Program.ForegroundWindow != Handle) return;

            await Task.Run(() =>
            {
                this.InvokeIfRequired(() =>
                {
                    KeyPressOutputLabel.Text = $"Device name: {e.Device.Name}\r\n" +
                      $"Device ID: {e.Device.ID}\r\n" +
                      $"Key: {e.CorrectedKey}\r\n" +
                      $"State: {e.KeyPressState}\r\n";

                    DevicesListView.SuspendLayout();

                    foreach (DeviceListViewItem item in DevicesListView.Items)
                    {
                        item.BackColor = item.Device.ID == e.Device.ID ? Color.Green : SystemColors.Window;
                        item.ForeColor = item.Device.ID == e.Device.ID ? Color.White : Color.Black;
                    }

                    DevicesListView.ResumeLayout();
                });
            });
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
        }

        private void Keybinds_OnKeyPress(object sender, RawInputKeyPressEventArgs e)
        {
            Keybind[] keybinds = null;

            lock (KeybindDevices) if (KeybindDevices.ContainsKey(e.Device.ID)) keybinds = KeybindDevices[e.Device.ID].Keybinds.ToArray();

            if (keybinds?.Length > 0)
            {
                bool handled = false;

                KeyState state = (KeyState)e.KeyPressState, lastState = (KeyState)e.LastKeyPressState;

                foreach (Keybind keybind in keybinds)
                {
                    if (!keybind.Enabled || keybind.Keys == null) continue;

                    IEnumerable<Keys> pressed = state == KeyState.Make ? e.Device.Pressed : e.Device.LastPressed;

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

                    bool condition = keybind.TestCondition(keybind.Device, e.Key, e.CorrectedKey, state, lastState);

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
                            if (!KeybindsToInvoke.ContainsKey(e.GUID)) KeybindsToInvoke.Add(e.GUID, new List<Keybind>());

                            KeybindsToInvoke[e.GUID].Add(keybind);
                        }

                        if (!keybind.AllowOtherKeybinds) break;

                        //continue;
                    }

                    //if (state == KeyState.Make && lastState != KeyState.Make && keybind.BlockKeyDown || state == KeyState.Make && lastState == KeyState.Make && keybind.BlockKeyHold || state == KeyState.Break && keybind.BlockKeyUp) handled = true;
                }

                e.Handled = handled;
            }
        }

        private void Keybinds_HandledKeyPress(object sender, RawInputKeyPressEventArgs e)
        {
            List<Keybind> keybinds = null;

            lock (KeybindsToInvoke)
            {
                if (KeybindsToInvoke.ContainsKey(e.GUID))
                {
                    keybinds = KeybindsToInvoke[e.GUID];

                    KeybindsToInvoke.Remove(e.GUID);
                }
            }

            if (keybinds != null)
            {
                bool handled = false;

                KeyState state = (KeyState)e.KeyPressState, lastState = (KeyState)e.LastKeyPressState;

                foreach (Keybind keybind in keybinds) if (keybind.PerformMacro(keybind.Device, e.Key, e.CorrectedKey, state, lastState)) handled = true;

                e.Handled = handled;
            }
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