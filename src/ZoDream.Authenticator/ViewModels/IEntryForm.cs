namespace ZoDream.Authenticator.ViewModels
{
    public interface IEntryForm
    {

        public bool TryParse(out EntryItemViewModel entry);
    }
}
