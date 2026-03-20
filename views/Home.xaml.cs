using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
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
