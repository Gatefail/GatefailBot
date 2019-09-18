using Newtonsoft.Json;

namespace GatefailBot.Database.Models
{
    public class GatefailUser : BaseEntity
    {
        public ulong DiscordId { get; set; }
        [JsonIgnore]
        public Guild Guild { get; set; }
    }
}