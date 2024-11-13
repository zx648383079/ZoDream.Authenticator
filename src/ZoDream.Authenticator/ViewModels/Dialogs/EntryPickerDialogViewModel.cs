using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Authenticator.Dialogs;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class EntryPickerDialogViewModel : BindableBase
    {

        public OptionItemViewModel[] OptionItems { get; private set; } = [
            new("登录", "\uF3B1", typeof(QuickDialog)),
            new("文件", "\uE8A6", typeof(FileEntryDialog)),
            new("便签", "\uE70B", typeof(NoteEntryDialog)),
            new("加密钱包", "\uE7F1", typeof(WalletEntryDialog)),
            new("银行卡", "\uE8C7", typeof(BankCardEntryDialog)),
            new("服务器", "\uE8CC", typeof(ServerEntryDialog)),
            new("FTP", "\uE723", typeof(FtpEntryDialog)),
            new("数据库", "\uE719", typeof(DatabaseEntryDialog)),
            new("API证书", "\uEB41", typeof(ApiEntryDialog)),
            new("软件证书", "\uED35", typeof(LicenseEntryDialog)),
            new("WIFI", "\uE701", typeof(WirelessEntryDialog)),
            new("Email", "\uE715", typeof(EmailEntryDialog)),
        ];

        private OptionItemViewModel? _selectedItem;

        public OptionItemViewModel? SelectedItem {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

    }
}
