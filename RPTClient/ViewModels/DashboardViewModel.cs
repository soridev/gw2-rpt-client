using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using RPTClient.Models;
using RPTClient.Repositories;
using RPTClient.Services;
using RPTClient.Services.Contracts;
using System;
using System.Diagnostics;
using System.Security;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace RPTClient.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        #region properties

        [ObservableProperty]
        private string _usernamePlaceholder = String.Empty;

        [ObservableProperty]
        private string _usernameValue = String.Empty;

        [ObservableProperty]
        private string _passwordPlaceholder = String.Empty;

        private string _passwordValue = String.Empty;

        [ObservableProperty]
        private string _loginButtonText = String.Empty;

        [ObservableProperty]
        private string _arcFolderButtonText = String.Empty;

        [ObservableProperty]
        private string _logRootLocationPlaceholder = String.Empty;

        [ObservableProperty]
        private string _remoteLogsCardFooter = String.Empty;

        [ObservableProperty]
        private string _localLogsCardFooter = String.Empty;

        [ObservableProperty]
        private string _diffLogsCardFooter = String.Empty;

        [ObservableProperty]
        private int _remoteLogCounter;

        [ObservableProperty]
        private int _localLogCounter = 0;

        [ObservableProperty]
        private int _diffLogCounter = 0;

        [ObservableProperty]
        private string _logRootLocation = String.Empty;

        #endregion

        #region services

        private IPageService _pageService;
        private ILogDialogService _logDialogService;

        #endregion

        private PerformanceTrackerRepository _performanceTrackerRepo;

        public DashboardViewModel(IPageService pageService)
        {
            _pageService = pageService;
            _logDialogService = new LogDialogService();
            _performanceTrackerRepo = new PerformanceTrackerRepository();

            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        private void InitializeViewModel()
        {
            // set labels to frontend components
            RemoteLogsCardFooter = "Remote logs found";
            LocalLogsCardFooter = "Local logs found";
            DiffLogsCardFooter = "Unregistered logs";
            LogRootLocationPlaceholder = "No log location selected.";
            UsernamePlaceholder = "Username";
            PasswordPlaceholder = "Password";
            LoginButtonText = "Login";
            ArcFolderButtonText = "Select log folder";
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }         

        public void SetPasswordValue(string value)
        {
            _passwordValue = value;
        }

        [ICommand]
        private void OnOpenFileDialog()
        {
            try
            {
                LogRootLocation = _logDialogService.OpenArcFolderDialog();
            }
            catch(Exception e){
                MessageBox box = new MessageBox();
                box.Content = e.ToString();
                box.Show();
            }
        }

        [ICommand]
        private void OnLogin()
        {
            _performanceTrackerRepo.Login(UsernameValue, _passwordValue);
            RemoteLogCounter = _performanceTrackerRepo.GetRemoteLogCount();

            _passwordValue = string.Empty;
        }
    }
}
