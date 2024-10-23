using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using ZoDream.Authenticator.Pages;

namespace ZoDream.Authenticator.ViewModels
{
    internal partial class AppViewModel
    {

        private Frame _rootFrame;

        public void Navigate<T>() where T : Page
        {
            _rootFrame.Navigate(typeof(T));
            BackEnabled = typeof(T) != typeof(StartupPage);
        }

        public void Navigate<T>(object parameter) where T : Page
        {
            _rootFrame.Navigate(typeof(T), parameter);
            BackEnabled = typeof(T) != typeof(StartupPage);
        }

        public void NavigateBack()
        {
            _rootFrame.GoBack();
            BackEnabled = false;
        }
        /// <summary>
        ///  起始页
        /// </summary>
        private void Startup()
        {
            var app = AppInstance.GetCurrent();
            var args = app.GetActivatedEventArgs();
            if (args.Kind != ExtendedActivationKind.File)
            {
                Navigate<StartupPage>();
                return;
            }
            if (args.Data is FileActivatedEventArgs e)
            {
                Navigate<WorkspacePage>(e.Files);
                //Task.Factory.StartNew(() => 
                //{
                //    Thread.Sleep(1000);
                //    DispatcherQueue.TryEnqueue(() => {
                //        _ = ConfirmAsync();
                //    });
                //});
                return;
            }

        }
    }
}
