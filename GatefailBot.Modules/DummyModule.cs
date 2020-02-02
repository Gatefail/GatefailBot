using System.Threading.Tasks;
using Discord.Commands;
using GatefailBot.Infrastructure;

namespace GatefailBot.Modules
{
    public class DummyModule : ModuleBase<SocketCommandContext>
    {
        private readonly IFarawayDataFetcher _fetcher;

        public DummyModule(IFarawayDataFetcher fetcher)
        {
            _fetcher = fetcher;
        }

//        [Command("say")]
//        public Task WriteShit() => ReplyAsync($"Shit: {_fetcher.FetchData()}");

        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder] string echo)
        {
            return ReplyAsync($"{_fetcher.FetchData()}");
        }
    }
}