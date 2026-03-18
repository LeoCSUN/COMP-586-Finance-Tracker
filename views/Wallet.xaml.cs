using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
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