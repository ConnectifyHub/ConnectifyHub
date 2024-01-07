using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Main
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        ServerCommunication serverCommunication = ServerCommunication.Instance;
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            /*if (txtEmail.BorderBrush == Brushes.Green &&
                txtPassword.BorderBrush == Brushes.Green &&
                txtPasswordConfirm.BorderBrush == Brushes.Green)
            {
                var brevoEmailVerifier = new BrevoEmailVerifier();
                bool isEmailValid = await brevoEmailVerifier.VerifyEmailAsync(txtEmail.Text);

                MessageBox.Show("Регистрация успешна!");
            }
            else
            {
                MessageBox.Show("Пожалуйста корректно заполните все обязательные пункты", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string isValidEmail = serverCommunication.SendAndReceive("IsValidEmail|" + email);  // Correct the method call
            if (isValidEmail != null)
            {
                txtEmail.BorderBrush = isValidEmail.Equals("True") ? Brushes.Green : Brushes.Red;
            }
            else
            {
                txtEmail.BorderBrush = Brushes.Gray;
            }
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            string password = txtPassword.Password;
            string isStrongPassword = serverCommunication.SendAndReceive("IsStrongPassword|" + password);
            if (isStrongPassword != null)
            {
                txtPassword.BorderBrush = isStrongPassword.Equals("True") ? Brushes.Green : Brushes.Red;
            }
            else
            {
                txtPassword.BorderBrush = Brushes.Gray;
            }
        }

        private void txtPasswordConfirm_LostFocus(object sender, RoutedEventArgs e)
        {
            string password = txtPassword.Password;
            string passwordConfirm = txtPasswordConfirm.Password;
            bool passwordsMatch = password == passwordConfirm;
            txtPasswordConfirm.BorderBrush = passwordsMatch ? Brushes.Green : Brushes.Red;
        }

        private void txtFirstName_LostFocus(object sender, RoutedEventArgs e)
        {
            string firstName = txtFirstName.Text;
            string isValidFirstName = serverCommunication.SendAndReceive("IsValidFirstName|" + firstName);
            if (isValidFirstName != null)
            {
                txtFirstName.BorderBrush = isValidFirstName.Equals("True") ? Brushes.Green : Brushes.Red;
            }
            else
            {
                txtFirstName.BorderBrush = Brushes.Gray;
            }
        }

        private void txtLastName_LostFocus(object sender, RoutedEventArgs e)
        {
            string lastName = txtLastName.Text;
            string isValidLastName = serverCommunication.SendAndReceive("IsValidLastName|" + lastName);
            if (isValidLastName != null)
            {
                txtLastName.BorderBrush = isValidLastName.Equals("True") ? Brushes.Green : Brushes.Red;
            }
            else
            {
                txtLastName.BorderBrush = Brushes.Gray;
            }
        }

        private void txtPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            string phoneNumber = txtPhone.Text;
            string isValidPhoneNumber = serverCommunication.SendAndReceive("IsValidPhoneNumber|" + phoneNumber);
            if (isValidPhoneNumber != null)
            {
                txtPhone.BorderBrush = isValidPhoneNumber.Equals("True") ? Brushes.Green : Brushes.Red;
            }
            else
            {
                txtPhone.BorderBrush = Brushes.Gray;
            }
        }
    }
}
