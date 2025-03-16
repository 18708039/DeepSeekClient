using DeepSeekClient.Core;
using DeepSeekClient.Events;
using DeepSeekClient.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Windows;

namespace DeepSeekClient.ViewModels
{
    internal class ConfigurationViewModel : BindableBase, IDialogAware
    {
        private readonly IEventAggregator _event;
        private readonly ConfigurationCore _configCore;

        private string _languageSet;
        private string _themeSet;
        private string _apiUri;
        private string _apiKey;
        private int _languageIndex;
        private int _themeIndex;

        public ConfigurationViewModel(IEventAggregator eventAggregator, ConfigurationCore configurationCore)
        {
            _event = eventAggregator;
            _configCore = configurationCore;
            _configCore.ConfigLoadAsync().Await();

            _languageSet = _configCore.Config.ConfigLanguage;
            _themeSet = _configCore.Config.ConfigTheme;
            _apiUri = _configCore.Config.ConfigUri;
            _apiKey = _configCore.Config.ConfigKey;

            _languageIndex = 0;
            _themeIndex = 0;

            LanguageIndexConvert();
            ThemeIndexConvert();

            CancelCommand = new DelegateCommand(CancelClose);
            SaveCommand = new DelegateCommand(async () => await SaveClose());
        }

        private async Task SaveClose()
        {
            if (string.IsNullOrEmpty(_apiUri) || string.IsNullOrEmpty(_apiKey))
            {
                MessageBox.Show("Pls fill in the Uri and Key.", "Check!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _configCore.Config.ConfigLanguage = _languageSet;
            _configCore.Config.ConfigTheme = _themeSet;
            _configCore.Config.ConfigUri = _apiUri;
            _configCore.Config.ConfigKey = _apiKey;

            await _configCore.ConfigSaveAsync();
            _event.GetEvent<ConfigurationChangedEvent>().Publish();

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void CancelClose()
        {
            if (_languageSet != _configCore.Config.ConfigLanguage) { _event.GetEvent<LanguageChangedEvent>().Publish(_configCore.Config.ConfigLanguage); }
            if (_themeSet != _configCore.Config.ConfigTheme) { _event.GetEvent<ThemeChangedEvent>().Publish(_configCore.Config.ConfigTheme); }
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private void LanguageIndexConvert()
        {
            LanguageIndex = _languageSet switch
            {
                "zh_CN" => 0,
                "en" => 1,
                _ => 0,
            };
        }

        private void ThemeIndexConvert()
        {
            ThemeIndex = _themeSet switch
            {
                "Dark" => 0,
                "Light" => 1,
                _ => 0,
            };
        }

        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

        public int LanguageIndex
        {
            get { return _languageIndex; }
            set
            {
                if (SetProperty(ref _languageIndex, value))
                {
                    _languageSet = value switch
                    {
                        0 => "zh_CN",
                        1 => "en",
                        _ => "zh_CN",
                    };
                    _event.GetEvent<LanguageChangedEvent>().Publish(_languageSet);
                }
            }
        }

        public int ThemeIndex
        {
            get { return _themeIndex; }
            set
            {
                if (SetProperty(ref _themeIndex, value))
                {
                    _themeSet = value switch
                    {
                        0 => "Dark",
                        1 => "Light",
                        _ => "Dark",
                    };
                    _event.GetEvent<ThemeChangedEvent>().Publish(_themeSet);
                }
            }
        }

        public string ApiUri
        {
            get { return _apiUri; }
            set { SetProperty(ref _apiUri, value); }
        }

        public string ApiKey
        {
            get { return _apiKey; }
            set { SetProperty(ref _apiKey, value); }
        }

        public string Title => string.Empty;

        public event Action<IDialogResult>? RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}