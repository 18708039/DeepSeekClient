using DeepSeekClient.Core;
using DeepSeekClient.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Configuration;
using System.Data;
using System.Windows;

namespace DeepSeekClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<InitializationCore>();
        containerRegistry.RegisterSingleton<ConfiguraionCore>();
        containerRegistry.RegisterSingleton<CharacterCore>();
        containerRegistry.RegisterSingleton<ConversationCore>();
    }
}