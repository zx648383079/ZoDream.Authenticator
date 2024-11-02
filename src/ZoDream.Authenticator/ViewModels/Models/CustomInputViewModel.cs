using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class CustomInputViewModel: BindableBase
    {

        public InputType Type { get; set; } = InputType.Text;

        private string _header = string.Empty;

        public string Header {
            get => _header;
            set => Set(ref _header, value);
        }

        private string _value = string.Empty;

        public string Value {
            get => _value;
            set => Set(ref _value, value);
        }

    }

    public enum InputType
    {
        Text,
        Password,

    }
}
