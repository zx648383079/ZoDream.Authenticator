using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class QuickDialogViewModel: BindableBase, IFormValidator, IEntryForm
    {

        private string _title = string.Empty;

        public string Title {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _account = string.Empty;

        public string Account {
            get => _account;
            set => Set(ref _account, value);
        }

        private string _password = string.Empty;

        public string Password {
            get => _password;
            set => Set(ref _password, value);
        }

        private string _url = string.Empty;

        public string Url {
            get => _url;
            set => Set(ref _url, value);
        }



        public bool IsValid => !string.IsNullOrWhiteSpace(Title) 
            && !string.IsNullOrWhiteSpace(Account);

        public bool TryParse(out EntryItemViewModel entry)
        {
            if (!IsValid)
            {
                entry = null;
                return false;
            }
            entry = new()
            {
                Title = Title,
                Account = Account,
            };
            return true;
        }
    }
}
