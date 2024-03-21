using Server.Data.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server
{
    class TcpServer
    {
        private TcpListener _tcpListener;
        private readonly Dictionary<string, Func<string[], bool>> _validationMethods;
        private readonly Dictionary<string, Action<User>> _databaseMethods;

        public TcpServer()
        {
            _tcpListener = new TcpListener(IPAddress.Any, 1234);
            _validationMethods = new Dictionary<string, Func<string[], bool>>
            {
                { "IsValidEmail", ValidationHelper.IsValidEmail },
                { "IsStrongPassword", ValidationHelper.IsStrongPassword },
                { "IsValidFirstName", ValidationHelper.IsValidFirstName },
                { "IsValidLastName", ValidationHelper.IsValidLastName },
                { "IsValidPhoneNumber", ValidationHelper.IsValidPhoneNumber },
                { "Login", ValidationHelper.IsValidEmailAndPassword }
            };
            _databaseMethods = new Dictionary<string, Action<User>>
            {
                { "RegisterMe", DatabaseUtils.AddUser },
            };

            Thread _listenerThread = new Thread(ListenForClients);
            _listenerThread.Start();
            Console.WriteLine("Server started!");
        }

        private void ListenForClients()
        {
            _tcpListener.Start();

            while (true)
            {
                TcpClient client = _tcpListener.AcceptTcpClient();
                Thread clientThread = new Thread(HandleClientComm);
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object clientObj)
        {
            TcpClient tcpClient = (TcpClient)clientObj;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                    break;

                string receivedMessage = Encoding.UTF8.GetString(message, 0, bytesRead);
                Console.WriteLine($"Received message from client: {receivedMessage}");

                if (receivedMessage.Contains("|"))
                {
                    string[] parts = receivedMessage.Split('|');

                    Console.WriteLine("Endpoint " + parts[0]);

                    string customMessage = null;
                    bool isValid = false;

                    if (parts[1].Length > 0)
                    {
                        switch (parts[0])
                        {
                            case "SendMessage":
                                DatabaseUtils.SendMessage(parts[1], parts[2], parts[3]);
                                isValid = true;
                                break;
                            case "GetMessages":
                                if (int.TryParse(parts[2], out int chatId))
                                {
                                    customMessage = DatabaseUtils.GetMessagesFromChat(parts[1], chatId);
                                    isValid = true;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid chat ID format.");
                                }
                                break;
                            case "WhoAmI":
                                customMessage = DatabaseUtils.GetUserByKeyStringified(parts[1]);
                                isValid = true;
                                break;
                            case "LatestLoginInfo":
                                var user = DatabaseUtils.GetUserByKey(parts[1]);
                                customMessage = user != null ? user.Email : null;
                                if (user != null)
                                {
                                    isValid = true;
                                }
                                break;
                        }


                        if (_databaseMethods.TryGetValue(parts[0], out var databaseMethod))
                        {
                            User userData = DataStrParser.ParseUserFromString(parts[1]);
                            databaseMethod(userData);
                            isValid = true;
                        }
                        else if (_validationMethods.TryGetValue(parts[0], out var validationMethod))
                        {
                            Console.WriteLine("Received data parts:");
                            foreach (var part in parts.Skip(1))
                            {
                                Console.WriteLine(part);
                            }

                            isValid = validationMethod(parts.Skip(1).ToArray());
                        }

                        if (isValid)
                        {
                            if (parts[0] == "Login" || parts[0] == "RegisterMe")
                            {
                                Console.WriteLine("Generating PublicKey");
                                customMessage = DatabaseUtils.GeneratePublicKey(parts[1]);
                                Console.WriteLine(customMessage);
                            }
                        } else {
                            customMessage = "False";
                        }
                    }
                    else
                    {
                        customMessage = "False";
                    }

                    if (customMessage != null) {
                        Console.WriteLine(customMessage);
                    }

                    var ready_answer = !string.IsNullOrEmpty(customMessage) ? customMessage : isValid.ToString();
                    Console.WriteLine($"Sending response to client: " + ready_answer);
                    byte[] responseData = Encoding.UTF8.GetBytes(ready_answer);
                    clientStream.Write(responseData, 0, responseData.Length);
                }
            }

            tcpClient.Close();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TcpServer server = new TcpServer();
        }
    }
}