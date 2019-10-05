using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GatefailBot.Services
{
    public interface IHastebinService
    {
        Task<HasteBinResult> Post(string content);
    }
    
    public class HastebinService : IHastebinService
    {
        private static readonly string TestHost = "hastebin.topping.dev";
        private readonly HttpClient _httpClient;

        public HastebinService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HasteBinResult> Post(string content)
        {
            var uri = new UriBuilder()
            {
                Scheme = "https://",
                Host = TestHost,
                Path = "documents",
            }.Uri;
            
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = uri,
                Content = new StringContent(content)
            };
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var hastebinResult = JsonConvert.DeserializeObject<HasteBinResult>(json);
                if (hastebinResult?.Key != null)
                {
                    hastebinResult.FullUrl = $"https://hastebin.topping.dev/{hastebinResult.Key}";
                    hastebinResult.IsSuccess = true;
                    hastebinResult.StatusCode = (int)response.StatusCode;
                    return hastebinResult;
                }
            }
            
            return new HasteBinResult()
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode
            };
        }
        
    }
    
    public class HasteBinResult
    {
        public string Key { get; set; }
        public string FullUrl { get; set; }
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
    }
}