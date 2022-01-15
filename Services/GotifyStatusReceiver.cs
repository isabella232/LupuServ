using System;
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

        private readonly IConfigurationSection _section;

        public GotifyStatusReceiver(IConfiguration config, ILogger<GotifyStatusReceiver> logger,
            TinyRestClient restClient)
        {
            _config = config;
            _logger = logger;
            _restClient = restClient;

            _section = _config.GetRequiredSection("Gotify:Status");
        }

        public async Task ProcessMessageAsync(IMessagePacket message, CancellationToken cancellationToken = default)
        {
            if (!bool.TryParse(_section.GetSection("IsEnabled")?.Value, out var isEnabled) || !isEnabled)
                return;

            _logger.LogInformation("Processing message to deliver via Gotify");

            try
            {
                var token = _section.GetSection("AppToken").Value;
                var title = _section.GetSection("Title").Value;
                var priority = _section.GetSection("Priority").Value;

                var response = await _restClient.PostRequest($"message?token={token}")
                    .AddFormParameter("priority", priority)
                    .AddFormParameter("title", title)
                    .AddFormParameter("message", message.MessageText)
                    .ExecuteAsync<GotifyResponse>(cancellationToken);

                _logger.LogInformation("Request result: {Response}", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deliver status message");
            }
        }
    }
}