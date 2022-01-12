using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmtpServer;
using ServiceProvider = SmtpServer.ComponentModel.ServiceProvider;

namespace LupuServ
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .AddEnvironmentVariables()
                    .Build();

                var port = int.Parse(config.GetSection("Port").Value);

                var serviceProvider = new ServiceProvider();
                serviceProvider.Add(new LupusMessageStore(config, _logger, _scopeFactory));

                var options = new SmtpServerOptionsBuilder()
                    .ServerName("localhost")
                    .Port(port)
                    .Build();

                _logger.LogInformation($"Starting SMTP server on port {port}");

                var smtpServer = new SmtpServer.SmtpServer(options, serviceProvider);

                await smtpServer.StartAsync(stoppingToken);
            }
        }
    }
}