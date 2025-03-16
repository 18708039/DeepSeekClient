using DeepSeekClient.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace DeepSeekClient.Core
{
    internal class ConversationCore
    {
        private readonly InitializationCore _initial;
        private readonly JsonSerializerSettings _jsonSettings;

        public ConversationCore(InitializationCore initializationCore)
        {
            _initial = initializationCore;

            _jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        public async Task<List<MessageModel>> ConversationLoad(string charId)
        {
            await ConversationSaveAsync(charId);
            var chatfilePath = Path.Combine(_initial.ChatDir, charId + _initial.ChatFileExt);
            var jsonString = File.ReadAllText(chatfilePath);
            return JsonConvert.DeserializeObject<List<MessageModel>>(jsonString) ?? [];
        }

        public async Task ConversationSaveAsync(MessageModel[] newConversation, string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId + _initial.ChatFileExt);
            var jsonString = JsonConvert.SerializeObject(newConversation, Formatting.Indented, _jsonSettings);
            await File.WriteAllTextAsync(chatfilePath, jsonString);
        }

        public async Task ConversationSaveAsync(string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId + _initial.ChatFileExt);

            if (!File.Exists(chatfilePath))
            {
                await File.WriteAllTextAsync(chatfilePath, "[]");
            }
        }

        public async Task ConversactionClearAsync(string charId)
        {
            ConversactionRemove(charId);
            await ConversationSaveAsync(charId);
        }

        public void ConversactionRemove(string charId)
        {
            var chatfilePath = Path.Combine(_initial.ChatDir, charId + _initial.ChatFileExt);
            if (File.Exists(chatfilePath)) { File.Delete(chatfilePath); }
        }
    }
}