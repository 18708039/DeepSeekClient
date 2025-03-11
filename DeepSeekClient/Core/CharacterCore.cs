using DeepSeekClient.Events;
using DeepSeekClient.Models;
using Newtonsoft.Json;
using Prism.Events;
using Prism.Ioc;
using System.IO;

namespace DeepSeekClient.Core
{
    internal class CharacterCore
    {
        private readonly InitializationCore _initial;
        public List<CharacterModel> CharList { get; private set; }

        public CharacterCore(InitializationCore initializationCore, IEventAggregator eventAggregator)
        {
            _initial = initializationCore;
            eventAggregator.GetEvent<CharacterChangedEvent>().Subscribe(CharChangedHandle, ThreadOption.BackgroundThread);

            CharList = [];

            CharacterListLoad();
        }

        public void CharacterListLoad()
        {
            if (!Directory.Exists(_initial.ChatDir))
            {
                Directory.CreateDirectory(_initial.ChatDir);
                var newChar = new CharacterModel(InitializationCore.CharacterId);
                CharacterSave(newChar);
                return;
            }

            DirectoryInfo directory = new(_initial.ChatDir);
            FileInfo[] files = [.. directory.GetFiles("*" + _initial.ChatFileExt).OrderByDescending(f => f.LastWriteTime)];

            foreach (var file in files)
            {
                var chatfilePath = file.FullName;
                var charfilePath = chatfilePath.Replace(_initial.ChatFileExt, _initial.CharFileExt);

                var jsonString = File.ReadAllText(charfilePath);
                var character = JsonConvert.DeserializeObject<CharacterModel>(jsonString);

                if (string.IsNullOrEmpty(character?.CharId))
                {
                    continue;
                }

                CharList.Add(character);
            }

            if (CharList.Count == 0)
            {
                var newChar = new CharacterModel(InitializationCore.CharacterId);
                CharacterSave(newChar);
                CharList.Add(newChar);
            }
        }

        public CharacterModel CharacterLoad(string charId)
        {
            var charfilePath = Path.Combine(_initial.ChatDir, charId + _initial.CharFileExt);
            var jsonString = File.ReadAllText(charfilePath);
            return JsonConvert.DeserializeObject<CharacterModel>(jsonString) ?? new CharacterModel(charId);
        }

        public void CharacterSave(CharacterModel newChar)
        {
            var charfilePath = Path.Combine(_initial.ChatDir, newChar.CharId + _initial.CharFileExt);
            var chatfilePath = charfilePath.Replace(_initial.CharFileExt, _initial.ChatFileExt);

            var jsonString = JsonConvert.SerializeObject(newChar, Formatting.Indented);

            File.WriteAllText(charfilePath, jsonString);

            if (!File.Exists(chatfilePath))
            {
                File.WriteAllText(chatfilePath, "[]");
                CharList.Insert(0, newChar);
            }
        }

        public void CharChangedHandle(CharacterModel newChar)
        {
            CharacterSave(newChar);
        }
    }
}