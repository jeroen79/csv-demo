using CsvVisualizer.Helpers;
using CsvVisualizer.Interfaces;
using CsvVisualizer.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CsvVisualizer
{
    internal static class Program
    {

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Create host with config and logger
            var host = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, builder) =>
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices(services =>
            {
                services.AddLogging(builder => builder.AddConsole());
                services.AddScoped<IBlobStorageConnection, AzureBlobStorageConnection>();
                services.AddScoped<CsvToObjectConvertor>();
                // services.AddHostedService<DataFetcher>();
                services.AddTransient<Main>();
            })
            .Build();

            // Run async for hosted services
            //host.RunAsync();

            Application.Run(host.Services.GetRequiredService<Main>());


        }
    }
}