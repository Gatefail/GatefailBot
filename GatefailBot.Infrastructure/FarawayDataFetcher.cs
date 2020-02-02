using Microsoft.Extensions.Logging;

namespace GatefailBot.Infrastructure
{
    public interface IFarawayDataFetcher
    {
        string FetchData();
    }
    public class FarawayDataFetcher : IFarawayDataFetcher
    {
        private readonly ILogger<FarawayDataFetcher> _logger;

        public FarawayDataFetcher(ILogger<FarawayDataFetcher> logger)
        {
            _logger = logger;
        }

        public string FetchData()
        {
            _logger.LogInformation("FETCHING THE FAR FAR AWAY DATA");
            return "THE DATA";
        }
    }
}