using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    public class GroupItemViewModel: BindableBase
    {

        public string Name { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;

        private ObservableCollection<GroupItemViewModel> _children = [];

        public ObservableCollection<GroupItemViewModel> Children {
            get => _children;
            set => Set(ref _children, value);
        }

        public GroupItemViewModel()
        {
            
        }

        public GroupItemViewModel(string name, string tag)
        {
            Name = name;
            Tag = tag;
        }

        public GroupItemViewModel(string name, string icon, string tag)
            : this(name, tag)
        {
            Icon = icon; 
        }
    }
}
