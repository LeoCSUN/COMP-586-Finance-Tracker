using personal_finance_tracker.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace personal_finance_tracker.views
{
    public partial class WalletView : Page
    {
        public string FullName => App.CurrentUser?.FullName ?? "";
        private Wallet _wallet;
        private bool _isUpdatingDropdowns = false;
        private string? _lastSortedProperty = null;
        private ListSortDirection _lastSortDirection = ListSortDirection.Ascending;

        private static readonly string[] _monthNames =
        {
            "All", "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        private static readonly string[] _years = { "2024", "2025", "2026" };

        public WalletView()
        {
            InitializeComponent();

            _wallet = App.CurrentUser!.GetWallet();

            this.DataContext = _wallet;

            InitializeDropdowns();
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
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this), Label = "Enter Amount to Subtract" };
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
                ApplyFilter();
                try { await App.Users.UpdateUser(App.CurrentUser!); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private void InitializeDropdowns()
        {
            foreach (var month in _monthNames)
                TransMonthComboBox.Items.Add(month);
            foreach (var year in _years)
                TransYearComboBox.Items.Add(year);
            _isUpdatingDropdowns = true;
            TransYearComboBox.SelectedItem   = "2026";
            TransMonthComboBox.SelectedIndex = 5;
            _isUpdatingDropdowns = false;

            ApplyFilter();
        }

        private void MonthYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingDropdowns) return;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (TransMonthComboBox.SelectedItem == null || TransYearComboBox.SelectedItem == null) return;
            int year       = int.Parse((string)TransYearComboBox.SelectedItem);
            int monthIndex = TransMonthComboBox.SelectedIndex;

            IEnumerable<Transaction> filtered = _wallet.Transactions.Where(t => t.TransactionDate.Year == year);
            if (monthIndex > 0)
                filtered = filtered.Where(t => t.TransactionDate.Month == monthIndex);

            var filteredList = filtered.ToList();
            TransactionsListView.ItemsSource = filteredList;
            ApplySort();
            _wallet.UpdateChartDataFiltered(filteredList);

            string monthLabel = monthIndex > 0 ? _monthNames[monthIndex] : "All Months";
            ChartPeriodLabel.Text = $"{monthLabel} {year}";

            bool hasData     = filteredList.Count > 0;
            string noDataMsg = monthIndex > 0
                ? $"No Transactions in {_monthNames[monthIndex]}"
                : "No Transactions";

            NoChartDataLabel.Text       = noDataMsg;
            NoChartDataLabel.Visibility = hasData ? Visibility.Collapsed : Visibility.Visible;
            NoTransDataLabel.Text       = noDataMsg;
            NoTransDataLabel.Visibility = hasData ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TransactionHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not GridViewColumnHeader header || header.Column == null) return;

            string? property = header.Column.DisplayMemberBinding is Binding b ? b.Path.Path : null;
            if (property == null) return;

            if (property == _lastSortedProperty)
                _lastSortDirection = _lastSortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            else
            {
                _lastSortedProperty = property;
                _lastSortDirection = ListSortDirection.Ascending;
            }

            ApplySort();
        }

        private void ApplySort()
        {
            if (_lastSortedProperty == null) return;
            ICollectionView view = CollectionViewSource.GetDefaultView(TransactionsListView.ItemsSource);
            if (view == null) return;
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(_lastSortedProperty, _lastSortDirection));
        }
    }
}
