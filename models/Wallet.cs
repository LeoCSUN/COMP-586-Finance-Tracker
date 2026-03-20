using finance_tracker_comp586.models;
using LiveChartsCore.SkiaSharpView.WPF;
using System.Collections.ObjectModel;

namespace finance_tracker_comp586
{   
    public class Wallet
    {
        private decimal currentAmount;
        private decimal monthlyBudget;
        private decimal amountSpentMonth;
        private readonly ObservableCollection<Transaction> transactions = new();

        public Wallet()
        {
            this.currentAmount = 0m;
            this.monthlyBudget = 0m;
            this.amountSpentMonth = 0m;
        }

        public decimal CurrentAmount => currentAmount;
        public decimal MonthlyBudget => monthlyBudget;
        public decimal AmountSpentMonth => amountSpentMonth;
        public IReadOnlyList<Transaction> Transactions => transactions;

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            currentAmount += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount > currentAmount) throw new InvalidOperationException("Insufficient funds.");
            currentAmount -= amount;
        }

        public void SetBudget(decimal budget)
        {
            if (budget < 0) throw new ArgumentOutOfRangeException(nameof(budget));
            monthlyBudget = budget;
        }

        public void AddTransaction(DateTime date, string description, decimal amount, TransactionCategory category)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description is required.", nameof(description));
            }

            transactions.Add(new Transaction(date, description, amount, category));
            currentAmount -= amount;
            amountSpentMonth += amount;
        }
    }

    public class Transaction
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public TransactionCategory Category {get; set;}

        public Transaction(DateTime transactionDate, string description, decimal amount, TransactionCategory category)
        {
            TransactionDate = transactionDate;
            Description = description;
            Amount = amount;
            Category = category;
        }
    }

    public enum TransactionCategory
    {
        Food,
        Utilities,
        Rent,
        Transportaion,
        Entertainment,
        Income,
        Other
    }
}