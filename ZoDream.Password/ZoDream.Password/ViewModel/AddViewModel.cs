using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZoDream.Password.Model;

namespace ZoDream.Password.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AddViewModel : ViewModelBase
    {
        private NotificationMessageAction<PasswordItem> _addItem;

        private PasswordItem _item = new PasswordItem();
        /// <summary>
        /// Initializes a new instance of the AddViewModel class.
        /// </summary>
        public AddViewModel()
        {
            Messenger.Default.Register<NotificationMessageAction<PasswordItem>>(this, "add", m =>
            {
                _addItem = m;
                if (m.Sender == null) return;
                _item = (PasswordItem)m.Sender;
            });
        }

        /// <summary>
        /// The <see cref="Url" /> property's name.
        /// </summary>
        public const string UrlPropertyName = "Url";

        /// <summary>
        /// Sets and gets the Url property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Url
        {
            get
            {
                return _item.Url;
            }
            set
            {
                var temp = string.Empty;
                Set(UrlPropertyName, ref temp, value);
                _item.Url = temp;
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _item.Name;
            }
            set
            {
                var temp = string.Empty;
                Set(NamePropertyName, ref temp, value);
                _item.Name = temp;
            }
        }

        /// <summary>
        /// The <see cref="Email" /> property's name.
        /// </summary>
        public const string EmailPropertyName = "Email";

        /// <summary>
        /// Sets and gets the Email property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Email
        {
            get
            {
                return _item.Email;
            }
            set
            {
                var temp = string.Empty;
                Set(EmailPropertyName, ref temp, value);
                _item.Email = temp;
            }
        }

        /// <summary>
        /// The <see cref="Number" /> property's name.
        /// </summary>
        public const string NumberPropertyName = "Number";

        /// <summary>
        /// Sets and gets the Number property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Number
        {
            get
            {
                return _item.Number;
            }
            set
            {
                var temp = string.Empty;
                Set(NumberPropertyName, ref temp, value);
                _item.Number = temp;
            }
        }

        /// <summary>
        /// The <see cref="Password" /> property's name.
        /// </summary>
        public const string PasswordPropertyName = "Password";

        /// <summary>
        /// Sets and gets the Password property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Password
        {
            get
            {
                return _item.Password;
            }
            set
            {
                var temp = string.Empty;
                Set(PasswordPropertyName, ref temp, value);
                _item.Password = temp;
            }
        }

        /// <summary>
        /// The <see cref="Mark" /> property's name.
        /// </summary>
        public const string MarkPropertyName = "Mark";

        /// <summary>
        /// Sets and gets the Mark property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Mark
        {
            get
            {
                return _item.Mark;
            }
            set
            {
                var temp = string.Empty;
                Set(MarkPropertyName, ref temp, value);
                _item.Mark = temp;
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

        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand
                    ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));
            }
        }

        private void ExecuteSaveCommand()
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Url)) return;
            _addItem.Execute(_item);
            if (0 < _item.Id) return;
            Name = Url = Email = Number = Password = Mark = string.Empty;
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