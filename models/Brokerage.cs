// Brokerage.cs
// Stores user's owned stocks
// Allows user to purchase additional stocks available on the market
// Displays price changes for specific stocks within different time spans
// A stock API will be used to retrieve real-time information about the specific stock

public class Brokerage
{
    private readonly List<Stock> ownedStocks;
    private readonly List<Stock> availableStocks;
    private readonly StockApiService stockApiService;

    public Brokerage(StockApiService stockApiService)
    {
        this.ownedStocks = new List<Stock>();
        this.availableStocks = new List<Stock>();
        this.stockApiService = stockApiService;
    }

    public async Task<decimal> GetStockPriceAsync(string symbol)
    {
        return await stockApiService.GetCurrentPriceAsync(symbol);
    }
}

class Stock
{
    public string Name { get; }
    public string Symbol { get; }

    public Stock(string name, string symbol)
    {
        this.Name = name;
        this.Symbol = symbol;
    }

    // Returns current price/share of stock
    public async Task<decimal> GetPriceAsync(StockApiService api)
        => await api.GetCurrentPriceAsync(Symbol);

    // Returns the daily price change of the stock
    public async Task<double> DayReturnAsync(StockApiService api)
        => await api.GetPercentReturnAsync(Symbol, 1);

    // Returns the monthly price change of the stock
    public async Task<double> MonthReturnAsync(StockApiService api)
        => await api.GetPercentReturnAsync(Symbol, 30);

    // Returns the 6 month price change of the stock
    public async Task<double> SixMonthReturnAsync(StockApiService api)
        => await api.GetPercentReturnAsync(Symbol, 180);

    // Returns the yearly price change of the stock
    public async Task<double> YearReturnAsync(StockApiService api)
        => await api.GetPercentReturnAsync(Symbol, 365);

    // Returns the 5 year price change of the stock
    public async Task<double> FiveYearReturnAsync(StockApiService api)
        => await api.GetPercentReturnAsync(Symbol, 365 * 5);

    // Returns the lifetime price change of the stock
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