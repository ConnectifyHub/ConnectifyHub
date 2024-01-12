using Main;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json;


namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ServerCommunication serverCommunication = ServerCommunication.Instance;
        public MainWindow()
        {
            InitializeComponent();
            LoadRegistrationDataFromJson();
        }

        private void LoadRegistrationDataFromJson()
        {
            if (File.Exists("registration_data.json"))
            {
                var jsonContent = File.ReadAllText("registration_data.json");
                string latest_email = serverCommunication.SendAndReceive("LatestLoginInfo|" + jsonContent);
                txtEmail.Text = latest_email != "False" ? latest_email : "Почта";
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;

            string loginResponse = serverCommunication.SendAndReceive($"Login|{email}|{password}");

            if (!loginResponse.Equals("False"))
            {
                MessageBox.Show("Вы успешно зашли!");
                File.WriteAllText("registration_data.json", $"{loginResponse}");
                LoginedWindow loginedWindow = new LoginedWindow();
                loginedWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неправильная почта или пароль");
                return;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Почта")
            {
                textBox.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Почта";
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            if (passwordBox.Password == "Пароль")
            {
                passwordBox.Password = "";
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                passwordBox.Password = "Пароль";
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }
    }
}