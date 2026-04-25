using System.Windows;

namespace personal_finance_tracker.views
{
    public enum SellMode { Shares, Dollars }

    public partial class SellStockWindow : Window
    {
        public decimal EnteredAmount { get; private set; }
        public SellMode SellMode     { get; private set; }

        public SellStockWindow(string symbol, string name)
        {
            InitializeComponent();
            TitleLabel.Text = $"Remove {symbol} — {name}";
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(InputTextBox.Text, out decimal amount) && amount > 0)
            {
                EnteredAmount = amount;
                SellMode      = SharesRadio.IsChecked == true ? SellMode.Shares : SellMode.Dollars;
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
