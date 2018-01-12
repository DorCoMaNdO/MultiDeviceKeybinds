using AudioSwitcher.AudioApi.CoreAudio;
using MultiDeviceKeybinds;
using System.Windows.Forms;

namespace CustomMacros
{
    class MicControlsMacro : IMacro
    {
        public string Name { get { return "Microphone Controls"; } }
        public string Description { get { return "Mute/unmute or change the volume of the microphone assigned as default"; } }
        public string ArgumentsTaken { get { return "[string] [toggle/un]mute/[set]volume\r\n[int] volumelevel0-100"; } }

        const string ToggleMute = "togglemute", Mute = "mute", Unmute = "unmute", SetVolume = "setvolume", Volume = "volume";

        public bool Perform(KeybindDevice device, Keys key, KeyState state, KeyState lastState, string guid, params object[] args)
        {
            AudioDeviceAction action = AudioDeviceAction.ToggleMute;

            if (args.Length > 0 && args[0] is string str)
            {
                if (str.Length == ToggleMute.Length && str.Equals(ToggleMute))
                {
                    action = AudioDeviceAction.ToggleMute;
                }
                else if (str.Length == Mute.Length && str.Equals(Mute))
                {
                    action = AudioDeviceAction.Mute;
                }
                else if (str.Length == Unmute.Length && str.Equals(Unmute))
                {
                    action = AudioDeviceAction.Unmute;
                }
                else if (str.Length == SetVolume.Length && str.Equals(SetVolume) || str.Length == Volume.Length && str.Equals(Volume))
                {
                    action = AudioDeviceAction.SetVolume;
                }
                else
                {
                    return false;
                }
            }

            CoreAudioDevice mic = Audio.Controller.DefaultCaptureDevice;
            
            Audio.Speech.SpeakAsyncCancelAll();

            if (action == AudioDeviceAction.ToggleMute || action == AudioDeviceAction.Mute || action == AudioDeviceAction.Unmute)
            {
                bool mute = action == AudioDeviceAction.ToggleMute ? !mic.IsMuted : action == AudioDeviceAction.Mute ? true : false;

                mic.Mute(mute);

                Audio.Speech.SpeakAsync($"Microphone {(mute ? "muted" : "activated")}");
                //Audio.Speech.SpeakAsync(mute ? "Muted" : "Activated");
            }
            else if (action == AudioDeviceAction.SetVolume)
            {
                if (args.Length < 2 || !(args[1] is long volume) || volume < 0 || volume > 100) return false;

                mic.Volume = volume;

                Audio.Speech.SpeakAsync($"Microphone volume set to {volume}");
            }

            return true;
        }
    }
}
