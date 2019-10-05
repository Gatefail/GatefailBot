using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using GatefailBot.Helpers;
using GatefailBot.Services;

namespace GatefailBot.Modules
{
    [IgnoreBots]
    [Group("modconfig")]
    [Aliases("mc")]
    [RequireUserPermissions(Permissions.Administrator)]
    public class ModuleControlModule : BaseCommandModule
    {
        private readonly ICachedModuleService _cachedModuleService;

        public ModuleControlModule(ICachedModuleService cachedModuleService)
        {
            _cachedModuleService = cachedModuleService;
        }

        [Command("list")]
        public async Task ListModules(CommandContext ctx)
        {
            var guildId = ctx.Guild.Id;
            
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