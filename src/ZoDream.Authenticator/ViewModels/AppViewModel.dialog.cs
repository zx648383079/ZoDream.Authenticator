using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using WinRT.Interop;
using ZoDream.Authenticator.Dialogs;

namespace ZoDream.Authenticator.ViewModels
{
    internal partial class AppViewModel
    {
        public XamlRoot BaseXamlRoot => _baseWindow!.Content.XamlRoot;

        public void InitializePicker(object target)
        {
            InitializeWithWindow.Initialize(target, _baseWindowHandle);
        }

        public async Task<bool> ConfirmAsync(string message, string title = "提示")
        {
            var dialog = new ConfirmDialog
            {
                Title = title,
                Content = message
            };
            return await OpenDialogAsync(dialog) == ContentDialogResult.Primary;
        }

        public IAsyncOperation<ContentDialogResult> OpenDialogAsync(ContentDialog target)
        {
            target.XamlRoot = BaseXamlRoot;
            return target.ShowAsync();
        }
    }
}
