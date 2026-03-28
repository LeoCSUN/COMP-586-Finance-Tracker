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

            _wallet = App.CurrentUser!.GetWallet();

            _wallet.UpdateChartData();

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
                _wallet.AddTransaction(
                    transactionWindow.TransactionDate,
                    transactionWindow.Description,
                    transactionWindow.EnteredAmount,
                    transactionWindow.Category
                );
            }
        }
    }
}