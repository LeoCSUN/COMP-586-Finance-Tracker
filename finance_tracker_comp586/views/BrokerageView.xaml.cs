using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
    public partial class BrokerageView : Page
    {
        private finance_tracker_comp586.Brokerage brokerage;

        public BrokerageView()
        {
            InitializeComponent();

            if (App.CurrentUser == null)
            {
                MessageBox.Show("Please log in first.");
                NavigationService?.Navigate(new LoginView());
                brokerage = new finance_tracker_comp586.Brokerage(App.StockApi);
                return;
            }

            brokerage = App.CurrentUser.Brokerage;

            LoadOwnedStocks();
        }

        private void LoadOwnedStocks()
        {
            OwnedStocksListBox.ItemsSource = brokerage.OwnedStocks;
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomeView());
        }
    }
}
