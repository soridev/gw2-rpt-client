using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using RPTClient.Models;
using RPTClient.Services;
using System;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Contracts;

namespace RPTClient.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {        
        [ObservableProperty]
        private string _logRootLocation = String.Empty;

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }
        
        public void ShowFlyOut()
        {
            Flyout flyout = new Flyout();
            
        }

        [ICommand]
        private void OnOpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                LogRootLocation = dialog.FileName;
            }
        }
    }
}
