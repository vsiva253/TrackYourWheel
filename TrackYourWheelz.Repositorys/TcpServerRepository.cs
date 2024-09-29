using System.Net.Sockets;
using System.Net;
using System.Text;
using TrackYourWheelz.Models.Models;

namespace TrackYourWheelz.Repositorys
{
    public class TcpServerRepository : ITcpServerRepository
    {
        private TcpListener _listener;
        private bool _isRunning;

        public TcpServerRepository()
        {
            _isRunning = false;
        }

        public async Task StartServerAsync(TcpServerConfig config, CancellationToken cancellationToken)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Server is already running.");
            }

            IPAddress ipAddress = IPAddress.Parse(config.IPAddress);
            _listener = new TcpListener(ipAddress, config.Port);
            _listener.Start();
            _isRunning = true;

            Console.WriteLine($"TCP Server started on {config.IPAddress}:{config.Port}");

            while (!cancellationToken.IsCancellationRequested)
            {
                if (_listener.Pending())
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClientAsync(client));
                }

                await Task.Delay(500, cancellationToken);  // Poll every 500ms
            }

            _listener.Stop();
            _isRunning = false;
            Console.WriteLine("TCP Server stopped.");
        }

        public Task StopServerAsync(CancellationToken cancellationToken)
        {
            if (!_isRunning)
            {
                throw new InvalidOperationException("Server is not running.");
            }

            _listener.Stop();
            _isRunning = false;

            Console.WriteLine("TCP Server stopped manually.");
            return Task.CompletedTask;
        }

        public bool IsServerRunning()
        {
            return _isRunning;
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received from client: " + receivedData);

                    // Send response to client
                    string response = "Hello from TCP Server!";
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseData, 0, responseData.Length);
                }
            }
            finally
            {
                client.Close();
            }
        }
    }
}
