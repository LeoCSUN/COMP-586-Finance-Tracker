using System.Security.Cryptography;
using System.Text;

namespace finance_tracker_comp586
{
    public class User
    {
        private string    username;
        private string    passwordHash;
        private string    salt;
        private string    firstName;
        private string    lastName;
        private Wallet    walletAccount;
        private Savings   savingsAccount;
        private Brokerage brokerageAccount;

        public User(string username, string password, string firstName, string lastName)
        {
            this.username      = username;
            this.salt          = GenerateSalt();
            this.passwordHash  = HashPassword(password, salt);
            this.firstName     = firstName;
            this.lastName      = lastName;
            walletAccount      = new Wallet();
            savingsAccount     = new Savings();
            brokerageAccount   = new Brokerage(App.StockApi);
        }

        public User(string username, string passwordHash, string salt, string firstName, string lastName)
            : this(username, passwordHash, salt, firstName, lastName, null)
        {
        }

        public User(string username, string passwordHash, string salt, string firstName, string lastName,
            IEnumerable<OwnedStock>? ownedStocks)
        {
            this.username     = username;
            this.passwordHash = passwordHash;
            this.salt         = salt;
            this.firstName    = firstName;
            this.lastName     = lastName;
            walletAccount     = new Wallet();
            savingsAccount    = new Savings();
            brokerageAccount  = new Brokerage(App.StockApi, ownedStocks);
        }

        public User(string username, string passwordHash, string salt, string firstName, string lastName,
            IEnumerable<OwnedStock>? ownedStocks, decimal walletAmount, decimal monthlyBudget,
            decimal amountSpentMonth, IEnumerable<Transaction> transactions)
        {
            this.username     = username;
            this.passwordHash = passwordHash;
            this.salt         = salt;
            this.firstName    = firstName;
            this.lastName     = lastName;
            walletAccount     = new Wallet(walletAmount, monthlyBudget, amountSpentMonth, transactions);
            savingsAccount    = new Savings();
            brokerageAccount  = new Brokerage(App.StockApi, ownedStocks);
        }

        public Brokerage Brokerage
        {
            get => brokerageAccount;
            set => brokerageAccount = value;
        }

        public string    GetUsername()     => username;
        public string    FirstName()       => firstName;
        public string    LastName()        => lastName;
        public string    GetPasswordHash() => passwordHash;
        public string    GetSalt()         => salt;
        public Wallet    GetWallet()       => walletAccount;
        public Savings   GetSavings()      => savingsAccount;
        public Brokerage GetBrokerage()    => brokerageAccount;

        public bool VerifyPassword(string password)
            => HashPassword(password, salt) == passwordHash;

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using SHA256 sha256    = SHA256.Create();
            byte[] combinedBytes   = Encoding.UTF8.GetBytes(salt + password);
            byte[] hash            = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hash);
        }
    }
}
