using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;


namespace CsvVisualizer.Helpers
{
    public abstract class BaseBlobStorageConnection
    {
        internal readonly ILogger _logger;
        internal readonly string _connectionString;

        public BaseBlobStorageConnection(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<AzureBlobStorageConnection>();
            _connectionString = configuration.GetValue<string>("BlobStorageConnectionString")!;
        }


    }
}
