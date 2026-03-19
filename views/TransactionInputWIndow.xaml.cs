using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace finance_tracker_comp586.views
{
    public partial class TransactionInputWindow : Window
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public decimal EnteredAmount { get; set; }
        public TransactionCategory Category { get; set; }

        public TransactionInputWindow()
        {
            InitializeComponent();
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int month = DateTime.ParseExact(((ComboBoxItem)MonthComboBox.SelectedItem).Content.ToString(), "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                int day = int.Parse(((ComboBoxItem)DayComboBox.SelectedItem).Content.ToString());
                int year = int.Parse(((ComboBoxItem)YearComboBox.SelectedItem).Content.ToString());

                if (!DateTime.TryParse($"{month}/{day}/{year}", out DateTime date))
                {
                    MessageBox.Show("Invalid date.");
                    return;
                }

                TransactionDate = date;

                Description = DescriptionInputBox.Text;

                if (!decimal.TryParse(AmountInputBox.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Enter a valid amount greater than 0.");
                    return;
                }

                EnteredAmount = amount;

                string selectedCategory = ((ComboBoxItem)CategoryComboBox.SelectedItem).Content.ToString();

                if (!Enum.TryParse(selectedCategory, out TransactionCategory category))
                {
                    MessageBox.Show("Invalid category.");
                    return;
                }

                Category = category;

                DialogResult = true;
            }
            catch
            {
                MessageBox.Show("Something went wrong. Check your inputs.");
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
