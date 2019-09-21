using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatefailBot.Database;
using GatefailBot.Database.Models;
using GatefailBot.Helpers;
using GatefailBot.Services.ResponseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GatefailBot.Services
{
    public interface ICachedModuleService
    {
        Task<bool> IsModuleEnabled(ulong guildId, string moduleName);
        Task<ToggleModuleResponse> SetModuleState(ulong guildId, string moduleName, bool activated);
    }

    public class CachedModuleService : ICachedModuleService
    {
        private const string CacheKeyPrefix = "MODULECONFIG";
        private readonly TimeSpan _expirationTime = TimeSpan.FromSeconds(30);
        private readonly GatefailContext _db;
        private readonly IMemoryCache _memoryCache;
        private readonly ModuleContainer _moduleContainer;
        
        public CachedModuleService(IMemoryCache memoryCache, GatefailContext db, ModuleContainer moduleContainer)
        {
            _memoryCache = memoryCache;
            _db = db;
            _moduleContainer = moduleContainer;
        }

        public IEnumerable<string> GetAllModuleNames()
        {
            return _moduleContainer.GetLoadedModules().Keys;
        }

        public async Task<ToggleModuleResponse> SetModuleState(ulong guildId, string moduleName, bool activated)
        {
            if (!_moduleContainer.ModuleExists(moduleName))
            {
                return ToggleModuleResponse.ToggleModuleFailed($"No module exists with name {moduleName}");
            }

            var guild = await _db.Guilds
                .Include(g => g.ModuleConfigurations)
                .FirstOrDefaultAsync(g => g.DiscordId == guildId);

            if (guild == null)
            {
                return ToggleModuleResponse.ToggleModuleFailed($"Failed to find guild in database with Discord Id: {guildId}");
            }

            var moduleConfig = guild.ModuleConfigurations.FirstOrDefault(m => m.ModuleName.Equals(moduleName));
            if (moduleConfig == null)
            {
                moduleConfig = new ModuleConfiguration()
                {
                    Activated = activated,
                    ModuleName = moduleName
                };
                guild.ModuleConfigurations.Add(moduleConfig);
            }
            else
            {
                moduleConfig.Activated = activated;
            }

            _db.ModuleConfigurations.Update(moduleConfig);
            await _db.SaveChangesAsync();
            
            _memoryCache.Remove(CacheKeyConvention(guildId, moduleName));
            return ToggleModuleResponse.ToggleModuleSuccess($"Successfully enabled module: {moduleName}");
        }

        public async Task<bool> IsModuleEnabled(ulong guildId, string moduleName)
        {
            var cacheKey = CacheKeyConvention(guildId, moduleName);
            var res = await _memoryCache.GetOrCreateAsync(cacheKey, cacheEntry => GetModuleEnablement(cacheEntry, guildId, moduleName));
            return res;
        }

        private async Task<bool> GetModuleEnablement(ICacheEntry cacheEntry, ulong guildId, string moduleName)
        {
            var guild = await _db.Guilds
                .Include(g => g.ModuleConfigurations)
                .FirstOrDefaultAsync(g => g.DiscordId == guildId);
            if (guild == null) { return false; }
            var moduleConfig = guild.ModuleConfigurations.FirstOrDefault(g => g.ModuleName.Equals(moduleName));
            var activated = moduleConfig == null ? false : moduleConfig.Activated;
            cacheEntry.Value = activated;
            cacheEntry.AbsoluteExpirationRelativeToNow = _expirationTime;
            return activated;
        }

        private string CacheKeyConvention(ulong guildId, string moduleName)
        {
            return $"{CacheKeyPrefix}!{guildId}!{moduleName}";
        }
    }
}