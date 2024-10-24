using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZoDream.Authenticator.Behaviors
{
    public class NavigationViewItemBehavior : Behavior<NavigationView>
    {
        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(NavigationViewItemBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ItemInvoked += AssociatedObject_ItemInvoked;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ItemInvoked -= AssociatedObject_ItemInvoked;
        }

        private void AssociatedObject_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                Command.Execute("__setting");
                return;
            }
            Command.Execute(args.InvokedItemContainer.Tag.ToString());
        }
    }
}
