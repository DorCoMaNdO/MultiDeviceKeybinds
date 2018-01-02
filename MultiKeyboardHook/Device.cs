using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MultiKeyboardHook
{
    /// <summary>
    /// An input device.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// The ID of the device.
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// The type of the device.
        /// </summary>
        public string Type { get; private set; }
        [JsonIgnore]
        /// <summary>
        /// The handle of the device.
        /// </summary>
        public IntPtr Handle { get; protected internal set; }
        /// <summary>
        /// The description (usually the name) of the device.
        /// </summary>
        public string Name { get; private set; }
        [JsonIgnore]
        /// <summary>
        /// The keys pressed on the device.
        /// </summary>
        public Keys[] Pressed { get; protected internal set; } = new Keys[0];
        [JsonIgnore]
        public bool RControl { get { return Pressed.Contains(Keys.RControlKey); } }
        [JsonIgnore]
        public bool LControl { get { return Pressed.Contains(Keys.LControlKey); } }
        [JsonIgnore]
        public bool Control
        {
            get
            {
                return Pressed.Contains(Keys.Control) || Pressed.Contains(Keys.ControlKey) || RControl || LControl;
            }
        }
        [JsonIgnore]
        public bool RShift { get { return Pressed.Contains(Keys.RShiftKey); } }
        [JsonIgnore]
        public bool LShift { get { return Pressed.Contains(Keys.LShiftKey); } }
        [JsonIgnore]
        public bool Shift
        {
            get
            {
                return Pressed.Contains(Keys.Shift) || Pressed.Contains(Keys.ShiftKey) || RShift || LShift;
            }
        }
        [JsonIgnore]
        public bool RAlt { get { return Pressed.Contains(Keys.RMenu); } }
        [JsonIgnore]
        public bool LAlt { get { return Pressed.Contains(Keys.LMenu); } }
        [JsonIgnore]
        public bool Alt
        {
            get
            {
                return Pressed.Contains(Keys.Alt) || Pressed.Contains(Keys.Menu) || RAlt || LAlt;
            }
        }

        [JsonConstructor]
        protected internal Device(string id, string type, string name) : this(id, type, IntPtr.Zero, name)
        {
        }

        protected internal Device(string id, string type, IntPtr handle, string name)
        {
            ID = id;
            Type = type;
            Handle = handle;
            Name = name;
        }
    }
}