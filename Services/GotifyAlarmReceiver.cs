using System.Threading;
using System.Threading.Tasks;
using LupuServ.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LupuServ.Services
{
    internal class GotifyAlarmReceiver : IAlarmReceiver
    {
        private readonly IConfiguration _config;

        private readonly ILogger<GotifyAlarmReceiver> _logger;

        public GotifyAlarmReceiver(IConfiguration config, ILogger<GotifyAlarmReceiver> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Task ProcessMessageAsync(MessagePacket message, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
