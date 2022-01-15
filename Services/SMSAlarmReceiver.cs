﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Text;
using LupuServ.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LupuServ.Services
{
    /// <summary>
    ///     An <see cref="IAlarmReceiver"/> that sens mobile carrier messages (SMS).
    /// </summary>
    internal class SmsAlarmReceiver : IAlarmReceiver
    {
        private readonly IConfiguration _config;

        private readonly ILogger<SmsAlarmReceiver> _logger;

        public SmsAlarmReceiver(IConfiguration config, ILogger<SmsAlarmReceiver> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task ProcessMessageAsync(IMessagePacket message, CancellationToken cancellationToken = default)
        {
            try
            {
                var apiKey = Guid.Parse(_config.GetSection("ApiKey").Value);

                var from = _config.GetSection("From").Value;

                var recipients = _config.GetSection("Recipients").GetChildren().Select(e => e.Value).ToList();

                _logger.LogInformation(
                    $"Will send alarm SMS to the following recipients: {string.Join(", ", recipients)}");

                var client = new TextClient(apiKey);

                var result = await client
                    .SendMessageAsync(message.MessageText, from, recipients, "Alarm", cancellationToken)
                    .ConfigureAwait(false);

                if (result.statusCode == TextClientStatusCode.Ok)
                {
                    _logger.LogInformation(
                        $"Successfully sent the following message to recipients: {message}");

                    return;
                }

                _logger.LogError($"Message delivery failed: {result.statusMessage} ({result.statusCode})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deliver status message");
            }
        }
    }
}