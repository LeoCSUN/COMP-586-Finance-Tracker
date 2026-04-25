using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace personal_finance_tracker
{
    public class Savings : INotifyPropertyChanged
    {
        private decimal   _balance;
        private decimal   _apy;
        private int       _accountAgeMonths;
        private decimal   _lifetimeInterestEarned;
        private decimal   _principal;
        private DateTime? _principalStartDate;

        public Savings() { }

        public decimal   Balance                { get => _balance;                private set { _balance                = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasBalance)); OnPropertyChanged(nameof(HasNoBalance)); } }
        public decimal   APY                    { get => _apy;                    private set { _apy                   = value; OnPropertyChanged(); } }
        public int       AccountAgeMonths       { get => _accountAgeMonths;       private set { _accountAgeMonths      = value; OnPropertyChanged(); } }
        public decimal   LifetimeInterestEarned { get => _lifetimeInterestEarned; private set { _lifetimeInterestEarned = value; OnPropertyChanged(); } }
        public decimal   Principal              { get => _principal;              private set { _principal             = value; OnPropertyChanged(); } }
        public DateTime? PrincipalStartDate     { get => _principalStartDate;     private set { _principalStartDate    = value; OnPropertyChanged(); } }
        public bool      HasBalance             => _balance > 0;
        public bool      HasNoBalance           => _balance == 0;

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (_balance == 0)
            {
                Principal          = amount;
                PrincipalStartDate = DateTime.Today;
            }
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount > _balance) throw new InvalidOperationException("Insufficient funds.");
            Balance -= amount;
        }

        public void SetApy(decimal amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            APY = amount;
        }

        public decimal InterestEarnedMonth    => _balance * (_apy / 100m) / 12m;
        public decimal InterestEarnedYear     => _balance * (_apy / 100m);
        public decimal InterestEarnedLifetime => _lifetimeInterestEarned;

        public decimal ApplyMonthlyInterest()
        {
            decimal interest = decimal.Round(InterestEarnedMonth, 2, MidpointRounding.ToEven);
            Balance                += interest;
            LifetimeInterestEarned += interest;
            AccountAgeMonths++;
            return interest;
        }

        public void LoadFromStorage(decimal balance, decimal apy, int accountAgeMonths, decimal lifetimeInterestEarned, decimal principal, DateTime? principalStartDate)
        {
            _balance                = balance;
            _apy                    = apy;
            _accountAgeMonths       = accountAgeMonths;
            _lifetimeInterestEarned = lifetimeInterestEarned;
            _principal              = principal;
            _principalStartDate     = principalStartDate;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
