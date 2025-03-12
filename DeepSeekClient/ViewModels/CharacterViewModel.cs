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
    internal class CharacterViewModel : BindableBase, IDialogAware
    {
        private readonly CharacterCore _charCore;
        private readonly ConversationCore _convCore;

        private CharacterModel _characterModel;
        private string _charId;
        private string _charName;
        private string _charSet;
        private double _charTemperature;
        private string _displayTemperature;
        private bool _customApi;
        private string _charUri;
        private string _charKey;

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。已在OnDialogOpened中初始化

        public CharacterViewModel(CharacterCore characterCore, ConversationCore conversationCore)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。
        {
            _charCore = characterCore;
            _convCore = conversationCore;

            CancelCommand = new DelegateCommand(CancelClose);
            SaveCommand = new DelegateCommand(SaveClose);
            DeleteCommand = new DelegateCommand(DeleteClose);
        }

        private void DeleteClose()
        {
            var result = MessageBox.Show("The deletion cannot be restored!", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                _convCore.ConversactionRemove(_charId);
                _charCore.CharacterRemove(_charId);

                RequestClose?.Invoke(new DialogResult(ButtonResult.Abort));
            }
        }

        private void SaveClose()
        {
            _characterModel.CharName = _charName;
            _characterModel.CharTemperature = _charTemperature;
            _characterModel.CharSet = _charSet;
            _characterModel.CustomApi = _customApi;
            _characterModel.CharUri = _charUri;
            _characterModel.CharKey = _charKey;

            _charCore.CharacterSave(_characterModel);
            _convCore.ConversationSave(_charId);

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        private void CancelClose()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public string CharName
        {
            get { return _charName; }
            set { SetProperty(ref _charName, value); }
        }

        public string CharSet
        {
            get { return _charSet; }
            set { SetProperty(ref _charSet, value); }
        }

        public double CharTemperature
        {
            get { return _charTemperature; }
            set
            {
                if (SetProperty(ref _charTemperature, Math.Round(value, 1)))
                {
                    DisplayTemperature = _charTemperature.ToString("0.0");
                }
            }
        }

        public string DisplayTemperature
        {
            get { return _displayTemperature; }
            set { SetProperty(ref _displayTemperature, value); }
        }

        public bool CustomApi
        {
            get { return _customApi; }
            set { SetProperty(ref _customApi, value); }
        }

        public string CharUri
        {
            get { return _charUri; }
            set { SetProperty(ref _charUri, value); }
        }

        public string CharKey
        {
            get { return _charKey; }
            set { SetProperty(ref _charKey, value); }
        }

        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }

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
            _charId = parameters.GetValue<string>("Data");

            if (string.IsNullOrEmpty(_charId))
            {
                _characterModel = _charCore.CharacterLoad();
            }
            else
            {
                _characterModel = _charCore.CharacterLoad(_charId);
            }

            _charId = _characterModel.CharId;
            CharName = _characterModel.CharName;
            CharTemperature = _characterModel.CharTemperature;
            DisplayTemperature = _charTemperature.ToString("0.0");
            CharSet = _characterModel.CharSet;
            CustomApi = _characterModel.CustomApi;
            CharUri = _characterModel.CharUri;
            CharKey = _characterModel.CharKey;
        }
    }
}