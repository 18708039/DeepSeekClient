using Prism.Ioc;
using DeepSeekClient.Models;
using System.IO;
using Prism.Events;
using Newtonsoft.Json;
using DeepSeekClient.Events;
using System.Diagnostics;

namespace DeepSeekClient.Core
{
    internal class ConfigurationCore
    {
        private readonly InitializationCore _initial;

        public ConfigurationModel Config { get; private set; }

        public ConfigurationCore(InitializationCore initializationCore)
        {
            _initial = initializationCore;
            Config = new ConfigurationModel();
            ConfigLoadAsync().Await();
        }

        public async Task ConfigLoadAsync()
        {
            if (!File.Exists(_initial.ConfigFile))
            {
                await ConfigSaveAsync();
                return;
            }

            var jsonString = File.ReadAllText(_initial.ConfigFile);
            Config = JsonConvert.DeserializeObject<ConfigurationModel>(jsonString) ?? new ConfigurationModel();
        }

        public async Task ConfigSaveAsync()
        {
            var jsonString = JsonConvert.SerializeObject(Config, Formatting.Indented);
            await File.WriteAllTextAsync(_initial.ConfigFile, jsonString);
        }
    }
}