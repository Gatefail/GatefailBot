using System;
using System.Collections.Generic;
using System.Net.Http;

namespace GatefailBot.Services.MagicHttp
{
    public class MagicHttpRequestModel
    {
        public string BaseUrl { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; }
        public string MediaType { get; set; }
        public Object Body { get; set; }
    }
}