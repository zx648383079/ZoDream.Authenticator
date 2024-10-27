using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Authenticator.ViewModels
{
    public class OptionItemViewModel
    {

        public string Name { get; set; }

        public string Icon { get; set; }

        public Type? TargetType { get; set; }

        public OptionItemViewModel(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }

        public OptionItemViewModel(string name, string icon, Type targetType)
            : this(name, icon)
        {
            TargetType = targetType;
        }
    }
}
