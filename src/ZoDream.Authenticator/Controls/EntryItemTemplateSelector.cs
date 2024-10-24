using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Authenticator.ViewModels;

namespace ZoDream.Authenticator.Controls
{
    public class EntryItemTemplateSelector: DataTemplateSelector
    {

        public DataTemplate? DefaultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is EntryItemViewModel)
            {
                return DefaultTemplate;
            }
            return base.SelectTemplateCore(item, container);
        }
    }
}
