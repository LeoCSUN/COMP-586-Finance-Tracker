using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
    public partial class Wallet : Page
    {
        private finance_tracker_comp586.Wallet wallet;
        public Wallet()
        {
            InitializeComponent();

            wallet = new finance_tracker_comp586.Wallet();
            DataContext = wallet;
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Home());
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow();
            amountWindow.Owner = Window.GetWindow(this);
            bool? result = amountWindow.ShowDialog();

            if (result == true)
            {
                decimal enteredAmount = amountWindow.EnteredAmount;
                wallet.Deposit(enteredAmount);
            }
        }

        private void Subtract_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow();
            amountWindow.Owner = Window.GetWindow(this);
            bool? result = amountWindow.ShowDialog();

            if (result == true)
            {
                decimal enteredAmount = amountWindow.EnteredAmount;
                wallet.Withdraw(enteredAmount);
            }
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow();
            amountWindow.Owner = Window.GetWindow(this);
            bool? result = amountWindow.ShowDialog();

            if (result == true)
            {
                decimal enteredAmount = amountWindow.EnteredAmount;
                wallet.SetBudget(enteredAmount);
            }
        }

        private void Add_Transaction_Button_Click(object sender, RoutedEventArgs e)
        {
            var transactionWindow = new TransactionInputWindow();
            transactionWindow.Owner = Window.GetWindow(this);
            bool? result = transactionWindow.ShowDialog();

            if (result == true)
            {
                DateTime date = transactionWindow.TransactionDate;
                string description = transactionWindow.Description;
                decimal amount = transactionWindow.EnteredAmount;
                TransactionCategory category = transactionWindow.Category;
                wallet.AddTransaction(date, description, amount, category);
            }
        }
    }
}