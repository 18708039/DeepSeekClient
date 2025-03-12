using DeepSeekClient.Models;
using Newtonsoft.Json;
using System.IO;

namespace DeepSeekClient.Core
{
    internal class ConversationCore
    {
        private readonly InitializationCore _initial;
        private JsonSerializerSettings _jsonSettings;
        public ConversationModel Conversation { get; private set; }

        public ConversationCore(InitializationCore initializationCore)
        {
            _initial = initializationCore;

            _jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            Conversation = new ConversationModel();
        }

        public void ConversationLoad(string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId + _initial.ChatFileExt);
            Conversation = JsonConvert.DeserializeObject<ConversationModel>(chatfilePath) ?? new ConversationModel();
        }

        public void ConversationSave(ConversationModel newConversation, string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId + _initial.ChatFileExt);
            var jsonString = JsonConvert.SerializeObject(newConversation, Formatting.Indented, _jsonSettings);
            File.WriteAllText(chatfilePath, jsonString);
        }

        public void ConversationSave(string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId + _initial.ChatFileExt);
            if (!File.Exists(chatfilePath)) { File.WriteAllText(chatfilePath, "[]"); }
        }

        public void ConversactionClear(string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId + _initial.ChatFileExt);
            File.WriteAllText(chatfilePath, "[]");
        }

        public void ConversactionRemove(string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId + _initial.ChatFileExt);
            if (File.Exists(chatfilePath)) { File.Delete(chatfilePath); }
        }
    }
}