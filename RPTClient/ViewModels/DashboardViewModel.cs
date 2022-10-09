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
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _logRootLocation = String.Empty;
        private IPageService _pageService;        

        public DashboardViewModel(IPageService pageService)
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
        }

        private void InitializeViewModel()
        {
            this._pageService = App.GetService<IPageService>();
        }

        public void OnNavigatedTo()
        {
        }

        public void OnNavigatedFrom()
        {
        }         

        [ICommand]
        private void OnOpenFileDialog()
        {                        
            var pageService = (PageService)_pageService;            
            this._logRootLocation = pageService.OpenFolderDialog();
        }
    }
}
