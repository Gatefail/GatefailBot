using System;
using System.Linq;
using System.Threading.Tasks;
using GatefailBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GatefailBot.Services
{
    public interface ICachedModuleService
    {
        Task<bool> IsModuleEnabled(ulong guildId, string moduleName);
    }

    public class CachedModuleService : ICachedModuleService
    {
        private const string CacheKeyPrefix = "MODULECONFIG";
        private readonly TimeSpan _expirationTime = TimeSpan.FromSeconds(30);
        private readonly GatefailContext _db;
        private readonly IMemoryCache _memoryCache;
        
        public CachedModuleService(IMemoryCache memoryCache, GatefailContext db)
        {
            _memoryCache = memoryCache;
            _db = db;
        }

        public async Task<bool> IsModuleEnabled(ulong guildId, string moduleName)
        {
            var cacheKey = CacheKeyConvention(guildId, moduleName);
            return await _memoryCache.GetOrCreateAsync(cacheKey, cacheEntry => GetModuleEnablement(cacheEntry, guildId, moduleName));
        }

        private async Task<bool> GetModuleEnablement(ICacheEntry cacheEntry, ulong guildId, string moduleName)
        {
            var guild = await _db.Guilds
                .Include(g => g.ModuleConfigurations)
                .FirstOrDefaultAsync(g => g.DiscordId == guildId);
            if (guild == null) { return false; }
            var moduleExists = guild.ModuleConfigurations.Any(g => g.ModuleName.Equals(moduleName));
            cacheEntry.Value = moduleExists;
            cacheEntry.AbsoluteExpirationRelativeToNow = _expirationTime;
            return moduleExists;
        }

        private string CacheKeyConvention(ulong guildId, string moduleName)
        {
            return $"{CacheKeyPrefix}!{guildId}!{moduleName}";
        }
    }
}