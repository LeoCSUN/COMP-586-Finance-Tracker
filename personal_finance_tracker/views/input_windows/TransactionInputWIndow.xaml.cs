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

namespace personal_finance_tracker.views
{
    public partial class TransactionInputWindow : Window
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
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
                if (MonthComboBox.SelectedItem is not ComboBoxItem monthItem
                    || DayComboBox.SelectedItem is not ComboBoxItem dayItem
                    || YearComboBox.SelectedItem is not ComboBoxItem yearItem)
                {
                    MessageBox.Show("Select month, day, and year.");
                    return;
                }

                string? monthName = monthItem.Content?.ToString();
                string? dayText = dayItem.Content?.ToString();
                string? yearText = yearItem.Content?.ToString();

                if (string.IsNullOrWhiteSpace(monthName)
                    || !DateTime.TryParseExact(monthName, "MMMM", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime monthParsed)
                    || !int.TryParse(dayText, out int day)
                    || !int.TryParse(yearText, out int year))
                {
                    MessageBox.Show("Invalid date.");
                    return;
                }

                int month = monthParsed.Month;

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

                if (CategoryComboBox.SelectedItem is not ComboBoxItem categoryItem)
                {
                    MessageBox.Show("Select a category.");
                    return;
                }

                string? selectedCategory = categoryItem.Content?.ToString();

                if (string.IsNullOrWhiteSpace(selectedCategory))
                {
                    MessageBox.Show("Invalid category.");
                    return;
                }

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
