using RPTClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Common.Interfaces;

namespace RPTClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for DiscordPage.xaml
    /// </summary>
    public partial class DiscordPage : INavigableView<DiscordViewModel>
    {
        public DiscordPage(DiscordViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        public DiscordViewModel ViewModel { get; }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.ManageDiscordIntegration(true);
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.ManageDiscordIntegration(false);
        }
    }
}
