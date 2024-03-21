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
using System.Collections.ObjectModel;

namespace Main
{
    /// <summary>
    /// Логика взаимодействия для LoginedWindow.xaml
    /// </summary>
    public partial class LoginedWindow : Window
    {

        ServerCommunication serverCommunication = ServerCommunication.Instance;
        public dynamic User { get; private set; }
        private ObservableCollection<Message> messagesList = new ObservableCollection<Message>();
        public LoginedWindow()
        {
            InitializeComponent();
            var jsonContent = File.ReadAllText("registration_data.json");
            string[] whoami = (serverCommunication.SendAndReceive("WhoAmI|", jsonContent)).Split("|");
            User = new
            {
                Email = whoami[0],
                Name = whoami[1],
                Surname = whoami[2],
            };
            WelcomeTextBlock.Text = $"С возвращением, {User.Name} {User.Surname}!";
        }

        private void UpdateMessages(ListBox listBox)
        {
            string messages = serverCommunication.SendAndReceive("GetMessages", "0|0");
            string[] messages_parts = messages.Split("||");

            // Clear the existing items before adding new ones
            messagesList.Clear();

            for (int i = 0; i < messages_parts.Length - 1; i++)
            {
                var one_message = new Message
                {
                    Name = messages_parts[i].Split("|")[0],
                    Text = messages_parts[i].Split("|")[1],
                    Sended = messages_parts[i].Split("|")[2],
                };
                messagesList.Add(one_message);
            }

            listBox.ItemsSource = messagesList;
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text != "")
            {
                string message = MessageTextBox.Text;
                string jsonContent = File.ReadAllText("registration_data.json");
                string response = serverCommunication.SendAndReceive("SendMessage", $"{message}|{jsonContent}|0");
                if (response == "True")
                {
                    MessageBox.Show("Сообщение отправлено!");
                    MessageTextBox.Text = "";
                    UpdateMessages(ChatListBox);
                }
                else
                {
                    MessageBox.Show("Сообщение не отправлено!");
                }
            }
            else
            {
                MessageBox.Show("Введите сообщение!");
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateMessages(ChatListBox);
        }
    }

    public class UserInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class Message
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Sended { get; set; }
    }
}
