using Server;
using Server.Data.Entities;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class TcpServer
    {
        private TcpListener _tcpListener;
        private Thread _listenerThread;

        public TcpServer()
        {
            _tcpListener = new TcpListener(IPAddress.Any, 1234);
            _listenerThread = new Thread(new ThreadStart(ListenForClients));
            _listenerThread.Start();
            Console.WriteLine("Server started!");
        }

        private void ListenForClients()
        {
            _tcpListener.Start();

            while (true)
            {
                TcpClient client = _tcpListener.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private readonly Dictionary<string, Func<string[], bool>> validationMethods = new Dictionary<string, Func<string[], bool>>
        {
            { "IsValidEmail", ValidationHelper.IsValidEmail },
            { "IsStrongPassword", ValidationHelper.IsStrongPassword },
            { "IsValidFirstName", ValidationHelper.IsValidFirstName },
            { "IsValidLastName", ValidationHelper.IsValidLastName },
            { "IsValidPhoneNumber", ValidationHelper.IsValidPhoneNumber },
            { "Login", ValidationHelper.IsValidEmailAndPassword },
        };

        private readonly Dictionary<string, Action<User>> databaseMethods = new Dictionary<string, Action<User>>
        {
            { "RegisterMe", DatabaseUtils.AddUser },
        };

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
                    string leftPart = parts[0];

                    bool isValid = false;
                    string customMessage = null;

                    if (parts[1].Length != 0)
                    {
                        if (databaseMethods.TryGetValue(leftPart, out var databaseMethod))
                        {
                            string userDataString = receivedMessage.Substring(leftPart.Length + 1);
                            User userData = DataStrParser.ParseUserFromString(userDataString);
                            string methodName = parts[0];

                            databaseMethod(userData);
                            isValid = true;
                        }
                        if (validationMethods.TryGetValue(leftPart, out var validationMethod))
                        {
                            isValid = validationMethod(parts);
                        }

                        if (leftPart == "LatestLoginInfo")
                        {
                            var user = DatabaseUtils.GetUserByKey(parts[1]);
                            customMessage = user != null ? user.Email : null;
                        }

                        if (leftPart == "WhoAmI")
                        {
                            customMessage = DatabaseUtils.GetUserByKeyStringified(parts[1]);
                        }

                        if ((leftPart == "Login" || leftPart == "RegisterMe") && isValid == true)
                        {
                            customMessage = DatabaseUtils.GeneratePublicKey(parts[1]);
                        }
                    }

                    Console.WriteLine($"Sending response to client: {(customMessage != null ? customMessage : isValid)}");
                    byte[] responseData = Encoding.UTF8.GetBytes(customMessage != null ? customMessage.ToString() : isValid.ToString());
                    clientStream.Write(responseData, 0, responseData.Length);

                }
            }

            tcpClient.Close();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        TcpServer server = new TcpServer();
    }
}