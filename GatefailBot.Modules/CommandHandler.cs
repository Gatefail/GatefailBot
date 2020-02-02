using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace GatefailBot.Modules
{
    public class CommandHandler
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _provider;

        public CommandHandler(CommandService commandService, DiscordSocketClient client, ILogger<CommandHandler> logger, IServiceProvider provider)
        {
            _commandService = commandService;
            _client = client;
            _logger = logger;
            _provider = provider;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            var ass = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name.Equals(GetType().Namespace));
            await _commandService.AddModulesAsync(ass, _provider);
        }
        
        
        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) || 
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            // Keep in mind that result does not indicate a return value
            // rather an object stating if the command executed successfully.
            var result = await _commandService.ExecuteAsync(
                context: context, 
                argPos: argPos,
                services: _provider);

            // Optionally, we may inform the user if the command fails
            // to be executed; however, this may not always be desired,
            // as it may clog up the request queue should a user spam a
            // command.
            if (result.IsSuccess)
            {
                // Add 
                try
                {
                    // ❌ 
                    await context.Message.AddReactionAsync(new Emoji("\u274C"),
                        new RequestOptions()
                        {
                            RetryMode = RetryMode.AlwaysFail
                        });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Failed reacting to message ID: {context.Message.Id}");
                }
            }
        }
    }
}