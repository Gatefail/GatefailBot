using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GatefailBot.Helpers;
using Serilog;

namespace GatefailBot.Modules
{
    [IgnoreBots]
    public class DebugModule : BaseCommandModule
    {
        [Command("ping")]
        [Description("Pings the bot and sends a response, if the bot is alive!")]
        public async Task Ping(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithDescription("🏓 Pong!")
                .AddField("WebSocket", $"{ctx.Client.Ping}ms", true);

            var sw = Stopwatch.StartNew();
            var message = await ctx.RespondAsync(embed: embed);
            sw.Stop();

            embed.AddField("REST API", $"{sw.ElapsedMilliseconds}ms", true);
            await message.ModifyAsync(embed: embed.Build());
        }

        [Command("throw")]
        [Description("Intentionally logs at error level")]
        public Task Throw(CommandContext ctx)
        {
            return Throw($"An error was intentionally thrown at {DateTime.Now} by {ctx.User.Username}");
        }

        [Command("throw")]
        [Description("Intentionally logs at error level")]
        public Task Throw([RemainingText] string message)
        {
            Log.Error(message);

            return Task.CompletedTask;
        }

        [Command("dumproles")]
        [Description("Dumps roles")]
        [RequireUserPermissions(Permissions.Administrator)]
        public Task DumpRoles(CommandContext ctx)
        {
            var roles = ctx.Guild.Roles.Select(x => $"{x.Key} `{x.Value.Name}`");

            var output = string.Join(", ", roles);

            return ctx.RespondAsync(output);
        }
    }
}