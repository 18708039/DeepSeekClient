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
        public string CharFileExt { get; } = "_char.json";
        public string ChatFileExt { get; } = "_chat.json";
        public string ConfigFile { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");
        public string ChatDir { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Chats");
        public string TimeStamp { get => DateTime.Now.ToString("g"); }
        public string CharacterId { get => DateTime.Now.Ticks.ToString("X"); }
    }
}