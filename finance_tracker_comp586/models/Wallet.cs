using finance_tracker_comp586.utils;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace finance_tracker_comp586
{
    public class Wallet : INotifyPropertyChanged
    {
        private decimal                                    _currentAmount;
        private decimal                                    _monthlyBudget;
        private decimal                                    _amountSpentMonth;
        private ISeries[]                                  _spendingSeries = Array.Empty<ISeries>();
        private readonly ObservableCollection<Transaction> _transactions   = new();
        private readonly ObservableCollection<LegendItem>  _spendingLegend = new();

        public Wallet()
        {
            _currentAmount    = 0m;
            _monthlyBudget    = 0m;
            _amountSpentMonth = 0m;
        }

        public Wallet(decimal currentAmount, decimal monthlyBudget, decimal amountSpentMonth, IEnumerable<Transaction> transactions)
        {
            _currentAmount    = currentAmount;
            _monthlyBudget    = monthlyBudget;
            _amountSpentMonth = amountSpentMonth;
            foreach (Transaction t in transactions)
                _transactions.Add(t);
        }

        public decimal   CurrentAmount    { get => _currentAmount;    private set { _currentAmount    = value; OnPropertyChanged(); } }
        public decimal   MonthlyBudget    { get => _monthlyBudget;    private set { _monthlyBudget    = value; OnPropertyChanged(); } }
        public decimal   AmountSpentMonth { get => _amountSpentMonth; private set { _amountSpentMonth = value; OnPropertyChanged(); } }
        public ISeries[] SpendingSeries   { get => _spendingSeries;   private set { _spendingSeries   = value; OnPropertyChanged(); } }

        public ObservableCollection<Transaction> Transactions   => _transactions;
        public ObservableCollection<LegendItem>  SpendingLegend => _spendingLegend;

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            CurrentAmount += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount > _currentAmount) throw new InvalidOperationException("Insufficient funds.");
            CurrentAmount -= amount;
        }

        public void SetBudget(decimal budget)
        {
            if (budget < 0) throw new ArgumentOutOfRangeException(nameof(budget));
            MonthlyBudget = budget;
        }

        public void AddTransaction(DateTime date, string description, decimal amount, TransactionCategory category)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.", nameof(description));

            _transactions.Add(new Transaction(date, description, amount, category));

            if (category == TransactionCategory.Income)
                CurrentAmount += amount;
            else
            {
                CurrentAmount    -= amount;
                AmountSpentMonth += amount;
            }

            UpdateChartData();
        }

        public void UpdateChartData()
        {
            SpendingSeries = ChartHelper.GetPieSeries(this);

            _spendingLegend.Clear();
            var sorted = _transactions
                .Where(t => t.Category != TransactionCategory.Income)
                .GroupBy(t => t.Category)
                .Select(g =>
                {
                    var cat = g.Key.ToString();
                    ChartHelper.CategoryColors.TryGetValue(cat, out var colors);
                    return new LegendItem
                    {
                        Category   = cat,
                        Amount     = g.Sum(t => t.Amount),
                        ColorBrush = colors.WpfBrush ?? Brushes.Black
                    };
                })
                .OrderByDescending(x => x.Amount);
            foreach (var item in sorted)
                _spendingLegend.Add(item);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class Transaction
    {
        public DateTime            TransactionDate { get; set; }
        public string              Description     { get; set; }
        public decimal             Amount          { get; set; }
        public TransactionCategory Category        { get; set; }

        public Transaction(DateTime transactionDate, string description, decimal amount, TransactionCategory category)
        {
            TransactionDate = transactionDate;
            Description     = description;
            Amount          = amount;
            Category        = category;
        }
    }

    public enum TransactionCategory
    {
        Food, Utilities, Rent, Transportation, Entertainment, Income, Other
    }

    public class LegendItem
    {
        public string          Category   { get; set; } = string.Empty;
        public decimal         Amount     { get; set; }
        public SolidColorBrush ColorBrush { get; set; } = Brushes.Black;
    }
}