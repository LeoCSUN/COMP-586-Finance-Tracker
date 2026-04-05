using System.Windows;

namespace finance_tracker_comp586.views
{
    public enum BuyMode { Shares, Dollars }

    public partial class BuyStockWindow : Window
    {
        public decimal EnteredAmount { get; private set; }
        public BuyMode BuyMode       { get; private set; }

        public BuyStockWindow(string symbol, string name, double price)
        {
            InitializeComponent();
            TitleLabel.Text = $"Add {symbol} — {name} @ {price:C}";
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(InputTextBox.Text, out decimal amount) && amount > 0)
            {
                EnteredAmount = amount;
                BuyMode       = SharesRadio.IsChecked == true ? BuyMode.Shares : BuyMode.Dollars;
                DialogResult  = true;
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
