namespace finance_tracker_comp586
{
    // Savings.cs
    // Stores user's savings funds and current APY value
    // Funds and APY can be modified
    // Interest earned within month, year, and lifetime can be displayed

    public class Savings
    {
        private decimal balance;
        private decimal apy;
        private int accountAgeMonths;
        private decimal lifetimeInterestEarned;

        public Savings()
        {
            this.balance = 0m;
            this.apy = 0m;
        }

        public decimal Balance => balance;
        public decimal APY => apy;
        public int AccountAgeMonths => accountAgeMonths;
        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount > balance) throw new InvalidOperationException("Insufficient funds.");
            balance -= amount;
        }

        public decimal InterestEarnedMonth() => balance * apy / 12m;
        public decimal InterestEarnedYear() => balance * apy;
        public decimal InterestEarnedLifetime() => lifetimeInterestEarned;

        public decimal ApplyMonthlyInterest()
        {
            decimal interest = decimal.Round(InterestEarnedMonth(), 2, MidpointRounding.ToEven);
            balance += interest;
            lifetimeInterestEarned += interest;
            accountAgeMonths++;
            return interest;
        }
    }
}