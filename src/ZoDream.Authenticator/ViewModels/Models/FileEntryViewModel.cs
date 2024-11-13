using ZoDream.Shared.Database;

namespace ZoDream.Authenticator.ViewModels.Models
{
    public class FileEntryViewModel : EntryItemViewModel, IFileEntryEntity
    {
        public string FileName { get; set; } = string.Empty;

    }
}
