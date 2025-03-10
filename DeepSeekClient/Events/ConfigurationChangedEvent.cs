using DeepSeekClient.Models;
using Prism.Events;

namespace DeepSeekClient.Events
{
    internal class ConfigurationChangedEvent : PubSubEvent<ConfigurationModel>
    {
    }
}