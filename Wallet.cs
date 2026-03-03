// Wallet class
// Performs calculations for the user's cash wallet
// Manages user's funds, budget, and transactions

public class Wallet
{
    private int currentAmount;
    private int monthlyBudget;
    private List<Transaction> transactions;

    public Wallet(int currentAmount, int monthlyBudget, List<Transaction> transactions)
    {
        if (currentAmount < 0) {
            throw new ArgumentException("Starting amount cannot be negative", nameof(currentAmount));
        }

        if (monthlyBudget < 0) {
            throw new ArgumentException("Starting budget cannot be negative", nameof(monthlyBudget));
        }

        this.currentAmount = currentAmount;
        this.monthlyBudget = monthlyBudget;
        this.transactions = transactions;
    }

    void addAmount(int amount) { this.currentAmount += amount; }

    int getAmount() { return this.currentAmount; }

    void setBudget(int budget)
    {
        if (budget < 0)
        {
            throw new ArgumentException("New budget cannot be negative.", nameof(budget));
        }

        this.monthlyBudget = budget;
    }

    int getBudget() { return this.monthlyBudget; }

    void addTransaction(DateTime transactionDate, string description, int amount)
    {
        Transaction transaction = new Transaction(transactionDate, description, amount);
        this.transactions.Add(transaction);
    }
}

public struct Transaction
{
    public DateTime transactionDate;
    public string description;
    public int amount;

    public Transaction(DateTime transactionDate, string description, int amount)
    {
        this.transactionDate = transactionDate;
        this.description = description;
        this.amount = amount;
    }
}