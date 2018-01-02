using MultiKeyboardHook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace MultiDeviceKeybinds
{
    class Keybind
    {
        //public const string KeysSeparator = ",";
        public const string KeysSeparator = "+";

        public string Name { get; internal set; }
        public bool Enabled { get; internal set; }
        public List<Keys> Keys { get; private set; } = new List<Keys>();
        public bool ActivateOnKeyDown { get; internal set; } = true;
        public bool ActivateOnHold { get; internal set; } = false;
        public bool ActivateOnKeyUp { get; internal set; } = false;
        public bool ActivateIfMoreKeysPressed { get; internal set; } = false;
        [JsonIgnore]
        public ICondition Condition { get; internal set; }
        [JsonProperty("Condition")]
        internal string ConditionTypeName { get { return Condition == null ? null : Program.MainForm.Conditions.ElementAt(Array.IndexOf(Program.MainForm.Conditions.Values.ToArray(), Condition)).Key; } }
        public object[] ConditionArgs { get; internal set; }
        [JsonIgnore]
        public IMacro Macro { get; internal set; }
        [JsonProperty("Macro")]
        internal string MacroTypeName { get { return Macro == null ? null : Program.MainForm.Macros.ElementAt(Array.IndexOf(Program.MainForm.Macros.Values.ToArray(), Macro)).Key; } }
        public object[] MacroArgs { get; internal set; }
        public bool AllowOtherKeybinds { get; internal set; } = true;

        [JsonIgnore]
        public KeybindDevice Device { get; internal set; }
        [JsonIgnore]
        public string GUID { get; private set; } = Guid.NewGuid().ToString();

        internal Keybind()
        {
        }

        internal Keybind(string name, List<Keys> keys, ICondition condition, object[] conditionArgs, IMacro macro, object[] macroArgs)
        {
            Name = name;

            Keys = keys ?? new List<Keys>();

            Condition = condition;
            ConditionArgs = conditionArgs;

            Macro = macro;
            MacroArgs = macroArgs;
        }

        [JsonConstructor]
        internal Keybind(string name, bool enabled, string conditionTypeName, object[] conditionArgs, string macroTypeName, object[] macroArgs, bool activateIfMoreKeysPressed, bool allowOtherKeybinds, bool activateOnKeyDown, bool activateOnHold, bool activateOnKeyUp)
        {
            Name = name;

            Enabled = enabled;

            if (conditionTypeName != null && Program.MainForm.Conditions.ContainsKey(conditionTypeName))
            {
                Condition = Program.MainForm.Conditions[conditionTypeName];

                ConditionArgs = conditionArgs;
            }
            else if (!string.IsNullOrEmpty(conditionTypeName))
            {
                if (MessageBox.Show($"The condition type \"{conditionTypeName}\" was not found.\r\n\r\n" +
                    $"It is used by the keybind \"{Name}\", continuing may result in loss of data for this keybind.", "Condition type missing", MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                {
                    Environment.Exit(0);
                }
            }

            if (macroTypeName != null && Program.MainForm.Macros.ContainsKey(macroTypeName))
            {
                Macro = Program.MainForm.Macros[macroTypeName];

                MacroArgs = macroArgs;
            }
            else if (!string.IsNullOrEmpty(macroTypeName))
            {
                if (MessageBox.Show($"The macro type \"{macroTypeName}\" was not found.\r\n\r\n" +
                    $"It is used by the keybind \"{Name}\", continuing may result in loss of data for this keybind.", "Macro type missing", MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                {
                    Environment.Exit(0);
                }
            }

            ActivateIfMoreKeysPressed = activateIfMoreKeysPressed;
            AllowOtherKeybinds = allowOtherKeybinds;

            ActivateOnKeyDown = activateOnKeyDown;
            ActivateOnHold = activateOnHold;
            ActivateOnKeyUp = activateOnKeyUp;
        }

        public bool TestCondition(KeybindDevice device, Keys key, Keys correctedKey, KeyState state, KeyState lastState)
        {
            try
            {
                return (state == KeyState.Make && lastState != KeyState.Make && ActivateOnKeyDown || state == KeyState.Make && lastState == KeyState.Make && ActivateOnHold || state == KeyState.Break && ActivateOnKeyUp) && (Condition == null || Condition.Test(device, key, correctedKey, state, lastState, GUID, ConditionArgs ?? new object[0]));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error executing ICondition type \"{ConditionTypeName}\":\r\n{e}");
                SystemSounds.Exclamation.Play();

                return false;
            }
        }

        public bool PerformMacro(KeybindDevice device, Keys key, Keys correctedKey, KeyState state, KeyState lastState, bool testCondition = false)
        {
            try
            {
                if (testCondition && !TestCondition(device, key, correctedKey, state, lastState)) return false;

                return Macro == null || Macro.Perform(device, key, correctedKey, state, lastState, GUID, MacroArgs ?? new object[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error executing IMacro type \"{MacroTypeName}\":\r\n{e}");
                SystemSounds.Exclamation.Play();

                return false;
            }
        }
    }
}