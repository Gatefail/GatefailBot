using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GatefailBot.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GatefailBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Information("Starting up...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Fatal exception");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(ConfigureLogging)
                .ConfigureServices(ConfigureServices);

        private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs\\vital_information.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10,
                    shared: true)
                .CreateLogger();

            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(logger);
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var client = new DiscordSocketClient();
            client.Log += LogDiscordMessage;
            services.Configure<BotOptions>(context.Configuration.GetSection("GF"));
            services.AddHostedService<Worker>();
            services.AddSingleton(client);
        }

        private static Task LogDiscordMessage(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Debug:
                    Log.Debug(msg.Message);
                    break;
                case LogSeverity.Info:
                    Log.Information(msg.Message);
                    break;
                case LogSeverity.Warning:
                    Log.Warning(msg.Message);
                    break;
                case LogSeverity.Error:
                    Log.Error(msg.Exception, msg.Message);
                    break;
                case LogSeverity.Critical:
                    Log.Error(msg.Exception, msg.Message);
                    break;
                default:
                    Log.Debug(msg.Message);
                    break;
                    
            }

            return Task.CompletedTask;
        }
    }
}