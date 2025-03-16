using DeepSeekClient.Core;
using DeepSeekClient.Events;
using DeepSeekClient.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;

namespace DeepSeekClient.ViewModels
{
    internal class SideBarViewModel : BindableBase
    {
        private readonly IDialogService _dialog;
        private readonly IEventAggregator _event;
        private readonly CharacterCore _charCore;

        private bool _isVisible;
        private int _currentCharIndex;
        private ObservableCollection<CharacterModel> _characters;

        public SideBarViewModel(IDialogService dialogService, IEventAggregator eventAggregator, CharacterCore characterCore)
        {
            _dialog = dialogService;
            _charCore = characterCore;
            _event = eventAggregator;

            MenuSwitchCommand = new DelegateCommand(MenuSwitch);
            NewCharCommand = new DelegateCommand(NewCharDialog);
            SetParamCommand = new DelegateCommand(SetParamDialog);
            CardOptionCommand = new DelegateCommand<string>(CardOption);

            _characters = [];
            _characters.AddRange(_charCore.CharList);

            _isVisible = true;
            _currentCharIndex = 0;

            _event.GetEvent<CharacterChangedEvent>().Subscribe(CharacterChangedHandle);
        }

        private void CharacterChangedHandle(CharacterModel newChar)
        {
            var targetIndex = _characters.ToList().FindIndex(c => c.CharId == newChar.CharId);

            if (targetIndex >= 0)
            {
                Characters.RemoveAt(targetIndex);
                Characters.Insert(0, newChar);
            }
            else
            {
                Characters.Insert(0, newChar);
            }

            CurrentCharIndex = 0;
        }

        private void CardOption(string charId)
        {
            var parameters = new DialogParameters
            {
                {"Data",charId }
            };

            _dialog.ShowDialog("CharacterView", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var tmpIndex = _currentCharIndex;
                    _characters.Clear();
                    Characters.AddRange(_charCore.CharList);
                    CurrentCharIndex = tmpIndex;
                }

                if (result.Result == ButtonResult.Abort)
                {
                    var charIndex = _characters.ToList().FindIndex(c => c.CharId == charId);
                    var tmpIndex = _currentCharIndex;

                    if (charIndex >= 0)
                    {
                        _characters.Clear();
                        Characters.AddRange(_charCore.CharList);
                        if (charIndex == tmpIndex) { CurrentCharIndex = 0; }
                        else if (charIndex > tmpIndex) { CurrentCharIndex = tmpIndex; }
                        else { CurrentCharIndex = tmpIndex - 1; }
                    }
                }
            });
        }

        private void SetParamDialog()
        {
            _dialog.ShowDialog("ConfigurationView");
        }

        private void NewCharDialog()
        {
            _dialog.ShowDialog("CharacterView", result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    _characters.Clear();
                    Characters.AddRange(_charCore.CharList);
                    CurrentCharIndex = 0;
                }
            });
        }

        private void MenuSwitch()
        {
            IsVisible = !IsVisible;
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        public int CurrentCharIndex
        {
            get { return _currentCharIndex; }
            set
            {
                if (SetProperty(ref _currentCharIndex, value) && value >= 0)
                {
                    var charId = _characters[_currentCharIndex].CharId;
                    _event.GetEvent<ConversationChangedEvent>().Publish(charId);
                    _event.GetEvent<DoFocusEvent>().Publish();
                }
            }
        }

        public ObservableCollection<CharacterModel> Characters
        {
            get { return _characters; }
            set { SetProperty(ref _characters, value); }
        }

        public DelegateCommand MenuSwitchCommand { get; private set; }
        public DelegateCommand NewCharCommand { get; private set; }
        public DelegateCommand SetParamCommand { get; private set; }
        public DelegateCommand<string> CardOptionCommand { get; private set; }
    }
}