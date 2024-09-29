using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackYourWheelz.Models.Models;

namespace TrackYourWheelz.Repositorys
{
    public interface ITcpServerRepository
    {
        Task StartServerAsync(TcpServerConfig config, CancellationToken cancellationToken);
        Task StopServerAsync(CancellationToken cancellationToken);
        bool IsServerRunning();
    }
}
