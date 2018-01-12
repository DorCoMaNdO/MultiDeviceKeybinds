using Newtonsoft.Json;
using System.IO;

namespace MultiDeviceKeybinds
{
    enum InputInterceptionMode
    {
        Hook = 0,
        Interception = 1,
    }

    class Settings
    {
        public bool ShowConsole { get; set; } = false;
        public InputInterceptionMode InputInterceptionMode { get; set; } = InputInterceptionMode.Hook;

        public static Settings Load()
        {
            string file = Path.Combine(Program.Location, "settings.json");

            Settings settings = File.Exists(file) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(file)) : new Settings();

            return settings;
        }

        public void Save()
        {
            string file = Path.Combine(Program.Location, "settings.json");

            File.WriteAllText(Path.Combine(Program.Location, "settings.json"), JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}
