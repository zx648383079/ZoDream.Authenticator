using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Database;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class EntryItemViewModel: BindableBase, IAccountEntryEntity
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        private string _title = string.Empty;

        public string Title {
            get => _title;
            set => Set(ref _title, value);
        }


        private string _icon = string.Empty;

        public string Icon {
            get => _icon;
            set => Set(ref _icon, value);
        }

        private string _account = string.Empty;

        public string Account {
            get => _account;
            set => Set(ref _account, value);
        }
    }
}
