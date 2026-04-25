using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace personal_finance_tracker.data
{
    public class SqliteUserRepository : IUserRepository
    {
        private readonly string _databasePath;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        public SqliteUserRepository()
        {
            string storageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PersonalFinanceTracker");
            Directory.CreateDirectory(storageFolder);
            _databasePath = Path.Combine(storageFolder, "personal_finance_tracker.db");
            EnsureDatabase();
        }

        private void EnsureDatabase()
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Username TEXT PRIMARY KEY,
                    PasswordHash TEXT NOT NULL,
                    Salt TEXT NOT NULL,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    WalletAmount REAL NOT NULL,
                    MonthlyBudget REAL NOT NULL,
                    AmountSpentMonth REAL NOT NULL,
                    TransactionsJson TEXT NOT NULL,
                    OwnedStocksJson TEXT NOT NULL,
                    SavingsBalance REAL NOT NULL,
                    SavingsApy REAL NOT NULL,
                    SavingsAccountAgeMonths INTEGER NOT NULL,
                    SavingsLifetimeInterestEarned REAL NOT NULL,
                    SavingsPrincipal REAL NOT NULL,
                    SavingsPrincipalStartDate TEXT
                );";
            command.ExecuteNonQuery();
        }

        public User? GetUser(string username)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT PasswordHash, Salt, FirstName, LastName,
                       WalletAmount, MonthlyBudget, AmountSpentMonth,
                       TransactionsJson, OwnedStocksJson,
                       SavingsBalance, SavingsApy, SavingsAccountAgeMonths,
                       SavingsLifetimeInterestEarned, SavingsPrincipal,
                       SavingsPrincipalStartDate
                FROM Users
                WHERE Username = $username;";
            command.Parameters.AddWithValue("$username", username);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return null;

            string passwordHash = reader.GetString(0);
            string salt = reader.GetString(1);
            string firstName = reader.GetString(2);
            string lastName = reader.GetString(3);
            decimal walletAmount = Convert.ToDecimal(reader.GetDouble(4));
            decimal monthlyBudget = Convert.ToDecimal(reader.GetDouble(5));
            decimal amountSpentMonth = Convert.ToDecimal(reader.GetDouble(6));
            string transactionsJson = reader.GetString(7);
            string ownedStocksJson = reader.GetString(8);
            decimal savingsBalance = Convert.ToDecimal(reader.GetDouble(9));
            decimal savingsApy = Convert.ToDecimal(reader.GetDouble(10));
            int savingsAccountAgeMonths = reader.GetInt32(11);
            decimal savingsLifetimeInterestEarned = Convert.ToDecimal(reader.GetDouble(12));
            decimal savingsPrincipal = Convert.ToDecimal(reader.GetDouble(13));
            DateTime? savingsPrincipalStartDate = reader.IsDBNull(14)
                ? null
                : DateTime.Parse(reader.GetString(14), null, System.Globalization.DateTimeStyles.RoundtripKind);

            var transactions = string.IsNullOrWhiteSpace(transactionsJson)
                ? new List<Transaction>()
                : JsonSerializer.Deserialize<List<TransactionDto>>(transactionsJson, _jsonOptions)
                    ?.Select(dto => new Transaction(dto.TransactionDate, dto.Description, dto.Amount, dto.Category))
                    .ToList() ?? new List<Transaction>();

            var ownedStocks = string.IsNullOrWhiteSpace(ownedStocksJson)
                ? new List<OwnedStock>()
                : JsonSerializer.Deserialize<List<OwnedStockDto>>(ownedStocksJson, _jsonOptions)
                    ?.Select(dto => new OwnedStock
                    {
                        Stock = new Stock(dto.Name, dto.Symbol, dto.Sector),
                        Shares = dto.Shares,
                        AvgPrice = dto.AvgPrice
                    })
                    .ToList() ?? new List<OwnedStock>();

            var user = new User(username, passwordHash, salt, firstName, lastName, ownedStocks, walletAmount, monthlyBudget, amountSpentMonth, transactions);
            user.InitializeSavingsFromStorage(savingsBalance, savingsApy, savingsAccountAgeMonths, savingsLifetimeInterestEarned, savingsPrincipal, savingsPrincipalStartDate);
            return user;
        }

        public void AddUser(User user)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (
                    Username, PasswordHash, Salt, FirstName, LastName,
                    WalletAmount, MonthlyBudget, AmountSpentMonth,
                    TransactionsJson, OwnedStocksJson,
                    SavingsBalance, SavingsApy, SavingsAccountAgeMonths,
                    SavingsLifetimeInterestEarned, SavingsPrincipal,
                    SavingsPrincipalStartDate
                ) VALUES (
                    $username, $passwordHash, $salt, $firstName, $lastName,
                    $walletAmount, $monthlyBudget, $amountSpentMonth,
                    $transactionsJson, $ownedStocksJson,
                    $savingsBalance, $savingsApy, $savingsAccountAgeMonths,
                    $savingsLifetimeInterestEarned, $savingsPrincipal,
                    $savingsPrincipalStartDate
                );";

            BindUserParameters(command, user);
            command.ExecuteNonQuery();
        }

        public async Task UpdateUser(User user)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Users SET
                    PasswordHash = $passwordHash,
                    Salt = $salt,
                    FirstName = $firstName,
                    LastName = $lastName,
                    WalletAmount = $walletAmount,
                    MonthlyBudget = $monthlyBudget,
                    AmountSpentMonth = $amountSpentMonth,
                    TransactionsJson = $transactionsJson,
                    OwnedStocksJson = $ownedStocksJson,
                    SavingsBalance = $savingsBalance,
                    SavingsApy = $savingsApy,
                    SavingsAccountAgeMonths = $savingsAccountAgeMonths,
                    SavingsLifetimeInterestEarned = $savingsLifetimeInterestEarned,
                    SavingsPrincipal = $savingsPrincipal,
                    SavingsPrincipalStartDate = $savingsPrincipalStartDate
                WHERE Username = $username;";

            BindUserParameters(command, user);
            await command.ExecuteNonQueryAsync();
        }

        public void RemoveUser(string username)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM Users WHERE Username = $username;";
            command.Parameters.AddWithValue("$username", username);
            command.ExecuteNonQuery();
        }

        private void BindUserParameters(SqliteCommand command, User user)
        {
            command.Parameters.AddWithValue("$username", user.GetUsername());
            command.Parameters.AddWithValue("$passwordHash", user.GetPasswordHash());
            command.Parameters.AddWithValue("$salt", user.GetSalt());
            command.Parameters.AddWithValue("$firstName", user.FirstName);
            command.Parameters.AddWithValue("$lastName", user.LastName);
            command.Parameters.AddWithValue("$walletAmount", (double)user.GetWallet().CurrentAmount);
            command.Parameters.AddWithValue("$monthlyBudget", (double)user.GetWallet().MonthlyBudget);
            command.Parameters.AddWithValue("$amountSpentMonth", (double)user.GetWallet().AmountSpentMonth);
            command.Parameters.AddWithValue("$transactionsJson", SerializeTransactions(user.GetWallet().Transactions));
            command.Parameters.AddWithValue("$ownedStocksJson", SerializeOwnedStocks(user.GetBrokerage().OwnedStocks));
            command.Parameters.AddWithValue("$savingsBalance", (double)user.GetSavings().Balance);
            command.Parameters.AddWithValue("$savingsApy", (double)user.GetSavings().APY);
            command.Parameters.AddWithValue("$savingsAccountAgeMonths", user.GetSavings().AccountAgeMonths);
            command.Parameters.AddWithValue("$savingsLifetimeInterestEarned", (double)user.GetSavings().LifetimeInterestEarned);
            command.Parameters.AddWithValue("$savingsPrincipal", (double)user.GetSavings().Principal);
            command.Parameters.AddWithValue("$savingsPrincipalStartDate", user.GetSavings().PrincipalStartDate?.ToString("o") ?? (object)DBNull.Value);
        }

        private string SerializeTransactions(IEnumerable<Transaction> transactions)
            => JsonSerializer.Serialize(transactions.Select(t => new TransactionDto
            {
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                Amount = t.Amount,
                Category = t.Category
            }), _jsonOptions);

        private string SerializeOwnedStocks(IEnumerable<OwnedStock> stocks)
            => JsonSerializer.Serialize(stocks.Select(s => new OwnedStockDto
            {
                Name = s.Stock.Name,
                Symbol = s.Stock.Symbol,
                Sector = s.Stock.Sector,
                Shares = s.Shares,
                AvgPrice = s.AvgPrice
            }), _jsonOptions);

        private sealed class OwnedStockDto
        {
            public string Name { get; set; } = string.Empty;
            public string Symbol { get; set; } = string.Empty;
            public string Sector { get; set; } = string.Empty;
            public int Shares { get; set; }
            public decimal AvgPrice { get; set; }
        }

        private sealed class TransactionDto
        {
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public TransactionCategory Category { get; set; }
        }
    }
}
