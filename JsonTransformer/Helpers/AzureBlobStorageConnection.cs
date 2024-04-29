using Azure.Storage.Blobs;
using JsonTransformer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;


namespace JsonTransformer.Helpers
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

    }
}
