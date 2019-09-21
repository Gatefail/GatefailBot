using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GatefailBot.Services;

namespace GatefailBot.Helpers
{
    public class CheckModuleEnabled : CheckBaseAttribute
    {
        private string _moduleName;

        public CheckModuleEnabled(string moduleName)
        {
            _moduleName = moduleName;
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            var service =
                (ICachedModuleService)ctx.Services.GetService(typeof(ICachedModuleService));
            return await service.IsModuleEnabled(ctx.Guild.Id, _moduleName);
        }
    }
}