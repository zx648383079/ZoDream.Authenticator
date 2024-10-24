using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class GroupDialogViewModel: BindableBase, IFormValidator
    {

        private string _name = string.Empty;

        public string Name {
            get => _name;
            set => Set(ref _name, value);
        }

        private GroupItemViewModel[] _groupItems = [];

        public GroupItemViewModel[] GroupItems {
            get => _groupItems;
            set => Set(ref _groupItems, value);
        }


        private int _parentIndex;

        public int ParentIndex {
            get => _parentIndex;
            set => Set(ref _parentIndex, value);
        }


        public bool IsValid => !string.IsNullOrWhiteSpace(Name);
    }
}
