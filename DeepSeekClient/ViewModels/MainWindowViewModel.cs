using DeepSeekClient.Models;
using DeepSeekClient.Views;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace DeepSeekClient.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("SideBarRegion", typeof(SideBarView));
        }

        public string CurrentCharater { get; set; } = string.Empty;

        public string CurrentLanguage { get; set; } = "zh_CN";

        public string CurrentTheme { get; set; } = "Dark";

        public ObservableCollection<CharacterModel> Characters { get; set; } = [];

        public ObservableCollection<MessageModel> Conversation { get; set; } = [];

        public static void SetLanguage()
        { }

        public static void SetTheme()
        { }

        public static void CharacterChangedHandle()
        { }

        public static void ConversationChangedHandle()
        { }

        public static void ConversationUpdatedHandle()
        { }
    }
}