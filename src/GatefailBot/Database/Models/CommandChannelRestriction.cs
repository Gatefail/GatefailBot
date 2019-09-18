using System.ComponentModel.DataAnnotations;

namespace GatefailBot.Database.Models
{
    public class CommandChannelRestriction
    {
        public CommandChannelRestriction(string command, ulong discordChannelId)
        {
            Command = command;
            NormalizedCommand = command.ToUpper();
            DiscordChannelId = discordChannelId;
        }

        [Key] public int Id { get; set; }

        public ulong DiscordChannelId { get; set; }

        [Required] public string Command { get; set; }

        [Required] public string NormalizedCommand { get; set; }
    }
}