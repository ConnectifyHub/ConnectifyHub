using System.Net.Sockets;
using System.Text;

namespace Main
{
    public class ServerCommunication
    {
        private static ServerCommunication instance;
        private TcpClient client;
        private NetworkStream stream;

        private ServerCommunication(string serverAddress, int serverPort)
        {
            client = new TcpClient(serverAddress, serverPort);
            stream = client.GetStream();
        }

        public static ServerCommunication Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerCommunication("127.0.0.1", 1234);
                }
                return instance;
            }
        }

        public string SendAndReceive(string message)
        {
            if (message.Contains("|"))
            {
                string[] parts = message.Split('|');
                if (parts[1].Length == 0) return null;
            }
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            byte[] responseBuffer = new byte[1024];
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            return Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
            
        }

        public void CloseConnection()
        {
            stream.Close();
            client.Close();
        }
    }
}