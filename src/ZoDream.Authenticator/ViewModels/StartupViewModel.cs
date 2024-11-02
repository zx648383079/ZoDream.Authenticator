using System;
using System.Windows.Input;
using Windows.Storage.Pickers;
using ZoDream.Authenticator.Pages;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    internal class StartupViewModel : BindableBase
    {

        public StartupViewModel()
        {
            OpenCommand = new RelayCommand(TapOpen);
            CreateCommand = new RelayCommand(TapCreate);
            EnterCommand = new RelayCommand(TapEnter);
            PickCommand = new RelayCommand(TapPick);
            _fileName = App.ViewModel.Setting.Get<string>(SettingNames.DatabaseFileName);
            if (!string.IsNullOrWhiteSpace(_fileName))
            {
                IsNextStep = true;
            }
        }

        private string _fileName = string.Empty;
        public string Version { get; private set; } = App.ViewModel.Version;

        private string _keyFile = string.Empty;

        public string KeyFile {
            get => _keyFile;
            set => Set(ref _keyFile, value);
        }

        private string _password = string.Empty;

        public string Password {
            get => _password;
            set => Set(ref _password, value);
        }

        private bool _isNextStep;

        public bool IsNextStep {
            get => _isNextStep;
            set {
                Set(ref _isNextStep, value);
                App.ViewModel.BackEnabled = value;
            }
        }

        private bool _isCreateNew;

        public bool IsCreateNew {
            get => _isCreateNew;
            set => Set(ref _isCreateNew, value);
        }



        public ICommand OpenCommand { get; private set; }
        public ICommand CreateCommand { get; private set; }
        public ICommand EnterCommand { get; private set; }
        public ICommand PickCommand { get; private set; }
        public ICommand CreateKeyCommand { get; private set; }

        private async void TapPick(object? _)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            App.ViewModel.InitializePicker(picker);
            var items = await picker.PickSingleFileAsync();
            if (items is null)
            {
                return;
            }
            KeyFile = items.Path;
        }

        private async void TapCreateKey(object? _)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Key", [".key"]);
            App.ViewModel.InitializePicker(picker);
            var items = await picker.PickSaveFileAsync();
            if (items is null)
            {
                return;
            }
            KeyFile = items.Path;
        }

        private async void TapOpen(object? _)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            App.ViewModel.InitializePicker(picker);
            var items = await picker.PickSingleFileAsync();
            if (items is null)
            {
                return;
            }
            IsNextStep = true;
            IsCreateNew = false;
            _fileName = items.Path;
        }

        private async void TapCreate(object? _)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Database", [".kdb"]);
            App.ViewModel.InitializePicker(picker);
            var items = await picker.PickSaveFileAsync();
            if (items is null)
            {
                return;
            }
            _fileName = items.Path;
            IsNextStep = true;
            IsCreateNew = true;
        }

        private void TapEnter(object? _)
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(KeyFile))
            {

            }
            App.ViewModel.Navigate<WorkspacePage>();
        }
    }
}
