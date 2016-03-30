using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ZoDream.Password.Helper;
using ZoDream.Password.Model;
using ZoDream.Password.View;

namespace ZoDream.Password.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private NotificationMessageAction _closeView;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Messenger.Default.Register<NotificationMessageAction>(this, "main", m => _closeView = m);
            var result = new LoginView().ShowDialog();
            if (true != result)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(500);
                    _closeView.Execute();
                });
                return;
            }
            ViewVisibility = Visibility.Visible;
            GetPassword();
        }

        private void GetPassword(string where = null, params SQLiteParameter[] parameters)
        {
            Task.Factory.StartNew(() =>
            {
                DatabaseHelper.Open();
                var reader = DatabaseHelper.Select<PasswordItem>("*", $"{where} GROUP BY Url ORDER BY Url", parameters);
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var item = new PasswordItem(reader);
                        Application.Current.Dispatcher.Invoke(() => { PasswordList.Add(item); });
                    }
                }
                reader.Close();
                DatabaseHelper.Close();
            });
        }

        /// <summary>
        /// The <see cref="ViewVisibility" /> property's name.
        /// </summary>
        public const string ViewVisibilityPropertyName = "ViewVisibility";

        private Visibility _viewVisibility = Visibility.Hidden;

        /// <summary>
        /// Sets and gets the ViewVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility ViewVisibility
        {
            get
            {
                return _viewVisibility;
            }
            set
            {
                Set(ViewVisibilityPropertyName, ref _viewVisibility, value);
            }
        }

        /// <summary>
        /// The <see cref="Content" /> property's name.
        /// </summary>
        public const string ContentPropertyName = "Content";

        private string _content = string.Empty;

        /// <summary>
        /// Sets and gets the Content property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                Set(ContentPropertyName, ref _content, value);
            }
        }


        /// <summary>
        /// The <see cref="PasswordList" /> property's name.
        /// </summary>
        public const string PasswordListPropertyName = "PasswordList";

        private ObservableCollection<PasswordItem> _passwordList = new ObservableCollection<PasswordItem>();

        /// <summary>
        /// Sets and gets the PasswordList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<PasswordItem> PasswordList
        {
            get
            {
                return _passwordList;
            }
            set
            {
                Set(PasswordListPropertyName, ref _passwordList, value);
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

        private RelayCommand _searchCommand;

        /// <summary>
        /// Gets the SearchCommand.
        /// </summary>
        public RelayCommand SearchCommand
        {
            get
            {
                return _searchCommand
                    ?? (_searchCommand = new RelayCommand(ExecuteSearchCommand));
            }
        }

        private void ExecuteSearchCommand()
        {
            PasswordList.Clear();
            GetPassword("WHERE Url LIKE @text", new SQLiteParameter("@text", $"%{Content}%"));
        }

        private RelayCommand<int> _viewCommand;

        /// <summary>
        /// Gets the ViewCommand.
        /// </summary>
        public RelayCommand<int> ViewCommand
        {
            get
            {
                return _viewCommand
                    ?? (_viewCommand = new RelayCommand<int>(ExecuteViewCommand));
            }
        }

        private void ExecuteViewCommand(int index)
        {
            if (index < 0 || index >= PasswordList.Count) return;
            new ViewView().Show();
            Messenger.Default.Send(new NotificationMessageAction(PasswordList[index], null, ()=> {}), "view");
        }

        private RelayCommand _importCommand;

        /// <summary>
        /// Gets the ImportCommand.
        /// </summary>
        public RelayCommand ImportCommand
        {
            get
            {
                return _importCommand
                    ?? (_importCommand = new RelayCommand(ExecuteImportCommand));
            }
        }

        private void ExecuteImportCommand()
        {

        }

        private RelayCommand _exportCommand;

        /// <summary>
        /// Gets the ExportCommand.
        /// </summary>
        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand
                    ?? (_exportCommand = new RelayCommand(ExecuteExportCommand));
            }
        }

        private void ExecuteExportCommand()
        {

        }

        private RelayCommand<int> _deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        public RelayCommand<int> DeleteCommand
        {
            get
            {
                return _deleteCommand
                    ?? (_deleteCommand = new RelayCommand<int>(ExecuteDeleteCommand));
            }
        }

        private void ExecuteDeleteCommand(int index)
        {
            if (index < 0 || index >= PasswordList.Count) return;

            DatabaseHelper.Open();
            var row = DatabaseHelper.Delete<PasswordItem>($"Id = {PasswordList[index].Id}");
            DatabaseHelper.Close();
            if (row > 0)
            {
                PasswordList.RemoveAt(index);
                _showMessage("已成功删除一条数据！");
            }
        }

        private RelayCommand<int> _editCommand;

        /// <summary>
        /// Gets the EditCommand.
        /// </summary>
        public RelayCommand<int> EditCommand
        {
            get
            {
                return _editCommand
                    ?? (_editCommand = new RelayCommand<int>(ExecuteEditCommand));
            }
        }

        private void ExecuteEditCommand(int index)
        {
            if (index < 0 || index >= PasswordList.Count) return;
            new AddView().Show();
            Messenger.Default.Send(new NotificationMessageAction<PasswordItem>(PasswordList[index], null, item =>
            {
                DatabaseHelper.Open();
                var row = DatabaseHelper.Update<PasswordItem>(
                    "Name = @name, Url = @url, Email = @email, Number = @number, Password = @password, Mark = @mark", 
                    $"Id = {item.Id}",
                        new SQLiteParameter("@name", item.Name),
                        new SQLiteParameter("@url", item.Url),
                        new SQLiteParameter("@email", item.Email),
                        new SQLiteParameter("@number", item.Number),
                        new SQLiteParameter("@password", item.Password),
                        new SQLiteParameter("@mark", item.Mark));
                DatabaseHelper.Close();
                if (row > 0)
                {
                    PasswordList[index] = item;
                    _showMessage("已成功修改一条数据！");
                }
            }), "add");
        }

        private RelayCommand _addCommand;

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand(ExecuteAddCommand));
            }
        }

        private void ExecuteAddCommand()
        {
            new AddView().Show();
            Messenger.Default.Send(new NotificationMessageAction<PasswordItem>(null, item =>
            {
                DatabaseHelper.Open();
                var id = DatabaseHelper.InsertId<PasswordItem>(
                    "Name, Url, Email, Number, Password, Mark",
                    "@name, @url, @email, @number, @password, @mark",
                        new SQLiteParameter("@name", item.Name),
                        new SQLiteParameter("@url", item.Url),
                        new SQLiteParameter("@email", item.Email),
                        new SQLiteParameter("@number", item.Number),
                        new SQLiteParameter("@password", item.Password),
                        new SQLiteParameter("@mark", item.Mark));
                DatabaseHelper.Close();
                if (id > 0)
                {
                    item.Id = id;
                    PasswordList.Add(item);
                    _showMessage("已成功添加一条数据！");
                }
            }), "add");
        }

        private RelayCommand<int> _copyNameCommand;

        /// <summary>
        /// Gets the CopyNameCommand.
        /// </summary>
        public RelayCommand<int> CopyNameCommand
        {
            get
            {
                return _copyNameCommand
                    ?? (_copyNameCommand = new RelayCommand<int>(ExecuteCopyNameCommand));
            }
        }

        private void ExecuteCopyNameCommand(int index)
        {
            if (index < 0 || index >= PasswordList.Count) return;
            LocalHelper.Copy(PasswordList[index].Name);
            _showMessage("复制账号成功！");
        }

        private RelayCommand<int> _copyEmailCommand;

        /// <summary>
        /// Gets the CopyNameCommand.
        /// </summary>
        public RelayCommand<int> CopyEmailCommand
        {
            get
            {
                return _copyEmailCommand
                    ?? (_copyEmailCommand = new RelayCommand<int>(ExecuteCopyEmailCommand));
            }
        }

        private void ExecuteCopyEmailCommand(int index)
        {
            if (index < 0 || index >= PasswordList.Count) return;
            LocalHelper.Copy(PasswordList[index].Email);
            _showMessage("复制邮箱成功！");
        }


        private RelayCommand<int> _copyNumberCommand;

        /// <summary>
        /// Gets the CopyNumberCommand.
        /// </summary>
        public RelayCommand<int> CopyNumberCommand
        {
            get
            {
                return _copyNumberCommand
                    ?? (_copyNumberCommand = new RelayCommand<int>(ExecuteCopyNumberCommand));
            }
        }

        private void ExecuteCopyNumberCommand(int index)
        {
            if (index < 0 || index >= PasswordList.Count) return;
            LocalHelper.Copy(PasswordList[index].Number);
            _showMessage("复制手机号成功！");
        }

        private RelayCommand<int> _copyPasswordCommand;

        /// <summary>
        /// Gets the CopyNameCommand.
        /// </summary>
        public RelayCommand<int> CopyPasswordCommand
        {
            get
            {
                return _copyPasswordCommand
                    ?? (_copyPasswordCommand = new RelayCommand<int>(ExecuteCopyPasswordCommand));
            }
        }

        private void ExecuteCopyPasswordCommand(int index)
        {
            if (index < 0 || index >= PasswordList.Count) return;
            LocalHelper.Copy(PasswordList[index].Password);
            _showMessage("复制密码成功！");
        }


        private RelayCommand<int> _openWebCommand;

        /// <summary>
        /// Gets the OpenWebCommand.
        /// </summary>
        public RelayCommand<int> OpenWebCommand
        {
            get
            {
                return _openWebCommand
                    ?? (_openWebCommand = new RelayCommand<int>(ExecuteOpenWebCommand));
            }
        }

        private void ExecuteOpenWebCommand(int index)
        {
            if (index < 0 || index >= PasswordList.Count) return;
            _showMessage(LocalHelper.OpenBrowser(PasswordList[index].Url) ? "已打开浏览器！" : "打开浏览器失败！");
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

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}