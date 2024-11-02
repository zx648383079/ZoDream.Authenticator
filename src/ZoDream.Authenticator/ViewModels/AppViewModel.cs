﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    internal partial class AppViewModel
    {
        public AppViewModel()
        {
            BackCommand = new RelayCommand(_ => {
                NavigateBack();
            });
        }

        public SettingContainer Setting { get; } = new();

        public async Task InitializeAsync()
        {
            await Setting.LoadAsync();
        }
    }
}
