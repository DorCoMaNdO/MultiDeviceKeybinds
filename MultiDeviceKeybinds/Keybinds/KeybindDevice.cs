using MultiKeyboardHook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    public class KeybindDevice : Device
    {
        #region Expose Device properties without the need to reference MultiKeyboardHook
        /// <summary>
        /// The ID of the device.
        /// </summary>
        public new string ID { get { return base.ID; } }
        /// <summary>
        /// The type of the device.
        /// </summary>
        public new string Type { get { return base.Type; } }
        [JsonIgnore]
        /// <summary>
        /// The handle of the device.
        /// </summary>
        public new IntPtr Handle { get { return base.Handle; } internal set { base.Handle = value; } }
        /// <summary>
        /// The description (usually the name) of the device.
        /// </summary>
        public new string Name { get { return base.Name; } }
        [JsonIgnore]
        /// <summary>
        /// The keys pressed on the device.
        /// </summary>
        public new Keys[] Pressed { get { return base.Pressed; } internal set { base.Pressed = value; } }
        [JsonIgnore]
        public new bool RControl { get { return base.RControl; } }
        [JsonIgnore]
        public new bool LControl { get { return base.LControl; } }
        [JsonIgnore]
        public new bool Control { get { return base.Control; } }
        [JsonIgnore]
        public new bool RShift { get { return base.RShift; } }
        [JsonIgnore]
        public new bool LShift { get { return base.LShift; } }
        [JsonIgnore]
        public new bool Shift { get { return base.Shift; } }
        [JsonIgnore]
        public new bool RAlt { get { return base.RAlt; } }
        [JsonIgnore]
        public new bool LAlt { get { return base.LAlt; } }
        [JsonIgnore]
        public new bool Alt { get { return base.Alt; } }
        #endregion

        [JsonIgnore]
        public bool Connected { get; internal set; }
        public bool Hidden { get; internal set; }
#warning keybind profiles with a macro to switch them
        [JsonProperty]
        internal List<Keybind> Keybinds { get; private set; } = new List<Keybind>();

        protected internal KeybindDevice(Device device) : base(device.ID, device.Type, device.Handle, device.Name)
        {
        }

        [JsonConstructor]
        protected internal KeybindDevice(string id, string type, string name, bool hidden) : this(id, type, IntPtr.Zero, name, hidden)
        {
        }

        protected internal KeybindDevice(string id, string type, IntPtr handle, string name, bool hidden = false) : base(id, type, handle, name)
        {
            Hidden = hidden;
        }

        public void Save()
        {
            string folder = Path.Combine(Program.Location, "Keybinds");
            Directory.CreateDirectory(folder);

            string file = Path.Combine(folder, GetFileName());

            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static string GetFileName(string deviceID)
        {
            string id = deviceID;
            id = id.Replace(Path.AltDirectorySeparatorChar.ToString(), "");
            id = id.Replace(Path.DirectorySeparatorChar.ToString(), "");
            id = id.Replace("?", "").Replace("{", "").Replace("}", "");
            foreach (char c in Path.GetInvalidPathChars()) id = id.Replace(c.ToString(), "");

            return $"{id}.json";
        }

        private string GetFileName()
        {
            return GetFileName(ID);
        }

        public static KeybindDevice Load(string deviceIDfile)
        {
            string folder = Path.Combine(Program.Location, "Keybinds");
            Directory.CreateDirectory(folder);

            string file = Path.Combine(folder, deviceIDfile);

            KeybindDevice device = File.Exists(file) ? JsonConvert.DeserializeObject<KeybindDevice>(File.ReadAllText(file)) : null;

            if (device != null) foreach (Keybind keybind in device.Keybinds) keybind.Device = device;

            return device;
        }
    }
}