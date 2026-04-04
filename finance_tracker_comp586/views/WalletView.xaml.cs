using finance_tracker_comp586.models;
using System;
using System.Threading.Tasks;
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

        private void LogOff_Button_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService?.Navigate(new LoginView());
        }

        private async void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this), Label = "Enter Amount to Add" };
            if (amountWindow.ShowDialog() == true)
            {
                _wallet.Deposit(amountWindow.EnteredAmount);
                _wallet.UpdateChartData();
                try { await App.Users.UpdateUser(App.CurrentUser!); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private async void Subtract_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this), Label = "Enter Amount to Subtract"};
            if (amountWindow.ShowDialog() == true)
            {
                try
                {
                    _wallet.Withdraw(amountWindow.EnteredAmount);
                    _wallet.UpdateChartData();
                    await App.Users.UpdateUser(App.CurrentUser!);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this), Label = "Enter New Monthly Bugdet" };
            if (amountWindow.ShowDialog() == true)
            {
                _wallet.SetBudget(amountWindow.EnteredAmount);
                try { await App.Users.UpdateUser(App.CurrentUser!); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private async void Add_Transaction_Button_Click(object sender, RoutedEventArgs e)
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
                try { await App.Users.UpdateUser(App.CurrentUser!); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }
    }
}