using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using ZoDream.Authenticator.Dialogs;
using ZoDream.Authenticator.ViewModels.Models;
using ZoDream.Shared.Database;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class EntryViewModel: BindableBase
    {
        public EntryViewModel()
        {
            AddCommand = new RelayCommand(TapAdd);
            ScanCommand = new RelayCommand(TapScan);
            SaveCommand = new RelayCommand(TapSave);
            SearchCommand = new RelayCommand(TapSearch);
            EditCommand = new RelayCommand(TapEdit);
            ViewCommand = new RelayCommand(TapView);
        }

        private readonly AppViewModel _app = App.ViewModel;

        private ObservableCollection<EntryBaseViewModel> _entryItems = [];

        public ObservableCollection<EntryBaseViewModel> EntryItems {
            get => _entryItems;
            set => Set(ref _entryItems, value);
        }

        private bool _isUpdated;

        public bool IsUpdated {
            get => _isUpdated;
            set => Set(ref _isUpdated, value);
        }


        public ICommand AddCommand { get; private set; }
        public ICommand ScanCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand ViewCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        private async void TapScan(object? _)
        {
            var dialog = new ScanDialog();
            var res = await _app.OpenDialogAsync(dialog);
            if (res != ContentDialogResult.None)
            {
                return;
            }
            if (WirelessEntryViewModel.TryParse(dialog.Text, out var model))
            {
                _app.Database?.Insert(model);
                EntryItems.Add(model);
                IsUpdated = true;
                return;
            }
            if (TOTPEntryViewModel.TryParse(dialog.Text, out var mo))
            {
                _app.Database?.Insert(mo);
                EntryItems.Add(mo);
                IsUpdated = true;
            }
        }

        private void TapSave(object? _)
        {
            if (!IsUpdated)
            {
                return;
            }
            _app.Database?.Flush();
            IsUpdated = false;
        }

        private async void TapSearch(object? _)
        {
            var dialog = new SearchDialog();
            var res = await _app.OpenFormAsync(dialog);
            if (!res)
            {
                return;
            }

        }

        private async void TapAdd(object? _)
        {
            var picker = new EntryPickerDialog();
            var model = picker.ViewModel;
            if (await _app.OpenDialogAsync(picker) == ContentDialogResult.Secondary)
            {
                return;
            }
            var pageType = model.SelectedItem?.TargetType;
            if (pageType is null)
            {
                return;
            }
            var obj = Activator.CreateInstance(pageType);
            if (obj is not ContentDialog dialog)
            {
                return;
            }
            if (!await _app.OpenFormAsync(dialog))
            {
                return;
            }
            if (dialog.DataContext is IEntryForm form && form.TryParse(out var item))
            {
                _app.Database?.Insert(item);
                EntryItems.Add(item);
                IsUpdated = true;
            }
        }

        private async void TapEdit(object? _)
        {
            var dialog = new EditDialog();
            var model = dialog.ViewModel;
            if (!await _app.OpenFormAsync(dialog))
            {
                return;
            }
        }

        private async void TapView(object? _)
        {
            var dialog = new EntryDialog();
            var model = dialog.ViewModel;
            if (!await _app.OpenFormAsync(dialog))
            {
                return;
            }
        }

        private void TapCopy(object? _)
        {
            var package = new DataPackage();
            package.SetText("Copy this text");
            Clipboard.SetContentWithOptions(package, new()
            {
                IsAllowedInHistory = false,
                IsRoamable = false,
            });
        }

        public void LoadAsync()
        {
            if (_app.Database is null)
            {
                return;
            }
            EntryItems.Clear();
            var items = _app.Database.Fetch(Create);
            foreach (var item in items)
            {
                EntryItems.Add(item);
            }
        }

        private EntryBaseViewModel Create(EntryType type)
        {
            return type switch
            {
                EntryType.ToTp => new TOTPEntryViewModel(),
                EntryType.Wireless => new WirelessEntryViewModel(),
                _ => new EntryItemViewModel(),
            };
        }
    }
}
