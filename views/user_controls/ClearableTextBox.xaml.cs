using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace finance_tracker_comp586.views.user_controls
{
    public partial class ClearableTextBox : UserControl
    {
        public ClearableTextBox()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // Designer-safe
                DataContext = this;
            }
        }

        #region DependencyProperty: Placeholder

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(ClearableTextBox),
                new PropertyMetadata("Placeholder", OnPlaceholderChanged));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ClearableTextBox)d;
            if (control.textPlaceholder != null)
            {
                control.textPlaceholder.Text = (string)e.NewValue;
            }
        }

        #endregion

        #region IsPassword property

        private bool isPassword = false;
        public bool IsPassword
        {
            get => isPassword;
            set
            {
                isPassword = value;
                if (!DesignerProperties.GetIsInDesignMode(this))
                    UpdateInputControl();
            }
        }

        #endregion

        #region Text property

        public string Text
        {
            get
            {
                return IsPassword ? passwordInput.Password : textInput.Text;
            }
            set
            {
                if (IsPassword)
                    passwordInput.Password = value;
                else
                    textInput.Text = value;
            }
        }

        #endregion

        #region Private Methods

        private void UpdateInputControl()
        {
            if (IsPassword)
            {
                textInput.Visibility = Visibility.Collapsed;
                passwordInput.Visibility = Visibility.Visible;
            }
            else
            {
                textInput.Visibility = Visibility.Visible;
                passwordInput.Visibility = Visibility.Collapsed;
            }

            UpdatePlaceholderVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            if (IsPassword)
                textPlaceholder.Visibility = string.IsNullOrEmpty(passwordInput.Password) ? Visibility.Visible : Visibility.Hidden;
            else
                textPlaceholder.Visibility = string.IsNullOrEmpty(textInput.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion

        #region Event Handlers

        private void textInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void passwordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            if (IsPassword)
                passwordInput.Clear();
            else
                textInput.Clear();

            if (IsPassword)
                passwordInput.Focus();
            else
                textInput.Focus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure placeholder shows correctly when loaded
            textPlaceholder.Text = Placeholder;
            UpdateInputControl();
        }

        #endregion
    }
}