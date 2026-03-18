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
    }
}
