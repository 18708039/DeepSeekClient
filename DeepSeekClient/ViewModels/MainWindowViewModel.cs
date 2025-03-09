using DeepSeekClient.Models;
using Prism.Ioc;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace DeepSeekClient.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        public string CurrentCharater { get; set; } = string.Empty;

        public string CurrentLanguage { get; set; } = "zh_CN";

        public string CurrentTheme { get; set; } = "Dark";

        public ObservableCollection<CharacterModel> Characters { get; set; } = [];

        public ObservableCollection<MessageModel> Conversation { get; set; } = [];

        public void SetLanguage()
        { }

        public void SetTheme()
        { }

        public void CharacterChangedHandle()
        { }

        public void ConversationChangedHandle()
        { }

        public void ConversationUpdatedHandle()
        { }
    }
}