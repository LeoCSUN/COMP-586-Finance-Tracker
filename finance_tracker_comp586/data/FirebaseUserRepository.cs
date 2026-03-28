using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Globalization;

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
            {
                return null;
            }

            string json = response.Content.ReadAsStringAsync().Result;
            using JsonDocument doc = JsonDocument.Parse(json);

            var fields = doc.RootElement.GetProperty("fields");

            string? _username = fields.GetProperty("username").GetProperty("stringValue").GetString();
            string? hash = fields.GetProperty("passwordHash").GetProperty("stringValue").GetString();
            string? salt = fields.GetProperty("salt").GetProperty("stringValue").GetString();
            string? firstName = fields.GetProperty("firstName").GetProperty("stringValue").GetString();
            string? lastName = fields.GetProperty("lastName").GetProperty("stringValue").GetString();

            if (string.IsNullOrWhiteSpace(_username)
                || string.IsNullOrWhiteSpace(hash)
                || string.IsNullOrWhiteSpace(salt)
                || string.IsNullOrWhiteSpace(firstName)
                || string.IsNullOrWhiteSpace(lastName))
            {
                return null;
            }

            List<OwnedStock> ownedStocks = new List<OwnedStock>();

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
                    {
                        continue;
                    }

                    string? name = stockFields.GetProperty("name").GetProperty("stringValue").GetString();
                    string? symbol = stockFields.GetProperty("symbol").GetProperty("stringValue").GetString();

                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(symbol))
                    {
                        continue;
                    }

                    int shares = 0;
                    if (stockFields.TryGetProperty("shares", out JsonElement sharesElement)
                        && sharesElement.TryGetProperty("integerValue", out JsonElement integerValue))
                    {
                        int.TryParse(integerValue.GetString(), out shares);
                    }

                    decimal avgPrice = 0m;
                    if (stockFields.TryGetProperty("avgPrice", out JsonElement avgElement))
                    {
                        if (avgElement.TryGetProperty("doubleValue", out JsonElement doubleValue))
                        {
                            avgPrice = Convert.ToDecimal(doubleValue.GetDouble());
                        }
                        else if (avgElement.TryGetProperty("integerValue", out JsonElement avgIntegerValue))
                        {
                            decimal.TryParse(avgIntegerValue.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out avgPrice);
                        }
                    }

                    ownedStocks.Add(new OwnedStock
                    {
                        Stock = new Stock(name, symbol),
                        Shares = shares,
                        AvgPrice = avgPrice
                    });
                }
            }

            return new User(
                _username,
                hash,
                salt,
                firstName,
                lastName,
                ownedStocks
                );
        }

        public void AddUser(User user)
        {
            var dto = new UserDto
            {
                Username = user.GetUsername(),
                PasswordHash = user.GetPasswordHash(),
                Salt = user.GetSalt(),
                FirstName = user.FirstName(),
                LastName = user.LastName(),
                Brokerage = new BrokerageDto
                {
                    OwnedStocks = user.GetBrokerage().OwnedStocks
                        .Select(s => new OwnedStockDto
                        {
                            Name = s.Stock.Name,
                            Symbol = s.Stock.Symbol,
                            Shares = s.Shares,
                            AvgPrice = s.AvgPrice
                        })
                        .ToList()
                }
            };

            var firestoreOwnedStocks = dto.Brokerage.OwnedStocks
                .Select(s => new
                {
                    mapValue = new
                    {
                        fields = new
                        {
                            name = new { stringValue = s.Name },
                            symbol = new { stringValue = s.Symbol },
                            shares = new { integerValue = s.Shares.ToString() },
                            avgPrice = new { doubleValue = (double)s.AvgPrice }
                        }
                    }
                })
                .ToArray();

            string json = JsonSerializer.Serialize(new
            {
                fields = new
                {
                    username = new { stringValue = dto.Username },
                    passwordHash = new { stringValue = dto.PasswordHash },
                    salt = new { stringValue = dto.Salt },
                    firstName = new { stringValue = dto.FirstName },
                    lastName = new { stringValue = dto.LastName },
                    brokerage = new
                    {
                        mapValue = new
                        {
                            fields = new
                            {
                                ownedStocks = new
                                {
                                    arrayValue = new
                                    {
                                        values = firestoreOwnedStocks
                                    }
                                }
                            }
                        }
                    }
                }
            });

            string url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users?documentId={dto.Username}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(url, content).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to add user to Firebase: " + response.Content.ReadAsStringAsync().Result);
            }
        }

        public void RemoveUser(string username)
        {
            string url = $"https://firestore.googleapis.com/v1/projects/{_projectId}/databases/(default)/documents/users/{username}";

            var response = _httpClient.DeleteAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete user: " + response.Content.ReadAsStringAsync().Result);
            }
        }
    }
}