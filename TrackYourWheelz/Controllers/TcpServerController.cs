using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrackYourWheelz.Models.DTO;
using TrackYourWheelz.Models.Models;
using TrackYourWheelz.Repositorys;

namespace TrackYourWheelz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TcpServerController : ControllerBase
    {
        private readonly ITcpServerRepository _tcpServerRepository;

        public TcpServerController(ITcpServerRepository tcpServerRepository)
        {
            _tcpServerRepository = tcpServerRepository;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetYourServerDetails()
        {
            try
            {
                // Get the local host name
                string hostName = Dns.GetHostName();

                // Get the IP addresses associated with the local host
                var ipAddresses = Dns.GetHostAddresses(hostName);

                // Get the currently active IP address (IPv4)
                var currentIpAddress = string.Empty;
                foreach (var ip in ipAddresses)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        currentIpAddress = ip.ToString();
                        break;
                    }
                }

                if (string.IsNullOrEmpty(currentIpAddress))
                {
                    return NotFound("No IPv4 address found.");
                }

                // http Port number
                var port = 5121;

                // Return IP address and port details
                var serverDetails = new
                {
                    IpAddress = currentIpAddress,
                    Port = port
                };

                return Ok(serverDetails);
            }
            catch (Exception ex)
            {
                // Return error response in case of an exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartTcpServer([FromBody] TcpServerConfigDto configDto, CancellationToken cancellationToken)
        {
            if (_tcpServerRepository.IsServerRunning())
            {
                return BadRequest("Server is already running.");
            }

            TcpServerConfig config = new TcpServerConfig
            {
                IPAddress = configDto.IPAddress,
                Port = configDto.Port
            };

            await _tcpServerRepository.StartServerAsync(config, cancellationToken);
            return Ok($"Server started on {config.IPAddress}:{config.Port}");
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopTcpServer(CancellationToken cancellationToken)
        {
            if (!_tcpServerRepository.IsServerRunning())
            {
                return BadRequest("Server is not running.");
            }

            await _tcpServerRepository.StopServerAsync(cancellationToken);
            return Ok("Server stopped.");
        }

        [HttpGet("status")]
        public IActionResult GetServerStatus()
        {
            bool isRunning = _tcpServerRepository.IsServerRunning();
            return Ok(new { IsRunning = isRunning });
        }
    }
}
