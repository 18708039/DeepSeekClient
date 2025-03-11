using DeepSeekClient.Core;
using DeepSeekClient.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
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
            _event = eventAggregator;
            _charCore = characterCore;

            MenuSwitchCommand = new DelegateCommand(MenuSwitch);
            NewCharCommand = new DelegateCommand(NewCharDialog);
            SetParamCommand = new DelegateCommand(SetParamDialog);
            CardOptionCommand = new DelegateCommand<string>(CardOption);

            _characters = [];
            _characters.AddRange(_charCore.CharList);

            _isVisible = true;
            _currentCharIndex = 0;
        }

        private void CardOption(string CharId)
        {
            throw new NotImplementedException();
        }

        private void SetParamDialog()
        {
            throw new NotImplementedException();
        }

        private void NewCharDialog()
        {
            throw new NotImplementedException();
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
            set { SetProperty(ref _currentCharIndex, value); }
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