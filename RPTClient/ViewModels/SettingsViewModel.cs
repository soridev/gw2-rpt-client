using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using RPTClient.Models;
using RPTClient.Services;
using RPTClient.Services.Contracts;
using System;
using System.Windows.Input;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace RPTClient.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        #region Properties
        [ObservableProperty]
        private string _logRootLocationPlaceholder = String.Empty;

        [ObservableProperty]
        private string _appVersion = String.Empty;

        [ObservableProperty]
        private string _arcFolderButtonText = String.Empty;

        [ObservableProperty]
        private Wpf.Ui.Appearance.ThemeType _currentTheme = Wpf.Ui.Appearance.ThemeType.Unknown;
        [ObservableProperty]
        private UserSettings _userSettings = new UserSettings();
        #endregion
        #region Services
        private ILogDialogService _logDialogService;
        private ISettingsService _settingsService;
        #endregion
        #region controls

        private IDialogControl _dialogControl;

        #endregion

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
            CurrentTheme = Wpf.Ui.Appearance.Theme.GetAppTheme();
            AppVersion = $"RPTClient - {GetAssemblyVersion()}";
            ArcFolderButtonText = "Select folder selected";
            LogRootLocationPlaceholder = "No log location selected.";

            this.LoadSettings();

            _isInitialized = true;
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? String.Empty;
        }

        [ICommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (CurrentTheme == Wpf.Ui.Appearance.ThemeType.Light)
                        break;

                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light);
                    CurrentTheme = Wpf.Ui.Appearance.ThemeType.Light;

                    break;

                default:
                    if (CurrentTheme == Wpf.Ui.Appearance.ThemeType.Dark)
                        break;

                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark);
                    CurrentTheme = Wpf.Ui.Appearance.ThemeType.Dark;

                    break;
            }
        }

        [ICommand]
        private void OnOpenFileDialog()
        {
            try
            {
                _userSettings.DefaultArcFolderPath = _logDialogService.OpenArcFolderDialog();
            }
            catch (Exception e)
            {
                _dialogControl.Show("Error", "An error occurred while trying to set the arc folder path:\n" + e.ToString());
            }
        }

        [ICommand]
        private void SaveSettings()
        {
            try
            {
                _settingsService.SerializeSettings(UserSettings);
            }
            catch (Exception e)
            {
                _dialogControl.Show("Error", "An error occurred while trying to save the settings:\n" + e.ToString());
            }
        }
        
        private void LoadSettings()
        {
            try
            {
                _userSettings = _settingsService.DeserializeSettings();
            }
            catch (Exception e)
            {
                _dialogControl.Show("Error", "An error occurred while trying to load the settings:\n" + e.ToString());
            }
        }
    }
}
