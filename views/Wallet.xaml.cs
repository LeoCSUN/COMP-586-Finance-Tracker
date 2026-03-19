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
            // Pop open an entry box with confirm + cancel buttons on the side
            // Take contents of entry box, making sure confirm button is only selectable when amount is over 0.00
            // Add entered amount to Wallet.currentAmount
        }

        private void Subtract_Button_Click(object sender, RoutedEventArgs e)
        {
            // Pop open an entry box with confirm + cancel buttons on the side
            // Take contents of entry box, making sure confirm button is only selectable when amount is over 0.00
            // Subtract entered amount from Wallet.currentAmount
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            // Pop open an entry box with confirm + cancel buttons on the side
            // Take contents of entry box, making sure confirm button is only selectable when amount is over 0.00
            // Set Wallet.monthlyBudget to whatever was entered
        }

        private void Add_Transaction_Button_Click(object sender, RoutedEventArgs e)
        {
            // Pop open a new window
            // Ask user to input following info: date, description, amount
            // Create new Transaction instance with that input and add it to Wallet.transactions
        }
    }
}