using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace V5LocationTrackerServer
{
    class DeviceConnection
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public DeviceConnection(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public void SendData(string message)
        {
            if (_stream == null)
            {
                Console.WriteLine("No active connection to send data.");
                return;
            }

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);
                Console.WriteLine($"Sent data: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data: {ex.Message}");
                CloseConnection();
            }
        }

        public void ListenForData()
        {
            if (_stream == null)
            {
                Console.WriteLine("No active connection to listen for data.");
                return;
            }

            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = _stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received data: {receivedData}");

                    // Simulate processing delay
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
        }

        public void CloseConnection()
        {
            if (_stream != null)
            {
                _stream.Close();
            }

            if (_client != null)
            {
                _client.Close();
            }

            Console.WriteLine("Connection closed.");
        }
    }

    class TcpServer
    {
        private TcpListener _listener;

        public TcpServer(string ipAddress, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine($"Server started on {_listener.LocalEndpoint}");

            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

                DeviceConnection connection = new DeviceConnection(client);

                Thread listenThread = new Thread(connection.ListenForData);
                listenThread.Start();

                // Example of sending a message to the client every 5 seconds
                Thread commandThread = new Thread(() =>
                {
                    while (true)
                    {
                        connection.SendData("Hello Device");
                        Thread.Sleep(5000);
                    }
                });

                commandThread.Start();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string ipAddress = "0.0.0.0";  // Listen on all available IPs
            int port = 8080;  // Use port 8080 for the server

            TcpServer server = new TcpServer(ipAddress, port);
            server.Start();  // Start the TCP server
        }
    }
}
