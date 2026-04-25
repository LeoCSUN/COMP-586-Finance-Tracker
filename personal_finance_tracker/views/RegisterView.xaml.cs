using System.Windows;
using System.Windows.Controls;

namespace personal_finance_tracker.views
{
    public partial class RegisterView : Page
    {
        private AuthService _authService;

        public RegisterView()
        {
            InitializeComponent();
            _authService = App.Auth;
        }

        private void Registration_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;

            try
            {
                User newlyRegisteredUser = _authService.Register(username, password, firstName, lastName);
                App.CurrentUser = newlyRegisteredUser;

                MessageBox.Show("Registration successful!");
                NavigationService.Navigate(new HomeView());
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(exception.Message);
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong. Please try again.");
            }
        }

        private void Login_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginView());
        }
    }
}
