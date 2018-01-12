using AudioSwitcher.AudioApi.CoreAudio;
using MultiDeviceKeybinds;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace CustomMacros
{
    static class Audio
    {
        public static CoreAudioController Controller = new CoreAudioController();
        public static SpeechSynthesizer Speech = new SpeechSynthesizer();

        static Audio()
        {
            Speech.Rate = 2;
        }
    }

    enum AudioDeviceAction
    {
        ToggleMute,
        Mute,
        Unmute,
        SetVolume
    }

    class PlaybackDeviceControlsMacro : IMacro
    {
        public string Name { get { return "Playback Device Controls"; } }
        public string Description { get { return "Mute/unmute or change the volume of the playback device assigned as default"; } }
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

            CoreAudioDevice playbackDevice = Audio.Controller.DefaultPlaybackDevice;

            Audio.Speech.SpeakAsyncCancelAll();

            if (action == AudioDeviceAction.ToggleMute || action == AudioDeviceAction.Mute || action == AudioDeviceAction.Unmute)
            {
                bool mute = action == AudioDeviceAction.ToggleMute ? !playbackDevice.IsMuted : action == AudioDeviceAction.Mute ? true : false;

                playbackDevice.Mute(mute);
            }
            else if (action == AudioDeviceAction.SetVolume)
            {
                if (args.Length < 2 || !(args[1] is long volume) || volume < 0 || volume > 100) return false;

                playbackDevice.Volume = volume;

                Audio.Speech.SpeakAsync($"Playback volume set to {volume}");
            }

            return true;
        }
    }
}
