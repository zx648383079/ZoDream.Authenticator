using ZoDream.Shared.Database;

namespace ZoDream.Authenticator.ViewModels.Models
{
    public class PasswordEntryViewModel : EntryItemViewModel, IPasswordEntryEntity
    {
        public string Password { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

    }
}
