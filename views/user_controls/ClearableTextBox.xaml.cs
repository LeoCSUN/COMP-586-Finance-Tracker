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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace finance_tracker_comp586.views.user_controls
{
    /// <summary>
    /// Interaction logic for ClearableTextBox.xaml
    /// </summary>
    public partial class ClearableTextBox : UserControl
    {
        public ClearableTextBox()
        {
            DataContext = this;
            InitializeComponent();
        }

        private string placeholder;

        public string Placeholder
        {
            get { return placeholder; }
            set 
            {
                placeholder = value;
                textPlaceholder.Text = placeholder;
            }
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            textInput.Clear();
            textInput.Focus();
        }

        private void textInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(textInput.Text))
            {
                textPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                textPlaceholder.Visibility = Visibility.Hidden;
            }
        }
    }
}
