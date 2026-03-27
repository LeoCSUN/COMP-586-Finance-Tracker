using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
    public partial class AmountInputWindow : Window
    {
        public decimal EnteredAmount { get; set; }

        public AmountInputWindow()
        {
            InitializeComponent();
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(InputTextBox.Text, out decimal amount) && amount > 0)
            {
                EnteredAmount = amount;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Enter a valid amount greater than 0.");
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
