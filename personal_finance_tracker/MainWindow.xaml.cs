using personal_finance_tracker.views;
using System.Windows;

namespace personal_finance_tracker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginView());
        }
    }
}
