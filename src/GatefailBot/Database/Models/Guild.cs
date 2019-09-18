using System.Collections.Generic;

namespace GatefailBot.Database.Models
{
    public class Guild : BaseEntity
    {
        public ulong DiscordId { get; set; }
        public List<GatefailUser> Users { get; set; } = new List<GatefailUser>();
    }
}