using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using GatefailBot.Database;
using GatefailBot.Helpers;
using GatefailBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Serilog.Core;

namespace GatefailBot
{
    public static class Program
    {
        private static CancellationTokenSource _cts;

        public static async Task Main()
        {
            _cts = new CancellationTokenSource();

            Log.Logger = CreateLogger();

            Log.Debug("Starting up bot...");

            var configuration = BotConfigurationBuilder.Build();

            var botConfiguration = new BotConfiguration();
            configuration.Bind(botConfiguration);
            
            var memCacheOpt = Options.Create(new MemoryCacheOptions()
            {
                ExpirationScanFrequency = TimeSpan.FromSeconds(30)
            });
            
            var moduleContainer = GetLoadedCommandModules();
            var services = new ServiceCollection()
                .Configure<BotConfiguration>(configuration)
                .AddDbContext<GatefailContext>(op => op.UseNpgsql(configuration.GetBotDbConnectionString()))
                .AddTransient<IUserService, UserService>()
                .AddTransient<IGuildService, GuildService>()
                .AddTransient<ICommandChannelRestrictionService, CommandChannelRestrictionService>()
                .AddTransient<ICachedModuleService, CachedModuleService>()
                .AddSingleton<IMemoryCache>(provider => new MemoryCache(memCacheOpt))
                .AddSingleton<ModuleContainer>(provider => moduleContainer);
            services.AddHttpClient<IMagicHttpService, MagicHttpService>()
                .ConfigureHttpClient(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    client.MaxResponseContentBufferSize = 5000000;
                });
            services.AddHttpClient<IHastebinService, HastebinService>()
                .ConfigureHttpClient(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    client.MaxResponseContentBufferSize = 5000000;
                });
            var serviceProvider = services.BuildServiceProvider();
            
            var dbContext = serviceProvider.GetRequiredService<GatefailContext>();

            dbContext.Database.Migrate();

            var bot = new Bot(serviceProvider, botConfiguration);
            GetLoadedCommandModules();

            Log.Debug("Running bot...");

            await bot.Run(_cts.Token).ConfigureAwait(false);
        }

        private static Logger CreateLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10,
                    shared: true)
                .CreateLogger();
        }

        private static ModuleContainer GetLoadedCommandModules()
        {
            var modules = Assembly.GetExecutingAssembly().ExportedTypes
                .Where(t => t.IsSubclassOf(typeof(BaseCommandModule)))
                .Where(t => t.GetCustomAttributes(typeof(CheckModuleEnabled), true).Length > 0)
                .GroupBy(t => t.Name, type => type)
                .ToDictionary(g => g.Key, g => g.First());

            return new ModuleContainer(modules);
        }
    }
}