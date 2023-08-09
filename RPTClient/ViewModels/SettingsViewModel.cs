using System;
using System.Reflection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using RPTClient.Models;
using RPTClient.Services;
using RPTClient.Services.Contracts;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace RPTClient.ViewModels;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    
    // internals
    private readonly IDialogControl _dialogControl;    
    private bool _isInitialized;

    // Properties
    [ObservableProperty] private string _logRootLocationPlaceholder = string.Empty;
    [ObservableProperty] private string _logRootLocation = string.Empty;
    [ObservableProperty] private string _discordWebhookUrl = string.Empty;
    [ObservableProperty] private string _appVersion = string.Empty;
    [ObservableProperty] private string _arcFolderButtonText = string.Empty;
    [ObservableProperty] private ThemeType _currentTheme = ThemeType.Unknown;
    [ObservableProperty] private UserSettings _userSettings = new UserSettings();
    
    // Services
    private readonly ILogDialogService _logDialogService;
    private readonly ISettingsService _settingsService;
    
    public SettingsViewModel(IDialogService dialogService)
    {
        _logDialogService = new LogDialogService();
        _dialogControl = dialogService.GetDialogControl();
        _settingsService = new SettingsService();
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        CurrentTheme = Theme.GetAppTheme();
        AppVersion = $"RPTClient - {GetAssemblyVersion()}";
        ArcFolderButtonText = "Select folder selected";
        LogRootLocationPlaceholder = "No log location selected.";

        LoadSettings();

        _isInitialized = true;
    }

    private string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    }

    [ICommand]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (CurrentTheme == ThemeType.Light)
                    break;

                Theme.Apply(ThemeType.Light);
                CurrentTheme = ThemeType.Light;

                break;

            default:
                if (CurrentTheme == ThemeType.Dark)
                    break;

                Theme.Apply(ThemeType.Dark);
                CurrentTheme = ThemeType.Dark;

                break;
        }
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
    private void SaveSettings()
    {
        try
        {
            // set all the values in the settings object.
            UserSettings.DefaultArcFolderPath = LogRootLocation;
            UserSettings.DiscordWebhookUrl = DiscordWebhookUrl;

            _settingsService.SerializeSettings(UserSettings);
        }
        catch (Exception e)
        {
            _dialogControl.Show("Error", "An error occurred while trying to save the settings:\n" + e);
        }
    }

    private void LoadSettings()
    {
        try
        {
            _userSettings = _settingsService.DeserializeSettings();

            if (UserSettings.DefaultArcFolderPath != null)
            {
                LogRootLocation = _userSettings.DefaultArcFolderPath;
            }
            if (!String.IsNullOrEmpty(UserSettings.DiscordWebhookUrl))
            {
                DiscordWebhookUrl = UserSettings.DiscordWebhookUrl;
            }            
        }
        catch (Exception e)
        {
            _dialogControl.Show("Error", "An error occurred while trying to load the settings:\n" + e);
        }
    }    
}