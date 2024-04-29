using Google.Protobuf.WellKnownTypes;
using JsonTransformer.Extensions;
using JsonTransformer.Helpers;
using JsonTransformer.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;


namespace JsonTransformer
{
    public class JsonConvertor
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IBlobStorageConnection _blobStorageConnection;
        private readonly JsonFetcher _jsonFetcher;
        private readonly JsonTableConvertor _jsonTableConvertor;

        public JsonConvertor(ILoggerFactory loggerFactory, IConfiguration configuration, IBlobStorageConnection blobStorageConnection, JsonFetcher jsonFetcher, JsonTableConvertor jsonTableConvertor)
        {
            _logger = loggerFactory.CreateLogger<JsonConvertor>();
            _configuration = configuration;
            _blobStorageConnection = blobStorageConnection;
            _jsonFetcher = jsonFetcher;
            _jsonTableConvertor = jsonTableConvertor;
        }

        [Function("JsonConvertor")]
        public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo timer)
        {
            _logger.LogInformation($"Starting JsonConvertor at: {DateTime.Now}");

            // Download json from api
            string apiUrl = _configuration.GetValue<string>("ApiUrl");
            var jsonData = await _jsonFetcher.GetJson(apiUrl);

            if (jsonData == null)
            {
                // Just return, JsonFetcher already logs the errors
                return;
            }

            // Convert json to list of flat Expandoobjects
            var records = _jsonTableConvertor.ReadAsDynamicList(jsonData);

            if (records == null)
            {
                _logger.LogWarning($"Unable to convert json to table!");
                return;
            }

            // Convert datatable to csv
            char csvSeperator = _configuration.GetValue<char>("CsvSeperator");

            var csvString = records.ToCSVString(csvSeperator, null, true);
            var csvStream = new MemoryStream(Encoding.UTF8.GetBytes(csvString ?? ""));

            // Upload Csv to blob storage
            var uploadResult = await _blobStorageConnection.SaveFileAsync("newData.csv", csvStream);

            if (!uploadResult)
            {
                _logger.LogWarning($"Failed to upload CSV!");
            }
            else
            {
                _logger.LogInformation($"Successfully uploaded CSV");
            }

            if (timer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {timer.ScheduleStatus.Next}");
            }
        }
    }
}
