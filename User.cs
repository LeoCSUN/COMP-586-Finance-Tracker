// User class
// Stores user's info
// Contains Wallet, Savings, and Brokerage class instances

public class User
{
    private string name;
    private Wallet walletAccount;
    private Savings savingsAccount;
    private Brokerage brokerageAccount;

    public User(string name, Wallet walletAccount, Savings savingsAccount, Brokerage brokerageAccount)
    {
        this.name = name;
        this.walletAccount = walletAccount;
        this.savingsAccount = savingsAccount;
        this.brokerageAccount = brokerageAccount;
    }
}