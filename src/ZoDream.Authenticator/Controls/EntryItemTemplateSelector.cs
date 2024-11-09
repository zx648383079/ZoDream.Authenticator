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
        public DataTemplate? IconTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is EntryBaseViewModel e)
            {
                return string.IsNullOrWhiteSpace(e.Icon) || e.Icon.Length == 1 ? IconTemplate : DefaultTemplate;
            }
            return base.SelectTemplateCore(item, container);
        }
    }
}
