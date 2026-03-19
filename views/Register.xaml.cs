using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views
{
    public partial class Register : Page
    {
        private AuthService _authService;

        public Register()
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

                MessageBox.Show("Registration successful!");
                NavigationService.Navigate(new Home());
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(exception.Message);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Something went wrong. Please try again.");
            }
        }

        private void Login_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Login());
        }
    }
}
