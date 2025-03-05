using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZoDream.Authenticator.Behaviors
{
    public class PointerHoverBehavior : Behavior<FrameworkElement>
    {
        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ListItemDoubleClickBehavior), new PropertyMetadata(null));

        

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PointerEntered += AssociatedObject_PointerEntered;
            AssociatedObject.PointerExited += AssociatedObject_PointerExited;
        }

        private void AssociatedObject_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "HoverPanelHidden", true);
            Command?.Execute(false);
        }

        private void AssociatedObject_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Pen)
            {
                VisualStateManager.GoToState(sender as Control, "HoverPanelShown", true);
                Command?.Execute(true);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PointerEntered -= AssociatedObject_PointerEntered;
            AssociatedObject.PointerExited -= AssociatedObject_PointerExited;
        }
    }
}
