using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GatefailBot.Helpers;
using GatefailBot.Services;

namespace GatefailBot.Modules
{
    [RequireUserPermissions(Permissions.Administrator)]
    [IgnoreBots]
    public class CommandChannelRestrictionModule : BaseCommandModule
    {
        private readonly ICommandChannelRestrictionService _commandChannelRestrictionService;

        public CommandChannelRestrictionModule(ICommandChannelRestrictionService commandChannelRestrictionService)
        {
            _commandChannelRestrictionService = commandChannelRestrictionService;
        }

        [Command("restrict")]
        public async Task Add(CommandContext ctx, string command, DiscordChannel channel)
        {
            var searchResult = ctx.CommandsNext.FindCommand(command, out _);

            if (searchResult == null)
            {
                await ctx.Message.AddError();
                return;
            }

            var commandNames = searchResult.Aliases.ToList();

            commandNames.Add(searchResult.Name);

            await _commandChannelRestrictionService.Add(commandNames, channel.Id);

            await ctx.Message.AddConfirmation().ConfigureAwait(false);
        }

        [Command("unrestrict")]
        public async Task Unrestrict(CommandContext ctx, string command, DiscordChannel channel)
        {
            var searchResult = ctx.CommandsNext.FindCommand(command, out _);

            if (searchResult == null)
            {
                await ctx.Message.AddError();
                return;
            }

            var commandNames = searchResult.Aliases.ToList();

            commandNames.Add(searchResult.Name);

            await _commandChannelRestrictionService.Delete(commandNames, channel.Id);

            await ctx.Message.AddConfirmation().ConfigureAwait(false);
        }
    }
}