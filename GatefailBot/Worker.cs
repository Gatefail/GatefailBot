using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GatefailBot.Modules;
using GatefailBot.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GatefailBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _handler;
        private readonly string _token;

        public Worker(DiscordSocketClient client, CommandHandler handler, ILogger<Worker> logger, IOptions<BotOptions> opt, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
            _handler = handler;
            _token = opt.Value.Token;
            _client = client;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Ready += async () =>
            {
                _logger.LogInformation("BOT IS READY");
                await _client.SetGameAsync($"Running since: {DateTimeOffset.UtcNow.ToString("s", CultureInfo.InvariantCulture)}Z");
            };
            await _handler.InstallCommandsAsync(_provider);
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(-1, stoppingToken);
        }
    }
}