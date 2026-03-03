// Savings class
// Stores user's savings funds and current APY value
// Funds and APY can be modified
// Interest earned within month, year, and lifetime can be displayed

public class Savings {
    private int currentAmount;
    private double APY;

    public Savings (int currentAmount, double APY) {
    
        if (currentAmount < 0) {
            throw new ArgumentException("Starting amount cannot be negative", nameof(currentAmount));
        }

        if (APY < 0.0) {
            throw new ArgumentException("Starting APY cannot be negative", nameof(APY));
        }

        this.currentAmount = currentAmount;
        this.APY = APY;
    }

    public void deposit(int amount) { 
        if (amount < 0) {
            throw new ArgumentException("Deposit amount cannot be negative", nameof(amount));
        }

        this.currentAmount += amount; 
    }

    public void withdrawal(int amount);
    public int getAmount() { return this.currentAmount; }
    public int interestEarnedMonth();
    public int interestEarnedYear();
    public int interestEarnedLifetime();
}