using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GatefailBot.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GatefailBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _token;
        private readonly DiscordSocketClient _client;

        public Worker(ILogger<Worker> logger, IOptions<BotOptions> opt, DiscordSocketClient client)
        {
            _logger = logger;
            _token = opt.Value.Token;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            _client.MessageReceived += MessageReceived;
            _client.Ready += async () =>
            {
                _logger.LogInformation("BOT IS READY");
                await _client.SetGameAsync($"{DateTimeOffset.Now}");
            };
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            await Task.Delay(-1, stoppingToken);
        }
        
        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Ostereje!");
            }
        }
    }
}