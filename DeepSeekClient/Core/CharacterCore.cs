using DeepSeekClient.Models;
using Prism.Ioc;
using System.IO;

namespace DeepSeekClient.Core
{
    internal class CharacterCore
    {
        public void ConversationLoad()
        {
        }

        public void ConversationSave()
        {
        }

        public void CharacterListLoad()
        {
            if (!Directory.Exists())
            {
                return;
            }
        }

        public void CharacterLoad()
        { }

        public void CharacterSave()
        { }
    }
}