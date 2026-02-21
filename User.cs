// User class
// Stores user's info
// Contains Checking and Savings class instances

public class User
{
    private string name;
    private Checking checkingAccount;
    private Savings savingsAccount;

    public User(string name, Checking checkingAccount, Savings savingsAccount)
    {
        this.name = name;
        this.checkingAccount = checkingAccount;
        this.savingsAccount = savingsAccount;
    }
}