using CsvVisualizer.Helpers;
using CsvVisualizer.Interfaces;
using CsvVisualizer.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestJsonProvider.Models;
using static CsvVisualizer.Workers.DataFetcher;

namespace CsvVisualizer
{
    public partial class Main : Form
    {
        private IConfiguration _configuration;
        private ILoggerFactory _loggerFactory;
        private IBlobStorageConnection _blobStorageConnection;
        private CsvToObjectConvertor _csvToObjectConvertor;

        private char _csvSeperator;

        public Main(IConfiguration configuration, ILoggerFactory loggerFactory, IBlobStorageConnection blobStorageConnection, CsvToObjectConvertor csvToObjectConvertor)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
            _blobStorageConnection = blobStorageConnection;
            _csvToObjectConvertor = csvToObjectConvertor;

            _csvSeperator = _configuration.GetValue<char>("CsvSeperator");

            InitializeComponent();

            InitializeGrid();

            InitializeWorker();

        }

        private void InitializeWorker()
        {
            var fetcher = new DataFetcher(_configuration, _loggerFactory, _blobStorageConnection);
            fetcher.OnNewData += OnNewData;
            _ = fetcher.StartAsync(new CancellationToken());
        }

        private void InitializeGrid()
        {
            gridview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        void OnNewData(object? sender, NewDataEventArgs e)
        {
            if (e.file == null)
            {
                return;
            }

            // Convert CSV back to real Models
            var records = _csvToObjectConvertor.Convert<BankAccount>(e.file, _csvSeperator);
            var displayView = records.Select(r => new
            {
                Name = r.Name,
                Iban = r.Iban,
                Debt = r.Debt,
                Balance = r.Balance,
                Locked = r.IsLocked,
                TakeOutaLoan = r.ToTakeOutaLoan,
                CardNr = r.CreditCard.Number,
                CardExpiry = new DateOnly(r.CreditCard.CreditCardExpireYear, r.CreditCard.CreditCardExpireMonth, 1),
            }).ToList();

            // Rebind so grid updates, this needs to be done using invoke since event it fired from other thread
            gridview.Invoke(new Action(() =>
            {
                gridview.DataSource = null;
                gridview.DataSource = displayView;
            }));

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
