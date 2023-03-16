using System.Windows;
using System.Windows.Controls;
using RPTClient.ViewModels;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace RPTClient.Views.Pages;

/// <summary>
///     Interaction logic for DashboardPage.xaml
/// </summary>
public partial class DashboardPage : INavigableView<DashboardViewModel>
{
    public DashboardPage(DashboardViewModel viewModel, IPageService pageService, ISnackbarService snackbarService,
        IDialogService dialogService)
    {
        ViewModel = viewModel;

        InitializeComponent();
    }

    public DashboardViewModel ViewModel { get; }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.SetPasswordValue(((PasswordBox)sender).Password);
    }
}