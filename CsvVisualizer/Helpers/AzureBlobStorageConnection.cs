using Azure.Storage.Blobs;
using CsvVisualizer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;
using System.Threading;


namespace CsvVisualizer.Helpers
{
    public class AzureBlobStorageConnection : BaseBlobStorageConnection, IBlobStorageConnection
    {

        private readonly string _container;

        public AzureBlobStorageConnection(ILoggerFactory loggerFactory, IConfiguration configuration) : base(loggerFactory, configuration)
        {
            _container = configuration.GetValue<string>("BlobStorageContainer")!;
        }


        public async Task<bool> SaveFileAsync(string fullPath, Stream file)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _container);
                containerClient.CreateIfNotExists();

                BlobClient blobClient = containerClient.GetBlobClient(fullPath.ToString());
                await blobClient.UploadAsync(file, true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving file to BlobStorage: {ex.Message}");
                return false;
            }

        }

        public async Task<MemoryStream?> DownloadFileAsync(string fullPath)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _container);

                BlobClient blobClient = containerClient.GetBlobClient(fullPath.ToString());

                if (!blobClient.ExistsAsync().Result)
                {
                    _logger.LogError($"Error file not found: {fullPath}");
                    return null;
                }

                var ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);
                return ms;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading file from BlobStorage: {ex.Message}");
                return null;
            }
        }

        public async Task<DateTimeOffset?> GetFileModificationAsync(string fullPath)
        {
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(_connectionString, _container);

                BlobClient blobClient = containerClient.GetBlobClient(fullPath.ToString());

                if (!blobClient.ExistsAsync().Result)
                {
                    _logger.LogError($"Error file not found: {fullPath}");
                    return null;
                }

                var blobProperties = await blobClient.GetPropertiesAsync();

                return blobProperties.Value.LastModified;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading file from BlobStorage: {ex.Message}");
                return null;
            }
        }

    }
}
