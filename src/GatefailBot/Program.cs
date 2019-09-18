using System.Threading;
using System.Threading.Tasks;
using GatefailBot.Database;
using GatefailBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            var serviceProvider = new ServiceCollection()
                .Configure<BotConfiguration>(configuration)
                .AddDbContext<GatefailContext>(op => op.UseNpgsql(configuration.GetBotDbConnectionString()))
                .AddHttpClient()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IGuildService, GuildService>()
                .AddTransient<ICommandChannelRestrictionService, CommandChannelRestrictionService>()
                .BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<GatefailContext>();

            dbContext.Database.Migrate();

            var bot = new Bot(serviceProvider, botConfiguration);

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
    }
}