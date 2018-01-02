using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    class KeybindListViewItem : ListViewItem
    {
        public Keybind Keybind { get; internal set; }

        public KeybindListViewItem(Keybind keybind) : base()
        {
            Keybind = keybind;

            Text = Keybind?.Name ?? "Unnamed Macro";

            Checked = (Keybind?.Enabled).GetValueOrDefault();

            string keys = string.Join(Keybind.KeysSeparator, Keybind?.Keys);
            SubItems.AddRange(new ListViewSubItem[]
            {
                //new ListViewSubItem(this, (Keybind?.Enabled).GetValueOrDefault() ? "✔" : "✕"){ Name = "Enabled" },
                new ListViewSubItem(this, !string.IsNullOrEmpty(keys) ? keys : "Not set"){ Name = "Keys" },
                new ListViewSubItem(this, Keybind?.Condition?.Name ?? "N/A"){ Name = "Condition" },
                new ListViewSubItem(this, Keybind?.Macro?.Name ?? "N/A - Blocking"){ Name = "Macro" },
            });
        }

        public void Update()
        {
            Text = Keybind?.Name ?? "Unnamed Macro";

            Checked = (Keybind?.Enabled).GetValueOrDefault();

            //SubItems["Enabled"].Text = (Keybind?.Enabled).GetValueOrDefault() ? "✔" : "✕";

            string keys = string.Join(Keybind.KeysSeparator, Keybind?.Keys);
            SubItems["Keys"].Text = !string.IsNullOrEmpty(keys) ? keys : "Not set";

            SubItems["Condition"].Text = Keybind?.Condition?.Name ?? "N/A";

            SubItems["Macro"].Text = Keybind?.Macro?.Name ?? "N/A - Blocking";
        }
    }
}