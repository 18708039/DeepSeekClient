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

        public CharacterCore(InitializationCore initializationCore)
        {
            _initial = initializationCore;

            CharList = [];

            CharacterListLoad();
        }

        public void CharacterListLoad()
        {
            CharList.Clear();

            if (!Directory.Exists(_initial.ChatDir))
            {
                Directory.CreateDirectory(_initial.ChatDir);
                CharacterSave(new CharacterModel(_initial.CharacterId) { CharName = "default" });
                return;
            }

            DirectoryInfo directory = new(_initial.ChatDir);
            FileInfo[] files = [.. directory.GetFiles("*" + _initial.CharFileExt)];

            foreach (var file in files)
            {
                var charfilePath = file.FullName;
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
                CharacterSave(new CharacterModel(_initial.CharacterId) { CharName = "default" });
                return;
            }

            if (CharList.Count > 1)
            {
                CharList = [.. CharList.OrderByDescending(c => c.LastStamp)];
            }
        }

        public CharacterModel CharacterLoad(string charId)
        {
            var charfilePath = Path.Combine(_initial.ChatDir, charId + _initial.CharFileExt);
            var jsonString = File.ReadAllText(charfilePath);
            return JsonConvert.DeserializeObject<CharacterModel>(jsonString) ?? throw new Exception(charfilePath + " loading err.");
        }

        public CharacterModel CharacterLoad()
        {
            return new CharacterModel(_initial.CharacterId);
        }

        public void CharacterSave(CharacterModel newChar)
        {
            var charfilePath = Path.Combine(_initial.ChatDir, newChar.CharId + _initial.CharFileExt);

            var jsonString = JsonConvert.SerializeObject(newChar, Formatting.Indented);

            File.WriteAllText(charfilePath, jsonString);

            int charIndex = CharList.FindIndex(c => c.CharId == newChar.CharId);

            if (charIndex >= 0)
            {
                CharList[charIndex] = newChar;
            }
            else
            {
                CharList.Insert(0, newChar);
            }
        }

        public void CharacterRemove(string charId)
        {
            var charfilePath = Path.Combine(_initial.ChatDir, charId + _initial.CharFileExt);

            if (File.Exists(charfilePath)) { File.Delete(charfilePath); }

            int charIndex = CharList.FindIndex(c => c.CharId == charId);

            if (charIndex >= 0) { CharList.RemoveAt(charIndex); }

            if (CharList.Count == 0) { CharacterListLoad(); }
        }
    }
}