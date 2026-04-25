using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace personal_finance_tracker.utils
{
    public class BudgetColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is decimal spent && values[1] is decimal budget)
                return spent > budget ? Brushes.Red : Brushes.Green;
            return Brushes.Black;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
