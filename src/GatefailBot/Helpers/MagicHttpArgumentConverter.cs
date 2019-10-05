using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using GatefailBot.Services.MagicHttp;
using Newtonsoft.Json;

namespace GatefailBot.Helpers
{
    public class MagicHttpArgumentConverter : IArgumentConverter<MagicHttpRequestModel>
    {
        private static readonly char CodeTagChar = '`';
        
        public async Task<Optional<MagicHttpRequestModel>> ConvertAsync(string value, CommandContext ctx)
        {
            var sanitized = SanitizeInput(value);
            var model = JsonConvert.DeserializeObject<MagicHttpRequestModel>(sanitized);
            return Optional.FromValue(model);
        }

        private string SanitizeInput(string input)
        {
            return Regex.Replace(input, @"`|\t|\n|\r", "");
        }
    }
}