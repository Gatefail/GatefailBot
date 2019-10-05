using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using GatefailBot.Services.MagicHttp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GatefailBot.Services
{
    public interface IMagicHttpService
    {
        Task<MagicHttpResponseModel> MakeCall(ulong discordUserId, MagicHttpRequestModel requestModel);
    }
    
    public class MagicHttpService : IMagicHttpService
    {
        private static readonly TimeSpan MinimumRequestInterval = TimeSpan.FromSeconds(10);
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public MagicHttpService(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        private Tuple<bool, string> CanUrlBeCalled(ulong discordUserId, Uri uri)
        {
            if (IsInvalidUrl(uri))
            {
                return new Tuple<bool, string>(false, "Invalid URL or Scheme");
            }
            
            if (_memoryCache.TryGetValue(discordUserId, out _))
            {
                return new Tuple<bool, string>(false, "Too many requests by the same user.");
            }
            
            if (_memoryCache.TryGetValue(uri.Host, out _))
            {
                return new Tuple<bool, string>(false, "Too many requests against the same host");
            }

            _memoryCache.Set(uri.Host, uri.Host, DateTimeOffset.Now.Add(MinimumRequestInterval));
            return new Tuple<bool, string>(true, "");
        }

        public async Task<MagicHttpResponseModel> MakeCall(ulong discordUserId, MagicHttpRequestModel requestModel)
        {   
            var uri = new Uri(requestModel.BaseUrl);
            var validation = CanUrlBeCalled(discordUserId, uri);
            if (!validation.Item1)
            {
                return new MagicHttpResponseModel()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ResponseContent = validation.Item2,
                    OriginalRequestUrl = uri
                }; // Url is real weird and bad like
            }
            _memoryCache.Set(discordUserId, discordUserId, DateTimeOffset.Now.Add(MinimumRequestInterval));

            var body = JsonConvert.SerializeObject(requestModel.Body);
            var requestMessage = new HttpRequestMessage()
            {
                Method = requestModel.HttpMethod,
                RequestUri = uri,
                Content = new StringContent(body, Encoding.UTF8, requestModel.MediaType)
            };
            
            if (requestModel.Headers?.Count > 0)
            {
                foreach (var kvp in requestModel.Headers)
                {
                    requestMessage.Headers.Add(kvp.Key, kvp.Value);
                }    
            }
            
            
            var response = await _httpClient.SendAsync(requestMessage);
            var responseBody = await response.Content.ReadAsStringAsync();
            var content = FormatResponse(responseBody);
            
            return new MagicHttpResponseModel()
            {
                StatusCode = response.StatusCode,
                ResponseContent = content,
                OriginalRequestUrl = uri
            };
        }

        private string FormatResponse(string content)
        {
            try
            {
                return JToken.Parse(content).ToString();
            }
            catch (Exception e)
            {
                return content;
            }
        }

        private bool IsInvalidUrl(Uri uri)
        {
            var containsQueryParams = !String.IsNullOrEmpty(uri.Query);
            var isNotHttp = !uri.Scheme.Equals("http") && !uri.Scheme.Equals("https");
            return containsQueryParams || isNotHttp;
        }
    }
}