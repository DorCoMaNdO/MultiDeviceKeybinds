using MultiKeyboardHook;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
#warning argument class?
    enum ArgType
    {
        String = 0,
        Int
    }

    partial class KeybindForm : Form
    {
        private Keybind Keybind;
        private bool Configuring = false;
        internal KeybindDevice Device = null;

        public KeybindForm()
        {
            InitializeComponent();

            NameTextBox.AutoSize = KeysTextBox.AutoSize = ConditionArgTextBox.AutoSize = MacroArgTextBox.AutoSize = false;
            NameTextBox.Height = KeysTextBox.Height = ConditionArgTextBox.Height = MacroArgTextBox.Height = 21;

            KeybindForm_Resize(null, EventArgs.Empty);

            ConditionArgTypeComboBox.Items.AddRange(Enum.GetNames(typeof(ArgType)));
            MacroArgTypeComboBox.Items.AddRange(Enum.GetNames(typeof(ArgType)));

            Program.Hook.HookDisabledOnKeyPress += RawInputHook_HookDisabledOnKeyPress;
        }

        internal Keybind Add(MainForm parent, KeybindDevice device, Keybind clone = null)
        {
            Configuring = true;

            AddEditButton.Text = "Add as new keybind";

            return Edit(parent, device, clone ?? null);
        }

        internal Keybind Edit(MainForm parent, KeybindDevice device, Keybind keybind)
        {
            if (!Configuring) AddEditButton.Text = "Confirm changes";

            Configuring = true;

            Device = device;

            Keybind = keybind == null ? new Keybind() : new Keybind(keybind.Name, keybind.Keys.ToList(), keybind.Condition, keybind.ConditionArgs, keybind.Macro, keybind.MacroArgs)
            {
                Device = Device,
                Enabled = keybind.Enabled,
                MatchKeysOrder = keybind.MatchKeysOrder,
                ActivateOnKeyDown = keybind.ActivateOnKeyDown,
                ActivateOnHold = keybind.ActivateOnHold,
                ActivateOnKeyUp = keybind.ActivateOnKeyUp,
                ActivateIfMoreKeysPressed = keybind.ActivateIfMoreKeysPressed,
                AllowOtherKeybinds = keybind.AllowOtherKeybinds,
            };

            NameTextBox.Text = Keybind.Name ?? "";

            NameTextBox.Select();

            EnabledCheckBox.Checked = Keybind.Enabled;

            UpdateKeys();

            ActivateOnKeyDownCheckBox.Checked = Keybind.ActivateOnKeyDown;
            ActivateOnHoldCheckBox.Checked = Keybind.ActivateOnHold;
            ActivateOnKeyUpCheckBox.Checked = Keybind.ActivateOnKeyUp;

            ActivateIfMoreKeysPressedCheckBox.Checked = Keybind.ActivateIfMoreKeysPressed;

            UpdateCondition();

            ConditionArgsListBox.Items.Clear();
            if (Keybind.ConditionArgs != null) foreach (object arg in Keybind.ConditionArgs) ConditionArgsListBox.Items.Add(arg);

            ConditionArgTypeComboBox.SelectedIndex = 0;

            UpdateMacro();

            MacroArgsListBox.Items.Clear();
            if (Keybind.MacroArgs != null) foreach (object arg in Keybind.MacroArgs) MacroArgsListBox.Items.Add(arg);

            MacroArgTypeComboBox.SelectedIndex = 0;

            AllowOtherKeybindsCheckBox.Checked = Keybind.AllowOtherKeybinds;

            Configuring = false;

            CheckCanSave();

            if (ShowDialog(parent) == DialogResult.Cancel) Keybind = null;

            return Keybind;
        }

        private void KeybindForm_Resize(object sender, EventArgs e)
        {
            ConditionDescriptionLabel.MaximumSize = ConditionArgsTakenLabel.MaximumSize = MacroDescriptionLabel.MaximumSize = MacroArgsTakenLabel.MaximumSize = InfoLabel.MaximumSize = new Size(ClientSize.Width - 6, ConditionArgsTakenLabel.MaximumSize.Height);
        }

        private void CheckCanSave()
        {
            AddEditButton.Enabled = Keybind.Name?.Length > 2;
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            Keybind.Name = NameTextBox.Text;

            CheckCanSave();
        }

        private void EnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Keybind.Enabled = EnabledCheckBox.Checked;
        }

        private void KeysTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void UpdateKeys()
        {
            KeysTextBox.Text = Keybind.Keys.Count > 0 ? string.Join(Keybind.KeysSeparator, Keybind.Keys) : "Not set";

            MatchKeysOrderCheckBox.Enabled = Keybind.Keys.Count > 1;

            ClearKeysButton.Enabled = Keybind.Keys.Count > 0;

            CheckCanSave();
        }

        private void RawInputHook_HookDisabledOnKeyPress(object sender, RawInputKeyPressEventArgs e)
        {
            if (Program.ForegroundWindow != Handle || !Visible || !KeysTextBox.Focused || Device?.ID != e.Device.ID) return;

            e.Handled = true;

            if ((KeyState)e.KeyPressState != KeyState.Make) return;

            Keybind.Keys.Clear();

            Keybind.Keys.AddRange(e.Device.Pressed);

            UpdateKeys();
        }

        private void MatchKeysOrderCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Keybind.MatchKeysOrder = MatchKeysOrderCheckBox.Checked;
        }

        private void ClearKeysButton_Click(object sender, EventArgs e)
        {
            Keybind.Keys.Clear();

            UpdateKeys();
        }

        private void ActivateOnKeyDownCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Keybind.ActivateOnKeyDown = ActivateOnKeyDownCheckBox.Checked;
        }

        private void ActivateOnHoldCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Keybind.ActivateOnHold = ActivateOnHoldCheckBox.Checked;
        }

        private void ActivateOnKeyUpCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Keybind.ActivateOnKeyUp = ActivateOnKeyUpCheckBox.Checked;
        }

        private void ActivateIfMoreKeysPressedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Keybind.ActivateIfMoreKeysPressed = ActivateIfMoreKeysPressedCheckBox.Checked;
        }

        private void ConditionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keybind.Condition = ConditionComboBox.SelectedIndex == 0 ? null : Program.MainForm.Conditions.ElementAt(ConditionComboBox.SelectedIndex - 1).Value;

            if (!Configuring) UpdateCondition();
        }

        private void ConditionUpdateControlPositions(int height)
        {
            if (height > ConditionArgsTakenLabel.MaximumSize.Height) height = ConditionArgsTakenLabel.MaximumSize.Height;
            if (height < -ConditionArgsTakenLabel.MaximumSize.Height) height = -ConditionArgsTakenLabel.MaximumSize.Height;

            label4.Top += height;
            ConditionArgsListBox.Top += height;
            ConditionArgTypeComboBox.Top += height;
            ConditionArgTextBox.Top += height;
            AddConditionArgButton.Top += height;
            RemoveConditionArgButton.Top += height;

            label3.Top += height;
            MacroComboBox.Top += height;
            MacroDescriptionLabel.Top += height;

            label7.Top += height;
            MacroArgsTakenLabel.Top += height;

            MacroUpdateControlPositions(height);
        }

        private void UpdateCondition()
        {
            ConditionUpdateControlPositions(-ConditionArgsTakenLabel.Height);

            string description = "Checks nothing", argstaken = "None";

            if (Keybind.ConditionTypeName != null)
            {
                ConditionComboBox.SelectedIndex = Array.IndexOf(Program.MainForm.Conditions.Keys.ToArray(), Keybind.ConditionTypeName) + 1;

                try
                {
                    description = Keybind.Condition.Description;
                }
                catch (NotImplementedException)
                {
                    description = "A description was not provided";
                }

                try
                {
                    argstaken = Keybind.Condition.ArgumentsTaken;
                }
                catch (NotImplementedException)
                {
                    argstaken = "Arguments taken were not provided";
                }
            }
            else
            {
                ConditionComboBox.SelectedIndex = 0;
            }

            ConditionDescriptionLabel.Text = description ?? "N/A";

            ConditionArgsTakenLabel.Text = argstaken ?? "None";

            ConditionUpdateControlPositions(ConditionArgsTakenLabel.Height);

            if (!Configuring)
            {
                ConditionArgsListBox.Items.Clear();
            }
            else
            {
                ConditionArgTextBox.Text = "";
            }
        }

        private void ConditionArgsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveConditionArgButton.Enabled = ConditionArgsListBox.SelectedIndex > -1;
        }

        private void ConditionArgTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckCanAddConditionArg();
        }

        private void ConditionArgTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckCanAddConditionArg();
        }

        private void ConditionArgTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                AddConditionArgButton.PerformClick();
            }
        }

        private void CheckCanAddConditionArg()
        {
            AddConditionArgButton.Enabled = ConditionComboBox.SelectedIndex > 0 && ConditionArgTextBox.Text.Length > 0 && TryGetArg(ConditionArgTextBox.Text, (ArgType)ConditionArgTypeComboBox.SelectedIndex, out _);
        }

        private void AddConditionArgButton_Click(object sender, EventArgs e)
        {
            if (!AddConditionArgButton.Enabled) return;

            string str = ConditionArgTextBox.Text;

            ConditionArgTextBox.Text = "";

            ConditionArgsListBox.Items.Add(GetArg(str, (ArgType)ConditionArgTypeComboBox.SelectedIndex));

            ConditionArgsListBox.SelectedIndex = ConditionArgsListBox.Items.Count - 1;

            ConditionArgTextBox.Focus();
        }

        private void RemoveConditionArgButton_Click(object sender, EventArgs e)
        {
            int index = ConditionArgsListBox.SelectedIndex;
            ConditionArgsListBox.Items.RemoveAt(index);

            ConditionArgsListBox.SelectedIndex = ConditionArgsListBox.Items.Count > index ? index : ConditionArgsListBox.Items.Count - 1;
        }

        private void MacroComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Keybind.Macro = MacroComboBox.SelectedIndex == 0 ? null : Program.MainForm.Macros.ElementAt(MacroComboBox.SelectedIndex - 1).Value;

            if (!Configuring) UpdateMacro();
        }

        private void MacroUpdateControlPositions(int height)
        {
            if (height > MacroArgsTakenLabel.MaximumSize.Height) height = MacroArgsTakenLabel.MaximumSize.Height;
            if (height < -MacroArgsTakenLabel.MaximumSize.Height) height = -MacroArgsTakenLabel.MaximumSize.Height;

            label8.Top += height;
            MacroArgsListBox.Top += height;
            MacroArgTypeComboBox.Top += height;
            MacroArgTextBox.Top += height;
            AddMacroArgButton.Top += height;
            RemoveMacroArgButton.Top += height;

            Height += height;
        }

        private void UpdateMacro()
        {
            MacroUpdateControlPositions(-MacroArgsTakenLabel.Height);

            string description = "Checks nothing", argstaken = "None";

            if (Keybind.MacroTypeName != null)
            {
                MacroComboBox.SelectedIndex = Array.IndexOf(Program.MainForm.Macros.Keys.ToArray(), Keybind.MacroTypeName) + 1;

                try
                {
                    description = Keybind.Macro.Description;
                }
                catch (NotImplementedException)
                {
                    description = "A description was not provided";
                }

                try
                {
                    argstaken = Keybind.Macro.ArgumentsTaken;
                }
                catch (NotImplementedException)
                {
                    argstaken = "Arguments taken were not provided";
                }
            }
            else
            {
                MacroComboBox.SelectedIndex = 0;
            }

            MacroDescriptionLabel.Text = description ?? "N/A";

            MacroArgsTakenLabel.Text = argstaken ?? "None";

            MacroUpdateControlPositions(MacroArgsTakenLabel.Height);

            if (!Configuring)
            {
                MacroArgsListBox.Items.Clear();
            }
            else
            {
                MacroArgTextBox.Text = "";
            }
        }

        private void MacroArgsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveMacroArgButton.Enabled = MacroArgsListBox.SelectedIndex > -1;
        }

        private void MacroArgTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckCanAddMacroArg();
        }

        private void MacroArgTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckCanAddMacroArg();
        }

        private void MacroArgTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                AddMacroArgButton.PerformClick();
            }
        }

        private void CheckCanAddMacroArg()
        {
            AddMacroArgButton.Enabled = MacroComboBox.SelectedIndex > 0 && MacroArgTextBox.Text.Length > 0 && TryGetArg(MacroArgTextBox.Text, (ArgType)MacroArgTypeComboBox.SelectedIndex, out _);
        }

        private void AddMacroArgButton_Click(object sender, EventArgs e)
        {
            if (!AddMacroArgButton.Enabled) return;

            string str = MacroArgTextBox.Text;

            MacroArgTextBox.Text = "";

            MacroArgsListBox.Items.Add(GetArg(str, (ArgType)MacroArgTypeComboBox.SelectedIndex));

            MacroArgsListBox.SelectedIndex = MacroArgsListBox.Items.Count - 1;

            MacroArgTextBox.Focus();
        }

        private void RemoveMacroArgButton_Click(object sender, EventArgs e)
        {
            int index = MacroArgsListBox.SelectedIndex;
            MacroArgsListBox.Items.RemoveAt(index);

            MacroArgsListBox.SelectedIndex = MacroArgsListBox.Items.Count > index ? index : MacroArgsListBox.Items.Count - 1;
        }

        private void AllowOtherKeybindsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Keybind.AllowOtherKeybinds = AllowOtherKeybindsCheckBox.Checked;
        }

        private void AddEditButton_Click(object sender, EventArgs e)
        {
            Keybind.ConditionArgs = ConditionArgsListBox.Items.Cast<object>().ToArray();

            Keybind.MacroArgs = MacroArgsListBox.Items.Cast<object>().ToArray();
        }

        private object GetArg(string str, ArgType type)
        {
            object arg;

            switch (type)
            {
                case ArgType.Int:
                    arg = long.Parse(str);

                    break;
                default:
                    arg = str;

                    break;
            }

            return arg;
        }

        private bool TryGetArg(string str, ArgType type, out object arg)
        {
            try
            {
                arg = GetArg(str, type);

                return true;
            }
            catch
            {
            }

            arg = null;

            return false;
        }
    }
}