using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace JsonTransformer.Helpers
{
    public class JsonFetcher
    {
        private readonly ILogger _logger;
        private readonly int _timeout;

        public JsonFetcher(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<JsonFetcher>();
            _timeout = configuration.GetValue<int>("ApiTimeout");
        }

        public async Task<string?> GetJson(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(_timeout);

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "Application/json");

                try
                {
                    var result = await client.SendAsync(request);
                    var jsonData = await result.Content.ReadAsStringAsync();

                    return jsonData;

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error fetching data from {url}:{ex.Message}");
                    return null;
                }

            }

        }

    }
}
