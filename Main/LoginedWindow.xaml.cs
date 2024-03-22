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

        string jsonContent = File.ReadAllText("registration_data.json");

        private void UpdateUser()
        {
            string[] whoami = (serverCommunication.SendAndReceive("WhoAmI", jsonContent)).Split("|");
            User = new
            {
                Email = whoami[0],
                Name = whoami[1],
                Surname = whoami[2],
                Chat = whoami[3],
                ID = whoami[4],
            };
        }

        public LoginedWindow()
        {
            InitializeComponent();
            UpdateUser();
            WelcomeTextBlock.Text = $"С возвращением, (#{User.ID}) {User.Name} {User.Surname}!";

            string chatsResponse = serverCommunication.SendAndReceive("WhatIsMyChats", jsonContent);
            string[] chatDetails = chatsResponse.Split("||");
            ChatSelectionComboBox.Items.Clear();
            foreach (string chatDetail in chatDetails)
            {
                string[] parts = chatDetail.Split('|');
                if (parts.Length >= 2)
                {
                    string chatName = parts[0];
                    if (int.TryParse(parts[1], out int chatId))
                    {
                        ChatSelectionComboBox.Items.Add(new ComboBoxItem { Content = chatName, Tag = chatId });
                    }
                }
            }
        }

        private void UpdateMessages(ListBox listBox)
        {
            string messages = serverCommunication.SendAndReceive("GetMessages", $"{User.Chat}|0");
            string[] messages_parts = messages.Split("||");

            messagesList.Clear();

            for (int i = 0; i < messages_parts.Length - 1; i++)
            {
                var one_message = new Message
                {
                    Name = $"(#{messages_parts[i].Split("|")[3]})" + messages_parts[i].Split("|")[0],
                    Text = messages_parts[i].Split("|")[1],
                    Sended = messages_parts[i].Split("|")[2]
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
               
                string response = serverCommunication.SendAndReceive("SendMessage", $"{message}|{jsonContent}|{User.Chat}");
                if (response == "True")
                {
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

        private void UserIdTextBox_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (UserIdTextBox.Text == "Укажите User ID для чата")
            {
                UserIdTextBox.Text = "";
            }
        }

        private void UserIdTextBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserIdTextBox.Text))
            {
                UserIdTextBox.Text = "Укажите User ID для чата";
            } else
            {
                serverCommunication.SendAndReceive("CreateChatWith", $"{jsonContent}|{UserIdTextBox.Text}");
            }
        }

        private void ChatSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedComboBoxItem = (ComboBoxItem)ChatSelectionComboBox.SelectedItem;
            if (selectedComboBoxItem != null && selectedComboBoxItem.Tag != null && int.TryParse(selectedComboBoxItem.Tag.ToString(), out int chatId))
            {
                var res = serverCommunication.SendAndReceive("SwitchChat", $"{jsonContent}|{chatId}");
                if (res == "True")
                {
                    UpdateUser();
                    UpdateMessages(ChatListBox);
                }
            }
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
