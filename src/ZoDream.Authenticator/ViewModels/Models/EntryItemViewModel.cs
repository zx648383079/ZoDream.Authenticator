using ZoDream.Shared.Database;

namespace ZoDream.Authenticator.ViewModels
{
    public class EntryItemViewModel: EntryBaseViewModel, IAccountEntryEntity
    {

        private string _account = string.Empty;

        public string Account {
            get => _account;
            set => Set(ref _account, value);
        }
    }
}
