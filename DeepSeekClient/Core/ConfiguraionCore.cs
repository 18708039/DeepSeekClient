using Prism.Ioc;
using System.IO;

namespace DeepSeekClient.Core
{
    internal class ConfiguraionCore
    {
        private InitializationCore _init;

        public ConfiguraionCore(InitializationCore initializationCore)
        {
            _init = initializationCore;
        }

        public void ConfigLoad()
        {
            if (!File.Exists(_init.ConfigFile))
            {
                return;
            }
        }

        public void ConfigSave()
        { }
    }
}