using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;

namespace Aspid
{
    class Config
    {
        private const string LinuxPrefix = "/home/xent/Aspid/";
        private static string configFolder = "Resources";
        private const string configFile = "Load.json";

        public static BotConfig bot;

        static Config()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                configFolder = LinuxPrefix + configFolder;

            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            if (!File.Exists(configFolder + "/" + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configFolder + "/" + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configFolder + "/" + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }
        }
    }

    public struct BotConfig
    {
        public string token;

        public string prefix;

        public ulong mainGuild;

        public int deadPeople;

        public int connectionString;
    }
}
