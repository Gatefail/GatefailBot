using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatefailBot.Database;
using GatefailBot.Database.BatchQueries;
using GatefailBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GatefailBot.Services
{
    public interface IUserService
    {
        Task EnsureCreated(ulong guildSnowflake, ulong userSnowflake);
        Task<GatefailUser> GetUser(ulong guildSnowflake, ulong userSnowflake);
        Task DeleteUser(ulong guildSnowflake, ulong userSnowflake);
        Task EnsureCreatedMany(ulong guildSnowflake, IEnumerable<ulong> userSnowflakes);
    }

    public class UserService : IUserService
    {
        private readonly WowTrackerContext _db;

        public UserService(WowTrackerContext db)
        {
            _db = db;
        }

        public async Task EnsureCreated(ulong guildSnowflake, ulong userSnowflake)
        {
            if (_db.Users.Any(u => u.Guild.DiscordId == guildSnowflake && u.DiscordId == userSnowflake)) return;

            var guild = await _db.Guilds.FirstOrDefaultAsync(g => g.DiscordId == guildSnowflake);
            if(guild == null) throw new ArgumentException($"Cannot add user with id {userSnowflake} to guild {guildSnowflake} because it doesn't exist.");
            
            var newUser = new GatefailUser() { DiscordId = userSnowflake };
            guild.Users.Add(newUser);
            _db.Guilds.Update(guild);
            await _db.SaveChangesAsync();
        }

        public async Task EnsureCreatedMany(ulong guildSnowflake, IEnumerable<ulong> userSnowflakes)
        {
            var uniqueId = Guid.NewGuid();
            try
            {
                foreach (var userSnowflake in userSnowflakes)
                {
                    _db.BatchQueries.Add(new BatchQueryItem() {UniqueId = uniqueId, IdToQuery = userSnowflake});
                }

                await _db.SaveChangesAsync();
                var existingUserIds = _db.Users
                    .Where(u => u.Guild.DiscordId == guildSnowflake)
                    .Select(u => u.DiscordId)
                    .Join(_db.BatchQueries, userId => userId, batchItem => batchItem.IdToQuery,
                        (userId, batchItem) => userId)
                    .ToHashSet();
                
                var newUsers = userSnowflakes
                    .Where(u => !existingUserIds.Contains(u))
                    .Select(u => new GatefailUser() {DiscordId = u});
                _db.Users.AddRange(newUsers);
                await _db.SaveChangesAsync();
            }
            finally
            {
                await _db.Database.ExecuteSqlCommandAsync($"DELETE FROM \"BatchQueries\" WHERE \"UniqueId\" = {{0}}", uniqueId);
            }
        }

        public async Task<GatefailUser> GetUser(ulong guildSnowflake, ulong userSnowflake)
        {
            await EnsureCreated(guildSnowflake, userSnowflake);
            
            return await _db.Users
                .AsNoTracking()
                .Where(u => u.Guild.DiscordId == guildSnowflake)
                .FirstOrDefaultAsync(u => u.DiscordId == userSnowflake);
        }

        public async Task DeleteUser(ulong guildSnowflake, ulong userSnowflake)
        {
            var user = await _db.Users
                .Where(u => u.Guild.DiscordId == guildSnowflake)
                .FirstOrDefaultAsync(u => u.DiscordId == userSnowflake);
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }
}