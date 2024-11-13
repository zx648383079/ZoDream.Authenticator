using ZoDream.Shared.Database;

namespace ZoDream.Authenticator.ViewModels.Models
{
    public class WirelessEntryViewModel : EntryItemViewModel, IWirelessEntryEntity
    {
        public string Password { get; set; } = string.Empty;
        public string Security { get; set; } = string.Empty;

        public WirelessEntryViewModel()
        {
            
        }

        public static bool TryParse(string text, out WirelessEntryViewModel model)
        {
            if (!text.StartsWith("WIFI:"))
            {
                model = null;
                return false;
            }
            // WIFI:T:WPA;P:123456789;S:zre;H:false;
            model = new();
            foreach (var item in text[5..].Split([';']))
            {
                var args = item.Split(':', 2);
                if (args.Length != 2)
                {
                    continue;
                }
                switch (args[0])
                {
                    case "T":
                        model.Security = args[1];
                        break;
                    case "P":
                        model.Password = args[1];
                        break;
                    case "S":
                        model.Title = model.Account = args[1];
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
    }
}
