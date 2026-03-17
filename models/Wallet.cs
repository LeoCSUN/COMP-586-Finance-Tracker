namespace finance_tracker_comp586
{   
    public class Wallet
    {
        private decimal currentAmount;
        private decimal monthlyBudget;
        private decimal amountSpentMonth;
        private readonly List<Transaction> transactions = new();

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

        public void AddTransaction(DateTime date, string description, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.", nameof(description));

            transactions.Add(new Transaction(date, description, amount));
            currentAmount -= amount;
            amountSpentMonth += amount;
        }
    }

    public record Transaction(DateTime TransactionDate, string Description, decimal Amount);
}