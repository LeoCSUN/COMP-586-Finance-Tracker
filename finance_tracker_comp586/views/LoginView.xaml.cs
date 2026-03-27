using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace finance_tracker_comp586.views
{
    public partial class LoginView : Page
    {
        private AuthService _authService;

        public LoginView()
        {
            InitializeComponent();
            _authService = App.Auth;
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            User? loggedInUser = _authService.Login(username, password);

            if (loggedInUser != null)
            {
                App.CurrentUser = loggedInUser;
                NavigationService.Navigate(new HomeView());
            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }

        private void Register_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterView());
        }
    }
}
