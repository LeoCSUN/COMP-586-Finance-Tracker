using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace finance_tracker_comp586.views
{
    public partial class BrokerageView : Page
    {
        // Collections bound to the ListViews
        public ObservableCollection<OwnedStockDto> OwnedStocks { get; set; } = new();
        public ObservableCollection<StockSearchResultDto> SearchResults { get; set; } = new();

        public BrokerageView()
        {
            InitializeComponent();

            // Bind collections to ListViews
            OwnedStocksListBox.ItemsSource = OwnedStocks;
            SearchResultsListView.ItemsSource = SearchResults;

            // Example: populate owned stocks (replace with real data)
            OwnedStocks.Add(new OwnedStockDto { Stock = new StockDto { Symbol = "AAPL", Name = "Apple Inc." }, Shares = 10, AvgPrice = 150 });
            OwnedStocks.Add(new OwnedStockDto { Stock = new StockDto { Symbol = "MSFT", Name = "Microsoft Corp." }, Shares = 5, AvgPrice = 280 });
        }

        // Click handler for the Search button
        private void SearchStock_Click(object sender, RoutedEventArgs e)
        {
            string symbol = StockSearchBox.Text.Trim();

            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Please enter a stock symbol to search.");
                return;
            }

            // Clear previous search results
            SearchResults.Clear();

            // TODO: Replace with actual search logic / API call
            // Example: dummy search
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

        // Press Enter in search box to trigger search
        private void StockSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchStock_Click(sender, e);
            }
        }

        // Home button click
        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement navigation to Home page
            MessageBox.Show("Navigate to Home page");
        }
    }

    // DTOs for demo purposes
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