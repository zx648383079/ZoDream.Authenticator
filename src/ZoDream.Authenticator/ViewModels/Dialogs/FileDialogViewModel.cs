using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using ZoDream.Authenticator.ViewModels.Models;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class FileDialogViewModel: BindableBase, IFormValidator, IEntryForm
    {
        public FileDialogViewModel()
        {
            OpenCommand = new RelayCommand(TapOpen);
        }


        private string _fullPath = string.Empty;

        private string _title = string.Empty;

        public string Title {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _fileName = string.Empty;

        public string FileName {
            get => _fileName;
            set => Set(ref _fileName, value);
        }


        public ICommand OpenCommand { get; private set; }

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
            _fullPath = items.Path;
            FileName = items.Name;
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Title);

        public bool TryParse(out EntryItemViewModel entry)
        {
            if (!IsValid)
            {
                entry = null;
                return false;
            }
            entry = new FileEntryViewModel()
            {
                Title = Title,
                Account = FileName,
                FileName = _fullPath
            };
            return true;
        }
    }
}
