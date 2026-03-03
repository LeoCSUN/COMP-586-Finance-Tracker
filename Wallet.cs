// Wallet class
// Performs calculations for the user's cash wallet
// Manages user's funds, budget, and transactions

public class Wallet
{
    private decimal currentAmount;
    private decimal monthlyBudget;
    private readonly List<Transaction> transactions = new();

    public Wallet(decimal startingAmount, decimal monthlyBudget)
    {
        if (startingAmount < 0) throw new ArgumentOutOfRangeException(nameof(startingAmount));
        if (monthlyBudget < 0) throw new ArgumentOutOfRangeException(nameof(monthlyBudget));

        this.currentAmount = startingAmount;
        this.monthlyBudget = monthlyBudget;
    }

    public decimal CurrentAmount => currentAmount;
    public decimal MonthlyBudget => monthlyBudget;
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
        currentAmount += amount;
    }
}

public record Transaction(DateTime TransactionDate, string Description, decimal Amount);