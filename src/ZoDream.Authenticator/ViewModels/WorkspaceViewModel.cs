using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using ZoDream.Authenticator.Dialogs;
using ZoDream.Authenticator.Pages;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    internal class WorkspaceViewModel : BindableBase
    {

        public WorkspaceViewModel()
        {
            GroupCommand = new RelayCommand<string>(TapGroup);
            LoadAsync();
        }

        private readonly AppViewModel _app = App.ViewModel;
        private CancellationTokenSource _cancellation = new();

        private ObservableCollection<GroupItemViewModel> _groupItems = [];

        public ObservableCollection<GroupItemViewModel> GroupItems {
            get => _groupItems;
            set => Set(ref _groupItems, value);
        }

        private ObservableCollection<GroupItemViewModel> _bottomItems = [new("添加分组", "\uE82E", "__add")];

        public ObservableCollection<GroupItemViewModel> BottomItems {
            get => _bottomItems;
            set => Set(ref _bottomItems, value);
        }

        private int _progress;

        public int Progress {
            get => _progress;
            set => Set(ref _progress, value);
        }

        private string _message = string.Empty;

        public string Message {
            get => _message;
            set => Set(ref _message, value);
        }

        public ICommand GroupCommand { get; private set; }

        private void TapGroup(string tag)
        {
            if (tag.StartsWith("home_"))
            {
                _app.Navigate<EntryPage>(int.Parse(tag[5..]));
                return;
            }
            switch (tag)
            {
                case "__add":
                    TapAddGroup();
                    break;
                case "__setting":
                    TapSetting();
                    break;
                default:
                    break;
            }
        }

        private async void TapAddGroup()
        {
            var dialog = new GroupDialog();
            var model = dialog.ViewModel;
            model.GroupItems = [.. GroupItems];
            if (!await _app.OpenFormAsync(dialog))
            {
                return;
            }
            var data = new GroupItemViewModel()
            {
                Name = model.Name,
                ParentId = model.ParentIndex
            };
            _app.Database?.Insert(data);
            GroupItems[model.ParentIndex].Children.Add(data);
        }

        private void TapSetting()
        {

        }

        public void CopyText(string text, int timeout = 60)
        {
            _cancellation.Cancel();
            _cancellation = new();
            var package = new DataPackage();
            package.SetText(text);
            Clipboard.SetContentWithOptions(package, new()
            {
                IsAllowedInHistory = false,
                IsRoamable = false,
            });
            Message = $"已复制，有效期：{timeout}s";
            Progress = 100;
            var token = _cancellation.Token;
            Task.Factory.StartNew(() => {
                var val = timeout;
                while (val > 0)
                {
                    Thread.Sleep(1000);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    val -= 1;
                    _app.DispatcherQueue.TryEnqueue(() => {
                        Progress = val * 100 / timeout;
                    });
                }
                _app.DispatcherQueue.TryEnqueue(() => {
                    Message = string.Empty;
                });
                Clipboard.Clear();
            }, token);
        }

        private void LoadAsync()
        {
            if (_app.Database is null)
            {
                return;
            }
            GroupItems.Clear();
            var items = _app.Database.Fetch<GroupItemViewModel>();
            GroupItems.Add(new("私人", "\uE705", "home_0"));
            GroupItems.Add(new("工作", "\uEC64", "home_1"));
            GroupItems.Add(new("验证器", "\uEE6F", "home_2"));
            GroupItems.Add(new("文件", "\uE8B7", "home_3"));
            foreach (var item in items)
            {
                if (GroupItems.Count > item.ParentId)
                {
                    item.Tag = "home_" + item.Id;
                    GroupItems[item.ParentId].Children.Add(item);
                }
            }
        }
    }
}
