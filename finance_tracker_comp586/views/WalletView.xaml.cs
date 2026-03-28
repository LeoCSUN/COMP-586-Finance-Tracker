using finance_tracker_comp586.models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
    public partial class WalletView : Page
    {
        private Wallet _wallet;

        public WalletView()
        {
            InitializeComponent();

            // Load the current user's wallet
            _wallet = App.CurrentUser!.GetWallet();

            // Initial calculation of the chart data based on existing transactions
            _wallet.UpdateChartData();

            // Set the DataContext so the XAML {Binding} properties 
            // (CurrentAmount, Transactions, SpendingSeries) connect to the model.
            this.DataContext = _wallet;
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomeView());
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this) };
            if (amountWindow.ShowDialog() == true)
            {
                _wallet.Deposit(amountWindow.EnteredAmount);
                // Note: Balance changes don't usually change the pie chart unless logic dictates,
                // but we call this to ensure the UI is fully synced.
                _wallet.UpdateChartData();
            }
        }

        private void Subtract_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this) };
            if (amountWindow.ShowDialog() == true)
            {
                try
                {
                    _wallet.Withdraw(amountWindow.EnteredAmount);
                    _wallet.UpdateChartData();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this) };
            if (amountWindow.ShowDialog() == true)
            {
                _wallet.SetBudget(amountWindow.EnteredAmount);
            }
        }

        private void Add_Transaction_Button_Click(object sender, RoutedEventArgs e)
        {
            var transactionWindow = new TransactionInputWindow { Owner = Window.GetWindow(this) };
            if (transactionWindow.ShowDialog() == true)
            {
                // Adding the transaction via the Wallet model triggers the update 
                // to CurrentAmount and AmountSpentMonth automatically.
                _wallet.AddTransaction(
                    transactionWindow.TransactionDate,
                    transactionWindow.Description,
                    transactionWindow.EnteredAmount,
                    transactionWindow.Category
                );

                // Because of INotifyPropertyChanged in Wallet.cs, the chart and 
                // list in the XAML will update automatically now.
            }
        }
    }
}