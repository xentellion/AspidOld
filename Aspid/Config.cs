using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices;

namespace Aspid
{
    public struct BotConfig
    {
        public string token;

        public string prefix;

        public int deadPeople;
    }

    public class Hero
    {
        public string name { get; set; }

        public ulong owner { get; set; }

        public string image { get; set; }

        public string level { get; set; }

        public string bio { get; set; }

        public string inv { get; set; }

        public string intellect { get; set; }

        public string magic { get; set; }

        public string nature { get; set; }
    }

    class Config
    {
        static string LinuxFolder { get; } = "/home/xentellion/Aspid/Data";
        static string WindowsFolder { get; } = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)}/Xentellion/Aspid";

        internal static string configPath;

        const string configFile = "Load.json";
        internal const string mydb = "AspidDataBase.db";

        static internal string connectionString;

        internal static BotConfig bot;

        
        static Config()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                configPath = LinuxFolder;
            else
                configPath = WindowsFolder;

            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);

            configPath += "/";

            if (!File.Exists(configPath + configFile))
            {
                bot = new BotConfig();
                string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
                File.WriteAllText(configPath + configFile, json);
            }
            else
            {
                string json = File.ReadAllText(configPath + configFile);
                bot = JsonConvert.DeserializeObject<BotConfig>(json);
            }

            if(!File.Exists(configPath + mydb))
                File.Create(configPath + mydb);

            connectionString = "Filename = " + configPath + mydb;
        }

        public static void SaveDead()
        {
            string json = JsonConvert.SerializeObject(bot, Formatting.Indented);
            File.WriteAllText(configPath + configFile, json);
        }
    }
}
