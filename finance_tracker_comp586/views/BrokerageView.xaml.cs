using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace finance_tracker_comp586.views
{
    public partial class BrokerageView : Page
    {
        public ObservableCollection<OwnedStockDto> OwnedStocks { get; set; } = new();
        public ObservableCollection<StockSearchResultDto> SearchResults { get; set; } = new();

        public BrokerageView()
        {
            InitializeComponent();

            OwnedStocksListBox.ItemsSource = OwnedStocks;
            SearchResultsListView.ItemsSource = SearchResults;

            OwnedStocks.Add(new OwnedStockDto { Stock = new StockDto { Symbol = "AAPL", Name = "Apple Inc." }, Shares = 10, AvgPrice = 150 });
            OwnedStocks.Add(new OwnedStockDto { Stock = new StockDto { Symbol = "MSFT", Name = "Microsoft Corp." }, Shares = 5, AvgPrice = 280 });
        }

        private void SearchStock_Click(object sender, RoutedEventArgs e)
        {
            string symbol = StockSearchBox.Text.Trim();

            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Please enter a stock symbol to search.");
                return;
            }

            SearchResults.Clear();

            if (symbol.ToUpper() == "AAPL")
            {
                SearchResults.Add(new StockSearchResultDto
                {
                    Symbol = "AAPL",
                    Name = "Apple Inc.",
                    Price = 153.25,
                    Change = "+1.25"
                });
            }
            else if (symbol.ToUpper() == "MSFT")
            {
                SearchResults.Add(new StockSearchResultDto
                {
                    Symbol = "MSFT",
                    Name = "Microsoft Corp.",
                    Price = 281.50,
                    Change = "-0.75"
                });
            }
            else
            {
                MessageBox.Show($"No results found for symbol '{symbol}'.");
            }
        }

        private void StockSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchStock_Click(sender, e);
            }
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomeView());
        }
    }

    public class OwnedStockDto
    {
        public StockDto Stock { get; set; } = new();
        public int Shares { get; set; }
        public double AvgPrice { get; set; }
    }

    public class StockDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class StockSearchResultDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Change { get; set; } = string.Empty;
    }
}