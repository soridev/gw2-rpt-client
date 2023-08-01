using System;
using System.Windows;
using System.Windows.Controls;
using RPTClient.ViewModels;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace RPTClient.Views;

/// <summary>
///     Interaction logic for Container.xaml
/// </summary>
public partial class Container : INavigationWindow
{
    public Container(ContainerViewModel viewModel, IPageService pageService, INavigationService navigationService,
        ISnackbarService snackbarService, IDialogService dialogService)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
        SetPageService(pageService);

        navigationService.SetNavigationControl(RootNavigation);

        // Allows you to use the Snackbar control defined in this window in other pages or windows
        snackbarService.SetSnackbarControl(RootSnackbar);

        // Allows you to use the Dialog control defined in this window in other pages or windows
        dialogService.SetDialogControl(RootDialog);
    }

    public ContainerViewModel ViewModel { get; }

    /// <summary>
    ///     Raises the closed event.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Make sure that closing this window will begin the process of closing the application.
        Application.Current.Shutdown();
    }

    #region INavigationWindow methods

    public Frame GetFrame()
    {
        return RootFrame;
    }

    public INavigation GetNavigation()
    {
        return RootNavigation;
    }

    public bool Navigate(Type pageType)
    {
        return RootNavigation.Navigate(pageType);
    }

    public void SetPageService(IPageService pageService)
    {
        RootNavigation.PageService = pageService;
    }

    public void ShowWindow()
    {
        Show();
    }

    public void CloseWindow()
    {
        Close();
    }

    #endregion INavigationWindow methods
}