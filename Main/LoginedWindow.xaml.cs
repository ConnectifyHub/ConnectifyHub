using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace Main
{
    /// <summary>
    /// Логика взаимодействия для LoginedWindow.xaml
    /// </summary>
    public partial class LoginedWindow : Window
    {
        ServerCommunication serverCommunication = ServerCommunication.Instance;
        public dynamic User { get; private set; }
        public LoginedWindow()
        {
            InitializeComponent();
            var jsonContent = File.ReadAllText("registration_data.json");
            string[] whoami = (serverCommunication.SendAndReceive("WhoAmI|" + jsonContent)).Split("|");
            User = new
            {
                Email = whoami[0],
                Name = whoami[1],
                Surname = whoami[2],
            };
            WelcomeTextBlock.Text = $"С возвращением, {User.Name} {User.Surname}!";
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
