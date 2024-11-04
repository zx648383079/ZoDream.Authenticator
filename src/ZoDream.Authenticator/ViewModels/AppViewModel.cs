using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using ZoDream.Shared.Database;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    internal partial class AppViewModel: IDisposable
    {
        public AppViewModel()
        {
            BackCommand = new RelayCommand(_ => {
                NavigateBack();
            });
        }

        public SettingContainer Setting { get; } = new();
        public IDatabase? Database { get; set; }

       

        public async Task InitializeAsync()
        {
            await Setting.LoadAsync();
        }

        public void Dispose()
        {
            Database?.Dispose();
            Clipboard.Clear();
        }
    }
}
