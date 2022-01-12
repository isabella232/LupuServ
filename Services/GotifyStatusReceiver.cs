using System.Threading;
using System.Threading.Tasks;
using LupuServ.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LupuServ.Services
{
    internal class GotifyStatusReceiver : IStatusReceiver
    {
        private readonly IConfiguration _config;

        private readonly ILogger<GotifyStatusReceiver> _logger;

        public GotifyStatusReceiver(IConfiguration config, ILogger<GotifyStatusReceiver> logger)
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
