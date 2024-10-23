﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Authenticator.Pages;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Authenticator.ViewModels
{
    internal class StartupViewModel : BindableBase
    {

        public StartupViewModel()
        {
            EnterCommand = new RelayCommand(TapEnter);
        }

        private string _account = string.Empty;

        public string Account {
            get => _account;
            set => Set(ref _account, value);
        }

        private string _password = string.Empty;

        public string Password {
            get => _password;
            set => Set(ref _password, value);
        }



        public ICommand EnterCommand { get; private set; }

        private void TapEnter(object? _)
        {
            App.ViewModel.Navigate<WorkspacePage>();
        }
    }
}