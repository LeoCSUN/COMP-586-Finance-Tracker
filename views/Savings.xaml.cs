using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
    public partial class Savings : Page
    {
        private finance_tracker_comp586.Savings savings;
        public Savings()
        {
            InitializeComponent();
            DataContext = savings;
        }

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Home());
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            // Pop open an entry box with confirm + cancel buttons on the side
            // Take contents of entry box, making sure confirm button is only selectable when amount is over 0.00
            // Add entered amount to Savings.balance
        }

        private void Subtract_Button_Click(object sender, RoutedEventArgs e)
        {
            // Pop open an entry box with confirm + cancel buttons on the side
            // Take contents of entry box, making sure confirm button is only selectable when amount is over 0.00
            // Subtract entered amount from Savings.balance
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            // Pop open an entry box with confirm + cancel buttons on the side
            // Take contents of entry box, making sure confirm button is only selectable when amount is over 0.00
            // Set Savings.apy to whatever was entered
        }
    }
}
