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
            try
            {
                client = new TcpClient(serverAddress, serverPort);
                stream = client.GetStream();
            } catch {
                ShowConnectionErrorDialog();
                return;
            }
        }

        public static ServerCommunication Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServerCommunication("monorail.proxy.rlwy.net", 47917);
                }
                return instance;
            }
        }

        public string SendAndReceive(string endpoint, string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(endpoint + "|" + message);
                stream.Write(data, 0, data.Length);

                byte[] responseBuffer = new byte[1024];
                int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
                return Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
            }
            catch
            {
                ShowConnectionErrorDialog();
                return null;
            }
        }

        public void CloseConnection()
        {
            stream.Close();
            client.Close();
        }

        private void ShowConnectionErrorDialog()
        {
            var dialog = new ConnectionErrorDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                RetryConnection();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void RetryConnection()
        {
            try
            {
                instance = Instance;
            }
            catch
            {
                ShowConnectionErrorDialog();
            }
        }
    }
}