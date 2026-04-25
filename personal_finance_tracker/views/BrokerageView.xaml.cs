using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using personal_finance_tracker.utils;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace personal_finance_tracker.views
{
    public partial class BrokerageView : Page
    {
        public ObservableCollection<OwnedStockDto> OwnedStocks { get; set; } = new();
        public ObservableCollection<StockSearchResultDto> SearchResults { get; set; } = new();

        private string? _lastSortedProperty = null;
        private ListSortDirection _lastSortDirection = ListSortDirection.Ascending;

        private string? _chartSymbol = null;
        private PriceRange _chartRange = PriceRange.OneDay;

        private readonly ObservableCollection<LegendItem> _diversificationLegend = new();

        public BrokerageView()
        {
            InitializeComponent();
            DataContext = App.CurrentUser;

            OwnedStocksListBox.ItemsSource = OwnedStocks;
            SearchResultsListView.ItemsSource = SearchResults;
            DiversificationLegend.ItemsSource = _diversificationLegend;

            foreach (var s in App.CurrentUser!.GetBrokerage().OwnedStocks)
            {
                OwnedStocks.Add(new OwnedStockDto
                {
                    Stock    = new StockDto { Symbol = s.Stock.Symbol, Name = s.Stock.Name },
                    Shares   = s.Shares,
                    AvgPrice = (double)s.AvgPrice
                });
            }

            UpdateDiversificationChart();
        }

        private async void SearchStock_Click(object sender, RoutedEventArgs e)
        {
            string symbol = StockSearchBox.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(symbol))
            {
                MessageBox.Show("Please enter a stock symbol to search.");
                return;
            }

            SearchResults.Clear();
            SearchButton.IsEnabled = false;

            try
            {
                var (sym, name, price, change) = await App.StockApi.SearchStockAsync(symbol);
                SearchResults.Add(new StockSearchResultDto
                {
                    Symbol = sym,
                    Name   = name,
                    Price  = (double)price,
                    Change = change
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not retrieve data for '{symbol}': {ex.Message}");
            }
            finally
            {
                SearchButton.IsEnabled = true;
            }
        }

        private void StockSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchStock_Click(sender, e);
            }
        }

        private async void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.Button)?.Tag is not StockSearchResultDto result) return;

            var window = new BuyStockWindow(result.Symbol, result.Name, result.Price)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true) return;

            double sharesToBuy;
            if (window.BuyMode == BuyMode.Shares)
            {
                sharesToBuy = (double)window.EnteredAmount;
            }
            else
            {
                sharesToBuy = (double)window.EnteredAmount / result.Price;
            }

            var existing = OwnedStocks.FirstOrDefault(s => s.Stock.Symbol == result.Symbol);
            if (existing != null)
            {
                double totalShares  = existing.Shares + sharesToBuy;
                existing.AvgPrice   = ((existing.Shares * existing.AvgPrice) + (sharesToBuy * result.Price)) / totalShares;
                existing.Shares     = (int)totalShares;
                OwnedStocksListBox.Items.Refresh();
            }
            else
            {
                OwnedStocks.Add(new OwnedStockDto
                {
                    Stock    = new StockDto { Symbol = result.Symbol, Name = result.Name },
                    Shares   = (int)sharesToBuy,
                    AvgPrice = result.Price
                });
            }

            await App.CurrentUser!.GetBrokerage().AddStockAsync(result.Symbol, (int)sharesToBuy, (decimal)result.Price);
            await App.Users.UpdateUser(App.CurrentUser!);
            UpdateDiversificationChart();
        }

        private async void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.Button)?.Tag is not OwnedStockDto stock) return;

            var window = new SellStockWindow(stock.Stock.Symbol, stock.Stock.Name)
            {
                Owner = Window.GetWindow(this)
            };

            if (window.ShowDialog() != true) return;

            int netSharesToRemove;

            if (window.SellMode == SellMode.Shares)
            {
                double sharesToSell = (double)window.EnteredAmount;
                if (sharesToSell > stock.Shares)
                {
                    MessageBox.Show($"You only have {stock.Shares} shares of {stock.Stock.Symbol}.");
                    return;
                }
                netSharesToRemove = (int)sharesToSell;
                stock.Shares -= netSharesToRemove;
            }
            else
            {
                double dollarsToSell = (double)window.EnteredAmount;
                double totalValue    = stock.Shares * stock.AvgPrice;
                if (dollarsToSell > totalValue)
                {
                    MessageBox.Show($"Dollar amount exceeds total position value ({totalValue:C}).");
                    return;
                }
                netSharesToRemove = (int)(dollarsToSell / stock.AvgPrice);
                stock.Shares -= netSharesToRemove;
            }

            if (stock.Shares <= 0)
                OwnedStocks.Remove(stock);
            else
                OwnedStocksListBox.Items.Refresh();

            App.CurrentUser!.GetBrokerage().RemoveStock(stock.Stock.Symbol, netSharesToRemove);
            await App.Users.UpdateUser(App.CurrentUser!);
            UpdateDiversificationChart();
        }

        private void PortfolioHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not GridViewColumnHeader header || header.Column == null) return;

            string? property = (header.Column.DisplayMemberBinding as System.Windows.Data.Binding)?.Path?.Path;
            if (property == null) return;

            ListSortDirection direction = (property == _lastSortedProperty && _lastSortDirection == ListSortDirection.Ascending)
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            var view = System.Windows.Data.CollectionViewSource.GetDefaultView(OwnedStocksListBox.ItemsSource);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(property, direction));

            _lastSortedProperty = property;
            _lastSortDirection  = direction;
        }

        private string? _lastSearchSortedProperty = null;
        private ListSortDirection _lastSearchSortDirection = ListSortDirection.Ascending;

        private void SearchResultsHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not GridViewColumnHeader header || header.Column == null) return;

            string? property = (header.Column.DisplayMemberBinding as System.Windows.Data.Binding)?.Path?.Path;
            if (property == null) return;

            ListSortDirection direction = (property == _lastSearchSortedProperty && _lastSearchSortDirection == ListSortDirection.Ascending)
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            var view = System.Windows.Data.CollectionViewSource.GetDefaultView(SearchResultsListView.ItemsSource);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(property, direction));

            _lastSearchSortedProperty = property;
            _lastSearchSortDirection  = direction;
        }

        private void UpdateDiversificationChart()
        {
            var ownedStocks = App.CurrentUser!.GetBrokerage().OwnedStocks;
            if (ownedStocks.Count == 0)
            {
                DiversificationChart.Visibility  = Visibility.Collapsed;
                NoDiversificationText.Visibility = Visibility.Visible;
                _diversificationLegend.Clear();
                return;
            }

            var (series, legend) = ChartHelper.GetSectorPieSeries(ownedStocks);
            DiversificationChart.Series     = series;
            DiversificationChart.Visibility  = Visibility.Visible;
            NoDiversificationText.Visibility = Visibility.Collapsed;

            _diversificationLegend.Clear();
            foreach (var item in legend)
                _diversificationLegend.Add(item);
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

        private void OwnedStocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OwnedStocksListBox.SelectedItem is OwnedStockDto stock)
            {
                SearchResultsListView.SelectedItem = null;
                _chartSymbol          = stock.Stock.Symbol;
                PriceChartLabel.Text  = stock.Stock.Symbol;
                _ = UpdatePriceChart();
            }
        }

        private void SearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchResultsListView.SelectedItem is StockSearchResultDto result)
            {
                OwnedStocksListBox.SelectedItem = null;
                _chartSymbol          = result.Symbol;
                PriceChartLabel.Text  = result.Symbol;
                _ = UpdatePriceChart();
            }
        }

        private void PriceRange_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is string tag)
            {
                _chartRange = tag switch
                {
                    "1D"  => PriceRange.OneDay,
                    "5D"  => PriceRange.FiveDay,
                    "1M"  => PriceRange.OneMonth,
                    "6M"  => PriceRange.SixMonth,
                    "YTD" => PriceRange.YTD,
                    "1Y"  => PriceRange.OneYear,
                    "5Y"  => PriceRange.FiveYear,
                    "Max" => PriceRange.Max,
                    _     => PriceRange.OneDay
                };
                if (_chartSymbol != null)
                    _ = UpdatePriceChart();
            }
        }

        private async Task UpdatePriceChart()
        {
            if (_chartSymbol == null) return;

            NoPriceDataText.Text       = "Loading...";
            NoPriceDataText.Visibility = Visibility.Visible;
            PriceChart.Visibility      = Visibility.Collapsed;

            try
            {
                var series = await App.StockApi.GetPriceSeriesAsync(_chartSymbol, _chartRange);

                if (series.Count < 2)
                {
                    NoPriceDataText.Text = "Not enough data for this range.";
                    return;
                }

                var plotPoints = series
                    .Select(p => new DateTimePoint(p.Time, (double)p.Price))
                    .ToList();

                string axisFormat = _chartRange switch
                {
                    PriceRange.OneDay   => "HH:mm",
                    PriceRange.FiveDay  => "ddd HH:mm",
                    PriceRange.OneMonth => "MM/dd",
                    PriceRange.SixMonth => "MMM yy",
                    PriceRange.YTD      => "MMM",
                    PriceRange.OneYear  => "MMM yy",
                    PriceRange.FiveYear => "MMM yyyy",
                    PriceRange.Max      => "yyyy",
                    _                   => "MM/dd"
                };

                double minStep = _chartRange switch
                {
                    PriceRange.OneDay   => (double)TimeSpan.FromHours(1).Ticks,
                    PriceRange.FiveDay  => (double)TimeSpan.FromDays(1).Ticks,
                    PriceRange.OneMonth => (double)TimeSpan.FromDays(5).Ticks,
                    PriceRange.SixMonth => (double)TimeSpan.FromDays(14).Ticks,
                    PriceRange.YTD      => (double)TimeSpan.FromDays(14).Ticks,
                    PriceRange.OneYear  => (double)TimeSpan.FromDays(30).Ticks,
                    PriceRange.FiveYear => (double)TimeSpan.FromDays(180).Ticks,
                    PriceRange.Max      => (double)TimeSpan.FromDays(365).Ticks,
                    _                   => (double)TimeSpan.FromDays(1).Ticks
                };

                var stroke = new SKColor(139, 0, 0);

                PriceChart.Series = [new LineSeries<DateTimePoint>
                {
                    Values       = plotPoints,
                    Stroke       = new SolidColorPaint(stroke) { StrokeThickness = 2 },
                    Fill         = new SolidColorPaint(new SKColor(139, 0, 0, 40)),
                    GeometrySize = 0,
                }];

                PriceChart.XAxes = [new Axis
                {
                    Labeler        = v => new DateTime((long)v).ToString(axisFormat),
                    MinStep        = minStep,
                    LabelsRotation = 0
                }];

                PriceChart.YAxes = [new Axis
                {
                    Labeler = v => $"${v:N2}"
                }];

                NoPriceDataText.Visibility = Visibility.Collapsed;
                PriceChart.Visibility      = Visibility.Visible;
            }
            catch
            {
                NoPriceDataText.Text = "Could not load price data.";
            }
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
