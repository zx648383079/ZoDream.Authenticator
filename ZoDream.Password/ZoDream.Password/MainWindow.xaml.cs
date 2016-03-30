using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using ZoDream.Password.ViewModel;

namespace ZoDream.Password
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Send(new NotificationMessageAction(null, ()=>
            {
                Dispatcher.Invoke(() =>
                {
                    Close();
                });
            }), "main");
        }
    }
}