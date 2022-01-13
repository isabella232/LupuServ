using System;
using System.Threading;
using System.Threading.Tasks;
using LupuServ.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tiny.RestClient;

namespace LupuServ.Services
{
    internal class GotifyAlarmReceiver : IAlarmReceiver
    {
        private readonly IConfiguration _config;

        private readonly ILogger<GotifyAlarmReceiver> _logger;

        private readonly TinyRestClient _restClient;

        public GotifyAlarmReceiver(IConfiguration config, ILogger<GotifyAlarmReceiver> logger,
            TinyRestClient restClient)
        {
            _config = config;
            _logger = logger;
            _restClient = restClient;
        }

        public async Task ProcessMessageAsync(MessagePacket message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing alarm message to deliver via Gotify");

            try
            {
                var token = _config.GetSection("Gotify:Alarm:AppToken").Value;
                var title = _config.GetSection("Gotify:Alarm:Title").Value;
                var priority = _config.GetSection("Gotify:Alarm:Priority").Value;

                var request = await _restClient.PostRequest($"message?token={token}")
                    .AddFormParameter("priority", priority)
                    .AddFormParameter("title", title)
                    .AddFormParameter("message", message.ToString())
                    .ExecuteAsync<GotifyResponse>(cancellationToken);

                _logger.LogInformation("Request result: {0}", request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deliver status message");
            }
        }
    }
}