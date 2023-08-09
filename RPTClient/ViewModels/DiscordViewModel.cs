using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
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

public partial class DiscordViewModel : ObservableObject, INavigationAware
{
    // internal
    private bool _isInitialized;
    private Thread? _discordThread = null;
    private DiscordRepository? _discordRepository = null;

    // properties
    [ObservableProperty] private UserSettings _userSettings = new UserSettings();
    [ObservableProperty] private bool _uploadRadioButtonStatus = false;

    // services
    private readonly ISettingsService _settingsService;

    public DiscordViewModel()
    {
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
        LoadSettings();
        _isInitialized = true;
    }

    private void StartupDiscord()
    {
        if(UserSettings.DiscordWebhookUrl != null)
        {
            _discordRepository = new DiscordRepository(UserSettings.DiscordWebhookUrl);
            _discordRepository.MainAsync().GetAwaiter().GetResult();
        }
        else
        {
            // todo: Popup that no discord webhook is set.
        }
    }

    public void ManageDiscordIntegration(bool activate)
    {
        if(activate)
        {
            if (_discordThread is null)
            {
                _discordThread = new Thread(StartupDiscord);
                _discordThread.Start();
            }
            else
            {
                if(_discordThread.ThreadState == ThreadState.Stopped)
                {
                    _discordThread = new Thread(StartupDiscord);
                    _discordThread.Start();
                }
                else
                {
                    throw new Exception("Discord integration is already running.");
                }
            }
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
            // _dialogControl.Show("Error", "An error occurred while trying to load the settings:\n" + e);
        }
    }
}