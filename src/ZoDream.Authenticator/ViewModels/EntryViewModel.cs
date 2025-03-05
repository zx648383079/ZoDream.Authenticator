using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using ZoDream.Authenticator.Dialogs;
using ZoDream.Authenticator.ViewModels.Models;
using ZoDream.Shared.Database;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public partial class EntryViewModel: BindableBase
    {
        public EntryViewModel()
        {
            AddCommand = new RelayCommand(TapAdd);
            ScanCommand = new RelayCommand(TapScan);
            SaveCommand = new RelayCommand(TapSave);
            FindCommand = new RelayCommand(TapFind);
            EditCommand = new RelayCommand(TapEdit);
            ViewCommand = new RelayCommand(TapView);
            DeleteCommand = new RelayCommand(TapDelete);
            OpenUrlCommand = new RelayCommand(TapOpenUrl);
            CopyAccountCommand = new RelayCommand(TapCopyAccount);
            CopyPasswordCommand = new RelayCommand(TapCopyPassword);
            CopyUrlCommand = new RelayCommand(TapCopyUrl);
            CopyCodeCommand = new RelayCommand(TapCopyCode);
        }

        private readonly AppViewModel _app = App.ViewModel;
        private int _groupId = 0;

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
        public ICommand FindCommand { get; private set; }

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
                model.GroupId = _groupId;
                _app.Database?.Insert(model);
                EntryItems.Add(model);
                IsUpdated = true;
                return;
            }
            if (TOTPEntryViewModel.TryParse(dialog.Text, out var mo))
            {
                mo.GroupId = _groupId;
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

        private async void TapFind(object? _)
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
                item.GroupId = _groupId;
                _app.Database?.Insert(item);
                EntryItems.Add(item);
                IsUpdated = true;
            }
        }

        public void LoadAsync(object? arg)
        {
            if (arg is int i)
            {
                _groupId = i;
            } else if (arg is IGroupEntity e)
            {
                _groupId = e.Id;
            }
            if (_app.Database is null)
            {
                return;
            }
            EntryItems.Clear();
            var items = _app.Database.Fetch(_groupId, Create);
            foreach (var item in items)
            {
                item.Workspace = this;
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
