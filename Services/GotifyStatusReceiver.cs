using System.Threading;
using System.Threading.Tasks;
using LupuServ.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tiny.RestClient;

namespace LupuServ.Services
{
    internal class GotifyStatusReceiver : IStatusReceiver
    {
        private readonly IConfiguration _config;

        private readonly ILogger<GotifyStatusReceiver> _logger;

        private readonly TinyRestClient _restClient;

        public GotifyStatusReceiver(IConfiguration config, ILogger<GotifyStatusReceiver> logger,
            TinyRestClient restClient)
        {
            _config = config;
            _logger = logger;
            _restClient = restClient;
        }

        public async Task ProcessMessageAsync(MessagePacket message, CancellationToken cancellationToken = default)
        {
            var token = _config.GetSection("Gotify:Status:AppToken").Value;

            var request = await _restClient.PostRequest($"/message?token{token}")
                .AddFormParameter("priority", "1")
                .AddFormParameter("title", "Status Update")
                .AddFormParameter("message", message.ToString())
                .ExecuteAsync<GotifyResponse>(cancellationToken);

            _logger.LogInformation("Request result: {0}", request);
        }
    }
}