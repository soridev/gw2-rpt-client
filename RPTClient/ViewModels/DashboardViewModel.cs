using System;
using System.IO;
using System.Windows;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using RPTClient.Models;
using RPTClient.Repositories;
using RPTClient.Services;
using RPTClient.Services.Contracts;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;
using Clipboard = System.Windows.Clipboard;

namespace RPTClient.ViewModels;

public partial class DashboardViewModel : ObservableObject, INavigationAware
{
    #region controls

    private readonly IDialogControl _dialogControl;

    #endregion

    private readonly bool _isInitialized = false;

    private readonly PerformanceTrackerRepository _performanceTrackerRepo;

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

    private void InitializeViewModel()
    {
        // set labels to frontend components
        RemoteLogsCardFooter = "Remote logs found";
        LocalLogsCardFooter = "Local logs found";
        DiffLogsCardFooter = "Unregistered logs";
        LogRootLocationPlaceholder =
            string.IsNullOrEmpty(LogRootLocation) ? "No log location selected." : LogRootLocation;
        UsernamePlaceholder = "Username";
        PasswordPlaceholder = "Password";
        LoginButtonText = "Login";
        ArcFolderButtonText = "Select log folder";
        ApiStatusText = "Disconnected";
        UploadButtonText = "Start Uploading";
    }

    /// <summary>
    ///     Deserializes the user settings, and if settings exist
    ///     set the according properties.
    /// </summary>
    private void InitializeSettings()
    {
        _userSettings = _settingsService.DeserializeSettings();
        LogRootLocation = _userSettings.DefaultArcFolderPath;

        if (!string.IsNullOrEmpty(_userSettings.UsernameEncrypted) ||
            !string.IsNullOrEmpty(_userSettings.PasswordEncrypted))
        {
            UsernameValue = _userSettings.Username;
            _passwordValue = _userSettings.Password;
        }
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
        Clipboard.SetText(dialogControl.Message);
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
            _dialogControl.Show("Error", "An error occurred while trying to set the arc folder path:\n" + e);
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
            _dialogControl.Show("Error", "An error occurred while trying to log you in:\n" + e);
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
            _snackbarService.Show("Upload Service", "Stopped to monitoring changes in your log directory.",
                SymbolRegular.Eye24);

            return;
        }

        if (_logRootLocation == string.Empty)
        {
            _snackbarService.Show("Upload Service", "You need to specify your local arcdps log directory.",
                SymbolRegular.ErrorCircle24);
            return;
        }

        if (!Directory.Exists(_logRootLocation))
        {
            _snackbarService.Show("Upload Service", "Your specified arcdps log directory does not exits.",
                SymbolRegular.ErrorCircle24);
            return;
        }

        _performanceTrackerRepo.StartFSWatcher(_logRootLocation);
        _uploadOn = true;
        UploadButtonText = "Stop Uploading";

        _snackbarService.Show("Upload Service", "Starting to monitoring changes in your log directory.",
            SymbolRegular.Eye24);
    }

    #region properties

    [ObservableProperty] private string _usernamePlaceholder = string.Empty;

    [ObservableProperty] private string _usernameValue = string.Empty;

    [ObservableProperty] private string _passwordPlaceholder = string.Empty;

    private string _passwordValue = string.Empty;

    [ObservableProperty] private string _loginButtonText = string.Empty;

    [ObservableProperty] private string _arcFolderButtonText = string.Empty;

    [ObservableProperty] private string _logRootLocationPlaceholder = string.Empty;

    [ObservableProperty] private string _remoteLogsCardFooter = string.Empty;

    [ObservableProperty] private string _localLogsCardFooter = string.Empty;

    [ObservableProperty] private string _diffLogsCardFooter = string.Empty;

    [ObservableProperty] private int _remoteLogCounter;

    [ObservableProperty] private int _localLogCounter;

    [ObservableProperty] private int _diffLogCounter;

    [ObservableProperty] private string _logRootLocation = string.Empty;

    [ObservableProperty] private string _apiStatusText = string.Empty;

    [ObservableProperty] private string _uploadButtonText = string.Empty;

    [ObservableProperty] private Visibility _loginBarVisibility = Visibility.Visible;

    [ObservableProperty] private Visibility _uploadAgentBarVisibility = Visibility.Collapsed;

    private UserSettings _userSettings = new();

    private bool _uploadOn;

    #endregion

    #region services

    private IPageService _pageService;
    private readonly ISnackbarService _snackbarService;
    private readonly ILogDialogService _logDialogService;
    private readonly ISettingsService _settingsService;

    #endregion
}