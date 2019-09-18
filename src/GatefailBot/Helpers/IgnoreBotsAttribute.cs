using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace GatefailBot.Helpers
{
    public class IgnoreBotsAttribute : CheckBaseAttribute
    {
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(!ctx.User.IsBot);
        }
    }
}