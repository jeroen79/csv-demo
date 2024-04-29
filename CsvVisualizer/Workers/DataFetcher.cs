using CsvVisualizer.Helpers;
using CsvVisualizer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading;
using Timer = System.Threading.Timer;

namespace CsvVisualizer.Workers
{
    public class DataFetcher : BackgroundService
    {
        private IConfiguration _configuration;
        private ILogger _logger;
        private IBlobStorageConnection _blobStorageConnection;

        private Timer? _timer = null;
        private int _interval = 10;
        private string _filePath;

        private DateTimeOffset? _currentVersion = null;

        // Events
        public event EventHandler<NewDataEventArgs> OnNewData = delegate { };

        public DataFetcher(IConfiguration configuration, ILoggerFactory loggerFactory, IBlobStorageConnection blobStorageConnection)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<AzureBlobStorageConnection>();
            _blobStorageConnection = blobStorageConnection;
            _filePath = _configuration.GetValue<string>("FilePath")!;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _ = Task.Run(() =>
            {
                //launch async thread to prepare stuff
                this.Prepare().ConfigureAwait(false);
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("DataFetcher is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

        }

        private Task<bool> Prepare()
        {

            _logger.LogInformation("DataFetcher is running.");

            //start the main timer
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_interval));

            return Task.FromResult(true);
        }

        public async void DoWork(object? state)
        {
            _logger.LogInformation("DataFetcher DoWork.");

            var modificationDate = await _blobStorageConnection.GetFileModificationAsync(_filePath);

            // Check if file has actualy been changeds
            if (_currentVersion != null && modificationDate != null)
            {
                // File hasn't changed
                if (_currentVersion == modificationDate)
                {
                    return;
                }
            }

            var file = await _blobStorageConnection.DownloadFileAsync(_filePath);
            if (file == null)
            {
                return;
            }

            _currentVersion = modificationDate;

            // Convert stream to string
            string csvData = Encoding.UTF8.GetString(file.ToArray());

            var args = new NewDataEventArgs()
            {
                file = csvData
            };
            OnNewData.Invoke(this, args);
        }


        public class NewDataEventArgs : EventArgs
        {
            public string? file { get; set; }
        }
    }
}
