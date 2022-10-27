using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using RPTClient.Models;
using RPTClient.Repositories;
using RPTClient.Services;
using RPTClient.Services.Contracts;
using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
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

        [ObservableProperty]
        private string _apiStatusText = String.Empty;

        [ObservableProperty]
        private string _uploadButtonText = String.Empty;

        [ObservableProperty]
        private Visibility _loginBarVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility _uploadAgentBarVisibility = Visibility.Collapsed;

        private UserSettings _userSettings = new UserSettings();

        private bool _uploadOn = false;

        #endregion

        #region services

        private IPageService _pageService;
        private ISnackbarService _snackbarService;
        private ILogDialogService _logDialogService;
        private ISettingsService _settingsService;
        #endregion

        #region controls

        private IDialogControl _dialogControl;

        #endregion

        private PerformanceTrackerRepository _performanceTrackerRepo;

        public DashboardViewModel(IPageService pageService, ISnackbarService snackbarService, IDialogService dialogService)
        {
            _settingsService = new SettingsService();
            if (!_isInitialized)
            {
                InitializeSettings();
                InitializeViewModel();
            }

            _pageService = pageService;
            _snackbarService = snackbarService;
            _dialogControl = dialogService.GetDialogControl();
            _logDialogService = new LogDialogService();
            _performanceTrackerRepo = new PerformanceTrackerRepository();
        }

        private void InitializeViewModel()
        {
            // set labels to frontend components
            RemoteLogsCardFooter = "Remote logs found";
            LocalLogsCardFooter = "Local logs found";
            DiffLogsCardFooter = "Unregistered logs";
            LogRootLocationPlaceholder = String.IsNullOrEmpty(LogRootLocation) ? "No log location selected." : LogRootLocation;
            UsernamePlaceholder = "Username";
            PasswordPlaceholder = "Password";
            LoginButtonText = "Login";
            ArcFolderButtonText = "Select log folder";
            ApiStatusText = "Disconnected";
            UploadButtonText = "Start Uploading";
        }

        /// <summary>
        /// Deserializes the user settings, and if settings exist
        /// set the according properties.
        /// </summary>
        private void InitializeSettings()
        {
            _userSettings = _settingsService.DeserializeSettings();
            LogRootLocation = _userSettings.DefaultArcFolderPath;

            if (!String.IsNullOrEmpty(_userSettings.UsernameEncrypted) || !String.IsNullOrEmpty(_userSettings.PasswordEncrypted))
            {
                UsernameValue = _userSettings.Username;
                _passwordValue = _userSettings.Password;
            }
        }

        public void OnNavigatedTo()
        {
            _dialogControl.ButtonRightClick += DialogControlOnButtonRightClick;
            _dialogControl.ButtonLeftClick += DialogControlOnButtonLeftClick;
        }

        public void OnNavigatedFrom()
        {
            _dialogControl.ButtonRightClick -= DialogControlOnButtonRightClick;
            _dialogControl.ButtonLeftClick -= DialogControlOnButtonLeftClick;
        }

        public void SetPasswordValue(string value)
        {
            _passwordValue = value;
        }

        private static void DialogControlOnButtonRightClick(object sender, RoutedEventArgs e)
        {
            var dialogControl = (IDialogControl)sender;
            dialogControl.Hide();
        }

        private static void DialogControlOnButtonLeftClick(object sender, RoutedEventArgs e)
        {
            var dialogControl = (IDialogControl)sender;
            System.Windows.Clipboard.SetText(dialogControl.Message);
        }

        [ICommand]
        private void OnOpenFileDialog()
        {
            try
            {
                LogRootLocation = _logDialogService.OpenArcFolderDialog();
            }
            catch (Exception e)
            {
                _dialogControl.Show("Error", "An error occurred while trying to set the arc folder path:\n" + e.ToString());
            }
        }

        [ICommand]
        private async void OnLogin()
        {
            try
            {
                // Save new user settings.
                _userSettings.Password = _passwordValue;
                _userSettings.Username = UsernameValue;
                _userSettings.DefaultArcFolderPath = LogRootLocation;

                // Do an async login request to the server.
                _performanceTrackerRepo.Login(UsernameValue, _passwordValue);
                _passwordValue = string.Empty;

                // Show notification about successful login.
                ApiStatusText = "Connected";
                _snackbarService.Show("Login Service", "Successfully logged in.", SymbolRegular.AccessibilityCheckmark24);

                // Hide login UI elements. Show UI elements about connection to server.
                LoginBarVisibility = Visibility.Collapsed;
                UploadAgentBarVisibility = Visibility.Visible;

                var task = _performanceTrackerRepo.GetRemoteLogCount();
                RemoteLogCounter = await task;

                _settingsService.SerializeSettings(_userSettings);
            }
            catch (Exception e)
            {
                _dialogControl.Show("Error", "An error occurred while trying to log you in:\n" + e.ToString());
            }
        }

        [ICommand]
        private void OnUpload()
        {
            if (_uploadOn)
            {
                _performanceTrackerRepo.StopFSWatcher();
                _uploadOn = false;

                UploadButtonText = "Start Uploading";
                _snackbarService.Show("Upload Service", "Stopped to monitoring changes in your log directory.", SymbolRegular.Eye24);

                return;
            }

            if (_logRootLocation == String.Empty)
            {
                _snackbarService.Show("Upload Service", "You need to specify your local arcdps log directory.", SymbolRegular.ErrorCircle24);
                return;
            }
            else if (!Directory.Exists(_logRootLocation))
            {
                _snackbarService.Show("Upload Service", "Your specified arcdps log directory does not exits.", SymbolRegular.ErrorCircle24);
                return;
            }

            _performanceTrackerRepo.StartFSWatcher(_logRootLocation);
            _uploadOn = true;
            UploadButtonText = "Stop Uploading";

            _snackbarService.Show("Upload Service", "Starting to monitoring changes in your log directory.", SymbolRegular.Eye24);
        }
    }
}
