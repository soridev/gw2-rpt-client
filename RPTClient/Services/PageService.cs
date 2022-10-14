using Microsoft.Win32;
using System;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace RPTClient.Services
{
    /// <summary>
    /// Service that provides pages for navigation.
    /// </summary>
    public class PageService : IPageService
    {
        /// <summary>
        /// Service which provides the instances of pages.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates new instance and attaches the <see cref="IServiceProvider"/>.
        /// </summary>
        public PageService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public T? GetPage<T>() where T : class
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException("The page should be a WPF control.");

            return (T?)_serviceProvider.GetService(typeof(T));
        }

        /// <inheritdoc />
        public FrameworkElement? GetPage(Type pageType)
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
                throw new InvalidOperationException("The page should be a WPF control.");

            return _serviceProvider.GetService(pageType) as FrameworkElement;
        }        

        /// <summary>
        /// Opens a dialog to select a directory.
        /// </summary>
        /// <returns>Full path to selected directory.</returns>
        public string OpenFolderDialog()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            string intialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\Guild Wars 2\\addons\\arcdps\\arcdps.cbtlogs";
            dialog.InitialDirectory = intialDirectory;
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return String.Empty;
        }
    }
}
