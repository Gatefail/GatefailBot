using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using GatefailBot.Helpers;
using GatefailBot.Modules.Help;
using GatefailBot.Services;
using Serilog;

namespace GatefailBot
{
    internal class Bot
    {
        private const string COMMAND_PREFIX = "!";
        private readonly BotConfiguration _botConfiguration;
        private readonly CommandsNextExtension _commands;
        private readonly DiscordClient _discordClient;

        private readonly string[] _ignoredLogMessages = {"Socket connection terminated", "Ratelimit hit"};

        private readonly IServiceProvider _serviceProvider;
        private bool _clientIsReady;

        internal Bot(IServiceProvider serviceProvider,
            BotConfiguration botConfiguration)
        {
            _serviceProvider = serviceProvider;
            _botConfiguration = botConfiguration;

            var discordConfig = new DiscordConfiguration
            {
                Token = _botConfiguration.DiscordToken,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = false,
                ReconnectIndefinitely = true
            };

            _discordClient = new DiscordClient(discordConfig);

            var interactivityConfig = new InteractivityConfiguration {PaginationBehaviour = PaginationBehaviour.Ignore};

            _discordClient.UseInteractivity(interactivityConfig);

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] {COMMAND_PREFIX},
                Services = serviceProvider,
                CaseSensitive = false,
                EnableMentionPrefix = false
            };
            _commands = _discordClient.UseCommandsNext(commandsConfig);
            _commands.RegisterCommands(Assembly.GetExecutingAssembly());
            _commands.SetHelpFormatter<HelpFormatter>();

            _discordClient.DebugLogger.LogMessageReceived += OnLogMessageReceived;
            _discordClient.GuildCreated += OnGuildAvailable;
            _discordClient.GuildDeleted += OnGuildDeleted;
            _discordClient.GuildMemberAdded += OnGuildMemberAdded;
            _discordClient.GuildMemberRemoved += OnGuildMemberRemoved;
            _discordClient.SocketErrored += OnSocketErrored;
            _discordClient.SocketClosed += OnSocketClosed;
            _discordClient.Ready += _ => Task.FromResult(_clientIsReady = true);
            _commands.CommandErrored += OnCommandError;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await _discordClient.ConnectAsync().ConfigureAwait(false);

            // Start an infinite delay to wait for, so we don't shut down
            await Task.Delay(-1, cancellationToken).ConfigureAwait(false);
        }

        private void OnLogMessageReceived(object sender, DebugLogMessageEventArgs logMessage)
        {
            switch (logMessage.Level)
            {
                case LogLevel.Debug:
                    Log.Debug(logMessage.Exception, logMessage.Message);
                    break;
                case LogLevel.Info:
                    Log.Information(logMessage.Exception, logMessage.Message);
                    break;
                case LogLevel.Warning:
                    Log.Warning(logMessage.Exception, logMessage.Message);
                    break;
                case LogLevel.Error:
                    Log.Error(logMessage.Exception, logMessage.Message);
                    break;
                case LogLevel.Critical:
                    Log.Fatal(logMessage.Exception, logMessage.Message);
                    break;
            }

            // There are set of log messages we want to swallow and never show to the
            // user or log to Discord itself, therefore check if we want to send
            // the message to Discord
            if (logMessage.Level == LogLevel.Critical
                && !_ignoredLogMessages.Any(d => logMessage.Message.StartsWith(d)))
            {
                // Fire and forget
                _ = SendLogToDiscord(logMessage);
            }
        }

        private async Task OnGuildAvailable(GuildCreateEventArgs e)
        {
            Log.Information("New guild added. Creating base dataset");
            var guildservice = (IGuildService) _serviceProvider.GetService(typeof(IGuildService));
            await guildservice.EnsureCreated(e.Guild.Id);
            
            Log.Information($"Adding initial members to Guild: {e.Guild.Name}");
            var userservice = (IUserService) _serviceProvider.GetService(typeof(IUserService));
            var guildSnowflake = e.Guild.Id;
            await userservice.EnsureCreatedMany(guildSnowflake, e.Guild.Members.Where(kvp => !kvp.Value.IsBot).Select(kvp => kvp.Key));
        }

        private async Task OnGuildDeleted(GuildDeleteEventArgs e)
        {
            var guildservice = (IGuildService) _serviceProvider.GetService(typeof(IGuildService));
            await guildservice.DeleteGuild(e.Guild.Id);
            Log.Information($"Guild Deleted - {e.Guild.Name}");
        }

        private async Task OnGuildMemberAdded(GuildMemberAddEventArgs e)
        {
            if (e.Member.IsBot) return;
            
            var userservice = (IUserService) _serviceProvider.GetService(typeof(IUserService));
            await userservice.EnsureCreated(e.Guild.Id, e.Member.Id);
        }

        private async Task OnGuildMemberRemoved(GuildMemberRemoveEventArgs e)
        {
            if (e.Member.IsBot) return;
            
            var userservice = (IUserService) _serviceProvider.GetService(typeof(IUserService));
            await userservice.DeleteUser(e.Guild.Id, e.Member.Id);
        }

        private async Task OnSocketErrored(SocketErrorEventArgs e)
        {
            Log.Error("Socket Error: " + e);
            Environment.Exit(1);
        }

        private async Task OnSocketClosed(SocketCloseEventArgs e)
        {
            Log.Error("Socket Closed: " + e.CloseMessage);
            Environment.Exit(1);
        }
        
        private Task OnCommandError(CommandErrorEventArgs e)
        {
            if (e?.Command != null)
            {
                Log.Error(e.Exception, "Command {command} failed executing for user {userMention}", e.Command.Name,
                    e.Context.User.Mention);
                e.Context.Message.AddError();
            }

            return Task.CompletedTask;
        }
        
        

        private async Task SendLogToDiscord(DebugLogMessageEventArgs logMessage)
        {
            if (_botConfiguration.LogChannelId == null || !_clientIsReady)
            {
                return;
            }

            var guild = _discordClient.Guilds.First();

            var channels = await guild.Value.GetChannelsAsync();

            var channel = channels.Single(x => x.Id == _botConfiguration.LogChannelId);

            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle($"{logMessage.Level} Log")
                .WithColor(DiscordColor.Red)
                .WithFooter(logMessage.Timestamp.ToString("dd/MM/yyyy HH:mm:ss"));

            if (!string.IsNullOrWhiteSpace(logMessage.Message))
            {
                embedBuilder.AddField("Message", $"```{logMessage.Message}```");
            }

            if (logMessage.Exception != null && !string.IsNullOrWhiteSpace(logMessage.Exception.ToString()))
            {
                embedBuilder.AddField("Stack", $"```{logMessage.Exception}```");
            }

            await channel.SendMessageAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
        }
    }
}