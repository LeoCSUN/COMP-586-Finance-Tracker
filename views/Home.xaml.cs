using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace finance_tracker_comp586.views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private finance_tracker_comp586.User user;
        public Home()
        {
            InitializeComponent();
            DataContext = user;
        }

        private void Wallet_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Wallet());
        }

        private void Savings_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Savings());
        }

        private void Brokerage_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Brokerage());
        }
    }
}
