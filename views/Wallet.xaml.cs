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
    /// Interaction logic for Wallet.xaml
    /// </summary>
    public partial class Wallet : Page
    {
        private finance_tracker_comp586.Wallet wallet;
        public Wallet()
        {
            InitializeComponent();

            wallet = new finance_tracker_comp586.Wallet();
            DataContext = wallet;
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Home());
        }
    }
}