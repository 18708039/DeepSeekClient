﻿using DeepSeekClient.Core;
using DeepSeekClient.Events;
using DeepSeekClient.Models;
using DeepSeekClient.Services;
using DeepSeekClient.Views;
using Microsoft.VisualBasic;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DeepSeekClient.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly IEventAggregator _event;
        private readonly IContainerProvider _container;
        private DeepSeekService _apiService;

        private readonly ConfigurationCore _configCore;
        private readonly CharacterCore _charCore;
        private readonly ConversationCore _converCore;

        private ConfigurationModel _currentConfig;
        private CharacterModel _currentChar;
        private ObservableCollection<MessageModel> _currentConversation;
        private CancellationTokenSource _cancelTokenSource;

        private string _currentCharName;
        private bool _allowInput;
        private bool _isR1Checked;
        private string _inputMessage;
        private string _stopTag;

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IContainerProvider containerProvider)
        {
            ClearChatCommand = new DelegateCommand(ClearChat);
            SendMessageAsyncCommand = new DelegateCommand(async () => await SendMessageAsync());

            _event = eventAggregator;
            _container = containerProvider;
            _configCore = _container.Resolve<ConfigurationCore>();
            _charCore = _container.Resolve<CharacterCore>();
            _converCore = _container.Resolve<ConversationCore>();

            Initialize();
            SubscribeToEvents();

            regionManager.RegisterViewWithRegion("SideBarRegion", typeof(SideBarView));
        }

        private void Initialize()
        {
            _currentConfig = _configCore.Config;
            _currentChar = _charCore.CharList[0];
            _currentConversation = [.. _converCore.ConversationLoad(_currentChar.CharId)];
            _currentCharName = _currentChar.CharName;
            _allowInput = true;
            _isR1Checked = false;
            _inputMessage = string.Empty;
            _stopTag = string.Empty;
            _cancelTokenSource = new CancellationTokenSource();

            ThemeChangedHandle(_configCore.Config.ConfigTheme);
            LanguageChangedHandle(_configCore.Config.ConfigLanguage);
        }

        private void SubscribeToEvents()
        {
            _event.GetEvent<LanguageChangedEvent>().Subscribe(LanguageChangedHandle);
            _event.GetEvent<ThemeChangedEvent>().Subscribe(ThemeChangedHandle);
            _event.GetEvent<ConfigurationChangedEvent>().Subscribe(ConfigurationChangedHandle);
            _event.GetEvent<ConversationChangedEvent>().Subscribe(ConversationChangedHandle);
            _event.GetEvent<ConversationUpdatedEvent>().Subscribe(ConversationUpdatedHandle, ThreadOption.UIThread);
        }

        private void ConfigurationChangedHandle()
        {
            _currentConfig = _configCore.Config;
        }

        private async Task SendMessageAsync()
        {
            try
            {
                if (_stopTag == "Stop")
                {
                    _cancelTokenSource.Cancel();
                    return;
                }

                if (!ValidateConfiguration())
                {
                    MessageBox.Show("Pls fill in the Uri and Key.", "CHECK", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_inputMessage)) { return; }

                AllowInput = false;
                StopTag = "Stop";
                _currentChar.CharModel = _isR1Checked ? "deepseek-reasoner" : "deepseek-chat";
                _apiService = _container.Resolve<DeepSeekService>();

                await _apiService.SendRequestAsync(_inputMessage, _currentConfig, _currentChar, _cancelTokenSource.Token);

                InputMessage = string.Empty;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show($"error: {ex.Message}", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                StopTag = string.Empty;
                AllowInput = true;

                _apiService?.Dispose();
                _cancelTokenSource?.Dispose();
                _cancelTokenSource = new CancellationTokenSource();

                _event.GetEvent<DoFocusEvent>().Publish();
            }
        }

        private bool ValidateConfiguration()
        {
            return !string.IsNullOrEmpty(_currentConfig.ConfigUri) && !string.IsNullOrEmpty(_currentConfig.ConfigKey);
        }

        private void ClearChat()
        {
            _converCore.ConversactionClear(_currentChar.CharId);
            CurrentConversation.Clear();
        }

        private void ThemeChangedHandle(string setTheme)
        {
            string dictPath = $"/Resources/Themes/{setTheme}Theme.xaml";
            var themeDict = new ResourceDictionary
            {
                Source = new Uri(dictPath, UriKind.Relative)
            };

            Application.Current.Resources.MergedDictionaries.Add(themeDict);
        }

        private void LanguageChangedHandle(string setLanguage)
        {
            string dictPath = $"/Resources/Languages/Strings_{setLanguage}.xaml";
            var languagesDict = new ResourceDictionary
            {
                Source = new Uri(dictPath, UriKind.Relative)
            };

            Application.Current.Resources.MergedDictionaries.Add(languagesDict);
        }

        private void ConversationUpdatedHandle((MessageModel Message, string CharId) parameters)
        {
            if (parameters.CharId == _currentChar.CharId)
            {
                if (parameters.Message.role == "stop_marker" ||
                    (string.IsNullOrEmpty(parameters.Message.content) && string.IsNullOrEmpty(parameters.Message.reasoning_content))) { return; }

                var lastIndex = _currentConversation.Count - 1;

                if (lastIndex < 0 || _currentConversation[lastIndex].role != parameters.Message.role)
                {
                    CurrentConversation.Add(parameters.Message); return;
                }

                var oldMessage = _currentConversation[lastIndex];
                if (!string.IsNullOrEmpty(parameters.Message.content))
                {
                    oldMessage.content += parameters.Message.content;
                }
                if (!string.IsNullOrEmpty(parameters.Message.reasoning_content))
                {
                    oldMessage.reasoning_content += parameters.Message.reasoning_content;
                }
                CurrentConversation.RemoveAt(lastIndex);
                CurrentConversation.Add(oldMessage);
            }
        }

        private void ConversationChangedHandle(string charId)
        {
            _currentChar = _charCore.CharacterLoad(charId);
            CurrentCharName = _currentChar.CharName;
            var messages = _converCore.ConversationLoad(charId);
            CurrentConversation = [.. messages];
            _event.GetEvent<CollectionChangedEvent>().Publish();
        }

        public bool AllowInput
        {
            get { return _allowInput; }
            set { SetProperty(ref _allowInput, value); }
        }

        public string StopTag
        {
            get { return _stopTag; }
            set { SetProperty(ref _stopTag, value); }
        }

        public string InputMessage
        {
            get { return _inputMessage; }
            set { SetProperty(ref _inputMessage, value); }
        }

        public bool IsR1Checked
        {
            get { return _isR1Checked; }
            set { SetProperty(ref _isR1Checked, value); }
        }

        public string CurrentCharName
        {
            get { return _currentCharName; }
            set { SetProperty(ref _currentCharName, value); }
        }

        public ObservableCollection<MessageModel> CurrentConversation
        {
            get { return _currentConversation; }
            set { SetProperty(ref _currentConversation, value); }
        }

        public DelegateCommand ClearChatCommand { get; private set; }
        public DelegateCommand SendMessageAsyncCommand { get; private set; }
    }
}