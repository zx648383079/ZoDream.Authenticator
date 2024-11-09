using ZoDream.Shared.Database;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class EntryBaseViewModel: BindableBase, IEntryEntity
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

    }
}
