using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LupuServ.Services;
using LupuServ.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace LupuServ
{
    /// <summary>
    ///     Handles incoming SMTP messages.
    /// </summary>
    public class LupusMessageStore : MessageStore
    {
        public LupusMessageStore(IConfiguration config, ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _config = config;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        private readonly IConfiguration _config;

        private readonly ILogger<Worker> _logger;

        private readonly IServiceScopeFactory _scopeFactory;

        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction,
            ReadOnlySequence<byte> buffer,
            CancellationToken cancellationToken)
        {
            var statusUser = _config.GetSection("StatusUser").Value;
            var alarmUser = _config.GetSection("AlarmUser").Value;

            await using var stream = new MemoryStream();

            var position = buffer.GetPosition(0);
            while (buffer.TryGet(ref position, out var memory)) await stream.WriteAsync(memory, cancellationToken);

            stream.Position = 0;

            var message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);

            var user = transaction.To.First().User;

            //
            // Call Alarm handlers
            // 
            if (user.Equals(alarmUser, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogInformation("Received alarm event");

                var template = _config.GetSection("Templates:Alarm").Value;

                using var scope = _scopeFactory.CreateScope();

                foreach (var receiver in scope.ServiceProvider.GetServices<IAlarmReceiver>())
                    await receiver.ProcessMessageAsync(MessagePacket.DecodeFrom(message.TextBody, template),
                        cancellationToken);
            }

            //
            // Call Status handlers
            // 
            if (user.Equals(statusUser, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogInformation($"Received status change: {message.TextBody}");

                var template = _config.GetSection("Templates:Status").Value;

                using var scope = _scopeFactory.CreateScope();

                foreach (var receiver in scope.ServiceProvider.GetServices<IStatusReceiver>())
                    await receiver.ProcessMessageAsync(MessagePacket.DecodeFrom(message.TextBody, template),
                        cancellationToken);
            }
            //
            // Anything unknown goes into the logs
            // 
            else
            {
                _logger.LogWarning($"Unknown message received (maybe a test?): {message.TextBody}");
            }

            //
            // There isn't really any benefit to report an error back to the station
            // 
            return SmtpResponse.Ok;
        }
    }
}