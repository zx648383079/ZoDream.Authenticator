using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using Windows.ApplicationModel.Activation;
using ZoDream.Authenticator.Pages;

namespace ZoDream.Authenticator.ViewModels
{
    internal partial class AppViewModel
    {

        private Frame _rootFrame;
        private Frame? _innerFrame;

        internal void BindFrame(Frame? frame)
        {
            _innerFrame = frame;
            if (frame is not null)
            {
                Navigate<EntryPage>();
            } else
            {
                BackEnabled = false;
            }
        }

        private bool IsRootPage(Type page)
        {
            return page == typeof(StartupPage) || page == typeof(WorkspacePage);
        }

        public void Navigate<T>() where T : Page
        {
            var page = typeof(T);
            if (IsRootPage(page))
            {
                _rootFrame.Navigate(page);
            } else
            {
                _innerFrame?.Navigate(page);
            }
            BackEnabled = _innerFrame is not null &&  _innerFrame.CanGoBack;
        }

        public void Navigate<T>(object parameter) where T : Page
        {
            var page = typeof(T);
            if (IsRootPage(page))
            {
                _rootFrame.Navigate(page, parameter);
            }
            else
            {
                _innerFrame?.Navigate(page, parameter);
            }
            BackEnabled = _innerFrame is not null && _innerFrame.CanGoBack;
        }

        public void NavigateBack()
        {
            if (_rootFrame.Content is StartupPage o)
            {
                o.ViewModel.IsNextStep = false;
                return;
            }
            if (_innerFrame is null)
            {
                return;
            }
            _innerFrame.GoBack();
            BackEnabled = _innerFrame.CanGoBack;
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
