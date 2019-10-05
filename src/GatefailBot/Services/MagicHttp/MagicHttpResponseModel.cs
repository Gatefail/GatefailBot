using System;
using System.Net;

namespace GatefailBot.Services.MagicHttp
{
    public class MagicHttpResponseModel
    {
        public Uri OriginalRequestUrl { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseContent { get; set; }
    }
}