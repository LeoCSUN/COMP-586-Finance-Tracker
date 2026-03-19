using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace finance_tracker_comp586.views
{
    public partial class Login : Page
    {
        private AuthService _authService;

        public Login()
        {
            InitializeComponent();
            _authService = App.Auth;
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            bool loginSuccess = _authService.Login(username, password);

            if (loginSuccess)
            {
                NavigationService.Navigate(new Home());
            }
            else
            {
                // Throw an error of some kind
            }
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Register());
        }
    }
}
