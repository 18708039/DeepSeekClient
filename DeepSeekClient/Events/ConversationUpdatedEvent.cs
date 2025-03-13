using DeepSeekClient.Models;
using Prism.Events;

namespace DeepSeekClient.Events
{
    internal class ConversationUpdatedEvent : PubSubEvent<(MessageModel Message, string CharId)>
    {
    }
}