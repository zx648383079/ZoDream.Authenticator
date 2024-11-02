using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Controls
{
    public partial class SettingCard
    {
        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="Header"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(SettingCard),
            new PropertyMetadata(defaultValue: null, (d, e) => ((SettingCard)d).OnHeaderPropertyChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="Description"/> property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(object),
            typeof(SettingCard),
            new PropertyMetadata(defaultValue: null, (d, e) => ((SettingCard)d).OnDescriptionPropertyChanged((object)e.OldValue, (object)e.NewValue)));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="HeaderIcon"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderIconProperty = DependencyProperty.Register(
            nameof(HeaderIcon),
            typeof(IconElement),
            typeof(SettingCard),
            new PropertyMetadata(defaultValue: null, (d, e) => ((SettingCard)d).OnHeaderIconPropertyChanged((IconElement)e.OldValue, (IconElement)e.NewValue)));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="ActionIcon"/> property.
        /// </summary>
        public static readonly DependencyProperty ActionIconProperty = DependencyProperty.Register(
            nameof(ActionIcon),
            typeof(IconElement),
            typeof(SettingCard),
            new PropertyMetadata(defaultValue: "\ue974"));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="ActionIconToolTip"/> property.
        /// </summary>
        public static readonly DependencyProperty ActionIconToolTipProperty = DependencyProperty.Register(
            nameof(ActionIconToolTip),
            typeof(string),
            typeof(SettingCard),
            new PropertyMetadata(defaultValue: null));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="IsClickEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty IsClickEnabledProperty = DependencyProperty.Register(
            nameof(IsClickEnabled),
            typeof(bool),
            typeof(SettingCard),
            new PropertyMetadata(defaultValue: false, (d, e) => ((SettingCard)d).OnIsClickEnabledPropertyChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="IsActionIconVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty IsActionIconVisibleProperty = DependencyProperty.Register(
            nameof(IsActionIconVisible),
            typeof(bool),
            typeof(SettingCard),
            new PropertyMetadata(defaultValue: true, (d, e) => ((SettingCard)d).OnIsActionIconVisiblePropertyChanged((bool)e.OldValue, (bool)e.NewValue)));

        /// <summary>
        /// Gets or sets the Header.
        /// </summary>
        public object Header {
            get => (object)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
#pragma warning disable CS0109 // Member does not hide an inherited member; new keyword is not required
        public new object Description
#pragma warning restore CS0109 // Member does not hide an inherited member; new keyword is not required
        {
            get => (object)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        /// <summary>
        /// Gets or sets the icon on the left.
        /// </summary>
        public IconElement HeaderIcon {
            get => (IconElement)GetValue(HeaderIconProperty);
            set => SetValue(HeaderIconProperty, value);
        }

        /// <summary>
        /// Gets or sets the icon that is shown when IsClickEnabled is set to true.
        /// </summary>
        public IconElement ActionIcon {
            get => (IconElement)GetValue(ActionIconProperty);
            set => SetValue(ActionIconProperty, value);
        }

        /// <summary>
        /// Gets or sets the tooltip of the ActionIcon.
        /// </summary>
        public string ActionIconToolTip {
            get => (string)GetValue(ActionIconToolTipProperty);
            set => SetValue(ActionIconToolTipProperty, value);
        }

        /// <summary>
        /// Gets or sets if the card can be clicked.
        /// </summary>
        public bool IsClickEnabled {
            get => (bool)GetValue(IsClickEnabledProperty);
            set => SetValue(IsClickEnabledProperty, value);
        }


        /// <summary>
        /// Gets or sets if the ActionIcon is shown.
        /// </summary>
        public bool IsActionIconVisible {
            get => (bool)GetValue(IsActionIconVisibleProperty);
            set => SetValue(IsActionIconVisibleProperty, value);
        }

        protected virtual void OnIsClickEnabledPropertyChanged(bool oldValue, bool newValue)
        {
            OnIsClickEnabledChanged();
        }
        protected virtual void OnHeaderIconPropertyChanged(IconElement oldValue, IconElement newValue)
        {
            OnHeaderIconChanged();
        }

        protected virtual void OnHeaderPropertyChanged(object oldValue, object newValue)
        {
            OnHeaderChanged();
        }

        protected virtual void OnDescriptionPropertyChanged(object oldValue, object newValue)
        {
            OnDescriptionChanged();
        }

        protected virtual void OnIsActionIconVisiblePropertyChanged(bool oldValue, bool newValue)
        {
            OnActionIconChanged();
        }
    }
}
