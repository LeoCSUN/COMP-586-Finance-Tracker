namespace finance_tracker_comp586
{
    using System.Net.Http;
    using System.Text.Json;

    public class StockApiService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;

        public StockApiService(string apiKey)
        {
            this.httpClient = new HttpClient();
            this.apiKey = apiKey;
        }

        public async Task<decimal> GetCurrentPriceAsync(string symbol)
        {
            string url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}";

            string json = await httpClient.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(json);

            string priceText = doc.RootElement
                .GetProperty("Global Quote")
                .GetProperty("05. price")
                .GetString()!;

            return decimal.Parse(priceText);
        }

        public async Task<Dictionary<DateTime, decimal>> GetDailyClosesAsync(string symbol)
        {
            string url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={symbol}&outputsize=full&apikey={apiKey}";

            string json = await httpClient.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(json);

            JsonElement series = doc.RootElement.GetProperty("Time Series (Daily)");

            Dictionary<DateTime, decimal> closes = new();

            foreach (JsonProperty day in series.EnumerateObject())
            {
                DateTime date = DateTime.Parse(day.Name);
                string closeText = day.Value.GetProperty("4. close").GetString()!;
                closes[date] = decimal.Parse(closeText);
            }

            return closes;
        }

        public async Task<double> GetPercentReturnAsync(string symbol, int daysBack)
        {
            var closes = await GetDailyClosesAsync(symbol);

            var ordered = closes
                .OrderByDescending(x => x.Key)
                .ToList();

            if (ordered.Count == 0)
            {
                throw new InvalidOperationException("No stock data returned.");
            }

            decimal latestPrice = ordered[0].Value;

            DateTime targetDate = ordered[0].Key.AddDays(-daysBack);

            var olderPoint = ordered
                .Where(x => x.Key <= targetDate)
                .OrderByDescending(x => x.Key)
                .FirstOrDefault();

            if (olderPoint.Equals(default(KeyValuePair<DateTime, decimal>)))
            {
                throw new InvalidOperationException("Not enough historical data.");
            }

            decimal oldPrice = olderPoint.Value;

            double percentReturn = (double)((latestPrice - oldPrice) / oldPrice * 100m);
            return percentReturn;
        }

        public async Task<string> GetStockSectorAsync(string symbol)
        {
            string url = $"https://www.alphavantage.co/query?function=OVERVIEW&symbol={symbol}&apikey={apiKey}";

            try
            {
                string json = await httpClient.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("Sector", out JsonElement sectorElement))
                {
                    return sectorElement.GetString() ?? "Other";
                }

                return "Unknown";
            }
            catch (Exception)
            {
                return "Other";
            }
        }
    }
}