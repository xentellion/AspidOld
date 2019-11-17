using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;

namespace Aspid
{
    class Config
    {
        private const string LinuxPrefix = "/home/xentellion/Aspid1/Data/";
        private static string configFolder = "C:/Data";
        private const string configFile = "Load.json";
        private const string mydb = "AspidDataBase.db";

        public static BotConfig bot;

        static Config()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                configFolder = LinuxPrefix;

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

            if(!File.Exists(configFolder + "/" + mydb))
            {
                File.Create(configFolder + "/" + mydb);
            }
        }
    }

    public struct BotConfig
    {
        public string token;

        public string prefix;

        public int deadPeople;
    }
}
