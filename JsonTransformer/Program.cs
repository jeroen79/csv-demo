using JsonTransformer.Helpers;
using JsonTransformer.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddTransient<IBlobStorageConnection, AzureBlobStorageConnection>();
        services.AddTransient<JsonFetcher>();
        services.AddTransient<JsonTableConvertor>();
    })
    .Build();

host.Run();
