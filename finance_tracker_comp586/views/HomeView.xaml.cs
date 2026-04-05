using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
    public partial class HomeView : Page
    {
        public HomeView()
        {
            InitializeComponent();
            DataContext = App.CurrentUser;
        }

        private void Wallet_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new WalletView());
        }

        private void Savings_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new SavingsView());
        }

        private void Brokerage_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new BrokerageView());
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomeView());
        }

        private void LogOff_Button_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService?.Navigate(new LoginView());
        }
    }
}
