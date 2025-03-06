using System;
using System.Linq;
using System.Windows.Input;
using ZoDream.Authenticator.Dialogs;
using ZoDream.Authenticator.ViewModels.Models;
using ZoDream.Shared.Database;

namespace ZoDream.Authenticator.ViewModels
{
    public partial class EntryViewModel
    {
        private EntryBaseViewModel? _selectedItem;

        public EntryBaseViewModel? SelectedItem {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }


        public ICommand EditCommand { get; private set; }
        public ICommand ViewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand OpenUrlCommand { get; private set; }
        public ICommand CopyAccountCommand { get; private set; }
        public ICommand CopyPasswordCommand { get; private set; }
        public ICommand CopyUrlCommand { get; private set; }
        public ICommand CopyCodeCommand { get; private set; }

        private async void TapDelete(object? arg)
        {
            var item = arg is EntryBaseViewModel o ? o : SelectedItem;
            if (item is null)
            {
                TapDeleteChecked();
                return;
            }
            if (!await _app.ConfirmAsync($"是否删除“{item.Title}”记录?"))
            {
                return;
            }
            _app.Database?.Delete(item);
            EntryItems.Remove(item);
            SelectedItem = null;
        }
        private async void TapDeleteChecked()
        {
            var items = EntryItems.Where(i => i.IsChecked).Select(i => (IEntryEntity)i).ToArray();
            if (items.Length == 0)
            {
                return;
            }
            if (!await _app.ConfirmAsync($"是否删除选中的{items.Length}项记录?"))
            {
                return;
            }
            _app.Database?.Delete(items);
            for (int i = EntryItems.Count - 1; i >= 0; i--)
            {
                if (items.Contains(EntryItems[i]))
                {
                    EntryItems.RemoveAt(i);
                }
            }
        }


        private async void TapOpenUrl(object? arg)
        {
            var item = arg is EntryBaseViewModel o ? o : SelectedItem;
            if (item is null)
            {
                return;
            }
            var text = _app.Database?.ScalarEntry(item.Id, "Url");
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            if (!text.Contains("://"))
            {
                text = "https://" + text;
            }
            if (Uri.TryCreate(text, UriKind.Absolute, out var uri))
            {
                await Windows.System.Launcher.LaunchUriAsync(uri);
                return;
            }
            _app.CopyText(text, 200);
            await _app.ConfirmAsync("网址不完整，已复制到剪切板，请手动粘贴到浏览器地址栏打开！");
        }

        private void TapCopyAccount(object? arg)
        {
            var item = arg is EntryBaseViewModel o ? o : SelectedItem;
            if (item is null)
            {
                return;
            }
            var text = _app.Database?.ScalarEntry(item.Id, "Account");
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            _app.CopyText(text);
        }
        private void TapCopyPassword(object? arg)
        {
            var item = arg is EntryBaseViewModel o ? o : SelectedItem;
            if (item is null)
            {
                return;
            }
            var text = _app.Database?.ScalarEntry(item.Id, "Password");
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            _app.CopyText(text);
        }

        private void TapCopyUrl(object? arg)
        {
            var item = arg is EntryBaseViewModel o ? o : SelectedItem;
            if (item is null)
            {
                return;
            }
            var text = _app.Database?.ScalarEntry(item.Id, "Url");
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            _app.CopyText(text);
        }

        private void TapCopyCode(object? arg)
        {
            var item = arg is EntryBaseViewModel o ? o : SelectedItem;
            if (item is not TOTPEntryViewModel)
            {
                return;
            }
            var data = _app.Database?.SingleEntry<TOTPEntryViewModel>(item.Id);
            var timeout = 200;
            var text = data?.TryGetCode(out timeout);
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            _app.CopyText(text, timeout);
        }

        private async void TapEdit(object? arg)
        {
            var dialog = new EditDialog();
            var model = dialog.ViewModel;
            if (!await _app.OpenFormAsync(dialog))
            {
                return;
            }
        }

        private async void TapView(object? arg)
        {
            var dialog = new EntryDialog();
            var model = dialog.ViewModel;
            if (!await _app.OpenFormAsync(dialog))
            {
                return;
            }
        }
    }
}
