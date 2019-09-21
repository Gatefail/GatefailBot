using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GatefailBot.Helpers;
using GatefailBot.Services;

namespace GatefailBot.Modules
{
    [IgnoreBots]
    [Group("test")]
    [Aliases("t")]
    [RequireUserPermissions(Permissions.Administrator)]
    public class TestModule : BaseCommandModule
    {
        private readonly ICachedModuleService _cachedModuleService;

        public TestModule(ICachedModuleService cachedModuleService)
        {
            _cachedModuleService = cachedModuleService;
        }

        [Command("enable")]
        public async Task EnableModule(CommandContext ctx, string moduleName)
        {
            var response = await _cachedModuleService.SetModuleState(ctx.Guild.Id, moduleName, true);
            if (response.Success)
            {
                await ctx.Message.AddConfirmation();
            }
            else
            {
                await ctx.Message.AddError();
            }
        }
        
        [Command("disable")]
        public async Task DisableModule(CommandContext ctx, string moduleName)
        {
            var response = await _cachedModuleService.SetModuleState(ctx.Guild.Id, moduleName, false);
            if (response.Success)
            {
                await ctx.Message.AddConfirmation();
            }
            else
            {
                await ctx.Message.AddError();
            }
        }
    }
}