using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
    public partial class SavingsView : Page
    {
        private finance_tracker_comp586.Savings savings;
        public SavingsView()
        {
            InitializeComponent();
            savings = App.CurrentUser!.GetSavings();
            DataContext = savings;
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

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow();
            amountWindow.Owner = Window.GetWindow(this);
            bool? result = amountWindow.ShowDialog();

            if (result == true)
            {
                decimal enteredAmount = amountWindow.EnteredAmount;
                savings.Deposit(enteredAmount);
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
                savings.Withdraw(enteredAmount);
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
                savings.SetApy(enteredAmount);
            }
        }
    }
}
