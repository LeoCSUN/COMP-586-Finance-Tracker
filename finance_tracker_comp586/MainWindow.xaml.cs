using finance_tracker_comp586.views;
using System.Windows;

namespace finance_tracker_comp586
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