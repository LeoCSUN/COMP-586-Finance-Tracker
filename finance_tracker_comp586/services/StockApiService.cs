using System.Net.Http;
using System.Text.Json;

namespace finance_tracker_comp586
{
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
    }
}