using DeepSeekClient.Models;
using Newtonsoft.Json;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekClient.Core
{
    internal class InitializationCore
    {
        public const string charFileExt = "_char.json";

        public const string chatFileExt = "_chat.json";
        public string ConfigFile { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");
        public string ChatDir { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Chats");
        public static string TimeStamp => DateTime.Now.ToString("g");
        public static string CharacterId => DateTime.Now.Ticks.ToString("X");
    }
}