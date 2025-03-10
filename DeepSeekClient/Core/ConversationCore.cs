using DeepSeekClient.Events;
using DeepSeekClient.Models;
using Newtonsoft.Json;
using Prism.Events;
using System.IO;

namespace DeepSeekClient.Core
{
    internal class ConversationCore
    {
        private readonly InitializationCore _initial;
        public ConversationModel Conversation { get; private set; }

        public ConversationCore(InitializationCore initializationCore)
        {
            _initial = initializationCore;
            Conversation = new ConversationModel();
        }

        public void ConversationLoad(string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId, _initial.ChatFileExt);
            Conversation = JsonConvert.DeserializeObject<ConversationModel>(chatfilePath) ?? new ConversationModel();
        }

        public void ConversationSave(ConversationModel newConversation, string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId, _initial.ChatFileExt);
            var jsonString = JsonConvert.SerializeObject(newConversation, Formatting.Indented);
            File.WriteAllText(chatfilePath, jsonString);
        }
    }
}