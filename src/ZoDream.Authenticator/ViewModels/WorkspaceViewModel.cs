using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
            GroupItems.Add(new("私人", "\uE705", "home"));
            GroupItems.Add(new("工作", "\uEC64", "home"));
            GroupItems.Add(new("验证器", "\uEE6F", "home"));
            GroupItems.Add(new("文件", "\uE8B7", "home"));
        }

        private readonly AppViewModel _app = App.ViewModel;

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


        public ICommand GroupCommand { get; private set; }

        private void TapGroup(string tag)
        {
            Debug.WriteLine(tag);
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
            GroupItems[model.ParentIndex].Children.Add(new(model.Name, "", "_item"));
        }

        private void TapSetting()
        {

        }
    }
}
