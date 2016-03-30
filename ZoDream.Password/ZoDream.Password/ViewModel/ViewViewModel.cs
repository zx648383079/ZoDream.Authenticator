using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZoDream.Password.Helper;
using ZoDream.Password.Model;

namespace ZoDream.Password.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ViewViewModel class.
        /// </summary>
        public ViewViewModel()
        {
            Messenger.Default.Register<NotificationMessageAction>(this, "view", m =>
            {
                var item = (PasswordItem)m.Sender;
                Name = item.Name;
                Url = item.Url;
                Email = item.Email;
                Number = item.Number;
                Password = item.Password;
                Mark = item.Mark;
            });
        }

        /// <summary>
        /// The <see cref="Url" /> property's name.
        /// </summary>
        public const string UrlPropertyName = "Url";

        private string _url = string.Empty;

        /// <summary>
        /// Sets and gets the Url property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                Set(UrlPropertyName, ref _url, value);
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = string.Empty;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(NamePropertyName, ref _name, value);
            }
        }

        /// <summary>
        /// The <see cref="Email" /> property's name.
        /// </summary>
        public const string EmailPropertyName = "Email";

        private string _email = string.Empty;

        /// <summary>
        /// Sets and gets the Email property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                Set(EmailPropertyName, ref _email, value);
            }
        }

        /// <summary>
        /// The <see cref="Number" /> property's name.
        /// </summary>
        public const string NumberPropertyName = "Number";

        private string _number = string.Empty;

        /// <summary>
        /// Sets and gets the Number property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Number
        {
            get
            {
                return _number;
            }
            set
            {
                Set(NumberPropertyName, ref _number, value);
            }
        }

        /// <summary>
        /// The <see cref="Password" /> property's name.
        /// </summary>
        public const string PasswordPropertyName = "Password";

        private string _password = string.Empty;

        /// <summary>
        /// Sets and gets the Password property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                Set(PasswordPropertyName, ref _password, value);
            }
        }

        /// <summary>
        /// The <see cref="Mark" /> property's name.
        /// </summary>
        public const string MarkPropertyName = "Mark";

        private string _mark = string.Empty;

        /// <summary>
        /// Sets and gets the Mark property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Mark
        {
            get
            {
                return _mark;
            }
            set
            {
                Set(MarkPropertyName, ref _mark, value);
            }
        }

        /// <summary>
        /// The <see cref="Message" /> property's name.
        /// </summary>
        public const string MessagePropertyName = "Message";

        private string _message = string.Empty;

        /// <summary>
        /// Sets and gets the Message property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                Set(MessagePropertyName, ref _message, value);
            }
        }

        private RelayCommand _copyNameCommand;

        /// <summary>
        /// Gets the CopyNameCommand.
        /// </summary>
        public RelayCommand CopyNameCommand
        {
            get
            {
                return _copyNameCommand
                    ?? (_copyNameCommand = new RelayCommand(ExecuteCopyNameCommand));
            }
        }

        private void ExecuteCopyNameCommand()
        {
            LocalHelper.Copy(Name);
            _showMessage("复制账号成功！");
        }

        private RelayCommand _copyEmailCommand;

        /// <summary>
        /// Gets the CopyNameCommand.
        /// </summary>
        public RelayCommand CopyEmailCommand
        {
            get
            {
                return _copyEmailCommand
                    ?? (_copyEmailCommand = new RelayCommand(ExecuteCopyEmailCommand));
            }
        }

        private void ExecuteCopyEmailCommand()
        {
            LocalHelper.Copy(Email);
            _showMessage("复制邮箱成功！");
        }


        private RelayCommand _copyNumberCommand;

        /// <summary>
        /// Gets the CopyNumberCommand.
        /// </summary>
        public RelayCommand CopyNumberCommand
        {
            get
            {
                return _copyNumberCommand
                    ?? (_copyNumberCommand = new RelayCommand(ExecuteCopyNumberCommand));
            }
        }

        private void ExecuteCopyNumberCommand()
        {
            LocalHelper.Copy(Number);
            _showMessage("复制手机号成功！");
        }

        private RelayCommand _copyPasswordCommand;

        /// <summary>
        /// Gets the CopyNameCommand.
        /// </summary>
        public RelayCommand CopyPasswordCommand
        {
            get
            {
                return _copyPasswordCommand
                    ?? (_copyPasswordCommand = new RelayCommand(ExecuteCopyPasswordCommand));
            }
        }

        private void ExecuteCopyPasswordCommand()
        {
            LocalHelper.Copy(Password);
            _showMessage("复制密码成功！");
        }


        private RelayCommand _openWebCommand;

        /// <summary>
        /// Gets the OpenWebCommand.
        /// </summary>
        public RelayCommand OpenWebCommand
        {
            get
            {
                return _openWebCommand
                    ?? (_openWebCommand = new RelayCommand(ExecuteOpenWebCommand));
            }
        }

        private void ExecuteOpenWebCommand()
        {
            _showMessage(LocalHelper.OpenBrowser(Url) ? "已打开浏览器！" : "打开浏览器失败！");
        }

        private void _showMessage(string message)
        {
            Message = message;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(3000);
                Message = string.Empty;
            });
        }
    }
}