using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Controls
{
    [TemplatePart(Name = ActionIconPresenterHolder, Type = typeof(Viewbox))]
    [TemplatePart(Name = HeaderPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = DescriptionPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = HeaderIconPresenterHolder, Type = typeof(Viewbox))]

    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = PointerOverState, GroupName = CommonStates)]
    [TemplateVisualState(Name = PressedState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    public partial class SettingCard: ButtonBase
    {
        internal const string CommonStates = "CommonStates";
        internal const string NormalState = "Normal";
        internal const string PointerOverState = "PointerOver";
        internal const string PressedState = "Pressed";
        internal const string DisabledState = "Disabled";

        internal const string ActionIconPresenterHolder = "PART_ActionIconPresenterHolder";
        internal const string HeaderPresenter = "PART_HeaderPresenter";
        internal const string DescriptionPresenter = "PART_DescriptionPresenter";
        internal const string HeaderIconPresenterHolder = "PART_HeaderIconPresenterHolder";

        public SettingCard()
        {
            DefaultStyleKey = typeof(SettingCard);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            IsEnabledChanged -= OnIsEnabledChanged;
            OnActionIconChanged();
            OnHeaderChanged();
            OnHeaderIconChanged();
            OnDescriptionChanged();
            OnIsClickEnabledChanged();
            IsEnabledChanged += OnIsEnabledChanged;
        }


        private void Control_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.GamepadA)
            {
                VisualStateManager.GoToState(this, NormalState, true);
            }
        }

        private void Control_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.GamepadA)
            {
                // Check if the active focus is on the card itself - only then we show the pressed state.
                if (GetFocusedElement() is SettingCard)
                {
                    VisualStateManager.GoToState(this, PressedState, true);
                }
            }
        }

        public void Control_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            VisualStateManager.GoToState(this, PointerOverState, true);
        }

        public void Control_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            VisualStateManager.GoToState(this, NormalState, true);
        }

        private void Control_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);
            VisualStateManager.GoToState(this, NormalState, true);
        }

        private void Control_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            base.OnPointerCanceled(e);
            VisualStateManager.GoToState(this, NormalState, true);
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            //  e.Handled = true;
            if (IsClickEnabled)
            {
                base.OnPointerPressed(e);
                VisualStateManager.GoToState(this, PressedState, true);
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (IsClickEnabled)
            {
                base.OnPointerReleased(e);
                VisualStateManager.GoToState(this, NormalState, true);
            }
        }

        private void EnableButtonInteraction()
        {
            DisableButtonInteraction();

            IsTabStop = true;
            PointerEntered += Control_PointerEntered;
            PointerExited += Control_PointerExited;
            PointerCaptureLost += Control_PointerCaptureLost;
            PointerCanceled += Control_PointerCanceled;
            PreviewKeyDown += Control_PreviewKeyDown;
            PreviewKeyUp += Control_PreviewKeyUp;
        }

        private void DisableButtonInteraction()
        {
            IsTabStop = false;
            PointerEntered -= Control_PointerEntered;
            PointerExited -= Control_PointerExited;
            PointerCaptureLost -= Control_PointerCaptureLost;
            PointerCanceled -= Control_PointerCanceled;
            PreviewKeyDown -= Control_PreviewKeyDown;
            PreviewKeyUp -= Control_PreviewKeyUp;
        }

        private void OnIsClickEnabledChanged()
        {
            OnActionIconChanged();
            if (IsClickEnabled)
            {
                EnableButtonInteraction();
            }
            else
            {
                DisableButtonInteraction();
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, true);
        }

        private void OnActionIconChanged()
        {
            if (GetTemplateChild(ActionIconPresenterHolder) is FrameworkElement actionIconPresenter)
            {
                if (IsClickEnabled && IsActionIconVisible)
                {
                    actionIconPresenter.Visibility = Visibility.Visible;
                }
                else
                {
                    actionIconPresenter.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void OnHeaderIconChanged()
        {
            if (GetTemplateChild(HeaderIconPresenterHolder) is FrameworkElement headerIconPresenter)
            {
                headerIconPresenter.Visibility = HeaderIcon != null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void OnDescriptionChanged()
        {
            if (GetTemplateChild(DescriptionPresenter) is FrameworkElement descriptionPresenter)
            {
                descriptionPresenter.Visibility = IsNullOrEmptyString(Description)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }

        }

        private void OnHeaderChanged()
        {
            if (GetTemplateChild(HeaderPresenter) is FrameworkElement headerPresenter)
            {
                headerPresenter.Visibility = IsNullOrEmptyString(Header)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }

        }

        private FrameworkElement? GetFocusedElement()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.UIElement", "XamlRoot") && XamlRoot != null)
            {
                return FocusManager.GetFocusedElement(XamlRoot) as FrameworkElement;
            }
            else
            {
                return FocusManager.GetFocusedElement() as FrameworkElement;
            }
        }

        private static bool IsNullOrEmptyString(object obj)
        {
            if (obj == null)
            {
                return true;
            }

            if (obj is string objString && objString == string.Empty)
            {
                return true;
            }

            return false;
        }
    }
}
