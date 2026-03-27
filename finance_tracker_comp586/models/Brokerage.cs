namespace finance_tracker_comp586
{
    public class Brokerage
    {
        private readonly List<OwnedStock> ownedStocks;
        private readonly List<Stock> availableStocks;
        private readonly StockApiService stockApiService;

        public Brokerage(StockApiService stockApiService, IEnumerable<OwnedStock>? initialOwnedStocks = null)
        {
            this.ownedStocks = initialOwnedStocks?.Select(s => new OwnedStock
            {
                Stock = new Stock(s.Stock.Name, s.Stock.Symbol),
                Shares = s.Shares,
                AvgPrice = s.AvgPrice
            }).ToList() ?? new List<OwnedStock>();
            this.availableStocks = new List<Stock>();
            this.stockApiService = stockApiService;
        }

        public async Task<decimal> GetStockPriceAsync(string symbol)
        {
            return await stockApiService.GetCurrentPriceAsync(symbol);
        }

        public void AddStock(OwnedStock newStock)
        {
            var existing = ownedStocks.FirstOrDefault(s => s.Stock.Symbol == newStock.Stock.Symbol);

            if (existing != null)
            {
                existing.Shares += newStock.Shares;
            }
            else
            {
                ownedStocks.Add(newStock);
            }
        }

        public IReadOnlyList<OwnedStock> OwnedStocks => ownedStocks;
    }

    public class Stock
    {
        public string Name { get; }
        public string Symbol { get; }

        public Stock(string name, string symbol)
        {
            this.Name = name;
            this.Symbol = symbol;
        }

        public async Task<decimal> GetPriceAsync(StockApiService api)
            => await api.GetCurrentPriceAsync(Symbol);

        public async Task<double> DayReturnAsync(StockApiService api)
            => await api.GetPercentReturnAsync(Symbol, 1);

        public async Task<double> MonthReturnAsync(StockApiService api)
            => await api.GetPercentReturnAsync(Symbol, 30);

        public async Task<double> SixMonthReturnAsync(StockApiService api)
            => await api.GetPercentReturnAsync(Symbol, 180);

        public async Task<double> YearReturnAsync(StockApiService api)
            => await api.GetPercentReturnAsync(Symbol, 365);

        public async Task<double> FiveYearReturnAsync(StockApiService api)
            => await api.GetPercentReturnAsync(Symbol, 365 * 5);

        public async Task<double> ChangeMaxAsync(StockApiService api)
        {
            var closes = await api.GetDailyClosesAsync(Symbol);
            var ordered = closes.OrderBy(x => x.Key).ToList();

            if (ordered.Count < 2)
            {
                throw new InvalidOperationException("Not enough historical data.");
            }

            decimal first = ordered.First().Value;
            decimal last = ordered.Last().Value;

            return (double)((last - first) / first * 100m);
        }
    }

    public class OwnedStock
    {
        public Stock Stock { get; set; }
        public int Shares { get; set; }
        public decimal AvgPrice { get; set; }
    }
}