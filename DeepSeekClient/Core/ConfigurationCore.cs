using Prism.Ioc;
using DeepSeekClient.Models;
using System.IO;
using Prism.Events;
using Newtonsoft.Json;
using DeepSeekClient.Events;

namespace DeepSeekClient.Core
{
    internal class ConfigurationCore
    {
        private readonly InitializationCore _initial;

        public ConfigurationModel Config { get; private set; }

        public ConfigurationCore(InitializationCore initializationCore, IEventAggregator eventAggregator)
        {
            _initial = initializationCore;
            eventAggregator.GetEvent<ConfigurationChangedEvent>().Subscribe(ConfigChangedHandle, ThreadOption.BackgroundThread);
            Config = new ConfigurationModel();
            ConfigLoad();
        }

        public void ConfigLoad()
        {
            if (!File.Exists(_initial.ConfigFile))
            {
                ConfigSave();
                return;
            }

            var jsonString = File.ReadAllText(_initial.ConfigFile);
            Config = JsonConvert.DeserializeObject<ConfigurationModel>(jsonString) ?? new ConfigurationModel();
        }

        public void ConfigSave()
        {
            var jsonString = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(_initial.ConfigFile, jsonString);
        }

        public void ConfigChangedHandle(ConfigurationModel newConfig)
        {
            Config = newConfig;
            ConfigSave();
        }
    }
}