/*

TO DO:
- find a way provide api to Brokerage() instance in the User() constructor

*/

using System.Security.Cryptography;
using System.Text;
using financial_tracker;
public class User
{
    private string username;
    private string passwordHash;
    private string salt;
    private string name;

    private Wallet walletAccount;
    private Savings savingsAccount;
    private Brokerage brokerageAccount;

    public User(string username, string password, string name)
    {
        this.username = username;
        this.salt = GenerateSalt();
        this.passwordHash = HashPassword(password, salt);

        this.name = name;

        this.walletAccount = new Wallet(0m, 0m);
        this.savingsAccount = new Savings(0m, 0m);
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
}