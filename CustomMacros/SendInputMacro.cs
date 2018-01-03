using MultiDeviceKeybinds;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CustomMacros
{
    public class SendInputMacro : IMacro
    {
        public string Name { get { return "Send Input"; } }
        public string Description { get { return "Sends input to the foreground window"; } }
        public string ArgumentsTaken { get { return "(string) Keyname [delay next in ms] [down/up].\r\nI.E. \"Enter 50 down\" \"W 50\" \"S\""; } }

        const int DefaultDelay = 10;

        string[] keynames;
        Keys[] keys;

        public SendInputMacro()
        {
            keynames = Enum.GetNames(typeof(Keys));

            keys = (Keys[])Enum.GetValues(typeof(Keys));
        }

        public bool Perform(KeybindDevice device, Keys pressedKey, Keys correctedPressedKey, KeyState state, KeyState lastState, string guid, params object[] args)
        {
            if (args.Length == 0) return false;

            List<Tuple<int, KeyState, int>> inputs = new List<Tuple<int, KeyState, int>>();

            foreach (object arg in args)
            {
                if (!(arg is string str)) continue;

                string[] key = str.Trim().Split(' ');

                int index = Array.FindIndex(keynames, k => k.Length == key[0].Length && k.Equals(key[0], StringComparison.InvariantCultureIgnoreCase));

                int delay = DefaultDelay;

                if (key.Length > 1)
                {
                    int keystateindex = 1;

                    if (key.Length > 1)
                    {
                        if (!int.TryParse(key[1], out delay))
                        {
                            delay = DefaultDelay;
                        }
                        else
                        {
                            keystateindex++;
                        }
                    }

                    if (key.Length > keystateindex)
                    {
                        inputs.Add(new Tuple<int, KeyState, int>(index, key[keystateindex].Length == 4 && (key[keystateindex].Equals("make", StringComparison.InvariantCultureIgnoreCase) || key[keystateindex].Equals("down", StringComparison.InvariantCultureIgnoreCase)) ? KeyState.Make : KeyState.Break, delay));

                        continue;
                    }
                }

                inputs.Add(new Tuple<int, KeyState, int>(index, KeyState.Make, delay));
                inputs.Add(new Tuple<int, KeyState, int>(index, KeyState.Break, delay));
            }

            foreach (Tuple<int, KeyState, int> input in inputs)
            {
                InputSimulation.SetKeyState(keys[input.Item1], input.Item2);

                InputSimulation.Sleep(input.Item3);
            }

            return true;
        }
    }
}