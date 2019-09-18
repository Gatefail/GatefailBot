using System.Linq;
using System.Threading.Tasks;
using GatefailBot.Database;
using GatefailBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GatefailBot.Services
{
    public interface IGuildService
    {
        Task EnsureCreated(ulong guildId);
        Task DeleteGuild(ulong guildId);
    }

    public class GuildService : IGuildService
    {
        private readonly GatefailContext _db;

        public GuildService(GatefailContext db)
        {
            _db = db;
        }

        public async Task EnsureCreated(ulong guildId)
        {
            if (_db.Guilds.Any(g => g.DiscordId == guildId)) return;

            var guild = new Guild() {DiscordId = guildId};
            _db.Guilds.Add(guild);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteGuild(ulong guildId)
        {
            var guild = await _db.Guilds.FirstOrDefaultAsync(g => g.DiscordId == guildId);
            _db.Guilds.Remove(guild);
            await _db.SaveChangesAsync();
        }
    }
}