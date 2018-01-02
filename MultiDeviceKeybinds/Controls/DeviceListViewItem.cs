using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    class DeviceListViewItem : ListViewItem
    {
        public KeybindDevice Device { get; private set; }

        public DeviceListViewItem(KeybindDevice device) : base()
        {
            Device = device;

            Text = $"{(Device.Connected ? "" : "[Unplugged] ")}{Device.Name}";

            SubItems.AddRange(new ListViewSubItem[]
            {
                new ListViewSubItem(this, Device?.ID){ Name = "ID" },
            });
        }
    }
}