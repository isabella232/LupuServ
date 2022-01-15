using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using LupuServ.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Tiny.RestClient;

namespace LupuServ
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            CreateHostBuilder(args).UseSerilog().Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped(provider =>
                    {
                        var endpoint = provider.GetRequiredService<IConfiguration>().GetSection("Gotify:Url").Value;
                        
                        return new TinyRestClient(new HttpClient(), endpoint);
                    });

                    services.AddScoped<IStatusReceiver, GotifyStatusReceiver>();
                    services.AddScoped<IAlarmReceiver, SmsAlarmReceiver>();
                    services.AddScoped<IAlarmReceiver, GotifyAlarmReceiver>();

                    services.AddHostedService<Worker>();
                });
        }
    }
}