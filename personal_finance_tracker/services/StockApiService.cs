using System.Net.Http;
using System.Text.Json;

namespace personal_finance_tracker
{
    public enum PriceRange { OneDay, FiveDay, OneMonth, SixMonth, YTD, OneYear, FiveYear, Max }
    public class StockApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string     _apiKey;

        public StockApiService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey     = apiKey;
        }

        public async Task<decimal> GetCurrentPriceAsync(string symbol)
        {
            string json = await _httpClient.GetStringAsync(
                $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}");

            using JsonDocument doc = JsonDocument.Parse(json);
            return decimal.Parse(doc.RootElement
                .GetProperty("Global Quote")
                .GetProperty("05. price")
                .GetString()!);
        }

        public async Task<Dictionary<DateTime, decimal>> GetDailyClosesAsync(string symbol)
        {
            string json = await _httpClient.GetStringAsync(
                $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={symbol}&outputsize=full&apikey={_apiKey}");

            using JsonDocument doc  = JsonDocument.Parse(json);
            JsonElement        series = doc.RootElement.GetProperty("Time Series (Daily)");
            Dictionary<DateTime, decimal> closes = new();

            foreach (JsonProperty day in series.EnumerateObject())
                closes[DateTime.Parse(day.Name)] = decimal.Parse(day.Value.GetProperty("4. close").GetString()!);

            return closes;
        }

        public async Task<double> GetPercentReturnAsync(string symbol, int daysBack)
        {
            var ordered = (await GetDailyClosesAsync(symbol))
                .OrderByDescending(x => x.Key)
                .ToList();

            if (ordered.Count == 0)
                throw new InvalidOperationException("No stock data returned.");

            decimal  latestPrice = ordered[0].Value;
            DateTime targetDate  = ordered[0].Key.AddDays(-daysBack);

            var olderPoint = ordered
                .Where(x => x.Key <= targetDate)
                .OrderByDescending(x => x.Key)
                .FirstOrDefault();

            if (olderPoint.Equals(default(KeyValuePair<DateTime, decimal>)))
                throw new InvalidOperationException("Not enough historical data.");

            return (double)((latestPrice - olderPoint.Value) / olderPoint.Value * 100m);
        }

        public async Task<string> GetStockSectorAsync(string symbol)
        {
            try
            {
                string json = await _httpClient.GetStringAsync(
                    $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey={_apiKey}");

                using JsonDocument doc = JsonDocument.Parse(json);
                return doc.RootElement.TryGetProperty("Sector", out JsonElement sector)
                    ? sector.GetString() ?? "Other"
                    : "Unknown";
            }
            catch (Exception)
            {
                return "Other";
            }
        }

        public async Task<(string Symbol, string Name, decimal Price, string Change)> SearchStockAsync(string symbol)
        {
            string quoteJson    = await _httpClient.GetStringAsync(
                $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={Uri.EscapeDataString(symbol)}&apikey={_apiKey}");
            string overviewJson = await _httpClient.GetStringAsync(
                $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={Uri.EscapeDataString(symbol)}&apikey={_apiKey}");

            using JsonDocument quoteDoc    = JsonDocument.Parse(quoteJson);
            using JsonDocument overviewDoc = JsonDocument.Parse(overviewJson);

            if (!quoteDoc.RootElement.TryGetProperty("Global Quote", out JsonElement quote)
                || !quote.TryGetProperty("05. price", out JsonElement priceEl)
                || string.IsNullOrWhiteSpace(priceEl.GetString()))
                throw new InvalidOperationException($"Symbol '{symbol}' not found.");

            string  sym    = quote.GetProperty("01. symbol").GetString() ?? symbol.ToUpper();
            decimal price  = decimal.Parse(priceEl.GetString()!, System.Globalization.CultureInfo.InvariantCulture);
            string  change = quote.TryGetProperty("10. change percent", out JsonElement chEl)
                                 ? chEl.GetString() ?? "0%"
                                 : "0%";
            string  name   = overviewDoc.RootElement.TryGetProperty("Name", out JsonElement nameEl)
                                 && !string.IsNullOrWhiteSpace(nameEl.GetString())
                                 ? nameEl.GetString()!
                                 : sym;

            return (sym, name, price, change);
        }

        public async Task<List<(DateTime Time, decimal Price)>> GetPriceSeriesAsync(string symbol, PriceRange range)
        {
            if (range == PriceRange.OneDay || range == PriceRange.FiveDay)
            {
                string interval   = range == PriceRange.OneDay ? "5min" : "60min";
                string outputSize = range == PriceRange.OneDay ? "compact" : "full";
                string json = await _httpClient.GetStringAsync(
                    $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={Uri.EscapeDataString(symbol)}&interval={interval}&outputsize={outputSize}&apikey={_apiKey}");

                using JsonDocument doc = JsonDocument.Parse(json);
                string key = $"Time Series ({interval})";

                if (!doc.RootElement.TryGetProperty(key, out JsonElement series))
                    throw new InvalidOperationException("No intraday data available.");

                var points = series.EnumerateObject()
                    .Select(p => (
                        Time:  DateTime.Parse(p.Name, System.Globalization.CultureInfo.InvariantCulture),
                        Price: decimal.Parse(p.Value.GetProperty("4. close").GetString()!, System.Globalization.CultureInfo.InvariantCulture)))
                    .OrderBy(p => p.Time)
                    .ToList();

                if (range == PriceRange.OneDay)
                {
                    DateTime lastDay = points.Max(p => p.Time).Date;
                    return points.Where(p => p.Time.Date == lastDay).ToList();
                }
                else
                {
                    var tradingDays = points.Select(p => p.Time.Date).Distinct()
                        .OrderByDescending(d => d).Take(5).ToHashSet();
                    return points.Where(p => tradingDays.Contains(p.Time.Date)).OrderBy(p => p.Time).ToList();
                }
            }
            else
            {
                var closes = await GetDailyClosesAsync(symbol);
                var today  = DateTime.Today;
                DateTime cutoff = range switch
                {
                    PriceRange.OneMonth  => today.AddMonths(-1),
                    PriceRange.SixMonth  => today.AddMonths(-6),
                    PriceRange.YTD       => new DateTime(today.Year, 1, 1),
                    PriceRange.OneYear   => today.AddYears(-1),
                    PriceRange.FiveYear  => today.AddYears(-5),
                    PriceRange.Max       => DateTime.MinValue,
                    _                    => today.AddMonths(-1)
                };

                return closes
                    .Where(x => x.Key >= cutoff)
                    .OrderBy(x => x.Key)
                    .Select(x => (x.Key, x.Value))
                    .ToList();
            }
        }
    }
}
