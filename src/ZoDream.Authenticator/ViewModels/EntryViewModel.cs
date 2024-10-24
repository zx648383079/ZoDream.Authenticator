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
            var dialog = new QuickDialog();
            var model = dialog.ViewModel;
            if (!await _app.OpenFormAsync(dialog))
            {
                return;
            }
            EntryItems.Add(new()
            {
                Title = model.Title,
                Account = model.Account,
            });
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
