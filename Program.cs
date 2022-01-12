using System;
using System.Globalization;
using System.IO;
using System.Threading;
using LupuServ.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LupuServ
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<IStatusReceiver, GotifyStatusReceiver>();
                    services.AddScoped<IAlarmReceiver, SmsAlarmReceiver>();
                    services.AddScoped<IAlarmReceiver, GotifyAlarmReceiver>();

                    services.AddHostedService<Worker>();
                });
        }
    }
}