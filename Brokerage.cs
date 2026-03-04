// Brokerage.cs
// Stores user's owned stocks
// Allows user to purchase additional stocks available on the market
// Displays price changes for specific stocks within different time spans
// A stock API will be used to retrieve real-time information about the specific stock

public class Brokerage {
    private List<Stock> ownedStocks;
    private List<Stock> availableStocks;
}

class Stock {
    private string name;
    private string abbreviation;

    public Stock(string name, string abbreviation) {
        this.name = name;
        this.abbreviation = abbreviation;
    }

    // Returns current price/share of stock
    public int GetPrice() {}

    // Returns the daily price change of the stock
    public double DayReturn() {}

    // Returns the monthly price change of the stock
    public double MonthReturn() {}

    // Returns the 6 month price change of the stock
    public double SixMonthReturn() {}

    // Returns the yearly price change of the stock
    public double YearReturn() {}

    // Returns the 5 year price change of the stock
    public double FiveYearReturn() {}

    // Returns the lifetime price change of the stock
    public double ChangeMax() {}
}