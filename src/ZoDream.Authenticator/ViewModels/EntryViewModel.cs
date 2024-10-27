using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using ZoDream.Authenticator.Dialogs;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class EntryViewModel: BindableBase
    {
        public EntryViewModel()
        {
            AddCommand = new RelayCommand(TapAdd);
            EditCommand = new RelayCommand(TapEdit);
            ViewCommand = new RelayCommand(TapView);
        }

        private readonly AppViewModel _app = App.ViewModel;

        private ObservableCollection<EntryItemViewModel> _entryItems = [];

        public ObservableCollection<EntryItemViewModel> EntryItems {
            get => _entryItems;
            set => Set(ref _entryItems, value);
        }

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand ViewCommand { get; private set; }

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
                EntryItems.Add(item);
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
            Clipboard.SetContent(package);
        }
    }
}
