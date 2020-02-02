using System.Threading.Tasks;
using Discord.Commands;

namespace GatefailBot.Modules
{
    public class OtherDummyModule : ModuleBase<SocketCommandContext>
    {
        [Command("poop")]
        public Task DoPoop() => ReplyAsync("Poop");
    }
}