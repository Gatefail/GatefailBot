using System;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GatefailBot.Helpers;
using GatefailBot.Services;
using GatefailBot.Services.MagicHttp;
using Newtonsoft.Json;

namespace GatefailBot.Modules
{
    [IgnoreBots]
    [Group("magichttp")]
    [Aliases("mh")]
    public class MagicHttpModule : BaseCommandModule
    {
        private readonly IMagicHttpService _magicHttpService;
        private readonly IHastebinService _hastebinService;

        public MagicHttpModule(IMagicHttpService magicHttpService, IHastebinService hastebinService)
        {
            _magicHttpService = magicHttpService;
            _hastebinService = hastebinService;
        }

        [Command("request")]
        [Description("Sends the magic request you want to send")]
        public async Task MakeRequest(CommandContext ctx, [RemainingText] string requestJson)
        {
            var embedBuilder = new DiscordEmbedBuilder()
            {
                Title = "Magic Response"
            };
            
            var converter = new MagicHttpArgumentConverter();
            var requestModel = (await converter.ConvertAsync(requestJson, ctx)).Value;
            var response = await _magicHttpService.MakeCall(ctx.User.Id, requestModel);
            embedBuilder.AddField("Original Request Url", response.OriginalRequestUrl.ToString());
            embedBuilder.AddField("Response Code", ((int)response.StatusCode).ToString());
            embedBuilder.Color = ConvertCodeToColor(response.StatusCode);
            if (response.ResponseContent.Length > 1023)
            {
                var hasteResult = await _hastebinService.Post(response.ResponseContent);
                embedBuilder.AddField("Response Content",
                    $"Your response was too long. It has been uploaded here: {hasteResult.FullUrl}");
            }
            else
            {
                embedBuilder.AddField("Response Content", FormatResponseToCodeBlock(response.ResponseContent));    
            }
            await ctx.Message.RespondAsync(embed: embedBuilder.Build());
        }

        private string FormatResponseToCodeBlock(string input)
        {
            return $"```{Environment.NewLine}{input}{Environment.NewLine}```";
        }

        private DiscordColor ConvertCodeToColor(HttpStatusCode statusCode)
        {
            int statusCodeInt = (int) statusCode;
            if (statusCodeInt < 200)
            {
                return DiscordColor.Blue;
            } else if (statusCodeInt >= 200 && statusCodeInt < 300)
            {
                return DiscordColor.Green;
            } else if (statusCodeInt >= 300 && statusCodeInt < 400)
            {
                return DiscordColor.Aquamarine;
            } else
            {
                return DiscordColor.Red;
            }
        }
    }
}