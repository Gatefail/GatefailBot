using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace GatefailBot.Helpers
{
    public static class DiscordMessageExtensions
    {
        public static Task AddConfirmation(this DiscordMessage message)
        {
            return message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));
        }

        public static Task AddError(this DiscordMessage message)
        {
            return message.CreateReactionAsync(DiscordEmoji.FromUnicode("🛑"));
        }

        public static Task CleanupAndRespond(this DiscordMessage message, string response = "",
            DiscordEmbed embedResponse = null)
        {
            var respondTask = message.RespondAsync(response, false, embedResponse);
            var deleteTask = message.DeleteAsync("Cleanup");
            return Task.WhenAll(respondTask, deleteTask);
        }
    }
}