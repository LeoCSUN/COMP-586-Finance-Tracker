// User class
// Stores user's info
// Contains Checking and Savings class instances

using Microsoft.VisualBasic;

public class User
{
    private string name;
    private Checking checkingAccount;
    private Savings savingsAccount;
    private Brokerage brokerageAccount;

    public User(string name, Checking checkingAccount, Savings savingsAccount, Brokerage brokerageAccount)
    {
        this.name = name;
        this.checkingAccount = checkingAccount;
        this.savingsAccount = savingsAccount;
        this.brokerageAccount = brokerageAccount;
    }
}