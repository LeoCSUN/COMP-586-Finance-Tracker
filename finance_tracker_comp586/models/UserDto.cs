using System.Collections.ObjectModel;

namespace finance_tracker_comp586.models
{
    public class UserDto
    {
        public string       Username     { get; set; } = string.Empty;
        public string       PasswordHash { get; set; } = string.Empty;
        public string       Salt         { get; set; } = string.Empty;
        public string       FirstName    { get; set; } = string.Empty;
        public string       LastName     { get; set; } = string.Empty;
        public WalletDto    Wallet       { get; set; } = new WalletDto();
        public SavingsDto   Savings      { get; set; } = new SavingsDto();
        public BrokerageDto Brokerage    { get; set; } = new BrokerageDto();
    }

    public class WalletDto
    {
        public decimal                            CurrentAmount    { get; set; }
        public decimal                            MonthlyBudget    { get; set; }
        public decimal                            AmountSpentMonth { get; set; }
        public ObservableCollection<TransactionDto> Transactions   { get; set; } = new();
    }

    public class SavingsDto
    {
        public decimal Balance                { get; set; }
        public decimal APY                    { get; set; }
        public int     AccountAgeMonths       { get; set; }
        public decimal LifetimeInterestEarned { get; set; }
    }

    public class BrokerageDto
    {
        public List<OwnedStockDto> OwnedStocks     { get; set; } = new();
        public List<StockDto>      AvailableStocks  { get; set; } = new();
    }

    public class TransactionDto
    {
        public DateTime          TransactionDate { get; set; }
        public string            Description     { get; set; } = string.Empty;
        public decimal           Amount          { get; set; }
        public TransactionCategory Category      { get; set; }
    }

    public class StockDto
    {
        public string Name   { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
    }

    public class OwnedStockDto
    {
        public string  Name     { get; set; } = string.Empty;
        public string  Symbol   { get; set; } = string.Empty;
        public int     Shares   { get; set; }
        public decimal AvgPrice { get; set; }
    }
}

