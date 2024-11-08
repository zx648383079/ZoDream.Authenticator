using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class SearchDialogViewModel: BindableBase, IFormValidator
    {

        private string _keywords = string.Empty;

        public string Keywords {
            get => _keywords;
            set => Set(ref _keywords, value);
        }


        public bool IsValid => !string.IsNullOrWhiteSpace(Keywords);
    }
}
