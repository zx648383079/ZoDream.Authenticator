using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class WalletDialogViewModel: BindableBase, IFormValidator, IEntryForm
    {
        private string _title = string.Empty;

        public string Title {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _walletAddress = string.Empty;

        public string WalletAddress {
            get => _walletAddress;
            set => Set(ref _walletAddress, value);
        }

        private string _password = string.Empty;

        public string Password {
            get => _password;
            set => Set(ref _password, value);
        }

        private string _recoveryPhrase = string.Empty;

        public string RecoveryPhrase {
            get => _recoveryPhrase;
            set => Set(ref _recoveryPhrase, value);
        }


        public bool IsValid => !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(WalletAddress);

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
                Account = WalletAddress,
            };
            return true;
        }
    }
}
