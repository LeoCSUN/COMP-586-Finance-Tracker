using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using finance_tracker_comp586.models;

namespace finance_tracker_comp586.data
{
    class FirebaseUserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _projectId;

        public FirebaseUserRepository(string projectId)
        {
            _httpClient = new HttpClient();
            _projectId = projectId;
        }

        public User? GetUser(string username)
        {
            string url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{username}";
            var response = _httpClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            string json = response.Content.ReadAsStringAsync().Result;
            using JsonDocument doc = JsonDocument.Parse(json);
            var fields = doc.RootElement.GetProperty("fields");

            string? _username  = fields.GetProperty("username").GetProperty("stringValue").GetString();
            string? hash       = fields.GetProperty("passwordHash").GetProperty("stringValue").GetString();
            string? salt       = fields.GetProperty("salt").GetProperty("stringValue").GetString();
            string? firstName  = fields.GetProperty("firstName").GetProperty("stringValue").GetString();
            string? lastName   = fields.GetProperty("lastName").GetProperty("stringValue").GetString();

            if (string.IsNullOrWhiteSpace(_username)
                || string.IsNullOrWhiteSpace(hash)
                || string.IsNullOrWhiteSpace(salt)
                || string.IsNullOrWhiteSpace(firstName)
                || string.IsNullOrWhiteSpace(lastName))
            {
                return null;
            }

            var ownedStocks = new List<OwnedStock>();

            if (fields.TryGetProperty("brokerage", out JsonElement brokerageElement)
                && brokerageElement.TryGetProperty("mapValue", out JsonElement brokerageMap)
                && brokerageMap.TryGetProperty("fields", out JsonElement brokerageFields)
                && brokerageFields.TryGetProperty("ownedStocks", out JsonElement ownedStocksElement)
                && ownedStocksElement.TryGetProperty("arrayValue", out JsonElement arrayValue)
                && arrayValue.TryGetProperty("values", out JsonElement values))
            {
                foreach (JsonElement entry in values.EnumerateArray())
                {
                    if (!entry.TryGetProperty("mapValue", out JsonElement mapValue)
                        || !mapValue.TryGetProperty("fields", out JsonElement stockFields))
                        continue;

                    string? name   = stockFields.GetProperty("name").GetProperty("stringValue").GetString();
                    string? symbol = stockFields.GetProperty("symbol").GetProperty("stringValue").GetString();

                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(symbol))
                        continue;

                    int shares = 0;
                    if (stockFields.TryGetProperty("shares", out JsonElement sharesEl)
                        && sharesEl.TryGetProperty("integerValue", out JsonElement sharesVal))
                        int.TryParse(sharesVal.GetString(), out shares);

                    decimal avgPrice = 0m;
                    if (stockFields.TryGetProperty("avgPrice", out JsonElement avgEl))
                    {
                        if (avgEl.TryGetProperty("doubleValue", out JsonElement avgDbl))
                            avgPrice = Convert.ToDecimal(avgDbl.GetDouble());
                        else if (avgEl.TryGetProperty("integerValue", out JsonElement avgInt))
                            decimal.TryParse(avgInt.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out avgPrice);
                    }

                    ownedStocks.Add(new OwnedStock
                    {
                        Stock    = new Stock(name, symbol),
                        Shares   = shares,
                        AvgPrice = avgPrice
                    });
                }
            }

            decimal currentAmount    = 0m;
            decimal monthlyBudget    = 0m;
            decimal amountSpentMonth = 0m;
            var transactions = new List<Transaction>();

            if (fields.TryGetProperty("wallet", out JsonElement walletElement)
                && walletElement.TryGetProperty("mapValue", out JsonElement walletMap)
                && walletMap.TryGetProperty("fields", out JsonElement walletFields))
            {
                if (walletFields.TryGetProperty("currentAmount", out JsonElement caEl)
                    && caEl.TryGetProperty("doubleValue", out JsonElement caVal))
                    currentAmount = Convert.ToDecimal(caVal.GetDouble());

                if (walletFields.TryGetProperty("monthlyBudget", out JsonElement mbEl)
                    && mbEl.TryGetProperty("doubleValue", out JsonElement mbVal))
                    monthlyBudget = Convert.ToDecimal(mbVal.GetDouble());

                if (walletFields.TryGetProperty("amountSpentMonth", out JsonElement asmEl)
                    && asmEl.TryGetProperty("doubleValue", out JsonElement asmVal))
                    amountSpentMonth = Convert.ToDecimal(asmVal.GetDouble());

                if (walletFields.TryGetProperty("transactions", out JsonElement txArrayEl)
                    && txArrayEl.TryGetProperty("arrayValue", out JsonElement txArray)
                    && txArray.TryGetProperty("values", out JsonElement txValues))
                {
                    foreach (JsonElement txEl in txValues.EnumerateArray())
                    {
                        if (!txEl.TryGetProperty("mapValue", out JsonElement txMap)
                            || !txMap.TryGetProperty("fields", out JsonElement txFields))
                            continue;

                        string? dateStr = txFields.GetProperty("transactionDate").GetProperty("stringValue").GetString();
                        string? desc    = txFields.GetProperty("description").GetProperty("stringValue").GetString();
                        string? catStr  = txFields.GetProperty("category").GetProperty("stringValue").GetString();
                        decimal amount  = 0m;

                        if (txFields.TryGetProperty("amount", out JsonElement amtEl)
                            && amtEl.TryGetProperty("doubleValue", out JsonElement amtVal))
                            amount = Convert.ToDecimal(amtVal.GetDouble());

                        if (DateTime.TryParse(dateStr, out DateTime date)
                            && !string.IsNullOrWhiteSpace(desc)
                            && Enum.TryParse(catStr, out TransactionCategory category))
                        {
                            transactions.Add(new Transaction(date, desc, amount, category));
                        }
                    }
                }
            }

            return new User(_username, hash, salt, firstName, lastName,
                ownedStocks, currentAmount, monthlyBudget, amountSpentMonth, transactions);
        }

        public void AddUser(User user)
        {
            var ownedStockDtos = user.GetBrokerage().OwnedStocks
                .Select(s => new
                {
                    mapValue = new
                    {
                        fields = new
                        {
                            name      = new { stringValue  = s.Stock.Name },
                            symbol    = new { stringValue  = s.Stock.Symbol },
                            shares    = new { integerValue = s.Shares.ToString() },
                            avgPrice  = new { doubleValue  = (double)s.AvgPrice }
                        }
                    }
                })
                .ToArray();

            string json = JsonSerializer.Serialize(new
            {
                fields = new
                {
                    username     = new { stringValue = user.GetUsername() },
                    passwordHash = new { stringValue = user.GetPasswordHash() },
                    salt         = new { stringValue = user.GetSalt() },
                    firstName    = new { stringValue = user.FirstName() },
                    lastName     = new { stringValue = user.LastName() },
                    brokerage = new
                    {
                        mapValue = new
                        {
                            fields = new
                            {
                                ownedStocks = new { arrayValue = new { values = ownedStockDtos } }
                            }
                        }
                    }
                }
            });

            string url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users?documentId={user.GetUsername()}";
            var content  = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(url, content).Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to add user to Firebase: " + response.Content.ReadAsStringAsync().Result);
        }

        public void RemoveUser(string username)
        {
            string url     = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{username}";
            var response = _httpClient.DeleteAsync(url).Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to delete user: " + response.Content.ReadAsStringAsync().Result);
        }

        public async Task UpdateUser(User user)
        {
            Wallet wallet = user.GetWallet();

            var firestoreTransactions = wallet.Transactions
                .Select(t => new
                {
                    mapValue = new
                    {
                        fields = new
                        {
                            transactionDate = new { stringValue  = t.TransactionDate.ToString("o") },
                            description     = new { stringValue  = t.Description },
                            amount          = new { doubleValue  = (double)t.Amount },
                            category        = new { stringValue  = t.Category.ToString() }
                        }
                    }
                })
                .ToArray();

            var firestoreOwnedStocks = user.GetBrokerage().OwnedStocks
                .Select(s => new
                {
                    mapValue = new
                    {
                        fields = new
                        {
                            name     = new { stringValue  = s.Stock.Name },
                            symbol   = new { stringValue  = s.Stock.Symbol },
                            shares   = new { integerValue = s.Shares.ToString() },
                            avgPrice = new { doubleValue  = (double)s.AvgPrice }
                        }
                    }
                })
                .ToArray();

            object txArrayValue     = firestoreTransactions.Length > 0  ? (object)new { values = firestoreTransactions }  : new { };
            object stocksArrayValue = firestoreOwnedStocks.Length  > 0  ? (object)new { values = firestoreOwnedStocks }   : new { };

            string json = JsonSerializer.Serialize(new
            {
                fields = new
                {
                    username     = new { stringValue = user.GetUsername() },
                    passwordHash = new { stringValue = user.GetPasswordHash() },
                    salt         = new { stringValue = user.GetSalt() },
                    firstName    = new { stringValue = user.FirstName() },
                    lastName     = new { stringValue = user.LastName() },
                    wallet = new
                    {
                        mapValue = new
                        {
                            fields = new
                            {
                                currentAmount    = new { doubleValue = (double)wallet.CurrentAmount },
                                monthlyBudget    = new { doubleValue = (double)wallet.MonthlyBudget },
                                amountSpentMonth = new { doubleValue = (double)wallet.AmountSpentMonth },
                                transactions     = new { arrayValue  = txArrayValue }
                            }
                        }
                    },
                    brokerage = new
                    {
                        mapValue = new
                        {
                            fields = new
                            {
                                ownedStocks = new { arrayValue = stocksArrayValue }
                            }
                        }
                    }
                }
            });

            string url     = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{user.GetUsername()}";
            var content    = new StringContent(json, Encoding.UTF8, "application/json");
            var request    = new HttpRequestMessage(HttpMethod.Patch, url) { Content = content };
            var response   = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception("Failed to update user: " + body);
            }
        }
    }
}