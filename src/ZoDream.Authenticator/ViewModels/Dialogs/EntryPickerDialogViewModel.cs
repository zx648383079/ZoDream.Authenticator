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
            new("加密钱包", "\uE70B", typeof(WalletEntryDialog)),
            new("银行卡", "\uE70B", typeof(BankCardEntryDialog)),
            new("服务器", "\uE70B", typeof(ServerEntryDialog)),
            new("FTP", "\uE70B", typeof(FtpEntryDialog)),
            new("数据库", "\uE70B", typeof(DatabaseEntryDialog)),
            new("API证书", "\uE70B", typeof(ApiEntryDialog)),
            new("软件证书", "\uE70B", typeof(LicenseEntryDialog)),
            new("WIFI", "\uE70B", typeof(WirelessEntryDialog)),
            new("Email", "\uE70B", typeof(EmailEntryDialog)),
        ];

        private OptionItemViewModel? _selectedItem;

        public OptionItemViewModel? SelectedItem {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

    }
}
