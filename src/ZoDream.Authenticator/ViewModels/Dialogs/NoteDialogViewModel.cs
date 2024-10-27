using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class NoteDialogViewModel: BindableBase, IFormValidator, IEntryForm
    {
        private string _title = string.Empty;

        public string Title {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _content = string.Empty;

        public string Content {
            get => _content;
            set => Set(ref _content, value);
        }


        public bool IsValid => !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Content);

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
