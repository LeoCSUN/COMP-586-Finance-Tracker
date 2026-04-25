using System.Windows;
using System.Windows.Controls;

namespace personal_finance_tracker.views
{
    public partial class AmountInputWindow : Window
    {
        public decimal EnteredAmount { get; set; }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(AmountInputWindow), new PropertyMetadata("Placeholder"));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

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
