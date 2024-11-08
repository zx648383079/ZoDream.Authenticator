using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture.Frames;
using Windows.Media.Capture;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using ZoDream.Authenticator.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ZoDream.Authenticator.Dialogs
{
    public sealed partial class ScanDialog : ContentDialog
    {
        public ScanDialog()
        {
            this.InitializeComponent();
        }

        public string Text { get; private set; } = string.Empty;

        private void BarcodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var barcode = (BarcodeBox)sender;
            Text = barcode.Text;
            barcode.Stop();
            Hide();
        }
    }
}
