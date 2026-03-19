// Handles hashing of user data

namespace finance_tracker_comp586
{
    using System.Security.Cryptography;
    using System.Text;
    public class User
    {
        private string username;
        private string passwordHash;
        private string salt;
        private string firstName;
        private string lastName;

        private Wallet walletAccount;
        private Savings savingsAccount;
        private Brokerage brokerageAccount;

        public User(string username, string password, string firstName, string lastName)
        {
            this.username = username;
            this.salt = GenerateSalt();
            this.passwordHash = HashPassword(password, salt);

            this.firstName = firstName;
            this.lastName = lastName;

            this.walletAccount = new Wallet();
            this.savingsAccount = new Savings();
            this.brokerageAccount = new Brokerage(App.StockApi);
        }

        public User(string username, string passwordHash, string salt, string firstName, string lastName)
        {
            this.username = username;
            this.passwordHash = passwordHash;
            this.salt = salt;
            this.firstName = firstName;
            this.lastName = lastName;

            this.walletAccount = new Wallet();
            this.savingsAccount = new Savings();
            this.brokerageAccount = new Brokerage(App.StockApi);
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(salt + password);
                byte[] hash = sha256.ComputeHash(combinedBytes);

                return Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPassword(string password)
        {
            string hashToCheck = HashPassword(password, salt);
            return hashToCheck == passwordHash;
        }
        public string GetUsername() => this.username;
        public string FirstName() => this.firstName;
        public string LastName() => this.lastName;
        public string GetPasswordHash() => this.passwordHash;
        public string GetSalt() => this.salt;
    }
}