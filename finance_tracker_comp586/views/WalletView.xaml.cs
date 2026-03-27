using finance_tracker_comp586.models;
using LiveChartsCore.SkiaSharpView.WPF;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using SkiaSharp;

namespace finance_tracker_comp586.views
{
    public partial class WalletView : Page
    {
        private finance_tracker_comp586.Wallet wallet;
        public WalletView()
        {
            InitializeComponent();

            wallet = App.CurrentUser!.GetWallet();
            DataContext = wallet;

            RefreshChart();
        }

        private void RefreshChart()
        {
            PieChartContainer.Children.Clear();

            var pieChart = new LiveChartsCore.SkiaSharpView.WPF.PieChart
            {
                Series = ChartHelper.GetPieSeries(wallet)
            };

            PieChartContainer.Children.Add(pieChart);
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomeView());
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
                RefreshChart();
            }
        }
    }
}