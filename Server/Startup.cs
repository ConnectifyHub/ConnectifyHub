using Server;
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
                    bool correct = false;
                    bool responce = false;

                    if (leftPart == "IsValidEmail")
                    {
                        correct = true;
                        responce = ValidationHelper.IsValidEmail(parts[1]);
                    }

                    if (leftPart == "IsStrongPassword")
                    {
                        correct = true;
                        responce = ValidationHelper.IsStrongPassword(parts[1]);
                    }

                    if (leftPart == "IsValidFirstName")
                    {
                        correct = true;
                        responce = ValidationHelper.IsValidFirstName(parts[1]);
                    }

                    if (leftPart == "IsValidLastName")
                    {
                        correct = true;
                        responce = ValidationHelper.IsValidLastName(parts[1]);
                    }

                    if (leftPart == "IsValidPhoneNumber")
                    {
                        correct = true;
                        responce = ValidationHelper.IsValidPhoneNumber(parts[1]);
                    }

                    if (correct) {
                        Console.WriteLine("Sending responce to client: " + responce.ToString());
                        byte[] responseData = Encoding.UTF8.GetBytes(responce.ToString());
                        clientStream.Write(responseData, 0, responseData.Length);
                    }
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