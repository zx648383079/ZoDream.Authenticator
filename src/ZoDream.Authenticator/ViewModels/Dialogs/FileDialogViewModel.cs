using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class FileDialogViewModel: BindableBase, IFormValidator, IEntryForm
    {
        private string _title = string.Empty;

        public string Title {
            get => _title;
            set => Set(ref _title, value);
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Title);

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
            };
            return true;
        }
    }
}
