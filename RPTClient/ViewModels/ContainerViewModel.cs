using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using RPTClient.Views.Pages;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace RPTClient.ViewModels;

public partial class ContainerViewModel : ObservableObject
{
    [ObservableProperty] private string _applicationTitle = string.Empty;

    private bool _isInitialized;

    [ObservableProperty] private ObservableCollection<INavigationControl> _navigationFooter = new();

    [ObservableProperty] private ObservableCollection<INavigationControl> _navigationItems = new();

    [ObservableProperty] private ObservableCollection<MenuItem> _trayMenuItems = new();

    public ContainerViewModel(INavigationService navigationService)
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        ApplicationTitle = "GW2 Raid Performance Tracker";

        NavigationItems = new ObservableCollection<INavigationControl>
        {
            new NavigationItem
            {
                Content = "Home",
                PageTag = "dashboard",
                Icon = SymbolRegular.Home24,
                PageType = typeof(DashboardPage)
            },
            new NavigationItem
            {
                Content = "Data",
                PageTag = "data",
                Icon = SymbolRegular.DataHistogram24,
                PageType = typeof(DataPage)
            }
        };

        NavigationFooter = new ObservableCollection<INavigationControl>
        {
            new NavigationItem
            {
                Content = "Settings",
                PageTag = "settings",
                Icon = SymbolRegular.Settings24,
                PageType = typeof(SettingsPage)
            }
        };

        TrayMenuItems = new ObservableCollection<MenuItem>
        {
            new()
            {
                Header = "Home",
                Tag = "tray_home"
            }
        };

        _isInitialized = true;
    }
}