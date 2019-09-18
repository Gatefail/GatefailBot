using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GatefailBot.Services;

namespace GatefailBot.Helpers
{
    public class CheckRestrictionAttribute : CheckBaseAttribute
    {
        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            var service =
                (ICommandChannelRestrictionService)ctx.Services.GetService(typeof(ICommandChannelRestrictionService));

            var restrictionMeta = await service.GetRestrictionForCommand(ctx.Command.Name).ConfigureAwait(false);

            if (restrictionMeta.HasRestriction && !restrictionMeta.RestrictedToChannelIds.Contains(ctx.Channel.Id))
            {
                return false;
            }

            return true;
        }
    }
}