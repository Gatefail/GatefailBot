using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatefailBot.Database;
using GatefailBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GatefailBot.Services
{
    public interface ICommandChannelRestrictionService
    {
        Task Add(IEnumerable<string> commands, ulong discordChannelId);
        Task Delete(IEnumerable<string> commands, ulong discordChannelId);
        Task<RestrictionDto> GetRestrictionForCommand(string command);
    }

    public class CommandChannelRestrictionService : ICommandChannelRestrictionService
    {
        private readonly GatefailContext _db;

        public CommandChannelRestrictionService(GatefailContext db)
        {
            _db = db;
        }

        public async Task<RestrictionDto> GetRestrictionForCommand(string command)
        {
            var normalized = command.ToUpper();

            var commandRestriction = new RestrictionDto();

            var restrictedChannelIdsForCommand = await _db.CommandChannelRestrictions
                .Where(x => x.NormalizedCommand == normalized)
                .Select(x => x.DiscordChannelId)
                .ToListAsync().ConfigureAwait(false);

            if (!restrictedChannelIdsForCommand.Any())
            {
                return commandRestriction;
            }

            commandRestriction.HasRestriction = true;

            commandRestriction.RestrictedToChannelIds.AddRange(restrictedChannelIdsForCommand);

            return commandRestriction;
        }

        public async Task Add(IEnumerable<string> commands, ulong discordChannelId)
        {
            var newRestrictions = commands.Select(x => new CommandChannelRestriction(x, discordChannelId));

            _db.CommandChannelRestrictions.AddRange(newRestrictions);

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task Delete(IEnumerable<string> commands, ulong discordChannelId)
        {
            var normalizedCommands = commands.Select(x => x.ToUpper());

            var restrictions = await _db.CommandChannelRestrictions
                .Where(x => x.DiscordChannelId == discordChannelId)
                .Where(x => normalizedCommands.Contains(x.NormalizedCommand))
                .ToListAsync().ConfigureAwait(false);

            foreach (var restriction in restrictions)
            {
                _db.CommandChannelRestrictions.Remove(restriction);
            }

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public class RestrictionDto
    {
        public bool HasRestriction { get; set; }
        public List<ulong> RestrictedToChannelIds { get; set; } = new List<ulong>();
    }
}